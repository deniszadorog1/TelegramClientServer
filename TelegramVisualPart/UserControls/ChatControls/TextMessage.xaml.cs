using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
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
using TelegramLib.UserSettings;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для TextMessage.xaml
    /// </summary>
    public partial class TextMessage : UserControl
    {
        private string _text;
        private string _imgName;
        private TelSystem _system;
        private TelegramLib.MainClasses.Messages.Message? _toReply;

        private int? _forwardedFrom = null;

        public TextMessage(TelSystem system,
            string text, string senderImageName,
            string fontName,
            TelegramLib.MainClasses.Messages.Message? toReply = null,
            int? forwardedFrom = null)
        {
            _system = system;
            _text = text;
            _imgName = senderImageName;
            _toReply = toReply;
            _forwardedFrom = forwardedFrom;

            InitializeComponent();

            //BgBrush.ImageSource = img.Source;
            Message.Text = text;
            SetWidth(fontName);

            SetImageSource();
            SetFont(fontName);

            SetMessageReplyControl();

            SetForwardedFromRow();

            SetEvents();
        }

        public void SetEvents()
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

            ForwardedRow.Height = new GridLength(40);
            LoginForwarded.Text = from.Login;
        }
        private void SetMessageReplyControl()
        {
            if (_toReply is null || _system is null)
            {
                ReplyedRow.Height = new GridLength(0);
                //Set null value
                return;
            }
            ReplyedRow.Height = new GridLength(50);
            ReplyControl.SetReplyMessageParams(_system, _toReply);

            ReplyControl.PreviewMouseDown += (sender, e) =>
            {
                if (_toReply is null) return;
                //Set scrolling to message
                ((MainWindow)Window.GetWindow(this))
                .ShowChosenMessageByMessageId(_toReply.Id);
            };
        }

        private void SetFont(string font)
        {
            if (font == string.Empty) font = "Times New Roman";
            Message.FontFamily = new FontFamily(font);
        }

        private void SetImageSource()
        {
            if (_imgName is null)
            {
                BgBrush.ImageSource = BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetSystemImagePath("StopSign.png"), UriKind.Absolute));
                return;
            }

            BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_imgName), UriKind.Absolute));
        }

        private const int _minMessageWidth = 75;
        private void SetWidth(string fontName)
        {
            double blockSize = GetStringWidth(fontFamily: fontName) + 10;

            if (blockSize < _minMessageWidth) blockSize = _minMessageWidth;

            Width = blockSize + ImageColumnSize.Width.Value + Message.FontSize;
            SetTime();
            //Height = 50;
        }

        private void SetTime()
        {
            DateTime time = DateTime.Now;
            SetTime(DateTime.Now);
            //SentTime.Text = $"{VisHelper.GetCorrectTimeParamVis(time.Hour.ToString())}:" +
            //    $"{VisHelper.GetCorrectTimeParamVis(time.Minute.ToString())}";
        } 

        public void SetTime(DateTime time)
        {
            SentTime.Text = $"{VisHelper.GetCorrectTimeParamVis(time.Hour.ToString())}:" +
                $"{VisHelper.GetCorrectTimeParamVis(time.Minute.ToString())}";
        }

        public double GetStringWidth(string fontFamily = "Segoe UI")
        {
            var typeface = new Typeface(new FontFamily(fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            var formattedText = new FormattedText(
                _text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                Message.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip
            );
            return formattedText.Width;
        }

        private const int _tickColWidth = 25;
        public void SetTickVis(string iconName)
        {
            TickColumn.Width = new GridLength(_tickColWidth);
            SetVisibility(iconName);
        }

        public void SetVisibility(string iconName)
        {
            ReadIconFlag.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
        }

        private const int _basePinWidth = 20;
        public void SetPinColumnState(bool isPinned)
        {
            if (isPinned) PinnIcon.Visibility = Visibility.Visible;
            else PinnIcon.Visibility = Visibility.Hidden;
        }

        private void LoginForwarded_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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

        private void LoginForwarded_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void LoginForwarded_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void UserEllipseImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserEllipseImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void UserEllipseImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Get user 
            DependencyObject check = this.Parent;
            if (check is not ListBoxItem item) return;

            int.TryParse(item.Tag.ToString(), out int mesId);

            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);
            if (mes is null) return;

            //Settings logged user page
            if (_system.LoggedUser.Id == mes.SenderUserId)
            {
                LoggedUserProfile logged =
                    new LoggedUserProfile(_system.LoggedUser, _system);

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(logged);
                return;
            }

            //Set other user page
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByMessage(mes);

            UserInfo info = new UserInfo(chat, _system);
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(info);
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

        public bool IsMessageIdTicked()
        {
            return SelectionTickObj.GetChosenStatus();
        }
    }
}
