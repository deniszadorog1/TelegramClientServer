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
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;

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
        private int? _forwardedFrom = null;

        private TelSystem _system;

        public MediaMessage(TelSystem system, Image img, bool isSticker,
            string senderImgName, DateTime sendTime, int? forwardedFromId = null)
        {
            _img = img;
            IsSticker = isSticker;
            _senderImgName = senderImgName;
            _forwardedFrom = forwardedFromId;
            _system = system;

            InitializeComponent();
            ImgMessage.ImageSource = _img.Source;

            SetImgMessageSize(_img, ImageBorder);

            HideAllBorders();
            ImageBorder.Visibility = Visibility.Visible;
            SetSenderImage();

            SetTime(sendTime);

            SetTickEvent();
            SetForwardedFromRow();
        }


        private const int _minMediaSize = 125;
        private const int _maxMediaSize = 225;

        public void SetImgMessageSize(Image img, Border border)
        {
            if (img.Source is not BitmapImage bitmap) return;

            border.Width = bitmap.PixelWidth;
            border.Height = bitmap.PixelHeight;

            if (border.Width < _minMediaSize) border.Width = _minMediaSize;
            if (border.Width > _maxMediaSize) border.Width = _maxMediaSize;

            if (border.Height < _minMediaSize) border.Height = _minMediaSize;
            if (border.Height > _maxMediaSize) border.Height = _maxMediaSize;
        }

        public MediaMessage(string gifPath, string senderImgName, DateTime sentTime,
            int? forwardedFromId)
        {
            _gifPath = gifPath;
            _senderImgName = senderImgName;
            _forwardedFrom = forwardedFromId;

            InitializeComponent();

            HideAllBorders();
            GifBorder.Visibility = Visibility.Visible;

            SetGif(gifPath);
            SetSenderImage();

            SetTime(sentTime);

            SetTickEvent();
        }

        private void SetTime(DateTime time)
        {
            TimeBlock.Text = $"{VisHelper.GetCorrectTimeParamVis(time.Hour.ToString())}:" +
                $"{VisHelper.GetCorrectTimeParamVis(time.Minute.ToString())}";
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

            SetGifSize(GifImage, GifBorder, source);
        }

        public void SetGifSize(Image img, Border border, BitmapImage bitmap)
        {
            border.Width = bitmap.PixelWidth;
            border.Height = bitmap.PixelHeight;

            if (border.Width < _minMediaSize) border.Width = _minMediaSize;
            if (border.Width > _maxMediaSize) border.Width = _maxMediaSize;

            if (border.Height < _minMediaSize) border.Height = _minMediaSize;
            if (border.Height > _maxMediaSize) border.Height = _maxMediaSize;
        }

        public MediaMessage(MediaElement media, string senderImgName,
            MediaAction mediaLogicEl, int? forwardedFromId)
        {
            _media = media;
            _senderImgName = senderImgName;
            _forwardedFrom = forwardedFromId;

            InitializeComponent();

            HideAllBorders();
            //VideoBorder.Visibility = Visibility.Visible;
            ImageBorder.Visibility = Visibility.Visible;
            SetVideoPreview();
            SetSenderImage();

            SetTickEvent();

            SetTime(mediaLogicEl.SentTime);
        }

        public void SetTickEvent()
        {
            SelectionTickObj.StatusChanged += () =>
            {
                //Pressed on tick
                //Update counter on user chat
                ((MainWindow)Window.GetWindow(this)).UpdateUserChatSelectedAmount();
            };
        }

        private async Task SetForwardedFromRow()
        {
            if (_forwardedFrom is null) return;

            TelegramLib.MainClasses.User from =
                await ApiService.GetUserById((int)_forwardedFrom);
            if (from is null) return;


            //Set forwarded from user id as tag
            LoginForwarded.Tag = from.Id;

            ForwardedRow.Height = new GridLength(20);
            LoginForwarded.Text = from.Login;
        }

        public bool IsMessageIdTicked()
        {
            return SelectionTickObj.GetChosenStatus();
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

            SetImgMessageSize(img, ImageBorder);
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

        private const int _tickColWidth = 20;
        public void SetTickVis(string iconName, bool isCanBeVis)
        {
            TickColumn.Width = new GridLength(_tickColWidth);
            SetVisibility(iconName);
            if (isCanBeVis) TickIcon.Visibility = Visibility.Visible;
        }

        private const int _selectTickColWidth = 30;
        public void SetTickVisibility(bool isVis)
        {
            if (isVis)
            {
                this.Width += _selectTickColWidth;
                TickColumnDef.Width = new GridLength(_selectTickColWidth);
            }
            else
            {
                this.Width -= _selectTickColWidth;
                TickColumnDef.Width = new GridLength(0);
            }
        }

        public void SetVisibility(string iconName)
        {
            TickIcon.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
        }

        public void SetPinColumnState(bool isPinned)
        {
            if (isPinned) PinnIcon.Visibility = Visibility.Visible;
            else PinnIcon.Visibility = Visibility.Hidden;
        }

        private void LoginForwarded_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void LoginForwarded_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void LoginForwarded_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //return;
            int.TryParse(LoginForwarded.Tag.ToString(), out int userId);
            var user = Task.Run(() => ApiService.GetUserById(userId)).Result;

            if (user is null) return;
            if (!SetIsUserCanSeeChattersInfo(user.Id))
            {
                MessageBox.Show("No no no mister fish, you go to tasik");
                return;
            }

            if (_system.LoggedUser.Id == userId)
            {
                //set logged user info page
                LoggedUserProfile logged = new LoggedUserProfile(_system.LoggedUser, _system);
                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(logged);
                return;
            }

            //set chatter info page
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(userId);
            if (chat is null) return;

            UserInfo infoPage = new UserInfo(chat, _system);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(infoPage);
        }

        public bool SetIsUserCanSeeChattersInfo(int userId)
        {
            MainSettings setUserSettings = Task.Run(() => ApiService.GetSettingsByUserId(userId)).Result;

            if (setUserSettings.PrivacySettings.ForwardMesPrivacy
                .ShareWithExps.Any(x => x.Id == _system.LoggedUser.Id)) return true;
            else if (setUserSettings.PrivacySettings.ForwardMesPrivacy
                .NeverShareExps.Any(x => x.Id == _system.LoggedUser.Id)) return false;

            return setUserSettings.PrivacySettings.
                ForwardMesPrivacy.IsUserPageCanBeSeen(_system.Contacts, userId);
        }

    }
}
