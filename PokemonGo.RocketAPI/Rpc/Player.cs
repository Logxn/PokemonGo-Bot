using Google.Protobuf.Collections;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Shared;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Player : BaseRpc
    {
        public GetPlayerResponse PlayerResponse {
            get;
            set;
        }

        public Player(Client client) : base(client)
        {
            Client = client;
        }

        public GetPlayerResponse GetPlayer(bool forceRequest = true)
        {
            if (forceRequest)
                PlayerResponse = PostProtoPayloadCommonR<Request, GetPlayerResponse>(RequestType.GetPlayer, CommonRequest.GetPlayerMessageRequest(LocaleInfo.Country, LocaleInfo.Language, LocaleInfo.TimeZone)).Result;
            return PlayerResponse;
        }

        public GetPlayerProfileResponse GetPlayerProfile(string playerName = "")
        {
            if (string.IsNullOrEmpty( playerName) && PlayerResponse !=null &&  PlayerResponse.PlayerData!= null)
                playerName = PlayerResponse.PlayerData.Username;
            return PostProtoPayloadCommonR<Request, GetPlayerProfileResponse>(RequestType.GetPlayerProfile, new GetPlayerProfileMessage()
            {
                PlayerName = playerName
            }).Result;
        }

        public CheckAwardedBadgesResponse GetNewlyAwardedBadges()
        {
            return  PostProtoPayloadCommonR<Request, CheckAwardedBadgesResponse>(RequestType.CheckAwardedBadges, new CheckAwardedBadgesMessage()).Result;
        }

        public CollectDailyBonusResponse CollectDailyBonus()
        {
            return  PostProtoPayloadCommonR<Request, CollectDailyBonusResponse>(RequestType.CollectDailyBonus, new CollectDailyBonusMessage()).Result;
        }


        public EquipBadgeResponse EquipBadge(BadgeType type)
        {
            return PostProtoPayloadCommonR<Request, EquipBadgeResponse>(RequestType.EquipBadge, new EquipBadgeMessage() { BadgeType = type }).Result;
        }

        public LevelUpRewardsResponse GetLevelUpRewards(int level)
        {
            return  PostProtoPayloadCommonR<Request, LevelUpRewardsResponse>(RequestType.LevelUpRewards, new LevelUpRewardsMessage()
            {
                Level = level
            }).Result;
        }

        public ListAvatarCustomizationsResponse ListAvatarCustomizations(PlayerAvatarType _avatarType, RepeatedField<Filter> _filters, RepeatedField<Slot> _slot, int _limit, int _start)
        {
            return PostProtoPayloadCommonR<Request, ListAvatarCustomizationsResponse>(RequestType.ListAvatarCustomizations, new ListAvatarCustomizationsMessage()
            {
                AvatarType = _avatarType,
                //Filters = _filters,
                //Slot = _slot,
                Start = _start,
                Limit = _limit
            }).Result;
        }

        public async Task<SetAvatarResponse> SetAvatar(PlayerAvatar playerAvatar)
        {
            return await PostProtoPayloadCommonR<Request, SetAvatarResponse>(RequestType.SetAvatar, new SetAvatarMessage()
            {
                PlayerAvatar = playerAvatar
            }).ConfigureAwait(false);
        }

        public SetContactSettingsResponse SetContactSetting(ContactSettings contactSettings)
        {
            return  PostProtoPayloadCommonR<Request, SetContactSettingsResponse>(RequestType.SetContactSettings, new SetContactSettingsMessage()
            {
                ContactSettings = contactSettings
            }).Result;
        }

        public SetPlayerTeamResponse SetPlayerTeam(TeamColor teamColor)
        {
            return  PostProtoPayloadCommonR<Request, SetPlayerTeamResponse>(RequestType.SetPlayerTeam, new SetPlayerTeamMessage()
            {
                Team = teamColor
            }).Result;
        }

        public VerifyChallengeResponse VerifyChallenge(string token)
        {
            return  PostProtoPayloadCommonR<Request, VerifyChallengeResponse>(RequestType.VerifyChallenge, CommonRequest.GetVerifyChallenge(token)).Result;
        }

        public void SetCoordinates(double latitude, double longitude, double altitude)
        {
            Client.CurrentLatitude = latitude;
            Client.CurrentLongitude = longitude;
            Client.CurrentAltitude = altitude;
        }
    }
}
