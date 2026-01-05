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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using TelegramVisualPart.UserControls;
using TelegramVisualPart.UserControls.ChatsControls;

namespace TelegramVisualPart.CustWindows
{
    /// <summary>
    /// Логика взаимодействия для MenuGlobalWindow.xaml
    /// </summary>
    public partial class MenuGlobalWindow : Window
    {
        private MainWindow _godWindow;
        private UserChatMenu _menu;
        private Point _loc;

        public MenuGlobalWindow(MainWindow godWindow,
            UserChatMenu menu, Point menuLoc)
        {
            _godWindow = godWindow;
            _menu = menu;
            _loc = menuLoc;

            InitializeComponent();

            AddMenu();
        }

        public void AddMenu()
        {
            _loc = new Point(
                _loc.X + _godWindow.Left,
                _loc.Y + _godWindow.Top);

            Canvas.SetLeft(_menu, _loc.X);
            Canvas.SetTop(_menu, _loc.Y);

            MenusCanvas.Children.Add(_menu);
        }

        public UserChatMenu? GetMenu()
        {
            return MenusCanvas.Children
                .OfType<UserChatMenu>()
                .FirstOrDefault();
        }

        private void MenusCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MenusCanvas.Children.Clear();
            this.Close();
        }
    }
}
