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

namespace TelegramVisualPart.UserControls.ChatsSearch.SearchMediaMenuFolder
{
    /// <summary>
    /// Логика взаимодействия для SearchMediaMenuItem.xaml
    /// </summary>
    public partial class SearchMediaMenuItem : UserControl
    {
        public SearchMediaMenuItem()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = new SolidColorBrush(Colors.Transparent);
        }

        public void SetMainGridBg(SolidColorBrush color)
        {
            MainGrid.Background = color;
        }

        public void SetParams(PackIconKind kind, string text)
        {
            Icon.Kind = kind;
            ItemTextBlock.Text = text;
        }
    }
}
