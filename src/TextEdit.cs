using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paintr
{
    public partial class TextEdit : UserControl
    {
        private ThemedTextBox fontSizeBox;
        private SelectInput fontBox;
        public TextEdit()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Styling.BackgroundColor;
            InitializeComponent();
            Styling.FormatButton(xButton);
            xButton.FlatAppearance.BorderSize = 0;
            Styling.FormatButton(underlineButton);
            Styling.FormatButton(italicButton);
            Styling.FormatButton(boldButton);
            Underline = false;
            Italic = false;
            Bold = false;
            fontSizeBox = ThemedTextBox.FloatBox();
            fontSizeBox.Font = new Font("Segoe UI", 24f);
            fontSizeBox.SetText(FontSize.ToString());
            fontSizeBox.InputDone += FontSizeBox_InputDone;
            Controls.Add(fontSizeBox);
            fontBox = new SelectInput()
            {
                MaximumSize = new Size(int.MaxValue, 600)
            };
            InstalledFontCollection ifc = new();
            int arialIndex = 0;
            for(int i = 0; i < ifc.Families.Length; i++)
            {
                if (ifc.Families[i].Name is "Arial") arialIndex = i;
                fontBox.Items.Add(new SelectInputItem(ifc.Families[i].Name, ifc.Families[i].Name));
                ifc.Families[i].Dispose();
            }
            ifc.Dispose();
            fontBox.SelectedIndex = arialIndex;
            fontBox.OpenedChanged += FontBox_OpenedChanged;
            fontBox.SelectedIndexChanged += FontBox_SelectedIndexChanged;
            Controls.Add(fontBox);
            Disposed += TextEdit_Disposed;
            SizeChanged += TextEdit_SizeChanged;
            AdjustSize();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!fontBox.Opened) e.Graphics.Clear(BackColor);
        }
        private void FontBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OutputFontChanged is not null) OutputFontChanged(this, new EventArgs());
        }

        private void FontBox_OpenedChanged(object sender, EventArgs e)
        {
            if(fontBox.Opened)
            {
                oldHeight = Height;
                suppressAdjustSize = true;
                Height = fontBox.Height;
                suppressAdjustSize = false;
            }
            else
            {
                suppressAdjustSize = true;
                Height = oldHeight;
                suppressAdjustSize = false;
            }
        }
        private int oldHeight;
        private void TextEdit_SizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
        }
        private bool suppressAdjustSize;
        public void AdjustSize()
        {
            if (suppressAdjustSize) return;
            Size sz = ClientSize;
            xButton.Location = new Point(sz.Width - sz.Height, 0);
            xButton.Size = new Size(sz.Height, sz.Height);
            underlineButton.Location = new Point(sz.Width - sz.Height * 2, 0);
            underlineButton.Size = new Size(sz.Height, sz.Height);
            italicButton.Location = new Point(sz.Width - sz.Height * 3, 0);
            italicButton.Size = new Size(sz.Height, sz.Height);
            boldButton.Location = new Point(sz.Width - sz.Height * 4, 0);
            boldButton.Size = new Size(sz.Height, sz.Height);
            fontSizeBox.Width = sz.Height * 3 / 2;
            fontSizeBox.Location = new Point(sz.Width - sz.Height * 11 / 2, (sz.Height - fontSizeBox.Height) / 2);
            fontBox.Size = new Size(sz.Width - sz.Height * 11 / 2 - 1, sz.Height);
            fontBox.Location = Point.Empty;
            if(FindForm() is not null) fontBox.MaximumSize = new Size(int.MaxValue, FindForm().ClientSize.Height - Bottom - 40);
        }
        private void TextEdit_Disposed(object sender, EventArgs e)
        {
            if(fontSizeBox is not null)
            {
                Controls.Remove(fontSizeBox);
                fontSizeBox.Dispose();
                fontSizeBox = null;
            }
        }

        private void FontSizeBox_InputDone(object sender, EventArgs e)
        {
            FontSize = float.Parse(fontSizeBox.SavedText);
        }
        private float fontSize = 11f;
        public float FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                fontSizeBox.SetText(fontSize.ToString());
                if (OutputFontChanged is not null) OutputFontChanged(this, new EventArgs());
            }
        }
        private bool underline;
        public bool Underline
        {
            get => underline;
            set
            {
                underline = value;
                underlineButton.BackColor = Underline ? Styling.ContrastBackgroundColor : Styling.ButtonBackgroundColor;
                if (OutputFontChanged is not null) OutputFontChanged(this, new EventArgs());
            }
        }
        private bool italic;
        public bool Italic
        {
            get => italic;
            set
            {
                italic = value;
                italicButton.BackColor = Italic ? Styling.ContrastBackgroundColor : Styling.ButtonBackgroundColor;
                if (OutputFontChanged is not null) OutputFontChanged(this, new EventArgs());
            }
        }
        private bool bold;
        public bool Bold
        {
            get => bold;
            set
            {
                bold = value;
                boldButton.BackColor = Bold ? Styling.ContrastBackgroundColor : Styling.ButtonBackgroundColor;
                if (OutputFontChanged is not null) OutputFontChanged(this, new EventArgs());
            }
        }
        public string FontFamilyName
        {
            get => (string)fontBox.SelectedItem;
            set
            {
                SelectInputItem item = fontBox.Items.FirstOrDefault((SelectInputItem it) => (string)it.Item == value);
                if (item is not null) fontBox.SelectedIndex = fontBox.Items.FirstIndex((SelectInputItem it) => it == item);
            }
        }
        public event EventHandler OutputFontChanged;
        public event EventHandler XClicked;
        private void BoldClicked(object sender, EventArgs e)
        {
            Bold = !Bold;
        }

        private void ItalicClicked(object sender, EventArgs e)
        {
            Italic = !Italic;
        }

        private void UnderlineClicked(object sender, EventArgs e)
        {
            Underline = !Underline;
        }

        private void XButtonClicked(object sender, EventArgs e)
        {
            if (XClicked is not null) XClicked(this, new EventArgs());
        }
    }
}
