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

namespace TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages
{
    /// <summary>
    /// Логика взаимодействия для SetChatWallpaper.xaml
    /// </summary>
    public partial class SetChatWallpaper : Page
    {
        public SetChatWallpaper()
        {
            InitializeComponent();
        }

        private void CloseBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void ChooseFromFileGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ChooseFromFileGrid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];

        }

        private void ChooseFromFileGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ChooseFromFileGrid.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ChooseFromFileGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new WallpaperPreview());
        }

        private void WallpaperBotBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearPageFromParentFrame(this);
        }
    }
}
