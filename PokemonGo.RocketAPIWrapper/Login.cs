/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/09/2017
 * Time: 1:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using POGOLib.Official.LoginProviders;
using POGOLib.Official.Net;

namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Login.
    /// </summary>
    public class Login
    {
        internal Client client;
        public Login(Client client)
        {
            this.client = client;
        }
        public async Task DoLogin()
        {
            ILoginProvider provider;
            if (client._clientSettings.Username.IndexOf("@", StringComparison.Ordinal) > 0)
                provider = new GoogleLoginProvider(client._clientSettings.Username, client._clientSettings.Password);
            else
                provider = new PtcLoginProvider(client._clientSettings.Username, client._clientSettings.Password);
            client.session = await POGOLib.Official.Net.Authentication.Login.GetSession(provider, client._clientSettings.Latitude, client._clientSettings.Longitude);
        }

    }
}
