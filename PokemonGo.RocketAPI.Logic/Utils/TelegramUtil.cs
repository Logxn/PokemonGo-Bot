namespace PokemonGo.RocketAPI.Logic.Utils
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using PokemonGo.RocketAPI.GeneratedCode;
    using PokemonGo.RocketAPI.Helpers;

    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class TelegramUtil
    {
        private readonly ISettings _clientSettings;

        private readonly Client _client;
        private readonly Inventory _inventory;

        private readonly TelegramBotClient _telegram;

        private long chatid = -1;

        private bool informations;

        private int level;
        private bool livestats;

        public TelegramUtil(Client client, TelegramBotClient telegram, ISettings settings, Inventory inv)
        {
            this._client = client;
            this._telegram = telegram;
            this._clientSettings = settings;
            this._inventory = inv;
            this.DoLiveStats(settings);
            this.DoInformation();
        }

        public async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await this._telegram.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }

        public async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                var message = messageEventArgs.Message;

                if (message == null || message.Type != MessageType.TextMessage)
                    return;

                Logger.ColoredConsoleWrite(ConsoleColor.Red, "[TelegramAPI]Got Request from " + messageEventArgs.Message.From.Username + " | " + message.Text);
                var usernames = this._clientSettings.TelegramName.Split(';');

                if (!usernames.Contains(messageEventArgs.Message.From.Username))
                {
                    var usage = "I dont hear at you!";
                    await this._telegram.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
                    return;
                }

                if (message.Text.StartsWith("/stats"))
                {
                    // send inline keyboard
                    var usage = string.Empty;
                    var inventory = await this._client.GetInventory();
                    var profil = await this._client.GetProfile();
                    var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PlayerStats).ToArray();
                    foreach (var c in stats)
                    {
                        if (c != null)
                        {
                            var l = c.Level;

                            var expneeded = c.NextLevelXp - c.PrevLevelXp - StringUtils.getExpDiff(c.Level);
                            var curexp = c.Experience - c.PrevLevelXp - StringUtils.getExpDiff(c.Level);
                            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;
                            var curloc = this._client.CurrentLat + "%20" + this._client.CurrentLng;
                            curloc = curloc.Replace(",", ".");
                            var curlochtml = "https://www.google.de/maps/search/" + curloc + "/";
                            var pokevis = this._client.CurrentLat + ";" + this._client.CurrentLng;
                            pokevis = pokevis.Replace(",", ".").Replace(";", ",");
                            var pokevishtml = "https://pokevision.com/#/@" + pokevis;

                            usage += "\nNickname: " + profil.Profile.Username + "\nLevel: " + c.Level + "\nEXP Needed: " + (c.NextLevelXp - c.PrevLevelXp - StringUtils.getExpDiff(c.Level)) + $"\nCurrent EXP: {curexp} ({Math.Round(curexppercent)}%)" + "\nEXP to Level up: " + (c.NextLevelXp - c.Experience) + "\nKM walked: " + c.KmWalked + "\nPokeStops visited: " + c.PokeStopVisits + "\nStardust: " + profil.Profile.Currency.ToArray()[1].Amount + "\nCurentLocation: " + curlochtml + "\nPokevision: " + pokevishtml;
                        }
                    }

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
                }
                else if (message.Text.StartsWith("/livestats"))
                {
                    // send custom keyboard
                    var usage = string.Empty;
                    if (this.livestats)
                    {
                        usage = "Disabled Live Stats.";
                        this.livestats = false;
                        this.chatid = -1;
                    }
                    else
                    {
                        usage = "Enabled Live Stats.";
                        this.livestats = true;
                        this.chatid = message.Chat.Id;
                    }

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
                }
                else if (message.Text.StartsWith("/information"))
                {
                    // send custom keyboard
                    var usage = string.Empty;
                    if (this.informations)
                    {
                        usage = "Disabled Information.";
                        this.informations = false;
                        this.chatid = -1;
                    }
                    else
                    {
                        usage = "Enabled Information.";
                        this.informations = true;
                        this.chatid = message.Chat.Id;
                    }

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
                }
                else if (message.Text.StartsWith("/evolve"))
                {
                    var usage = "Evolving the shit out!";

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
                }
                else if (message.Text.StartsWith("/top"))
                {
                    int shows;
                    try
                    {
                        shows = int.Parse(message.Text.Replace("/top", string.Empty).Replace(" ", string.Empty));
                    }
                    catch (Exception)
                    {
                        var usage = "Error! This is not a Number: " + message.Text.Replace("/top", string.Empty).Replace(" ", string.Empty) + "!";

                        await this._telegram.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
                        return;
                    }

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, "Showing " + shows + " Pokemons...\nSorting...", replyMarkup: new ReplyKeyboardHide());

                    var myPokemons = await this._inventory.GetPokemons();
                    myPokemons = myPokemons.OrderByDescending(x => x.Cp);

                    var profil = await this._client.GetProfile();
                    var u = $"Top {shows} Pokemons of {profil.Profile.Username}!";

                    var count = 0;
                    foreach (var pokemon in myPokemons)
                    {
                        if (count == shows)
                            break;

                        u = u + "\n" + pokemon.PokemonId + " (" + StringUtils.getPokemonNameGer(pokemon.PokemonId) + ")  |  CP: " + pokemon.Cp + " (" + PokemonInfo.CalculatePokemonPerfection(pokemon) + "% perfect)";
                        count++;
                    }

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, u, replyMarkup: new ReplyKeyboardHide());
                }
                else if (message.Text.StartsWith("/forceevolve"))
                {
                    var pokemonToEvolve = await this._inventory.GetPokemonToEvolve(null);
                    if (pokemonToEvolve.Count() > 30)
                    {
                        // Use EGG - need to add this shit
                    }

                    foreach (var pokemon in pokemonToEvolve)
                    {
                        if (!this._clientSettings.pokemonsToEvolve.Contains(pokemon.PokemonId))
                        {
                            continue;
                        }

                        var evolvePokemonOutProto = await this._client.EvolvePokemon(pokemon.Id);

                        if (evolvePokemonOutProto.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
                        {
                            await this._telegram.SendTextMessageAsync(message.Chat.Id, $"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp", replyMarkup: new ReplyKeyboardHide());
                        }
                        else
                        {
                            await this._telegram.SendTextMessageAsync(message.Chat.Id, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", replyMarkup: new ReplyKeyboardHide());
                        }

                        await RandomHelper.RandomDelay(1000, 2000);
                    }

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, "Done.", replyMarkup: new ReplyKeyboardHide());
                }
                else
                {
                    var usage = @"Usage:
                    /stats   - Get Current Stats
                    /livestats - Enable/Disable Live Stats
                    /information - Enable/Disable Informations
                    /top <HowMany?> - Outputs Top (?) Pokemons
                    /forceevolve - Forces Evolve";

                    await this._telegram.SendTextMessageAsync(message.Chat.Id, usage, replyMarkup: new ReplyKeyboardHide());
                }
            }
            catch (Exception)
            {
            }
        }

        public async void DoInformation()
        {
            try
            {
                if (this.chatid != -1 && this.informations)
                {
                    var current = 0;
                    var usage = string.Empty;
                    var inventory = await this._client.GetInventory();
                    var profil = await this._client.GetProfile();
                    var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PlayerStats).ToArray();
                    foreach (var c in stats)
                    {
                        if (c != null)
                        {
                            current = c.Level;
                        }
                    }

                    if (current != this.level)
                    {
                        this.level = current;
                        usage = "You got Level Up! Your new Level is now " + this.level + "!";
                        await this._telegram.SendTextMessageAsync(this.chatid, usage, replyMarkup: new ReplyKeyboardHide());
                    }
                }

                await Task.Delay(5000);
                this.DoInformation();
            }
            catch (Exception)
            {
            }
        }

        public async void DoLiveStats(ISettings settings)
        {
            try
            {
                if (this.chatid != -1 && this.livestats)
                {
                    var usage = string.Empty;
                    var inventory = await this._client.GetInventory();
                    var profil = await this._client.GetProfile();
                    var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PlayerStats).ToArray();
                    foreach (var c in stats)
                    {
                        if (c != null)
                        {
                            var l = c.Level;

                            var expneeded = c.NextLevelXp - c.PrevLevelXp - StringUtils.getExpDiff(c.Level);
                            var curexp = c.Experience - c.PrevLevelXp - StringUtils.getExpDiff(c.Level);
                            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

                            usage += "\nNickname: " + profil.Profile.Username + "\nLevel: " + c.Level + "\nEXP Needed: " + (c.NextLevelXp - c.PrevLevelXp - StringUtils.getExpDiff(c.Level)) + $"\nCurrent EXP: {curexp} ({Math.Round(curexppercent)}%)" + "\nEXP to Level up: " + (c.NextLevelXp - c.Experience) + "\nKM walked: " + c.KmWalked + "\nPokeStops visited: " + c.PokeStopVisits + "\nStardust: " + profil.Profile.Currency.ToArray()[1].Amount;
                        }
                    }

                    await this._telegram.SendTextMessageAsync(this.chatid, usage, replyMarkup: new ReplyKeyboardHide());
                }

                await Task.Delay(settings.TelegramLiveStatsDelay);
                this.DoLiveStats(settings);
            }
            catch (Exception)
            {
            }
        }

        public TelegramBotClient getClient()
        {
            return this._telegram;
        }
    }
}