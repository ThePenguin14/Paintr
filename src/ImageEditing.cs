using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Paintr
{
    public static class ImageEditing
    {
        /*public static unsafe Region GetFloodSelection(Bitmap bmp, Point pt, Color col, int tolerance)
        {
            Region rgn = new Region();
            Rectangle r = new Rectangle();
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int bpp = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            int widthBytes = d.Width * bpp;
            int heightPixels = d.Height;
            byte* ptrPixel0 = (byte*)d.Scan0;
            bool[] checkedPixel = new bool[d.Width * d.Height];
            int x = pt.X;
            while (true)
            {

            }
            bmp.UnlockBits(d);
            return rgn;
        }*/
        public static void DoEffect(string s, Bitmap bmp, Region rgn)
        {
            switch (s)
            {
                case "negate":
                    NegateRegion(bmp, rgn);
                    break;
                case "grayscale":
                    GrayscaleRegion(bmp, rgn);
                    break;
            }
        }
        public static unsafe void NegateRegion(Bitmap bmp, Region rgn)
        {
            Matrix m = new Matrix();
            RectangleF[] rs = rgn.GetRegionScans(m);
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            foreach (RectangleF rf in rs)
            {
                Rectangle re = Rectangle.FromLTRB((int)rf.Left, (int)rf.Top, (int)rf.Right, (int)rf.Bottom);
                for (int y = re.Top; y < re.Bottom; y++) {
                    for (int x = re.Left; x < re.Right; x++)
                    {
                        byte* data = scan0 + y * stride + x * bytesPerPixel;
                        data[0] = (byte)(255 - data[0]);
                        data[1] = (byte)(255 - data[1]);
                        data[2] = (byte)(255 - data[2]);
                    }
                }
            }
            bmp.UnlockBits(d);
            m.Dispose();
        }
        public static unsafe void NegateRegion(Bitmap bmp, Rectangle re)
        {
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int maxPointerLength = width * height * Image.GetPixelFormatSize(PixelFormat.Format32bppArgb);
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            for (int y = re.Top; y < re.Bottom; y++)
            {
                for (int x = re.Left; x < re.Right; x++)
                {
                    byte* data = scan0 + y * stride + x * bytesPerPixel;
                    data[0] = (byte)(255 - data[0]);
                    data[1] = (byte)(255 - data[1]);
                    data[2] = (byte)(255 - data[2]);

                }
            }
            bmp.UnlockBits(d);
        }
        public static unsafe void GrayscaleRegion(Bitmap bmp, Region rgn)
        {
            Matrix m = new Matrix();
            RectangleF[] rs = rgn.GetRegionScans(m);
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            foreach (RectangleF rf in rs)
            {
                Rectangle re = Rectangle.FromLTRB((int)rf.Left, (int)rf.Top, (int)rf.Right, (int)rf.Bottom);
                for (int y = re.Top; y < re.Bottom; y++)
                {
                    for (int x = re.Left; x < re.Right; x++)
                    {
                        byte* data = scan0 + y * stride + x * bytesPerPixel;
                        Color col = Color.FromArgb(data[0], data[1], data[2]);
                        col = Extensions.ColorFromHSV(0d, 0d, col.GetDoubleValue());
                        data[0] = col.B;
                        data[1] = col.G;
                        data[2] = col.R;
                    }
                }
            }
            bmp.UnlockBits(d);
            m.Dispose();
        }
        public static unsafe void GrayscaleRegion(Bitmap bmp, Rectangle re)
        {
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int maxPointerLength = width * height * Image.GetPixelFormatSize(PixelFormat.Format32bppArgb);
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            for (int y = re.Top; y < re.Bottom; y++)
            {
                for (int x = re.Left; x < re.Right; x++)
                {
                    byte* data = scan0 + y * stride + x * bytesPerPixel;
                    Color col = Color.FromArgb(data[0], data[1], data[2]);
                    col = Extensions.ColorFromHSV(0d, 0d, col.GetDoubleValue());
                    data[0] = col.B;
                    data[1] = col.G;
                    data[2] = col.R;

                }
            }
            bmp.UnlockBits(d);
        }

        public static unsafe Region FloodSelect(Bitmap bmp, Point pt, int tolerance)
        {
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int maxPointerLength = width * height * Image.GetPixelFormatSize(PixelFormat.Format32bppArgb);
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            byte* data = scan0 + pt.Y * stride + (pt.X - 1) * bytesPerPixel;
            Color c = Color.FromArgb(data[0], data[1], data[2]);
            Region rgn = new Region(Rectangle.Empty);
            List<Point> pts = new List<Point>();
            FloodSelect(scan0, stride, bytesPerPixel, width, height, pt, tolerance, pts, c);
            foreach (Point p in pts) rgn.Complement(new Rectangle(p, new Size(1, 1)));
            bmp.UnlockBits(d);
            return rgn;
        }
        private static unsafe void FloodSelect(byte* scan0, int stride, int bytesPerPixel, int w, int h, Point pt, int t, List<Point> pts, Color col)
        {
            if (pt.X - 1 >= 0 && !pts.Contains(new Point(pt.X - 1, pt.Y)))
            {
                byte* data = scan0 + pt.Y * stride + (pt.X - 1) * bytesPerPixel;
                Color c = Color.FromArgb(data[0], data[1], data[2]);
                if (c.R + t >= col.R && c.R - t <= col.R && c.G + t >= col.G && c.G - t <= col.G && c.B + t >= col.B && c.B - t <= col.B)
                {
                    pts.Add(new Point(pt.X - 1, pt.Y));
                    FloodSelect(scan0, stride, bytesPerPixel, w, h, new Point(pt.X - 1, pt.Y), t, pts, col);
                }
            }
            if (pt.X + 1 < w && !pts.Contains(new Point(pt.X + 1, pt.Y)))
            {
                byte* data = scan0 + pt.Y * stride + (pt.X + 1) * bytesPerPixel;
                Color c = Color.FromArgb(data[0], data[1], data[2]);
                if (c.R + t >= col.R && c.R - t <= col.R && c.G + t >= col.G && c.G - t <= col.G && c.B + t >= col.B && c.B - t <= col.B)
                {
                    pts.Add(new Point(pt.X + 1, pt.Y));
                    FloodSelect(scan0, stride, bytesPerPixel, w, h, new Point(pt.X + 1, pt.Y), t, pts, col);
                }
            }
            if (pt.Y - 1 >= 0 && !pts.Contains(new Point(pt.X, pt.Y - 1)))
            {
                byte* data = scan0 + (pt.Y - 1) * stride + pt.X * bytesPerPixel;
                Color c = Color.FromArgb(data[0], data[1], data[2]);
                if (c.R + t >= col.R && c.R - t <= col.R && c.G + t >= col.G && c.G - t <= col.G && c.B + t >= col.B && c.B - t <= col.B)
                {
                    pts.Add(new Point(pt.X, pt.Y - 1));
                    FloodSelect(scan0, stride, bytesPerPixel, w, h, new Point(pt.X, pt.Y - 1), t, pts, col);
                }
            }
            if (pt.Y + 1 < h && !pts.Contains(new Point(pt.X, pt.Y + 1)))
            {
                byte* data = scan0 + (pt.Y + 1) * stride + pt.X * bytesPerPixel;
                Color c = Color.FromArgb(data[0], data[1], data[2]);
                if (c.R + t >= col.R && c.R - t <= col.R && c.G + t >= col.G && c.G - t <= col.G && c.B + t >= col.B && c.B - t <= col.B)
                {
                    pts.Add(new Point(pt.X, pt.Y + 1));
                    FloodSelect(scan0, stride, bytesPerPixel, w, h, new Point(pt.X, pt.Y + 1), t, pts, col);
                }
            }
        }
        public static unsafe void ColorizeValueRegion(Bitmap bmp, Rectangle re, double hue, double saturation)
        {
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            for (int y = re.Top; y < re.Bottom; y++)
            {
                for (int x = re.Left; x < re.Right; x++)
                {
                    byte* data = scan0 + y * stride + x * bytesPerPixel;
                    Color col = Color.FromArgb(data[0], data[1], data[2]);
                    col = Extensions.ColorFromHSV(hue, saturation, col.GetDoubleValue());
                    data[0] = col.R;
                    data[1] = col.G;
                    data[2] = col.B;
                }
            }
            bmp.UnlockBits(d);
        }
        public static unsafe void ColorPixelRegion(Bitmap bmp, Rectangle re, Color col, bool keepAlpha)
        {
            BitmapData d = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int width = d.Width;
            int height = d.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int stride = d.Stride;
            byte* scan0 = (byte*)d.Scan0.ToPointer();
            for (int y = re.Top; y < re.Bottom; y++)
            {
                for (int x = re.Left; x < re.Right; x++)
                {
                    byte* data = scan0 + y * stride + x * bytesPerPixel;
                    if(data[3] is not 0)
                    {
                        data[0] = col.B;
                        data[1] = col.G;
                        data[2] = col.R;
                        if (!keepAlpha) data[3] = col.A;
                    }
                }
            }
            bmp.UnlockBits(d);
        }
    }
}
