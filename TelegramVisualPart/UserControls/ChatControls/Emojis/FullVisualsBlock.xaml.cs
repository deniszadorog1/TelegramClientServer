using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace TelegramVisualPart.UserControls.ChatControls.Emojis
{
    /// <summary>
    /// Логика взаимодействия для FullVisualsBlock.xaml
    /// </summary>
    public partial class FullVisualsBlock : UserControl
    {
        public FullVisualsBlock()
        {
            InitializeComponent();

            SetBasicParams();

            SetGifs();
        }

        public void SetGifs()
        {
            //Set gifs here
            
            Image img = new Image()
            {
                Width = 100,
                Height = 100
            };

            var gifUri = new Uri("pack://siteoforigin:,,,/Visuals/Gifs/TestGif.gif", UriKind.Absolute);
            var image = new BitmapImage(gifUri);
            ImageBehavior.SetAnimatedSource(img, image);

            GIFsPanel.Children.Add(img);

            img.PreviewMouseDown += Gif_PreviewMouseDown;
        }

        private void Gif_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image) return;

            Image img = sender as Image;
            string gifPath = ((BitmapImage)ImageBehavior.GetAnimatedSource(img)).UriSource.ToString();

            ((MainWindow)Window.GetWindow(this)).SendGif(gifPath);
        }

        public void SetBasicParams()
        {
            EmojisPanel.Visibility = Visibility.Visible;
            GIFsPanel.Visibility = Visibility.Hidden;
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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
                To = targetPos.X - 40,
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

        private void GIFsTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            EmojisPanel.Visibility = Visibility.Hidden;
            GIFsPanel.Visibility = Visibility.Visible;
        }

        private void StickersTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void EmojiTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            EmojisPanel.Visibility = Visibility.Visible;
            GIFsPanel.Visibility = Visibility.Hidden;
        }
    }
}
