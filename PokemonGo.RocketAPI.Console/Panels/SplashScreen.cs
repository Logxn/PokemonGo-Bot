﻿/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/01/2017
 * Time: 1:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console.Panels
{
    /// <summary>
    /// Description of SplashScreen.
    /// </summary>
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
