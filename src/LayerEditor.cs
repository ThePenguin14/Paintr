using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paintr
{
    public partial class LayerEditor : Control
    {
        public LayerEditor()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            LoadIcons();
            InitializeComponent();
            Disposed += LayerEditor_Disposed;
        }
        public void DisposeIcons()
        {
            if (visibleIcon is not null)
            {
                visibleIcon.Dispose();
                visibleIcon = null;
            }
            if (invisibleIcon is not null)
            {
                invisibleIcon.Dispose();
                invisibleIcon = null;
            }
            if (propertiesIcon is not null)
            {
                propertiesIcon.Dispose();
                propertiesIcon = null;
            }
            if (addIcon is not null)
            {
                addIcon.Dispose();
                addIcon = null;
            }
        }
        public void LoadIcons()
        {
            visibleIcon = Styling.ColorImage(Styling.VisibleIconPath, Styling.ContrastBackgroundColor);
            invisibleIcon = Styling.ColorImage(Styling.InvisibleIconPath, Styling.ContrastBackgroundColor);
            propertiesIcon = Styling.ColorImage(Styling.PropertiesIconPath, Styling.TextUnsavedColor);
            addIcon = Styling.ColorImage(Styling.AddIconPath, Styling.ContrastBackgroundDownColor.OfHue(120d));
        }
        public void ResetIcons()
        {
            DisposeIcons();
            LoadIcons();
        }
        public struct MouseRectangle : IEquatable<MouseRectangle>
        {
            public MouseRectangle(ushort place, ushort layer)
            {
                Place = place;
                Layer = layer;
            }
            public ushort Place { get; }
            public ushort Layer { get; }

            public override bool Equals(object obj)
            {
                return obj is MouseRectangle rectangle && Equals(rectangle);
            }

            public bool Equals(MouseRectangle other)
            {
                return Place == other.Place &&
                       Layer == other.Layer;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Place, Layer);
            }

            public static bool operator ==(MouseRectangle left, MouseRectangle right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(MouseRectangle left, MouseRectangle right)
            {
                return !(left == right);
            }
        }
        private MouseRectangle mouseRect;
        public MouseRectangle RectangleOfPoint(Point pt)
        {
            if (clipY is 0 || Editor.Viewport is null) return default;
            Size sz = ClientSize;
            float h = MathF.Min(LayerBoxHeight * sz.Height, 40f);
            if(pt.Y >= sz.Height - 33 && pt.Y < sz.Height - 3)
            {
                if (pt.X <= 30) return new MouseRectangle(4, 0);
                else return default;
            }
            else if (pt.Y > Editor.Viewport.LayerCount * h + clipY || pt.Y < clipY) return default;
            else if (pt.X >= sz.Width - h) return new MouseRectangle(2, (ushort)MathF.Floor((pt.Y - clipY) / h));
            else if (pt.X >= sz.Width - h * 2f) return new MouseRectangle(3, (ushort)MathF.Floor((pt.Y - clipY) / h));
            else return new MouseRectangle(1, (ushort)MathF.Floor((pt.Y - clipY) / h));
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point pt = PointToClient(Cursor.Position);
            MouseRectangle r = RectangleOfPoint(pt);
            if (mouseRect != r)
            {
                mouseRect = r;
                Invalidate();
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (mouseRect.Place is not 0)
            {
                mouseRect = default;
                Invalidate();
            }
            base.OnMouseLeave(e);
        }
        protected override void OnClick(EventArgs e)
        {
            switch (mouseRect.Place)
            {
                case 1:
                    if (Editor.Viewport is not null)
                    {
                        Editor.Viewport.SelectedLayer = Editor.Viewport.LayerCount - mouseRect.Layer - 1;
                        Invalidate();
                    }
                    break;
                case 2:
                    if (Editor.Viewport is not null) {
                        DrawingLayer l = Editor.Viewport.LayerAtIndex(Editor.Viewport.LayerCount - mouseRect.Layer - 1);
                        l.Visible = !l.Visible;
                        Invalidate();
                    }
                    break;
                case 3:
                    if (Editor.Viewport is not null)
                    {
                        DrawingLayer l = Editor.Viewport.LayerAtIndex(Editor.Viewport.LayerCount - mouseRect.Layer - 1);
                        Dialog.LayerProperties lp = new Dialog.LayerProperties() { SelectedLayer = l };
                        lp.ShowDialog(FindForm());
                        lp.Dispose();
                    }
                    break;
                case 4:
                    if (Editor.Viewport is not null)
                    {
                        Editor.Viewport.AddLayer(new DrawingLayer(new Bitmap(Editor.Viewport.ImageSize.Width, Editor.Viewport.ImageSize.Height),
                            string.Format(Properties.Strings.DefaultLayerName, Editor.Viewport.LayerCount + 1)));
                        Editor.Viewport.SelectedLayer = Editor.Viewport.LayerCount - 1;
                        Invalidate();
                    }
                    break;
            }
            base.OnClick(e);
        }
        private void LayerEditor_Disposed(object sender, EventArgs e)
        {
            DisposeIcons();
        }
        public void InvalidateVisibleButton(int index)
        {
            Size sz = ClientSize;
            float h = MathF.Min(LayerBoxHeight * sz.Height, 40f);
            Invalidate(new RectangleF(sz.Width - h, index * h + clipY, h, h).InflateToRectangle());
        }
        public Rectangle VisibleButtonRect(int index)
        {
            Size sz = ClientSize;
            float h = MathF.Min(LayerBoxHeight * sz.Height, 40f);
            return new RectangleF(sz.Width - h, index * h + clipY, h, h).InflateToRectangle();
        }
        public Rectangle PropertiesButtonRect(int index)
        {
            Size sz = ClientSize;
            float h = MathF.Min(LayerBoxHeight * sz.Height, 40f);
            return new RectangleF(sz.Width - h * 2f, index * h + clipY, h, h).InflateToRectangle();
        }
        public void InvalidateLayerRow(int index)
        {
            Size sz = ClientSize;
            float h = MathF.Min(LayerBoxHeight * sz.Height, 40f);
            Invalidate(new RectangleF(0f, index * h + clipY, sz.Width, h).InflateToRectangle());
        }
        public Rectangle LayerRowRect(int index)
        {
            Size sz = ClientSize;
            float h = MathF.Min(LayerBoxHeight * sz.Height, 40f);
            return new RectangleF(0f, index * h + clipY, sz.Width, h).InflateToRectangle();
        }
        public Rectangle MouseRectangleRect(MouseRectangle r)
        {
            switch (r.Place)
            {
                case 1:
                    return LayerRowRect(r.Layer);
                case 2:
                    return VisibleButtonRect(r.Layer);
                case 3:
                    return PropertiesButtonRect(r.Layer);
                case 4:
                    return new Rectangle(0, ClientSize.Height - 33, 30, 30);
                default:
                    return default;
            }
        }
        private Bitmap visibleIcon;
        private Bitmap invisibleIcon;
        private Bitmap propertiesIcon;
        private Bitmap addIcon;
        private int clipY;
        protected override void OnPaint(PaintEventArgs e)
        {
            Size sz = ClientSize;
            SizeF labelSz = e.Graphics.MeasureString(Properties.Strings.LayersLabel, Font);
            using Brush greyBrush = new SolidBrush(Styling.TextUnsavedColor);
            using Pen greyPen = new Pen(greyBrush, 1f);
            using Brush textBrush = new SolidBrush(Styling.TextColor);
            using Brush back1 = new SolidBrush(Styling.BackgroundColor.GetStandOutColor(3));
            using Brush back2 = new SolidBrush(Styling.BackgroundColor.GetStandOutColor(6));
            using Brush hili = new SolidBrush(Styling.TextDisabledColor.GetStandOutColor(6));
            using StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawLine(greyPen, 0f, labelSz.Height / 2f + 1f, sz.Width / 8f, labelSz.Height / 2f + 1f);
            e.Graphics.DrawString(Properties.Strings.LayersLabel, Font, greyBrush, sz.Width / 8f, 1f);
            e.Graphics.DrawLine(greyPen, sz.Width / 8f - 2f + labelSz.Width, labelSz.Height / 2f + 1f, sz.Width, labelSz.Height / 2f + 1f);
            e.Graphics.DrawLine(greyPen, 0, sz.Height - 3, sz.Width, sz.Height - 3);
            clipY = (int)MathF.Ceiling(labelSz.Height) + 1;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (mouseRect.Place is 4) e.Graphics.FillRectangle(hili, new Rectangle(0, sz.Height - 33, 30, 30));
            e.Graphics.DrawImage(addIcon, new Rectangle(5, sz.Height - 27, 20, 20));

            e.Graphics.SetClip(new Rectangle(0, clipY, sz.Width, sz.Height - 3 - clipY - 30));
            if (Editor.Viewport is not null)
            {
                Point cursorPt = PointToClient(Cursor.Position);
                System.Collections.ObjectModel.ReadOnlyCollection<DrawingLayer> layers = Editor.Viewport.Layers;
                float h = MathF.Min(LayerBoxHeight * sz.Height, 40f);
                int c = layers.Count;
                for (int i = 0; i < c; i++)
                {
                    e.Graphics.FillRectangle(mouseRect.Place is 1 && mouseRect.Layer == i ? hili : i % 2 is 0 ? back1 : back2,
                        new RectangleF(0f, i * h + clipY, sz.Width, h));
                    if (mouseRect.Place is 3 && mouseRect.Layer == i) e.Graphics.FillRectangle(hili, new RectangleF(sz.Width - h * 2f, i * h + clipY, h, h));
                    e.Graphics.DrawImage(propertiesIcon, new RectangleF(sz.Width - h * 1.5f - 10f, i * h + clipY + h / 2f - 10f, 20f, 20f));
                    if (mouseRect.Place is 2 && mouseRect.Layer == i) e.Graphics.FillRectangle(hili, new RectangleF(sz.Width - h, i * h + clipY, h, h));
                    e.Graphics.DrawImage(layers[^(i + 1)].Visible ? visibleIcon : invisibleIcon,
                        new RectangleF(sz.Width - h / 2f - 10f, i * h + clipY + h / 2f - 10f, 20f, 20f));
                    e.Graphics.DrawString(layers[^(i + 1)].Name, Font, Editor.Viewport.SelectedLayer == c - i - 1 ? textBrush : greyBrush,
                        new RectangleF(3f, i * h + clipY, sz.Width - 30f, h), format);
                }
            }
            e.Graphics.ResetClip();
            base.OnPaint(e);
        }
        public static float LayerBoxHeight { get; set; } = 1f / 3f;
    }
}
