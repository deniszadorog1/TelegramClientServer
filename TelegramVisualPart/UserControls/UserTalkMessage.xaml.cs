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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        public UserTalkMessage(string imgName)
        {
            _imgName = imgName;
            InitializeComponent();

            SetContactImage();
        }

        public void SetContactImage()
        {
            ImageIcon.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_imgName), UriKind.Absolute));
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
    }
}
