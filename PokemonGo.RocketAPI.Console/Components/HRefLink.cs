/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 31/01/2017
 * Time: 21:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PokemonGo.RocketAPI.Console.Components
{
    /// <summary>
    /// Description of HRefLink.
    /// </summary>
    public class HRefLink
    {
        public HRefLink(string text, string url)
        {
            Text = text;
            URL = url;
        }

        public String Text {get;set;}
        public String URL {get;set;}
    }
}
