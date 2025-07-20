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

namespace TelegramVisualPart.UserControls.ChatsSearch
{
    /// <summary>
    /// Логика взаимодействия для SearchMessage.xaml
    /// </summary>
    public partial class SearchMessage : UserControl
    {
        public SearchMessage()
        {
            InitializeComponent();

            CloseBut.IconType.Kind = PackIconKind.Close; 
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.Parent is Grid) ((Grid)this.Parent).Visibility = Visibility.Hidden;
            ((MainWindow)Window.GetWindow(this)).SetChatsMessages();
        }
    }
}
