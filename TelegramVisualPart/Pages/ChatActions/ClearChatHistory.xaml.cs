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
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для ClearChatHistory.xaml
    /// </summary>
    public partial class ClearChatHistory : Page
    {
        private UserChat _chat;
        private TelSystem _system;
        public ClearChatHistory(UserChat chat, TelSystem system)
        {
            _chat = chat;
            _system = system;

            InitializeComponent();

            SetBasicParams();

            SetLanguageText.ClearChatHistory(this);
        }

        public async Task SetBasicParams()
        {
            TelegramLib.MainClasses.User user =
                await ApiService.GetUserById(_chat.Chatter.ContactUserId);
            UsernameBlock.Text = user.Login;
            UsernameCheckBoxBlock.Text = user.Login;
        }

        private async void DeleteBut_Click(object sender, RoutedEventArgs e)
        {
            await ClearChat();
        }

        private async Task ClearChat()
        {
            //Is to clear both users
            bool isClearBoth = (bool)ShowChatNameBox.IsChecked;
            if (isClearBoth)
            {
                //Clear for chatter
                await ClearForChatter();
            }
            //clear for temp user
            await ClearTempUserChat();
        }

        public async Task ClearForChatter()
        {
            bool isChatterOnline = await ApiService.IsUserOnline(_chat.Chatter.ContactUserId);
            TelegramLib.MainClasses.User chatter = await ApiService.GetUserById(_chat.Chatter.ContactUserId);

            if (isChatterOnline)
            {
                SignalRService.ClearChat(chatter.Id, _system.LoggedUser);
            }
            else
            {
                UserContactcs userContact = await ApiService.GetContactByUserAndFriendIds(chatter.Id, _system.LoggedUser.Id);
                UserChat chat = await ApiService.GetChatByUserAndSenderId(chatter.Id, userContact.Id);
                await ApiService.ClearChat(chat);
            }
        }

        private async Task ClearTempUserChat()
        {
            await ApiService.ClearChat(_chat);
            _chat.ClearChat();

            ((MainWindow)Window.GetWindow(this)).ClearChat();
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void DeleteBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["CloseButBg"];
        }

        private void DeleteBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void EnAutoDelete_MouseEnter(object sender, MouseEventArgs e)
        {
            EnAutoDelete.TextDecorations = TextDecorations.Underline;
        }

        private void EnAutoDelete_MouseLeave(object sender, MouseEventArgs e)
        {
            EnAutoDelete.TextDecorations = null;
        }

        private void EnAutoDelete_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Show auto delete message 

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new NewMessagesDeletion(_chat, _system));
        }

    }
}
