using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    public static class StringUtils
    {
        private static Dictionary<PokemonId,string> _pokemonNameLookupTable;
        private static readonly int[] expDiffList;
        static StringUtils()
        {
            generatePokemonNameLookupTable();
            expDiffList = [0,1000,2000,3000,4000,
                            5000,6000,7000,8000,9000,
                            10000,10000,10000,10000,15000,
                            20000,20000,20000,25000,25000,
                            50000,75000,100000,125000,150000,
                            190000,200000,250000,300000,350000,
                            500000,500000,750000,1000000,1250000,
                            1500000,2000000,2500000,1000000,1000000];
        }
        private static generatePokemonNameLookupTable()
        {
            _pokemonNameLookupTable = new Dictionary<PokemonId,string>();
            foreach (PokemonId pIdIter in Enum.GetValues(typeof(PokemonId))){
                 _pokemonNameLookupTable.Add(pIdIter,"Pokemon.{}".Format(Enum.getName(typeof(PokemonId), pIdIter)));
            }

        }

        public static string getPokemonNameByLanguage(ISettings clientSettings, PokemonId pId)
        {
            if (clientSettings.Language)
            {
                try{
                    return TranslationHandler.getString(_pokemonNameLookupTable[pId]);
                }
                catch(KeyNotFoundException e)
                {
                    return TranslationHandler.getString(_pokemonNameLookupTable[PokemonId.Missingno]);
                }
                        
            }
            else
            {
                return pId.ToString();
            }
        }
        public static int getExpDiff(int level)
        {
            if (level > 0 || level < 41)
            {
                return expDiffList[level];
            }
            return 0;
        }

        public static string GetSummedFriendlyNameOfItemAwardList(IEnumerable<FortSearchResponse.Types.ItemAward> items)
        {
            var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

            if (!enumerable.Any())
                return string.Empty;

            return
                enumerable.GroupBy(i => i.ItemId)
                          .Select(kvp => new { ItemName = kvp.Key.ToString(), Amount = kvp.Sum(x => x.ItemCount) })
                          .Select(y => $"{y.Amount} x {y.ItemName}")
                          .Aggregate((a, b) => $"{a}, {b}");
        }
    }
}
