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

namespace TelegramVisualPart.UserControls.ChatsControls
{
    /// <summary>
    /// Логика взаимодействия для UserChatsMenuElement.xaml
    /// </summary>
    public partial class UserChatsMenuElement : UserControl
    {
        public UserChatsMenuElement()
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
    }
}
