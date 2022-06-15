using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paintr
{
    public class ThemedTextBox : TextBox
    {
        public ThemedTextBox() : base()
        {
            BorderStyle = BorderStyle.FixedSingle;
            if (!DesignMode)
            {
                BackColor = Styling.ButtonBackgroundColor;
                ForeColor = Styling.TextColor;
            }
            CausesValidation = false;
            KeyDown += ThemedTextBox_KeyDown;
        }
        private void ThemedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData is Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = suppressLostFocus = true;
                OnInputDone(new EventArgs());
                suppressLostFocus = false;
            }
        }

        public ThemedTextBox(IEnumerable<char> charWhitelist) : this()
        {
            CharWhitelist = new(charWhitelist);
        }
        public List<char> CharWhitelist { get; set; }
        protected override void OnTextChanged(EventArgs e)
        {
            if(CharWhitelist is not null)
                for(int i = 0; i < Text.Length; i++)
                {
                    if(!CharWhitelist.Contains(Text[i]))
                    {
                        int j = i;
                        int s = SelectionStart;
                        int l = SelectionLength;
                        Text = i + 1 == Text.Length ? $"{Text[..i]}" : $"{Text[..i]}{Text[(--i + 2)..]}";
                        if(s > j)
                        {
                            s--;
                        }
                        else if(s + l >= j)
                        {
                            l--;
                        }
                        Select(s, l);
                    }
                }
            if (savedText != Text) ForeColor = Styling.TextUnsavedColor;
            else ForeColor = Styling.TextColor;
            base.OnTextChanged(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Disabled) return;
            base.OnKeyDown(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (Disabled || suppressLostFocus) return;
            OnInputDone(new EventArgs());
        }
        protected string savedText = "";
        public string SavedText { get => savedText; }
        public Predicate<string> ValidateInput { get; set; }
        private bool suppressLostFocus;
        protected virtual void OnInputDone(EventArgs e)
        {
            if(ValidateInput is null || ValidateInput(Text))
            {
                savedText = Text;
                ForeColor = Styling.TextColor;
                if (InputDone is not null) InputDone(this, e);
            }
            else
            {
                Text = savedText;
                Select(Text.Length, 0);
                ForeColor = Styling.TextColor;
            }
        }
        public event EventHandler InputDone;
        public bool Disabled
        {
            get => ReadOnly;
            set
            {
                ReadOnly = value;
                if (Disabled) ForeColor = Styling.TextDisabledColor;
                else ForeColor = Text == savedText ? Styling.TextColor : Styling.TextUnsavedColor;
            }
        }
        public void SetText(string text, bool moveSelectionToEnd = true)
        {
            Text = text;
            savedText = text;
            if(!Disabled) ForeColor = Text == savedText ? Styling.TextColor : Styling.TextUnsavedColor;
            if (moveSelectionToEnd && Focused) Select(Text.Length, 0);
        }
        public void SetTextIfNeeded(string text, bool moveSelectionToEnd = true)
        {
            if (SavedText != text) SetText(text, moveSelectionToEnd);
        }
        public static char[] DecimalFilter
        {
            get => new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', Styling.DecimalSeparator, '-' };
        }
        public static char[] DecimalPercentFilter
        {
            get => new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', Styling.DecimalSeparator, '-', '%' };
        }
        public static char[] DecimalListFilter
        {
            get => new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', Styling.DecimalSeparator, '-', Styling.NumberComma, ' ' };
        }
        public static char[] IntegerListFilter
        {
            get => new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', Styling.NumberComma, ' ' };
        }
        public static char[] HexFilter
        {
            get => new char[] { 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        }
        public static char[] IntegerFilter { get => new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-' }; }
        public static char[] PositiveIntegerFilter { get => new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }; }
        public static ThemedTextBox DoubleBox()
        {
            return new(DecimalFilter) { ValidateInput = (string txt) => double.TryParse(txt, out _) };
        }
        public static ThemedTextBox FloatBox()
        {
            return new(DecimalFilter) { ValidateInput = (string txt) => float.TryParse(txt, out _) };
        }
        public static ThemedTextBox NullDoubleBox()
        {
            return new(DecimalFilter) { ValidateInput = (string txt) => string.IsNullOrWhiteSpace(txt) || double.TryParse(txt, out _) };
        }
        public static ThemedTextBox IntBox()
        {
            return new(IntegerFilter) { ValidateInput = (string txt) => int.TryParse(txt, out _) };
        }
        public static ThemedTextBox UIntBox()
        {
            return new(PositiveIntegerFilter) { ValidateInput = (string txt) => uint.TryParse(txt, out _) };
        }
        public static ThemedTextBox FloatPairBox()
        {
            return new(DecimalListFilter) { ValidateInput = (string txt) => Extensions.TryStringToFloatArray(txt, out float[] arr) && arr.Length == 2 };
        }
        public static ThemedTextBox IntPairBox()
        {
            return new(IntegerListFilter) { ValidateInput = (string txt) => Extensions.TryStringToIntArray(txt, out int[] arr) && arr.Length == 2 };
        }
        public static ThemedTextBox IntTripletBox()
        {
            return new(IntegerListFilter) { ValidateInput = (string txt) => Extensions.TryStringToIntArray(txt, out int[] arr) && arr.Length == 3 };
        }
        public static ThemedTextBox DoubleTripletBox()
        {
            return new(DecimalListFilter) { ValidateInput = (string txt) => Extensions.TryStringToDoubleArray(txt, out double[] arr) && arr.Length == 3 };
        }
        public static ThemedTextBox ByteBox()
        {
            return new(IntegerFilter) { ValidateInput = (string txt) => byte.TryParse(txt, out _) };
        }
        public static ThemedTextBox PercentDoubleBox()
        {
            return new(DecimalPercentFilter) { ValidateInput = (string txt) => double.TryParse(txt.Replace("%", ""), out _) };
        }
    }
}
