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

namespace PokeMaster.Logic.Shared
{

    public class ManualSnipePokemon 
    {
        public bool Enabled
        { get; set; }
        public PokemonId ID 
        { get; set; }
        public GeoCoordinate Location
        { get; set; }
        public int WaitSecond
        { get; set; }
        public int NumTries
        { get; set; }
        public bool TransferIt
        { get; set; }
        public bool UsePinap 
        { get; set; }
    }
}