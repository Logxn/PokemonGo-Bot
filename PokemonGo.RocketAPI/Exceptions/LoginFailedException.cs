﻿using System;

namespace PokemonGo.RocketAPI.Exceptions
{
    public class LoginFailedException : Exception
    {

        public LoginFailedException(string message) : base(message)
        {

        }
        public LoginFailedException()
        {

        }
    }
}