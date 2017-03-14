using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using PokeMaster.Logic.Utils;
using PokemonGo.RocketAPI;

namespace PokeMaster.Helper
{
    public static class VersionHelper
    {
        public static Version CurrentNianticAPIVersion;
        public static Version BotVersion = Assembly.GetEntryAssembly().GetName().Version;
        public static Version BotApiSupportedVersion = new Version (Client.API_VERSION);

        /// <summary>
        /// Checks if current API version is supported, so if it is not bigger that the current Bot supported version
        /// </summary>
        /// <param name="_botAPISupportedVersion">Current BOT Supported API Version</param>
        /// <param name="_NianticAPIVersion">Current NIANTIC API Version</param>
        /// <returns></returns>
        public static bool CheckAPIVersionCompatibility( )
        {
            return (CurrentNianticAPIVersion <= BotApiSupportedVersion);
        }

        /// <summary>
        /// Returns a string with current Niantic forced API version
        /// </summary>
        /// <returns>string</returns>
        public static void RequestCurrentNianticAPIVersion()
        {
            CurrentNianticAPIVersion = new Version();
            try {
                CurrentNianticAPIVersion = new Version(HttpGetCurrentNianticAPIVersion());
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString());
            }
        }

        /// <summary>
        /// Calls NIANTIC end potint that gives current API version
        /// </summary>
        /// <returns>string</returns>
        static string HttpGetCurrentNianticAPIVersion()
        {
            string _returnedVersion = "";

            using (var _httpClient = new System.Net.Http.HttpClient())
            {
                try
                {
                    var _uri = new Uri(Constants.VersionUrl);
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage _response =  _httpClient.GetAsync(_uri).Result;

                    if (_response.IsSuccessStatusCode)
                    {
                        _returnedVersion =  _response.Content.ReadAsStringAsync().Result;
                        _returnedVersion = new string(_returnedVersion.Where(c => !char.IsControl(c)).ToArray());
                        return _returnedVersion;
                    }
                    return "Error";
                }
                catch (Exception ex)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: CurrentAPIVersion.cs - getCurrentNianticAPIVersion()");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
                    throw;
                }
            }
        }
    }
}