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
    public static class BlendMixing
    {
        public static void Blend(BlendMode mode, Bitmap upper, Bitmap lower, double opacity, ImageEdits edits = null, Rectangle? r = null)
        {
            if (edits is null) edits = new ImageEdits();
            if (r.HasValue && r.Value.GeoArea() is 0) return;
            switch (mode)
            {
                case BlendMode.Standard:
                    BlendStandard(upper, lower, opacity, edits, r);
                    break;
                case BlendMode.Color:
                    BlendColor(upper, lower, opacity, edits, r);
                    break;
                case BlendMode.Copy:
                    BlendCopy(upper, lower, opacity, edits, r);
                    break;
                case BlendMode.Lighten:
                    BlendLighten(upper, lower, opacity, edits, r);
                    break;
                default:
                    throw new ArgumentException("", nameof(mode));
            }
        }
        public static void BlendStandard(Bitmap upper, Bitmap lower, double opacity, ImageEdits edits, Rectangle? r = null)
        {
            Rectangle rect = r.HasValue ? r.Value : new Rectangle(Point.Empty, lower.Size);
            Graphics g = Graphics.FromImage(lower);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            ColorMatrix m = new ColorMatrix(edits.ColorMatrix);
            m.Matrix33 = (float)opacity * m.Matrix33;
            ImageAttributes attr = new ImageAttributes();
            attr.SetColorMatrix(m, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            g.DrawImage(upper, rect, rect.Left, rect.Top, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
            g.Dispose();
            attr.Dispose();
        }
        public static unsafe void BlendColor(Bitmap upper, Bitmap lower, double opacity, ImageEdits edits, Rectangle? r = null)
        {
            Rectangle rect = r.HasValue ? r.Value : new Rectangle(Point.Empty, lower.Size);
            BitmapData lowerData = lower.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData upperData = upper.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int width = rect.Width;
            int height = rect.Height;
            
            byte* lowerPointer = (byte*)lowerData.Scan0;
            byte* upperPointer = (byte*)upperData.Scan0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color lowerColor = Color.FromArgb(lowerPointer[3], lowerPointer[2], lowerPointer[1], lowerPointer[0]);
                    Color upperColor = Color.FromArgb(upperPointer[3], upperPointer[2], upperPointer[1], upperPointer[0]);
                    if (lowerColor.A is not 0 && upperColor.A is not 0) {
                        lowerColor.TryToHSL(out double hl, out _, out double l);
                        upperColor.TryToHSL(out double hu, out double s, out _);
                        Color outputColor = Extensions.ColorFromHSL((hu - hl) * opacity + hl, s, l).Transform(edits.ColorMatrix);

                        lowerPointer[0] = outputColor.B;
                        lowerPointer[1] = outputColor.G;
                        lowerPointer[2] = outputColor.R;
                        lowerPointer[3] = lowerColor.Transform(edits.ColorMatrix).A;
                    }
                    else if (lowerColor.A is 0)
                    {
                        Color outputColor = upperColor.Transform(edits.ColorMatrix);
                        lowerPointer[0] = outputColor.B;
                        lowerPointer[1] = outputColor.G;
                        lowerPointer[2] = outputColor.R;
                        lowerPointer[3] = lowerColor.Transform(edits.ColorMatrix).A;
                    }
                    else
                    {
                        Color outputColor = lowerColor.Transform(edits.ColorMatrix);
                        lowerPointer[0] = outputColor.B;
                        lowerPointer[1] = outputColor.G;
                        lowerPointer[2] = outputColor.R;
                        lowerPointer[3] = outputColor.A;
                    }
                    // Moving the pointers by 3 bytes per pixel
                    lowerPointer += 4;
                    upperPointer += 4;
                }

                // Moving the pointers to the next pixel row
                lowerPointer += lowerData.Stride - (width * 4);
                upperPointer += upperData.Stride - (width * 4);
            }

            lower.UnlockBits(lowerData);
            upper.UnlockBits(upperData);
        }
        public static unsafe void BlendCopy(Bitmap upper, Bitmap lower, double opacity, ImageEdits edits, Rectangle? r = null)
        {
            Rectangle rect = r.HasValue ? r.Value : new Rectangle(Point.Empty, lower.Size);
            BitmapData lowerData = lower.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData upperData = upper.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int width = rect.Width;
            int height = rect.Height;

            byte* lowerPointer = (byte*)lowerData.Scan0;
            byte* upperPointer = (byte*)upperData.Scan0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color lowerColor = Color.FromArgb(lowerPointer[3], lowerPointer[2], lowerPointer[1], lowerPointer[0]);
                    Color upperColor = Color.FromArgb(upperPointer[3], upperPointer[2], upperPointer[1], upperPointer[0]);
                    if (upperColor.A is not 0)
                    {
                        Color outputColor = upperColor.Transform(edits.ColorMatrix);
                        lowerPointer[0] = outputColor.B;
                        lowerPointer[1] = outputColor.G;
                        lowerPointer[2] = outputColor.R;
                        lowerPointer[3] = lowerColor.Transform(edits.ColorMatrix).A;
                    }
                    // Moving the pointers by 3 bytes per pixel
                    lowerPointer += 4;
                    upperPointer += 4;
                }

                // Moving the pointers to the next pixel row
                lowerPointer += lowerData.Stride - (width * 4);
                upperPointer += upperData.Stride - (width * 4);
            }

            lower.UnlockBits(lowerData);
            upper.UnlockBits(upperData);
        }
        public static unsafe void BlendLighten(Bitmap upper, Bitmap lower, double opacity, ImageEdits edits, Rectangle? r = null)
        {
            Rectangle rect = r.HasValue ? r.Value : new Rectangle(Point.Empty, lower.Size);
            BitmapData lowerData = lower.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData upperData = upper.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int width = rect.Width;
            int height = rect.Height;

            byte* lowerPointer = (byte*)lowerData.Scan0;
            byte* upperPointer = (byte*)upperData.Scan0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color lowerColor = Color.FromArgb(lowerPointer[3], lowerPointer[2], lowerPointer[1], lowerPointer[0]);
                    Color upperColor = Color.FromArgb(upperPointer[3], upperPointer[2], upperPointer[1], upperPointer[0]);
                    if (upperColor.A is 0)
                    {
                        Color outputColor = lowerColor.Transform(edits.ColorMatrix);
                        lowerPointer[0] = outputColor.B;
                        lowerPointer[1] = outputColor.G;
                        lowerPointer[2] = outputColor.R;
                        lowerPointer[3] = outputColor.A;
                    }
                    else if(lowerColor.A is 0)
                    {
                        Color outputColor = upperColor.Transform(edits.ColorMatrix);
                        lowerPointer[0] = outputColor.B;
                        lowerPointer[1] = outputColor.G;
                        lowerPointer[2] = outputColor.R;
                        lowerPointer[3] = outputColor.A;
                    }
                    else
                    {
                        Color output = Color.FromArgb(lowerColor.R > upperColor.R ? (byte)((upperColor.R - lowerColor.R) * opacity + lowerColor.R) : lowerColor.R,
                            lowerColor.G > upperColor.G ? (byte)((upperColor.G - lowerColor.G) * opacity + lowerColor.G) : lowerColor.G,
                            lowerColor.B > upperColor.B ? (byte)((upperColor.B - lowerColor.B) * opacity + lowerColor.B) : lowerColor.B).Transform(edits.ColorMatrix);
                        lowerPointer[0] = output.B;
                        lowerPointer[1] = output.G;
                        lowerPointer[2] = output.R;
                        lowerPointer[3] = lowerColor.Transform(edits.ColorMatrix).A;
                    }
                    // Moving the pointers by 3 bytes per pixel
                    lowerPointer += 4;
                    upperPointer += 4;
                }

                // Moving the pointers to the next pixel row
                lowerPointer += lowerData.Stride - (width * 4);
                upperPointer += upperData.Stride - (width * 4);
            }

            lower.UnlockBits(lowerData);
            upper.UnlockBits(upperData);
        }
    }
    public enum BlendMode
    {
        Standard,
        Color,
        Copy,
        Lighten
    }
    public class ImageEdits
    {
        public ImageEdits()
        {
            ColorMatrix = new ColorMatrix().ToFloat2dArray();
        }
        public ImageEdits(ColorMatrix colorMatrix)
        {
            ColorMatrix = colorMatrix.ToFloat2dArray() ?? new ColorMatrix().ToFloat2dArray();
        }
        public ImageEdits(float[][] colorMatrix)
        {
            if (colorMatrix is not null && colorMatrix.Length is not 5 || colorMatrix[0].Length is not 5 || colorMatrix[1].Length is not 5 ||
                colorMatrix[2].Length is not 5 || colorMatrix[3].Length is not 5 || colorMatrix[4].Length is not 5)
                throw new ArgumentException("Color matrix must be 5x5.", nameof(colorMatrix));
            ColorMatrix = colorMatrix ?? new ColorMatrix().ToFloat2dArray();
        }
        protected ImageEdits(string name, float[][] colorMatrix) : this(colorMatrix)
        {
            if (colorMatrix is not null && colorMatrix.Length is not 5 || colorMatrix[0].Length is not 5 || colorMatrix[1].Length is not 5 ||
                colorMatrix[2].Length is not 5 || colorMatrix[3].Length is not 5 || colorMatrix[4].Length is not 5)
                throw new ArgumentException("Color matrix must be 5x5.", nameof(colorMatrix));
            Name = name;
        }
        protected ImageEdits(string name) : this()
        {
            Name = name;
        }
        public string Name { get; }
        public float[][] ColorMatrix { get; }
        static ImageEdits()
        {
            PresetEdits[StandardEditId] = new ImageEdits(Properties.Strings.ShaderStandard);
            PresetEdits[LightenEditId] = new ImageEdits(Properties.Strings.ShaderLighten, new float[5][]
            {
                new float[5] { 2f, 0f, 0f, 0f, 0f },
                new float[5] { 0f, 2f, 0f, 0f, 0f },
                new float[5] { 0f, 0f, 2f, 0f, 0f },
                new float[5] { 0f, 0f, 0f, 2f, 0f },
                new float[5] { 0f, 0f, 0f, 0f, 0f }
            });
        }
        public static Dictionary<string, ImageEdits> PresetEdits { get; } = new();
        public const string StandardEditId = "base:standardshader";
        public const string LightenEditId = "base:lightenshader";
    }
}
