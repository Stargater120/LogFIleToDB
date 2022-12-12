using Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogFileToDB
{
    /// <summary>
    /// Interaktionslogik für IPInput.xaml
    /// </summary>
    public partial class IPInput : UserControl
    {
        private static readonly List<Key> DigitKeys = new List<Key>
        {
            Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0, Key.NumPad1,
            Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9
        };

        private static readonly List<Key> MoveForwardKeys = new List<Key> { Key.Right };
        private static readonly List<Key> MoveBackwardKeys = new List<Key> { Key.Left };
        private static readonly List<Key> OtherAllowedKeys = new List<Key> { Key.Tab, Key.Delete };
        public event EventHandler<EmitEvent> EmitIP;

        private List<TextBox> _segments = new List<TextBox>();

        private bool _suppressAddressUpdate = false;


        public IPInput()
        {
            InitializeComponent();
            _segments.Add(FirstSegment);
            _segments.Add(SecondSegment);
            _segments.Add(ThirdSegment);
            _segments.Add(LastSegment);
        }

        public void Clear()
        {
            foreach (TextBox segment in _segments)
            {
                segment.Clear();
            }
        }

        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
            "Address", typeof(string), typeof(IPInput), new FrameworkPropertyMetadata(default(string), AddressChanged)
            {
                BindsTwoWayByDefault = true
            });

        private static void AddressChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var ipTextBox = dependencyObject as IPInput;
            var text = e.NewValue as string;

            if (text != null && ipTextBox != null)
            {
                ipTextBox._suppressAddressUpdate = true;
                var i = 0;
                foreach (var segment in text.Split('.'))
                {
                    ipTextBox._segments[i].Text = segment;
                    i++;
                }

                ipTextBox._suppressAddressUpdate = false;
            }

            if (ipTextBox.CheckSegments())
            {
                ipTextBox.SetIPAddress(ipTextBox);
            }
        }

        private bool CheckSegments()
        {
            bool areAllFilled = false;
            foreach (var segment in _segments)
            {
                areAllFilled = segment.Text.Length > 0;
            }

            return areAllFilled;
        }

        private void SetIPAddress(IPInput ipTextBox)
        {
            string ipAddress = "";
            int i = 0;
            foreach (var segment in ipTextBox._segments)
            {
                ipAddress += segment.Text;
                if (i != 3)
                {
                    ipAddress += ".";
                }

                i++;
            }

            var emit = new EmitEvent();
            emit.IPAddress = ipAddress;
            EmitIP(this, emit);
        }

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DigitKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelDigitKeyPress(sender as TextBox);
                HandleDigitPress(sender as TextBox);
            }
            else if (MoveBackwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelBackwardKeyPress(sender as TextBox);
                HandleBackwardKeyPress(sender as TextBox);
            }
            else if (MoveForwardKeys.Contains(e.Key))
            {
                e.Handled = ShouldCancelForwardKeyPress(sender as TextBox);
                HandleForwardKeyPress(sender as TextBox);
            }
            else if (e.Key == Key.Back)
            {
                HandleBackspaceKeyPress(sender as TextBox);
            }
            else if (e.Key == Key.OemPeriod)
            {
                e.Handled = true;
                HandlePeriodKeyPress(sender as TextBox);
            }
            else
            {
                e.Handled = !AreOtherAllowedKeysPressed(e);
            }
        }

        private bool AreOtherAllowedKeysPressed(KeyEventArgs e)
        {
            return e.Key == Key.C && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.V && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.A && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   e.Key == Key.X && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0) ||
                   OtherAllowedKeys.Contains(e.Key);
        }

        private void HandleDigitPress(TextBox sender)
        {
            var currentTextBox = sender;

            if (currentTextBox != null && currentTextBox.Text.Length == 3 &&
                currentTextBox.CaretIndex == 3 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private bool ShouldCancelDigitKeyPress(TextBox sender)
        {
            var currentTextBox = sender;
            return currentTextBox != null &&
                   currentTextBox.Text.Length == 3 &&
                   currentTextBox.CaretIndex == 3 &&
                   currentTextBox.SelectedText.Length == 0;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_suppressAddressUpdate)
            {
                Address = string.Format("{0}.{1}.{2}.{3}", FirstSegment.Text, SecondSegment.Text, ThirdSegment.Text,
                    LastSegment.Text);
            }

            var currentTextBox = FocusManager.GetFocusedElement(this) as TextBox;

            if (currentTextBox != null && currentTextBox.Text.Length == 3 && currentTextBox.CaretIndex == 3)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private bool ShouldCancelBackwardKeyPress(TextBox sender)
        {
            var currentTextBox = sender;
            return currentTextBox != null && currentTextBox.CaretIndex == 0;
        }

        private void HandleBackspaceKeyPress(TextBox sender)
        {
            var currentTextBox = sender;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToPreviousSegment(currentTextBox);
            }
        }

        private void HandleBackwardKeyPress(TextBox sender)
        {
            var currentTextBox = sender;

            if (currentTextBox != null && currentTextBox.CaretIndex == 0)
            {
                MoveFocusToPreviousSegment(currentTextBox);
            }
        }

        private bool ShouldCancelForwardKeyPress(TextBox sender)
        {
            var currentTextBox = sender;
            return currentTextBox != null && currentTextBox.CaretIndex == 3;
        }

        private void HandleForwardKeyPress(TextBox sender)
        {
            var currentTextBox = sender;

            if (currentTextBox != null && currentTextBox.CaretIndex == currentTextBox.Text.Length)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private void HandlePeriodKeyPress(TextBox sender)
        {
            var currentTextBox = sender;

            if (currentTextBox != null && currentTextBox.Text.Length > 0 &&
                currentTextBox.CaretIndex == currentTextBox.Text.Length)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private void MoveFocusToPreviousSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, FirstSegment))
            {
                var previousSegmentIndex = _segments.FindIndex(box => ReferenceEquals(box, currentTextBox)) - 1;
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                _segments[previousSegmentIndex].CaretIndex = _segments[previousSegmentIndex].Text.Length;
            }
        }

        private void MoveFocusToNextSegment(TextBox currentTextBox)
        {
            if (!ReferenceEquals(currentTextBox, LastSegment))
            {
                currentTextBox.SelectionLength = 0;
                currentTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void DataObject_OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
            {
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            int num;

            if (!int.TryParse(text, out num))
            {
                e.CancelCommand();
            }
        }
    }
}