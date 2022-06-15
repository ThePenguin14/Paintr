using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paintr
{
    public static class Editor
    {
        static Editor()
        {
            brush = BrushRack.Brushes["base:paintbrush"];
            Brush = BrushRack.Brushes["base:paintbrush"];
        }
        public static Viewport Viewport { get; set; }
        public static LayerEditor LayerEditor { get; set; }
        public static Bitmap EditingImage { get => Viewport is not null ? Viewport.LayerAtIndex(Viewport.SelectedLayer).Bitmap : null; }
        public static DrawingLayer EditingLayer { get => Viewport is not null ? Viewport.LayerAtIndex(Viewport.SelectedLayer) : null; }
        private static DrawBrush brush;
        public static DrawBrush Brush
        {
            get => brush;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                DrawBrush old = brush;
                brush = value;
                if(old.HasWidth) Brush.WidthChanged -= EditorWidthChanged;
                if (brush.HasWidth) Brush.WidthChanged += EditorWidthChanged;
                if (BrushChanged is not null) BrushChanged(null, new PropertyChangedEventArgs<DrawBrush>(old));
            }
        }
        public static TextEdit TextEdit { get; set; }
        public static event EventHandler RequestShowTextEdit;
        public static void FireRequestShowTextEdit()
        {
            if (RequestShowTextEdit is not null) RequestShowTextEdit(null, new EventArgs());
        }
        public static event EventHandler RequestHideTextEdit;
        public static void FireRequestHideTextEdit()
        {
            if (RequestHideTextEdit is not null) RequestHideTextEdit(null, new EventArgs());
        }
        private static void EditorWidthChanged(object sender, EventArgs e)
        {
            if (CurrentBrushWidthChanged is not null) CurrentBrushWidthChanged(null, new EventArgs());
        }

        public static event PropertyChangedEventHandler<DrawBrush> BrushChanged;
        public static double? CurrentBrushWidth { get => Brush.HasWidth ? Brush.Width : null; }
        public static event EventHandler CurrentBrushWidthChanged;

        private static Region selectionRegion = new Region(Rectangle.Empty);
        public static Region SelectionRegion
        {
            get => selectionRegion;
        }
        public static event RegionModifiedEventHandler SelectionRegionModified;
        private static bool roundSelection = true;
        public static bool RoundSelection
        {
            get => roundSelection;
            set => roundSelection = value;
        }
        public static void ComplementSelectionRegion(Rectangle rect)
        {
            selectionRegion.Complement(rect);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Complement, rect));
        }
        public static void ComplementSelectionRegion(RectangleF rect)
        {
            selectionRegion.Complement(rect);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Complement, rect));
        }
        public static void ComplementSelectionRegion(System.Drawing.Drawing2D.GraphicsPath path)
        {
            selectionRegion.Complement(path);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Complement, path));
        }
        public static void ComplementSelectionRegion(Region region)
        {
            selectionRegion.Complement(region);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Complement, region));
        }
        public static void UnionSelectionRegion(Rectangle rect)
        {
            selectionRegion.Union(rect);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Union, rect));
        }
        public static void UnionSelectionRegion(RectangleF rect)
        {
            selectionRegion.Union(rect);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Union, rect));
        }
        public static void UnionSelectionRegion(System.Drawing.Drawing2D.GraphicsPath path)
        {
            selectionRegion.Union(path);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Union, path));
        }
        public static void UnionSelectionRegion(Region region)
        {
            selectionRegion.Union(region);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Union, region));
        }
        public static void ExcludeSelectionRegion(Rectangle rect)
        {
            selectionRegion.Exclude(rect);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Exclude, rect));
        }
        public static void ExcludeSelectionRegion(RectangleF rect)
        {
            selectionRegion.Exclude(rect);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Exclude, rect));
        }
        public static void ExcludeSelectionRegion(System.Drawing.Drawing2D.GraphicsPath path)
        {
            selectionRegion.Exclude(path);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Exclude, path));
        }
        public static void ExcludeSelectionRegion(Region region)
        {
            selectionRegion.Exclude(region);
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Exclude, region));
        }
        public static void MakeEmptySelectionRegion()
        {
            selectionRegion.MakeEmpty();
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.MakeEmpty));
        }
        public static void MakeInfiniteSelectionRegion()
        {
            selectionRegion.MakeInfinite();
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.MakeInfinite));
        }
        public static void FireSelectionRegionModified()
        {
            if (SelectionRegionModified is not null) SelectionRegionModified(null, new RegionModifiedEventArgs(RegionModificationMethod.Unknown));
        }
        public static ColorPicker ColorPicker { get; set; }
        public static List<object> ScrollStealers { get; } = new();
        public static string AppData { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "A Teal Penguin",
            "Paintr");
    }
    public delegate void PropertyChangedEventHandler<T>(object sender, PropertyChangedEventArgs<T> e);
    public class PropertyChangedEventArgs<T> : EventArgs
    {
        public PropertyChangedEventArgs(T old)
        {
            OldValue = old;
        }
        public T OldValue { get; }
    }
    public delegate void RegionModifiedEventHandler(object sender, RegionModifiedEventArgs e);
    public class RegionModifiedEventArgs : EventArgs
    {
        public RegionModifiedEventArgs(RegionModificationMethod t)
        {
            ModificationMethod = t;
        }
        public RegionModifiedEventArgs(RegionModificationMethod t, Rectangle r)
        {
            Rectangle = r;
            ModificationMethod = t;
            ModificationType = RegionModificationType.Rectangle;
        }
        public RegionModifiedEventArgs(RegionModificationMethod t, RectangleF r)
        {
            RectangleF = r;
            ModificationMethod = t;
            ModificationType = RegionModificationType.RectangleF;
        }
        public RegionModifiedEventArgs(RegionModificationMethod t, System.Drawing.Drawing2D.GraphicsPath path)
        {
            GraphicsPath = path;
            ModificationMethod = t;
            ModificationType = RegionModificationType.GraphicsPath;
        }
        public RegionModifiedEventArgs(RegionModificationMethod t, Region region)
        {
            Region = region;
            ModificationMethod = t;
            ModificationType = RegionModificationType.Region;
        }
        public Rectangle? Rectangle { get; }
        public RectangleF? RectangleF { get; }
        public System.Drawing.Drawing2D.GraphicsPath GraphicsPath { get; }
        public Region Region { get; }
        public RegionModificationMethod ModificationMethod { get; }
        public RegionModificationType ModificationType { get; }
    }
    public enum RegionModificationMethod
    {
        Unknown,
        Complement,
        Exclude,
        MakeEmpty,
        MakeInfinite,
        Union
    }
    public enum RegionModificationType
    {
        Unknown,
        Rectangle,
        RectangleF,
        GraphicsPath,
        Region
    }
}
