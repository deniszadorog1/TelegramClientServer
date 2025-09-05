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
using TelegramVisualPart.Enums;

namespace TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls
{
    /// <summary>
    /// Логика взаимодействия для ThemeSetBut.xaml
    /// </summary>
    public partial class ThemeSetBut : UserControl
    {
        public ThemeSetBut()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = Brushes.Transparent;
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
