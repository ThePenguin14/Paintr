
namespace Paintr
{
    partial class ColorPicker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hexLabel = new System.Windows.Forms.Label();
            this.rgbLabel = new System.Windows.Forms.Label();
            this.hslLabel = new System.Windows.Forms.Label();
            this.hsvLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // hexLabel
            // 
            this.hexLabel.AutoSize = true;
            this.hexLabel.Location = new System.Drawing.Point(10, 265);
            this.hexLabel.Name = "hexLabel";
            this.hexLabel.Size = new System.Drawing.Size(28, 15);
            this.hexLabel.TabIndex = 0;
            this.hexLabel.Text = "Hex";
            this.hexLabel.Click += new System.EventHandler(this.HexLabelClick);
            // 
            // rgbLabel
            // 
            this.rgbLabel.AutoSize = true;
            this.rgbLabel.Location = new System.Drawing.Point(10, 290);
            this.rgbLabel.Name = "rgbLabel";
            this.rgbLabel.Size = new System.Drawing.Size(29, 15);
            this.rgbLabel.TabIndex = 1;
            this.rgbLabel.Text = "RGB";
            this.rgbLabel.Click += new System.EventHandler(this.RgbLabelClick);
            // 
            // hslLabel
            // 
            this.hslLabel.AutoSize = true;
            this.hslLabel.Location = new System.Drawing.Point(10, 340);
            this.hslLabel.Name = "hslLabel";
            this.hslLabel.Size = new System.Drawing.Size(28, 15);
            this.hslLabel.TabIndex = 2;
            this.hslLabel.Text = "HSL";
            this.hslLabel.Click += new System.EventHandler(this.HslLabelClick);
            // 
            // hsvLabel
            // 
            this.hsvLabel.AutoSize = true;
            this.hsvLabel.Location = new System.Drawing.Point(10, 315);
            this.hsvLabel.Name = "hsvLabel";
            this.hsvLabel.Size = new System.Drawing.Size(29, 15);
            this.hsvLabel.TabIndex = 3;
            this.hsvLabel.Text = "HSV";
            this.hsvLabel.Click += new System.EventHandler(this.HsvLabelClick);
            // 
            // ColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hsvLabel);
            this.Controls.Add(this.hslLabel);
            this.Controls.Add(this.rgbLabel);
            this.Controls.Add(this.hexLabel);
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(300, 455);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label hexLabel;
        private System.Windows.Forms.Label rgbLabel;
        private System.Windows.Forms.Label hslLabel;
        private System.Windows.Forms.Label hsvLabel;
    }
}
