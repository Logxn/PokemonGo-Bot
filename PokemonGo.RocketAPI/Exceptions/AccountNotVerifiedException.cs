namespace PokemonGo.RocketAPI.Exceptions
{
    using System;

    public class AccountNotVerifiedException : Exception
    {
        public AccountNotVerifiedException(string message)
            : base(message)
        {
        }
    }
}