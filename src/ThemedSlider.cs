using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paintr
{
    public partial class ThemedSlider : Control
    {
        public ThemedSlider()
        {
            BackColor = Styling.BackgroundColor.GetStandOutColor(4);
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            buttonBack = new SolidBrush(Styling.ButtonBackgroundColor);
            contrast = new SolidBrush(Styling.ContrastBackgroundColor);
            textBrush = new SolidBrush(Styling.TextColor);
            InitializeComponent();
            Disposed += ThemedSlider_Disposed;
            MouseMove += ThemedSlider_MouseMove;
            MouseDown += ThemedSlider_MouseDown;
            SizeChanged += ThemedSlider_SizeChanged;
        }

        private void ThemedSlider_SizeChanged(object sender, EventArgs e)
        {
            Brush b = contrast;
            contrast = new System.Drawing.Drawing2D.LinearGradientBrush(BarArea, Styling.ContrastBackgroundColor, Styling.ContrastBackgroundColor.GetStandOutColor(100),
                System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
            if (b is not null) b.Dispose();
        }

        private void ThemedSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left)
            {
                Point pt = PointToClient(Cursor.Position);
                if (BarArea.Contains(pt))
                {
                    UserSetValue((((double)pt.X - BarArea.X) / BarArea.Width).GetInRange(0d, 1d) * (MaximumValue - MinimumValue) + MinimumValue);
                    mouseDownIn = 1;
                }
                else if (new Rectangle(0, 0, ClientSize.Height, ClientSize.Height).Contains(pt))
                {
                    double step = ModifierKeys is Keys.Control ? Step * 5 : Step;
                    if (Value - step >= MinimumValue) UserSetValue(Value - step);
                    mouseDownIn = 2;
                }
                else if (new Rectangle(ClientSize.Width - ClientSize.Height, 0, ClientSize.Height, ClientSize.Height).Contains(pt))
                {
                    double step = ModifierKeys is Keys.Control ? Step * 5 : Step;
                    if (Value + step <= MaximumValue) UserSetValue(Value + step);
                    mouseDownIn = 3;
                }
                else mouseDownIn = 0;
            }
        }
        private byte mouseDownIn = 0;
        private void ThemedSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button is MouseButtons.Left)
            {
                Point pt = PointToClient(Cursor.Position);
                switch (mouseDownIn)
                {
                    case 0:
                        return;
                    case 1:
                        UserSetValue((((double)pt.X - BarArea.X) / BarArea.Width).GetInRange(0d, 1d) * (MaximumValue - MinimumValue) + MinimumValue);
                        return;
                }
            }
        }

        private void ThemedSlider_Disposed(object sender, EventArgs e)
        {
            if (buttonBack is not null)
            {
                buttonBack.Dispose();
                buttonBack = null;
                contrast.Dispose();
                contrast = null;
                textBrush.Dispose();
                textBrush = null;
            }
        }
        private Brush buttonBack;
        private Brush contrast;
        private Brush textBrush;
        private double value;
        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                Invalidate(BarArea);
                if (ValueChanged is not null) ValueChanged(this, new EventArgs());
            }
        }
        public event EventHandler ValueChanged;
        protected void UserSetValue(double value)
        {
            this.value = value;
            Invalidate(BarArea);
            if (UserInput is not null) UserInput(this, new EventArgs());
            if (ValueChanged is not null) ValueChanged(this, new EventArgs());
        }
        public event EventHandler UserInput;
        private double minValue;
        public double MinimumValue
        {
            get => minValue;
            set
            {
                minValue = value;
                Invalidate(BarArea);
                if (MinimumValueChanged is not null) MinimumValueChanged(this, new EventArgs());
            }
        }
        public event EventHandler MinimumValueChanged;
        private double maxValue = 100d;
        public double MaximumValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                Invalidate(BarArea);
                if (MaximumValueChanged is not null) MaximumValueChanged(this, new EventArgs());
            }
        }
        public event EventHandler MaximumValueChanged;
        private double step = 1d;
        public double Step
        {
            get => step;
            set
            {
                step = value;
                if (StepChanged is not null) StepChanged(this, new EventArgs());
            }
        }
        public event EventHandler StepChanged;
        private uint decimalPlaces = 3U;
        public uint DecimalPlaces
        {
            get => decimalPlaces;
            set
            {
                decimalPlaces = value;
                Invalidate(BarArea);
                if (DecimalPlacesChanged is not null) DecimalPlacesChanged(this, new EventArgs());
            }
        }
        public event EventHandler DecimalPlacesChanged;
        private Rectangle BarArea
        {
            get => new Rectangle(ClientSize.Height, 0, ClientSize.Width - ClientSize.Height * 2, ClientSize.Height);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Size sz = ClientSize;
            int h = sz.Height - sz.Height % 4;
            e.Graphics.FillRectangle(buttonBack, new Rectangle(0, 0, sz.Height, sz.Height));
            e.Graphics.FillRectangle(buttonBack, new Rectangle(sz.Width - sz.Height, 0, sz.Height, sz.Height));
            e.Graphics.FillRectangle(textBrush, h / 4, h / 2 - 2, h / 2, 4);
            e.Graphics.FillRectangle(textBrush, sz.Width - (h * 3 / 4), h / 2 - 2, h / 2, 4);
            e.Graphics.FillRectangle(textBrush, sz.Width - (h / 2) - 2, h / 4, 4, h / 2);
            e.Graphics.FillRectangle(contrast, new RectangleF(ClientSize.Height, 0,
                (float)(Value / (MaximumValue - MinimumValue) * BarArea.Width), ClientSize.Height));
            e.Graphics.DrawString(Value.ToString($"F{DecimalPlaces}"), Font, textBrush, BarArea);
        }
    }
}
