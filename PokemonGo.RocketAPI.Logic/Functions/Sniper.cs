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
            if (_botSettings == null && _client ==null )
            {
                SendToLog($" client or settings are not set");
                return;
            }
            try
            {
                remoteCoords.Altitude = LocationUtils.getAltidude(remoteCoords.Altitude, remoteCoords.Longitude);
                
                SendToLog($"Trying to capture {pokeid}  at { remoteCoords.Latitude } / {remoteCoords.Longitude}");
                SendToLog(LocationUtils.FindAddress(remoteCoords.Latitude,remoteCoords.Longitude));
                var result = _client.Player.UpdatePlayerLocation(remoteCoords.Latitude, remoteCoords.Longitude, remoteCoords.Altitude).Result;

                SendToLog($"Went to sniping location.");
                SendToLog($"Waiting {GlobalVars.SnipeOpts.WaitSecond} seconds for Pokemon to appear...");
                RandomHelper.RandomSleep(GlobalVars.SnipeOpts.WaitSecond*1000, GlobalVars.SnipeOpts.WaitSecond*1100);

                TrySnipePokemons(pokeid);
                
                SendToLog($"Location after Snipe : {_botSettings.DefaultLatitude} / {_botSettings.DefaultLongitude}");
                SendToLog(LocationUtils.FindAddress(_botSettings.DefaultLatitude,_botSettings.DefaultLongitude));
                
                RandomHelper.RandomSleep(20000, 22000);  // Avoid cache after snipe

            }
            catch (Exception ex1)
            {
                Logger.ExceptionInfo(ex1.ToString());
                SendToLog($"Go to {_botSettings.DefaultLatitude} / {_botSettings.DefaultLongitude}.");
                var result = _client.Player.UpdatePlayerLocation(
                        _botSettings.DefaultLatitude,
                        _botSettings.DefaultLongitude,
                        _botSettings.DefaultAltitude).Result;
            }
            GlobalVars.SnipeOpts.Enabled = false;
            GlobalVars.SnipeOpts.ID = PokemonId.Missingno;
            GlobalVars.SnipeOpts.Location = null;
            GlobalVars.SnipeOpts.WaitSecond = 6;
            GlobalVars.SnipeOpts.NumTries = 3;
            
        }
        
        private bool TrySnipePokemons(PokemonId pokeid)
        {
            const bool goBack = true;
            var tries = 1;
            var found = false;

            do{
                var mapObjectsResponse = _client.Map.GetMapObjects(true).Result.Item1;
                var pokemons = mapObjectsResponse.MapCells.SelectMany(i => i.CatchablePokemons).Where(x => x.PokemonId == pokeid);
                SendToLog($"Try {tries} of {GlobalVars.SnipeOpts.NumTries}");
                if (pokemons.Any())
                {
                    var pokemon = pokemons.FirstOrDefault();
                    SendToLog($"Found {pokemons.Count()} catchable Pokemon(s): {StringUtils.getPokemonNameByLanguage(_botSettings, pokemon.PokemonId)}" );
                    Logic.Instance.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.PokemonId, pokemon.Longitude, pokemon.Latitude, goBack);
                    found = true;
                }
                else
                {
                    SendToLog($"No Pokemon Found!");
                    SendToLog($"Waiting {GlobalVars.SnipeOpts.WaitSecond} seconds for Pokemon to appear...");
                    RandomHelper.RandomSleep(GlobalVars.SnipeOpts.WaitSecond*1000, GlobalVars.SnipeOpts.WaitSecond*1100);
                }
                tries ++;
            
            }while ((tries < GlobalVars.SnipeOpts.NumTries) && !found);
            
            if (!found){
                SendToLog( $"Go to {_botSettings.DefaultLatitude} / {_botSettings.DefaultLongitude}.");
                var result = _client.Player.UpdatePlayerLocation(
                        _botSettings.DefaultLatitude,
                        _botSettings.DefaultLongitude,
                        _botSettings.DefaultAltitude).Result;
            }
            return found;
        }

    }
}
