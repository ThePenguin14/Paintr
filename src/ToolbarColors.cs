using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Paintr
{
    public class ToolbarColors : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Styling.BackgroundColor;
        public override Color CheckBackground => Styling.BackgroundColor;
        public override Color CheckPressedBackground => Styling.BackgroundColor;
        public override Color CheckSelectedBackground => Styling.BackgroundColor;
        public override Color ToolStripBorder => Styling.TextDisabledColor;
        public override Color ButtonSelectedHighlight => Styling.ButtonBackgroundColor;
        public override Color SeparatorLight => Styling.TextDisabledColor;
        public override Color SeparatorDark => Styling.TextUnsavedColor;
        public override Color MenuItemSelected => Styling.ButtonBackgroundColor;
        public override Color StatusStripBorder => Styling.TextDisabledColor;
        public override Color GripLight => Styling.BackgroundColor;
        public override Color GripDark => Styling.BackgroundColor;
    }
    public class ToolbarRenderer : ToolStripProfessionalRenderer
    {
        private Bitmap checkedIcon;
        public ToolbarRenderer() : base(new ToolbarColors())
        {
            checkedIcon = Styling.ColorImage(Styling.CheckBoxCheckedPath, Styling.TextColor);
        }
        ~ToolbarRenderer()
        {
            if (checkedIcon is not null) {
                checkedIcon.Dispose();
                checkedIcon = null;
            }
        }
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (checkedIcon is not null) e.Graphics.DrawImage(checkedIcon, e.ImageRectangle);
            else base.OnRenderItemCheck(e);
        }
        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {

        }
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            using Brush b = new SolidBrush(Styling.BackgroundColor);
            e.Graphics.FillRectangle(b, e.AffectedBounds);
            b.Dispose();
        }
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Styling.TextColor;
            base.OnRenderItemText(e);
        }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                Brush b = new SolidBrush(ColorTable.ButtonSelectedHighlight);
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
                b.Dispose();
            }
            else base.OnRenderMenuItemBackground(e);
        }
        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                Brush b = new SolidBrush(ColorTable.ButtonSelectedHighlight);
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
                b.Dispose();
            }
            else base.OnRenderItemBackground(e);
        }
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                Brush b = new SolidBrush(ColorTable.ButtonSelectedHighlight);
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
                b.Dispose();
            }
            else base.OnRenderDropDownButtonBackground(e);
        }
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                Brush b = new SolidBrush(ColorTable.ButtonSelectedHighlight);
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
                b.Dispose();
            }
            else base.OnRenderButtonBackground(e);
        }
    }
}
