
namespace Paintr
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.fileDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.openImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.resizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.negateSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.greyscaleSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.rulerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.protractorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pixelGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.resetViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.selectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.viewportStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowMerge = false;
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileDropDown,
            this.editDropDown,
            this.viewDropDown});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // fileDropDown
            // 
            this.fileDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openImageMenuItem,
            this.saveImageToolStripMenuItem,
            this.toolStripSeparator2,
            this.resizeToolStripMenuItem});
            this.fileDropDown.Image = ((System.Drawing.Image)(resources.GetObject("fileDropDown.Image")));
            this.fileDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileDropDown.Name = "fileDropDown";
            this.fileDropDown.ShowDropDownArrow = false;
            this.fileDropDown.Size = new System.Drawing.Size(29, 22);
            this.fileDropDown.Text = "File";
            // 
            // openImageMenuItem
            // 
            this.openImageMenuItem.Name = "openImageMenuItem";
            this.openImageMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openImageMenuItem.Size = new System.Drawing.Size(191, 22);
            this.openImageMenuItem.Text = "Open Image...";
            this.openImageMenuItem.Click += new System.EventHandler(this.OpenImageClick);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveImageToolStripMenuItem.Text = "Save Image...";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.SaveImageClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // resizeToolStripMenuItem
            // 
            this.resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
            this.resizeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resizeToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.resizeToolStripMenuItem.Text = "Resize...";
            this.resizeToolStripMenuItem.Click += new System.EventHandler(this.ResizeMenuClicked);
            // 
            // editDropDown
            // 
            this.editDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllMenuItem,
            this.toolStripSeparator1,
            this.negateSelectionToolStripMenuItem,
            this.greyscaleSelectionToolStripMenuItem,
            this.toolStripSeparator4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.editDropDown.Image = ((System.Drawing.Image)(resources.GetObject("editDropDown.Image")));
            this.editDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editDropDown.Name = "editDropDown";
            this.editDropDown.ShowDropDownArrow = false;
            this.editDropDown.Size = new System.Drawing.Size(31, 22);
            this.editDropDown.Text = "Edit";
            // 
            // selectAllMenuItem
            // 
            this.selectAllMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("selectAllMenuItem.Image")));
            this.selectAllMenuItem.Name = "selectAllMenuItem";
            this.selectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllMenuItem.Size = new System.Drawing.Size(240, 22);
            this.selectAllMenuItem.Text = "Select All";
            this.selectAllMenuItem.Click += new System.EventHandler(this.SelectAllEditClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(237, 6);
            // 
            // negateSelectionToolStripMenuItem
            // 
            this.negateSelectionToolStripMenuItem.Name = "negateSelectionToolStripMenuItem";
            this.negateSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.N)));
            this.negateSelectionToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.negateSelectionToolStripMenuItem.Text = "Negate Selection";
            this.negateSelectionToolStripMenuItem.Click += new System.EventHandler(this.NegateSelectionEditClicked);
            // 
            // greyscaleSelectionToolStripMenuItem
            // 
            this.greyscaleSelectionToolStripMenuItem.Name = "greyscaleSelectionToolStripMenuItem";
            this.greyscaleSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.G)));
            this.greyscaleSelectionToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.greyscaleSelectionToolStripMenuItem.Text = "Greyscale Selection";
            this.greyscaleSelectionToolStripMenuItem.Click += new System.EventHandler(this.GrayscaleSelectionEditClicked);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(237, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutMenuClicked);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyMenuClicked);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteMenuClicked);
            // 
            // viewDropDown
            // 
            this.viewDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rulerToolStripMenuItem,
            this.protractorToolStripMenuItem,
            this.pixelGridToolStripMenuItem,
            this.tileViewToolStripMenuItem,
            this.toolStripSeparator3,
            this.resetViewToolStripMenuItem});
            this.viewDropDown.Image = ((System.Drawing.Image)(resources.GetObject("viewDropDown.Image")));
            this.viewDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewDropDown.Name = "viewDropDown";
            this.viewDropDown.ShowDropDownArrow = false;
            this.viewDropDown.Size = new System.Drawing.Size(36, 22);
            this.viewDropDown.Text = "View";
            // 
            // rulerToolStripMenuItem
            // 
            this.rulerToolStripMenuItem.CheckOnClick = true;
            this.rulerToolStripMenuItem.Name = "rulerToolStripMenuItem";
            this.rulerToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.rulerToolStripMenuItem.Text = "Ruler";
            this.rulerToolStripMenuItem.CheckedChanged += new System.EventHandler(this.RulerChecked);
            // 
            // protractorToolStripMenuItem
            // 
            this.protractorToolStripMenuItem.CheckOnClick = true;
            this.protractorToolStripMenuItem.Name = "protractorToolStripMenuItem";
            this.protractorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.protractorToolStripMenuItem.Text = "&Protractor";
            this.protractorToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ProtractorChecked);
            // 
            // pixelGridToolStripMenuItem
            // 
            this.pixelGridToolStripMenuItem.CheckOnClick = true;
            this.pixelGridToolStripMenuItem.Name = "pixelGridToolStripMenuItem";
            this.pixelGridToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.pixelGridToolStripMenuItem.Text = "Pixel Grid";
            this.pixelGridToolStripMenuItem.Click += new System.EventHandler(this.PixelGridMenuClicked);
            // 
            // tileViewToolStripMenuItem
            // 
            this.tileViewToolStripMenuItem.CheckOnClick = true;
            this.tileViewToolStripMenuItem.Name = "tileViewToolStripMenuItem";
            this.tileViewToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.tileViewToolStripMenuItem.Text = "Tile View";
            this.tileViewToolStripMenuItem.Click += new System.EventHandler(this.TileViewMenuClicked);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(167, 6);
            // 
            // resetViewToolStripMenuItem
            // 
            this.resetViewToolStripMenuItem.Name = "resetViewToolStripMenuItem";
            this.resetViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.resetViewToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.resetViewToolStripMenuItem.Text = "Reset View";
            this.resetViewToolStripMenuItem.Click += new System.EventHandler(this.ResetViewMenuClicked);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectionStatusLabel,
            this.viewportStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // selectionStatusLabel
            // 
            this.selectionStatusLabel.Name = "selectionStatusLabel";
            this.selectionStatusLabel.Size = new System.Drawing.Size(392, 17);
            this.selectionStatusLabel.Spring = true;
            this.selectionStatusLabel.Text = "SelPos: 0, 0; SelSize: 0, 0";
            // 
            // viewportStatusLabel
            // 
            this.viewportStatusLabel.Name = "viewportStatusLabel";
            this.viewportStatusLabel.Size = new System.Drawing.Size(392, 17);
            this.viewportStatusLabel.Spring = true;
            this.viewportStatusLabel.Text = "Zoom: 100%; Position: +0, +0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Paintr";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton fileDropDown;
        private System.Windows.Forms.ToolStripMenuItem openImageMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton editDropDown;
        private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel selectionStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem negateSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem greyscaleSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem resizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton viewDropDown;
        private System.Windows.Forms.ToolStripMenuItem rulerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem protractorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pixelGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel viewportStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem resetViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileViewToolStripMenuItem;
    }
}

