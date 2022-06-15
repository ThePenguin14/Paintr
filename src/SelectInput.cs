using System;
using System.Collections;
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
    public interface IItemsModifiable
    {
        public void ItemCollectionModified();
    }
    public partial class SelectInput : Control, IItemsModifiable
    {
        public SelectInput()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Styling.ButtonBackgroundColor;
            textBrush = new SolidBrush(Styling.TextColor);
            borderPen = new Pen(Styling.ButtonBorderColor, 1f);
            contrast = new SolidBrush(Styling.ContrastBackgroundColor);
            format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            InitializeComponent();
            items = new(this);
            Disposed += SelectInput_Disposed;
            SizeChanged += SelectInput_SizeChanged;
        }

        private void SelectInput_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        ~SelectInput()
        {
            if (Editor.ScrollStealers.Contains(this)) Editor.ScrollStealers.Remove(this);
        }
        private Brush textBrush;
        private Pen borderPen;
        private Brush contrast;
        private StringFormat format;
        private int hoverIndex;
        private void SelectInput_Disposed(object sender, EventArgs e)
        {
            if (textBrush is not null)
            {
                textBrush.Dispose();
                textBrush = null;
            }
            if (borderPen is not null)
            {
                borderPen.Dispose();
                borderPen = null;
            }
            if (contrast is not null)
            {
                contrast.Dispose();
                contrast = null;
            }
            if (format is not null)
            {
                format.Dispose();
                format = null;
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (Opened)
            {
                if ((DateTime.Now - keyTime).TotalSeconds > 1d) keyString = "";
                keyString += e.KeyChar;
                SelectInputItem item = Items.FirstOrDefault((SelectInputItem item) => item.Name.StartsWith(keyString, StringComparison.OrdinalIgnoreCase));
                if (item is not null)
                {
                    SelectedIndex = Items.FirstIndex((SelectInputItem it) => it == item);
                }
                keyTime = DateTime.Now;
            }
            base.OnKeyPress(e);
        }
        private DateTime keyTime;
        private string keyString = "";
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Opened)
            {
                scrollTop = (scrollTop - e.Delta).GetInRange(0, (int)MathF.Ceiling(Items.Count * (measureSize.Height + 5f)) - Height + closedHeight);
                Point pt = PointToClient(Cursor.Position);
                if (pt.Y > closedHeight && pt.Y <= Items.Count * (measureSize.Height + 5f) + closedHeight)
                {
                    int inx = (int)Math.Floor((pt.Y - closedHeight) / (measureSize.Height + 5d) + scrollTop / (measureSize.Height + 5d));
                    if (inx >= 0 && inx < Items.Count) hoverIndex = inx;
                    Invalidate();
                }
            }
            Invalidate();
            base.OnMouseWheel(e);
        }
        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (value < 0 && value >= Items.Count) throw new ArgumentOutOfRangeException();
                selectedIndex = value;
                scrollTop = (int)(SelectedIndex * (measureSize.Height + 5d)) + closedHeight;
                if (Opened)
                {
                    Point pt = PointToClient(Cursor.Position);
                    if (pt.Y > closedHeight && pt.Y <= Items.Count * (measureSize.Height + 5f) + closedHeight)
                    {
                        int inx = (int)Math.Floor((pt.Y - closedHeight) / (measureSize.Height + 5d) + scrollTop / (measureSize.Height + 5d));
                        if (inx >= 0 && inx < Items.Count) hoverIndex = inx;
                    }
                }
                Invalidate();
                if (SelectedIndexChanged is not null) SelectedIndexChanged(this, new EventArgs());
            }
        }
        public event EventHandler SelectedIndexChanged;
        public object SelectedItem { get => Items[SelectedIndex].Item; }
        private bool opened;
        public bool Opened
        {
            get => opened;
            set
            {
                bool old = opened;
                opened = value;
                if (old != opened)
                {
                    if (opened)
                    {
                        scrollTop = 0;
                        hoverIndex = selectedIndex;
                        closedHeight = ClientSize.Height;
                        suppressSizeChange = true;
                        Height = closedHeight + (int)(Items.Count * (measureSize.Height > 0f ? measureSize.Height + 5d : closedHeight));
                        suppressSizeChange = false;
                        Editor.ScrollStealers.Add(this);
                        BringToFront();
                    }
                    else
                    {
                        suppressSizeChange = true;
                        Height = closedHeight;
                        suppressSizeChange = false;
                        Editor.ScrollStealers.Remove(this);
                    }
                }
                Invalidate();
                if (OpenedChanged is not null) OpenedChanged(this, new EventArgs());
            }
        }
        public event EventHandler OpenedChanged;
        private int closedHeight;
        private bool suppressSizeChange;
        private int scrollTop;
        protected override void OnClick(EventArgs e)
        {
            if (!Opened)
            {
                Focus();
                Opened = true;
            }
            else
            {
                Opened = false;
                Point pt = PointToClient(Cursor.Position);
                if (pt.Y > closedHeight && pt.Y <= Items.Count * (measureSize.Height + 5f) + closedHeight)
                {
                    int inx = (int)Math.Floor((pt.Y - closedHeight) / (measureSize.Height + 5d) + scrollTop / (measureSize.Height + 5d));
                    if (inx >= 0 && inx < Items.Count)
                    {
                        selectedIndex = inx;
                        Invalidate();
                        if (SelectedIndexChanged is not null) SelectedIndexChanged(this, new EventArgs());
                    }
                }
            }
            base.OnClick(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Opened)
            {
                Point pt = PointToClient(Cursor.Position);
                if (pt.Y > closedHeight && pt.Y <= Items.Count * (measureSize.Height + 5f) + closedHeight)
                {
                    int inx = (int)Math.Floor((pt.Y - closedHeight) / (measureSize.Height + 5d) + scrollTop / (measureSize.Height + 5d));
                    if (inx >= 0 && inx < Items.Count) hoverIndex = inx;
                    Invalidate();
                }
            }
            base.OnMouseMove(e);
        }
        private SizeF measureSize = new SizeF(100f, 60f);
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Opened)
            {
                SizeF sz = selectedIndex < Items.Count ? pe.Graphics.MeasureString(Items[selectedIndex].Name, Font) : new Size(10, closedHeight - 10);
                measureSize = sz;
                SizeF cs = ClientSize;
                if(selectedIndex < Items.Count) pe.Graphics.DrawString(Items[selectedIndex].Name, Font, textBrush, 3, (closedHeight - sz.Height) / 2);
                pe.Graphics.DrawRectangle(borderPen, 0f, 0f, cs.Width - 1f, closedHeight - 1f);
                pe.Graphics.SetClip(new RectangleF(0f, closedHeight, cs.Width, cs.Height - closedHeight));
                for (int i = 0; i < Items.Count; i++)
                {
                    if(i == hoverIndex) pe.Graphics.FillRectangle(contrast, 0f, (sz.Height + 5f) * i + closedHeight - scrollTop, cs.Width, sz.Height + 5f);
                    pe.Graphics.DrawString(Items[i].Name, Font, textBrush, new RectangleF(3f, (sz.Height + 5f) * i + closedHeight - scrollTop,
                        cs.Width - 6f, sz.Height + 2f), format);
                }
                pe.Graphics.ResetClip();
                pe.Graphics.DrawRectangle(borderPen, 0, 0, cs.Width - 1, cs.Height - 1);
                pe.Graphics.FillPolygon(textBrush, new PointF[] { new PointF(cs.Width - 17f, closedHeight  / 2f + 4f), new PointF(cs.Width - 9f, closedHeight / 2f + 4f),
                    new PointF(cs.Width - 13f, closedHeight / 2f - 4f) });
            }
            else
            {
                SizeF sz = selectedIndex < Items.Count ? pe.Graphics.MeasureString(Items[selectedIndex].Name, Font) : measureSize;
                measureSize = sz;
                SizeF cs = ClientSize;
                if(selectedIndex < Items.Count)
                    pe.Graphics.DrawString(Items[selectedIndex].Name, Font, textBrush, RectangleF.FromLTRB(3f, (cs.Height - sz.Height) / 2, cs.Width - 26f,
                        cs.Height - 3f));
                pe.Graphics.DrawRectangle(borderPen, 0f, 0f, cs.Width - 1, cs.Height - 1);
                if (Focused) pe.Graphics.DrawRectangle(borderPen, 2f, 2f, cs.Width - 5f, closedHeight - 5f);

                pe.Graphics.FillPolygon(textBrush, new PointF[] { new PointF(cs.Width - 17f, cs.Height / 2f - 4f), new PointF(cs.Width - 9f, cs.Height / 2f - 4f),
                    new PointF(cs.Width - 13f, cs.Height / 2f + 4f) });
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            Opened = false;
            base.OnLostFocus(e);
        }
        public void ItemCollectionModified()
        {
            Invalidate();
        }
        protected override void SetClientSizeCore(int x, int y)
        {
            if (Opened && !suppressSizeChange)
            {
                Opened = false;
                base.SetClientSizeCore(x, y);
                Opened = true;
            }
            else base.SetClientSizeCore(x, y);
        }
        private SelectInputItemCollection items;
        public SelectInputItemCollection Items { get => items; }
    }
    public class SelectInputItemCollection : ICollection<SelectInputItem>, IEnumerable, IEnumerable<SelectInputItem>, IList<SelectInputItem>,
        IReadOnlyCollection<SelectInputItem>, IReadOnlyList<SelectInputItem>, ICollection, IList
    {
        public SelectInputItemCollection(IItemsModifiable inp)
        {
            this.inp = inp;
            list = new();
        }
        ~SelectInputItemCollection()
        {
            foreach (SelectInputItem item in list) item.Owner = null;
        }
        private IItemsModifiable inp;
        public IItemsModifiable Owner { get => inp; }

        public int Count => ((ICollection<SelectInputItem>)list).Count;

        public bool IsReadOnly => ((ICollection<SelectInputItem>)list).IsReadOnly;

        public bool IsSynchronized => ((ICollection)list).IsSynchronized;

        public object SyncRoot => ((ICollection)list).SyncRoot;

        public bool IsFixedSize => ((IList)list).IsFixedSize;

        object IList.this[int index] { get => ((IList)list)[index]; set => ((IList)list)[index] = value; }

        private List<SelectInputItem> list;
        public void Add(SelectInputItem item)
        {
            item.Owner = this;
            list.Add(item);
            inp.ItemCollectionModified();
        }
        public void AddRange(SelectInputItem[] items)
        {
            foreach (SelectInputItem item in items) item.Owner = this;
            list.AddRange(items);
            inp.ItemCollectionModified();
        }
        public void Insert(int index, SelectInputItem item)
        {
            item.Owner = this;
            list.Insert(index, item);
            inp.ItemCollectionModified();
        }
        public void InsertRange(int index, SelectInputItem[] items)
        {
            foreach (SelectInputItem item in items) item.Owner = this;
            list.InsertRange(index, items);
            inp.ItemCollectionModified();
        }
        public void Remove(SelectInputItem item)
        {
            item.Owner = null;
            list.Remove(item);
            inp.ItemCollectionModified();
        }
        public void RemoveAt(int index)
        {
            list[index].Owner = null;
            list.RemoveAt(index);
        }
        public void RemoveRange(int start, int count)
        {
            for(int i = start; i < start + count; i++) list[i].Owner = null;
            list.RemoveRange(start, count);
        }
        public int FirstIndex(Predicate<SelectInputItem> match)
        {
            return list.FindIndex(match);
        }

        public IEnumerator<SelectInputItem> GetEnumerator()
        {
            return ((IEnumerable<SelectInputItem>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }

        public void Clear()
        {
            foreach (SelectInputItem item in list) item.Owner = null;
            list.Clear();
            Owner.ItemCollectionModified();
        }

        public bool Contains(SelectInputItem item)
        {
            return list.Contains(item);
        }

        public void CopyTo(SelectInputItem[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        bool ICollection<SelectInputItem>.Remove(SelectInputItem item)
        {
            item.Owner = null;
            return list.Remove(item);
        }

        public int IndexOf(SelectInputItem item)
        {
            return list.IndexOf(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)list).CopyTo(array, index);
        }

        public int Add(object value)
        {
            if (value is SelectInputItem item) item.Owner = this;
            return ((IList)list).Add(value);
        }

        public bool Contains(object value)
        {
            return ((IList)list).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)list).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            if (value is SelectInputItem item) item.Owner = this;
            ((IList)list).Insert(index, value);
        }

        public void Remove(object value)
        {
            if (value is SelectInputItem item) item.Owner = null;
            ((IList)list).Remove(value);
        }

        public SelectInputItem this[int i]
        {
            get => list[i];
            set
            {
                value.Owner = this;
                list[i] = value;
                inp.ItemCollectionModified();
            }
        }
    }
    public class SelectInputItem
    {
        public SelectInputItem(string name, object item)
        {
            this.name = name;
            this.item = item;
        }
        private string name;
        public virtual string Name
        {
            get => name;
            set
            {
                name = value is null ? "" : value;
                owner.Owner.ItemCollectionModified();
            }
        }
        private object item;
        public virtual object Item
        {
            get => item;
            set
            {
                item = value;
                owner.Owner.ItemCollectionModified();
            }
        }
        private SelectInputItemCollection owner;
        public SelectInputItemCollection Owner
        {
            get => owner;
            set
            {
                if (owner is not null && value is not null) throw new InvalidOperationException();
                owner = value;
            }
        }
    }
}
