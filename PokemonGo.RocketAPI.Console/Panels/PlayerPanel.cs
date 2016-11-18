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
        
        public PlayerPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
        }


        public async void Execute( bool refreshData = true)
        {
            pictureBoxTeam.Image = null;
            pictureBoxPlayerAvatar.Image = null;
            pictureBoxBuddyPokemon.Image = null;
            labelUserProperty1Value.Text = "";
            labelUserProperty2Value.Text = "";
            labelUserProperty3Value.Text = "";
            labelUserProperty4Value.Text = "";
            labelUserProperty5Value.Text = "";
            labelUserProperty6Value.Text = "";

            await check();
            var client = Logic.Logic.Client;
            if (client.readyToUse != false)
            {
                /*labelUserProperty1Title.Text = "Username:";  TODO: internationalize*/

                if (refreshData)
                {
                    profile = await client.Player.GetPlayer();
                    await Task.Delay(1000); // Pause to simulate human speed. 
                    var playerStats = await client.Inventory.GetPlayerStats();
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
            switch (team)
            {
                case TeamColor.Neutral:
                    return null;
                    break;
                case TeamColor.Blue:
                    return Properties.Resources.team_mystic;
                    break;
                case TeamColor.Red:
                    return Properties.Resources.team_valor;
                    break;
                case TeamColor.Yellow:
                    return Properties.Resources.team_instinct;
                    break;
                default:
                    return null;
                    break;
            }
        }
        private void updatePlayerImages()
        {
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
            pictureBoxTeam.Refresh();

            pictureBoxPlayerAvatar.Parent = pictureBoxTeam;
            pictureBoxPlayerAvatar.BackColor = Color.Transparent;
            if (profile.PlayerData.Avatar != null)
            {
                pictureBoxPlayerAvatar.Image = getImageForGender(profile.PlayerData.Avatar.Gender);
            }
            else
            {
                pictureBoxPlayerAvatar.Image = getImageForGender(Gender.Male);
            }
            pictureBoxPlayerAvatar.Height = (int)(pictureBoxTeam.Height * 0.85);
            pictureBoxPlayerAvatar.Width = pictureBoxTeam.Width;
            var playerLocation = new Point(0, pictureBoxTeam.Height - pictureBoxPlayerAvatar.Height);
            pictureBoxPlayerAvatar.Location = playerLocation;
            pictureBoxPlayerAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPlayerAvatar.Refresh();

            pictureBoxBuddyPokemon.Parent = pictureBoxPlayerAvatar;
            pictureBoxBuddyPokemon.BackColor = Color.Transparent;
            var buddyLocation = new Point(45, pictureBoxPlayerAvatar.Height - pictureBoxBuddyPokemon.Height + 15);
            pictureBoxBuddyPokemon.Image = getImageForBuddy(profile.PlayerData.BuddyPokemon);
            pictureBoxBuddyPokemon.Location = buddyLocation;
        }

        private async void updatePlayerInfoLabels()
        {

            if (profile != null){
                labelUserProperty1Value.Text = profile.PlayerData.Username;
                labelUserProperty3Value.Text = profile.PlayerData.Currencies[1].Amount.ToString("N0");
            }


            if (stats != null){
                
                var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
                var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
                var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

                var curexppercentrounded = Math.Round(curexppercent, 2);
                labelUserProperty2Value.Text = string.Format("{0} | {1}/{2}({3}%)", stats.Level, curexp, expneeded, curexppercentrounded);

                var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(150) * 100;
                var pokedexpercent = Math.Floor(pokedexpercentraw);
                labelUserProperty5Value.Text = string.Format("{0}/ 150 [{1}%]", stats.UniquePokedexEntries, pokedexpercent);

                var kmWalked = Math.Round(stats.KmWalked, 2);
                labelUserProperty6Value.Text = string.Format("{0}km", kmWalked);

                /*
                var pokemonToEvolve = (await client.Inventory.GetPokemonToEvolve()).Count();
                labelUserProperty4Value.Text = string.Format("{0} + {1} Eggs / {2} ({3} Evolvable)", await client.Inventory.getPokemonCount(), await client.Inventory.GetEggsCount(), profile.PlayerData.MaxPokemonStorage, pokemonToEvolve);
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
        private Image getImageForGender(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return Properties.Resources.Trainer_M;
                case Gender.Female:
                    return Properties.Resources.Trainer_F;
                default:
                    return Properties.Resources.Trainer_M;
            }
        }

        private async void BtnTeamClick(object sender, EventArgs e)
        {
            var teamSelect =new TeamSelect();
            if (teamSelect.ShowDialog() == DialogResult.OK){
                // Simulate to enter in a gym before select a team.
                var client = Logic.Logic.Client;
                var mapObjects = await client.Map.GetMapObjects();
                var mapCells = mapObjects.Item1.MapCells;

                var pokeGyms = mapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Gym );
	            if (pokeGyms.Any() )
	            {
	            	var pokegym = pokeGyms.First();

                    var resp = await GetGym(pokegym.Id,pokegym.Latitude,pokegym.Longitude);
                    if (resp.Status)
                    {
                        var team = teamSelect.selected;
                        await Task.Delay(1000); // Pause to simulate human speed. 
                        var resp2 = await SelectTeam(team);
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
            	var client = Logic.Logic.Client;
            	var resp2 = await client.Player.SetPlayerTeam(teamColor);

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
                await SelectTeam(teamColor);
            }
            return resp1;
        }	
        //GetGymDetails
        private static async Task<taskResponse> GetGym(string gym, double lat, double lng)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic.Client;
            	var resp2 = await client.Fort.GetGymDetails( gym,lat,lng);

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
                await GetGym(gym,lat,lng);
            }
            return resp1;
        }			
        public async Task check()
        {
            while (true)
            {
                try
                {
                    if (Logic.Logic.Client != null && Logic.Logic.Client.readyToUse != false)
                    {
                        break;
                    }
                }
                catch (Exception) { }
            }
        }		

    }
}
