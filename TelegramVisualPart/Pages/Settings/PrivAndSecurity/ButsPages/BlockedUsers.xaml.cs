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
using TelegramVisualPart.Pages.Contacts;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages
{
    /// <summary>
    /// Логика взаимодействия для BlockedUsers.xaml
    /// </summary>
    public partial class BlockedUsers : Page
    {
        public BlockedUsers()
        {
            InitializeComponent();

            SetButsVisualState();
        }

        public void SetButsVisualState()
        {
            ToBlockBut.IconType.Foreground = 
                (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButForeGround"];
            ToBlockBut.IconType.Kind = PackIconKind.Hand;

            ToBlockBut.ButName.Foreground =
                (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButForeGround"];
            ToBlockBut.ButName.Text = "Block user";
        }

        private void Buts_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;

            if (sender is PackIcon icon)
            {
                icon.Foreground = Brushes.White;
            }
        }

        private void Buts_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is PackIcon icon)
            {
                icon.Foreground = Brushes.Gray;
            }
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new PrivacyAndSecurity());
        }

        private void ToBlockBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContacts toBlock = new MainContacts(Enums.ContactsPageAction.Block);

            toBlock.ContactsBlock.Text = "Select user to block";
            toBlock.SortBut.Visibility = Visibility.Hidden;
            toBlock.AddContactBut.Visibility = Visibility.Hidden;

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(toBlock);
        }
    }
}
