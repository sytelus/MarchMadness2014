﻿namespace GamePredictor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonMain = new System.Windows.Forms.Button();
            this.buttonCalibrateRegularization = new System.Windows.Forms.Button();
            this.buttonHomeAdvantage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonMain
            // 
            this.buttonMain.Location = new System.Drawing.Point(29, 12);
            this.buttonMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonMain.Name = "buttonMain";
            this.buttonMain.Size = new System.Drawing.Size(195, 53);
            this.buttonMain.TabIndex = 0;
            this.buttonMain.Text = "Run Main";
            this.buttonMain.UseVisualStyleBackColor = true;
            this.buttonMain.Click += new System.EventHandler(this.buttonMain_Click);
            // 
            // buttonCalibrateRegularization
            // 
            this.buttonCalibrateRegularization.Location = new System.Drawing.Point(29, 140);
            this.buttonCalibrateRegularization.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCalibrateRegularization.Name = "buttonCalibrateRegularization";
            this.buttonCalibrateRegularization.Size = new System.Drawing.Size(195, 50);
            this.buttonCalibrateRegularization.TabIndex = 1;
            this.buttonCalibrateRegularization.Text = "Calibrate Regularization";
            this.buttonCalibrateRegularization.UseVisualStyleBackColor = true;
            this.buttonCalibrateRegularization.Click += new System.EventHandler(this.buttonCalibrateRegularization_Click);
            // 
            // buttonHomeAdvantage
            // 
            this.buttonHomeAdvantage.Location = new System.Drawing.Point(29, 258);
            this.buttonHomeAdvantage.Name = "buttonHomeAdvantage";
            this.buttonHomeAdvantage.Size = new System.Drawing.Size(195, 46);
            this.buttonHomeAdvantage.TabIndex = 2;
            this.buttonHomeAdvantage.Text = "Calculate Home Advantage";
            this.buttonHomeAdvantage.UseVisualStyleBackColor = true;
            this.buttonHomeAdvantage.Click += new System.EventHandler(this.buttonHomeAdvantage_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 466);
            this.Controls.Add(this.buttonHomeAdvantage);
            this.Controls.Add(this.buttonCalibrateRegularization);
            this.Controls.Add(this.buttonMain);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonMain;
        private System.Windows.Forms.Button buttonCalibrateRegularization;
        private System.Windows.Forms.Button buttonHomeAdvantage;
    }
}

