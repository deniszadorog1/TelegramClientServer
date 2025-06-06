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

namespace TelegramVisualPart.UserControls.SettingsControls.FoldersPrivacy
{
    /// <summary>
    /// Логика взаимодействия для ChatToAddInFolder.xaml
    /// </summary>
    public partial class ChatToAddInFolder : UserControl
    {
        public ChatToAddInFolder()
        {
            InitializeComponent();
        }

        private void ColumnDefinition_MouseWheel(object sender, MouseWheelEventArgs e)
        {

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

        private bool _isClicked = false;
        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isClicked = !_isClicked;

            if (_isClicked)
            {
                ChosenChatIconBorder.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButForeGround"];
                return;
            }
            ChosenChatIconBorder.Background = Brushes.Transparent;
        }
    }
}
