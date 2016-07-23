using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

using AllEnum;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Logic;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    public class TelegramUtil
    {

        private Client _client;

        private Telegram.Bot.TelegramBotClient _telegram;

        private readonly ISettings _clientSettings;

        private long chatid = -1;
        private bool livestats = false;

        private bool informations = false;

        public TelegramUtil(Client client, Telegram.Bot.TelegramBotClient telegram, ISettings settings)
        {
            _client = client;
            _telegram = telegram;
            _clientSettings = settings;
            DoLiveStats();
            DoInformation();
        }

        public Telegram.Bot.TelegramBotClient getClient()
        {
            return _telegram;
        }

        public async void DoLiveStats()
        {
            if (chatid != -1 && livestats)
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

                        usage += "\nNickname: " + profil.Profile.Username +
                            "\nLevel: " + c.Level
                            + "\nEXP Needed: " + ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level))
                            + "\nCurrent EXP: " + ((c.Experience - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level))
                            + "\nEXP to Level up: " + ((c.NextLevelXp) - (c.Experience))
                            + "\nKM walked: " + c.KmWalked
                            + "\nPokeStops visited: " + c.PokeStopVisits
                            + "\nStardust: " + profil.Profile.Currency.ToArray()[1].Amount;
                    }
                }
                
                await _telegram.SendTextMessageAsync(chatid, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
            await Task.Delay(5000);
            DoLiveStats();
        }

         int level;
        
        public async void DoInformation()
        {
            if (chatid != -1 && informations)
            {
                int current = 0;
                var usage = "";
                var inventory = await _client.GetInventory();
                var profil = await _client.GetProfile();
                var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PlayerStats).ToArray();
                foreach (var c in stats)
                {
                    if (c != null)
                    {
                        current = c.Level;
                    }
                }

                if (current != level)
                {
                    level = current;
                    usage = "You got Level Up! Your new Level is now " + level + "!";
                    await _telegram.SendTextMessageAsync(chatid, usage,
                   replyMarkup: new ReplyKeyboardHide());
                }
            }
            await Task.Delay(5000);
            DoInformation();
        }


        public async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            
            if (message == null || message.Type != MessageType.TextMessage) return;

            Logger.ColoredConsoleWrite(ConsoleColor.Red, "[TelegramAPI]Got Request from " + messageEventArgs.Message.From.Username + " | " + message.Text);

            if (messageEventArgs.Message.From.Username != _clientSettings.TelegramName)
            {
                var usage = "I dont hear at you!";
                await _telegram.SendTextMessageAsync(message.Chat.Id, usage,
                   replyMarkup: new ReplyKeyboardHide());
                return;
            }

            if (message.Text.StartsWith("/stats")) // send inline keyboard
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

                        usage += "\nNickname: " + profil.Profile.Username + 
                            "\nLevel: " + c.Level
                            + "\nEXP Needed: " + ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level))
                            + "\nCurrent EXP: " + ((c.Experience - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level))
                            + "\nEXP to Level up: " + ((c.NextLevelXp) - (c.Experience))
                            + "\nKM walked: " + c.KmWalked
                            + "\nPokeStops visited: " + c.PokeStopVisits
                            + "\nStardust: " + profil.Profile.Currency.ToArray()[1].Amount;
                    }
                }
                await _telegram.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
            else if (message.Text.StartsWith("/livestats")) // send custom keyboard
            {
                var usage = "";
                if (livestats)
                {
                    usage = "Disabled Live Stats.";
                    livestats = false;
                    chatid = -1;
                } else
                {
                    usage = "Enabled Live Stats.";
                    livestats = true;
                    chatid = message.Chat.Id;
                }
                await _telegram.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
            else if (message.Text.StartsWith("/informations")) // send custom keyboard
            {
                var usage = "";
                if (livestats)
                {
                    usage = "Disabled Informations.";
                    informations = false;
                    chatid = -1;
                }
                else
                {
                    usage = "Enabled Informations.";
                    informations = true;
                    chatid = message.Chat.Id;
                }
                await _telegram.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
            else if (message.Text.StartsWith("/evolve"))
            {
                var usage = "Evolving the shit out!";
                
                await _telegram.SendTextMessageAsync(message.Chat.Id, usage,
                   replyMarkup: new ReplyKeyboardHide());
            }
            else
            {
                var usage = @"Usage:
                    /stats   - Get Current Stats
                    /livestats - Enable/Disable Live Stats
                    /informations - Enable/Disable Informations
                    ";

                await _telegram.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
        }


        public async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await _telegram.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }
    }
}
