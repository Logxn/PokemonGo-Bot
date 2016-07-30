namespace PokemonGo.RocketAPI.Console
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using GMap.NET;
    using GMap.NET.MapProviders;
    using GMap.NET.WindowsForms;

    using GoogleMapsApi;
    using GoogleMapsApi.Entities.Common;
    using GoogleMapsApi.Entities.Elevation.Request;
    using GoogleMapsApi.Entities.Elevation.Response;

    public partial class LocationSelect : Form
    {
        public double alt;
        public bool close = true;
        public GMapOverlay markersOverlay = new GMapOverlay("markers");

        private delegate void SetTextCallback(double cord);

        public LocationSelect()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Globals.latitute = this.map.Position.Lat;
            Globals.longitude = this.map.Position.Lng;
            Globals.altitude = this.alt;
            this.close = false;
            ActiveForm.Dispose();
        }

        private void LocationSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.close)
            {
                var result = MessageBox.Show("You didn't set start location! Are you sure you want to exit this window?", "Location selector", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No || result == DialogResult.Abort)
                {
                    e.Cancel = true;
                }
            }
        }

        private void map_Load(object sender, EventArgs e)
        {
            this.showMap();
        }

        private void map_OnMapDrag()
        {
            Task.Run(() =>
            {
                var request = new WebClient();
                var elevationRequest = new ElevationRequest
                                       {
                                           Locations = new[]
                                                       {
                                                           new Location(this.map.Position.Lat, this.map.Position.Lng)
                                                       }
                                       };
                try
                {
                    var elevation = GoogleMaps.Elevation.Query(elevationRequest);
                    if (elevation.Status == Status.OK)
                    {
                        foreach (var result in elevation.Results)
                        {
                            this.SetText(result.Elevation);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            });
            this.textBox1.Text = this.map.Position.Lat.ToString();
            this.textBox2.Text = this.map.Position.Lng.ToString();
        }

        private void SetText(double cord)
        {
            if (this.textBox3.InvokeRequired)
            {
                SetTextCallback d = this.SetText;
                this.Invoke(d, cord);
            }
            else
            {
                this.textBox3.Text = cord.ToString();
                this.alt = cord;
            }
        }

        private void showMap()
        {
            try
            {
                this.map.DragButton = MouseButtons.Left;
                this.map.MapProvider = GMapProviders.BingMap;
                this.map.Position = new PointLatLng(Globals.latitute, Globals.longitude);
                this.map.MinZoom = 0;
                this.map.MaxZoom = 20;
                this.map.Zoom = 16;

                this.textBox1.Text = Globals.latitute.ToString();
                this.textBox2.Text = Globals.longitude.ToString();
                this.textBox3.Text = Globals.altitude.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Length > 0 && this.textBox1.Text != "-")
            {
                try
                {
                    var lat = double.Parse(this.textBox1.Text.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (lat > 90.0 || lat < -90.0)
                        throw new ArgumentException("Value has to be between 180 and -180!");
                    this.map.Position = new PointLatLng(lat, this.map.Position.Lng);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.textBox1.Text = string.Empty;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox2.Text.Length > 0 && this.textBox2.Text != "-")
            {
                try
                {
                    var lng = double.Parse(this.textBox2.Text.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (lng > 180.0 || lng < -180.0)
                        throw new ArgumentException("Value has to be between 90 and -90!");
                    this.map.Position = new PointLatLng(this.map.Position.Lat, lng);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.textBox2.Text = string.Empty;
                }
            }
        }
    }
}