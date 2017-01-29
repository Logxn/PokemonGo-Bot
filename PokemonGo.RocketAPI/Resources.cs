using System;
using System.Reflection;

namespace PokemonGo.RocketAPI
{
    public class Resources
    {
        /*******************************************************************************************************************
         * URLs
         * ****************************************************************************************************************/

        public const string RpcUrl = @"https://pgorelease.nianticlabs.com/plfe/rpc";
        public const string NumberedRpcUrl = @"https://pgorelease.nianticlabs.com/plfe/{0}/rpc";
        public const string GetRpcVersionUrl = @"https://pgorelease.nianticlabs.com/plfe/version";

        public const string PtcLoginUrl =
            "https://sso.pokemon.com/sso/login?service=https%3A%2F%2Fsso.pokemon.com%2Fsso%2Foauth2.0%2FcallbackAuthorize";

        public const string PtcLoginOauth = "https://sso.pokemon.com/sso/oauth2.0/accessToken";
        public const string GoogleGrantRefreshAccessUrl = "https://android.clients.google.com/auth";

        // ????
        public static string currentGameHash = "54b359c97e46900f87211ef6e6dd0b7f2a3ea1f5";


        /*******************************************************************************************************************
         * v0.53.x
         ******************************************************************************************************************/
        public const long Unknown25 = -76506539888958491;
        public const string UnknownPtr8_RequestMessage = "e40c3e64817d9c96d99d28f6488a2efc40b11046";
        public const string ClientVersionString = "0.53.0";
        public const uint ClientVersionInt = 5301;

        /*******************************************************************************************************************
         * Bot Supported Version
         ******************************************************************************************************************/
        public static Version BotVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());
        public static Version BotApiSupportedVersion = new Version("0.53.0");
    }
}