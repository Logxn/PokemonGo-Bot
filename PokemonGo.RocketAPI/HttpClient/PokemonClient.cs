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
            Proxy = Client.proxy
        };

        public const string Header_Requests_UserAgent = "Niantic App";
        public const string Header_Requests_Accept = "/";
        public const string Header_Requests_AcceptEncoding = "identity, gzip";
        public const string Header_Requests_Connection = "keep-alive";
        public const string Header_Requests_Host = "pgorelease.nianticlabs.com";
        public const string Header_Requests_ContentType = "application/binary";
        
        public PokemonHttpClient() : base(new RetryHandler(Handler))
        {
            //DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Niantic App");
            //DefaultRequestHeaders.ExpectContinue = false;
            //DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            //DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            //DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", Header_Requests_ContentType);
            DefaultRequestHeaders.Host = Header_Requests_Host;
            DefaultRequestHeaders.UserAgent.TryParseAdd(Header_Requests_UserAgent);
            DefaultRequestHeaders.AcceptEncoding.TryParseAdd(Header_Requests_AcceptEncoding);
            DefaultRequestHeaders.ExpectContinue = false;
        }
    }
}
