﻿/*
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
                    return Properties.Resources.mystic;
                    break;
                case TeamColor.Red:
                    return Properties.Resources.valor;
                    break;
                case TeamColor.Yellow:
                    return Properties.Resources.instinct;
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
            if (profile.PlayerData.Avatar != null)
                pictureBoxPlayerAvatar.Image = getImageForGender(profile.PlayerData.Avatar.Gender);

            pictureBoxTeam.Location = new Point(0, 0);
            pictureBoxTeam.Image = getImageForTeam(profile.PlayerData.Team);
            Control parent = pictureBoxTeam;
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

            pictureBoxTeam.Refresh();

            pictureBoxPlayerAvatar.Parent = parent;
            var playerLocation = new Point(pictureBoxTeam.Width - (pictureBoxTeam.Width / 2) - (pictureBoxPlayerAvatar.Width / 2), pictureBoxTeam.Height - pictureBoxPlayerAvatar.Height);
            pictureBoxPlayerAvatar.Height = (int)(pictureBoxTeam.Height * 0.75);
            pictureBoxPlayerAvatar.Width = pictureBoxTeam.Width;
            pictureBoxPlayerAvatar.Location = playerLocation;
            pictureBoxPlayerAvatar.BackColor = Color.Transparent;
            pictureBoxPlayerAvatar.BringToFront();

            pictureBoxPlayerAvatar.Refresh();

            pictureBoxBuddyPokemon.Parent = pictureBoxPlayerAvatar;
            var buddyLocation = new Point(60, pictureBoxPlayerAvatar.Height - pictureBoxBuddyPokemon.Height);
            pictureBoxBuddyPokemon.Image = getImageForBuddy(profile.PlayerData.BuddyPokemon);
            pictureBoxBuddyPokemon.Location = buddyLocation;
            pictureBoxBuddyPokemon.BackColor = Color.Transparent;
            pictureBoxBuddyPokemon.BringToFront();
            //Changed this Section until 0.37 compatible!
            pictureBoxBuddyPokemon.Visible = false;
            //if (profile.PlayerData.BuddyPokemon == null || profile.PlayerData.BuddyPokemon.ToString() == "{ }")
            if (true == true)
            {
                labelNoBuddySelected.Parent = pictureBoxBuddyPokemon;
                //Changed this Section until 0.37 compatible!
                labelNoBuddySelected.Visible = false;
                labelNoBuddySelected.Width = pictureBoxBuddyPokemon.Width - 35;
                labelNoBuddySelected.Height = pictureBoxBuddyPokemon.Height;
                labelNoBuddySelected.Location = new Point(0, 0);
                labelNoBuddySelected.TextAlign = ContentAlignment.MiddleCenter;
                labelNoBuddySelected.BringToFront();
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
                    return Properties.Resources.boy;
                case Gender.Female:
                    return Properties.Resources.girl;
                default:
                    return Properties.Resources.boy;
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
				
				//TODO: Simulate to enter in a gym before select a team.
				
				var team = teamSelect.selected;				
                var resp = await SelectTeam(team);
                if (resp.Status)
                {
                	Execute(profile, pokemons);
                }
                else
                    MessageBox.Show(resp.Message + "Set Team failed!", "Set Team Status", MessageBoxButtons.OK);				
			}
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
    }
}
