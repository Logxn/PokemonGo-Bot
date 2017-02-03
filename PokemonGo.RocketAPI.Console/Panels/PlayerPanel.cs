/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/09/2016
 * Time: 17:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Data.Player;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of PlayerPanel.
    /// </summary>
    public partial class PlayerPanel : UserControl
    {
        private static GetPlayerResponse profile = null;
        private static IOrderedEnumerable<PokemonData> pokemons = null;
        private static POGOProtos.Data.Player.PlayerStats stats;
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        public PlayerPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            InitializeData();
            th.Translate(this);
            
        }
        
        private void InitializeData()
        {
            this.listView.Items.AddRange(new [] {
                new ListViewItem(new string[] { "Username",""}),
                new ListViewItem(new string[] {"Coins",""}),
                new ListViewItem(new string[] {"Stardust",""}),
                new ListViewItem(new string[] {"Max Items",""}),
                new ListViewItem(new string[] {"Max Pokemons",""}),
                new ListViewItem(new string[] {"Battle Lockout End (Ms)",""}),
                new ListViewItem(new string[] {"Level",""}),
                new ListViewItem(new string[] {"Pokedex",""}),
                new ListViewItem(new string[] {"Kms Walked",""}),
                new ListViewItem(new string[] {"Eggs Hatched",""}),
                new ListViewItem(new string[] {"Evolutions",""}),
                new ListViewItem(new string[] {"PokeStop Visits",""}),
                new ListViewItem(new string[] {"Pokeballs Thrown",""}),
                new ListViewItem(new string[] {"Battle Attack",""}),
                new ListViewItem(new string[] {"Battle Defended",""}),
                new ListViewItem(new string[] {"Battle Training",""}),
                new ListViewItem(new string[] {"Pokemon Deployed",""}),
                new ListViewItem(new string[] {"Pokemons Captured",""}),
                new ListViewItem(new string[] {"Pokemons Encountered",""}),
                new ListViewItem(new string[] {"Prestige Dropped",""}),
                new ListViewItem(new string[] {"Prestige Raised",""}),
                new ListViewItem(new string[] {"Small Rattata Caught",""}),
                new ListViewItem(new string[] {"Big Magikarp Caught",""}),
                new ListViewItem(new string[] {"Used Km Pool",""})
                        });
        }

        public void Execute( bool refreshData = true)
        {
            pictureBoxTeam.Image = null;
            pictureBoxPlayerAvatar.Image = null;
            pictureBoxBuddyPokemon.Image = null;
            foreach (ListViewItem element in listView.Items) {
                element.SubItems[1].Text= "";
            }

            var client = Logic.Logic.objClient;
            if (client !=null && client.ReadyToUse)
            {
                if (refreshData)
                {
                    profile = client.Player.GetPlayer().Result;
                    RandomHelper.RandomSleep(300,400);
                    var playerStats = client.Inventory.GetPlayerStats().Result;
                    stats = playerStats.First();
                }
                updatePlayerImages();
                updatePlayerInfoLabels();
            }
        }
        
        public void setProfile(GetPlayerResponse prof){
            profile = prof;
        }

        public void SetPokemons( IOrderedEnumerable<PokemonData> poks)
        {
            pokemons = poks;
        }
        /// <summary>
        /// Gets the image for team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns></returns>
        private Image getImageForTeam(TeamColor team)
        {
            Image img = null;
            switch (team)
            {
                case TeamColor.Blue:
                    img = Properties.Resources.team_mystic;
                    break;
                case TeamColor.Red:
                    img = Properties.Resources.team_valor;
                    break;
                case TeamColor.Yellow:
                    img = Properties.Resources.team_instinct;
                    break;
            }
            return img;
        }
        private void updatePlayerImages()
        {
            this.Enabled = false;
            if (profile == null)
                return;

            pictureBoxTeam.Parent = panelLeftArea;
            pictureBoxTeam.BackColor = Color.Transparent;
            pictureBoxTeam.Location = new Point(0, 0);
            pictureBoxTeam.Image = null;
            if (profile.PlayerData.Team != TeamColor.Neutral)
            {
                pictureBoxTeam.Image = getImageForTeam(profile.PlayerData.Team);
            }

            pictureBoxPlayerAvatar.Parent = pictureBoxTeam;
            pictureBoxPlayerAvatar.BackColor = Color.Transparent;
            if (profile.PlayerData.Avatar != null)
            {
                pictureBoxPlayerAvatar.Image = getImageForGender((PlayerAvatarType) profile.PlayerData.Avatar.Avatar);
            }
            else
            {
                pictureBoxPlayerAvatar.Image = getImageForGender(PlayerAvatarType.PlayerAvatarMale);
            }
            pictureBoxPlayerAvatar.Height = (int)(pictureBoxTeam.Height * 0.85);
            pictureBoxPlayerAvatar.Width = pictureBoxTeam.Width;
            var playerLocation = new Point(0, pictureBoxTeam.Height - pictureBoxPlayerAvatar.Height);
            pictureBoxPlayerAvatar.Location = playerLocation;
            pictureBoxPlayerAvatar.SizeMode = PictureBoxSizeMode.Zoom;

            pictureBoxBuddyPokemon.Parent = pictureBoxPlayerAvatar;
            pictureBoxBuddyPokemon.BackColor = Color.Transparent;
            var buddyLocation = new Point(45, pictureBoxPlayerAvatar.Height - pictureBoxBuddyPokemon.Height + 15);
            pictureBoxBuddyPokemon.Image = getImageForBuddy(profile.PlayerData.BuddyPokemon);
            pictureBoxBuddyPokemon.Location = buddyLocation;
            this.Enabled = true;
        }

        private void updatePlayerInfoLabels()
        {

            if (profile != null){
                listView.Items[0].SubItems[1].Text = profile.PlayerData.Username;
                listView.Items[1].SubItems[1].Text = profile.PlayerData.Currencies[0].Amount.ToString("N0");
                listView.Items[2].SubItems[1].Text = profile.PlayerData.Currencies[1].Amount.ToString("N0");
                listView.Items[3].SubItems[1].Text = ""+profile.PlayerData.MaxItemStorage;
                listView.Items[4].SubItems[1].Text = ""+profile.PlayerData.MaxPokemonStorage;
                listView.Items[5].SubItems[1].Text = ""+profile.PlayerData.BattleLockoutEndMs;
            }


            if (stats != null){
                
                var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
                var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
                var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

                var curexppercentrounded = Math.Round(curexppercent, 2);
                listView.Items[6].SubItems[1].Text = string.Format("{0} | {1}/{2}({3}%)", stats.Level, curexp, expneeded, curexppercentrounded);

                var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(150) * 100;
                var pokedexpercent = Math.Floor(pokedexpercentraw);
                listView.Items[7].SubItems[1].Text = string.Format("{0}/ 150 [{1}%]", stats.UniquePokedexEntries, pokedexpercent);

                var kmWalked = Math.Round(stats.KmWalked, 2);
                listView.Items[8].SubItems[1].Text = string.Format("{0}km", kmWalked); 
                listView.Items[9].SubItems[1].Text = "" + stats.EggsHatched;
                listView.Items[10].SubItems[1].Text = "" + stats.Evolutions;
                listView.Items[11].SubItems[1].Text = "" + stats.PokeStopVisits;
                listView.Items[12].SubItems[1].Text = "" + stats.PokeballsThrown;
                listView.Items[13].SubItems[1].Text = $"{stats.BattleAttackWon} / {stats.BattleAttackTotal}";
                listView.Items[14].SubItems[1].Text = ""+stats.BattleDefendedWon;
                listView.Items[15].SubItems[1].Text = $"{stats.BattleTrainingWon} / {stats.BattleTrainingTotal}";
                listView.Items[16].SubItems[1].Text = "" + stats.PokemonDeployed;
                listView.Items[17].SubItems[1].Text = "" + stats.PokemonsCaptured;
                listView.Items[18].SubItems[1].Text = "" + stats.PokemonsEncountered;
                listView.Items[19].SubItems[1].Text = "" + stats.PrestigeDroppedTotal;
                listView.Items[20].SubItems[1].Text = "" + stats.PrestigeRaisedTotal;
                listView.Items[21].SubItems[1].Text = "" + stats.SmallRattataCaught;
                listView.Items[22].SubItems[1].Text = "" + stats.BigMagikarpCaught;
                listView.Items[23].SubItems[1].Text = "" + stats.UsedKmPool;
                
                /*
                var pokemonToEvolve = (await client.Inventory.GetPokemonToEvolve()).Count().ConfigureAwait(false);
                labelUserProperty4Value.Text = string.Format("{0} + {1} Eggs / {2} ({3} Evolvable)", await client.Inventory.getPokemonCount().ConfigureAwait(false), await client.Inventory.GetEggsCount().ConfigureAwait(false), profile.PlayerData.MaxPokemonStorage, pokemonToEvolve).ConfigureAwait(false);
                */
            }

        }

        /// <summary>
        /// Gets the image for buddy.
        /// </summary>
        /// <param name="buddyPokemon">The buddy pokemon.</param>
        /// <returns></returns>
        private Image getImageForBuddy(BuddyPokemon buddyPokemon)
        {
            if (pokemons == null)
                return null;

            if (buddyPokemon == null || buddyPokemon.ToString() == "{ }")
            {
                return null;
            }
            else
            {
                var buddyPoke = pokemons.FirstOrDefault(x => x.Id == buddyPokemon.Id);
                if (buddyPoke != null)
                {
                    return PokeImgManager.GetPokemonImagefromResource(buddyPoke.PokemonId, "200");
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the image for gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <returns></returns>
        private Image getImageForGender(PlayerAvatarType gender)
        {
            switch (gender)
            {
                case PlayerAvatarType.PlayerAvatarMale:
                    return Properties.Resources.Trainer_M;
                case PlayerAvatarType.PlayerAvatarFemale:
                    return Properties.Resources.Trainer_F;
                default:
                    return Properties.Resources.Trainer_M;
            }
        }

        private void BtnTeamClick(object sender, EventArgs e)
        {
            var teamSelect =new TeamSelect();
            if (teamSelect.ShowDialog() == DialogResult.OK){
                // Simulate to enter in a gym before select a team.
                var client = Logic.Logic.objClient;
                var mapObjects = client.Map.GetMapObjects().Result;
                var mapCells = mapObjects.Item1.MapCells;

                var pokeGyms = mapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Gym );
	            if (pokeGyms.Any() )
	            {
	            	var pokegym = pokeGyms.First();

                    var resp = GetGym(pokegym.Id, pokegym.Latitude, pokegym.Longitude).Result;
                    if (resp.Status)
                    {
                        var team = teamSelect.selected;
                        RandomHelper.RandomSleep(1000,1100);
                        var resp2 = SelectTeam(team).Result;
                        if (resp2.Status)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Selected Team: " + team.ToString());
                            Execute();
                        }
                        else
                        MessageBox.Show(resp.Message + "Set Team failed!", "Set Team Status", MessageBoxButtons.OK);
                    }
	                else
	                    MessageBox.Show(resp.Message + "Set Team failed!", "Set Team Status", MessageBoxButtons.OK);            	               
	            }
	            else
	            	MessageBox.Show("Set Team failed!\n non nearby Gym ", "Set Team Status", MessageBoxButtons.OK);
			}
            else
                MessageBox.Show("Set Team canceled!", "Set Team Status", MessageBoxButtons.OK);

        }

        public class taskResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public taskResponse() { }
            public taskResponse(bool status, string message)
            {
                Status = status;
                Message = message;
            }
        }
		private static async Task<taskResponse> SelectTeam(TeamColor teamColor)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic.objClient;
            	var resp2 = await client.Player.SetPlayerTeam(teamColor).ConfigureAwait(false);

                if (resp2.Status == SetPlayerTeamResponse.Types.Status.Success)
                {
                    resp1.Status = true;
                }
                else
                {
                	resp1.Message = teamColor.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error SelectTeam: " + e.Message);
                await SelectTeam(teamColor).ConfigureAwait(false);
            }
            return resp1;
        }	
        //GetGymDetails
        private static async Task<taskResponse> GetGym(string gym, double lat, double lng)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic.objClient;
            	var resp2 = await client.Fort.GetGymDetails( gym,lat,lng).ConfigureAwait(false);

                if (resp2.Result == GetGymDetailsResponse.Types.Result.Success)
                {
                    resp1.Status = true;
                }
                else
                {
                	resp1.Message = gym;
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error GetGym: " + e.Message);
                await GetGym(gym,lat,lng).ConfigureAwait(false);
            }
            return resp1;
        }			
        public void check()
        {
            while (true)
            {
                try
                {
                    if (Logic.Logic.objClient != null && Logic.Logic.objClient.ReadyToUse != false)
                    {
                        break;
                    }
                }
                catch (Exception) { }
            }
        }
        void btnColect_Click(object sender, EventArgs e)
        {
            collectCoins();
        }
        private  void collectCoins(){
            const string prefix = "(Coin Collection)";
            var res = Logic.Logic.objClient.Player.CollectDailyDefenderBonus().Result;

            var result = res.Result.ToString();
            var currentDefenders = res.DefendersCount;
            var currency = res.CurrencyType;
            var awardedCurrency = res.CurrencyAwarded;

            switch(res.Result.ToString())
            {
                
                case "NoDefenders":
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{prefix} - Result: You dont have any pokemons in a gym.");
                    break;
                case "Success": // May need to change this
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"{prefix} - Current Pokemons In Gyms: {currentDefenders} | Currency Type: {currency} | Awarded: {awardedCurrency} Coins");
                    break;
                case "Failure": // May need to change this
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"{prefix} - Failed!");
                    break;
                case "TooSoon": // May need to change this
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"{prefix} - Its not time yet to collect your coins!");
                    break;
                case "Unset": // May need to change this
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, prefix +"- Result Unset? => "+ result+" | Please screenshot this error and send it to us on Discord or GitHub"); //<-- This string is longer than "$" command supports.
                    break;
                default:
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"{prefix} - Result: {result} | Please screenshot this error and send it to us on Discord or GitHub");
                    break;
            }


            /*// TO-DO Save the last they in config and check if there were 24h between the last check

            var resultx = Logic.Logic.objClient.Player.CollectDailyBonus().Result;
            var resultString = resultx.Result.ToString();

            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Result: {resultx}");
            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Result string: {resultString}");
            switch (resultString)
            {
                case "Unset":
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"(Daily Bonus) - The result was unset!");
                    break;
                case "Success":
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"(Daily Bonus) - We've collected your daily bonus for you!");
                    break;
                case "Failure":
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"(Daily Bonus) - Failure!");
                    break;
                case "TooSoon":
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"(Daily Bonus) - It's to soon to collect your daily bonus!");
                    break;
                default:
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"(Daily Bonus) - Default switch statement reached! => {resultString} | Please screenshot this error and send it to us on Discord or GitHub");
                    break;
            }*/
        }
    }
}
