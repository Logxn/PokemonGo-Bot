namespace PokemonGo.RocketAPI.Helpers
{
    using System;

    public class Utils
    {
        public static ulong FloatAsUlong(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}