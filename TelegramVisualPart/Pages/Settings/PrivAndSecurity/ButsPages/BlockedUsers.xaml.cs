using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
using TelegramLib.Models;
using TelegramVisualPart.Pages.Contacts;
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

        public void SetBlockedContacts()
        {
            BlockedUsersPanel.Items.Clear();

            List<User> blocked = _system.LoggedUser.BlockedUsers;
            for (int i = 0; i < blocked.Count; i++)
            {   
                ToUnblockUser blockedControl = new ToUnblockUser();

                blockedControl.SetUserImage(blocked[i].GetFirstImageName().Name);

                blockedControl.ChaterLogin.Text = blocked[i].Name;
                blockedControl.UserName.Text = blocked[i].Login;


                ListBoxItem item = new ListBoxItem()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Content = blockedControl.Content
                };

                blockedControl.UnblockBut.PreviewMouseDown += (sender, e) =>
                {
                    User contact =
                    _system.LoggedUser.BlockedUsers.Where(
                        x => x.Name == blockedControl.ChaterLogin.Text).First();

                    _system.LoggedUser.BlockedUsers.Remove(contact);

                    BlockedUsersPanel.Items.Remove(item);

                    AmountNum.Text = _system.LoggedUser.BlockedUsers.Count.ToString();

                    //Update if its temp chat 
                    ((MainWindow)Window.GetWindow(this)).UpdateChatParamsVis();
                    ((MainWindow)Window.GetWindow(this)).SetBlockedUserVisParams(false, contact);

                };

                BlockedUsersPanel.Items.Add(item);
            }

            AmountNum.Text = blocked.Count.ToString();
        }

        public void SetButsVisualState()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            //ToBlockBut.IconType.Foreground = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButForeGround"];
            ToBlockBut.IconType.Kind = PackIconKind.Hand;

            //ToBlockBut.ButName.Foreground = SolidColorBrush)Application.Current.Resources["DarkThemeProfileButForeGround"];
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
