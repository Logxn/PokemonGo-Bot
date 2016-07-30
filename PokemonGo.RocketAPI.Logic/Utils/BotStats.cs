namespace PokemonGo.RocketAPI.Logic.Utils
{
    using System;

    public class BotStats
    {
        private readonly DateTime _initialSessionDateTime = DateTime.Now;
        private int _totalExperience;
        private int _totalPokemons;

        public void addExperience(int exp)
        {
            this._totalExperience += exp;
        }

        public void addPokemon(int count)
        {
            this._totalPokemons += count;
        }

        public override string ToString()
        {
            return "EXP/Hour: " + Math.Round(this._totalExperience / this._getBottingSessionTime()) + " EXP | Pokemon/Hour: " + Math.Round(this._totalPokemons / this._getBottingSessionTime()) + " Pokemon(s)";
        }

        private double _getBottingSessionTime()
        {
            return (DateTime.Now - this._initialSessionDateTime).TotalSeconds / 3600;
        }
    }
}