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

namespace TelegramVisualPart.UserControls.ContactsControls
{
    /// <summary>
    /// Логика взаимодействия для UserContact.xaml
    /// </summary>
    public partial class UserContact : UserControl
    {
        public UserContact()
        {
            InitializeComponent();
        }

        private string _imgSource;
        private string _login;
        private DateTime? _lastSeenOnline;

        public UserContact(string imgSource, string login,
            DateTime? lastOnline)
        {
            _imgSource = imgSource;
            _login = login;
            _lastSeenOnline = lastOnline;

            InitializeComponent();

            SetParams();
        }

        public void SetParams()
        {
            if (_imgSource != string.Empty)
            {
                ImgBrushSource.ImageSource = new BitmapImage(new Uri(_imgSource, UriKind.Absolute));
            }

            UserLogin.Text = _login;
            if (_lastSeenOnline is not null)
                LastSennOnline.Text = $"{_lastSeenOnline.Value.Month}.{_lastSeenOnline.Value.Day}.{_lastSeenOnline.Value.Year}";
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = Brushes.Transparent;
        }
    }
}
