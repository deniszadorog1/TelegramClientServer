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

namespace TelegramVisualPart.Pages.Settings.Folders
{
    /// <summary>
    /// Логика взаимодействия для FolderAction.xaml
    /// </summary>
    public partial class FolderAction : Page
    {
        public FolderAction()
        {
            InitializeComponent();

            SetBlocks();
        }

        private void SetBlocks()
        {
            CreateNewFolderBut.NewFolderText.Text = "Add chat";
            ChatToExcludeBut.NewFolderText.Text = "Add Chats to Exclude";

            CreateInviteLinkBut.NewFolderText.Text = "Create an Invite Link";
            CreateInviteLinkBut.IconType.Kind = PackIconKind.LinkVariant;

            ChatToExcludeBut.IconType.Kind = PackIconKind.Minus; 
        }

        private void ToChooseFolderIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            FolderIcon.Visibility = Visibility.Visible;
        }

        private void ToChooseFolderIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            //FolderIcon.Visibility = Visibility.Hidden;
        }

        private void CreateBut_Click(object sender, RoutedEventArgs e)
        {
            //To add new Folder
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void FolderIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            FolderIcon.Visibility = Visibility.Hidden;
        }

        private void CreateNewFolderBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new FoldersChatAction(Enums.FolderChatActionType.AddChatInFolder));
        }

        private void ChatToExcludeBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new FoldersChatAction(Enums.FolderChatActionType.ExcludeChat));
        }
    }
}
