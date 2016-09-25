/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 25/09/2016
 * Time: 2:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using POGOProtos.Enums;
using System.Globalization;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of PokeImgManager.
    /// </summary>
    public static class PokeImgManager
    {
        public static Bitmap GetPokemonSmallImage(PokemonId pokemon)
        {
            return GetPokemonImagefromResource(pokemon, "20");
        }

        public static Bitmap GetPokemonMediumImage(PokemonId pokemon)
        {
            return GetPokemonImagefromResource(pokemon, "35");
        }

        public static Bitmap GetPokemonLargeImage(PokemonId pokemon)
        {
            return GetPokemonImagefromResource(pokemon, "50");
        }

        public static Bitmap GetPokemonVeryLargeImage(PokemonId pokemon)
        {
            return GetPokemonImagefromResource(pokemon, "200");
        }

        /// <summary>
        /// Gets the pokemon imagefrom resource.
        /// </summary>
        /// <param name="pokemon">The pokemon.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static Bitmap GetPokemonImagefromResource(PokemonId pokemon, string size)
        {
            var resource = PokemonGo.RocketAPI.Console.Properties.PokemonSprites.ResourceManager.GetObject("_" + (int)pokemon + "_" + size, CultureInfo.CurrentCulture);
            if (resource != null && resource is Bitmap)
            {
                return new Bitmap(resource as Bitmap);
            }
            else
                return null;
        }
    }
}
