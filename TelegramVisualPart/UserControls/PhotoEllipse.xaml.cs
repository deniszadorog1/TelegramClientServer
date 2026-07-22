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
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PhotoEllipse.xaml
    /// </summary>
    public partial class PhotoEllipse : UserControl
    {
        public PhotoEllipse()
        {
            InitializeComponent();
        }

        private TelegramLib.MainClasses.User _user;
        private TelegramLib.MainClasses.UserContactcs _contact;
        private TelegramLib.MainClasses.TelSystem _system;
        public void SetLoggedUser(TelegramLib.MainClasses.User user)
        {
            _user = user;
        }
        
        private void SetUserContacts(TelegramLib.MainClasses.UserContactcs contact)
        {
            _contact = contact;
        }
        
        private void SetSystemParam(TelegramLib.MainClasses.TelSystem system)
        {
            _system = system;
        }

        private void UserIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            const int moveVal = 50;
            Cursor = Cursors.Hand;

            AnimateMenuGrid(moveVal);
        }

        private void UserIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            AnimateMenuGrid(0);
        }

        private void AnimateMenuGrid(double toValue)
        {
            const int duration = 300;
            var anim = new DoubleAnimation
            {
                To = toValue,
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            (MenuGrid.RenderTransform as TranslateTransform)?.BeginAnimation(TranslateTransform.YProperty, anim);
        }

        public async Task SetUserInfo()
        {
            if (_system is null) return;
            UserImage.ImageSource = new BitmapImage(new Uri(
                 await FilesAction.GetUserImagePath(_system.LoggedUser.GetFirstImageName().Name), UriKind.Absolute));

        }

    }
}
