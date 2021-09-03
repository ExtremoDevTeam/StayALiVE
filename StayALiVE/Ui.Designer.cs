namespace StayALiVE
{
    partial class Ui
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
            this.StartSwitch = new MetroFramework.Controls.MetroToggle();
            this.pidValueBox = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // metroToggle1
            // 
            this.StartSwitch.AutoSize = true;
            this.StartSwitch.Location = new System.Drawing.Point(6, 12);
            this.StartSwitch.Name = "metroToggle1";
            this.StartSwitch.Size = new System.Drawing.Size(80, 17);
            this.StartSwitch.TabIndex = 0;
            this.StartSwitch.Text = "Off";
            this.StartSwitch.UseVisualStyleBackColor = true;
            this.StartSwitch.CheckedChanged += new System.EventHandler(Program.SwitchOnlineState);
            // 
            // pidValueBox
            // 
            this.pidValueBox.AutoSize = true;
            this.pidValueBox.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pidValueBox.Location = new System.Drawing.Point(12, 32);
            this.pidValueBox.Name = "pidValueBox";
            this.pidValueBox.Size = new System.Drawing.Size(21, 22);
            this.pidValueBox.TabIndex = 1;
            this.pidValueBox.Text = "0";
            // 
            // Ui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(93, 61);
            this.Controls.Add(this.pidValueBox);
            this.Controls.Add(this.StartSwitch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Ui";
            this.ShowIcon = false;
            this.Text = " Offline";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal MetroFramework.Controls.MetroToggle StartSwitch;
        internal System.Windows.Forms.Label pidValueBox;
    }
}

