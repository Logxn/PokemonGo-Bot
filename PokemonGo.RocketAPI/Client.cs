using static PokemonGo.RocketAPI.GeneratedCode.Response.Types;

namespace PokemonGo.RocketAPI
{
    using System;
    using System.Device.Location;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Protobuf;

    using PokemonGo.RocketAPI.Enums;
    using PokemonGo.RocketAPI.Exceptions;
    using PokemonGo.RocketAPI.Extensions;
    using PokemonGo.RocketAPI.GeneratedCode;
    using PokemonGo.RocketAPI.Helpers;
    using PokemonGo.RocketAPI.Login;

    public class Client
    {
        public static GetPlayerResponse _cachedProfile;

        public static DateTime _lastRefresh;
        private readonly HttpClient _httpClient;
        private readonly ISettings _settings;
        private string _apiUrl;
        private AuthType _authType = AuthType.Google;
        private Request.Types.UnknownAuth _unknownAuth;

        public Client(ISettings settings)
        {
            this._settings = settings;
            if (this._settings.UseLastCords)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
                if (!Directory.Exists(path))
                {
                    var di = Directory.CreateDirectory(path);
                }

                var fullPath = Path.Combine(path, "LastCoords.txt");
                if (File.Exists(fullPath) && File.ReadAllText(fullPath).Contains(":"))
                {
                    var latlngFromFile = File.ReadAllText(fullPath);
                    var latlng = latlngFromFile.Split(':');
                    double latitude, longitude;
                    if (latlng[0].Length > 0 && double.TryParse(latlng[0], out latitude) && latitude >= -90.0 && latitude <= 90.0 && latlng[1].Length > 0 && double.TryParse(latlng[1], out longitude) && longitude >= -180.0 && longitude <= 180.0)
                    {
                        var dif1 = latitude - this._settings.DefaultLatitude;
                        var dif2 = longitude - this._settings.DefaultLongitude;
                        if (dif1 > 3 || dif1 < -3 || dif2 > 3 || dif2 < -3)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "There is a difference of your Last Location File.\n" + path);
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Latidude difference: " + dif1 + " Longitude difference :" + dif2);
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Sleeping 10 Seconds.");
                            Thread.Sleep(10000);
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "I warned you. Starting..");
                        }

                        try
                        {
                            this.SetCoordinates(latitude, longitude, this._settings.DefaultAltitude);
                        }
                        catch (Exception)
                        {
                            this.SetCoordinates(this._settings.DefaultLatitude, this._settings.DefaultLongitude, this._settings.DefaultAltitude);
                        }
                    }
                    else
                    {
                        this.SetCoordinates(this._settings.DefaultLatitude, this._settings.DefaultLongitude, this._settings.DefaultAltitude);
                    }
                }
                else
                {
                    this.SetCoordinates(this._settings.DefaultLatitude, this._settings.DefaultLongitude, this._settings.DefaultAltitude);
                }
            }
            else
            {
                this.SetCoordinates(this._settings.DefaultLatitude, this._settings.DefaultLongitude, this._settings.DefaultAltitude);
            }

            // Setup HttpClient and create default headers
            var handler = new HttpClientHandler
                          {
                              AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                              AllowAutoRedirect = false
                          };
            this._httpClient = new HttpClient(new RetryHandler(handler));
            this._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Niantic App");

            // "Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-G900F Build/LMY48G)");
            this._httpClient.DefaultRequestHeaders.ExpectContinue = false;
            this._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            this._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            this._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
        }

        public string AccessToken
        {
            get;
            set;
        }

        public double CurrentAltitude
        {
            get;
            private set;
        }

        public double CurrentLat
        {
            get;
            private set;
        }

        public double CurrentLng
        {
            get;
            private set;
        }

        public async Task<CatchPokemonResponse> CatchPokemon(ulong encounterId, string spawnPointGuid, double pokemonLat, double pokemonLng, MiscEnums.Item pokeball)
        {
            var customRequest = new Request.Types.CatchPokemonRequest
                                {
                                    EncounterId = encounterId,
                                    Pokeball = (int) pokeball,
                                    SpawnPointGuid = spawnPointGuid,
                                    HitPokemon = 1,
                                    NormalizedReticleSize = Utils.FloatAsUlong(1.950),
                                    SpinModifier = Utils.FloatAsUlong(1),
                                    NormalizedHitPosition = Utils.FloatAsUlong(1)
                                };

            var catchPokemonRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                           {
                                                                                                                                               Type = (int) RequestType.CATCH_POKEMON,
                                                                                                                                               Message = customRequest.ToByteString()
                                                                                                                                           });
            return await this._httpClient.PostProtoPayload<Request, CatchPokemonResponse>($"https://{this._apiUrl}/rpc", catchPokemonRequest);
        }

        public async Task DoGoogleLogin()
        {
            this._authType = AuthType.Google;
            this.AccessToken = GoogleLoginGPSOAuth.DoLogin(this._settings.PtcUsername, this._settings.PtcPassword); // TempFix

            // GoogleLogin.TokenResponseModel tokenResponse = null;
            // if (_settings.GoogleRefreshToken != string.Empty)
            // {
            // tokenResponse = await GoogleLogin.GetAccessToken(_settings.GoogleRefreshToken);
            // AccessToken = tokenResponse?.id_token;
            // }

            // if (AccessToken == null)
            // {
            // var deviceCode = await GoogleLogin.GetDeviceCode();
            // tokenResponse = await GoogleLogin.GetAccessToken(deviceCode);
            // _settings.GoogleRefreshToken = tokenResponse?.refresh_token;
            // AccessToken = tokenResponse?.id_token;
            // }
        }

        public async Task DoPtcLogin(string username, string password)
        {
            this.AccessToken = await PtcLogin.GetAccessToken(username, password);
            this._authType = AuthType.Ptc;
        }

        public async Task<EncounterResponse> EncounterPokemon(ulong encounterId, string spawnPointGuid)
        {
            var customRequest = new Request.Types.EncounterRequest
                                {
                                    EncounterId = encounterId,
                                    SpawnpointId = spawnPointGuid,
                                    PlayerLatDegrees = Utils.FloatAsUlong(this.CurrentLat),
                                    PlayerLngDegrees = Utils.FloatAsUlong(this.CurrentLng)
                                };

            var encounterResponse = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                         {
                                                                                                                                             Type = (int) RequestType.ENCOUNTER,
                                                                                                                                             Message = customRequest.ToByteString()
                                                                                                                                         });
            return await this._httpClient.PostProtoPayload<Request, EncounterResponse>($"https://{this._apiUrl}/rpc", encounterResponse);
        }

        public async Task<EvolvePokemonOut> EvolvePokemon(ulong pokemonId)
        {
            var customRequest = new EvolvePokemon
                                {
                                    PokemonId = pokemonId
                                };

            var releasePokemonRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                             {
                                                                                                                                                 Type = (int) RequestType.EVOLVE_POKEMON,
                                                                                                                                                 Message = customRequest.ToByteString()
                                                                                                                                             });
            return await this._httpClient.PostProtoPayload<Request, EvolvePokemonOut>($"https://{this._apiUrl}/rpc", releasePokemonRequest);
        }

        public async Task<GetPlayerResponse> GetCachedProfile(bool request = false)
        {
            var now = DateTime.Now;
            var ss = new SemaphoreSlim(10);

            if (_lastRefresh.AddSeconds(30).Ticks > now.Ticks && request == false)
            {
                return _cachedProfile;
            }

            await ss.WaitAsync();

            try
            {
                _lastRefresh = now;
                try
                {
                    _cachedProfile = await this.GetProfile();
                }
                catch
                {
                }
            }
            finally
            {
                ss.Release();
            }

            return _cachedProfile;
        }

        public async Task<FortDetailsResponse> GetFort(string fortId, double fortLat, double fortLng)
        {
            var customRequest = new Request.Types.FortDetailsRequest
                                {
                                    Id = ByteString.CopyFromUtf8(fortId),
                                    Latitude = Utils.FloatAsUlong(fortLat),
                                    Longitude = Utils.FloatAsUlong(fortLng)
                                };

            var fortDetailRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                         {
                                                                                                                                             Type = (int) RequestType.FORT_DETAILS,
                                                                                                                                             Message = customRequest.ToByteString()
                                                                                                                                         });
            return await this._httpClient.PostProtoPayload<Request, FortDetailsResponse>($"https://{this._apiUrl}/rpc", fortDetailRequest);
        }

        public async Task<GetInventoryResponse> GetInventory()
        {
            var inventoryRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, RequestType.GET_INVENTORY);
            return await this._httpClient.PostProtoPayload<Request, GetInventoryResponse>($"https://{this._apiUrl}/rpc", inventoryRequest);
        }

        public async Task<DownloadItemTemplatesResponse> GetItemTemplates()
        {
            var settingsRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, 10, RequestType.DOWNLOAD_ITEM_TEMPLATES);
            return await this._httpClient.PostProtoPayload<Request, DownloadItemTemplatesResponse>($"https://{this._apiUrl}/rpc", settingsRequest);
        }

        public GeoCoordinate GetLocation()
        {
            return new GeoCoordinate(this.CurrentLat, this.CurrentLng);
        }

        public async Task<GetMapObjectsResponse> GetMapObjects()
        {
            var customRequest = new Request.Types.MapObjectsRequest
                                {
                                    CellIds = ByteString.CopyFrom(ProtoHelper.EncodeUlongList(S2Helper.GetNearbyCellIds(this.CurrentLng, this.CurrentLat))),
                                    Latitude = Utils.FloatAsUlong(this.CurrentLat),
                                    Longitude = Utils.FloatAsUlong(this.CurrentLng),
                                    Unknown14 = ByteString.CopyFromUtf8("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
                                };

            var mapRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, 10, new Request.Types.Requests
                                                                                                                {
                                                                                                                    Type = (int) RequestType.GET_MAP_OBJECTS,
                                                                                                                    Message = customRequest.ToByteString()
                                                                                                                }, new Request.Types.Requests
                                                                                                                   {
                                                                                                                       Type = (int) RequestType.GET_HATCHED_OBJECTS
                                                                                                                   }, new Request.Types.Requests
                                                                                                                      {
                                                                                                                          Type = (int) RequestType.GET_INVENTORY,
                                                                                                                          Message = new Request.Types.Time
                                                                                                                                    {
                                                                                                                                        Time_ = DateTime.UtcNow.ToUnixTime()
                                                                                                                                    }.ToByteString()
                                                                                                                      }, new Request.Types.Requests
                                                                                                                         {
                                                                                                                             Type = (int) RequestType.CHECK_AWARDED_BADGES
                                                                                                                         }, new Request.Types.Requests
                                                                                                                            {
                                                                                                                                Type = (int) RequestType.DOWNLOAD_SETTINGS,
                                                                                                                                Message = new Request.Types.SettingsGuid
                                                                                                                                          {
                                                                                                                                              Guid = ByteString.CopyFromUtf8("4a2e9bc330dae60e7b74fc85b98868ab4700802e")
                                                                                                                                          }.ToByteString()
                                                                                                                            });

            return await this._httpClient.PostProtoPayload<Request, GetMapObjectsResponse>($"https://{this._apiUrl}/rpc", mapRequest);
        }

        public async Task<string> getNickname()
        {
            var profil = await this.GetProfile();
            return profil.Profile.Username;
        }

        public async Task<GetPlayerResponse> GetProfile()
        {
            var profileRequest = RequestBuilder.GetInitialRequest(this.AccessToken, this._authType, this.CurrentLat, this.CurrentLng, 10, new Request.Types.Requests
                                                                                                                                          {
                                                                                                                                              Type = (int) RequestType.GET_PLAYER
                                                                                                                                          });
            return await this._httpClient.PostProtoPayload<Request, GetPlayerResponse>($"https://{this._apiUrl}/rpc", profileRequest);
        }

        public ISettings getSettingHandle()
        {
            return this._settings;
        }

        public async Task<DownloadSettingsResponse> GetSettings()
        {
            var settingsRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, 10, RequestType.DOWNLOAD_SETTINGS);
            return await this._httpClient.PostProtoPayload<Request, DownloadSettingsResponse>($"https://{this._apiUrl}/rpc", settingsRequest);
        }

        public async Task<EvolvePokemonOut> PowerUp(ulong pokemonId)
        {
            var customRequest = new EvolvePokemon
                                {
                                    PokemonId = pokemonId
                                };

            var releasePokemonRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                             {
                                                                                                                                                 Type = (int) RequestType.UPGRADE_POKEMON,
                                                                                                                                                 Message = customRequest.ToByteString()
                                                                                                                                             });
            return await this._httpClient.PostProtoPayload<Request, EvolvePokemonOut>($"https://{this._apiUrl}/rpc", releasePokemonRequest);
        }

        public async Task<RecycleInventoryItemResponse> RecycleItem(ItemId itemId, int amount)
        {
            var customRequest = new RecycleInventoryItem
                                {
                                    ItemId = (ItemId) Enum.Parse(typeof (ItemId), itemId.ToString()),
                                    Count = amount
                                };

            var releasePokemonRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                             {
                                                                                                                                                 Type = (int) RequestType.RECYCLE_INVENTORY_ITEM,
                                                                                                                                                 Message = customRequest.ToByteString()
                                                                                                                                             });
            return await this._httpClient.PostProtoPayload<Request, RecycleInventoryItemResponse>($"https://{this._apiUrl}/rpc", releasePokemonRequest);
        }

        public void SaveLatLng(double lat, double lng)
        {
            var latlng = lat + ":" + lng;
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Configs\\LastCoords.txt", latlng);
        }

        public async Task<FortSearchResponse> SearchFort(string fortId, double fortLat, double fortLng)
        {
            var customRequest = new Request.Types.FortSearchRequest
                                {
                                    Id = ByteString.CopyFromUtf8(fortId),
                                    FortLatDegrees = Utils.FloatAsUlong(fortLat),
                                    FortLngDegrees = Utils.FloatAsUlong(fortLng),
                                    PlayerLatDegrees = Utils.FloatAsUlong(this.CurrentLat),
                                    PlayerLngDegrees = Utils.FloatAsUlong(this.CurrentLng)
                                };

            var fortDetailRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                         {
                                                                                                                                             Type = (int) RequestType.FORT_SEARCH,
                                                                                                                                             Message = customRequest.ToByteString()
                                                                                                                                         });
            return await this._httpClient.PostProtoPayload<Request, FortSearchResponse>($"https://{this._apiUrl}/rpc", fortDetailRequest);
        }

        public async Task SetServer()
        {
            var serverRequest = RequestBuilder.GetInitialRequest(this.AccessToken, this._authType, this.CurrentLat, this.CurrentLng, 10, RequestType.GET_PLAYER, RequestType.GET_HATCHED_OBJECTS, RequestType.GET_INVENTORY, RequestType.CHECK_AWARDED_BADGES, RequestType.DOWNLOAD_SETTINGS);
            var serverResponse = await this._httpClient.PostProto(Resources.RpcUrl, serverRequest);

            if (serverResponse.Auth == null)
                throw new AccessTokenExpiredException();

            this._unknownAuth = new Request.Types.UnknownAuth
                                {
                                    Unknown71 = serverResponse.Auth.Unknown71,
                                    Timestamp = serverResponse.Auth.Timestamp,
                                    Unknown73 = serverResponse.Auth.Unknown73
                                };

            this._apiUrl = serverResponse.ApiUrl;
            if (this._apiUrl != string.Empty)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "We got the API Url: " + this._apiUrl);
            }
            else
            {
                Logger.Error("PokemonGo Servers are probably Offline.");
            }
        }

        public async Task<TransferPokemonOut> TransferPokemon(ulong pokemonId)
        {
            var customRequest = new TransferPokemon
                                {
                                    PokemonId = pokemonId
                                };

            var releasePokemonRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                             {
                                                                                                                                                 Type = (int) RequestType.RELEASE_POKEMON,
                                                                                                                                                 Message = customRequest.ToByteString()
                                                                                                                                             });
            return await this._httpClient.PostProtoPayload<Request, TransferPokemonOut>($"https://{this._apiUrl}/rpc", releasePokemonRequest);
        }

        public async Task<PlayerUpdateResponse> UpdatePlayerLocation(double lat, double lng, double alt)
        {
            this.SetCoordinates(lat, lng, alt);
            var customRequest = new Request.Types.PlayerUpdateProto
                                {
                                    Lat = Utils.FloatAsUlong(this.CurrentLat),
                                    Lng = Utils.FloatAsUlong(this.CurrentLng)
                                };

            var updateRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                     {
                                                                                                                                         Type = (int) RequestType.PLAYER_UPDATE,
                                                                                                                                         Message = customRequest.ToByteString()
                                                                                                                                     });
            var updateResponse = await this._httpClient.PostProtoPayload<Request, PlayerUpdateResponse>($"https://{this._apiUrl}/rpc", updateRequest);
            return updateResponse;
        }

        public async Task<UseItemCaptureRequest> UseCaptureItem(ulong encounterId, ItemId itemId, string spawnPointGuid)
        {
            var customRequest = new UseItemCaptureRequest
                                {
                                    EncounterId = encounterId,
                                    ItemId = itemId,
                                    SpawnPointGuid = spawnPointGuid
                                };

            var useItemRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, 30, new Request.Types.Requests
                                                                                                                    {
                                                                                                                        Type = (int) RequestType.USE_ITEM_CAPTURE,
                                                                                                                        Message = customRequest.ToByteString()
                                                                                                                    });
            return await this._httpClient.PostProtoPayload<Request, UseItemCaptureRequest>($"https://{this._apiUrl}/rpc", useItemRequest);
        }

        public async Task<UseItemRequest> UseItemIncense(ItemId itemId)
        {
            // changed from UseItem to UseItemXpBoost because of the RequestType
            var customRequest = new UseItemRequest
                                {
                                    ItemId = itemId
                                };

            var useItemRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                      {
                                                                                                                                          Type = (int) RequestType.USE_INCENSE,
                                                                                                                                          Message = customRequest.ToByteString()
                                                                                                                                      });
            return await this._httpClient.PostProtoPayload<Request, UseItemRequest>($"https://{this._apiUrl}/rpc", useItemRequest);
        }

        public async Task<UseItemRequest> UseItemXpBoost(ItemId itemId)
        {
            // changed from UseItem to UseItemXpBoost because of the RequestType
            var customRequest = new UseItemRequest
                                {
                                    ItemId = itemId
                                };

            var useItemRequest = RequestBuilder.GetRequest(this._unknownAuth, this.CurrentLat, this.CurrentLng, this.CurrentAltitude, new Request.Types.Requests
                                                                                                                                      {
                                                                                                                                          Type = (int) RequestType.USE_ITEM_XP_BOOST,
                                                                                                                                          Message = customRequest.ToByteString()
                                                                                                                                      });
            return await this._httpClient.PostProtoPayload<Request, UseItemRequest>($"https://{this._apiUrl}/rpc", useItemRequest);
        }

        private void SetCoordinates(double lat, double lng, double altitude)
        {
            this.CurrentLat = lat;
            this.CurrentLng = lng;
            this.CurrentAltitude = altitude;
            if (this._settings.UseLastCords)
            {
                this.SaveLatLng(lat, lng);
            }
        }
    }
}