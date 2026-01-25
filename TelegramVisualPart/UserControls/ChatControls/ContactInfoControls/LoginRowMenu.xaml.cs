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
using static System.Net.Mime.MediaTypeNames;

namespace TelegramVisualPart.UserControls.ChatControls.ContactInfoControls
{
    /// <summary>
    /// Логика взаимодействия для LoginRowMenu.xaml
    /// </summary>
    public partial class LoginRowMenu : UserControl
    {
        private string _text;
        public event Action TextCopied;

        public LoginRowMenu(string text)
        {
            _text = text;
            InitializeComponent();
        }

        private void CopyTextGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            CopyTextGrid.Background = 
                (SolidColorBrush)System.Windows.Application.Current.
                Resources["DarkThemeOne"];

        }

        private void CopyTextGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            CopyTextGrid.Background = 
                new SolidColorBrush(Colors.Transparent);
        }

        private void CopyTextGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(_text);
            Window window = Window.GetWindow(this);
            if (window is MainWindow main)
            {
                main.SetTemporaryText("Username copied to clipboard");
            }
            TextCopied?.Invoke();
        }
    }
}
