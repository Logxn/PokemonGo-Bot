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
        private System.Windows.Forms.GroupBox groupBoxGender;
        private System.Windows.Forms.RadioButton radioButtonRandom;
        private System.Windows.Forms.RadioButton radioButtonFemale;
        private System.Windows.Forms.RadioButton radioButtonMale;
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
            this.groupBoxGender = new System.Windows.Forms.GroupBox();
            this.radioButtonRandom = new System.Windows.Forms.RadioButton();
            this.radioButtonFemale = new System.Windows.Forms.RadioButton();
            this.radioButtonMale = new System.Windows.Forms.RadioButton();
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
            this.ltNickPrefix = new PokemonGo.RocketAPI.Console.Components.LabelText();
            this.ltNickSufix = new PokemonGo.RocketAPI.Console.Components.LabelText();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxGender.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxGender
            // 
            this.groupBoxGender.Controls.Add(this.radioButtonRandom);
            this.groupBoxGender.Controls.Add(this.radioButtonFemale);
            this.groupBoxGender.Controls.Add(this.radioButtonMale);
            this.groupBoxGender.Location = new System.Drawing.Point(274, 13);
            this.groupBoxGender.Name = "groupBoxGender";
            this.groupBoxGender.Size = new System.Drawing.Size(200, 114);
            this.groupBoxGender.TabIndex = 0;
            this.groupBoxGender.TabStop = false;
            this.groupBoxGender.Text = "Gender";
            // 
            // radioButtonRandom
            // 
            this.radioButtonRandom.Location = new System.Drawing.Point(6, 79);
            this.radioButtonRandom.Name = "radioButtonRandom";
            this.radioButtonRandom.Size = new System.Drawing.Size(188, 24);
            this.radioButtonRandom.TabIndex = 2;
            this.radioButtonRandom.TabStop = true;
            this.radioButtonRandom.Text = "Random";
            this.radioButtonRandom.UseVisualStyleBackColor = true;
            // 
            // radioButtonFemale
            // 
            this.radioButtonFemale.Location = new System.Drawing.Point(6, 49);
            this.radioButtonFemale.Name = "radioButtonFemale";
            this.radioButtonFemale.Size = new System.Drawing.Size(188, 24);
            this.radioButtonFemale.TabIndex = 1;
            this.radioButtonFemale.TabStop = true;
            this.radioButtonFemale.Text = "Female";
            this.radioButtonFemale.UseVisualStyleBackColor = true;
            // 
            // radioButtonMale
            // 
            this.radioButtonMale.Location = new System.Drawing.Point(6, 19);
            this.radioButtonMale.Name = "radioButtonMale";
            this.radioButtonMale.Size = new System.Drawing.Size(188, 24);
            this.radioButtonMale.TabIndex = 0;
            this.radioButtonMale.TabStop = true;
            this.radioButtonMale.Text = "Male";
            this.radioButtonMale.UseVisualStyleBackColor = true;
            // 
            // lcBackpack
            // 
            this.lcBackpack.Caption = "Backpack";
            this.lcBackpack.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcBackpack.Location = new System.Drawing.Point(3, 13);
            this.lcBackpack.Name = "lcBackpack";
            this.lcBackpack.SelectedIndex = 0;
            this.lcBackpack.SeparatorPoint = 109;
            this.lcBackpack.Size = new System.Drawing.Size(265, 29);
            this.lcBackpack.TabIndex = 1;
            // 
            // lcEyes
            // 
            this.lcEyes.Caption = "Eyes";
            this.lcEyes.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcEyes.Location = new System.Drawing.Point(3, 45);
            this.lcEyes.Name = "lcEyes";
            this.lcEyes.SelectedIndex = 0;
            this.lcEyes.SeparatorPoint = 109;
            this.lcEyes.Size = new System.Drawing.Size(265, 29);
            this.lcEyes.TabIndex = 2;
            // 
            // lcHair
            // 
            this.lcHair.Caption = "Hair";
            this.lcHair.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcHair.Location = new System.Drawing.Point(3, 80);
            this.lcHair.Name = "lcHair";
            this.lcHair.SelectedIndex = 0;
            this.lcHair.SeparatorPoint = 109;
            this.lcHair.Size = new System.Drawing.Size(265, 29);
            this.lcHair.TabIndex = 3;
            // 
            // lcShirt
            // 
            this.lcShirt.Caption = "Shirt";
            this.lcShirt.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcShirt.Location = new System.Drawing.Point(3, 182);
            this.lcShirt.Name = "lcShirt";
            this.lcShirt.SelectedIndex = 0;
            this.lcShirt.SeparatorPoint = 109;
            this.lcShirt.Size = new System.Drawing.Size(265, 29);
            this.lcShirt.TabIndex = 6;
            // 
            // lcPants
            // 
            this.lcPants.Caption = "Pants";
            this.lcPants.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcPants.Location = new System.Drawing.Point(3, 147);
            this.lcPants.Name = "lcPants";
            this.lcPants.SelectedIndex = 0;
            this.lcPants.SeparatorPoint = 109;
            this.lcPants.Size = new System.Drawing.Size(265, 29);
            this.lcPants.TabIndex = 5;
            // 
            // lcHat
            // 
            this.lcHat.Caption = "Hat";
            this.lcHat.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcHat.Location = new System.Drawing.Point(3, 115);
            this.lcHat.Name = "lcHat";
            this.lcHat.SelectedIndex = 0;
            this.lcHat.SeparatorPoint = 109;
            this.lcHat.Size = new System.Drawing.Size(265, 29);
            this.lcHat.TabIndex = 4;
            // 
            // lcSkin
            // 
            this.lcSkin.Caption = "Skin";
            this.lcSkin.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcSkin.Location = new System.Drawing.Point(3, 252);
            this.lcSkin.Name = "lcSkin";
            this.lcSkin.SelectedIndex = 0;
            this.lcSkin.SeparatorPoint = 109;
            this.lcSkin.Size = new System.Drawing.Size(265, 29);
            this.lcSkin.TabIndex = 8;
            // 
            // lcShoes
            // 
            this.lcShoes.Caption = "Shoes";
            this.lcShoes.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.lcShoes.Location = new System.Drawing.Point(3, 217);
            this.lcShoes.Name = "lcShoes";
            this.lcShoes.SelectedIndex = 0;
            this.lcShoes.SeparatorPoint = 109;
            this.lcShoes.Size = new System.Drawing.Size(265, 29);
            this.lcShoes.TabIndex = 7;
            this.lcShoes.UseWaitCursor = true;
            // 
            // lcPokemon
            // 
            this.lcPokemon.Caption = "Starter Pokemon";
            this.lcPokemon.Items.AddRange(new object[] {
            "Picachu",
            "Bulbasaur",
            "Squirtle",
            "Charmander"});
            this.lcPokemon.Location = new System.Drawing.Point(274, 147);
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
            this.groupBox1.Location = new System.Drawing.Point(280, 182);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 115);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Nicname";
            // 
            // ltNickPrefix
            // 
            this.ltNickPrefix.Caption = "Prefix";
            this.ltNickPrefix.Location = new System.Drawing.Point(12, 19);
            this.ltNickPrefix.Name = "ltNickPrefix";
            this.ltNickPrefix.SeparatorPoint = 60;
            this.ltNickPrefix.Size = new System.Drawing.Size(182, 24);
            this.ltNickPrefix.TabIndex = 0;
            // 
            // ltNickSufix
            // 
            this.ltNickSufix.Caption = "Sufix";
            this.ltNickSufix.Location = new System.Drawing.Point(12, 49);
            this.ltNickSufix.Name = "ltNickSufix";
            this.ltNickSufix.SeparatorPoint = 60;
            this.ltNickSufix.Size = new System.Drawing.Size(182, 24);
            this.ltNickSufix.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nickname will formed:\r\nPrefix{countIfNeeded}Sufix";
            // 
            // TutorialPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Controls.Add(this.groupBoxGender);
            this.Name = "TutorialPanel";
            this.Size = new System.Drawing.Size(498, 317);
            this.groupBoxGender.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
