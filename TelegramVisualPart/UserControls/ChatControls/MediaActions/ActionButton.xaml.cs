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

namespace TelegramVisualPart.UserControls.ChatControls.MediaActions
{
    /// <summary>
    /// Логика взаимодействия для ActionButton.xaml
    /// </summary>
    public partial class ActionButton : UserControl
    {
        public ActionButton()
        {
            InitializeComponent();
        }

        private void TestGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            TestBorder.Background = new SolidColorBrush(Colors.Gray);
            Cursor = Cursors.Hand;
        }

        private void TestGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            TestBorder.Background = new SolidColorBrush(Colors.Transparent);
            Cursor = null;
        }
    }
}
