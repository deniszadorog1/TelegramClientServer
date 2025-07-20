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

namespace TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion
{
    /// <summary>
    /// Логика взаимодействия для SetCustomTime.xaml
    /// </summary>
    public partial class SetCustomTime : Page
    {
        public event EventHandler ChosenAutoDelete;

        public SetCustomTime()
        {
            InitializeComponent();
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            //Set Chosen AutoDelet param;
            ChosenAutoDelete?.Invoke(this, EventArgs.Empty);

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }


    }
}
