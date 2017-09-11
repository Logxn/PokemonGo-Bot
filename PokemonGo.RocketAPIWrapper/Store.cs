/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/09/2017
 * Time: 0:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using POGOLib.Official.Net;

namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Store.
    /// </summary>
    public class Store
    {
        internal Session session;
        public Store(Session session)
        {
            this.session = session;
        }
    }
}
