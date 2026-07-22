using System.CodeDom;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TelegramLib.Enums.Chat;

namespace TelegramVisualPart.UserControls.ContactsControls.AutoDeleteControls
{
    /// <summary>
    /// Логика взаимодействия для SpecialListBox.xaml
    /// </summary>
    public partial class SpecialListBox : UserControl
    {
        public event Action SelectedIndexUpdate;

        private const int _borderVal = 2;
        private readonly Dictionary<int, string> _voc = new Dictionary<int, string>()
        {
            { -1, " " },
            { 0, " " },
            { 1, "1 day" },
            { 2, "2 days" },
            { 3, "3 days" },
            { 4, "4 days" },
            { 5, "5 days" },
            { 6, "6 days" },
            { 7, "1 week" },
            { 14, "2 weeks" },
            { 21, "3 weeks" },
            { 30, "1 month" },
            { 60, "2 months" },
            { 90, "3 months" },
            { 120, "4 months" },
            { 150, "5 months" },
            { 180, "6 months" },
            { 365, "1 year" },
            { -100, "" },
            { -200, " " },
        };

        public SpecialListBox()
        {
            InitializeComponent();
        }

        private AutoDeleteDuration _duration;
        public void SetAutoDeletionValue(AutoDeleteDuration duration)
        {
            _duration = duration;

            //Test
            //_duration = new AutoDeleteDuration(AutoDeleteType.FiveMonths);
            if (_duration is null) return;

            //Set value on list
            for (int i = (int)AutoDeleteType.Nothing; i <= (int)AutoDeleteType.OneYear; i++)
            {
                if ((AutoDeleteType)i == _duration.Type)
                {
                    //set i value
                    ValueByIndex(i);
                    return;
                }
            }
        }

        public void ValueByIndex(int index)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _selectedIndex += index;
                var args = new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, 1)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent,
                    Source = ScrollView
                };
                ScrollViewer_PreviewMouseWheel(ScrollView, args);
            }), DispatcherPriority.Background);
        }

        private int _selectedIndex = 2;

        public int GetSelectedIndex()
        {
            return _selectedIndex;
        }

        public string GetValueBySelectedIndex()
        {
            if(CheckPanel.Children[_selectedIndex] is TextBlock block)
            {
                return block.Text;
            }
            throw new Exception("Impossible");
        }

        public void SetSelectedIndex(int index)
        {
            _selectedIndex = index;
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CheckPanel.Children.Count == 0)
                return;

            UpdateSelectedItem(e.Delta);
        }

        public void UpdateSelectedItem(double delta)
        {
            const int minIndex = 2;
            const int adder = 1;

            if (delta < 0)
                _selectedIndex = Math.Min(CheckPanel.Children.Count - adder - minIndex, _selectedIndex + adder);
            else if (delta > 0)
                _selectedIndex = Math.Max(minIndex, _selectedIndex - adder);

            var selectedElement = CheckPanel.Children[_selectedIndex] as UIElement;
            CenterElementInScrollViewer(selectedElement);

            SelectedIndexUpdate?.Invoke();
        }

        public AutoDeleteType GetChosenAutoDelItem()
        {
            return (AutoDeleteType)_selectedIndex - 1;
        }

        private void CenterElementInScrollViewer(UIElement element)
        {
            const int divider = 2;
            if (element == null) return;

            var transform = element.TransformToAncestor(ScrollView);
            Point position = transform.Transform(new Point(0, 0));

            double elementCenter = position.Y + ((FrameworkElement)element).ActualHeight / divider;
            double scrollViewerCenter = ScrollView.ViewportHeight / divider;

            double offset = ScrollView.VerticalOffset + (elementCenter - scrollViewerCenter);

            offset = Math.Max(0, Math.Min(offset, ScrollView.ScrollableHeight));

            SmoothScrollTo(offset);
        }

        private void SmoothScrollTo(double targetOffset)
        {
            const int duration = 75;
            double start = ScrollView.VerticalOffset;

            var animation = new DoubleAnimation
            {
                From = start,
                To = targetOffset,
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var clock = animation.CreateClock();

            clock.CurrentTimeInvalidated += (s, e) =>
            {
                if (clock.CurrentProgress.HasValue)
                {
                    double progress = clock.CurrentProgress.Value;
                    double current = start + (targetOffset - start) * progress;
                    ScrollView.ScrollToVerticalOffset(current);
                }
            };

            clock.Controller.Begin();
        }

        private void HighlightSelectedElement(int selectedIndex)
        {
            for (int i = 0; i < CheckPanel.Children.Count; i++)
            {
                if (CheckPanel.Children[i] is TextBlock tb)
                {
                    tb.Background = (i == selectedIndex) ? Brushes.LightGray : Brushes.Transparent;
                }
            }
        }
        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (CheckPanel.Children.Count != 0) return;

            foreach (var val in _voc)
            {
                TextBlock block = GetTextBlock(val.Value);
                CheckPanel.Children.Add(block);
            }
        }

        private TextBlock GetTextBlock(string text)
        {
            const int height = 40;
            const int fontSize = 16;
            const int padding = 10;

            TextBlock res = new TextBlock()
            {
                Text = text,
                Height = height,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = fontSize,
                Padding = new Thickness(padding),
                Foreground = new SolidColorBrush(Colors.Gray)
            };

            res.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
            res.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
            res.MouseMove += TextBlock_MouseMove;
         
            return res;
        }

        private bool _isDragging = false;
        private Point _startPoint;

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            const int adder = 1;
            if (sender is TextBlock tb)
            {
                int index = CheckPanel.Children.IndexOf(tb);
                _selectedIndex = index + adder;
                ValueByIndex(0);
            }

            _isDragging = true;
            _startPoint = e.GetPosition(ScrollView);
            (sender as TextBlock).CaptureMouse();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            (sender as TextBlock).ReleaseMouseCapture();

            ValueByIndex(1);
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPoint = e.GetPosition(ScrollView);
                double delta = currentPoint.Y - _startPoint.Y;

                ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - delta);

                UpdatePressMoveSelectedIndex(delta);
                _startPoint = currentPoint;
            }
        }

        private double _accumulatedDelta = 0;
        private const int _thresholdValue = 30;

        public void UpdatePressMoveSelectedIndex(double delta)
        {
            const int minIndex = 2;
            const int adder = 1;

            _accumulatedDelta += delta;

            while (_accumulatedDelta <= -_thresholdValue)
            {
                _selectedIndex = Math.Min(CheckPanel.Children.Count - adder - minIndex, _selectedIndex + adder);

                _accumulatedDelta += _thresholdValue;
                CenterElementInScrollViewer(CheckPanel.Children[_selectedIndex] as UIElement);

                SelectedIndexUpdate?.Invoke();
            }

            while (_accumulatedDelta >= _thresholdValue)
            {
                _selectedIndex = Math.Max(2, _selectedIndex - adder);

                _accumulatedDelta -= _thresholdValue;
                CenterElementInScrollViewer(CheckPanel.Children[_selectedIndex] as UIElement);

                SelectedIndexUpdate?.Invoke();
            }
        }

        public void SetListWithBlocks(List<string> toAdd)
        {
            CheckPanel.Children.Clear();

            for (int i = 0; i < toAdd.Count; i++)
            {
                TextBlock block = GetTextBlock(toAdd[i]);
                CheckPanel.Children.Add(block);
            }
        }

        public void ClearCheckPanel()
        {
            CheckPanel.Children.Clear();
        }
    }
}
