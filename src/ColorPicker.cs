using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Drawing2D;

namespace Paintr
{
    public partial class ColorPicker : UserControl
    {
        private ThemedTextBox hexBox;
        private ThemedTextBox rgbBox;
        private ThemedTextBox hsvBox;
        private ThemedTextBox hslBox;

        private ColorBlend satGrad;
        private ColorBlend valGrad;
        private ColorBlend hueGrad;
        private LinearGradientBrush satBrush;
        private LinearGradientBrush valBrush;
        private LinearGradientBrush hueBrush;
        public ColorPicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            InitializeComponent();
            BackColor = Styling.BackgroundColor;
            Styling.FormatLabel(hexLabel);
            Styling.FormatLabel(rgbLabel);
            Styling.FormatLabel(hsvLabel);
            Styling.FormatLabel(hslLabel);
            hexBox = new ThemedTextBox(ThemedTextBox.HexFilter)
            {
                ValidateInput = ValidateHex,
                CharacterCasing = CharacterCasing.Upper,
                Location = new Point(42, 263),
                Size = new Size(MainColorArea.Right - 42, 13)
            };
            hexBox.CharWhitelist.Add('#');
            hexBox.InputDone += HexBox_InputDone;
            Controls.Add(hexBox);
            rgbBox = ThemedTextBox.IntTripletBox();
            rgbBox.Location = new Point(42, 287);
            rgbBox.Size = new Size(MainColorArea.Right - 42, 13);
            rgbBox.ValidateInput = ValidateRgb;
            rgbBox.InputDone += RgbBox_InputDone;
            hsvBox = ThemedTextBox.DoubleTripletBox();
            hsvBox.Location = new Point(42, 312);
            hsvBox.Size = new Size(MainColorArea.Right - 42, 13);
            hsvBox.ValidateInput = ValidateHsl;
            hsvBox.CharWhitelist.Add('%');
            hsvBox.InputDone += HsvBox_InputDone;
            hslBox = ThemedTextBox.DoubleTripletBox();
            hslBox.Location = new Point(42, 337);
            hslBox.Size = new Size(MainColorArea.Right - 42, 13);
            hslBox.ValidateInput = ValidateHsl;
            hslBox.CharWhitelist.Add('%');
            hslBox.InputDone += HslBox_InputDone;
            Controls.Add(rgbBox);
            Controls.Add(hsvBox);
            Controls.Add(hslBox);

            hueGrad = new ColorBlend(4);
            hueGrad.Colors = new Color[] { Color.Red, Color.Yellow, Color.Lime, Color.Cyan, Color.Blue, Color.Magenta, Color.Red };
            hueGrad.Positions = new float[] { 0f, 1f / 6f, 1f / 3f, 0.5f, 2f / 3f, 5f / 6f, 1f };
            hueBrush = new LinearGradientBrush(HueArea, Color.Black, Color.Black, LinearGradientMode.Vertical);
            hueBrush.InterpolationColors = hueGrad;

            valGrad = new ColorBlend(2);
            satGrad = new ColorBlend(2);
            GetSatValBlend(Color.GetDoubleHue(), ref satGrad, ref valGrad);
            valBrush = new LinearGradientBrush(MainColorArea, Color.Black, Color.Black, LinearGradientMode.Horizontal) { InterpolationColors = valGrad };
            satBrush = new LinearGradientBrush(MainColorArea, Color.Black, Color.Black, LinearGradientMode.Vertical) { InterpolationColors = satGrad };

            Disposed += ColorPicker_Disposed;
            Paint += ColorPicker_Paint;
            Click += ColorPicker_Click;
            MouseMove += ColorPicker_MouseMove;
            MouseDown += ColorPicker_MouseDown;

            Color = Color.Red;

            Load += ColorPicker_Load;
            SizeChanged += ColorPicker_SizeChanged;
            AdjustSize();
        }
        public void SwapColors()
        {
            Color col = color;
            Color = altColor;
            AlternateColor = col;
        }
        private bool hasTwoColors;
        public bool HasTwoColors
        {
            get => hasTwoColors;
            set
            {
                hasTwoColors = value;
            }
        }
        private void ColorPicker_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left || (e.Button is MouseButtons.Right && HasTwoColors))
            {
                Point pt = PointToClient(Cursor.Position);
                if (HueArea.Contains(pt))
                {
                    downIn = 1;
                    ActiveControl = hexLabel;
                }
                else if (MainColorArea.Contains(pt))
                {
                    downIn = 2;
                    ActiveControl = hexLabel;
                }
                else if (AlphaSliderArea.Contains(pt))
                {
                    downIn = 3;
                    ActiveControl = hexLabel;
                }
                else downIn = 0;
            }
        }
        private byte downIn = 0;
        private void ColorPicker_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left)
            {
                Point pt = PointToClient(Cursor.Position);
                if(downIn is 1)
                {
                    if (HueArea.Contains(pt)) SetHue(((double)pt.Y - HueArea.Y) * 360d / HueArea.Height);
                    else if (BroadenedHueArea.Contains(pt) || MainColorArea.Contains(pt))
                        SetHue((((double)pt.Y - HueArea.Y) * 360d / HueArea.Height).GetInRange(0d, 360d - double.Epsilon));
                }
                else if(downIn is 2)
                {
                    if (MainColorArea.Contains(pt))
                        SetColorFromPosition(((double)pt.X - MainColorArea.X) / MainColorArea.Width, ((double)pt.Y - MainColorArea.Y) / MainColorArea.Height);
                    else if (MainColorArea.Padded(51).Contains(pt))
                        SetColorFromPosition(((double)pt.X - MainColorArea.X).GetInRange(0, MainColorArea.Width) / MainColorArea.Width,
                         ((double)pt.Y - MainColorArea.Y).GetInRange(0, MainColorArea.Height) / MainColorArea.Height);
                }
                else if (downIn is 3)
                {
                    if (AlphaSliderArea.Contains(pt)) SetAlpha((byte)(((double)pt.X - AlphaSliderArea.Left) / AlphaSliderArea.Width * 255));
                    else SetAlpha((byte)(((double)pt.X - AlphaSliderArea.Left) / AlphaSliderArea.Width * 255).GetInRange(0d, 255d));
                }
            }
            else if(e.Button is MouseButtons.Right && HasTwoColors)
            {
                Point pt = PointToClient(Cursor.Position);
                if (downIn is 1)
                {
                    if (HueArea.Contains(pt)) SetAltHue(((double)pt.Y - HueArea.Y) * 360d / HueArea.Height);
                    else if (BroadenedHueArea.Contains(pt) || MainColorArea.Contains(pt))
                        SetAltHue((((double)pt.Y - HueArea.Y) * 360d / HueArea.Height).GetInRange(0d, 360d - double.Epsilon));
                }
                else if (downIn is 2)
                {
                    if (MainColorArea.Contains(pt))
                        SetAltColorFromPosition(((double)pt.X - MainColorArea.X) / MainColorArea.Width, ((double)pt.Y - MainColorArea.Y) / MainColorArea.Height);
                    else if (MainColorArea.Padded(51).Contains(pt))
                        SetAltColorFromPosition(((double)pt.X - MainColorArea.X).GetInRange(0, MainColorArea.Width) / MainColorArea.Width,
                         ((double)pt.Y - MainColorArea.Y).GetInRange(0, MainColorArea.Height) / MainColorArea.Height);
                }
                else if (downIn is 3)
                {
                    if (AlphaSliderArea.Contains(pt)) SetAlpha((byte)(((double)pt.X - AlphaSliderArea.Left) / AlphaSliderArea.Width * 255));
                    else SetAlpha((byte)(((double)pt.X - AlphaSliderArea.Left) / AlphaSliderArea.Width * 255).GetInRange(0d, 255d));
                }
            }
        }
        private void ColorPicker_Click(object sender, EventArgs e)
        {
            if (HasTwoColors && MouseButtons is MouseButtons.Right)
            {
                Point pt = PointToClient(Cursor.Position);
                if (downIn is 1 && HueArea.Contains(pt)) SetAltHue(((double)pt.Y - HueArea.Y) * 360d / HueArea.Height);
                else if (downIn is 2 && MainColorArea.Contains(pt)) SetAltColorFromPosition(((double)pt.X - MainColorArea.X) / MainColorArea.Width,
                     ((double)pt.Y - MainColorArea.Y) / MainColorArea.Height);
                else if (downIn is 3 && AlphaSliderArea.Contains(pt)) SetAlpha((byte)(((double)pt.X - AlphaSliderArea.Left) / AlphaSliderArea.Width * 255));
            }
            else
            {
                Point pt = PointToClient(Cursor.Position);
                if (downIn is 1 && HueArea.Contains(pt)) SetHue(((double)pt.Y - HueArea.Y) * 360d / HueArea.Height);
                else if (downIn is 2 && MainColorArea.Contains(pt)) SetColorFromPosition(((double)pt.X - MainColorArea.X) / MainColorArea.Width,
                     ((double)pt.Y - MainColorArea.Y) / MainColorArea.Height);
            }
        }

        private void ColorPicker_SizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
        }
        public void AdjustSize()
        {
            Size sz = ClientSize;
            hexLabel.Top = sz.Height - 95;
            hexBox.Top = sz.Height - 97;
            hexBox.Size = new Size(MainColorArea.Right - 42, 13);
            rgbLabel.Top = sz.Height - 70;
            rgbBox.Top = sz.Height - 72;
            rgbBox.Size = new Size(MainColorArea.Right - 42, 13);
            hsvLabel.Top = sz.Height - 45;
            hsvBox.Top = sz.Height - 47;
            hsvBox.Size = new Size(MainColorArea.Right - 42, 13);
            hslLabel.Top = sz.Height - 20;
            hslBox.Top = sz.Height - 22;
            hslBox.Size = new Size(MainColorArea.Right - 42, 13);
            LinearGradientBrush b = valBrush;
            valBrush = new LinearGradientBrush(MainColorArea, Color.Black, Color.Black, LinearGradientMode.Horizontal) { InterpolationColors = valGrad };
            if(b is not null) b.Dispose();
            b = satBrush;
            satBrush = new LinearGradientBrush(MainColorArea, Color.Black, Color.Black, LinearGradientMode.Vertical) { InterpolationColors = satGrad };
            if(b is not null) b.Dispose();
            b = hueBrush;
            hueBrush = new LinearGradientBrush(HueArea, Color.Black, Color.Black, LinearGradientMode.Vertical) { InterpolationColors = hueGrad };
            b.Dispose();
            Invalidate();
        }
        public static Color GetColorFromPosition(double hue, double x, double y)
        {
            return Extensions.ColorFromHSV(hue, x.GetInRange(0, 1), (1 - y).GetInRange(0, 1));
        }
        public static Color GetColorFromPosition(double alpha, double hue, double x, double y)
        {
            return Extensions.ColorFromHSV(alpha, hue, x.GetInRange(0, 1), (1 - y).GetInRange(0, 1));
        }
        public void SetColorFromPosition(double x, double y)
        {
            SetSatVal(x.GetInRange(0, 1), (1 - y).GetInRange(0, 1));
        }
        public void SetAltColorFromPosition(double x, double y)
        {
            SetAltSatVal(x.GetInRange(0, 1), (1 - y).GetInRange(0, 1));
        }
        public static (double, double) GetPositionFromColor(Color color)
        {
            color.TryToHSV(out _, out double s, out double v);
            return (s, 1 - v);
        }
        private double lastHue;
        private double lastSat;
        private double lastVal;
        private Rectangle MainColorArea { get => new Rectangle(10, 105, ClientSize.Width - 60, ClientSize.Width - 60); }
        private Rectangle HueArea { get => new Rectangle(ClientSize.Width - 40, 105, 30, ClientSize.Width - 60); }
        private Rectangle BroadenedHueArea { get => new Rectangle(ClientSize.Width - 50, 95, 50, ClientSize.Width - 40); }
        private Rectangle AlphaSliderArea { get => new Rectangle(MainColorArea.Left + (MainColorArea.Top - 35) * 2 + 5, 30,
            ClientSize.Width - MainColorArea.Left - (MainColorArea.Top - 35) * 2 - 35, MainColorArea.Top - 35); }
        private void ColorPicker_Paint(object sender, PaintEventArgs e)
        {
            if (hueBrush is not null) e.Graphics.FillRectangle(hueBrush, HueArea);
            if (valBrush is not null) e.Graphics.FillRectangle(valBrush, MainColorArea);
            if (satBrush is not null) e.Graphics.FillRectangle(satBrush, MainColorArea);
            SolidBrush b = new SolidBrush(Color);
            SolidBrush b2 = new SolidBrush(AlternateColor);
            (double x, double y) pos = GetPositionFromColor(Color);
            e.Graphics.FillRectangle(b, new Rectangle(MainColorArea.Left, 30, MainColorArea.Top - 35, MainColorArea.Top - 35));
            e.Graphics.FillRectangle(b2, new Rectangle(MainColorArea.Left + MainColorArea.Top - 35, 30, MainColorArea.Top - 35, MainColorArea.Top - 35));
            e.Graphics.FillEllipse(b, new Rectangle(MainColorArea.Left + (int)(pos.x * MainColorArea.Width) - 7, MainColorArea.Top + (int)(pos.y * MainColorArea.Height) - 7, 14, 14));
            e.Graphics.DrawEllipse(Color.GetBrightness() > 0.8f ? Pens.Black : Pens.White, new Rectangle(MainColorArea.Left + (int)(pos.x * MainColorArea.Width) - 7,
                MainColorArea.Top + (int)(pos.y * MainColorArea.Height) - 7, 14, 14));
            b.Color = Extensions.ColorFromHSV(lastHue, 1d, 1d);
            e.Graphics.FillRoundedRectangle(b, new Rectangle(MainColorArea.Right + 7, MainColorArea.Top + (int)(lastHue * MainColorArea.Height / 360d) - 2,
                ClientSize.Width - MainColorArea.Right - 14, 4), 1);
            e.Graphics.DrawRoundedRectangle(Pens.White, new Rectangle(MainColorArea.Right + 7, MainColorArea.Top + (int)(lastHue * MainColorArea.Height / 360d) - 2,
                ClientSize.Width - MainColorArea.Right - 14, 4), 1);
            Rectangle asa = AlphaSliderArea;
            using LinearGradientBrush transp = new LinearGradientBrush(asa, Color.Transparent, Color.White, LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(Brushes.Gray, asa);
            e.Graphics.FillRectangle(Viewport.Transparent, asa);
            e.Graphics.FillRectangle(transp, asa);
            e.Graphics.DrawRectangle(Pens.Black, asa);
            e.Graphics.FillRoundedRectangle(Brushes.White, new Rectangle(asa.Left + (int)(Color.A * asa.Width / 255d), asa.Top - 2, 4, asa.Height + 4), 1);
            e.Graphics.DrawRoundedRectangle(Pens.Black, new Rectangle(asa.Left + (int)(Color.A * asa.Width / 255d), asa.Top - 2, 4, asa.Height + 4), 1);
            b.Dispose();
            b2.Dispose();
        }

        private void HsvBox_InputDone(object sender, EventArgs e)
        {
            double[] arr = Extensions.StringToDoubleArray(hsvBox.SavedText.Replace("%", ""));
            lastHue = arr[0];
            Color = Extensions.ColorFromHSV(arr[0], arr[1] / 100d, arr[2] / 100d);
        }

        private void HslBox_InputDone(object sender, EventArgs e)
        {
            double[] arr = Extensions.StringToDoubleArray(hslBox.SavedText.Replace("%", ""));
            lastHue = arr[0];
            Color = Extensions.ColorFromHSL(arr[0], arr[1] / 100d, arr[2] / 100d);
        }

        private void ColorPicker_Load(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void HexBox_InputDone(object sender, EventArgs e)
        {
            string s = hexBox.SavedText;
            Color = s.Length is 7 ? Color.FromArgb(int.Parse(s[1..3], NumberStyles.HexNumber), int.Parse(s[3..5], NumberStyles.HexNumber), int.Parse(s[5..], NumberStyles.HexNumber)) :
                Color.FromArgb(int.Parse(s[..2], NumberStyles.HexNumber), int.Parse(s[2..4], NumberStyles.HexNumber), int.Parse(s[4..], NumberStyles.HexNumber));
        }

        private void RgbBox_InputDone(object sender, EventArgs e)
        {
            int[] arr = Extensions.StringToIntArray(rgbBox.SavedText);
            Color = Color.FromArgb(arr[0], arr[1], arr[2]);
        }

        private Color color = Color.FromArgb(0, 0, 0);
        public Color Color
        {
            get => color;
            set
            {
                SetColor(value);
            }
        }
        public event EventHandler ColorChanged;

        private Color altColor = Color.FromArgb(255, 255, 255);
        public Color AlternateColor
        {
            get => altColor;
            set
            {
                altColor = Color.FromArgb(value.A, value.R, value.G, value.B);
                Invalidate();
                if (AlternateColorChanged is not null) AlternateColorChanged(this, new EventArgs());
            }
        }
        public event EventHandler AlternateColorChanged;
        private void SetColor(Color value)
        {
            if (Color.ToArgb() != value.ToArgb())
            {
                Color oldCol = color;
                color = Color.FromArgb(value.A, value.R, value.G, value.B);
                if (color.GetDoubleValue() >= 0.001d && color.GetDoubleSaturation() >= 0.001d) lastHue = color.GetDoubleHue();
                lastSat = color.GetDoubleSaturation();
                lastVal = color.GetDoubleValue();
                hexBox.SetTextIfNeeded(HexBoxText(Color));
                rgbBox.SetTextIfNeeded(RgbBoxText(Color));
                if (color.TryToHSL(out double h, out double s, out double l))
                {
                    hslBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, s * 100d, l * 100d));
                    if (h != oldCol.GetDoubleHue()) GetSatValBlend(lastHue, ref satGrad, ref valGrad);
                }
                hsvBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, color.GetDoubleSaturation() * 100d, color.GetDoubleValue() * 100d));
            }
            Region rgn = new Region(Rectangle.Empty);
            rgn.Complement(new Rectangle(0, MainColorArea.Top - 11, ClientSize.Width, MainColorArea.Height + 22));
            rgn.Complement(new Rectangle(MainColorArea.Left, 30, (MainColorArea.Top - 35) * 2, MainColorArea.Top - 35));
            Invalidate(rgn);
            rgn.Dispose();
            if (ColorChanged is not null) ColorChanged(this, new EventArgs());
        }
        private void SetHue(double hue)
        {
            Color oldCol = color;
            lastHue = hue;
            color = Extensions.ColorFromHSV(color.A / 255d, hue, lastSat, lastVal);
            hexBox.SetTextIfNeeded(HexBoxText(Color));
            rgbBox.SetTextIfNeeded(RgbBoxText(Color));
            if (color.TryToHSL(out double h, out double s, out double l))
            {
                hslBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, s * 100d, l * 100d));
                GetSatValBlend(lastHue, ref satGrad, ref valGrad);
            }
            hsvBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, color.GetDoubleSaturation() * 100d, color.GetDoubleValue() * 100d));
            Invalidate(new Rectangle(0, 30, ClientSize.Width, MainColorArea.Bottom - 19));
            if (ColorChanged is not null) ColorChanged(this, new EventArgs());
        }
        private void SetSatVal(double sat, double val)
        {
            (double x, double y) pos = GetPositionFromColor(Color);
            Rectangle oldSel = new Rectangle(MainColorArea.Left + (int)(pos.x * MainColorArea.Width) - 8, MainColorArea.Top + (int)(pos.y * MainColorArea.Height) - 8, 16, 16);
            Color oldCol = color;
            lastSat = sat;
            lastVal = val;
            color = Extensions.ColorFromHSV(color.A / 255d, lastHue, sat, val);
            hexBox.SetTextIfNeeded(HexBoxText(Color));
            rgbBox.SetTextIfNeeded(RgbBoxText(Color));
            if (color.TryToHSL(out double h, out double s, out double l))
            {
                hslBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, s * 100d, l * 100d));
            }
            hsvBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, color.GetDoubleSaturation() * 100d, color.GetDoubleValue() * 100d));
            pos = GetPositionFromColor(Color);
            Invalidate(oldSel);
            Invalidate(new Rectangle(MainColorArea.Left, 30, (MainColorArea.Top - 35) * 2, MainColorArea.Top - 35));
            Invalidate(new Rectangle(MainColorArea.Left + (int)(pos.x * MainColorArea.Width) - 8, MainColorArea.Top + (int)(pos.y * MainColorArea.Height) - 8, 16, 16));
            if (ColorChanged is not null) ColorChanged(this, new EventArgs());
        }
        private void SetAlpha(byte a)
        {
            Color oldCol = color;
            color = Color.FromArgb(a, color);
            hexBox.SetTextIfNeeded(HexBoxText(Color));
            rgbBox.SetTextIfNeeded(RgbBoxText(Color));
            Invalidate(AlphaSliderArea.Padded(4));
            if (ColorChanged is not null) ColorChanged(this, new EventArgs());
        }
        private string HexBoxText(Color color) => $"#{(color.A is not 255 ? color.A.ToString("X2") : "")}{color.R:X2}{color.G:X2}{color.B:X2}";
        private string RgbBoxText(Color color) => $"{color.R}, {color.G}, {color.B}{(color.A is not 255 ? $", {color.A}" : "")}";
        private void SetAltHue(double hue)
        {
            Color oldCol = altColor;
            altColor = Extensions.ColorFromHSV(altColor.A / 255d, hue, lastSat, lastVal);
            hexBox.SetTextIfNeeded($"#{color.R:X2}{color.G:X2}{color.B:X2}");
            rgbBox.SetTextIfNeeded($"{color.R}, {color.G}, {color.B}");
            if (altColor.TryToHSL(out double h, out double s, out double l))
            {
                hslBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, s * 100d, l * 100d));
                GetSatValBlend(lastHue, ref satGrad, ref valGrad);
            }
            hsvBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, altColor.GetDoubleSaturation() * 100d, altColor.GetDoubleValue() * 100d));
            Invalidate(new Rectangle(0, 30, ClientSize.Width, MainColorArea.Bottom - 19));
            if (AlternateColorChanged is not null) AlternateColorChanged(this, new EventArgs());
        }
        private void SetAltSatVal(double sat, double val)
        {
            (double x, double y) pos = GetPositionFromColor(altColor);
            Rectangle oldSel = new Rectangle(MainColorArea.Left + (int)(pos.x * MainColorArea.Width) - 8, MainColorArea.Top + (int)(pos.y * MainColorArea.Height) - 8,
                16, 16);
            Color oldCol = altColor;
            altColor = Extensions.ColorFromHSV(altColor.A / 255d, lastHue, sat, val);
            hexBox.SetTextIfNeeded($"#{altColor.R:X2}{altColor.G:X2}{altColor.B:X2}");
            rgbBox.SetTextIfNeeded($"{altColor.R}, {altColor.G}, {altColor.B}");
            if (altColor.TryToHSL(out double h, out double s, out double l))
            {
                hslBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, s * 100d, l * 100d));
            }
            hsvBox.SetTextIfNeeded(string.Format("{0:0}, {1:0.##}%, {2:0.##}%", lastHue, altColor.GetDoubleSaturation() * 100d, altColor.GetDoubleValue() * 100d));
            pos = GetPositionFromColor(altColor);
            Invalidate(oldSel);
            Invalidate(new Rectangle(MainColorArea.Left, 30, (MainColorArea.Top - 35) * 2, MainColorArea.Top - 35));
            Invalidate(new Rectangle(MainColorArea.Left + (int)(pos.x * MainColorArea.Width) - 8, MainColorArea.Top + (int)(pos.y * MainColorArea.Height) - 8, 16, 16));
            if (AlternateColorChanged is not null) AlternateColorChanged(this, new EventArgs());
        }
        public bool ValidateHex(string str)
        {
            return (str.Length is 7 && str[0] is '#' && uint.TryParse(str[1..], NumberStyles.HexNumber, null, out _)) || (str.Length is 6 &&
                uint.TryParse(str, NumberStyles.HexNumber, null, out _));
        }
        public bool ValidateRgb(string str)
        {
            if (Extensions.TryStringToIntArray(str, out int[] arr) && arr.Length == 3)
            {
                return arr.All((int num) => num is >= 0 and <= 255);
            }
            else return false;
        }
        public bool ValidateHsl(string str)
        {
            if (Extensions.TryStringToDoubleArray(str.Replace("%", ""), out double[] arr) && arr.Length == 3)
            {
                return arr[0] is >= 0d and < 360d && arr[1] is >= 0d and <= 100d && arr[2] is >= 0d and <= 100d;
            }
            else return false;
        }
        public bool GetSatValBlend(double hue, ref ColorBlend sat, ref ColorBlend val)
        {
            val.Colors = new Color[] { Extensions.ColorFromHSV(hue, 0d, 1d), Extensions.ColorFromHSV(hue, 1d, 1d) };
            val.Positions = new float[] { 0f, 1f };
            sat.Colors = new Color[] { Extensions.ColorFromHSV(0d, hue, 1d, 1d), Extensions.ColorFromHSV(hue, 0d, 0d) };
            sat.Positions = new float[] { 0f, 1f };
            LinearGradientBrush b = valBrush;
            valBrush = new LinearGradientBrush(MainColorArea, Color.Black, Color.Black, LinearGradientMode.Horizontal) { InterpolationColors = valGrad };
            if (b is not null) b.Dispose();
            b = satBrush;
            satBrush = new LinearGradientBrush(MainColorArea, Color.Black, Color.Black, LinearGradientMode.Vertical) { InterpolationColors = satGrad };
            if (b is not null) b.Dispose();
            return true;
        }
        private void ColorPicker_Disposed(object sender, EventArgs e)
        {
            if (hexBox is not null)
            {
                Controls.Remove(hexBox);
                hexBox.Dispose();
                hexBox = null;
            }
            if (rgbBox is not null)
            {
                Controls.Remove(rgbBox);
                rgbBox.Dispose();
                rgbBox = null;
            }
            if (hslBox is not null)
            {
                Controls.Remove(hslBox);
                hslBox.Dispose();
                hslBox = null;
            }
            if (hueBrush is not null)
            {
                hueBrush.Dispose();
                hueBrush = null;
            }
            if (satBrush is not null)
            {
                satBrush.Dispose();
                satBrush = null;
            }
            if (valBrush is not null)
            {
                valBrush.Dispose();
                valBrush = null;
            }
        }

        private void HexLabelClick(object sender, EventArgs e)
        {
            if (hexBox is not null) hexBox.Focus();
        }

        private void RgbLabelClick(object sender, EventArgs e)
        {
            if (rgbBox is not null) rgbBox.Focus();
        }

        private void HslLabelClick(object sender, EventArgs e)
        {
            if (hslBox is not null) hslBox.Focus();
        }

        private void HsvLabelClick(object sender, EventArgs e)
        {
            if (hsvBox is not null) hsvBox.Focus();
        }
    }
}
