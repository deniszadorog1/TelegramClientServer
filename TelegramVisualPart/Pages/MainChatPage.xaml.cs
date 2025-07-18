using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.UserControls;
using TelegramVisualPart.UserControls.ContactsControls;
using TelegramVisualPart.UserControls.DifferButs;
using static MaterialDesignThemes.Wpf.Theme;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainChatPage.xaml
    /// </summary>
    public partial class MainChatPage : Page
    {
        private TelSystem _system;
        public MainChatPage(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            LeftButtons.OnMenuClick += LeftButtons_OnMenuClick;
            SetDrawButsStyles();

            SetChatClick();

            LeftButtons.SetSystemParam(_system);

            UserChat.SetSystemParam(_system);

            SetSearchMessageParams();

            UpdateUserChatsPanel();
        }

        public void SetSearchMessageParams()
        {
            //Search in chat list of messages
            //SearchMessage.

        }

        public void SetChatClick()
        {
            UserChat.FindMessageBut.PreviewMouseDown += Magnifier_PreviewMouseDown;
        }

        private void Magnifier_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideAllChatBlocks();
            SearchMessageGrid.Visibility = Visibility.Visible;
        }

        public void HideAllChatBlocks()
        {
            ChatsBox.Visibility = Visibility.Hidden;
            SearchBoxGrid.Visibility = Visibility.Hidden;
            SearchMessageGrid.Visibility = Visibility.Hidden;
        }

        public void SetMessageGridMagnifier()
        {
            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;
        }

        private void LeftButtons_OnMenuClick(object sender, EventArgs e)
        {
            DrawerHost.OpenDrawerCommand.Execute(Dock.Left, MainDrawerHost);
        }

        private void SetDrawButsStyles()
        {
            ClearTextBut.IconType.Kind = PackIconKind.Close;

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
            if (sender is MenuIconTextBut icon)
            {
                Page page = GetPageByIcon(icon);

                if (page is MainContacts mainContact) SetMainContactEvents(mainContact);
                else if (page is null) return;

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
            }
        }

        public void SetMainContactEvents(MainContacts mainContacts)
        {
            mainContacts.ChosenContact += ContactsChatChosen_PreviewMouseDown;
        }

        private void ContactsChatChosen_PreviewMouseDown(object sender, EventArgs e)
        {
            if (sender is not UserContact userControl) return;
            SetUserChat(userControl);
            UpdateUserChatsPanel();
        }

        public void SetUserChat(UserContact contact)
        {
            //SET PAGE FILLING
            // Set chatter page
            _system.SetTempChatter(contact.UserLogin.Text);
            //Check isf set
            if (!_system.IsChatterIsSet()) return;

            ChosoeChatBorder.Visibility = Visibility.Hidden;
            UserChat.Visibility = Visibility.Visible;

            UserChat.SetUserChat(_system.GetUserChatByChatterName(
                _system.ChosenChatContact.Name));
        }

        public Page GetPageByIcon(MenuIconTextBut icon)
        {
            return icon.Name == MyProfileDrawBut.Name.ToString() ? new LoggedUserProfile(_system.LoggedUser) :
                icon.Name == ContactsDrawBut.Name.ToString() ? new Contacts.MainContacts(Enums.ContactsPageAction.AddContact, _system.Contacts) :
                icon.Name == SettingsDrawBut.Name.ToString() ? new Settings.SettingsPage(_system) : null;
        }

        private void ChatsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetChatsWidth();
        }

        public void SetChatsWidth()
        {
            for (int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is System.Windows.Controls.ListBoxItem item &&
                    item.Content is UserTalkMessage message)
                {
                    message.Width = ChatsGrid.ActualWidth;
                }

            }
        }

        private void UserChat_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Controls.ListBoxItem item ||
                item.Content is not UserTalkMessage talkControl) return;

            ShowChatControl();

            UserChat.SetUserChat(
                _system.GetUserChatByChatterName(talkControl.FriendLogin.Text));
        }

        public void ShowChatControl()
        {
            ChosoeChatBorder.Visibility = Visibility.Hidden;
            UserChat.Visibility = Visibility.Visible;
        }

        public void SetChat(UserTalkMessage chat)
        {
            //Get chat obj from db(nah, another shit should be here)
            //Set chat params
        }

        private void SavedMessagesDrawBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set Saved Messages chat control
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Point mousePos = Mouse.GetPosition(Application.Current.MainWindow);
            double desired = ChatsColumn.ActualWidth + e.HorizontalChange;

            if (mousePos.X < 300)
            {
                //hide all chat textBlocks except UserImage
                HideAllChatBlocks();

                ChatsColumn.MinWidth = 75;
                ChatsColumn.Width = new GridLength(75);
                SetVisibilityForChatObjects(true);

                ChatsBox.Visibility = Visibility.Visible;
            }
            else
            {
                double clamped = Math.Max(100, desired);
                ChatsColumn.Width = new GridLength(clamped);
                SetVisibilityForChatObjects(false);
                SearchChatBut.Visibility = Visibility.Visible;
            }
            SetSearchPanelWidth();
        }

        public void SetSearchPanelWidth()
        {
            if (SearchBoxGrid.Visibility == Visibility.Hidden) return;

            SearchControl.SetControlSize();
        }

        private void SetVisibilityForChatObjects(bool isShort)
        {
            SetChatInfoVisibility(isShort);
            SetSearchLineVisibility(isShort);
        }

        public void SetSearchLineVisibility(bool isShort)
        {
            Visibility magniVis = isShort ? Visibility.Visible : Visibility.Hidden;

            MagnifierGrid.Visibility = magniVis;
            SearchBorder.Visibility = magniVis == Visibility.Visible ?
                Visibility.Hidden : Visibility.Visible;
        }

        public void SetChatInfoVisibility(bool isShort)
        {
            Visibility vis = isShort ? Visibility.Hidden : Visibility.Visible;

            foreach (var mes in ChatsBox.Items)
            {
                if (mes is not UserTalkMessage) return;
                UserTalkMessage message = mes as UserTalkMessage;
                message.InfoGrid.Visibility = vis;
            }
        }

        private void MagnifierGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            SearchChatBut.Foreground = Brushes.White;
        }

        private void MagnifierGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            SearchChatBut.Foreground = Brushes.Gray;
        }

        private void MagnifierGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ChatsColumn.Width = new GridLength(300);
            SetVisibilityForChatObjects(false);
            //Change border size   
            SetSearchBoxVisible();
        }

        private void SarchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SetSearchBoxVisible();
        }

        public void SetSearchBoxVisible()
        {
            HideAllChatBlocks();
            SearchBoxGrid.Visibility = Visibility.Visible;
            ChatsColumn.MinWidth = 300;
            SearchControl.UpdateColors();
        }

        private void SarchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;
            ChatsColumn.MinWidth = 50;
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            return;
            var focusedEl = Keyboard.FocusedElement;

            if (focusedEl is System.Windows.Controls.TextBox box
                && box.Name == SarchBox.Text)
            {

            }
            bool ifFoc = SarchBox.IsFocused;
        }

        private void ClearTextBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SarchBox.Text = string.Empty;

            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;

            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(SarchBox), null);
            Keyboard.ClearFocus();
        }

        public void UpdateUserChatsPanel()
        {
            ChatsBox.Items.Clear();

            int chatsCount = _system.GetChatsAmount();
            for (int i = 0; i < chatsCount; i++)
            {
                System.Windows.Controls.ListBoxItem item = new
                    System.Windows.Controls.ListBoxItem()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Content = GetTalkMessage(i)
                };
                item.PreviewMouseDown += UserChat_PreviewMouseDown;

                ChatsBox.Items.Add(item);
            }
        }

        public UserTalkMessage GetTalkMessage(int chatIndex)
        {
            UserTalkMessage chatControl = new UserTalkMessage()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = ChatsGrid.ActualWidth
            };

            TelegramLib.MainClasses.UserChat chat =
                _system.GetChatByIndex(chatIndex);

            chatControl.FriendLogin.Text = chat.GetChatter().Name;

            DateTime? date = chat.GetLastMessageDateTime();

            if (date is not null) chatControl.LastMessageTime.Text = $"{((DateTime)date).Day}.{((DateTime)date).Month}.{((DateTime)date).Year}";
            chatControl.LastMessage.Text = chat.GetLastMessage();

            //Set Image icon
            return chatControl;
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                UserChat.Visibility = Visibility.Hidden;
                ChosoeChatBorder.Visibility = Visibility.Visible;
            }
        }
    }
}
