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

namespace TelegramVisualPart.Pages.Settings.Folders
{
    /// <summary>
    /// Логика взаимодействия для FoldersPage.xaml
    /// </summary>
    public partial class FoldersPage : Page
    {
        private Frame _frame;
        public FoldersPage(Frame frame)
        {
            _frame = frame;
            InitializeComponent();
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
            ((MainWindow)Window.GetWindow(_frame)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(_frame)).SetSecondaryFrame(new SettingsPage(_frame));
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateNewFolderBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(_frame)).SetThirdFrame(new FolderAction(_frame));
        }
    }
}
