/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/09/2017
 * Time: 0:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using POGOLib.Official.Net;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using Google.Protobuf;


namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Encounter.
    /// </summary>
    public class Encounter
    {
        internal Session session;
        public Encounter(Session session)
        {
            this.session = session;
        }
        public async Task<EncounterResponse> EncounterPokemon(ulong encounterId, string spawnpointId)
        {
            var msg = new EncounterMessage();
            msg.EncounterId  = encounterId;
            msg.SpawnPointId = spawnpointId;
            msg.PlayerLatitude = session.Player.Latitude;
            msg.PlayerLongitude = session.Player.Longitude;
            var request = new Request();
            request.RequestType = RequestType.Encounter;
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return EncounterResponse.Parser.ParseFrom(response);
        }

        public async Task<CatchPokemonResponse> CatchPokemon(ulong encounterId, string spawnPointGuid, ItemId pokeballItemId, bool forceHit, double normalizedRecticleSize = 1.950, double spinModifier = 1, double normalizedHitPos = 1)
        {
            var message = new CatchPokemonMessage
            {
                EncounterId = encounterId,
                Pokeball = pokeballItemId,
                SpawnPointId = spawnPointGuid,
                HitPokemon = forceHit,
                NormalizedReticleSize = normalizedRecticleSize,
                SpinModifier = spinModifier,
                NormalizedHitPosition = normalizedHitPos
            };
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = message.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return CatchPokemonResponse.Parser.ParseFrom(response);
        }
        public async Task<IncenseEncounterResponse> IncenseEncounter(ulong encounterId, string encounterLocation)
        {
            var message = new IncenseEncounterMessage()
            {
                EncounterId = encounterId,
                EncounterLocation = encounterLocation
            };
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = message.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return IncenseEncounterResponse.Parser.ParseFrom(response);
        }
        public async Task<DiskEncounterResponse> DiskEncounter(ulong encounterId, string fortId)
        {
            var message = new DiskEncounterMessage()
            {
                EncounterId = encounterId,
                FortId = fortId,
                PlayerLatitude = session.Player.Latitude,
                PlayerLongitude = session.Player.Longitude
            };
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = message.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return DiskEncounterResponse.Parser.ParseFrom(response);
        }
    }
}
