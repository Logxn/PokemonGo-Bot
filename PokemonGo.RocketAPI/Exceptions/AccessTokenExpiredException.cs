using System;
using System.Net;
using System.Threading;
using POGOProtos.Enums;
using POGOProtos.Networking.Envelopes;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Hash;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.HttpClient;
using PokemonGo.RocketAPI.Encryption;

namespace PokemonGo.RocketAPI.Exceptions
{
    public class AccessTokenExpiredException : Exception
    {
        
    }
}