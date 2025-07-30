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
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages
{
    /// <summary>
    /// Логика взаимодействия для ChatToApply.xaml
    /// </summary>
    public partial class ChatToApply : UserControl
    {
        public ChatToApply()
        {
            InitializeComponent();
        }

        public void SetParams(string imgName, string upperText, string bottomText)
        {
            UserImageBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(imgName), UriKind.Absolute));

            TypeName.Text = upperText;
            AutoDeletionType.Text = bottomText;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeDeviderField"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = new SolidColorBrush(Colors.Transparent);
        }

        private bool _isClicked = false;
        public void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isClicked = !_isClicked;

            if (_isClicked)
            {
                ChosenChatIconBorder.Background = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
                return;
            }
            ChosenChatIconBorder.Background = Brushes.Transparent;
        }

        public bool GetIdClicked()
        {
            return _isClicked;
        }

        public void DiscardChat()
        {
            _isClicked = false;
            ChosenChatIconBorder.Background = Brushes.Transparent;
        }

        public string GetTypeName()
        {
            return TypeName.Text;
        }
    }
}
