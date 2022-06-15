using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Paintr
{
    public abstract class DrawBrush
    {
        public abstract string FriendlyName { get; }
        public abstract string Id { get; }
        public abstract void BrushPress(BrushPressedEventArgs e);
        public virtual void BrushEquipped() { }
        public virtual void BrushUnequipped() { }
        public virtual CursorType CursorType { get => CursorType.Standard; }

        public abstract bool HasColor { get; }
        public abstract Color Color { get; set; }
        public event EventHandler ColorChanged;

        public abstract bool HasWidth { get; }
        public abstract double Width { get; set; }
        public event EventHandler WidthChanged;

        protected void FireColorChanged()
        {
            if (ColorChanged is not null) ColorChanged(this, new EventArgs());
        }
        protected void FireWidthChanged()
        {
            if (WidthChanged is not null) WidthChanged(this, new EventArgs());
        }
    }
    public static class BrushRack
    {
        static BrushRack()
        {
            Brushes = new();
            Brushes[PaintBrushId] = new PaintBrush();
            Brushes[PixelPerfectId] = new PixelPerfectBrush();
            Brushes[RectSelectId] = new RectSelect();
            Brushes[SelectBrushId] = new SelectBrush();
            Brushes[PaintBucketId] = new FillBrush();
            Brushes[TextBrushId] = new TextBrush();
            Brushes[ColorPickerId] = new ColorPickerBrush();
            Brushes[InkPenId] = new DrawBrushes.InkPenBrush();
            foreach(string path in System.IO.Directory.EnumerateFiles("XmlBrushes"))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.Open);
                doc.Load(fs);
                XmlBrush.Register(doc);
                fs.Dispose();
            }
        }
        public static Dictionary<string, DrawBrush> Brushes { get; }

        public const string PaintBrushId = "base:paintbrush";
        public const string PixelPerfectId = "base:pixelperfect";
        public const string RectSelectId = "base:rectselect";
        public const string SelectBrushId = "base:selectbrush";
        public const string PaintBucketId = "base:fillbrush";
        public const string TextBrushId = "base:textbrush";
        public const string ColorPickerId = "base:colorpicker";
        public const string InkPenId = "base:inkpen";
    }
    public class PaintBrush : DrawBrush
    {
        public PaintBrush()
        {
            pen = new Pen(Color.Black, 8f);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            brush = new SolidBrush(Color.Black);
        }
        ~PaintBrush()
        {
            pen.Dispose();
            brush.Dispose();
        }
        private Pen pen;
        private SolidBrush brush;
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
                pen.Color = color;
                brush.Color = color;
                FireColorChanged();
            }
        }
        public override string FriendlyName { get => Properties.Strings.BrushPaintBrush; }
        public override string Id { get => "base:paintbrush"; }
        public override string ToString()
        {
            return FriendlyName;
        }
        private PointF lastPt;
        public override void BrushPress(BrushPressedEventArgs e)
        {
            Region clip = null;
            if (!Editor.SelectionRegion.IsEmpty(e.Layer.Graphics)) {
                clip = e.Layer.Graphics.Clip;
                e.Layer.Graphics.Clip = Editor.SelectionRegion;
            }
            float w;
            RectangleF lt;
            RectangleF br;
            switch (e.ActionType)
            {
                case BrushPressedAction.Down:
                    w = (float)Width;
                    e.Layer.Graphics.FillEllipse(brush, new RectangleF(e.Location.X - w / 2f, e.Location.Y - w / 2f, w, w));
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(e.Location.X - w / 2f, e.Location.Y - w / 2f, w, w);
                    Editor.Viewport.InvalidateImage(lt);
                    lastPt = e.Location;
                    break;
                case BrushPressedAction.Move:
                    e.Layer.Graphics.DrawLine(pen, lastPt, e.Location);
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    lastPt = e.Location;
                    break;
                case BrushPressedAction.Up:
                    e.Layer.Graphics.DrawLine(pen, lastPt, e.Location);
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    break;
            }
            if(clip is not null) e.Layer.Graphics.Clip = clip;
        }
        public override CursorType CursorType { get => CursorType.Standard; }
    }
    public class PixelPerfectBrush : DrawBrush
    {
        public PixelPerfectBrush()
        {
            pen = new Pen(Color.Black, 8f);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            brush = new SolidBrush(Color.Black);
        }
        private Pen pen;
        private SolidBrush brush;
        private double width = 8d;
        public override bool HasWidth { get => true; }
        public override bool HasColor { get => true; }
        public override double Width
        {
            get => width;
            set
            {
                if (value <= 0d) throw new ArgumentException("", nameof(value));
                width = value;
                pen.Width = (float)width;
                FireWidthChanged();
            }
        }
        private Color color;
        public override Color Color
        {
            get => color;
            set
            {
                color = value;
                pen.Color = color;
                brush.Color = color;
                FireColorChanged();
            }
        }
        public override string FriendlyName { get => Properties.Strings.BrushPixelPerfect; }
        public override string Id { get => "base:pixelperfect"; }
        public override string ToString()
        {
            return FriendlyName;
        }
        private PointF lastPt;
        public override void BrushPress(BrushPressedEventArgs e)
        {
            SmoothingMode smoothing = e.Layer.Graphics.SmoothingMode;
            e.Layer.Graphics.SmoothingMode = SmoothingMode.None;
            float w;
            RectangleF lt;
            RectangleF br;
            PointF loc = new PointF(e.Location.X - e.Location.X % 1, e.Location.Y - e.Location.Y % 1);
            switch (e.ActionType)
            {
                case BrushPressedAction.Down:
                    w = (float)Width;
                    lt = new RectangleF(loc.X - w / 2, loc.Y - w / 2, w, w);
                    if (w >= 2f) e.Layer.Graphics.FillEllipse(brush, lt);
                    else e.Layer.Graphics.FillRectangle(brush, loc.X - loc.X % 1f, loc.Y - loc.Y % 1f, 1f, 1f);
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(loc.X - w / 2, loc.Y - w / 2, w, w);
                    Editor.Viewport.InvalidateImage(lt);
                    lastPt = loc;
                    break;
                case BrushPressedAction.Move:
                    e.Layer.Graphics.DrawLine(pen, lastPt, loc);
                    w = (float)Width;
                    lt = new RectangleF(e.Location.X - w / 2, loc.Y - w / 2, w, w);
                    if (w >= 2f) e.Layer.Graphics.FillEllipse(brush, lt);
                    else e.Layer.Graphics.FillRectangle(brush, loc.X - loc.X % 1f, loc.Y - loc.Y % 1f, 1f, 1f);
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(loc.X - w / 2, loc.Y - w / 2, w, w);
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    lastPt = loc;
                    break;
                case BrushPressedAction.Up:
                    e.Layer.Graphics.DrawLine(pen, lastPt, loc);
                    w = (float)Width;
                    lt = new RectangleF(loc.X - w / 2, loc.Y - w / 2, w, w);
                    if (w >= 2f) e.Layer.Graphics.FillEllipse(brush, lt);
                    else e.Layer.Graphics.FillRectangle(brush, loc.X - loc.X % 1f, loc.Y - loc.Y % 1f, 1f, 1f);
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(loc.X - w / 2, loc.Y - w / 2, w, w);;
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    break;
            }
            e.Layer.Graphics.SmoothingMode = smoothing;
        }
        public override CursorType CursorType { get => CursorType.Standard; }
    }
    public class RectSelect : DrawBrush
    {
        public RectSelect()
        {

        }
        public override string FriendlyName { get => Properties.Strings.BrushRectSelect; }
        public override string Id { get => "base:rectselect"; }
        public override string ToString()
        {
            return FriendlyName;
        }
        private PointF orig;
        private RectangleF rect;
        public override bool HasWidth { get => false; }
        public override bool HasColor { get => false; }
        public override void BrushPress(BrushPressedEventArgs e)
        {
            PointF p;
            switch (e.ActionType)
            {
                case BrushPressedAction.Down:
                    orig = e.Layer.ImageRect.GetContainingPoint(e.Location);
                    rect = RectangleF.Empty;
                    if (!e.ControlDown) Editor.MakeEmptySelectionRegion();
                    break;
                case BrushPressedAction.Move:
                    p = e.Layer.ImageRect.GetContainingPoint(e.Location);
                    Editor.SelectionRegion.Exclude(rect);
                    rect = RectangleF.FromLTRB(orig.X < p.X ? orig.X : p.X, orig.Y < p.Y ? orig.Y : p.Y, orig.X > p.X ? orig.X : p.X, orig.Y > p.Y ? orig.Y : p.Y);
                    rect = Editor.RoundSelection ? rect.Size == SizeF.Empty ? RectangleF.Empty : rect.InflateToRectangle() : rect;
                    Editor.UnionSelectionRegion(rect);
                    break;
                case BrushPressedAction.Up:
                    p = e.Layer.ImageRect.GetContainingPoint(e.Location);
                    Editor.SelectionRegion.Exclude(rect);
                    rect = RectangleF.FromLTRB(orig.X < p.X ? orig.X : p.X, orig.Y < p.Y ? orig.Y : p.Y, orig.X > p.X ? orig.X : p.X, orig.Y > p.Y ? orig.Y : p.Y);
                    rect = Editor.RoundSelection ? rect.Size == SizeF.Empty ? RectangleF.Empty : rect.InflateToRectangle() : rect;
                    Editor.UnionSelectionRegion(rect);
                    break;
            }
        }
        public override CursorType CursorType { get => CursorType.ForceCrosshair; }

        public override Color Color { get => Color.Black; set { } }
        public override double Width { get => 1d; set { } }
    }
    public enum CursorType
    {
        Standard,
        ForceCrosshair
    }



    public class SelectBrush : DrawBrush
    {
        public SelectBrush()
        {

        }
        public override bool HasWidth { get => true; }
        public override bool HasColor { get => false; }
        private double width = 8d;
        public override double Width
        {
            get => width;
            set
            {
                if (value <= 0d) throw new ArgumentException(nameof(value));
                width = value;
                FireWidthChanged();
            }
        }
        public override Color Color
        {
            get => Color.Black;
            set { }
        }
        public override string FriendlyName { get => Properties.Strings.BrushSelectBrush; }
        public override string Id { get => "base:selectbrush"; }
        public override string ToString()
        {
            return FriendlyName;
        }
        private PointF lastPt;
        public override void BrushPress(BrushPressedEventArgs e)
        {
            GraphicsPath p = new GraphicsPath();
            float w;
            RectangleF lt;
            RectangleF br;
            switch (e.ActionType)
            {
                case BrushPressedAction.Down:
                    w = (float)Width;
                    lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    if (w >= 2f) p.AddEllipse(lt);
                    else p.AddRectangle(new RectangleF(e.Location.X - e.Location.X % 1f, e.Location.Y - e.Location.Y % 1f, 1f, 1f));
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.SelectionRegion.Union(p);
                    Editor.Viewport.InvalidateImage(lt);
                    lastPt = e.Location;
                    break;
                case BrushPressedAction.Move:
                    p.AddLine(lastPt, e.Location);
                    w = (float)Width;
                    lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    if (w >= 2f) p.AddEllipse(lt);
                    else p.AddRectangle(new RectangleF(e.Location.X - e.Location.X % 1f, e.Location.Y - e.Location.Y % 1f, 1f, 1f));
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.SelectionRegion.Union(p);
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    lastPt = e.Location;
                    break;
                case BrushPressedAction.Up:
                    p.AddLine(lastPt, e.Location);
                    w = (float)Width;
                    lt = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    if (w >= 2f) p.AddEllipse(lt);
                    else p.AddRectangle(new RectangleF(e.Location.X - e.Location.X % 1f, e.Location.Y - e.Location.Y % 1f, 1f, 1f));
                    w = (int)Math.Ceiling(Width);
                    lt = new RectangleF(lastPt.X - w / 2, lastPt.Y - w / 2, w, w);
                    br = new RectangleF(e.Location.X - w / 2, e.Location.Y - w / 2, w, w);
                    Editor.SelectionRegion.Union(p);
                    Editor.Viewport.InvalidateImage(RectangleF.Union(lt, br));
                    break;
            }
            p.Dispose();
        }
        public override CursorType CursorType { get => CursorType.Standard; }
    }

    public class FillBrush : DrawBrush
    {
        public FillBrush()
        {
            brush = new SolidBrush(Color.Black);
        }
        ~FillBrush()
        {
            brush.Dispose();
        }
        private SolidBrush brush;
        public override bool HasWidth { get => false; }
        public override bool HasColor { get => true; }
        public override double Width
        {
            get => 1d;
            set { }
        }
        private Color color;
        public override Color Color
        {
            get => color;
            set
            {
                color = value;
                brush.Color = color;
                FireColorChanged();
            }
        }
        public override string FriendlyName { get => Properties.Strings.BrushFill; }
        public override string Id { get => "base:fillbrush"; }
        public override string ToString()
        {
            return FriendlyName;
        }
        public override void BrushPress(BrushPressedEventArgs e)
        {
            if(e.ActionType is BrushPressedAction.Down)
            {
                Region rgn = ImageEditing.FloodSelect(e.Layer.Bitmap, new Point((int)MathF.Floor(e.Location.X), (int)MathF.Floor(e.Location.Y)), 10);
                e.Layer.Graphics.FillRegion(brush, rgn);
                rgn.Dispose();
                Editor.Viewport.InvalidateImage();
            }
        }
        public override CursorType CursorType { get => CursorType.ForceCrosshair; }
    }

    public class TextBrush : DrawBrush
    {
        public TextBrush()
        {
            brush = new SolidBrush(Color.Black);
        }
        ~TextBrush()
        {
            brush.Dispose();
        }
        private SolidBrush brush;
        public override bool HasWidth { get => false; }
        public override bool HasColor { get => true; }
        public override double Width
        {
            get => 1d;
            set { }
        }
        private Color color;
        public override Color Color
        {
            get => color;
            set
            {
                color = value;
                brush.Color = color;
                FireColorChanged();
            }
        }
        public override string FriendlyName { get => Properties.Strings.BrushText; }
        public override string Id { get => "base:textbrush"; }
        public override string ToString()
        {
            return FriendlyName;
        }
        public override void BrushPress(BrushPressedEventArgs e)
        {
            if (e.ActionType is BrushPressedAction.Down)
            {
                Editor.FireRequestShowTextEdit();
            }
        }
        public override CursorType CursorType { get => CursorType.ForceCrosshair; }
    }
    public class ColorPickerBrush : DrawBrush
    {
        public ColorPickerBrush()
        {

        }
        public override bool HasWidth { get => false; }
        public override bool HasColor { get => false; }
        public override double Width
        {
            get => 1d;
            set { }
        }
        public override Color Color
        {
            get => default; set { }
        }
        public override string FriendlyName { get => Properties.Strings.BrushColorPicker; }
        public override string Id { get => "base:colorpicker"; }
        public override string ToString()
        {
            return FriendlyName;
        }
        public override void BrushPress(BrushPressedEventArgs e)
        {
            if (e.InImage && e.ActionType is BrushPressedAction.Down)
            {
                if(e.MouseButtons is System.Windows.Forms.MouseButtons.Right)
                    Editor.ColorPicker.AlternateColor = Editor.EditingImage.GetPixel((int)MathF.Floor(e.Location.X), (int)MathF.Floor(e.Location.Y));
                else Editor.ColorPicker.Color = Editor.EditingImage.GetPixel((int)MathF.Floor(e.Location.X), (int)MathF.Floor(e.Location.Y));
            }
        }
        public override CursorType CursorType { get => CursorType.ForceCrosshair; }
    }
}
