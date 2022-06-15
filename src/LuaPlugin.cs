using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NLua;
using System.Reflection;
using System.Drawing;

namespace Paintr
{
    public class LuaPlugin
    {
        public LuaPlugin(string file)
        {
            if (file is null) throw new ArgumentNullException("", nameof(file));
            this.file = file;
        }
        private string file;
        public bool Run()
        {
            try
            {
                Lua.DoFile(file);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(Form.ActiveForm, $"Plugin error occurred (file {Path.GetFileName(file)}).\n{e.GetType().Name}: {e.Message}");
                return false;
            }
        }
        #region Static
        public static void StartPlugins()
        {
            if (!Directory.Exists(LuaPluginDirectory)) Directory.CreateDirectory(LuaPluginDirectory);
            if (!File.Exists(PrecedenceFile))
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                System.Xml.XmlElement root = xml.CreateElement("plugins");
                xml.AppendChild(root);
                xml.Save(PrecedenceFile);
            }
            Lua = new Lua();
            Lua.State.Encoding = Encoding.UTF8;
            Lua.DoString("import = function () error(\"Import is disabled.\",2) end");
            Lua.RegisterFunction("winMsg", typeof(LuaPlugin).GetMethod(nameof(LuaWindowsMsg)));
            Lua.RegisterFunction("setBrushColor", typeof(LuaPlugin).GetMethod(nameof(LuaSetColor)));
            Lua.RegisterFunction("color", typeof(LuaPlugin).GetMethod(nameof(LuaGetColorString)));
            Lua.RegisterFunction("rgb", typeof(LuaPlugin).GetMethod(nameof(LuaGetColorRgb)));
            Lua.RegisterFunction("argb", typeof(LuaPlugin).GetMethod(nameof(LuaGetColorArgb)));
            LuaPlugin[] plugs = LoadPlugins();
            foreach (LuaPlugin p in plugs) p.Run();
            RunningPlugins.AddRange(plugs);
        }
        public static List<LuaPlugin> RunningPlugins { get; } = new();
        public static NLua.Lua Lua { get; private set; }
        public static LuaPlugin[] LoadPlugins()
        {
            System.Xml.XmlDocument xml = new();
            FileStream fs = File.OpenRead(PrecedenceFile);
            xml.Load(fs);
            List<LuaPlugin> plugs = new();
            foreach(System.Xml.XmlNode n in xml.DocumentElement.SelectNodes("/plugins/plugin[@enabled='true']"))
            {
                plugs.Add(new LuaPlugin(Path.Combine(LuaPluginDirectory, $"{n.InnerText}.lua")));
            }
            fs.Dispose();
            return plugs.ToArray();
        }
        public static string LuaPluginDirectory { get; set; } = Path.Combine(Editor.AppData, "LuaPlugins");
        public static string PrecedenceFile { get; set; } = Path.Combine(Editor.AppData, "LuaPlugins.xml");
        #endregion
        #region Lua Functions
        public static string LuaWindowsMsg(string text, string title = "", string buttons = nameof(MessageBoxButtons.OK), string icon = nameof(MessageBoxIcon.None),
            string defButton = nameof(MessageBoxDefaultButton.Button1), string options = "None")
        {
            if (!Enum.TryParse(typeof(MessageBoxButtons), buttons, true, out object b)) throw new LuaArgumentException("Invalid button name.", nameof(buttons));
            if (!Enum.TryParse(typeof(MessageBoxIcon), icon, true, out object i)) throw new LuaArgumentException("Invalid icon name.", nameof(icon));
            if (!Enum.TryParse(typeof(MessageBoxDefaultButton), defButton, true, out object db))
                throw new LuaArgumentException("Invalid default button name. Must be 'Button1', 'Button2', 'Button3', or 'Button4'.", nameof(defButton));
            if (!Enum.TryParse(typeof(MessageBoxOptions), options, true, out object o))
            {
                if (options.Equals("None", StringComparison.OrdinalIgnoreCase)) o = (MessageBoxOptions)0;
                else throw new LuaArgumentException("Invalid options string. Default is 'None'.", nameof(defButton));
            }
            return MessageBox.Show(Form.ActiveForm, text, title, (MessageBoxButtons)b, (MessageBoxIcon)i, (MessageBoxDefaultButton)db,
                (MessageBoxOptions)o).ToString();
        }
        public static void LuaSetColor(int color)
        {
            Editor.ColorPicker.Color = Color.FromArgb(color);
        }
        public static int LuaGetColorString(string color)
        {
            PropertyInfo p = typeof(Color).GetProperty(color, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
            return p is not null ? ((Color)p.GetValue(null)).ToArgb() : throw new LuaArgumentException("Invalid color name.", nameof(color));
        }
        public static int LuaGetColorRgb(int r, int g, int b)
        {
            if (r is >= 0 and <= 255 && g is >= 0 and <= 255 && b is >= 0 and <= 255) return Color.FromArgb(r, g, b).ToArgb();
            else throw new LuaArgumentException("r, g, and b must be numbers between 0 and 255 inclusive.");
        }
        public static int LuaGetColorArgb(int a, int r, int g, int b)
        {
            if (r is >= 0 and <= 255 && g is >= 0 and <= 255 && b is >= 0 and <= 255 && a is >= 0 and <= 255) return Color.FromArgb(a, r, g, b).ToArgb();
            else throw new LuaArgumentException("a, r, g, and b must be numbers between 0 and 255 inclusive.");
        }
        #endregion
    }

    [Serializable]
    public class LuaPluginException : Exception
    {
        public LuaPluginException() { }
        public LuaPluginException(string message) : base(message) { }
        public LuaPluginException(string message, Exception inner) : base(message, inner) { }
        protected LuaPluginException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class LuaArgumentException : LuaPluginException
    {
        private string parameter;
        public string Parameter { get; }
        public LuaArgumentException() { }
        public LuaArgumentException(string message) : base(message) { }
        public LuaArgumentException(string message, string parameter) : base($"{message}\nParameter name: {parameter}") { this.parameter = parameter; }
        public LuaArgumentException(string message, Exception inner) : base(message, inner) { }
        public LuaArgumentException(string message, string parameter, Exception inner) : base($"{message}\nParameter name: {parameter}", inner) {
            this.parameter = parameter; }
        protected LuaArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
