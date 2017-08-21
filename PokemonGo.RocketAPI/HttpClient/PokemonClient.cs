using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.HttpClient
{
    public class PokemonHttpClient : System.Net.Http.HttpClient
    {
        private static readonly HttpClientHandler Handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = false,
            UseProxy = Client.proxy != null,
            Proxy = Client.proxy,
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        public CookieContainer Cookies
        {
            get { return Handler.CookieContainer; }
            set { Handler.CookieContainer = value; }
        }

        public PokemonHttpClient() : base(new RetryHandler(Handler))
        {
            DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", Resources.Header_Requests_ContentType);
            DefaultRequestHeaders.Host = Resources.Header_Requests_Host;
            DefaultRequestHeaders.UserAgent.TryParseAdd(Resources.Header_Requests_UserAgent);
            DefaultRequestHeaders.AcceptEncoding.TryParseAdd(Resources.Header_Requests_AcceptEncoding);
            DefaultRequestHeaders.ExpectContinue = false;
        }
    }
}
