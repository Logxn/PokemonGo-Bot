/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/01/2017
 * Time: 14:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console.Panels
{
    partial class TutorialPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcBackpack;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcEyes;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcHair;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcShirt;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcPants;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcHat;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcSkin;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcShoes;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcPokemon;
        private System.Windows.Forms.GroupBox groupBox1;
        private PokemonGo.RocketAPI.Console.Components.LabelText ltNickPrefix;
        private System.Windows.Forms.Label label1;
        private PokemonGo.RocketAPI.Console.Components.LabelText ltNickSufix;
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcGender;
        
        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.lcBackpack = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcEyes = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcHair = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcShirt = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcPants = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcHat = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcSkin = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcShoes = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.lcPokemon = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ltNickSufix = new PokemonGo.RocketAPI.Console.Components.LabelText();
            this.ltNickPrefix = new PokemonGo.RocketAPI.Console.Components.LabelText();
            this.lcGender = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lcBackpack
            // 
            this.lcBackpack.Caption = "Backpack";
            this.lcBackpack.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "Random"});
            this.lcBackpack.Location = new System.Drawing.Point(3, 252);
            this.lcBackpack.Name = "lcBackpack";
            this.lcBackpack.SelectedIndex = 3;
            this.lcBackpack.SeparatorPoint = 109;
            this.lcBackpack.Size = new System.Drawing.Size(265, 29);
            this.lcBackpack.TabIndex = 1;
            // 
            // lcEyes
            // 
            this.lcEyes.Caption = "Eyes";
            this.lcEyes.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "Random"});
            this.lcEyes.Location = new System.Drawing.Point(3, 80);
            this.lcEyes.Name = "lcEyes";
            this.lcEyes.SelectedIndex = 4;
            this.lcEyes.SeparatorPoint = 109;
            this.lcEyes.Size = new System.Drawing.Size(265, 29);
            this.lcEyes.TabIndex = 2;
            // 
            // lcHair
            // 
            this.lcHair.Caption = "Hair";
            this.lcHair.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "Random"});
            this.lcHair.Location = new System.Drawing.Point(3, 45);
            this.lcHair.Name = "lcHair";
            this.lcHair.SelectedIndex = 6;
            this.lcHair.SeparatorPoint = 109;
            this.lcHair.Size = new System.Drawing.Size(265, 29);
            this.lcHair.TabIndex = 3;
            // 
            // lcShirt
            // 
            this.lcShirt.Caption = "Shirt";
            this.lcShirt.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "Random"});
            this.lcShirt.Location = new System.Drawing.Point(3, 147);
            this.lcShirt.Name = "lcShirt";
            this.lcShirt.SelectedIndex = 3;
            this.lcShirt.SeparatorPoint = 109;
            this.lcShirt.Size = new System.Drawing.Size(265, 29);
            this.lcShirt.TabIndex = 6;
            // 
            // lcPants
            // 
            this.lcPants.Caption = "Pants";
            this.lcPants.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "Random"});
            this.lcPants.Location = new System.Drawing.Point(3, 182);
            this.lcPants.Name = "lcPants";
            this.lcPants.SelectedIndex = 3;
            this.lcPants.SeparatorPoint = 109;
            this.lcPants.Size = new System.Drawing.Size(265, 29);
            this.lcPants.TabIndex = 5;
            // 
            // lcHat
            // 
            this.lcHat.Caption = "Hat";
            this.lcHat.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "Random"});
            this.lcHat.Location = new System.Drawing.Point(3, 115);
            this.lcHat.Name = "lcHat";
            this.lcHat.SelectedIndex = 3;
            this.lcHat.SeparatorPoint = 109;
            this.lcHat.Size = new System.Drawing.Size(265, 29);
            this.lcHat.TabIndex = 4;
            // 
            // lcSkin
            // 
            this.lcSkin.Caption = "Skin";
            this.lcSkin.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "Random"});
            this.lcSkin.Location = new System.Drawing.Point(3, 13);
            this.lcSkin.Name = "lcSkin";
            this.lcSkin.SelectedIndex = 4;
            this.lcSkin.SeparatorPoint = 109;
            this.lcSkin.Size = new System.Drawing.Size(265, 29);
            this.lcSkin.TabIndex = 8;
            // 
            // lcShoes
            // 
            this.lcShoes.Caption = "Shoes";
            this.lcShoes.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "Random"});
            this.lcShoes.Location = new System.Drawing.Point(3, 217);
            this.lcShoes.Name = "lcShoes";
            this.lcShoes.SelectedIndex = 3;
            this.lcShoes.SeparatorPoint = 109;
            this.lcShoes.Size = new System.Drawing.Size(265, 29);
            this.lcShoes.TabIndex = 7;
            this.lcShoes.UseWaitCursor = true;
            // 
            // lcPokemon
            // 
            this.lcPokemon.Caption = "Starter Pokemon";
            this.lcPokemon.Items.AddRange(new object[] {
            "Bulbasaur",
            "Charmander",
            "Squirtle",
            "Picachu",
            "Random"});
            this.lcPokemon.Location = new System.Drawing.Point(280, 80);
            this.lcPokemon.Name = "lcPokemon";
            this.lcPokemon.SelectedIndex = 0;
            this.lcPokemon.SeparatorPoint = 100;
            this.lcPokemon.Size = new System.Drawing.Size(200, 29);
            this.lcPokemon.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ltNickSufix);
            this.groupBox1.Controls.Add(this.ltNickPrefix);
            this.groupBox1.Location = new System.Drawing.Point(280, 131);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 115);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Nicname";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nickname will be formed as:\r\nPrefix{countIfNeeded}Sufix";
            // 
            // ltNickSufix
            // 
            this.ltNickSufix.Caption = "Sufix";
            this.ltNickSufix.Location = new System.Drawing.Point(12, 49);
            this.ltNickSufix.Name = "ltNickSufix";
            this.ltNickSufix.SeparatorPoint = 60;
            this.ltNickSufix.Size = new System.Drawing.Size(182, 24);
            this.ltNickSufix.TabIndex = 1;
            this.ltNickSufix.Value = "";
            // 
            // ltNickPrefix
            // 
            this.ltNickPrefix.Caption = "Prefix";
            this.ltNickPrefix.Location = new System.Drawing.Point(12, 19);
            this.ltNickPrefix.Name = "ltNickPrefix";
            this.ltNickPrefix.SeparatorPoint = 60;
            this.ltNickPrefix.Size = new System.Drawing.Size(182, 24);
            this.ltNickPrefix.TabIndex = 0;
            this.ltNickPrefix.Value = "Ash";
            // 
            // lcGender
            // 
            this.lcGender.Caption = "Gender";
            this.lcGender.Items.AddRange(new object[] {
            "Male",
            "Female",
            "Random"});
            this.lcGender.Location = new System.Drawing.Point(280, 45);
            this.lcGender.Name = "lcGender";
            this.lcGender.SelectedIndex = 2;
            this.lcGender.SeparatorPoint = 100;
            this.lcGender.Size = new System.Drawing.Size(200, 29);
            this.lcGender.TabIndex = 11;
            // 
            // TutorialPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lcGender);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lcPokemon);
            this.Controls.Add(this.lcSkin);
            this.Controls.Add(this.lcShoes);
            this.Controls.Add(this.lcShirt);
            this.Controls.Add(this.lcPants);
            this.Controls.Add(this.lcHat);
            this.Controls.Add(this.lcHair);
            this.Controls.Add(this.lcEyes);
            this.Controls.Add(this.lcBackpack);
            this.Name = "TutorialPanel";
            this.Size = new System.Drawing.Size(498, 315);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
