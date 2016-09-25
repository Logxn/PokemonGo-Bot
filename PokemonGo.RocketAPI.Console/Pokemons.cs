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
            pokemonsPanel1.init();
            pokemonsPanel1.Execute();
            Execute();
            locationPanel1.Init(true, 0, 0, 0);
            itemsPanel1.Execute();
            eggsPanel1.Execute();
            sniperPanel1.Execute();
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
                    if (Logic.Logic._client != null && Logic.Logic._client.readyToUse != false)
                    {
                        break;
                    }
                }
                catch (Exception) { }
            }
        }

        private async void Execute()
        {
            await check();
            try
            {
                var client = Logic.Logic._client;
                if (client.readyToUse != false)
                {                    
                    profile = await client.Player.GetPlayer();
                    await Task.Delay(1000); // Pause to simulate human speed. 
                    Text = "Pokemon List | User: " + profile.PlayerData.Username + " | Pokemons: " + pokemonsPanel1.pokemons.Count() + "/" + profile.PlayerData.MaxPokemonStorage;
                    var arrStats = await client.Inventory.GetPlayerStats();
                    stats = arrStats.First();
                    playerPanel1.setProfile(profile);
                    playerPanel1.Execute();
                    locationPanel1.CreateBotMarker((int)profile.PlayerData.Team, stats.Level, stats.Experience);
                }
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
    }
}
