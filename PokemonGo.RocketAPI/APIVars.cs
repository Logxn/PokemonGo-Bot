/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 16/02/2017
 * Time: 22:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PokemonGo.RocketAPI
{
    /// <summary>
    /// Description of APIVars.
    /// </summary>
    public class APIVars
    {
        public long Unknown25 { get; set; }
        public string UnknownPtr8Message { get; set; }
        public string ClientVersion { get; set; }
        public uint ClientVersionInt { 
            get { return uint.Parse(ClientVersion.Replace(".", "")); } 
        }
        public APIVars(long uk25, string uk8, string cv)
        {
            Unknown25 = uk25;
            UnknownPtr8Message = uk8;
            ClientVersion = cv;
        }
    }
}
