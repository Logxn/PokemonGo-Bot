using System;

namespace PokeMaster.Logic.Utils
{
    public class BotStats
    {
        private int _totalExperience;
        private int _totalPokemons;
        private int _totalStardust;
        private readonly DateTime _initialSessionDateTime = DateTime.Now;

        private double _getBottingSessionTime()
        {
            return (DateTime.Now - _initialSessionDateTime).TotalSeconds / 3600;
        }

        public void AddExperience(int exp)
        {
            _totalExperience += exp;
        }

        public void AddPokemon(int count)
        {
            _totalPokemons += count;
        }

        public void AddStardust(int stardust)
        {
            _totalStardust += stardust;
        }

        public override string ToString()
        {
            return "xp/h: " + GetXP_H().ToString("N0") 
                + " pokemon/h: " + Math.Round(_totalPokemons / _getBottingSessionTime()).ToString("N0") 
                + " stardust/h: " + Math.Round(_totalStardust / _getBottingSessionTime()).ToString("N0");
        }

        public string ToString(double expleft)
        {
            return "xp/h: " + GetXP_H().ToString("N0") +" (TL:"+ GettimeLeft(expleft).ToString(@"dd\.hh\:mm")+")"
                + " pokemon/h: " + Math.Round(_totalPokemons / _getBottingSessionTime()).ToString("N0") 
                + " stardust/h: " + Math.Round(_totalStardust / _getBottingSessionTime()).ToString("N0");
        }

        private double GetXP_H(){
            return Math.Round(_totalExperience / _getBottingSessionTime());
        }

        public TimeSpan GettimeLeft(double expleft)
        {
            var xph = GetXP_H();
            return xph > 0 ? TimeSpan.FromMinutes(expleft / xph*60) : TimeSpan.FromMinutes( 0);
        }
    }
}
