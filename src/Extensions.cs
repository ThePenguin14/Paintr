using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paintr
{
    public static class Extensions
    {
        public static float[] StringToFloatArray(string str)
        {
            if (str is null) throw new ArgumentNullException("", nameof(str));
            string[] s = str.Split(Styling.NumberComma);
            float[] t = new float[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                t[i] = float.Parse(s[i]);
            }
            return t;
        }
        public static bool TryStringToFloatArray(string str, out float[] result)
        {
            result = null;
            if (str is null) return false;
            string[] s = str.Split(Styling.NumberComma);
            float[] t = new float[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                if (float.TryParse(s[i], out float res)) t[i] = res;
                else return false;
            }
            result = t;
            return true;
        }
        public static double[] StringToDoubleArray(string str)
        {
            if (str is null) throw new ArgumentNullException("", nameof(str));
            string[] s = str.Split(Styling.NumberComma);
            double[] t = new double[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                t[i] = double.Parse(s[i]);
            }
            return t;
        }
        public static bool TryStringToDoubleArray(string str, out double[] result)
        {
            result = null;
            if (str is null) return false;
            string[] s = str.Split(Styling.NumberComma);
            double[] t = new double[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                if (double.TryParse(s[i], out double res)) t[i] = res;
                else return false;
            }
            result = t;
            return true;
        }
        public static int[] StringToIntArray(string str)
        {
            if (str is null) throw new ArgumentNullException("", nameof(str));
            string[] s = str.Split(Styling.NumberComma);
            int[] t = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                t[i] = int.Parse(s[i]);
            }
            return t;
        }
        public static bool TryStringToIntArray(string str, out int[] result)
        {
            result = null;
            if (str is null) return false;
            string[] s = str.Split(Styling.NumberComma);
            int[] t = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                if (int.TryParse(s[i], out int res)) t[i] = res;
                else return false;
            }
            result = t;
            return true;
        }
        public static System.Drawing.Color ColorFromHSL(double hue, double sat, double light)
        {
            if (sat is 0d) return System.Drawing.Color.FromArgb((int)light, (int)light, (int)light);
            double h = hue / 360d;
            double max = light < 0.5d ? light * (1 + sat) : (light + sat) - (light * sat);
            double min = (light * 2d) - max;
            return System.Drawing.Color.FromArgb((int)(255 * RGBChannelFromHue(min, max, h + 1 / 3d)), (int)(255 * RGBChannelFromHue(min, max, h)),
                (int)(255 * RGBChannelFromHue(min, max, h - 1 / 3d)));
        }
        /// <param name="alpha">A double 0 to 1 where 1 is opaque and 0 is transparent.</param>
        public static System.Drawing.Color ColorFromHSL(double alpha, double hue, double sat, double light)
        {
            if (sat is 0d) return System.Drawing.Color.FromArgb((int)(alpha * 255), (int)light, (int)light, (int)light);
            double h = hue / 360d;
            double max = light < 0.5d ? light * (1 + sat) : (light + sat) - (light * sat);
            double min = (light * 2d) - max;
            return System.Drawing.Color.FromArgb((int)(alpha * 255), (int)(255 * RGBChannelFromHue(min, max, h + 1 / 3d)), (int)(255 * RGBChannelFromHue(min, max, h)),
                (int)(255 * RGBChannelFromHue(min, max, h - 1 / 3d)));
        }
        public static double RGBChannelFromHue(double m1, double m2, double h)
        {
            h = (h + 1d) % 1d;
            if (h < 0) h += 1;
            if (h * 6 < 1) return m1 + (m2 - m1) * 6 * h;
            else if (h * 2 < 1) return m2;
            else if (h * 3 < 2) return m1 + (m2 - m1) * 6 * (2d / 3d - h);
            else return m1;

        }
        public static bool TryToHSL(this System.Drawing.Color col, out double hue, out double sat, out double light)
        {
            double r = (col.R / 255d);
            double g = (col.G / 255d);
            double b = (col.B / 255d);

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;

            double h = 0d;
            double s = 0d;
            double l = (double)((max + min) / 2.0f);

            if (delta != 0)
            {
                if (l < 0.5d)
                {
                    s = delta / (max + min);
                }
                else
                {
                    s = delta / (2.0d - max - min);
                }


                if (r == max)
                {
                    h = (g - b) / delta;
                }
                else if (g == max)
                {
                    h = 2d + (b - r) / delta;
                }
                else if (b == max)
                {
                    h = 4d + (r - g) / delta;
                }
            }
            if ((h *= 60d) < 0d) h += 360d;
            hue = h;
            sat = s;
            light = l;
            return true;
        }
        public static double GetDoubleHue(this System.Drawing.Color col)
        {
            double r = (col.R / 255d);
            double g = (col.G / 255d);
            double b = (col.B / 255d);

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;

            double hue = 0d;
            if (delta != 0)
            {
                if (r == max)
                {
                    hue = (g - b) / delta;
                }
                else if (g == max)
                {
                    hue = 2d + (b - r) / delta;
                }
                else if (b == max)
                {
                    hue = 4d + (r - g) / delta;
                }
            }
            if ((hue *= 60d) < 0d) hue += 360d;
            return hue;
        }
        public static double GetDoubleValue(this System.Drawing.Color color)
        {
            return Math.Min(color.R, Math.Min(color.G, color.B)) / 255d;
        }
        public static double GetDoubleSaturation(this System.Drawing.Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));
            return (max == 0) ? 0 : 1d - (1d * min / max);
        }
        public static bool TryToHSV(this System.Drawing.Color color, out double hue, out double sat, out double val)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            sat = (max == 0) ? 0 : 1d - (1d * min / max);
            val = max / 255d;
            return true;
        }

        public static System.Drawing.Color ColorFromHSV(double hue, double sat, double val)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            val *= 255;
            int v = Convert.ToInt32(val);
            int p = Convert.ToInt32(val * (1 - sat));
            int q = Convert.ToInt32(val * (1 - f * sat));
            int t = Convert.ToInt32(val * (1 - (1 - f) * sat));

            if (hi == 0)
                return System.Drawing.Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb(255, t, p, v);
            else
                return System.Drawing.Color.FromArgb(255, v, p, q);
        }
        /// <param name="alpha">A double 0 to 1 where 1 is opaque and 0 is transparent.</param>
        public static System.Drawing.Color ColorFromHSV(double alpha, double hue, double sat, double val)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            val *= 255;
            int v = Convert.ToInt32(val);
            int p = Convert.ToInt32(val * (1 - sat));
            int q = Convert.ToInt32(val * (1 - f * sat));
            int t = Convert.ToInt32(val * (1 - (1 - f) * sat));

            if (hi == 0)
                return System.Drawing.Color.FromArgb((int)(alpha * 255), v, t, p);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb((int)(alpha * 255), q, v, p);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb((int)(alpha * 255), p, v, t);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb((int)(alpha * 255), p, q, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb((int)(alpha * 255), t, p, v);
            else
                return System.Drawing.Color.FromArgb((int)(alpha * 255), v, p, q);
        }
        public static System.Drawing.Rectangle Padded(this System.Drawing.Rectangle r, int px)
        {
            return System.Drawing.Rectangle.FromLTRB(r.Left - px, r.Top - px, r.Right + px, r.Bottom + px);
        }
        public static System.Drawing.RectangleF Padded(this System.Drawing.RectangleF r, float px)
        {
            return System.Drawing.RectangleF.FromLTRB(r.Left - px, r.Top - px, r.Right + px, r.Bottom + px);
        }
        public static double GetInRange(this double num, double min, double max)
        {
            return num < min ? min : num > max ? max : num;
        }
        public static float GetInRange(this float num, float min, float max)
        {
            return num < min ? min : num > max ? max : num;
        }
        public static int GetInRange(this int num, int min, int max)
        {
            return num < min ? min : num > max ? max : num;
        }
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));

            using GraphicsPath path = RoundedRect(bounds, cornerRadius);
            graphics.DrawPath(pen, path);
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));

            using GraphicsPath path = RoundedRect(bounds, cornerRadius);
            graphics.FillPath(brush, path);
        }
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int topLeft, int topRight, int bottomRight, int bottomLeft)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));

            using GraphicsPath path = RoundedRect(bounds, topLeft, topRight, bottomRight, bottomLeft);
            graphics.FillPath(brush, path);
        }
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius) => RoundedRect(bounds, radius, radius, radius, radius);
        public static GraphicsPath RoundedRect(Rectangle bounds, int topLeft, int topRight, int bottomRight, int bottomLeft)
        {
            int diameter1 = topLeft * 2;
            int diameter2 = topRight * 2;
            int diameter3 = bottomRight * 2;
            int diameter4 = bottomLeft * 2;

            Rectangle arc1 = new Rectangle(bounds.Location, new Size(diameter1, diameter1));
            Rectangle arc2 = new Rectangle(bounds.Location, new Size(diameter2, diameter2));
            Rectangle arc3 = new Rectangle(bounds.Location, new Size(diameter3, diameter3));
            Rectangle arc4 = new Rectangle(bounds.Location, new Size(diameter4, diameter4));
            GraphicsPath path = new GraphicsPath();

            // top left arc  
            if (topLeft == 0)
            {
                path.AddLine(arc1.Location, arc1.Location);
            }
            else
            {
                path.AddArc(arc1, 180, 90);
            }

            // top right arc  
            arc2.X = bounds.Right - diameter2;
            if (topRight == 0)
            {
                path.AddLine(arc2.Location, arc2.Location);
            }
            else
            {
                path.AddArc(arc2, 270, 90);
            }

            // bottom right arc  

            arc3.X = bounds.Right - diameter3;
            arc3.Y = bounds.Bottom - diameter3;
            if (bottomRight == 0)
            {
                path.AddLine(arc3.Location, arc3.Location);
            }
            else
            {
                path.AddArc(arc3, 0, 90);
            }

            // bottom left arc 
            arc4.X = bounds.Right - diameter4;
            arc4.Y = bounds.Bottom - diameter4;
            arc4.X = bounds.Left;
            if (bottomLeft == 0)
            {
                path.AddLine(arc4.Location, arc4.Location);
            }
            else
            {
                path.AddArc(arc4, 90, 90);
            }

            path.CloseFigure();
            return path;
        }
        public static Bitmap GetResizedImage(Image image, int width, int height, InterpolationMode method = InterpolationMode.HighQualityBicubic)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = method;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static Color GetStandOutColor(this Color c, int diff = 10)
        {
            if (c.GetBrightness() > 0.5f) return Color.FromArgb(c.R - diff >= 0 ? c.R + diff : 0, c.G + diff >= 0 ? c.G + diff : 0,
                c.B + diff >= 0 ? c.B + diff : 0);
            else return Color.FromArgb(c.R + diff <= 255 ? c.R + diff : 255, c.G + diff <= 255 ? c.G + diff : 255, c.B + diff <= 255 ? c.B + diff : 255);
        }
        public static Rectangle InflateToRectangle(this RectangleF r)
        {
            return Rectangle.FromLTRB((int)MathF.Floor(r.X), (int)MathF.Floor(r.Y), (int)MathF.Ceiling(r.Right), (int)MathF.Ceiling(r.Bottom));
        }
        public static Rectangle DeflateToRectangle(this RectangleF r)
        {
            return Rectangle.FromLTRB((int)MathF.Ceiling(r.X), (int)MathF.Ceiling(r.Y), (int)MathF.Floor(r.Right), (int)MathF.Floor(r.Bottom));
        }
        public static Rectangle InflateToRectangle(this RectangleF r, Point minPt, Size maxSz)
        {
            int x = Math.Max((int)MathF.Floor(r.X), minPt.X);
            int y = Math.Max((int)MathF.Floor(r.Y), minPt.Y);
            return Rectangle.FromLTRB(x, y, (int)MathF.Ceiling(MathF.Min(r.Width, maxSz.Width) + r.Left), (int)MathF.Ceiling(MathF.Min(r.Height, maxSz.Height) + r.Top));
        }
        public static Rectangle InflateToRectangle(this RectangleF r, int minX, int minY, int maxRight, int maxBottom)
        {
            int x = Math.Max((int)MathF.Floor(r.X), minX);
            int y = Math.Max((int)MathF.Floor(r.Y), minY);
            return Rectangle.FromLTRB(x, y, Math.Min((int)MathF.Ceiling(r.Right), maxRight), Math.Min((int)MathF.Ceiling(r.Bottom), maxBottom));
        }
        public static RectangleF Positive(this RectangleF r)
        {
            return RectangleF.FromLTRB(r.Width < 0 ? r.Left + r.Width : r.Left, r.Height < 0 ? r.Top + r.Height : r.Top,
                r.Width < 0 ? r.Left : r.Right, r.Height < 0 ? r.Top : r.Bottom);
        }
        public static Rectangle Positive(this Rectangle r)
        {
            return Rectangle.FromLTRB(r.Width < 0 ? r.Left + r.Width : r.Left, r.Height < 0 ? r.Top + r.Height : r.Top,
                r.Width < 0 ? r.Left : r.Right, r.Height < 0 ? r.Top : r.Bottom);
        }
        public static RectangleF ToRectangleF(this Rectangle r)
        {
            return new RectangleF(r.Left, r.Top, r.Width, r.Height);
        }
        public static PointF GetContainingPoint(this RectangleF r, PointF pt)
        {
            float x = pt.X;
            float y = pt.Y;
            if (x < r.X) x = r.X;
            else if (x > r.Right) x = r.Right;
            if (y < r.Y) y = r.Y;
            else if (y > r.Bottom) y = r.Bottom;
            return new PointF(x, y);
        }
        public static PointF GetContainingPoint(this Rectangle r, PointF pt)
        {
            float x = pt.X;
            float y = pt.Y;
            if (x < r.X) x = r.X;
            else if (x > r.Right) x = r.Right;
            if (y < r.Y) y = r.Y;
            else if (y > r.Bottom) y = r.Bottom;
            return new PointF(x, y);
        }
        public static void DrawRectangle(this Graphics g, Pen pen, RectangleF rect) => g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        public static float GeoArea(this RectangleF r) => r.Width * r.Height < 0f ? r.Width * r.Height * -1f : r.Width * r.Height;
        public static int GeoArea(this Rectangle r) => r.Width * r.Height < 0 ? r.Width * r.Height * -1 : r.Width * r.Height;
        public static float GeoArea(this SizeF r) => r.Width * r.Height < 0f ? r.Width * r.Height * -1f : r.Width * r.Height;
        public static int GeoArea(this Size r) => r.Width * r.Height < 0 ? r.Width * r.Height * -1 : r.Width * r.Height;
        public static Size InflateToSize(this SizeF sz) => new Size((int)MathF.Ceiling(sz.Width), (int)MathF.Ceiling(sz.Height));
        public static Color OfHue(this Color c, double hue)
        {
            return c.TryToHSV(out _, out double s, out double v) ? ColorFromHSV(hue, s, v) : default;
        }
        public static Color OfValue(this Color c, double value)
        {
            return c.TryToHSV(out double h, out double s, out _) ? ColorFromHSV(h, s, value) : default;
        }
        public static RectangleF[] SurroundRectangle(RectangleF r)
        {
            return new RectangleF[8]
            {
                new RectangleF(r.Left - r.Width, r.Top - r.Height, r.Width, r.Height),
                new RectangleF(r.Left, r.Top - r.Height, r.Width, r.Height),
                new RectangleF(r.Left + r.Width, r.Top - r.Height, r.Width, r.Height),
                new RectangleF(r.Left + r.Width, r.Top, r.Width, r.Height),
                new RectangleF(r.Left + r.Width, r.Top + r.Height, r.Width, r.Height),
                new RectangleF(r.Left, r.Top + r.Height, r.Width, r.Height),
                new RectangleF(r.Left - r.Width, r.Top + r.Height, r.Width, r.Height),
                new RectangleF(r.Left - r.Width, r.Top, r.Width, r.Height)
            };
        }
        public static unsafe void Clear(this Bitmap bmp, Color c, Rectangle r)
        {
            if (r.GeoArea() is 0) return;
            System.Drawing.Imaging.BitmapData d = bmp.LockBits(r, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(System.Drawing.Imaging.PixelFormat.Format32bppArgb) / 8;
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* data = scan0 + y * stride + x * bytesPerPixel;
                    data[0] = c.B;
                    data[1] = c.G;
                    data[2] = c.R;
                    data[3] = c.A;
                }
            }
            bmp.UnlockBits(d);
        }
        public static Rectangle Constrain(this Rectangle r, Rectangle c)
        {
            r = r.Positive();
            return Rectangle.FromLTRB(r.Left.GetInRange(c.Left, c.Right), r.Top.GetInRange(c.Top, c.Bottom), r.Right.GetInRange(c.Left, c.Right),
                r.Bottom.GetInRange(c.Top, c.Bottom));
        }
        public static Rectangle Constrain(this Rectangle r, Point pt, Size sz)
        {
            r = r.Positive();
            return Rectangle.FromLTRB(r.Left.GetInRange(pt.X, sz.Width + pt.X), r.Top.GetInRange(pt.Y, sz.Height + pt.Y), r.Right.GetInRange(pt.X, sz.Width + pt.X),
                r.Bottom.GetInRange(pt.Y, sz.Height + pt.Y));
        }
        public static RectangleF Constrain(this RectangleF r, RectangleF c)
        {
            r = r.Positive();
            return RectangleF.FromLTRB(r.Left.GetInRange(c.Left, c.Right), r.Top.GetInRange(c.Top, c.Bottom), r.Right.GetInRange(c.Left, c.Right),
                r.Bottom.GetInRange(c.Top, c.Bottom));
        }
        public static RectangleF Constrain(this RectangleF r, PointF pt, SizeF sz)
        {
            r = r.Positive();
            return RectangleF.FromLTRB(r.Left.GetInRange(pt.X, sz.Width + pt.X), r.Top.GetInRange(pt.Y, sz.Height + pt.Y), r.Right.GetInRange(pt.X, sz.Width + pt.X),
                r.Bottom.GetInRange(pt.Y, sz.Height + pt.Y));
        }
        public static int MathMax(params int[] nums)
        {
            if (nums is null) throw new ArgumentNullException("", nameof(nums));
            if (nums.Length is 0) throw new ArgumentException("", nameof(nums));
            int h = nums[0];
            for(int i = 0; i < nums.Length; i++)
            {
                if (nums[i] > h) h = nums[i];
            }
            return h;
        }
        public static double MathMax(params double[] nums)
        {
            if (nums is null) throw new ArgumentNullException("", nameof(nums));
            if (nums.Length is 0) throw new ArgumentException("", nameof(nums));
            double h = nums[0];
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] > h) h = nums[i];
            }
            return h;
        }
        public static float MathMax(params float[] nums)
        {
            if (nums is null) throw new ArgumentNullException("", nameof(nums));
            if (nums.Length is 0) throw new ArgumentException("", nameof(nums));
            float h = nums[0];
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] > h) h = nums[i];
            }
            return h;
        }
        public static float[][] ToFloat2dArray(this System.Drawing.Imaging.ColorMatrix m)
        {
            float[][] arr = new float[5][];
            for(int i = 0; i < 4; i++)
            {
                arr[i] = new float[5];
                for(int j = 0; j < 4; j++)
                {
                    arr[i][j] = m[i, j];
                }
            }
            arr[4] = new float[5];
            return arr;
        }
        public static System.Drawing.Imaging.ColorMatrix Clone(this System.Drawing.Imaging.ColorMatrix m)
        {
            return new(m.ToFloat2dArray());
        }
        public static Color Transform(this Color c, float[][] m)
        {
            return m[0][0] is 1f && m[1][1] is 1f && m[2][2] is 1f && m[3][3] is 1f && m[4][0] is 0f && m[4][1] is 0f && m[4][2] is 0f && m[4][3] is 0f ? c :
                Color.FromArgb((byte)(c.R * m[0][0] + m[4][0]), (byte)(c.G * m[1][1] + m[4][1]), (byte)(c.B * m[2][2] + m[4][2]), (byte)(c.A * m[3][3] + m[4][3]));
        }
        public static int CalcIndexOf<T>(this IEnumerable<T> en, T item)
        {
            int i = 0;
            foreach(T t in en)
            {
                i++;
                if (t.Equals(item)) return i;
            }
            return -1;
        }
        public static T CalcItemAt<T>(this IEnumerable<T> en, int index)
        {
            int i = 0;
            foreach (T t in en)
            {
                if (i++ == index) return t;
            }
            return default;
        }
    }
}
