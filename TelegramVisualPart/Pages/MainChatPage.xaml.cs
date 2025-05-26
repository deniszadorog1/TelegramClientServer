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

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainChatPage.xaml
    /// </summary>
    public partial class MainChatPage : Page
    {
        public MainChatPage()
        {
            InitializeComponent();

            LeftButtons.OnMenuClick += LeftButtons_OnMenuClick;
            SetDrawButsStyles();
        }

        private void LeftButtons_OnMenuClick(object sender, EventArgs e)
        {
            DrawerHost.OpenDrawerCommand.Execute(Dock.Left, MainDrawerHost);
        }

        private void SetDrawButsStyles()
        {
            MyProfileDrawBut.IconType.Kind = PackIconKind.AccountCircleOutline;
            MyProfileDrawBut.ButName.Text = "My Profile";

            WalletDrawBut.IconType.Kind = PackIconKind.AccountBalanceWalletOutline;
            WalletDrawBut.ButName.Text = "Wallet";

            NewGroupDrawBut.IconType.Kind = PackIconKind.Users;
            NewGroupDrawBut.ButName.Text = "New Group";

            NewChannelDrawBut.IconType.Kind = PackIconKind.Megaphone;
            NewChannelDrawBut.ButName.Text = "New Channel";

            ContactsDrawBut.IconType.Kind = PackIconKind.Account;
            ContactsDrawBut.ButName.Text = "Contacts";

            CallsDrawBut.IconType.Kind = PackIconKind.TelephoneInTalk;
            CallsDrawBut.ButName.Text = "Calls";

            SavedMessagesDrawBut.IconType.Kind = PackIconKind.ContentSaveOutline;
            SavedMessagesDrawBut.ButName.Text = "Saved Messages";

            SettingsDrawBut.IconType.Kind = PackIconKind.SettingsOutline;
            SettingsDrawBut.ButName.Text = "Settings"; 
            


        }
    }
}
