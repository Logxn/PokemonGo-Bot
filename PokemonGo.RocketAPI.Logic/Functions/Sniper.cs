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
using PokemonGo.RocketAPI.Helpers;
using System.Linq;
using PokemonGo.RocketAPI.Logic.Utils;

namespace PokemonGo.RocketAPI.Logic.Functions
{
    /// <summary>
    /// Class to manage logic to snipe pokemons.
    /// </summary>
    public class Sniper
    {
        public Client _client;
        public ISettings _clientSettings;
        private const string LogPrefix = "(SNIPING)";
        private ConsoleColor LogColor = ConsoleColor.Cyan;

        public Sniper(Client client)
        {
            _client = client;
            _clientSettings = client.Settings;
        }
        
        private void SendToLog(string line){
            Logger.ColoredConsoleWrite(LogColor, $"{LogPrefix} {line} ");
        }
        
        public void Execute(  PokemonId pokeid, GeoCoordinate remoteCoords )
        {
            if (_clientSettings == null && _client ==null )
            {
                SendToLog($" client or settings are not set");
                return;
            }
            try
            {
                remoteCoords.Altitude = LocationUtils.getAltidude(remoteCoords.Altitude, remoteCoords.Longitude);
                
                SendToLog($"Trying to capture {pokeid}  at { remoteCoords.Latitude } / {remoteCoords.Longitude}");
                var result = _client.Player.UpdatePlayerLocation(remoteCoords.Latitude, remoteCoords.Longitude, remoteCoords.Altitude).Result;

                SendToLog($"Went to sniping location.");
                SendToLog($"Waiting {_clientSettings.secondsSnipe} seconds for Pokemon to appear...");
                RandomHelper.RandomSleep(_clientSettings.secondsSnipe*1000, _clientSettings.secondsSnipe*1100);

                TrySnipePokemons(pokeid);
                
                SendToLog($"Location after Snipe : {_clientSettings.DefaultLatitude} / {_clientSettings.DefaultLongitude}");
                
                RandomHelper.RandomSleep(20000, 22000);  // Avoid cache after snipe

            }
            catch (Exception ex1)
            {
                Logger.ExceptionInfo(ex1.ToString());
                SendToLog($"Go to {_clientSettings.DefaultLatitude} / {_clientSettings.DefaultLongitude}.");
                var result = _client.Player.UpdatePlayerLocation(
                        _clientSettings.DefaultLatitude,
                        _clientSettings.DefaultLongitude,
                        _clientSettings.DefaultAltitude).Result;
            }
            _clientSettings.ForceSnipe = false;
            _clientSettings.ManualSnipePokemonID = null;
            _clientSettings.ManualSnipePokemonLocation = null;
            _clientSettings.secondsSnipe = 6;
            _clientSettings.triesSnipe = 3;
            
        }
        
        private bool TrySnipePokemons(PokemonId pokeid)
        {
            const bool goBack = true;
            var tries = 1;
            var found = false;

            do{
                var mapObjectsResponse = _client.Map.GetMapObjects(true).Result.Item1;
                var pokemons = mapObjectsResponse.MapCells.SelectMany(i => i.CatchablePokemons).Where(x => x.PokemonId == pokeid);
                SendToLog($"Try {tries} of {_clientSettings.triesSnipe}");
                if (pokemons.Any())
                {
                    var pokemon = pokemons.FirstOrDefault();
                    SendToLog($"Found {pokemons.Count()} catchable Pokemon(s): {StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId)}" );
                    Logic.Instance.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.PokemonId, pokemon.Longitude, pokemon.Latitude, goBack);
                    found = true;
                }
                else
                {
                    SendToLog($"No Pokemon Found!");
                    SendToLog($"Waiting {_clientSettings.secondsSnipe} seconds for Pokemon to appear...");
                    RandomHelper.RandomSleep(_clientSettings.secondsSnipe*1000, _clientSettings.secondsSnipe*1100);
                }
                tries ++;
            
            }while ((tries < _clientSettings.triesSnipe) && !found);
            
            if (!found){
                SendToLog( $"Go to {_clientSettings.DefaultLatitude} / {_clientSettings.DefaultLongitude}.");
                var result = _client.Player.UpdatePlayerLocation(
                        _clientSettings.DefaultLatitude,
                        _clientSettings.DefaultLongitude,
                        _clientSettings.DefaultAltitude).Result;
            }
            return found;
        }

    }
}
