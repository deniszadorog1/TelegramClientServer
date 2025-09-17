using ControlzEx.Standard;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses;
using TelegramLib.Services;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Settings.PrivAndSecurity;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для AutoDeleteForUsers.xaml
    /// </summary>
    public partial class ToChooseChats : Page
    {
        private TelSystem _system;
        private List<UserContactcs> _contacts;
        private ChooseType _type;
        private PrivacySub _sub;

        private AutoDeleteType? _newAutoDelType = null;
        public ToChooseChats(TelSystem system, AutoDeleteType type)
        {
            _system = system;
            _contacts = _system.Contacts;
            _newAutoDelType = type;

            InitializeComponent();

            SetActionContacts();

            SetLanguageText.SetToChooseChat(this);
        }

        public void SetActionContacts()
        {
            ChatsPanelToChoose.Children.Clear();
            for (int i = 0; i < _contacts.Count; i++)
            {
                ChatToApply contact = new ChatToApply(_contacts[i]);

                contact.Tag = _contacts[i].GetFirstImageName().Name;
                contact.Name = "contact_" + Guid.NewGuid().ToString("N");

                contact.PreviewMouseDown += AutoDeleteContact_PreviewMouseDown;

                ChatsPanelToChoose.Children.Add(contact);
            }
        }

        private void AutoDeleteContact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ChatToApply chatToApply) return;

            if (chatToApply.GetIdClicked())
            {
                AddAppliedContactToAutoDelete(chatToApply);
                return;
            }
            RemoveAppliedChat(chatToApply);
        }

        public void AddAppliedContactToAutoDelete(ChatToApply appliedChat)
        {
            ChosenChat toAdd = new ChosenChat()
            {
                Tag = appliedChat.Tag.ToString(),
                Name = appliedChat.Name,
                VerticalAlignment = VerticalAlignment.Center
            };

            toAdd.SetBasicParams(appliedChat.Tag.ToString(),
                appliedChat.GetTypeName());

            toAdd._removeChatEvent += ChosenAutoDeleteChat_RemoveClicked;

            //Set params of chosen chat
            ChatsPanel.Children.Add(toAdd);
        }

        private void ChosenAutoDeleteChat_RemoveClicked(object sender, EventArgs e)
        {
            if (sender is not ChosenChat test) return;

            //Clear chat to apply
            ClearChatToApply(test);

            //Remove chosen chat
            ChatsPanel.Children.Remove(test);
        }

        //Set here chosen contacts
        private PrivAndSecSettings _privSettings;
        private Page _prevPage = null;

        public ToChooseChats(ChooseType type, List<UserContactcs> contacts, PrivacySub sub,
            PrivAndSecSettings settings, TelSystem system)
        {
            _contacts = contacts;
            _type = type;
            _sub = sub;
            _privSettings = settings;
            _system = system;

            InitializeComponent();

            SetParams();

            SetContacts();
            SetChosenContacts();
        }

        public void SetExtraPage(Page page)
        {
            _prevPage = page;
        }

        private void SetChosenContacts()
        {
            List<UserContactcs> contacts = _type == ChooseType.AlwaysShare ? _sub.ShareWithExps : _sub.NeverShareExps;

            for (int i = 0; i < contacts.Count; i++)
            {
                ChatToApply? chosen = ChatsPanelToChoose.Children.OfType<ChatToApply>().Where
                    (x => x.TypeName.Text == contacts[i].Name).FirstOrDefault();
                if (chosen is null) continue;

                chosen.UserControl_PreviewMouseDown(chosen, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = UIElement.PreviewMouseDownEvent,
                    Source = chosen
                });

                //Set chosen action 
                Contact_PreviewMouseDown(chosen, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = UIElement.PreviewMouseDownEvent,
                    Source = chosen
                });
            }
        }

        public void SetContacts()
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                ChatToApply contact = new ChatToApply(_contacts[i]);

                contact.Tag = _contacts[i].GetFirstImageName().Name;
                contact.Name = "contact_" + Guid.NewGuid().ToString("N");
                // (i + 1).ToString();

                DateTime? lastSeen = _contacts[i].LastSeen;

                string lastSeenStr = lastSeen is null ? VisConstParamsJsonService.GetStringByName("RecentlyStat") :
                    $"{((DateTime)lastSeen).Day}.{((DateTime)lastSeen).Month}.{((DateTime)lastSeen).Year}";

                contact.SetParams(_contacts[i].GetFirstImageName().Name, _contacts[i].Name, lastSeenStr);
                contact.PreviewMouseDown += Contact_PreviewMouseDown;

                ChatsPanelToChoose.Children.Add(contact);
            }
        }

        private void Contact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ChatToApply temp) return;

            if (temp.GetIdClicked())
            {
                AddAppliedChat(temp);
                return;
            }
            RemoveAppliedChat(temp);
        }

        public void SetParams()
        {
            PageName.Text = _type == ChooseType.AlwaysShare ?
                VisConstParamsJsonService.GetStringByName("AlwayShareWith") :
                VisConstParamsJsonService.GetStringByName("NeverShareWith");
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is System.Windows.Controls.Button but) but.Background =
                (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is System.Windows.Controls.Button but) 
                but.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            if(_prevPage is not null)
            {
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(_prevPage);
                return;
            }
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private async void ApplyBut_Click(object sender, RoutedEventArgs e)
        {
            //Set signalR Action

            //Save chosen contacts
            if (_newAutoDelType is null)
            {
                await SaveChosenContacts();
                await CallSignalRMethods();

                //return;
            }
            else ApplyAutoDeletion();


            if (_prevPage is not null)
            {
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(_prevPage);
                return;
            }

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private async Task CallSignalRMethods()
        {
            await SignalRService.SetPhoneNumVisByExps(_system.LoggedUser);
            await SignalRService.SetContactLastSeenVisState(_system.LoggedUser);
            
            await SignalRService.UpdateBirtDate(_system.LoggedUser);
            await SignalRService.UpdateContactPhotoVis(_system.LoggedUser);
        }

        private void ApplyAutoDeletion()
        {
            //go through all chosen contacts
            //Compare tags 
            //Assign new Deletion to contacts

            for (int i = 0; i < ChatsPanel.Children.Count; i++)
            {
                if (ChatsPanel.Children[i] is not ChosenChat chat) continue;

                UserContactcs? contact = _system.Contacts.Where(
                    x => x.Name == chat.UserName.Text).FirstOrDefault();
                if (contact is null) continue;

                contact.SetAutoDeleteDuration(_newAutoDelType);
            }
        }

        private async Task SaveChosenContacts()
        {
            switch (_type)
            {
                case ChooseType.AlwaysShare:
                    {
                        _sub.ShareWithExps.AddRange(GetContactsFromChosenChats(_sub.ShareWithExps));
                        RemoveChosenContacts();
                        break;
                    }
                case ChooseType.NeverShare:
                    {
                        _sub.NeverShareExps.AddRange(GetContactsFromChosenChats(_sub.NeverShareExps));
                        RemoveChosenContacts();
                        break;
                    }
            }

            //DbService.UpdatePrivacySettings(_privSettings);

            await ApiService.UpdatePrivSettings(_privSettings);
        }

        public void RemoveChosenContacts()
        {
            List<UserContactcs> removeFrom = _type == ChooseType.AlwaysShare ? _sub.NeverShareExps : _sub.ShareWithExps;
            List<UserContactcs> toRemove = _type == ChooseType.AlwaysShare ? _sub.ShareWithExps : _sub.NeverShareExps;

            List<UserContactcs> remove = removeFrom.Where(x => toRemove.Select(y => y.Name).Contains(x.Name)).ToList();

            foreach (var contact in remove)
            {
                removeFrom.Remove(contact);
            }
        }

        private List<UserContactcs> GetContactsFromChosenChats(List<UserContactcs> chosenContacts)
        {
            List<string> names = ChatsPanel.Children.OfType<ChosenChat>().Select(x => x.UserName.Text).ToList();

            return _contacts.Where(x => names.Contains(x.Name) &&
            !chosenContacts.Select(x => x.Name).Contains(x.Name)).ToList();
        }

        public void AddAppliedChat(ChatToApply chatControl)
        {
            ChosenChat toAdd = new ChosenChat()
            {
                //Set here name as chatUser login (to compare with it)
                Tag = chatControl.Tag.ToString(),
                Name = chatControl.Name,
                VerticalAlignment = VerticalAlignment.Center
            };

            toAdd.SetBasicParams(chatControl.Tag.ToString(),
                chatControl.GetTypeName());

            toAdd._removeChatEvent += ChosenChat_RemoveClicked;

            //Set params of chosen chat
            ChatsPanel.Children.Add(toAdd);
        }

        public void RemoveAppliedChat(ChatToApply toRemove)
        {
            if (_newAutoDelType is null) RemoveContact(toRemove.TypeName.Text);

            ChatsPanel.Children.Remove(ChatsPanel.Children.OfType<ChosenChat>().
                Where(x => x.Name == toRemove.Name).First());
        }

        private void TestParam_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ChatToApply) return;

            ChatToApply temp = sender as ChatToApply;

            if (temp.GetIdClicked())
            {
                AddAppliedChat(temp);
                return;
            }
            RemoveAppliedChat(temp);
        }

        private void ChosenChat_RemoveClicked(object sender, EventArgs e)
        {
            if (sender is not ChosenChat) return;
            ChosenChat test = sender as ChosenChat;

            //Clear chat to apply
            ClearChatToApply(test);

            //Remove chosen chat
            ChatsPanel.Children.Remove(test);
        }

        private void ClearChatToApply(ChosenChat test)
        {
            ChatToApply? toClear = ChatsPanelToChoose.Children.OfType<ChatToApply>().
                Where(x => x.Name == test.Name).FirstOrDefault();

            if (toClear is null) return;

            if (_newAutoDelType is null) RemoveContact(toClear.TypeName.Text);

            toClear.DiscardChat();
        }

        public void RemoveContact(string name)
        {
            List<UserContactcs> contacts = _type == ChooseType.AlwaysShare ?
                _sub.ShareWithExps : _sub.NeverShareExps;
            if (contacts.Count == 0) return;

            UserContactcs? contact = contacts.FirstOrDefault(x => x.Name == name);
            if (contact is null) return;
            contacts.Remove(contact);
        }

    }
}
