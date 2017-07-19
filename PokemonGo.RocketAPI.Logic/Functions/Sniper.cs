/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 16/01/2017
 * Time: 22:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Device.Location;
using POGOProtos.Enums;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using System.Linq;
using PokeMaster.Logic.Utils;
using PokeMaster.Logic.Shared;

namespace PokeMaster.Logic.Functions
{
    /// <summary>
    /// Class to manage logic to snipe pokemons.
    /// </summary>
    public class Sniper
    {
        public Client _client;
        public PokeMaster.Logic.Shared.ISettings _botSettings;
        private const string LogPrefix = "(SNIPING)";
        private ConsoleColor LogColor = ConsoleColor.Cyan;
        

        public Sniper(Client client, PokeMaster.Logic.Shared.ISettings ClientSettings)
        {
            _client = client;
            _botSettings = ClientSettings;
        }
        
        private void SendToLog(string line)
        {
            Logger.ColoredConsoleWrite(LogColor, $"{LogPrefix} {line} ");
        }
        
        public void Execute(PokemonId pokeid, GeoCoordinate remoteCoords)
        {
            var returnCoords = new GeoCoordinate();

            returnCoords.Latitude = _client.CurrentLatitude;
            returnCoords.Longitude = _client.CurrentLongitude;
            returnCoords.Altitude = _client.CurrentAltitude;
            
            var distanceToTarget = LocationUtils.CalculateDistanceInMeters(returnCoords, remoteCoords);
            if (distanceToTarget > 100000){
                Logger.Warning("Distance greater than 100 kms. Skipping Snipe.");
                return;
            }

            
            //LocationUtils.updatePlayerLocation(_client, returnCoords.Latitude, returnCoords.Longitude, returnCoords.Altitude);

            if (_botSettings == null && _client == null) {
                SendToLog($" client or settings are not set");
                return;
            }

            try {
                
                remoteCoords.Altitude = LocationUtils.GetAltitude(remoteCoords.Latitude, remoteCoords.Longitude);

                SendToLog($"Trying to capture {pokeid}  at { remoteCoords.Latitude } / {remoteCoords.Longitude}");
                SendToLog(LocationUtils.FindAddress(remoteCoords.Latitude, remoteCoords.Longitude));
                LocationUtils.updatePlayerLocation(_client, remoteCoords.Latitude, remoteCoords.Longitude, remoteCoords.Altitude, false);
                var gmp = _client.Map.GetMapObjects(true).Result;

                SendToLog($"We are at sniping location...");
                SendToLog($"Waiting {GlobalVars.SnipeOpts.WaitSecond} seconds for Pokemon to appear...");
                RandomHelper.RandomSleep(GlobalVars.SnipeOpts.WaitSecond * 1000);
                
                var catchedID = 0UL;
                if (pokeid == PokemonId.Missingno)
                    catchedID = TrySnipeGym(remoteCoords, returnCoords);
                else{
                    RandomHelper.RandomSleep(1000);
                    catchedID = TrySnipePokemons(pokeid, remoteCoords, returnCoords);
                }

                LocationUtils.updatePlayerLocation(_client, returnCoords.Latitude, returnCoords.Longitude, returnCoords.Altitude);
                gmp = _client.Map.GetMapObjects(true).Result;

                SendToLog($"Location after Snipe : {_client.CurrentLatitude} / {_client.CurrentLongitude} / {_client.CurrentAltitude}");
                SendToLog(LocationUtils.FindAddress(_client.CurrentLatitude, _client.CurrentLongitude));

                if ((catchedID > 0) && GlobalVars.SnipeOpts.TransferIt && pokeid != PokemonId.Missingno) {
                    var trResult = Logic.objClient.Inventory.TransferPokemon(catchedID);
                    if (trResult.Result == ReleasePokemonResponse.Types.Result.Success) {
                        SendToLog("Pokemon was transfered.");
                        SendToLog("Candies awarded: " + trResult.CandyAwarded);
                    }
                }

                RandomHelper.RandomSleep(20000, 22000);  // Avoid cache after snipe

            } catch (Exception ex) {
                Logger.ExceptionInfo(ex.ToString());
                SendToLog($"Go to {returnCoords.Latitude} / {returnCoords.Longitude} / {returnCoords.Altitude}.");
                LocationUtils.updatePlayerLocation(_client, returnCoords.Latitude, returnCoords.Longitude, returnCoords.Altitude);
                var gmp = _client.Map.GetMapObjects(true).Result;
            }

            GlobalVars.SnipeOpts.Enabled = false;
            GlobalVars.SnipeOpts.ID = PokemonId.Missingno;
            GlobalVars.SnipeOpts.Location = null;
            GlobalVars.SnipeOpts.WaitSecond = 6;
            GlobalVars.SnipeOpts.NumTries = 3;
        }
        const double Epsilon = 0.000001;

        private ulong TrySnipeGym(GeoCoordinate pokeCoords, GeoCoordinate returnCoords)
        {
            var found = false;
            var tries = 1;
            ulong caught = 0;
            
            do {

                var mapObjectsResponse = _client.Map.GetMapObjects(true).Result;
                var pokeGyms = mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == POGOProtos.Map.Fort.FortType.Gym);
                Logger.Debug("pokeCoords:" + pokeCoords);
                foreach (var element in pokeGyms) {
                    if (Math.Abs(element.Latitude - pokeCoords.Latitude) < Epsilon && Math.Abs(element.Longitude - pokeCoords.Longitude) < Epsilon) {
                        SendToLog("Found Gym to Snipe");
                        var profile = _client.Player.GetPlayer();
                        var buddyid = 0UL;
                        if (profile.PlayerData.BuddyPokemon != null)
                            buddyid = profile.PlayerData.BuddyPokemon.Id;
                        var gymDet = _client.Fort.GymGetInfo(element.Id, element.Latitude, element.Longitude);
                        if (gymDet.Result == GymGetInfoResponse.Types.Result.Success) {
                            found = true;
                            var pokeToDeploy = GymsLogic.getPokeToPut(_client, buddyid, element.GuardPokemonCp);
                            if (pokeToDeploy != null) {
                                var res = _client.Fort.FortDeployPokemon(element.Id, pokeToDeploy.Id);
                                if (res.Result == FortDeployPokemonResponse.Types.Result.Success) {
                                    caught = pokeToDeploy.Id;
                                    SendToLog(GymsLogic.strPokemon(pokeToDeploy) + " Deployed!");
                                }else{
                                    SendToLog(res.Result.ToString());
                                }
                            }
                            break;
                        }
                    }    
                }
                if (!found) {
                    SendToLog($"No Gym Found!");
                    if ((tries <= GlobalVars.SnipeOpts.NumTries)){
                        SendToLog($"Waiting {GlobalVars.SnipeOpts.WaitSecond} seconds to check again...");
                        RandomHelper.RandomSleep(GlobalVars.SnipeOpts.WaitSecond * 1200);
                    }
                }
                tries++;
                
            } while ((tries <= GlobalVars.SnipeOpts.NumTries) && caught == 0);
            if (caught == 0)
                SendToLog("Gym not found or Pokemon not deployed!");
                
            return caught;
        }

        private bool FindPokemon(PokemonId pokeid, GeoCoordinate pokeCoords,   IEnumerable<MapPokemon> pokemons, out MapPokemon pokemonOUT)
        {
            var found = false;
            pokemonOUT = null;
            foreach (var pokemon in pokemons) {
                Logger.Debug("pokemon:" + pokemon);
                if (AreEquals(pokemon.Latitude, pokeCoords.Latitude)  && AreEquals(pokemon.Longitude , pokeCoords.Longitude) ) {
                    if (pokemon.PokemonId == pokeid || pokemon.PokemonId == PokemonId.Ditto){
                        found = true;
                        pokemonOUT = pokemon;
                        break;
                    }
                }
            }
            return found;
        }

        private bool AreEquals(double a, double b){
            return (Math.Abs(Math.Round(a, 5) - Math.Round(b, 5)) < Epsilon);
        }

        private ulong TrySnipePokemons(PokemonId pokeid, GeoCoordinate pokeCoords, GeoCoordinate returnCoords)
        {
            const bool goBack = false; // since new changes in server before of api 0.63 it must be "false".
            var tries = 1;
            var found = false;
            ulong caught = 0;

            do {
                SendToLog($"Try {tries} of {GlobalVars.SnipeOpts.NumTries}");
                var mapObjectsResponse = _client.Map.GetMapObjects(true).Result;
                var pokemons = mapObjectsResponse.MapCells.SelectMany(i => i.CatchablePokemons);
                if (pokemons.Any()) {
                    SendToLog($"Found {pokemons.Count()} catchable Pokemon(s)");
                    MapPokemon pokemon = null;
                    found = FindPokemon(pokeid,pokeCoords,pokemons,out pokemon);
                    if (found)
                        caught = CatchingLogic.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.PokemonId, pokemon.Longitude, pokemon.Latitude, goBack, returnCoords.Latitude, returnCoords.Longitude);
                }
                tries++;
                if (!found) {
                    SendToLog($"No Pokemon Found!");
                    if ((tries <= GlobalVars.SnipeOpts.NumTries)){
                        SendToLog($"Waiting {GlobalVars.SnipeOpts.WaitSecond} seconds for Pokemon to appear...");
                        RandomHelper.RandomSleep(GlobalVars.SnipeOpts.WaitSecond * 1200);
                    }
                }
            } while ((tries <= GlobalVars.SnipeOpts.NumTries) && !found);

            if (caught != 0)
                SendToLog($"{pokeid} caught!");
            else if (found)
                SendToLog($"{ pokeid} not caught!");

            return caught;
        }

        public static void SendToSnipe( PokemonId pokemon, GeoCoordinate location)
        {
            if (GlobalVars.SnipeOpts.Enabled){
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "There is a Snipe in process.");
                return;
            }
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Manual Snipe Triggered! We'll stop farming and go catch the pokemon ASAP");
            GlobalVars.SnipeOpts.ID = pokemon;
            GlobalVars.SnipeOpts.Location = location;
            GlobalVars.SnipeOpts.WaitSecond = 7;
            GlobalVars.SnipeOpts.NumTries = 3;
            GlobalVars.SnipeOpts.TransferIt = false;
            GlobalVars.SnipeOpts.UsePinap = false;
            GlobalVars.SnipeOpts.Enabled = true;
        }

    }
}
