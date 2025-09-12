using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace TelegramVisualPart.Pages.LittleMenuPages
{
    /// <summary>
    /// Логика взаимодействия для CustomMute.xaml
    /// </summary>
    public partial class CustomMute : Page
    {
        private TelSystem _system;
        public CustomMute(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            DaysBox.AdditionalTextBlock.Text = "Days";
            HoursBox.AdditionalTextBlock.Text = "Hours";
            MinutesBox.AdditionalTextBlock.Text = "Minutes";
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is System.Windows.Controls.Button but) but.Background =
                (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is System.Windows.Controls.Button but)
                but.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void MuteBut_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(DaysBox.Content.ToString(), out int days);
            int.TryParse(HoursBox.Content.ToString(), out int hours);
            int.TryParse(MinutesBox.Content.ToString(), out int minutes);

            _system.Settings.SoundNotifSettings
                .SetCustomDate(days, hours, minutes);

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this))
                .SetSecondaryFrame(new MuteDuration(_system));
        }
    }
}
