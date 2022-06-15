namespace Paintr
{
    partial class BrushSelection
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
            this.currentButton = new System.Windows.Forms.Button();
            this.paintBrushButton = new System.Windows.Forms.Button();
            this.pixelPerfectButton = new System.Windows.Forms.Button();
            this.rectSelectButton = new System.Windows.Forms.Button();
            this.selectOtherButton = new System.Windows.Forms.Button();
            this.paintBucketButton = new System.Windows.Forms.Button();
            this.colorPickerButton = new System.Windows.Forms.Button();
            this.textButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // currentButton
            // 
            this.currentButton.Location = new System.Drawing.Point(3, 3);
            this.currentButton.Name = "currentButton";
            this.currentButton.Size = new System.Drawing.Size(294, 45);
            this.currentButton.TabIndex = 0;
            this.currentButton.Text = "Paint Brush";
            this.currentButton.UseVisualStyleBackColor = true;
            this.currentButton.Click += new System.EventHandler(this.CurrentBrushButtonClick);
            // 
            // paintBrushButton
            // 
            this.paintBrushButton.Location = new System.Drawing.Point(3, 105);
            this.paintBrushButton.Name = "paintBrushButton";
            this.paintBrushButton.Size = new System.Drawing.Size(96, 45);
            this.paintBrushButton.TabIndex = 1;
            this.paintBrushButton.Text = "Paint Brush";
            this.paintBrushButton.UseVisualStyleBackColor = true;
            this.paintBrushButton.Click += new System.EventHandler(this.PaintBrushButtonClick);
            // 
            // pixelPerfectButton
            // 
            this.pixelPerfectButton.Location = new System.Drawing.Point(102, 105);
            this.pixelPerfectButton.Name = "pixelPerfectButton";
            this.pixelPerfectButton.Size = new System.Drawing.Size(96, 45);
            this.pixelPerfectButton.TabIndex = 2;
            this.pixelPerfectButton.Text = "Pixel Perfect";
            this.pixelPerfectButton.UseVisualStyleBackColor = true;
            this.pixelPerfectButton.Click += new System.EventHandler(this.PixelPerfectButtonClick);
            // 
            // rectSelectButton
            // 
            this.rectSelectButton.Location = new System.Drawing.Point(3, 54);
            this.rectSelectButton.Name = "rectSelectButton";
            this.rectSelectButton.Size = new System.Drawing.Size(96, 45);
            this.rectSelectButton.TabIndex = 3;
            this.rectSelectButton.Text = "Rect Select";
            this.rectSelectButton.UseVisualStyleBackColor = true;
            this.rectSelectButton.Click += new System.EventHandler(this.RectSelectButtonClicked);
            // 
            // selectOtherButton
            // 
            this.selectOtherButton.Location = new System.Drawing.Point(102, 54);
            this.selectOtherButton.Name = "selectOtherButton";
            this.selectOtherButton.Size = new System.Drawing.Size(96, 45);
            this.selectOtherButton.TabIndex = 4;
            this.selectOtherButton.Text = "Select...";
            this.selectOtherButton.UseVisualStyleBackColor = true;
            this.selectOtherButton.Click += new System.EventHandler(this.SelectOtherButtonClicked);
            // 
            // paintBucketButton
            // 
            this.paintBucketButton.Location = new System.Drawing.Point(3, 156);
            this.paintBucketButton.Name = "paintBucketButton";
            this.paintBucketButton.Size = new System.Drawing.Size(96, 45);
            this.paintBucketButton.TabIndex = 5;
            this.paintBucketButton.Text = "Paint Bucket";
            this.paintBucketButton.UseVisualStyleBackColor = true;
            this.paintBucketButton.Click += new System.EventHandler(this.PaintBucketButtonClick);
            // 
            // colorPickerButton
            // 
            this.colorPickerButton.Location = new System.Drawing.Point(102, 156);
            this.colorPickerButton.Name = "colorPickerButton";
            this.colorPickerButton.Size = new System.Drawing.Size(96, 45);
            this.colorPickerButton.TabIndex = 6;
            this.colorPickerButton.Text = "Color Picker";
            this.colorPickerButton.UseVisualStyleBackColor = true;
            this.colorPickerButton.Click += new System.EventHandler(this.ColorPickerButtonClick);
            // 
            // textButton
            // 
            this.textButton.Location = new System.Drawing.Point(201, 105);
            this.textButton.Name = "textButton";
            this.textButton.Size = new System.Drawing.Size(96, 45);
            this.textButton.TabIndex = 7;
            this.textButton.Text = "Text";
            this.textButton.UseVisualStyleBackColor = true;
            this.textButton.Click += new System.EventHandler(this.TextBrushButtonClicked);
            // 
            // BrushSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textButton);
            this.Controls.Add(this.colorPickerButton);
            this.Controls.Add(this.paintBucketButton);
            this.Controls.Add(this.selectOtherButton);
            this.Controls.Add(this.rectSelectButton);
            this.Controls.Add(this.pixelPerfectButton);
            this.Controls.Add(this.paintBrushButton);
            this.Controls.Add(this.currentButton);
            this.Name = "BrushSelection";
            this.Size = new System.Drawing.Size(300, 450);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button currentButton;
        private System.Windows.Forms.Button paintBrushButton;
        private System.Windows.Forms.Button pixelPerfectButton;
        private System.Windows.Forms.Button rectSelectButton;
        private System.Windows.Forms.Button selectOtherButton;
        private System.Windows.Forms.Button paintBucketButton;
        private System.Windows.Forms.Button colorPickerButton;
        private System.Windows.Forms.Button textButton;
    }
}
