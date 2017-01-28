/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 28/01/2017
 * Time: 18:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PokemonGo.RocketAPI.Logic.Shared;

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using PokemonGo.RocketApi.PokeMap;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Utils;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using Telegram.Bot;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Logic;
using PokemonGo.RocketApi.PokeMap.DataModel;
using System.IO;
using System.Text;
using POGOProtos.Map.Pokemon;
using PokemonGo.RocketAPI.Logic.Functions;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Logic.Shared;
using PokemonGo.RocketAPI.HttpClient;
using System.Net.Http.Headers;
using System.Net.Http;
using POGOProtos.Data;

namespace PokemonGo.RocketAPI.Logic.Functions
{
    /// <summary>
    /// Description of GymsLogic.
    /// </summary>
    public static class GymsLogic
    {
        private static List<string> gymsVisited = new List<string>();
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
        public static void Execute()
        {
            if (!GlobalVars.FarmGyms)
                return;
            //narrow map data to gyms within walking distance
            var gyms = GetNearbyGyms();
            var gymsWithinRangeStanding = gyms.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.objClient.CurrentLatitude, Logic.objClient.CurrentLongitude, i.Latitude, i.Longitude) < 40);

            var withinRangeStandingList = gymsWithinRangeStanding as IList<FortData> ?? gymsWithinRangeStanding.ToList();
            var inRange = withinRangeStandingList.Count;
            if (withinRangeStandingList.Any())
            {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - {inRange} gyms are within range of the user");

                foreach (var gym in withinRangeStandingList)
                {
                    var fortInfo = Logic.objClient.Fort.GetFort(gym.Id, gym.Latitude, gym.Longitude).Result;
                    CheckAndPutInNearbyGym(gym, Logic.objClient, fortInfo);
                    Setout.SetCheckTimeToRun();
                    RandomHelper.RandomSleep(100, 200);
                }
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

        private static bool CheckAndPutInNearbyGym(FortData gym, Client client, FortDetailsResponse fortInfo)
        {
            var gymColorLog = ConsoleColor.DarkGray;

            if (gymsVisited.IndexOf(gym.Id) > -1  ){
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym was already visited.");
                return false;
            }
            if (GlobalVars.FarmGyms)
            {
                var pokemons = (client.Inventory.GetPokemons().Result).ToList();

                PokemonData pokemon;

                if (GlobalVars.LeaveInGyms == 0)
                {
                    var rnd=new Random();
                    pokemon = pokemons.Where(x => ( (!x.IsEgg) && (x.DeployedFortId == "") )).OrderBy(x => rnd.Next()).FirstOrDefault();
                }
                else if (GlobalVars.LeaveInGyms == 1)
                    pokemon = pokemons.Where(x => ( (!x.IsEgg) && (x.DeployedFortId == "") )).OrderByDescending(x => x.Cp).FirstOrDefault();
                else
                    pokemon = pokemons.Where(x => ( (!x.IsEgg) && (x.DeployedFortId == "") )).OrderBy(x => x.Cp).FirstOrDefault();
                
                if (pokemon == null)
                {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are no pokemons to assign.");
                    return false;
                }
                RandomHelper.RandomSleep(100, 200);
                var profile = client.Player.GetPlayer().Result;
                if ( (gym.OwnedByTeam ==  profile.PlayerData.Team) || (gym.OwnedByTeam == POGOProtos.Enums.TeamColor.Neutral ))
                {
                    RandomHelper.RandomSleep(100, 200);
                    var gymDetails = client.Fort.GetGymDetails(gym.Id,gym.Latitude,gym.Longitude).Result;
                    Logger.ColoredConsoleWrite(gymColorLog, "Members: " +gymDetails.GymState.Memberships.Count +". Level: "+ GetGymLevel(gym.GymPoints));
                    if (gymDetails.GymState.Memberships.Count < GetGymLevel(gym.GymPoints))
                    {
                        RandomHelper.RandomSleep(100, 200);
                        var fortSearch = client.Fort.FortDeployPokemon(gym.Id, pokemon.Id).Result;
                        if (fortSearch.Result.ToString().ToLower() == "success" ){
                            Logger.ColoredConsoleWrite(gymColorLog, StringUtils.getPokemonNameByLanguage((PokemonId)pokemon.PokemonId) +" inserted into the gym");
                            gymsVisited.Add(gym.Id);
                            var pokesInGym = pokemons.Where(x => ( (!x.IsEgg) && (x.DeployedFortId != "") )).OrderBy(x => x.Cp).ToList().Count();
                            Logger.ColoredConsoleWrite(gymColorLog, "pokesInGym: "+ pokesInGym);
                            if (pokesInGym >9 )
                            { 
                                var res = client.Player.CollectDailyDefenderBonus().Result;
                                Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Collected: {res.CurrencyAwarded} Coins.");
                            }
                        }
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is no free space in the gym");
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym is not your team.");
                    // TO-DO ATTACK ;)?
                    //var getPokemon = getpokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId != ""))).OrderBy(x => x.Cp);
                    //var getOwnPokemon = client.Inventory.GetPokemons().Result.Where(x => !x.IsEgg).OrderBy(x => x.Cp);

                    //var resp = client.Fort.StartGymBattle(gym.Id, getPokemon, getOwnPokemon)
                    //We need a list for the "getOwnPokemons" that can attack. I think its a max of 6 that can attack. Not sure tho
                }
            }
            return true;
        }
    }
}
