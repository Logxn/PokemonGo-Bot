﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Login;
using static PokemonGo.RocketAPI.GeneratedCode.Response.Types;
using System.Device.Location;
using PokemonGo.RocketAPI.Exceptions;
using System.IO;
using System.Linq;
using System.Threading;

namespace PokemonGo.RocketAPI
{

    public class Client
    {
        private readonly ISettings _settings;
        private readonly HttpClient _httpClient;
        private AuthType _authType = AuthType.Google;
        public string AccessToken { get; set; }
        private string _apiUrl;
        private Request.Types.UnknownAuth _unknownAuth;

        public double CurrentLat { get; private set; }
        public double CurrentLng { get; private set; }
        public double CurrentAltitude { get; private set; }

        public Client(ISettings settings)
        {
            _settings = settings;
            if (_settings.UseLastCords)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }

                string fullPath = Path.Combine(path, "LastCoords.txt");
                if (File.Exists(fullPath) && File.ReadAllText(fullPath).Contains(":"))
                {
                    var latlngFromFile = File.ReadAllText(fullPath);
                    var latlng = latlngFromFile.Split(':');
                    double latitude, longitude;
                    if ((latlng[0].Length > 0 && double.TryParse(latlng[0], out latitude) && latitude >= -90.0 &&
                         latitude <= 90.0) &&
                        (latlng[1].Length > 0 && double.TryParse(latlng[1], out longitude) && longitude >= -180.0 &&
                         longitude <= 180.0))
                    {

                        double dif1 = latitude - _settings.DefaultLatitude;
                        double dif2 = longitude - _settings.DefaultLongitude;
                        if (dif1 > 3 || dif1 < -3 || dif2 > 3 || dif2 < -3)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red,
                                "There is a difference of your Last Location File.\n" + path);
                            Logger.ColoredConsoleWrite(ConsoleColor.Red,
                                "Latidude difference: " + dif1 + " Longitude difference :" + dif2);
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Sleeping 10 Seconds.");
                            Thread.Sleep(10000);
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "I warned you. Starting..");

                        }

                        try
                        {
                            SetCoordinates(latitude, longitude, _settings.DefaultAltitude);
                        }
                        catch (Exception)
                        {
                            SetCoordinates(_settings.DefaultLatitude, _settings.DefaultLongitude,
                                _settings.DefaultAltitude);
                        }
                    }
                    else
                    {
                        SetCoordinates(_settings.DefaultLatitude, _settings.DefaultLongitude, _settings.DefaultAltitude);
                    }
                }
                else
                {
                    SetCoordinates(_settings.DefaultLatitude, _settings.DefaultLongitude, _settings.DefaultAltitude);
                }
            }
            else
            {
                SetCoordinates(_settings.DefaultLatitude, _settings.DefaultLongitude, _settings.DefaultAltitude);
            }

            //Setup HttpClient and create default headers
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false
            };
            _httpClient = new HttpClient(new RetryHandler(handler));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Niantic App");
            //"Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-G900F Build/LMY48G)");
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                "application/x-www-form-urlencoded");
        }

        public ISettings getSettingHandle()
        {
            return _settings;
        }

        private void SetCoordinates(double lat, double lng, double altitude)
        {
            CurrentLat = lat;
            CurrentLng = lng;
            CurrentAltitude = altitude;
            if (_settings.UseLastCords)
            {
                SaveLatLng(lat, lng);
            }
        }

        public void SaveLatLng(double lat, double lng)
        {
            string latlng = lat.ToString() + ":" + lng.ToString();
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Configs\\LastCoords.txt", latlng);
        }

        public void DoGoogleLogin()
        {
            _authType = AuthType.Google;
            AccessToken = GoogleLoginGPSOAuth.DoLogin(_settings.PtcUsername, _settings.PtcPassword); // TempFix 

            //GoogleLogin.TokenResponseModel tokenResponse = null;
            //if (_settings.GoogleRefreshToken != string.Empty)
            //{
            //    tokenResponse = await GoogleLogin.GetAccessToken(_settings.GoogleRefreshToken);
            //    AccessToken = tokenResponse?.id_token;
            //}

            //if (AccessToken == null)
            //{
            //    var deviceCode = await GoogleLogin.GetDeviceCode();
            //    tokenResponse = await GoogleLogin.GetAccessToken(deviceCode);
            //    _settings.GoogleRefreshToken = tokenResponse?.refresh_token;
            //    AccessToken = tokenResponse?.id_token;
            //}
        }

        public GeoCoordinate GetLocation()
        {
            return new GeoCoordinate(CurrentLat, CurrentLng);
        }

        public async Task DoPtcLogin(string username, string password)
        {
            AccessToken = await PtcLogin.GetAccessToken(username, password);
            _authType = AuthType.Ptc;
        }

        public async Task<PlayerUpdateResponse> UpdatePlayerLocation(double lat, double lng, double alt)
        {
            this.SetCoordinates(lat, lng, alt);
            var customRequest = new Request.Types.PlayerUpdateProto
            {
                Lat = Utils.FloatAsUlong(CurrentLat),
                Lng = Utils.FloatAsUlong(CurrentLng)
            };

            var updateRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.PLAYER_UPDATE,
                    Message = customRequest.ToByteString()
                });
            var updateResponse =
                await
                    _httpClient.PostProtoPayload<Request, PlayerUpdateResponse>($"https://{_apiUrl}/rpc", updateRequest);
            return updateResponse;
        }



        public async Task SetServer()
        {
            var serverRequest = RequestBuilder.GetInitialRequest(AccessToken, _authType, CurrentLat, CurrentLng, 10,
                RequestType.GET_PLAYER, RequestType.GET_HATCHED_OBJECTS, RequestType.GET_INVENTORY,
                RequestType.CHECK_AWARDED_BADGES, RequestType.DOWNLOAD_SETTINGS);
            var serverResponse = await _httpClient.PostProto<Request>(Resources.RpcUrl, serverRequest);

            if (serverResponse.Auth == null)
                throw new AccessTokenExpiredException();

            _unknownAuth = new Request.Types.UnknownAuth
            {
                Unknown71 = serverResponse.Auth.Unknown71,
                Timestamp = serverResponse.Auth.Timestamp,
                Unknown73 = serverResponse.Auth.Unknown73,
            };

            _apiUrl = serverResponse.ApiUrl;
            if (_apiUrl != "")
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "We got the API Url: " + _apiUrl);
            }
            else
            {
                Logger.Error("PokemonGo Servers are probably Offline.");
            }
        }

        public static DateTime _lastRefresh;
        public static GetPlayerResponse _cachedProfile;

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
                    _cachedProfile = await GetProfile();
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

        public async Task<GetPlayerResponse> GetProfile()
        {
            var profileRequest = RequestBuilder.GetInitialRequest(AccessToken, _authType, CurrentLat, CurrentLng, 10,
                new Request.Types.Requests { Type = (int)RequestType.GET_PLAYER });
            return
                await _httpClient.PostProtoPayload<Request, GetPlayerResponse>($"https://{_apiUrl}/rpc", profileRequest);
        }

        public async Task<string> getNickname()
        {
            var profil = await GetProfile();
            return profil.Profile.Username;
        }

        public async Task<DownloadSettingsResponse> GetSettings()
        {
            var settingsRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, 10,
                RequestType.DOWNLOAD_SETTINGS);
            return
                await
                    _httpClient.PostProtoPayload<Request, DownloadSettingsResponse>($"https://{_apiUrl}/rpc",
                        settingsRequest);
        }

        public async Task<DownloadItemTemplatesResponse> GetItemTemplates()
        {
            var settingsRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, 10,
                RequestType.DOWNLOAD_ITEM_TEMPLATES);
            return
                await
                    _httpClient.PostProtoPayload<Request, DownloadItemTemplatesResponse>($"https://{_apiUrl}/rpc",
                        settingsRequest);
        }

        public async Task<UseItemCaptureRequest> UseCaptureItem(ulong encounterId, ItemId itemId, string spawnPointGuid)
        {
            var customRequest = new UseItemCaptureRequest
            {
                EncounterId = encounterId,
                ItemId = itemId,
                SpawnPointGuid = spawnPointGuid
            };

            var useItemRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, 30,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.USE_ITEM_CAPTURE,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, UseItemCaptureRequest>($"https://{_apiUrl}/rpc",
                        useItemRequest);
        }

        public async Task<GetMapObjectsResponse> GetMapObjects()
        {
            var customRequest = new Request.Types.MapObjectsRequest
            {
                CellIds =
                    ByteString.CopyFrom(
                        ProtoHelper.EncodeUlongList(S2Helper.GetNearbyCellIds(CurrentLng,
                            CurrentLat))),
                Latitude = Utils.FloatAsUlong(CurrentLat),
                Longitude = Utils.FloatAsUlong(CurrentLng),
                Unknown14 = ByteString.CopyFromUtf8("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
            };

            var mapRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, 10,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.GET_MAP_OBJECTS,
                    Message = customRequest.ToByteString()
                },
                new Request.Types.Requests { Type = (int)RequestType.GET_HATCHED_OBJECTS },
                new Request.Types.Requests
                {
                    Type = (int)RequestType.GET_INVENTORY,
                    Message = new Request.Types.Time { Time_ = DateTime.UtcNow.ToUnixTime() }.ToByteString()
                },
                new Request.Types.Requests { Type = (int)RequestType.CHECK_AWARDED_BADGES },
                new Request.Types.Requests
                {
                    Type = (int)RequestType.DOWNLOAD_SETTINGS,
                    Message =
                        new Request.Types.SettingsGuid
                        {
                            Guid = ByteString.CopyFromUtf8("b1f2bf509a025b7cd76e1c484e2a24411c50f061")
                        }.ToByteString()
                });

            return
                await _httpClient.PostProtoPayload<Request, GetMapObjectsResponse>($"https://{_apiUrl}/rpc", mapRequest);
        }

        public async Task<FortDetailsResponse> GetFort(string fortId, double fortLat, double fortLng)
        {
            var customRequest = new Request.Types.FortDetailsRequest
            {
                Id = ByteString.CopyFromUtf8(fortId),
                Latitude = Utils.FloatAsUlong(fortLat),
                Longitude = Utils.FloatAsUlong(fortLng)
            };

            var fortDetailRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.FORT_DETAILS,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, FortDetailsResponse>($"https://{_apiUrl}/rpc",
                        fortDetailRequest);
        }

        public async Task<FortSearchResponse> SearchFort(string fortId, double fortLat, double fortLng)
        {
            var customRequest = new Request.Types.FortSearchRequest
            {
                Id = ByteString.CopyFromUtf8(fortId),
                FortLatDegrees = Utils.FloatAsUlong(fortLat),
                FortLngDegrees = Utils.FloatAsUlong(fortLng),
                PlayerLatDegrees = Utils.FloatAsUlong(CurrentLat),
                PlayerLngDegrees = Utils.FloatAsUlong(CurrentLng)
            };

            var fortDetailRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.FORT_SEARCH,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, FortSearchResponse>($"https://{_apiUrl}/rpc",
                        fortDetailRequest);
        }

        public async Task<EncounterResponse> EncounterPokemon(ulong encounterId, string spawnPointGuid)
        {
            var customRequest = new Request.Types.EncounterRequest
            {
                EncounterId = encounterId,
                SpawnpointId = spawnPointGuid,
                PlayerLatDegrees = Utils.FloatAsUlong(CurrentLat),
                PlayerLngDegrees = Utils.FloatAsUlong(CurrentLng)
            };

            var encounterResponse = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.ENCOUNTER,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, EncounterResponse>($"https://{_apiUrl}/rpc", encounterResponse);
        }

        public async Task<CatchPokemonResponse> CatchPokemon(ulong encounterId, string spawnPointGuid, double pokemonLat,
            double pokemonLng, MiscEnums.Item pokeball)
        {

            var customRequest = new Request.Types.CatchPokemonRequest
            {
                EncounterId = encounterId,
                Pokeball = (int)pokeball,
                SpawnPointGuid = spawnPointGuid,
                HitPokemon = 1,
                NormalizedReticleSize = Utils.FloatAsUlong(1.950),
                SpinModifier = Utils.FloatAsUlong(1),
                NormalizedHitPosition = Utils.FloatAsUlong(1)
            };

            var catchPokemonRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.CATCH_POKEMON,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, CatchPokemonResponse>($"https://{_apiUrl}/rpc",
                        catchPokemonRequest);
        }

        public async Task<UseItemRequest> UseItemXpBoost(ItemId itemId)
        {
            var customRequest = new UseItemRequest
            {
                ItemId = itemId,
            };

            var useItemRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.USE_ITEM_XP_BOOST,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, UseItemRequest>($"https://{_apiUrl}/rpc",
                        useItemRequest);
        }

        public async Task<UseItemRequest> UseItemIncense(ItemId itemId)
        //changed from UseItem to UseItemXpBoost because of the RequestType
        {
            var customRequest = new UseItemRequest
            {
                ItemId = itemId,
            };

            var useItemRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.USE_INCENSE,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, UseItemRequest>($"https://{_apiUrl}/rpc",
                        useItemRequest);
        }

        public async Task<TransferPokemonOut> TransferPokemon(ulong pokemonId)
        {
            var customRequest = new TransferPokemon
            {
                PokemonId = pokemonId
            };

            var releasePokemonRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.RELEASE_POKEMON,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, TransferPokemonOut>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest);
        }

        public async Task<EvolvePokemonOut> EvolvePokemon(ulong pokemonId)
        {
            var customRequest = new EvolvePokemon
            {
                PokemonId = pokemonId
            };

            var releasePokemonRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.EVOLVE_POKEMON,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, EvolvePokemonOut>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest);
        }

        public async Task<GetInventoryResponse> GetInventory()
        {
            var inventoryRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                RequestType.GET_INVENTORY);
            return
                await
                    _httpClient.PostProtoPayload<Request, GetInventoryResponse>($"https://{_apiUrl}/rpc",
                        inventoryRequest);
        }

        public async Task<RecycleInventoryItemResponse> RecycleItem(ItemId itemId, int amount)
        {
            var customRequest = new RecycleInventoryItem
            {
                ItemId = (ItemId)Enum.Parse(typeof(ItemId), itemId.ToString()),
                Count = amount
            };

            var releasePokemonRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.RECYCLE_INVENTORY_ITEM,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, RecycleInventoryItemResponse>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest);
        }

        public async Task<EvolvePokemonOut> PowerUp(ulong pokemonId)
        {
            var customRequest = new EvolvePokemon
            {
                PokemonId = pokemonId
            };

            var releasePokemonRequest = RequestBuilder.GetRequest(_unknownAuth, CurrentLat, CurrentLng, CurrentAltitude,
                new Request.Types.Requests
                {
                    Type = (int)RequestType.UPGRADE_POKEMON,
                    Message = customRequest.ToByteString()
                });
            return
                await
                    _httpClient.PostProtoPayload<Request, EvolvePokemonOut>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest);
        }

        public async Task Incubate(float kmWalked, List<EggIncubator> incubators, List<PokemonData> unusedEggs, List<PokemonData> pokemons)
        {
            await Task.Delay(0);
            foreach (var incubator in incubators)
            {
                if (incubator.PokemonId == 0)
                {
                    // Unlimited incubators prefer short eggs, limited incubators prefer long eggs
                    var egg = incubator.ItemId == ItemId.ItemIncubatorBasicUnlimited.ToString()
                        ? unusedEggs.FirstOrDefault()
                        : unusedEggs.LastOrDefault();

                    if (egg == null)
                    {
                        continue;
                    }

                    //TODO: Use incubator.Id with egg.Id
                    unusedEggs.Remove(egg);
                }
                else
                {
                    // Wird gerade gebrütet
                    var kmToWalk = incubator.TargetKmWalked - incubator.StartKmWalked;
                    var kmRemaining = incubator.TargetKmWalked - kmWalked;
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"Incubator {incubator.ItemId} needs {kmRemaining.ToString("N2")}km/{kmToWalk.ToString("N2")}km to hatch.");
                }
            }
        }
    }
}
