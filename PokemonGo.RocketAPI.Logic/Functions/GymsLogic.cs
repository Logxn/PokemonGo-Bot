/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 28/01/2017
 * Time: 18:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using POGOProtos.Settings.Master;
using PokemonGo.RocketAPI.Logic.Shared;

using System.Collections.Generic;
using System.Linq;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Utils;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Logic;
using PokemonGo.RocketAPI.Logic.Functions;
using POGOProtos.Data;
using POGOProtos.Data.Battle;
using PokemonGo.RocketAPI.Rpc;

namespace PokemonGo.RocketAPI.Logic.Functions
{
    /// <summary>
    /// Description of GymsLogic.
    /// </summary>
    public static class GymsLogic
    {
        private static List<string> gymsVisited = new List<string>();
        private static int  GetGymLevel(long value)
        {
            if (value >= 50000)
                return 10;
            if (value >= 40000)
                return 9;
            if (value >= 30000)
                return 8;
            if (value >= 20000)
                return 7;
            if (value >= 16000)
                return 6;
            if (value >= 12000)
                return 5;
            if (value >= 8000)
                return 4;
            if (value >= 4000)
                return 3;
            if (value >= 2000)
                return 2;
            return 1;
        }
        private static string GetTeamName(TeamColor team)
        {
            switch (team) {
                case TeamColor.Red:
                    return "Valor";
                case TeamColor.Yellow:
                    return "Instinct";
                case TeamColor.Blue:
                    return "Mystic";
            }
            return "Neutral";
        }
        public static void Execute()
        {
            if (!GlobalVars.FarmGyms)
                return;
            //narrow map data to gyms within walking distance
            var gyms = GetNearbyGyms();
            var gymsWithinRangeStanding = gyms.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude) < 40);

            var withinRangeStandingList = gymsWithinRangeStanding as IList<FortData> ?? gymsWithinRangeStanding.ToList();
            var inRange = withinRangeStandingList.Count;
            if (withinRangeStandingList.Any()) {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - {inRange} gyms are within range of the user");

                foreach (var gym in withinRangeStandingList) {
                    var fortInfo = Logic.objClient.Fort.GetFort(gym.Id, gym.Latitude, gym.Longitude).Result;
                    CheckAndPutInNearbyGym(gym, Logic.objClient, fortInfo);
                    Setout.SetCheckTimeToRun();
                    RandomHelper.RandomSleep(100, 200);
                }
            }

        }
        
        private static FortData[] GetNearbyGyms(GetMapObjectsResponse mapObjectsResponse = null)
        {
            if (mapObjectsResponse == null)
                mapObjectsResponse = Logic.objClient.Map.GetMapObjects().Result.Item1;

            var pokeGyms = Logic.Instance.navigation
                .pathByNearestNeighbour(
                               mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Gym)
                    .OrderBy(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude))
                    .ToArray(), GlobalVars.WalkingSpeedInKilometerPerHour);

            return pokeGyms;
        }

        static void ShowPokemons(IEnumerable<PokemonData> pokeAttackers)
        {
            var str = "";
            foreach (var element in pokeAttackers) {
                str = $"{str}{element.PokemonId.ToString()}(CP:{element.Cp}-HP:{element.Stamina}), ";
            }
            Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - " + str);
        }

        private static bool CheckAndPutInNearbyGym(FortData gym, Client client, FortDetailsResponse fortInfo)
        {
            var gymColorLog = ConsoleColor.DarkGray;

            if (gymsVisited.IndexOf(gym.Id) > -1) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym was already visited.");
                return false;
            }

            if (!GlobalVars.FarmGyms) {
                return false;
            }
            Logger.Debug("(Gym) - Reviving pokemons.");
            ReviveAndCurePokemons(client);
            var pokemons = (client.Inventory.GetPokemons().Result).ToList();

            RandomHelper.RandomSleep(400, 500);
            var profile = client.Player.GetPlayer().Result;

            PokemonData pokemon = getPokeToPut(client, profile.PlayerData.BuddyPokemon.Id);
            
            Logger.Debug("Gym) - Pokemon to leave: " +pokemon.PokemonId);

            if (pokemon == null) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are no pokemons to assign.");
                return false;
            }

            if ((gym.OwnedByTeam == profile.PlayerData.Team) || (gym.OwnedByTeam == TeamColor.Neutral)) {
                RandomHelper.RandomSleep(200, 300);
                var gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
                Logger.ColoredConsoleWrite(gymColorLog, "Team:" + GetTeamName(gym.OwnedByTeam) + ". Members: " + gymDetails.GymState.Memberships.Count + ". Level: " + GetGymLevel(gym.GymPoints));
                if (gymDetails.GymState.Memberships.Count < GetGymLevel(gym.GymPoints)) {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is a free space");
                    putInGym(client, gym, pokemon, pokemons);
                } else if (GlobalVars.AttackGyms && gymDetails.GymState.Memberships.Count == 1) {
                    GlobalVars.PauseTheWalking = true;
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is only one defender. Let's go to train");
                    var pokeAttackers = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Stamina > 0))).OrderByDescending(x => x.Cp).Take(6);
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Selected pokemons to train:");
                    ShowPokemons(pokeAttackers);
                    var defenders = gymDetails.GymState.Memberships.Select(x => x.PokemonData);
                    var defender = defenders.FirstOrDefault();
                    Logger.Debug("Gym) - Pokemon defender: " +defender.PokemonId);
                    var attResp = AttackGym(gym, client, fortInfo, pokeAttackers, defender.Id, gymDetails.GymState.Memberships.Count, profile.PlayerData.BuddyPokemon.Id);
                    GlobalVars.PauseTheWalking = false;
                } else {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is no free space in the gym");
                }

            } else {
                
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym is not from your team.");
                if (!GlobalVars.AttackGyms)
                    return false;
                
                Shared.GlobalVars.PauseTheWalking = true;
                Logger.Debug("(Gym) - Stop walking");
                var gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
                Logger.ColoredConsoleWrite(gymColorLog, "Team:" + GetTeamName(gym.OwnedByTeam) + ". Members: " + gymDetails.GymState.Memberships.Count + ". Level: " + GetGymLevel(gym.GymPoints));

                // TODO: ATTACK more than 2 defender
                if (gymDetails.GymState.Memberships.Count >= 1 && gymDetails.GymState.Memberships.Count <=2) {
                    if (gymDetails.GymState.Memberships.Count == 1)
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is only one rival. Let's go to fight");
                    else if (gymDetails.GymState.Memberships.Count == 2)
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are two rivals. Let's go to fight");
                    var pokeAttackers = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Stamina > 0))).OrderByDescending(x => x.Cp).Take(6);
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Selected Atackers:");
                    ShowPokemons(pokeAttackers);
                    var defenders = gymDetails.GymState.Memberships.Select(x => x.PokemonData);
                    var defender = defenders.FirstOrDefault();
                    Logger.Debug("Gym) - Pokemon defender: " +defender.PokemonId);
                    var attResp = AttackGym(gym, client, fortInfo, pokeAttackers, defender.Id, gymDetails.GymState.Memberships.Count, profile.PlayerData.BuddyPokemon.Id);
                }
                GlobalVars.PauseTheWalking = false;
                Logger.Debug("(Gym) - Continnue walking");
            }
            return true;
        }
        private static AttackGymResponse AttackGym(FortData gym, Client client, FortDetailsResponse fortInfo,
            IEnumerable<PokemonData> pokeAttackers, ulong defenderId, int numDefenders, ulong buddyPokemonId)
        {
            var gymColorLog = ConsoleColor.DarkGray;
            var pokeAttackersIds = pokeAttackers.Select(x => x.Id);
            var moveSettings = GetMoveSettings(client);
            RandomHelper.RandomSleep(1000, 1500);
            var resp = client.Fort.StartGymBattle(gym.Id, defenderId, pokeAttackersIds).Result;
            // Sometimes we get a null from startgymBattle
            if (resp == null) {
                Logger.Debug("Response to start battle was null");
                return null;
            }
            if (resp.BattleLog == null) {
                Logger.Debug("BatlleLog to start battle was null");
                return null;
            }
            if (resp.BattleLog.State == BattleState.Active) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Battle Started");
                RandomHelper.RandomSleep(1000, 1100);
                   
                var battleActions = new List<BattleAction>();
                var lastRetrievedAction = new BattleAction();
                var battleStartMs = resp.BattleLog.BattleStartTimestampMs;
                var attResp = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction).Result;
                Logger.Debug("(Gym) - Attack Result: " + attResp.Result);
                Logger.Debug("(Gym) - Battle State: " + attResp.BattleLog.State);
                var inBattle = (attResp.Result == AttackGymResponse.Types.Result.Success);
                inBattle = inBattle && (attResp.BattleLog.State == BattleState.Active);
                var count = 1;
                while (inBattle) {
                    var timeMs = attResp.BattleLog.ServerMs;
                    var move1Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == attResp.ActiveAttacker.PokemonData.Move1).MoveSettings;
                    var attack = new BattleAction();
                    attack.Type = BattleActionType.ActionAttack;
                    attack.DurationMs = move1Settings.DurationMs; 
                    attack.DamageWindowsStartTimestampMs = move1Settings.DamageWindowStartMs;
                    attack.DamageWindowsEndTimestampMs = move1Settings.DamageWindowEndMs;
                    attack.ActionStartMs = timeMs + move1Settings.DurationMs;
                    attack.TargetIndex = -1;
                    attack.ActivePokemonId = attResp.ActiveAttacker.PokemonData.Id;
                    attack.TargetPokemonId = attResp.ActiveDefender.PokemonData.Id;
                    battleActions.Clear();
                    battleActions.Add(attack);
                    lastRetrievedAction = attResp.BattleLog.BattleActions.LastOrDefault();
                    attResp = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction).Result;
                    Logger.Debug("(Gym) - Attack Result: " + attResp.Result);
                    inBattle = (attResp.Result == AttackGymResponse.Types.Result.Success);
                    if (inBattle) {
                        Logger.Debug("(Gym) - Battle State: " + attResp.BattleLog.State);
                        inBattle = inBattle && (attResp.BattleLog.State == BattleState.Active);
                        Logger.Debug($"Attack {count} done.");
                        count++;
                        Logger.Debug("(Gym) - Wait a moment before next attact");
                        RandomHelper.RandomSleep(move1Settings.DurationMs + 30, move1Settings.DurationMs + 50);
                    }
                }
                Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Battle Finished in {count} attacks.");
                if (attResp.Result == AttackGymResponse.Types.Result.Success) {
                    if (attResp.BattleLog.State == BattleState.Defeated){
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have lost");
                        if (numDefenders > 1){
                            Logger.Debug("(Gym) - Leaving Battle");
                            attResp = LeaveBattle( gym,  client,   resp,  attResp,  battleActions, lastRetrievedAction);
                        }
                    }else if (attResp.BattleLog.State == BattleState.Victory) {
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have won");
                        ReviveAndCurePokemons(client);
                        if (numDefenders > 1){
                            Logger.Debug("(Gym) - Leaving Battle");
                            attResp = LeaveBattle( gym,  client,   resp,  attResp,  battleActions, lastRetrievedAction);
                        }else{
                            var pokemons = (client.Inventory.GetPokemons().Result).ToList();
                            putInGym(client, gym, getPokeToPut(client, buddyPokemonId), pokemons);
                        }
                    } else if (attResp.BattleLog.State == BattleState.TimedOut)
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Timed Out");
                    if (numDefenders == 1 && !gymsVisited.Contains(gym.Id))
                        gymsVisited.Add(gym.Id);
                }
                return attResp;
            }
            return null;
        }

        private static  AttackGymResponse LeaveBattle(FortData gym, Client client,  StartGymBattleResponse resp, AttackGymResponse attResp, List<BattleAction> battleActions,BattleAction lastRetrievedAction){
            var timeMs = attResp.BattleLog.ServerMs;
            var attack = new BattleAction();
            attack.Type = BattleActionType.ActionPlayerQuit;
            attack.DurationMs = 0; 
            attack.DamageWindowsStartTimestampMs = 0;
            attack.DamageWindowsEndTimestampMs = 0;
            attack.ActionStartMs = timeMs;
            attack.TargetIndex = -1;
            attack.ActivePokemonId = attResp.ActiveAttacker.PokemonData.Id;
            //attack.TargetPokemonId = attResp.ActiveDefender.PokemonData.Id;
            battleActions.Clear();
            battleActions.Add(attack);
            lastRetrievedAction = attResp.BattleLog.BattleActions.LastOrDefault();
            return client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction).Result;
        }

        private static PokemonData getPokeToPut(Client client, ulong buddyPokemon)
        {
            var pokemons = (client.Inventory.GetPokemons().Result).ToList();

            PokemonData pokemon;

            if (GlobalVars.LeaveInGyms == 0) {
                var rnd = new Random();
                pokemon = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax))).OrderBy(x => rnd.Next()).FirstOrDefault();
            } else if (GlobalVars.LeaveInGyms == 1)
                pokemon = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax))).OrderByDescending(x => x.Cp).FirstOrDefault();
            else
                pokemon = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax))).OrderBy(x => x.Cp).FirstOrDefault();
            return pokemon;
        }

        private static void putInGym(Client client, FortData gym, PokemonData pokemon, IEnumerable<PokemonData> pokemons)
        {
            RandomHelper.RandomSleep(100, 200);
            var fortSearch = client.Fort.FortDeployPokemon(gym.Id, pokemon.Id).Result;
            if (fortSearch.Result.ToString().ToLower() == "success") {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, pokemon.PokemonId + " inserted into the gym");
                if (!gymsVisited.Contains(gym.Id))
                    gymsVisited.Add(gym.Id);
                var pokesInGym = pokemons.Count(x => ((!x.IsEgg) && (x.DeployedFortId != ""))) + 1;
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "Pokemons in gyms: " + pokesInGym);
                if (pokesInGym > 9) { 
                    var res = client.Player.CollectDailyDefenderBonus().Result;
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - Collected: {res.CurrencyAwarded} Coins.");
                }
            } else
                Logger.Debug("error: " + fortSearch.Result);
        }
        

        private static IEnumerable<DownloadItemTemplatesResponse.Types.ItemTemplate> GetMoveSettings(Client client)
        {
            var templates = client.Download.GetItemTemplates().Result.ItemTemplates;
            return templates.Where(x => x.MoveSettings != null);
            // && x.MoveSettings.MovementId == move
        }

        private static void ReviveAndCurePokemons(Client client)
        {
            try {
                RandomHelper.RandomSleep(8000, 9000); // If we don`t wait, getpokemons return null.
                var pokemons = client.Inventory.GetPokemons(true).Result;
                foreach (var pokemon in pokemons) {
                    if (pokemon.Stamina <= 0) {
                        RandomHelper.RandomSleep(300, 400);
                        var revive = client.Inventory.GetItemAmountByType(ItemId.ItemRevive).Result;
                        if (revive > 0) {
                            var response = client.Inventory.UseItemRevive(ItemId.ItemRevive, pokemon.Id).Result;
                            if (response.Result == UseItemReviveResponse.Types.Result.Success) {
                                pokemon.Stamina = pokemon.StaminaMax/2;
                                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - Pokemon revived: " + pokemon.PokemonId);
                                CurePokemon(client, pokemon);
                            }else
                                Logger.Debug("Use revive result: "+ response.Result);
                        }
                    } else if (pokemon.Stamina < pokemon.StaminaMax) {
                        CurePokemon(client, pokemon);
                    }
                }
            } catch (Exception e) {
                Logger.ExceptionInfo(e.ToString());
            }
        }

        private static void CurePokemon(Client client, PokemonData pokemon)
        {
            var potion = GetNextAvailablePotion(client);
            var fails = 0;
            while (pokemon.Stamina < pokemon.StaminaMax && potion != 0 && fails < 3) {
                RandomHelper.RandomSleep(3000,4000);
                var response = client.Inventory.UseItemPotion(potion, pokemon.Id).Result;
                if (response.Result == UseItemPotionResponse.Types.Result.Success) {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - Pokemon {pokemon.PokemonId} cured. Stamina: {response.Stamina}/{pokemon.StaminaMax}" );
                    pokemon.Stamina = response.Stamina;
                    potion = GetNextAvailablePotion(client);
                    fails = 0;
                }else{
                    fails++;
                    Logger.Debug("Use potion result: "+ response.Result);
                }
            }
        }

        static ItemId GetNextAvailablePotion(Client client)
        {
            RandomHelper.RandomSleep(100, 200);
            var count = client.Inventory.GetItemAmountByType(ItemId.ItemPotion).Result;
            if (count > 0)
                return ItemId.ItemPotion;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemSuperPotion).Result;
            if (count > 0)
                return ItemId.ItemSuperPotion;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemHyperPotion).Result;
            if (count > 0)
                return ItemId.ItemHyperPotion;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemMaxPotion).Result;
            return count > 0 ? ItemId.ItemMaxPotion : 0;
        }
    }
}
