using POGOProtos.Data;
using POGOProtos.Data.Battle;
using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeMaster.Logic.Functions
{
    public static class GymsLogicAttack
    {
        private static Random RandomNumbers = new Random();
        private static ConsoleColor gymColorLog = ConsoleColor.DarkGray;

        public static int AttackGym(FortData gym, Client client)
        {
            GymsLogic.StopAttack = false;
            GymGetInfoResponse gymGetInfoResponse = null;
            GymStartSessionResponse gymStartSessionResponse = null;

            gymGetInfoResponse = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);

            // Get defenders info
            int currentDefender = 0;
            var defenders = gymGetInfoResponse.GymStatusAndDefenders.GymDefender.Select(x => x.MotivatedPokemon.Pokemon).ToList();
            var defender = gymGetInfoResponse.GymStatusAndDefenders.GymDefender[currentDefender];

            for (int i = 0; i < defenders.Count(); i++)
            {
                Logger.Debug($"(Gym) Defender {i + 1}: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.Pokemon.PokemonId}" +
                    $" | CpNow: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.CpNow}" +
                    $" | MotivationNow: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.MotivationNow}" +
                    $" | Id: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.Pokemon.Id}");
            }

            // Get atackers
            var pokeAttackers = getPokeAttackers(defender.MotivatedPokemon.Pokemon, GlobalVars.Gyms.Attackers);
            var pokeAttackersIds = pokeAttackers.Select(x => x.Id);

            if (pokeAttackers.Count() < 6) return 0;

            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Defender: " + GymsLogic.strPokemon(defender.MotivatedPokemon.Pokemon) + $"[{defender.TrainerPublicProfile.Name} ({defender.TrainerPublicProfile.Level})]");
            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Selected Atackers: ");
            GymsLogic.ShowPokemons(pokeAttackers);

            Logger.Info($"(Gym) {gymGetInfoResponse.Name} => {gym.OwnedByTeam} | {gym.GymPoints} points | {gym.GuardPokemonId} ({gym.GuardPokemonCp} CP)");

            while (currentDefender < defenders.Count())
            {
                try
                {
                    // Initialization of attach agains a defender
                    gymStartSessionResponse = GymBattleStartSession(client, gym, defenders[currentDefender].Id, pokeAttackersIds);

                }
                catch (Exception ex)
                {
                    Logger.ExceptionInfo($"Can't start battle against defender {currentDefender}: " + ex.Message);
                    break;
                }

                if (gymStartSessionResponse == null || gymStartSessionResponse.Battle == null || gymStartSessionResponse.Result != GymStartSessionResponse.Types.Result.Success) return -1;

                // Battle loop
                if (gymStartSessionResponse.Battle.BattleLog.State == BattleState.Active)
                {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Battle Started");
                    RandomHelper.RandomSleep(2000);

                    // Attack each defender
                    BattleState attackDefender = AttackDefender(gymStartSessionResponse, client, gym);
                    if (attackDefender == BattleState.Victory)
                    {
                        currentDefender++;
                    }
                    else return -1;
                }
            }
            return currentDefender;
        }

        private static BattleState AttackDefender(GymStartSessionResponse gymBattleStartResponse, Client client, FortData gym)
        {
            PokemonData Attacker = gymBattleStartResponse.Battle.Attacker.ActivePokemon.PokemonData;
            PokemonData Defender = gymBattleStartResponse.Battle.Defender.ActivePokemon.PokemonData;

            Int64 battleStartMs = gymBattleStartResponse.Battle.BattleLog.BattleStartTimestampMs;

            List<BattleAction> battleActions = new List<BattleAction>();
            BattleAction battleAction = new BattleAction();

            IEnumerable<BattleAction> lastActions = gymBattleStartResponse.Battle.BattleLog.BattleActions.ToList();
            BattleAction lastAction = new BattleAction();

            IEnumerable<DownloadItemTemplatesResponse.Types.ItemTemplate> moveSettings = client.Download.GetItemTemplates().ItemTemplates.Where(x => x.MoveSettings != null);

            Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) Battle against {gymBattleStartResponse.Battle.Defender.ActivePokemon.PokemonData.PokemonId}" +
                $" | CP: {gymBattleStartResponse.Battle.Defender.ActivePokemon.PokemonData.Cp}");

            int AttackerEnergy = 0;

            while (true)
            {
                lastAction = lastActions.LastOrDefault();
                var AttackerMove1Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == gymBattleStartResponse.Battle.Attacker.ActivePokemon.PokemonData.Move1).MoveSettings;
                var AttackerMove2Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == gymBattleStartResponse.Battle.Attacker.ActivePokemon.PokemonData.Move2).MoveSettings;

                battleAction.ActionStartMs = DateTime.UtcNow.ToUnixTime(); //battleStartMs + RandomNumbers.Next(110, 170);
                battleAction.TargetIndex = -1; // What is that?
                battleAction.ActivePokemonId = Attacker.Id;
                battleAction.TargetPokemonId = Defender.Id;

                // Selection of attacks
                // two each ten times we try to evade attack
                if (RandomNumbers.Next(1, 10) <= 3)
                {
                    var dodgeAction = new BattleAction();
                    dodgeAction.Type = BattleActionType.ActionDodge;
                    dodgeAction.ActionStartMs = battleAction.ActionStartMs;
                    dodgeAction.DurationMs = 500;
                    //dodgeAction.EnergyDelta = ??
                    dodgeAction.TargetIndex = battleAction.TargetIndex;
                    dodgeAction.TargetPokemonId = battleAction.TargetPokemonId;
                    dodgeAction.ActivePokemonId = battleAction.ActivePokemonId;
                    battleActions.Add(dodgeAction);
                    Logger.Debug("Dodge Action Added");
                    battleAction.ActionStartMs = dodgeAction.ActionStartMs + dodgeAction.DurationMs;
                }
                if (AttackerEnergy >= Math.Abs(AttackerMove2Settings.EnergyDelta))
                {
                    var specialAttack = new BattleAction();
                    specialAttack.Type = BattleActionType.ActionSpecialAttack;
                    specialAttack.ActionStartMs = battleAction.ActionStartMs;
                    specialAttack.DurationMs = AttackerMove2Settings.DurationMs;
                    specialAttack.EnergyDelta = AttackerMove2Settings.EnergyDelta;
                    specialAttack.TargetIndex = battleAction.TargetIndex;
                    specialAttack.TargetPokemonId = battleAction.TargetPokemonId;
                    specialAttack.ActivePokemonId = battleAction.ActivePokemonId;
                    specialAttack.DamageWindowsStartTimestampMs = specialAttack.ActionStartMs + AttackerMove2Settings.DamageWindowStartMs;
                    specialAttack.DamageWindowsEndTimestampMs = specialAttack.ActionStartMs + AttackerMove2Settings.DamageWindowEndMs;
                    battleActions.Add(specialAttack);
                    Logger.Debug("Special Attack Added");
                    battleAction.ActionStartMs = specialAttack.ActionStartMs + specialAttack.DurationMs;
                }
                else
                {
                    // One each nine times we do not attack
                    if (RandomNumbers.Next(1, 9) != 1)
                    {
                        var normalAttack = new BattleAction();
                        normalAttack.Type = BattleActionType.ActionAttack;
                        normalAttack.ActionStartMs = battleAction.ActionStartMs;
                        normalAttack.TargetIndex = battleAction.TargetIndex;
                        normalAttack.TargetPokemonId = battleAction.TargetPokemonId;
                        normalAttack.ActivePokemonId = battleAction.ActivePokemonId;
                        normalAttack.DurationMs = battleAction.DurationMs;
                        normalAttack.DamageWindowsStartTimestampMs = normalAttack.ActionStartMs + AttackerMove1Settings.DamageWindowStartMs;
                        normalAttack.DamageWindowsEndTimestampMs = normalAttack.ActionStartMs + AttackerMove1Settings.DamageWindowEndMs;
                        normalAttack.EnergyDelta = AttackerMove1Settings.EnergyDelta;
                        battleActions.Add(normalAttack);
                        Logger.Debug("Normal Attack Added");
                        battleAction.ActionStartMs = normalAttack.ActionStartMs + normalAttack.DurationMs;
                    }
                }

                long TimeBeforeAttack = DateTime.UtcNow.ToUnixTime();
                GymBattleAttackResponse attackResponse = client.Fort.GymBattleAttack(gym.Id, gymBattleStartResponse.Battle.BattleId, battleActions, lastAction, battleStartMs);
                long TimeAfterAttack = DateTime.UtcNow.ToUnixTime();

                var AttackDuration = battleActions.Sum(x => x.DurationMs);
                if (battleActions.Any(a => a.Type != BattleActionType.ActionSpecialAttack)) AttackDuration = AttackDuration - (int)(TimeAfterAttack - TimeBeforeAttack);
                if (battleActions.Any(a => a.Type == BattleActionType.ActionSwapPokemon)) AttackDuration = AttackDuration + 2000;
                if (AttackDuration > 0) RandomHelper.RandomSleep(AttackDuration);

                //lastActions = attackResponse.BattleUpdate.BattleLog.BattleActions.LastOrDefault(x => x.ActivePokemonId != attackResponse.BattleUpdate.ActiveAttacker.PokemonData.Id);
                //var timeMs = attackResponse.BattleUpdate.BattleLog.ServerMs;
                //bool inBattle = (attackResponse.Result == GymBattleAttackResponse.Types.Result.Success) && (attackResponse.BattleUpdate.BattleLog.State == BattleState.Active);
                //Logger.Debug("(Gym) - battleActions: " + string.Join(",", battleActions));
                //attackResponse = client.Fort.GymBattleAttack(gym.Id, gymBattleStartResponse.Battle.BattleId, battleActions, lastActions);
                //Logger.Debug("attResp: " + attackResponse);
                //Logger.Debug("attResp BattleActions: " + string.Join(",", attackResponse.BattleUpdate.BattleLog.BattleActions));
                //ShowBattleActions(attackResponse.BattleUpdate.BattleLog.BattleActions);
                if (attackResponse.Result == GymBattleAttackResponse.Types.Result.Success)
                {
                    Defender = attackResponse.BattleUpdate.ActiveDefender?.PokemonData;

                    if (attackResponse.BattleUpdate.BattleLog != null && attackResponse.BattleUpdate.BattleLog.BattleActions.Count > 0)
                    {
                        lastActions = attackResponse.BattleUpdate.BattleLog.BattleActions.OrderBy(o => o.ActionStartMs).Distinct();
                    }
                    battleStartMs = attackResponse.BattleUpdate.BattleLog.ServerMs;

                    switch (attackResponse.BattleUpdate.BattleLog.State)
                    {
                        case BattleState.Active:
                            if (attackResponse.BattleUpdate.ActiveAttacker != null && attackResponse.BattleUpdate.ActiveDefender != null)
                            {
                                AttackerEnergy = attackResponse.BattleUpdate.ActiveAttacker.CurrentEnergy;
                                var AttackerHealth = attackResponse.BattleUpdate.ActiveAttacker.CurrentHealth;
                                var ActiveAttacker = attackResponse.BattleUpdate.ActiveAttacker.PokemonData.PokemonId;
                                var DefenderEnergy = attackResponse.BattleUpdate.ActiveDefender.CurrentEnergy;
                                var DefenderHealth = attackResponse.BattleUpdate.ActiveDefender.CurrentHealth;
                                var ActiveDefender = attackResponse.BattleUpdate.ActiveDefender.PokemonData.PokemonId;

                                Attacker = attackResponse.BattleUpdate.ActiveAttacker.PokemonData;
                                Defender = attackResponse.BattleUpdate.ActiveDefender.PokemonData;

                                Logger.Info($"(Gym) - Attacker: {ActiveAttacker} E={AttackerEnergy}, H={AttackerHealth} | Defender: {ActiveDefender} E={DefenderEnergy}, H={DefenderHealth}");
                            }
                            break;
                        case BattleState.Defeated:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have lost");
                            //if (gymGetInfoResponse.GymStatusAndDefenders.GymDefender.Count > 1)
                            //    GymsLogic.AddVisited(gym.Id, 3600000);
                            //inBattle = false;
                            return BattleState.Defeated;
                        case BattleState.Victory:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Defender " + Defender.PokemonId + " defeated.");
                            foreach (var element in attackResponse.BattleUpdate.BattleLog.BattleActions)
                            {
                                if (element.BattleResults != null)
                                {
                                    Logger.Info("(Gym) - Gym points: " + element.BattleResults.GymPointsDelta);
                                    Logger.Info("(Gym) - Experience Awarded: " + element.BattleResults.PlayerXpAwarded);
                                    Logger.Debug("(Gym) - Next Pokemon: " + element.BattleResults.NextDefenderPokemonId);
                                }
                            }

                            //if (element.BattleResults.NextDefenderPokemonId == 0)
                            //{
                            //    Logger.Debug("(Gym) - Leaving Battle");
                            //    var times = 3;
                            //    do
                            //    {
                            //        attackResponse = LeaveBattle(gym, client, gymBattleStartResponse, attackResponse, lastActions, defender.MotivatedPokemon.Pokemon.Id);
                            //        times--;
                            //    } while (attackResponse.Result != GymBattleAttackResponse.Types.Result.Success && times > 0);
                            //    //const int secondsBetweenAttacks = 60; //300
                            //    //Logger.Info($"Waiting {secondsBetweenAttacks} seconds before of a new battle.");
                            //    //for (var i = 0; i < secondsBetweenAttacks + 1; i++)
                            //    //{
                            //    //    if (GymsLogic.StopAttack)
                            //    //    {
                            //    //        GymsLogic.AddVisited(gym.Id, 3600000);
                            //    //        break;
                            //    //    }
                            //    //    if ((i != 0) && (i % 10 == 0))
                            //    //        Logger.Info($"Seconds Left {secondsBetweenAttacks - i}");
                            //    //    if (i % 30 == 0)
                            //    //        Logic.objClient.Map.GetMapObjects().Wait();
                            //    //    Task.Delay(1000).Wait();
                            //    //}
                            //    GymsLogic.ReviveAndCurePokemons(client);
                            //    var pokemons = (client.Inventory.GetPokemons()).ToList();
                            //    RandomHelper.RandomSleep(400);
                            //    gymGetInfoResponse = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
                            //    //Logger.Debug("(Gym) - Gym Details: " + gymGetInfoResponse);
                            //    if (gymGetInfoResponse.GymStatusAndDefenders.GymDefender.Count < 1)
                            //    {
                            //        GymsLogic.CheckAndPutInNearbyGym(gym, client);
                            //    }
                            //    inBattle = false;
                            //}
                            //else
                            //{

                            //}
                            return BattleState.Victory;
                        case BattleState.TimedOut:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Timed Out");
                            return BattleState.TimedOut;
                        case BattleState.StateUnset:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Status UNSET");
                            return BattleState.StateUnset;
                    }
                }
                else return BattleState.StateUnset;
            }
        }

        private static IEnumerable<PokemonData> getPokeAttackers(PokemonData defender, int AttackersSelectionmode = 4)
        {
            var pokemons = (Logic.objClient.Inventory.GetPokemons()).ToList();

            var filter1 = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Stamina > 0)));

            if (AttackersSelectionmode == 0)
            {
                var rnd = new Random();
                return filter1.OrderBy(x => rnd.Next()).Take(6);
            }

            if (AttackersSelectionmode == 1)
            {
                return filter1.OrderByDescending(x => x.Cp).Take(6);
            }

            if (AttackersSelectionmode == 2)
            {
                return filter1.OrderByDescending(x => x.Favorite).ThenByDescending(x => x.Cp).Take(6);
            }

            if (AttackersSelectionmode == 3)
            {
                var filteratt = filter1.Where(x => x.Cp < defender.Cp).OrderByDescending(x => x.Cp).Take(6);
                if (filteratt.Count() < 6)
                {
                    var left = 6 - filteratt.Count();
                    filteratt = filteratt.Concat(filter1.Except(filteratt).OrderBy(x => x.Cp).Take(left));
                }
                return filteratt;
            }

            var filter2 = filter1.OrderByDescending(x => x.Cp);
            if (AttackersSelectionmode == 5)
            {
                filter2 = filter1.Where(x => x.Cp < defender.Cp).OrderByDescending(x => x.Cp);
            }

            if (defender.PokemonId == PokemonId.Blissey)
                filter2 = filter2.Where(x => GymsLogicData.AttackersToNormalTypes.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            if (defender.PokemonId == PokemonId.Snorlax)
                filter2 = filter2.Where(x => GymsLogicData.AttackersToNormalTypes.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            if (defender.PokemonId == PokemonId.Gyarados)
                filter2 = filter2.Where(x => GymsLogicData.Electric.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            if (defender.PokemonId == PokemonId.Rhydon)
                filter2 = filter2.Where(x => GymsLogicData.Grass.Contains(x.PokemonId)).OrderByDescending(x => x.Cp);

            var filter3 = filter2.Take(6);
            if (filter3.Count() < 6)
            {
                var left = 6 - filter3.Count();
                filter1 = filter1.Except(filter3);
                filter3 = filter3.Concat(filter1.OrderByDescending(x => x.Cp).Take(left));
            }

            return filter3;
        }

        //private static long GetNextDefender(Google.Protobuf.Collections.RepeatedField<BattleAction> battleActions)
        //{
        //    foreach (var element in battleActions)
        //        if (element.BattleResults != null)
        //            if (element.BattleResults.NextDefenderPokemonId != -1)
        //                return element.BattleResults.NextDefenderPokemonId;
        //    return -1L;
        //}

        private static GymStartSessionResponse GymBattleStartSession(Client client, FortData gym, ulong defenderId, IEnumerable<ulong> attackersIDs)
        {
            // Sometimes we get a null from startgymBattle so we try to start battle 3 times
            var numTries = 3;
            GymStartSessionResponse gymStartSessionResponse = null;

            do
            {
                try
                {
                    gymStartSessionResponse = client.Fort.GymStartSession(gym.Id, defenderId, attackersIDs).Result;

                }
                catch (Exception ex)
                {
                    Logger.ExceptionInfo("Exception [gymStartSession]:" + ex.Message);
                    return null;
                }
                switch (gymStartSessionResponse.Result)
                {
                    case GymStartSessionResponse.Types.Result.Unset:
                        Logger.Info("(Gym) Failed with error UNSET");
                        break;
                    case GymStartSessionResponse.Types.Result.Success:
                        return gymStartSessionResponse;
                    case GymStartSessionResponse.Types.Result.ErrorGymNotFound:
                        Logger.Info("(Gym) Failed with error ERROR_GYM_NOT_FOUND");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorGymNeutral:
                        Logger.Info("(Gym) Failed with error ERROR_GYM_NEUTRAL");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorGymWrongTeam:
                        Logger.Info("(Gym) Failed with error ERROR_GYM_WRONG_TEAM");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorGymEmpty:
                        Logger.Info("(Gym) Failed with error ERROR_GYM_EMPTY");
                        // Call to DeployPokemon
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorInvalidDefender:
                        Logger.Info("(Gym) Failed with error ERROR_INVALID_DEFENDER");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorTrainingInvalidAttackerCount:
                        Logger.Info("(Gym) Failed with error ERROR_TRAINING_INVALID_ATTACKER_COUNT");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorAllPokemonFainted:
                        Logger.Info("(Gym) Failed with error ERROR_ALL_POKEMON_FAINTED");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorTooManyBattles:
                        Logger.Info("(Gym) Failed with error ERROR_TOO_MANY_BATTLES");
                        // Set to try later
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorTooManyPlayers:
                        Logger.Info("(Gym) Failed with error ERROR_TOO_MANY_PLAYERS");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorGymBattleLockout:
                        Logger.Info("(Gym) Failed with error ERROR_GYM_BATTLE_LOCKOUT");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorPlayerBelowMinimumLevel:
                        Logger.Info("(Gym) Failed with error ERROR_PLAYER_BELOW_MINIMUM_LEVEL");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorNotInRange:
                        Logger.Info("(Gym) Failed with error ERROR_NOT_IN_RANGE");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorPoiInaccessible:
                        Logger.Info("(Gym) Failed with error ERROR_POI_INACCESSIBLE");
                        break;
                    case GymStartSessionResponse.Types.Result.ErrorRaidActive:
                        Logger.Info("(Gym) Failed with error ERROR_RAID_ACTIVE");
                        // Go to Battle Raid
                        break;
                }
                Logger.Info("(Gym) Start Gym Failed (" + numTries + "): " + gymStartSessionResponse.Result);
                numTries--;
            } while (numTries > 0 && gymStartSessionResponse != null);

            return gymStartSessionResponse;

        }
  
        private static void ShowBattleActions(IEnumerable<BattleAction> actions)
        {
            Logger.Debug("Actions: " + actions.Count());
            var i = 1;
            foreach (var element in actions)
            {
                Logger.Debug($"Action {i}: {element}");
                i++;
            }
        }

        //public static GymStartSessionResponse StartGymBattle(Client client, string gymId, ulong defendingPokemonId,
        //    IEnumerable<ulong> attackingPokemonIds)
        //{
        //    GymStartSessionResponse resp = null;
        //    var numTries = 3;
        //    var startOk = false;
        //    do
        //    {
        //        try
        //        {
        //            resp = client.Fort.GymStartSession(gymId, defendingPokemonId, attackingPokemonIds).Result;
        //            if (resp == null)
        //            {
        //                Logger.Debug("(Gym) - Response to start battle was null.");
        //            }
        //            else
        //            {
        //                if (resp.Battle.BattleLog == null)
        //                {
        //                    Logger.Debug("(Gym) - BatlleLog to start battle was null");
        //                }
        //                else
        //                {
        //                    startOk = true;
        //                    Logger.Debug("StartGymBattle Response:" + resp);
        //                }
        //            }
        //        }
        //        catch (Exception ex1)
        //        {
        //            Logger.ExceptionInfo("StartGymBattle: " + ex1);
        //            RandomHelper.RandomSleep(5000);
        //            if (GlobalVars.Gyms.Testing == "Relogin")
        //            {
        //                client.Login.DoLogin().Wait();
        //            }
        //            else if (GlobalVars.Gyms.Testing == "GetPlayer")
        //            {
        //                client.Player.GetPlayer();
        //                RandomHelper.RandomSleep(3000);
        //            }
        //            else if (GlobalVars.Gyms.Testing == "Wait 2 minutes catching pokemons")
        //            {
        //                if (GlobalVars.CatchPokemon)
        //                    Logger.Info("Trying to catch pokemons until next attack");
        //                // 0.00001 = 1 meters
        //                // http://www.um.es/geograf/sigmur/temariohtml/node6_mn.html
        //                //http://gizmodo.com/how-precise-is-one-degree-of-longitude-or-latitude-1631241162
        //                var gymloc = new GeoCoordinate(client.CurrentLongitude, client.CurrentLatitude, client.CurrentAltitude);
        //                for (var times = 1; times < 5; times++)
        //                {
        //                    var rnd = RandomHelper.GetLongRandom(8, 9) * 0.00001;
        //                    Logger.Debug("going to 8 meters far of gym");
        //                    LocationUtils.updatePlayerLocation(client, gymloc.Longitude + rnd, gymloc.Latitude, gymloc.Altitude);
        //                    RandomHelper.RandomSleep(10000);
        //                    CatchingLogic.Execute();
        //                    rnd = RandomHelper.GetLongRandom(8, 9) * 0.00001;
        //                    Logger.Debug("going to 8 meters far of gym");
        //                    LocationUtils.updatePlayerLocation(client, gymloc.Longitude + rnd, gymloc.Latitude, gymloc.Altitude);
        //                    RandomHelper.RandomSleep(10000);
        //                    CatchingLogic.Execute();
        //                    Logger.Debug("returning to gym location");
        //                    // go back
        //                    LocationUtils.updatePlayerLocation(client, gymloc.Longitude, gymloc.Latitude, gymloc.Altitude);
        //                    RandomHelper.RandomSleep(10000);
        //                    CatchingLogic.Execute();
        //                }
        //                RandomHelper.RandomSleep(2000);
        //            }
        //            else
        //            {
        //                RandomHelper.RandomSleep(115000);
        //                client.Login.DoLogin().Wait();
        //            }
        //        }
        //        numTries--;
        //    } while (!startOk && numTries > 0);
        //    return resp;
        //}

        //private static GymBattleAttackResponse LeaveBattle(FortData gym, Client client, GymStartSessionResponse resp, GymBattleAttackResponse attResp, BattleAction lastRetrievedAction, ulong nextDefenderID)
        //{
        //    GymBattleAttackResponse ret = attResp;
        //    var times = 3;
        //    var exit = false;
        //    do
        //    {
        //        var timeMs = ret.BattleUpdate.BattleLog.ServerMs;
        //        var attack = new BattleAction();
        //        attack.Type = BattleActionType.ActionPlayerQuit;
        //        attack.TargetPokemonId = (ulong)nextDefenderID;
        //        if (attResp.BattleUpdate.ActiveAttacker != null)
        //            attack.ActivePokemonId = attResp.BattleUpdate.ActiveAttacker.PokemonData.Id;
        //        var battleActions = new List<BattleAction>();
        //        battleActions.Add(attack);
        //        lastRetrievedAction = new BattleAction();
        //        ret = client.Fort.GymBattleAttack(gym.Id, resp.Battle.BattleId, battleActions, lastRetrievedAction);
        //        //Logger.Debug($"ret {times}: {ret}");
        //        //Logger.Debug("ret BattleActions: ");
        //        //ShowBattleActions(attResp.BattleUpdate.BattleLog.BattleActions);
        //        times--;
        //        if (ret.Result == GymBattleAttackResponse.Types.Result.Success)
        //        {
        //            foreach (var element in ret.BattleUpdate.BattleLog.BattleActions)
        //            {
        //                if (element.Type == BattleActionType.ActionPlayerQuit)
        //                {
        //                    Logger.Info("(Gym) - Gym points: " + element.BattleResults.GymPointsDelta);
        //                    Logger.Info("(Gym) - Experience Awarded: " + element.BattleResults.PlayerXpAwarded);
        //                    Logger.Debug("(Gym) - Next Pokemon: " + element.BattleResults.NextDefenderPokemonId);
        //                    exit = true;
        //                }
        //            }
        //        }
        //    } while (!exit && times > 0);

        //    return ret;
        //}
    }
}
