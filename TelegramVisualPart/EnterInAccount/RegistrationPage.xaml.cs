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
using TelegramVisualPart.Pages;

namespace TelegramVisualPart.EnterInAccount
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void GetBackGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = TextDecorations.Underline;
        }

        private void GetBackGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = null;
        }

        private void GetBackGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetMainFrameContent(new EnterPage());
        }

        private void RegisterBut_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
