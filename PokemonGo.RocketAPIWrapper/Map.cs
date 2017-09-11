/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2017
 * Time: 22:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using POGOLib.Official.Net;
using POGOProtos.Map;

namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Map.
    /// </summary>
    public class Map
    {
        internal Session session;
        public Map(Session session)
        {
            this.session = session;
        }
        public RepeatedField<MapCell> GetMapObjects()
        {
            return session.Map.Cells;
        }
    }
}
