using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses.Messages;
using TelegramLib.MainClasses.UserParams;
using TelegramLib.Models;
using TelegramVisualPart.Enums.MediaShow;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls.MediaActions;
using UserImage = TelegramLib.MainClasses.UserParams.UserImage;

namespace TelegramVisualPart.Windows
{
    /// <summary>
    /// Логика взаимодействия для MediaWindow.xaml
    /// </summary>
    public partial class MediaWindow : Window
    {
        private TelegramLib.MainClasses.User _user;
        private MainWindow _godWindow;
        private MediaShowType _type;
        private TelegramLib.MainClasses.TelSystem _system;

        public event EventHandler ToRemoveImage;

        //Base
        public MediaWindow(TelegramLib.MainClasses.User user,
            MainWindow godWindow,
            MediaShowType type,
            TelegramLib.MainClasses.TelSystem system)
        {
            _user = user;
            _godWindow = godWindow;
            _type = type;
            _system = system;

            InitializeComponent();

            SetBasicParams();

            if (_type == MediaShowType.UserImages ||
                _type == MediaShowType.OtherUserImages) SetUserImages();

            godWindow.AddMediaWindow(this);

            RemoveParamFromMenu();
        }

        public void RemoveParamFromMenu()
        {
            if (_type != MediaShowType.UserImages)
            {
                UsersImageMenu.ChildrenPanel.Children.Remove(UsersImageMenu.Delete);
            }
        }

        private List<string> _mediaPaths;
        private MediaElement _media;

        public void SetVideos(MediaElement media, List<string> vidPaths,
            List<MediaAction> videos)
        {
            _media = media;
            _mediaPaths = vidPaths;
            _mediaMessages = videos;

            SetVidIndex();

            VideoToShow.Source = media.Source;

            //SetBasicParams();

            if (_type != MediaShowType.Videos) ImageToShow.Visibility = Visibility.Hidden;

            _allImagesInfo = null;
            SetMediaParams(_mediaMessages[_tempMediaIndex]);
        }

        public void SetGif(int startIndex, List<string> gifPaths,
            List<MediaAction> gifs)
        {
            _mediaPaths = gifPaths;
            _mediaMessages = gifs;
            _tempMediaIndex = startIndex;

            //SetBasicParams();

            _allImagesInfo = null;
            UpdateVideoByTempIndex();
        }

        public void UpdateVideoByTempIndex()
        {
            SetMediaParams(_mediaMessages[_tempMediaIndex]);
            SetMediaFileByTempIndex();
        }

        public void SetMediaFileByTempIndex()
        {
            HideAllShows();
            //MediaType type = FilesAction.GetMediaTypeFromFilename(_mediaPaths[_tempMediaIndex]);

            if (_type == MediaShowType.Gif)
            {
                ImageToShow.Visibility = Visibility.Visible;
                SetGifParams(_mediaPaths[_tempMediaIndex]);
            }
            else if (_type == MediaShowType.Videos)
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

        public void SetGifParams(string path)
        {
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            var source = new BitmapImage(uri);
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ImageToShow, source);
            WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(ImageToShow, RepeatBehavior.Forever);

            ImageToShow.RenderTransform = new RotateTransform(_rotation,
                    source.Width / 2, source.Height / 2);
        }

        public void HideAllShows()
        {
            ImageToShow.Visibility = Visibility.Hidden;
            VideoToShow.Visibility = Visibility.Hidden;
        }

        public void SetMediaParams(MediaAction media)
        {
            ElementName.Text = media.MediaName;
            PositionInFolder.Text = $"Photo {_mediaMessages.FindIndex(x => x.Id == media.Id) + 1} of {_mediaMessages.Count}";

            SentDate.Text = $"{media.GetSentDate().Value.Day} {media.GetSentDate().Value.Month} {media.GetSentDate().Value.Year}";

            SenderName.Text = _system.GetSenderUserById(_mediaMessages[_tempMediaIndex].SenderUserId).Login;
        }

        public void SetVidIndex()
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

        private TelegramLib.MainClasses.Messages.Message _chosenMedia;
        private List<TelegramLib.MainClasses.Messages.MediaAction> _mediaMessages;
        //Chat messages
        public void SetChatImageMessages(TelegramLib.MainClasses.Messages.Message chosen,
            List<TelegramLib.MainClasses.Messages.MediaAction> messages)
        {
            _chosenMedia = chosen;
            _mediaMessages = messages;

            _allImagesInfo.Clear();
            for (int i = 0; i < _mediaMessages.Count; i++)
            {
                string filePath = FilesAction.GetFullChatImagePath(_mediaMessages[i].MediaName);

                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(filePath, UriKind.Absolute)),
                    Tag = _mediaMessages[i].MediaName
                };

                _allImagesInfo.Add((img, _mediaMessages[i].SentTime, _system.GetTrueUserById(_mediaMessages[i].SenderUserId).Login));
                if (_mediaMessages[i].Id == _chosenMedia.Id) _imgInfo = (img, _mediaMessages[i].SentTime, _system.GetTrueUserById(_mediaMessages[i].SenderUserId).Login);
            }

            
            SetChatImage(_imgInfo.Value.Img.Tag.ToString());
            SetStratImgIndex();

            SetImgMediaParam();
        }

        public void SetStratImgIndex()
        {
            //Get Img

            for (int i = 0; i < _allImagesInfo.Count; i++)
            {
                if (_allImagesInfo[i].Item1 == _imgInfo.Value.Img)
                {
                    _tempMediaIndex = i;

                }
            }
        }

        public void SetChatImage(string imgName)
        {
            //we have a name of image
            //Get path to it
            string filePath = FilesAction.GetFullChatImagePath(imgName);
            SetImageToShow(filePath);
        }

        //User Images(Profile) action 
        public (Image Img, DateTime sentTime, string Login)? _imgInfo;
        public List<(Image, DateTime, string)> _allImagesInfo = new List<(Image, DateTime, string)>();

        public void SetUserImages()
        {
            //_user - Chatter for who to set images
            //Set first one
            SetAllUserImages(
                _user.UserImages.Select(x => x.Name).ToList(),
                _user.UserImages.Select(x => x.Date).ToList(),
                GetSendersForUserImages(_user.UserImages.Count));

            _imgInfo = _allImagesInfo.FirstOrDefault();

            SetUserImage(_user.UserImages.First().Name);
            SetImgMediaParam();

            MediaMenu = null;
        }

        public List<string> GetSendersForUserImages(int amount)
        {
            List<string> res = new List<string>();

            for (int i = 0; i < amount; i++)
            {
                res.Add(_user.Login);
            }
            return res;
        }

        public void SetAllUserImages(
            List<string> names,
            List<DateTime> sentTime,
            List<string> senderLogin)
        {
            _allImagesInfo.Clear();
            for (int i = 0; i < names.Count; i++)
            {
                string filePath = FilesAction.GetUserImagePath(names[i]);

                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(filePath, UriKind.Absolute)),
                    Tag = names[i]
                };

                _allImagesInfo.Add((img, sentTime[i], senderLogin[i]));
            }
        }

        public void SetUserImage(string imgName)
        {
            //we have a name of image
            //Get path to it
            string filePath = FilesAction.GetUserImagePath(imgName);
            SetImageToShow(filePath);
        }

        public void SetImageToShow(string allPath)
        {
            Image img = new Image()
            {
                Source = new BitmapImage(new Uri(allPath, UriKind.Absolute))
            };
            ImageToShow.Source = img.Source;

            ClearRotationValues();
        }

        public void ClearRotationValues()
        {
            _rotation = 0;
            ImageToShow.RenderTransform = null;
        }

        //Chat image 
        //chat Videos
        public bool IsUsersIdsAreEqual(int compareUserId)
        {
            return _user.IsIdsAreEqual(compareUserId);
        }

        public void SetBasicParams()
        {
            LeftArrowEl.TestIcon.Kind = PackIconKind.KeyboardArrowLeft;
            RightArrowEl.TestIcon.Kind = PackIconKind.KeyboardArrowRight;

            SaveBut.TestIcon.Kind = PackIconKind.ContentSaveOutline;
            RotateBut.TestIcon.Kind = PackIconKind.RotateLeft;
            MenuBut.TestIcon.Kind = PackIconKind.DotsVertical;

            //if(_type == MediaShowType.OtherUserImages) MenuBut.R

            SetEventsForMenu();
        }

        public void SetEventsForMenu()
        {
            MediaMenu.GoToMessage.PreviewMouseDown += MoveToMessage_PreviewMouseDown;
            MediaMenu.ShowInFolder.PreviewMouseDown += ShowInFolder_PreviewMouseDown;
            MediaMenu.CopyFrame.PreviewMouseDown += CopyFrame_PreviewMouseDown;
            MediaMenu.Forward.PreviewMouseDown += Forward_PreviewMouseDown;
            MediaMenu.Delete.PreviewMouseDown += Delete_PreviewMouseDown;
            MediaMenu.SaveAs.PreviewMouseDown += SaveBut_PreviewMouseDown;

            UsersImageMenu.Copy.PreviewMouseDown += CopyFrame_PreviewMouseDown;
            UsersImageMenu.Delete.PreviewMouseDown += DeleteImage_PreviewMouseDown;
            UsersImageMenu.SaveAs.PreviewMouseDown += SaveBut_PreviewMouseDown;
            UsersImageMenu.WatchInFiles.PreviewMouseDown += ShowInFolder_PreviewMouseDown;
            UsersImageMenu.Report.PreviewMouseDown += Report_PreviewMouseDown;
        }

        private void DeleteImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_system.LoggedUser.UserImages.Count <= 1) return;

            //TO MAKE REMOVE USER IMAGE FROM DB
            //ApiService.AddUserImage(_system.LoggedUser, System.IO.Path.GetFileName(filePath));
            TelegramLib.MainClasses.UserParams.UserImage img =
                _system.LoggedUser.GetUserImageById(_tempMediaIndex);
            ApiService.DeleteUserImage(img, _system.LoggedUser.Id);

            //Remove in system 
            _system.LoggedUser.RemoveImageByIndex(_tempMediaIndex);

            SetNewTempIndexAfterDeletion();

            //Update in system
            if (_godWindow is not null &&
                _godWindow is MainWindow mainWindow)
            {
                //Update in temp page
                SetUserImages();

                //Update Chat(if visible)
                mainWindow.UpdateChat();

                //Update my profile
                mainWindow.UpdateMyProfilePage();

                //Update in SignalR (message, userTalkMessage)
                SignalRService.UpdateUserImages(_system.LoggedUser);
            }
        }

        private void SetNewTempIndexAfterDeletion()
        {
            if (_tempMediaIndex == 0) return;
            _tempMediaIndex--;
        }

        private void Report_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            MessageBox.Show("You cant report here!!");
        }

        private void MoveToMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set move to message on chat

            //Get message
            MediaAction mediaAction = GetChosenMedia();
            if (mediaAction is null) return;

            //Go through main window to scroll to chosen message

            if (_godWindow is null || mediaAction is null) return;

            _godWindow.ShowChosenMessageByMessageId(mediaAction.Id);
            _godWindow.DeleteMediaWindow(this);
        }

        private void ShowInFolder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string? mediaName = string.Empty;
            //Show folder
            if ((_type == MediaShowType.UserImages ||
                _type == MediaShowType.OtherUserImages) &&
                _imgInfo is not null)
            {
                mediaName = _imgInfo.Value.Img.Tag.ToString();
            }
            else mediaName = _mediaMessages[_tempMediaIndex].MediaName;

            //Get full filePath
            string? fullPath = FilesAction.GetFullPath(mediaName, _type); //GetFullPath(mediaName);

            if (fullPath is null || !File.Exists(fullPath))
            {
                MessageBox.Show("Not exist man!");
                return;
            }
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
            _godWindow.DeleteMediaWindow(this);
        }

        private string? GetFullPath(string fileName)
        {
            if (fileName is null) return string.Empty;
            switch (_type)
            {
                case MediaShowType.ChatImages:
                    {
                        return FilesAction.GetFullChatImagePath(fileName);
                    }
                case MediaShowType.UserImages:
                    {
                        return FilesAction.GetFullUserImagePath(fileName);
                    }
                case MediaShowType.Videos:
                    {
                        return FilesAction.GetFullVideoPath(fileName);
                    }
                case MediaShowType.OtherUserImages:
                    {
                        return FilesAction.GetFullChatImagePath(fileName);
                    }
                case MediaShowType.Gif:
                    {
                        return FilesAction.GetFullGifPath(fileName);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        private void CopyFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Copy this into buffer

            //Image or video

            if (_type == MediaShowType.UserImages ||
                _type == MediaShowType.OtherUserImages ||
                _type == MediaShowType.ChatImages) Clipboard.SetImage((BitmapSource)ImageToShow.Source);
            else if (_type == MediaShowType.Videos)
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)VideoToShow.ActualWidth,
                    (int)VideoToShow.ActualHeight,
                    96, 96, PixelFormats.Pbgra32);

                rtb.Render(VideoToShow);
                Clipboard.SetImage(rtb);
                //SaveElements.SaveVideoAs(VideoToShow);
            }
            _godWindow.DeleteMediaWindow(this);
        }

        private void Forward_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mediaMessages is null || _mediaMessages.Count <= _tempMediaIndex) return;

            //Resent element to another user
            TelegramLib.MainClasses.Messages.Message mes = _mediaMessages[_tempMediaIndex];

            _godWindow.SendOneForwardMessage(mes);

            _godWindow.DeleteMediaWindow(this);
        }

        private void Delete_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Get message
            MediaAction mediaAction = GetChosenMedia();
            if (mediaAction is null) return;

            _godWindow.DeleteMessage(mediaAction);

            _godWindow.DeleteMediaWindow(this);

        }

        public MediaAction GetChosenMedia()
        {
            MediaAction messageToShow = null;
            //if media
            if (_type == MediaShowType.Videos ||
                _type == MediaShowType.Gif ||
                _type == MediaShowType.ChatImages)
            {
                messageToShow = _mediaMessages[_tempMediaIndex];
            }

            return messageToShow;
        }

        private void UpperBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            //Check pro
            if (sender is Button button)
            {
                button.Background =
                    (SolidColorBrush)System.Windows.Application.Current.Resources["OtherUpperButColor"];
            }
            else if (sender is Grid grid)
            {
                grid.Background =
                    (SolidColorBrush)System.Windows.Application.Current.Resources["OtherUpperButColor"];
            }
        }

        private void UpperBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;

            if (sender is Button button) button.Background = Brushes.Transparent;
            else if (sender is Grid grid)
            {
                grid.Background = Brushes.Transparent;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private readonly Size _littleWindowSize = new Size(800, 600);
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;

                var screenWidth = SystemParameters.WorkArea.Width;
                var screenHeight = SystemParameters.WorkArea.Height;

                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;

                Width = _littleWindowSize.Width;
                Height = _littleWindowSize.Height;

                WindowSizerIcon.Kind = PackIconKind.CropSquare;
                return;
            }
            this.WindowState = WindowState.Maximized;
            WindowSizerIcon.Kind = PackIconKind.WindowRestore;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //Remove from MainWindow
            _godWindow.RemoveFromGodWindow(this);
        }

        private void SaveBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveElement();
        }

        public void SaveElement()
        {
            if (_type == MediaShowType.UserImages ||
                _type == MediaShowType.OtherUserImages ||
                _type == MediaShowType.ChatImages) SaveElements.SaveImageAs(ImageToShow);
            else if (_type == MediaShowType.Videos) SaveElements.SaveVideoAs(VideoToShow);
        }

        private int _rotation = 0;
        private const int _rotateAngle = 90;
        private void RotateBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement el = _type == MediaShowType.Videos ? VideoToShow : ImageToShow;

            double width = _type == MediaShowType.Videos ? VideoToShow.ActualWidth : ImageToShow.ActualWidth;
            double height = _type == MediaShowType.Videos ? VideoToShow.ActualHeight : ImageToShow.ActualHeight;

            RotateTransform rotateTransform = new RotateTransform(_rotation, width / 2, height / 2);
            el.RenderTransform = rotateTransform;

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

        private System.Windows.Point _mouseDownPosition;
        private bool _isMouseDown = false;
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && this.WindowState == WindowState.Maximized && e.LeftButton == MouseButtonState.Pressed)
            {
                const int _windWidth = 1000;
                _isMouseDown = false;

                var mousePosition = e.GetPosition(this);
                double percentHorizontal = mousePosition.X / this.ActualWidth;
                double targetWidth = _windWidth;

                this.WindowState = WindowState.Normal;

                var screenPoint = PointToScreen(mousePosition);
                this.Left = screenPoint.X - targetWidth * percentHorizontal;
                this.Top = 0;
                this.Width = targetWidth;

                WindowSizerIcon.Kind = PackIconKind.CropSquare;

                this.DragMove();
            }
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            //SetMainPageOnWindowSizeChange();
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _mouseDownPosition = e.GetPosition(this);
                _isMouseDown = true;

                if (this.WindowState != WindowState.Maximized)
                {
                    this.DragMove();
                }
            }
        }

        private void CloseWindowBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                    (SolidColorBrush)System.Windows.Application.Current.Resources["CloseWindowColor"];
        }

        private void ImageToShow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ImageToShow is null /*|| _gifPath is not null*/) return;
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

        private void SetImgMediaParam()
        {
            if (_imgInfo is null) return;

            ElementName.Text = _imgInfo.Value.Img.Tag.ToString();
            PositionInFolder.Text = $"{_tempMediaIndex + 1} of {_allImagesInfo.Count}";
            SenderName.Text = _imgInfo.Value.Login;
            SentDate.Text = $"{_imgInfo.Value.sentTime.Date.Day}.{_imgInfo.Value.sentTime.Date.Month}.{_imgInfo.Value.sentTime.Date.Year}";
        }

        private int _tempMediaIndex = 0;

        public void SetImageByIndex()
        {
            _imgInfo = _allImagesInfo[_tempMediaIndex];

            SetImgMediaParam();

            switch (_type)
            {
                case MediaShowType.ChatImages:
                    {
                        SetChatImage(_imgInfo.Value.Img.Tag.ToString());
                        break;
                    }
                case MediaShowType.UserImages:
                    {
                        SetUserImage(_imgInfo.Value.Img.Tag.ToString());
                        break;
                    }
                case MediaShowType.Videos:
                    break;
                case MediaShowType.OtherUserImages:
                    {
                        SetUserImage(_imgInfo.Value.Img.Tag.ToString());
                        break;
                    }
            }
        }

        private void LeftArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((_tempMediaIndex - 1) >= 0)
            {
                _tempMediaIndex--;
                if (_type == MediaShowType.Videos ||
                    _type == MediaShowType.Gif) UpdateVideoByTempIndex();
                else SetImageByIndex();
            }
        }

        private void RightArrowEl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int maxVal = (_type == MediaShowType.Videos ||
                _type == MediaShowType.Gif) ? _mediaMessages.Count : _allImagesInfo.Count;

            if ((_tempMediaIndex + 1) < maxVal)
            {
                _tempMediaIndex++;
                if (_type == MediaShowType.Videos ||
                    _type == MediaShowType.Gif) UpdateVideoByTempIndex();
                else SetImageByIndex();
            }
        }
    }
}
