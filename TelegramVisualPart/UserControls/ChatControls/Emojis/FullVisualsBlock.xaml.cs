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
using TelegramLib.MainClasses;
using WpfAnimatedGif;

namespace TelegramVisualPart.UserControls.ChatControls.Emojis
{
    /// <summary>
    /// Логика взаимодействия для FullVisualsBlock.xaml
    /// </summary>
    public partial class FullVisualsBlock : UserControl
    {
        private TelSystem _system;
        public FullVisualsBlock()
        {
            InitializeComponent();

            SetBasicParams();

            SetUserElements();

            SetGifs();
            SetImgsEvents(StickerPanel);
            SetImgsEvents(GIFsPanel);
        }

        public void SetSystem(TelSystem system)
        {
            _system = system;
        }

        public void SetUserElements()
        {

        }

        public void SetImgsEvents(WrapPanel panel)
        {
            for(int i = 0; i < panel.Children.Count; i++)
            {
                if (panel.Children[i] is Image img)
                {
                    img.MouseEnter += Image_MouseEnter;
                    img.MouseLeave += Image_MouseLeave;
                }
            }
        }

        private void Sticker_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img) return;
            ((MainWindow)Window.GetWindow(this)).SendStickerInChat(img, _system.LoggedUser.GetFirstImageName().Name);
        }

        private bool _isBlockMedias;
        public void SetIsBlockMedias(bool isBlock)
        {
            _isBlockMedias = isBlock;
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

            ((MainWindow)Window.GetWindow(this)).SendGif(gifPath, _system.LoggedUser.GetFirstImageName().Name);
        }

        public void SetBasicParams()
        {
            HideAllPanels();
            EmojisPanel.Visibility = Visibility.Visible;
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
            HideAllPanels();
            GifScroll.Visibility = Visibility.Visible;
        }

        private void StickersTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideAllPanels();
            StickerScroll.Visibility = Visibility.Visible;
        }

        private void EmojiTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideAllPanels();
            EmojisPanel.Visibility = Visibility.Visible;
        }

        private void HideAllPanels()
        {
            EmojisPanel.Visibility = Visibility.Hidden;
            GifScroll.Visibility = Visibility.Hidden;
            StickerScroll.Visibility = Visibility.Hidden;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void TextBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void TextBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
