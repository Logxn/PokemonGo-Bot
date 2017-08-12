using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Globalization;

using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using PokeMaster.Logic.Functions;
using PokemonGo.RocketAPI;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using PokeMaster.Logic.Utils;
using PokemonGo.RocketAPI.Helpers;

using PokeMaster.Logic.Translation;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Data;

namespace PokeMaster.Logic.Utils
{
    public class TelegramUtil
    {

        private static TelegramUtil instance;

        public static TelegramUtil getInstance()
        {
            return instance;
        }

        public enum TelegramUtilInformationTopics
        {
            Pokestop,
            Catch,
            Evolve,
            Transfer,
            Levelup
        }
        private Dictionary<TelegramUtilInformationTopics, Boolean> _information = new Dictionary<TelegramUtilInformationTopics, Boolean>();
        private Dictionary<TelegramUtilInformationTopics, String> _informationDescription = new Dictionary<TelegramUtilInformationTopics, String>();

        private Dictionary<TelegramUtilInformationTopics, String> _informationDescriptionDefault = new Dictionary<TelegramUtilInformationTopics, String>() {
            { TelegramUtilInformationTopics.Pokestop, @"Notifies you when a pokestop was visited" },
            { TelegramUtilInformationTopics.Catch, @"Notifies you when a pokemon is caught" },
            { TelegramUtilInformationTopics.Evolve, @"Notifies you when a pokemon was evolved" },
            { TelegramUtilInformationTopics.Transfer, @"Notifies you when a pokemon is transfered" },
            { TelegramUtilInformationTopics.Levelup, @"Notifies you when you got a level up" },
        };
        private Dictionary<TelegramUtilInformationTopics, String> _informationDescriptionIDs = new Dictionary<TelegramUtilInformationTopics, String>() {
            { TelegramUtilInformationTopics.Pokestop, @"telegram_pokestop_description" },
            { TelegramUtilInformationTopics.Catch, @"telegram_catch_description" },
            { TelegramUtilInformationTopics.Evolve, @"telegram_evolve_description" },
            { TelegramUtilInformationTopics.Transfer, @"telegram_transfer_description" },
            { TelegramUtilInformationTopics.Levelup, @"telegram_levelup_description" },
        };


        public Dictionary<TelegramUtilInformationTopics, String> _informationTopicDefaultTexts = new Dictionary<TelegramUtilInformationTopics, String>() {
            { TelegramUtilInformationTopics.Pokestop, "Visited a pokestop {0}\nXP: {1}, Eggs: {2}, Gems:{3}, Items: {4}" },
            { TelegramUtilInformationTopics.Catch, "Caught {0} CP {1} IV {2}% using {3} got {4} XP." },
            { TelegramUtilInformationTopics.Evolve, "Evolved {0} CP {1} {2}%  to {3} CP: {4} for {5}xp" },
            { TelegramUtilInformationTopics.Transfer, "Transfer {0} CP {1} IV {2}% (Best: {3} CP)" },
            { TelegramUtilInformationTopics.Levelup, "You ({0}) got Level Up! Your new Level is now {1}!" },
        };

        public Dictionary<TelegramUtilInformationTopics, String> _informationTopicDefaultTextIDs = new Dictionary<TelegramUtilInformationTopics, String>() {
            { TelegramUtilInformationTopics.Pokestop, @"telegram_pokestop" },
            { TelegramUtilInformationTopics.Catch, @"telegram_catch" },
            { TelegramUtilInformationTopics.Evolve, @"telegram_evolve" },
            { TelegramUtilInformationTopics.Transfer, @"telegram_transfer" },
            { TelegramUtilInformationTopics.Levelup, @"telegram_levelup" },
        };

        public void sendInformationText(TelegramUtilInformationTopics topic, params object[] args)
        {
            if(_information.ContainsKey(topic) && _information[topic] == true && _informationTopicDefaultTexts.ContainsKey(topic) && _informationTopicDefaultTextIDs.ContainsKey(topic))
            {
                String unformatted = TranslationHandler.GetString(_informationTopicDefaultTextIDs[topic], _informationTopicDefaultTexts[topic]);
                String formatted = string.Format(unformatted, args);
                sendMessage(formatted);
            }
        }

        public void sendMessage(String msg)
        {
            try
            {
                if (_chatid != -1)
                {
                    _telegram.SendTextMessageAsync(_chatid, msg, replyMarkup: new ReplyKeyboardHide());
                }
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString());
            }
        }

        #region private properties
        private Client _client;
        private Inventory _inventory;

        private Telegram.Bot.TelegramBotClient _telegram;
        private readonly PokeMaster.Logic.Shared.ISettings _botSettings;

        private long _chatid = -1;
        private bool _livestats = false;
        private const bool _informations = false;

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
            RUN_FORCEEVOLVE,
            RUN_SNIPE,
            FORCE_STOP,
            TRANSFER_POKEMON,
            GET_ITEMSLIST,
            DROP_ITEM
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
            { @"/snipe", TelegramUtilTask.RUN_SNIPE },
            { @"/stopbot", TelegramUtilTask.FORCE_STOP },
            { @"/transfer", TelegramUtilTask.TRANSFER_POKEMON },
            { @"/items", TelegramUtilTask.GET_ITEMSLIST },
            { @"/drop", TelegramUtilTask.DROP_ITEM },
        };
        #endregion

        public TelegramUtil(Client client, Telegram.Bot.TelegramBotClient telegram, PokeMaster.Logic.Shared.ISettings settings, Inventory inv)
        {
            instance = this;
            _client = client;
            _telegram = telegram;
            _botSettings = settings;
            _inventory = inv;

            Array values = Enum.GetValues(typeof(TelegramUtilInformationTopics));
            foreach (TelegramUtilInformationTopics topic in values)
            {
                _informationDescription[topic] = TranslationHandler.GetString(_informationDescriptionIDs[topic], _informationDescriptionDefault[topic]);
                _information[topic] = false;
            }

            DoLiveStats(settings);
            DoInformation();
        }

        public Telegram.Bot.TelegramBotClient getClient()
        {
            return _telegram;
        }

        public async void DoLiveStats(PokeMaster.Logic.Shared.ISettings settings)
        {
            try
            {

                if (_chatid != -1 && _livestats)
                {
                    var usage = "";
                    var inventory =  _client.Inventory.GetInventory();
                    var profil =  _client.Player.GetPlayer();
                    var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PlayerStats).ToArray();
                    foreach (var c in stats)
                    {
                        if (c != null)
                        {
                            int l = c.Level;

                            var expneeded = ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level));
                            var curexp = ((c.Experience - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level));
                            var curexppercent = (Convert.ToDouble(curexp) / Convert.ToDouble(expneeded)) * 100;

                            usage += "\nNickname: " + profil.PlayerData.Username +
                                "\nLevel: " + c.Level
                                + "\nEXP Needed: " + ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level))
                                + $"\nCurrent EXP: {curexp} ({Math.Round(curexppercent)}%)"
                                + "\nEXP to Level up: " + ((c.NextLevelXp) - (c.Experience))
                                + "\nKM walked: " + c.KmWalked
                                + "\nPokeStops visited: " + c.PokeStopVisits
                                + "\nStardust: " + profil.PlayerData.Currencies.ToArray()[1].Amount;
                        }
                    }

                    await _telegram.SendTextMessageAsync(_chatid, usage, replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);
                }
                await System.Threading.Tasks.Task.Delay(settings.TelegramLiveStatsDelay).ConfigureAwait(false);
                DoLiveStats(settings);
            } catch (Exception ex1){
                Logger.ExceptionInfo(ex1.ToString());
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
                    var stats =  _client.Inventory.GetPlayerStats();

                    current = stats.First().Level;
                    
                    if (current != level)
                    {
                        level = current;
                        string nick = _client.Player.GetPlayer().PlayerData.Username;

                        sendInformationText(TelegramUtilInformationTopics.Levelup, nick, level);

                    }
                }
                await System.Threading.Tasks.Task.Delay(5000).ConfigureAwait(false);
                DoInformation();
            } catch (Exception ex1){
                Logger.ExceptionInfo(ex1.ToString());
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
                string username = _botSettings.TelegramName;
                string telegramAnswer = string.Empty;
                
                if (username != message.From.Username)
                {
                    using (System.IO.Stream stream = new System.IO.MemoryStream())
                    {
                        PokemonGo.RocketAPI.Logic.Properties.Resources.norights.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        stream.Position = 0;
                        await _telegram.SendPhotoAsync(_chatid, new FileToSend("norights.jpg", stream), replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);
                    }
                    return;
                }

                // [0]-Commando; [1+]-Argument
                var msgText = message.Text;
                string[] textCMD = msgText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                TelegramUtilTask cmd = getTask(textCMD[0]);
                switch (cmd)
                {
                    case TelegramUtilTask.UNKNOWN:
                        telegramAnswer = string.Format("Usage:\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}\r\n{8}\r\n{9}",
                            @"/stats - Get Current Stats",
                            @"/livestats - Enable/Disable Live Stats",
                            @"/information <topic> - Enable/Disable Information topics",
                            @"/top <HowMany?> - Outputs Top (?) Pokemons",
                            @"/transfer pokemon cp - transfer pokemons that matches with the given data",
                            @"/items  - Outputs Items List",
                            @"/drop item amount  - Throws the amount of items given",
                            @"/forceevolve - Forces Evolve",
                            @"/snipe pokemon latitude, longitude - snipe desired pokemon",
                            @"/stopbot - stop running bot");
                        break;
                    case TelegramUtilTask.GET_STATS:
                        var ps =  _client.Inventory.GetPlayerStats(); 

                        int l = ps.First().Level; 
                        long expneeded = ((ps.First().NextLevelXp - ps.First().PrevLevelXp) - StringUtils.getExpDiff(ps.First().Level));
                        long curexp = ((ps.First().Experience - ps.First().PrevLevelXp) - StringUtils.getExpDiff(ps.First().Level));
                        double curexppercent = (Convert.ToDouble(curexp) / Convert.ToDouble(expneeded)) * 100;
                        string curloc = _client.CurrentLatitude + "%20" + _client.CurrentLongitude;
                        curloc = curloc.Replace(",", ".");
                        string curlochtml = "https://www.google.com/maps/search/" + curloc + "/";
                        double shortenLng = Math.Round(_client.CurrentLongitude, 3);
                        double shortenLat = Math.Round(_client.CurrentLatitude, 3);
                        var player = _client.Player.GetPlayer();
                        telegramAnswer += "\nNickname: " + player.PlayerData.Username;
                        telegramAnswer += "\nLevel: " + ps.First().Level;
                        telegramAnswer += "\nEXP Needed: " + ((ps.First().NextLevelXp - ps.First().PrevLevelXp) - StringUtils.getExpDiff(ps.First().Level));
                        telegramAnswer += $"\nCurrent EXP: {curexp} ({Math.Round(curexppercent)}%)";
                        telegramAnswer += "\nEXP to Level up: " + ((ps.First().NextLevelXp) - (ps.First().Experience));
                        telegramAnswer += "\nKM walked: " + ps.First().KmWalked;
                        telegramAnswer += "\nPokeStops visited: " + ps.First().PokeStopVisits;
                        telegramAnswer += "\nStardust: " + player.PlayerData.Currencies.ToArray()[1].Amount;
                        telegramAnswer += "\nPokemons: " + ( _client.Inventory.GetPokemons()).Count() + "/" + player.PlayerData.MaxPokemonStorage;
                        telegramAnswer += "\nItems: " +  _client.Inventory.GetItems().Count() + " / " + player.PlayerData.MaxItemStorage;
                        telegramAnswer += "\nCurentLocation:\n" + curlochtml;
                        break;
                    case TelegramUtilTask.GET_TOPLIST:
                        int shows = 10;
                        if (textCMD.Length > 1 && !int.TryParse(textCMD[1], out shows))
                        {
                            telegramAnswer += $"Error! This is not a Number: {textCMD[1]}\nNevermind...\n";
                            shows = 10; //TryParse error will reset to 0
                        }
                        telegramAnswer += "Showing " + shows + " Pokemons...\nSorting...";
                        await _telegram.SendTextMessageAsync(_chatid, telegramAnswer, replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);

                        var myPokemons =  _client.Inventory.GetPokemons();
                        myPokemons = myPokemons.OrderByDescending(x => x.Cp);
                        var PogoUsername = _client.Player.GetPlayer().PlayerData.Username;
                        telegramAnswer = $"Top {shows} Pokemons of {PogoUsername}:";

                        IEnumerable<PokemonData> topPokemon = myPokemons.Take(shows);
                        foreach (PokemonData pokemon in topPokemon)
                        {
                            telegramAnswer += string.Format("\n{0} |  CP: {1} ({2}% perfect)",
                                pokemon.PokemonId,
                                pokemon.Cp,
                                Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon), 2));
                        }
                        break;
                    case TelegramUtilTask.SWITCH_LIVESTATS:
                        _livestats = SwitchAndGetAnswer(_livestats, out telegramAnswer, "Live Stats");
                        break;
                    case TelegramUtilTask.SWITCH_INFORMATION:
                        //_informations = SwitchAndGetAnswer(_informations, out telegramAnswer, "Information");
                        Array topics = Enum.GetValues(typeof(TelegramUtilInformationTopics));
                        if (textCMD.Length > 1)
                        {
                            if(textCMD[1] == "all-enable")
                            {
                                foreach (TelegramUtilInformationTopics topic in topics)
                                {
                                    String niceName = topic.ToString().Substring(0, 1).ToUpper() + topic.ToString().Substring(1).ToLower();
                                    telegramAnswer += "Enabled information topic " + niceName + "\n";
                                    _information[topic] = true;
                                }
                                break;
                            }
                            else if(textCMD[1] == "all-disable")
                            {
                                foreach (TelegramUtilInformationTopics topic in topics)
                                {
                                    String niceName = topic.ToString().Substring(0, 1).ToUpper() + topic.ToString().Substring(1).ToLower();
                                    telegramAnswer += "Disabled information topic " + niceName + "\n";
                                    _information[topic] = false;
                                }
                                break;
                            }
                            else { 
                                foreach (TelegramUtilInformationTopics topic in topics)
                                {
                                    if (textCMD[1].ToLower() == topic.ToString().ToLower()) {
                                        String niceName = topic.ToString().Substring(0, 1).ToUpper() + topic.ToString().Substring(1).ToLower();
                                        _information[topic] = !_information[topic];
                                        telegramAnswer = (_information[topic] ? "En" : "Dis") + "abled information topic " + niceName + "\n";
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (TelegramUtilInformationTopics topic in topics)
                            {
                                String niceName = topic.ToString().Substring(0, 1).ToUpper() + topic.ToString().Substring(1).ToLower();
                                telegramAnswer += " - " + niceName + "\n";
                                telegramAnswer += " -     " + _informationDescription[topic] + "\n";
                                telegramAnswer += " -     Currently " + (_information[topic] ? "enabled" : "disabled") + "\n";
                                telegramAnswer += "\n";
                            }

                            telegramAnswer += " - all-disable\n";
                            telegramAnswer += " -     " + TranslationHandler.GetString("telegram-disable-all", "Disable all topics") + "\n";
                            telegramAnswer += "\n";

                            telegramAnswer += " - all-enable\n";
                            telegramAnswer += " -     " + TranslationHandler.GetString("telegram-enable-all", "Enable all topics") + "\n";
                            telegramAnswer += "\n";
                            break;
                        }

                        break;
                    case TelegramUtilTask.RUN_FORCEEVOLVE:
                        IEnumerable<PokemonData> pokemonToEvolve =  Setout.GetPokemonToEvolve(true);
                        if (pokemonToEvolve.Count() > 3)
                        {
                             Setout.UseLuckyEgg(_client);
                        }
                        foreach (PokemonData pokemon in pokemonToEvolve)
                        {
                            if (_botSettings.pokemonsToEvolve.Contains(pokemon.PokemonId))
                            {
                                var evolvePokemonOutProto =  _client.Inventory.EvolvePokemon((ulong)pokemon.Id);
                                if (evolvePokemonOutProto.Result == POGOProtos.Networking.Responses.EvolvePokemonResponse.Types.Result.Success)
                                {
                                    await _telegram.SendTextMessageAsync(_chatid, $"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExperienceAwarded}xp", replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);
                                }
                                else
                                {
                                    await _telegram.SendTextMessageAsync(_chatid, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);
                                }
                                RandomHelper.RandomSleep(2000);
                            }
                        }
                        telegramAnswer = "Done.";
                        break;
                    case TelegramUtilTask.RUN_SNIPE:
                        telegramAnswer = ProcessSnipeCommand(msgText.Replace(textCMD[0],"").Trim());
                        break;
                    case TelegramUtilTask.FORCE_STOP:
                        var secs = 1;
                        if (textCMD.Length > 1)
                            int.TryParse(textCMD[1].Trim(),out secs);
                        telegramAnswer = $"Stopping bot in {secs} seconds";
                        await _telegram.SendTextMessageAsync(_chatid, telegramAnswer, replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);
                        _telegram.StopReceiving();
                        RandomHelper.RandomSleep(secs*1000);
                        Environment.Exit(0);
                        break;
                    case TelegramUtilTask.TRANSFER_POKEMON:
                        telegramAnswer = ProcessTransferPokemon(msgText.Replace(textCMD[0],"").Trim());
                        break;
                    case TelegramUtilTask.GET_ITEMSLIST:
                        telegramAnswer = ProcessGetItemList();
                        break;
                    case TelegramUtilTask.DROP_ITEM:
                        telegramAnswer = ProcessDropItem(msgText.Replace(textCMD[0],"").Trim());
                        break;
                }

                await _telegram.SendTextMessageAsync(_chatid, telegramAnswer, replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString());
                var apiRequestException = ex as ApiRequestException;
                if (apiRequestException != null)
                    await _telegram.SendTextMessageAsync(_chatid, apiRequestException.Message, replyMarkup: new ReplyKeyboardHide()).ConfigureAwait(false);
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
            await _telegram.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Received {callbackQueryEventArgs.CallbackQuery.Data}").ConfigureAwait(false);
        }

        private string ProcessSnipeCommand(string txt)
        {
            var splt1 = txt.Split(' ');
            if (splt1.Length < 2)
                return "Pokemon or location cannot be read";
            var splt2= splt1[1].Split(',');
            if (splt2.Length < 2)
                return "Location cannot be read";
            var pokemon = getPokemonID(splt1[0]);
            if (pokemon == PokemonId.Missingno)
                return "Pokemon cannot be read";
            var location = getLatLng(splt2[0],splt2[1]);
            Sniper.SendToSnipe(pokemon,location);
            return "Command sent succefully to the bot.";
        }
        
        private GeoCoordinate getLatLng(string strlat,string strlng)
        {
            var lat = double.Parse(strlat.Trim(), CultureInfo.InvariantCulture);
            var lng = double.Parse(strlng.Trim(), CultureInfo.InvariantCulture);
            return new GeoCoordinate(lat, lng);
        }

        private PokemonId getPokemonID (string text){
            var pokemons = Enum.GetNames(typeof(PokemonId));
            var lowerText = text.ToLower();
            var pokemonName = pokemons.LastOrDefault(p => lowerText.Contains(p.ToLower())) ?? PokemonId.Missingno.ToString();
            if (pokemonName == PokemonId.Pidgeot.ToString()){
                if (lowerText.Contains(PokemonId.Pidgeotto.ToString().ToLower()))
                    pokemonName = PokemonId.Pidgeotto.ToString();
            }
            return (PokemonId)Enum.Parse(typeof(PokemonId), pokemonName);
        }

        string ProcessTransferPokemon(string txt)
        {
            var splt1 = txt.Split(' ');
            if (splt1.Length < 2)
                return "Pokemon or cp cannot be read";
            var pokemon = getPokemonID(splt1[0]);
            if (pokemon == PokemonId.Missingno)
                return "Pokemon cannot be read";
            var cp = 0;
            int.TryParse(splt1[1].Trim(),out cp);
            if (cp == 0)
                return "CP cannot be read";
            var pokemons = _client.Inventory.GetPokemons().Where(x=> x.PokemonId == pokemon && x.Cp ==cp);
            var resp = "";
            if (!pokemons.Any()){
                resp = $"{pokemon} with cp={cp} not found";
                return resp;
            }
            _client.Inventory.TransferPokemons(pokemons.Select(x => x.Id).ToList());
            return "Command sent succefully to the bot.";
        }

        string ProcessGetItemList()
        {
            var result = "ITEMS:\n";
            var items = _client.Inventory.GetItems();
            foreach (var element in items) {
                result += element.ItemId.ToString().Replace("Item","") + " : " + element.Count +"\n";
            }
            return result;
        }

        string ProcessDropItem(string txt)
        {
            var splt1 = txt.Split(' ');
            if (splt1.Length < 2)
                return "item or amount cannot be read";
            var item = getItemID(splt1[0]);
            if (item == ItemId.ItemUnknown)
                return "item cannot be read";
            var amount = 0;
            int.TryParse(splt1[1].Trim(),out amount);
            if (amount == 0)
                return "amount cannot be read";
            _client.Inventory.RecycleItem(item,amount).Wait();
            return "Command sent succefully to the bot.";
        }

        private ItemId getItemID (string text){
            var items = Enum.GetNames(typeof(ItemId));
            var lowerText = text.ToLower();
            var itemName = items.LastOrDefault(p => lowerText.Contains(p.ToLower())) ?? ItemId.ItemUnknown.ToString();
            return (ItemId)Enum.Parse(typeof(ItemId), itemName);
        }

    }
}
