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

        public RandomRequestID()
        {
            LastRequestID = (LastRequestID == 0) ? 1 : LastRequestID;
        }

        public ulong Last()
        {
            return LastRequestID;
        }

        public ulong Next()
        {
            ulong Hi = 0;
            ulong Lo = 0;
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
    }
}
