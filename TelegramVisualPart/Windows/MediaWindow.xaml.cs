using MahApps.Metro.Behaviors;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
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
using System.Windows.Threading;
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

        private bool _isSchedule = false;

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

            godWindow.CloseAllMediaWindows();
            godWindow.AddMediaWindow(this);

            RemoveParamFromMenu();

            if (_type == MediaShowType.Videos) SetVideoTimers();
            else VideoPanel.Visibility = Visibility.Hidden;
        }

        private double _tickTime = 0.1;
        public void SetVideoTimers()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(_tickTime);
            timer.Tick += timer_Tick;

            _videoLimitTimer = new DispatcherTimer();
            _videoLimitTimer.Interval = TimeSpan.FromSeconds(_videoDurCheckTime);
            _videoLimitTimer.Tick += VideoLimitTimer_Tick;
        }

        private void VideoLimitTimer_Tick(object sender, EventArgs e)
        {
            if (VideoToShow.Position >= TimeSpan.FromSeconds(_maxVideoDuration))
            {
                VideoToShow.Position = TimeSpan.Zero;
                VideoToShow.Play();
            }
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
            List<MediaAction> videos, bool isShed = false)
        {
            _media = media;
            _mediaPaths = vidPaths;
            _mediaMessages = videos;
            _isSchedule = isShed;

            SetVidIndex();

            VideoToShow.Source = media.Source;


            //SetBasicParams();

            if (_type != MediaShowType.Videos) ImageToShow.Visibility = Visibility.Hidden;

            _allImagesInfo = null;
            SetMediaParams(_mediaMessages[_tempMediaIndex]);

            SetMenuWithSchedMessages();

            SetLoadBehavior();
            VideoToShow.Play();
        }

        public void SetGif(int startIndex, List<string> gifPaths,
            List<MediaAction> gifs, bool isSched)
        {
            _mediaPaths = gifPaths;
            _mediaMessages = gifs;
            _tempMediaIndex = startIndex;
            _isSchedule = isSched;

            //SetBasicParams();

            _allImagesInfo = null;
            UpdateVideoByTempIndex();
            SetMenuWithSchedMessages();

            MediaMenuEl.MenuPanel.Children.Remove(MediaMenuEl.SaveAs);
            UsersImageMenu.ChildrenPanel.Children.Remove(UsersImageMenu.SaveAs);

            SaveBut.Visibility = Visibility.Hidden;
        }

        public void UpdateVideoByTempIndex()
        {
            if (_tempMediaIndex == -1) return;

            SetMediaParams(_mediaMessages[_tempMediaIndex]);
            SetMediaFileByTempIndex();
        }

        public void SetMediaFileByTempIndex()
        {
            HideAllShows();
            if (_type == MediaShowType.Gif)
            {
                ImageToShow.Visibility = Visibility.Visible;
                SetGifParams(_mediaPaths[_tempMediaIndex]);
            }
            else if (_type == MediaShowType.Videos)
            {
                string fullVideoPath = FilesAction.GetFullVideoPath(System.IO.Path.GetFileName(_mediaPaths[_tempMediaIndex]));

                var media = new MediaElement
                {
                    Source = new Uri(fullVideoPath, UriKind.Absolute),
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
            ElementName.Text = System.IO.Path.GetFileName(media.MediaName);
            PositionInFolder.Text = $"{_mediaMessages.FindIndex(x => x.Id == media.Id) + 1} of {_mediaMessages.Count}";

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

            if (_chosenMedia is null) return;

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

            if (_imgInfo is null) return;
            SetChatImage(_imgInfo.Value.Img.Tag.ToString());
            SetStratImgIndex();

            SetImgMediaParam();
            SetMenuWithSchedMessages();
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

            MediaMenuEl = null;
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
            MediaMenuEl.GoToMessage.PreviewMouseDown += MoveToMessage_PreviewMouseDown;
            MediaMenuEl.ShowInFolder.PreviewMouseDown += ShowInFolder_PreviewMouseDown;
            MediaMenuEl.CopyFrame.PreviewMouseDown += CopyFrame_PreviewMouseDown;
            MediaMenuEl.Forward.PreviewMouseDown += Forward_PreviewMouseDown;
            MediaMenuEl.Delete.PreviewMouseDown += Delete_PreviewMouseDown;
            MediaMenuEl.SaveAs.PreviewMouseDown += SaveBut_PreviewMouseDown;

            UsersImageMenu.Copy.PreviewMouseDown += CopyFrame_PreviewMouseDown;
            UsersImageMenu.Delete.PreviewMouseDown += DeleteImage_PreviewMouseDown;
            UsersImageMenu.SaveAs.PreviewMouseDown += SaveBut_PreviewMouseDown;
            UsersImageMenu.WatchInFiles.PreviewMouseDown += ShowInFolder_PreviewMouseDown;
            UsersImageMenu.Report.PreviewMouseDown += Report_PreviewMouseDown;
        }

        private const int _thickMult = -30;
        public void SetMenuWithSchedMessages()
        {
            if (!_isSchedule) return;

            MediaMenuEl.MenuPanel.Children.Remove(MediaMenuEl.Forward);
            MediaMenuEl.MenuPanel.Children.Remove(MediaMenuEl.Delete);
            //MediaMenuEl.Margin = new Thickness(-200, MediaMenuEl.MenuPanel.Children.Count * _thickMult, 0, 0);

            UsersImageMenu.ChildrenPanel.Children.Remove(UsersImageMenu.Delete);
            UsersImageMenu.Margin = new Thickness(-200, UsersImageMenu.ChildrenPanel.Children.Count * _thickMult, 0, 0);
        }

        private void DeleteImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _godWindow.ClearThirdFrame();

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
            _godWindow.ClearThirdFrame();
            _godWindow.ClearSecFrame();


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
                _type == MediaShowType.ChatImages)
            {
                string mediaPath = FilesAction.GetFullChatImagePath(_allImagesInfo[_tempMediaIndex].Item1.Tag.ToString());

                DataObject data = new DataObject();
                data.SetData(DataFormats.FileDrop, new string[] { mediaPath });
                data.SetImage((BitmapSource)ImageToShow.Source);

                Clipboard.SetDataObject(data);
            }
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
            _godWindow.ClearThirdFrame();

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

        private readonly Size _littleWindowSize = new Size(1000, 550);

        private bool _isMax = false;

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {

            if (!_isMax)
            {
                this.Height = SystemParameters.WorkArea.Height;
                this.Width = SystemParameters.WorkArea.Width;

                this.Left = SystemParameters.WorkArea.Left;
                this.Top = SystemParameters.WorkArea.Top;

                _isMax = true;
            }
            else
            {
                this.Height = _littleWindowSize.Height;
                this.Width = _littleWindowSize.Width;

                this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2 + SystemParameters.WorkArea.Left;
                this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2 + SystemParameters.WorkArea.Top;

                this.WindowState = WindowState.Normal;
                _isMax = false;
            }

            /*            if (WindowState == WindowState.Maximized)
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
                        WindowSizerIcon.Kind = PackIconKind.WindowRestore;*/
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (VideoToShow is not null &&
                VideoToShow.LoadedBehavior == MediaState.Manual) VideoToShow.Stop();

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


        private void MediaMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaMenuEl.Visibility = Visibility.Hidden;
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

            VideoIsOpened();

            double maxDuration = VideoToShow.NaturalDuration.TimeSpan.TotalSeconds;
            TimelineSlider.Maximum = Math.Min(maxDuration, 20);

            _videoLimitTimer.Start();
        }

        private void SetImgMediaParam()
        {
            if (_imgInfo is null) return;

            ElementName.Text = System.IO.Path.GetFileName(_imgInfo.Value.Img.Tag.ToString());
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
            MoveMediaToLeft();
        }

        public void MoveMediaToLeft()
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

        public void MoveMediaToRight()
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                MoveMediaToLeft();
            }
            else if (e.Key == Key.Right)
            {
                MoveMediaToRight();
            }
        }


        private DispatcherTimer timer;
        private DispatcherTimer _videoLimitTimer;

        private const int _maxVideoDuration = 20;
        private const int _videoDurCheckTime = 500;

        private bool isDragging = false;
        private bool _isPlaying = false;

        private void VideoIsOpened()
        {
            if (VideoToShow.NaturalDuration.HasTimeSpan)
            {
                TimelineSlider.Maximum = VideoToShow.NaturalDuration.TimeSpan.TotalSeconds;
                TotalTimeText.Text = VideoToShow.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }

            timer.Start();
            //VideoToShow.Play();
            _isPlaying = true;

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging && VideoToShow.NaturalDuration.HasTimeSpan)
            {
                UpdateVideoDurationBlocks();
            }
        }

        public void UpdateVideoDurationBlocks()
        {
            TimelineSlider.Value = VideoToShow.Position.TotalSeconds;
            CurrentTimeText.Text = VideoToShow.Position.ToString(@"mm\:ss");
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                //SetLoadBehavior();
                VideoToShow.Pause();
                PlayPauseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            }
            else
            {
                VideoToShow.Play();
                PlayPauseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            _isPlaying = !_isPlaying;

            if (_videoLimitTimer.IsEnabled) _videoLimitTimer.Stop();
            else _videoLimitTimer.Start();
        }

        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDragging || Mouse.LeftButton == MouseButtonState.Pressed)
            {
                VideoToShow.Position = TimeSpan.FromSeconds(TimelineSlider.Value);
                UpdateVideoDurationBlocks();
            }
        }

        private void TimelineSlider_DragStarted(object sender, EventArgs e)
        {
            SetLoadBehavior();
            isDragging = true;
            VideoToShow.Stop();
        }

        public void SetLoadBehavior()
        {
            if (VideoToShow.LoadedBehavior != MediaState.Manual) VideoToShow.LoadedBehavior = MediaState.Manual;
            if (VideoToShow.UnloadedBehavior != MediaState.Manual) VideoToShow.UnloadedBehavior = MediaState.Manual;
        }

        private void TimelineSlider_DragCompleted(object sender, EventArgs e)
        {
            isDragging = false;
            VideoToShow.Position = TimeSpan.FromSeconds(TimelineSlider.Value);

            if (_isPlaying) VideoToShow.Play();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoToShow != null)
            {
                VideoToShow.Volume = e.NewValue;
            }
        }

        private void MenuBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MediaMenuEl is not null)
            {
                MediaMenuEl.Visibility = Visibility.Visible;

                MediaMenuEl.VerticalAlignment = VerticalAlignment.Bottom;
                MediaMenuEl.HorizontalAlignment = HorizontalAlignment.Right;
                MediaMenuEl.Margin = new Thickness(0);

                return;
            }
            UsersImageMenu.Visibility = Visibility.Visible;

            UsersImageMenu.VerticalAlignment = VerticalAlignment.Bottom;
            UsersImageMenu.HorizontalAlignment = HorizontalAlignment.Right;
            UsersImageMenu.Margin = new Thickness(0);
        }

        private void VideoToShow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetMediaMenuPosition(e);
        }

        public void SetMediaMenuPosition(MouseButtonEventArgs e)
        {
            MediaMenuEl.VerticalAlignment = VerticalAlignment.Top;
            MediaMenuEl.HorizontalAlignment = HorizontalAlignment.Left;

            MediaMenuEl.Visibility = Visibility.Visible;
            MediaMenuEl.UpdateLayout();

            (double x, double y) cord = GetMenuMargin
                (e, MediaMenuEl.ActualWidth, MediaMenuEl.ActualHeight);

            MediaMenuEl.Margin = new Thickness(cord.x, cord.y, 0, 0);

            e.Handled = true;
        }

        private void ImageToShow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MediaMenuEl is not null)
            {
                SetMediaMenuPosition(e);
                return;
            }

            UsersImageMenu.VerticalAlignment = VerticalAlignment.Top;
            UsersImageMenu.HorizontalAlignment = HorizontalAlignment.Left;

            UsersImageMenu.Visibility = Visibility.Visible;
            UsersImageMenu.UpdateLayout();

            (double x, double y) cord = GetMenuMargin
                (e, UsersImageMenu.ActualWidth, UsersImageMenu.ActualHeight);

            UsersImageMenu.Margin = new Thickness(cord.x, cord.y, 0, 0);

            e.Handled = true;
        }

        private (double, double) GetMenuMargin(MouseButtonEventArgs e, double menuW, double menuH)
        {
            Point clickPoint = e.GetPosition(this);


            double winW = this.ActualWidth;
            double winH = this.ActualHeight;

            double x = clickPoint.X;
            double y = clickPoint.Y;

            if (x + menuW > winW)
                x = winW - menuW;

            if (y + menuH > winH)
                y = winH - menuH;

            x = Math.Max(0, x);
            y = Math.Max(0, y);

            return (x, y);
        }
    }
}
