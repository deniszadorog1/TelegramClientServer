using MaterialDesignThemes.Wpf;
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
using TelegramLib.Models;
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages
{
    /// <summary>
    /// Логика взаимодействия для ShareContactControl.xaml
    /// </summary>
    public partial class ShareContactControl : UserControl
    {
        public event Func<Task> SharedClicked;
        public ShareContactControl()
        {
            InitializeComponent();
        }

        private void ContactRow_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void ContactRow_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void SetSenderImage(string imgName)
        {
            BgBrush.ImageSource = new BitmapImage(
            new Uri(FilesAction.GetUserImagePath(imgName), UriKind.Absolute));
        }

        public void SetSharedUserImage(string imgName)
        {
            ImageIcon.ImageSource = new BitmapImage(
                new Uri(FilesAction.GetUserImagePath(imgName), UriKind.Absolute));
        }

        public void SetSharedUserName(string name)
        {
            NameBlock.Text = name;
        }

        public void SetSharedUserPhoneNumber(string number)
        {
            PhoneNumberBlock.Text = number;
        }

        public void SetSendTime()
        {
            string hours = MakeCorrectTimeString(DateTime.Now.Hour.ToString());
            string minutes = MakeCorrectTimeString(DateTime.Now.Minute.ToString());
            
            SendTimeBlock.Text = $"{hours}:{minutes}";
        }

        private string MakeCorrectTimeString(string temp)
        {
            if (temp.Length < 2) temp = temp.Insert(0, "0");
            return temp;
        }

        private void ContactRow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SharedClicked?.Invoke();
        }

        private const int _tickColWidth = 25;
        public void SetTickVis(string iconName)
        {
            TickColumn.Width = new GridLength(_tickColWidth);
            SetVisibility(iconName);
        }

        public void SetVisibility(string iconName)
        {
            TickIcon.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
        }
    }
}
