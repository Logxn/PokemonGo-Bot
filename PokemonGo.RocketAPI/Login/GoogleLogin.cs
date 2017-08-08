﻿using GPSOAuthSharp;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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
                "audience:server:client_id:848232511240-7so421jotr2609rmqakceuu1luuq0ptb.apps.googleusercontent.com",
                "com.nianticlabs.pokemongo",
                "321187995bc7cdc2b5fc91b11a96e2baa8602c62").ConfigureAwait(false);

            if (!oauthResponse.ContainsKey("Auth")){
                Logger.Error("Auth String not found.");
                return null;
            }
            return oauthResponse["Auth"];
        }
    }
}