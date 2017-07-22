using System;
using System.Reflection;

namespace PokemonGo.RocketAPI
{
    public class Resources
    {
        /*******************************************************************************************************************
         * URLs
         * ****************************************************************************************************************/

        //public const string LoginUserAgent = "Niactic App";
        public const string LoginUserAgent = "pokemongo/1 CFNetwork/808.3 Darwin/16.3.0";
        public const string RpcUrl = @"https://pgorelease.nianticlabs.com/plfe/rpc";
        public const string NumberedRpcUrl = @"https://pgorelease.nianticlabs.com/plfe/{0}/rpc";
        public const string GetRpcVersionUrl = @"https://pgorelease.nianticlabs.com/plfe/version";

        public const string PtcLoginUrl =
            "https://sso.pokemon.com/sso/login?service=https%3A%2F%2Fsso.pokemon.com%2Fsso%2Foauth2.0%2FcallbackAuthorize";

        public const string PtcLoginOauth = "https://sso.pokemon.com/sso/oauth2.0/accessToken";
        public const string GoogleGrantRefreshAccessUrl = "https://android.clients.google.com/auth";

        // 0.45 hash
        //public static string currentGameHash = "54b359c97e46900f87211ef6e6dd0b7f2a3ea1f5";

        /*******************************************************************************************************************
         * Bot Supported Version
         ******************************************************************************************************************/
        public static Version BotVersion = new Version(Assembly.GetEntryAssembly().GetName().Version.ToString());

        /* Obsolete APIs
        private static APIVars Apiv53 = new APIVars(-1, "0.55.0",
                                                    -76506539888958491, "1.23.1",
                                                "e40c3e64817d9c96d99d28f6488a2efc40b11046",
                                                "api/v123_1/hash");

        private static APIVars Apiv55 = new APIVars(-1, "0.55.0",
                                                    -9156899491064153954,"1.25.0",
                                                "90f6a704505bccac73cec99b07794993e6fd5a12",
                                                "api/v125/hash");

        private static APIVars Apiv572 = new APIVars(7472694709694384708,"0.57.2",
                                                     -816976800928766045,"1.27.2",
                                                "90f6a704505bccac73cec99b07794993e6fd5a12",
                                                "api/v127_2/hash");

        private static APIVars Apiv573 = new APIVars(7472694709694384708, "0.57.3", -816976800928766045, "1.27.3", "90f6a704505bccac73cec99b07794993e6fd5a12", "api/v127_3/hash");

        private static APIVars Apiv574 = new APIVars(7472694709694384708, "0.57.4",
                                                     -816976800928766045, "1.27.4",
                                                     "90f6a704505bccac73cec99b07794993e6fd5a12",
                                                     "api/v127_4/hash");
        private static APIVars apiv0591 = new APIVars(-3226782243204485589, "0.59.1", -3226782243204485589, "1.29.1", "api/v129_1/hash");

        private static APIVars apiv0600 = new APIVars(0x11fdf018c941ef22, "0.61.0", 0x11fdf018c941ef22, "1.31.0",  "api/v131_0/hash");

        private static APIVars api063_1 = new APIVars(0x11fdf018c941ef22, "0.63.1", 0x4A3889A251CCAD52, "1.33.1", "api/v133_1/hash");
        */


        private static APIVars apiv0690 = new APIVars(0x4AE22D4661C83701, "0.69.0", 0x4AE22D4661C83701, "1.39.0", "api/v137_1/hash");

        public static APIVars Api = apiv0690;

        //BotApiSupportedVersion Must go here to can use Api.ClientVersion value
        public static Version BotApiSupportedVersion = new Version(Api.AndroidClientVersion);

    }
}