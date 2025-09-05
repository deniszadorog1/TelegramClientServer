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

namespace TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.SetWallpapersControls
{
    /// <summary>
    /// Логика взаимодействия для WallpaperBotBut.xaml
    /// </summary>
    public partial class WallpaperBotBut : UserControl
    {
        public WallpaperBotBut()
        {
            InitializeComponent();
        }

        private void BorderBg_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderBg.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void BorderBg_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderBg.Background = new SolidColorBrush(Colors.Gray);
                  // (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        private void BorderBg_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
