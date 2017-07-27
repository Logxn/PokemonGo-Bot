/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 28/01/2017
 * Time: 18:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Device.Location;
using System.Threading.Tasks;
using POGOProtos.Data.Gym;
using POGOProtos.Settings.Master;
using PokeMaster.Logic.Shared;

using System.Collections.Generic;
using System.Linq;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic.Utils;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI;
using PokeMaster.Logic;
using PokeMaster.Logic.Functions;
using POGOProtos.Data;
using POGOProtos.Data.Battle;

namespace PokeMaster.Logic.Functions
{
    /// <summary>
    /// Description of GymsLogic.
    /// </summary>
    public static class GymsLogic
    {
        private static List<string> gymsVisited = new List<string>();
        private static bool restoreWalkingAfterLogic = false;
        private static ConsoleColor gymColorLog = ConsoleColor.DarkGray;
        private static Random RandomNumbers = new Random();
        public static bool StopAttack = false;

        public static void ResetVisitedMarks(){
            gymsVisited.Clear();
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
                    .ContinueWith(t => {
                    if (gymsVisited.Contains(id))
                        gymsVisited.Remove(id);
                });
            }
        }

        public static void Execute()
        {
            if (!GlobalVars.Gyms.Farm) {
                Logger.Debug("Farm gyms is not enabled.");
                return;
            }
            //narrow map data to gyms within walking distance
            var gyms = GetNearbyGyms();
            
            Logger.Debug("gyms: " + gyms.Count());
            var gymsWithinRangeStanding = gyms.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude) <= 40);
            var inRange = gymsWithinRangeStanding.Count();
            Logger.Debug("gymsWithinRangeStanding: " + inRange);

            if (gymsWithinRangeStanding.Any()) {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - {inRange} gyms are within range of the user");

                foreach (var element in gymsWithinRangeStanding) {
                    var gym = element;
                    if (gymsVisited.Contains(gym.Id)) {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "(Gym) - This gym was already visited.");
                        continue;
                    }
                    var attackCount = 1;
                    while (attackCount <= GlobalVars.Gyms.MaxAttacks && !gymsVisited.Contains(gym.Id)) {
                        Logger.Debug("(Gym) - Attack number " + attackCount);
                        CheckAndPutInNearbyGym(gym, Logic.objClient);
                        attackCount++;
                        if (attackCount > GlobalVars.Gyms.MaxAttacks) {
                            Logger.Warning("(Gym) - Maximum number of attacks reached. Will be checked after of one minute.");
                            AddVisited(gym.Id, 60000);
                        }
                    }
                    Setout.SetCheckTimeToRun();
                }
                GlobalVars.PauseTheWalking &= !restoreWalkingAfterLogic;
            }

        }

        private static void RefreshGymsInMap(FortData[] gyms)
        {
            foreach (var element in gyms) {
                Logic.Instance.infoObservable.PushUpdatePokeGym(element);
            }
        }

        private static FortData[] GetNearbyGyms()
        {
            LocationUtils.updatePlayerLocation(Logic.objClient, Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, Logic.objClient.CurrentAltitude);
            var mapObjectsResponse = Logic.objClient.Map.GetMapObjects().Result;
            
            var pokeGyms1 = mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                .Where(i => i.Type == FortType.Gym);

            var pokeGyms2 = pokeGyms1
                .OrderBy(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude));

            var pokeGyms = pokeGyms2
                    .ToArray();

            Logger.Debug("pokeGyms: " + pokeGyms.Length);

            Task.Factory.StartNew(() => RefreshGymsInMap(pokeGyms));
            return pokeGyms;
        }

        public static string strPokemon(PokemonData pokemon)
        {
            var str = $"{pokemon.PokemonId.ToString()} (CP: {pokemon.Cp} | HP: {pokemon.Stamina})";
            return str;
        }

        public static string strPokemons(IEnumerable<GymDefender> pokemons)
        {
            var str = "";
            foreach (var element in pokemons) {
                str = $"{str}{strPokemon(element.MotivatedPokemon.Pokemon)}\n";
            }
            if (str.Length > 1)
                str = str.Substring(0, str.Length - 1);
            return str;
        }

        public static string strPokemons(IEnumerable<POGOProtos.Data.Gym.GymMembership> pokemons)
        {
            var str = "";
            foreach (var element in pokemons) {
                str = $"{str}{strPokemon(element.PokemonData)}  [ {element.TrainerPublicProfile.Name} ({element.TrainerPublicProfile.Level}) ]\n";
            }
            if (str.Length > 1)
                str = str.Substring(0, str.Length - 1);
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

        private static IEnumerable<PokemonData>  getPokeAttackers(IEnumerable<PokemonData> pokemons, PokemonData defender, int attackers = 4)
        {
            var filter1 = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Stamina > 0)));
            
            if (attackers == 0) {
                var rnd = new Random();
                return filter1.OrderBy(x => rnd.Next()).Take(6);
            }
            if (attackers == 1) {
                return  filter1.OrderByDescending(x => x.Cp).Take(6);
            }
            if (attackers == 2) {
                return filter1.OrderByDescending(x => x.Favorite).ThenByDescending(x => x.Cp).Take(6);
            }
            if (attackers == 3) {
                var filteratt = filter1.Where(x => x.Cp < defender.Cp).OrderByDescending(x => x.Cp).Take(6);
                if (filteratt.Count() < 6) {
                    var left = 6 - filteratt.Count();
                    filteratt = filteratt.Concat(filter1.Except(filteratt).OrderBy(x => x.Cp).Take(left));
                }
                return filteratt;
            }
            var filter2 = filter1.OrderByDescending(x => x.Cp);
            if (attackers == 5) {
                filter2 = filter1.Where(x => x.Cp < defender.Cp).OrderByDescending(x => x.Cp);
            }

            if (defender.PokemonId == PokemonId.Blissey)
                filter2 = filter2.Where(x => AttackersToNormalTypes.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            if (defender.PokemonId == PokemonId.Snorlax)
                filter2 = filter2.Where(x => AttackersToNormalTypes.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            if (defender.PokemonId == PokemonId.Gyarados)
                filter2 = filter2.Where(x => Electric.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            if (defender.PokemonId == PokemonId.Rhydon)
                filter2 = filter2.Where(x => Grass.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            var filter3 = filter2.Take(6);
            if (filter3.Count() < 6) {
                var left = 6 - filter3.Count();
                filter1 = filter1.Except(filter3);
                filter3 = filter3.Concat(filter1.OrderByDescending(x => x.Cp).Take(left));
            }

            return filter3;
        }

        private static bool CheckAndPutInNearbyGym(FortData gym, Client client)
        {

            if (!GlobalVars.Gyms.Farm)
                return false;

            if (gymsVisited.IndexOf(gym.Id) > -1)
                return false;


            Logger.Debug("(Gym) - Reviving pokemons.");
            ReviveAndCurePokemons(client);
            var pokemons = (client.Inventory.GetPokemons()).ToList();

            RandomHelper.RandomSleep(200);

            var buddyID = 0UL;
            if (client.Player.PlayerResponse.PlayerData.BuddyPokemon != null)
                buddyID = client.Player.PlayerResponse.PlayerData.BuddyPokemon.Id;

            PokemonData pokemon = getPokeToPut(client, buddyID, gym.GuardPokemonCp);

            if (pokemon == null) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are no pokemons to assign.");
                return false;
            }

            Logger.Debug("(Gym) - Pokemon to deploy: " + strPokemon(pokemon));

            var gymDetails = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Team: " + GetTeamName(gym.OwnedByTeam) + ".");
            
            if (gymDetails.GymStatusAndDefenders!=null && gymDetails.GymStatusAndDefenders.GymDefender != null && gymDetails.GymStatusAndDefenders.GymDefender.Count > 0) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Members: " + gymDetails.GymStatusAndDefenders.GymDefender.Count +$".");
            }
            Logger.Info("(Gym) - Name: " + gymDetails.Name);

            if (gym.OwnedByTeam == TeamColor.Neutral) {
                RandomHelper.RandomSleep(250);
                putInGym(client, gym, pokemon, pokemons);
            } else if ((gym.OwnedByTeam == client.Player.PlayerResponse.PlayerData.Team)) {
                RandomHelper.RandomSleep(250);
                if (gymDetails.GymStatusAndDefenders.GymDefender.Count < 6 ) {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is a free space");
                    putInGym(client, gym, pokemon, pokemons);
                } else {
                    Logger.Warning("(Gym) - There is no free space in the gym");
                    AddVisited(gym.Id, 600000);
                }

            } else {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym is not from your team.");
                if (!GlobalVars.Gyms.Attack) {
                    Logger.Debug("Attack is disabled");
                    AddVisited(gym.Id, 600000);
                    return false;
                }
                if (gymDetails.GymStatusAndDefenders.GymDefender.Count >= 1 && gymDetails.GymStatusAndDefenders.GymDefender.Count <= GlobalVars.Gyms.NumDefenders) {
                    restoreWalkingAfterLogic = !GlobalVars.PauseTheWalking;
                    GlobalVars.PauseTheWalking = true;
                    Logger.Debug("(Gym) - Stop walking ");
                    var defender = gymDetails.GymStatusAndDefenders.GymDefender.FirstOrDefault();
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Defender: " + strPokemon(defender.MotivatedPokemon.Pokemon) +$"[{defender.TrainerPublicProfile.Name} ({defender.TrainerPublicProfile.Level})]");
                    var pokeAttackers = getPokeAttackers(pokemons, defender.MotivatedPokemon.Pokemon, GlobalVars.Gyms.Attackers);
                    if (pokeAttackers.Count() < 6) {
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are not enouth pokemons to fight");
                        return false;
                    }
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Let's go to fight");
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Selected Atackers:");
                    ShowPokemons(pokeAttackers);
                    var attResp = AttackGym(gym, client, pokeAttackers, defender.MotivatedPokemon.Pokemon.Id, gymDetails.GymStatusAndDefenders.GymDefender.Count, buddyID);
                } else {
                    AddVisited(gym.Id, 600000);
                }
            }
            return true;
        }

        private static long GetNextDefender(Google.Protobuf.Collections.RepeatedField<BattleAction> battleActions)
        {
            foreach (var element in battleActions)
                if (element.BattleResults != null)
                if (element.BattleResults.NextDefenderPokemonId != -1)
                    return element.BattleResults.NextDefenderPokemonId;
            return -1L;
        }

        private static AttackGymResponse AttackGym(FortData gym, Client client,
            IEnumerable<PokemonData> pokeAttackers, ulong defenderId, int numDefenders, ulong buddyPokemonId)
        {
            StopAttack = false;
            var pokeAttackersIds = pokeAttackers.Select(x => x.Id);
            var moveSettings = GetMoveSettings(client);
            GymGetInfoResponse gymDetails = null;
            StartGymBattleResponse resp = null;

            // Sometimes we get a null from startgymBattle so we try to start battle 3 times
            var numTries = 3;
            var startOk = false;
            do {
                gymDetails = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
                RandomHelper.RandomSleep(200);
                try {
                    resp = client.Fort.StartGymBattle(gym.Id, defenderId, pokeAttackersIds).Result;
                    if (resp != null && resp.BattleLog != null) {
                        startOk = true;
                    } else {
                        Logger.Info("(Gym) - Start Gym Failed.");
                        Logger.Debug("(Gym) - resp:" + resp);
                    }
                } catch (Exception ex1) {
                    Logger.Info("(Gym) - Start Gym Failed. Reason:" + ex1);
                }
                numTries--;
            } while (!startOk && numTries > 0 && !StopAttack);

            if (!startOk) {
                Logger.Info("(Gym) - Start Gym Failed. After of 3 tries");
                return null;
            }

            if (resp.BattleLog.State == BattleState.Active) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Battle Started");
                RandomHelper.RandomSleep(2000);
                var battleActions = new List<BattleAction>();
                var lastRetrievedAction = new BattleAction();
                var battleStartMs = resp.BattleLog.BattleStartTimestampMs;
                var attResp = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction);
                var inBattle = (attResp.Result == AttackGymResponse.Types.Result.Success);
                inBattle = inBattle && (attResp.BattleLog.State == BattleState.Active);
                var count = 1;
                var energy = 0;
                Logger.Debug("attResp: " + attResp);
                while (inBattle && !StopAttack) {
                    var timeMs = attResp.BattleLog.ServerMs;
                    var move1Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == attResp.ActiveAttacker.PokemonData.Move1).MoveSettings;
                    var move2Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == attResp.ActiveAttacker.PokemonData.Move2).MoveSettings;
                    battleActions = new List<BattleAction>();
                    
                    var baseAction = new BattleAction();
                    baseAction.ActionStartMs = timeMs + RandomNumbers.Next(110, 170);
                    baseAction.TargetIndex = -1;
                    if (attResp.ActiveDefender != null)
                        baseAction.TargetPokemonId = attResp.ActiveDefender.PokemonData.Id;
                    if (attResp.ActiveAttacker.PokemonData.Stamina > 0)
                        baseAction.ActivePokemonId = attResp.ActiveAttacker.PokemonData.Id;
                    // One each ten times we try to evade attack
                    if (RandomNumbers.Next(1, 10) == 1) {
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
                        baseAction.ActionStartMs = specialAttack.ActionStartMs + specialAttack.DurationMs;
                    } else {
                        // One each nine times we do not attack
                        if (RandomNumbers.Next(1, 9) != 1) {
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
                            baseAction.ActionStartMs = normalAttack.ActionStartMs + normalAttack.DurationMs;
                        }
                    }

                    lastRetrievedAction = attResp.BattleLog.BattleActions.LastOrDefault(x => x.ActivePokemonId != attResp.ActiveAttacker.PokemonData.Id);

                    var str = string.Join(",", battleActions);
                    Logger.Debug("(Gym) - battleActions: " + str);
                    attResp = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction);
                    Logger.Debug("attResp: " + attResp);
                    Logger.Debug("attResp BattleActions: ");
                    ShowBattleActions(attResp.BattleLog.BattleActions);
                    inBattle = (attResp.Result == AttackGymResponse.Types.Result.Success);
                    if (inBattle) {

                        inBattle = (attResp.BattleLog.State == BattleState.Active);

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
                        // Wait until all attack are done. but not more than 1.5 seconds.
                        var waitTime = (int)(baseAction.ActionStartMs - attResp.BattleLog.ServerMs);
                        if (waitTime < 0)
                            waitTime = 0;
                        else if (waitTime > 1200)
                            waitTime = 1200;
                        RandomHelper.RandomSleep(waitTime, waitTime + 100);
                    }
                }

                Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Battle Finished in {count} Rounds.");
                if (attResp.Result == AttackGymResponse.Types.Result.Success) {
                    switch (attResp.BattleLog.State) {
                        case BattleState.Defeated:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have lost");
                            if (numDefenders > 1)
                                AddVisited(gym.Id, 3600000);
                            break;
                        case BattleState.Victory:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have won");
                            var nextDefenderID = -1L;
                            foreach (var element in attResp.BattleLog.BattleActions) {
                                if (element.BattleResults != null) {
                                    Logger.Info("(Gym) - Gym points: " + element.BattleResults.GymPointsDelta);
                                    Logger.Info("(Gym) - Experience Awarded: " + element.BattleResults.PlayerExperienceAwarded);
                                    Logger.Debug("(Gym) - Next Pokemon: " + element.BattleResults.NextDefenderPokemonId);
                                    if (element.BattleResults.NextDefenderPokemonId != -1)
                                        nextDefenderID = element.BattleResults.NextDefenderPokemonId;
                                }
                            }
                            if (numDefenders > 1) {
                                Logger.Debug("(Gym) - Leaving Battle");
                                var times = 3;
                                do {
                                    attResp = LeaveBattle(gym, client, resp, attResp, lastRetrievedAction, nextDefenderID);
                                    times--;
                                } while (attResp.Result != AttackGymResponse.Types.Result.Success && times > 0);
                                const int secondsBetweenAttacks = 300;
                                Logger.Info($"Waiting {secondsBetweenAttacks} seconds before of a new battle.");
                                for (var i = 0; i < secondsBetweenAttacks + 1; i++) {
                                    if (StopAttack) {
                                        AddVisited(gym.Id, 3600000);
                                        break;
                                    }
                                    if ((i != 0) && (i % 10 == 0))
                                        Logger.Info($"Seconds Left {secondsBetweenAttacks-i}");
                                    if (i % 30 == 0)
                                        Logic.objClient.Map.GetMapObjects().Wait();
                                    Task.Delay(1000).Wait();
                                }
                            } else {
                                ReviveAndCurePokemons(client);
                                var pokemons = (client.Inventory.GetPokemons()).ToList();
                                RandomHelper.RandomSleep(400);
                                gymDetails = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
                                Logger.Debug("(Gym) - Gym Details: " + gymDetails);
                                if (gymDetails.GymStatusAndDefenders.GymDefender.Count < 1) {
                                    putInGym(client, gym, getPokeToPut(client, buddyPokemonId, gym.GuardPokemonCp), pokemons);
                                }
                            }
                            break;
                        case BattleState.TimedOut:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Timed Out");
                            AddVisited(gym.Id, 1800000);
                            break;
                    }
                } else {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Battle Failed: " + attResp.Result);
                }
                return attResp;
            }
            return null;
        }
        private static void ShowBattleActions(IEnumerable<BattleAction> actions)
        {
            Logger.Debug("Actions: " + actions.Count());
            var i = 1;
            foreach (var element in actions) {
                Logger.Debug($"Action {i}: {element}");
                i++;
            }
        }

        public static StartGymBattleResponse StartGymBattle(Client client, string gymId, ulong defendingPokemonId,
            IEnumerable<ulong> attackingPokemonIds)
        {
            StartGymBattleResponse resp = null;
            var numTries = 3;
            var startOk = false;
            do {
                try {
                    resp = client.Fort.StartGymBattle(gymId, defendingPokemonId, attackingPokemonIds).Result;
                    if (resp == null) {
                        Logger.Debug("(Gym) - Response to start battle was null.");
                    } else {
                        if (resp.BattleLog == null) {
                            Logger.Debug("(Gym) - BatlleLog to start battle was null");
                        } else {
                            startOk = true;
                            Logger.Debug("StartGymBattle Response:" + resp);
                        }
                    }
                } catch (Exception ex1) {
                    Logger.ExceptionInfo("StartGymBattle: " + ex1);
                    RandomHelper.RandomSleep(5000);
                    if (GlobalVars.Gyms.Testing == "Relogin") {
                        client.Login.DoLogin().Wait();
                    } else if (GlobalVars.Gyms.Testing == "GetPlayer") {
                        client.Player.GetPlayer();
                        RandomHelper.RandomSleep(3000);
                    } else if (GlobalVars.Gyms.Testing == "Wait 2 minutes catching pokemons") {
                        if (GlobalVars.CatchPokemon)
                            Logger.Info("Trying to catch pokemons until next attack");
                        // 0.00001 = 1 meters
                        // http://www.um.es/geograf/sigmur/temariohtml/node6_mn.html
                        //http://gizmodo.com/how-precise-is-one-degree-of-longitude-or-latitude-1631241162
                        var gymloc = new GeoCoordinate(client.CurrentLongitude, client.CurrentLatitude, client.CurrentAltitude);
                        for (var times = 1; times < 5; times++) {
                            var rnd = RandomHelper.GetLongRandom(8, 9) * 0.00001;
                            Logger.Debug("going to 8 meters far of gym");
                            LocationUtils.updatePlayerLocation(client, gymloc.Longitude + rnd, gymloc.Latitude, gymloc.Altitude);
                            RandomHelper.RandomSleep(10000);
                            CatchingLogic.Execute();
                            rnd = RandomHelper.GetLongRandom(8, 9) * 0.00001;
                            Logger.Debug("going to 8 meters far of gym");
                            LocationUtils.updatePlayerLocation(client, gymloc.Longitude + rnd, gymloc.Latitude, gymloc.Altitude);
                            RandomHelper.RandomSleep(10000);
                            CatchingLogic.Execute();
                            Logger.Debug("returning to gym location");
                            // go back
                            LocationUtils.updatePlayerLocation(client, gymloc.Longitude, gymloc.Latitude, gymloc.Altitude);
                            RandomHelper.RandomSleep(10000);
                            CatchingLogic.Execute();
                        }
                        RandomHelper.RandomSleep(2000);
                    } else {
                        RandomHelper.RandomSleep(115000);
                        client.Login.DoLogin().Wait();
                    }
                }
                numTries--;
            } while (!startOk && numTries > 0);
            return resp;
        }

        private static  AttackGymResponse LeaveBattle(FortData gym, Client client, StartGymBattleResponse resp, AttackGymResponse attResp, BattleAction lastRetrievedAction, long nextDefenderID)
        {
            AttackGymResponse ret = attResp;
            var times = 3;
            var exit = false;
            do {
                var timeMs = ret.BattleLog.ServerMs;
                var attack = new BattleAction();
                attack.Type = BattleActionType.ActionPlayerQuit;
                attack.TargetPokemonId = (ulong)nextDefenderID;
                if (attResp.ActiveAttacker != null)
                    attack.ActivePokemonId = attResp.ActiveAttacker.PokemonData.Id;
                var battleActions = new List<BattleAction>();
                battleActions.Add(attack);
                lastRetrievedAction = new BattleAction();
                ret = client.Fort.AttackGym(gym.Id, resp.BattleId, battleActions, lastRetrievedAction);
                Logger.Debug($"ret {times}: {ret}");
                Logger.Debug("ret BattleActions: ");
                ShowBattleActions(attResp.BattleLog.BattleActions);
                times--;
                if (ret.Result == AttackGymResponse.Types.Result.Success) {
                    foreach (var element in  ret.BattleLog.BattleActions) {
                        if (element.Type == BattleActionType.ActionPlayerQuit) {
                            Logger.Info("(Gym) - Gym points: " + element.BattleResults.GymPointsDelta);
                            Logger.Info("(Gym) - Experience Awarded: " + element.BattleResults.PlayerExperienceAwarded);
                            Logger.Debug("(Gym) - Next Pokemon: " + element.BattleResults.NextDefenderPokemonId);
                            exit = true;
                        }
                    }
                }
            } while (!exit && times > 0);

            return ret;
        }

        public static PokemonData getPokeToPut(Client client, ulong buddyPokemon, int minCP)
        {
            var pokemons = (client.Inventory.GetPokemons()).ToList();

            switch (GlobalVars.Gyms.DeployPokemons) {
                case 1:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                        .OrderByDescending(x => x.Cp).FirstOrDefault();
                case 2:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                        .OrderBy(x => x.Cp).FirstOrDefault();
                case 3:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                        .OrderByDescending(x => x.Favorite).ThenByDescending(x => x.Cp).FirstOrDefault();
                case 4:
                    var pok = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax) && (x.Cp > minCP)))
                        .OrderBy(x => x.Cp).FirstOrDefault();
                    if (pok == null)
                        pok = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                             .OrderBy(x => x.Cp).FirstOrDefault();
                    return pok;
            }
            var rnd = new Random();
            return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax)))
                .OrderBy(x => rnd.Next()).FirstOrDefault();

        }

        private static void putInGym(Client client, FortData gym, PokemonData pokemon, IEnumerable<PokemonData> pokemons)
        {
            RandomHelper.RandomSleep(400);
            var fortSearch = client.Fort.FortDeployPokemon(gym.Id, pokemon.Id);
            if (fortSearch.Result == FortDeployPokemonResponse.Types.Result.Success) {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - " + pokemon.PokemonId + " deployed into the gym");
                var pokesInGym = pokemons.Count(x => ((!x.IsEgg) && (x.DeployedFortId != ""))) + 1;
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Pokemons in gyms: " + pokesInGym);
                if (pokesInGym > 9) {
                    var res = client.Player.CollectDailyDefenderBonus();
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
            //var templates = client.Download.GetItemTemplates().Result.ItemTemplates;
            var templates = client.Download.GetItemTemplates().ItemTemplates;
            return templates.Where(x => x.MoveSettings != null);
        }

        private static void ReviveAndCurePokemons(Client client)
        {
            var moreRevives = true;
            try {
                RandomHelper.RandomSleep(7000); // If we don`t wait, getpokemons return null.
                var pokemons = client.Inventory.GetPokemons().Where(x => x.Stamina < x.StaminaMax);
                foreach (var pokemon in pokemons) {
                    if (pokemon.Stamina <= 0) {
                        if (moreRevives) {
                            var revive = GetNextAvailableRevive(client);
                            Logger.Debug("revive:" + revive);
                            if (revive != 0) {
                                RandomHelper.RandomSleep(250);
                                var response = client.Inventory.UseItemRevive(revive, pokemon.Id);
                                if (response.Result == UseItemReviveResponse.Types.Result.Success) {
                                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Pokemon revived: " + pokemon.PokemonId);
                                    if (revive == ItemId.ItemRevive) {
                                        pokemon.Stamina = pokemon.StaminaMax / 2;
                                        CurePokemon(client, pokemon);
                                    } else
                                        pokemon.Stamina = pokemon.StaminaMax;
                                } else
                                    Logger.Debug("Use revive result: " + response.Result);
                            } else {
                                moreRevives = false;
                            }
                        }
                    } else if (pokemon.Stamina < pokemon.StaminaMax)
                        CurePokemon(client, pokemon);
                    
                }
            } catch (Exception e) {
                Logger.ExceptionInfo(e.ToString());
            }
        }

        private static void CurePokemon(Client client, PokemonData pokemon)
        {
            var potion = GetNextAvailablePotion(client);
            var fails = 0;
            Logger.Debug("potion:" + potion);
            while (pokemon.Stamina < pokemon.StaminaMax && potion != 0 && fails < 3) {
                RandomHelper.RandomSleep(2000);
                var response = client.Inventory.UseItemPotion(potion, pokemon.Id);
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
            RandomHelper.RandomSleep(250);
            var count = client.Inventory.GetItemData(ItemId.ItemPotion).Count;
            Logger.Debug("count ItemPotion:" + count);
            if (count > 0)
                return ItemId.ItemPotion;
            count = client.Inventory.GetItemData(ItemId.ItemSuperPotion).Count;
            Logger.Debug("count ItemSuperPotion:" + count);
            if (count > 0)
                return ItemId.ItemSuperPotion;
            count = client.Inventory.GetItemData(ItemId.ItemHyperPotion).Count;
            Logger.Debug("count ItemHyperPotion:" + count);
            if (count > 0)
                return ItemId.ItemHyperPotion;
            count = client.Inventory.GetItemData(ItemId.ItemMaxPotion).Count;
            Logger.Debug("count ItemMaxPotion:" + count);
            return count > 0 ? ItemId.ItemMaxPotion : 0;
        }

        private static ItemId GetNextAvailableRevive(Client client)
        {
            RandomHelper.RandomSleep(250);
            var count = client.Inventory.GetItemData(ItemId.ItemRevive).Count;
            Logger.Debug("count ItemRevive:" + count);
            if (count > 0)
                return ItemId.ItemRevive;
            count = client.Inventory.GetItemData(ItemId.ItemMaxRevive).Count;
            Logger.Debug("count ItemMaxRevive:" + count);
            return count > 0 ? ItemId.ItemMaxRevive : 0;
        }
        
        private static PokemonId[] AttackersToNormalTypes = {
            PokemonId.Machamp,
            PokemonId.Tyranitar,
            PokemonId.Dragonite,
            PokemonId.Heracross,
            PokemonId.Flareon,
            PokemonId.Exeggutor,
            PokemonId.Gyarados 
        };
        private static PokemonId[] Electric = {
            PokemonId.Magneton, PokemonId.Electabuzz, PokemonId.Lanturn,
            PokemonId.Raichu, PokemonId.Electrode, PokemonId.Flaaffy,
            PokemonId.Magnemite, PokemonId.Elekid, PokemonId.Chinchou,
            PokemonId.Mareep, PokemonId.Voltorb, PokemonId.Pikachu,
            PokemonId.Pichu
        };
        private static PokemonId[] Grass = {
            PokemonId.Celebi,
            PokemonId.Exeggutor,
            PokemonId.Venusaur,
            PokemonId.Vileplume,
            PokemonId.Victreebel,
            PokemonId.Meganium,
            PokemonId.Tangela,
            PokemonId.Bellossom,
            PokemonId.Sunflora,
            PokemonId.Parasect,
            PokemonId.Jumpluff,
            PokemonId.Ivysaur,
            PokemonId.Gloom,
            PokemonId.Weepinbell,
            PokemonId.Bayleef,
            PokemonId.Exeggcute,
            PokemonId.Oddish,
            PokemonId.Bulbasaur,
            PokemonId.Bellsprout,
            PokemonId.Skiploom,
            PokemonId.Paras,
            PokemonId.Chikorita,
            PokemonId.Hoppip,
            PokemonId.Sunkern

        };
    }
}
