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
