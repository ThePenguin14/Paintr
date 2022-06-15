namespace Paintr
{
    partial class TextEdit
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
            this.xButton = new System.Windows.Forms.Button();
            this.underlineButton = new System.Windows.Forms.Button();
            this.italicButton = new System.Windows.Forms.Button();
            this.boldButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // xButton
            // 
            this.xButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.xButton.Font = new System.Drawing.Font("Segoe MDL2 Assets", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.xButton.Location = new System.Drawing.Point(450, 0);
            this.xButton.Name = "xButton";
            this.xButton.Size = new System.Drawing.Size(50, 50);
            this.xButton.TabIndex = 0;
            this.xButton.Text = "X";
            this.xButton.UseVisualStyleBackColor = true;
            this.xButton.Click += new System.EventHandler(this.XButtonClicked);
            // 
            // underlineButton
            // 
            this.underlineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.underlineButton.Font = new System.Drawing.Font("Sitka Small", 10F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.underlineButton.Location = new System.Drawing.Point(400, 0);
            this.underlineButton.Name = "underlineButton";
            this.underlineButton.Size = new System.Drawing.Size(50, 50);
            this.underlineButton.TabIndex = 1;
            this.underlineButton.Text = "U";
            this.underlineButton.UseVisualStyleBackColor = true;
            this.underlineButton.Click += new System.EventHandler(this.UnderlineClicked);
            // 
            // italicButton
            // 
            this.italicButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.italicButton.Font = new System.Drawing.Font("Sitka Small", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.italicButton.Location = new System.Drawing.Point(350, 0);
            this.italicButton.Name = "italicButton";
            this.italicButton.Size = new System.Drawing.Size(50, 50);
            this.italicButton.TabIndex = 2;
            this.italicButton.Text = "I";
            this.italicButton.UseVisualStyleBackColor = true;
            this.italicButton.Click += new System.EventHandler(this.ItalicClicked);
            // 
            // boldButton
            // 
            this.boldButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.boldButton.Font = new System.Drawing.Font("Sitka Small", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.boldButton.Location = new System.Drawing.Point(300, 0);
            this.boldButton.Name = "boldButton";
            this.boldButton.Size = new System.Drawing.Size(50, 50);
            this.boldButton.TabIndex = 3;
            this.boldButton.Text = "B";
            this.boldButton.UseVisualStyleBackColor = true;
            this.boldButton.Click += new System.EventHandler(this.BoldClicked);
            // 
            // TextEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.boldButton);
            this.Controls.Add(this.italicButton);
            this.Controls.Add(this.underlineButton);
            this.Controls.Add(this.xButton);
            this.Name = "TextEdit";
            this.Size = new System.Drawing.Size(500, 50);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button xButton;
        private System.Windows.Forms.Button underlineButton;
        private System.Windows.Forms.Button italicButton;
        private System.Windows.Forms.Button boldButton;
    }
}
