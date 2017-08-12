/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 07/03/2017
 * Time: 0:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Enums;
using PokemonGo.RocketAPI.Helpers;

namespace PokeMaster.Dialogs
{
    /// <summary>
    /// Description of EvolvingDialog.
    /// </summary>
    public partial class EvolvingDialog : System.Windows.Forms.Form
    {
        public EvolvingDialog()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        public void RunAnimation( PokemonId source, PokemonId target)
        {
            var sourceImage = PokeImgManager.GetPokemonVeryLargeImage(source);
            var targetImage = PokeImgManager.GetPokemonVeryLargeImage(target);
            pictureBox1.Image = sourceImage;
            pictureBox2.Visible =false;
            progressBar1.Value = 0;
            pictureBox1.Location = new Point(26, 222);
            for (var i = 0; i < 44;i++){
                Application.DoEvents();
                progressBar1.Value += progressBar1.Maximum/(48);
                RandomHelper.RandomSleep(550);
                var x = pictureBox1.Location.X;
                var y = pictureBox1.Location.Y - 5;
                pictureBox1.Location = new Point(x,y);
            }
            pictureBox1.Image =  targetImage;
            for (var i = 0; i < 8;i++){
                Application.DoEvents();
                progressBar1.Value += progressBar1.Maximum/(48)/2;
                RandomHelper.RandomSleep(275);
                var x = pictureBox1.Location.X;
                var y = pictureBox1.Location.Y + 27;
                pictureBox1.Location = new Point(x,y);
            }
            progressBar1.Value = progressBar1.Maximum;
            for (var i = 0; i < 8;i++){
                Application.DoEvents();
                RandomHelper.RandomSleep(275);
            }
        }
        public void RunAnimationOld( PokemonId source, PokemonId target)
        {
            var sourceImage = PokeImgManager.GetPokemonVeryLargeImage(source);
            var targetImage = PokeImgManager.GetPokemonVeryLargeImage(target);
            pictureBox1.Image = sourceImage;
            pictureBox2.Image = null;
            pictureBox2.Visible =true;
            progressBar1.Value = 0;
            const int times = 12;
            for (var i = 0; i < times;i++){
                progressBar1.Value += progressBar1.Maximum/times;
                pictureBox1.Image =  (i % 2 == 0)?sourceImage:null;
                pictureBox2.Image =  (i % 2 == 1)?targetImage:null;
                Refresh();
                RandomHelper.RandomSleep(2400);
            }
        }
    }
}
