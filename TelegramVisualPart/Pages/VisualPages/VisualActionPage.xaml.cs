using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
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
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Helper;
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

        private TelSystem _system;
        private int _tempMediaIndex = -1;
        private List<MediaAction> _messages;

        private List<Image> _imgs;
        //private int _tempMediaIndex;

        public VisualActionPage(Image img, List<Image> chatImgs)
        {
            _img = img;
            _imgs = chatImgs;
            SetImgIndex();

            InitializeComponent();

            SetBasicParams();
            ImageToShow.Source = _img.Source;

            VideoToShow.Visibility = Visibility.Hidden;
            //VideoToShow = null;
        }

        public void SetUserChat(TelSystem system, List<MediaAction> messages, int startElementIndex)
        {
            _system = system;
            _messages = messages;
            _tempMediaIndex = startElementIndex;

            //_tempMediaIndex = _tempMediaIndex;

            SetMediaParams();
        }

        public void SetMediaParams()
        {
            if (_system is null || _messages is null || _tempMediaIndex == -1) return;

            if (_tempMediaIndex == -1 || _messages[_tempMediaIndex] is not MediaAction media) return;

            ElementName.Text = media.MediaName;
            PositionInFolder.Text = $"Photo {_messages.FindIndex(x => x.Id == media.Id) + 1} of {_messages.Count}";

            SentDate.Text = $"{media.GetSentDate().Value.Day} {media.GetSentDate().Value.Month} {media.GetSentDate().Value.Year}";

            SenderName.Text = media.SenderId == -1 ? _system.LoggedUser.Name :
                _system.Contacts[_system.Contacts.FindIndex(x => x.Id == media.SenderId)].Name;
        }

        

        public void SetImgIndex()
        {
            string imgFileName = System.IO.Path.GetFileName(_img.Source.ToString());
            for (int i = 0; i < _imgs.Count; i++)
            {
                string tempImgFileName = System.IO.Path.GetFileName(_imgs[i].Source.ToString());
                if (tempImgFileName == imgFileName)
                {
                    _tempMediaIndex = i;
                    return;
                }
            }
        }

        private List<string> _mediaPaths;
        public VisualActionPage(MediaElement media, List<string> mediasPaths)
        {
            _media = media;
            _mediaPaths = mediasPaths;

            SetMediaIndex();

            InitializeComponent();

            VideoToShow.Source = media.Source;

            SetBasicParams();
            ImageToShow.Visibility = Visibility.Hidden;
            //ImageToShow = null;
        }

        public void HideAllShows()
        {
            ImageToShow.Visibility = Visibility.Hidden;
            VideoToShow.Visibility = Visibility.Hidden;
        }
        public void SetVideo(string videoPath)
        {
            var media = new MediaElement
            {
                Source = new Uri(videoPath, UriKind.Absolute),
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            media.Play();
        }

        public void SetMediaIndex()
        {
            string mediaFileName = System.IO.Path.GetFileName(_media.Source.ToString());
            for (int i = 0; i < _mediaPaths.Count; i++)
            {
                string tempMediaFileName = System.IO.Path.GetFileName(_mediaPaths[i]);
                if (tempMediaFileName == mediaFileName)//  ArePathsEqual(_mediaPaths[i], _media.Source))
                {
                    _tempMediaIndex = i;
                    return;
                }
            }
        }

        public static bool ArePathsEqual(string filePath, Uri fileUri)
        {
            return string.Equals(System.IO.Path.GetFullPath(filePath), fileUri.LocalPath, StringComparison.OrdinalIgnoreCase);
        }

        public string? _gifPath = null;
        public VisualActionPage(string gifPath, List<string> mediasPaths)
        {
            _media = new MediaElement();

            _gifPath = gifPath;
            _mediaPaths = mediasPaths;
            SetGifIndex();

            InitializeComponent();

            SetGifParams();
            SetBasicParams();

            VideoToShow.Visibility = Visibility.Hidden;
            //VideoToShow = null;
        }

        public void SetGifIndex()
        {
            for (int i = 0; i < _mediaPaths.Count; i++)
            {
                if (_mediaPaths[i].ToString() == _gifPath.ToString())
                {
                    _tempMediaIndex = i;
                    return;
                }
            }
        }

        public void SetGifParams()
        {
            var uri = new Uri(_gifPath, UriKind.RelativeOrAbsolute);
            var source = new BitmapImage(uri);
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ImageToShow, source);
            WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(ImageToShow, RepeatBehavior.Forever);

            ImageToShow.RenderTransform = new RotateTransform(_rotation,
                    source.Width / 2, source.Height / 2);
        }

        public void SetBasicParams()
        {
            LeftArrowEl.TestIcon.Kind = PackIconKind.KeyboardArrowLeft;
            RightArrowEl.TestIcon.Kind = PackIconKind.KeyboardArrowRight;

            SaveBut.TestIcon.Kind = PackIconKind.ContentSaveOutline;
            RotateBut.TestIcon.Kind = PackIconKind.RotateLeft;
            MenuBut.TestIcon.Kind = PackIconKind.DotsVertical;

            SetEventsForMenu();
        }

        public void SetEventsForMenu()
        {
            MediaMenu.GoToMessage.PreviewMouseDown += MoveToMessage_PreviewMouseDown;
            MediaMenu.ShowInFolder.PreviewMouseDown += ShowInFolder_PreviewMouseDown;
            MediaMenu.CopyFrame.PreviewMouseDown += CopyFrame_PreviewMouseDown;
            MediaMenu.Forward.PreviewMouseDown += Forward_PreviewMouseDown;
            MediaMenu.Delete.PreviewMouseDown += Delete_PreviewMouseDown;
            MediaMenu.SaveAs.PreviewMouseDown += SaveAs_PreviewMouseDown;
        }

        private void MoveToMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set move to message on chat
        }

        private void ShowInFolder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Show folder
        }

        private void CopyFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Copy this into buffer
        }

        private void Forward_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Resent element to another user
        }

        private void Delete_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Delete element
        }

        private void SaveAs_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveElement();
        }

        private void RightArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_img is not null &&
                (_tempMediaIndex + 1) < _imgs.Count)
            {
                _tempMediaIndex++;
                ImageToShow.Source = _imgs[_tempMediaIndex].Source;
                _img = _imgs[_tempMediaIndex];

                RightArrowActions();
            }
            else if (_media is not null &&
                (_tempMediaIndex + 1) < _mediaPaths.Count)
            {
                _tempMediaIndex++;
                SetMediaFile();

                RightArrowActions();
            }

            //Set next visual element
        }

        public void RightArrowActions()
        {
            ClearRenderTransform();

           // _tempMediaIndex++;
            SetMediaParams();
        }

        public void SetMediaFile()
        {
            HideAllShows();

            MediaType type = FilesAction.GetMediaTypeFromFilename(_mediaPaths[_tempMediaIndex]);

            if (type is MediaType.Gif)
            {
                ImageToShow.Visibility = Visibility.Visible;
                _gifPath = _mediaPaths[_tempMediaIndex];
                SetGifParams();
            }
            else if (type is MediaType.Video)
            {
                var media = new MediaElement
                {
                    Source = new Uri(_mediaPaths[_tempMediaIndex], UriKind.Absolute),
                    Width = 300,
                    Height = 200,
                    LoadedBehavior = MediaState.Manual,
                    UnloadedBehavior = MediaState.Manual
                };
                media.Play();

                VideoToShow.Source = media.Source;

                VideoToShow.Visibility = Visibility.Visible;
            }
        }

        private void LeftArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_img is not null &&
                (_tempMediaIndex - 1) >= 0)
            {
                _tempMediaIndex--;
                ImageToShow.Source = _imgs[_tempMediaIndex].Source;
                _img = _imgs[_tempMediaIndex];

                LeftArrowAction();
            }
            else if (_media is not null &&
                (_tempMediaIndex - 1) >= 0)
            {
                _tempMediaIndex--;
                SetMediaFile();
                LeftArrowAction();
            }
        }

        public void LeftArrowAction()
        {
            ClearRenderTransform();
            //_tempMediaIndex--;
            SetMediaParams();
        }



        public void ClearRenderTransform()
        {
            ImageToShow.RenderTransform = null;
            _rotation = 0;
        }

        private void SaveBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveElement();
        }

        public void SaveElement()
        {
            if (ImageToShow is not null && _gifPath is not null) SaveElements.SaveGifAs(_gifPath);
            else if (ImageToShow is not null) SaveElements.SaveImageAs(_img);
            else if (VideoToShow is not null) SaveElements.SaveVideoAs(VideoToShow);
        }

        private int _rotation = 0;
        private const int _rotateAngle = 90;

        private void RotateBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement el = ImageToShow.Visibility == Visibility.Hidden ? VideoToShow : ImageToShow;

            double width = ImageToShow.Visibility == Visibility.Hidden ? VideoToShow.ActualWidth : ImageToShow.ActualWidth;
            double height = ImageToShow.Visibility == Visibility.Hidden ? VideoToShow.ActualHeight : ImageToShow.ActualHeight;

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
            if (ImageToShow is null || _gifPath is not null) return;
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
