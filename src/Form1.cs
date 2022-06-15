using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Paintr.Dialog;

namespace Paintr
{
    public partial class Form1 : Form
    {
        private ColorPicker colorPicker;
        private BrushSelection brushSel;
        private Viewport viewport;
        private ThemedSlider widthSlider;
        private TextEdit textEdit;
        private LayerEditor layerEditor;
        public Form1()
        {
            MouseWheel += Form1_MouseWheel;
            WindowState = FormWindowState.Maximized;
            InitializeComponent();
            SetTexts();
            toolStrip1.Renderer = new ToolbarRenderer();
            toolStrip1.BackColor = Styling.BackgroundColor;
            toolStrip1.ForeColor = Styling.TextColor;
            statusStrip1.BackColor = Styling.BackgroundColor;
            statusStrip1.ForeColor = Styling.TextColor;
            statusStrip1.Renderer = new ToolbarRenderer();
            SizeChanged += Form1_SizeChanged;
            viewport = new Viewport()
            {
                ImageSize = new Size(800, 600)
            };
            viewport.AddLayer(new DrawingLayer(new Bitmap(800, 600), string.Format(Properties.Strings.DefaultLayerName, 1)));
            viewport.BrushPressed += Viewport_BrushPressed;
            viewport.ViewZoomChanged += Viewport_ViewZoomChanged;
            viewport.ViewOffsetChanged += Viewport_ViewOffsetChanged;
            viewport.ImageSizeChanged += Viewport_ImageSizeChanged;
            Editor.Viewport = viewport;
            Editor.CurrentBrushWidthChanged += Editor_CurrentBrushWidthChanged;
            Controls.Add(viewport);
            colorPicker = new ColorPicker()
            {
                HasTwoColors = true,
                Location = new Point(5, 0),
                Size = new Size(300, 455),
                Color = Color.Black
            };
            colorPicker.ColorChanged += ColorPicker_ColorChanged;
            Editor.ColorPicker = colorPicker;
            Controls.Add(colorPicker);

            brushSel = new BrushSelection()
            {
                Location = new Point(5, 450),
                Size = new Size(300, (ClientSize.Height - statusStrip1.Height - 415) / 2 - 2)
            };
            Controls.Add(brushSel);
            widthSlider = new ThemedSlider()
            {
                Location = new Point(310, (50 - ClientRectangle.Top) / 2),
                Size = new Size(300, 50),
                MinimumValue = 1d,
                MaximumValue = 300d,
                Value = 8d,
                Step = 1d,
                DecimalPlaces = 0
            };
            widthSlider.ValueChanged += WidthSlider_ValueChanged;
            Controls.Add(widthSlider);
            textEdit = new TextEdit()
            {
                FontSize = 11f
            };
            textEdit.XClicked += TextEdit_XClicked;
            textEdit.OutputFontChanged += TextEdit_OutputFontChanged;
            layerEditor = new LayerEditor()
            {
                Location = new Point(5, (ClientSize.Height - statusStrip1.Height - 415) / 2 + 1),
                Size = new Size(300, (ClientSize.Height - statusStrip1.Height - 415) / 2 - 5)
            };
            Editor.LayerEditor = layerEditor;
            Controls.Add(layerEditor);

            FontStyle style = FontStyle.Regular;
            if (textEdit.Bold) style |= FontStyle.Bold;
            if (textEdit.Italic) style |= FontStyle.Italic;
            if (textEdit.Underline) style |= FontStyle.Underline;
            textBrushFont = new Font(textEdit.FontFamilyName, textEdit.FontSize, style);


            Editor.TextEdit = textEdit;
            Editor.RequestHideTextEdit += Editor_RequestHideTextEdit;
            Editor.RequestShowTextEdit += Editor_RequestShowTextEdit;

            Disposed += Form1_Disposed;
            Load += Form1_Load;
            Editor.BrushChanged += Editor_BrushChanged;
            Editor.SelectionRegionModified += Editor_SelectionRegionModified;

            BackColor = Styling.BackgroundColor;
            AdjustSize();
            Load += Form1_Load1;
        }

        private void Form1_Load1(object sender, EventArgs e)
        {
            LuaPlugin.StartPlugins();
        }

        private void Viewport_ImageSizeChanged(object sender, EventArgs e)
        {
            UpdateCursorSize();
        }

        private void TextEdit_OutputFontChanged(object sender, EventArgs e)
        {
            FontStyle style = FontStyle.Regular;
            if (textEdit.Bold) style |= FontStyle.Bold;
            if (textEdit.Italic) style |= FontStyle.Italic;
            if (textEdit.Underline) style |= FontStyle.Underline;
            Font f = textBrushFont;
            textBrushFont = new Font(textEdit.FontFamilyName, textEdit.FontSize, style);
            f.Dispose();
        }

        private void TextEdit_XClicked(object sender, EventArgs e)
        {
            Editor.FireRequestHideTextEdit();
        }

        private void Editor_RequestShowTextEdit(object sender, EventArgs e)
        {
            if (!Controls.Contains(textEdit))
            {
                Controls.Add(textEdit);
                textEdit.BringToFront();
            }
        }

        private void Editor_RequestHideTextEdit(object sender, EventArgs e)
        {
            if (Controls.Contains(textEdit))
            {
                Controls.Remove(textEdit);
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Editor.ScrollStealers.Count is 0)
            {
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    if (viewport.ViewZoom + e.Delta * viewport.ViewZoom / 1000d is > double.MinValue and < double.MaxValue)
                    {
                        viewport.ViewZoom += e.Delta * viewport.ViewZoom / 1000d;
                        viewport.ViewOffset += new PointD(e.Delta * viewport.ViewOffset.X / 2000d, e.Delta * viewport.ViewOffset.Y / -2000d);
                    }
                }
                else if (ModifierKeys.HasFlag(Keys.Shift))
                {
                    viewport.ViewOffset -= new PointD(e.Delta / 5d, 0d);
                }
                else
                {
                    viewport.ViewOffset -= new PointD(0d, e.Delta / 5d);
                }
            }
        }

        private void Viewport_ViewOffsetChanged(object sender, EventArgs e)
        {
            viewportStatusLabel.Text = string.Format(Properties.Strings.ViewportStatusLabel,
                $"{(viewport.ViewZoom * 100d).ToString(viewport.ViewZoom < 0.01d ? "" : "F0")}%",
                viewport.ViewOffset.X < 0 ? $"{viewport.ViewOffset.X:F0}" : $"+{viewport.ViewOffset.X:F0}",
                viewport.ViewOffset.Y < 0 ? $"{viewport.ViewOffset.Y:F0}" : $"+{viewport.ViewOffset.Y:F0}");
        }

        private void Viewport_ViewZoomChanged(object sender, EventArgs e)
        {
            viewportStatusLabel.Text = string.Format(Properties.Strings.ViewportStatusLabel,
                $"{(viewport.ViewZoom * 100d).ToString(viewport.ViewZoom < 0.01d ? "" : "F0")}%",
                viewport.ViewOffset.X < 0 ? $"{viewport.ViewOffset.X:F0}" : $"+{viewport.ViewOffset.X:F0}",
                viewport.ViewOffset.Y < 0 ? $"{viewport.ViewOffset.Y:F0}" : $"+{viewport.ViewOffset.Y:F0}");
            UpdateCursorSize();
        }
        public void UpdateCursorSize()
        {
            if (Editor.CurrentBrushWidth.HasValue) viewport.SetCursorSize(Editor.Brush.Width is 1d ? 1 :
                (int)viewport.ScaleToImageX(Editor.CurrentBrushWidth.Value) + 4);
        }
        public void SetTexts()
        {
            fileDropDown.Text = Properties.Strings.MenuFile;
            openImageMenuItem.Text = Properties.Strings.MenuOpenImage;
            negateSelectionToolStripMenuItem.Text = Properties.Strings.NegateSelectionMenu;
            greyscaleSelectionToolStripMenuItem.Text = Properties.Strings.GrayscaleSelectionMenu;
            editDropDown.Text = Properties.Strings.MenuEdit;
            viewDropDown.Text = Properties.Strings.MenuView;
            rulerToolStripMenuItem.Text = Properties.Strings.RulerMenu;
            protractorToolStripMenuItem.Text = Properties.Strings.ProtractorMenu;
            pixelGridToolStripMenuItem.Text = Properties.Strings.PixelGridMenu;
            resetViewToolStripMenuItem.Text = Properties.Strings.ResetViewMenu;

            selectionStatusLabel.Text = string.Format(Properties.Strings.SelectionStatusLabel, "0.0", "0.0", "0.0", "0.0");
            viewportStatusLabel.Text = string.Format(Properties.Strings.ViewportStatusLabel, "100%", "+0", "+0");
        }
        private Region prevRgn = new Region(Rectangle.Empty);
        private void Editor_SelectionRegionModified(object sender, RegionModifiedEventArgs e)
        {
            System.Drawing.Drawing2D.Matrix m = new();
            RectangleF re = viewport.ImageRect;
            Size sz = viewport.ImageSize;
            m.Scale(re.Width / sz.Width, re.Height / sz.Height);
            prevRgn.Xor(Editor.SelectionRegion);
            prevRgn.Transform(m);
            prevRgn.Translate(re.Left, re.Top);
            viewport.InvalidateDontComposite(prevRgn);
            m.Dispose();
            RectangleF r = Editor.SelectionRegion.GetBounds(Editor.EditingLayer.Graphics);
            selectionStatusLabel.Text = string.Format(Properties.Strings.SelectionStatusLabel, r.Left.ToString("F1"), r.Top.ToString("F1"), r.Width.ToString("F1"),
                r.Height.ToString("F1"));
            prevRgn.MakeEmpty();
            prevRgn.Union(Editor.SelectionRegion);
        }

        private void Editor_BrushChanged(object sender, PropertyChangedEventArgs<DrawBrush> e)
        {
            if (Editor.Brush.HasColor) Editor.Brush.Color = colorPicker.Color;
            if (Editor.Brush.HasWidth) Editor.Brush.Width = widthSlider.Value;
            switch (Editor.Brush.CursorType)
            {
                case CursorType.Standard:
                    UpdateCursorSize();
                    break;
                case CursorType.ForceCrosshair:
                    viewport.SetCursorSize(1);
                    break;
            }
        }

        private void Viewport_BrushPressed(object sender, BrushPressedEventArgs e)
        {
            Editor.Brush.BrushPress(e);
        }

        private void WidthSlider_ValueChanged(object sender, EventArgs e)
        {
            if (Editor.CurrentBrushWidth.HasValue && Editor.CurrentBrushWidth.Value != widthSlider.Value) Editor.Brush.Width = widthSlider.Value;
        }

        private void Editor_CurrentBrushWidthChanged(object sender, EventArgs e)
        {
            if (Editor.CurrentBrushWidth.HasValue)
            {
                UpdateCursorSize();
                widthSlider.Value = Editor.CurrentBrushWidth.Value;
            }
            else viewport.DestroyCustomCursor();
        }

        private void ColorPicker_ColorChanged(object sender, EventArgs e)
        {
            if (Editor.Brush.HasColor) Editor.Brush.Color = colorPicker.Color;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ActiveControl = viewport;
        }

        private void Form1_Disposed(object sender, EventArgs e)
        {
            if (colorPicker is not null)
            {
                if(Controls.Contains(colorPicker)) Controls.Remove(colorPicker);
                colorPicker.Dispose();
                colorPicker = null;
            }
            if (brushSel is not null)
            {
                if (Controls.Contains(brushSel)) Controls.Remove(brushSel);
                brushSel.Dispose();
                brushSel = null;
            }
            if(textEdit is not null)
            {
                if (Controls.Contains(textEdit)) Controls.Remove(textEdit);
                textEdit.Dispose();
                textEdit = null;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
        }

        public void AdjustSize()
        {
            Size sz = new Size(ClientSize.Width, ClientSize.Height - statusStrip1.Height);
            viewport.Location = new Point(MinToolboxWidth, 100);
            viewport.Size = new Size(sz.Width - MinToolboxWidth, sz.Height - 100);
            int w = MinToolboxWidth;
            int colPickW = (sz.Width - w - 10).GetInRange(300, 350);
            colorPicker.Size = new Size(colPickW, colPickW + 155);
            brushSel.Top = colorPicker.Bottom + 5;
            brushSel.Size = new Size(colPickW, (sz.Height - brushSel.Top) / 2 - 10);
            widthSlider.Location = new Point(colorPicker.Right, (50 - toolStrip1.Height) / 2 + toolStrip1.Height);
            textEdit.Location = new Point(colorPicker.Right + 10, 105);
            textEdit.Size = new Size(sz.Width - textEdit.Left - 5, 50);
            layerEditor.Top = brushSel.Bottom + 3;
            layerEditor.Size = new Size(colPickW, (sz.Height - brushSel.Top) / 2 - 10);
            UpdateCursorSize();
        }
        public const int MinToolboxWidth = 360;
        private void OpenImageClick(object sender, EventArgs e)
        {
            viewport.Enabled = false;
            using OpenFileDialog ofd = new()
            {
                Filter = "Image Files|*.bmp;*.gif;*.jpeg;*.jpg;*.png;*tiff;*.tif|All Files|*|PNG Files|*.png|JPEG Files|*.jpeg;*.jpg|Bitmap Files|*.bmp|TIFF Files|*.tiff;*.tif|" +
                "GIF Files|*.gif"
            };
            if(ofd.ShowDialog(this) is DialogResult.OK)
            {
                viewport.DisposeLayers(false);
                System.IO.FileStream fs = System.IO.File.OpenRead(ofd.FileName);
                Image img = Image.FromStream(fs);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
                fs.Dispose();
                DrawingLayer l = new DrawingLayer((Bitmap)Image.FromStream(ms), string.Format(Properties.Strings.DefaultLayerName, 1));
                viewport.ImageSize = l.Bitmap.Size;
                viewport.AddLayer(l);
                AdjustSize();
            }
            viewport.Enabled = true;
        }
        protected override bool ProcessTabKey(bool forward)
        {
            return false;
        }
        private void CanvasMouseDown(object sender, MouseEventArgs e)
        {
            ActiveControl = viewport;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ActiveControl is null || !ActiveControl.GetType().IsAssignableTo(typeof(TextBoxBase)))
            {
                Brush b;
                switch (keyData)
                {
                    case Keys.S:
                        if (Editor.Brush.Id is BrushRack.RectSelectId) Editor.Brush = BrushRack.Brushes[BrushRack.SelectBrushId];
                        else Editor.Brush = BrushRack.Brushes[BrushRack.RectSelectId];
                        break;
                    case Keys.B:
                        Editor.Brush = BrushRack.Brushes[BrushRack.PaintBrushId];
                        break;
                    case Keys.P:
                        Editor.Brush = BrushRack.Brushes[BrushRack.PixelPerfectId];
                        break;
                    case Keys.X:
                        colorPicker.SwapColors();
                        break;
                    case Keys.F:
                        Editor.Brush = BrushRack.Brushes[BrushRack.PaintBucketId];
                        break;
                    case Keys.T:
                        Editor.Brush = BrushRack.Brushes[BrushRack.TextBrushId];
                        break;
                    case Keys.K:
                        Editor.Brush = BrushRack.Brushes[BrushRack.ColorPickerId];
                        break;
                    case Keys.Back:
                        b = new SolidBrush(colorPicker.Color);
                        Editor.EditingLayer.Graphics.FillRegion(b, Editor.SelectionRegion);
                        b.Dispose();
                        viewport.Invalidate();
                        break;
                    case Keys.Delete:
                        b = new SolidBrush(Color.Transparent);
                        System.Drawing.Drawing2D.CompositingMode m = Editor.EditingLayer.Graphics.CompositingMode;
                        Editor.EditingLayer.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                        Editor.EditingLayer.Graphics.FillRegion(b, Editor.SelectionRegion);
                        Editor.EditingLayer.Graphics.CompositingMode = m;
                        Editor.MakeEmptySelectionRegion();
                        b.Dispose();
                        break;
                    case Keys.OemOpenBrackets:
                        if (Editor.Brush.HasWidth) Editor.Brush.Width = (Editor.Brush.Width - 1d).GetInRange(1, int.MaxValue);
                        break;
                    case Keys.OemCloseBrackets:
                        if (Editor.Brush.HasWidth) Editor.Brush.Width += 1d;
                        break;
                    case Keys.Control | Keys.Oemplus:
                        if(viewport.ViewZoom / 10d + viewport.ViewZoom < double.MaxValue) viewport.ViewZoom += viewport.ViewZoom / 10d;
                        break;
                    case Keys.Control | Keys.OemMinus:
                        if (viewport.ViewZoom - viewport.ViewZoom / 10d > double.MinValue) viewport.ViewZoom -= viewport.ViewZoom / 10d;
                        break;
                    case Keys.Control | Keys.B:
                        BrushDialog bd = new BrushDialog();
                        if (bd.ShowDialog(this) is DialogResult.OK)
                        {
                            Editor.Brush = bd.Brush;
                        }
                        bd.Dispose();
                        break;
                    case Keys.Control | Keys.OemOpenBrackets:
                        if (Editor.Brush.HasWidth) Editor.Brush.Width = (Editor.Brush.Width - 5d).GetInRange(1, int.MaxValue);
                        break;
                    case Keys.Control | Keys.OemCloseBrackets:
                        if (Editor.Brush.HasWidth) Editor.Brush.Width += 5d;
                        break;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SelectAllEditClicked(object sender, EventArgs e)
        {
            Editor.SelectionRegion.MakeEmpty();
            Editor.ComplementSelectionRegion(new Rectangle(Point.Empty, Editor.EditingImage.Size));
        }

        private void NegateSelectionEditClicked(object sender, EventArgs e)
        {
            if (Editor.SelectionRegion.IsEmpty(Editor.EditingLayer.Graphics)) ImageEditing.NegateRegion(Editor.EditingImage, Editor.EditingLayer.ImageRect);
            else ImageEditing.NegateRegion(Editor.EditingImage, Editor.SelectionRegion);
            viewport.Invalidate();
        }

        private void GrayscaleSelectionEditClicked(object sender, EventArgs e)
        {
            if (Editor.SelectionRegion.IsEmpty(Editor.EditingLayer.Graphics)) ImageEditing.GrayscaleRegion(Editor.EditingImage, Editor.EditingLayer.ImageRect);
            else ImageEditing.GrayscaleRegion(Editor.EditingImage, Editor.SelectionRegion);
            viewport.Invalidate();
        }

        private void ResizeMenuClicked(object sender, EventArgs e)
        {
            ResizeDialog rd = new ResizeDialog();
            if (rd.ShowDialog(this) is DialogResult.OK)
            {
                viewport.SetImageSize(new Size(rd.PixelsWidth, rd.PixelsHeight));
            }
            rd.Dispose();
        }

        private void SaveImageClick(object sender, EventArgs e)
        {
            viewport.Enabled = false;
            using SaveFileDialog ofd = new()
            {
                Filter = "PNG Files|*.png|All Files|*|JPEG Files|*.jpeg;*.jpg|Bitmap Files|*.bmp" +
                "|TIFF Files|*.tiff;*.tif|GIF Files|*.gif"
            };
            if (ofd.ShowDialog(this) is DialogResult.OK)
            {
                Editor.EditingImage.Save(ofd.FileName);
            }
            viewport.Enabled = true;
        }

        private void RulerChecked(object sender, EventArgs e)
        {
            viewport.ShowRuler = rulerToolStripMenuItem.Checked;
        }

        private void ProtractorChecked(object sender, EventArgs e)
        {
            viewport.ShowProtractor = protractorToolStripMenuItem.Checked;
        }

        private void PixelGridMenuClicked(object sender, EventArgs e)
        {
            viewport.ShowPixelGrid = pixelGridToolStripMenuItem.Checked;
        }

        private void ResetViewMenuClicked(object sender, EventArgs e)
        {
            viewport.ViewOffset = PointD.Empty;
            viewport.ViewZoom = 1d;
        }

        private void CopyMenuClicked(object sender, EventArgs e)
        {
            CopySelection();
        }
        public void CopySelection()
        {
            RectangleF r = Editor.SelectionRegion.GetBounds(Editor.EditingLayer.Graphics);
            Bitmap bmp = new Bitmap((int)MathF.Ceiling(r.Size.Width), (int)MathF.Ceiling(r.Size.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.DrawImage(Editor.EditingImage, new RectangleF(PointF.Empty, r.Size), r, GraphicsUnit.Pixel);
            Region rgn = new Region(r.InflateToRectangle());
            rgn.Exclude(Editor.SelectionRegion);
            g.FillRegion(Brushes.Transparent, rgn);
            rgn.Dispose();
            g.Dispose();
            Clipboard.SetImage(bmp);
            bmp.Dispose();
        }

        private void CutMenuClicked(object sender, EventArgs e)
        {
            CopySelection();
            Brush b = new SolidBrush(Color.Transparent);
            System.Drawing.Drawing2D.CompositingMode m = Editor.EditingLayer.Graphics.CompositingMode;
            Editor.EditingLayer.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            Editor.EditingLayer.Graphics.FillRegion(b, Editor.SelectionRegion);
            Editor.EditingLayer.Graphics.CompositingMode = m;
            Editor.MakeEmptySelectionRegion();
            b.Dispose();
        }

        private void PasteMenuClicked(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {

            }
            else MessageBox.Show(this, Properties.Strings.ClipboardNoImage, Properties.Strings.PasteFail, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private string textBrushText = "";
        private Font textBrushFont;
        private void KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (Controls.Contains(textEdit))
            {
                textBrushText += e.KeyChar;
                viewport.Invalidate(viewport.ImageRect.InflateToRectangle());
            }
        }

        private void TileViewMenuClicked(object sender, EventArgs e)
        {
            viewport.TileMode = tileViewToolStripMenuItem.Checked;
        }
    }
}
