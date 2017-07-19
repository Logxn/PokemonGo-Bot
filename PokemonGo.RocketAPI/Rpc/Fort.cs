#region using directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using POGOProtos.Data.Battle;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public class Fort : BaseRpc
    {
        public Fort(Client client) : base(client)
        {
        }

        public FortDetailsResponse GetFortOnly(string fortId, double fortLatitude, double fortLongitude)
        {
            var message = new FortDetailsMessage
            {
                FortId = fortId,
                Latitude = fortLatitude,
                Longitude = fortLongitude
            };

            return PostProtoPayload<Request, FortDetailsResponse>(RequestType.FortDetails, message);
        }
        
        public async Task<FortDetailsResponse> GetFort(string fortId, double fortLatitude, double fortLongitude)
        {
            var request = new Request
            {
                RequestType = RequestType.FortDetails,
                RequestMessage = ((IMessage)new FortDetailsMessage
                {
                    FortId = fortId,
                    Latitude = fortLatitude,
                    Longitude = fortLongitude
                }).ToByteString()
            };

            return await PostProtoPayloadCommonR<Request, FortDetailsResponse>(request).ConfigureAwait(false);
        }

        public FortSearchResponse SearchFortOnly(string fortId, double fortLat, double fortLng)
        {
            var message = new FortSearchMessage
            {
                FortId = fortId,
                FortLatitude = fortLat,
                FortLongitude = fortLng,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, FortSearchResponse>(RequestType.FortSearch, message);
        }
        
        public async Task<FortSearchResponse> SearchFort(string fortId, double fortLat, double fortLng)
        {
            var request = new Request
            {
                RequestType = RequestType.FortSearch,
                RequestMessage = ((IMessage)new FortSearchMessage
                {
                    FortId = fortId,
                    FortLatitude = fortLat,
                    FortLongitude = fortLng,
                    PlayerLatitude = Client.CurrentLatitude,
                    PlayerLongitude = Client.CurrentLongitude
                }).ToByteString()
            };

            return await PostProtoPayloadCommonR<Request, FortSearchResponse>(request).ConfigureAwait(false);
        }

        public AddFortModifierResponse AddFortModifier(string fortId, ItemId modifierType)
        {
            var message = new AddFortModifierMessage
            {
                FortId = fortId,
                ModifierType = modifierType,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, AddFortModifierResponse>(RequestType.AddFortModifier, message);
        }

        public AttackGymResponse AttackGym(string fortId, string battleId, List<BattleAction> battleActions,
            BattleAction lastRetrievedAction)
        {
            var message = new AttackGymMessage
            {
                BattleId = battleId,
                GymId = fortId,
                LastRetrievedAction = lastRetrievedAction,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude,
                AttackActions = { battleActions }
            };

            message.AttackActions.AddRange(battleActions);

            return PostProtoPayload<Request, AttackGymResponse>(RequestType.AttackGym, message);
        }

        /* Obsoleted call ? */
        public FortDeployPokemonResponse FortDeployPokemon(string fortId, ulong pokemonId)
        {
            var message = new FortDeployPokemonMessage
            {
                PokemonId = pokemonId,
                FortId = fortId,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, FortDeployPokemonResponse>(RequestType.FortDeployPokemon, message);
        }
        
        public GymFeedPokemonResponse GymFeedPokemon(string fortId, ulong pokemonId)
        {
            var message = new GymFeedPokemonMessage
            {
                PokemonId = pokemonId,
                GymId = fortId,
                PlayerLatDegrees = Client.CurrentLatitude,
                PlayerLngDegrees = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, GymFeedPokemonResponse>(RequestType.GymFeedPokemon, message);
        }

        public FortRecallPokemonResponse FortRecallPokemon(string fortId, ulong pokemonId)
        {
            var message = new FortRecallPokemonMessage
            {
                PokemonId = pokemonId,
                FortId = fortId,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, FortRecallPokemonResponse>(RequestType.FortRecallPokemon, message);
        }


        /* Obosoleted call
        public GetGymDetailsResponse GetGymDetails(string gymId, double gymLat, double gymLng)
        {
            var message = new GetGymDetailsMessage
            {
                GymId = gymId,
                GymLatitude = gymLat,
                GymLongitude = gymLng,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, GetGymDetailsResponse>(RequestType.GetGymDetails, message);
        }
        */

        public GymGetInfoResponse GymGetInfo(string gymId, double gymLat, double gymLng)
        {
            var message = new GymGetInfoMessage
            {
                GymId = gymId,
                GymLatDegrees = gymLat,
                GymLngDegrees = gymLng,
                PlayerLatDegrees = Client.CurrentLatitude,
                PlayerLngDegrees = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, GymGetInfoResponse>(RequestType.GymGetInfo, message);
        }

        public StartGymBattleResponse StartGymBattleOnly(string gymId, ulong defendingPokemonId,
            IEnumerable<ulong> attackingPokemonIds)
        {
            var message = new StartGymBattleMessage ();
            message.GymId = gymId;
            message.DefendingPokemonId = defendingPokemonId;
            message.AttackingPokemonIds.Add(attackingPokemonIds);
            message.PlayerLatitude = Client.CurrentLatitude;
            message.PlayerLongitude = Client.CurrentLongitude;

            return PostProtoPayload<Request, StartGymBattleResponse>(RequestType.StartGymBattle, message);
        }
        
        public async Task<StartGymBattleResponse> StartGymBattle(string gymId, ulong defendingPokemonId,
            IEnumerable<ulong> attackingPokemonIds)
        {
            var request = new Request
            {
                RequestType = RequestType.StartGymBattle,
                RequestMessage = ((IMessage)new StartGymBattleMessage
                {
                    GymId = gymId,
                    DefendingPokemonId = defendingPokemonId,
                    AttackingPokemonIds = { attackingPokemonIds },
                    PlayerLatitude = Client.CurrentLatitude,
                    PlayerLongitude = Client.CurrentLongitude
                }).ToByteString()
            };
            return await PostProtoPayloadCommonR<Request, StartGymBattleResponse>(request).ConfigureAwait(false);

        }
    }
}