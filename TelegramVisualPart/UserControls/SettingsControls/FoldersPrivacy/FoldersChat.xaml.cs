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

namespace TelegramVisualPart.UserControls.SettingsControls.FoldersPrivacy
{
    /// <summary>
    /// Логика взаимодействия для FoldersChat.xaml
    /// </summary>
    public partial class FoldersChat : UserControl
    {
        public event EventHandler RemoveControl;

        public FoldersChat()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Icon.Foreground = new SolidColorBrush(Colors.LightGray);
            Cursor = Cursors.Hand;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Icon.Foreground = new SolidColorBrush(Colors.Gray);
            Cursor = null;
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RemoveControl?.Invoke(this, EventArgs.Empty);
        }

        public string GetFolderChatName()
        {
            return NewFoldersChatText.Text;
        }
    }
}
