using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paintr.Dialog
{
    public partial class ResizeDialog : Form
    {
        private ThemedTextBox widthBox;
        private ThemedTextBox heightBox;
        private SelectInput resizeAlgorithm;
        public ResizeDialog()
        {
            InitializeComponent();
            widthBox = ThemedTextBox.IntBox();
            widthBox.TabIndex = 1;
            widthBox.Size = new Size(613, 17);
            widthBox.Location = new Point(150, 69);
            widthBox.SetText(Editor.Viewport.ImageSize.Width.ToString());
            Controls.Add(widthBox);
            heightBox = ThemedTextBox.IntBox();
            heightBox.TabIndex = 2;
            heightBox.Size = new Size(613, 17);
            heightBox.Location = new Point(150, 99);
            heightBox.SetText(Editor.Viewport.ImageSize.Height.ToString());
            Controls.Add(heightBox);
            resizeAlgorithm = new SelectInput()
            {
                Location = new Point(150, 26),
                Size = new Size(613, 23),
                TabIndex = 0
            };
            resizeAlgorithm.Items.Add(new SelectInputItem(Properties.Strings.ScalingBilinear, System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear));
            resizeAlgorithm.Items.Add(new SelectInputItem(Properties.Strings.ScalingBicubic, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic));
            resizeAlgorithm.Items.Add(new SelectInputItem(Properties.Strings.ScalingNearestNeighbor, System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor));
            Controls.Add(resizeAlgorithm);
            resizeAlgorithm.BringToFront();
            Styling.FormatLabel(label1);
            Styling.FormatLabel(label2);
            Styling.FormatLabel(label3);
            Styling.FormatButton(cancel);
            cancel.Text = Properties.Strings.ButtonCancel;
            Styling.FormatButton(okay);
            okay.Text = Properties.Strings.ButtonOkay;
            BackColor = Styling.BackgroundColor;
            Disposed += ResizeDialog_Disposed;
            Shown += ResizeDialog_Shown;
        }

        private void ResizeDialog_Shown(object sender, EventArgs e)
        {
            widthBox.Focus();
        }

        private void ResizeDialog_Disposed(object sender, EventArgs e)
        {
            if(widthBox is not null)
            {
                Controls.Remove(widthBox);
                widthBox.Dispose();
                widthBox = null;
            }
            if (heightBox is not null)
            {
                Controls.Remove(heightBox);
                heightBox.Dispose();
                heightBox = null;
            }
        }

        public int PixelsWidth { get => int.Parse(widthBox.SavedText); }
        public int PixelsHeight { get => int.Parse(heightBox.SavedText); }
    }
}
