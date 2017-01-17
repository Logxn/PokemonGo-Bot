/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/01/2017
 * Time: 21:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using Newtonsoft.Json;
using System.IO;

namespace PokemonGo.RocketAPI.Shared
{
    /// <summary>
    /// Description of KeyCollection.
    /// </summary>
    public static class KeyCollection
    {
        private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        private static string filename = Path.Combine(path, "keys.json");
        
        private static Queue keys;
        public static void Save()
        {
            string strJSON = JsonConvert.SerializeObject(keys,Formatting.Indented);
            File.WriteAllText(filename,strJSON);
        }
        public static void Load()
        {
            var strJSON = File.ReadAllText(filename);
            keys = JsonConvert.DeserializeObject(strJSON) as Queue;
        }
        public static string nextKey(){
            if ((keys !=null) && (keys.Count > 0))
            {
                var str = keys.Dequeue() as string;
                keys.Enqueue(str);
                return str;
            }
            return "";
        }
        public static void addKey(string str){
            if (keys == null)
                keys = new Queue();
            keys.Enqueue(str);
        }
        
    }
}
