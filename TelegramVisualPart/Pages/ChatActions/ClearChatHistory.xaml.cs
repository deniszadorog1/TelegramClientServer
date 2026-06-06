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
            if (_chat.Chatter is null)
            {
                UsernameBlock.Text = "?"; //"Clear saved messes";
                BothPanel.Visibility = Visibility.Hidden;
                EnAutoDelete.Visibility = Visibility.Hidden;

                BothPanelRow.Height = new GridLength(0);
                EnAutoDeleteRow.Height = new GridLength(0);

                Height -= 100;
                return;
            }
            TelegramLib.MainClasses.User user =
                await ApiService.GetUserById(_chat.Chatter.Id);
            UsernameBlock.Text = user.Login;
            UsernameCheckBoxBlock.Text = user.Login;
        }

        private async void DeleteBut_Click(object sender, RoutedEventArgs e)
        {
            await ClearChat();
        }

        private async Task ClearChat()
        {
            //Saved messages
            Window window = Window.GetWindow(this);
            if(_chat is TelegramLib.MainClasses.SavedMessagesChat saved)
            {
                _system.RemoveAllMessagesFromChat(_chat);

                //clear from system value 
                _chat.ClearChat();

                //clear chat in vis
                if(window is MainWindow mainWindow)
                {
                    mainWindow.CloseAllMediaWindows();
                    mainWindow.ClearVisChat();

                    await mainWindow.UpdateGlobalMedias();
                }

                //clear from db
                await ApiService.ClearSaveChatById(saved.Id);

                ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
                return;
            }

            if (window is MainWindow main)
            {
                main.CloseAllMediaWindows();

               await main.UpdateGlobalMedias();
            }
            //Is to clear both users
            bool isClearBoth = (bool)ShowChatNameBox.IsChecked;
            if (isClearBoth)
            {
                //Clear for chatter
                await ClearForChatter();
            }
            //clear for temp user
            await ClearTempUserChat(_chat);
        }

        public async Task ClearForChatter()
        {
            bool isChatterOnline = await ApiService.IsUserOnline(_chat.Chatter.Id);
            TelegramLib.MainClasses.User chatter = await ApiService.GetUserById(_chat.Chatter.Id);

            if (isChatterOnline)
            {
               await SignalRService.ClearChat(chatter.Id, _system.LoggedUser);
            }
            else
            {
                UserContactcs userContact = await ApiService.GetContactByUserAndFriendIds(chatter.Id, _system.LoggedUser.Id);
                UserChat? chat = await ApiService.GetChatByUserAndSenderId(chatter.Id, _system.LoggedUser.Id);
                await ApiService.ClearChat(chat);
            }
        }

        private async Task ClearTempUserChat(UserChat chat)
        {
            await ApiService.ClearChat(_chat);

            _system.RemoveAllMessagesFromChat(_chat);
            _chat.ClearChat();

            ((MainWindow)Window.GetWindow(this)).ClearChat(chat);
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }


        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
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

        private void BothPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowChatNameBox.IsChecked = !ShowChatNameBox.IsChecked;
        }

        private void BothPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void BothPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
