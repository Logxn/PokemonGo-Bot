/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 18/01/2017
 * Time: 18:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PokeMaster.Logic.Shared
{
    /// <summary>
    /// Description of ProxSettings.
    /// </summary>
    public class ProxySettings
    {
       public bool enabled
        { get; set; }
       public string hostName
        { get; set; }
       public int port
        { get; set; }
       public bool useAuth
        { get; set; }
       public string username
        { get; set; }
       public string password
        { get; set; }
    }
}
