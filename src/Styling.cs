using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Paintr
{
    public static class Styling
    {
        public static Color ButtonBackgroundColor { get => Properties.Settings.Default.ButtonBackgroundColor; }
        public static Color ButtonBorderColor { get => Properties.Settings.Default.ButtonBorderColor; }
        public static Color ButtonMouseOverColor { get => Properties.Settings.Default.ButtonMouseOverColor; }
        public static Color ButtonMouseDownColor { get => Properties.Settings.Default.ButtonMouseDownColor; }
        public static Color TextColor { get => Properties.Settings.Default.TextColor; }
        public static Color TextUnsavedColor { get => Properties.Settings.Default.TextUnsavedColor; }
        public static Color TextDisabledColor { get => Properties.Settings.Default.TextDisabledColor; }
        public static Color BackgroundColor { get => Properties.Settings.Default.BackgroundColor; }
        public static Color WindowGripColor { get => Properties.Settings.Default.WindowGripColor; }
        public static Color ContrastBackgroundColor { get => Properties.Settings.Default.ContrastBackgroundColor; }
        public static Color ContrastBackgroundDownColor { get => Properties.Settings.Default.ContrastBackgroundDownColor; }
        public static Color[] GetTileModeGridColors() => new Color[8] { Color.Red, Color.DarkOrange, Color.Lime, Color.Cyan, Color.Blue, Color.RebeccaPurple, Color.Purple,
            Color.Magenta };
        public static char NumberComma { get => System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Contains(',') ? ';' : ','; }
        public static char DecimalSeparator { get => System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]; }
        public static string VisibleIconPath { get; } = System.IO.Path.Combine("Image", "Visible.png");
        public static string InvisibleIconPath { get; } = System.IO.Path.Combine("Image", "Invisible.png");
        public static string PropertiesIconPath { get; } = System.IO.Path.Combine("Image", "Property.png");
        public static string AddIconPath { get; } = System.IO.Path.Combine("Image", "Add.png");
        public static string CheckBoxCheckedPath { get; } = System.IO.Path.Combine("Image", "CheckBoxChecked.png");
        public static void FormatButton(Button b, bool setFlatStyle = true)
        {
            b.ForeColor = TextColor;
            b.BackColor = ButtonBackgroundColor;
            if(setFlatStyle) b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor = ButtonBorderColor;
            b.FlatAppearance.MouseOverBackColor = ButtonMouseOverColor;
            b.FlatAppearance.MouseDownBackColor = ButtonMouseDownColor;
            b.FlatAppearance.BorderSize = 2;
        }
        public static void FormatLabel(Label l)
        {
            l.ForeColor = TextColor;
        }
        public static void FormatControl(Control c)
        {
            c.ForeColor = TextColor;
            c.BackColor = BackgroundColor;
        }
        public static Bitmap ColorImage(string path, Color col)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(path);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;
            Bitmap bmp = (Bitmap)Image.FromStream(ms);
            fs.Dispose();
            ImageEditing.ColorPixelRegion(bmp, new Rectangle(Point.Empty, bmp.Size), col, true);
            return bmp;
        }
        public static Bitmap LoadImageFromFile(string path)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(path);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;
            Bitmap bmp = (Bitmap)Image.FromStream(ms);
            fs.Dispose();
            return bmp;
        }
    }
}
