using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paintr.Dialog
{
    public partial class LayerProperties : Form
    {
        private ThemedTextBox opacityBox;
        private ThemedSlider opacitySlider;
        private SelectInput blendModeSelect;
        private ThemedTextBox nameBox;
        private SelectInput shadersSelect;
        public LayerProperties()
        {
            BackColor = Styling.BackgroundColor;
            InitializeComponent();
            opacityLabel.Text = Properties.Strings.LabelLayerOpacity;
            Styling.FormatLabel(opacityLabel);
            okay.Text = Properties.Strings.ButtonConfirm;
            Styling.FormatButton(okay);
            cancel.Text = Properties.Strings.ButtonCancel;
            Styling.FormatButton(cancel);
            opacityBox = ThemedTextBox.PercentDoubleBox();
            opacityBox.InputDone += OpacityBox_InputDone;
            opacitySlider = new ThemedSlider() { MaximumValue = 1d, MinimumValue = 0d, Step = 0.01d };
            opacitySlider.UserInput += OpacitySlider_UserInput;
            Controls.Add(opacityBox);
            Controls.Add(opacitySlider);
            blendModeLabel.Text = Properties.Strings.LabelLayerBlendMode;
            Styling.FormatLabel(blendModeLabel);
            blendModeSelect = new SelectInput();
            foreach (BlendMode m in Enum.GetValues(typeof(BlendMode))) blendModeSelect.Items.Add(new SelectInputItem(m.ToString(), m));
            blendModeSelect.SelectedIndexChanged += BlendModeSelect_SelectedIndexChanged;
            Controls.Add(blendModeSelect);
            nameBox = new ThemedTextBox();
            nameBox.InputDone += NameBox_InputDone;
            Controls.Add(nameBox);
            Styling.FormatLabel(nameLabel);
            nameLabel.Text = Properties.Strings.LabelLayerName;
            shadersSelect = new SelectInput();
            foreach (KeyValuePair<string, ImageEdits> k in ImageEdits.PresetEdits) shadersSelect.Items.Add(new SelectInputItem(k.Value.Name, k.Value));
            shadersSelect.SelectedIndexChanged += ShadersSelect_SelectedIndexChanged;
            Controls.Add(shadersSelect);
            Styling.FormatLabel(shadersLabel);
            SizeChanged += LayerProperties_SizeChanged;
            AdjustSize();
            Load += LayerProperties_Load;
            Disposed += LayerProperties_Disposed;
        }

        private void ShadersSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (layer is not null) layer.ImageEdits = ImageEdits.PresetEdits.Values.CalcItemAt(shadersSelect.SelectedIndex);
        }

        private void LayerProperties_Disposed(object sender, EventArgs e)
        {
            if(nameBox is not null)
            {
                if (Controls.Contains(nameBox)) Controls.Remove(nameBox);
                nameBox.Dispose();
                nameBox = null;
                if (Controls.Contains(opacityBox)) Controls.Remove(opacityBox);
                opacityBox.Dispose();
                opacityBox = null;
                if (Controls.Contains(opacitySlider)) Controls.Remove(opacitySlider);
                opacitySlider.Dispose();
                opacitySlider = null;
                if (Controls.Contains(blendModeSelect)) Controls.Remove(blendModeSelect);
                blendModeSelect.Dispose();
                blendModeSelect = null;
                if (Controls.Contains(shadersSelect)) Controls.Remove(shadersSelect);
                shadersSelect.Dispose();
                shadersSelect = null;
            }
            SelectedLayer = null;
        }

        private void LayerProperties_Load(object sender, EventArgs e)
        {
            blendModeSelect.Focus();
        }
        private void NameBox_InputDone(object sender, EventArgs e)
        {
            if (layer is not null) layer.Name = nameBox.SavedText;
        }

        private void BlendModeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (layer is not null) layer.BlendMode = (BlendMode)blendModeSelect.SelectedItem;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if(DialogResult is not DialogResult.OK && layer is not null)
            {
                layer.LayerOpacity = originalOpacity;
                layer.BlendMode = originalBlendMode;
            }
            base.OnFormClosing(e);
        }
        private void OpacityBox_InputDone(object sender, EventArgs e)
        {
            if (layer is not null) layer.LayerOpacity = double.Parse(opacityBox.SavedText.Replace("%", "")) / 100d;
        }

        private void OpacitySlider_UserInput(object sender, EventArgs e)
        {
            if (layer is not null) layer.LayerOpacity = opacitySlider.Value;
        }

        private DrawingLayer layer;
        public DrawingLayer SelectedLayer
        {
            get => layer;
            set
            {
                if(layer is not null)
                {
                    layer.LayerOpacityChanged -= Layer_LayerOpacityChanged;
                    layer.BlendModeChanged -= Layer_BlendModeChanged;
                    layer.NameChanged -= Layer_NameChanged;
                    layer.ImageEditsChanged -= Layer_ImageEditsChanged;
                }
                layer = value;
                if (layer is not null)
                {
                    layer.LayerOpacityChanged += Layer_LayerOpacityChanged;
                    layer.BlendModeChanged += Layer_BlendModeChanged;
                    layer.NameChanged += Layer_NameChanged;
                    layer.ImageEditsChanged += Layer_ImageEditsChanged;
                    originalOpacity = layer.LayerOpacity;
                    originalBlendMode = layer.BlendMode;
                }
                UpdateName();
                UpdateOpacity();
                UpdateBlendMode();
                if (SelectedLayerChanged is not null) SelectedLayerChanged(this, new EventArgs());
            }
        }

        private void Layer_ImageEditsChanged(object sender, EventArgs e)
        {
            UpdateShaders();
        }

        private void Layer_NameChanged(object sender, EventArgs e)
        {
            UpdateName();
        }

        protected double originalOpacity;
        protected BlendMode originalBlendMode;
        private void Layer_BlendModeChanged(object sender, EventArgs e)
        {
            UpdateBlendMode();
        }

        private void Layer_LayerOpacityChanged(object sender, EventArgs e)
        {
            UpdateOpacity();
        }
        public event EventHandler SelectedLayerChanged;
        private void LayerProperties_SizeChanged(object sender, EventArgs e)
        {
            AdjustSize();
        }

        public void AdjustSize()
        {
            Size sz = ClientSize;
            int r = Extensions.MathMax(nameLabel.Right, opacityLabel.Right, blendModeLabel.Right);
            nameBox.Location = new Point(r + 4, nameLabel.Top - 1);
            nameBox.Width = sz.Width - 8 - r;
            opacityBox.Location = new Point(r + 4, opacityLabel.Top - 1);
            opacitySlider.Location = new Point(opacityBox.Right + 4, opacityBox.Top);
            opacitySlider.Size = new Size(sz.Width - opacityBox.Right - 8, opacityBox.Height);
            blendModeSelect.Location = new Point(r + 4, blendModeLabel.Top - 1);
            blendModeSelect.MaximumSize = new Size(int.MaxValue, sz.Height - blendModeSelect.Bottom - 8);
            blendModeSelect.Size = new Size(sz.Width - r - 8, opacityBox.Height + 2);
            shadersSelect.Location = new Point(r + 4, shadersLabel.Top - 1);
            shadersSelect.MaximumSize = new Size(int.MaxValue, sz.Height - shadersSelect.Bottom - 8);
            shadersSelect.Size = new Size(sz.Width - r - 8, shadersLabel.Height + 2);
            cancel.Top = sz.Height - cancel.Height - cancel.Left;
            cancel.Width = (sz.Width - cancel.Left * 3) / 2;
            okay.Width = (sz.Width - cancel.Left * 3) / 2;
            okay.Location = new Point(cancel.Left * 2 + cancel.Width, sz.Height - cancel.Height - cancel.Left);
        }
        public void UpdateName()
        {
            if (layer is not null)
            {
                if (nameBox is null) return;
                nameBox.SetTextIfNeeded(layer.Name);
            }
        }
        public void UpdateOpacity()
        {
            if (layer is not null)
            {
                if (opacitySlider is null) return;
                if (opacitySlider.Value != layer.LayerOpacity) opacitySlider.Value = layer.LayerOpacity;
                opacityBox.SetTextIfNeeded(string.Format("{0:0.##}%", layer.LayerOpacity * 100d));
            }
        }
        public void UpdateBlendMode()
        {
            if (layer is not null)
            {
                if (blendModeSelect is null) return;
                if ((BlendMode)blendModeSelect.SelectedItem != layer.BlendMode) blendModeSelect.SelectedIndex = (int)layer.BlendMode;
            }
        }
        public void UpdateShaders()
        {
            if (layer is not null)
            {
                if (shadersSelect is null) return;
                if ((ImageEdits)shadersSelect.SelectedItem != layer.ImageEdits) shadersSelect.SelectedIndex = ImageEdits.PresetEdits.Values.CalcIndexOf(layer.ImageEdits);
            }
        }
        private void OpacityLabelClicked(object sender, EventArgs e)
        {
            opacityBox.Focus();
        }

        private void BlendModeLabelClicked(object sender, EventArgs e)
        {
            blendModeSelect.Focus();
        }

        private void NameLabelClicked(object sender, EventArgs e)
        {
            nameBox.Focus();
        }

        private void ShadersLabelClicked(object sender, EventArgs e)
        {
            shadersSelect.Focus();
        }
    }
}
