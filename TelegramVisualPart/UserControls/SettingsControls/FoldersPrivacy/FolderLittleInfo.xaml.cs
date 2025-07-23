using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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
    /// Логика взаимодействия для FolderLittleInfo.xaml
    /// </summary>
    public partial class FolderLittleInfo : UserControl
    {
        public FolderLittleInfo()
        {
            InitializeComponent();
        }

        public void SetIcon(PackIconKind icon)
        {
            IconType.Kind = icon;
        }

        public void SetFolderName(string name)
        {
            FolderName.Text = name;
        }

        public void SetAmountOfItems(int amount)
        {
            AmountOfChats.Text = $"{amount} chats";
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            this.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            BucketIcon.Foreground = new SolidColorBrush(Colors.LightGray);
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            BucketIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }
    }
}
