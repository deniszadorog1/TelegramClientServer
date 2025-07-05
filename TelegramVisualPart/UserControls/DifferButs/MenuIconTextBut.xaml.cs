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

namespace TelegramVisualPart.UserControls.DifferButs
{
    /// <summary>
    /// Логика взаимодействия для MenuIconTextBut.xaml
    /// </summary>
    public partial class MenuIconTextBut : UserControl
    {
        public MenuIconTextBut()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = Brushes.Transparent;
            Cursor = null;
        } 
    }
}
