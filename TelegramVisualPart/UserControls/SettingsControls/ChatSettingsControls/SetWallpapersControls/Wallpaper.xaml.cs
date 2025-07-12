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
    /// Логика взаимодействия для Wallpaper.xaml
    /// </summary>
    public partial class Wallpaper : UserControl
    {
        public Wallpaper()
        {
            InitializeComponent();
        }

        private void WallpaperImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void WallpaperImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
