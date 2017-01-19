/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/01/2017
 * Time: 17:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace TestTranlate
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        TranslatorHelper th = new TranslatorHelper();
        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            th.Translate(this);
        }
        void button1_Click(object sender, EventArgs e)
        {
            th.ExtractTexts(this);
        }
        void button2_Click(object sender, EventArgs e)
        {
            TranslatorHelper.Translate(this,"es");
        }
    }
}
