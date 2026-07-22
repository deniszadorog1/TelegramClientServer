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
using TelegramVisualPart.Pages.UserInfoContact.ActionsFolder;

namespace TelegramVisualPart.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ToggleIconBut.xaml
    /// </summary>
    public partial class ToggleIconBut : UserControl
    {
        public ToggleIconBut()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = Brushes.Transparent;

        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Toggle.IsChecked = !Toggle.IsChecked;
        }

        private void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
