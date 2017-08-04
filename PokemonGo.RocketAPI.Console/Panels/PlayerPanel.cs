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
using PokeMaster.Dialogs;
using PokeMaster.Logic.Utils;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Data.Player;

namespace PokeMaster
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
                new ListViewItem(new string[] {"1", th.TS("Username"),""}),
                new ListViewItem(new string[] {"2", th.TS("Coins"),""}),
                new ListViewItem(new string[] {"3", th.TS("Stardust"),""}),
                new ListViewItem(new string[] {"4", th.TS("Max Items"),""}),
                new ListViewItem(new string[] {"5", th.TS("Max Pokemons"),""}),
                new ListViewItem(new string[] {"6", th.TS("Battle Lockout End (Ms)"),""}),
                new ListViewItem(new string[] {"7", th.TS("Level"),""}),
                new ListViewItem(new string[] {"8", th.TS("Pokedex"),""}),
                new ListViewItem(new string[] {"9", th.TS("Kms Walked"),""}),
                new ListViewItem(new string[] {"10", th.TS("Eggs Hatched"),""}),
                new ListViewItem(new string[] {"11", th.TS("Evolutions"),""}),
                new ListViewItem(new string[] {"12", th.TS("PokeStop Visits"),""}),
                new ListViewItem(new string[] {"13", th.TS("Pokeballs Thrown"),""}),
                new ListViewItem(new string[] {"14", th.TS("Battle Attack"),""}),
                new ListViewItem(new string[] {"15", th.TS("Battle Defended"),""}),
                new ListViewItem(new string[] {"16", th.TS("Battle Training"),""}),
                new ListViewItem(new string[] {"17", th.TS("Pokemon Deployed"),""}),
                new ListViewItem(new string[] {"18", th.TS("Pokemons Captured"),""}),
                new ListViewItem(new string[] {"19", th.TS("Pokemons Encountered"),""}),
                new ListViewItem(new string[] {"20", th.TS("Prestige Dropped"),""}),
                new ListViewItem(new string[] {"21", th.TS("Prestige Raised"),""}),
                new ListViewItem(new string[] {"22", th.TS("Small Rattata Caught"),""}),
                new ListViewItem(new string[] {"23", th.TS("Big Magikarp Caught"),""}),
                new ListViewItem(new string[] {"24", th.TS("Used Km Pool"),""})
                        });
        }

        public void Execute( bool refreshData = true)
        {
            pictureBoxTeam.Image = null;
            pictureBoxPlayerAvatar.Image = null;
            pictureBoxBuddyPokemon.Image = null;
            foreach (ListViewItem element in listView.Items) {
                element.SubItems[2].Text= "";
            }

            var client = Logic.Logic.objClient;
            if (client !=null && client.ReadyToUse)
            {
                if (refreshData)
                {
                    profile = client.Player.GetPlayer();
                    RandomHelper.RandomSleep(300,400);
                    var playerStats = client.Inventory.GetPlayerStats();
                    stats = playerStats.First();
                }
                updatePlayerImages();
                updatePlayerInfoLabels();
            }
            labelPokemons.Text = ""+ Logic.Functions.Setout.pokemonCatchCount;
            labelPokestops.Text = ""+ Logic.Functions.Setout.pokeStopFarmedCount;
            labelTimeLeft.Text = ""+ Logic.Functions.Setout.timeLeftToNextLevel;
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
            pictureBoxPlayerAvatar.Image = profile.PlayerData.Avatar != null ? getImageForGender((PlayerAvatarType)profile.PlayerData.Avatar.Avatar) : getImageForGender(PlayerAvatarType.PlayerAvatarMale);
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
                listView.Items[0].SubItems[2].Text = profile.PlayerData.Username;
                listView.Items[1].SubItems[2].Text = profile.PlayerData.Currencies[0].Amount.ToString("N0");
                listView.Items[2].SubItems[2].Text = profile.PlayerData.Currencies[1].Amount.ToString("N0");
                listView.Items[3].SubItems[2].Text = ""+profile.PlayerData.MaxItemStorage;
                listView.Items[4].SubItems[2].Text = ""+profile.PlayerData.MaxPokemonStorage;
                listView.Items[5].SubItems[2].Text = ""+profile.PlayerData.BattleLockoutEndMs;
            }


            if (stats != null){
                
                var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
                var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
                var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

                var curexppercentrounded = Math.Round(curexppercent, 2);
                listView.Items[6].SubItems[2].Text = string.Format("{0} | {1}/{2}({3}%)", stats.Level, curexp, expneeded, curexppercentrounded);

                var numDifferentPokemons = Enum.GetNames(typeof(PokemonId)).Length -1;
                var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(numDifferentPokemons) * 100;
                var pokedexpercent = Math.Floor(pokedexpercentraw);
                listView.Items[7].SubItems[2].Text = string.Format("{0}/ {1} [{2}%]", stats.UniquePokedexEntries,numDifferentPokemons, pokedexpercent);

                var kmWalked = Math.Round(stats.KmWalked, 2);
                listView.Items[8].SubItems[2].Text = string.Format("{0}km", kmWalked); 
                listView.Items[9].SubItems[2].Text = "" + stats.EggsHatched;
                listView.Items[10].SubItems[2].Text = "" + stats.Evolutions;
                listView.Items[11].SubItems[2].Text = "" + stats.PokeStopVisits;
                listView.Items[12].SubItems[2].Text = "" + stats.PokeballsThrown;
                listView.Items[13].SubItems[2].Text = $"{stats.BattleAttackWon} / {stats.BattleAttackTotal}";
                listView.Items[14].SubItems[2].Text = ""+stats.BattleDefendedWon;
                listView.Items[15].SubItems[2].Text = $"{stats.BattleTrainingWon} / {stats.BattleTrainingTotal}";
                listView.Items[16].SubItems[2].Text = "" + stats.PokemonDeployed;
                listView.Items[17].SubItems[2].Text = "" + stats.PokemonsCaptured;
                listView.Items[18].SubItems[2].Text = "" + stats.PokemonsEncountered;
                listView.Items[19].SubItems[2].Text = "" + stats.PrestigeDroppedTotal;
                listView.Items[20].SubItems[2].Text = "" + stats.PrestigeRaisedTotal;
                listView.Items[21].SubItems[2].Text = "" + stats.SmallRattataCaught;
                listView.Items[22].SubItems[2].Text = "" + stats.BigMagikarpCaught;
                listView.Items[23].SubItems[2].Text = "" + stats.UsedKmPool;
                
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
                return buddyPoke != null ? PokeImgManager.GetPokemonImagefromResource(buddyPoke.PokemonId, "200") : null;
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
                var mapCells = mapObjects.MapCells;

                var pokeGyms = mapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Gym );
	            if (pokeGyms.Any() )
	            {
	            	var pokegym = pokeGyms.First();

                    var resp = GetGym(pokegym.Id, pokegym.Latitude, pokegym.Longitude);
                    if (resp.Status)
                    {
                        var team = teamSelect.selected;
                        RandomHelper.RandomSleep(1000,1100);
                        var resp2 = SelectTeam(team);
                        if (resp2.Status)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Selected Team: " + team.ToString());
                            Execute();
                        }
                        else
                            MessageBox.Show(resp.Message + th.TS("Set Team failed!"), th.TS("Set Team Status"), MessageBoxButtons.OK);
                    }
	                else
	                    MessageBox.Show(resp.Message + th.TS("Set Team failed!"), th.TS("Set Team Status"), MessageBoxButtons.OK);
	            }
	            else
	                MessageBox.Show(th.TS("Set Team failed!\n non nearby Gym "), th.TS("Set Team Status"), MessageBoxButtons.OK);
			}
            else
                MessageBox.Show(th.TS("Set Team canceled!"), th.TS("Set Team Status"), MessageBoxButtons.OK);

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
		private static taskResponse SelectTeam(TeamColor teamColor)
        {
            var resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic.objClient;
            	var resp2 = client.Player.SetPlayerTeam(teamColor);

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
                SelectTeam(teamColor);
            }
            return resp1;
        }	
        //GetGymDetails
        private static taskResponse GetGym(string gym, double lat, double lng)
        {
            var resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic.objClient;
            	var resp2 = client.Fort.GymGetInfo( gym,lat,lng);

                if (resp2.Result == GymGetInfoResponse.Types.Result.Success)
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
                GetGym(gym,lat,lng);
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
                catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }
            }
        }

        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var order = (sender as ListView).Sorting;
            listView.ListViewItemSorter = new Components.ListViewItemComparer(e.Column, order);
            (sender as ListView).Sorting = order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
    }
}
