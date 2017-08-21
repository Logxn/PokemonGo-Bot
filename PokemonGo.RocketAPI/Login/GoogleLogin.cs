using GPSOAuthSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Login
{
    public class GoogleLogin : ILoginType
    {
        private readonly string password;
        private readonly string email;

        public GoogleLogin(string email, string password)
        {
            this.email = email;
            this.password = password;
        }

        public async Task<string> GetAccessToken()
        {
            var client = new GPSOAuthClient(email, password);
            Dictionary<string, string> response = null;
            try{
                response = await client.PerformMasterLogin().ConfigureAwait(false);
            } catch (NullReferenceException){
                return null;
            }

            if (response.ContainsValue("NeedsBrowser")){
                Logger.Error("Your Google Account uses 2FA. Create a Password for the Application here:");
                Logger.Error("https://security.google.com/settings/security/apppasswords");
                Logger.Error("And use that for Login with Google.");
                Logger.Error("Opening the Site in 5 Seconds.");
                Thread.Sleep(5000);
                Process.Start("https://security.google.com/settings/security/apppasswords");
                Logger.Error("The Program is now freezed.");
                Thread.Sleep(Timeout.Infinite);
            }

            if (response.ContainsKey("Error")){
                Logger.Error(response["Error"]);
                return null;
            }

            //Todo: captcha/2fa implementation

            if (!response.ContainsKey("Auth")){
                Logger.Error("Auth String not found.");
                return null;
            }

            var oauthResponse = await client.PerformOAuth(response["Token"],
                Resources.GoogleAuth_AuthService,
                Resources.GoogleAuth_App,
                Resources.GoogleAuth_ClientSig).ConfigureAwait(false);

            if (!oauthResponse.ContainsKey("Auth")){
                Logger.Error("Auth String not found.");
                return null;
            }
            return oauthResponse["Auth"];
        }
    }
}