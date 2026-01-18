using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserTalkMessage.xaml
    /// </summary>
    public partial class UserTalkMessage : UserControl
    {
        private string _imgName;
        private TelegramLib.MainClasses.UserChat _chat;
        public UserTalkMessage(string imgName)
        {
            _imgName = imgName;
            InitializeComponent();

            SetContactImage();
        }

        public void SetChat(TelegramLib.MainClasses.UserChat chat)
        {
            _chat = chat;
            if(_chat is TelegramLib.MainClasses.SavedMessagesChat)
            {
                SetSavedMessageBlock();
            }
        }

        public TelegramLib.MainClasses.UserChat GetChat()
        {
            return _chat;
        }

        public void SetNewImgName(string newName)
        {
            _imgName = newName;
        }

        public void SetContactImage()
        {
            if (_imgName is null)
            {
                ImageIcon.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetSystemImagePath("StopSign.png"), UriKind.Absolute));
                return;
            }

            ImageIcon.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_imgName), UriKind.Absolute));
        }

        public void UpdateImage(string imgName)
        {
            _imgName = imgName;
            SetContactImage();
        }

        public string GetLastMessageText()
        {
            return LastMessage.Text;
        }

        public string GetFriendName()
        {
            return FriendLogin.Text;
        }

        public void SetDefaultValues()
        {
            LastMessage.Text = "no messages";
            LastMessageTime.Text = "message time";
        }

        public void SetVisibilityToPinBlock(bool isVisible)
        {
            PinBlock.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public void ChangePinVisOnOposit()
        {
            PinBlock.Visibility = PinBlock.Visibility == Visibility.Hidden ? 
                Visibility.Visible : Visibility.Hidden;
        }

        public void SetVisibilityToUnreadEllipse(bool isVisible)
        {
            UnreadEllipse.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public void ChangeUnreadEllipseVisOnOtherDirection()
        {
            UnreadEllipse.Visibility = UnreadEllipse.Visibility == Visibility.Hidden ?
                Visibility.Visible : Visibility.Hidden;
        }

        public void SetUnreadMessageValue(int amount)
        {
            UnredCountBorder.Visibility = amount > 0 ? Visibility.Visible : Visibility.Hidden;
            MesCounter.Text = amount.ToString();
        }

        public void SetUnreadAmountVisibility(bool vis)
        {
            UnredCountBorder.Visibility = vis ? Visibility.Visible : Visibility.Hidden;
        }

        public void SetBackground(SolidColorBrush background)
        {
            Background = background;
        }

        public void SetAutoDelDurationCircle(AutoDeleteType type, 
            string duration)
        {
            if (type == AutoDeleteType.Nothing)
            {
                AutoDelDurationGrid.Visibility = Visibility.Hidden;
                return;
            }

            AutoDelDurationGrid.Visibility = Visibility.Visible;
            AutoDelDurBlock.Text = duration;
        }

        public void SetText(string text)
        {
            LastMessage.Text = Regex.Replace(text, @"^\s+|\s+$", "");
        }

        public void SetSavedMessageBlock()
        {
            ImageIcon = null;

            UserEllipseImage.Fill = 
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
            SavedMassageIcon.Visibility = Visibility.Visible;
        }
    }
}
