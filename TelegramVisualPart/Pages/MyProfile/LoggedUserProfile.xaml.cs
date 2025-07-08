using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoggedUserProfile.xaml
    /// </summary>
    public partial class LoggedUserProfile : Page
    {
        public LoggedUserProfile()
        {
            InitializeComponent();
            SetBasicParams();
        }

        public void SetBasicParams()
        {
            CloseBut.IconType.Kind = PackIconKind.Close;
            SettingsBut.IconType.Kind = PackIconKind.LeadPencil;
        }

        private void Buts_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.White;
            Cursor = Cursors.Hand;
        }

        private void Buts_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.Gray;
            Cursor = null;
        }

        private void SettingsBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new MyProfile.MyProfileSettings());
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }
    }
}
