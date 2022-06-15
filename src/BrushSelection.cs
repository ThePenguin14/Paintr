using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paintr
{
    public partial class BrushSelection : UserControl
    {
        public BrushSelection()
        {
            InitializeComponent();
            SetTexts();
            BackColor = Styling.BackgroundColor;
            Styling.FormatButton(currentButton);
            currentButton.BackColor = Styling.ContrastBackgroundDownColor;
            currentButton.FlatAppearance.MouseOverBackColor = Styling.ContrastBackgroundDownColor;
            currentButton.FlatAppearance.MouseDownBackColor = Styling.ContrastBackgroundDownColor;
            currentButton.FlatAppearance.BorderColor = Styling.ContrastBackgroundColor;
            Styling.FormatButton(rectSelectButton);
            Styling.FormatButton(selectOtherButton);
            Styling.FormatButton(paintBrushButton);
            Styling.FormatButton(pixelPerfectButton);
            Styling.FormatButton(paintBucketButton);
            Styling.FormatButton(textButton);
            Styling.FormatButton(colorPickerButton);
            Editor.BrushChanged += Editor_BrushChanged;
            Button b = GetButtonOf(Editor.Brush.Id);
            if (b is not null) b.FlatAppearance.BorderColor = Styling.ContrastBackgroundDownColor;
            SizeChanged += BrushSelection_SizeChanged;
            currentButton.Text = Editor.Brush.FriendlyName;
        }
        public void SetTexts()
        {
            rectSelectButton.Text = Properties.Strings.BrushRectSelect;
            pixelPerfectButton.Text = Properties.Strings.BrushPixelPerfect;
            paintBrushButton.Text = Properties.Strings.BrushPaintBrush;
            paintBucketButton.Text = Properties.Strings.BrushFill;
        }
        private void BrushSelection_SizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
        }

        public void AdjustSize()
        {
            Size sz = ClientSize;
            currentButton.Width = sz.Width - 6;
            rectSelectButton.Width = (sz.Width - 12) / 3;
            selectOtherButton.Width = (sz.Width - 12) / 3;
            selectOtherButton.Left = rectSelectButton.Right + 3;
            paintBrushButton.Width = (sz.Width - 12) / 3;
            pixelPerfectButton.Left = paintBrushButton.Right + 3;
            pixelPerfectButton.Width = (sz.Width - 12) / 3;
            textButton.Left = pixelPerfectButton.Right + 3;
            textButton.Width = (sz.Width - 12) / 3;
            paintBucketButton.Width = (sz.Width - 12) / 3;
            colorPickerButton.Left = paintBucketButton.Right + 3;
            colorPickerButton.Width = (sz.Width - 12) / 3;
        }
        public Button GetButtonOf(string t)
        {
            switch (t)
            {
                case BrushRack.PaintBrushId:
                    return paintBrushButton;
                case BrushRack.PixelPerfectId:
                    return pixelPerfectButton;
                case BrushRack.RectSelectId:
                    return rectSelectButton;
                case BrushRack.PaintBucketId:
                    return paintBucketButton;
                case BrushRack.ColorPickerId:
                    return colorPickerButton;
                case BrushRack.TextBrushId:
                    return textButton;
                default:
                    return null;
            }
        }
        private void Editor_BrushChanged(object sender, PropertyChangedEventArgs<DrawBrush> e)
        {
            Button b = GetButtonOf(e.OldValue.Id);
            if (b is not null) b.FlatAppearance.BorderColor = Styling.ButtonBorderColor;
            b = GetButtonOf(Editor.Brush.Id);
            if (b is not null) b.FlatAppearance.BorderColor = Styling.ContrastBackgroundDownColor;
            currentButton.Text = Editor.Brush.FriendlyName;
        }

        private void PixelPerfectButtonClick(object sender, EventArgs e)
        {
            ActiveControl = null;
            Editor.Brush = BrushRack.Brushes[BrushRack.PixelPerfectId];
        }

        private void PaintBrushButtonClick(object sender, EventArgs e)
        {
            ActiveControl = null;
            Editor.Brush = BrushRack.Brushes[BrushRack.PaintBrushId];
        }

        private void CurrentBrushButtonClick(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void SelectOtherButtonClicked(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void RectSelectButtonClicked(object sender, EventArgs e)
        {
            ActiveControl = null;
            Editor.Brush = BrushRack.Brushes[BrushRack.RectSelectId];
        }
        protected override bool ProcessTabKey(bool forward)
        {
            return false;
        }

        private void PaintBucketButtonClick(object sender, EventArgs e)
        {
            ActiveControl = null;
            Editor.Brush = BrushRack.Brushes[BrushRack.PaintBucketId];
        }

        private void ColorPickerButtonClick(object sender, EventArgs e)
        {
            ActiveControl = null;
            Editor.Brush = BrushRack.Brushes[BrushRack.ColorPickerId];
        }

        private void TextBrushButtonClicked(object sender, EventArgs e)
        {
            ActiveControl = null;
            Editor.Brush = BrushRack.Brushes[BrushRack.TextBrushId];
        }
    }
}
