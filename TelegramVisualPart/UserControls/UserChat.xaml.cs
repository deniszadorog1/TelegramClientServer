using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;
using Path = System.IO.Path;

using System.Data.Common;
using TelegramLib.Models;
using System.Net.Sockets;
using static System.Data.Entity.Infrastructure.Design.Executor;
using TelegramLib.UserSettings;
using System.Windows.Documents;
using TelegramVisualPart.Enums;
using System.Drawing;
using Brushes = System.Windows.Media.Brushes;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using TelegramLib.UserSettings.SettingsTypes;
using System;
using TelegramVisualPart.UserControls.ChatControls.ChatMessages;
using System.Collections.Frozen;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using Microsoft.AspNetCore.Http.Metadata;
using TelegramVisualPart.Enums.Menus;
using TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu;
using MahApps.Metro.Controls;
using TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages;
using static System.Collections.Specialized.BitVector32;
using System.IO;


namespace TelegramVisualPart.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserChat.xaml
    /// </summary>
    public partial class UserChat : UserControl
    {
        public List<TelegramLib.MainClasses.Messages.Message> _chatMessages =
            new List<TelegramLib.MainClasses.Messages.Message>();
        public UserChat()
        {
            InitializeComponent();
            SetMarginForChatMenu();
            SetAutoDeleteTimer();

            SetBasicSignalRMethods();
        }

        public void SetBasicSignalRMethods()
        {
            SignalRService.TextMessageReceived -= OnTextMessageReceived;
            SignalRService.TextMessageReceived += OnTextMessageReceived;

            SignalRService.MediaMessageReceived -= OnMediaMessageReceived;
            SignalRService.MediaMessageReceived += OnMediaMessageReceived;

            SignalRService.UpdateOnlineStatusDel -= UpdateOnlineStatus;
            SignalRService.UpdateOnlineStatusDel += UpdateOnlineStatus;

            SignalRService.UpdateUserImage -= UpdateUserImage;
            SignalRService.UpdateUserImage += UpdateUserImage;

            SignalRService.ClearChatDel -= ClearChatAction;
            SignalRService.ClearChatDel += ClearChatAction;

            SignalRService.SetContactLastSeenVisStateDel -= SetLastVisState;
            SignalRService.SetContactLastSeenVisStateDel += SetLastVisState;

            SignalRService.UpdateContactPhotoDel -= UpdateChatterIamge;
            SignalRService.UpdateContactPhotoDel += UpdateChatterIamge;

            SignalRService.DeleteMessageByIdDel -= RemoveMessageById;
            SignalRService.DeleteMessageByIdDel += RemoveMessageById;
        }

        public void ReplyMessage(TelegramLib.MainClasses.User chatter, 
            TelegramLib.MainClasses.Messages.Message mes)
        {
            
        }

        public void RemoveMessageById(TelegramLib.MainClasses.User chatter,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            //Get Pair message from mes to delete
            TelegramLib.MainClasses.Messages.Message pair = ApiService.GetPairOfMessage(mes).Result;
            if (pair is null) return;

            //Remove from db
            ApiService.DeleteMessageById(pair.Id);

            //remove from system
            _system.RemoveMessageById(pair.Id);

            //remove from vis
            ListBoxItem? item = ChatBox.Items
                .OfType<ListBoxItem>().FirstOrDefault(x => x.Tag.ToString() == pair.Id.ToString());

            if (item is null) return;
            ChatBox.Items.Remove(item);
        }

        public void SetChatterImageVisibility()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindow window = (MainWindow)Window.GetWindow(this);

                if (window.GetIsOnlyChat()) return;
                UpperChatterImage.Width = new GridLength(0);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void UpdateChatterIamge(TelegramLib.MainClasses.User user)
        {
            UpdateChatImages(user);
        }

        public void SetLastVisState(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_chat is null || _chat.GetChatter().Id != user.Id) return;

                IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);

                await SignalRHelperService.SetLastSeenString(user, shareType, _chat, ChatFriendLastSeen);
            });
        }

        /*        private async Task SetLastSeenString(TelegramLib.MainClasses.User user,
                    IsPrivacyException type)
                {

                    if (_chat is null || _chat.GetChatter().ContactUserId != user.Id) return;

                    MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

                    if (type == IsPrivacyException.Share)
                    {
                        HelperService.SetOnlineStatusInTextBox(
                            ChatFriendLastSeen, user.IsOnline, user.LastSeenOnline);
                        return;
                    }

                    if (settings.PrivacySettings.LastSeenPrivacy.ShareType ==
                        TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                        type == IsPrivacyException.NeverShare)
                    {
                        ChatFriendLastSeen.Foreground = new SolidColorBrush(Colors.Gray);
                        ChatFriendLastSeen.Text = "You cant see this LOOOOLL";
                        return;
                    }
                    HelperService.SetOnlineStatusInTextBox(
                        ChatFriendLastSeen, user.IsOnline, user.LastSeenOnline);
                }*/

        public void ClearChatAction(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                TelegramLib.MainClasses.UserChat? chat = _system.Chats.FirstOrDefault(x => x.Chatter.Id == user.Id);
                if (chat is null) return;

                //Is temp is Chosen -> clear vis
                if (_chat.Id == chat.Id)
                {
                    ChatBox.Items.Clear();
                }
                //Clear from system
                chat.Messages.Clear();
                //Clear from Db
                await ApiService.ClearChat(chat);
            });
        }

        public void UpdateUserImage(TelegramLib.MainClasses.User user)
        {
            //Chat
            UpdateChatImages(user);
        }

        public void UpdateChatImages(TelegramLib.MainClasses.User user)
        {
            Dispatcher.Invoke(async () =>
            {
                TelegramLib.MainClasses.UserChat chat = _system.GetChosenChat();

                if (chat is null || chat.Chatter.Id != user.Id) return;

                await SignalRHelperService.SetFastParamsForPhotoUpdate(user);

                for (int i = 0; i < ChatBox.Items.Count; i++)
                {
                    if (chat.Messages[i].SenderUserId != user.Id || 
                    ChatBox.Items[i] is not ListBoxItem item) continue;

                    int.TryParse(item.Tag.ToString(), out int mesId);
                    TelegramLib.MainClasses.Messages.Message mes =
                    chat.GetMessageById(mesId);
                    if (mes is null) return;
                    
                    
                    if (item.Content is ChatControls.TextMessage textMes)
                    {
                        SignalRHelperService.FastSetContactPhoto(user, _chat, textMes.BgBrush, textMes.UserEllipseImage);
                        //await SignalRHelperService.SetContactPhoto(user, _chat, textMes.BgBrush, textMes.UserEllipseImage);    
                    }
                    else if (item.Content is MediaMessage mediaMes)
                    {
                        //SetSenderImageByListBoxItem(item, _system.LoggedUser.Id == user.SenderUserId);
                        SignalRHelperService.FastSetContactPhoto(user, _chat, mediaMes.BgBrush, mediaMes.UserEllipseImage);
                    }
                    else if(item.Content is ShareContactControl share)
                    {
                        SignalRHelperService.FastSetContactPhoto(user,
                            _chat, share.BgBrush, share.SenderEllipseImage);
                    }
                }
            });
        }

        public void UpdateOnlineStatus(TelegramLib.MainClasses.User toUpdate)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(toUpdate, Enums.PrivacySettingType.LastSeen);
                await SignalRHelperService.SetLastSeenString(toUpdate, shareType, _chat, ChatFriendLastSeen);

                /* if (_chat is null || _chat.GetChatter().ContactUserId != toUpdate.Id) return;
                 HelperService.SetOnlineStatusInTextBox(ChatFriendLastSeen, toUpdate.IsOnline, toUpdate.LastSeenOnline);
             */
            });
        }

        public void OnMediaMessageReceived(TelegramLib.MainClasses.User sender,
            TelegramLib.MainClasses.Messages.MediaAction message)
        {
            Dispatcher.Invoke(async Task () =>
            {
                //Get chat wherer Logged is Sender 
                TelegramLib.MainClasses.UserChat chat = await GetChatByUserSendersIds(_system.LoggedUser.Id, sender.Id);
                //_system.GetChatByChatterId(sender.Id);
                if (chat is null) return;

                SetNewUserChatMessageControl(chat);

                if (_chat is null || chat.Id != _chat.Id)
                {
                    AddMediaMessageInUnChosenChat(chat, message);
                }
                else AddMediaMessageInChosenChat(message, sender);
            });
        }

        private async void AddMediaMessageInChosenChat(MediaAction message, TelegramLib.MainClasses.User sender)
        {
            //Add media in vis
            SetMediaMessageInChat(message,
               await SignalRHelperService.GetUserPhotoToSet(sender) /*sender.GetFirstImageNameInString()*/);

            //Add in system
            _chat.Messages.Add(message);

            //add in db
            await ApiService.AddMessage(message, _chat);
            ToUpdateUserControlMessage();
        }

        private async void AddMediaMessageInUnChosenChat(TelegramLib.MainClasses.UserChat chat, MediaAction message)
        {
            //Add in system 
            chat.Messages.Add(message);
            //Add in db
            await ApiService.AddMessage(message, chat);
            ToUpdateUserControlMessage();
        }

        private void OnTextMessageReceived(TelegramLib.MainClasses.User sender,
            TelegramLib.MainClasses.Messages.TextMessage message)
        {
            Dispatcher.Invoke(async Task () =>
            {
                if (message.RepliedMessageId is not null)
                {
                    TelegramLib.MainClasses.Messages.Message toGetPair = await ApiService.GetTextMessageById((int)message.RepliedMessageId);
                    if (toGetPair is not null)
                    {
                       TelegramLib.MainClasses.Messages.Message replied = await ApiService.GetPairOfMessage(toGetPair);
                        if (replied is not null) message.RepliedMessageId = replied.Id;
                    }
                }

                if (!await ApiService.IsUserOnline(_system.LoggedUser.Id))
                {
                    //_chat = null;
                    //return;
                }//Get chat where Logged is Sender 
                /*                TelegramLib.MainClasses.UserChat chat =
                                    _system.GetChatByChatterId(sender.Id);*/

                TelegramLib.MainClasses.UserChat chat =
                    await GetChatByUserSendersIds(_system.LoggedUser.Id, sender.Id);
                if (chat is null) return;

                //Set user talk if not contains un chats
                SetNewUserChatMessageControl(chat);

                if (_chat is null ||
                chat.Id != _chat.Id)
                {
                    AddTextMessageInUnChosenChat(chat, message);
                }
                else AddTextMessageInChosenChat(message, sender);

                //Is temp chat is chosen

                ToUpdateUserControlMessage();
            });
        }

        public void SetNewUserChatMessageControl(TelegramLib.MainClasses.UserChat chat)
        {
            if (_system.IsChatContainsInChats(chat.Id)) return;

            //Add chat + Update User talk controls(Chats)
            _system.AddChat(chat);
            ((MainWindow)Window.GetWindow(this)).UpdateChatControls();
        }

        private async Task AddTextMessageInChosenChat(TelegramLib.MainClasses.Messages.TextMessage message,
            TelegramLib.MainClasses.User sender)
        {
            TelegramLib.MainClasses.Messages.Message replied = _system.GetMessageById(message.RepliedMessageId);

            ChatControls.TextMessage text = new ChatControls.TextMessage(_system,
                GetConvertedStringMessage(message.Text),
                /*sender.GetFirstImageNameInString()*/ await SignalRHelperService.GetUserPhotoToSet(sender),
                _system.Settings.GetChatSettings().FontName, toReply:replied);

            //ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            await ApiService.AddMessage(message, _chat);

            message = (TelegramLib.MainClasses.Messages.TextMessage)await ApiService.GetLastChatMessage(_chat.Id);
            AddTextControl(text, message.Id);

            _chatMessages.Add(message);

            if (!await ApiService.IsUserOnline(_system.LoggedUser.Id)) return;

            ToUpdateUserControlMessage();
        }

        public void ToUpdateUserControlMessage()
        {
            MainWindow window = ((MainWindow)Window.GetWindow(this));

            if (window is not null)
            {
                window.UpdateUserChatTalkControl();
                return;
            }
            //_mainWindow.UpdateUserChatTalkControl();
        }

        public void AddTextControl(ChatControls.TextMessage text, int mesId)
        {
            ListBoxItem item = new ListBoxItem()
            {
                Content = text,
                Tag = mesId.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            //SetMessagePositionSettings(item);

            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);
        }

        private async void AddTextMessageInUnChosenChat(TelegramLib.MainClasses.UserChat chat,
            TelegramLib.MainClasses.Messages.TextMessage message)
        {
            //Add in system 
            chat.Messages.Add(message);
            //Add in db
            await ApiService.AddMessage(message, chat);

            ToUpdateUserControlMessage();
        }

        public TelegramLib.MainClasses.UserChat _chat;
        public event Action SettingEnded;

        public async void SetUserChat(TelegramLib.MainClasses.UserChat chat)
        {
            MessageMenu.Children.Clear();
            ReplyMessageRow.Height = new GridLength(0);

            if (chat is null) return;
            SetChatterImageVisibility();

            _chat = chat;
            SetPinnedMessages();

            SetUnblockGridVis();

            await SetOnlineStatus();

            //_chat.Messages.Add(new TelegramLib.MainClasses.Messages.TextMessage(1, 1, DateTime.Now, "asd"));

            UserChatMenu.SetChatParam(_chat);

            ClearChat();

            SetChatParams(_chat.GetChatter());
            SetChatMessages();

            UserChatMenu.SetChatParam(_chat);

            RemoveRightContactInfo();
            SetUserBg();

            SetChatterImage();

            SettingEnded?.Invoke();
        }

        public void SetPinnedMessages()
        {
            if (_chat is null) return;
            if (_chat.PinnedMessages.Count == 0)
            {
                PinRow.Height = new GridLength(0);
                return;
            }
            PinRow.Height = new GridLength(50);

            //Get messages to Set
            for (int i = 0; i < _chat.PinnedMessages.Count; i++)
            {
                SetPinnedMessageInPanel(_chat.PinnedMessages[i]);
                //Set them
            }
        }

        public bool IsChoseChatIdIsEqual(int id)
        {
            if (_chat is null) return false;
            return _chat.Id == id;
        }

        public void SetUnblockGridVis()
        {
            if (_chat is null && _system is null) return;

            bool isBlocked =
                _system.LoggedUser.IsUserIsBlockedById(_chat.Chatter.Id);

            UnBlockBorder.Visibility = isBlocked ? Visibility.Visible : Visibility.Hidden;
        }

        public void SetChatterImage()
        {
            UserImage.ImageSource = new BitmapImage(
                new Uri(FilesAction.GetUserImagePath(
                    _chat.Chatter.GetFirstImageNameInString()), UriKind.Absolute));
        }

        public async Task SetOnlineStatus()
        {
            if (_chat is null) return;
            TelegramLib.MainClasses.User user = await ApiService.GetUserById(_chat.GetChatter().Id);
            if (user is null) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);
            await SignalRHelperService.SetLastSeenString(user, shareType, _chat, ChatFriendLastSeen);
        }

        public void SetUserBg()
        {
            ChatBackground bg = _chat.GetBackground();
            if (bg is null) return;

            ImageBrush brush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(FilesAction.GetWallpaperPathByName(bg.GetFileName()), UriKind.Absolute)),
                Stretch = Stretch.UniformToFill
            };

            CustomBg.Background = brush;

            if (bg.IsBlurred)
            {
                CustomBg.Effect = new BlurEffect()
                {
                    Radius = 15
                };
            }
            else CustomBg.Effect = null;
        }

        public void SetChatMessages()
        {
            //Get Chatter here (Contact type)
            _chatMessages = _chat.GetChatMessages();
            SetMessagesInChat();
        }

        private string _lastSeenDefault = "recently";
        public void SetChatParams(TelegramLib.MainClasses.User contact)
        {
            UserContactcs? cont = _system.Contacts.FirstOrDefault(x => x.ContactUserId == contact.Id);
            ChatFriendLogin.Text = cont is null ? contact.Name : cont.Name;
            ChatFriendSurname.Text = cont is null ? contact.Surname : cont.Surname;
            /*
                        ChatFriendLastSeen.Text = contact.LastSeen is null ? _lastSeenDefault :
                            $"{contact.LastSeen.Value.Month}.{contact.LastSeen.Value.Day}.{contact.LastSeen.Value.Year}";
                  */
        }

        public void ClearChat()
        {
            ChatBox.Items.Clear();
        }

        private TelSystem _system;
        private MainWindow _mainWindow;
        public void SetSystemAndMainWindowParam(TelSystem system, MainWindow window)
        {
            //Set here chat messages(by ref)
            _system = system;
            _mainWindow = window;


            UserChatMenu.SetSystemParam(system);
            SetTestChatMessages();

            EmojisBoard.SetSystem(_system);

            SetLanguageText.SetUserChat(this);

        }

        public void SetTestChatMessages()
        {
            //Get Chatter here (Contact type)
            _chatMessages = _system.GetTestMessages();
            SetMessagesInChat();
        }

        public async void SetMessagesInChat()
        {
            for (int i = 0; i < _chatMessages.Count; i++)
            {
                string imgName = _chatMessages[i].SenderUserId == _system.LoggedUser.Id ?
                    _system.LoggedUser.GetFirstImageName().Name : _chat.GetChatter().GetFirstImageName().Name;

                if (_chatMessages[i] is TelegramLib.MainClasses.Messages.TextMessage text)
                {
                    SetTextMessageInChat(text, imgName);
                    //text
                }
                else if (_chatMessages[i] is MediaAction media)
                {
                    //Video or photo
                    SetMediaMessageInChat(media, imgName);
                }
                else if (_chatMessages[i] is
                    TelegramLib.MainClasses.Messages.ShareContactMessage share)
                {
                    ShareContact(share.SharedUser, share.SharedName, _chatMessages[i]);
                }

                if (_chatMessages[i].IsPinned)
                {
                    ListBoxItem? item =  ChatBox.Items.OfType<ListBoxItem>().LastOrDefault();
                    if (item is null ||
                        item.Content is not UserControl control) continue;

                    SetPinMessage(_chatMessages[i], control);
                }

            }
            //Set chatter photo image
            if (_chatMessages.Count > 0) UpdateChatImages(await ApiService.GetUserById(_chat.Chatter.Id));
        }

        public void SetMediaMessageInChat(MediaAction message, string senderImgName)
        {
            //Got type (To know what folder to search in)
            MediaType type = message.IsSticker ? MediaType.Sticker :
                FilesAction.GetMediaTypeFromFilename(message.MediaName);

            string path = FilesAction.GetFilePathByMediaType(type, message.MediaName);

            switch (type)
            {
                case MediaType.Image:
                    {
                        AddImageMessage(path, false, senderImgName, message);

                        return;
                    }
                case MediaType.Gif:
                    {
                        SendGif(path, senderImgName, isAdd: false);
                        return;
                    }
                case MediaType.Video:
                    {
                        AddMediaElement(path, senderImgName, message);
                        return;
                    }
                case MediaType.Sticker:
                    {
                        AddImageMessage(path, true, senderImgName, message);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public void SetTextMessageInChat(
            TelegramLib.MainClasses.Messages.TextMessage message,
            string senderImageName)
        {
            TelegramLib.MainClasses.Messages.Message? mes =  message.RepliedMessageId is null 
                ? null 
                : _system.GetMessageById((int)message.RepliedMessageId);

            ChatControls.TextMessage newMes =
                new ChatControls.TextMessage(_system, GetConvertedStringMessage(message.Text),
                senderImageName, _system.Settings.GetChatSettings().FontName, toReply:mes);

            newMes.SetTime(message.SentTime);

            ListBoxItem item = new ListBoxItem()
            {
                Content = newMes,
                Tag = message.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            item.PreviewMouseRightButtonDown += SetMessageMenu_PreviewRightMouseDown;

            //SetMessagePositionSettings(item);

            ChatBox.Items.Add(item);
            SetTickStatusIfCorrectMes(item, message);

            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
            SetMessagesPosition(_isGluedToLeft);

            SetSenderImageByListBoxItem(item, _system.LoggedUser.Id == message.SenderUserId);
        }

        public HorizontalAlignment GetHorAlignmentForMessage()
        {
            return _isGluedToLeft ?
                HorizontalAlignment.Left :
                HorizontalAlignment.Right;
        }

        public void SetMarginForChatMenu()
        {
            UserChatMenu.Margin = new Thickness(
                0,
                UpperRow.Height.Value,
                20,
                0
            );
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (string.IsNullOrEmpty(CommentTextBox.Text)) return;
                MessageMenu.Children.Clear();

                //To send text message
                AddTextMessageControl(_system.LoggedUser.GetFirstImageName().Name,
                    CommentTextBox.Text);
                ReplyMessageRow.Height = new GridLength(0);
            }
        }

        public bool IsReplyMessage()
        {
            return ReplyBorder.Visibility == Visibility.Visible ||
                ReplyedImageColumn.Width.Value != 0;
        }

        public TelegramLib.MainClasses.Messages.Message GetMessageToReply()
        {
            if (!IsReplyMessage() || _mesMenu is null ||
                _mesMenu.GetChosenListBoxItem() is null ||
                ReplyMessageRow.Height.Value == 0) return null;

            int.TryParse(_mesMenu.GetChosenListBoxItem().Tag.ToString(), out int id);

            return _system.GetMessageById(id);
        }

        private async void AddTextMessageControl(string senderImageName, string sendText)
        {
            //Is reply
            //Get reply message
            TelegramLib.MainClasses.Messages.Message toReply = GetMessageToReply();

            //Visaul add
            ChatControls.TextMessage text = new ChatControls.TextMessage(_system,
                GetConvertedStringMessage(sendText),
                senderImageName, _system.Settings.GetChatSettings().FontName,
                toReply);



            ListBoxItem item = new ListBoxItem()
            {
                Content = text,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = GetHorAlignmentForMessage(),
            };
            item.PreviewMouseRightButtonDown += SetMessageMenu_PreviewRightMouseDown;


            //SetMessagePositionSettings(item);

            //system add

            int? replyId = toReply is null ? null : toReply.Id;
            // UserContactcs contact = await ApiService.GetContactByUserAndFriendIds(_system.LoggedUser.Id, _chat.Chatter.Id);
            TelegramLib.MainClasses.Messages.Message toAdd =
                new TelegramLib.MainClasses.Messages.TextMessage(
                            _chatMessages.Count, _system.LoggedUser.Id,
                            DateTime.Now, sendText, false, replyId, false, null);

            //Adding in DB
            await ApiService.AddMessage(toAdd, _chat);

            toAdd = await ApiService.GetLastChatMessage(_chat.Id);
            item.Tag = toAdd.Id.ToString();
            ChatBox.Items.Add(item);
            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            if (toAdd is not TelegramLib.MainClasses.Messages.TextMessage toAddText) return;

            //await SendMessageToReceiver(toAddText);

            //toAdd.SenderId = contact.Id;
            _chatMessages.Add(toAddText);
            if (CommentTextBox.Text == sendText) CommentTextBox.Text = string.Empty;

            //Add Message In DB (On chatterss side) 
            AddTextMessageInDb(toAddText);

            //Set vis tick 
            SetTickStatusIfCorrectMes(item, toAdd);

            ((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();
            SetMessagesPosition(_isGluedToLeft);
        }

        public void SetMessageMenu_PreviewRightMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item) return;
            System.Windows.Point clickPosition = e.GetPosition(this);

            if (item.Content is ChatControls.TextMessage text)
            {
                //Set message menu for text
                AddMessageMenu(MessageMenuType.TextMessage, clickPosition, item);
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                AddMessageMenu(MessageMenuType.MediaMessage, clickPosition, item);
            }
            else if (item.Content is ShareContactControl share)
            {
                AddMessageMenu(MessageMenuType.ShareContact, clickPosition, item);
            }
        }

        private MesMenu _mesMenu;
        public void AddMessageMenu(
            MessageMenuType menuType,
            System.Windows.Point point,
            ListBoxItem item)
        {
            MessageMenu.Children.Clear();

            _mesMenu = new MesMenu(menuType);
            _mesMenu.SetClickedListBoxItem(item);

            Message mes = GetMessageByListBoxTag(item);
            _mesMenu.SetPinVisStatus(mes);

            MessageMenu.Children.Add(_mesMenu);
            SetMesMenuActions(_mesMenu);

            //is x to big
            if (point.X + _mesMenu.Width > this.ActualWidth)
            {
                Canvas.SetLeft(_mesMenu, point.X - _mesMenu.Width);
            }
            else Canvas.SetLeft(_mesMenu, point.X);

            //is y too big
            if (point.Y + _mesMenu.Height > this.ActualHeight)
            {
                Canvas.SetTop(_mesMenu, point.Y - _mesMenu.Height);
            }
            else Canvas.SetTop(_mesMenu, point.Y);
        }

        public void SetMesMenuActions(MesMenu menu)
        {
            menu.ReplyAct += () => SetReplyMessageRow();
            menu.PinAct += () => SetPinnedAction();
            menu.ForwardAct += () => ForwardMesAction();

            menu.DeleteAct += () => DeleteMessageAction();
            menu.CopyAct += () => CopyMessageAction();
            menu.SaveAct += () => SaveMediaAction();
        }

        public void ForwardMesAction()
        {
            //Set page to choose destionation of forwarding
            //Set stuff
        }

        public void SaveMediaAction()
        {
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = GetMessageByListBoxTag(item);
            if (mes is null || mes is not MediaAction media) return;

            if (media.IsGif())
            {
                string gifFullPath = FilesAction.GetFullChatImagePath(media.MediaName);
                SaveElements.SaveGifAs(media.MediaName);
            }
            else if (media.IsImage())
            {
                string imgPath = FilesAction.GetFullChatImagePath(media.MediaName);
                var image = new Image();
                image.Source =  new BitmapImage(new Uri(imgPath));
                SaveElements.SaveImageAs(image);
            }
            else if (media.IsVideo())
            {
                MediaElement videoElement = FilesAction.GetMediaElementByVideoName(media.MediaName);
                SaveElements.SaveVideoAs(videoElement);
            }
        }

        public void CopyMessageAction()
        {
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = GetMessageByListBoxTag(item);
            if (mes is null) return;

            if (mes is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                Clipboard.SetText(text.Text);
            }
            else if (mes is TelegramLib.MainClasses.Messages.MediaAction media)
            {
                string mediaPath = FilesAction.GetFullChatImagePath(media.MediaName);
                var image = new BitmapImage(new Uri(mediaPath));
                Clipboard.SetImage(image);
            }
            else if (mes is TelegramLib.MainClasses.Messages.ShareContactMessage share)
            {
                Clipboard.SetText("Contact");
            }
        }

        public void DeleteMessageAction()
        {
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = GetMessageByListBoxTag(item);
            if (mes is null) return;

            //Remove from system
            _system.RemoveMessageById(mes.Id);

            //Remove from Visual
            ChatBox.Items.Remove(item);

            //Remove from db
            ApiService.DeleteMessageById(mes.Id);

            //Remove in SignalR
        }

        public void SetPinOnVisControl(UserControl control, bool isPinned)
        {
            //Set pin icon on message userControl
            if (control is ChatControls.TextMessage text)
            {
                text.SetPinColumnState(isPinned);
            }
            else if (control is ChatControls.MediaMessage media)
            {
                media.SetPinColumnState(isPinned);
            }
            else if (control is ShareContactControl share)
            {
                share.SetPinColumnState(isPinned);
            }
        }

        public void SetPinnedAction()
        {
            //Get ListBoxItem From Menu
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();  //GetListBoxItemFromMenu();
            if (item is null || item.Content is not UserControl control) return;

            //item.Tag == message id; Get message by system
            Message mes = GetMessageByListBoxTag(item);
            if (mes is null) return;

            //Set Pin status in system
            mes.MirrorPinStatus();

            ApiService.SetPinStatus(mes.Id, mes.IsPinned);

            SetPinMessage(mes, control);

/*            if (mes.IsPinned)
            {
                //Set pin in message visual
                AddPinnedMessage(mes);
                SetPinOnVisControl(control, true);
                return;
            }
            else
            {
                DeletePinnedMessage(mes);
                SetPinOnVisControl(control, false);
            }*/
        }

        public void SetPinMessage(TelegramLib.MainClasses.Messages.Message mes, 
            UserControl control)
        {
            if (mes.IsPinned)
            {
                //Set pin in message visual
                AddPinnedMessage(mes);
                SetPinOnVisControl(control, true);
                return;
            }
            else
            {
                DeletePinnedMessage(mes);
                SetPinOnVisControl(control, false);
            }
        }

        public bool IsHidePinnedMessesRow(TelegramLib.MainClasses.Messages.Message lastDeletedPinned)
        {
            bool isAnyPinnedMessages = _system.IsAnyPinnedMessagesByMessage(lastDeletedPinned);
            if (!isAnyPinnedMessages)
            {
                PinRow.Height = new GridLength(0);
                return true;
            }
            return false;
        }

        public void DeletePinnedMessage(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            //Delete from vis
            int.TryParse(PinRowBorder.Tag.ToString(), out int tempMesId);

            //Change temp on next
            if (mes.Id == tempMesId)
            {
                TelegramLib.MainClasses.Messages.Message nextMes =
                     _system.GetNextPinnedMessage(mes);
                SetPinnedMessageInPanel(nextMes);
            }

            //Remove mes from system
            _system.DeletePinnedMessage(mes);

            //Delete in DB
            //Delete with SignalR

            if (IsHidePinnedMessesRow(mes)) return;
            //Is pinned panel is visible
            PinRow.Height = new GridLength(50);
        }

        public void AddPinnedMessage(TelegramLib.MainClasses.Messages.Message mes)
        {
            //Is pinned panel is visible
            PinRow.Height = new GridLength(50);

            //Add in last position
            _system.AddPinnedMessage(mes);

            //Set in last position + show this in panel
            SetPinnedMessageInPanel(mes);
        }

        public void SetPinnedMessageInPanel(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            if (mes is MediaAction mediaAct)
            {
                string path = FilesAction.GetUserImagePath(mediaAct.MediaName);
                if (File.Exists(path))
                {
                    PinnedImage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                }
            }
            else PinnedImage.Source = null;

            PinMesNumber.Text = $"Pinned message #{_system.GetPinnedMessageIndex(mes)}";
            PinnedMessage.Text =
                mes is TelegramLib.MainClasses.Messages.TextMessage text ? text.Text :
                mes is TelegramLib.MainClasses.Messages.MediaAction media ? "Media" :
                mes is TelegramLib.MainClasses.Messages.ShareContactMessage share ? "Contact" :
                "Pinned message";

            PinRowBorder.Tag = mes.Id.ToString();
        }

        public async Task AddTextMessageInDb(
            TelegramLib.MainClasses.Messages.TextMessage toAddText)
        {
            if (await ApiService.IsUserIsBlocked(_chat.Chatter.Id, _system.LoggedUser.Id)) return;

            //TO add in both chats if chatter online
            if (await ApiService.IsUserOnline(_chat.Chatter.Id))
            {
                await SignalRService.SendTextMessage(_system.LoggedUser, toAddText, _chat.Chatter);
                return;
            }

            //just to Add in chatters chat in db
            //Get chat
            TelegramLib.MainClasses.UserChat chat = await GetChatByUserSendersIds(_chat.Chatter.Id, _system.LoggedUser.Id);
            //await ApiService.GetChatByUserAndSenderId(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (chat is null) return;

            if (toAddText.RepliedMessageId is not null) await ChangeReplyMessageId(toAddText);

            //Add in chats db
            await ApiService.AddMessage(toAddText, chat);
        }

        public async Task ChangeReplyMessageId(TelegramLib.MainClasses.Messages.TextMessage message)
        {
            //Check the refferancwe passing in message
            if (message.RepliedMessageId is null) return;

            //Get mirror of the message to reply
            TelegramLib.MainClasses.Messages.Message mes = 
                await ApiService.GetTextMessageById((int)message.RepliedMessageId);

            TelegramLib.MainClasses.Messages.Message? res = await ApiService.GetPairOfMessage(mes);

            if (res is null) message.RepliedMessageId = -1;
            else message.RepliedMessageId = res.Id;

            //return message;
        }

        public async Task<TelegramLib.MainClasses.UserChat> GetChatByUserSendersIds(int userId, int senderId)
        {
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(senderId);

            if (chat is null)
            {
                chat = await ApiService.GetChatByUserAndSenderId(userId, senderId);
            }
            if (chat is null)
            {
                await ApiService.AddNewChat(userId, senderId);

                chat = await ApiService.GetChatByUserAndSenderId(userId, senderId);
            }
            return chat;
        }

        private async Task SendMessageToReceiver(Message toAdd)
        {
            bool isReceiverOnline = await ApiService.IsUserOnline(_chat.GetChatter().Id);

            if (!isReceiverOnline)
            {
                TelegramLib.MainClasses.User receiver =
                await ApiService.GetUserById(_chat.GetChatter().Id);

                //UserContactcs contact = await ApiService.GetContactByUserAndFriendIds(_system.LoggedUser.Id, receiver.Id);

                TelegramLib.MainClasses.UserChat chat =
                    await ApiService.GetChatByUserAndSenderId(receiver.Id, _system.LoggedUser.Id);
                await ApiService.AddMessage(toAdd, chat);

                return;
            }
            await SendSignalRMessage(toAdd);
        }

        public void AddEmoji(string emoji)
        {
            CommentTextBox.Text += emoji;

            EmojisBoard.Visibility = Visibility.Hidden;
        }

        private string GetConvertedStringMessage(string str)
        {
            const int checker = 15;

            for (int i = 0; i < str.Length; i++)
            {
                if (i % checker == 0 && i != 0)
                {
                    str = str.Insert(i, "\n");
                }
            }
            return str;
        }

        private void AddFile_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            AddFile.Foreground = Brushes.White;
        }

        private void AddFile_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            AddFile.Foreground = Brushes.Gray;
        }

        private void AddFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose image or video",
                Filter = "Image and Video files|*.png;*.jpg;*.jpeg;*.mp4;*.mov;*.avi"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();


                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    AddMediaPage(filePath);
                    /*                    AddImageMessage(filePath, false, _system.LoggedUser.GetFirstImageName().Name);
                                        AddMediaPath(filePath);*/
                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                {
                    bool isAdd = AddMediaPath(filePath).Result; //??

                    if (isAdd)
                    {
                        MediaAction toCheck = (MediaAction)_chatMessages.Last();
                        AddMediaElement(filePath, _system.LoggedUser.GetFirstImageName().Name, toCheck);
                    }
                    UpdateContactInfoBlock();
                }
            }
        }

        public void AddMediaPage(string fullMediaPath)
        {
            ((MainWindow)Window.GetWindow(this)).AddAddMediaPage(fullMediaPath);
        }

        public void AddImageMessage(string filePath, bool isSticker, string senderImageName,
            MediaAction mediaMes)
        {
            var img = new Image
            {
                Source = new BitmapImage(new Uri(filePath, UriKind.Absolute)),
            };

            //Is image is contains in user chat folder
            if (!FilesAction.IsUserChatMediaIsExist(Path.GetFileName(filePath)))
            {
                FilesAction.CopyImageToImageFolder(filePath);
            }
            AddImageMessage(img, isSticker, senderImageName, mediaMes);
        }

        public async Task<bool> AddMediaPath(string filePath,
            bool isSticker = false, bool isAdd = true)
        {
            string fileName = Path.GetFileName(filePath);

            //Check it for presance

            /*            if (_chatMessages.Where(x => x is MediaAction media &&
                                media.MediaName == Path.GetFileName(fileName))
                            .ToList()
                            .Any()) return;*/

            Message newMediaMes = new MediaAction(-1, _system.LoggedUser.Id,
                DateTime.Now, fileName, isSticker, false, false, null);

            if (isAdd)
            {
                await ApiService.AddMessage(newMediaMes, _chat);
                //Set id to image
                newMediaMes = await ApiService.GetLastChatMessage(_chat.Id);

                _chatMessages.Add(newMediaMes);

                await SendMessageToReceiver(newMediaMes);
                //await SendSignalRMessage(newMediaMes);
                return true;
            }
            return false;
        }

        public void AddMediaElement(string filePath, string senderImageName, MediaAction mes)
        {
            var media = new MediaElement
            {
                Source = new Uri(filePath, UriKind.Absolute),
                Width = 300,
                Height = 200,
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            media.Play();

            //Is video is contains in user chat folder
            if (!FilesAction.IsVideoIsExistInSecFolder(Path.GetFileName(filePath)))
            {
                FilesAction.CopyVideoToVideoFolder(filePath);
            }

            AddVideoMessage(media, senderImageName, mes);
        }

        public void SendGif(string gifPath, string senderImageName,
            bool isAdd = true, MediaAction mes = null)
        {
            DateTime sentDate = mes is null ? DateTime.Now : mes.SentTime;

            var message = new MediaMessage(gifPath, senderImageName, sentDate);
            message.PreviewMouseDown += ChatGif_PreviewMouseDown;

            if (isAdd) AddMediaPath(gifPath, isAdd: isAdd);
            if (mes is null) mes = (MediaAction)_chatMessages.Last();

            ListBoxItem item = new ListBoxItem()
            {
                Content = message,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Tag = mes.Id.ToString()
            };
            item.PreviewMouseRightButtonDown += SetMessageMenu_PreviewRightMouseDown;

            //SetMessagePositionSettings(item);

            ChatBox.Items.Add(message);
            SetMessagesPosition(_isGluedToLeft);
        }


        private void ChatGif_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage) return;
            MediaMessage message = sender as MediaMessage;

            VisualActionPage page = new VisualActionPage(message.GetGifPath(), GetChatMediaPaths(MediaType.Gif));

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);

            VisualActionPageParams(message, MediaType.Gif, page);
        }

        private void AddVideoMessage(MediaElement el, string senderImageName, MediaAction mes)
        {
            var video = new MediaMessage(el, senderImageName);
            video.PreviewMouseDown += ChatVideo_PreviewMouseDown;

            ListBoxItem item = new ListBoxItem()
            {
                Content = video,
                Tag = mes.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            //SetMessagePositionSettings(item);

            ChatBox.Items.Add(video);
            SetMessagesPosition(_isGluedToLeft);
        }

        private void ChatVideo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage) return;
            MediaMessage message = sender as MediaMessage;

            VisualActionPage page = new VisualActionPage(message.GetVideo(), GetChatMediaPaths(MediaType.Video));

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);

            VisualActionPageParams(message, MediaType.Video, page);

            /*            List<MediaAction> videos =
                            FilesAction.GetMediaElementsFromListByType(_chat.GetMediaMessages(), MediaType.Video);

                        int chosenVideoIndex = GetChosenVideoIndex(message, videos);

                        page.SetUserChat(_system, videos, chosenVideoIndex);*/
        }

        public void VisualActionPageParams(MediaMessage mediaMes, MediaType type,
            VisualActionPage page)
        {
            List<MediaAction> elements =
                FilesAction.GetMediaElementsFromListByType(_chat.GetMediaMessages(), type);

            int chosenVideoIndex = GetChosenVideoIndex(mediaMes, elements);

            page.SetUserChat(_system, elements, chosenVideoIndex, _chat);
        }

        public int GetChosenVideoIndex(MediaMessage message, List<MediaAction> videos)
        {
            int messageItemIndex = ChatBox.Items.IndexOf(message);

            return videos.IndexOf((MediaAction)_chat.Messages[messageItemIndex]);
        }

        public List<string> GetChatMediaPaths(MediaType type)
        {
            List<string> res = new List<string>();
            for (int i = 0; i < _chatMessages.Count; i++)
            {
                if (_chatMessages[i] is MediaAction media &&
                    FilesAction.GetMediaTypeFromFilename(media.MediaName) == type)
                {
                    string path = FilesAction.GetFilePathByMediaType(type, media.MediaName);
                    res.Add(path);
                }
            }
            return res;
        }

        private bool _isGetStikers;

        public void AddImageMessage(Image img, bool isSticker, string senderImgName,
            MediaAction media)
        {
            var message = new MediaMessage(img, isSticker, senderImgName, media.SentTime);
            message.MouseLeftButtonDown += ChatImage_PreviewMouseDown;


            //Set tick vis
            SetMediaTickVis(media, message);

            ListBoxItem item = new ListBoxItem()
            {
                Content = message,
                Tag = media.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            item.PreviewMouseRightButtonDown += SetMessageMenu_PreviewRightMouseDown;

            //SetMessagePositionSettings(item);
            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);
            SetSenderImageByListBoxItem(item, _system.LoggedUser.Id == media.SenderUserId);
        }

        public void SetMediaTickVis(MediaAction media, MediaMessage message)
        {
            if (_system.LoggedUser.Id != media.SenderUserId) return;

            string tickVis = media.IsRead ? _readIconName : _unreadIconName;
            message.SetTickVis(tickVis, media.SenderUserId == _system.LoggedUser.Id);
        }

        public void AddStickerMessage(Image img, string senderImageName)
        {
            var message = new MediaMessage(img, true, senderImageName, DateTime.Now);
            //message.PreviewMouseDown += ChatImage_PreviewMouseDown;
            ListBoxItem item = new ListBoxItem()
            {
                Content = message,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            //SetMessagePositionSettings(item);

            ChatBox.Items.Add(item);

            //TelegramLib.MainClasses.UserChat messages = _system.GetChosenChat();

            //_chat.AddSticker(img.Tag.ToString(), _system.LoggedUser.Id);

            AddMediaPath(img.Tag.ToString(), true);
            SetMessagesPosition(_isGluedToLeft);
        }

        private void ChatImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage message) return;
            _isGetStikers = message.IsSticker;
            if (_isGetStikers) return; //NO TO STICKERS

            VisualActionPage page = new VisualActionPage(message.GetImage(), GetChatImages());
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);

            ListBoxItem item = ChatBox.Items.OfType<ListBoxItem>()
                .Where(x => x.Content == message).First();

            int index = ChatBox.Items.IndexOf(item);

            List<MediaAction> imgMedias = _chat.GetMediaMessages().Where(x => FilesAction.IsFileIsImage(x.MediaName)).ToList();

            int imgIndex = imgMedias.FindIndex(x => x == _chat.Messages[index] as MediaAction);

            page.SetUserChat(_system, imgMedias, imgIndex, _chat);
        }

        public List<Image> GetChatImages()
        {
            List<Image> res = new List<Image>();

            for (int i = 0; i < _chatMessages.Count; i++)
            {
                //For images (NO STIKER)
                if (_chatMessages[i] is MediaAction media && media.IsSticker == _isGetStikers &&
                    FilesAction.GetMediaTypeFromFilename(media.MediaName) == MediaType.Image)
                {
                    string path = FilesAction.GetFilePathByMediaType(
                        media.IsSticker ? MediaType.Sticker : MediaType.Image, media.MediaName);
                    res.Add(new Image
                    {
                        Source = new BitmapImage(new Uri(path, UriKind.Absolute)),
                    });
                }
            }
            return res;
        }

        public bool _isLoopPressed = false;
        private void FindMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //find message menu
            _isLoopPressed = true;
        }

        public bool GetLoopState() => _isLoopPressed;
        public void TurnOfLoopState()
        {
            _isLoopPressed = false;
        }

        public void TurnOnLoopState()
        {
            _isLoopPressed = true;
        }

        private void UserInfoBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UserInfoColumn.Width.Value == 0)
            {
                AddContactInfo();
                return;
            }
            RemoveRightContactInfo();
        }

        public async Task AddContactInfo()
        {
            const int _userContactWidth = 450;
            double windowWidth = ((MainWindow)Window.GetWindow(this)).ActualWidth;

            ContactInfo info = new ContactInfo();
            ContactInfoGrid.Children.Add(info);


            //info.SetContactInfo(_chat, _system, _system.GetContactByUserId(_chat.Chatter.Id)); /*_system.ChosenChatContact*/

            info.LoadEnd += () =>
            {
                if (windowWidth + _userContactWidth <=
                    SystemParameters.PrimaryScreenWidth)
                {
                    ((MainWindow)Window.GetWindow(this)).Width =
                        windowWidth + _userContactWidth;
                }

                info.CloseButGrid.MouseDown += CloseContactInfo_MouseDown;

                UserInfoColumn.Width = new GridLength(_userContactWidth);
            };

            await info.SetContactInfo(_chat, _system,
                _system.GetContactByUserId(_chat.Chatter.Id), isSetMaxHeight: false);
        }

        public void UpdateContactInfoBlock()
        {
            //update blocked bage vis
            if (_system.LoggedUser.BlockedUsers.Contains(_chat.Chatter))
            {
                UnBlockBorder.Visibility = Visibility.Visible;
            }
            else UnBlockBorder.Visibility = Visibility.Hidden;


            //update contact
            if (!ContactInfoGrid.Children.OfType<ContactInfo>().Any()) return;

            ContactInfo info =
                ContactInfoGrid.Children
                .OfType<ContactInfo>()
                .First();

            if (info is null) return;
            info?.SetContactInfo(_chat, _system,
                 _system.GetContactByUserId(_chat.Chatter.Id), isSetMaxHeight: false);
        }

        public void CloseContactInfo_MouseDown(object sender, MouseEventArgs e)
        {
            RemoveRightContactInfo();
        }

        public void RemoveRightContactInfo()
        {
            ContactInfoGrid.Children.Clear();
            UserInfoColumn.Width = new GridLength(0);
        }

        public void UpdateColors()
        {
            EmojisBoard.ActiveRect.Fill =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            TextBlock? block = EmojisBoard.TabsPanel.Children.OfType<TextBlock>().Where
                (x => !CompareColors(x)).FirstOrDefault();

            if (block is null) return;
            block.Foreground =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
        }

        private bool CompareColors(TextBlock block)
        {
            return block.Foreground is SolidColorBrush brush &&
                brush.Color == Colors.Gray;
        }


        private void UserChatMenuBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //show user menu
            UserChatMenu.Visibility = Visibility.Visible;
        }

        private void UserChatMenuBut_MouseEnter(object sender, MouseEventArgs e)
        {
            SetForegroundForIcon(UserChatMenuIcon, Brushes.White);
        }

        private void UserChatMenuBut_MouseLeave(object sender, MouseEventArgs e)
        {
            SetForegroundForIcon(UserChatMenuIcon, Brushes.Gray);
        }

        private void UserInfoBut_MouseEnter(object sender, MouseEventArgs e)
        {
            SetForegroundForIcon(UserInfoIcon, Brushes.White);
        }

        private void UserInfoBut_MouseLeave(object sender, MouseEventArgs e)
        {
            SetForegroundForIcon(UserInfoIcon, Brushes.Gray);
        }

        private void FindMessageBut_MouseEnter(object sender, MouseEventArgs e)
        {
            SetForegroundForIcon(FindMessageIcon, Brushes.White);
        }

        private void FindMessageBut_MouseLeave(object sender, MouseEventArgs e)
        {
            SetForegroundForIcon(FindMessageIcon, Brushes.Gray);
        }

        public void SetForegroundForIcon(PackIcon icon, Brush color)
        {
            icon.Foreground = color;
        }

        private void UserInforGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserInforGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void UserInforGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pages.UserInfo info = new Pages.UserInfo(_chat, _system);
            SetUserInfoPageHeight(info);

            info.ContactInfo.LoadEnd += () =>
            {
                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(info);
            };
        }

        public void SetUserInfoPageHeight(Pages.UserInfo info)
        {
            UserContactcs contact =
                _system.GetContactByUserId(_chat.Chatter.Id);

            double windowHeight = ((MainWindow)Window.GetWindow(this)).ActualHeight;
            info.Height = windowHeight <= info.Height ? info.Height : windowHeight - 230
            - (contact is null ? info.GetHiddenLineIfContactNull() : 0);
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void EmojisGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateColors();
            EmojisBoard.Visibility = Visibility.Visible;

            Emojis.Foreground = new SolidColorBrush(Colors.LightGray);
            Cursor = Cursors.Hand;
        }

        private void EmojisBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            EmojisBoard.Visibility = Visibility.Hidden;
        }

        public void ScrollToMessageByMessageId(int messageId)
        {
            ListBoxItem? item = ChatBox.Items
                .OfType<ListBoxItem>()
                .FirstOrDefault(x => x.Tag.ToString() == messageId.ToString());
            if (item is null) return;

            int index = ChatBox.Items.IndexOf(item);
            if (index == -1) return;

            ScrollToChosenItem(index);
        }

        public void ScrollToChosenItem(int index)
        {
            if (ChatBox.Items.Count <= index)
            {
                index = ChatBox.Items.Count - 1;
            }

            var item = ChatBox.Items[index];
            ChatBox.ScrollIntoView(item);

            SolidColorBrush resourceBrush =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            Color color = resourceBrush.Color;


            HighlightListBoxItem(index, color);
        }

        public void HighlightListBoxItem(int index, Color highlightColor)
        {
            var item = (ListBoxItem)ChatBox.ItemContainerGenerator.ContainerFromIndex(index);
            if (item == null) return;

            var brush = new SolidColorBrush(highlightColor);
            item.Background = brush;

            var animation = new ColorAnimation()
            {
                From = highlightColor,
                To = Colors.Transparent,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, e) =>
            {
                item.Background = Brushes.Transparent;
            };

            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        public void SetBackground()
        {
            /*   if (_chat is null || 
                   _chat.ChatBg is null) throw new Exception("Chat cant be null");*/

            //set local
            if (_chat is not null && !_chat.GetBackground().IsGeneral)
            {
                SetChatBackground();
                return;
            }
            //set general
            if (_chat is not null && _chat.GetBackground().IsGeneral)
            {
                SetGeneralBackground();
                return;
            }
            //set transparent
            CustomBg.Background = new SolidColorBrush(Colors.Transparent);
        }

        public void SetGeneralBackground()
        {
            //Update Every unset bgs in chat
            CustomBg.Background = GetBgImageBrush(
                _system.Settings.GetChatSettings().Wallpaper.WallpaperName);

            if (_system.Settings.GetChatSettings().Wallpaper.IsBlurred)
            {
                CustomBg.Effect = new BlurEffect()
                {
                    Radius = 20
                };
                return;
            }
            CustomBg.Effect = null;
        }

        public void SetChatBackground()
        {
            CustomBg.Background = GetBgImageBrush(_chat.GetBackground().GetFileName());

            if (_chat.GetBackground().GetBlurState())
            {
                CustomBg.Effect = new BlurEffect()
                {
                    Radius = 20
                };
            }
        }

        public ImageBrush GetBgImageBrush(string fileName)
        {
            //System.UnauthorizedAccessException: "Access to the path 'B:\GitHub\TelegramClientServer\TelegramVisualPart\Visuals\Images\Wallpapers' is denied."
            return new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(FilesAction.GetWallpaperPathByName(fileName), UriKind.Absolute)), // или Relative
                Stretch = Stretch.UniformToFill
            };
        }
        private DispatcherTimer _timer;
        public void SetAutoDeleteTimer()
        {
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += (s, e) =>
            {
                if (_chat is null) return;

                //Get First message date
                DateTime? time = _chat.GetFirstMessageDateTime();
                if (time is null) return;

                //Get auto delete date time
                DateTime deleteTime = DateTime.Now.AddYears(-10); // _chat.AutoDelDuration.Duration;

                //if need to delete
                DateTime? firstMessageTime = _chat.GetFirstMessageDateTime();
                if (firstMessageTime is null ||
                deleteTime < firstMessageTime) return; //no need in delete

                //NEED to delete
                ChatBox.Items.RemoveAt(0);
                _chat.RemoveFirstMessage();


                //Check IF NEED to update
                //update vis + code (check it)

            };

            _timer.Start();
        }

        private void EmojisGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Emojis.Foreground = new SolidColorBrush(Colors.Gray);
            Cursor = null;
        }

        public void RemoveElementFromChatBox(int elIndex, MediaType type)
        {
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is MediaMessage media)
                {
                    if (type == MediaType.Image && (media.GetImage() is null || media.IsSticker)) continue;
                    if (type == MediaType.Video && media.GetVideo() is null) continue;
                    if (type == MediaType.Gif && media._gifPath is null) continue;

                    if (elIndex == 0)
                    {
                        ChatBox.Items.Remove(media);
                        return;
                    }
                    elIndex--;
                }
            }
        }

        public async Task SendSignalRMessage(Message message)
        {
            //So now here is SENDERS chat
            //we need to send RECEIVERS chat to update it here
            //Where sender is receiver; receiver is sender

            //Get contact where senderId is friendId, UserId - receiverId
            UserContactcs contact =
                await ApiService.GetContactByUserAndFriendIds(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (contact is null) return;
            /*
                        TelegramLib.MainClasses.UserChat chat = 
                            await ApiService.GetChatByUserAndSenderId(_chat.Chatter.Id, contact.ContactUserId);
                        if (chat is null) return;*/
            if (_chat is null) return;


            if (message is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                await SignalRService.SendTextMessage(_system.LoggedUser, text, _chat.Chatter);
            }
            else if (message is TelegramLib.MainClasses.Messages.MediaAction media)
            {
                await SignalRService.SendMediaMessage(_system.LoggedUser, media, _chat.Chatter);
            }
        }


        private bool _isGluedToLeft = false;
        public void SetMessagesPosition(bool isGluedToLeft)
        {
            _isGluedToLeft = isGluedToLeft;

            if (_chatMessages.Count == 0) return;
            //Set that in chat can be ONLY MESSAGES
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item ||
                    item.Tag is null)
                {
                    continue;
                }
                    int.TryParse(item.Tag.ToString(), out int id);

                Message mes = _chatMessages.FirstOrDefault(x => x.Id == id);
                if (mes is null) return;
                if (mes.SenderUserId != _system.LoggedUser.Id) continue;

                SetMessagePositionSettings(item);

                /* if (_chatMessages[i].SenderUserId == _system.LoggedUser.Id &&
                     item.Content is UserControl ctrl)
                 {
                     if (_isGluedToLeft)
                     {
                         item.HorizontalAlignment = HorizontalAlignment.Left;
                         item.Margin = new Thickness(0, 0, 0, 0);
                     }
                     else
                     {
                         item.HorizontalAlignment = HorizontalAlignment.Right;
                         item.Margin = new Thickness(0, 0, 0, 0);
                     }

                     if (item.Content is ChatControls.TextMessage text)
                     {
                         if (!_isGluedToLeft) text.UserEllipseImage.Visibility = Visibility.Hidden;
                         else text.UserEllipseImage.Visibility = Visibility.Visible;
                     }
                     else if (item.Content is ChatControls.MediaMessage media)
                     {
                         if (!_isGluedToLeft) media.UserEllipseImage.Visibility = Visibility.Hidden;
                         else media.UserEllipseImage.Visibility = Visibility.Visible;
                     }
                 }*/

            }
        }

        public void SetMessagePositionSettings(ListBoxItem item)
        {
            if (item.Content is UserControl ctrl)
            {
                if (_isGluedToLeft)
                {
                    item.HorizontalContentAlignment = HorizontalAlignment.Left;
                    item.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    item.HorizontalContentAlignment = HorizontalAlignment.Right;
                    item.Margin = new Thickness(0, 0, 0, 0);
                }

                if (item.Content is ChatControls.TextMessage text)
                {
                    if (!_isGluedToLeft) text.UserEllipseImage.Visibility = Visibility.Hidden;
                    else text.UserEllipseImage.Visibility = Visibility.Visible;
                }
                else if (item.Content is ChatControls.MediaMessage media)
                {
                    if (!_isGluedToLeft) media.UserEllipseImage.Visibility = Visibility.Hidden;
                    else media.UserEllipseImage.Visibility = Visibility.Visible;
                }
                else if(item.Content is ShareContactControl share)
                {
                    if (!_isGluedToLeft) share.SenderEllipseImage.Visibility = Visibility.Hidden;
                    else share.SenderEllipseImage.Visibility = Visibility.Visible;
                }
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            BackButIcon.Foreground = Brushes.LightGray;
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            BackButIcon.Foreground = Brushes.DarkGray;
        }

        public void SetVisibilityToBackBut(bool isVisible)
        {
            BackButColumn.Width = isVisible ? new GridLength(50) :
                new GridLength(0);
        }

        public event Action BackButton_MouseDown;
        private void BackButGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BackButton_MouseDown?.Invoke();
        }

        public bool IsChatsAreEqual(TelegramLib.MainClasses.UserChat chat)
        {
            return _chat is null ? false : _chat.Id == chat.Id;
        }

        public void UpdateChatterName(UserContactcs contact)
        {
            if (_chat.Chatter.Id != contact.ContactUserId) return;

            ChatFriendLogin.Text = contact.Name;
            ChatFriendSurname.Text = contact.Surname;
        }

        public void SetNameSurnameInUserParams()
        {
            ChatFriendLogin.Text = _chat.Chatter.Name;
            ChatFriendSurname.Text = _chat.Chatter.Surname;
        }

        private void UnblockGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            UnblockGrid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        private void UnblockGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            UnblockGrid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
        }

        private void UnblockGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ApiService.RemoveBlockedContact(_system.LoggedUser.Id, _chat.Chatter.Id);

            _system.LoggedUser.UnblockUserById(_chat.Chatter.Id);
            UnBlockBorder.Visibility = Visibility.Hidden;

            //Update Chat contact info
            UpdateContactInfoBlock();
        }

        public void RemoveContactAction()
        {
            ChatFriendLogin.Text = _chat.Chatter.Name;
            ChatFriendSurname.Text = _chat.Chatter.Name;
        }

        private void ClearStickerPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            EmojisBoard.Visibility = Visibility.Hidden;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            EmojisBoard.Visibility = Visibility.Hidden;
        }

        public async Task AddBigMediaImagesMessage(string capture, List<Image> imgs)
        {
            if (!string.IsNullOrEmpty(capture))
            {
                AddTextMessageControl(_system.LoggedUser.GetFirstImageName().Name, capture);
            }

            foreach (var img in imgs)
            {
                string fullPath = img.Tag.ToString();
                bool isAdd = await AddMediaPath(fullPath);

                if (isAdd)
                {
                    MediaAction toCheck = (MediaAction)_chatMessages.Last();
                    AddImageMessage(fullPath, false,
                        _system.LoggedUser.GetFirstImageName().Name, toCheck);
                }
            }

            UpdateContactInfoBlock();
        }

        //The True one
        public void ShareContact(TelegramLib.MainClasses.User sharedContact,
            string sharedName, Message mes)
        {
            //Set send message control
            ShareContactControl shareContact =
                new ShareContactControl();

            //Set control params
            shareContact.SetSenderImage(_system.LoggedUser.GetFirstImageNameInString());
            shareContact.SetSharedUserImage(sharedContact.GetFirstImageNameInString());
            shareContact.SetSharedUserName(sharedName);
            shareContact.SetSharedUserPhoneNumber(sharedContact.PhoneNumber);
            shareContact.SetSendTime();
            shareContact.Tag = sharedContact.Id;

            //Add In DB 
            //ApiService.AddMessage

            //Add in system
            //AddSharedMessageInSystem(sharedContact);

            //Events etc...
            shareContact.SharedClicked += async () =>
            {
                int.TryParse(shareContact.Tag.ToString(), out int tagId);
                TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(tagId);

                //If chat is not found -> add it
                if (chat is null)
                {
                    await AddChat(sharedContact);

                    chat = await ApiService.GetChatByUserAndSenderId(_system.LoggedUser.Id, sharedContact.Id);
                    //chat = _system.GetChatByChatterId(tagId);

                    //Add chat in system
                    _system.AddChat(chat);
                }

                if (chat is null) return;

                //Set another chat with this 
                SetUserChat(chat);

                //Set Chosen chat
                ((MainWindow)Window.GetWindow(this)).SetChosenChat(chat);
                _system.ChosenChatContact = chat.Chatter;
            };

            //Change Chat

            ListBoxItem item = new ListBoxItem()
            {
                Content = shareContact,
                Tag = mes.Id,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            item.PreviewMouseRightButtonDown += SetMessageMenu_PreviewRightMouseDown;


            //SetMessagePositionSettings(item);

            //Set user tick icon
            if (mes.SenderUserId == _system.LoggedUser.Id)
            {
                string tickVis = mes.IsRead ? _readIconName : _unreadIconName;
                shareContact.SetTickVis(tickVis);
            }

            //Add it in chat
            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);
        }

        public void AddSharedMessageInSystem(UserContactcs contact)
        {
            _system.AddShareMessage(contact);
        }

        public async Task SetReadMessageAction()
        {
            //Tag => message Id for every message Control
            if (_chat is null) return;

            //Not every chat messages should marked as read
            //only that that is are in visible borders of ListBox
            List<Message> messages =
                    _chat.GetMessageByGivenIds(GetIdsByVisibleElems());

            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].SenderUserId != _system.LoggedUser.Id &&
                    !messages[i].IsRead)
                {
                    messages[i].IsRead = true;
                    await ApiService.ReadMessage(messages[i].Id);

                    //Update read amount
                    UpdateUserTalkReadAmount();
                }
            }

            //Send signalR To set message as read
            //var objsInView = Helper.VisHelper.GetVisibleItems(ChatBox);

            await SetChatterMessagesReadStatus();
        }

        public void UpdateUserTalkReadAmount()
        {
            Window window = Window.GetWindow(this);
            if (window is null || window is not MainWindow main) return;

            if (_chat is null) return;
            main.UpdateReadCountOfReadMessages(_chat.Id);
        }

        public async Task SetChatterMessagesReadStatus()
        {
            //Is online
            if (await ApiService.IsUserOnline(_chat.Chatter.Id))
            {
                await SignalRService.UpdateReadStatusMethod(_system.LoggedUser, _chat.Chatter);
                return;
            }
            //Set only in db
        }

        public List<int> GetIdsByVisibleElems()
        {
            List<int> res = new List<int>();
            Application.Current.Dispatcher.Invoke(() =>
            {
                IEnumerable<object> elems = VisHelper.GetVisibleItems(ChatBox);
                foreach (var el in elems)
                {
                    if (el is ListBoxItem item)
                    {
                        if (item.Content is ChatControls.TextMessage text)
                        {
                            int.TryParse(item.Tag.ToString(), out int id);
                            res.Add(id);
                        }
                        else if (item.Content is ShareContactControl share)
                        {
                            int.TryParse(item.Tag.ToString(), out int id);
                            res.Add(id);
                        }
                        else if (item.Content is MediaMessage media)
                        {
                            int.TryParse(item.Tag.ToString(), out int id);
                            res.Add(id);
                        }
                    }
                }
            });
            return res;
        }

        public void UpdateReadStatus(TelegramLib.MainClasses.User chatter)
        {
            //COMPARE IN DB by SEND TIME
            //Get chat with chatter
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(chatter.Id);
            if (chat is null) return;


            List<Message> messages =
                chat.GetMessageByGivenIds(GetIdsByVisibleElems());

            if (chat is null) return;
            //Go through it
            //compare every chat in db(by time)
            //change read status in db(if need)
            for (int i = 0; i < messages.Count; i++)
            {
                if (!messages[i].IsRead)
                {
                    ApiService.SetReadStatus(messages[i].Id);

                    //Get and change status from db
                    bool updatedStatus =
                        ApiService.GetMessageReadStatus(messages[i].Id).Result;

                    messages[i].IsRead = updatedStatus;
                }
            }
            UpdateReadStatus(chat);
            //change vis state
        }

        public void UpdateReadStatus(TelegramLib.MainClasses.UserChat chat)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => UpdateReadStatus(chat));
                return;
            }

            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is ListBoxItem item)
                {
                    //make it in one in future (CHECK IF THERE CAN BE OTHER TYPES)
                    if (item.Content is ChatControls.TextMessage text)
                    {
                        //if (text.Tag is null) continue;
                        int.TryParse(item.Tag.ToString(), out int mesId);
                        SetVisualReadIconKind(item,
                            chat.Messages.FirstOrDefault(x => x.Id == mesId));
                    }
                    else if (item.Content is ShareContactControl shareControl)
                    {
                        int.TryParse(item.Tag.ToString(), out int mesId);

                        SetVisualReadIconKind(item,
                            chat.Messages.FirstOrDefault(x => x.Id == mesId));
                    }
                    else if (item.Content is MediaMessage mediaImg)//image (Sticker, image, etc...)
                    {
                        int.TryParse(item.Tag.ToString(), out int mesId);
                        SetVisualReadIconKind(item,
                            chat.Messages.FirstOrDefault(x => x.Id == mesId));
                    }
                    else if (item.Content is MediaElement mediaEl)//video
                    {
                        int.TryParse(item.Tag.ToString(), out int mesId);
                        SetVisualReadIconKind(item,
                            chat.Messages.FirstOrDefault(x => x.Id == mesId));
                    }
                }
            }
        }

        public void SetVisualReadIconKind(ListBoxItem item,
            TelegramLib.MainClasses.Messages.Message? mes)
        {
            if (mes is null ||
                 mes.SenderUserId != _system.LoggedUser.Id) return;

            string tickVis = mes.IsRead ? _readIconName : _unreadIconName;
            if (item.Content is ChatControls.TextMessage textMes)
            {
                textMes.SetVisibility(tickVis);
            }
            else if (item.Content is ShareContactControl shareControl)
            {
                shareControl.SetVisibility(tickVis);
            }
            else if (item.Content is MediaMessage mediaImg)//Image
            {
                mediaImg.SetVisibility(tickVis);
            }
        }

        public void SetTickStatusIfCorrectMes(ListBoxItem item,
            TelegramLib.MainClasses.Messages.Message? mes)
        {
            if (mes is null ||
                mes.SenderUserId != _system.LoggedUser.Id) return;
            SetVisualReadStatusForMessage(item, mes.IsRead);
        }

        private const string _readIconName = "TickAll";
        private const string _unreadIconName = "Tick";
        public void SetVisualReadStatusForMessage(ListBoxItem mes, bool isRead)
        {
            string tickVis = isRead ? _readIconName : _unreadIconName;
            if (mes.Content is ChatControls.TextMessage textMes)
            {
                textMes.SetTickVis(tickVis);
            }
        }

        private void ChatBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Hidden) return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                SetReadMessageAction();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        public void UpdateUserChat(int chatId)
        {
            if (chatId != _chat.Id) return;

            //Update whole chat or just add share message control
        }

        public async Task AddChat(TelegramLib.MainClasses.User toMakeChatWith)
        {
            TelegramLib.MainClasses.UserChat? firstPartChat = await ApiService.GetChatByUserAndSenderId
                (_system.LoggedUser.Id, toMakeChatWith.Id);

            if (firstPartChat is null)
            {
                await ApiService.AddNewChat(_system.LoggedUser.Id, toMakeChatWith.Id);
            }

            TelegramLib.MainClasses.UserChat? otherPartChat = await ApiService.GetChatByUserAndSenderId
                (toMakeChatWith.Id, _system.LoggedUser.Id);

            if (otherPartChat is null)
            {
                await ApiService.AddNewChat(toMakeChatWith.Id, _system.LoggedUser.Id);
            }
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageMenu.Children.Clear();
        }

        private void CloseReplyGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = new SolidColorBrush(Colors.White);
            Cursor = Cursors.Hand;
        }

        private void CloseReplyGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = new SolidColorBrush(Colors.Gray);
            Cursor = null;
        }

        private void CloseReplyGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Close reply action

            ReplyMessageRow.Height = new GridLength(0);
        }

        public void SetReplyMessageRow()
        {
            //Get ListBoxItem From Menu
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();  //GetListBoxItemFromMenu();
            if (item is null || item.Content is not UserControl control) return;

            //item.Tag == message id; Get essage by system
            Message mes = GetMessageByListBoxTag(item);
            if (mes is null) return;

            ReplyMessageRow.Height = new GridLength(50);

            //Set Image to reply
            if (control is MediaMessage media)
            {
                ReplyedImageColumn.Width = new GridLength(50);
                ReplyedImage.Source = media._img.Source; /*new BitmapImage(
                    new Uri(FilesAction.GetUserImagePath(
                        _chat.Chatter.GetFirstImageNameInString()), UriKind.Absolute));
            */
            }
            else
            {
                ReplyedImageColumn.Width = new GridLength(0);
                ReplyedImage.Source = null;
            }

            //Set sender name
            ReplySenderText.Text = $"Reply to {_system.GetMessageSender(mes.SenderUserId).Login}";

            //Set text
            ReplyedMessageText.Text =
                mes is MediaAction ? "Reply media" :
                mes is TelegramLib.MainClasses.Messages.TextMessage text ? text.Text :
                mes is TelegramLib.MainClasses.Messages.ShareContactMessage share ? "Contact" :
                "Some shit";
        }

        public ListBoxItem? GetListBoxItemFromMenu()
        {
            MesMenu menu = MessageMenu.Children.OfType<MesMenu>().FirstOrDefault();
            return menu is null ? null : menu.GetChosenListBoxItem();
        }

        public Message GetMessageByListBoxTag(ListBoxItem item)
        {
            int.TryParse(item.Tag.ToString(), out int id);
            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(id);
            return mes;
        }

        private void PinRowBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void PinRowBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void PinRowBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int.TryParse(PinRowBorder.Tag.ToString(), out int mesId);
            //Get Message
            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);
            if (mes is null) return;

            //Show Pinned message
            ScrollToMessageByMessageId(mes.Id);

            //Get next mes to set
            Message nextMes = _system.GetNextPinnedMessage(mes);

            //Change it to the next
            SetPinnedMessageInPanel(nextMes);
        }

        public async Task SetStopMessageForChatter()
        {
            //string stopSignPath = FilesAction.GetSystemImagePath("StopSign.png");

            //Get is need to set stop img
            IEnumerable<ListBoxItem> items = ChatBox.Items.OfType<ListBoxItem>();

            foreach(var item in items)
            {
                int.TryParse(item.Tag.ToString(), out int itemTag);
                Message? mes = _chat.Messages.FirstOrDefault(x => x.Id == itemTag);
                if (mes is null || mes.SenderUserId == _system.LoggedUser.Id) continue;

                await SetSenderImageByListBoxItem(item);

/*                if(item.Content is ChatControls.TextMessage text)
                {
                    await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter, 
                        text.BgBrush, text.UserEllipseImage);
                    //text.BgBrush.ImageSource = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
                }
                else if(item.Content is ChatControls.MediaMessage media)
                {
                    await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                        media.BgBrush, media.UserEllipseImage);
                    //media.BgBrush.ImageSource = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
                }
                else if(item.Content is ShareContactControl share)
                {
                    await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                        share.BgBrush, share.SenderEllipseImage);

                    await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                        share.ImageIcon, share.UserEllipseImage);

              *//*      share.BgBrush.ImageSource = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
                    share.ImageIcon.ImageSource = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
               *//* }*/
            }
        }

        public async Task SetSenderImageByListBoxItem(ListBoxItem item, bool isChatter = true)
        {
            if (!isChatter) return;

            if (item.Content is ChatControls.TextMessage text)
            {
                await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                    text.BgBrush, text.UserEllipseImage);
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                    media.BgBrush, media.UserEllipseImage);
            }
            else if (item.Content is ShareContactControl share)
            {
                await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                    share.BgBrush, share.SenderEllipseImage);

                await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                    share.ImageIcon, share.UserEllipseImage);
            }
        }


    }
}
