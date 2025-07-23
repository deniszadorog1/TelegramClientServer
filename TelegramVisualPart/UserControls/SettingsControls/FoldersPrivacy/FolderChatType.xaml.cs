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
    /// Логика взаимодействия для FolderChatType.xaml
    /// </summary>
    public partial class FolderChatType : UserControl
    {
        public FolderChatType()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeDeviderField"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private bool _isClicked = false;
        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeActivenessState();
        }

        public void ChangeActivenessState()
        {
            _isClicked = !_isClicked;

            if(_isClicked)
            {
                ChosenChatIconBorder.Background = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
                return;
            }
            ChosenChatIconBorder.Background = Brushes.Transparent;
        }

        public void HideIcon()
        {
            IconType.Visibility = Visibility.Hidden;
        }
    }
}
