/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/09/2016
 * Time: 17:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Networking.Responses;
using POGOProtos.Enums;
using POGOProtos.Data;
using PokemonGo.RocketAPI.Logic.Utils;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using POGOProtos.Map.Fort;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of PlayerPanel.
    /// </summary>
    public partial class PlayerPanel : UserControl
    {
        private static GetPlayerResponse profile = null;
        private static IOrderedEnumerable<PokemonData> pokemons = null;
        public PlayerPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //

            pictureBoxBuddyPokemon.Visible = false;
            pictureBoxPlayerAvatar.Visible = false;
            pictureBoxTeam.Visible = false;

        }

        private bool buddyInfoEnabled = false;

        public bool BuddyInfoEnabled
        {
            get
            {
                return buddyInfoEnabled;
            }
            set
            {
                buddyInfoEnabled = value;
            }
        }

        public void Execute(GetPlayerResponse prof, IOrderedEnumerable<PokemonData> poks)
        {
            profile = prof;
            pokemons = poks;
            updatePlayerImages();
            updatePlayerInfoLabels();
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

            labelNoTeamSelected.Visible = false;
            labelNoBuddySelected.Visible = false;

            Control parent = null;
            if (profile.PlayerData.Team == TeamColor.Neutral)
            {
                labelNoTeamSelected.Location = new Point(0, 0);
                labelNoTeamSelected.Parent = pictureBoxTeam;
                labelNoTeamSelected.Width = pictureBoxTeam.Width;
                labelNoTeamSelected.Height = pictureBoxTeam.Height;
                labelNoTeamSelected.Visible = true;
                labelNoTeamSelected.TextAlign = ContentAlignment.TopCenter;
                parent = labelNoTeamSelected;
            }
            else
            {
                pictureBoxTeam.Location = new Point(0, 0);
                pictureBoxTeam.Image = getImageForTeam(profile.PlayerData.Team);
                pictureBoxTeam.Visible = true;
                parent = pictureBoxTeam;
                pictureBoxTeam.Refresh();
            }

            parent.Parent = panelLeftArea;
            parent.BringToFront();
            parent.Visible = true;
            parent.BackColor = Color.Transparent;

            pictureBoxPlayerAvatar.Parent = parent;
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
            pictureBoxPlayerAvatar.BackColor = Color.Transparent;
            pictureBoxPlayerAvatar.Visible = true;
            pictureBoxPlayerAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPlayerAvatar.Refresh();
            pictureBoxPlayerAvatar.BringToFront();

            pictureBoxBuddyPokemon.Parent = pictureBoxPlayerAvatar;
            var buddyLocation = new Point(45, pictureBoxPlayerAvatar.Height - pictureBoxBuddyPokemon.Height + 15);
            pictureBoxBuddyPokemon.Image = getImageForBuddy(profile.PlayerData.BuddyPokemon);
            pictureBoxBuddyPokemon.Location = buddyLocation;
            pictureBoxBuddyPokemon.BackColor = Color.Transparent;
            pictureBoxBuddyPokemon.Visible = buddyInfoEnabled;  
            //Changed this Section until 0.37 compatible!
            if (pictureBoxBuddyPokemon.Visible)
            {
                pictureBoxBuddyPokemon.BringToFront();
            }

              
            if (profile.PlayerData.BuddyPokemon == null || profile.PlayerData.BuddyPokemon.ToString() == "{ }")
            {
                labelNoBuddySelected.Parent = pictureBoxBuddyPokemon;
                //Changed this Section until 0.37 compatible!
                labelNoBuddySelected.Visible = buddyInfoEnabled;
                
                labelNoBuddySelected.Width = pictureBoxBuddyPokemon.Width - 35;
                labelNoBuddySelected.Height = pictureBoxBuddyPokemon.Height;
                labelNoBuddySelected.Location = new Point(10, 0);
                labelNoBuddySelected.TextAlign = ContentAlignment.MiddleCenter;
                if (labelNoBuddySelected.Visible)
                {
                    labelNoBuddySelected.BringToFront();
                }
            }
        }

        private async void updatePlayerInfoLabels()
        {
            if (profile == null)
                return;

            var client = Logic.Logic._client;
            var playerStats = await client.Inventory.GetPlayerStats();
            var stats = playerStats.First();

            labelUserProperty1Title.Text = "Username:";
            labelUserProperty1Value.Text = profile.PlayerData.Username;

            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

            var pokemonToEvolve = (await client.Inventory.GetPokemonToEvolve()).Count();
            var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(150) * 100;
            var pokedexpercent = Math.Floor(pokedexpercentraw);

            labelUserProperty2Title.Text = "Level:";
            var curexppercentrounded = Math.Round(curexppercent, 2);
            var kmWalked = Math.Round(stats.KmWalked, 2);

            labelUserProperty2Value.Text = string.Format("{0} | {1}/{2}({3}%)", stats.Level, curexp, expneeded, curexppercentrounded);

            labelUserProperty3Title.Text = "Stardust:";
            labelUserProperty3Value.Text = profile.PlayerData.Currencies[1].Amount.ToString("N0");

            labelUserProperty4Title.Text = "Pokemon:";
            labelUserProperty4Value.Text = string.Format("{0} + {1} Eggs / {2} ({3} Evolvable)", await client.Inventory.getPokemonCount(), await client.Inventory.GetEggsCount(), profile.PlayerData.MaxPokemonStorage, pokemonToEvolve);

            labelUserProperty5Title.Text = "Pokedex:";
            labelUserProperty5Value.Text = string.Format("{0}/ 150 [{1}%]", stats.UniquePokedexEntries, pokedexpercent);

            labelUserProperty6Title.Text = "Walked:";
            labelUserProperty6Value.Text = string.Format("{0}km", kmWalked);
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
                    return getPokemonImagefromResource(buddyPoke.PokemonId, "200");
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

        private static Bitmap getPokemonImagefromResource(PokemonId pokemon, string size)
        {
            var resource = PokemonGo.RocketAPI.Console.Properties.Resources.ResourceManager.GetObject(string.Format("_{0}_{1}", (int)pokemon, size), CultureInfo.CurrentCulture);
            if (resource != null && resource is Bitmap)
            {
                return new Bitmap(resource as Bitmap);
            }
            else
            {
                return null;
            }
        }
		private async void BtnTeamClick(object sender, EventArgs e)
		{
			var teamSelect =new TeamSelect();
			if (teamSelect.ShowDialog() == DialogResult.OK){
				
				
				// Simulate to enter in a gym before select a team.
				var client = Logic.Logic._client;
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
		                var resp2 = await SelectTeam(team);
		                if (resp2.Status)
		                {
		                	Logger.ColoredConsoleWrite(ConsoleColor.Green, "Selected Team: " + team.ToString());
		                	Execute(profile, pokemons);
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
            	var client = Logic.Logic._client;
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
		
		private static async Task<taskResponse> GetGym(string gym, double lat, double lng)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic._client;
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
		
		//GetGymDetails
    }
}
