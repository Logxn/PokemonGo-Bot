using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Helpers
{
    public class RandomRequestID
    {
        public static ulong LastRequestID { get; private set; }

        // Thanks to Noctem
        // Lehmer random number generator - https://en.wikipedia.org/wiki/Lehmer_random_number_generator

        ulong MersenePrime = 0x7FFFFFFF;           // M = 2^31 -1 = 2,147,483,647 (Mersenne prime M31)
        ulong PrimeRoot = 0x41A7;                  // A = 16807 (a primitive root modulo M31)
        ulong Quotient = 0x1F31D;                  // Q = 127773 = M / A (to avoid overflow on A * seed)
        ulong Rest = 0xB14;                        // R = 2836 = M % A (to avoid overflow on A * seed)
        ulong Hi = 1;
        ulong Lo = 2;

        public RandomRequestID()
        {
            LastRequestID = (LastRequestID == 0) ? 1 : LastRequestID;
        }

        public ulong Last()
        {
            return LastRequestID;
        }

        // Old method to obtain the request ID
        public ulong NextLehmerRandom()
        {
            Hi = 0;
            Lo = 0;
            ulong NewRequestID;

            Hi = LastRequestID / Quotient;
            Lo = LastRequestID % Quotient;

            NewRequestID = PrimeRoot * Lo - Rest * Hi;
            if (NewRequestID <= 0)
                NewRequestID = NewRequestID + MersenePrime;

            //Logger.Debug($"{NewRequestID.ToString("X")} [{Hi.ToString("X")},{Lo.ToString("X")}]");

            NewRequestID = NewRequestID % 0x80000000;
            LastRequestID = NewRequestID;

            return NewRequestID;
        }

        // New method to obtain the request ID (extracted from pgoapi)
        // TODO: Check this with pgoapi. This has  not sense for me (Xelwon) .
        // https://github.com/pogodevorg/pgoapi/blob/develop/pgoapi/rpc_api.py
        // Line 82
        public ulong NextSinceAPI0691()
        {
            Hi = PrimeRoot * Hi % MersenePrime;
            var NewRequestID = Lo++ | (Hi << 32);
            LastRequestID = NewRequestID;
            return NewRequestID;
        }

        public ulong Next()
        {
             return NextLehmerRandom();
            // return NextSinceAPI0691();
        }

    }
}
