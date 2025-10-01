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
using TelegramLib.MainClasses;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages
{
    /// <summary>
    /// Логика взаимодействия для SetLocalCode.xaml
    /// </summary>
    public partial class SetLocalCode : Page
    {
        private TelSystem _system;
        private bool _isEnterCode;
        
        public SetLocalCode(TelSystem system, bool isEnterCode = false)
        {
            _system = system;
            _isEnterCode = isEnterCode;

            InitializeComponent();
            SetBasicParams();

            SetPassCode();
        }

        public void SetPassCode()
        {
            if (!_isEnterCode) return;

            SecondBoxCode.Visibility = Visibility.Hidden;

            AddPasscode.Content = "Submit";

            BoxesRow.Height = new GridLength(BoxesRow.Height.Value - 60);
            //Height -= 160;
        }

        public void SetBasicParams()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new PrivacyAndSecurity(_system));
        }

        private void AddPasscode_Click(object sender, RoutedEventArgs e)
        {
            if (_isEnterCode)
            {
                if (FirstCodeBox.Text !=
                    _system.Settings.PrivacySettings.PassCode.PassCode) return;

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                    new PasscodePages.PasscodePage(_system));
                return;
            }


            if (string.IsNullOrWhiteSpace(FirstCodeBox.Text) ||
               string.IsNullOrWhiteSpace(SecondBoxCode.Text) ||
               FirstCodeBox.Text != SecondBoxCode.Text)
            {
                return;
            }

            _system.Settings.PrivacySettings.PassCode =
                new TelegramLib.UserSettings.SettingsTypes.SubSettings
                .PrivAnSecSubs.PasscodeSettings();

            _system.Settings.PrivacySettings.PassCode.PassCode = FirstCodeBox.Text;

           ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new PasscodePages.PasscodePage(_system));
        }
    }
}
