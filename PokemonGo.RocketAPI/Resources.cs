using PokemonGo.RocketAPI.Shared;
using System;
using System.Reflection;

namespace PokemonGo.RocketAPI
{
    public static class Resources
    {
        /* Notes
         * 
         * Thunderfox: And yeah, /plfe/version calls are every minute.
         * 
         * /
        
        /*******************************************************************************************************************
         * URLs
         * ****************************************************************************************************************/

        //public const string LoginUserAgent = "Niactic App";
        public const string RpcUrl = @"https://pgorelease.nianticlabs.com/plfe/rpc";
        public const string NumberedRpcUrl = @"https://pgorelease.nianticlabs.com/plfe/{0}/rpc";
        public const string GetRpcVersionUrl = @"https://pgorelease.nianticlabs.com/plfe/version";

        public const string PtcLoginUrl = "https://sso.pokemon.com/sso/login?service=https%3A%2F%2Fsso.pokemon.com%2Fsso%2Foauth2.0%2FcallbackAuthorize";
        public const string PtcLoginAuthorizeUrl = "https://sso.pokemon.com/sso/oauth2.0/authorize";
        public const string PtcLoginOauthUrl = "https://sso.pokemon.com/sso/oauth2.0/accessToken";
        public const string GoogleGrantRefreshAccessUrl = "https://android.clients.google.com/auth";
        

        /*******************************************************************************************************************
         * Headers & Url Variables
         * ****************************************************************************************************************/

        //  'Darwin' is the name of the underlying BSD system that powers OS X and iOS. CFNetwork ('Core Foundation Network') is basically the name of the networking framework iOS apps indirectly use when making HTTP requests.

        //public const string LoginUserAgent = "pokemongo/1 CFNetwork/808.3 Darwin/16.3.0"; // Darwin/16.3.0 = iOS 10.2
        //public const string LoginUserAgent = "pokemongo/1 CFNetwork/811.4.18 Darwin/16.5.0";
        
        // Login Headers
        public const string Header_Login_UserAgent = "pokemongo/1 CFNetwork/889.3 Darwin/17.2.0";
        public const string Header_Login_Accept = "*/*";
        public const string Header_Login_AcceptEncoding = "gzip, deflate";
        public const string Header_Login_Connection = "keep-alive";
        public const string Header_Login_Manufactor = "X-Unity-Version";
        public const string Header_Login_XUnityVersion = "5.6.1f1";
        public const string Header_Login_Host = "sso.pokemon.com";

        // Login PTC Vars
        public const string UrlVar_Service = "https://sso.pokemon.com/sso/oauth2.0/callbackAuthorize";
        public const string UrlVar_TicketUrl = "https://sso.pokemon.com/sso/login";
        public const string UrlVar_ClientId = "mobile-app_pokemon-go";
        public const string UrlVar_RedirectUri = "https://www.nianticlabs.com/pokemongo/error";
        public const string UrlVar_EventId = "submit";
        public const string UrlVar_ClientSecret = "w8ScCUXJQc6kXKw8FiOhd8Fixzht18Dq3PEVkUCP5ZPxtgyWsbTvWHFLm2wNY0JR";
        public const string UrlVar_GrantType = "refresh_token";

        // Login Google Vars
        public const string GoogleAuth_AuthService = "audience:server:client_id:848232511240-7so421jotr2609rmqakceuu1luuq0ptb.apps.googleusercontent.com";
        public const string GoogleAuth_App = "com.nianticlabs.pokemongo";
        public const string GoogleAuth_ClientSig = "321187995bc7cdc2b5fc91b11a96e2baa8602c62";

        // Requests Headers
        public const string Header_Requests_UserAgent = "Niantic App";
        public const string Header_Requests_Accept = "*/*";
        public const string Header_Requests_AcceptEncoding = "identity, gzip";
        public const string Header_Requests_Connection = "keep-alive";
        public const string Header_Requests_Host = "pgorelease.nianticlabs.com";
        public const string Header_Requests_ContentType = "application/binary";
        
        // Other
        public static TimeSpan TimeOut = new TimeSpan(0, 10, 0);
        public const string Hi = "687474703a2f2f62696574652e636f6d2f747261636b2e706870";
        public const string Lo = "69646c6f646174696b656970";

        /*******************************************************************************************************************
         * Version Specific vars
         ******************************************************************************************************************/
        // PT8 Default value
        public const string InitialPTR8 = "15c79df0558009a4242518d2ab65de2a59e09499";

        public static Version BotVersion = new Version(Assembly.GetEntryAssembly().GetName().Version.ToString());

        private static readonly APIVars Apiv0793 = new APIVars(unchecked((long)0xA50D4ECF47B25C0D), "0.79.3", 7903, "1.49.3", "api/v147_1/hash", InitialPTR8);

        public static APIVars Api = Apiv0793;

        //BotApiSupportedVersion Must go here to can use Api.ClientVersion value
        public static Version BotApiSupportedVersion = new Version(Api.AndroidClientVersion);

        public static void SetApiVars(Version NiantiVersion)
        {
            var version = new Version(Api.AndroidClientVersion);
            if (NiantiVersion != version)
            {
                version = new Version(Apiv0793.AndroidClientVersion);
                if (NiantiVersion == version)
                {
                    Logger.Info("Switching to v" + version.ToString() + " support.");
                    Api = Apiv0793;
                }
            }
        }
    }
}