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

namespace TelegramVisualPart.UserControls.MyProfileControls
{
    /// <summary>
    /// Логика взаимодействия для MyProfileSettingsButton.xaml
    /// </summary>
    public partial class MyProfileSettingsButton : UserControl
    {
        public MyProfileSettingsButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background =(SolidColorBrush)Application.Current.FindResource("DarkThemeMouseEnterBut");

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = Brushes.Transparent;
        }
    }
}
