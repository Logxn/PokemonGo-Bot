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
        private PokemonGo.RocketAPI.Console.Components.LabelCombo lcEyes;
        
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
            this.lcEyes = new PokemonGo.RocketAPI.Console.Components.LabelCombo();
            this.groupBoxGender.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxGender
            // 
            this.groupBoxGender.Controls.Add(this.radioButtonRandom);
            this.groupBoxGender.Controls.Add(this.radioButtonFemale);
            this.groupBoxGender.Controls.Add(this.radioButtonMale);
            this.groupBoxGender.Location = new System.Drawing.Point(3, 3);
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
            // lcEyes
            // 
            this.lcEyes.Location = new System.Drawing.Point(209, 10);
            this.lcEyes.Name = "lcEyes";
            this.lcEyes.SelectedIndex = -1;
            this.lcEyes.Size = new System.Drawing.Size(265, 29);
            this.lcEyes.TabIndex = 1;
            this.lcEyes.TextLabel = "Eyes";
            // 
            // TutorialPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lcEyes);
            this.Controls.Add(this.groupBoxGender);
            this.Name = "TutorialPanel";
            this.Size = new System.Drawing.Size(493, 329);
            this.groupBoxGender.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
