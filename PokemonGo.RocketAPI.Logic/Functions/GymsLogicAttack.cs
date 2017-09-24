using POGOProtos.Data;
using POGOProtos.Data.Battle;
using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using PokemonGo.RocketAPI;
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

        #region Attack
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

        private static long GetNextDefender(Google.Protobuf.Collections.RepeatedField<BattleAction> battleActions)
        {
            foreach (var element in battleActions)
                if (element.BattleResults != null)
                    if (element.BattleResults.NextDefenderPokemonId != -1)
                        return element.BattleResults.NextDefenderPokemonId;
            return -1L;
        }

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

        public static GymBattleAttackResponse AttackGym(FortData gym, Client client)
        {
            GymsLogic.StopAttack = false;
            GymGetInfoResponse gymGetInfoResponse = null;
            GymStartSessionResponse gymStartSessionResponse = null;

            gymGetInfoResponse = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
            int numOfDefenders = gymGetInfoResponse.GymStatusAndDefenders.GymDefender.Count();
            int currentDefender = 0;

            for(int i=0; i < numOfDefenders; i++)
            {
                Logger.Debug($"(Gym) Defender {i + 1}: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.Pokemon.PokemonId}" +
                    $" | CpNow: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.CpNow}" +
                    $" | MotivationNow: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.MotivationNow}" +
                    $" | Id: {gymGetInfoResponse.GymStatusAndDefenders.GymDefender[i].MotivatedPokemon.Pokemon.Id}");
            }

            var defender = gymGetInfoResponse.GymStatusAndDefenders.GymDefender[currentDefender];

            var pokeAttackers = getPokeAttackers(defender.MotivatedPokemon.Pokemon, GlobalVars.Gyms.Attackers);
            var pokeAttackersIds = pokeAttackers.Select(x => x.Id);
            var moveSettings = client.Download.GetItemTemplates().ItemTemplates.Where(x => x.MoveSettings != null);

            if (pokeAttackers.Count() < 6)
            {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Not enougth pokemons to fight.");
                return null;
            }

            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Defender: " + GymsLogic.strPokemon(defender.MotivatedPokemon.Pokemon) + $"[{defender.TrainerPublicProfile.Name} ({defender.TrainerPublicProfile.Level})]");
            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Selected Atackers: ");
            GymsLogic.ShowPokemons(pokeAttackers);

            Logger.Info($"(Gym) {gymGetInfoResponse.Name} => {gym.OwnedByTeam} | {gym.GymPoints} points | {gym.GuardPokemonId} ({gym.GuardPokemonCp} CP)");

            gymStartSessionResponse = GymBattleStartSession(client, gym, defender.MotivatedPokemon.Pokemon.Id, pokeAttackersIds);

            if (gymStartSessionResponse == null || gymStartSessionResponse.Battle == null) return null;

            if (gymStartSessionResponse.Battle.BattleLog.State == BattleState.Active)
            {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Battle Started");
                RandomHelper.RandomSleep(2000);

                var battleActions = new List<BattleAction>();
                var lastRetrievedAction = new BattleAction();
                var battleStartMs = gymStartSessionResponse.Battle.BattleLog.BattleStartTimestampMs;

                GymBattleAttackResponse attackResponse = client.Fort.GymBattleAttack(gym.Id, gymStartSessionResponse.Battle.BattleId, battleActions, lastRetrievedAction);

                var inBattle = (attackResponse.Result == GymBattleAttackResponse.Types.Result.Success) && (attackResponse.BattleUpdate.BattleLog.State == BattleState.Active);

                var count = 1;
                var AttackerEnergy = 0;

                //Logger.Debug("attResp: " + attackResponse);

                while (inBattle && !GymsLogic.StopAttack)
                {
                    var timeMs = attackResponse.BattleUpdate.BattleLog.ServerMs;
                    var move1Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == attackResponse.BattleUpdate.ActiveAttacker.PokemonData.Move1).MoveSettings;
                    var move2Settings = moveSettings.FirstOrDefault(x => x.MoveSettings.MovementId == attackResponse.BattleUpdate.ActiveAttacker.PokemonData.Move2).MoveSettings;
                    battleActions = new List<BattleAction>();

                    BattleAction battleAction = new BattleAction();

                    battleAction.ActionStartMs = timeMs + RandomNumbers.Next(110, 170);
                    battleAction.TargetIndex = -1; // What is that?

                    if (attackResponse.BattleUpdate.ActiveDefender != null)
                        battleAction.TargetPokemonId = attackResponse.BattleUpdate.ActiveDefender.PokemonData.Id;
                    if (attackResponse.BattleUpdate.ActiveAttacker.PokemonData.Stamina > 0)
                        battleAction.ActivePokemonId = attackResponse.BattleUpdate.ActiveAttacker.PokemonData.Id;

                    // two each ten times we try to evade attack
                    if (RandomNumbers.Next(1, 10) <= 3)
                    {
                        var dodgeAction = new BattleAction();
                        dodgeAction.ActionStartMs = battleAction.ActionStartMs;
                        dodgeAction.TargetIndex = battleAction.TargetIndex;
                        dodgeAction.TargetPokemonId = battleAction.TargetPokemonId;
                        dodgeAction.ActivePokemonId = battleAction.ActivePokemonId;
                        dodgeAction.Type = BattleActionType.ActionDodge;
                        dodgeAction.DurationMs = 500;
                        battleActions.Add(dodgeAction);
                        Logger.Debug("Dodge Action Added");
                        battleAction.ActionStartMs = dodgeAction.ActionStartMs + dodgeAction.DurationMs;
                    }

                    if (AttackerEnergy >= Math.Abs(move2Settings.EnergyDelta))
                    {
                        var specialAttack = new BattleAction();
                        specialAttack.ActionStartMs = battleAction.ActionStartMs;
                        specialAttack.TargetIndex = battleAction.TargetIndex;
                        specialAttack.TargetPokemonId = battleAction.TargetPokemonId;
                        specialAttack.ActivePokemonId = battleAction.ActivePokemonId;
                        specialAttack.Type = BattleActionType.ActionSpecialAttack;
                        specialAttack.DurationMs = move2Settings.DurationMs;
                        specialAttack.DamageWindowsStartTimestampMs = specialAttack.ActionStartMs + move2Settings.DamageWindowStartMs;
                        specialAttack.DamageWindowsEndTimestampMs = specialAttack.ActionStartMs + move2Settings.DamageWindowEndMs;
                        specialAttack.EnergyDelta = move2Settings.EnergyDelta;
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
                            normalAttack.ActionStartMs = battleAction.ActionStartMs;
                            normalAttack.TargetIndex = battleAction.TargetIndex;
                            normalAttack.TargetPokemonId = battleAction.TargetPokemonId;
                            normalAttack.ActivePokemonId = battleAction.ActivePokemonId;
                            normalAttack.Type = BattleActionType.ActionAttack;
                            normalAttack.DurationMs = battleAction.DurationMs;
                            normalAttack.DamageWindowsStartTimestampMs = normalAttack.ActionStartMs + move1Settings.DamageWindowStartMs;
                            normalAttack.DamageWindowsEndTimestampMs = normalAttack.ActionStartMs + move1Settings.DamageWindowEndMs;
                            normalAttack.EnergyDelta = move1Settings.EnergyDelta;
                            battleActions.Add(normalAttack);
                            Logger.Debug("Normal Attack Added");
                            battleAction.ActionStartMs = normalAttack.ActionStartMs + normalAttack.DurationMs;
                        }
                    }

                    lastRetrievedAction = attackResponse.BattleUpdate.BattleLog.BattleActions.LastOrDefault(x => x.ActivePokemonId != attackResponse.BattleUpdate.ActiveAttacker.PokemonData.Id);

                    //Logger.Debug("(Gym) - battleActions: " + string.Join(",", battleActions));
                    attackResponse = client.Fort.GymBattleAttack(gym.Id, gymStartSessionResponse.Battle.BattleId, battleActions, lastRetrievedAction);
                    //Logger.Debug("attResp: " + attackResponse);
                    //Logger.Debug("attResp BattleActions: " + string.Join(",", attackResponse.BattleUpdate.BattleLog.BattleActions));
                    //ShowBattleActions(attackResponse.BattleUpdate.BattleLog.BattleActions);
                    inBattle = (attackResponse.Result == GymBattleAttackResponse.Types.Result.Success);

                    switch (attackResponse.BattleUpdate.BattleLog.State)
                    {
                        case BattleState.Active:
                            inBattle = (attackResponse.BattleUpdate.BattleLog.State == BattleState.Active);

                            if (attackResponse.BattleUpdate.ActiveAttacker != null && attackResponse.BattleUpdate.ActiveDefender != null)
                            {
                                AttackerEnergy = attackResponse.BattleUpdate.ActiveAttacker.CurrentEnergy;
                                var AttackerHealth = attackResponse.BattleUpdate.ActiveAttacker.CurrentHealth;
                                var ActiveAttacker = attackResponse.BattleUpdate.ActiveAttacker.PokemonData.PokemonId;
                                var DefenderEnergy = attackResponse.BattleUpdate.ActiveDefender.CurrentEnergy;
                                var DefenderHealth = attackResponse.BattleUpdate.ActiveDefender.CurrentHealth;
                                var ActiveDefender = attackResponse.BattleUpdate.ActiveDefender.PokemonData.PokemonId;

                                Logger.Info($"(Gym) - Attacker: {ActiveAttacker} E={AttackerEnergy}, H={AttackerHealth} | Defender: {ActiveDefender} E={DefenderEnergy}, H={DefenderHealth}");
                                //                            Logger.Info($"(Gym) - Defender: {ActiveDefender} Energy={DefenderEnergy}, Health={DefenderHealth}");
                            }
                            count++;
                            // Wait until all attack are done. but not more than 1.5 seconds.
                            var waitTime = (int)(battleAction.ActionStartMs - attackResponse.BattleUpdate.BattleLog.ServerMs);
                            if (waitTime < 0)
                                waitTime = 0;
                            else if (waitTime > 1200)
                                waitTime = 1200;
                            RandomHelper.RandomSleep(waitTime, waitTime + 100);
                            break;
                        case BattleState.Defeated:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have lost");
                            if (gymGetInfoResponse.GymStatusAndDefenders.GymDefender.Count > 1)
                                GymsLogic.AddVisited(gym.Id, 3600000);
                            inBattle = false;
                            break;
                        case BattleState.Victory:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - We have won");
                            foreach (var element in attackResponse.BattleUpdate.BattleLog.BattleActions)
                            {
                                if (element.BattleResults != null)
                                {
                                    Logger.Info("(Gym) - Gym points: " + element.BattleResults.GymPointsDelta);
                                    Logger.Info("(Gym) - Experience Awarded: " + element.BattleResults.PlayerXpAwarded);
                                    Logger.Debug("(Gym) - Next Pokemon: " + element.BattleResults.NextDefenderPokemonId + " | " + gymGetInfoResponse.GymStatusAndDefenders.GymDefender[currentDefender+1]);
                                    currentDefender++;
                                    defender = gymGetInfoResponse.GymStatusAndDefenders.GymDefender[currentDefender];
                                    gymStartSessionResponse = GymBattleStartSession(client, gym, defender.MotivatedPokemon.Pokemon.Id, pokeAttackersIds);
                                    if (gymStartSessionResponse == null || gymStartSessionResponse.Battle == null) return null;
                                }
                            }

                            if (defender.MotivatedPokemon.Pokemon.Id == 0)
                            {
                                Logger.Debug("(Gym) - Leaving Battle");
                                var times = 3;
                                do
                                {
                                    attackResponse = LeaveBattle(gym, client, gymStartSessionResponse, attackResponse, lastRetrievedAction, defender.MotivatedPokemon.Pokemon.Id);
                                    times--;
                                } while (attackResponse.Result != GymBattleAttackResponse.Types.Result.Success && times > 0);
                                //const int secondsBetweenAttacks = 60; //300
                                //Logger.Info($"Waiting {secondsBetweenAttacks} seconds before of a new battle.");
                                //for (var i = 0; i < secondsBetweenAttacks + 1; i++)
                                //{
                                //    if (GymsLogic.StopAttack)
                                //    {
                                //        GymsLogic.AddVisited(gym.Id, 3600000);
                                //        break;
                                //    }
                                //    if ((i != 0) && (i % 10 == 0))
                                //        Logger.Info($"Seconds Left {secondsBetweenAttacks - i}");
                                //    if (i % 30 == 0)
                                //        Logic.objClient.Map.GetMapObjects().Wait();
                                //    Task.Delay(1000).Wait();
                                //}
                                GymsLogic.ReviveAndCurePokemons(client);
                                var pokemons = (client.Inventory.GetPokemons()).ToList();
                                RandomHelper.RandomSleep(400);
                                gymGetInfoResponse = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
                                //Logger.Debug("(Gym) - Gym Details: " + gymGetInfoResponse);
                                if (gymGetInfoResponse.GymStatusAndDefenders.GymDefender.Count < 1)
                                {
                                    GymsLogic.CheckAndPutInNearbyGym(gym, client);
                                }
                                inBattle = false;
                            }
                            else
                            {

                            }
                            break;
                        case BattleState.TimedOut:
                            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Timed Out");
                            inBattle = false;
                            //GymsLogic.AddVisited(gym.Id, 1800000);
                            break;
                    }
                }

                Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Battle Finished in {count} Rounds.");
                if (attackResponse.Result == GymBattleAttackResponse.Types.Result.Success)
                {
                    
                }
                else
                {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - Battle Failed: " + attackResponse.Result);
                }

                return attackResponse;
            }
            return null;
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

        public static GymStartSessionResponse StartGymBattle(Client client, string gymId, ulong defendingPokemonId,
            IEnumerable<ulong> attackingPokemonIds)
        {
            GymStartSessionResponse resp = null;
            var numTries = 3;
            var startOk = false;
            do
            {
                try
                {
                    resp = client.Fort.GymStartSession(gymId, defendingPokemonId, attackingPokemonIds).Result;
                    if (resp == null)
                    {
                        Logger.Debug("(Gym) - Response to start battle was null.");
                    }
                    else
                    {
                        if (resp.Battle.BattleLog == null)
                        {
                            Logger.Debug("(Gym) - BatlleLog to start battle was null");
                        }
                        else
                        {
                            startOk = true;
                            Logger.Debug("StartGymBattle Response:" + resp);
                        }
                    }
                }
                catch (Exception ex1)
                {
                    Logger.ExceptionInfo("StartGymBattle: " + ex1);
                    RandomHelper.RandomSleep(5000);
                    if (GlobalVars.Gyms.Testing == "Relogin")
                    {
                        client.Login.DoLogin().Wait();
                    }
                    else if (GlobalVars.Gyms.Testing == "GetPlayer")
                    {
                        client.Player.GetPlayer();
                        RandomHelper.RandomSleep(3000);
                    }
                    else if (GlobalVars.Gyms.Testing == "Wait 2 minutes catching pokemons")
                    {
                        if (GlobalVars.CatchPokemon)
                            Logger.Info("Trying to catch pokemons until next attack");
                        // 0.00001 = 1 meters
                        // http://www.um.es/geograf/sigmur/temariohtml/node6_mn.html
                        //http://gizmodo.com/how-precise-is-one-degree-of-longitude-or-latitude-1631241162
                        var gymloc = new GeoCoordinate(client.CurrentLongitude, client.CurrentLatitude, client.CurrentAltitude);
                        for (var times = 1; times < 5; times++)
                        {
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
                    }
                    else
                    {
                        RandomHelper.RandomSleep(115000);
                        client.Login.DoLogin().Wait();
                    }
                }
                numTries--;
            } while (!startOk && numTries > 0);
            return resp;
        }

        private static GymBattleAttackResponse LeaveBattle(FortData gym, Client client, GymStartSessionResponse resp, GymBattleAttackResponse attResp, BattleAction lastRetrievedAction, ulong nextDefenderID)
        {
            GymBattleAttackResponse ret = attResp;
            var times = 3;
            var exit = false;
            do
            {
                var timeMs = ret.BattleUpdate.BattleLog.ServerMs;
                var attack = new BattleAction();
                attack.Type = BattleActionType.ActionPlayerQuit;
                attack.TargetPokemonId = (ulong)nextDefenderID;
                if (attResp.BattleUpdate.ActiveAttacker != null)
                    attack.ActivePokemonId = attResp.BattleUpdate.ActiveAttacker.PokemonData.Id;
                var battleActions = new List<BattleAction>();
                battleActions.Add(attack);
                lastRetrievedAction = new BattleAction();
                ret = client.Fort.GymBattleAttack(gym.Id, resp.Battle.BattleId, battleActions, lastRetrievedAction);
                //Logger.Debug($"ret {times}: {ret}");
                //Logger.Debug("ret BattleActions: ");
                //ShowBattleActions(attResp.BattleUpdate.BattleLog.BattleActions);
                times--;
                if (ret.Result == GymBattleAttackResponse.Types.Result.Success)
                {
                    foreach (var element in ret.BattleUpdate.BattleLog.BattleActions)
                    {
                        if (element.Type == BattleActionType.ActionPlayerQuit)
                        {
                            Logger.Info("(Gym) - Gym points: " + element.BattleResults.GymPointsDelta);
                            Logger.Info("(Gym) - Experience Awarded: " + element.BattleResults.PlayerXpAwarded);
                            Logger.Debug("(Gym) - Next Pokemon: " + element.BattleResults.NextDefenderPokemonId);
                            exit = true;
                        }
                    }
                }
            } while (!exit && times > 0);

            return ret;
        }
        #endregion
    }
}
