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

        public async Task<GetPlayerProfileResponse> GetPlayerProfile(string playerName)
        {
            return  PostProtoPayload<Request, GetPlayerProfileResponse>(RequestType.GetPlayerProfile, new GetPlayerProfileMessage()
            {
                PlayerName = playerName
            });
        }

        public async Task<CheckAwardedBadgesResponse> GetNewlyAwardedBadges()
        {
            return  PostProtoPayload<Request, CheckAwardedBadgesResponse>(RequestType.CheckAwardedBadges, new CheckAwardedBadgesMessage());
        }

        public async Task<CollectDailyBonusResponse> CollectDailyBonus()
        {
            return  PostProtoPayload<Request, CollectDailyBonusResponse>(RequestType.CollectDailyBonus, new CollectDailyBonusMessage());
        }

        public async Task<CollectDailyDefenderBonusResponse> CollectDailyDefenderBonus()
        {
            return PostProtoPayload<Request, CollectDailyDefenderBonusResponse>(RequestType.CollectDailyDefenderBonus, new CollectDailyDefenderBonusMessage());
        }

        public async Task<EquipBadgeResponse> EquipBadge(BadgeType type)
        {
            return PostProtoPayload<Request, EquipBadgeResponse>(RequestType.EquipBadge, new EquipBadgeMessage() { BadgeType = type });
        }

        public async Task<LevelUpRewardsResponse> GetLevelUpRewards(int level)
        {
            return  PostProtoPayload<Request, LevelUpRewardsResponse>(RequestType.LevelUpRewards, new LevelUpRewardsMessage()
            {
                Level = level
            });
        }

        public async Task<SetAvatarResponse> SetAvatar(PlayerAvatar playerAvatar)
        {
            return  PostProtoPayload<Request, SetAvatarResponse>(RequestType.SetAvatar, new SetAvatarMessage()
            {
                PlayerAvatar = playerAvatar
            });
        }

        public async Task<SetContactSettingsResponse> SetContactSetting(ContactSettings contactSettings)
        {
            return  PostProtoPayload<Request, SetContactSettingsResponse>(RequestType.SetContactSettings, new SetContactSettingsMessage()
            {
                ContactSettings = contactSettings
            });
        }

        public async Task<SetPlayerTeamResponse> SetPlayerTeam(TeamColor teamColor)
        {
            return  PostProtoPayload<Request, SetPlayerTeamResponse>(RequestType.SetPlayerTeam, new SetPlayerTeamMessage()
            {
                Team = teamColor
            });
        }

        public async Task<VerifyChallengeResponse> VerifyChallenge(string token)
        {
            return  PostProtoPayload<Request, VerifyChallengeResponse>(RequestType.VerifyChallenge, CommonRequest.GetVerifyChallenge(token));
        }
    }
}
