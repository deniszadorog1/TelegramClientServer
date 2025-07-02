using MaterialDesignThemes.Wpf;
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
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.VisualPages
{
    /// <summary>
    /// Логика взаимодействия для VisualActionPage.xaml
    /// </summary>
    public partial class VisualActionPage : Page
    {
        private Image _img;
        private MediaElement _media;

        public VisualActionPage(Image img)
        {
            _img = img;
            InitializeComponent();

            SetBasicParams();
            ImageToShow.Source = _img.Source;

            VideoToShow.Visibility = Visibility.Hidden;
            VideoToShow = null;
        }

        public VisualActionPage(MediaElement media)
        {
            _media = media;
            InitializeComponent();

            VideoToShow.Source = media.Source;

            SetBasicParams();
            ImageToShow.Visibility = Visibility.Hidden;
            ImageToShow = null;
        }

        public void SetBasicParams()
        {
            LeftArrowEl.TestIcon.Kind = PackIconKind.KeyboardArrowLeft;
            RightArrowEl.TestIcon.Kind = PackIconKind.KeyboardArrowRight;

            SaveBut.TestIcon.Kind = PackIconKind.ContentSaveOutline;
            RotateBut.TestIcon.Kind = PackIconKind.RotateLeft;
            MenuBut.TestIcon.Kind = PackIconKind.DotsVertical;
        }

        private void RightArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set next visual element
        }

        private void LeftArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set previous visual element
        }

        private void SaveBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(ImageToShow is not null) SaveElements.SaveImageAs(_img);
        }

        private int _rotation = 0;
        private const int _rotateAngle = 90;

        private void RotateBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement el = ImageToShow is null ? VideoToShow : ImageToShow;

            double width = ImageToShow is null ? VideoToShow.ActualWidth : ImageToShow.ActualWidth;
            double height = ImageToShow is null ? VideoToShow.ActualHeight : ImageToShow.ActualHeight;
            
            if (!(el.RenderTransform is RotateTransform rotateTransform))
            {
                rotateTransform = new RotateTransform(_rotation, width / 2, height / 2);
                el.RenderTransform = rotateTransform;
            }
            _rotation += _rotateAngle;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = _rotation,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        private void MenuBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImageToShow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ImageToShow is null) return;
            ImageToShow.RenderTransform = new RotateTransform(_rotation, 
                ImageToShow.ActualWidth / 2, ImageToShow.ActualHeight / 2);
        }

        private void VideoToShow_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoToShow is null) return;
            var width = VideoToShow.NaturalVideoWidth;
            var height = VideoToShow.NaturalVideoHeight;

            if (width > 0 && height > 0)
            {
                VideoToShow.RenderTransform = new RotateTransform(_rotation, width / 2, height / 2);
            }
        }

        private void MenuBut_MouseEnter(object sender, MouseEventArgs e)
        {
            MediaMenu.Visibility = Visibility.Visible;
        }

        private void MediaMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaMenu.Visibility = Visibility.Hidden;
        }
    }
}
