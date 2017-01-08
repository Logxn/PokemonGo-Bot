using PokemonGo.RocketAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Hash
{
    public class LegacyHasher   : IHasher
    {
        public long Client_Unknown25   =>-1553869577012279119;
        public  HashResponseContent RequestHashes(HashRequestContent request)
        {
            return RequestHashesAsync(request).Result;
        }
        public async Task<HashResponseContent> RequestHashesAsync(HashRequestContent request)
        {
            byte[] locationBytes = BitConverter.GetBytes(request.Latitude).Reverse()
                 .Concat(BitConverter.GetBytes(request.Longitude).Reverse())
                 .Concat(BitConverter.GetBytes(request.Altitude).Reverse()).ToArray();

            var hashed = new HashResponseContent()
            {
                LocationAuthHash =  (uint)Utils.GenerateLocation1(locationBytes, request.AuthTicket),
                LocationHash = (uint)Utils.GenerateLocation2(locationBytes),
                RequestHashes = new List<long>()
            };
            foreach (var req in request.Requests)
                hashed.RequestHashes.Add((long)Utils.GenerateRequestHash(request.AuthTicket, req));

            return hashed;
        }
    }
}
