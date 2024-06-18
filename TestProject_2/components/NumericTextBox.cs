using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace TestProject_2.components
{
    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            this.PreviewTextInput += OnPreviewTextInput;
            this.PreviewKeyDown += OnPreviewKeyDown;
            DataObject.AddPastingHandler(this, OnPaste);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab ||
                e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Enter)
            {
                return;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                if (e.Key == Key.C || e.Key == Key.X || e.Key == Key.V)
                {
                    return;
                }
            }

            e.Handled = !IsKeyAllowed(e.Key);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsKeyAllowed(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9 || key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                return true;
            }
            return false;
        }
    }
}
