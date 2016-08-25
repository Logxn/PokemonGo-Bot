using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Logic.Tasks
{
    class TransferDuplicatePokemon
    {
        public static async Task startTransferingDuplicatePokemon(ISettings _clientSettings, Client _client, bool keepPokemonsThatCanEvolve = false, bool TransferFirstLowIV = false)
        {
            if (_clientSettings.TransferDoublePokemons)
            {
                var duplicatePokemons = await _client.Inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve, TransferFirstLowIV);
          
                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    if (!_clientSettings.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {
                        if (duplicatePokemon.Cp >= _clientSettings.DontTransferWithCPOver || PokemonInfo.CalculatePokemonPerfection(duplicatePokemon) >= _client.Settings.ivmaxpercent)
                        {
                            continue;
                        }

                        var bestPokemonOfType = await _client.Inventory.GetHighestCPofType(duplicatePokemon);
                        var bestPokemonsCPOfType = await _client.Inventory.GetHighestCPofType2(duplicatePokemon);
                        var bestPokemonsIVOfType = await _client.Inventory.GetHighestIVofType(duplicatePokemon);

                        var transfer = await _client.Inventory.TransferPokemon(duplicatePokemon.Id);
                        if (TransferFirstLowIV)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best IV: {PokemonInfo.CalculatePokemonPerfection(bestPokemonsIVOfType.First()).ToString("0.00")} %)", LogLevel.Info);
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best: {bestPokemonsCPOfType.First().Cp} CP)", LogLevel.Info);
                        }

                        TelegramUtil.getInstance().sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Transfer,
                            StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId), duplicatePokemon.Cp,
                            PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00"), bestPokemonOfType);

                        await RandomHelper.RandomDelay(500, 700);
                    }
                }
            }
        }
    }
}
