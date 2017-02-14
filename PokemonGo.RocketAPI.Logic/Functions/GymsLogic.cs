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
        private static ConsoleColor gymColorLog = ConsoleColor.DarkGray;
        
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
                    while (numberOfAttacks > 0 && !gymsVisited.Contains(gym.Id)) {
                        Logger.Debug("(Gym) - Attack number " + (GlobalVars.MaxAttacks + 1 - numberOfAttacks));
                        CheckAndPutInNearbyGym(gym, Logic.objClient);
                        numberOfAttacks--;
                        if (numberOfAttacks > 0 && !gymsVisited.Contains(gym.Id)) {
                            RandomHelper.RandomSleep(900);
                            gym = GetNearbyGyms().FirstOrDefault(x => x.Id == gym.Id);
                        }
                        if (numberOfAttacks == 0) {
                            Logger.Warning("(Gym) - Maximun number of attacks reached. Will be checked after of one minute.");
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
            Logic.objClient.Player.UpdatePlayerLocation(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude,Logic.objClient.CurrentAltitude);
            RandomHelper.RandomSleep(400);

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
            var str = $"{pokemon.PokemonId.ToString()}(CP:{pokemon.Cp}|HP:{pokemon.Stamina})";
            return str;
        }
        private static void ShowPokemons(IEnumerable<PokemonData> pokeAttackers)
        {
            var str = "";
            foreach (var element in pokeAttackers) {
                str = $"{str}{strPokemon(element)}, ";
            }
            if (str.Length > 2)
                str = str.Substring(0, str.Length - 2);
            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - " + str);
        }

        private static IEnumerable<PokemonData>  getPokeAttackers(IEnumerable<PokemonData> pokemons, PokemonData defender)
        {
            var filter1 = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Stamina > 0)));
            var filter2 = filter1;
            switch (GlobalVars.GymAttackers) {
                case 1:
                    return filter1.OrderByDescending(x => x.Cp).Take(6);
                case 2:
                    return filter1.OrderByDescending(x => x.Favorite).ThenByDescending(x => x.Cp).Take(6);
                case 3:
                    filter2 = filter1.Where(x => x.Cp < defender.Cp).OrderByDescending(x => x.Cp).Take(6);
                    if (filter2.Count() < 6) {
                        var left = 6 - filter2.Count();
                        filter2 = filter1.Concat(filter1.OrderByDescending(x => x.Cp).Take(left));
                    }
                    return filter2;
            }
            // GlobalVars.GymAttackers ==  0
            var rnd = new Random();
            filter2 = filter1.OrderBy(x => rnd.Next()).Take(6);
            return filter2;
        }

        private static bool CheckAndPutInNearbyGym(FortData gym, Client client)
        {

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

            if (pokemon == null) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are no pokemons to assign.");
                return false;
            }

            Logger.Debug("(Gym) - Pokemon to insert: " + strPokemon(pokemon));

            var gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Team: " + GetTeamName(gym.OwnedByTeam) + ".");

            if (gym.OwnedByTeam == TeamColor.Neutral) {
                RandomHelper.RandomSleep(250);
                putInGym(client, gym, pokemon, pokemons);
            } else if ((gym.OwnedByTeam == profile.PlayerData.Team)) {
                RandomHelper.RandomSleep(250);
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
            
            var pokeAttackersIds = pokeAttackers.Select(x => x.Id);
            var moveSettings = GetMoveSettings(client);
            GetMapObjectsResponse  mapObjectsResponse;
            GetGymDetailsResponse gymDetails;
            StartGymBattleResponse  resp = null;

            // Sometimes we get a null from startgymBattle so we try to start battle 3 times
            var numTries = 3;
            var startFailed = true;

            while (startFailed && numTries > 0) {
                RandomHelper.RandomSleep(10000, 11000);
                mapObjectsResponse = Logic.objClient.Map.GetMapObjects().Result.Item1;
                RandomHelper.RandomSleep(800);
                gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
				RandomHelper.RandomSleep(800);
                resp = client.Fort.StartGymBattle(gym.Id, defenderId, pokeAttackersIds).Result;
                startFailed = false;
                if (resp == null){
                    Logger.Debug("(Gym) - Response to start battle was null.");
                    startFailed = true;
                }else{
                    if (resp.BattleLog == null){
                        Logger.Debug("(Gym) - BatlleLog to start battle was null");
                        startFailed = true;
                    }
                }
                if (startFailed)
                    Logger.Debug("(Gym) - Trying again after 12 seconds");
                
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
                    battleActions = new List<BattleAction>();
                    
                    var baseAction = new BattleAction();
                    baseAction.ActionStartMs = timeMs + RandomHelper.RandomNumber(110, 170);
                    baseAction.TargetIndex = -1;
                    baseAction.TargetPokemonId = attResp.ActiveDefender.PokemonData.Id;
                    if (attResp.ActiveAttacker.PokemonData.Stamina > 0)
                        baseAction.ActivePokemonId = attResp.ActiveAttacker.PokemonData.Id;
                    // One each six times we try to evade attack
                    if (RandomHelper.RandomNumber(1, 6)==1){
                        var dodgeAction = new BattleAction();
                        dodgeAction.ActionStartMs = baseAction.ActionStartMs;
                        dodgeAction.TargetIndex = baseAction.TargetIndex;
                        dodgeAction.TargetPokemonId = baseAction.TargetPokemonId;
                        dodgeAction.ActivePokemonId = baseAction.ActivePokemonId;
                        dodgeAction.Type = BattleActionType.ActionDodge;
                        dodgeAction.DurationMs = 500;
                        battleActions.Add(dodgeAction);
                        Logger.Debug("Dodge Action Added");
                        baseAction.ActionStartMs = dodgeAction.ActionStartMs + dodgeAction.DurationMs;
                    }
                    // Don`t know how to use this action. Reading about it is an automatic action when dying.                    
                    /*if (RandomHelper.RandomNumber(1, 10)==1){
                        var faintAction = new BattleAction();
                        faintAction.ActionStartMs = baseAction.ActionStartMs;
                        faintAction.TargetIndex = baseAction.TargetIndex;
                        faintAction.TargetPokemonId = baseAction.TargetPokemonId;
                        faintAction.ActivePokemonId = baseAction.ActivePokemonId;
                        faintAction.Type = BattleActionType.ActionFaint;
                        battleActions.Add(faintAction);
                        Logger.Debug("Faint Action Added");
                        baseAction.ActionStartMs = faintAction.ActionStartMs + faintAction.DurationMs;
                    }*/
                    if (energy >= Math.Abs(move2Settings.EnergyDelta)) {
                        var specialAttack = new BattleAction();
                        specialAttack.ActionStartMs = baseAction.ActionStartMs;
                        specialAttack.TargetIndex = baseAction.TargetIndex;
                        specialAttack.TargetPokemonId = baseAction.TargetPokemonId;
                        specialAttack.ActivePokemonId = baseAction.ActivePokemonId;
                        specialAttack.Type = BattleActionType.ActionSpecialAttack;
                        specialAttack.DurationMs = move2Settings.DurationMs;
                        specialAttack.DamageWindowsStartTimestampMs = specialAttack.ActionStartMs + move2Settings.DamageWindowStartMs;
                        specialAttack.DamageWindowsEndTimestampMs = specialAttack.ActionStartMs + move2Settings.DamageWindowEndMs;
                        specialAttack.EnergyDelta = move2Settings.EnergyDelta;
                        battleActions.Add(specialAttack);
                        Logger.Debug("Special Attack Added");
                        baseAction.ActionStartMs = specialAttack.ActionStartMs+ specialAttack.DurationMs;
                    }else{
                        // One each six times we do not attack
                        if (RandomHelper.RandomNumber(1, 6) > 1){
                            var normalAttack = new BattleAction();
                            normalAttack.ActionStartMs = baseAction.ActionStartMs;
                            normalAttack.TargetIndex = baseAction.TargetIndex;
                            normalAttack.TargetPokemonId = baseAction.TargetPokemonId;
                            normalAttack.ActivePokemonId = baseAction.ActivePokemonId;
                            normalAttack.Type = BattleActionType.ActionAttack;
                            normalAttack.DurationMs = baseAction.DurationMs;
                            normalAttack.DamageWindowsStartTimestampMs = normalAttack.ActionStartMs + move1Settings.DamageWindowStartMs;
                            normalAttack.DamageWindowsEndTimestampMs = normalAttack.ActionStartMs + move1Settings.DamageWindowEndMs;
                            normalAttack.EnergyDelta = move1Settings.EnergyDelta;
                            battleActions.Add(normalAttack);
                            Logger.Debug("Normal Attack Added");
                            baseAction.ActionStartMs = normalAttack.ActionStartMs+ normalAttack.DurationMs;
                        }
                    }

                    lastRetrievedAction = new BattleAction(); //attResp.BattleLog.BattleActions.FirstOrDefault();
                    var str  =string.Join(",", battleActions);
                    Logger.Debug("(Gym) - battleActions: " + str);
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
                        // Wait until all attack are done. but not more than 1 second.
                        var waitTime = (int) (baseAction.ActionStartMs - attResp.BattleLog.ServerMs);
                        if (waitTime < 0)
                            waitTime =0;
                        else if (waitTime > 1000)
                            waitTime = 1000;
                        RandomHelper.RandomSleep(waitTime,waitTime+100);
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
                        if (numDefenders > 1) {
                            attResp = LeaveBattle(gym, client, resp, attResp, lastRetrievedAction);
                            Logger.Debug("(Gym) - Leaving Battle");
                        } else {
                            ReviveAndCurePokemons(client);
                            var pokemons = (client.Inventory.GetPokemons().Result).ToList();
                            RandomHelper.RandomSleep(400);
                            gymDetails = client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude).Result;
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
            if (attResp.ActiveAttacker != null)
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
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - " + pokemon.PokemonId + " inserted into the gym");
                var pokesInGym = pokemons.Count(x => ((!x.IsEgg) && (x.DeployedFortId != ""))) + 1;
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Pokemons in gyms: " + pokesInGym);
                if (pokesInGym > 9) {
                    var res = client.Player.CollectDailyDefenderBonus().Result;
                    Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Collected: {res.CurrencyAwarded} Coins.");
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
                RandomHelper.RandomSleep(7000); // If we don`t wait, getpokemons return null.
                pokemons = client.Inventory.GetPokemons(true).Result.Where(x => x.Stamina < x.StaminaMax);
                foreach (var pokemon in pokemons) {
                    if (pokemon.Stamina <= 0) {
                        var revive = GetNextAvailableRevive(client);
                        Logger.Debug("revive:" +revive);
                        if (revive != 0) {
                            RandomHelper.RandomSleep(500);
                            var response = client.Inventory.UseItemRevive(revive, pokemon.Id).Result;
                            if (response.Result == UseItemReviveResponse.Types.Result.Success) {
                                if (revive == ItemId.ItemRevive) {
                                    pokemon.Stamina = pokemon.StaminaMax / 2;
                                    CurePokemon(client, pokemon);
                                } else
                                    pokemon.Stamina = pokemon.StaminaMax;
                                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Pokemon revived: " + pokemon.PokemonId);

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
            Logger.Debug("potion:" +potion);
            while (pokemon.Stamina < pokemon.StaminaMax && potion != 0 && fails < 3) {
                RandomHelper.RandomSleep(2000);
                var response = client.Inventory.UseItemPotion(potion, pokemon.Id).Result;
                if (response.Result == UseItemPotionResponse.Types.Result.Success) {
                    Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Pokemon {pokemon.PokemonId} cured. Stamina: {response.Stamina}/{pokemon.StaminaMax}" );
                    pokemon.Stamina = response.Stamina;
                    fails = 0;
                } else {
                    fails++;
                    Logger.Debug("Use potion result: " + response.Result);
                }
                potion = GetNextAvailablePotion(client);
            }
        }

        private static ItemId GetNextAvailablePotion(Client client)
        {
            var count = client.Inventory.GetItemAmountByType(ItemId.ItemPotion).Result;
            Logger.Debug("count ItemPotion:" +count);
            if (count > 0)
                return ItemId.ItemPotion;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemSuperPotion).Result;
            Logger.Debug("count ItemSuperPotion:" +count);
            if (count > 0)
                return ItemId.ItemSuperPotion;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemHyperPotion).Result;
            Logger.Debug("count ItemHyperPotion:" +count);
            if (count > 0)
                return ItemId.ItemHyperPotion;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemMaxPotion).Result;
            Logger.Debug("count ItemMaxPotion:" +count);
            return count > 0 ? ItemId.ItemMaxPotion : 0;
        }

        private static ItemId GetNextAvailableRevive(Client client)
        {
            var count = client.Inventory.GetItemAmountByType(ItemId.ItemRevive).Result;
            Logger.Debug("count ItemRevive:" +count);
            if (count > 0)
                return ItemId.ItemRevive;
            count = client.Inventory.GetItemAmountByType(ItemId.ItemMaxRevive).Result;
            Logger.Debug("count ItemMaxRevive:" +count);
            return count > 0 ? ItemId.ItemMaxRevive : 0;
        }

        public static void putInPokestop(Client client, ulong buddyPokemon, FortData gym, IEnumerable<PokemonData> pokemons)
        {
            var poke = getPokeToPut(client, buddyPokemon);
            putInGym(client, gym, poke, pokemons);
        }
    }
}
