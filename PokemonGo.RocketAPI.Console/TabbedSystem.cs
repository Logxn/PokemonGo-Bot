using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Device.Location;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Shared;

namespace PokemonGo.RocketAPI.Console
{
    public partial class TabbedSystem : Form
    {
        private static GetPlayerResponse profile;
        private static POGOProtos.Data.Player.PlayerStats stats;
        public static bool skipReadyToUse = false;
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        public class taskResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public taskResponse()
            {
            }
            public taskResponse(bool status, string message)
            {
                Status = status;
                Message = message;
            }
        }

        public TabbedSystem()
        {
            InitializeComponent();
            th.Translate(this);
            changesPanel1.Execute();
            changesPanel1.OnChangeLanguage = TranslateAll;
            webPanel1.AddButtonClick(new System.EventHandler(this.HideWebPanel));
            sniperPanel1.AddButtonClick( new System.EventHandler(this.AddLink));
            sniperPanel1.webBrowser = webPanel1.webBrowser1;

            if (!GlobalVars.EnableConsoleInTab) {
                loggerPanel1.Visible=false;
                splitContainer1.Panel2Collapsed = true;
            }


       }
        

        private void Pokemons_Load(object sender, EventArgs e)
        {
            locationPanel1.Init(true, 0, 0, 0);
            Execute();
            sniperPanel1.Execute();
            pokemonsPanel1.playerPanel1 = playerPanel1;

        }

        private void Pokemons_Close(object sender, FormClosingEventArgs e)
        {
            if (!GlobalVars.EnableConsoleInTab) {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void Execute()
        {
            try {
                var client = Logic.Logic.objClient;
                if (!skipReadyToUse){
                    // Wait to client is ready to use
                    while (client == null || !client.ReadyToUse) {
                        Logger.Debug("Client not ready to use. Waiting 5 seconds to retry");
                        RandomHelper.RandomSleep(5000, 5100);
                        client = Logic.Logic.objClient;
                    }
                    profile = client.Player.GetPlayer();
                    RandomHelper.RandomSleep(1000, 1100); // Pause to simulate human speed.
                    Text = "User: " + profile.PlayerData.Username;
                    var arrStats = client.Inventory.GetPlayerStats().GetEnumerator();
                    arrStats.MoveNext();
                    stats = arrStats.Current;
                    locationPanel1.CreateBotMarker((int)profile.PlayerData.Team, stats.Level, stats.Experience);
                    playerPanel1.setProfile(profile);
                    //pokemonsPanel1.profile = profile;
                }
            } catch (Exception e) {
                Logger.Error("[PokemonList-Error] " + e.StackTrace);
                RandomHelper.RandomSleep(1000, 1100);  // Lets the API make a little pause, so we dont get blocked
            }
        }

        private void CreateRoute_Click(object sender, EventArgs e)
        {
            if (CreateRoute.Text.Equals("Define Route")) {
                GlobalVars.pauseAtPokeStop = true;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Create Route Enabled - Click Pokestops in the order you would like to walk them and then Click 'Run Route'");
                if (GlobalVars.RouteToRepeat.Count > 0) {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Defined Route Cleared!");
                    GlobalVars.RouteToRepeat.Clear();
                }
                CreateRoute.Text = "Run Route";
                RepeatRoute.Enabled = true;
            } else {
                GlobalVars.pauseAtPokeStop = false;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Resume walking between Pokestops.");
                if (GlobalVars.RouteToRepeat.Count > 0) {
                    foreach (var geocoord in GlobalVars.RouteToRepeat) {
                        GlobalVars.NextDestinationOverride.AddLast(geocoord);
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
            TabPage current = (sender as TabControl).SelectedTab;
            switch (current.Name) {
                case "tpPokemons":
                    pokemonsPanel1.Execute(profile);
                    break;
                case "tpItems":
                    itemsPanel1.Execute(profile);
                    break;
                case "tpEggs":
                    eggsPanel1.Execute();
                    break;
                case "tpPlayerInfo":
                    playerPanel1.Execute();
                    break;
            }
        }
        public void ShowWebPanel()
        {
            if (!TabControl1.Contains(tpWeb)) {
                TabControl1.Controls.Add(tpWeb);
            }
        }
        public void HideWebPanel(object sender, EventArgs e)
        {
            if (TabControl1.Contains(tpWeb)) {
                TabControl1.Controls.Remove(tpWeb);
            }        	
        }
        public void AddLink(object sender, EventArgs e)
        {
            if (!sniperPanel1.checkBoxExternalWeb.Checked){
                ShowWebPanel();
                webPanel1.EnableIE11Emulation();
                TabControl1.SelectedTab = tpWeb;
            }
        }
        public void TranslateAll(){
            th.Translate(this);
            th.Translate(locationPanel1);
            th.Translate(pokemonsPanel1);
            th.Translate(itemsPanel1);
            th.Translate(eggsPanel1);
            th.Translate(playerPanel1);
            th.Translate(sniperPanel1);
            th.Translate(webPanel1);
        }
    }
}