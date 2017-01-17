/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/01/2017
 * Time: 20:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
namespace PokemonGo.RocketAPI.Exceptions
{
    public class HaserExceptionTooManyRequests :Exception
    {
        public HaserExceptionTooManyRequests(string message): base(message)
        {

        }
    }
}
