using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.SignalR.Protocol;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;
using Image = System.Windows.Controls.Image;

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

        private bool _isOnlyVisual = false;
        private MediaAction _message;

        public event Action PushForwarded;

        private List<string> _bandPaths = new List<string>();
        private bool _isImageBand;
        public List<Border> _bandBorders = new List<Border>();
        private List<ImageBrush> _bandBrushes = new List<ImageBrush>();

        //Send band of medias
        public MediaMessage(TelSystem system, List<string> paths, bool isImage)
        {
            _isOnlyVisual = false;
            _system = system;
            IsSticker = false;

            _bandPaths = paths;
            _isImageBand = isImage;

            InitializeComponent();

            HideBordersExceptBand();
            SetBandBorderLists();
            SetMessages();
        }

        public bool IsBandBorderContainsId(int mesId)
        {
            return _bandBorders.Any(x => x.Tag is not null && x.Tag.ToString() == mesId.ToString());
        }

        public bool IsBandMedia() => _bandPaths.Count > 1;

        public void SetMessages()
        {
            if (!_isImageBand) return;

            for (int i = 0; i < _bandPaths.Count; i++)
            {
                SetBandImg(_bandBorders[i], _bandBrushes[i], _bandPaths[i]);
            }

            if (_bandPaths.Count <= 6) BottomBandRow.Height = new GridLength(0);
            if (_bandPaths.Count <= 3) MiddleBandRow.Height = new GridLength(0);
            if (_bandPaths.Count <= 2) RightBandColumn.Width = new GridLength(0);
            if (_bandPaths.Count <= 1) MiddleBandColumn.Width = new GridLength(0);


            for(int i = _bandPaths.Count; i < _bandBorders.Count; i++)
            {
                _bandBorders[i].Width = 0;
                _bandBorders[i].Height = 0;
                _bandBorders[i].Visibility = Visibility.Hidden;
            }
        }

        public string GetImageBorderSource(int messageId)
        {
            for(int i = 0; i < _bandBorders.Count; i++)
            {
                if (_bandBorders[i].Tag is not null && 
                    _bandBorders[i].Tag.ToString() == messageId.ToString())
                {
                    string res = System.IO.Path.GetFileName(_bandPaths[i]);
                    return FilesAction.GetFullChatImagePath(res);
                }
            }
            return string.Empty;
        }

        public void SetBandImg(Border border, ImageBrush brush, string path)
        {
            path = System.IO.Path.GetFileName(path);

            border.Visibility = Visibility.Visible;
            brush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetFullChatImagePath(path), UriKind.Absolute));
        }

        private void SetBandBorderLists()
        {
            _bandBorders.Add(OneInGroupBorder);
            _bandBorders.Add(TwoInGroupBorder);
            _bandBorders.Add(ThreeInGroupBorder);
            _bandBorders.Add(FourInGroupBorder);
            _bandBorders.Add(FiveInGroupBorder);
            _bandBorders.Add(SixInGroupBorder);
            _bandBorders.Add(SevenInGroupBorder);
            _bandBorders.Add(EightInGroupBorder);
            _bandBorders.Add(NineInGroupBorder);

            _bandBrushes.Add(OneImg);
            _bandBrushes.Add(TwoImg);
            _bandBrushes.Add(ThreeImg);
            _bandBrushes.Add(FourImg);
            _bandBrushes.Add(FiveImg);
            _bandBrushes.Add(SixImg);
            _bandBrushes.Add(SevenImg);
            _bandBrushes.Add(EightImg);
            _bandBrushes.Add(NineImg);
        }

        public void SetTagIdsToBandBorders(List<MediaAction> medias)
        {
            for(int i = 0; i < medias.Count; i++)
            {
                _bandBorders[i].Tag = medias[i].Id;
            }
        }

        public List<int> GetBandMessagesIds()
        {
            List<int> res = new List<int>();

            for(int i = 0; i < _bandBorders.Count; i++)
            {
                if (_bandBorders[i] is not null &&  _bandBorders[i].Tag is not null &&
                    int.TryParse(_bandBorders[i].Tag.ToString(), out int id))
                {
                    res.Add(id);
                } 
            }
            return res;
        }

        private void HideBordersExceptBand()
        {
            GifBorder.Visibility = Visibility.Hidden;
            VideoBorder.Visibility = Visibility.Hidden;
            ImageBorder.Visibility = Visibility.Hidden;

            ImgGroupBorder.Visibility = Visibility.Visible;
        }

        public MediaMessage(TelSystem system, MediaAction media)
        {
            _isOnlyVisual = true;
            _message = media;

            _system = system;
            IsSticker = media.IsSticker;
            _forwardedFrom = media.ForwardedFromId;
            //_senderImgName = media.SenderUserId;

            InitializeComponent();
            HideAllBorders();
            SetMedia();

            SetTime(media.SentTime);
            SetForwardedFromRow();
        }

        public async ValueTask SetMedia()
        {
            if (_message.IsImage())
            {
                ImgMessage.ImageSource =
                    new BitmapImage(new Uri(
                        FilesAction.GetFullChatImagePath(_message.MediaName),
                        UriKind.Absolute));

                //SetImgMessageSize(_img, ImageBorder);
                ImageBorder.Visibility = Visibility.Visible;
            }
            else if (_message.IsVideo())
            {
                string name = System.IO.Path.GetFileName(_message.MediaName);
                Image img = await VisHelper.GetFirstFrameAsync(name);

                GifBorder.Visibility = Visibility.Visible;
                GifImage.Source = img.Source;
            }
            else if (_message.IsGif())
            {
                string name = System.IO.Path.GetFileName(_message.MediaName);
                string gifPath = FilesAction.GetFullGifPath(name);

                BitmapSource source = FilesAction.GetFirstImageFromGif(gifPath);
                if (source is null) return;

                GifBorder.Tag = name;
                GifBorder.Visibility = Visibility.Visible;
                GifImage.Source = source;
            }
        }

        public MediaMessage(TelSystem system,
            System.Windows.Controls.Image img, bool isSticker,
            string senderImgName, DateTime sendTime,
            int? forwardedFromId = null)
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

        public MediaMessage(TelSystem system, string gifPath,
            string senderImgName, DateTime sentTime,
            int? forwardedFromId)
        {
            _system = system;
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

        public void SetTime(DateTime time)
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

        public MediaMessage(TelSystem system,
            MediaElement media,
            string senderImgName,
            MediaAction mediaLogicEl,
            int? forwardedFromId)
        {
            _system = system;
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

        private const int _visForwardRowHeight = 20;
        private async Task SetForwardedFromRow()
        {
            if (_forwardedFrom is null) return;
            TelegramLib.MainClasses.User from =
                await ApiService.GetUserById((int)_forwardedFrom);
            if (from is null) return;

            //Set forwarded from user id as tag
            LoginForwarded.Tag = from.Id;

            ForwardedRow.Height = new GridLength(_visForwardRowHeight);
            LoginForwarded.Text = from.Login;
        }

        public void SetForwardedRowHeight(bool isShow)
        {
            if (isShow) ForwardedRow.Height = new GridLength(_visForwardRowHeight);
            else ForwardedRow.Height = new GridLength(0);
        }

        public bool IsForwardedRowIsHidden()
        {
            return ForwardedRow.Height.Value == 0;
        }

        public bool IsMessageIdTicked()
        {
            return SelectionTickObj.GetChosenStatus();
        }

        public void SetSenderImage()
        {
            if (_senderImgName is null)
            {
                BgBrush.ImageSource = new BitmapImage(new Uri(
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
            if (_isOnlyVisual) return;
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
            if (_isOnlyVisual) return;
            Cursor = Cursors.Hand;
        }

        private void LoginForwarded_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void LoginForwarded_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isOnlyVisual) return;

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
        public bool IsTickVisible()
        {
            return TickColumnDef.Width.Value != 0;
        }

        public void ChangeTickStatus()
        {
            if (!IsTickVisible()) return;
            SelectionTickObj.SetMirrorStatus();
        }

        public MediaAction GetMessage()
        {
            return _message;
        }

        private void GoToForwardedGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void GoToForwardedGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void GoToForwardedGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PushForwarded.Invoke();
        }

        public void SetPushForwardedVis()
        {
            const int goToMesGridWidth = 30;

            GoToMessage.Width = new GridLength(goToMesGridWidth);
            Width += goToMesGridWidth;

            ForwardedGrid.Visibility = Visibility.Hidden;
            Height -= _visForwardRowHeight;
            ForwardedRow.Height = new GridLength(0);
        }

        private void ImageBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void ImageBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void ImageBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_system is null) return;
            //Get user 
            DependencyObject check = this.Parent;
            if (check is not ListBoxItem item) return;

            int.TryParse(item.Tag.ToString(), out int mesId);

            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);
            if (mes is null) return;

            bool isSavedChat = _system.GetIsSavedMesChatStatus();

            //Settings logged user page
            if ((_system.LoggedUser.Id == mes.SenderUserId && !isSavedChat) ||

                (isSavedChat && mes.ForwardedFromId is null && mes.SenderUserId == 0) ||
                (isSavedChat && _system.LoggedUser.Id == mes.ForwardedFromId))
            {

                UserInfo logged = new UserInfo(_system.SavedMesesChat, _system);
                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(logged);
                return;
            }

            //Set other user page
            TelegramLib.MainClasses.UserChat chat = isSavedChat && mes.ForwardedFromId is not null ?
                _system.GetChatByChatterId((int)mes.ForwardedFromId) :
                _system.GetChatByMessage(mes);

            UserInfo info = new UserInfo(chat, _system);
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(info);
        }

        public string GetMediaPath()
        {
            if (_gifPath is not null && _gifPath != string.Empty) return _gifPath;
            if (_stickerPath is not null && _stickerPath != string.Empty) return _stickerPath;
            if (_message is not null) return _message.MediaName;

            Console.WriteLine(GifImage.Source);
            Console.WriteLine(ImgMessage.ImageSource);
            Console.WriteLine(MyVideoPlayer);

            return string.Empty;
        }


    }


}
