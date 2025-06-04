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

namespace TelegramVisualPart.UserControls.SettingsControls
{
    /// <summary>
    /// Логика взаимодействия для ToUnblockUser.xaml
    /// </summary>
    public partial class ToUnblockUser : UserControl
    {
        public ToUnblockUser()
        {
            InitializeComponent();
        }

        private void UnblockBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void UnblockBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            UnblockBut.TextDecorations = TextDecorations.Underline;
        }

        private void UnblockBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            UnblockBut.TextDecorations = null;
        }
    }
}
