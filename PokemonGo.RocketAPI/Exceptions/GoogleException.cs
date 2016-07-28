namespace PokemonGo.RocketAPI.Exceptions
{
    using System;

    public class GoogleException : Exception
    {
        public GoogleException(string message)
            : base(message)
        {
        }
    }
}