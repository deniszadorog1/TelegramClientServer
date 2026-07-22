using MaterialDesignThemes.Wpf;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.MainClasses.UserParams;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls.MediaActions;
using Image = System.Windows.Controls.Image;

namespace TelegramVisualPart.Pages.VisualPages
{
    /// <summary>
    /// Логика взаимодействия для VisualActionPage.xaml
    /// </summary>
    public partial class VisualActionPage : Page
    {
        public event EventHandler ToRemoveImage;

        private Image _img;
        private MediaElement _media;

        private TelSystem _system;
        private int _tempMediaIndex = -1;
        private List<MediaAction> _messages;

        private List<UserImage> _userImages;
        private string _userName;

        private List<Image> _imgs;

        private UserChat _chat;
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
        }

        public void SetUserImages(List<UserImage> images, TelSystem system,
            string userName, bool isLoggedUser, UserChat chat)
        {
            MediaMenu = null;
            _imgs = null;

            _system = system;
            _userImages = images;
            _userName = userName;
            _tempMediaIndex = 0;
            _chat = chat;

            SetUserImageParams();

            SetEventsForUsersImagesMenu();
        }

        public void SetEventsForUsersImagesMenu()
        {
            UsersImageMenu.SaveAs.PreviewMouseDown += SaveBut_PreviewMouseDown;
            UsersImageMenu.Copy.PreviewMouseDown += CopyUserImage_PreviewMouseDown;
            UsersImageMenu.Report.PreviewMouseDown += ReportUserImage_PreviewMouseDown;
        }

        private void ReportUserImage_PreviewMouseDown(object senner, MouseButtonEventArgs e)
        {
            MessageBox.Show("!!!You cant report here!!!");
        }

        private void CopyUserImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var bitmapSource = (BitmapSource)ImageToShow.Source;
            Clipboard.SetImage(bitmapSource);
        }

        private async void DeleteImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UserImage userImage = _userImages[_tempMediaIndex];

            _userImages.RemoveAt(_tempMediaIndex);

            _tempMediaIndex = _userImages.Count <= _tempMediaIndex ? --_tempMediaIndex : _tempMediaIndex;
            ToRemoveImage?.Invoke(this, EventArgs.Empty);

            if (_userImages.Count == 0)
            {
                ((MainWindow)Window.GetWindow(this)).ClearVisualActionPage();
                return;
            }

            ImageToShow.Source = new BitmapImage(new Uri(
                await FilesAction.GetUserImagePath(_userImages.First().Name), UriKind.Absolute));

            SetUserImageParams();
        }

        public int GetTempImageIndex()
        {
            return _tempMediaIndex;
        }

        private void SetUserImageParams()
        {
            if (_system is null || _userImages is null || _tempMediaIndex == -1 ||
                _userImages.Count == 0) return;

            UserImage userImage = _userImages[_tempMediaIndex];

            ElementName.Text = userImage.Name;
            PositionInFolder.Text = $"{_tempMediaIndex + 1} of {_userImages.Count}";
            SenderName.Text = _userName;
            SentDate.Text = $"{userImage.Date.Day}.{userImage.Date.Month}.{userImage.Date.Year}";
        }


        public void SetUserChat(TelSystem system, List<MediaAction> messages, int startElementIndex, UserChat chat)
        {
            _system = system;
            _messages = messages;
            _tempMediaIndex = startElementIndex;
            _chat = chat;
            //_tempMediaIndex = _tempMediaIndex;

            SetMediaParams();
        }

        public void SetMediaParams()
        {
            if (_system is null || _messages is null || _tempMediaIndex == -1 || _messages.Count <= _tempMediaIndex ||
                _messages.Count == 0 || _messages[_tempMediaIndex] is not MediaAction media) return;

            ElementName.Text = media.MediaName;
            PositionInFolder.Text = $"Photo {_messages.FindIndex(x => x.Id == media.Id) + 1} of {_messages.Count}";

            SentDate.Text = $"{media.GetSentDate().Value.Day} {media.GetSentDate().Value.Month} {media.GetSentDate().Value.Year}";


            //Set sender name
            SenderName.Text = _system.GetMessageSenderLoginByMessage(_messages[_tempMediaIndex]);
            return;
            //set text
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

            //Get temp message
            //go trough 
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
            if (_system is null) return;

            //Remove chosen element
            if (_tempMediaIndex == -1) return;
            RemoveFromChat(_tempMediaIndex);

            if (_chat is not null) ApiService.UpdateChat(_chat);
        }
        private void RemoveChosenImage()
        {
            _imgs.RemoveAt(_tempMediaIndex);
            _messages.RemoveAt(_tempMediaIndex);

            if (_imgs.Count == 0)
            {
                //Clear this window
                ((MainWindow)Window.GetWindow(this)).ClearVisualActionPage();
                return;
            }

            _tempMediaIndex = _imgs.Count <= _tempMediaIndex ? --_tempMediaIndex : _tempMediaIndex;

            ImageToShow.Source = _imgs[_tempMediaIndex].Source;
        }

        private void RemoveChosenVideo()
        {
            _mediaPaths.RemoveAt(_tempMediaIndex);
            _messages.RemoveAt(_tempMediaIndex);

            if (_mediaPaths.Count == 0)
            {
                ((MainWindow)Window.GetWindow(this)).ClearVisualActionPage();
                return;
            }
            _tempMediaIndex = _mediaPaths.Count <= _tempMediaIndex ? --_tempMediaIndex : _tempMediaIndex;

            SetMediaFile();
        }

        private void RemoveChosenGif()
        {
            _mediaPaths.RemoveAt(_tempMediaIndex);
            _messages.RemoveAt(_tempMediaIndex);

            if (_mediaPaths.Count == 0)
            {
                ((MainWindow)Window.GetWindow(this)).ClearVisualActionPage();
                return;
            }
            _tempMediaIndex = _mediaPaths.Count <= _tempMediaIndex ? --_tempMediaIndex : _tempMediaIndex;

            SetMediaFile();
        }

        /// <summary>
        /// Remove from view and logic
        /// </summary>
        /// <param name="mediaIndex"></param>
        public void RemoveFromChat(int mediaIndex)
        {
            if (_system.ChosenChatContact is null) return;
            //If this is img || video || gif        
            if (_gifPath is not null)//its gif
            {
                ((MainWindow)Window.GetWindow(this)).RemoveElementFromChat(mediaIndex, MediaType.Gif);
                _system.RemoveElemetFromChosenChat(mediaIndex, MediaType.Gif);
                RemoveChosenVideo();
            }
            else if (_img is not null)//its image 
            {
                //Remove from Visual part 
                ((MainWindow)Window.GetWindow(this)).RemoveElementFromChat(mediaIndex, MediaType.Image);

                //Remove from logic
                _system.RemoveElemetFromChosenChat(mediaIndex, MediaType.Image);

                //Go to other in here (forward, if not -> backwards) //Delete element
                RemoveChosenImage();
            }
            else if (_media is not null)//its video
            {
                ((MainWindow)Window.GetWindow(this)).RemoveElementFromChat(mediaIndex, MediaType.Video);
                _system.RemoveElemetFromChosenChat(mediaIndex, MediaType.Video);
                RemoveChosenVideo();
            }

            ClearRenderTransform();
            //_tempMediaIndex--;
            SetMediaParams();
        }

        private void SaveAs_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveElement();
        }

        private async void RightArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            const int adderIndex = 1;
            if (_userImages is not null &&
                (_tempMediaIndex + 1) < _userImages.Count)
            {
                _tempMediaIndex++;
                await SetUserImage();
            }
            else if (_img is not null &&
                _imgs is not null &&
                (_tempMediaIndex + adderIndex) < _imgs.Count)
            {
                _tempMediaIndex++;
                ImageToShow.Source = _imgs[_tempMediaIndex].Source;
                _img = _imgs[_tempMediaIndex];

                RightArrowActions();
            }
            else if (_media is not null &&
                (_tempMediaIndex + adderIndex) < _mediaPaths.Count)
            {
                _tempMediaIndex++;
                SetMediaFile();

                RightArrowActions();
            }
            //Set next visual element
        }

        public async Task SetUserImage()
        {
            _img = await FilesAction.GetUserImage(_userImages[_tempMediaIndex].Name);
            ImageToShow.Source = _img.Source;

            ClearRenderTransform();

            SetUserImageParams();
        }

        public void RightArrowActions()
        {
            ClearRenderTransform();

            // _tempMediaIndex++;
            SetMediaParams();
        }

        public void SetMediaFile()
        {
            //no readonly, no const ?
            Size size = new Size(300, 200);
            
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
                    Width = size.Width,
                    Height = size.Height,
                    LoadedBehavior = MediaState.Manual,
                    UnloadedBehavior = MediaState.Manual
                };
                media.Play();

                VideoToShow.Source = media.Source;

                VideoToShow.Visibility = Visibility.Visible;
            }
        }

        private async void LeftArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            const int lessIndex = 1;
            if (_userImages is not null &&
                (_tempMediaIndex - lessIndex) >= 0)
            {
                _tempMediaIndex--;
                await SetUserImage();
            }
            else if (_img is not null &&
                _imgs is not null &&
                (_tempMediaIndex - lessIndex) >= 0)
            {
                _tempMediaIndex--;
                ImageToShow.Source = _imgs[_tempMediaIndex].Source;
                _img = _imgs[_tempMediaIndex];

                LeftArrowAction();
            }
            else if (_media is not null &&
                (_tempMediaIndex - lessIndex) >= 0)
            {
                _tempMediaIndex--;
                SetMediaFile();
                LeftArrowAction();
            }
        }

        public void LeftArrowAction()
        {
            ClearRenderTransform();
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
            else if (ImageToShow is not null) SaveElements.SaveImageAs(ImageToShow);
            else if (VideoToShow is not null) SaveElements.SaveVideoAs(VideoToShow);
        }

        private int _rotation = 0;
        private const int _rotateAngle = 90;

        private void RotateBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            const int divider = 2;
            const int duration = 300;

            UIElement el = ImageToShow.Visibility == Visibility.Hidden ? VideoToShow : ImageToShow;

            double width = ImageToShow.Visibility == Visibility.Hidden ? VideoToShow.ActualWidth : ImageToShow.ActualWidth;
            double height = ImageToShow.Visibility == Visibility.Hidden ? VideoToShow.ActualHeight : ImageToShow.ActualHeight;

            if (!(el.RenderTransform is RotateTransform rotateTransform))
            {
                rotateTransform = new RotateTransform(_rotation, width / divider, height / divider);
                el.RenderTransform = rotateTransform;
            }
            _rotation += _rotateAngle;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = _rotation,
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        private void MenuBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private const int halfDivider = 2;
        private void ImageToShow_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (ImageToShow is null || _gifPath is not null) return;
            ImageToShow.RenderTransform = new RotateTransform(_rotation,
                ImageToShow.ActualWidth / halfDivider, ImageToShow.ActualHeight / halfDivider);
        }

        private void VideoToShow_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoToShow is null) return;
            var width = VideoToShow.NaturalVideoWidth;
            var height = VideoToShow.NaturalVideoHeight;

            if (width > 0 && height > 0)
            {
                VideoToShow.RenderTransform = new RotateTransform(_rotation, width / halfDivider, height / halfDivider);
            }
        }

        private void MenuBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MediaMenu is not null) MediaMenu.Visibility = Visibility.Visible;
            else UsersImageMenu.Visibility = Visibility.Visible;
        }

        private void MediaMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaMenu.Visibility = Visibility.Hidden;
        }

        private void UsersImageMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            UsersImageMenu.Visibility = Visibility.Hidden;
        }

    }
}
