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
using System.Net;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of PokeImgManager.
    /// </summary>
    public static class PokeImgManager
    {
        public static string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sprites");
        
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
            if (PokemonGo.RocketAPI.Logic.Shared.GlobalVars.UseSpritesFolder)
            {
                var bmp = new Bitmap(1,1);
                DownloadSprite("PokemonGo.RocketAPI.Console/Sprites",path, ""+(int)pokemon);
                var filename = System.IO.Path.Combine(path, ""+(int)pokemon+".png");
                if (System.IO.File.Exists(filename))
                    bmp = new Bitmap(filename);
                else{
                    filename = System.IO.Path.Combine(path, "0.png");
                    if (System.IO.File.Exists(filename))
                        bmp = new Bitmap(filename);
                
                }
                return new Bitmap(bmp,50,50);
            }
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
        public static void DownloadSprite(string remoteDir, string outDir, string name)
        {
            var resourceName = name + ".png";
            var filename = outDir + "\\" + resourceName;
            if (!System.IO.Directory.Exists(outDir))
                System.IO.Directory.CreateDirectory(outDir);

            if (!System.IO.File.Exists(filename))
            {
                try {
                    using (var wC = new WebClient())
                    {
                         wC.DownloadFile("https://raw.githubusercontent.com/Logxn/PokemonGo-Bot/master/"+remoteDir+"/"+resourceName,filename);
                         //wC.DownloadFile("https://veekun.com/dex/media/pokemon/global-link/"+resourceName,filename);
                         //wC.DownloadFile("https://df48mbt4ll5mz.cloudfront.net/images/pokemon/"+resourceName,filename);

                         if (System.IO.File.ReadAllText(filename) == "")
                             System.IO.File.Delete(filename);
                    }
                } catch  {
                }
            }


        }
    }
}
