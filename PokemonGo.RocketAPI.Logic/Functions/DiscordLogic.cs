/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 19/03/2017
 * Time: 11:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Linq;
using Discord;
using POGOProtos.Data;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using POGOProtos.Enums;
using PokemonGo.RocketAPI;

namespace PokeMaster.Logic.Functions
{
    /// <summary>
    /// Description of DiscordLogic.
    /// </summary>
    public static class DiscordLogic
    {
        private static DiscordClient client = null;
        private static Channel channel = null;
        
        public static void Init()
        {
            if (!GlobalVars.SendToDiscord){
                client = null;
                return ;
            }
            
            client = new DiscordClient();
            
            client.Connect(GlobalVars.DiscordUser, GlobalVars.DiscordPassword).Wait();
            var server = client.Servers.FirstOrDefault(x => x.Id == GlobalVars.DiscordServerID);
            if (server == null) {
                client.Disconnect();
                client = null;
                Logger.Info("Server not found");
                return;
            }
            channel = server.AllChannels.FirstOrDefault(x => x.Name == "shared_pokemons");
            if (channel == null) {
                client.Disconnect();
                client = null;
                Logger.Info("Channel not found");
                return;
            }
            client.MessageReceived += OnMessageReceived;
        }

        public static void SendMessage(string mesage)
        {
            if (!GlobalVars.SendToDiscord){
                return ;
            }
            var user = channel.GetUser(client.CurrentUser.Id);
            if (user == null)
                return;
            var rules = channel.GetPermissionsRule(user);
            if (rules.SendMessages == PermValue.Allow)
                channel.SendMessage(mesage);
        }

        public static void Finish()
        {
            if (client != null) {
                client.Disconnect();
                client = null;
                channel = null;
            }
        }
        public static string FormatMessage(WildPokemon wildPokemon, double dbliv, double probability){
           PokemonData pokemonData = wildPokemon?.PokemonData;
           var iv = (int) dbliv ;
           var ivIcon = (iv >= 90 ? iv == 100 ? ":100:" : ":ok_hand:" : "");
           var prob = (int) probability;
           var cp = pokemonData.Cp;
           var level = PokemonInfo.GetLevel(pokemonData);
           var gender = "";
           var genderIcon = "";
           if (pokemonData.PokemonDisplay.Gender == Gender.Male) {
               genderIcon = "♂";
               gender = " " + pokemonData.PokemonDisplay.Gender;
           } else if (pokemonData.PokemonDisplay.Gender == Gender.Female) {
               genderIcon = "♀";
               gender = " " + pokemonData.PokemonDisplay.Gender;
           }
           var form = "";
           if (pokemonData.PokemonDisplay.Form != Form.Unset) {
               form = $" ({pokemonData.PokemonDisplay.Form})";
               form = form.Replace("Unown", "");
           }
           var costume = pokemonData.PokemonDisplay.Costume.ToString();
           var address = LocationUtils.FindAddress(wildPokemon.Latitude, wildPokemon.Longitude);
           var despawn = StringUtils.TimeMStoString(wildPokemon.LastModifiedTimestampMs, @"mm\:ss"); 
           var tillhidden = StringUtils.TimeMStoString(wildPokemon.TimeTillHiddenMs, @"mm\:ss");
           var move1 = pokemonData.Move1;
           var move2 = pokemonData.Move2;
           var shiny = (pokemonData.PokemonDisplay.Shiny?"shiny":"");
           
           var shinyIcon = (shiny=="")?"":"✵";
          
           
           var message = "{EncounterId}= {ivIcon} {shinyIcon}**{name}**{genderIcon}{form} {lat}, {lon} ({address}) IV: {iv}% LV: {level} **CP: {cp}**{gender} {move1}/{move2} Costume: {costume} Prob: {prob}% {despawn} {tillhidden} {shiny}";
           
           message = message.Replace("{EncounterId}", "" + wildPokemon.EncounterId);
           message = message.Replace("{name}", "" + pokemonData.PokemonId);
           message = message.Replace("{iv}", "" + iv);
           message = message.Replace("{ivIcon}", "" + ivIcon);
           message = message.Replace("{prob}", "" + prob);
           message = message.Replace("{cp}", "" + cp);
           
           message = message.Replace("{level}", "" + level);
           message = message.Replace("{gender}", "" + gender);
           message = message.Replace("{genderIcon}", "" + genderIcon);
           message = message.Replace("{form}", "" + form);
           message = message.Replace("{costume}", "" + costume);

           message = message.Replace("{lat}", "" + wildPokemon.Latitude.ToString(CultureInfo.InvariantCulture));
           message = message.Replace("{lon}", "" + wildPokemon.Longitude.ToString(CultureInfo.InvariantCulture));
           message = message.Replace("{address}", "" + address);

           message = message.Replace("{despawn}", "" + despawn);
           message = message.Replace("{tillhidden}", "" + tillhidden);
           message = message.Replace("{move1}", "" + move1);
           message = message.Replace("{move2}", "" + move2);
           message = message.Replace("{shiny}", "" + shiny);
           message = message.Replace("{shinyIcon}", "" + shinyIcon);
           return message;
        }
        public static event EventHandler<DiscordLogic.DiscordReceivedDataEventArgs> MessageReceived;
        
        private static void OnMessageReceived(object s, MessageEventArgs e)
        {
            // Ignore target channels messages
            if (e.Channel!= channel ){
                return;
            }
            if (e.Message.IsAuthor ){
                return;
            }
            var args = new DiscordLogic.DiscordReceivedDataEventArgs();
            args.Message = e.Message.Text;
            args.Username = e.User.Nickname;
            if (string.IsNullOrEmpty(args.Username))
                args.Username = e.User.Name;
            MessageReceived?.Invoke(null, args);
        }
        public class DiscordReceivedDataEventArgs : EventArgs
        {
            public string Message {get;set;}
            public string Username {get;set;}
        }
    }
    
}
