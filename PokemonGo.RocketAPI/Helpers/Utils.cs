
using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace PokemonGo.RocketAPI.Helpers
{
    public class Utils
    {
        public static ulong FloatAsUlong(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static long GetTime(bool ms = false)
        {
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);

            if (ms)
                return (long)Math.Round(timeSpan.TotalMilliseconds);
            return (long)Math.Round(timeSpan.TotalSeconds);
        }

        public static class HtmlRemoval
        {
            /// <summary>
            /// Remove HTML from string with Regex.
            /// </summary>
            public static string StripTagsRegex(string source)
            {
                return Regex.Replace(source, "<.*?>", string.Empty);
            }

            /// <summary>
            /// Compiled regular expression for performance.
            /// </summary>
            static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

            /// <summary>
            /// Remove HTML from string with compiled Regex.
            /// </summary>
            public static string StripTagsRegexCompiled(string source)
            {
                return _htmlRegex.Replace(source, string.Empty);
            }

            /// <summary>
            /// Remove HTML tags from string using char array.
            /// </summary>
            public static string StripTagsCharArray(string source)
            {
                char[] array = new char[source.Length];
                int arrayIndex = 0;
                bool inside = false;

                for (int i = 0; i < source.Length; i++)
                {
                    char let = source[i];
                    if (let == '<')
                    {
                        inside = true;
                        continue;
                    }
                    if (let == '>')
                    {
                        inside = false;
                        continue;
                    }
                    if (!inside)
                    {
                        array[arrayIndex] = let;
                        arrayIndex++;
                    }
                }
                return new string(array, 0, arrayIndex);
            }
        }

        public static uint GenLocation1(byte[] authTicket, double lat, double lng, double alt)
        {
            byte[] locationBytes = BitConverter.GetBytes(lat).Reverse()
                 .Concat(BitConverter.GetBytes(lng).Reverse())
                 .Concat(BitConverter.GetBytes(alt).Reverse()).ToArray();

            return HashBuilder.Hash32Salt(locationBytes, HashBuilder.Hash32(authTicket));
        }

        public static uint GenLocation2(double lat, double lng, double alt)
        {
            byte[] locationBytes = BitConverter.GetBytes(lat).Reverse()
                 .Concat(BitConverter.GetBytes(lng).Reverse())
                 .Concat(BitConverter.GetBytes(alt).Reverse()).ToArray();
            return HashBuilder.Hash32(locationBytes);
        }

        public static ulong GenRequestHash(byte[] authTicket, byte[] hashR)
        {
            ulong seed = HashBuilder.Hash64(authTicket);
            return HashBuilder.Hash64Salt64(hashR, seed);
        }

    }
}