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
        public long AndroidUnknown25 { get; set; }
        public string AndroidClientVersion { get; set; }
        public long IOSUnknown25 { get; set; }
        public string IOSClientVersion { get; set; }
        public string UnknownPtr8Message { get; set; }
        public string EndPoint { get; set; }
        public uint AndroidClientVersionInt { 
            get { return uint.Parse(AndroidClientVersion.Replace(".", "")); } 
        }
        public APIVars(long uk25, string cv, long ios_uk25, string ios_cv, string endPoint)
        {
            AndroidUnknown25 = uk25;
            AndroidClientVersion = cv;
            IOSUnknown25 = ios_uk25;
            IOSClientVersion = ios_cv;
            EndPoint = endPoint;
        }
    }
}
