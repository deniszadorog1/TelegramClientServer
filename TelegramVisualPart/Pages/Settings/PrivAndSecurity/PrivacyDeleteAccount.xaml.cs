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
using TelegramVisualPart.Enums;

using TelegramLib.UserSettings.SettingsTypes;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity
{
    /// <summary>
    /// Логика взаимодействия для PrivacyDeleteAccount.xaml
    /// </summary>
    public partial class PrivacyDeleteAccount : Page
    {
        private PrivAndSecSettings _settings;
        public PrivacyDeleteAccount(PrivAndSecSettings settings)
        {
            _settings = settings;
            InitializeComponent();

            SetParam();
        }

        public void SetParam()
        {
            int chosenIndex = (int)_settings.SelfDeleteTime;
            RadioPanel.Children.OfType<RadioButton>().ToList()[chosenIndex].IsChecked = true;
        }

        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radio) return;

           int index = RadioPanel.Children.OfType<RadioButton>().ToList().IndexOf(radio);

            _settings.SelfDeleteTime = (AwayForTime)index;
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
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

    }
}
