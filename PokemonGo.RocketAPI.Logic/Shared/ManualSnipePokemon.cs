/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 18/01/2017
 * Time: 15:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using POGOProtos.Enums;
using System.Device.Location;

namespace PokemonGo.RocketAPI.Logic.Shared
{

/*	PokemonId? ID = null;
        GeoCoordinate Location = null;
        int secondsSnipe = 6;
        int triesSnipe = 3;
        TODO: implements : IEquatable<ManualSnipePokemon>
*/
	public struct ManualSnipePokemon 
	{
        public PokemonId ID ;
        public GeoCoordinate Location ;
        public int secondsSnipe ;
        public int triesSnipe ;
	}
}
