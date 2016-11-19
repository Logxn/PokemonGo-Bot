using POGOProtos.Data;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Console.PokeData;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Device.Location;

namespace PokemonGo.RocketAPI.Console
{
    public partial class Pokemons : Form
    {
        private static GetPlayerResponse profile;
        private static POGOProtos.Data.Player.PlayerStats stats;
        public static ISettings ClientSettings;
        public bool waitingApiResponse = false;

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

        public Pokemons()
        {
            InitializeComponent();
            ClientSettings = new Settings();
            changesPanel1.Execute();
        }

        private void Pokemons_Load(object sender, EventArgs e)
        {
            Globals.pauseAtPokeStop = false;
            locationPanel1.Init(true, 0, 0, 0);
            Execute();
            sniperPanel1.Execute();
            pokemonsPanel1.playerPanel1 = playerPanel1;

        }

        private void Pokemons_Close(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
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

        private async void Execute()
        {
            TabControl1.Enabled = false;
            await check();
            try
            {
                var client = Logic.Logic.Client;
                if (client.readyToUse != false)
                {                    
                    profile = await client.Player.GetPlayer();
                    await Task.Delay(1000); // Pause to simulate human speed. 
                    Text = "User: " + profile.PlayerData.Username;
                    var arrStats = await client.Inventory.GetPlayerStats();
                    stats = arrStats.First();
                    locationPanel1.CreateBotMarker((int)profile.PlayerData.Team, stats.Level, stats.Experience);
                    playerPanel1.setProfile(profile);
                    pokemonsPanel1.profile = profile;
                }
                TabControl1.Enabled = true;
            }
            catch (Exception e)
            {
                Logger.Error("[PokemonList-Error] " + e.StackTrace);
                await Task.Delay(1000); // Lets the API make a little pause, so we dont get blocked
                Execute();
            }
        }

        private void CreateRoute_Click(object sender, EventArgs e)
        {
            if (CreateRoute.Text.Equals("Define Route"))
            {
                Globals.pauseAtPokeStop = true;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Create Route Enabled - Click Pokestops in the order you would like to walk them and then Click 'Run Route'");
                if (Globals.RouteToRepeat.Count > 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Defined Route Cleared!");
                    Globals.RouteToRepeat.Clear();
                }
                CreateRoute.Text = "Run Route";
                RepeatRoute.Enabled = true;
            }
            else
            {
                Globals.pauseAtPokeStop = false;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Resume walking between Pokestops.");
                if (Globals.RouteToRepeat.Count > 0)
                {
                    foreach (var geocoord in Globals.RouteToRepeat)
                    {
                        Globals.NextDestinationOverride.AddLast(geocoord);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Defined Route Captured! Beginning Route Momentarily.");
                }
                CreateRoute.Text = "Define Route";
                RepeatRoute.Enabled = false;
            }
        }
        void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTabs(sender, e);
        }

        private async Task ChangeTabs(object sender, EventArgs e)
        {
            while (waitingApiResponse)
            {
                await Task.Delay(1000);
            }
            waitingApiResponse = true;
            TabPage current = (sender as TabControl).SelectedTab;
            switch (current.Name)
            {
                case "tpPokemons":
                    await pokemonsPanel1.Execute();
                    break;
                case "tpItems":
                    await itemsPanel1.Execute();
                    break;
                case "tpEggs":
                    await eggsPanel1.Execute();
                    break;
                case "tpPlayerInfo":
                    await playerPanel1.Execute();
                    break;
            }
            waitingApiResponse = false;
        }
    }
}