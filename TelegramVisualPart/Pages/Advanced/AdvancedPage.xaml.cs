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
using TelegramVisualPart.UserControls.SettingsControls.AdvancedControls.AdvancedButtons;

namespace TelegramVisualPart.Pages.Advanced
{
    /// <summary>
    /// Логика взаимодействия для AdvancedPage.xaml
    /// </summary>
    public partial class AdvancedPage : Page
    {
        public AdvancedPage()
        {
            InitializeComponent();

            SetBaseBlocks();
        }

        public void SetBaseBlocks()
        {
            GetBackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            DownloadPathBut.IconType.Kind = PackIconKind.FileOutline;
            DownloadPathBut.ButName.Text = "Download path";
            DownloadPathBut.TempStatusBut.Text = "Default folder";

            Downloads.IconType.Kind = PackIconKind.DownloadOutline;
            Downloads.ButName.Text = "Downloads";

            IsAskDownloadPath.TextBlock.Text = "Ask download path for each file";

            PrivateChatsBut.IconType.Kind = PackIconKind.AccountCircleOutline;
            PrivateChatsBut.ButName.Text = "In private chats";

            ShowChatNameBox.Content = "Show chat name";
            UnreadCountBox.Content = "Total unread count";
            WindowFrame.Content = "Use system window frame";

            VersionBut.FirstTextBlock.Text = "Update automatically";
            VersionBut.SecondTextBlock.Text = "temp version";

            InstalBetaBut.TextBlock.Text = "Install beta version";
            CheckForUpdates.TextBlock.Text = "Check for updates";
        }

        private void GetBackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new Settings.SettingsPage());
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
