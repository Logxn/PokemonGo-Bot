﻿/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 08/05/2017
 * Time: 1:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PokemonGo.RocketAPI.Encryption
{
    /// <summary>
    /// Description of Rand.
    /// </summary>
    public class Rand
    {
        private long state;

        public Rand(long state)
        {
            this.state = state;
        }

        public byte Next()
        {
            state = (state * 0x41C64E6D) + 0x3039;
            return (byte)((state >> 16) & 0xFF);
        }
    }

}
