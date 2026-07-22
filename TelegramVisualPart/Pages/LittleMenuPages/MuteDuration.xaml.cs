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

namespace TelegramVisualPart.Pages.LittleMenuPages
{
    /// <summary>
    /// Логика взаимодействия для MuteDuration.xaml
    /// </summary>
    public partial class MuteDuration : Page
    {
        private TelSystem _system;

        private readonly List<string> _durations = new List<string>()
        {
            "",
            "",
            "15 minutes",
            "30 minutes",
            "1 hour",
            "2 hours",
            "3 hours",
            "4 hours",
            "8 hours",
            "12 hours",
            "1 day",
            "2 days",
            "3 days",
            "1 week",
            "2 weeks",
            "1 month",
            "2 months",
            "3 months",
            "",
            "",
        };


        public MuteDuration(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            ChooseDate.SetListWithBlocks(_durations);
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
            //Get mute value
            int selectedIndex = ChooseDate.GetSelectedIndex();

            _system.Settings.SoundNotifSettings.AddDuration(selectedIndex);
            _system.Settings.SoundNotifSettings.SetMuteTime();

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void DotsGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Dots.Foreground = Brushes.White;
            Cursor = Cursors.Hand;
        }

        private void DotsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Dots.Foreground = Brushes.Gray;
            Cursor = null;
        }

        private void DotsGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set custom mut choose
            ((MainWindow)Window.GetWindow(this))
                .SetSecondaryFrame(new CustomMute(_system));
        }
    }
}
