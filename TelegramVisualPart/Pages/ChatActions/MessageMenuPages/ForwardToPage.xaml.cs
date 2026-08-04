using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TelegramLib.MainClasses;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace TelegramVisualPart.Pages.ChatActions.MessageMenuPages
{
    /// <summary>
    /// Логика взаимодействия для ForwardToPage.xaml
    /// </summary>
    public partial class ForwardToPage : Page
    {
        private TelSystem _system;
        private TelegramLib.MainClasses.Messages.Message _mes;

        public event Action<int?> ForwardSelected;
        public event Action CancelDel;

        public ForwardToPage(TelSystem system,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            _system = system;
            _mes = mes;

            InitializeComponent();

            Loaded += async (s, e) => await SetBasicParams();
            //SetBasicParams();
        }

        public ForwardToPage(TelSystem system)
        {
            _system = system;

            InitializeComponent();
            
            Loaded += async (s, e) => await SetBasicParams();
            //SetBasicParams();
        }

        public async Task SetBasicParams()
        {
            ChatsPanel.Children.Clear();

            for (int i = 0; i < _system.Chats.Count; i++)
            {
                await SetChatListBoxItem(_system.Chats[i]);
            }
            await SetChatListBoxItem(_system.GetSavedChatMessages());

            if (ChatsPanel.Children.Count == 0) Visibility = Visibility.Visible;
        }

        private List<ListBoxItem> _items = new List<ListBoxItem>();
        public async Task SetChatListBoxItem(TelegramLib.MainClasses.UserChat chat)
        {
            ListBoxItem item = new ListBoxItem()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Tag = chat is TelegramLib.MainClasses.SavedMessagesChat ? null : chat.Chatter.Id,
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Top    
            };
            item.PreviewMouseDown += Contacts_PreviewMouseDown;

            await SetContactToApply(chat is TelegramLib.MainClasses.SavedMessagesChat ?
                _system.LoggedUser : chat.Chatter, item);
        }

        public async Task SetContactToApply(TelegramLib.MainClasses.User user,
            ListBoxItem item)
        {
            ChatToApply contact = new ChatToApply(user);
            //await contact.SetActions();

            contact.HorizontalAlignment = HorizontalAlignment.Stretch;

            if (item.Tag is not null)
            {
                await SignalRHelperService.SetPhotoInEllipse(user,
                 contact.UserImageBrush, contact.UserImageEllipse);
               
                //contact.AddedUserImage(user);
             
                contact.TypeName.Text = user.Login;
                contact.AutoDeletionType.Text = user.GetLastSeenInChat();

                contact.Tag = user.GetFirstImageName().Name;
            }
            else
            {
                //Saved chat messages
                contact.SetSavedMesChatGrid();
            }
            item.Content = contact;
            ChatPanel.Items.Add(item);

            _items.Add(item);
        }

        public void Contacts_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item) return;

            //null = saved chat
            int? userSendId = null;
            if (int.TryParse(item.Tag?.ToString(), out int temp))
            {
                userSendId = temp;
            }

            if (_mes is null)
            {
                ForwardSelected?.Invoke(userSendId);
                ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                return;
            }

            ((MainWindow)Window.GetWindow(this)).SetForwardMessage(_mes, userSendId);
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);

        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
            CancelDel?.Invoke();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChatPanel.Items.Clear();

            foreach(var item in _items)
            {
                if(item.Content is ChatToApply val && 
                    (val.IsSameName(SearchName.Text) || string.IsNullOrWhiteSpace(SearchName.Text)))
                {
                    ChatPanel.Items.Add(item);
                }

            }
        }

    }
}
