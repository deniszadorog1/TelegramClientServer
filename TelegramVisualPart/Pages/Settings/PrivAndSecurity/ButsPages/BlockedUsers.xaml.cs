using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TelegramLib.MainClasses;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls;
using User = TelegramLib.MainClasses.User;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages
{
    /// <summary>
    /// Логика взаимодействия для BlockedUsers.xaml
    /// </summary>
    public partial class BlockedUsers : Page
    {
        private TelSystem _system;
        public BlockedUsers(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetButsVisualState();

            SetBlockedContacts();
        }

        public async void SetBlockedContacts()
        {
            BlockedUsersPanel.Items.Clear();

            List<User> blocked = _system.LoggedUser.BlockedUsers;
            for (int i = 0; i < blocked.Count; i++)
            {
                ToUnblockUser blockedControl = new ToUnblockUser();

                await blockedControl.SetUserImage(blocked[i].GetFirstImageName().Name);

                blockedControl.ChaterLogin.Text = blocked[i].Name;
                blockedControl.UserName.Text = blocked[i].Login;


                ListBoxItem item = new ListBoxItem()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Content = blockedControl.Content
                };

                blockedControl.UnblockBut.PreviewMouseDown += async (sender, e) =>
                {
                    User contact =
                    _system.LoggedUser.BlockedUsers.Where(
                        x => x.Name == blockedControl.ChaterLogin.Text).First();

                    _system.LoggedUser.BlockedUsers.Remove(contact);

                    if (contact is not null) await ApiService.RemoveBlockedContact(_system.LoggedUser.Id, contact.Id);

                    BlockedUsersPanel.Items.Remove(item);

                    AmountNum.Text = _system.LoggedUser.BlockedUsers.Count.ToString();

                    //Update if its temp chat 
                    ((MainWindow)Window.GetWindow(this)).UpdateChatParamsVis();
                    await ((MainWindow)Window.GetWindow(this)).SetBlockedUserVisParams(false, contact);

                };

                BlockedUsersPanel.Items.Add(item);
            }

            AmountNum.Text = blocked.Count.ToString();
        }

        public void SetButsVisualState()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            ToBlockBut.IconType.Kind = PackIconKind.Hand;

            ToBlockBut.ButName.Text = "Block user";

            UpdateAmountOfBlocked();
        }

        public void UpdateAmountOfBlocked()
        {
            AmountNum.Text = BlockedUsersPanel.Items.Count.ToString();
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new PrivacyAndSecurity(_system));
        }

        private void ToBlockBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContacts toBlock = new MainContacts
                (Enums.ContactsPageAction.Block, _system, true);

            toBlock.ContactsBlock.Text = "Select user to block";
            toBlock.SortBut.Visibility = Visibility.Hidden;
            toBlock.AddContactBut.Visibility = Visibility.Hidden;

            toBlock.ContactClicked += UserBlock_Event;

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(toBlock);
        }

        public void UserBlock_Event(object sender, EventArgs e)
        {
            SetBlockedContacts();
        }
    }
}
