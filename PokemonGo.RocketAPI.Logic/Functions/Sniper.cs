/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 16/01/2017
 * Time: 22:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Device.Location;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Helpers;
using System.Linq;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Logic.Shared;

namespace PokemonGo.RocketAPI.Logic.Functions
{
    /// <summary>
    /// Class to manage logic to snipe pokemons.
    /// </summary>
    public class Sniper
    {
        public Client _client;
        public ISettings _botSettings;
        private const string LogPrefix = "(SNIPING)";
        private ConsoleColor LogColor = ConsoleColor.Cyan;

        public Sniper(Client client, ISettings ClientSettings)
        {
            _client = client;
            _botSettings = ClientSettings;
        }
        
        private void SendToLog(string line){
            Logger.ColoredConsoleWrite(LogColor, $"{LogPrefix} {line} ");
        }
        
        public void Execute(  PokemonId pokeid, GeoCoordinate remoteCoords )
        {
            GeoCoordinate returnCoords = new GeoCoordinate();

            returnCoords.Latitude = _client.CurrentLatitude;
            returnCoords.Longitude = _client.CurrentLongitude;
            returnCoords.Altitude = _client.CurrentAltitude;
            var result = _client.Player.UpdatePlayerLocation(returnCoords.Latitude, returnCoords.Longitude, returnCoords.Altitude).Result;

            if (_botSettings == null && _client ==null )
            {
                SendToLog($" client or settings are not set");
                return;
            }

            try
            {
                remoteCoords.Altitude = LocationUtils.getAltitude(remoteCoords.Latitude, remoteCoords.Longitude);
                
                SendToLog($"Trying to capture {pokeid}  at { remoteCoords.Latitude } / {remoteCoords.Longitude}");
                SendToLog(LocationUtils.FindAddress(remoteCoords.Latitude,remoteCoords.Longitude));
                result = _client.Player.UpdatePlayerLocation(remoteCoords.Latitude, remoteCoords.Longitude, remoteCoords.Altitude).Result;

                SendToLog($"We are at sniping location...");
                SendToLog($"Waiting {GlobalVars.SnipeOpts.WaitSecond} seconds for Pokemon to appear...");
                RandomHelper.RandomSleep(GlobalVars.SnipeOpts.WaitSecond*1000, GlobalVars.SnipeOpts.WaitSecond*1100);
                
                var catchedID = TrySnipePokemons(pokeid, remoteCoords, returnCoords);
                if ( (catchedID >0) && GlobalVars.SnipeOpts.TransferIt)
                {
                    var trResult = Logic.objClient.Inventory.TransferPokemon(catchedID).Result;
                    if (trResult.Result == ReleasePokemonResponse.Types.Result.Success)
                    {
                        SendToLog("Pokemon was transfered.");
                        SendToLog("Candies awarded: " + trResult.CandyAwarded );
                    }
                }

                // Restore Position
                result = _client.Player.UpdatePlayerLocation(returnCoords.Latitude, returnCoords.Longitude, returnCoords.Altitude).Result;

                SendToLog($"Location after Snipe : {returnCoords.Latitude} / {returnCoords.Longitude} / {returnCoords.Altitude}");
                SendToLog(LocationUtils.FindAddress(returnCoords.Latitude, returnCoords.Longitude));
                
                RandomHelper.RandomSleep(20000, 22000);  // Avoid cache after snipe

            }
            catch (Exception ex)
            {
                Logger.ExceptionInfo(ex.ToString());
                SendToLog($"Go to {returnCoords.Latitude} / {returnCoords.Longitude} / {returnCoords.Altitude}.");
                result = _client.Player.UpdatePlayerLocation(returnCoords.Latitude, returnCoords.Longitude, returnCoords.Altitude).Result;
            }

            GlobalVars.SnipeOpts.Enabled = false;
            GlobalVars.SnipeOpts.ID = PokemonId.Missingno;
            GlobalVars.SnipeOpts.Location = null;
            GlobalVars.SnipeOpts.WaitSecond = 6;
            GlobalVars.SnipeOpts.NumTries = 3;
        }
        
        private ulong TrySnipePokemons(PokemonId pokeid,GeoCoordinate pokeCoords, GeoCoordinate returnCoords)
        {
            const bool goBack = true;
            var tries = 1;
            var found = false;
            ulong caught = 0;
            const double Epsilon = 0.0000005;

            do{
                SendToLog($"Try {tries} of {GlobalVars.SnipeOpts.NumTries}");
                var mapObjectsResponse = _client.Map.GetMapObjects(true).Result.Item1;
                var pokemons = mapObjectsResponse.MapCells.SelectMany(i => i.CatchablePokemons);
                if (pokemons.Any()){
                    SendToLog($"Found {pokemons.Count()} catchable Pokemon(s)");
                    foreach (var pokemon in pokemons) {
                        Logger.Debug("pokemon:" + pokemon);
                        Logger.Debug("pokeCoords:" + pokeCoords);
                        if (Math.Abs(pokemon.Latitude - pokeCoords.Latitude) < Epsilon && Math.Abs(pokemon.Longitude - pokeCoords.Longitude) < Epsilon){
                            SendToLog($"Found {pokemon.PokemonId} to Snipe");
                            caught = Logic.Instance.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.PokemonId, pokemon.Longitude, pokemon.Latitude, goBack, returnCoords.Latitude, returnCoords.Longitude);
                            found = true;
                            break;
                        }
                    }
                }
                if (!found){
                    SendToLog($"No Pokemon Found!");
                    SendToLog($"Waiting {GlobalVars.SnipeOpts.WaitSecond} seconds for Pokemon to appear...");
                    RandomHelper.RandomSleep(GlobalVars.SnipeOpts.WaitSecond*1000, GlobalVars.SnipeOpts.WaitSecond*1100);
                }
                tries ++;
            
            }while ((tries <= GlobalVars.SnipeOpts.NumTries) && caught==0);

            if (caught != 0) SendToLog($"{StringUtils.getPokemonNameByLanguage(_botSettings, pokeid)} caught!");
            else SendToLog($"{ StringUtils.getPokemonNameByLanguage(_botSettings, pokeid)} not found or caught!");

            return caught;
        }

    }
}
