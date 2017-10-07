using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.HttpClient
{
    public class CurrentAPIVersion
    {
        public Version CurrentNianticAPIVersion;

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
        public  bool CheckAPIVersionCompatibility( Version _botAPISupportedVersion)
        {
            return (CurrentNianticAPIVersion <= _botAPISupportedVersion);
        }

        /// <summary>
        /// Returns a string with current Niantic forced API version
        /// </summary>
        /// <returns>string</returns>
        Version RequestCurrentNianticAPIVersion()
        {
            CurrentNianticAPIVersion = new Version();
            try {
                CurrentNianticAPIVersion = new Version(HttpGetCurrentNianticAPIVersion().Result);
                Logger.Debug("Debug: Returned API version string is " + CurrentNianticAPIVersion.ToString());
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString() + CurrentNianticAPIVersion.ToString());
            }
            return CurrentNianticAPIVersion;
        }

        /// <summary>
        /// Calls NIANTIC end point that gives current API version
        /// </summary>
        /// <returns>string</returns>
        async Task<string> HttpGetCurrentNianticAPIVersion()
        {
            string _returnedVersion = "";

            using (var _httpClient = new System.Net.Http.HttpClient())
            //using (var _httpClient = new HttpClient.LoginHttpClient())
            {
                var tries = 0;
                while (tries < 10)
                {
                    try
                    {
                        var _uri = new Uri(Resources.GetRpcVersionUrl);

                        HttpResponseMessage _response = await _httpClient.GetAsync(_uri).ConfigureAwait(false);

                        if (_response.IsSuccessStatusCode)
                        {
                            _returnedVersion = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            _returnedVersion = new string(_returnedVersion.Where(c => !char.IsControl(c)).ToArray());
                            return _returnedVersion;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug("Error: CurrentAPIVersion.cs - getCurrentNianticAPIVersion(): " + ex.Message);
                    }
                    tries++;
                    await Task.Delay(1000).ConfigureAwait(false);
                    Logger.Debug("Trying Again. Try: " + tries);
                }
                Logger.Debug("Error: Too many tries without sucess.");
                return "Error";
            }
        }
    }
}