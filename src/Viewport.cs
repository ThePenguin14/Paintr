using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Paintr
{
    public partial class Viewport : Control
    {
        public Viewport()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            selectionBrush = new SolidBrush(Color.FromArgb(64, Styling.ContrastBackgroundColor));
            buttonBackgroundBrush = new SolidBrush(Styling.ButtonBackgroundColor);
            buttonBorderPen = new Pen(Styling.ButtonBorderColor, 3f);
            textPen = new Pen(Styling.TextColor, 1f);
            textUnsavedPen = new Pen(Styling.TextUnsavedColor, 1f);
            pixelGridPen = new Pen(Styling.TextUnsavedColor, 1f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            if (Editor.CurrentBrushWidth.HasValue) SetCursorSize((int)Editor.CurrentBrushWidth.Value);
            BackColor = Styling.BackgroundColor.GetStandOutColor(2);
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Disposed += Viewport_Disposed;
            SizeChanged += Viewport_SizeChanged;
            singleImage = new Bitmap(imageSize.Width, imageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        private void Viewport_SizeChanged(object sender, EventArgs e)
        {
            rulerWidth = ImageRect.Width * 1.1f;
            rulerPosition = new PointF(ImageRect.Left - ImageRect.Width * 0.05f, ImageRect.Top);
            protractorWidth = ImageRect.Width / 2f;
            protractorPosition = new PointF(ImageRect.Left + ImageRect.Width * 0.25f, ImageRect.Top);
        }

        private void Viewport_Disposed(object sender, EventArgs e)
        {
            if (selectionBrush is not null)
            {
                selectionBrush.Dispose();
                selectionBrush = null;
            }
            if (buttonBackgroundBrush is not null)
            {
                buttonBackgroundBrush.Dispose();
                buttonBackgroundBrush = null;
            }
            if (buttonBorderPen is not null)
            {
                buttonBorderPen.Dispose();
                buttonBorderPen = null;
            }
            if (textPen is not null)
            {
                textPen.Dispose();
                textPen = null;
            }
            if (textUnsavedPen is not null)
            {
                textUnsavedPen.Dispose();
                textUnsavedPen = null;
            }
            if (pixelGridPen is not null)
            {
                pixelGridPen.Dispose();
                pixelGridPen = null;
            }
            if (singleImage is not null)
            {
                singleImage.Dispose();
                singleImage = null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left)
            {
                if (Editor.Brush.HasColor && Editor.Brush.Color != Editor.ColorPicker.Color) Editor.Brush.Color = Editor.ColorPicker.Color;
                RectangleF r = ImageRect;
                Point pt = PointToClient(Cursor.Position);
                mouseOn = 0;
                if (ImageRect.Contains(pt))
                {
                    if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Down, layers[layerIndex],
                        ToImagePoint(pt), pt, true, e.Button, ModifierKeys.HasFlag(Keys.Control)));
                }
                else if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Down, layers[layerIndex],
                        ToImagePoint(pt), pt, false, e.Button, ModifierKeys.HasFlag(Keys.Control)));
            }
            else if(e.Button is MouseButtons.Right)
            {
                Point pt = PointToClient(Cursor.Position);
                if (ShowRuler && RulerRect.Contains(pt))
                {
                    mouseOn = 1;
                    lastPt = new PointF(pt.X - rulerPosition.X, pt.Y - rulerPosition.Y);
                }
                else if (ShowRuler && new RectangleF(RulerRect.Right - 5, RulerRect.Top + RulerRect.Height / 2f - 5f, 10f, 10f).Contains(pt))
                {
                    mouseOn = 2;
                    lastW = rulerWidth;
                    lastPt = pt;
                }
                else
                {
                    mouseOn = 0;
                    if (Editor.Brush.HasColor && Editor.Brush.Color != Editor.ColorPicker.AlternateColor) Editor.Brush.Color = Editor.ColorPicker.AlternateColor;
                    RectangleF r = ImageRect;
                    if (ImageRect.Contains(pt))
                    {
                        if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Down, layers[layerIndex],
                            ToImagePoint(pt), pt, true, e.Button, ModifierKeys.HasFlag(Keys.Control)));
                    }
                    else if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Down, layers[layerIndex],
                            ToImagePoint(pt), pt, false, e.Button, ModifierKeys.HasFlag(Keys.Control)));
                }
            }
            base.OnMouseDown(e);
        }
        private byte mouseOn;
        private PointF lastPt;
        private float lastW;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left or MouseButtons.Right) {
                RectangleF r = ImageRect;
                Point pt = PointToClient(Cursor.Position);
                switch (mouseOn)
                {
                    case 0:
                        if (ImageRect.Contains(pt))
                        {
                            if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Move, layers[layerIndex],
                                ToImagePoint(pt), pt, true, e.Button, ModifierKeys.HasFlag(Keys.Control)));
                        }
                        else if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Move, layers[layerIndex],
                                ToImagePoint(pt), pt, false, e.Button, ModifierKeys.HasFlag(Keys.Control)));
                        break;
                    case 1:
                        rulerPosition = new PointF(pt.X - lastPt.X, pt.Y - lastPt.Y);
                        Invalidate();
                        break;
                    case 2:
                        rulerWidth = lastW + pt.X - lastPt.X > 55f ? lastW + pt.X - lastPt.X : 55f;
                        Invalidate();
                        break;
                }
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left or MouseButtons.Right && mouseOn is 0)
            {
                Point pt = PointToClient(Cursor.Position);
                if (ImageRect.Contains(pt))
                {
                    if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Up, layers[layerIndex],
                        ToImagePoint(pt), pt, true, e.Button, ModifierKeys.HasFlag(Keys.Control)));
                }
                else if (BrushPressed is not null) BrushPressed(this, new BrushPressedEventArgs(BrushPressedAction.Up, layers[layerIndex],
                        ToImagePoint(pt), pt, false, e.Button, ModifierKeys.HasFlag(Keys.Control)));
            }
            base.OnMouseUp(e);
        }
        public event BrushPressedEventHandler BrushPressed;
        public PointF ToImagePoint(PointF pt)
        {
            RectangleF r = ImageRect;
            return new PointF((pt.X - r.Left) / r.Width * ImageSize.Width, (pt.Y - r.Top) / r.Height * ImageSize.Height);
        }
        public double ScaleToImageX(double x)
        {
            return x / 2 * ImageRect.Width / ImageSize.Width;
        }
        public RectangleF ToImageRectangle(RectangleF r)
        {
            PointF p1 = ToImagePoint(r.Location);
            PointF p2 = ToImagePoint(new PointF(r.Right, r.Bottom));
            return RectangleF.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
        }
        public PointF ToControlPoint(PointF pt)
        {
            RectangleF r = ImageRect;
            return new PointF(pt.X / ImageSize.Width * r.Width + r.Left, pt.Y / ImageSize.Height * r.Height + r.Top);
        }
        public RectangleF ToControlRectangle(RectangleF r)
        {
            PointF p1 = ToControlPoint(r.Location);
            PointF p2 = ToControlPoint(new PointF(r.Right, r.Bottom));
            return RectangleF.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
        }
        public void InvalidateImage(RectangleF r)
        {
            RectangleF re = ToControlRectangle(r.Padded(2f)).InflateToRectangle();
            Region rgn = new Region(re);
            if (TileMode) foreach (RectangleF rs in Extensions.SurroundRectangle(new RectangleF(PointF.Empty, ImageRect.Size)))
                    rgn.Union(new RectangleF(re.X + rs.X, re.Y + rs.Y, re.Width, re.Height).InflateToRectangle());
            Invalidate(rgn);
            rgn.Dispose();
        }
        public void InvalidateImage(Rectangle r)
        {
            RectangleF re = ToControlRectangle(r.Padded(2)).InflateToRectangle();
            Region rgn = new Region(re);
            if (TileMode) foreach (RectangleF rs in Extensions.SurroundRectangle(new RectangleF(PointF.Empty, ImageRect.Size)))
                    rgn.Union(new RectangleF(re.X + rs.X, re.Y + rs.Y, re.Width, re.Height).InflateToRectangle());
            Invalidate(rgn);
            rgn.Dispose();
        }
        public void InvalidateImage()
        {
            Invalidate(FullImageRect.Padded(2f).InflateToRectangle());
        }
        private bool tileMode;
        public bool TileMode
        {
            get => tileMode;
            set
            {
                tileMode = value;
                if (TileMode) Invalidate(FullImageRect.Padded(2f).InflateToRectangle());
                else Invalidate();
                if (TileModeChanged is not null) TileModeChanged(this, new EventArgs());
            }
        }
        public event EventHandler TileModeChanged;
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr hIcon);
        private Cursor cursor;
        private Bitmap cursorBmp;
        private Brush selectionBrush;
        private Brush buttonBackgroundBrush;
        private Pen buttonBorderPen;
        private Pen textPen;
        private Pen textUnsavedPen;
        private Pen pixelGridPen;
        private IntPtr? cursorPtr;
        public static double CursorMaxRadius { get; set; } = 0.5d;
        public void SetCursorSize(int radius)
        {
            if (radius > CursorMaxRadius * ClientSize.Width) radius = (int)(CursorMaxRadius * ClientSize.Width);
            if (radius is >= 1 and < 8)
            {
                Cursor.Hide();
                Cursor = Cursors.Default;
                if (cursorPtr.HasValue) DestroyIcon(cursorPtr.Value);
                if (cursor is not null) cursor.Dispose();
                if (cursorBmp is not null) cursorBmp.Dispose();
                cursorBmp = new Bitmap(16, 16);
                Graphics g = Graphics.FromImage(cursorBmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLine(cursorBlack, 0f, 7.5f, 15f, 7.5f);
                g.DrawLine(cursorBlack, 7.5f, 0f, 7.5f, 15f);
                g.DrawLine(Pens.White, 2f, 7.5f, 13f, 7.5f);
                g.DrawLine(Pens.White, 7.5f, 2f, 7.5f, 13f);
                g.Dispose();
                cursorPtr = cursorBmp.GetHicon();
                cursor = new Cursor(cursorPtr.Value);
                Cursor = cursor;
                Cursor.Show();
            }
            else
            {
                if (radius is < 8) radius = 8;
                Cursor.Hide();
                Cursor = Cursors.Default;
                if (cursorPtr.HasValue) DestroyIcon(cursorPtr.Value);
                if (cursor is not null) cursor.Dispose();
                if (cursorBmp is not null) cursorBmp.Dispose();
                cursorBmp = new Bitmap(radius * 2, radius * 2);
                Graphics g = Graphics.FromImage(cursorBmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawArc(cursorBlack, new RectangleF(2.5f, 2.5f, radius * 2f - 6f, radius * 2f - 6f), 0f, 360f);
                g.DrawArc(Pens.White, new RectangleF(2.5f, 2.5f, radius * 2f - 6f, radius * 2f - 6f), 0f, 360f);
                g.Dispose();
                cursorPtr = cursorBmp.GetHicon();
                cursor = new Cursor(cursorPtr.Value);
                Cursor = cursor;
                Cursor.Show();
            }
        }

        private bool showRuler;
        public bool ShowRuler
        {
            get => showRuler;
            set
            {
                showRuler = value;
                Invalidate();
            }
        }
        private float rulerWidth;
        private PointF rulerPosition;
        private bool showProtractor;
        public bool ShowProtractor
        {
            get => showProtractor;
            set
            {
                showProtractor = value;
                Invalidate();
            }
        }
        private float protractorWidth;
        private PointF protractorPosition;
        public void DestroyCustomCursor()
        {
            Cursor = Cursors.Default;
            if (cursorPtr.HasValue)
            {
                DestroyIcon(cursorPtr.Value);
                cursorPtr = null;
            }
            if (cursor is not null)
            {
                cursor.Dispose();
                cursor = null;
            }
            if (cursorBmp is not null)
            {
                cursorBmp.Dispose();
                cursorBmp = null;
            }
        }
        public RectangleF ImageRect
        {
            get
            {
                Size sz = ClientSize;
                double w;
                double h;
                if (!IsHeightLimiting)
                {
                    w = sz.Width * 0.95d * viewZoom;
                    h = sz.Width / ImageAspectRatio * 0.95d * viewZoom;
                }
                else
                {
                    w = sz.Height * ImageAspectRatio * 0.95d * viewZoom;
                    h = sz.Height * 0.95d * viewZoom;
                }
                return new RectangleF((float)((sz.Width - w) / 2d - viewOffset.X), (float)((sz.Height - h) / 2d - viewOffset.Y), (float)w, (float)h);
            }
        }
        public RectangleF FullImageRect
        {
            get
            {
                Size sz = ClientSize;
                double w;
                double h;
                if (!IsHeightLimiting)
                {
                    w = sz.Width * 0.95d * viewZoom;
                    h = sz.Width / ImageAspectRatio * 0.95d * viewZoom;
                }
                else
                {
                    w = sz.Height * ImageAspectRatio * 0.95d * viewZoom;
                    h = sz.Height * 0.95d * viewZoom;
                }
                RectangleF r = new RectangleF((float)((sz.Width - w) / 2d - viewOffset.X), (float)((sz.Height - h) / 2d - viewOffset.Y), (float)w, (float)h);
                return TileMode ? RectangleF.FromLTRB(r.Left - r.Width, r.Top - r.Height, r.Width * 3f, r.Height * 3f) : r;
            }
        }
        private bool showPixelGrid;
        public bool ShowPixelGrid
        {
            get => showPixelGrid;
            set
            {
                showPixelGrid = value;
                Invalidate(ImageRect.InflateToRectangle());
            }
        }
        /*public Bitmap CreateSingleImage()
        {
            Bitmap output = new Bitmap(ImageSize.Width, ImageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            foreach (DrawingLayer l in layers)
            {
                if (!l.Visible) continue;
                Bitmap old = output;
                output = BlendMixing.Blend(l.BlendMode, l.Bitmap, output, l.LayerOpacity);
                old.Dispose();
            }
            return output;
        }*/
        public void CreateSingleImage(Rectangle? r = null)
        {
            singleImage.Clear(Color.Transparent, r ?? new Rectangle(Point.Empty, ImageSize));
            foreach(DrawingLayer l in layers)
            {
                if (!l.Visible) continue;
                BlendMixing.Blend(l.BlendMode, l.Bitmap, singleImage, l.LayerOpacity, l.ImageEdits, r);
            }
        }
        private Bitmap singleImage;
        public void InvalidateDontComposite()
        {
            suppressCompositing = true;
            Invalidate();
            suppressCompositing = false;
        }
        public void InvalidateDontComposite(Region rgn)
        {
            suppressCompositing = true;
            Invalidate(rgn);
            suppressCompositing = false;
        }
        public void InvalidateDontComposite(Rectangle r)
        {
            suppressCompositing = true;
            Invalidate(r);
            suppressCompositing = false;
        }
        private bool suppressCompositing;
        protected override void OnPaint(PaintEventArgs e)
        {
            RectangleF r = ImageRect;
            e.Graphics.FillRectangle(Transparent, r);

            if(!suppressCompositing) CreateSingleImage(ToImageRectangle(e.ClipRectangle).InflateToRectangle().Padded(2).Constrain(Point.Empty, ImageSize));

            if (ImageRect.Width / ImageSize.Width > 4f && ImageRect.Height / ImageSize.Height > 4f)
            {
                InterpolationMode m = e.Graphics.InterpolationMode;
                PixelOffsetMode p = e.Graphics.PixelOffsetMode;

                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                float widthOffset = ImageRect.Width / ImageSize.Width / 2f;
                float heightOffset = ImageRect.Height / ImageSize.Height / 2f;
                e.Graphics.DrawImage(singleImage, r);
                if (TileMode)
                {
                    foreach (RectangleF re in Extensions.SurroundRectangle(ImageRect))
                    {
                        e.Graphics.DrawImage(singleImage, re);
                        Brush b = new SolidBrush(Color.FromArgb(64, Styling.BackgroundColor));
                        e.Graphics.FillRectangle(b, re);
                        b.Dispose();
                    }
                }
                e.Graphics.InterpolationMode = m;
                e.Graphics.PixelOffsetMode = p;
            }
            else
            {
                e.Graphics.DrawImage(singleImage, r);
                if (TileMode)
                {
                    foreach (RectangleF re in Extensions.SurroundRectangle(ImageRect))
                    {
                        e.Graphics.DrawImage(singleImage, re);
                        Brush b = new SolidBrush(Color.FromArgb(64, Styling.BackgroundColor));
                        e.Graphics.FillRectangle(b, re);
                        b.Dispose();
                    }
                }
            }
            if (!Editor.SelectionRegion.IsEmpty(e.Graphics))
            {
                Region rgn = new Region(Rectangle.Empty);
                rgn.Union(Editor.SelectionRegion);
                Matrix m = new();
                m.Translate(ImageRect.Left, ImageRect.Top);
                m.Scale(ImageRect.Width / ImageSize.Width, ImageRect.Height / ImageSize.Height);
                rgn.Transform(m);
                e.Graphics.FillRegion(selectionBrush, rgn);
                rgn.Dispose();
                m.Dispose();
            }
            if (ShowPixelGrid && ImageRect.Width / ImageSize.Width > 4f && ImageRect.Height / ImageSize.Height > 4f)
            {
                if (TileMode)
                {
                    RectangleF[] rs = Extensions.SurroundRectangle(ImageRect);
                    Color[] cs = Styling.GetTileModeGridColors();
                    for(int su = 0; su < rs.Length; su++)
                    {
                        Pen p = new Pen(Color.FromArgb(255, cs[su]), pixelGridPen.Width);
                        p.DashStyle = DashStyle.Dot;
                        p.DashPattern = new float[] { 2f, 4f };
                        for (int i = 1; i < ImageSize.Width; i++)
                        {
                            PointF pt1 = ToControlPoint(new PointF(i, 0f));
                            PointF pt2 = ToControlPoint(new PointF(i, ImageSize.Height));
                            e.Graphics.DrawLine(p, new PointF(pt1.X + r.Left - rs[su].Left, pt1.Y + r.Top - rs[su].Top),
                                new PointF(pt2.X + r.Left - rs[su].Left, pt2.Y + r.Top - rs[su].Top));
                        }
                        for (int i = 1; i < ImageSize.Height; i++)
                        {
                            PointF pt1 = ToControlPoint(new PointF(0f, i));
                            PointF pt2 = ToControlPoint(new PointF(ImageSize.Width, i));
                            e.Graphics.DrawLine(p, new PointF(pt1.X + r.Left - rs[su].Left, pt1.Y + r.Top - rs[su].Top),
                                new PointF(pt2.X + r.Left - rs[su].Left, pt2.Y + r.Top - rs[su].Top));
                        }
                        e.Graphics.DrawRectangle(p, rs[su]);
                        p.Dispose();
                    }
                }
                for (int i = 1; i < ImageSize.Width; i++)
                {
                    e.Graphics.DrawLine(pixelGridPen, ToControlPoint(new PointF(i, 0f)), ToControlPoint(new PointF(i, ImageSize.Height)));
                }
                for (int i = 1; i < ImageSize.Height; i++)
                {
                    e.Graphics.DrawLine(pixelGridPen, ToControlPoint(new PointF(0f, i)), ToControlPoint(new PointF(ImageSize.Width, i)));
                }
            }
            if (TileMode)
            {
                Pen p = new Pen(Color.Gray, 2f);
                e.Graphics.DrawRectangle(p, r.Left - 1, r.Top - 1, r.Width, r.Height);
                p.Dispose();
            }
            e.Graphics.DrawRectangle(Pens.LightGray, r.Left - 1, r.Top - 1, r.Width, r.Height);

            if (showProtractor)
            {
                e.Graphics.FillRectangle(buttonBackgroundBrush, new RectangleF(protractorPosition, new SizeF(protractorWidth, protractorWidth)));
            }
            if (showRuler)
            {
                e.Graphics.FillRectangle(buttonBackgroundBrush, RulerRect);
                e.Graphics.DrawRectangle(buttonBorderPen, RulerRect);
                float dpiX = e.Graphics.DpiX;
                for (int i = 0; i < rulerWidth * 4f / dpiX + 1f; i++)
                {
                    if (i % 4 == 0)
                    {
                        e.Graphics.DrawLine(textPen, rulerPosition.X + i / 4f * dpiX, rulerPosition.Y + 30f, rulerPosition.X + i / 4f * dpiX,
                        rulerPosition.Y + 55f);
                        if (i > 0 && i < rulerWidth * 4f / dpiX)
                            e.Graphics.DrawString((i / 4).ToString(), Font, textPen.Brush, rulerPosition.X + i / 4f * dpiX,
                                RulerRect.Bottom - e.Graphics.MeasureString((i / 4).ToString(), Font).Height);
                    }
                    else e.Graphics.DrawLine(textPen, rulerPosition.X + i / 4f * dpiX, rulerPosition.Y + 40f, rulerPosition.X + i / 4f * dpiX, rulerPosition.Y + 55f);
                }
                float dpcX = e.Graphics.DpiX / 2.54f;
                for (int i = 0; i < rulerWidth * 2f / dpcX + 1f; i++)
                {
                    if (i % 2 == 0)
                    {
                        e.Graphics.DrawLine(textUnsavedPen, rulerPosition.X + i / 2f * dpcX, rulerPosition.Y, rulerPosition.X + i / 2f * dpcX,
                         rulerPosition.Y + 20f);
                        if(i > 0 && i < rulerWidth * 2f / dpcX)
                            e.Graphics.DrawString((i / 2).ToString(), Font, textUnsavedPen.Brush, rulerPosition.X + i / 2f * dpcX, rulerPosition.Y);
                    }
                    else e.Graphics.DrawLine(textUnsavedPen, rulerPosition.X + i / 2f * dpcX, rulerPosition.Y, rulerPosition.X + i / 2f * dpcX, rulerPosition.Y + 10f);
                }
                e.Graphics.DrawString("in", Font, textPen.Brush, rulerPosition.X, RulerRect.Bottom - e.Graphics.MeasureString("in", Font).Height);
                e.Graphics.DrawString("cm", Font, textUnsavedPen.Brush, rulerPosition.X, rulerPosition.Y);
                e.Graphics.DrawEllipse(textPen, new RectangleF(RulerRect.Right - 5f, RulerRect.Top + RulerRect.Height / 2f - 5f, 10f, 10f));
            }
        }
        private RectangleF RulerRect { get => new RectangleF(rulerPosition, new SizeF(rulerWidth, 55f)); }
        private int layerIndex;
        public int SelectedLayer
        {
            get => layerIndex;
            set
            {
                if (value < 0) throw new ArgumentException("Must be >= 0.", nameof(value));
                layerIndex = value;
                if (SelectedLayerChanged is not null) SelectedLayerChanged(this, new EventArgs());
            }
        }
        public event EventHandler SelectedLayerChanged;
        private Size imageSize = new Size(800, 600);
        public Size ImageSize
        {
            get => imageSize;
            set
            {
                if (value.Width * value.Height <= 0) throw new ArgumentException("Area must be > 0.", nameof(value));
                if (layers.Count > 0) throw new InvalidOperationException("Image currently has layers; use SetImageSize method instead.");
                imageSize = value;
                Bitmap old = singleImage;
                singleImage = new Bitmap(imageSize.Width, imageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                old.Dispose();
                if (ImageSizeChanged is not null) ImageSizeChanged(this, new EventArgs());
            }
        }
        public void SetImageSize(Size size, System.Drawing.Drawing2D.InterpolationMode method = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic)
        {
            if (size.Width * size.Height <= 0) throw new ArgumentException("Area must be > 0.", nameof(size));
            imageSize = size;
            List<DrawingLayer> ls = new List<DrawingLayer>(layers);
            layers.Clear();
            while(ls.Count > 0)
            {
                Bitmap n = Extensions.GetResizedImage(ls[0].Bitmap, size.Width, size.Height, method);
                DrawingLayer d = new DrawingLayer(ls[0], n);
                d.AppearanceModified += LayerAppearanceModified;
                d.VisibleChanged += LayerVisibleChanged;
                layers.Add(d);
                DrawingLayer l = ls[0];
                ls.RemoveAt(0);
                l.AppearanceModified -= LayerAppearanceModified;
                d.VisibleChanged -= LayerVisibleChanged;
                l.Dispose();
            }
            singleImage = new Bitmap(imageSize.Width, imageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            CreateSingleImage();
            Invalidate();
            if (ImageSizeChanged is not null) ImageSizeChanged(this, new EventArgs());
            if (LayersModified is not null) LayersModified(this, new EventArgs());
        }

        private void LayerVisibleChanged(object sender, EventArgs e)
        {
            if (Editor.LayerEditor is not null && sender is DrawingLayer l) Editor.LayerEditor.InvalidateVisibleButton(layers.IndexOf(l));
        }

        public event EventHandler ImageSizeChanged;
        public bool IsHeightLimiting { get => ImageAspectRatio <= (double)ClientSize.Width / ClientSize.Height; }
        public double ImageAspectRatio { get => (double)ImageSize.Width / ImageSize.Height; }
        private double viewZoom = 1d;
        public double ViewZoom {
            get => viewZoom;
            set
            {
                if (value <= 0d) throw new ArgumentException("Must be > 0.", nameof(value));
                viewZoom = value;
                Invalidate();
                if (ViewZoomChanged is not null) ViewZoomChanged(this, new EventArgs());
            }
        }
        public event EventHandler ViewZoomChanged;
        private PointD viewOffset;
        public PointD ViewOffset
        {
            get => viewOffset;
            set
            {
                viewOffset = value;
                Invalidate();
                if (ViewOffsetChanged is not null) ViewOffsetChanged(this, new EventArgs());
            }
        }
        public event EventHandler ViewOffsetChanged;
        private List<DrawingLayer> layers = new();
        public void AddLayer(DrawingLayer l, bool invalidate = true)
        {
            if (l is null) throw new ArgumentNullException("Layer cannot be null.", nameof(l));
            if (l.Bitmap.Size != ImageSize) throw new ArgumentException("Layer image must match ImageSize.", nameof(l));
            l.AppearanceModified += LayerAppearanceModified;
            l.VisibleChanged += LayerVisibleChanged;
            layers.Add(l);
            if(invalidate) Invalidate();
            if (LayersModified is not null) LayersModified(this, new EventArgs());
        }

        private void LayerAppearanceModified(object sender, DrawingLayerAppearanceModifiedEventArgs e)
        {
            CreateSingleImage(e.Region.GetBounds((sender as DrawingLayer).Graphics).InflateToRectangle());
            Invalidate();
        }

        public bool RemoveLayer(DrawingLayer l)
        {
            if (l is null) throw new ArgumentNullException("Layer cannot be null.", nameof(l));
            bool s = layers.Remove(l);
            if (s)
            {
                l.AppearanceModified -= LayerAppearanceModified;
                l.VisibleChanged -= LayerVisibleChanged;
            }
            Invalidate();
            if (LayersModified is not null) LayersModified(this, new EventArgs());
            return s;
        }
        public void InsertLayer(int i, DrawingLayer l)
        {
            if (l is null) throw new ArgumentNullException("Layer cannot be null.", nameof(l));
            if (l.Bitmap.Size != ImageSize) throw new ArgumentException("Layer image must match ImageSize.", nameof(l));
            l.AppearanceModified += LayerAppearanceModified;
            l.VisibleChanged += LayerVisibleChanged;
            layers.Insert(i, l);
            Invalidate();
            if (LayersModified is not null) LayersModified(this, new EventArgs());
        }
        public DrawingLayer LayerAtIndex(int i) => layers[i];
        public int FindLayerIndex(Predicate<DrawingLayer> p)
        {
            if (p is null) throw new ArgumentNullException("Predicate cannot be null.", nameof(p));
            return layers.FindIndex(p);
        }
        public int FindLayerIndex(int start, Predicate<DrawingLayer> p)
        {
            if (p is null) throw new ArgumentNullException("Predicate cannot be null.", nameof(p));
            return layers.FindIndex(start, p);
        }
        public int LayerCount { get => layers.Count; }
        public System.Collections.ObjectModel.ReadOnlyCollection<DrawingLayer> Layers => layers.AsReadOnly();
        public event EventHandler LayersModified;
        public void DisposeLayers(bool invalidate = true)
        {
            while(layers.Count > 0)
            {
                DrawingLayer l = layers[0];
                l.AppearanceModified -= LayerAppearanceModified;
                l.VisibleChanged -= LayerVisibleChanged;
                layers.RemoveAt(0);
                l.Dispose();
            }
            if(invalidate) Invalidate();
            if (LayersModified is not null) LayersModified(this, new EventArgs());
        }


        private static TextureBrush transparent = new TextureBrush(Image.FromFile(System.IO.Path.Combine("Image", "Transparent.png")));
        public static TextureBrush Transparent { get => transparent; }
        private static Pen cursorBlack = new Pen(Color.FromArgb(30, 30, 30), 5f);
    }
    public class DrawingLayer : IDisposable
    {
        public DrawingLayer(Bitmap bmp, string name = null)
        {
            if (name is null) name = Properties.Strings.GenericLayerName;
            if (bmp is null) throw new ArgumentNullException("Cannot be null.", nameof(bmp));
            bitmap = bmp;
            graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.name = name;
        }
        public DrawingLayer(DrawingLayer l, Bitmap bmp, string name = null)
        {
            if (name is null) name = Properties.Strings.GenericLayerName;
            if (bmp is null) throw new ArgumentNullException("Cannot be null.", nameof(bmp));
            bitmap = bmp;
            visible = l.Visible;
            blendMode = l.BlendMode;
            layerOpacity = l.LayerOpacity;
            graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.name = name;
        }
        private Bitmap bitmap;
        public Bitmap Bitmap { get => bitmap; }
        private Graphics graphics;
        public Graphics Graphics { get => graphics; }
        private bool visible = true;
        public Rectangle ImageRect { get => new Rectangle(Point.Empty, Bitmap.Size); }
        private BlendMode blendMode = BlendMode.Standard;
        public BlendMode BlendMode
        {
            get => blendMode;
            set
            {
                blendMode = value;
                if (BlendModeChanged is not null) BlendModeChanged(this, new EventArgs());
                FireAppearanceModified();
            }
        }
        public event EventHandler BlendModeChanged;
        private double layerOpacity = 1d;
        public double LayerOpacity
        {
            get => layerOpacity;
            set
            {
                if (value is < 0d or > 1d) throw new ArgumentException("Opacity must be within 0-1 inclusive.", nameof(value));
                layerOpacity = value;
                if (LayerOpacityChanged is not null) LayerOpacityChanged(this, new EventArgs());
                FireAppearanceModified();
            }
        }
        public event EventHandler LayerOpacityChanged;
        private string name = Properties.Strings.GenericLayerName;
        public string Name
        {
            get => name;
            set
            {
                if (value is null) throw new ArgumentNullException("", nameof(value));
                name = value;
                if (NameChanged is not null) NameChanged(this, new EventArgs());
            }
        }
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                if (VisibleChanged is not null) VisibleChanged(this, new EventArgs());
                FireAppearanceModified();
            }
        }
        protected void FireAppearanceModified()
        {
            Region rgn = new Region(new Rectangle(Point.Empty, Bitmap.Size));
            if (AppearanceModified is not null) AppearanceModified(this, new DrawingLayerAppearanceModifiedEventArgs(rgn));
            rgn.Dispose();
        }
        protected void FireAppearanceModified(Region rgn)
        {
            if (AppearanceModified is not null) AppearanceModified(this, new DrawingLayerAppearanceModifiedEventArgs(rgn));
        }
        protected void FireAppearanceModified(RectangleF r)
        {
            Region rgn = new Region(r);
            if (AppearanceModified is not null) AppearanceModified(this, new DrawingLayerAppearanceModifiedEventArgs(rgn));
            rgn.Dispose();
        }
        protected void FireAppearanceModified(Rectangle r)
        {
            Region rgn = new Region(r);
            if (AppearanceModified is not null) AppearanceModified(this, new DrawingLayerAppearanceModifiedEventArgs(rgn));
            rgn.Dispose();
        }
        public event EventHandler VisibleChanged;
        public event EventHandler NameChanged;
        public event DrawingLayerAppearanceModifiedEventHandler AppearanceModified;

        public void Dispose()
        {
            bitmap.Dispose();
            graphics.Dispose();
        }
        private ImageEdits edits = new ImageEdits();
        public ImageEdits ImageEdits
        {
            get => edits;
            set
            {
                if (value is null) throw new ArgumentNullException("", nameof(value));
                edits = value;
                if (ImageEditsChanged is not null) ImageEditsChanged(this, new EventArgs());
                FireAppearanceModified();
            }
        }
        public event EventHandler ImageEditsChanged;
    }
    public delegate void DrawingLayerAppearanceModifiedEventHandler(object sender, DrawingLayerAppearanceModifiedEventArgs e);
    public class DrawingLayerAppearanceModifiedEventArgs : EventArgs
    {
        public DrawingLayerAppearanceModifiedEventArgs(Region rgn) : base()
        {
            Region = rgn;
        }
        public Region Region { get; }
    }
    public struct PointD : IEquatable<PointD>
    {
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X { readonly get; set; }
        public double Y { readonly get; set; }
        public bool IsEmpty { get => X is 0 && Y is 0; }

        public override bool Equals(object obj)
        {
            return obj is PointD d && Equals(d);
        }

        public bool Equals(PointD other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public PointF ToPointF() => new PointF((float)X, (float)Y);
        public static bool operator ==(PointD l, PointD r) => l.X == r.X && l.Y == r.Y;
        public static bool operator !=(PointD l, PointD r) => l.X != r.X || l.Y != r.Y;
        public static PointD operator +(PointD l, PointD r) => new PointD(l.X + r.X, l.Y + r.Y);
        public static PointD operator +(PointD l, Size r) => new PointD(l.X + r.Width, l.Y + r.Height);
        public static PointD operator +(PointD l, SizeF r) => new PointD(l.X + r.Width, l.Y + r.Height);
        public static PointD operator -(PointD l, PointD r) => new PointD(l.X - r.X, l.Y - r.Y);
        public static PointD operator -(PointD l, Size r) => new PointD(l.X - r.Width, l.Y - r.Height);
        public static PointD operator -(PointD l, SizeF r) => new PointD(l.X - r.Width, l.Y - r.Height);
        public static explicit operator PointF(PointD pt) => pt.ToPointF();
        public static explicit operator PointD(Point pt) => new PointD(pt.X, pt.Y);
        public static explicit operator PointD(PointF pt) => new PointD(pt.X, pt.Y);
        public static readonly PointD Empty = new PointD(0d, 0d);
    }
    public delegate void BrushPressedEventHandler(object sender, BrushPressedEventArgs e);
    public class BrushPressedEventArgs : EventArgs
    {
        public BrushPressedEventArgs(BrushPressedAction a, DrawingLayer l, PointF location, PointF viewportLocation, bool inImage,
            MouseButtons mouseButtons, bool controlDown = false)
        {
            ActionType = a;
            Layer = l;
            Location = location;
            ViewportLocation = viewportLocation;
            InImage = inImage;
            ControlDown = controlDown;
            MouseButtons = mouseButtons;
        }
        public BrushPressedAction ActionType { get; }
        public DrawingLayer Layer { get; }
        public PointF Location { get; }
        public PointF ViewportLocation { get; }
        public bool InImage { get; }
        public bool ControlDown { get; }
        public MouseButtons MouseButtons { get; }
    }
    public enum BrushPressedAction
    {
        Down,
        Move,
        Up
    }
}
