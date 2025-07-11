using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace TelegramVisualPart.UserControls.ChatsSearch
{
    /// <summary>
    /// Логика взаимодействия для SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        const string _showAll = "show all";
        const string _collapse = "collapse";
        private void ShowFreqChats_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowFreqChats.Text = ShowFreqChats.Text.Equals(_showAll) ?
                _collapse : _showAll;

            _isShowChats = !_isShowChats;
            SetControlSize();
        }

        private bool _isShowChats = false;
        public void SetControlSize()
        {
            if (!_isShowChats)
            {
                //ChatsPanel.Height = TestChat.Height;
                this.Height = TestChat.Height * 2;
                ChatsPanel.Width = ChatsPanel.Children.Count * TestChat.Width;

                ChatsPanel.Background = new SolidColorBrush(Colors.Transparent);
                return;
            }

            double someInRow = this.ActualWidth / TestChat.Width;

            //some shit with math round to check
            double amountOfRows = Math.Ceiling((double)ChatsPanel.Children.Count / someInRow);


            ChatsPanel.Height = (amountOfRows) * TestChat.Height;
            ChatsPanel.Width = someInRow * TestChat.Width;

            this.Height = (amountOfRows) * TestChat.Height + (TypesRows.Height.Value + FrqContactRow.Height.Value);

            //ChatsPanel.Background = new SolidColorBrush(Colors.Red);
        }

        public void UpdateColors()
        {
            ActiveRect.Fill =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            TextBlock block = TabsPanel.Children.OfType<TextBlock>().Where
                (x => !CompareColors(x)).FirstOrDefault();

            if (block is null) return;
            block.Foreground =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
        }

        private bool CompareColors(TextBlock block)
        {
            return  block.Foreground is SolidColorBrush brush && 
                brush.Color == Colors.Gray;
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //UpdateColors();

            if (sender is not TextBlock block) return;
            ClearForegroundForTabs();

            block.Foreground =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];


            Point targetPos = block.TranslatePoint(new Point(0, 0), RectGrid);
            double targetWidth = block.ActualWidth;

            var transform = ActiveRect.RenderTransform as TranslateTransform;
            double currentX = transform?.X ?? 0;
            double currentWidth = ActiveRect.ActualWidth;

            Duration animDuration = TimeSpan.FromMilliseconds(300);

            var moveAnim = new DoubleAnimation
            {
                From = currentX,
                To = targetPos.X - 10,
                Duration = animDuration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var widthAnim = new DoubleAnimation
            {
                From = currentWidth,
                To = targetWidth,
                Duration = animDuration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            transform?.BeginAnimation(TranslateTransform.XProperty, moveAnim);
            ActiveRect.BeginAnimation(FrameworkElement.WidthProperty, widthAnim);
        }

        public void ClearForegroundForTabs()
        {
            for (int i = 0; i < TabsPanel.Children.Count; i++)
            {
                if (TabsPanel.Children[i] is TextBlock textBlock)
                {
                    textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }
    }
}
