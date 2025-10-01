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
using TelegramLib.MainClasses;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для BlockPage.xaml
    /// </summary>
    public partial class BlockPage : Page
    {
        private TelSystem _system;
        public BlockPage(TelSystem system)
        {
            _system = system;
            InitializeComponent();
        }

        private void LogOutBut_MouseEnter(object sender, MouseEventArgs e)
        {
            LogOutBut.TextDecorations = TextDecorations.Underline;
            Cursor = Cursors.Hand;
        }

        private void LogOutBut_MouseLeave(object sender, MouseEventArgs e)
        {
            LogOutBut.TextDecorations = null;
            Cursor = null;
        }

        private void LogOutBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearBlockFrame();
            ((MainWindow)Window.GetWindow(this)).LogOut();
        }

        private void PasswordBox_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void PasswordBox_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void PasswordBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SubmitBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void SubmitBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void SubmitBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_system.Settings.PrivacySettings.PassCode.PassCode != PasswordBox.Text) return;
            ((MainWindow)Window.GetWindow(this)).ClearBlockFrame();
        }
    }
}
