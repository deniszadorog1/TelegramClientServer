using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu
{
    /// <summary>
    /// Логика взаимодействия для MesMenuBut.xaml
    /// </summary>
    public partial class MesMenuBut : UserControl
    {
        public MesMenuBut()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Background = 
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = new SolidColorBrush(Colors.Transparent);
        }

        public void SetParams(PackIconKind kind, string text)
        {
            Icon.Kind = kind;
            ButText.Text = text;
        }

        public void PaintBlocks(SolidColorBrush color)
        {
            Icon.Foreground = color;
            ButText.Foreground = color;
        }
    }
}
