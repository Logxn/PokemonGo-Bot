/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/01/2017
 * Time: 22:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using POGOLib.Official.Logging;
using PokemonGo.RocketAPI;
using Telegram.Bot;

namespace PokeMaster.Logic.Functions
{
    /// <summary>
    /// Description of TelegramLogic.
    /// </summary>
    public static class TelegramLogic
    {
        public static void Instantiante()
        {
            if (!string.IsNullOrEmpty(Shared.GlobalVars.TelegramAPIToken) && !string.IsNullOrEmpty(Shared.GlobalVars.TelegramName))
            {
                try
                {
                    Logic.Instance.Telegram = new Utils.TelegramUtil(
                        Logic.objClient,
                        new TelegramBotClient(Shared.GlobalVars.TelegramAPIToken),
                        Logic.Instance.BotSettings,
                        Logic.objClient.Inventory);

                    Logger.Info( "To activate informations with Telegram, write the bot a message for more informations");

                    var me = Logic.Instance.Telegram.getClient().GetMeAsync().Result;
                    Logic.Instance.Telegram.getClient().OnCallbackQuery += Logic.Instance.Telegram.BotOnCallbackQueryReceived;
                    Logic.Instance.Telegram.getClient().OnMessage += Logic.Instance.Telegram.BotOnMessageReceived;
                    Logic.Instance.Telegram.getClient().OnMessageEdited += Logic.Instance.Telegram.BotOnMessageReceived;

                    Logger.Info( $"Telegram Name: {me.Username}");

                    Logic.Instance.Telegram.getClient().StartReceiving();
                }
                catch (Exception ex1)
                {
                    Logger.Debug("Exception: "+  ex1.ToString());
                }
            }
        }
        public static void Stop()
        {
            if (Logic.Instance.Telegram!=null)
            {
                try
                {
                    Logic.Instance.Telegram.getClient().StopReceiving();
                }
                catch (Exception ex1)
                {
                    var realerror = ex1;
                    while (realerror.InnerException != null)
                        realerror = realerror.InnerException;
                    Logger.Debug("Exception: "+ ex1.Message+"/"+realerror.ToString());
                }
            }
        }
        
    }
}
