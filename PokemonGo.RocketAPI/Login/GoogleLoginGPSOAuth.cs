using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DankMemes.GPSOAuthSharp;
using Newtonsoft.Json;
using PokemonGo.RocketAPI.Exceptions;
using System.Diagnostics;
using System.Threading;

namespace PokemonGo.RocketAPI.Login
{
    public static class GoogleLoginGPSOAuth
    {
        public static string DoLogin(string username, string password)
        {
            GPSOAuthClient client = new GPSOAuthClient(username, password);
            Dictionary<string, string> response = client.PerformMasterLogin();

            if (response.ContainsValue("NeedsBrowser"))
            {
                Logger.Error("Your Google Account uses 2FA. Create a Password for the Application here:");
                Logger.Error("https://security.google.com/settings/security/apppasswords");
                Logger.Error("And use that for Login with Google.");
                Logger.Error("Opening the Site in 5 Seconds.");
                Thread.Sleep(5000);
                Process.Start("https://security.google.com/settings/security/apppasswords");
                Logger.Error("The Program is now freezed.");
                Thread.Sleep(Timeout.Infinite);
            }

            if (response.ContainsKey("Error"))
                throw new GoogleException(response["Error"]);

            //Todo: captcha/2fa implementation

            if (!response.ContainsKey("Auth"))
                throw new GoogleOfflineException();

            Dictionary<string, string> oauthResponse = client.PerformOAuth(response["Token"],
                "audience:server:client_id:848232511240-7so421jotr2609rmqakceuu1luuq0ptb.apps.googleusercontent.com",
                "com.nianticlabs.pokemongo",
                "321187995bc7cdc2b5fc91b11a96e2baa8602c62");

            if (!oauthResponse.ContainsKey("Auth"))
                throw new GoogleOfflineException();

            return oauthResponse["Auth"];
        }
    }
}
