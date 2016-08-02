﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Extensions
{
    public static class HttpClientExtensions
    {
        static int err = 0;

        public static async Task<TResponsePayload> PostProtoPayload<TRequest, TResponsePayload>(this HttpClient client, string url, TRequest request) where TRequest : IMessage<TRequest> where TResponsePayload : IMessage<TResponsePayload>, new()
        {
            Logger.Write($"Requesting {typeof(TResponsePayload).Name}", LogLevel.Debug);
            await RandomHelper.RandomDelay(150, 300);
            var response = await PostProto<TRequest>(client, url, request);

            while (response.Payload.Count == 0) // WE WANT A FUCKING ANWSER POKEMON
            {
                if (err >= 3)
                {
                    err = 0;
                    throw new InvalidResponseException();
                }
                await RandomHelper.RandomDelay(150, 300);
                response = await PostProto<TRequest>(client, url, request);
                if (response.Payload.Count == 0)
                {
                    err++;
                    Logger.Error($"Error at Request PostProtoPayload {typeof(TResponsePayload).Name} retrying {err}/3");
                } else
                {
                    err = 0;
                    break;
                }
            }

            //Decode payload
            //todo: multi-payload support
            var payload = response.Payload[0];
            var parsedPayload = new TResponsePayload();
            parsedPayload.MergeFrom(payload);
            
            return parsedPayload;
        }

        public static async Task<Response> PostProto<TRequest>(this HttpClient client, string url, TRequest request) where TRequest : IMessage<TRequest>
        {
            //Encode payload and put in envelop, then send
            var data = request.ToByteString();
            var result  = await client.PostAsync(url, new ByteArrayContent(data.ToByteArray())); 
            //Decode message
            var responseData = await result.Content.ReadAsByteArrayAsync();
            var codedStream = new CodedInputStream(responseData);
            var decodedResponse = new Response();
            decodedResponse.MergeFrom(codedStream);
            
            return decodedResponse;
        }
    }
}
