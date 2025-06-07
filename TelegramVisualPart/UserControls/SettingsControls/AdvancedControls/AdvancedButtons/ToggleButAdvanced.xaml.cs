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

namespace TelegramVisualPart.UserControls.SettingsControls.AdvancedControls.AdvancedButtons
{
    /// <summary>
    /// Логика взаимодействия для ToggleButAdvanced.xaml
    /// </summary>
    public partial class ToggleButAdvanced : UserControl
    {
        public ToggleButAdvanced()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeDeviderField"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = Brushes.Transparent;
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Toggle.IsChecked = !Toggle.IsChecked;
        }
    }
}
