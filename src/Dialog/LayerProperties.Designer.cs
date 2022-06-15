namespace Paintr.Dialog
{
    partial class LayerProperties
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
            this.opacityLabel = new System.Windows.Forms.Label();
            this.okay = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.blendModeLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.shadersLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // opacityLabel
            // 
            this.opacityLabel.AutoSize = true;
            this.opacityLabel.Location = new System.Drawing.Point(12, 46);
            this.opacityLabel.Name = "opacityLabel";
            this.opacityLabel.Size = new System.Drawing.Size(48, 15);
            this.opacityLabel.TabIndex = 0;
            this.opacityLabel.Text = "Opacity";
            this.opacityLabel.Click += new System.EventHandler(this.OpacityLabelClicked);
            // 
            // okay
            // 
            this.okay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okay.Location = new System.Drawing.Point(403, 397);
            this.okay.Name = "okay";
            this.okay.Size = new System.Drawing.Size(385, 41);
            this.okay.TabIndex = 6;
            this.okay.Text = "Okay";
            this.okay.UseVisualStyleBackColor = true;
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel.Location = new System.Drawing.Point(12, 397);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(385, 41);
            this.cancel.TabIndex = 5;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // blendModeLabel
            // 
            this.blendModeLabel.AutoSize = true;
            this.blendModeLabel.Location = new System.Drawing.Point(12, 83);
            this.blendModeLabel.Name = "blendModeLabel";
            this.blendModeLabel.Size = new System.Drawing.Size(71, 15);
            this.blendModeLabel.TabIndex = 7;
            this.blendModeLabel.Text = "Blend Mode";
            this.blendModeLabel.Click += new System.EventHandler(this.BlendModeLabelClicked);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 9);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 15);
            this.nameLabel.TabIndex = 8;
            this.nameLabel.Text = "Name";
            this.nameLabel.Click += new System.EventHandler(this.NameLabelClicked);
            // 
            // shadersLabel
            // 
            this.shadersLabel.AutoSize = true;
            this.shadersLabel.Location = new System.Drawing.Point(12, 120);
            this.shadersLabel.Name = "shadersLabel";
            this.shadersLabel.Size = new System.Drawing.Size(48, 15);
            this.shadersLabel.TabIndex = 9;
            this.shadersLabel.Text = "Shaders";
            this.shadersLabel.Click += new System.EventHandler(this.ShadersLabelClicked);
            // 
            // LayerProperties
            // 
            this.AcceptButton = this.okay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.shadersLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.blendModeLabel);
            this.Controls.Add(this.okay);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.opacityLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LayerProperties";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LayerProperties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label opacityLabel;
        private System.Windows.Forms.Button okay;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label blendModeLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label shadersLabel;
    }
}