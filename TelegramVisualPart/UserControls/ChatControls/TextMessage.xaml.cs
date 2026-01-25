using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;
using static System.Net.Mime.MediaTypeNames;

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
        private TelegramLib.MainClasses.Messages.TextMessage _message;

        public TextMessage(TelSystem system,
            string text,
            string senderImageName,
            string fontName,
            bool isEdited,
            TelegramLib.MainClasses.Messages.TextMessage message,
            TelegramLib.MainClasses.Messages.Message? toReply = null,
            int? forwardedFrom = null)
        {
            _system = system;
            _text = text;
            _imgName = senderImageName;
            _toReply = toReply;
            _forwardedFrom = forwardedFrom;
            _message = message;

            InitializeComponent();

            //BgBrush.ImageSource = img.Source;
            //Message.Text = text;

            SetText(text);

            SetWidth(fontName);

            SetImageSource();
            SetFont(fontName);

            SetMessageReplyControl();

            SetForwardedFromRow();
            SetIsEditedVis(isEdited);

            SetEvents();
        }

        public void SetIsEditedVis(bool isVis)
        {
            const int baseEditColWidth = 40;
            if (isVis)
            {
                EditedColumn.Width =
                    new GridLength(baseEditColWidth);
                return;
            }
            EditedColumn.Width =
                new GridLength(0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_system.GetIsSavedMesChatStatus()) return;

            DependencyObject check = this.Parent;
            if (check is not ListBoxItem item) return;

            int.TryParse(item.Tag.ToString(), out int mesId);

            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);

            if (mes is null) mes = _system.GetScheduleMessageById(mesId);

            if (mes.SenderUserId == _system.LoggedUser.Id)
            {
                SolidColorBrush color =
                    (SolidColorBrush)System.Windows.Application.
                    Current.Resources["DarkThemeOne"];

                MessageColor.Background = color;
            }
        }

        public void SetText(string text)
        {
            text = text.Replace("\n", "");
            text = text.Replace("\r\n", "");

            var match = Regex.Match(text, @"https?:\/\/[^\s]+");

            if (match.Success)
            {
                string before = text.Substring(0, match.Index);
                string url = match.Value;
                string after = text.Substring(match.Index + match.Length);

                FirstPart.Text = before;
                LinkPart.Inlines.Add(url);
                SecondPart.Text = after;

                TrimText();
                return;
            }

            string linkText = new TextRange(
                    LinkPart.ContentStart,
                    LinkPart.ContentEnd).Text;

            FirstPart.Text = text;

            Message.Text = text;
            string fullText = FirstPart.Text + linkText + SecondPart.Text;
            SelectableText.Text = fullText;

            /*
                        LinkPart.Text = string.Empty;
                        SecondPart.Text = string.Empty;*/
            //TrimText();
        }

        public void TrimText()
        {
            FirstPart.Text = FirstPart.Text.Trim(' ');
            //LinkPart.Text = LinkPart.Text.Trim(' ');
            SecondPart.Text = SecondPart.Text.Trim(' ');
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

        private async void SetImageSource()
        {
            if (_imgName is null)
            {
                BgBrush.ImageSource = BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetSystemImagePath("StopSign.png"), UriKind.Absolute));
                return;
            }

            await SignalRHelperService.SetPhotoInEllipse(
                _system.GetUserById(_message.SenderUserId),
                BgBrush, UserEllipseImage);

            //this.Visibility = Visibility.Visible;
        }

        private const int _minMessageWidth = 125;
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
                VisualTreeHelper.GetDpi(System.Windows.Application.Current.MainWindow).PixelsPerDip
            );
            return formattedText.Width;
        }

        private const int _tickColWidth = 20;
        public void SetTickVis(string iconName)
        {
            TickColumn.Width = new GridLength(_tickColWidth);
            SetVisibility(iconName);
        }

        public void SetVisibility(string iconName)
        {
            ReadIconFlag.Kind =
                (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
        }

        private const int _basePinWidth = 15;
        public void SetPinColumnState(bool isPinned)
        {
            if (isPinned)
            {
                PinnIcon.Visibility = Visibility.Visible;
                PinColumn.Width = new GridLength(_basePinWidth);
                return;
            }
            PinnIcon.Visibility = Visibility.Hidden;
            PinColumn.Width = new GridLength(0);
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

            bool isSavedChat = _system.GetIsSavedMesChatStatus();

            //Settings logged user page
            if ((_system.LoggedUser.Id == mes.SenderUserId && !isSavedChat) ||

                (isSavedChat && mes.ForwardedFromId is null && mes.SenderUserId == 0) ||

                (isSavedChat && _system.LoggedUser.Id == mes.ForwardedFromId))
            {

                //MessageBox.Show("Ведутся технические работі");
                /*LoggedUserProfile logged =
                    new LoggedUserProfile(_system.LoggedUser, _system);*/

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


        private const int _selectTickColWidth = 30;
        public void SetTickVisibility(bool isVis)
        {
            if (isVis)
            {
                this.Width += _selectTickColWidth;
                TickColumnDef.Width = new GridLength(_selectTickColWidth);
            }
            else if (TickColumnDef.Width.Value != 0)
            {
                this.Width -= _selectTickColWidth;
                TickColumnDef.Width = new GridLength(0);
            }
        }

        public bool IsMessageIdTicked()
        {
            return SelectionTickObj.GetChosenStatus();
        }

        private void LinkPart_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not System.Windows.Documents.Hyperlink run) return;
            run.TextDecorations = TextDecorations.Underline;
            Cursor = Cursors.Hand;
            SelectableText.Visibility = Visibility.Hidden;
        }

        private void LinkPart_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not System.Windows.Documents.Hyperlink run) return;
            run.TextDecorations = null;
            Cursor = null;
            SelectableText.Visibility = Visibility.Visible;
        }

        private void LinkPart_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Documents.Hyperlink run) return;
            try
            {
                string linkText = new TextRange(
                        LinkPart.ContentStart,
                        LinkPart.ContentEnd).Text;

                Process.Start(new ProcessStartInfo(linkText)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Smth wrong with your url!!");
            }
        }

        private void Ellipse_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void GoToMessageGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void GoToMessageGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void GoToMessageGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
