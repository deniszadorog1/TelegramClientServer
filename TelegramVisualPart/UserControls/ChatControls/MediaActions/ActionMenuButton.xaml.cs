using MaterialDesignThemes.Wpf;
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
    /// Логика взаимодействия для ActionMenuButton.xaml
    /// </summary>
    public partial class ActionMenuButton : UserControl
    {
        public ActionMenuButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeOne"];
            Cursor = null;
        }     
        
        public void SetBasicParams(string text, PackIconKind kind)
        {
            Icon.Kind = kind;
            ButText.Text = text;
        }

        public void SetColor(SolidColorBrush color)
        {
            Icon.Foreground = color;
            ButText.Foreground = color;
        }
    }
}
