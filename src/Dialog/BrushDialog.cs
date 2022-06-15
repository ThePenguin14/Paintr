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
    public partial class BrushDialog : Form
    {
        public BrushDialog()
        {
            InitializeComponent();
            BackColor = Styling.BackgroundColor;
            listBox1.BackColor = Styling.ButtonBackgroundColor;
            listBox1.ForeColor = Styling.TextColor;
            Styling.FormatButton(cancel);
            Styling.FormatButton(okay);
            Text = Properties.Strings.WindowTitleSelectBrush;
            foreach (DrawBrush b in BrushRack.Brushes.Values) listBox1.Items.Add(b);
            if (BrushRack.Brushes.ContainsKey(Editor.Brush.Id)) listBox1.SelectedItem = BrushRack.Brushes[Editor.Brush.Id];
            listBox1.SelectedValueChanged += ListBox1_SelectedValueChanged;
        }

        private void ListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Brush = (DrawBrush)listBox1.SelectedItem;
        }

        private DrawBrush brush;
        public DrawBrush Brush
        {
            get => brush;
            set
            {
                brush = value;
                if (BrushChanged is not null) BrushChanged(this, new EventArgs());
            }
        }
        public event EventHandler BrushChanged;
    }
}
