using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Encryption
{
    public interface ICrypt
    {
        byte[] Encrypt(byte[] input, uint ms);
    }
}
