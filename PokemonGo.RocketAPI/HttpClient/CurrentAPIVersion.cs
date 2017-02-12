using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.HttpClient
{
    public class CurrentAPIVersion
    {
        public static Version CurrentNianticAPIVersion;

        public CurrentAPIVersion()
        {
            CurrentNianticAPIVersion = RequestCurrentNianticAPIVersion();
        }

        public Version GetNianticAPIVersion()
        {
            return CurrentNianticAPIVersion;
        }

        /// <summary>
        /// Checks if current API version is supported, so if it is not bigger that the current Bot supported version
        /// </summary>
        /// <param name="_botAPISupportedVersion">Current BOT Supported API Version</param>
        /// <param name="_NianticAPIVersion">Current NIANTIC API Version</param>
        /// <returns></returns>
        public bool CheckAPIVersionCompatibility( Version _botAPISupportedVersion)
        {
            return (CurrentNianticAPIVersion <= _botAPISupportedVersion);
        }

        /// <summary>
        /// Returns a string with current Niantic forced API version
        /// </summary>
        /// <returns>string</returns>
        Version RequestCurrentNianticAPIVersion()
        {
            CurrentNianticAPIVersion = new Version(HttpGetCurrentNianticAPIVersion().Result);
            return CurrentNianticAPIVersion;
        }

        /// <summary>
        /// Calls NIANTIC end potint that gives current API version
        /// </summary>
        /// <returns>string</returns>
        async Task<string> HttpGetCurrentNianticAPIVersion()
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
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: CurrentAPIVersion.cs - getCurrentNianticAPIVersion()");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
                    throw;
                }
            }
        }
    }
}