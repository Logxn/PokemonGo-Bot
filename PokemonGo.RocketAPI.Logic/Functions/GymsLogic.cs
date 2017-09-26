using System;
using System.Device.Location;
using System.Threading.Tasks;
using POGOProtos.Data.Gym;
using POGOProtos.Networking.Requests.Messages;
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

        public static bool StopAttack = false;

        public static void Execute()
        {
            if (!GlobalVars.Gyms.Farm) {
                Logger.Debug("Farm gyms is not enabled.");
                return;
            }

            //narrow map data to gyms within walking distance
            var gyms = GetNearbyGyms();
            var gymsWithinRangeStanding = gyms.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude) <= 40);
            
            Logger.Debug("gymsWithinRangeStanding: " + gymsWithinRangeStanding.Count() + " (of " + gyms.Count() + ")");

            if (gymsWithinRangeStanding.Any())
            {
                Logger.Debug("(Gym) Reviving pokemons.");
                ReviveAndCurePokemons(Logic.objClient);

                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) {gymsWithinRangeStanding.Count()} gyms are within your range.");

                foreach (FortData gym in gymsWithinRangeStanding)
                {
                    if (gymsVisited.Contains(gym.Id)) {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) This gym was already visited.");
                        continue;
                    }

                    if (gym.RaidInfo != null && !gym.RaidInfo.IsRaidHidden)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) This gym is in Raid mode (still not supported), skipping.");
                        AddVisited(gym.Id);
                        continue;
                    }

                    if (GlobalVars.Gyms.Farm && (gym.OwnedByTeam == TeamColor.Neutral || gym.OwnedByTeam == Logic.objClient.Player.PlayerResponse.PlayerData.Team))
                    {
                        CheckAndPutInNearbyGym(gym, Logic.objClient);

                        // We can add training later
                    }

                    if (GlobalVars.Gyms.Attack && (gym.OwnedByTeam != TeamColor.Neutral && gym.OwnedByTeam != Logic.objClient.Player.PlayerResponse.PlayerData.Team))
                    {
                        var gymDetails = Logic.objClient.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
                        if (gymDetails.GymStatusAndDefenders.GymDefender.Count >= 1 && gymDetails.GymStatusAndDefenders.GymDefender.Count <= GlobalVars.Gyms.NumDefenders)
                        {
                            restoreWalkingAfterLogic = !GlobalVars.PauseTheWalking;
                            GlobalVars.PauseTheWalking = true;

                            Logger.Debug("(Gym) Stop walking.");

                            var attackCount = 1;
                            while (attackCount <= GlobalVars.Gyms.MaxAttacks)
                            {
                                Logger.Debug("(Gym) We can attack this gym. Attack number " + attackCount);
                                BattleState isVictory = GymsLogicAttack.AttackGym(gym, Logic.objClient);
                                switch (isVictory)
                                {
                                    case BattleState.StateUnset:
                                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) NULL detected, something failed");
                                        attackCount = GlobalVars.Gyms.MaxAttacks;
                                        break;
                                    default:
                                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) We have won the attack.");
                                        CheckAndPutInNearbyGym(gym, Logic.objClient);
                                        break;
                                }

                                attackCount++;
                                Logger.Debug("(Gym) Reviving pokemons.");
                                ReviveAndCurePokemons(Logic.objClient);
                            }
                            Logger.Warning($"(Gym) Maximum number of {GlobalVars.Gyms.MaxAttacks} attacks reached. Will be checked after of one minute.");
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) This gym has more than {GlobalVars.Gyms.NumDefenders} defenders, skipping.");
                            continue;
                        }
                    }

                    AddVisited(gym.Id);
                    Setout.SetCheckTimeToRun();
                }
                GlobalVars.PauseTheWalking &= !restoreWalkingAfterLogic;
            }
        }

        #region AuxiliaryFunctions
        public static void ResetVisitedMarks()
        {
            gymsVisited.Clear();
        }

        private static string GetTeamName(TeamColor team)
        {
            switch (team)
            {
                case TeamColor.Red:
                    return "Valor";
                case TeamColor.Yellow:
                    return "Instinct";
                case TeamColor.Blue:
                    return "Mystic";
            }
            return "Neutral";
        }

        public static void AddVisited(string id, int milliseconds = 60000)
        {
            if (!gymsVisited.Contains(id))
            {
                gymsVisited.Add(id);
                Task.Delay(milliseconds)
                    .ContinueWith(t => {
                        if (gymsVisited.Contains(id))
                            gymsVisited.Remove(id);
                    });
            }
        }

        private static void RefreshGymsInMap(FortData[] gyms)
        {
            foreach (FortData gym in gyms) {
                Logic.Instance.infoObservable.PushUpdatePokeGym(gym);
            }
        }

        private static FortData[] GetNearbyGyms()
        {
            LocationUtils.updatePlayerLocation(Logic.objClient, Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, Logic.objClient.CurrentAltitude);
            var mapObjectsResponse = Logic.objClient.Map.GetMapObjects().Result;

            var pokeGyms = mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                .Where(i => i.Type == FortType.Gym )
                .OrderBy(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude))
                .ToArray();

            Task.Factory.StartNew(() => RefreshGymsInMap(pokeGyms));
            return pokeGyms;
        }

        public static string strPokemon(PokemonData pokemon)
        {
            return $"{pokemon.PokemonId.ToString()} (CP: {pokemon.Cp} | HP: {pokemon.Stamina})";
        }

        public static string strPokemons(IEnumerable<GymDefender> pokemons)
        {
            var str = "";
            foreach (GymDefender pokemon in pokemons) {
                str = $"{str}{strPokemon(pokemon.MotivatedPokemon.Pokemon)}\n";
            }
            if (str.Length > 1)
                str = str.Substring(0, str.Length - 1);
            return str;
        }

        public static string strPokemons(IEnumerable<POGOProtos.Data.Gym.GymMembership> pokemons)
        {
            var str = "";
            foreach (GymMembership pokemon in pokemons) {
                str = $"{str}{strPokemon(pokemon.PokemonData)}  [ {pokemon.TrainerPublicProfile.Name} ({pokemon.TrainerPublicProfile.Level}) ]\n";
            }
            if (str.Length > 1)
                str = str.Substring(0, str.Length - 1);
            return str;
        }

        public static void ShowPokemons(IEnumerable<PokemonData> pokeAttackers)
        {
            var str = "";
            foreach (PokemonData pokemon in pokeAttackers) {
                str = $"{str}{strPokemon(pokemon)}, ";
            }
            if (str.Length > 2)
                str = str.Substring(0, str.Length - 2);
            Logger.ColoredConsoleWrite(gymColorLog, "(Gym) " + str);
        }
        #endregion

        #region Deploy Pokemon in Gym
        public static bool CheckAndPutInNearbyGym(FortData gym, Client client)
        {
            var pokemons = (client.Inventory.GetPokemons()).ToList();

            RandomHelper.RandomSleep(200);

            var buddyID = 0UL;
            if (client.Player.PlayerResponse.PlayerData.BuddyPokemon != null)
                buddyID = client.Player.PlayerResponse.PlayerData.BuddyPokemon.Id;

            PokemonData pokemon = getPokeToPut(client, buddyID, gym.GuardPokemonCp);

            if (pokemon == null)
            {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) There are no pokemons to put in gym.");
                return false;
            }

            Logger.Debug("(Gym) Pokemon to deploy: " + strPokemon(pokemon));

            var gymDetails = client.Fort.GymGetInfo(gym.Id, gym.Latitude, gym.Longitude);
            Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) Name: {gymDetails.Name} Team: {GetTeamName(gym.OwnedByTeam)}");

            if (gymDetails.GymStatusAndDefenders != null && gymDetails.GymStatusAndDefenders.GymDefender != null && gymDetails.GymStatusAndDefenders.GymDefender.Count > 0)
            {
                Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) Members: {gymDetails.GymStatusAndDefenders.GymDefender.Count}");
            }

            if (gymDetails.GymStatusAndDefenders.GymDefender.Count < 6)
            {
                RandomHelper.RandomSleep(250);
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) There is a free space");
                DeployPokemonInGym(client, gym, pokemon, pokemons);
            }
            else
            {
                Logger.Warning("(Gym) There is no free space in gym");
                AddVisited(gym.Id);
            }

            return true;
        }

        public static PokemonData getPokeToPut(Client client, ulong buddyPokemon, int minCP)
        {
            var pokemons = (client.Inventory.GetPokemons()).ToList();

            switch (GlobalVars.Gyms.DeployPokemons) {
                case 1:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax) && (x.IsBad == false)))
                        .OrderByDescending(x => x.Cp).FirstOrDefault();
                case 2:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax) && (x.IsBad == false)))
                        .OrderBy(x => x.Cp).FirstOrDefault();
                case 3:
                    return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax) && (x.IsBad == false)))
                        .OrderByDescending(x => x.Favorite).ThenByDescending(x => x.Cp).FirstOrDefault();
                case 4:
                    var pok = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax) && (x.Cp > minCP) && (x.IsBad == false)))
                        .OrderBy(x => x.Cp).FirstOrDefault();
                    if (pok == null)
                        pok = pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax) && (x.IsBad == false)))
                             .OrderBy(x => x.Cp).FirstOrDefault();
                    return pok;
            }
            var rnd = new Random();
            return pokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId == "") && (x.Id != buddyPokemon) && (x.Stamina == x.StaminaMax) && (x.IsBad == false)))
                .OrderBy(x => rnd.Next()).FirstOrDefault();

        }

        public static void DeployPokemonInGym(Client client, FortData gym, PokemonData pokemon, IEnumerable<PokemonData> pokemons)
        {
            RandomHelper.RandomSleep(400);

            GymDeployResponse deployResponse = client.Fort.GymDeployPokemon(gym.Id, pokemon.Id);

            if (deployResponse.Result == GymDeployResponse.Types.Result.Success)
            {
                var pokesInGym = pokemons.Count(x => ((!x.IsEgg) && (x.DeployedFortId != ""))) + 1;
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) " + pokemon.PokemonId + " deployed into Gym (" + pokesInGym + " in total).");
                AddVisited(gym.Id, 3600000);
                return;
            }

            if (deployResponse.Result == GymDeployResponse.Types.Result.ErrorAlreadyHasPokemonOnFort) {
                Logger.Warning("(Gym) You already have a pokemon deployed in this Gym");
                AddVisited(gym.Id, 3600000);
                return;
            }

            if (deployResponse.Result == GymDeployResponse.Types.Result.ErrorTooManyOfSameKind && GlobalVars.Gyms.DeployPokemons >=0)
            {
                Logger.Warning("(Gym) Too many Pokemons of the same kind are already deployed in Gym. Deploying a random Pokemon...");
                var TempDeployPokemonsBackupVar = GlobalVars.Gyms.DeployPokemons;
                GlobalVars.Gyms.DeployPokemons = -1; // -1 indicated we have already tried to do so
                var buddyID = 0UL;
                if (client.Player.PlayerResponse.PlayerData.BuddyPokemon != null)
                    buddyID = client.Player.PlayerResponse.PlayerData.BuddyPokemon.Id;
                DeployPokemonInGym(client, gym, getPokeToPut(client, buddyID, gym.GuardPokemonCp), pokemons);
                GlobalVars.Gyms.DeployPokemons = TempDeployPokemonsBackupVar; // return to original setting
                return;
            }

            if (deployResponse.Result == GymDeployResponse.Types.Result.ErrorRaidActive) // should never happen because we check before
            {
                Logger.Warning("(Gym) A RAID is currently active, we cannot deploy a pokemon now.");
                return;
            }
               
            Logger.Debug("(Gym) Error: " + deployResponse.Result);
        }
        #endregion

        #region Revive & Cure
        public static void ReviveAndCurePokemons(Client client)
        {
            try
            {
                //RandomHelper.RandomSleep(7000); // If we don`t wait, getpokemons return null.
                var pokemons = client.Inventory.GetPokemons().Where(x => x.Stamina < x.StaminaMax);

                // Revive & Cure all pokemons which Stamina not maximum
                foreach (var pokemon in pokemons)
                {
                    if (pokemon.Stamina <= 0)
                        RevivePokemon(client, pokemon);
                    else CurePokemon(client, pokemon);
                }
            }
            catch (Exception e)
            {
                Logger.ExceptionInfo(e.ToString());
            }
        }

        private static void RevivePokemon (Client client, PokemonData pokemon)
        {
            ItemId ReviveItem = GetNextAvailableRevive(client);

            if (ReviveItem != ItemId.ItemUnknown)
            {
                RandomHelper.RandomSleep(250);

                UseItemReviveResponse useItemReviveResponse = client.Inventory.UseItemRevive(ReviveItem, pokemon.Id);

                if (useItemReviveResponse.Result == UseItemReviveResponse.Types.Result.Success)
                {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) " + pokemon.PokemonId + " revived with " + ReviveItem + ", with an Stamina of " + useItemReviveResponse.Stamina);
                    if (ReviveItem == ItemId.ItemRevive)
                    {
                        pokemon.Stamina = useItemReviveResponse.Stamina;
                        CurePokemon(client, pokemon);
                    }
                    else
                        pokemon.Stamina = pokemon.StaminaMax;
                }
                else
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "(Gym) Error reviving " + pokemon.PokemonId + ": " + useItemReviveResponse.Result);
            }
            else
            {
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) Cannot revive " + pokemon.PokemonId + ". No more Items Revive/ReviveMax.");
            }
        }

        private static void CurePokemon(Client client, PokemonData pokemon)
        {
            var potion = GetNextAvailablePotion(client);
            var fails = 0;
            Logger.Debug("potion:" + potion);
            while (pokemon.Stamina < pokemon.StaminaMax && potion != ItemId.ItemUnknown && fails < 3) {
                RandomHelper.RandomSleep(2000);
                var response = client.Inventory.UseItemPotion(potion, pokemon.Id);
                if (response.Result == UseItemPotionResponse.Types.Result.Success) {
                    Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) Pokemon {pokemon.PokemonId} cured with {potion}. Stamina: {response.Stamina}/{pokemon.StaminaMax}" );
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
            return count > 0 ? ItemId.ItemMaxPotion : ItemId.ItemUnknown;
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

            return count > 0 ? ItemId.ItemMaxRevive : ItemId.ItemUnknown;
        }
        #endregion

        #region Feed - Not Implemented
        private static void FeedPokemonInGym(Client client, FortData gym, PokemonData pokemon)
        {
            GymFeedPokemonResponse gymFeedPokemonResponse = client.Fort.GymFeedPokemon(gym.Id, pokemon.Id);

        }
        #endregion
    }
}
