using Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogFileToDB
{
    /// <summary>
    /// Interaktionslogik für DateTimeInput.xaml
    /// </summary>
    public partial class DateTimeInput : UserControl
    {
        private static readonly List<Key> DigitKeys = new List<Key>
        {
            Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.NumPad0, Key.NumPad1,
            Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9
        };

        private static readonly List<Key> MoveForwardKeys = new List<Key> { Key.Right };
        private static readonly List<Key> MoveBackwardKeys = new List<Key> { Key.Left };
        private static readonly List<Key> OtherAllowedKeys = new List<Key> { Key.Tab, Key.Delete };
        public event EventHandler<EmitDateTime> EmitDateTime;
        private readonly List<TextBox> _segments = new List<TextBox>();

        private bool _suppressDateTimeUpdate = false;

        public DateTimeInput()
        {
            InitializeComponent();
            _segments.Add(FirstSegment);
            _segments.Add(SecondSegment);
            _segments.Add(YearSegment);
            _segments.Add(HoursSegment);
            _segments.Add(MinutesSegment);
            _segments.Add(SecondsSegment);
        }

        public void Clear()
        {
            foreach (TextBox segment in _segments)
            {
                segment.Clear();
            }
        }

        public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(
            "Address", typeof(string), typeof(DateTimeInput),
            new FrameworkPropertyMetadata(default(string), AddressChanged)
            {
                BindsTwoWayByDefault = true
            });

        private static void AddressChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var dtTextBox = dependencyObject as DateTimeInput;
            var text = e.NewValue as string;
            char[] chars = { '.', ':', ',' };

            if (text != null && dtTextBox != null)
            {
                dtTextBox._suppressDateTimeUpdate = true;
                var i = 0;
                foreach (var segment in text.Split(chars))
                {
                    dtTextBox._segments[i].Text = segment;
                    i++;
                }

                dtTextBox._suppressDateTimeUpdate = false;
            }

            if (dtTextBox.CheckSegments())
            {
                dtTextBox.EmitEvent(dtTextBox);
            }
        }

        private bool CheckSegments()
        {
            if (_segments == null) return false;
            foreach (var segment in _segments)
            {
                if (segment.Text.Length < 1)
                {
                    return false;
                }
            }

            return true;
        }

        private void EmitEvent(DateTimeInput dtTextBox)
        {
            string dateTimeString = "";
            foreach (var segment in _segments)
            {
                dateTimeString += segment.Text as String;
            }

            if (DateTime.TryParse(dtTextBox.DateTimeString, out DateTime dateTime))
            {
                var dateTimeEvent = new EmitDateTime() { selectedTime = dateTime };
                dateTimeEvent.invalidDate = false;
                EmitDateTime(this, dateTimeEvent);
            }
            else
            {
                var dtEvent = new EmitDateTime() { selectedTime = DateTime.Now };
                dtEvent.invalidDate = true;
                EmitDateTime(this, dtEvent);
            }
        }

        public string DateTimeString
        {
            get { return (string)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
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
            bool isYear = false;

            if (currentTextBox != null)
            {
                isYear = currentTextBox.Name == YearSegment.Name;
            }

            if (currentTextBox != null && !isYear && currentTextBox.Text.Length == 2 &&
                currentTextBox.CaretIndex == 2 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToNextSegment(currentTextBox);
            }

            if (currentTextBox != null && isYear && currentTextBox.Text.Length == 4 &&
                currentTextBox.CaretIndex == 4 && currentTextBox.SelectedText.Length == 0)
            {
                MoveFocusToNextSegment(currentTextBox);
            }
        }

        private bool ShouldCancelDigitKeyPress(TextBox sender)
        {
            var currentTextBox = sender;
            bool isYear = false;

            if (currentTextBox != null)
            {
                isYear = currentTextBox.Name == YearSegment.Name;
            }

            if (!isYear)
            {
                return currentTextBox != null &&
                       currentTextBox.Text.Length == 2 &&
                       currentTextBox.CaretIndex == 2 &&
                       currentTextBox.SelectedText.Length == 0;
            }

            return currentTextBox != null &&
                   currentTextBox.Text.Length == 4 &&
                   currentTextBox.CaretIndex == 4 &&
                   currentTextBox.SelectedText.Length == 0;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_suppressDateTimeUpdate)
            {
                DateTimeString = string.Format("{0}.{1}.{2},{3}:{4}:{5}", FirstSegment.Text, SecondSegment.Text,
                    YearSegment.Text, HoursSegment.Text, MinutesSegment.Text, SecondsSegment.Text);
            }

            var currentTextBox = sender as TextBox;
            bool isYear = false;

            if (currentTextBox != null)
            {
                isYear = currentTextBox.Name == YearSegment.Name;
            }

            if (currentTextBox != null && !isYear && currentTextBox.Text.Length == 2 && currentTextBox.CaretIndex == 2)
            {
                MoveFocusToNextSegment(currentTextBox);
            }

            if (currentTextBox != null && isYear && currentTextBox.Text.Length == 4 && currentTextBox.CaretIndex == 4)
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
            bool isYear = false;

            if (currentTextBox != null)
            {
                isYear = currentTextBox.Name == YearSegment.Name;
            }

            if (!isYear)
            {
                return currentTextBox != null && currentTextBox.CaretIndex == 4;
            }

            return currentTextBox != null && currentTextBox.CaretIndex == 2;
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
            if (!ReferenceEquals(currentTextBox, SecondsSegment))
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