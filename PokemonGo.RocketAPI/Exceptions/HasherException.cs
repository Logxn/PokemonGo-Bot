﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Exceptions
{
    public class HasherException :Exception
    {
        public HasherException(string message): base(message)
        {

        }
    }
}
