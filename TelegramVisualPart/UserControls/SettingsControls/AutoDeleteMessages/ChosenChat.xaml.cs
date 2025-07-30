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
    /// Логика взаимодействия для ChosenChat.xaml
    /// </summary>
    public partial class ChosenChat : UserControl
    {
        public event EventHandler _removeChatEvent;

        public ChosenChat()
        {
            InitializeComponent();
        }

        public void SetBasicParams(string userImgName, string name)
        {
            UserImageBrush.ImageSource = new BitmapImage(new Uri(FilesAction.GetUserImagePath(userImgName), UriKind.Absolute));
            UserName.Text = name;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            IsEnterBut.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            IsEnterBut.Visibility = Visibility.Hidden;
        }


        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private string _clickedChatName = string.Empty;
        private void RemoveChat_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            _clickedChatName = this.Name;
            _removeChatEvent?.Invoke(this, EventArgs.Empty);
        }

        public string GetThisName()
        {
            return _clickedChatName;
        }
    }
}
