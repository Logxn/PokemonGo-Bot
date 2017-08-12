#region using directives

using System;
using System.Threading.Tasks;
using Google.Protobuf;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using PokemonGo.RocketAPI;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public class BaseRpc
    {
        protected Client Client;
        

        protected BaseRpc(Client client)
        {
            Client = client;
        }

        protected RequestBuilder GetRequestBuilder()
        {
            return new RequestBuilder(Client, Client.AuthToken, Client.AuthType, Client.CurrentLatitude, Client.CurrentLongitude,
                    Client.CurrentAltitude, Client.AuthTicket);
        }

        protected TResponsePayload PostProtoPayload<TRequest, TResponsePayload>(RequestEnvelope.Types.PlatformRequest platfReq )
            where TRequest : IMessage<TRequest>
            where TResponsePayload : IMessage<TResponsePayload>, new()
        {
            var tries = 0;
            while ( tries <3){
                try {
                    Logger.Debug("Before of GetPlatformRequestEnvelope: platfReq :"+ platfReq);
                    var requestEnvelops = GetRequestBuilder().GetPlatformRequestEnvelope(platfReq);
                    return Client.PokemonHttpClient.PostProtoPayload<TRequest, TResponsePayload>(Client.ApiUrl, requestEnvelops,
                                Client.ApiFailure);
                } catch (AccessTokenExpiredException) {
                    Logger.Warning("Invalid Token. Retrying in 1 second");
                    Task.Delay(1000).Wait();
                    Client.Login.Reauthenticate().Wait();
                } catch (InvalidPlatformException) {
                    Logger.Warning("Invalid Platform. Retrying in 1 second");
                    Task.Delay(1000).Wait();
                    Client.Login.DoLogin().Wait();
                } catch (RedirectException) {
                    Task.Delay(1000).Wait();
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return new TResponsePayload();
        }

        protected TResponsePayload PostProtoPayload<TRequest, TResponsePayload>(RequestType type,
            IMessage message) where TRequest : IMessage<TRequest>
            where TResponsePayload : IMessage<TResponsePayload>, new()
        {
            var tries = 0;
            while ( tries <3){
                try {
                    Logger.Debug("Before of GetRequestEnvelope: type :"+ type);

                    var requestEnvelops = GetRequestBuilder().GetRequestEnvelope(type, message);
                    return Client.PokemonHttpClient.PostProtoPayload<TRequest, TResponsePayload>(Client.ApiUrl, requestEnvelops,
                                Client.ApiFailure);
                } catch (AccessTokenExpiredException) {
                    Logger.Warning("Invalid Token. Retrying in 1 second");
                    Task.Delay(1000).Wait();
                    Client.Login.Reauthenticate().Wait();
                } catch (InvalidPlatformException) {
                    Logger.Warning("Invalid Platform. Retrying in 1 second");
                    Client.Login.DoLogin().Wait();
                    Task.Delay(1000).Wait();
                } catch (RedirectException) {
                    Task.Delay(1000).Wait();
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return new TResponsePayload();
        }

        protected  TResponsePayload PostProtoPayload<TRequest, TResponsePayload>(
            RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
            where TResponsePayload : IMessage<TResponsePayload>, new()
        {
            return
                    Client.PokemonHttpClient.PostProtoPayload<TRequest, TResponsePayload>(Client.ApiUrl, requestEnvelope,
                        Client.ApiFailure);
        }

        protected async Task<Tuple<T1, T2>> PostProtoPayload<TRequest, T1, T2>(RequestEnvelope requestEnvelope)
            where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
        {
            var responses = await PostProtoPayload<TRequest>(requestEnvelope, typeof(T1), typeof(T2)).ConfigureAwait(false);
            return new Tuple<T1, T2>(responses[0] as T1, responses[1] as T2);
        }

        protected async Task<Tuple<T1, T2, T3>> PostProtoPayload<TRequest, T1, T2, T3>(RequestEnvelope requestEnvelope)
            where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
        {
            var responses = await PostProtoPayload<TRequest>(requestEnvelope, typeof(T1), typeof(T2), typeof(T3)).ConfigureAwait(false);
            return new Tuple<T1, T2, T3>(responses[0] as T1, responses[1] as T2, responses[2] as T3);
        }

        protected async Task<Tuple<T1, T2, T3, T4>> PostProtoPayload<TRequest, T1, T2, T3, T4>(
            RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
            where T4 : class, IMessage<T4>, new()
        {
            var responses =
                await PostProtoPayload<TRequest>(requestEnvelope, typeof(T1), typeof(T2), typeof(T3), typeof(T4)).ConfigureAwait(false);
            return new Tuple<T1, T2, T3, T4>(responses[0] as T1, responses[1] as T2, responses[2] as T3,
                responses[3] as T4);
        }

        protected async Task<Tuple<T1, T2, T3, T4, T5>> PostProtoPayload<TRequest, T1, T2, T3, T4, T5>(
            RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
            where T4 : class, IMessage<T4>, new()
            where T5 : class, IMessage<T5>, new()
        {
            var responses =
                await
                    PostProtoPayload<TRequest>(requestEnvelope, typeof(T1), typeof(T2), typeof(T3), typeof(T4),
                        typeof(T5)).ConfigureAwait(false);
            return new Tuple<T1, T2, T3, T4, T5>(responses[0] as T1, responses[1] as T2, responses[2] as T3,
                responses[3] as T4, responses[4] as T5);
        }

        protected async Task<Tuple<T1, T2, T3, T4, T5, T6>> PostProtoPayload<TRequest, T1, T2, T3, T4, T5, T6>(
            RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
            where T4 : class, IMessage<T4>, new()
            where T5 : class, IMessage<T5>, new()
            where T6 : class, IMessage<T6>, new()
        {
            var responses =
                await
                    PostProtoPayload<TRequest>(requestEnvelope, typeof(T1), typeof(T2), typeof(T3), typeof(T4),
                        typeof(T5), typeof(T6)).ConfigureAwait(false);
            return new Tuple<T1, T2, T3, T4, T5, T6>(responses[0] as T1, responses[1] as T2, responses[2] as T3,
                responses[3] as T4, responses[4] as T5, responses[5] as T6);
        }

        protected async Task<Tuple<T1, T2, T3, T4, T5, T6, T7>> PostProtoPayload<TRequest, T1, T2, T3, T4, T5, T6, T7>(
            RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
            where T4 : class, IMessage<T4>, new()
            where T5 : class, IMessage<T5>, new()
            where T6 : class, IMessage<T6>, new()
            where T7 : class, IMessage<T7>, new()
        {
            var responses =
                await
                    PostProtoPayload<TRequest>(requestEnvelope, typeof(T1), typeof(T2), typeof(T3), typeof(T4),
                        typeof(T5), typeof(T6), typeof(T7)).ConfigureAwait(false);
            return new Tuple<T1, T2, T3, T4, T5, T6, T7>(responses[0] as T1, responses[1] as T2, responses[2] as T3,
                responses[3] as T4, responses[4] as T5, responses[5] as T6, responses[6] as T7);
        }

        protected async Task<IMessage[]> PostProtoPayload<TRequest>(RequestEnvelope requestEnvelope,
            params Type[] responseTypes) where TRequest : IMessage<TRequest>
        {
            return
                await
                    Client.PokemonHttpClient.PostProtoPayload<TRequest>(Client.ApiUrl, requestEnvelope,
                        Client.ApiFailure, responseTypes).ConfigureAwait(false);
        }

        public async Task<T1> PostProtoPayloadCommonR<TRequest,T1>(RequestType type, IMessage message) 
            where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
        {
            return await PostProtoPayloadCommonR<Request, T1> (new Request {
                    RequestType = type,
                    RequestMessage = message.ToByteString()
            }).ConfigureAwait(false);
        }
        
        public async Task<T1> PostProtoPayloadCommonR<TRequest,T1>(Request request) 
            where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
        {
            var tries = 0;
            while ( tries <3){
                try {
                    Logger.Debug("Before of GetRequestEnvelope: request :"+ request);
                       var requestEnvelope = GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(request, Client));
                        var response =
                            await
                            PostProtoPayload<TRequest>(requestEnvelope,typeof(T1)
                                        , typeof(CheckChallengeResponse), typeof(GetHatchedEggsResponse)
                                        , typeof(GetInventoryResponse), typeof(CheckAwardedBadgesResponse)
                                        , typeof(DownloadSettingsResponse), typeof(GetBuddyWalkedResponse), typeof(GetInboxResponse)
                                                      ).ConfigureAwait(false);
                        CommonRequest.ProcessCheckChallengeResponse(Client, response[1] as  CheckChallengeResponse);
                        CommonRequest.ProcessGetHatchedEggsResponse(Client, response[2] as  GetHatchedEggsResponse);
                        CommonRequest.ProcessGetInventoryResponse(Client, response[3] as  GetInventoryResponse);
                        CommonRequest.ProcessCheckAwardedBadgesResponse(Client, response[4] as  CheckAwardedBadgesResponse);
                        CommonRequest.ProcessDownloadSettingsResponse(Client, response[5] as  DownloadSettingsResponse);
                        CommonRequest.ProcessGetBuddyWalkedResponse(Client, response[6] as  GetBuddyWalkedResponse);
                        CommonRequest.ProcessGetInboxResponse(Client, response[7] as  GetInboxResponse);
                        return response[0] as T1 ;
                } catch (AccessTokenExpiredException) {
                    Logger.Warning("Invalid Token. Retrying in 1 second");
                    await Task.Delay(1000).ConfigureAwait(false);
                    await Client.Login.Reauthenticate().ConfigureAwait(false);
                } catch (InvalidPlatformException) {
                    Logger.Warning("Invalid Platform. Retrying in 1 second");
                    await Client.Login.DoLogin().ConfigureAwait(false);
                    await Task.Delay(1000).ConfigureAwait(false);
                } catch (RedirectException) {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return null as T1;
        }

        protected async Task<ResponseEnvelope> PostProto<TRequest>(RequestEnvelope requestEnvelope)
            where TRequest : IMessage<TRequest>
        {
            return await Client.PokemonHttpClient.PerformThrottledRemoteProcedureCall<TRequest>(Client.ApiUrl, requestEnvelope).ConfigureAwait(false);
        }
    }
}