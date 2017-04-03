using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Helpers;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Player : BaseRpc
    {
        public Player(Client client) : base(client)
        {
            Client = client;
        }

        public GetPlayerResponse GetPlayer()
        {
            return PostProtoPayload<Request, GetPlayerResponse>(RequestType.GetPlayer, new GetPlayerMessage());
        }

        public GetPlayerProfileResponse GetPlayerProfile(string playerName)
        {
            return  PostProtoPayload<Request, GetPlayerProfileResponse>(RequestType.GetPlayerProfile, new GetPlayerProfileMessage()
            {
                PlayerName = playerName
            });
        }

        public CheckAwardedBadgesResponse GetNewlyAwardedBadges()
        {
            return  PostProtoPayload<Request, CheckAwardedBadgesResponse>(RequestType.CheckAwardedBadges, new CheckAwardedBadgesMessage());
        }

        public CollectDailyBonusResponse CollectDailyBonus()
        {
            return  PostProtoPayload<Request, CollectDailyBonusResponse>(RequestType.CollectDailyBonus, new CollectDailyBonusMessage());
        }

        public CollectDailyDefenderBonusResponse CollectDailyDefenderBonus()
        {
            return PostProtoPayload<Request, CollectDailyDefenderBonusResponse>(RequestType.CollectDailyDefenderBonus, new CollectDailyDefenderBonusMessage());
        }

        public EquipBadgeResponse EquipBadge(BadgeType type)
        {
            return PostProtoPayload<Request, EquipBadgeResponse>(RequestType.EquipBadge, new EquipBadgeMessage() { BadgeType = type });
        }

        public LevelUpRewardsResponse GetLevelUpRewards(int level)
        {
            return  PostProtoPayload<Request, LevelUpRewardsResponse>(RequestType.LevelUpRewards, new LevelUpRewardsMessage()
            {
                Level = level
            });
        }

        public SetAvatarResponse SetAvatar(PlayerAvatar playerAvatar)
        {
            return  PostProtoPayload<Request, SetAvatarResponse>(RequestType.SetAvatar, new SetAvatarMessage()
            {
                PlayerAvatar = playerAvatar
            });
        }

        public SetContactSettingsResponse SetContactSetting(ContactSettings contactSettings)
        {
            return  PostProtoPayload<Request, SetContactSettingsResponse>(RequestType.SetContactSettings, new SetContactSettingsMessage()
            {
                ContactSettings = contactSettings
            });
        }

        public SetPlayerTeamResponse SetPlayerTeam(TeamColor teamColor)
        {
            return  PostProtoPayload<Request, SetPlayerTeamResponse>(RequestType.SetPlayerTeam, new SetPlayerTeamMessage()
            {
                Team = teamColor
            });
        }

        public VerifyChallengeResponse VerifyChallenge(string token)
        {
            return  PostProtoPayload<Request, VerifyChallengeResponse>(RequestType.VerifyChallenge, CommonRequest.GetVerifyChallenge(token));
        }

        public void SetCoordinates(double latitude, double longitude, double altitude)
        {
            Client.CurrentLatitude = latitude;
            Client.CurrentLongitude = longitude;
            Client.CurrentAltitude = altitude;
        }
    }
}
