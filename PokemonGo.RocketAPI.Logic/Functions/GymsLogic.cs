/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 28/01/2017
 * Time: 18:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
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
        private static bool restoreWalkingAfterLogic = false;
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

        private static void AddVisited(string id, int milliseconds)
        {
            if (!gymsVisited.Contains(id)) {
                gymsVisited.Add(id);
                Task.Delay(milliseconds)
                    .ContinueWith(t => gymsVisited.Remove(id));
            }
        }

        public static void Execute()
        {
            if (!GlobalVars.FarmGyms)
                return;
            //narrow map data to gyms within walking distance
            var gyms = GetNearbyGyms();
            var gymsWithinRangeStanding = gyms.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude) < 40);
            var withinRangeStandingList = gymsWithinRangeStanding as IList<FortData> ?? gymsWithinRangeStanding.ToList();

            if (withinRangeStandingList.Any()) {
                var inRange = withinRangeStandingList.Count;
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - {inRange} gyms are within range of the user");

                foreach (var element in withinRangeStandingList) {
                    var gym = element;
                    
                    if (gymsVisited.Contains(gym.Id)) {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - This gym was already visited.");
                        continue;
                    }
                    var numberOfAttacks = GlobalVars.MaxAttacks;
                    while (numberOfAttacks > 0 && gymsVisited.IndexOf(gym.Id) == -1) {
                        Logger.Debug("(Gym) - Attack number " + (GlobalVars.MaxAttacks + 1 - numberOfAttacks));
                        CheckAndPutInNearbyGym(gym, Logic.objClient);
                        numberOfAttacks--;
                        if (numberOfAttacks > 0 && gymsVisited.IndexOf(gym.Id) == -1) {
                            RandomHelper.RandomSleep(400);
                            gym = GetNearbyGyms().FirstOrDefault(x => x.Id == gym.Id);
                        }
                        if (numberOfAttacks == 0) {
                            Logger.Info("(Gym) - Maximun number of attacks reached. Will be checked after of one minute.");
                            AddVisited(gym.Id, 60000);
                        }
                    }
                    Setout.SetCheckTimeToRun();
                    RandomHelper.RandomSleep(300);
                }
                if (restoreWalkingAfterLogic)
                    GlobalVars.PauseTheWalking = false;

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

        private static string strPokemon(PokemonData pokemon)
        {
            var str = $"{pokemon.PokemonId.ToString()}(CP:{pokemon.Cp}-HP:{pokemon.Stamina}";
            return str;
        }
        private static void ShowPokemons(IEnumerable<PokemonData> pokeAttackers)
        {
            var str = "";
            foreach (var element in pokeAttackers) {
                str = $"{str}{strPokemon(element)}, ";
            }
            str = str.Substring(0, str.Length - 2);
            Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - " + str);
        }

        private static IEnumerable<PokemonData>  getPokeAttackers(IEnumerable<PokemonData> pokemons, PokemonData defender)
        {
            var filter1 = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Stamina > 0)));
            if (GlobalVars.GymAttackers == 1)
                filter1 = filter1.OrderByDescending(x => x.Cp).Take(6);
            else if (GlobalVars.GymAttackers == 2)
                filter1 = filter1.OrderByDescending(x => x.Favorite).ThenByDescending(x => x.Cp).Take(6);
            else if (GlobalVars.GymAttackers == 3)
                filter1 = filter1.Where(x => x.Cp < defender.Cp).OrderByDescending(x => x.Cp).Take(6);
            else {
                // GymAttackers == 0
                var rnd = new Random();
                filter1 = filter1.OrderBy(x => rnd.Next()).Take(6);
            }
            return filter1;
        }

        private static bool CheckAndPutInNearbyGym(FortData gym, Client client)
        {
            var gymColorLog = ConsoleColor.DarkGray;

            if (!GlobalVars.FarmGyms)
                return false;

            if (gymsVisited.IndexOf(gym.Id) > -1)
                return false;


            Logger.Debug("(Gym) - Reviving pokemons.");
            ReviveAndCurePokemons(client);
            var pokemons = (client.Inventory.GetPokemons().Result).ToList();

            RandomHelper.RandomSleep(900);
            var profile = client.Player.GetPlayer().Result;

            PokemonData pokemon = getPokeToPut(client, profile.PlayerData.BuddyPokemon.Id);

            Logger.Debug("(Gym) - Pokemon to insert: " + pokemon.PokemonId);

            if (pokemon == null) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are no pokemons to assign.");
                return false;
            }

            var gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Team: " + GetTeamName(gym.OwnedByTeam) + ".");

            if (gym.OwnedByTeam == TeamColor.Neutral) {
                RandomHelper.RandomSleep(200, 300);
                putInGym(client, gym, pokemon, pokemons);
            } else if ((gym.OwnedByTeam == profile.PlayerData.Team)) {
                RandomHelper.RandomSleep(200, 300);
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Members: " + gymDetails.GymState.Memberships.Count + ". Level: " + GetGymLevel(gym.GymPoints) + " (" + gym.GymPoints + ")");
                if (gymDetails.GymState.Memberships.Count < GetGymLevel(gym.GymPoints)) {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is a free space");
                    putInGym(client, gym, pokemon, pokemons);
                } else if (GlobalVars.AttackGyms && gymDetails.GymState.Memberships.Count <= GlobalVars.NumDefenders) {
                    restoreWalkingAfterLogic = !GlobalVars.PauseTheWalking;
                    GlobalVars.PauseTheWalking = true;
                    Logger.Debug("(Gym) - Stop walking ");
                    if (gymDetails.GymState.Memberships.Count == 1)
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is only one defender. Let's go to train");
                    else
                        Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - There are {gymDetails.GymState.Memberships.Count} defenders. Let's go to train");
                    var defenders = gymDetails.GymState.Memberships.Select(x => x.PokemonData);
                    var defender = defenders.FirstOrDefault();
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Defender: " + strPokemon(defender));
                    var pokeAttackers = getPokeAttackers(pokemons, defender);
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Selected pokemons to train:");
                    ShowPokemons(pokeAttackers);
                    var attResp = AttackGym(gym, client, pokeAttackers, defender.Id, gymDetails.GymState.Memberships.Count, profile.PlayerData.BuddyPokemon.Id);
                } else {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is no free space in the gym");
                    AddVisited(gym.Id, 600000);
                }

            } else {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym is not from your team.");
                if (!GlobalVars.AttackGyms)
                    return false;
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Members: " + gymDetails.GymState.Memberships.Count + ". Level: " + GetGymLevel(gym.GymPoints) + " (" + gym.GymPoints + ")");
                if (gymDetails.GymState.Memberships.Count >= 1 && gymDetails.GymState.Memberships.Count <= GlobalVars.NumDefenders) {
                    restoreWalkingAfterLogic = !GlobalVars.PauseTheWalking;
                    GlobalVars.PauseTheWalking = true;
                    Logger.Debug("(Gym) - Stop walking ");
                    
                    if (gymDetails.GymState.Memberships.Count == 1)
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is only one rival. Let's go to fight");
                    else
                        Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - There are {gymDetails.GymState.Memberships.Count} rivals. Let's go to fight");
                    var defenders = gymDetails.GymState.Memberships.Select(x => x.PokemonData);
                    var defender = defenders.FirstOrDefault();
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Defender: " + strPokemon(defender));
                    var pokeAttackers = getPokeAttackers(pokemons, defender);
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Selected Atackers:");
                    ShowPokemons(pokeAttackers);
                    var attResp = AttackGym(gym, client, pokeAttackers, defender.Id, gymDetails.GymState.Memberships.Count, profile.PlayerData.BuddyPokemon.Id);
                } else {
                    AddVisited(gym.Id, 600000);
                }
            }
            return true;
        }

        private static AttackGymResponse AttackGym(FortData gym, Client client,
            IEnumerable<PokemonData> pokeAttackers, ulong defenderId, int numDefenders, ulong buddyPokemonId)
        {
            var gymColorLog = ConsoleColor.DarkGray;
            var pokeAttackersIds = pokeAttackers.Select(x => x.Id);
            var moveSettings = GetMoveSettings(client);
            RandomHelper.RandomSleep(1100);
            var resp = client.Fort.StartGymBattle(gym.Id, defenderId, pokeAttackersIds).Result;
            var numTries = 3;
            // Sometimes we get a null from startgymBattle so we try to start battle 3 times
            var startFailed = true;
            startFailed = (resp == null);
            if (!startFailed)
                startFailed = (resp.BattleLog == null);

            while (startFailed && numTries > 0) {
                if (resp == null)
                    Logger.Debug("(Gym) - Response to start battle was null.");
                if (resp.BattleLog == null)
                    Logger.Debug("(Gym) - BatlleLog to start battle was null");
                Logger.Debug("(Gym) - Trying again after 11 seconds");
                RandomHelper.RandomSleep(10000, 11000);
                var mapObjectsResponse = Logic.objClient.Map.GetMapObjects().Result.Item1;
                RandomHelper.RandomSleep(800);
                var gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
                RandomHelper.RandomSleep(800);
                resp = client.Fort.StartGymBattle(gym.Id, defenderId, pokeAttackersIds).Result;
                startFailed = (resp == null);
                if (!startFailed)
                    startFailed = (resp.BattleLog == null);
                numTries--;
            }

            if (startFailed)
                return null;

            if (resp.BattleLog.State == BattleState.Active) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Battle Started");
                RandomHelper.RandomSleep(1000);
                var battleActions = new List<BattleAction>();
                var lastRetrievedAction = new BattleAction();
                var battleStartMs = resp.BattleLog.BattleStartTimestampMs;
                var attResp = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction).Result;
                var inBattle = (attResp.Result == AttackGymResponse.Types.Result.Success);
                inBattle = inBattle && (attResp.BattleLog.State == BattleState.Active);
                var count = 1;
                var energy = 0;
                Logger.Debug("attResp: " + attResp);
                while (inBattle) {
                    var timeMs = attResp.BattleLog.ServerMs;
                    var move1Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == attResp.ActiveAttacker.PokemonData.Move1).MoveSettings;
                    var move2Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == attResp.ActiveAttacker.PokemonData.Move2).MoveSettings;
                    var attack = new BattleAction();
                    attack.ActionStartMs = timeMs + RandomHelper.RandomNumber(110, 170);
                    attack.TargetIndex = -1;
                    attack.TargetPokemonId = attResp.ActiveDefender.PokemonData.Id;
                    if (attResp.ActiveAttacker.PokemonData.Stamina > 0)
                        attack.ActivePokemonId = attResp.ActiveAttacker.PokemonData.Id;

                    if (energy >= Math.Abs(move2Settings.EnergyDelta)) {
                        attack.Type = BattleActionType.ActionSpecialAttack;
                        attack.DurationMs = move2Settings.DurationMs;
                        attack.DamageWindowsStartTimestampMs = attack.ActionStartMs + move2Settings.DamageWindowStartMs;
                        attack.DamageWindowsEndTimestampMs = attack.ActionStartMs + move2Settings.DamageWindowEndMs;
                        attack.EnergyDelta = move2Settings.EnergyDelta;
                    } else {
                        var dodge = RandomHelper.RandomNumber(1, 60);
                        if (dodge == 1) {
                            attack.Type = BattleActionType.ActionDodge;
                            attack.DurationMs = 500;
                        } else if (dodge == 2) {
                            attack.Type = BattleActionType.ActionFaint;
                        } else {
                            attack.Type = BattleActionType.ActionAttack;
                            attack.DurationMs = move1Settings.DurationMs;
                            attack.DamageWindowsStartTimestampMs = attack.ActionStartMs + move1Settings.DamageWindowStartMs;
                            attack.DamageWindowsEndTimestampMs = attack.ActionStartMs + move1Settings.DamageWindowEndMs;
                            attack.EnergyDelta = move1Settings.EnergyDelta;
                        }
                    }

                    lastRetrievedAction = new BattleAction(); //attResp.BattleLog.BattleActions.FirstOrDefault();
                    battleActions = new List<BattleAction>();
                    battleActions.Add(attack);
                    Logger.Debug("(Gym) - Attack: " + attack);
                    attResp = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction).Result;
                    Logger.Debug("attResp: " + attResp);
                    inBattle = (attResp.Result == AttackGymResponse.Types.Result.Success);
                    if (inBattle) {
                        inBattle = inBattle && (attResp.BattleLog.State == BattleState.Active);

                        if (attResp.ActiveAttacker != null) {
                            energy = attResp.ActiveAttacker.CurrentEnergy;
                            var health = attResp.ActiveAttacker.CurrentHealth;
                            var activeAttacker = attResp.ActiveAttacker.PokemonData.PokemonId;
                            Logger.Debug($"(Gym) - Attacker: {activeAttacker} Energy={energy}, Health={health}");
                        }

                        if (attResp.ActiveDefender != null) {
                            var energyDef = attResp.ActiveDefender.CurrentEnergy;
                            var health = attResp.ActiveDefender.CurrentHealth;
                            var activeDefender = attResp.ActiveDefender.PokemonData.PokemonId;
                            Logger.Debug($"(Gym) - Defender: {activeDefender} Energy={energyDef}, Health={health}");
                        }

                        count++;
                        //var waitTime = 0; //attack.ActionStartMs + attack.DurationMs - attResp.BattleLog.ServerMs;
                        RandomHelper.RandomSleep(1, 99);
                    }
                }

                Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Battle Finished in {count} Rounds.");
                if (attResp.Result == AttackGymResponse.Types.Result.Success) {
                    if (attResp.BattleLog.State == BattleState.Defeated) {
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have lost");
                        if (numDefenders > 1) {
                            attResp = LeaveBattle(gym, client, resp, attResp, lastRetrievedAction);
                            Logger.Debug("(Gym) - Leaving Battle");
                            AddVisited(gym.Id, 3600000);
                        }
                    } else if (attResp.BattleLog.State == BattleState.Victory) {
                        
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have won");
                        ReviveAndCurePokemons(client);
                        if (numDefenders > 1) {
                            attResp = LeaveBattle(gym, client, resp, attResp, lastRetrievedAction);
                            Logger.Debug("(Gym) - Leaving Battle");
                        } else {
                            var pokemons = (client.Inventory.GetPokemons().Result).ToList();
                            RandomHelper.RandomSleep(400);
                            var gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
                            Logger.Debug("(Gym) - Gym Details: " + gymDetails);
                            if (gymDetails.GymState.Memberships.Count < 1) {
                                putInGym(client, gym, getPokeToPut(client, buddyPokemonId), pokemons);
                            }
                        }
                    } else if (attResp.BattleLog.State == BattleState.TimedOut)
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Timed Out");
                } else {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Battle Failed: " + attResp.Result);
                }
                return attResp;
            }
            return null;
        }

        private static  AttackGymResponse LeaveBattle(FortData gym, Client client, StartGymBattleResponse resp, AttackGymResponse attResp, BattleAction lastRetrievedAction)
        {
            var timeMs = attResp.BattleLog.ServerMs;
            var attack = new BattleAction();
            attack.Type = BattleActionType.ActionPlayerQuit;
            attack.ActionStartMs = timeMs + RandomHelper.RandomNumber(400, 500);
            attack.TargetIndex = -1;
            attack.ActivePokemonId = attResp.ActiveAttacker.PokemonData.Id;
            var battleActions = new List<BattleAction>();
            battleActions.Add(attack);
            lastRetrievedAction = new BattleAction();
            var ret = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction).Result;
            return ret;
        }

        private static PokemonData getPokeToPut(Client client, ulong buddyPokemon)
        {
            var pokemons = (client.Inventory.GetPokemons().Result).ToList();

            switch (GlobalVars.LeaveInGyms) {
                case 1:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                        .OrderByDescending(x => x.Cp).FirstOrDefault();
                case 2:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                        .OrderBy(x => x.Cp).FirstOrDefault();
                case 3:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                        .OrderByDescending(x => x.Favorite).ThenByDescending(x => x.Cp).FirstOrDefault();
            }
            // GlobalVars.LeaveInGyms ==0
            var rnd = new Random();
            return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                .OrderBy(x => rnd.Next()).FirstOrDefault();

        }

        private static void putInGym(Client client, FortData gym, PokemonData pokemon, IEnumerable<PokemonData> pokemons)
        {
            RandomHelper.RandomSleep(400);
            var fortSearch = client.Fort.FortDeployPokemon(gym.Id, pokemon.Id).Result;
            if (fortSearch.Result == FortDeployPokemonResponse.Types.Result.Success) {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - " + pokemon.PokemonId + " inserted into the gym");
                var pokesInGym = pokemons.Count(x => ((!x.IsEgg) && (x.DeployedFortId != ""))) + 1;
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - Pokemons in gyms: " + pokesInGym);
                if (pokesInGym > 9) {
                    var res = client.Player.CollectDailyDefenderBonus().Result;
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - Collected: {res.CurrencyAwarded} Coins.");
                }
                AddVisited(gym.Id, 3600000);
            } else {
                if (fortSearch.Result == FortDeployPokemonResponse.Types.Result.ErrorAlreadyHasPokemonOnFort) {
                    Logger.Warning("Already have a pokemon on the Gym");
                    AddVisited(gym.Id, 3600000);
                } else
                    Logger.Debug("error: " + fortSearch.Result);
            }
        }
        
        private static IEnumerable<DownloadItemTemplatesResponse.Types.ItemTemplate> GetMoveSettings(Client client)
        {
            var templates = client.Download.GetItemTemplates().Result.ItemTemplates;
            return templates.Where(x => x.MoveSettings != null);
        }

        private static void ReviveAndCurePokemons(Client client)
        {
            try {
                var pokemons = client.Inventory.GetPokemons().Result.Where(x => x.Stamina < x.StaminaMax);
                if (!pokemons.Any())
                    return;
                RandomHelper.RandomSleep(7000, 8000); // If we don`t wait, getpokemons return null.
                pokemons = client.Inventory.GetPokemons(true).Result.Where(x => x.Stamina < x.StaminaMax);
                foreach (var pokemon in pokemons) {
                    if (pokemon.Stamina <= 0) {
                        RandomHelper.RandomSleep(400, 500);
                        var revive = GetNextAvailableRevive(client);
                        if (revive != 0) {
                            var response = client.Inventory.UseItemRevive(revive, pokemon.Id).Result;
                            if (response.Result == UseItemReviveResponse.Types.Result.Success) {
                                if (revive == ItemId.ItemRevive) {
                                    pokemon.Stamina = pokemon.StaminaMax / 2;
                                    CurePokemon(client, pokemon);
                                } else
                                    pokemon.Stamina = pokemon.StaminaMax;
                                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - Pokemon revived: " + pokemon.PokemonId);

                            } else
                                Logger.Debug("Use revive result: " + response.Result);
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
                RandomHelper.RandomSleep(2000, 2500);
                var response = client.Inventory.UseItemPotion(potion, pokemon.Id).Result;
                if (response.Result == UseItemPotionResponse.Types.Result.Success) {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - Pokemon {pokemon.PokemonId} cured. Stamina: {response.Stamina}/{pokemon.StaminaMax}" );
                    pokemon.Stamina = response.Stamina;
                    potion = GetNextAvailablePotion(client);
                    fails = 0;
                } else {
                    fails++;
                    Logger.Debug("Use potion result: " + response.Result);
                }
            }
        }

        private static ItemId GetNextAvailablePotion(Client client)
        {
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

        private static ItemId GetNextAvailableRevive(Client client)
        {
            var count = client.Inventory.GetItemAmountByType(ItemId.ItemRevive).Result;
            if (count > 0)
                return ItemId.ItemRevive;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemMaxRevive).Result;
            return count > 0 ? ItemId.ItemMaxRevive : 0;
        }

        public static void putInPokestop(Client client, ulong buddyPokemon, FortData gym, IEnumerable<PokemonData> pokemons)
        {
            var poke = getPokeToPut(client, buddyPokemon);
            putInGym(client, gym, poke, pokemons);
        }
    }
}
