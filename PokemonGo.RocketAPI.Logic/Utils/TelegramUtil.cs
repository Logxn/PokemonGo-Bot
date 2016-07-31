using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Logic;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    public class TelegramUtil
    {
        #region private properties
        private Client _client;
        private Inventory _inventory;

        private Telegram.Bot.TelegramBotClient _telegram;
        private readonly ISettings _clientSettings;

        private long _chatid = -1;
        private bool _livestats = false;
        private bool _informations = false;

        /// <summary>
        /// defines what to do
        /// </summary>
        private enum TelegramUtilTask
        {
            UNKNOWN,
            /// <summary>
            /// Outputs Current Stats
            /// </summary>
            GET_STATS,
            /// <summary>
            /// Outputs Top (?) Pokemons
            /// </summary>
            GET_TOPLIST,
            /// <summary>
            /// Enable/Disable Live Stats
            /// </summary>
            SWITCH_LIVESTATS,
            /// <summary>
            /// Enable/Disable Informations
            /// </summary>
            SWITCH_INFORMATION,
            /// <summary>
            /// Forces Evolve
            /// </summary>
            RUN_FORCEEVOLVE
        }

        /// <summary>
        /// defined telegram commandos
        /// </summary>
        private Dictionary<string, TelegramUtilTask> _telegramCommandos = new Dictionary<string, TelegramUtilTask>()
        {
            { @"/stats", TelegramUtilTask.GET_STATS },
            { @"/top", TelegramUtilTask.GET_TOPLIST },
            { @"/livestats", TelegramUtilTask.SWITCH_LIVESTATS },
            { @"/information", TelegramUtilTask.SWITCH_INFORMATION },
            { @"/forceevolve", TelegramUtilTask.RUN_FORCEEVOLVE },
        };
        #endregion

        public TelegramUtil(Client client, Telegram.Bot.TelegramBotClient telegram, ISettings settings, Inventory inv)
        {
            _client = client;
            _telegram = telegram;
            _clientSettings = settings;
            _inventory = inv;
            DoLiveStats(settings);
            DoInformation();
        }

        public Telegram.Bot.TelegramBotClient getClient()
        {
            return _telegram;
        }

        public async void DoLiveStats(ISettings settings)
        {
            try
            {

                if (_chatid != -1 && _livestats)
                {
                    var usage = "";
                    var inventory = await _client.GetInventory();
                    var profil = await _client.GetProfile();
                    var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PlayerStats).ToArray();
                    foreach (var c in stats)
                    {
                        if (c != null)
                        {
                            int l = c.Level;

                            var expneeded = ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level));
                            var curexp = ((c.Experience - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level));
                            var curexppercent = (Convert.ToDouble(curexp) / Convert.ToDouble(expneeded)) * 100;

                            usage += "\nNickname: " + profil.Profile.Username +
                                "\nLevel: " + c.Level
                                + "\nEXP Needed: " + ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level))
                                + $"\nCurrent EXP: {curexp} ({Math.Round(curexppercent)}%)"
                                + "\nEXP to Level up: " + ((c.NextLevelXp) - (c.Experience))
                                + "\nKM walked: " + c.KmWalked
                                + "\nPokeStops visited: " + c.PokeStopVisits
                                + "\nStardust: " + profil.Profile.Currency.ToArray()[1].Amount;
                        }
                    }

                    await _telegram.SendTextMessageAsync(_chatid, usage, replyMarkup: new ReplyKeyboardHide());
                }
                await System.Threading.Tasks.Task.Delay(settings.TelegramLiveStatsDelay);
                DoLiveStats(settings);
            } catch (Exception)
            {

            }
        }

         int level;
        
        public async void DoInformation()
        {
            try
            {
                if (_chatid != -1 && _informations)
                {
                    int current = 0;
                    var usage = "";
                    var inventory = await _client.GetInventory();
                    var profil = await _client.GetProfile();
                    IEnumerable<PlayerStats> stats = inventory.InventoryDelta.InventoryItems
                                                                                .Select(i => i.InventoryItemData.PlayerStats)
                                                                                .Where(e => e != null);
                    foreach (PlayerStats s in stats)
                    {
                        if (s != null)
                        {
                            current = s.Level;
                        }
                    }

                    if (current != level)
                    {
                        level = current;
                        string nick = await _client.getNickname();
                        usage = $"You ({nick}) got Level Up! Your new Level is now {level}!";
                        await _telegram.SendTextMessageAsync(_chatid, usage, replyMarkup: new ReplyKeyboardHide());
                    }
                }
                await System.Threading.Tasks.Task.Delay(5000);
                DoInformation();
            } catch (Exception)
            {

            }
        }


        public async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            Message message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.TextMessage)
                return;
            _chatid = message.Chat.Id;
            try
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "[TelegramAPI] Got Request from " + message.From.Username + " | " + message.Text);
                string username = _clientSettings.TelegramName;
                string telegramAnswer = string.Empty;
                
                if (username != message.From.Username)
                {
                    using (System.IO.Stream stream = new System.IO.MemoryStream())
                    {
                        Properties.Resources.norights.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        stream.Position = 0;
                        await _telegram.SendPhotoAsync(_chatid, new FileToSend("norights.jpg", stream), replyMarkup: new ReplyKeyboardHide());
                    }
                    return;
                }

                // [0]-Commando; [1+]-Argument
                string[] textCMD = message.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                TelegramUtilTask cmd = getTask(textCMD[0]);
                switch (cmd)
                {
                    case TelegramUtilTask.UNKNOWN:
                        telegramAnswer = string.Format("Usage:\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}",
                            @"/stats - Get Current Stats",
                            @"/livestats - Enable/Disable Live Stats",
                            @"/information - Enable/Disable Informations",
                            @"/top <HowMany?> - Outputs Top (?) Pokemons",
                            @"/forceevolve - Forces Evolve");
                        break;
                    case TelegramUtilTask.GET_STATS:
                        var inventory = await _client.GetInventory();
                        var profil = await _client.GetProfile();
                        IEnumerable<PlayerStats> stats = inventory.InventoryDelta.InventoryItems
                                                                                        .Select(i => i.InventoryItemData.PlayerStats)
                                                                                        .Where(i => i != null);
                        foreach (PlayerStats ps in stats)
                        {
                            int l = ps.Level;

                            long expneeded = ((ps.NextLevelXp - ps.PrevLevelXp) - StringUtils.getExpDiff(ps.Level));
                            long curexp = ((ps.Experience - ps.PrevLevelXp) - StringUtils.getExpDiff(ps.Level));
                            double curexppercent = (Convert.ToDouble(curexp) / Convert.ToDouble(expneeded)) * 100;
                            string curloc = _client.CurrentLat + "%20" + _client.CurrentLng;
                            curloc = curloc.Replace(",", ".");
                            string curlochtml = "https://www.google.de/maps/search/" + curloc + "/";
                            double shortenLng = Math.Round(_client.CurrentLng, 3);
                            double shortenLat = Math.Round(_client.CurrentLat, 3);
                            string pokemap = shortenLat + ";" + shortenLng;
                            pokemap = pokemap.Replace(",", ".").Replace(";", ",");
                            string pokevishtml = "https://skiplagged.com/pokemon/#" + pokemap +",14";
                            telegramAnswer +=
                                "\nNickname: " + profil.Profile.Username 
                                + "\nLevel: " + ps.Level
                                + "\nEXP Needed: " + ((ps.NextLevelXp - ps.PrevLevelXp) - StringUtils.getExpDiff(ps.Level))
                                + $"\nCurrent EXP: {curexp} ({Math.Round(curexppercent)}%)"
                                + "\nEXP to Level up: " + ((ps.NextLevelXp) - (ps.Experience))
                                + "\nKM walked: " + ps.KmWalked
                                + "\nPokeStops visited: " + ps.PokeStopVisits
                                + "\nStardust: " + profil.Profile.Currency.ToArray()[1].Amount
                                + "\nPokemons: " + await _inventory.getPokemonCount() + "/" + profil.Profile.PokeStorage
                                + "\nItems: " + await _inventory.getInventoryCount() + " / " + profil.Profile.ItemStorage
                                + "\nCurentLocation:\n" + curlochtml
                                + "\nPokevision:\n" + pokevishtml;
                        }
                        break;
                    case TelegramUtilTask.GET_TOPLIST:
                        int shows = 10;
                        if (textCMD.Length > 1 && !int.TryParse(textCMD[1], out shows))
                        {
                            telegramAnswer += $"Error! This is not a Number: {textCMD[1]}\nNevermind...\n";
                            shows = 10; //TryParse error will reset to 0
                        }
                        telegramAnswer += "Showing " + shows + " Pokemons...\nSorting...";
                        await _telegram.SendTextMessageAsync(_chatid, telegramAnswer, replyMarkup: new ReplyKeyboardHide());

                        var myPokemons = await _inventory.GetPokemons();
                        myPokemons = myPokemons.OrderByDescending(x => x.Cp);
                        var profile = await _client.GetProfile();
                        telegramAnswer = $"Top {shows} Pokemons of {profile.Profile.Username}:";

                        IEnumerable<PokemonData> topPokemon = myPokemons.Take(shows);
                        foreach (PokemonData pokemon in topPokemon)
                        {
                            telegramAnswer += string.Format("\n{0} ({1})  |  CP: {2} ({3}% perfect)",
                                pokemon.PokemonId,
                                StringUtils.getPokemonNameGer(pokemon.PokemonId),
                                pokemon.Cp,
                                PokemonInfo.CalculatePokemonPerfection(pokemon));
                        }
                        break;
                    case TelegramUtilTask.SWITCH_LIVESTATS:
                        _livestats = SwitchAndGetAnswer(_livestats, out telegramAnswer, "Live Stats");
                        break;
                    case TelegramUtilTask.SWITCH_INFORMATION:
                        _informations = SwitchAndGetAnswer(_informations, out telegramAnswer, "Information");
                        break;
                    case TelegramUtilTask.RUN_FORCEEVOLVE:
                        IEnumerable<PokemonData> pokemonToEvolve = await _inventory.GetPokemonToEvolve(null);
                        if (pokemonToEvolve.Count() > 3)
                        {
                            await _inventory.UseLuckyEgg(_client);
                        }
                        foreach (PokemonData pokemon in pokemonToEvolve)
                        {
                            if (_clientSettings.pokemonsToEvolve.Contains(pokemon.PokemonId))
                            {
                                var evolvePokemonOutProto = await _client.EvolvePokemon((ulong)pokemon.Id);
                                if (evolvePokemonOutProto.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
                                {
                                    await _telegram.SendTextMessageAsync(_chatid, $"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp", replyMarkup: new ReplyKeyboardHide());
                                }
                                else
                                {
                                    await _telegram.SendTextMessageAsync(_chatid, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", replyMarkup: new ReplyKeyboardHide());
                                }
                                await RandomHelper.RandomDelay(1000, 2000);
                            }
                        }
                        telegramAnswer = "Done.";
                        break;
                }

                await _telegram.SendTextMessageAsync(_chatid, telegramAnswer, replyMarkup: new ReplyKeyboardHide());
            }
            catch (Exception ex)
            {
                if (ex is ApiRequestException)
                    await _telegram.SendTextMessageAsync(_chatid, (ex as ApiRequestException).Message, replyMarkup: new ReplyKeyboardHide());
            }
        }

        private bool SwitchAndGetAnswer(bool oldSwitchState, out string answerText, string text)
        {
            answerText = string.Format("{0} {1}.", oldSwitchState ? "Disabled" : "Enabled", text);
            return !oldSwitchState;
        }

        private TelegramUtilTask getTask(string cmdString)
        {
            if(_telegramCommandos.ContainsKey(cmdString))
            {
                TelegramUtilTask task;
                if (_telegramCommandos.TryGetValue(cmdString, out task))
                    return task;
            }
            return TelegramUtilTask.UNKNOWN;
        }

        public async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await _telegram.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }
    }
}
