using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    public class BotStats
    {
        private int _totalExperience;
        private int _totalPokemons;
        private DateTime _initialSessionDateTime = DateTime.Now;

        private double _getBottingSessionTime()
        {
            return ((DateTime.Now - _initialSessionDateTime).TotalSeconds) / 3600;
        }

        public void addExperience(int exp)
        {
            _totalExperience += exp;
        }

        public void addPokemon(int count)
        {
            _totalPokemons += count;
        }

        public override string ToString()
        {
            return "xp/h: " + Math.Round((_totalExperience / _getBottingSessionTime())) + "| pokemon/h: " + Math.Round((_totalPokemons / _getBottingSessionTime())) + "";
        }
    }
}