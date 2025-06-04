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
using TelegramVisualPart.UserControls;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainChatPage.xaml
    /// </summary>
    public partial class MainChatPage : Page
    {
        private Frame _frame;
        public MainChatPage(Frame frame)
        {
            _frame = frame;
            InitializeComponent();

            LeftButtons.OnMenuClick += LeftButtons_OnMenuClick;
            SetDrawButsStyles();

            SetChatClickEvent();
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

        private void LeftBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is IconTextBut icon)
            {
                Page page = GetPageByIcon(icon);
                if (page is null) return;

                ((MainWindow)Window.GetWindow(_frame)).SetSecondaryFrame(page);
            }
        }

        public Page GetPageByIcon(IconTextBut icon)
        {
            return icon.Name == MyProfileDrawBut.Name.ToString() ? new LoggedUserProfile(_frame) : 
                icon.Name == ContactsDrawBut.Name.ToString() ? new Contacts.MainContacts(_frame, Enums.ContactsPageAction.AddContact)  : 
                icon.Name == SettingsDrawBut.Name.ToString() ? new Settings.SettingsPage(_frame) : null;
        }

        private void ChatsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void SetChatClickEvent()
        {
            for(int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is UserTalkMessage chat)
                {
                    SetChat(chat);
                }
            }
        }

        public void SetChat(UserTalkMessage chat)
        {
            //Get chat obj from db
            //Set chat params
        }

        private void SavedMessagesDrawBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set Saved Messages chat control


                

        }

        

        
    }
}
