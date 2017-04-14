using System;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Exceptions;
using POGOProtos.Networking.Envelopes;
using System.Collections.Concurrent;
using System.Threading;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Extensions
{
    public enum ApiOperation
    {
        Retry,
        Abort
    }

    public interface IApiFailureStrategy
    {
        ApiOperation HandleApiFailure(RequestEnvelope request, ResponseEnvelope response);
        
        void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response);

        void HandleCaptcha(string challengeUrl, ICaptchaResponseHandler captchaResponseHandler);
    }

    public interface ICaptchaResponseHandler
    {
        void SetCaptchaToken(string captchaToken);
    }

    public static class HttpClientExtensions
    {
        public static async Task<IMessage[]> PostProtoPayload<TRequest>(this System.Net.Http.HttpClient client,
            string url, RequestEnvelope requestEnvelope,
            IApiFailureStrategy strategy,
            params Type[] responseTypes) where TRequest : IMessage<TRequest>
        {
            var result = new IMessage[responseTypes.Length];
            for (var i = 0; i < responseTypes.Length; i++)
            {
                result[i] = Activator.CreateInstance(responseTypes[i]) as IMessage;
                if (result[i] == null)
                {
                    throw new ArgumentException($"ResponseType {i} is not an IMessage");
                }
            }

            ResponseEnvelope response;
            while ((response = await PerformThrottledRemoteProcedureCall<TRequest>(client, url, requestEnvelope).ConfigureAwait(false)).Returns.Count !=
                   responseTypes.Length)
            {
                var operation = strategy.HandleApiFailure(requestEnvelope, response);
                if (operation == ApiOperation.Abort)
                {
                    throw new InvalidResponseException(
                        $"Expected {responseTypes.Length} responses, but got {response.Returns.Count} responses");
                }
            }

            strategy.HandleApiSuccess(requestEnvelope, response);

            for (var i = 0; i < responseTypes.Length; i++)
            {
                var payload = response.Returns[i];
                result[i].MergeFrom(payload);
            }
            return result;
        }

        public static TResponsePayload PostProtoPayload<TRequest, TResponsePayload>(
            this System.Net.Http.HttpClient client,
            string url, RequestEnvelope requestEnvelope, IApiFailureStrategy strategy)
            where TRequest : IMessage<TRequest>
            where TResponsePayload : IMessage<TResponsePayload>, new()
        {
            var response = PerformThrottledRemoteProcedureCall<TRequest>(client, url, requestEnvelope).Result;

            while (response.Returns.Count == 0)
            {
                Logger.Debug("Handling Failure");
                var operation = strategy.HandleApiFailure(requestEnvelope, response);

                if (operation == ApiOperation.Abort)
                    break;

                response = PerformThrottledRemoteProcedureCall<TRequest>(client, url, requestEnvelope).Result;
            }

            if (response.Returns.Count == 0)
                throw new InvalidResponseException();

            strategy.HandleApiSuccess(requestEnvelope, response);

            //Decode payload
            //TODO: multi-payload support
            var payload = response.Returns[0];
            var parsedPayload = new TResponsePayload();
            parsedPayload.MergeFrom(payload);

            return parsedPayload;
        }

        private static async Task<ResponseEnvelope> PerformRemoteProcedureCall<TRequest>(this System.Net.Http.HttpClient client,
            string url,
            RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
        {
            //Encode payload and put in envelope, then send
            var data = requestEnvelope.ToByteString();
            var result = await client.PostAsync(url, new ByteArrayContent(data.ToByteArray())).ConfigureAwait(false);

            //Decode message
            var responseData = await result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var codedStream = new CodedInputStream(responseData);
            var decodedResponse = new ResponseEnvelope();
            decodedResponse.MergeFrom(codedStream);
            Logger.Debug("requestEnvelope: "+ strRequestEnvelope(requestEnvelope));
            Logger.Debug("decodedResponse: "+ strResponseEnvelope(decodedResponse));

            return decodedResponse;
        }

        private static string strRequestEnvelope(RequestEnvelope input ){
            var str ="RequestId: "+ input.RequestId +" | statusCode: "+ input.StatusCode +" | Requests Type: ";
            foreach (var element in input.Requests) {
                str+=element.RequestType +", ";
            }
            str+=" | PlatformRequests Type: ";
            foreach (var element in input.PlatformRequests) {
                str+=element.Type + ", ";
            }
            return str;
        }

        private static string strResponseEnvelope(ResponseEnvelope input ){
            var str ="RequestId: "+ input.RequestId +" | statusCode: "+ input.StatusCode +"| PlatformReturns Type: "; ;
            foreach (var element in input.PlatformReturns) {
                str+=element.Type + ", ";
            }
            return str;
        }

        // RPC Calls need to be throttled 
        private static long lastRpc = 0;    // Starting at 0 to allow first RPC call to be done immediately
        private const int minDiff = 1000;   // Derived by trial-and-error. Up to 900 can cause server to complain.
        private static readonly ConcurrentQueue<RequestEnvelope> rpcQueue = new ConcurrentQueue<RequestEnvelope>();
        private static readonly ConcurrentDictionary<RequestEnvelope, ResponseEnvelope> responses = new ConcurrentDictionary<RequestEnvelope, ResponseEnvelope>();
        private static readonly Semaphore mutex = new Semaphore(1, 1);

        public static async Task<ResponseEnvelope> PerformThrottledRemoteProcedureCall<TRequest>(this System.Net.Http.HttpClient client, string url, RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
        {
            rpcQueue.Enqueue(requestEnvelope);
            var count = rpcQueue.Count;
            mutex.WaitOne();
            RequestEnvelope r;
            while (rpcQueue.TryDequeue(out r))
            {
                var diff = Math.Max(0, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - lastRpc);
                if (diff < minDiff)
                {
                    var delay = (int)((minDiff - diff) + (int)(new Random().NextDouble() * 0)); // Add some randomness
                    RandomHelper.RandomSleep(delay, delay + 100);
                }
                lastRpc = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                ResponseEnvelope response = await PerformRemoteProcedureCall<TRequest>(client, url, r).ConfigureAwait(false);
                responses.GetOrAdd(r, response);
            }
            ResponseEnvelope ret;
            responses.TryRemove(requestEnvelope, out ret);
            mutex.Release();
            return ret;
        }
    }
}