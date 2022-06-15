using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Paintr.DrawBrushes
{
    public class InkPenBrush : DrawBrush
    {
        public InkPenBrush()
        {
            pen = new Pen(Color.Black, 8f);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
        }
        ~InkPenBrush()
        {
            pen.Dispose();
        }
        private Pen pen;
        public override bool HasWidth { get => true; }
        public override bool HasColor { get => true; }
        private double width = 8d;
        public override double Width
        {
            get => width;
            set
            {
                if (value <= 0d) throw new ArgumentException(nameof(value));
                width = value;
                pen.Width = (float)width;
                FireWidthChanged();
            }
        }
        private Color color = Color.Black;
        public override Color Color
        {
            get => color;
            set
            {
                color = value;
                color.TryToHSV(out hue, out sat, out val);
                pen.Color = color;
                FireColorChanged();
            }
        }
        public override string FriendlyName { get => Properties.Strings.BrushInkPen; }
        public override string Id { get => BrushRack.InkPenId; }
        public override string ToString()
        {
            return FriendlyName;
        }
        private PointF lastPt;
        private bool isPressed;
        private double hue;
        private double sat;
        private double val;
        public override unsafe void BrushPress(BrushPressedEventArgs e)
        {
            Region clip = null;
            if (!Editor.SelectionRegion.IsEmpty(e.Layer.Graphics))
            {
                clip = e.Layer.Graphics.Clip;
                e.Layer.Graphics.Clip = Editor.SelectionRegion;
            }
            float w = (float)Width;
            RectangleF lt = new RectangleF(e.Location.X - w / 2f, e.Location.Y - w / 2f, w, w);
            RectangleF br = new RectangleF(lastPt.X - w / 2f, lastPt.Y - w / 2f, w, w);
            Rectangle dr = isPressed ? RectangleF.Union(lt, br).Constrain(e.Layer.Graphics.ClipBounds).DeflateToRectangle() :
                lt.Constrain(e.Layer.Graphics.ClipBounds).DeflateToRectangle();
            if (dr.GeoArea() is 0) return;
            GraphicsPath p;
            BitmapData d = e.Layer.Bitmap.LockBits(dr, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)d.Scan0;
            int width = d.Width;
            int height = d.Height;
            switch (e.ActionType)
            {
                case BrushPressedAction.Down:
                    p = new GraphicsPath();
                    p.AddEllipse(new RectangleF(lt.X % 1, lt.Y % 1, lt.Width, lt.Height));
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (p.IsVisible(x, y))
                            {
                                Color c = Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
                                c.TryToHSV(out double h, out double s, out double v);
                                Color output = Extensions.ColorFromHSV(hue, sat, (v - val) * 0.75d + val);
                                ptr[2] = output.R;
                                ptr[1] = output.G;
                                ptr[0] = output.B;
                                ptr[3] = (byte)(c.A + Color.A - Color.A * c.A);
                            }
                            ptr += 4;
                        }
                        ptr += d.Stride - (width * 4);
                    }
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(e.Location.X - w / 2f, e.Location.Y - w / 2f, w, w);
                    Editor.Viewport.InvalidateImage(lt);
                    lastPt = e.Location;
                    p.Dispose();
                    isPressed = true;
                    break;
                case BrushPressedAction.Move:
                    p = new GraphicsPath();
                    if (lastPt != e.Location && false) {
                        p.AddLine(lastPt, e.Location);
                        p.CloseFigure();
                        p.Widen(pen);
                    }
                    p.AddEllipse(new RectangleF(lt.X % 1, lt.Y % 1, lt.Width, lt.Height));
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (p.IsVisible(x, y))
                            {
                                Color c = Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
                                c.TryToHSV(out double h, out double s, out double v);
                                Color output = Extensions.ColorFromHSV(hue, sat, (v - val) * 0.75d + val);
                                ptr[2] = output.R;
                                ptr[1] = output.G;
                                ptr[0] = output.B;
                                ptr[3] = (byte)(c.A + Color.A - Color.A * c.A);
                            }
                            ptr += 4;
                        }
                        ptr += d.Stride - (width * 4);
                    }
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    lastPt = e.Location;
                    p.Dispose();
                    break;
                case BrushPressedAction.Up:
                    p = new GraphicsPath();
                    if (lastPt != e.Location && false) {
                        p.AddLine(lastPt, e.Location);
                        p.CloseFigure();
                        p.Widen(pen);
                    }
                    p.AddEllipse(new RectangleF(lt.X % 1, lt.Y % 1, lt.Width, lt.Height));
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (p.IsVisible(x, y))
                            {
                                Color c = Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
                                c.TryToHSV(out double h, out double s, out double v);
                                Color output = Extensions.ColorFromHSV(hue, sat, (v - val) * 0.75d + val);
                                ptr[2] = output.R;
                                ptr[1] = output.G;
                                ptr[0] = output.B;
                                ptr[3] = (byte)(c.A + Color.A - Color.A * c.A);
                            }
                            ptr += 4;
                        }
                        ptr += d.Stride - (width * 4);
                    }
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    p.Dispose();
                    isPressed = false;
                    break;
            }
            e.Layer.Bitmap.UnlockBits(d);
            if (clip is not null) e.Layer.Graphics.Clip = clip;
        }
        public override CursorType CursorType { get => CursorType.Standard; }
    }
}
