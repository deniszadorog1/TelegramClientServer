using MaterialDesignThemes.Wpf;
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
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ImageMessage.xaml
    /// </summary>
    public partial class MediaMessage : UserControl
    {
        public bool IsSticker { get; }

        public Image _img;
        public MediaElement _media;
        public string _gifPath;
        public string _stickerPath;

        public string _senderImgName;

        public MediaMessage(Image img, bool isSticker, string senderImgName)
        {
            _img = img;
            IsSticker = isSticker;
            _senderImgName = senderImgName;

            InitializeComponent();
            ImgMessage.ImageSource = _img.Source;

            HideAllBorders();
            ImageBorder.Visibility = Visibility.Visible;
            SetSenderImage();
        }

        public MediaMessage(string gifPath, string senderImgName)
        {
            _gifPath = gifPath;
            _senderImgName = senderImgName;

            InitializeComponent();

            HideAllBorders();
            GifBorder.Visibility = Visibility.Visible;

            SetGif(gifPath);
            SetSenderImage();
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

        public MediaMessage(MediaElement media, string senderImgName)
        {
            _media = media;
            _senderImgName = senderImgName;

            InitializeComponent();

            HideAllBorders();
            //VideoBorder.Visibility = Visibility.Visible;
            ImageBorder.Visibility = Visibility.Visible;
            SetVideoPreview();
            SetSenderImage();
        }

        public void SetSenderImage()
        {
            if (_senderImgName is null)
            {
                BgBrush.ImageSource = BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetSystemImagePath("StopSign.png"), UriKind.Absolute));
                return;
            }
            BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_senderImgName), UriKind.Absolute));
        }

        private void SetVideoPreview()
        {
            string fileName = System.IO.Path.GetFileName(_media.Source.LocalPath);

            Image img = FilesAction.GetImagePreviewForVideo(fileName);

            ImgMessage.ImageSource = img.Source;
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

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            SendInfoGrid.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            SendInfoGrid.Visibility = Visibility.Hidden;
        }

        public void SetInfoParams(string iconName, DateTime time)
        {
            TickIcon.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
            Time.Text = $"{time.Hour}:{time.Minute}"; 
        }
    }
}
