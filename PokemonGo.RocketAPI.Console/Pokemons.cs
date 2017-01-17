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
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Console
{
    public partial class Pokemons : Form
    {
        private static GetPlayerResponse profile;
        private static POGOProtos.Data.Player.PlayerStats stats;
        public static ISettings ClientSettings;
        public bool waitingApiResponse = false;
        private Panels.WebPanel webPanel;

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
            if (Globals.consoleInTab){
                this.TabControl1.Controls.Add(this.tpConsole);
                Logger.type = 1;
            }
            ClientSettings = new Settings();
            changesPanel1.Execute();
            webPanel1.AddButtonClick(new System.EventHandler(this.HideWebPanel));
            sniperPanel1.AddLinkClick(0,new System.EventHandler(this.AddLink));
            sniperPanel1.AddLinkClick(1,new System.EventHandler(this.AddLink));
            sniperPanel1.AddLinkClick(2,new System.EventHandler(this.AddLink));
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
            if (!Globals.consoleInTab){
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        public void waitToBeReady()
        {
            while (true)
            {
                try
                {
                    if (Logic.Logic.objClient != null && Logic.Logic.objClient.readyToUse != false)
                    {
                        break;
                    }
                }
                catch (Exception) { }
            }
        }

        private void Execute()
        {
            TabControl1.Enabled = false;
            waitToBeReady();

            try
            {
                var client = Logic.Logic.objClient;
                if (client.readyToUse != false)
                {
                    profile = client.Player.GetPlayer().Result;
                    RandomHelper.RandomSleep(1000,1100); // Pause to simulate human speed.
                    Text = "User: " + profile.PlayerData.Username;
                    var arrStats = client.Inventory.GetPlayerStats().Result;
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
                RandomHelper.RandomSleep(1000,1100);  // Lets the API make a little pause, so we dont get blocked
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

        private void ChangeTabs(object sender, EventArgs e)
        {
            while (waitingApiResponse)
            {
                RandomHelper.RandomSleep(1000,1100);
            }
            waitingApiResponse = true;
            TabPage current = (sender as TabControl).SelectedTab;
            switch (current.Name)
            {
                case "tpPokemons":
                    pokemonsPanel1.Execute();
                    break;
                case "tpItems":
                    itemsPanel1.Execute();
                    break;
                case "tpEggs":
                    eggsPanel1.Execute();
                    break;
                case "tpPlayerInfo":
                    playerPanel1.Execute();
                    break;
            }
            waitingApiResponse = false;
        }
        public void ShowInWebPanel( string weburl){
        	if (!TabControl1.Contains(tpWeb))
        	{
        		TabControl1.Controls.Add(tpWeb);
        		
        	}
      		webPanel.ChangeURL(weburl);
        }
        public void HideWebPanel(object sender, EventArgs e){
        	if (TabControl1.Contains(tpWeb))
        	{
        		TabControl1.Controls.Remove(tpWeb);
        	}        	
        }
        public void AddLink(object sender, EventArgs e){
        	var lbl = (LinkLabel ) sender;
        	webPanel1.ChangeURL(lbl.Tag.ToString());
        }
        	
    }
}