using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using POGOProtos.Networking.Platform;
using POGOProtos.Networking.Envelopes;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using System.IO;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Player : BaseRpc
    {
        public Player(Client client) : base(client)
        {
            Client = client;
        }

        public async Task<PlayerUpdateResponse> UpdatePlayerLocation(double latitude, double longitude, double altitude)
        {

            // This needs to me removed from here
            Client.CurrentLatitude = latitude;
            Client.CurrentLongitude = longitude;
            Client.CurrentAltitude = altitude;
            string latlngalt = latitude.ToString(CultureInfo.InvariantCulture) + ":" + longitude.ToString(CultureInfo.InvariantCulture) + ":" + altitude.ToString(CultureInfo.InvariantCulture);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Configs\\LastCoords.txt", latlngalt);

            var ret = new PlayerUpdateResponse();
            return ret;
            // TODO: check how to work PlayerUpdateMessage now 14/02/2017
            try {
                var message = new PlayerUpdateMessage
                {
                    Latitude = Client.CurrentLatitude,
                    Longitude = Client.CurrentLongitude
                };
                var updatePlayerLocationRequestEnvelope = GetRequestBuilder().GetRequestEnvelope(new Request[] {
                    new Request
                    {
                        RequestType = RequestType.PlayerUpdate,
                        RequestMessage = message.ToByteString()
                    }
                }).Result;
                ret = PostProtoPayload<Request, PlayerUpdateResponse>(updatePlayerLocationRequestEnvelope).Result;
            } catch (Exception ex1) {
                Logger.Debug("ex1:" + ex1);
            }

            return  ret;
        }


        
        public async Task<GetPlayerResponse> GetPlayer()
        {
            return await PostProtoPayload<Request, GetPlayerResponse>(RequestType.GetPlayer, new GetPlayerMessage()).ConfigureAwait(false);
        }

        public async Task<GetPlayerProfileResponse> GetPlayerProfile(string playerName)
        {
            return await PostProtoPayload<Request, GetPlayerProfileResponse>(RequestType.GetPlayerProfile, new GetPlayerProfileMessage()
            {
                PlayerName = playerName
            }).ConfigureAwait(false);
        }

        public async Task<CheckAwardedBadgesResponse> GetNewlyAwardedBadges()
        {
            return await PostProtoPayload<Request, CheckAwardedBadgesResponse>(RequestType.CheckAwardedBadges, new CheckAwardedBadgesMessage()).ConfigureAwait(false);
        }

        public async Task<CollectDailyBonusResponse> CollectDailyBonus()
        {
            return await PostProtoPayload<Request, CollectDailyBonusResponse>(RequestType.CollectDailyBonus, new CollectDailyBonusMessage()).ConfigureAwait(false);
        }

        public async Task<CollectDailyDefenderBonusResponse> CollectDailyDefenderBonus()
        {
            return await PostProtoPayload<Request, CollectDailyDefenderBonusResponse>(RequestType.CollectDailyDefenderBonus, new CollectDailyDefenderBonusMessage()).ConfigureAwait(false);
        }

        public async Task<EquipBadgeResponse> EquipBadge(BadgeType type)
        {
            return await PostProtoPayload<Request, EquipBadgeResponse>(RequestType.EquipBadge, new EquipBadgeMessage() { BadgeType = type }).ConfigureAwait(false);
        }

        public async Task<LevelUpRewardsResponse> GetLevelUpRewards(int level)
        {
            return await PostProtoPayload<Request, LevelUpRewardsResponse>(RequestType.LevelUpRewards, new LevelUpRewardsMessage()
            {
                Level = level
            }).ConfigureAwait(false);
        }

        public async Task<SetAvatarResponse> SetAvatar(PlayerAvatar playerAvatar)
        {
            return await PostProtoPayload<Request, SetAvatarResponse>(RequestType.SetAvatar, new SetAvatarMessage()
            {
                PlayerAvatar = playerAvatar
            }).ConfigureAwait(false);
        }

        public async Task<SetContactSettingsResponse> SetContactSetting(ContactSettings contactSettings)
        {
            return await PostProtoPayload<Request, SetContactSettingsResponse>(RequestType.SetContactSettings, new SetContactSettingsMessage()
            {
                ContactSettings = contactSettings
            }).ConfigureAwait(false);
        }

        public async Task<SetPlayerTeamResponse> SetPlayerTeam(TeamColor teamColor)
        {
            return await PostProtoPayload<Request, SetPlayerTeamResponse>(RequestType.SetPlayerTeam, new SetPlayerTeamMessage()
            {
                Team = teamColor
            }).ConfigureAwait(false);
        }

        public async Task<VerifyChallengeResponse> VerifyChallenge(string token)
        {
            return await PostProtoPayload<Request, VerifyChallengeResponse>(RequestType.VerifyChallenge, CommonRequest.GetVerifyChallenge(token)).ConfigureAwait(false);
        }
    }
}
