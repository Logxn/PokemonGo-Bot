/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 18/02/2017
 * Time: 0:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Net;

namespace PokemonGo.RocketAPI.Console.Helper
{
    /// <summary>
    /// Description of DownloadHelper.
    /// </summary>
    public static class DownloadHelper
    {
        const string rootGithubProject = "https://raw.githubusercontent.com/Logxn/PokemonGo-Bot/master/";

        public static void DownloadFile(string remoteDir, string outDir, string name)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            var filename = Path.Combine(outDir, name);
            if (!File.Exists(filename))
            {
                try {
                    using (var wC = new WebClient())
                    {
                         wC.DownloadFile(rootGithubProject+remoteDir+"/"+name,filename);
                         if (File.ReadAllText(filename) == "")
                             File.Delete(filename);
                    }
                } catch  {
                }
            }
        }        
    }
}
