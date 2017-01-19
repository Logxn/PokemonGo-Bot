using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.HttpClient
{
    public class CurrentAPIVersion
    {
        /// <summary>
        /// Checks if current API version is supported, so if it is not bigger that the current Bot supported version
        /// </summary>
        /// <param name="_botVersion">Current BOT Version</param>
        /// <param name="_botAPISupportedVersion">Current BOT Supported API Version</param>
        /// <param name="_NianticAPIVersion">Current NIANTIC API Version</param>
        /// <returns></returns>
        public bool CheckAPIVersionCompatibility(Version _botVersion, Version _botAPISupportedVersion, Version _NianticAPIVersion)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"Bot Current version: {_botVersion}");
            Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"Bot Supported API version: {_botAPISupportedVersion}");
            Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"Current API version: {_NianticAPIVersion}");

            if (_NianticAPIVersion > _botAPISupportedVersion)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Atention, current API version is new and still not supported by Bot.");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Bot will now exit to keep your account safe.");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"---------- PRESS ANY KEY TO CLOSE ----------");
                Console.ReadKey();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a string with current Niantic forced API version
        /// </summary>
        /// <returns>string</returns>
        public string GetCurrentAPIVersion()
        {
            return HttpGetCurrentAPIVersion().Result;
        }

        /// <summary>
        /// Calls NIANTIC end potint that gives current API version
        /// </summary>
        /// <returns>string</returns>
        public async Task<string> HttpGetCurrentAPIVersion()
        {
            string _returnedVersion = "";

            using (var _httpClient = new System.Net.Http.HttpClient())
            {
                try
                {
                    Uri _uri = new Uri(Resources.GetRpcVersionUrl);
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage _response = await _httpClient.GetAsync(_uri);

                    if (_response.IsSuccessStatusCode)
                    {
                        _returnedVersion = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        _returnedVersion = new string(_returnedVersion.Where(c => !char.IsControl(c)).ToArray());
                        return _returnedVersion;
                    }
                    else
                    {
                        return "Error";
                    }
                }
                catch (Exception ex)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: CurrentAPIVersion.cs - getCurrentAPIVersion()");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
                    throw;
                }
            }
        }
    }
}