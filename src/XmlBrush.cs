using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Paintr
{
    public class XmlBrush : DrawBrush
    {
        public XmlBrush(XmlDocument doc)
        {
            pen = new Pen(Color.Black, 8f);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            brush = new SolidBrush(Color.Black);
            id = doc.DocumentElement.SelectSingleNode("//brush/id").InnerText;
            XmlNode n;
            if ((n = doc.DocumentElement.SelectSingleNode($"//brush/name/{System.Globalization.CultureInfo.CurrentCulture.Name}")) is not null) friendlyName = n.InnerText;
            else if ((n = doc.DocumentElement.SelectSingleNode(
                $"//brush/name/{System.Globalization.CultureInfo.CurrentCulture.Name[..System.Globalization.CultureInfo.CurrentCulture.Name.IndexOf('-')]}")) is not null)
                friendlyName = n.InnerText;
            else friendlyName = doc.DocumentElement.SelectSingleNode("//brush/name/default").InnerText;
            hasColor = doc.DocumentElement.SelectSingleNode("//brush/color") is not null;
            hasWidth = doc.DocumentElement.SelectSingleNode("//brush/width") is not null;
            if ((n = doc.DocumentElement.SelectSingleNode("//brush/cursor")) is not null) cursorType = (CursorType)Enum.Parse(typeof(CursorType), n.InnerText, true);

            XmlNodeList xl = doc.DocumentElement.SelectNodes("//brush/effects/effect");
            effects = xl.Count is 0 ? Array.Empty<string>() : new string[xl.Count];
            if(xl.Count > 0)
            {
                bmp = new Bitmap(((int)width + 2) * 4, ((int)width + 2) * 4);
                grfx = Graphics.FromImage(bmp);
                grfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grfx.SmoothingMode = SmoothingMode.AntiAlias;
                region = new Region(new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i] = xl[i].InnerText;
            }
        }
        public static XmlBrush Register(XmlDocument doc)
        {
            XmlBrush b = new XmlBrush(doc);
            BrushRack.Brushes[b.Id] = b;
            return b;
        }
        ~XmlBrush()
        {
            pen.Dispose();
            brush.Dispose();
            if (grfx is not null) grfx.Dispose();
            if (bmp is not null) bmp.Dispose();
        }

        private string[] effects;
        private Brush brush;
        private Pen pen;
        private Bitmap bmp;
        private Graphics grfx;
        private Region region;

        private readonly string id;
        public override string Id { get => id; }
        private string friendlyName;
        public override string FriendlyName { get => friendlyName; }
        private Color color;
        public override Color Color
        {
            get => HasColor ? color : Color.Black;
            set
            {
                if (!HasColor) return;
                color = value;
                pen.Color = color;
                if(brush is SolidBrush b) b.Color = color;
                FireColorChanged();
            }
        }
        private bool hasColor;
        public override bool HasColor { get => hasColor; }
        private double width = 8d;
        public override double Width
        {
            get => HasWidth ? width : 1d;
            set
            {
                if (!HasWidth) return;
                if (value <= 0d) throw new ArgumentException("", nameof(value));
                width = value;
                pen.Width = (float)width;
                if (bmp is not null)
                {
                    Bitmap b = bmp;
                    Graphics g = grfx;
                    Region r = region;
                    bmp = new Bitmap(((int)width + 2) * 4, ((int)width + 2) * 4);
                    grfx = Graphics.FromImage(bmp);
                    grfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grfx.SmoothingMode = SmoothingMode.AntiAlias;
                    region = new Region(new Rectangle(0, 0, bmp.Width, bmp.Height));
                    r.Dispose();
                    g.Dispose();
                    b.Dispose();
                }
                FireWidthChanged();
            }
        }
        private bool hasWidth;
        public override bool HasWidth { get => hasWidth; }

        private PointF lastPt;
        public override void BrushPress(BrushPressedEventArgs e)
        {
            SmoothingMode smoothing = e.Layer.Graphics.SmoothingMode;
            e.Layer.Graphics.SmoothingMode = SmoothingMode.None;
            float w;
            RectangleF lt;
            RectangleF br;
            switch (e.ActionType)
            {
                case BrushPressedAction.Down:
                    w = (float)Width;
                    if (bmp is not null) {
                        lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                        grfx.Clear(Color.Transparent);
                        grfx.FillEllipse(brush, lt.Left + 2f, lt.Top + 2f, lt.Width, lt.Height);
                        foreach (string s in effects) ImageEditing.DoEffect(s, bmp, region);
                        e.Layer.Graphics.DrawImage(bmp, lt.Left - 2f, lt.Top - 2f);
                    }
                    else e.Layer.Graphics.FillEllipse(brush, new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w));
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.Viewport.Invalidate(Editor.Viewport.ToControlRectangle(lt).InflateToRectangle());
                    lastPt = e.Location;
                    break;
                case BrushPressedAction.Move:
                    if (bmp is not null) {
                        w = (float)Width;
                        lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                        grfx.Clear(Color.Transparent);

                        float x1 = lastPt.X < e.Location.X ? lastPt.X : e.Location.X;
                        float x2 = lastPt.X > e.Location.X ? lastPt.X : e.Location.X;
                        float y1 = lastPt.Y < e.Location.Y ? lastPt.Y : e.Location.Y;
                        float y2 = lastPt.Y > e.Location.Y ? lastPt.Y : e.Location.Y;

                        grfx.DrawLine(pen, 2f, 2f, x2 - x1 + 2f, y2 - y1 + 2f);
                        foreach (string s in effects) ImageEditing.DoEffect(s, bmp, region);
                        e.Layer.Graphics.DrawImage(bmp, lt.Left - 2f, lt.Top - 2f);
                    }
                    else e.Layer.Graphics.DrawLine(pen, lastPt, e.Location);
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.Viewport.Invalidate(Editor.Viewport.ToControlRectangle(RectangleF.Union(lt, br)).InflateToRectangle());
                    lastPt = e.Location;
                    break;
                case BrushPressedAction.Up:
                    if (bmp is not null)
                    {
                        w = (float)Width;
                        lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                        grfx.Clear(Color.Transparent);

                        float x1 = lastPt.X < e.Location.X ? lastPt.X : e.Location.X;
                        float x2 = lastPt.X > e.Location.X ? lastPt.X : e.Location.X;
                        float y1 = lastPt.Y < e.Location.Y ? lastPt.Y : e.Location.Y;
                        float y2 = lastPt.Y > e.Location.Y ? lastPt.Y : e.Location.Y;

                        grfx.DrawLine(pen, 2f, 2f, x2 - x1 + 2f, y2 - y1 + 2f);
                        foreach (string s in effects) ImageEditing.DoEffect(s, bmp, region);
                        e.Layer.Graphics.DrawImage(bmp, lt.Left - 2f, lt.Top - 2f);
                    }
                    else e.Layer.Graphics.DrawLine(pen, lastPt, e.Location);
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.Viewport.Invalidate(Editor.Viewport.ToControlRectangle(RectangleF.Union(lt, br)).InflateToRectangle());
                    break;
            }
            e.Layer.Graphics.SmoothingMode = smoothing;
        }
        private CursorType cursorType = CursorType.Standard;
        public override CursorType CursorType { get => cursorType; }
        public override string ToString()
        {
            return FriendlyName;
        }
    }
}
