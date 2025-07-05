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
    /// Логика взаимодействия для EnterPage.xaml
    /// </summary>
    public partial class EnterPage : Page
    {
        public EnterPage()
        {
            InitializeComponent();
        }

        private void RegistrationGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = null;
        }

        private void RegistrationGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = TextDecorations.Underline;
        }

        private void RegistrationGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set registration page
            ((MainWindow)Window.GetWindow(this)).SetMainFrameContent(new RegistrationPage());
        }

        private void EnterBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetMainFrameContent(new MainChatPage());
        }
    }
}
