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
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity
{
    /// <summary>
    /// Логика взаимодействия для PrivacyMessages.xaml
    /// </summary>
    public partial class PrivacyMessages : Page
    {
        private MessagesSub _mesSubs;
        public PrivacyMessages(MessagesSub messSettings)
        {
            _mesSubs = messSettings;
            InitializeComponent();

            SetClassParam();
        }

        public void SetClassParam()
        {
            if (_mesSubs.WhoCanSend == ShareWith.Everybody) EverybodyRadio.IsChecked = true;
            else if (_mesSubs.WhoCanSend == ShareWith.Contacts) MyContactsRadio.IsChecked = true;
        }

        private void EverybodyRadio_Checked(object sender, RoutedEventArgs e)
        {
            _mesSubs.WhoCanSend = ShareWith.Everybody;
        }

        private void MyContactsRadio_Checked(object sender, RoutedEventArgs e)
        {
            _mesSubs.WhoCanSend = ShareWith.Contacts;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }
    }
}
