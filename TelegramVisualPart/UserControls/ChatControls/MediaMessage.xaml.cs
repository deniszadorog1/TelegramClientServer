using Microsoft.AspNetCore.SignalR.Protocol;
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

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ImageMessage.xaml
    /// </summary>
    public partial class MediaMessage : UserControl
    {
        public Image _img;
        public MediaElement _media;
        public string _gifPath;

        public MediaMessage(Image img)
        {
            _img = img;
            InitializeComponent();
            ImgMessage.ImageSource = _img.Source;

            HideAllBorders();
            ImageBorder.Visibility = Visibility.Visible;
        }

        public MediaMessage(string gifPath)
        {
            _gifPath = gifPath;
            InitializeComponent();

            HideAllBorders();
            GifBorder.Visibility = Visibility.Visible;

            SetGif(gifPath);
        }


        public void SetGif(string gifPath)
        {
            ImgMessage = null;
            VideoBorder.Visibility = Visibility.Hidden;
            ImageBorder.Visibility = Visibility.Hidden;

            var uri = new Uri(gifPath, UriKind.RelativeOrAbsolute);
            var source = new BitmapImage(uri);
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(GifImage, source);
            WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(GifImage, RepeatBehavior.Forever);
        }

        public MediaMessage(MediaElement media)
        {
            _media = media;
            InitializeComponent();

            HideAllBorders();
            VideoBorder.Visibility = Visibility.Visible;
        }

        public void HideAllBorders()
        {
            VideoBorder.Visibility = Visibility.Hidden;
            ImageBorder.Visibility = Visibility.Hidden;
            GifBorder.Visibility = Visibility.Hidden;
        }

        public MediaElement GetVideo()
        {
            return _media;
        }

        public Image GetImage()
        {
            return _img;
        }

        public string GetGifPath()
        {
            return _gifPath;
        }
    }
}
