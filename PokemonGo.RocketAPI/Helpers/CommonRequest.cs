#region using directives

using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Exceptions;
using System.Linq;
using System;

#endregion

namespace PokemonGo.RocketAPI.Helpers
{
    public static class CommonRequest
    {
        public static Request GetDownloadRemoteConfigVersionMessageRequest(Client client)
        {
            var downloadRemoteConfigVersionMessage = new DownloadRemoteConfigVersionMessage
            {
                Platform = client.Platform,
                AppVersion = client.AppVersion
            };
            return new Request
            {
                RequestType = RequestType.DownloadRemoteConfigVersion,
                RequestMessage = downloadRemoteConfigVersionMessage.ToByteString()
            };
        }
        
        public static Request GetPlayerMessageRequest( string country ="", string language="", string zone="")
        {
            var locale = new GetPlayerMessage.Types.PlayerLocale();
            locale.Country =  country;
            locale.Language =  language;
            locale.Timezone =  zone;
            var req = new Request
            {
                RequestType = RequestType.GetPlayer,
                RequestMessage = new GetPlayerMessage {
                    PlayerLocale = locale
                }.ToByteString()
            };
            return req;
        }

        public static Request GetDownloadSettingsMessageRequest(Client client)
        {
            var downloadSettingsMessage = new DownloadSettingsMessage
            {
                Hash = client.SettingsHash
            };
            return new Request
            {
                RequestType = RequestType.DownloadSettings,
                RequestMessage = downloadSettingsMessage.ToByteString()
            };
        }

        public static Request GetDefaultGetInventoryMessage(Client client)
        {
            var getInventoryMessage = new GetInventoryMessage
            {
                LastTimestampMs = client.InventoryLastUpdateTimestamp
            };
            return new Request
            {
                RequestType = RequestType.GetInventory,
                RequestMessage = getInventoryMessage.ToByteString()
            };
        }

        public static Request[] AppendCheckChallenge(Request request)
        {
            return new[]
            {
                request,
                new Request
                {
                    RequestType = RequestType.CheckChallenge,
                    RequestMessage = new CheckChallengeMessage().ToByteString()
                }
            };
        }
        public static Request[] FillRequest(Request request, Client client, bool appendBuddyWalked = true, bool appendInBox = true)
        {
            var requests = new List<Request>
            {
                request,
                new Request
                {
                    RequestType = RequestType.CheckChallenge,
                    RequestMessage = new CheckChallengeMessage().ToByteString()
                },
                new Request
                {
                    RequestType = RequestType.GetHatchedEggs,
                    RequestMessage = new GetHatchedEggsMessage().ToByteString()
                },
                GetDefaultGetInventoryMessage(client),
                new Request
                {
                    RequestType = RequestType.CheckAwardedBadges,
                    RequestMessage = new CheckAwardedBadgesMessage().ToByteString()
                },
                GetDownloadSettingsMessageRequest(client),
            };
            if (appendBuddyWalked){
                var reqBuddy = new Request
                {
                    RequestType = RequestType.GetBuddyWalked,
                    RequestMessage = new GetBuddyWalkedMessage().ToByteString()
                };
                requests.Add(reqBuddy);
            }
            if (appendInBox){
                var reqInbox = new Request
                {
                    RequestType = RequestType.GetInbox,
                    RequestMessage = new GetInboxMessage{
                        IsHistory = true,
                        IsReverse = false,
                        NotBeforeMs = 0
                    }.ToByteString()
                };
                requests.Add(reqInbox);
            }
            return requests.ToArray();
        }
        
        public static Request[] AddChallengeRequest(Request request, Client client)
        {
            return new[]
            {
                request,
                new Request
                {
                    RequestType = RequestType.CheckChallenge,
                    RequestMessage = new CheckChallengeMessage().ToByteString()
                }
            };
        }

        public static Request[] GetCommonRequests(Client client)
        {
            return new[]
            {
                new Request
                {
                    RequestType = RequestType.CheckChallenge,
                    RequestMessage = new CheckChallengeMessage().ToByteString()
                },
                new Request
                {
                    RequestType = RequestType.GetHatchedEggs,
                    RequestMessage = new GetHatchedEggsMessage().ToByteString()
                },
                GetDefaultGetInventoryMessage(client),
                new Request
                {
                    RequestType = RequestType.CheckAwardedBadges,
                    RequestMessage = new CheckAwardedBadgesMessage().ToByteString()
                },
                GetDownloadSettingsMessageRequest(client)
            };
        }

        public static void ProcessGetInventoryResponse(Client client, GetInventoryResponse getInventoryResponse)
        {
            if (getInventoryResponse == null)
                return;

            if (getInventoryResponse.Success)
            {
                if (getInventoryResponse.InventoryDelta == null)
                    return;
                if (client.Inventory.CachedInventory == null)
                    client.Inventory.CachedInventory = getInventoryResponse;
                else{
                    //TODO: polish update inventory
                    /*
                    var deletedPokemons = getInventoryResponse.InventoryDelta.InventoryItems.Select(i => i.DeletedItem).Where(x => x !=null );
                    foreach (var element in deletedPokemons) {
                        var cachedElement = client.Inventory.CachedInventory.InventoryDelta.InventoryItems.FirstOrDefault(x => x.InventoryItemData.PokemonData !=null && x.InventoryItemData.PokemonData.Id == element.PokemonId);
                        if (cachedElement !=null)
                            client.Inventory.CachedInventory.InventoryDelta.InventoryItems.Remove(cachedElement);
                    }
                    var newPokemons = getInventoryResponse.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PokemonData).Where(x => x !=null );
                    foreach (var element in deletedPokemons) {
                        var cachedElement = client.Inventory.CachedInventory.InventoryDelta.InventoryItems.FirstOrDefault(x => x.InventoryItemData.PokemonData !=null && x.InventoryItemData.PokemonData.Id == element.PokemonId);
                        if (cachedElement !=null)
                            client.Inventory.CachedInventory.InventoryDelta.InventoryItems.Remove(cachedElement);
                    }
                    */
                }
            }
        }

        public static void ProcessDownloadSettingsResponse(Client client, DownloadSettingsResponse downloadSettingsResponse)
        {
            if (downloadSettingsResponse == null)
                return;

            if (string.IsNullOrEmpty(downloadSettingsResponse.Error))
            {
                if (downloadSettingsResponse.Settings == null)
                    return;

                client.SettingsHash = downloadSettingsResponse.Hash;

                if (!string.IsNullOrEmpty(downloadSettingsResponse.Settings.MinimumClientVersion))
                {
                    client.MinimumClientVersion = new Version(downloadSettingsResponse.Settings.MinimumClientVersion);
                    if (client.CheckCurrentVersionOutdated())
                        throw new MinimumClientVersionException(client.CurrentApiEmulationVersion, client.MinimumClientVersion);
                }
            }
        }

        public static void ProcessCheckChallengeResponse(Client client, CheckChallengeResponse checkChallengeResponse)
        {
            if (checkChallengeResponse == null)
                return;

            if (checkChallengeResponse.ShowChallenge)
                client.ApiFailure.HandleCaptcha(checkChallengeResponse.ChallengeUrl, client);
        }

        public static void ProcessGetPlayerResponse(Client client, GetPlayerResponse getPlayerResponse)
        {
            if (getPlayerResponse == null)
                return;
            
            if (getPlayerResponse.Banned)
                Logger.Debug("Your account is banned");
            if (getPlayerResponse.Warn)
                Logger.Debug("Your account is flagged");
            if (getPlayerResponse.Success){
                client.Player.PlayerResponse = getPlayerResponse;
            }

        }

        public static void Parse(Client client, RequestType requestType, ByteString data)
        {
            try
            {
                switch (requestType)
                {
                    case RequestType.GetInventory:
                        //TODO Update inventory
                        //client..getInventories().updateInventories(GetInventoryResponse.parseFrom(data));

                        //var getInventoryResponse = new GetInventoryResponse();
                        //getInventoryResponse.MergeFrom(data);

                        // Update inventory timestamp
                        client.InventoryLastUpdateTimestamp = Utils.GetTime(true);

                        break;
                    case RequestType.DownloadSettings:
                        //TODO Update settings
                        //api.getSettings().updateSettings(DownloadSettingsResponse.parseFrom(data));

                        // Update settings hash
                        var downloadSettingsResponse = new DownloadSettingsResponse();
                        downloadSettingsResponse.MergeFrom(data);
                        client.SettingsHash = downloadSettingsResponse.Hash;

                        break;
                }
            }
            catch (InvalidProtocolBufferException e)
            {
                throw e;
            }
        }

        public static Request GetVerifyChallenge(string token)
        {
            return new Request
            {
                RequestType = RequestType.VerifyChallenge,
                RequestMessage = new VerifyChallengeMessage()
                {
                    Token = token
                }.ToByteString()
            };
        }
        
        public static void ProcessCheckAwardedBadgesResponse(Client client, CheckAwardedBadgesResponse response)
        {
            if (response == null)
                return;

            if (response.Success)
            {
                //TODO: do something valid with the information
                var i = 0;
                foreach (var element in response.AvatarTemplateIds) {
                    Logger.Debug($"AvatarTemplateId {i}: {element}");
                    i++;
                }
                i = 0;
                foreach (var element in response.AwardedBadgeLevels) {
                    Logger.Debug($"AwardedBadgeLevel {i}: {element}");
                    i++;
                }
                i = 0;
                foreach (var element in response.AwardedBadges) {
                    Logger.Debug($"AwardedBadge {i}: {element}");
                    i++;
                }
            }
        }
        public static void ProcessGetHatchedEggsResponse(Client client, GetHatchedEggsResponse response)
        {
            if (response == null)
                return;
            // TODO: 
            /*
            response.CandyAwarded;
            response.EggKmWalked;
            response.ExperienceAwarded;
            response.HatchedPokemon;
            response.PokemonId;
            response.StardustAwarded;
            */
             Logger.Debug("CandyAwarded:" +response.CandyAwarded);
             Logger.Debug("EggKmWalked:" +response.EggKmWalked);
             Logger.Debug("ExperienceAwarded:" +response.ExperienceAwarded);
             Logger.Debug("HatchedPokemon:" +response.HatchedPokemon);
             Logger.Debug("PokemonId:" +response.PokemonId);
             Logger.Debug("StardustAwarded:" +response.StardustAwarded);
        }
        public static void ProcessGetBuddyWalkedResponse(Client client, GetBuddyWalkedResponse response)
        {
            if (response == null)
                return;
            Logger.Debug("Success:" + response.Success);
            Logger.Debug("CandyEarnedCount:" +response.CandyEarnedCount);
            Logger.Debug("FamilyCandyId:" +response.FamilyCandyId);
        }
        
        public static void ProcessGetInboxResponse(Client client, GetInboxResponse response)
        {
            if (response == null)
                return;
            Logger.Debug("Result:" + response.Result);
            var i = 0;
            foreach (var element in response.Inbox.BuiltinVariables) {
                Logger.Debug($"BuiltinVariable {i}: {element}");
                i++;
            }
            i = 0;
            foreach (var element in response.Inbox.Notifications) {
                Logger.Debug($"Notification {i}: {element}");
                i++;
            }
        }

        public static void ProcessDownloadRemoteConfigVersionResponse(Client client, DownloadRemoteConfigVersionResponse response)
        {
            if (response == null)
                return;
             // TODO: do something with this information 
             Logger.Debug("Result:" +response.Result);
             if ( response.Result ==DownloadRemoteConfigVersionResponse.Types.Result.Success){
                 Logger.Debug("AssetDigestTimestampMs:" +response.AssetDigestTimestampMs);
                 Logger.Debug("ItemTemplatesTimestampMs:" +response.ItemTemplatesTimestampMs);
             }
        }

        public static void ProcessCommonResponses(Client client, RepeatedField <ByteString> responses , bool processBuddyWalked = true, bool processInBox = true) 
        {
            if (responses != null)
            {
                var checkChallengeResponse = new CheckChallengeResponse();
                if ( responses.Count > 1)
                {
                    checkChallengeResponse.MergeFrom(responses[1]);
                    CommonRequest.ProcessCheckChallengeResponse( client, checkChallengeResponse);
                }

                var getHatchedEggsResponse = new GetHatchedEggsResponse();
                if ( responses.Count > 2)
                {
                    getHatchedEggsResponse.MergeFrom(responses[2]);
                    CommonRequest.ProcessGetHatchedEggsResponse( client, getHatchedEggsResponse);
                }

                var getInventoryResponse = new GetInventoryResponse();
                if ( responses.Count > 3)
                {
                    getInventoryResponse.MergeFrom(responses[3]);
                    CommonRequest.ProcessGetInventoryResponse( client, getInventoryResponse);
                }

                var checkAwardedBadgesResponse = new CheckAwardedBadgesResponse();
                if ( responses.Count > 4)
                {
                    checkAwardedBadgesResponse.MergeFrom(responses[4]);
                    CommonRequest.ProcessCheckAwardedBadgesResponse( client, checkAwardedBadgesResponse);
                }

                var downloadSettingsResponse = new DownloadSettingsResponse();
                if ( responses.Count > 5)
                {
                    downloadSettingsResponse.MergeFrom(responses[5]);
                    CommonRequest.ProcessDownloadSettingsResponse( client, downloadSettingsResponse);
                }
                var index = 5;
                if (processBuddyWalked)
                {
                    index ++;
                    var getBuddyWalkedResponse = new GetBuddyWalkedResponse();
                    if ( responses.Count > index)
                    {
                        getBuddyWalkedResponse.MergeFrom(responses[index]);
                        CommonRequest.ProcessGetBuddyWalkedResponse( client, getBuddyWalkedResponse);
                    }
                }
                if (processInBox)
                {
                    index ++;
                    var getInboxResponse = new GetInboxResponse();

                    if ( responses.Count > index)
                    {
                        getInboxResponse.MergeFrom(responses[index]);
                        CommonRequest.ProcessGetInboxResponse( client, getInboxResponse);
                    }
                }
            }
        }

        public static Request GetPlayerProfileMessageRequest( string playername ="")
        {
            var req = new Request
            {
                RequestType = RequestType.GetPlayerProfile,
                RequestMessage = new GetPlayerProfileMessage {
                    PlayerName = playername
                }.ToByteString()
            };
            return req;
        }

        public static void ProcessGetPlayerProfileResponse(Client client, GetPlayerProfileResponse response)
        {
            if (response == null)
                return;
             // TODO: do something with this information 
             Logger.Debug("Result:" +response.Result);
             if ( response.Result == GetPlayerProfileResponse.Types.Result.Success){
                var i = 0;
                foreach (var element in response.Badges) {
                    Logger.Debug($"Badges {i}: {element}");
                    i++;
                }
                Logger.Debug("GymBadges: " +response.GymBadges);
                Logger.Debug("StartTime: " +response.StartTime);
             }
        }

        public static Request LevelUpRewardsMessageRequest( int level = 0)
        {
            var req = new Request
            {
                RequestType = RequestType.LevelUpRewards,
                RequestMessage = new LevelUpRewardsMessage {
                   Level = level
                }.ToByteString()
            };
            return req;
        }

        public static void ProcessLevelUpRewardsResponse(Client client, LevelUpRewardsResponse response)
        {
            if (response == null)
                return;
             // TODO: do something with this information 
             Logger.Debug("Result:" +response.Result);
             if ( response.Result == LevelUpRewardsResponse.Types.Result.Success){
                var i = 0;
                foreach (var element in response.AvatarTemplateIds) {
                    Logger.Debug($"AvatarTemplateIds {i}: {element}");
                    i++;
                }
                i = 0;
                foreach (var element in response.ItemsAwarded) {
                    Logger.Debug($"ItemsAwarded {i}: {element}");
                    i++;
                }
                i = 0;
                foreach (var element in response.ItemsUnlocked) {
                    Logger.Debug($"ItemsUnlocked {i}: {element}");
                    i++;
                }
             }
        }

        public static Request GetGetAssetDigestMessageRequest(Client client)
        {
            var getAssetDigestMessage = new GetAssetDigestMessage
            {
                Platform = client.Platform,
                AppVersion = client.AppVersion
            };
            return new Request
            {
                RequestType = RequestType.GetAssetDigest,
                RequestMessage = getAssetDigestMessage.ToByteString()
            };
        }

        public static void ProcessGetAssetDigestResponse(Client client, GetAssetDigestResponse response)
        {
            if (response == null)
                return;
            var i = 0;
            foreach (var element in response.Digest) {
                Logger.Debug($"Digest {i}: {element}");
                i++;
            }
            Logger.Debug("PageOffset:" +response.PageOffset);
            client.PageOffset = response.PageOffset;
            Logger.Debug("TimestampMs:" +response.TimestampMs);
        }
        
        public static Request DownloadItemTemplatesRequest(Client client)
        {
            var downloadItemTemplatesMessage = new DownloadItemTemplatesMessage
            {
                PageOffset = client.PageOffset
                
            };
            return new Request
            {
                RequestType = RequestType.DownloadItemTemplates,
                RequestMessage = downloadItemTemplatesMessage.ToByteString()
            };
        }
        public static void ProcessDownloadItemTemplatesResponse(Client client, DownloadItemTemplatesResponse response)
        {
            if (response == null)
                return;
            var i = 0;
            foreach (var element in response.ItemTemplates) {
                Logger.Debug($"ItemTemplate {i}: {element}");
                i++;
            }
            Logger.Debug("PageOffset:" +response.PageOffset);
            client.PageOffset = response.PageOffset;
            Logger.Debug("TimestampMs:" +response.TimestampMs);
        }
        
        public static Request GetDownloadUrlsRequest(Client client)
        {
            var getDownloadUrlsMessage = new GetDownloadUrlsMessage();
            // TODO: get asset ids from digest;
            // d.bundle_name == 'i18n_general, i18n_moves, i18n_items
            // getDownloadUrlsMessage.AssetId = new RepeatedField<string>();
            return new Request
            {
                RequestType = RequestType.GetDownloadUrls,
                RequestMessage = getDownloadUrlsMessage.ToByteString()
            };
        
        }
        public static void ProcessGetDownloadUrlsResponse(Client client, GetDownloadUrlsResponse response)
        {
            if (response == null)
                return;
            var i = 0;
            foreach (var element in response.DownloadUrls) {
                Logger.Debug($"DownloadUrl {i}: {element}");
                i++;
            }
        }
    }
}