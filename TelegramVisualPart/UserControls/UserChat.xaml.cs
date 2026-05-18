using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TelegramLib.Enums.Chat;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;
using TelegramLib.UserSettings;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Enums.MediaShow;
using TelegramVisualPart.Enums.Menus;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.ChatActions;
using TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion;
using TelegramVisualPart.Pages.ChatActions.MessageMenuPages;
using TelegramVisualPart.Pages.ChatActions.SendMedia;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls;
using TelegramVisualPart.UserControls.ChatControls.ChatButsControls;
using TelegramVisualPart.UserControls.ChatControls.ChatMessages;
using TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu;
using TelegramVisualPart.UserControls.ChatControls.SavedChatControls;
using TelegramVisualPart.Windows;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using ListBoxItem = System.Windows.Controls.ListBoxItem;
using Path = System.IO.Path;
using SavedMessagesChat = TelegramLib.MainClasses.SavedMessagesChat;

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
            try
            {
                InitializeComponent();
                SetMarginForChatMenu();
                SetAutoDeleteTimer();

                SetBasicSignalRMethods();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Init Mistake in UserChat: {ex.Message}");
                throw;
            }
        }

        public void SetBasicSignalRMethods()
        {
            /*if (SignalRService.GetIsChatEventsAreSet()) return;
                  SignalRService.ChangeIsChatEventsAreSet(true);*/
            SignalRService.TextMessageReceived += OnTextMessageReceived;
            SignalRService.MediaMessageReceived += OnMediaMessageReceived;
            SignalRService.UpdateOnlineStatusDel += UpdateOnlineStatus;
            SignalRService.UpdateUserImage += UpdateUserImage;

            SignalRService.ClearChatDel += ClearChatAction; //Check withi first sent message late

            SignalRService.SetContactLastSeenVisStateDel += SetLastVisState;
            SignalRService.UpdateContactPhotoDel += UpdateChatterImage;
            SignalRService.UpdateForwardStatusDel += UpdateForwardStatusDel;

            SignalRService.DeleteMessageByIdDel += RemoveMessageById;
            SignalRService.RemoveManyMessagesDel += RemoveManyMessages;

            SignalRService.ToPinMessageDel += PinMessage;
            SignalRService.StatMessageReceived += SetStatMessageInFromSignalR;

            SignalRService.EditMessageDel += EditMessageSignlR;
            SignalRService.SendTypingActionDel += SetTypingAction;
        }

        public void ClearBasicSignalRMethods()
        {
            SignalRService.TextMessageReceived -= OnTextMessageReceived;
            SignalRService.MediaMessageReceived -= OnMediaMessageReceived;
            SignalRService.UpdateOnlineStatusDel -= UpdateOnlineStatus;
            SignalRService.UpdateUserImage -= UpdateUserImage;
            SignalRService.ClearChatDel -= ClearChatAction;
            SignalRService.SetContactLastSeenVisStateDel -= SetLastVisState;
            SignalRService.UpdateContactPhotoDel -= UpdateChatterImage;
            SignalRService.UpdateForwardStatusDel -= UpdateForwardStatusDel;
            SignalRService.DeleteMessageByIdDel -= RemoveMessageById;
            SignalRService.RemoveManyMessagesDel -= RemoveManyMessages;
            SignalRService.ToPinMessageDel -= PinMessage;
            SignalRService.StatMessageReceived -= SetStatMessageInFromSignalR;
            SignalRService.EditMessageDel -= EditMessageSignlR;
            SignalRService.SendTypingActionDel -= SetTypingAction;
        }

        //Tactic for differ window chats

        //Problem
        //Boss window and only chat window gets message
        //and both adding it to db etc

        //Result
        //if only chat window is exist(with chatter => return this)

        public bool IsOnlyChatWindowWithChatIsExist(TelegramLib.MainClasses.UserChat chat)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                Window window = Window.GetWindow(this);
                if (window is MainWindow mainWind)
                {
                    return mainWind.IsOnlyTempOnlyChatIsExist(chat);
                }
                return false;
            });
        }

        private CancellationTokenSource _typingCts;
        public async void SetTypingAction(
            TelegramLib.MainClasses.User toSetTyping)
        {
            if (_chat is null || _chat.GetChatter() is null ||
                _chat.GetChatter().Id != toSetTyping.Id) return;

            const string typeStr = "typing...";

            //Set typing stuff

            _typingCts?.Cancel();
            _typingCts = new CancellationTokenSource();
            var token = _typingCts.Token;

            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatFriendLastSeen.Text = typeStr;
            });

            try
            {
                //await Task.Delay(1000, token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ChatFriendLastSeen.Text != typeStr) return;
                    ChatFriendLastSeen.Text = "online";
                });
            }
            catch (TaskCanceledException)
            {
            }
        }

        public async Task EditMessageSignlR(TelegramLib.MainClasses.User user,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            if (mes is null || user is null) return;
            TelegramLib.MainClasses.Messages.Message? pairMes = await ApiService.GetPairOfMessage(mes);
            if (pairMes is null) return;

            TelegramLib.MainClasses.UserChat chat = _system.GetChatByMessage(pairMes);
            if (chat is null) return;

            ChangeEditedParams(pairMes, mes);

            await ToEditMessage(chat.Id, false, pairMes);
        }

        public void ChangeEditedParams(Message mes, Message edited)
        {
            if (edited is TelegramLib.MainClasses.Messages.TextMessage editedText &&
               mes is TelegramLib.MainClasses.Messages.TextMessage mesText)
            {
                mesText.Text = editedText.Text;
            }
        }

        public async Task SetStatMessageInFromSignalR(TelegramLib.MainClasses.User chatter,
            TelegramLib.MainClasses.Messages.StaticMessage mes)
        {
            if (mes.MessageReferenceId is null && mes.DelType is null && mes.Date is null) return;

            //Get referenced message
            TelegramLib.MainClasses.UserChat? chatterChat =
                await ApiService.GetChatByUserAndSenderId(chatter.Id, _system.LoggedUser.Id);
            if (chatterChat is null) return;

            TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(chatter.Id);
            if (chat is null) return;
            if (_chat is null) _chat = chat;

            if (mes.DelType is not null)
            {
                //Set auto del
                //_chat = _system.GetChatByChatterId(chatter.Id);

                UserContactcs contact = _system.GetContactByUserId(chatter.Id /*_chat.GetChatter().Id*/);
                if (contact is not null) contact.AutoDeletion = new AutoDeleteDuration((TelegramLib.Enums.Chat.AutoDeleteType)mes.DelType);

                chat.AutoDel = (TelegramLib.Enums.Chat.AutoDeleteType)mes.DelType;

                //Update chat state in db
                await ApiService.UpdateChat(chat);

                if (_chat.Id == chat.Id)
                {
                    SetNewAutoDelIconVisibility();
                }
                //Update chat little vis
                //_mainWindow.UpdateAutoDelVis(chat);
            }
            else if (mes.MessageReferenceId is not null)
            {
                TelegramLib.MainClasses.Messages.Message refedMessage =
                chatterChat.GetMessageById((int)mes.MessageReferenceId);
                if (refedMessage is null) return;

                //Get chat

                //Get Reference message in own chat
                int refId = chat.GetMesIdPairOfMessageByTime(refedMessage);
                mes.MessageReferenceId = refId;
            }
            else if (mes.Date is not null)
            {
                mes = (StaticMessage)DeepCopy(mes);
            }

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                //Adding In Db + logic + i vis
                await AddStatMessage(mes, false, chat);
            });

        }

        public void PinMessage(TelegramLib.MainClasses.User chatter,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            //Try to get pair of the message
            TelegramLib.MainClasses.Messages.Message pair =
                ApiService.GetPairOfMessage(mes).Result;
            if (pair is null) return;

            //Get listBoxItem
            Dispatcher.Invoke(() =>
            {
                //Check from chat
                TelegramLib.MainClasses.UserChat chat =
                _system.GetChatByMessage(pair);

                if (chat is null) return;
                if (IsOnlyChatWindowWithChatIsExist(chat)) return;

                //remove from vis
                ListBoxItem? item = ChatBox.Items
                .OfType<ListBoxItem>().FirstOrDefault(x => x.Tag.ToString() == pair.Id.ToString());
                //if (item is null) return;

                //Get  message with same id
                pair = _system.GetMessageById(pair.Id);

                //bool isBlocked = _system.IsChatterBlocked(chatter);
                SetPinAction(pair, item, false,
                    isChatterBlocked: false);

                if (IsOnlyPinnedChatIsOn()) SetChatMessages(true);

                ToUpdateUserControlMessage();
            });
        }

        public void RemoveManyMessages(List<DateTime> sentTimes, int chatterId)
        {
            TelegramLib.MainClasses.UserChat tempChat = _chat;
            _chat = _system.GetChatByChatterId(chatterId);
            if (_chat is null)
            {
                _chat = tempChat;
                return;
            }
            for (int i = 0; i < sentTimes.Count; i++)
            {
                _chat.RemoveMessageBySentTime(sentTimes[i]);
            }

            Dispatcher.Invoke(async () =>
            {
                List<Message> selected = GetSelectedMessages();

                await RemoveDateStateIfNoMesOnDate();

                //Update vis 
                bool isOnlyPinnedChat = IsOnlyPinnedChatIsOn();
                if (isOnlyPinnedChat) IsOnlyPinnedChatPinAction();
                else
                {
                    if (SavedMessagesGrid.Visibility != Visibility.Visible &&
                        SchedueleMessagesGrid.Visibility != Visibility.Visible &&
                        tempChat is not null && _chat is not null && tempChat.Id == _chat.Id)
                    {
                        await SetChatMessages();
                        HideSelectionRowFromSignalR(selected);
                    }
                }

                _chat = tempChat;
                _mainWindow.UpdateChatControls();

                UpdateGlobalMedias();
            });

        }

        public async void RemoveMessageById(TelegramLib.MainClasses.User chatter,
            Message mes, bool isUpdateVis)
        {
            //Get Pair message from mes to delete
            Message? pair = await ApiService.GetPairOfMessage(mes);

            if (pair is null) return;

            TelegramLib.MainClasses.UserChat tempChat = _chat;

            _chat = _system.GetChatByMessage(pair);
            if (IsOnlyChatWindowWithChatIsExist(_chat))
            {
                _chat = tempChat;
                return;
            }
            RemoveMessageFromSigR(pair);

            await Dispatcher.InvokeAsync(async () =>
            {
                if (!isUpdateVis || _chat is null)
                {
                    _chat = tempChat;
                    return;
                }

                List<Message> selected = GetSelectedMessages();

                if (SavedMessagesGrid.Visibility != Visibility.Visible &&
                    SchedueleMessagesGrid.Visibility != Visibility.Visible &&
                    tempChat is not null && _chat is not null && tempChat.Id == _chat.Id)
                {
                    await SetChatMessages();
                    HideSelectionRowFromSignalR(selected);
                }

                //Update UserTalkMessage(Chat) - if last message was removed

                _chat = tempChat;
                _mainWindow.UpdateChatControls();

                UpdateGlobalMedias();
            });
        }

        public async void RemoveMessageFromSigR(TelegramLib.MainClasses.Messages.Message mes)
        {
            //remove from system
            _system.RemoveMessageById(mes.Id);

            //Remove from db
            await ApiService.DeleteMessageById(mes.Id);

            //Is Need to remove date Message

            if (_chat is null || _chat.Messages.Count() == 0) return;
            TelegramLib.MainClasses.Messages.Message isDate = _chat.Messages.Last();

            if (isDate is not StaticMessage stat || stat.Date is null) return;
            _chat.Messages.Remove(stat);
            await ApiService.DeleteMessageById(stat.Id);

            await RemoveDateStateIfNoMesOnDate();
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

        public void UpdateChatterImage(TelegramLib.MainClasses.User user)
        {
            UpdateChatImages(user);
        }

        public void UpdateForwardStatusDel(TelegramLib.MainClasses.User user)
        {
            if (_chat is null) return;
            TelegramLib.MainClasses.UserChat setChat =
                _system.GetChatByUserId(user.Id);

            if (setChat is null) return;

            //setChat.FOrw
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

        public void ClearChatAction(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                HideSelectionRowFromSignalR(new List<Message>());
                TelegramLib.MainClasses.UserChat? chat = _system.Chats.FirstOrDefault(x => x.Chatter.Id == user.Id);
                if (chat is null) return;

                if (_chat is null) SetChatById(chat.Id);

                if (_chat is null) return;
                await RemoveAllMessagesFromChat(chat);

                UpdateTalkMessageTickStatus(chat);

                UpdateGlobalMedias();
            });
        }

        public async Task RemoveAllMessagesFromChat(TelegramLib.MainClasses.UserChat chat)
        {
            ((MainWindow)Window.GetWindow(this)).ClearAllChatWindows();

            //Is temp is Chosen -> clear vis
            if (_chat.Id == chat.Id) ClearChat();
            //Clear from system

            _system.RemoveAllMessagesFromChat(chat);
            chat.ClearChat();
            //Clear from Db
            await ApiService.ClearChat(chat);
        }

        public void SetChatById(int id)
        {
            _chat = _system.GetChatById(id);
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
                    if (/*chat.Messages[i].SenderUserId != user.Id ||
                   */ ChatBox.Items[i] is not ListBoxItem item) continue;
                    if (item.Tag is null) continue;

                    int.TryParse(item.Tag.ToString(), out int mesId);
                    TelegramLib.MainClasses.Messages.Message mes =
                    chat.GetMessageById(mesId);
                    if (mes is null || mes.SenderUserId == _system.LoggedUser.Id)
                    {
                        continue;
                    }

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
                    else if (item.Content is ShareContactControl share)
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
            List<TelegramLib.MainClasses.Messages.MediaAction> messages)
        {
            Dispatcher.Invoke(async Task () =>
            {
                RemoveRightContactInfo();
                List<Message> selected = GetSelectedMessages();

                bool isBandUpdate = false;
                TelegramLib.MainClasses.UserChat? chat = null;
                for (int i = 0; i < messages.Count; i++)
                {
                    MediaAction message = messages[i];
                    //Get chat where Logged is Sender 
                    chat = await GetChatByUserSendersIds(_system.LoggedUser.Id, sender.Id);

                    //is Chatter is blocked
                    if (chat is null || chat.Chatter is null ||
                        !_system.IsAllowedToWriteMessages(chat.Chatter) ||
                        _system.IsUserBlockedForMesSend(chat.Chatter))
                    {
                        return;
                    }


                    //_system.GetChatByChatterId(sender.Id);
                    if (chat is null) return;

                    SetNewUserChatMessageControl(chat);
                    if (IsOnlyChatWindowWithChatIsExist(chat)) return;

                    //Add media message in chat in db
                    //Task.Run(() => ApiService.AddMessage(message, chat)).Wait();

                    await ApiService.AddMessage(message, chat);

                    //There is no pair yet
                    message = GetPairOfMedia(message);

                    if (_chat is null || chat.Id != _chat.Id)
                    {
                        AddMediaMessageInUnChosenChat(chat, message);
                        ((MainWindow)Window.GetWindow(this)).UpdateReadCountOfReadMessages(chat.Id);
                    }
                    else
                    {
                        isBandUpdate = true;
                        AddMediaMessageInChosenChat(message, sender);
                    }
                    ToUpdateUserControlMessage();
                }

                if (SchedueleMessagesGrid.Visibility == Visibility.Visible) return;

                if (isBandUpdate && chat is not null && _chat is not null && chat.Id == _chat.Id)
                {
                    await SetChatMessages();
                }

                HideSelectionRowFromSignalR(selected);

                ((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();

                UpdateGlobalMedias();
            });
        }

        public MediaAction? GetPairOfMedia(MediaAction message)
        {
            //Get pair of this from db
            TelegramLib.MainClasses.Messages.Message? mes =
               Task.Run(() => ApiService.GetPairOfMessage(message)).Result;

            return mes is null ? null :
                mes is MediaAction media ? media : null;
        }

        private void AddMediaMessageInChosenChat(MediaAction message, TelegramLib.MainClasses.User sender)
        {
            //Add media in vis
            if (!IsOnlyPinnedChatIsOn())
            {
                /*  SetMediaMessageInChat(message,
                     await SignalRHelperService.GetUserPhotoToSet(sender) *//*sender.GetFirstImageNameInString()*//*);*/
            }
            //Add in system
            _chat.Messages.Add(message);

            //add in db
            //await ApiService.AddMessage(message, _chat);
            ToUpdateUserControlMessage();

            ScrollToNewMessage();
        }

        private async void AddMediaMessageInUnChosenChat(TelegramLib.MainClasses.UserChat chat, MediaAction message)
        {
            //Add in system 
            chat.Messages.Add(message);

            //Add in db
            //await ApiService.AddMessage(message, chat);

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
                        TelegramLib.MainClasses.Messages.Message? replied = await ApiService.GetPairOfMessage(toGetPair);
                        if (replied is not null) message.RepliedMessageId = replied.Id;
                    }
                }

                TelegramLib.MainClasses.UserChat? chat =
                    await GetChatByUserSendersIds(_system.LoggedUser.Id, sender.Id);

                //is Chatter is blocked
                if (chat is null || chat.Chatter is null ||
                    !_system.IsAllowedToWriteMessages(chat.Chatter) ||
                    _system.IsUserBlockedForMesSend(chat.Chatter)) return;

                //Set user talk if not contains un chats
                SetNewUserChatMessageControl(chat);
                if (IsOnlyChatWindowWithChatIsExist(chat)) return;


                if (_chat is null ||
                    chat.Id != _chat.Id)
                {
                    await AddTextMessageInUnChosenChat(chat, message);
                    ((MainWindow)Window.GetWindow(this)).UpdateReadCountOfReadMessages(chat.Id);
                }
                else await AddTextMessageInChosenChat(message, sender, chat);

                //SetOnlyChat(chat);

                //Is temp chat is chosen
                ToUpdateUserControlMessage();

                HideSelectionRowFromSignalR(GetSelectedMessages());
            });
        }

        public void HideSelectionRowFromSignalR(List<Message> selected)
        {
            //Is selection row is visible
            if (SelectedMessesGrid.Visibility == Visibility.Hidden) return;

            //If Selection row is visible => need to update it
            //1 - update on top(amount of selected)
            //2 - If amount of selected == 0 => clear selection action
            //3 - else - keep going

            int counter = 0;
            for (int i = 0; i < selected.Count; i++)
            {
                if (_system.GetMessageById(selected[i].Id) is not null) counter++;
            }

            if (selected.Count == 0)
            {
                HideSelectionRow();
                return;
            }

            SetTickVisForChat(true, selected);

            //Set amount of selected messages
            UpdateTickedAmount(counter);

            //UpdateSelectedAmount();
        }

        public void SetTickVisForChat(bool isVis, List<Message> selected)
        {
            //SetMessageSelectByOnlyTickCol(false);
            SetMessageSelectCircleVis(false);

            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;

                int? id = null;
                if (item.Tag is not null)
                {
                    id = int.Parse(item.Tag.ToString());
                }

                if (item.Content is ChatControls.TextMessage text)
                {
                    if (id is not null && selected.Any(x => x.Id == id))
                    {
                        text.SelectionTickObj.SetChosenParam(true);
                    }
                    text.SetTickVisibility(isVis);
                }
                else if (item.Content is ChatControls.MediaMessage media)
                {
                    if (media.IsBandMedia() && id is null)
                    {
                        List<MediaAction> bandMedias = selected.OfType<MediaAction>().Where(x => x.BandId != -1).ToList();

                        for (int j = 0; j < bandMedias.Count; j++)
                        {
                            media.SetChoseVisById(bandMedias[j].Id, Visibility.Visible);
                        }

                        if (media.IsAllMediasInBandAreChosen())
                        {
                            media.SelectionTickObj.SetChosenParam(true);
                        }
                    }
                    if (id is not null && selected.Any(x => x.Id == id))
                    {
                        media.SelectionTickObj.SetChosenParam(true);
                    }
                    media.SetTickVisibility(isVis);
                }
                else if (item.Content is ShareContactControl share)
                {
                    if (id is not null && selected.Any(x => x.Id == id))
                    {
                        share.SelectionTickObj.SetChosenParam(true);
                    }
                    share.SetTickVisibility(isVis);
                }
            }
        }

        public void SetOnlyChat(TelegramLib.MainClasses.UserChat chat)
        {
            TelegramLib.MainClasses.UserChat tempOnlyChat =
                ((MainWindow)Window.GetWindow(this))._onlyChatUserChat;

            if (chat is null || tempOnlyChat is null) return;

            if (chat.Id == tempOnlyChat.Id &&
                chat.GetType() == tempOnlyChat.GetType())
            {
                tempOnlyChat = chat;
            }
        }

        public void SetNewUserChatMessageControl(TelegramLib.MainClasses.UserChat chat)
        {
            if (_system.IsChatContainsInChats(chat.Id)) return;

            //Add chat + Update User talk controls(Chats)
            _system.AddChat(chat);
            ((MainWindow)Window.GetWindow(this)).UpdateChatControls();
        }

        private async Task AddTextMessageInChosenChat(
            TelegramLib.MainClasses.Messages.TextMessage message,
            TelegramLib.MainClasses.User sender,
            TelegramLib.MainClasses.UserChat chat)
        {
            TelegramLib.MainClasses.Messages.Message replied =
                _system.GetMessageById(message.RepliedMessageId);

            ChatControls.TextMessage text = new ChatControls.TextMessage(_system,
                GetConvertedStringMessage(message.Text),
                /*sender.GetFirstImageNameInString()*/ await SignalRHelperService.GetUserPhotoToSet(sender),
                _system.Settings.GetChatSettings().FontName,
                message.IsEdited,
                message,
                toReply: replied, forwardedFrom: message.ForwardedFromId);

            //

            if (replied is not null &&
                replied is TelegramLib.MainClasses.Messages.TextMessage textMes)
            {
                text.ReplyControl.ReplyedMessage.Text = textMes.Text;
            }

            await ApiService.AddMessage(message, chat);

            message =
                (TelegramLib.MainClasses.Messages.TextMessage)await ApiService.GetLastChatMessage(_chat.Id);
            chat.Messages.Add(message);

            if (SchedueleMessagesGrid.Visibility == Visibility.Visible) return;

            if (!IsOnlyPinnedChatIsOn()) AddTextControl(text, message);

            ScrollToNewMessage();
            ToUpdateUserControlMessage();
        }

        public void ScrollToNewMessage()
        {
            //Console.WriteLine();

            Dispatcher.BeginInvoke(new Action(() =>
            {           
                ChatBox.UpdateLayout();

                ScrollViewer sv = HelperService.GetScrollViewer(ChatBox);
                if (sv == null) return;

                double maxOffset = sv.ExtentHeight - sv.ViewportHeight;

                const int minScrollOffset = 0;
                const int maxPercent = 100;

                double percent = maxOffset <= 0
                    ? maxPercent
                    : (sv.VerticalOffset / maxOffset) * 100;

                if (ChatBox.Items.Count == 0) return;

                var lastItem = ChatBox.Items[ChatBox.Items.Count - 1];
                ChatBox.ScrollIntoView(lastItem);

                sv?.ScrollToBottom();

            }), DispatcherPriority.Loaded);
        }

        public void ToUpdateUserControlMessage()
        {
            Window window = Window.GetWindow(this);
            if (window is not MainWindow mainWindow) return;

            mainWindow.UpdateUserChatTalkControl();
        }

        public void AddTextControl(ChatControls.TextMessage text,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            //if (IsOnlyPinnedChatIsOn()) return;

            ListBoxItem item = new ListBoxItem()
            {
                Content = text,
                Tag = mes.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, mes);

            //SetMessagePositionSettings(item);

            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);
        }

        private async Task AddTextMessageInUnChosenChat(TelegramLib.MainClasses.UserChat chat,
            TelegramLib.MainClasses.Messages.TextMessage message)
        {
            //chat.Messages.Add(message);
            //Add in db
            await ApiService.AddMessage(message, chat);

            //Add in system 
            chat.Messages.Add(await ApiService.GetLastChatMessage(chat.Id));

            ToUpdateUserControlMessage();
        }

        public TelegramLib.MainClasses.UserChat _chat;
        public event Action SettingEnded;

        private bool _isSavedMessageChat;
        public async Task SetUserChat(TelegramLib.MainClasses.UserChat chat)
        {
            if (_isHiddenSender) _isHiddenSender = false;

            if (chat is null) return;
            SetAddMediaButVisibility();
            SendMesMenu.SetUserChatControl(this, _system);

            HideSelectionRow();
            SetIsSavedMessagesChat(chat);

            SetSavedMessagesChatVisibility();

            HideOnlyPinnedBorders();

            MessageMenu.Children.Clear();
            ReplyMessageRow.Height = new GridLength(0);
            SchedueleMessagesGrid.Visibility = Visibility.Hidden;

            if (chat is null) return;
            SetChatterImageVisibility();

            _chat = chat;
            SetNewAutoDelIconVisibility();
            SetScheduleMessageIconVisibility();

            SetPinnedMessages();

            SetUnblockGridVis();

            await SetOnlineStatus();

            UserChatMenu.SetChatParam(_chat);

            //ClearChat();

            SetChatParams(_chat.GetChatter());
            await SetChatMessages();

            RemoveRightContactInfo();
            SetUserBg();

            SetLittleChatterImage();

            SetChatterImage();

            SettingEnded?.Invoke();

            ScrollToNewMessage();
        }

        public async void SetLittleChatterImage()
        {
            if (_chat is null || _chat.Chatter is null) return;

            await SignalRHelperService.SetPhotoInEllipse(_chat.Chatter,
                 UserImage, LittlePhotoEllipse);
        }

        public void SetIsSavedMessagesChat(TelegramLib.MainClasses.UserChat chat)
        {
            _isSavedMessageChat = chat is SavedMessagesChat;
            _system.SetIsSavedMesChatStatus(_isSavedMessageChat);
        }

        public void SetSavedMessagesChatVisibility()
        {
            if (_isSavedMessageChat)
            {
                SetSavedMessagesChatView();
                return;
            }

            HideSavedChatVisibility();
        }

        public void HideSavedChatVisibility()
        {
            SavedMessagesGrid.Visibility = Visibility.Hidden;
        }

        public void SetSavedMessagesChatView()
        {
            SavedMessagesGrid.Visibility = Visibility.Visible;
        }

        private void SavedMessagesGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isSavedMessageChat) return;
            //Set the saved messages menu

            SavedMessagesChatPage page = new SavedMessagesChatPage(_system.SavedMesesChat);
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public void SetNewAutoDelIconVisibility()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AutoDelGrid.Visibility =
                    _chat.AutoDel == TelegramLib.Enums.Chat.AutoDeleteType.Nothing
                    ? Visibility.Hidden
                    : Visibility.Visible;
            });
        }

        public void SetScheduleMessageIconVisibility()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                const int _autoSchedGridWidth = 40;
                if (_chat.ScheduleMessages is null) return;

                ScheduleMessageGrid.Visibility =
                    _chat.ScheduleMessages.Count == 0
                    ? Visibility.Hidden
                    : Visibility.Visible;

                ScheduleMessageGrid.Width =
                ScheduleMessageGrid.Visibility == Visibility.Visible ?
                _autoSchedGridWidth : 0;

                int newLeftMargin =
                ScheduleMessageGrid.Visibility == Visibility.Visible ?
                10 : 0;
                ScheduleMessageGrid.Margin = new Thickness(0, 0, newLeftMargin, 0);

                SetAddMediaButVisibility();
            });
        }

        public void SetSystem(TelSystem system)
        {
            _system = system;
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

        public void ScheduleMessageGrid_PreviewMouseLeftButtonDown(
            object sender, MouseButtonEventArgs e)
        {
            if (SchedueleMessagesGrid.Visibility == Visibility.Visible) return;

            SetScheduleMessages();
        }

        public void SetScheduleMessages()
        {
            if (_chat is null ||
                _chat.ScheduleMessages is null ||
                _chat.ScheduleMessages.Count == 0) return;

            HideControlsToSetSchedMessages();

            _chat.UpdateScheduleMessages(_system.LoggedUser);

            SchedueleMessagesGrid.Visibility = Visibility.Visible;
            SetAddMediaButVisibility();
            SetChatMessages(isOnlySchedule: true);
        }

        public void HideControlsToSetSchedMessages()
        {
            HideSelectionRow();
        }

        public void UpdateScheduleChatIfNeed()
        {
            if (SchedueleMessagesGrid.Visibility != Visibility.Visible) return;
            SetScheduleMessages();
        }

        public bool IsChoseChatIdIsEqual(int id)
        {
            if (_chat is null) return false;
            return _chat.Id == id;
        }

        public void SetUnblockGridVis()
        {
            if (_chat is null || _system is null || _chat.Chatter is null) return;

            bool isBlocked =
                _system.LoggedUser.IsUserIsBlockedById(_chat.Chatter.Id);

            UnBlockBorder.Visibility = isBlocked ? Visibility.Visible : Visibility.Hidden;

            //Set Vis param UnblockGrid_PreviewMouseDown
        }

        public void SetChatterImage()
        {
            if (_chat.Chatter is null) return;
            string imgName = _chat.Chatter.GetFirstImageNameInString();

            UserImage.ImageSource = ApiService.GetCachedBitmap(imgName);

            /*            UserImage.ImageSource = new BitmapImage(
                            new Uri(FilesAction.GetUserImagePath(
                                _chat.Chatter.GetFirstImageNameInString()), UriKind.Absolute));*/
        }

        public async Task SetOnlineStatus()
        {
            if (_chat is null || _chat.Chatter is null) return;
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

        public async Task SetChatMessages(
            bool isOnlyPinned = false,
            bool isOnlySchedule = false)
        {
            //Get Chatter here (Contact type)
            if (isOnlySchedule)
            {
                _chatMessages = _chat.GetScheduleMessages();
                SetSchedMessagesMessagesSentEvent(_chatMessages);
            }
            else if (isOnlyPinned) _chatMessages = _chat.GetOnlyPinnedMessages();
            else _chatMessages = _chat.GetChatMessages();
            await SetMessagesInChat();
        }

        public void SetSchedMessagesMessagesSentEvent(List<Message> messages)
        {
            foreach (var mes in messages)
            {
                mes.StartTimer();
                mes.SentTimeIsNow += () =>
                {
                    ClearMenusAfterMessageReschedule();
                    mes.EndTimer();
                };
            }
        }

        public bool IfSchedTimerActionCanByInvoked(Message mes)
        {

            return false;
        }

        private string _lastSeenDefault = "recently";
        public void SetChatParams(TelegramLib.MainClasses.User contact)
        {
            if (contact is null) return;
            UserContactcs? cont = _system.Contacts.FirstOrDefault(x => x.ContactUserId == contact.Id);
            ChatFriendLogin.Text = cont is null ? contact.Name : cont.Name;
            ChatFriendSurname.Text = cont is null ? contact.Surname : cont.Surname;
        }

        public void ClearChat()
        {
            ChatBox.Items.Clear();
            PinRow.Height = new GridLength(0);
        }

        private TelSystem _system;
        private MainWindow _mainWindow;
        public void SetSystemAndMainWindowParam(TelSystem system, MainWindow window)
        {
            //Set here chat messages(by ref)
            _system = system;
            _mainWindow = window;


            UserChatMenu.SetSystemParam(system);
            //SetTestChatMessages();

            EmojisBoard.SetSystem(_system);

            SetLanguageText.SetUserChat(this);

        }


        private MainSettings _chatterSettings;
        private TelegramLib.MainClasses.User _chatter;
        public async Task SetMessagesInChat()
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                ChatBox.Items.Clear();
                CommentTextBox.Text = string.Empty;
                _isEdit = false;

                List<int> bandBankIds = new List<int>();

                //Update chatter CachedUser + his MainSettings 
                if (_chat is not null && _chat.Chatter is not null) await ApiService.UpdateChachedUserAndSettings(_chat.Chatter.Id);

                for (int i = 0; i < _chatMessages.Count; i++)
                {
                    if (_chatMessages[i] is MediaAction asd && asd.IsSticker)
                    {
                        Console.WriteLine();
                    }

                    if (_chatMessages[i] is MediaAction mediaAct &&
                        mediaAct.BandId != -1 && !bandBankIds.Contains(mediaAct.BandId))
                    {
                        List<MediaAction> mediaBand = _chatMessages.OfType<MediaAction>().Where(x => x.BandId == mediaAct.BandId).ToList();
                        await SetBandMessage(mediaBand);

                        foreach (MediaAction media in mediaBand) SetPinnedInSettingChatMessages(media);

                        bandBankIds.Add(mediaAct.BandId);
                        continue;
                    }
                    else if (_chatMessages[i] is MediaAction media && bandBankIds.Contains(media.BandId) &&
                    media.BandId != -1)
                    {
                        continue;
                    }

                    if (_chat is null || _chatMessages.Count <= i || _chatMessages[i] is null)
                    {
                        return;
                    }
                    string imgName = _chatMessages[i].SenderUserId == _system.LoggedUser.Id || _isSavedMessageChat ?
                            _system.LoggedUser.GetFirstImageName().Name :
                            _chat.GetChatter().GetFirstImageName().Name;

                    if (_chatMessages[i] is TelegramLib.MainClasses.Messages.TextMessage text)
                    {
                        SetTextMessageInChat(text, imgName);
                        //text
                    }
                    else if (_chatMessages[i] is MediaAction media)
                    {
                        //Video or photo
                        await SetMediaMessageInChat(media, imgName);
                    }
                    else if (_chatMessages[i] is
                        TelegramLib.MainClasses.Messages.ShareContactMessage share)
                    {
                        ShareContact(share.SharedUser, share.SharedName, _chatMessages[i]);
                    }
                    else if (_chatMessages[i] is
                        TelegramLib.MainClasses.Messages.StaticMessage statMes)
                    {
                        await SetStaticMessageInVis(statMes);
                    }

                    SetPinnedInSettingChatMessages(_chatMessages[i]);
                }

                if (GetAmountOfPinnMesses() == 0)
                    PinRow.Height = new GridLength(0);

                _mainWindow.UpdateUserChatTalkControl();
            });
        }

        public void SetPinnedInSettingChatMessages(TelegramLib.MainClasses.Messages.Message mes)
        {
            if (mes.IsPinned)
            {
                ListBoxItem? item = ChatBox.Items.OfType<ListBoxItem>().LastOrDefault();
                if (item is null ||
                    item.Content is not UserControl control) return;

                SetPinMessage(mes, control);
            }
        }

        public async Task SetStaticMessageInVis(
            TelegramLib.MainClasses.Messages.StaticMessage statMes)
        {
            //Get MessageById
            TelegramLib.MainClasses.Messages.Message? refMes =
               statMes.MessageReferenceId is null ? null :
               _chat.GetMessageById((int)statMes.MessageReferenceId);

            TelegramLib.MainClasses.User cached = ApiService.GetCachedUser(statMes.SenderUserId);
            TelegramLib.MainClasses.User sender = _isSavedMessageChat ? _system.LoggedUser :
                                cached is not null ? cached :
                                await ApiService.GetUserById(statMes.SenderUserId);
            /*await ApiService.GetUserById(statMes.SenderUserId);*/

            if (sender is null) return;

            MonthDay statControl = new MonthDay(sender.Login, refMes, statMes, _chat, _system);
            statControl.Tag = statMes.MessageReferenceId.ToString();

            statControl.ScrollToPinned += () =>
            {
                if (statControl.Tag is null ||
                statControl.Tag.ToString() == "-1") return;

                int.TryParse(statControl.Tag.ToString(), out int mesTag);
                if (mesTag == -1) return;

                ScrollToMessageByMessageId(mesTag);
            };

            ListBoxItem item = new ListBoxItem()
            {
                Content = statControl,
                Tag = statMes.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            SetChatItemEvents(item);

            if (statMes.Date is null) item.PreviewMouseRightButtonDown += SetStateMessageMenu;
            if (statMes.Date is not null) item.PreviewMouseLeftButtonDown += SetCalendarPage;

            ChatBox.Items.Add(item);

            ScrollToNewMessage();
        }

        public void SetCalendarPage(object sender, MouseButtonEventArgs e)
        {
            CalendarPage calPage = new CalendarPage();

            _mainWindow.SetSecondaryFrame(calPage);
        }

        public void SetStateMessageMenu(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                item.Content is not MonthDay statVisObj) return;

            System.Windows.Point clickPosition = e.GetPosition(this);
            int.TryParse(item.Tag.ToString(), out int mesId);

            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);

            //Set message menu for text
            AddMessageMenu(MessageMenuType.StatMessage, clickPosition,
                item, mes);
        }

        public int GetAmountOfPinnMesses()
        {
            int count = _chatMessages.Where(x => x.IsPinned).Count();
            return count;
        }

        public async ValueTask SetMediaMessageInChat(MediaAction message, string senderImgName)
        {
            //Got type (To know what folder to search in)
            MediaType type = message.IsSticker ? MediaType.Sticker :
                FilesAction.GetMediaTypeFromFilename(message.MediaName);

            string path = await FilesAction.GetFilePathByMediaType(type, message.MediaName);
            if(!message.IsSticker) path = FilesAction.GetPathByPseudoPath(path);

            if (path is null || path == string.Empty) return;

            switch (type)
            {
                case MediaType.Image:
                    {
                        await AddImageMessage(path, false, senderImgName, message);
                        return;
                    }
                case MediaType.Gif:
                    {
                        SendGif(path, senderImgName, isAdd: false, message);
                        return;
                    }
                case MediaType.Video:
                    {
                        AddMediaElement(path, senderImgName, message);
                        return;
                    }
                case MediaType.Sticker:
                    {
                        await AddImageMessage(path, true, senderImgName, message);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public void SetMediaForwardButVis(MediaAction mediaAct, MediaMessage control)
        {
            if (_chat is SavedMessagesChat chat &&
                mediaAct.ForwardedFromId is not null)
            {
                control.SetPushForwardedVis();
            }
        }

        public void SetTextMessageInChat(
            TelegramLib.MainClasses.Messages.TextMessage message,
            string senderImageName)
        {
            TelegramLib.MainClasses.Messages.Message? reply =
                message.RepliedMessageId is null ? null :
                message.RepliedMessageId == -1 ? new Message() :
                _system.GetRepliedMessageById((int)message.RepliedMessageId);

            if (reply is not null) reply.SetQuoteText(message.RepliedQuote);

            ChatControls.TextMessage newMes =
                new ChatControls.TextMessage(_system,
                GetConvertedStringMessage(message.Text),
                senderImageName,
                _system.Settings.GetChatSettings().FontName,
                message.IsEdited,
                message,
                toReply: reply,
                forwardedFrom: message.ForwardedFromId);

            newMes.SetTime(message.SentTime);

            ListBoxItem item = new ListBoxItem()
            {
                Content = newMes,
                Tag = message.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                //Padding = new Thickness(0, 1, 0, 1)
            };
            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, message);

            ChatBox.Items.Add(item);

            SetTickStatusIfCorrectMes(item, message);

            if (_chat is SavedMessagesChat chat &&
                message.ForwardedFromId is not null)
            {
                newMes.SetPushForwardedVis();
            }

            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
            SetMessagesPosition(_isGluedToLeft);

            //SetSenderImageByListBoxItem(item, _system.GetUserById(message.SenderUserId), _system.LoggedUser.Id == message.SenderUserId);
        }

        private void SetPaddingToMessageItem(ListBoxItem item,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            if (_tempSendTime is null)
            {
                _tempSendTime = mes.SentTime;
                //return;
            }

            SetChatItemPadding(item, mes);
            _tempSendTime = mes.SentTime;
        }

        private DateTime? _tempSendTime;
        private void SetChatItemPadding(ListBoxItem item,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            const int maxSecDiffer = 5;
            const int closeDiffer = 1;

            if (_tempSendTime is null) return;

            double secDiffer = (mes.SentTime - _tempSendTime.Value).TotalSeconds;

            item.Padding = new Thickness(7, 15, 10, 0);

            if (secDiffer >= 0 && secDiffer < maxSecDiffer)
            {
                item.Padding = new Thickness(7, closeDiffer, 10, closeDiffer);
            }
        }

        private bool _isMouseDown = false;
        private System.Windows.Point _stratSelectionPoint = new System.Windows.Point();
        private System.Windows.Point _prevSelectionPoint = new System.Windows.Point();

        private const int _emptySelStartDiffer = 8;
        public void SetChatItemEvents(ListBoxItem item)
        {
            item.PreviewMouseRightButtonDown += SetMessageMenu_PreviewRightMouseDown;

            item.MouseMove += ChatBoxItems_MouseMove;

            item.PreviewMouseLeftButtonDown += SetSelectingStatus_PreviewMouseDown;

            item.MouseEnter += MessageItem_MouseEnter;
            item.MouseLeave += MessageItem_MouseLeave;

            item.PreviewMouseLeftButtonUp += ChatItem_PreviewLeftMouseButtonUp;

            item.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                _isMouseDown = true;
            };
            item.MouseLeftButtonUp += (sender, e) =>
            {
                _isMouseDown = false;
            };

            SetPushForwardEvent(item);
        }

        public void SetPushForwardEvent(ListBoxItem item)
        {
            if (item.Content is ChatControls.TextMessage text)
            {
                text.PushForwarded += () =>
                {
                    TelegramLib.MainClasses.Messages.TextMessage mes =
                        text.GetMessage();
                    if (mes is null || mes.ForwardedFromId is null) return;

                    int? mesId = _system.GetMessageIdByText(mes.Text);
                    if (mesId is null)
                    {
                        ((MainWindow)Window.GetWindow(this)).SetTemporaryText("There is no such message");
                        return;
                    }
                    ScrollToMessageByMessageId((int)mesId);
                };
            }
            else if (item.Content is MediaMessage media)
            {
                media.PushForwarded += () =>
                {

                    MediaAction mediaEl = media.GetMessage();
                    Console.WriteLine(mediaEl);

                    TelegramLib.MainClasses.UserChat chat = _system.Chats.FirstOrDefault();
                    if (chat is null)
                    {
                        ((MainWindow)Window.GetWindow(this)).SetTemporaryText("Hell nah");
                        return;
                    };
                    SetUserChat(chat);
                };
            }
        }

        private System.Windows.Point GetPointChatBoxScroll(System.Windows.Point tempPoint)
        {
            ScrollViewer sv = HelperService.GetScrollViewer(ChatBox);
            return new System.Windows.Point(tempPoint.X, tempPoint.Y + sv.VerticalOffset);
        }

        public void ChatItem_PreviewLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item) return;
            _isMouseDown = false;
            UpdateSelectedAmount();
        }

        private bool _isSelected = true;
        private ListBoxItem _startChosenItem;
        public void SetSelectingStatus_PreviewMouseDown(
            object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item) return;

            _stratSelectionPoint = GetPointChatBoxScroll(e.GetPosition(this));
            _prevSelectionPoint = GetPointChatBoxScroll(e.GetPosition(this));
            _startChosenItem = item;

            if (item.Content is ChatControls.TextMessage text)
            {
                _isSelected = !text.SelectionTickObj._isChosen;
                text.ChangeTickStatus();
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                _isSelected = !media.SelectionTickObj._isChosen;
                media.ChangeTickStatus();

                DependencyObject clickedElement = e.OriginalSource as DependencyObject;

                if (media.IsBandMedia() &&
                    media.IsTickVisible() && clickedElement is not Border)
                {
                    media.SetBandSelection(_isSelected);
                }
            }
            else if (item.Content is ShareContactControl share)
            {
                _isSelected = !share.SelectionTickObj._isChosen;
                share.ChangeTickStatus();
            }

            //ShowSelectionBar();
            //SetSelectionTick(false, item);
        }

        public void MessageItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                !_isMouseDown || item == _startChosenItem)
            {
                return;
            }

            System.Windows.Point tempPoint = GetPointChatBoxScroll(e.GetPosition(this));

            //Add Upper
            if (tempPoint.Y < _stratSelectionPoint.Y ||
               tempPoint.Y > _stratSelectionPoint.Y)
            {
                SetAllBandMessages(item, true);

                SetSelectionTick(true, item);

                //Update Amount of chosen
                UpdateSelectedAmount();
            }
        }

        public void SetAllBandMessages(ListBoxItem item, bool isSelectAll)
        {
            if (item.Content is MediaMessage band && band.IsBandMedia())
            {
                band.SetBandSelection(isSelectAll);
            }
        }

        public void MessageItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                !_isMouseDown)
            {
                return;
            }

            System.Windows.Point tempPoint = GetPointChatBoxScroll(e.GetPosition(this));

            if ((tempPoint.Y < _stratSelectionPoint.Y &&
                _prevSelectionPoint.Y < tempPoint.Y) ||

                (tempPoint.Y > _stratSelectionPoint.Y &&
                _prevSelectionPoint.Y > tempPoint.Y))
            {
                SetAllBandMessages(item, false);
                SetSelectionTick(false, item);

                //Update Amount of chosen
                UpdateSelectedAmount();
            }

        }

        public void ChatBoxItems_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                !_isMouseDown || Cursor == Cursors.IBeam)
            {
                return;
            }


            SetOnlyChatVisState(false);
            //Update prev Point
            UpdatePrevPoint(GetPointChatBoxScroll(e.GetPosition(this)));

            bool isUnderBandMedia = IsMouseUnderBandElement(sender, e);

            //Set for start item
            if (SetTickForStrtChosenItem(item, GetPointChatBoxScroll(e.GetPosition(this)), isUnderBandMedia))
            {
                ((MainWindow)Window.GetWindow(this)).CloseAllMediaWindows();
                return;
            }
            //To show Selection params
            ShowSelectionBar();
        }


        public bool IsMouseUnderBandElement(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.Content is not MediaMessage media || !media.IsBandMedia()) return false;

                Point mousePos = e.GetPosition(item);

                IInputElement hitElement = item.InputHitTest(mousePos);

                if (hitElement == item || IsSystemVisual(hitElement))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;

        }

        private bool IsSystemVisual(IInputElement element)
        {
            if (element is FrameworkElement fe)
            {
                return fe.Name == "Bd" || fe.TemplatedParent != null && fe.DataContext == null;
            }
            return false;
        }


        private bool SetTickForStrtChosenItem(ListBoxItem item,
            System.Windows.Point tempPoint, bool isUnderMediaBand)
        {
            if (item == _startChosenItem)
            {
                ShowSelectionBar();
                if (_stratSelectionPoint.Y + _emptySelStartDiffer > tempPoint.Y &&
                    _stratSelectionPoint.Y - _emptySelStartDiffer < tempPoint.Y)
                {
                    if (!isUnderMediaBand)
                    {
                        SetAllBandMessages(item, false);
                    }
                    SetSelectionTick(false, item);
                }
                else
                {
                    if (!isUnderMediaBand)
                    {
                        SetAllBandMessages(item, true);
                    }
                    SetSelectionTick(true, item);

                }
                UpdateSelectedAmount();
                return true;
            }
            return false;
        }

        private const int _baseStep = 3;
        private void UpdatePrevPoint(System.Windows.Point tempPoint)
        {
            //Compere ony Y param
            if (tempPoint.Y - _prevSelectionPoint.Y >= _baseStep ||
               tempPoint.Y - _prevSelectionPoint.Y <= _baseStep)
            {
                _prevSelectionPoint = new System.Windows.Point(_prevSelectionPoint.X, tempPoint.Y);
            }
        }

        public void ClearMouseDown()
        {
            _isMouseDown = false;
        }

        public bool IsTickSetCorrectly(ListBoxItem item)
        {
            if (item.Content is ChatControls.TextMessage text)
            {
                return _isSelected == text.SelectionTickObj._isChosen;
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                return _isSelected == media.SelectionTickObj._isChosen;
            }
            return false;
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

        public bool IsSendingChatterIsBlocked()
        {
            if (_chat is not null && _chat.Chatter is not null &&
                _system.IsChatterBlocked(_chat.Chatter))
            {
                ((MainWindow)Window.GetWindow(this)).SetTemporaryText("Misha, stop doing weird stuff!!");
                ClearVisualStuffAfterBlocked();
                return true;
            }
            return false;
        }

        public void ClearVisualStuffAfterBlocked()
        {
            CommentTextBox.Text = string.Empty;
            HideSelectionRow();

            ReplyMessageRow.Height = new GridLength(0);

            _repliedMessage = null;
            _toForwardMessages = null;

        }

        bool _isSending = false;

        private async void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isSending) return;
            if (IsSendingChatterIsBlocked()) return;
            if (OnlyPinnedHeaderGrid.Visibility == Visibility.Visible) return;

            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Clipboard.ContainsFileDropList())
                {
                    var pathList = Clipboard.GetFileDropList().Cast<string>().ToList();

                    AddMediaPage(pathList, CommentTextBox.Text);
                    CommentTextBox.Text = string.Empty;

                    Clipboard.Clear();
                    e.Handled = true;
                    return;
                }
            }
            if (e.Key == Key.Enter && SchedueleMessagesGrid.Visibility == Visibility.Visible)
            {
                await SetScheduleMessageAction();
                return;
            }
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                int caret = CommentTextBox.CaretIndex;

                CommentTextBox.Text = CommentTextBox.Text.Insert(caret, Environment.NewLine);
                CommentTextBox.CaretIndex = caret + Environment.NewLine.Length;

                e.Handled = true;
            }
            else if (
                (_system.Settings.ChatsSettings.GetIsSendWithEnter() && e.Key == System.Windows.Input.Key.Enter) ||

                (!_system.Settings.ChatsSettings.GetIsSendWithEnter() &&
                e.Key == System.Windows.Input.Key.Enter && Keyboard.Modifiers == ModifierKeys.Control))
            {
                try
                {
                    _isSending = true;
                    //HideSelectionRow();

                    await SendMessage();

                    _isHiddenSender = false;
                    ScrollToNewMessage();

                    ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
                    ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();

                    HideSelectionRowFromSignalR(GetSelectedMessages());
                }
                finally
                {
                    _isSending = false;
                }
            }
        }

        public async Task SetScheduleMessageAction()
        {
            HideSelectionRow();
            if (_isEdit)
            {
                await ToEditSchedMessage();
                return;
            }

            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                ((MainWindow)Window.GetWindow(this)).SetTemporaryText("Misha, STOP SENDING EMPTY MESSAGES!!!");
                CommentTextBox.Text = string.Empty;
                return;
            }

            SetSchedulePage();
        }

        public bool IsTextMessageIsEmpty()
        {
            if (!string.IsNullOrWhiteSpace(CommentTextBox.Text)) return false;

            MessageMenu.Children.Clear();
            CommentTextBox.Text = string.Empty;

            ((MainWindow)Window.GetWindow(this)).SetTemporaryText("Misha, Stop It!!!");

            return true;
        }

        private async Task SendMessage()
        {
            if (await IsEditedMessage()) return;

            _textHistory.Clear();
            if (await SendSelectedMessagesToForward()) return;
            if (IsTextMessageIsEmpty()) return;

            MessageMenu.Children.Clear();

            //Clear unused
            string cleaned = Regex.Replace(CommentTextBox.Text, @"^\s+|\s+$", "");

            //To send text message
            await AddTextMessageControl(_system.LoggedUser.GetFirstImageName().Name, cleaned);

            ReplyMessageRow.Height = new GridLength(0);

            SetCommentBoxHeight();
            CommentTextBox.Clear();
        }


        public async Task<bool> IsEditedMessage()
        {
            if (_isEdit)
            {
                await ToEditMessage(_chat.Id);
                return true;
            }
            return false;
        }
        public void SetSchedulePage()
        {
            //form message
            (TelegramLib.MainClasses.Messages.Message mes,
             TelegramLib.MainClasses.Messages.Message toReply) =
             GetTextMessageToSend(CommentTextBox.Text);

            if (mes is null ||

               (mes is TelegramLib.MainClasses.Messages.TextMessage textMes &&
                textMes.Text == string.Empty)) return;

            SetScheduleMessage message =
                new SetScheduleMessage(GetChat(), new List<Message>() { mes }, _system, _toForwardMessages);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(message);
        }

        private const int _maxCommentBoxHeight = 150;
        private const int _baseCommentBoxHeight = 50;
        public void SetCommentBoxHeight()
        {
            //Get amount of new lines
            int count = CommentTextBox.Text
            .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
            .Length - 1;

            int maxSteps = _maxCommentBoxHeight / _baseCommentBoxHeight;

            //Set height of the comment height
            int newSize = count == 0 ? _baseCommentBoxHeight :
                (count > maxSteps + 1) ? _maxCommentBoxHeight :

                ((count + 1) * _baseCommentBoxHeight) > _maxCommentBoxHeight ?
                _maxCommentBoxHeight :
                (count + 1) * _baseCommentBoxHeight;

            CommentRow.Height = new GridLength(newSize);
        }

        public async Task ToEditSchedMessage(TelegramLib.MainClasses.Messages.Message toEdit = null)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ReplyMessageRow.Height = new GridLength(0);
            });

            _isEdit = false;

            if (toEdit is null) toEdit = GetMessageToEdit();
            if (toEdit is null) return;

            SetEditedParams(toEdit);

            //Logic
            _system.EditMessage(toEdit);

            //DB 
            await ApiService.EditSchedMessage(toEdit.Id, toEdit);

            //Visual
            await SetMessagesInChat();
        }

        public void SetIsMouseDownValue(bool val)
        {
            _isMouseDown = val;
        }

        public async Task ToEditMessage(int chatId, bool isBoth = true,
            TelegramLib.MainClasses.Messages.Message toEdit = null,
            bool isSched = false)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ReplyMessageRow.Height = new GridLength(0);
            });

            _isEdit = false;

            if (toEdit is null)
            {
                toEdit = GetMessageToEdit();
            }
            if (toEdit is null) return;

            if (_system.GetIsSavedMesChatStatus())
            {
                if (toEdit is TelegramLib.MainClasses.Messages.TextMessage savedTextMes)
                {
                    savedTextMes.Text = CommentTextBox.Text;
                    savedTextMes.IsEdited = true;
                }
                //In db
                await ApiService.EditSavedChatMessage(toEdit);

                //In system
                _system.EditMessage(toEdit);

                //Visual
                await SetMessagesInChat();
                return;
            }

            //SignalR (What if offline?)
            if (isBoth)
            {
                SetEditedParams(toEdit);
                await SignalRService.EditMessage(_system.LoggedUser, _chat.Chatter, toEdit);
            }

            if (toEdit is TelegramLib.MainClasses.Messages.TextMessage textMes) textMes.IsEdited = true;

            if (isSched) SetEditedParams(toEdit);

            //Logic
            _system.EditMessage(toEdit);

            //DB 
            await ApiService.EditMessage(chatId, toEdit);

            //Visual
            await SetMessagesInChat();
        }

        public void SetEditedParams(TelegramLib.MainClasses.Messages.Message toEdit)
        {
            if (toEdit is TelegramLib.MainClasses.Messages.TextMessage editedText)
            {
                editedText.Text = CommentTextBox.Text;
            }
        }

        public async Task<bool> SendSelectedMessagesToForward()
        {
            if (_toForwardMessages is not null &&
                _toForwardMessages.Count > 0 &&
                _forwardSenderId is not null)
            {
                await AddDateStatMessage();

                UpdateSentDateForMessagesToForward();

                TelegramLib.MainClasses.UserChat chat =
                    _system.GetChatByChatterId((int)_forwardSenderId);

                await AddAdditionalToForwardsTextMessage();

                ClearResenderInForwardedMessages();

                ClearForwardElements();
                await AddForwardedMessagesInDB(chat);
                chat.Messages.AddRange(_toForwardMessages);

                await SetMessagesInChat();

                ClearMessageForwarding();
                return true;
            }
            return false;
        }

        public void ClearForwardElements()
        {
            return;
            const int maxLeft = 5;

            if (_toForwardMessages.Count <= maxLeft) return;
            _toForwardMessages.RemoveRange(maxLeft, _toForwardMessages.Count - maxLeft);
        }

        public void ClearResenderInForwardedMessages()
        {
            if (!_isHiddenSender) return;

            foreach (var message in _toForwardMessages)
            {
                message.ClearForwarded();
            }
        }

        public void UpdateSentDateForMessagesToForward()
        {
            List<Message> res = new List<Message>();
            for (int i = 0; i < _toForwardMessages.Count; i++)
            {
                Message copy = (Message)DeepCopy(_toForwardMessages[i]);
                copy.SentTime = DateTime.Now;
                res.Add(copy);
            }
            _toForwardMessages = res;
        }

        public async Task AddForwardedMessagesInDB(TelegramLib.MainClasses.UserChat chat)
        {
            if (_toForwardMessages is null) return;
            bool isChatterOnline = UpdateChatStatusAsync(chat);

            await ChangeBandIdsInForward();

            HashSet<int> bandIds = new HashSet<int>();

            for (int i = 0; i < _toForwardMessages.Count; i++)
            {
                if (_toForwardMessages[i] is MediaAction media &&
                    media.BandId != -1)
                {
                    if (bandIds.Contains(media.BandId)) continue;

                    List<MediaAction> bandMedias = _toForwardMessages.OfType<MediaAction>().Where(x => x.BandId == media.BandId).ToList();

                    for (int j = 0; j < bandMedias.Count; j++)
                    {
                        //Add In Db (for sender user)
                        await ApiService.AddMessage(bandMedias[j], chat);

                        //Set correct id
                        await UpdateIdForMessageBySentDate(bandMedias[j]);
                    }

                    await AddForwardedMessageInDB(bandMedias.Cast<Message>().ToList(), chat, isChatterOnline);

                    bandIds.Add(media.BandId);
                    continue;
                }

                if (_toForwardMessages[i] is TelegramLib.MainClasses.Messages.ShareContactMessage share)
                {
                    await ApiService.AddShareContactMessage(share.SharedUser.Id,
                        share.SharedUser.Name, chat.Id, _system.LoggedUser.Id, DateTime.Now);
                }
                else
                {
                    //Add In Db (for sender user)
                    await ApiService.AddMessage(_toForwardMessages[i], chat);
                }

                //Set correct id
                await UpdateIdForMessageBySentDate(_toForwardMessages[i]);

                //Add for chatter
                await AddForwardedMessageInDB(new List<Message>() { _toForwardMessages[i] }, chat, isChatterOnline);
            }
        }

        public async Task ChangeBandIdsInForward()
        {
            int lastBandId = await ApiService.GetLastMessageBandId();

            for (int i = 0; i < _toForwardMessages.Count; i++)
            {
                if (_toForwardMessages[i] is MediaAction media &&
                    media.BandId != -1)
                {
                    media.BandId = lastBandId + 1;
                }
            }

        }

        public async Task UpdateIdForMessageBySentDate(TelegramLib.MainClasses.Messages.Message mes)
        {
            TelegramLib.MainClasses.Messages.Message checkMes = await ApiService.GetLastChatMessage(_chat.Id);

            if (checkMes is null) return;
            mes.Id = checkMes.Id;
        }

        private bool UpdateChatStatusAsync(TelegramLib.MainClasses.UserChat chat)
        {
            return Task.Run(async () =>
            {
                var user = await ApiService.GetUserById(chat.Chatter.Id);
                return user.IsOnline;
            }).Result;
        }

        public async Task AddForwardedMessageInDB(
            List<TelegramLib.MainClasses.Messages.Message> messages,
            TelegramLib.MainClasses.UserChat chat,
            bool isChatterOnline)
        {
            if (await ApiService.IsUserIsBlocked(chat.Chatter.Id, _system.LoggedUser.Id)) return;

            TelegramLib.MainClasses.UserChat? chattersChat =
                await GetChatByUserSendersIds(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (chattersChat is null) return;

            if (isChatterOnline)
            {
                SendForwardMessageInSignalR(chat, messages);
                return;
            }
            for (int i = 0; i < messages.Count; i++)
            {
                if (_toForwardMessages[i] is TelegramLib.MainClasses.Messages.ShareContactMessage share)
                {
                    await ApiService.AddShareContactMessage(share.SharedUser.Id,
                        share.SharedUser.Name, chattersChat.Id, _system.LoggedUser.Id, DateTime.Now);
                }
                else
                {
                    //Add In Db (for sender user)
                    await ApiService.AddMessage(messages[i], chattersChat);
                }
            }
        }

        public async Task AddAdditionalToForwardsTextMessage()
        {
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text)) return;

            MessageMenu.Children.Clear();
            _mesMenu = null;

            await AddTextMessageControl(
                _system.LoggedUser.GetFirstImageName().Name, CommentTextBox.Text);
        }

        public void ClearMessageForwarding()
        {
            _toForwardMessages = null;
            _forwardSenderId = null;
            _isHiddenSender = false;

            ReplyMessageRow.Height = new GridLength(0);

            MessageMenu.Children.Clear();
            CommentTextBox.Text = string.Empty;
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
                ReplyMessageRow.Height.Value == 0 ||
                _isHiddenSender) return null;

            //int.TryParse(_mesMenu.GetChosenListBoxItem().Tag.ToString(), out int id);

            int id = _repliedMessage is null ? _mesMenu.GetMessage().Id : _repliedMessage.Id;

            return _system.GetRepliedMessageById(id);
        }

        public (Message, Message) GetTextMessageToSend(string sendText)
        {
            //Get reply message
            TelegramLib.MainClasses.Messages.Message toReply = GetMessageToReply();

            //system add
            int? replyId = toReply is null ? null : toReply.Id;

            Message toAdd = new TelegramLib.MainClasses.Messages.TextMessage(
                            _chatMessages.Count, _system.LoggedUser.Id,
                            DateTime.Now, sendText, false, replyId, false, null, false);

            if (toReply is not null)
            {
                toAdd.SetQuoteText(ReplyedMessageText.Text);
                toReply.SetQuoteText(ReplyedMessageText.Text);
            }
            return (toAdd, toReply);
        }

        private async Task AddTextMessageControl(string senderImageName, string sendText)
        {
            await AddDateStatMessage();
            //Is reply

            (Message toAdd, Message toReply) = GetTextMessageToSend(sendText);

            //Adding in DB
            toAdd = await GetAndAddMessage(toAdd);

            //Visaul add
            ChatControls.TextMessage text = new ChatControls.TextMessage(_system,
                GetConvertedStringMessage(sendText),
                senderImageName,
                _system.Settings.GetChatSettings().FontName,
                false,
                (TelegramLib.MainClasses.Messages.TextMessage)toAdd,
                toReply: toReply);

            ListBoxItem item = new ListBoxItem()
            {
                Content = text,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = GetHorAlignmentForMessage(),
            };
            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, toAdd);

            item.Tag = toAdd.Id.ToString();
            ChatBox.Items.Add(item);
            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            if (toAdd is not TelegramLib.MainClasses.Messages.TextMessage toAddText) return;

            _chatMessages.Add(toAddText);
            if (CommentTextBox.Text == sendText) CommentTextBox.Text = string.Empty;

            //Add Message In DB (On chatters side) 
            await AddTextMessageInDb(toAddText);

            //Set vis tick 
            SetTickStatusIfCorrectMes(item, toAdd);

            ((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();
            SetMessagesPosition(_isGluedToLeft);
        }

        public async Task<TelegramLib.MainClasses.Messages.Message> GetAndAddMessage
            (TelegramLib.MainClasses.Messages.Message toAdd)
        {
            if (_isSavedMessageChat)
            {
                await ApiService.AddSavedMessage(_chat.Id, toAdd);
                int? id = await ApiService.GetIdOfLastSavedMessage(_chat.Id);
                if (id is not null) toAdd.Id = (int)id;
                return toAdd;
            }

            await ApiService.AddMessage(toAdd, _chat);
            return await ApiService.GetLastChatMessage(_chat.Id);
        }

        public void SetMessageMenu_PreviewRightMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item) return;
            System.Windows.Point clickPosition = e.GetPosition(this);

            var clicked = e.OriginalSource as DependencyObject;
            if (clicked is Ellipse) return;

            int mesId = GetMessageIdByListBoxItem(item, e);
            if (mesId == -1) return;

            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);

            if (item.Content is ChatControls.TextMessage text)
            {
                //Set message menu for text
                AddMessageMenu(MessageMenuType.TextMessage, clickPosition, item, mes);
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                AddMessageMenu(MessageMenuType.MediaMessage, clickPosition, item, mes);
            }
            else if (item.Content is ShareContactControl share)
            {
                AddMessageMenu(MessageMenuType.ShareContact, clickPosition, item, mes);
            }
        }

        public int GetMessageIdByListBoxItem(ListBoxItem item, MouseEventArgs e)
        {
            if (item.Content is MediaMessage media && media.IsBandMedia())
            {
                Point mousePosition = e.GetPosition(item);
                IInputElement clickedElement = item.InputHitTest(mousePosition);
                DependencyObject obj = clickedElement as DependencyObject;

                while (obj != null)
                {
                    if (obj is Border b && b.Name != media.ImgGroupBorder.Name)
                    {
                        if (b.Tag != null)
                        {

                            return int.Parse(b.Tag.ToString());
                        }
                    }
                    obj = VisualTreeHelper.GetParent(obj);
                }

                return -1;
            }
            if (item.Tag is null) return -1;
            int.TryParse(item.Tag.ToString(), out int mesId);
            return mesId;
        }

        public ListBoxItem GetListBoxItemByMessageId(int mesId)
        {
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;

                if (item.Tag is null && item.Content is MediaMessage media)
                {
                    if (media.IsBandBorderContainsId(mesId)) return item;
                }
                else if (item.Tag is not null)
                {
                    int.TryParse(item.Tag.ToString(), out int id);

                    if (id == mesId) return item;
                }
            }
            return null;
        }

        private MesMenu _mesMenu;
        public void AddMessageMenu(
            MessageMenuType menuType,
            System.Windows.Point point,
            ListBoxItem item,
            Message toMenuMes)
        {
            MessageMenu.Children.Clear();

            //If some parts are visible
            bool isOnlyPinnedChat = UnPinAllBorder.Visibility == Visibility.Visible ||
                OnlyPinnedHeaderGrid.Visibility == Visibility.Visible;

            bool isSchedMessage = SchedueleMessagesGrid.Visibility == Visibility.Visible;

            _mesMenu = new MesMenu(menuType, isOnlyPinnedChat,
                toMenuMes, _system, isSchedMessage);
            _mesMenu.SetClickedListBoxItem(item);

            _mesMenu.Loaded += (sender, e) =>
            {
                double actWidth = this.ActualWidth - GetUserInfoColumnWidth();
                //is x to big
                if (point.X + _mesMenu.ActualWidth > actWidth)
                {
                    Canvas.SetLeft(_mesMenu, point.X - _mesMenu.Width);
                }
                else Canvas.SetLeft(_mesMenu, point.X);

                //is y too big
                if (point.Y + _mesMenu.ActualHeight > this.ActualHeight)
                {
                    Canvas.SetTop(_mesMenu, this.ActualHeight - _mesMenu.ActualHeight);
                }
                else Canvas.SetTop(_mesMenu, point.Y);
            };

            //WTF is this???
            /*            Message mes = GetMessageByListBoxTag(item);
                        _mesMenu.SetPinVisStatus(mes);*/

            if (toMenuMes is not null) _mesMenu.SetPinVisStatus(toMenuMes);

            MessageMenu.Children.Add(_mesMenu);
            SetMesMenuActions(_mesMenu);
        }

        public void SetMesMenuActions(MesMenu menu)
        {
            menu.GoToMessageAct += () => GoToMessageAction();

            menu.ReplyAct += () => SetReplyMessageRow();
            menu.PinAct += () => SetMessagePinChat();

            menu.ShowInFolderAct += () => ShowInFolderAction();
            menu.ForwardAct += () => ForwardMesAction();

            menu.DeleteAct += () => SetIsBothDeletePage();
            menu.CopyAct += () => CopyMessageAction();
            menu.SaveAct += () => SaveMediaAction();

            menu.SelectAct += () => SelectionAction();

            menu.EditAct += () => EditMessageAct();

            menu.SendNowAct += () => SendSchedMessageNowFromMenu();
            menu.RescheduleMessageAct += () => RescheduleMessage();
        }

        public void RescheduleMessage()
        {
            List<Message> messages =
                GetChosenMesInMesMenu();

            if (messages is null ||

               (messages.First() is TelegramLib.MainClasses.Messages.TextMessage textMes &&
                textMes.Text == string.Empty)) return;

            SetScheduleMessage message =
                new SetScheduleMessage(GetChat(), messages, _system, _toForwardMessages, isUpdateDate: true);


            for (int i = 0; i < messages.Count; i++)
            {
                int index = i;
                messages[index].StartTimer();
                messages[index].SentTimeIsNow += () =>
                {
                    messages[index].EndTimer();
                    if (message._messages[index].Id != messages[index].Id)
                    {
                        return;
                    }
                    ClearMenusAfterMessageReschedule();
                };
            }

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(message);
        }

        public void ClearMenusAfterMessageReschedule()
        {
            HideSelectionRow();
            ClearReschedulePage();
        }

        public void ClearReschedulePage()
        {
            Window window = Window.GetWindow(this);
            if (window is MainWindow main)
            {
                main.ClearSchedulePage();
            }
        }

        public async void SendSchedMessageNowFromMenu()
        {
            List<Message> messages =
                GetChosenMesInMesMenu();

            foreach (var mes in messages)
            {
                await SendSchedMessageNow(mes);
            }
            //Send must be doing service in server part

            UpdateGlobalMedias();
        }

        public async Task SendSchedMessageNow(Message mes)
        {
            if (mes is null) return;
            //Update it in db
            await ApiService.UpdateSchedMessageDate(mes.Id, DateTime.Now);

            //Update in system
            mes.SentTime = DateTime.Now;
        }

        public List<Message> GetChosenMesInMesMenu()
        {
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();  //GetListBoxItemFromMenu();
            if (item is null || item.Content is not UserControl control) return null;

            if (item.Content is MediaMessage media &&
                media.IsBandMedia())
            {
                List<int> ids = media.GetBandMessagesIds();
                List<Message> res = new List<Message>();

                for (int i = 0; i < ids.Count; i++)
                {
                    Message tempMes = _system.GetMessageById(ids[i]);
                    if (tempMes is null) continue;
                    res.Add(tempMes);
                }

                return res;
            }

            Message mes = GetMessageByListBoxTag(item);
            return new List<Message>() { mes };
        }

        private bool _isEdit = false;
        public void EditMessageAct()
        {
            _isEdit = true;

            //ADD IS EDITED FLAG

            //Show reply line   
            SetReplyMessageRow();

            //set reply line 
            //set textbox + (is edit flag)
            SetParamsToEditText();
        }

        public void SetParamsToEditText()
        {
            TelegramLib.MainClasses.Messages.Message mes = GetMessageToEdit();

            if (mes is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                CommentTextBox.Text = text.Text;
            }

            ReplySenderText.Text = "Edit Message";
        }

        public TelegramLib.MainClasses.Messages.Message GetMessageToEdit()
        {
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();  //GetListBoxItemFromMenu();
            if (item is null || item.Content is not UserControl control) return null;

            //item.Tag == message id; Get message by system
            Message mes = GetMessageByListBoxTag(item);
            return mes;
        }

        public void GoToMessageAction()
        {
            //Hide all only pinned borders
            HideOnlyPinnedBorders();

            //Get ListBoxItem From Menu
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();  //GetListBoxItemFromMenu();
            if (item is null || item.Content is not UserControl control) return;

            //item.Tag == message id; Get message by system
            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null) return;

            SetChatMessages();

            ScrollToMessageByMessageId(mes.Id);
        }

        public void SelectionAction()
        {
            ((MainWindow)Window.GetWindow(this)).CloseAllMediaWindows();

            //Get message to resend
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);
            if (mes is null) return;

            if (mes is null) return;

            ShowSelectionBar();

            bool isBandMedia = IsListBoxItemIsBandMessage(item);
            if (isBandMedia && mes is MediaAction mediaMes)
            {
                SwapSelVisStateInMediaBand(item, mediaMes);
            }

            //Activate item
            ActivateSelectionTick(item);

            ((MainWindow)Window.GetWindow(this)).UpdateUserChatSelectedAmount();
        }

        public void SwapSelVisStateInMediaBand(ListBoxItem item, MediaAction mes)
        {
            MediaMessage media = item.Content as MediaMessage;
            media.ChangeSelectionBorderStatus(mes);
        }

        public bool IsListBoxItemIsBandMessage(ListBoxItem item)
        {
            return item.Content is MediaMessage media &&
                media.IsBandMedia();
        }

        public bool IsAllMessagesInMediaAreChosen(ListBoxItem item)
        {
            if (!IsListBoxItemIsBandMessage(item)) return false;

            MediaMessage media = item.Content as MediaMessage;

            return media.IsAllMediasInBandAreChosen();
        }

        public void ShowSelectionBar()
        {
            if (SelectedMessesGrid.Visibility == Visibility.Visible) return;
            SetMessageSelectCircleVis(true);
            SelectedMessesGrid.Visibility = Visibility.Visible;
            SetSelectedMessagesGridType();
            ChatterInfoGrid.Visibility = Visibility.Hidden;
        }

        const string _ifForwardSelectedText = "Forward";
        const string _ifSendNowSelected = "Send now";
        public void SetSelectedMessagesGridType()
        {
            if (SchedueleMessagesGrid.Visibility == Visibility.Visible)
            {
                ForwardSelectedButText.Text = _ifSendNowSelected;
                return;
            }
            ForwardSelectedButText.Text = _ifForwardSelectedText;
        }

        public void ActivateSelectionTick(ListBoxItem item)
        {
            if (item.Content is ChatControls.TextMessage text)
            {
                text.SelectionTickObj.ActivateTickAction(text);
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                media.SelectionTickObj.ActivateTickAction(media);
            }
        }

        public void SetSelectionTick(bool isSet, ListBoxItem item)
        {
            if (item.Content is ChatControls.TextMessage text)
            {
                text.SelectionTickObj.SetTickByGivenParam(isSet);
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                if (media.IsBandMedia())
                {
                    isSet = media.IsAllMediasInBandAreChosen();
                }

                media.SelectionTickObj.SetTickByGivenParam(isSet);
            }
            else if (item.Content is ShareContactControl share)
            {
                share.SelectionTickObj.SetTickByGivenParam(isSet);
            }

        }

        public async void SetBothUsersPage(TelegramLib.MainClasses.Messages.Message mes,
            ListBoxItem itemMessage, BothUsersMessageAction actionType)
        {
            if (_isSavedMessageChat)
            {
                await SetPinDeleteAction(mes, itemMessage, actionType, false);
                return;
            }

            IsMakeActionOnBothSides page =
                new IsMakeActionOnBothSides(_chat.Chatter, actionType);

            page.MakeAction += async () =>
            {
                //Is for both user action
                bool? isBoth = page.IsInBoth.IsChecked;
                if (isBoth is null) return;

                await SetPinDeleteAction(mes, itemMessage, actionType, (bool)isBoth);

                ((MainWindow)Window.GetWindow(this)).CloseAllMediaWindows();

                ((MainWindow)Window.GetWindow(this)).UpdateGlobalMedias();
            };
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public async Task SetPinDeleteAction(TelegramLib.MainClasses.Messages.Message mes,
            ListBoxItem itemMessage, BothUsersMessageAction actionType, bool isBoth)
        {
            if (actionType == BothUsersMessageAction.Delete ||
                actionType == BothUsersMessageAction.SchedDelete)
            {

                if (actionType == BothUsersMessageAction.SchedDelete) isBoth = false;
                //Set page actions
                await DeleteMessage(mes, itemMessage, isBoth);
            }
            else
            {
                SetPinAction(mes, itemMessage, isBoth);

                if (mes.IsPinned) await SetStatMessageAfterPin(mes, isBoth);
                if (IsOnlyPinnedChatIsOn()) IsOnlyPinnedChatPinAction();
            }
        }

        public void IsOnlyPinnedChatPinAction()
        {
            UpdateParamsInPinnedChat();
            SetChatMessages(true);
            IsOnlyPinIsClear();
            PinRow.Height = new GridLength(0);
        }

        public void UpdateParamsInPinnedChat()
        {
            //UPDATE THEM

            //Counter
            PinnedAmountRun.Text = _chat.GetAmountOfPinnedMessages().ToString();

            //Remove int vis

        }

        public async Task SetStatMessageAfterPin(Message referMessage, bool isBoth)
        {
            //Set basic stat message
            StaticMessage toAdd = new StaticMessage(
                referMessage.Id, -1, referMessage.SenderUserId);

            await AddStatMessage(toAdd, isBoth, _chat);
        }

        public async Task AddDateStatMessage()
        {
            if (_chat.IsStateDateExist(DateTime.Now)) return;

            StaticMessage dateMessage = new StaticMessage(DateTime.Now, _system.LoggedUser.Id);

            //Add date stat to chatter to chatter
            if (!_isSavedMessageChat) await AddDateStatMessageToChatter(dateMessage);

            //Add message
            await AddStatMessage(dateMessage, false, _chat);
        }

        public async Task AddDateStatMessageToChatter(
            TelegramLib.MainClasses.Messages.StaticMessage toAdd)
        {
            if (_chat.Chatter is null) return;

            bool isChatterOnline = await ApiService.IsUserOnline(_chat.Chatter.Id);

            bool? isExist = await ApiService.IsDateMesIsExistInChat(
                _system.LoggedUser.Id, _chat.Chatter.Id, (DateTime)toAdd.Date);
            if (isExist is null || (bool)isExist) return;

            StaticMessage stat = (StaticMessage)DeepCopy(toAdd);

            if (isChatterOnline)
            {
                //Set it in SignalR
                await SignalRService.AddStatMessage(_system.LoggedUser, stat, _chat.Chatter);
                return;
            }

            TelegramLib.MainClasses.UserChat? chatterChat =
                await ApiService.GetChatByUserAndSenderId(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (chatterChat is null) return;

            await ApiService.AddStatMessage(stat, chatterChat.Id);
        }

        public async Task<int?> AddStatMessageInDb(StaticMessage toAdd,
            TelegramLib.MainClasses.UserChat chat)
        {
            if (_isSavedMessageChat)
            {
                await ApiService.AddSavedMessage(chat.Id, toAdd);

                return await ApiService.GetLastStatDateIdInSavedChat(chat.Id);
            }

            //Add it in db
            await ApiService.AddStatMessage(toAdd, chat.Id);
            //Get static message from db + assign messageId
            return await ApiService.GetLastAddedStatMessageIdByChatId(chat.Id);
        }

        public async Task AddStatMessage(StaticMessage toAdd, bool isBoth,
            TelegramLib.MainClasses.UserChat chat)
        {
            /*            //Add it in db
                        await ApiService.AddStatMessage(toAdd, chat.Id);
                        //Get static message from db + assign messageId
                        int? lastMesId = await ApiService.GetLastAddedStatMessageIdByChatId(chat.Id);*/

            int? lastMesId = await AddStatMessageInDb(toAdd, chat);

            if (lastMesId is null) throw new ArgumentNullException("Wtf Bro?");
            toAdd.Id = (int)lastMesId;

            //Add it in chat logic part
            chat.Messages.Add(toAdd);

            //Set int vis (If need)
            if (_chat.Id == chat.Id &&
                !IsOnlyPinnedChatIsOn()) await SetStaticMessageInVis(toAdd);

            //Set in UserTalkMessage
            /*((MainWindow)Window.GetWindow(this))*/
            _mainWindow.UpdateUserChatTalkControl();

            //Set for chatter (only db or SignalR)
            _mainWindow.UpdateAutoDelVis(chat);
            if (!isBoth || chat.Chatter is null) return;

            bool isChatterOnline = await ApiService.IsUserOnline(chat.Chatter.Id);

            await AddStatMessageForChatter(toAdd, isChatterOnline);
        }

        public async Task AddStatMessageForChatter(StaticMessage stat, bool isChatterOnline)
        {
            if ((_chat is null || _chat.Chatter is null) ||
                (stat.MessageReferenceId is null &&
                stat.DelType is null)) return;

            bool isBlocked = await ApiService.IsUserIsBlocked(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (isBlocked) return;


            if (isChatterOnline)
            {
                //Set it in SignalR
                await SignalRService.AddStatMessage(_system.LoggedUser, stat, _chat.Chatter);
                return;
            }

            StaticMessage mes = (TelegramLib.MainClasses.Messages.StaticMessage)DeepCopy(stat);

            //Get chat from chatter Side
            TelegramLib.MainClasses.UserChat? chat =
                await ApiService.GetChatByUserAndSenderId(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (chat is null) return;

            Message? refedMes =
                (mes.MessageReferenceId is null && mes.DelType is not null) ? null :

                (mes.MessageReferenceId is null && mes.DelType is null) ? null :
                _chat.GetMessageById((int)mes.MessageReferenceId);


            if (stat.DelType is not null) chat.AutoDel = (TelegramLib.Enums.Chat.AutoDeleteType)stat.DelType;

            //Stat message Id from chatter side
            if (mes.MessageReferenceId is not null) mes.MessageReferenceId = chat.GetMesIdPairOfMessageByTime(refedMes);

            await ApiService.UpdateChat(chat);

            //Add 
            await ApiService.AddStatMessage(mes, chat.Id);
        }

        public void ForwardMesAction()
        {
            //Get message to resend
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null) return;

            //Set page to choose destionation of forwarding
            ForwardToPage page = new ForwardToPage(_system, mes);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public void SetMessageToForward(TelegramLib.MainClasses.Messages.Message mes)
        {
            ForwardToPage page = new ForwardToPage(_system, mes);
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        public void ShowInFolderAction()
        {
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null || mes is not MediaAction media) return;

            (string name, MediaShowType type) mediaParams = GetParamsToShowInFolder(media);

            //Get full filePath
            string fullPath = FilesAction.GetFullPath(mediaParams.name, mediaParams.type); //GetFullPath(mediaName);

            if (fullPath is null || fullPath == string.Empty || !File.Exists(fullPath))
            {
                MessageBox.Show("Its Server!");
                return;
            }
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
        }

        public (string, MediaShowType) GetParamsToShowInFolder(MediaAction media)
        {
            return media.IsImage() ? (media.MediaName, MediaShowType.ChatImages) :
                media.IsGif() ? (media.MediaName, MediaShowType.Gif) :
                media.IsVideo() ? (media.MediaName, MediaShowType.Videos) :
                (string.Empty, MediaShowType.OtherUserImages);
        }

        public void SaveMediaAction()
        {
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null || mes is not MediaAction media) return;
            if (media.IsGif())
            {
                //string gifFullPath = FilesAction.GetFullGifPath(media.MediaName);
                string gifFullPath = FilesAction.GetPathByName(media.MediaName);
                SaveElements.SaveGifAs(gifFullPath);
            }
            else if (media.IsImage())
            {
                //string imgPath = FilesAction.GetFullChatImagePath(media.MediaName);
                string imgPath = FilesAction.GetPathByName(media.MediaName);

                var image = new Image();
                image.Source = new BitmapImage(new Uri(imgPath, UriKind.Absolute));
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

            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null) return;

            if (mes is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                Clipboard.SetText(text.Text);
            }
            else if (mes is TelegramLib.MainClasses.Messages.MediaAction media)
            {
                if (!media.IsImage()) return;

                //string mediaPath = FilesAction.GetFullChatImagePath(media.MediaName);

                string mediaPath = FilesAction.GetPathByName(media.MediaName);
                if (mediaPath is null) return;

                DataObject data = new DataObject();
                data.SetData(DataFormats.FileDrop, new string[] { mediaPath });
                data.SetImage(new BitmapImage(new Uri(mediaPath)));

                Clipboard.SetDataObject(data);
            }
            else if (mes is TelegramLib.MainClasses.Messages.ShareContactMessage share)
            {
                Clipboard.SetText("Contact");
            }
        }

        public void SetIsBothDeletePage()
        {
            HideSelectionRow();

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
            RemoveRightContactInfo();

            ListBoxItem item = _mesMenu.GetChosenListBoxItem();
            if (item is null || item.Content is not UserControl control) return;

            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null) return;

            BothUsersMessageAction delType =
                SchedueleMessagesGrid.Visibility == Visibility.Visible ?
                BothUsersMessageAction.SchedDelete :
                BothUsersMessageAction.Delete;

            SetBothUsersPage(mes, item, delType);

            UpdateGlobalMedias();
        }

        public void DeleteMessageByMessage(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            if (!_chat.Messages.Any(x => x.Id == mes.Id)) return;

            RemoveRightContactInfo();

            IsMakeActionOnBothSides page =
                new IsMakeActionOnBothSides(_chat.Chatter, true);
            page.MakeAction += async () =>
            {
                //Get selected messages
                List<Message> toDelete = GetSelectedMessages();

                bool? isBoth = page.IsInBoth.IsChecked;
                if (isBoth is null) return;

                //Delete message
                ListBoxItem item = GetItemByTagId(mes.Id);
                if (item is null) return;

                //Set page actions
                await DeleteMessage(mes, item, (bool)isBoth);
            };
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public async void RemoveMessagesByDates(List<DateTime> dates)
        {
            ClearSelectionRow();

            if (_chat is not null && _chat.Id == _system.LoggedUser.Id)
            {
                await RemoveMessagesByDatesOnSide(dates, false);
                return;
            }

            //Set is both page 
            IsMakeActionOnBothSides page =
                new IsMakeActionOnBothSides(
                    _system.LoggedUser, BothUsersMessageAction.Delete);


            page.MakeAction += async () =>
            {
                bool? isBoth = page.IsInBoth.IsChecked;
                if (isBoth is null) return;

                await RemoveMessagesByDatesOnSide(dates, (bool)isBoth);
            };

            _mainWindow.SetSecondaryFrame(page);
        }

        public async Task RemoveMessagesByDatesOnSide(List<DateTime> dates, bool isBoth)
        {
            Window window = Window.GetWindow(this);
            SchedueleMessagesGrid.Visibility = Visibility.Hidden;

            //Get selected messages
            List<Message> toDelete = GetMessagesByDate(dates);

            //One by one is too slow try many at the same time 

            await RemoveManyMessages(toDelete, isBoth);

            HideSelectionRow();
            await RemoveDateStateIfNoMesOnDate();

            HideSelectionRow();

            if (window is not null && window is MainWindow main)
            {
                main.UpdateUserChatTalkControl();
                main.CloseAllMediaWindows();
            }
        }

        public List<Message> GetMessagesByDate(List<DateTime> dates)
        {
            List<Message> res = new List<Message>();

            for (int i = 0; i < dates.Count; i++)
            {
                res.AddRange(_chat.GetMessagesByDateTime(dates[i]));
            }

            return res;
        }

        public async Task RemoveDateStateIfNoMesOnDate()
        {
            //Get last message from chat
            TelegramLib.MainClasses.Messages.Message? isRemove =
                _chat.Messages.LastOrDefault();

            //is not stat + date => return
            if (isRemove is not StaticMessage stat || stat.Date is null) return;

            ListBoxItem? item = GetChatItemByMessageId(isRemove.Id);
            if (item is null) return;

            //remove this
            await DeleteMessage(isRemove, item, false, false);
        }
        public async Task DeleteMessage(
            TelegramLib.MainClasses.Messages.Message message,
            ListBoxItem item, bool isBoth, bool isUpdateChatVis = true)
        {
            if (isBoth)
            {
                await DeleteMessageForBoth(message, isVisUpdate: isUpdateChatVis);
                //SignalRService.DeleteMessageById(_system.LoggedUser, _chat.Chatter, mes);

                //await SignalRService.UpdateChatsControls(_system.LoggedUser, _chat.Chatter);
            }

            //Set for replied + pinned message
            _system.SetChatParamsAfterMessageRemoved(message);

            //Remove from system
            _system.RemoveMessageById(message.Id);

            //Remove from Visual
            if (item is not null && ChatBox.Items.Contains(item)) ChatBox.Items.Remove(item);

            //Hide upper borders
            HideUpperBorders();

            //Update vis 
            bool isOnlyPinnedChat = IsOnlyPinnedChatIsOn();
            if (isOnlyPinnedChat) IsOnlyPinnedChatPinAction();
            else if (isUpdateChatVis) SetChatMessages();

            //Remove from db
            await RemoveMessageFromDb(message.Id);

            await RemoveDateStateIfNoMesOnDate();
        }

        public void HideUpperBorders()
        {
            SchedueleMessagesGrid.Visibility = Visibility.Hidden;
        }

        public async Task RemoveMessageFromDb(int mesId)
        {
            if (_isSavedMessageChat)
            {
                await ApiService.RemoveSavedMessage(_chat.Id, new List<int>() { mesId });
                return;
            }
            await ApiService.DeleteMessageById(mesId);
        }

        public async Task DeleteMessageForBoth(Message toRemove, bool isVisUpdate = true)
        {
            //is user is online
            bool isChatterOnline = await ApiService.IsUserOnline(_chat.Chatter.Id);

            //remove in IRL If online
            if (isChatterOnline)
            {
                await SignalRService.DeleteMessageById(
                    _system.LoggedUser, _chat.Chatter, toRemove, isVisUpdate);
                return;
            }

            TelegramLib.MainClasses.UserChat? chat =
                await ApiService.GetChatByUserAndSenderId(_chat.Chatter.Id, _system.LoggedUser.Id);

            //remove from db
            TelegramLib.MainClasses.Messages.Message? pair =
                await ApiService.GetPairOfMessage(toRemove);

            if (pair is null) return;
            await ApiService.DeleteMessageById(pair.Id);

            chat.RemoveMessageById(pair.Id);
            //Is Need to remove date Message

            if (chat is null || chat.Messages.Count() == 0) return;
            TelegramLib.MainClasses.Messages.Message isDate = chat.Messages.Last();
            if (isDate is not StaticMessage stat || stat.Date is null) return;
            await ApiService.DeleteMessageById(stat.Id);
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

        public void SetMessagePinChat()
        {
            ClearSelectionRow();

            //Get ListBoxItem From Menu
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();  //GetListBoxItemFromMenu();
            if (item is null || item.Content is not UserControl control) return;

            //item.Tag == message id; Get message by system
            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null) return;

            BothUsersMessageAction actType = mes.IsPinned ?
                BothUsersMessageAction.UnPin :
                BothUsersMessageAction.Pin;

            SetBothUsersPage(mes, item, actType);
        }

        public void SetPinAction(
            TelegramLib.MainClasses.Messages.Message? mes,
            ListBoxItem item, bool isBoth, bool isChatterBlocked = false)
        {
            if (item is not null && item.Content is not UserControl) return;

            //if (item.Content is not UserControl control) return;
            //Check this
            UserControl? control = item is null ? null :
                item.Content is UserControl ctrl ? ctrl : null;

            //Set Pin status in system

            mes.MirrorPinStatus();
            ApiService.SetPinStatus(mes.Id, mes.IsPinned, _isSavedMessageChat);

            if (!isChatterBlocked) SetPinMessage(mes, control, isBoth);
        }

        public void SetPinMessage(TelegramLib.MainClasses.Messages.Message mes,
            UserControl control, bool bothPin = false)
        {
            if (mes.IsPinned) AddPinnedMessage(mes);
            else DeletePinnedMessage(mes);

            if (control is not null) SetPinOnVisControl(control, mes.IsPinned);

            if (bothPin)
            {
                bool isOnline = Task.Run(() => ApiService.IsUserOnline(_chat.Chatter.Id)).Result;

                if (isOnline) SignalRService.PinMessage(_system.LoggedUser, _chat.Chatter, mes);
                else
                {
                    Message? pair = Task.Run(() => ApiService.GetPairOfMessage(mes)).Result;
                    if (pair is null) return;
                    ApiService.SetPinStatus(pair.Id, mes.IsPinned, _isSavedMessageChat);
                }

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
            /* if (mes.Id == tempMesId)
             {*/
            TelegramLib.MainClasses.Messages.Message nextMes; /* =
               _system.GetLastPinnedMessageByMessage(mes);*/ // _system.GetNextPinnedMessage(mes);

            //Remove mes from system
            _system.DeletePinnedMessage(mes);

            nextMes = _chat.GetLastPinnedMessage();

            if (nextMes is not null) SetPinnedMessageInPanel(nextMes);

            //}

            //Delete in DB
            //Delete with SignalR

            if (IsHidePinnedMessesRow(mes)) return;
            //Is pinned panel is visible
            ShowPinnedIfNotOnlyPinned();
            //PinRow.Height = new GridLength(50);
        }

        public void AddPinnedMessage(TelegramLib.MainClasses.Messages.Message mes)
        {
            //Is pinned panel is visible
            ShowPinnedIfNotOnlyPinned();
            //PinRow.Height = new GridLength(50);

            //Add in last position
            _system.AddPinnedMessage(mes);

            //Set in last position + show this in panel
            SetPinnedMessageInPanel(mes);
        }

        public void ShowPinnedIfNotOnlyPinned()
        {
            if (IsOnlyPinnedChatIsOn()) return;
            PinRow.Height = new GridLength(50);
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
            if (_chat.Chatter is null) return;
            if (await ApiService.IsUserIsBlocked(_chat.Chatter.Id, _system.LoggedUser.Id))
            {
                return;
            }
            //TO add in both chats if chatter online
            if (await ApiService.IsUserOnline(_chat.Chatter.Id))
            {
                await SignalRService.SendTextMessage(_system.LoggedUser, toAddText, _chat.Chatter);
                return;
            }

            //just to Add in chatters chat in db
            //Get chat
            TelegramLib.MainClasses.UserChat chat =
                await GetChatByUserSendersIds(_chat.Chatter.Id, _system.LoggedUser.Id);
            //await ApiService.GetChatByUserAndSenderId(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (chat is null) return;

            if ((_forwardSenderId is null || _toForwardMessages is null
                || _toForwardMessages.Count == 0) &&
                toAddText.RepliedMessageId is not null)
            {
                TelegramLib.MainClasses.Messages.TextMessage text =
                     await ChangeReplyMessageId(toAddText);

                if (text is null) return;

                text.RepliedQuote = toAddText.RepliedQuote;
                await ApiService.AddMessage(text, chat);
                return;
            }
            //Add in chats db
            await ApiService.AddMessage(toAddText, chat);
        }

        public async Task<TelegramLib.MainClasses.Messages.TextMessage> ChangeReplyMessageId(TelegramLib.MainClasses.Messages.TextMessage message)
        {
            //Get mirror of the message to reply
            TelegramLib.MainClasses.Messages.Message mes =
                await ApiService.GetTextMessageById((int)message.RepliedMessageId);

            TelegramLib.MainClasses.Messages.Message? res =
                await ApiService.GetPairOfMessage(mes);
            if (res is null) return null;

            TelegramLib.MainClasses.Messages.TextMessage copy =
                (TelegramLib.MainClasses.Messages.TextMessage)DeepCopy(message);
            copy.RepliedMessageId = res.Id;

            if (res is null) copy.RepliedMessageId = -1;
            else copy.RepliedMessageId = res.Id;
            return copy;
        }

        public async Task<TelegramLib.MainClasses.UserChat?> GetChatByUserSendersIds(int userId, int senderId)
        {
            TelegramLib.MainClasses.UserChat? chat = _system.GetChatByChatterId(senderId);

            if (chat is null)
            {
                chat = await ApiService.GetChatByUserAndSenderId(userId, senderId);
            }
            if (chat is null)
            {
                await ApiService.AddNewChat(userId, senderId);

                chat = await ApiService.GetChatByUserAndSenderId(userId, senderId);

                if (chat is not null)
                {
                    chat.Chatter.GetFirstImageName();

                    StaticMessage date = new StaticMessage(DateTime.Now, userId);
                    date.SentTime = DateTime.Now.AddMilliseconds(-300);
                    await ApiService.AddStatMessage(date, chat.Id);

                    TelegramLib.MainClasses.Messages.Message? mes = await ApiService.GetLastChatMessage(chat.Id);

                    if (mes is not null) date.Id = mes.Id;
                    chat.Messages.Add(date);
                }
            }
            return chat;
        }

        private async Task SendMessageToReceiver(List<Message> toAdd)
        {
            if (_isSavedMessageChat) return;

            bool isReceiverOnline = await ApiService.IsUserOnline(_chat.GetChatter().Id);

            if (!isReceiverOnline)
            {
                TelegramLib.MainClasses.User receiver =
                await ApiService.GetUserById(_chat.GetChatter().Id);

                //UserContactcs contact = await ApiService.GetContactByUserAndFriendIds(_system.LoggedUser.Id, receiver.Id);

                TelegramLib.MainClasses.UserChat chat =
                    await ApiService.GetChatByUserAndSenderId(receiver.Id, _system.LoggedUser.Id);


                for (int i = 0; i < toAdd.Count; i++)
                {
                    await ApiService.AddMessage(toAdd[i], chat);
                }
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

        private async void AddFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose image or video",
                Filter = "Image and Video files|*.png;*.jpg;*.jpeg;*.mp4;*.mov;*.avi",
                Multiselect = true
            };

            RemoveRightContactInfo();

            if (openFileDialog.ShowDialog() == true)
            {
                string[] names = openFileDialog.FileNames;

                //Upload medias
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = await ApiService.UploadMediaAsync(names[i]);
                    names[i] = FilesAction.GetPathByPseudoPath(names[i]);
                }

                //Set media with schedule messages
                if (SchedueleMessagesGrid.Visibility == Visibility.Visible)
                {
                    //0 - Set medias 
                    SendMediaPage page = new SendMediaPage(names.ToList(), CommentTextBox.Text, _system, GetChat(), _toForwardMessages, true);
                    ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);

                    page.SendBut.PreviewMouseDown += (sender, e) =>
                    {
                        bool isBand = page.GroupItems.IsChecked is null ? false : (bool)page.GroupItems.IsChecked;

                        List<string> paths = page.GetPathsFromMedias();

                        List<MediaAction> medias = new List<MediaAction>();
                        for (int i = 0; i < paths.Count; i++)
                        {
                            medias.Add(new MediaAction(-1, _system.LoggedUser.Id, DateTime.Now.AddDays(1), Path.GetFileName(paths[i]), false, false, false, null));
                        }

                        SetScheduleMessage message =
                            new SetScheduleMessage(GetChat(),
                            medias.Cast<Message>().ToList(), _system,
                            _toForwardMessages,
                            isBandMessages: isBand);

                        ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(message);
                    };
                    return;
                }

                AddMediaPage(names.ToList(), CommentTextBox.Text);
                CommentTextBox.Text = string.Empty;
            }
        }

        public void SetMediaSchedMessage(string path, bool isSticker, string name)
        {
            Message media = new MediaAction(_chat.Messages.Count,
                _system.LoggedUser.Id, DateTime.Now, name, isSticker, false, false, null);

            SetScheduleMessage sched = new SetScheduleMessage(_chat,
                new List<Message>() { media }, _system, _toForwardMessages, false);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(sched);
        }

        public void SetStickerSchedMessage(TelegramLib.MainClasses.Messages.MediaAction sticker)
        {

        }

        public void AddMediaPage(List<string> fullMediaPath, string text)
        {
            ((MainWindow)Window.GetWindow(this)).AddAddMediaPage(fullMediaPath, text, GetChat(), GetToForwardMessages());
        }

        public async Task AddImageMessage(string filePath, bool isSticker,
            string senderImageName, MediaAction mediaMes)
        {
            //AddStickerMessage()

            BitmapImage bitmapImage = ApiService.GetCachedBitmap(filePath);

            var img = new Image
            {
                Source = bitmapImage is not null ? bitmapImage : SignalRHelperService.LoadBitmap(filePath)// new BitmapImage(new Uri(filePath, UriKind.Absolute)),
            };

            AddMediaIntoMediasFolder(filePath);
            await AddImageMessage(img, isSticker, senderImageName, mediaMes);
        }

        public void AddMediaIntoMediasFolder(string filePath)
        {
            return;
            //Is image is contains in user chat folder
            if (!FilesAction.IsUserChatMediaIsExist(Path.GetFileName(filePath)))
            {
                //FilesAction.CopyImageToImageFolder(filePath);
            }
        }

        public async Task<bool> AddMediaPath(string filePath,
            bool isSticker = false, bool isAdd = true)
        {
            string fileName = Path.GetFileName(filePath);

            Message newMediaMes = new MediaAction(-1, _system.LoggedUser.Id,
                DateTime.Now, fileName, isSticker, false, false, null);

            if (isAdd)
            {
                newMediaMes = await GetAndAddMessage(newMediaMes);

                _chatMessages.Add(newMediaMes);

                await SendMessageToReceiver(new List<Message>() { newMediaMes });
                return true;
            }
            return false;
        }

        public void AddMediaElement(string filePath, string senderImageName, MediaAction mes)
        {
            var media = new MediaElement
            {
                Source = new Uri(filePath, UriKind.Absolute),
                /*                Width = 300,
                                Height = 200,*/
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            media.Stop();

            /*            //Is video is contains in user chat folder
                        if (!FilesAction.IsVideoIsExistInSecFolder(Path.GetFileName(filePath)))
                        {
                            FilesAction.CopyVideoToVideoFolder(filePath);
                        }*/

            AddVideoMessage(media, senderImageName, mes);
        }

        public void SendGif(string gifPath, string senderImageName,
            bool isAdd = true, MediaAction mes = null)
        {
            if (SchedueleMessagesGrid.Visibility == Visibility.Visible &&
                mes is null)
            {
                string fileName = Path.GetFileName(gifPath);
                SetMediaSchedMessage(fileName, false, fileName);
                return;
            }

            DateTime sentDate = mes is null ? DateTime.Now : mes.SentTime;
            int? forwardedSenderId = mes is null ? null : mes.ForwardedFromId;

            var message = new MediaMessage(_system, gifPath, senderImageName, sentDate,
                _forwardSenderId = forwardedSenderId);

            message.GifImage.MouseLeftButtonDown += ChatGif_PreviewMouseDown;

            if (isAdd) Task.Run(() => AddMediaPath(gifPath, isAdd: isAdd)).Wait();
            if (mes is null) mes = (MediaAction)_chatMessages.Last();

            if (_system.LoggedUser.Id == mes.SenderUserId)
            {
                string tickVis = mes.IsRead ? _readIconName : _unreadIconName;
                message.SetTickVis(tickVis, mes.SenderUserId == _system.LoggedUser.Id);
            }

            message.Tag = mes.Id.ToString();

            ListBoxItem item = new ListBoxItem()
            {
                Content = message,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Tag = mes.Id.ToString()
            };
            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, mes);

            if (mes is not null) SetMediaForwardButVis(mes, message);

            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);

            //ScrollChatToEnd();
            ScrollToNewMessage();
        }


        private async void ChatGif_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedMessesGrid.Visibility == Visibility.Visible) return;

            var mediaParent = VisHelper.FindParent<MediaMessage>(sender as DependencyObject);
            if (mediaParent is not MediaMessage) return;

            MediaMessage message = mediaParent as MediaMessage;

            int.TryParse(message.Tag.ToString(), out int chosenMesId);

            List<string> baseGifPaths = await GetChatMediaPaths(MediaType.Gif);

            baseGifPaths = FilesAction.GetFullGifPaths(baseGifPaths);
            List<MediaAction> gifs = _chat.GetGifMessages(isSched: SchedueleMessagesGrid.Visibility == Visibility.Visible);

            int chosenMesIndex = gifs.FindIndex(x => x.Id == chosenMesId);

            MediaWindow mediaWindow = new MediaWindow(
                null, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.Gif, _system);

            mediaWindow.SetGif(chosenMesIndex, baseGifPaths, gifs,
                SchedueleMessagesGrid.Visibility == Visibility.Visible);
            mediaWindow.Show();

            /*
                        VisualActionPage page = new VisualActionPage(message.GetGifPath(), GetChatMediaPaths(MediaType.Gif));

                        ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);

                        VisualActionPageParams(message, MediaType.Gif, page);*/
        }

        private void AddVideoMessage(MediaElement el, string senderImageName,
            MediaAction mes)
        {
            var video = new MediaMessage(_system, el, senderImageName, mes, mes.ForwardedFromId);
            video.BigGrid.PreviewMouseLeftButtonDown += ChatVideo_PreviewMouseDown;

            if (mes.SenderUserId == _system.LoggedUser.Id)
            {
                string tickVis = mes.IsRead ? _readIconName : _unreadIconName;
                video.SetTickVis(tickVis, true);
            }

            ListBoxItem item = new ListBoxItem()
            {
                Content = video,
                // Width = 250,
                Tag = mes.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, mes);

            //SetMessagePositionSettings(item);
            item.Tag = mes.Id;
            video.Tag = mes.Id;

            SetMediaForwardButVis(mes, video);

            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);

            //Check is via signalR
            //ScrollToNewMessage();
            //ScrollChatToEnd();
        }

        private async void ChatVideo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedMessesGrid.Visibility == Visibility.Visible) return;

            var mediaParent = VisHelper.FindParent<MediaMessage>(sender as DependencyObject);
            if (mediaParent is not MediaMessage) return;

            MediaMessage message = mediaParent as MediaMessage;

            MediaElement videoElement = message.GetVideo();
            List<MediaAction> videos = GetChatMedias(MediaType.Video);// new List<MediaAction>();
                                                                      //GetChatMedias(MediaType.Video);// SetVideosInList(videos);

            List<string> allVideoPaths = await GetChatMediaPaths(MediaType.Video);

            //int chosenVideoIndex = GetImageIndex(img);// _videoPaths.IndexOf(tag);

            MediaWindow mediaWindow = new MediaWindow(
                null, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.Videos, _system);

            mediaWindow.SetVideos(videoElement, allVideoPaths, videos, isShed: SchedueleMessagesGrid.Visibility == Visibility.Visible);
            mediaWindow.Show();
        }

        public void SetVideosInList(List<MediaAction> videos)
        {
            if (SchedueleMessagesGrid.Visibility == Visibility.Visible && _chat is not null)
            {
                videos.AddRange(_chat.GetSchedVideos());
            }
            else videos.AddRange(_system.GetAllVideoMessages());
        }

        public void VisualActionPageParams(MediaMessage mediaMes, MediaType type,
            VisualActionPage page)
        {
            List<MediaAction> elements =
                FilesAction.GetMediaElementsFromListByType(_chat.GetMediaMessages(), type);

            int chosenVideoIndex = GetChosenVideoIndex(mediaMes, elements);
            if (chosenVideoIndex == -1) return;

            page.SetUserChat(_system, elements, chosenVideoIndex, _chat);
        }

        public int GetChosenVideoIndex(MediaMessage message, List<MediaAction> videos)
        {
            ListBoxItem item = ChatBox.Items.OfType<ListBoxItem>().FirstOrDefault(x => x.Content == message);
            if (item is null) return -1;

            int messageItemIndex = ChatBox.Items.IndexOf(item);
            if (messageItemIndex == -1) return -1;


            return videos.IndexOf((MediaAction)_chat.Messages[messageItemIndex]);
        }

        public async Task<List<string>> GetChatMediaPaths(MediaType type)
        {
            List<string> res = new List<string>();
            for (int i = 0; i < _chatMessages.Count; i++)
            {
                if (_chatMessages[i] is MediaAction media &&
                    FilesAction.GetMediaTypeFromFilename(media.MediaName) == type)
                {
                    string path = await FilesAction.GetFilePathByMediaType(type, media.MediaName);
                    res.Add(path);
                }
            }
            return res;
        }

        public List<MediaAction> GetChatMedias(MediaType type)
        {
            List<MediaAction> res = new List<MediaAction>();
            for (int i = 0; i < _chatMessages.Count; i++)
            {
                if (_chatMessages[i] is MediaAction media &&
                    FilesAction.GetMediaTypeFromFilename(media.MediaName) == type)
                {
                    res.Add(media);
                }
            }
            return res;
        }

        private bool _isGetStickers;

        public async Task AddImageMessage(Image img, bool isSticker, string senderImgName,
            MediaAction media)
        {
            var message = new MediaMessage(_system, img, isSticker,
                senderImgName, media.SentTime,
                forwardedFromId: media.ForwardedFromId);

            message.Tag = media.Id;

            message.ImageBorder.MouseLeftButtonDown += (sender, e) =>
            {
                if (SelectedMessesGrid.Visibility == Visibility.Visible) return;
                ChatImage_MouseDown(message);
            };

            //Set tick vis
            SetMediaTickVis(media, message);

            ListBoxItem item = new ListBoxItem()
            {
                Content = message,
                Tag = media.Id.ToString(),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, media);

            SetMediaForwardButVis(media, message);

            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);
            await SetSenderImageByListBoxItem(item, _system.GetUserById(media.SenderUserId),
                _system.LoggedUser.Id == media.SenderUserId);

            //Check is via signalR
            //ScrollToNewMessage();
            //ScrollChatToEnd();

            ScrollToNewMessage();
        }

        public void SetMediaTickVis(MediaAction media, MediaMessage message)
        {
            if (_system.LoggedUser.Id != media.SenderUserId) return;

            string tickVis = media.IsRead ? _readIconName : _unreadIconName;
            message.SetTickVis(tickVis, media.SenderUserId == _system.LoggedUser.Id);
        }

        public async void AddStickerMessage(Image img, string senderImageName)
        {
            //AddImageMessage

            string fileName = FilesAction.GetStickerPathObjByName(img.Tag.ToString());
            img.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));


            var message = new MediaMessage(_system, img, true,
                senderImageName, DateTime.Now);

            if (SchedueleMessagesGrid.Visibility == Visibility.Visible)
            {
                SetMediaSchedMessage(fileName, true, fileName);
                return;
            }


            //message.PreviewMouseDown += ChatImage_PreviewMouseDown;
            ListBoxItem item = new ListBoxItem()
            {
                Content = message,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            SetChatItemEvents(item);

            //SetMessagePositionSettings(item);

            ChatBox.Items.Add(item);

            //TelegramLib.MainClasses.UserChat messages = _system.GetChosenChat();
            //_chat.AddSticker(img.Tag.ToString(), _system.LoggedUser.Id);

            await AddMediaPath(img.Tag.ToString(), true);

            MediaAction mediaAct = (MediaAction)_chatMessages.Last();
            SetPaddingToMessageItem(item, mediaAct);


            //Added new mes to _chatMessages
            item.Tag = mediaAct.Id;
            SetMediaTickVis(mediaAct, message);

            SetMessagesPosition(_isGluedToLeft);

            //Check is via signalR
            ScrollToNewMessage();
            //ScrollChatToEnd();
        }

        public void ChatImage_MouseDown(MediaMessage message)
        {
            //if (sender is not MediaMessage message) return;
            if (SelectedMessesGrid.Visibility == Visibility.Visible) return;

            _isGetStickers = message.IsSticker;
            if (_isGetStickers) return; //NO TO STICKERS

            ListBoxItem item = ChatBox.Items.OfType<ListBoxItem>()
                .Where(x => x.Content == message).First();
            int index = ChatBox.Items.IndexOf(item);

            List<MediaAction> imgMedias =
                _chat.GetMediaMessages(isSched: SchedueleMessagesGrid.Visibility == Visibility.Visible).
                Where(x => FilesAction.IsFileIsImage(x.MediaName)).ToList();

            if (index >= _chat.Messages.Count) return;

            int imgIndex = imgMedias.FindIndex(x => x == _chat.Messages[index] as MediaAction);


            int.TryParse(message.Tag.ToString(), out int chosenId);

            Message chosen = _system.GetMessageById(chosenId);

            MediaWindow mediaWindow = new MediaWindow(
                null, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.ChatImages, _system);

            mediaWindow.SetChatImageMessages(chosen, imgMedias);
            mediaWindow.Show();
        }


        /*        public List<Image> GetChatImages()
                {
                    List<Image> res = new List<Image>();

                    for (int i = 0; i < _chatMessages.Count; i++)
                    {
                        //For images (NO STIKER)
                        if (_chatMessages[i] is MediaAction media && media.IsSticker == _isGetStickers &&
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
                }*/

        public bool _isLoopPressed = false;
        private void FindMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //find message menu
            _isLoopPressed = true;

            Window window = Window.GetWindow(this);
            if (window is not MainWindow main || !main.GetIsOnlyChat()) return;

            SetOnlyChatVisState(true);

            ClearVisualStuffAfterBlocked();
        }

        public void SetOnlyChatVisState(bool isVis)
        {
            Visibility visState = isVis ? Visibility.Visible : Visibility.Hidden;

            OnlyChatSearchChatGrid.Visibility = visState;
            BottomOnlyChatGrid.Visibility = visState;
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
            MirrorUserChatInfoVisibility();
        }

        public void MirrorUserChatInfoVisibility()
        {
            if (UserInfoColumn.Width.Value == 0)
            {
                AddContactInfo();
                return;
            }
            RemoveRightContactInfo();
        }

        private int _userContactWidth = 450;
        public async Task AddContactInfo()
        {
            double windowWidth = ((MainWindow)Window.GetWindow(this)).ActualWidth;

            if (_isSavedMessageChat)
            {
                SavedChatMenu menu = new SavedChatMenu();
                menu.SetChatParam(_system.SavedMesesChat);
                ContactInfoGrid.Children.Add(menu);

                menu.ControlLoaded += () =>
                {
                    if (windowWidth + _userContactWidth <=
                        SystemParameters.PrimaryScreenWidth &&
                        !_mainWindow.GetIsLongContnetChatState())
                    {
                        _mainWindow.SetIsLongContnetChatState(true);
                        ((MainWindow)Window.GetWindow(this)).Width =
                            windowWidth + _userContactWidth;
                    }

                    menu.CloseButtonGrid.MouseDown += CloseContactInfo_MouseDown;

                    UserInfoColumn.Width = new GridLength(_userContactWidth);
                };

                return;
            }

            ContactInfo info = new ContactInfo();
            ContactInfoGrid.Children.Add(info);

            //info.SetContactInfo(_chat, _system, _system.GetContactByUserId(_chat.Chatter.Id)); /*_system.ChosenChatContact*/

            info.LoadEnd += () =>
            {
                if (windowWidth + _userContactWidth <=
                    SystemParameters.PrimaryScreenWidth &&
                    !_mainWindow.GetIsLongContnetChatState())
                {
                    MoveWindowIfNeed(windowWidth);

                    _mainWindow.SetIsLongContnetChatState(true);

                    ((MainWindow)Window.GetWindow(this)).Width =
                        windowWidth + _userContactWidth;
                }

                info.CloseButGrid.MouseDown += CloseContactInfo_MouseDown;

                UserInfoColumn.Width = new GridLength(_userContactWidth);
            };

            await info.SetContactInfo(_chat, _system,
                _system.GetContactByUserId(_chat.Chatter.Id), isSetMaxHeight: false);
        }

        public void MoveWindowIfNeed(double windowWidth)
        {
            const int secondLevel = 1500;
            if (secondLevel <= windowWidth) return;

            Window window = Window.GetWindow(this);
            double moveParam = SystemParameters.PrimaryScreenWidth - (window.Left + windowWidth + _userContactWidth);

            if (moveParam > 0) return;
            window.Left = window.Left - Math.Abs(moveParam);
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

            if (_mainWindow is null) return;

            if (_mainWindow.GetIsLongContnetChatState())
            {
                ((MainWindow)Window.GetWindow(this)).Width -= _userContactWidth;
            }

            _mainWindow.SetIsLongContnetChatState(false);
        }

        private const int _basicButWidth = 30;
        public void SetUserInfoButGrid(bool isVis)
        {
            UserInfoBut.Width = isVis ? _basicButWidth : 0;
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
            //if (_isSavedMessageChat) return;
            //show user menu
            HideChatMenuBlocks();
            UserChatMenu.Visibility = Visibility.Visible;
        }

        public void HideChatMenuBlocks()
        {
            UserChatMenu.HideBloksIfSavedChat(_isSavedMessageChat);
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
            SetUserInfo();
        }

        public void SetUserInfo()
        {
            Pages.UserInfo info =
                new Pages.UserInfo(_chat, _system);
            SetUserInfoPageHeight(info);

            info.ContactInfo.LoadEnd += () =>
            {
                var currentWindow = Window.GetWindow(this);
                if (currentWindow == null) return;

                if (currentWindow is MainWindow main)
                {
                    main.SetSecondaryFrame(info);
                }
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

            EmojisBoard.EmojisPanel.SetEmojisList();
            EmojisBoard.SetIsBlockMedias(SchedueleMessagesGrid.Visibility == Visibility.Visible);

            Emojis.Foreground = new SolidColorBrush(Colors.LightGray);
            Cursor = Cursors.Hand;
        }

        private void EmojisBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            EmojisBoard.Visibility = Visibility.Hidden;
            Cursor = null;
        }

        public async void ScrollToMessageByMessageId(int messageId)
        {
            if (messageId == -1)
            {
                ((MainWindow)Window.GetWindow(this)).SetTemporaryText("Misha, why did you click here???");
                return;
            }
            //Is replied from other chat
            if (await IsRepliedFromOtherChat(messageId)) return;

            ListBoxItem? item = GetChatListBoxItemByMessageId(messageId); /*ChatBox.Items
                .OfType<ListBoxItem>()
                .FirstOrDefault(x => x.Tag.ToString() == messageId.ToString());*/


            if (item is null) return;

            int index = ChatBox.Items.IndexOf(item);
            if (index == -1) return;

            this.Visibility = Visibility.Visible;

            ScrollToChosenItem(index);
        }

        public ListBoxItem GetChatListBoxItemByMessageId(int mesId)
        {
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;
                if (item.Tag is null)
                {
                    if (item.Content is MediaMessage media && media.IsBandBorderContainsId(mesId))
                    {
                        return item;
                    }
                }
                else if (item.Tag.ToString() == mesId.ToString())
                {
                    return item;
                }
            }

            return null;
        }

        public async ValueTask<bool> IsRepliedFromOtherChat(int mesId)
        {
            if (SchedueleMessagesGrid.Visibility == Visibility.Hidden &&
                 _chat.IsMessageContains(mesId)) return false;

            //Set chat
            TelegramLib.MainClasses.UserChat chat =
                _system.GetChatByMessageId(mesId);

            if (chat is null) return false;
            ((MainWindow)Window.GetWindow(this)).HideEnnesChat(chat.Id);

            HideChatControlFeatures();
            await SetUserChat(chat);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                ScrollToMessageByMessageId(mesId);
            }), DispatcherPriority.ApplicationIdle);

            return true;
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
            if (_chat is null) return;
            else if (_chat.ChatBg is null) _chat.ChatBg = new ChatBackground();

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
            else CustomBg.Effect = null;
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

        public async Task SendSignalRMessage(List<Message> messages)
        {
            //So now here is SENDERS chat
            //we need to send RECEIVERS chat to update it here
            //Where sender is receiver; receiver is sender

            //Get contact where senderId is friendId, UserId - receiverId
            UserContactcs contact =
                await ApiService.GetContactByUserAndFriendIds(_chat.Chatter.Id, _system.LoggedUser.Id);
            if (contact is null) return;

            if (_chat is null) return;

            //If send as band
            if (_system.IsSendingBand(messages))
            {
                await SignalRService.SendMediaMessage(_system.LoggedUser, messages.Cast<MediaAction>().ToList(), _chat.Chatter);
                return;
            }

            foreach (var message in messages)
            {
                if (message is TelegramLib.MainClasses.Messages.TextMessage text)
                {
                    await SignalRService.SendTextMessage(_system.LoggedUser, text, _chat.Chatter);
                }
                else if (message is TelegramLib.MainClasses.Messages.MediaAction media)
                {
                    await SignalRService.SendMediaMessage(_system.LoggedUser, new List<MediaAction>() { media }, _chat.Chatter);
                }
                else if (message is TelegramLib.MainClasses.Messages.ShareContactMessage share)
                {
                    await SignalRService.AddShareContactMessage(_system.LoggedUser, _chat.Chatter, share.SharedUser.Id);
                }
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

                Message? mes = _chatMessages.FirstOrDefault(x => x.Id == id);
                if (mes is null) return;
                if (mes.SenderUserId != _system.LoggedUser.Id) continue;

                SetMessagePositionSettings(item);
            }
        }

        public void SetMessagePositionSettings(ListBoxItem item)
        {
            if (item.Content is UserControl ctrl)
            {
                if (ctrl is MonthDay) return;
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
                else if (item.Content is ShareContactControl share)
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
            if (_chat is null || _chat.Chatter.Id != contact.ContactUserId) return;

            ChatFriendLogin.Text = contact.Name;
            ChatFriendSurname.Text = contact.Surname;
        }

        public void SetNameSurnameInUserParams()
        {
            ChatFriendLogin.Text = _chat.Chatter.Name;
            ChatFriendSurname.Text = _chat.Chatter.Surname;
        }

        private void BottomGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is Grid grid)
            {
                grid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
            }
        }

        private void BottomGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is Grid grid)
            {
                grid.Background =
                    (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
            }
        }

        private void UnblockGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ApiService.RemoveBlockedContact(_system.LoggedUser.Id, _chat.GetChatter().Id);

            _system.LoggedUser.UnblockUserById(_chat.Chatter.Id);
            UnBlockBorder.Visibility = Visibility.Hidden;

            //Update Chat contact info
            UpdateContactInfoBlock();

            ((MainWindow)Window.GetWindow(this)).SetBlockedUserVisParams(false, _chat.GetChatter());
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

        private async Task SetBandMessage(List<MediaAction> medias)
        {
            List<string> paths = medias.Select(x => x.MediaName).ToList();
            if (paths is null && paths.Count == 0) return;

            MediaMessage bandMessage = new MediaMessage(_system, paths);

            //Set tag to every band image border
            bandMessage.SetTagIdsToBandBorders(medias);
            bandMessage.SetTime(medias[0].SentTime);

            //Add in vis
            await AddBandImgMessage(bandMessage, medias);

            ScrollToNewMessage();
        }

        private const int maxMediasInBand = 9;
        private async Task CreateBandMessage(List<string> paths)
        {
            for (int i = 0; i < paths.Count; i += maxMediasInBand)
            {
                int count = Math.Min(maxMediasInBand, paths.Count - i);
                List<string> ninePaths = paths.GetRange(i, count);

                MediaMessage bandMessage = new MediaMessage(_system, ninePaths);

                //Get last band id 
                int lastBandId = await ApiService.GetLastMessageBandId();

                //Adding in system + DB
                List<MediaAction> bandMessages = new List<MediaAction>();
                for (int j = 0; j < ninePaths.Count; j++)
                {
                    Message newMediaMes = new MediaAction(-1, _system.LoggedUser.Id,
                        DateTime.Now, ninePaths[j], false, false, false, null, lastBandId + 1);
                    newMediaMes.IsRead = true;

                    AddMediaIntoMediasFolder(ninePaths[j]);
                    newMediaMes = await GetAndAddMessage(newMediaMes);

                    bandMessages.Add((MediaAction)newMediaMes);
                    _chatMessages.Add(newMediaMes);
                }
                await SendMessageToReceiver(bandMessages.Cast<Message>().ToList()); //check this thing

                bandMessage.SetTime(DateTime.Now);

                //Set tag to every band image border
                bandMessage.SetTagIdsToBandBorders(bandMessages);

                //Add in vis
                await AddBandImgMessage(bandMessage, bandMessages);
            }
            ScrollToNewMessage();
        }

        public async Task AddBigMediaImagesMessage(string capture,
            List<Image> imgs, List<string> paths, SendMediaType type)
        {
            await AddDateStatMessage();

            if (!string.IsNullOrEmpty(capture))
            {
                await AddTextMessageControl(_system.LoggedUser.GetFirstImageName().Name, capture);
            }

            ScrollToNewMessage();

            if (/*(paths.Count == 1 && FilesAction.IsFileIsVideo(paths[0])) || 
                */(paths.Count > 1 && type == SendMediaType.Group))
            {
                //Divide in groups of 9(if more than nine)
                if (paths.Count > maxMediasInBand)
                {
                    for (int i = 0; i < paths.Count; i += maxMediasInBand)
                    {
                        int count = Math.Min(maxMediasInBand, paths.Count - i);
                        List<string> ninePaths = paths.GetRange(i, count);

                        await CreateBandMessage(ninePaths);
                    }
                }
                else await CreateBandMessage(paths);
            }
            else
            {
                foreach (var img in imgs)
                {
                    string fullPath = img.Tag.ToString();
                    bool isAdd = await AddMediaPath(fullPath);

                    if (isAdd)
                    {
                        MediaAction toCheck = (MediaAction)_chatMessages.Last();

                        if (FilesAction.IsFileIsVideo(fullPath))
                        {
                            await SetMediaMessageInChat(toCheck, _system.LoggedUser.GetFirstImageNameInString());
                        }
                        else
                        {
                            await AddImageMessage(fullPath, false,
                                _system.LoggedUser.GetFirstImageName().Name, toCheck);
                        }
                        ScrollToNewMessage();
                    }
                }
            }
            UpdateContactInfoBlock();

            ((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();
            HideSelectionRowFromSignalR(GetSelectedMessages());
            UpdateGlobalMedias();
        }

        public async Task AddBandImgMessage(MediaMessage bandMessage, List<MediaAction> medias)
        {
            // think about tag etc...
            // message.Tag = media.Id;

            MediaAction media = medias.First();

            for (int i = 0; i < bandMessage._bandBorders.Count; i++)
            {
                int localIndex = i;
                bandMessage._bandBorders[i].PreviewMouseLeftButtonDown += (sender, e) =>
                {
                    //Set Selection action
                    if (SelectedMessesGrid.Visibility == Visibility.Visible)
                    {
                        if (bandMessage._bandBorders[localIndex].Tag is null) return;
                        int.TryParse(bandMessage._bandBorders[localIndex].Tag.ToString(), out int id);

                        bandMessage.MirrorSelectionById(id);
                        return;
                    };

                    //If its selection action -> return
                    #region //There is action to show band media window

                    ListBoxItem item = ChatBox.Items.OfType<ListBoxItem>()
                        .Where(x => x.Content == bandMessage).First();
                    int index = ChatBox.Items.IndexOf(item);

                    //int imgIndex = imgMedias.FindIndex(x => x == _chat.Messages[index] as MediaAction);

                    if (bandMessage._bandBorders[localIndex].Tag is null) return;

                    int.TryParse(bandMessage._bandBorders[localIndex].Tag.ToString(), out int chosenId);
                    Message chosen = _system.GetMessageById(chosenId);

                    bool isImage = true;

                    if (chosen is MediaAction media)
                    {
                        string exts = Path.GetExtension(media.MediaName);
                        isImage = exts == ".mp4" ? false : true; //Set other video exts here 
                    }

                    List<MediaAction> imgMedias =
                        _chat.GetMediaMessages(isSched: SchedueleMessagesGrid.Visibility == Visibility.Visible).
                        Where(x => (isImage ? FilesAction.IsFileIsImage(x.MediaName) : FilesAction.IsFileIsVideo(x.MediaName))).ToList();

                    MediaShowType type = isImage ? MediaShowType.ChatImages : MediaShowType.Videos;

                    MediaWindow mediaWindow = new MediaWindow(
                        null, (MainWindow)Window.GetWindow(this),
                        type, _system);

                    if (type == MediaShowType.ChatImages)
                    {
                        mediaWindow.SetChatImageMessages(chosen, imgMedias);
                    }
                    else if (type == MediaShowType.Videos)
                    {
                        MediaElement videoElement = FilesAction.GetMediaElementByVideoName(Path.GetFileName(((MediaAction)chosen).MediaName));
                        mediaWindow.SetVideos(videoElement, imgMedias.Select(x => x.MediaName).ToList(), imgMedias, ScheduleMessageGrid.Visibility == Visibility.Visible);
                    }


                    mediaWindow.Show();
                    #endregion
                };
            }

            //Set tick vis
            SetMediaTickVis(medias.First(), bandMessage);

            ListBoxItem item = new ListBoxItem()
            {
                Content = bandMessage,
                //HorizontalAlignment = HorizontalAlignment.Stretch
            };

            //mb ok
            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, media);

            SetMediaForwardButVis(media, bandMessage);

            ChatBox.Items.Add(item);
            SetMessagesPosition(_isGluedToLeft);
            await SetSenderImageByListBoxItem(item, _system.GetUserById(media.SenderUserId), _system.LoggedUser.Id == media.SenderUserId);

            //Check is via signalR
            //ScrollChatToEnd();
            ScrollToNewMessage();
        }


        private void ClearAllMediaWindows()
        {
            Window window = Window.GetWindow(this);

            if (window is not null && window is MainWindow main)
            {
                main.CloseAllMediaWindows();
            }
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
            SetChatItemEvents(item);
            SetPaddingToMessageItem(item, mes);

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

            ScrollToNewMessage();
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
                    await SetChatterMessagesReadStatus();
                }
            }

            //Send signalR To set message as read
            //var objsInView = Helper.VisHelper.GetVisibleItems(ChatBox);

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
            if (_chat.Chatter is null) return;
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
                            if (media.Tag is not null)
                            {
                                int.TryParse(item.Tag.ToString(), out int id);
                                res.Add(id);
                            }
                            else if (media.IsBandMedia())
                            {
                                List<int> ids = media.GetBandMessagesIds();
                                res.AddRange(ids);
                            }
                        }
                    }
                }
            });
            return res;
        }

        public async void UpdateReadStatus(TelegramLib.MainClasses.User chatter)
        {
            if (_system is null) return;
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
                    await ApiService.SetReadStatus(messages[i].Id);

                    //Get and change status from db
                    bool updatedStatus =
                        ApiService.GetMessageReadStatus(messages[i].Id).Result;

                    messages[i].IsRead = updatedStatus;

                    //Set for band message
                }
            }
            UpdateReadStatus(chat);

            //Update UserTalkControl read tick
            UpdateTalkMessageTickStatus(chat);
        }

        public void UpdateTalkMessageTickStatus(TelegramLib.MainClasses.UserChat chat)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                //if (chat is null || chat.Chatter is null || chat.GetLastMessage() is null) return;

                //need main thread...bruh...
                Window window = Window.GetWindow(this);
                if (window is MainWindow main)
                {
                    main.UpdateUserTalkTickStatus(chat);
                }
            });
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
                        if (mediaImg.Tag is null)
                        {
                            if (mediaImg.IsBandMedia())
                            {
                                List<int> ids = mediaImg.GetBandMessagesIds();

                                bool isTick = true;
                                for (int j = 0; j < ids.Count; j++)
                                {
                                    Message? mes = chat.Messages.FirstOrDefault(x => x.Id == ids[j]);
                                    if (mes is not null && !mes.IsRead)
                                    {
                                        isTick = false;
                                        break;
                                    }
                                }

                                Message? toPutIn = chat.Messages.FirstOrDefault(x => x.Id == ids[0]);
                                if (toPutIn is not null && isTick)
                                {
                                    SetVisualReadIconKind(item, toPutIn);
                                }
                            }
                            continue;
                        }

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
            else if (item.Content is MediaMessage mediaImg)
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
            if (this.Visibility == Visibility.Hidden || _isSavedMessageChat) return;

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
            if (_isEdit)
            {
                _isEdit = false;
                CommentTextBox.Text = string.Empty;
            }
            ReplyMessageRow.Height = new GridLength(0);
            _toForwardMessages = null;
            Cursor = null;
        }

        public void SetReplyMessageRow()
        {
            ClearSelectionRow();
            HideSelectionRow();
            ClearReplyRowParams();

            //Get ListBoxItem From Menu
            ListBoxItem item = _mesMenu.GetChosenListBoxItem();  //GetListBoxItemFromMenu();
            if (item is null || item.Content is not UserControl control) return;

            //item.Tag == message id; Get message by system
            Message mes = _mesMenu.GetMessage();
            if (mes is null) mes = GetMessageByListBoxTag(item);

            if (mes is null) return;

            SetReplyRowParams(control, new List<Message>() { mes });
        }


        private TelegramLib.MainClasses.Messages.Message _repliedMessage;
        public async void SetReplyRowParams(UserControl control,
                List<Message> messages, bool isResend = false)
        {
            if (messages is null) return;
            ReplyMessageRow.Height = new GridLength(50);

            //Set Image to reply
            if (control is not null &&
                control is MediaMessage media)
            {
                ReplyedImageColumn.Width = new GridLength(50);

                if (media.IsBandMedia())
                {
                    //Check is Image
                    Message mes = messages.First();

                    if (mes is MediaAction mesMedia && FilesAction.IsFileIsVideo(mesMedia.MediaName))
                    {

                        Image firstVideoFrame = await VisHelper.GetFirstFrameAsync(mesMedia.MediaName);
                        if (firstVideoFrame is null) return;

                        ReplyedImage.Source = firstVideoFrame.Source;
                        //return;
                    }
                    else
                    {
                        string path = media.GetImageBorderSource(messages.First().Id);
                        //for image

                        ReplyedImage.Source = ApiService.GetCachedBitmap(path);
                        //ReplyedImage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                    }
                }
                else if (media._img is not null) ReplyedImage.Source = media._img.Source;
                else if (media._gifPath is not null &&
                    media._gifPath != string.Empty)
                {
                    ReplyedImage.Source = FilesAction.GetFirstImageFromGif(media._gifPath);
                }
                else
                {

                    if (messages.Count == 0 || messages.First() is not MediaAction video) return;
                    //Image img = await VisHelper.GetFirstFrameAsync(video.MediaName);
                    string fullPath = await ApiService.GetVideoPreviewPath(video.MediaName);

                    if (fullPath is not null && fullPath != string.Empty)
                    {
                        Image firstVideoFrame = await VisHelper.GetFirstFrameAsync(fullPath);
                        ReplyedImage.Source = firstVideoFrame.Source;
                    }
                    //ReplyedImage.Source = ApiService.GetCashedBitmap(video.MediaName);/*is BitmapImage bit and not null ? bit : img.Source;*/
                }
            }
            else
            {
                ReplyedImageColumn.Width = new GridLength(0);
                ReplyedImage.Source = null;
            }

            _repliedMessage = messages.First();

            string quoteText = string.Empty;
            if (control is ChatControls.TextMessage textControl &&
                textControl.SelectableText.SelectedText != string.Empty)
            {
                _repliedMessage.SetQuoteText(textControl.SelectableText.SelectedText);

                quoteText = textControl.SelectableText.SelectedText;
                textControl.SelectableText.Select(0, 0);
            }
            else if (/*!_isSavedMessageChat ||*/ !isResend)
            {
                _repliedMessage.RepliedQuote = string.Empty;
            }

            string actionName = _toForwardMessages is null ? "Reply to" : "Forwarded from";

            //Set sender name
            ReplySenderText.Text = $"{actionName} {_system.GetMessageSender(_repliedMessage.SenderUserId).Login}";

            //Set text
            ReplyedMessageText.Text =
                _toForwardMessages is not null && messages.Count == 1 ? "1 message" :
                (messages.Count > 1 || _toForwardMessages is not null) ? $"{messages.Count} messages" :
                _repliedMessage is MediaAction ? "Reply media" :

                _repliedMessage is TelegramLib.MainClasses.Messages.TextMessage text ? _repliedMessage.RepliedQuote == string.Empty ? text.Text : _repliedMessage.RepliedQuote :

                _repliedMessage is TelegramLib.MainClasses.Messages.ShareContactMessage share ? "Contact" :
                "Some shit";
        }

        public void ClearReplyRowParams()
        {
            _toForwardMessages = null;
            _repliedMessage = null;
        }

        public ListBoxItem? GetListBoxItemFromMenu()
        {
            MesMenu? menu = MessageMenu.Children.OfType<MesMenu>().FirstOrDefault();
            return menu is null ? null : menu.GetChosenListBoxItem();
        }

        public Message GetMessageByListBoxTag(ListBoxItem item)
        {
            if (item.Tag is null) return null;
            int.TryParse(item.Tag.ToString(), out int id);
            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(id);
            return mes;
        }

        public List<MediaAction> GetMessagesFromBandMessage(ListBoxItem item)
        {
            List<MediaAction> res = new List<MediaAction>();
            if (item.Content is not MediaMessage media) return res;

            List<int> mesIds = media.GetBandMessagesIds();

            foreach (var id in mesIds)
            {
                Message mes = _system.GetMessageById(id);
                if (mes is not null && mes is MediaAction act) res.Add(act);
            }

            return res;
        }

        public ListBoxItem? GetChatItemByMessageId(int id)
        {
            return ChatBox.Items
                .OfType<ListBoxItem>()
                .FirstOrDefault(x => x.Tag is not null && x.Tag.ToString() == id.ToString());
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
            //Get is need to set stop img
            IEnumerable<ListBoxItem> items = ChatBox.Items.OfType<ListBoxItem>();

            foreach (var item in items)
            {
                if (item.Tag is null && item.Content is MediaMessage media &&
                    media.IsBandMedia())
                {
                    List<int> ids = media.GetBandMessagesIds();

                    for (int i = 0; i < ids.Count; i++)
                    {
                        Message? mes = _chat.Messages.FirstOrDefault(x => x.Id == ids[i]);
                        if (mes is null || mes.SenderUserId == _system.LoggedUser.Id) continue;

                        await SetSenderImageByListBoxItem(item, _system.GetUserById(mes.SenderUserId));
                        break;
                    }
                }
                else if (item.Tag is not null)
                {
                    int.TryParse(item.Tag.ToString(), out int itemTag);
                    Message? mes = _chat.Messages.FirstOrDefault(x => x.Id == itemTag);
                    if (mes is null || mes.SenderUserId == _system.LoggedUser.Id) continue;

                    await SetSenderImageByListBoxItem(item, _system.GetUserById(mes.SenderUserId));
                }
            }
        }

        public async Task SetSenderImageByListBoxItem(ListBoxItem item,
            TelegramLib.MainClasses.User user, bool isChatter = true)
        {
            if (/*!isChatter ||*/ user is null) return;
            //chat.Chatter => _system.LoggedUser
            if (item.Content is ChatControls.TextMessage text)
            {
                await SignalRHelperService.SetPhotoInEllipse(user,
                    text.BgBrush, text.UserEllipseImage);
            }
            else if (item.Content is ChatControls.MediaMessage media)
            {
                await SignalRHelperService.SetPhotoInEllipse(user,
                    media.BgBrush, media.UserEllipseImage);
            }
            else if (item.Content is ShareContactControl share)
            {
                await SignalRHelperService.SetPhotoInEllipse(user,
                    share.BgBrush, share.SenderEllipseImage);

                await SignalRHelperService.SetPhotoInEllipse(user,
                    share.ImageIcon, share.UserEllipseImage);
            }
        }

        public void SetForwardMessages(int userToSendId, List<Message> messages)
        {
            _forwardSenderId = userToSendId;
            if (_toForwardMessages is null) _toForwardMessages = new List<Message>();
            else _toForwardMessages.Clear();

            _toForwardMessages.AddRange(messages);
        }

        public void HideChatControlFeatures()
        {
            //SetForwardMessages(userIdToSend, messages);
            HidePinnedChatAndShowChatMessages();

            //Hide selection stuff
            HideSelectionRow();
            SetMessageSelectCircleVis(false);

            SchedueleMessagesGrid.Visibility = Visibility.Hidden;
        }

        public async Task SetForwardedMessage(
            List<Message> messages,
            int? userIdToSend)
        {
            if (messages.Count == 0) return;
            HideChatControlFeatures();

            //Get control
            UserControl? messageControl = messages.Count > 1 ? null :
                GetMessageControlById(messages.First().Id);

            _toForwardMessages = await GetListOfMessagesToSendForward(messages, userIdToSend);
            ClearForwardMessagesFromReply();

            _forwardSenderId = userIdToSend;

            //To set in Saved messages
            if (userIdToSend is null)
            {
                await ToSendForwardedMessageInSave();
                return;
            }

            //Get temp active chat(From user chat control)
            TelegramLib.MainClasses.UserChat chat =
                _forwardSenderId is null ? _system.GetSavedChatMessages() :
                _system.GetChatByChatterId((int)userIdToSend);
            if (chat is null) return;

            if ((_forwardSenderId is not null && chat.Id != _chat.Id) ||
                chat.GetType() != _chat.GetType())
                await ((MainWindow)Window.GetWindow(this)).
                    SetOtherChatByUserId(chat.Chatter.Id);

            _chat = chat;

            //Set reply row in chat    
            SetReplyRowParams(messageControl, messages);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focusable = true;
                this.Focus();
                Keyboard.Focus(this);

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        public async Task ToSendForwardedMessageInSave()
        {
            //Add message is in save
            //Add in system

            for (int i = 0; i < _toForwardMessages.Count; i++)
            {
                await ApiService.AddSavedMessage(_system.SavedMesesChat.Id, _toForwardMessages[i]);

                Message newMes = await ApiService.GetLastSavedMessage(_system.SavedMesesChat.Id);
                _system.SavedMesesChat.Messages.Add(newMes);
            }

            ((MainWindow)Window.GetWindow(this)).SetTemporaryText("Sent to save chat");
        }

        public void ClearForwardMessagesFromReply()
        {
            for (int i = 0; i < _toForwardMessages.Count; i++)
            {
                if (_toForwardMessages[i] is TelegramLib.MainClasses.Messages.TextMessage text)
                {
                    text.RepliedMessageId = null;
                }
            }
        }

        public async Task<List<Message>> GetListOfMessagesToSendForward(
            List<Message> toConvert, int? toSendId)
        {
            List<Message> res = new List<Message>();

            for (int i = 0; i < toConvert.Count; i++)
            {
                //Get copy of message to forward
                TelegramLib.MainClasses.Messages.Message copy =
                (TelegramLib.MainClasses.Messages.Message)DeepCopy(toConvert[i]);

                copy.ForwardedFromId = toConvert[i].SenderUserId;

                //To check this

                //if (toSendId is not null) copy.SenderUserId = _system.LoggedUser.Id;
                copy.SenderUserId = toSendId is not null ? _system.LoggedUser.Id : copy.SenderUserId;

                copy.IsPinned = false;
                copy.IsRead = false;
                copy.SentTime = DateTime.Now;


                if (copy is TelegramLib.MainClasses.Messages.TextMessage text)
                    text.RepliedMessageId = null;
                res.Add(copy);
            }

            await ChangeSelectedMedias(res);
            return res;
        }

        public UserControl? GetMessageControlById(int id)
        {
            ListBoxItem? item = GetChatListBoxItemByMessageId(id); /*ChatBox.Items
                .OfType<ListBoxItem>()
                .FirstOrDefault(x => x.Tag.ToString() == id.ToString());*/

            return item is null || item.Content is null ? null
                : item.Content is UserControl control ? control : null;
        }

        public async void SendForwardMessageInSignalR(TelegramLib.MainClasses.UserChat chat,
                List<TelegramLib.MainClasses.Messages.Message> mes)
        {
            if (mes.Count == 1 && mes[0] is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                await SignalRService.SendTextMessage(_system.LoggedUser, text, chat.Chatter);
            }
            else if (mes.Count >= 1 && mes[0] is TelegramLib.MainClasses.Messages.MediaAction media)
            {
                await SignalRService.SendMediaMessage(_system.LoggedUser, mes.Cast<MediaAction>().ToList(), chat.Chatter);
            }
            else if (mes.Count == 1 && mes[0] is TelegramLib.MainClasses.Messages.ShareContactMessage share)
            {
                await SignalRService.AddShareContactMessage(_system.LoggedUser, _chat.Chatter, share.SharedUser.Id);
            }
        }

        public static object DeepCopy(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var type = obj.GetType();
            return JsonConvert.DeserializeObject(json, type);
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ClearSelectionRow();
        }

        public void ClearSelectionRow()
        {
            SelectedMessesGrid.Visibility = Visibility.Hidden;

            SetMessageSelectCircleVis(false);
            ClearAllSelectedBands();

            ChatterInfoGrid.Visibility = Visibility.Visible;
        }

        public void ClearAllSelectedBands()
        {
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;

                if (item.Content is MediaMessage band && band.IsBandMedia())
                {
                    band.SetBandSelection(false);
                }

            }
        }

        public void SetMessageSelectCircleVis(bool isVis)
        {
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;

                if (item.Content is ChatControls.TextMessage text)
                {
                    text.SelectionTickObj.SetChosenParam(false);
                    text.SetTickVisibility(isVis);
                }
                else if (item.Content is ChatControls.MediaMessage media)
                {
                    media.SelectionTickObj.SetChosenParam(false);
                    media.SetTickVisibility(isVis);
                }
                else if (item.Content is ShareContactControl share)
                {
                    share.SelectionTickObj.SetChosenParam(false);
                    share.SetTickVisibility(isVis);
                }
            }
        }

        public void SetMessageSelectByOnlyTickCol(bool isVis)
        {
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;

                if (item.Content is ChatControls.TextMessage text)
                {
                    text.SelectionTickObj.SetChosenParam(false);
                    text.SetTickVisOnlyTickCol(isVis);
                }
                else if (item.Content is ChatControls.MediaMessage media)
                {
                    media.SelectionTickObj.SetChosenParam(false);
                    media.SetTickVisOnlyTockCol(isVis);
                }
                else if (item.Content is ShareContactControl share)
                {
                    share.SelectionTickObj.SetChosenParam(false);
                    share.SetTickVisibility(isVis);
                }
            }
        }

        private List<Message> _toForwardMessages;
        private int? _forwardSenderId;

        public List<Message> GetToForwardMessages()
        {
            return _toForwardMessages;
        }

        private async void ForwardSelectedBut_Click(object sender, RoutedEventArgs e)
        {
            if (ForwardSelectedButText.Text == _ifSendNowSelected)
            {
                List<Message> selected = GetSelectedMessages();

                foreach (var mes in selected)
                {
                    await SendSchedMessageNow(mes);
                }

                ClearSelectionRow();
                SchedueleMessagesGrid.Visibility = Visibility.Visible;
                return;
            }

            ForwardToPage page = new ForwardToPage(_system);
            page.ForwardSelected += async (senderId) =>
            {
                HidePinnedChatAndShowChatMessages();

                //Set reply row
                List<Message> selected = GetSelectedMessages();

                HideSelectionRow();

                await SetForwardedMessage(selected, senderId);

                CommentTextBox.Focus();
            };
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public async Task ChangeSelectedMedias(List<Message> messages)
        {
            List<MediaAction> medias = messages.OfType<MediaAction>().Where(x => x.BandId != -1).ToList();
            if (medias.Count == 0) return;

            int maxBandId = await ApiService.GetLastMessageBandId() + 1;

            var uniqueBandIds = medias
                .Select(m => m.BandId)
                .Distinct()
                .ToList();

            for (int i = 0; i < uniqueBandIds.Count; i++)
            {
                List<MediaAction> toChange = medias.Where(x => x.BandId == uniqueBandIds[i]).ToList();

                for (int j = 0; j < toChange.Count; j++)
                {
                    toChange[j].BandId = maxBandId;
                }
                maxBandId++;
            }

        }

        private void DeleteSelectedBut_Click(object sender, RoutedEventArgs e)
        {
            //Remove selected messages
            SetDeleteMessagePage(isBoth: _chat.GetType() != typeof(SavedMessagesChat));
        }

        public void SetDeleteMessagePage(bool isBoth = true)
        {
            RemoveRightContactInfo();

            TelegramLib.MainClasses.User chatter =
                _chat.Chatter is null ? _system.LoggedUser :
                _chat.Chatter;

            isBoth = !isBoth ? false : SchedueleMessagesGrid.Visibility == Visibility.Visible ?
                false : true;

            IsMakeActionOnBothSides page =
                new IsMakeActionOnBothSides(chatter, isBoth);

            page.MakeAction += async () =>
            {
                SchedueleMessagesGrid.Visibility = Visibility.Hidden;

                //Get selected messages
                List<Message> toDelete = GetSelectedMessages();

                bool? isBoth = page.IsInBoth.IsChecked;
                if (isBoth is null) return;

                //One by one is too slow try many at the same time 

                await RemoveManyMessages(toDelete, (bool)isBoth);

                HideSelectionRow();
                await RemoveDateStateIfNoMesOnDate();
            };
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public async Task RemoveManyMessages(List<Message> toDelete, bool isBoth)
        {
            //Set one method for deleting in db (with static message)

            if (_isSavedMessageChat) await ApiService.RemoveSavedMessage(_system.LoggedUser.Id, toDelete.Select(x => x.Id).ToList());
            else await ApiService.DeleteManyMessages(toDelete.Select(x => x.Id).ToList(), isBoth);

            for (int i = 0; i < toDelete.Count; i++)
            {
                //Set for replied + pinned message
                _system.SetChatParamsAfterMessageRemoved(toDelete[i]);

                //Remove from system
                _system.RemoveMessageById(toDelete[i].Id);
            }

            if (isBoth)
            {
                await SignalRService.RemoveManyMessagesByDateTimes(
                    toDelete.Select(x => x.SentTime).ToList(), _system.LoggedUser.Id, _chat.Chatter.Id);
            }

            //Update vis 
            bool isOnlyPinnedChat = IsOnlyPinnedChatIsOn();
            if (isOnlyPinnedChat) IsOnlyPinnedChatPinAction();
            else SetChatMessages();

            //Set page actions
        }

        public ListBoxItem GetItemByTagId(int mesId)
        {
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is ListBoxItem item)
                {
                    if (item.Tag is null && item.Content is MediaMessage mediaMes &&
                        mediaMes.IsBandMedia() && mediaMes.IsBandBorderContainsId(mesId))
                    {
                        return item;
                    }
                    if (item.Tag is not null && item.Tag.ToString() == mesId.ToString()) return item;
                }


            }
            return null;
        }

        public List<Message> GetSelectedMessages()
        {
            List<Message> res = new List<Message>();

            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;

                if (item.Content is MediaMessage band &&
                    band.IsBandMedia())
                {
                    List<int> ids = band.GetSelectedMessagesIdsInBand();
                    _system.AddMessagesInList(ids, res);
                    continue;
                }

                if ((item.Content is ChatControls.TextMessage text &&
                    text.IsMessageIdTicked())

                    ||

                    (item.Content is ChatControls.MediaMessage media &&
                    media.IsMessageIdTicked())

                    ||

                    (item.Content is ShareContactControl share &&
                    share.IsMessageIdTicked())
                    )
                {
                    if (item.Content is MediaMessage medMes &&
                        medMes is not null && medMes.IsBandMedia())
                    {
                        List<int> ids = medMes.GetBandMessagesIds();
                        _system.AddMessagesInList(ids, res);
                        continue;
                    }

                    int.TryParse(item.Tag.ToString(), out int id);

                    TelegramLib.MainClasses.Messages.Message mes =
                        _system.GetMessageById(id);

                    if (mes is null) continue;

                    res.Add(mes);
                }
            }
            return res;
        }

        public void UpdateSelectedAmount()
        {
            int amount = GetSelectedAmount();

            //Update Amount
            UpdateTickedAmount(amount);

            if (amount == 0 && !_isMouseDown)
            {
                HideSelectionRow();
                _isMouseDown = false;
                _isSelected = false;
                return;
            }
        }

        public int GetSelectedAmount()
        {
            int amount = 0;

            //Get Amount
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue;

                if (item.Content is MediaMessage band &&
                    band.IsBandMedia())
                {
                    amount += band.GetAmountOfSelectedMediasInBand();
                    continue;
                }

                if ((item.Content is ChatControls.TextMessage text &&
                    text.IsMessageIdTicked())

                    ||

                    (item.Content is ChatControls.MediaMessage media &&
                    media.IsMessageIdTicked()

                    ||

                    (item.Content is ShareContactControl share &&
                    share.IsMessageIdTicked()))
                    )
                {
                    amount++;
                }
            }

            return amount;
        }

        public void HideSelectionRow()
        {
            ClearSelectionRow();
        }


        public void UpdateTickedAmount(int amount)
        {
            ForwardSelectedAmount.Text = amount.ToString();
            DeleteSelectedAmount.Text = amount.ToString();
        }

        private void UnPinAlGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //int _chatMessages there is only pinned now
            // unpin them
            for (int i = 0; i < _chatMessages.Count; i++)
            {
                ListBoxItem? item = ChatBox.Items
                    .OfType<ListBoxItem>()
                    .FirstOrDefault(x => x.Tag is not null && x.Tag.ToString() == _chatMessages[i].Id.ToString());

                if (item is null)
                {
                    item = ChatBox.Items
                       .OfType<ListBoxItem>()
                       .Where(x => x.Content is MediaMessage media &&
                            media.IsBandMedia() && media.IsBandMessageExistById(_chatMessages[i].Id))
                       .FirstOrDefault();
                }

                if (item is null) continue;

                SetPinAction(_chatMessages[i], item, true);
            }

            //Get back tpo chat 
            OnlyPinnedHeaderGrid.Visibility = Visibility.Hidden;
            UnPinAllBorder.Visibility = Visibility.Hidden;
            PinRow.Height = new GridLength(0);

            //Update chat messages
            SetChatMessages();
        }

        public void IsOnlyPinIsClear()
        {
            if (!IsOnlyPinnedChatIsOn() || _chatMessages.Count != 0) return;

            OnlyPinnedHeaderGrid.Visibility = Visibility.Hidden;
            UnPinAllBorder.Visibility = Visibility.Hidden;
            PinRow.Height = new GridLength(0);

            SetChatMessages();
        }

        private void BackToChatGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            BackToChatIcon.Foreground =
                new SolidColorBrush(Colors.White);
        }

        private void BackToChatGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            BackToChatIcon.Foreground =
                new SolidColorBrush(Colors.Gray);
        }

        private void BackToChatGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HidePinnedChatAndShowChatMessages();
        }

        public void HidePinnedChatAndShowChatMessages()
        {
            if (OnlyPinnedHeaderGrid.Visibility == Visibility.Hidden) return;
            //Get back tpo chat 
            HideOnlyPinnedBorders();
            PinRow.Height = new GridLength(50);

            //Update chat messages
            SetChatMessages();
        }

        private void PinnedChatBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void PinnedChatBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void PinnedChatBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set chat with only pinned messages

            //Hide other borders
            ClearSelectionRow(); //Hiding selection action
            ClearMessageForwarding(); //Hide reply border

            //Make upper + bottom borders visible
            OnlyPinnedHeaderGrid.Visibility = Visibility.Visible;
            UnPinAllBorder.Visibility = Visibility.Visible;

            //Set messages
            SetChatMessages(isOnlyPinned: true);

            //Set amount of pinned messages
            PinnedAmountRun.Text = _chatMessages.Count.ToString();
            PinRow.Height = new GridLength(0);

            //mb set some actions on Visual messages
        }

        public void HideOnlyPinnedBorders()
        {
            OnlyPinnedHeaderGrid.Visibility = Visibility.Hidden;
            UnPinAllBorder.Visibility = Visibility.Hidden;
        }

        public bool IsOnlyPinnedChatIsOn()
        {
            return
                OnlyPinnedHeaderGrid.Visibility == Visibility.Visible ||
                UnPinAllBorder.Visibility == Visibility.Visible;
        }

        private void AutoDelGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            AutoDelIcon.Foreground = new SolidColorBrush(Colors.White);
        }

        private void AutoDelGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            AutoDelIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void AutoDelGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            NewMessagesDeletion mesDel = new NewMessagesDeletion(_chat, _system);
            _mainWindow.SetSecondaryFrame(mesDel);
        }

        public void ScrollToMessageByDateTime(DateTime dateTime)
        {
            TelegramLib.MainClasses.Messages.Message mes =
                _chat.GetMessageByDateTime(dateTime);

            if (mes is null)
            {
                MessageBox.Show("No no noooo");
                return;
            }
            ScrollToMessageByMessageId(mes.Id);
        }

        List<string> _textHistory = new List<string>();
        private bool _textAddBlcoker = false;

        private void CommentTextBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxMenu textBoxMenu = new TextBoxMenu(CommentTextBox, _textHistory);
            textBoxMenu.SetEnableStatus(CommentTextBox.Text == string.Empty);
            System.Windows.Point point = e.GetPosition(this);

            textBoxMenu.SetPhoto += () =>
            {
                var pathList = Clipboard.GetFileDropList().Cast<string>().ToList();
                AddMediaPage(pathList, CommentTextBox.Text);
                CommentTextBox.Text = string.Empty;

                Clipboard.Clear();
            };

            textBoxMenu.Loaded += (sender, e) =>
            {
                //is x to big
                if (point.X + textBoxMenu.ActualWidth > this.ActualWidth)
                {
                    Canvas.SetLeft(textBoxMenu, point.X - textBoxMenu.Width);
                }
                else Canvas.SetLeft(textBoxMenu, point.X);

                //is y too big
                if (point.Y + textBoxMenu.ActualHeight > this.ActualHeight)
                {
                    Canvas.SetTop(textBoxMenu, this.ActualHeight - textBoxMenu.ActualHeight);
                }
                else Canvas.SetTop(textBoxMenu, point.Y);

                Keyboard.ClearFocus();
            };

            textBoxMenu.UnReDoAction += () =>
            {
                _textAddBlcoker = true;
            };

            MessageMenu.Children.Add(textBoxMenu);
        }

        private async void CommentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textAddBlcoker)
            {
                _textAddBlcoker = false;
                return;
            }
            _textHistory.Add(CommentTextBox.Text);

            if (_chat.GetChatter() is null) return;

            SetCommentBoxHeight();

            //Send typing event in SignalR
            await SignalRService.SendTypingAction(_system.LoggedUser, _chat.GetChatter());
        }

        private void SavedMessagesGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void SavedMessagesGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public bool IsSavedChat()
        {
            return _isSavedMessageChat;
        }

        public TelegramLib.MainClasses.UserChat GetChat()
        {
            return _chat;
        }

        private void SendMessageGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            EmojisBoard.Visibility = Visibility.Hidden;
            Emojis.Foreground = new SolidColorBrush(Colors.Gray);

            Cursor = Cursors.Hand;
        }

        private void SendMessageGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void SendMessageGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var target = Keyboard.FocusedElement as UIElement;
            if (target == null) return;

            var args = new KeyEventArgs(
                Keyboard.PrimaryDevice,
                PresentationSource.FromVisual(target),
                0,
                Key.Enter)
            {
                RoutedEvent = Keyboard.PreviewKeyDownEvent
            };

            InputManager.Current.ProcessInput(args);
        }

        public double GetUserInfoColumnWidth()
        {
            return UserInfoColumn.Width.Value;
        }

        public void ScrollChatToEnd()
        {
            return;
            ScrollViewer sv = HelperService.GetScrollViewer(ChatBox);
            if (sv == null) return;

            double from = sv.VerticalOffset;
            double to = sv.ScrollableHeight;

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            sv.BeginAnimation(
                ScrollViewerBehavior.AnimatedVerticalOffsetProperty,
                animation,
                HandoffBehavior.SnapshotAndReplace);
        }

        private void SendMessageGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SendMesMenu.Visibility == Visibility.Visible)
            {
                SendMesMenu.Visibility = Visibility.Hidden;
                return;
            }
            SendMesMenu.Visibility = Visibility.Visible;
        }

        private void ScheduleMessageGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            ScheduleMessagesIcon.Foreground =
                new SolidColorBrush(Colors.White);
        }

        private void ScheduleMessageGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            ScheduleMessagesIcon.Foreground =
                new SolidColorBrush(Colors.Gray);
        }

        private void ScheduleMessageGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ArrowLeftSchedGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            ArrowLeftSchedIcon.Foreground =
                new SolidColorBrush(Colors.White);
        }

        private void ArrowLeftSchedGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            ArrowLeftSchedIcon.Foreground =
                new SolidColorBrush(Colors.Gray);
        }

        private void ArrowLeftSchedGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Get back tpo chat 
            SchedueleMessagesGrid.Visibility = Visibility.Hidden;

            SetAddMediaButVisibility();

            //Update Icon Visibility
            SetScheduleMessageIconVisibility();

            //Update chat messages
            SetChatMessages();
        }

        public void UpdateVisAfterSchedUpdate()
        {
            //if sched messages
            if (SchedueleMessagesGrid.Visibility == Visibility.Visible)
            {
                SetChatMessages(isOnlySchedule: true);
                return;
            }

            //if pinned
            if (SavedMessagesGrid.Visibility == Visibility.Visible)
            {
                SetChatMessages(isOnlyPinned: false); //Check
                return;
            }

            //if chat
            SetChatMessages();
        }

        private void CommentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private bool _isHiddenSender = false;
        private const string _senderHidden = "Hidden";
        private void ReplyBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //_isHiddenSender = false;
            SetReplySentFromText();

            RepForType type = _toForwardMessages is not null ?
                RepForType.ForwardAction : RepForType.ReplyAction;

            ForRepMenu menuPage = GetNewForRepPage(type);
            if (menuPage is null) return;

            SetForRepPageActions(menuPage);

            Window window = Window.GetWindow(this);
            if (window is MainWindow main)
            {
                main.SetSecondaryFrame(menuPage);
            }
        }

        public ForRepMenu GetNewForRepPage(RepForType type)
        {
            if (_toForwardMessages is not null && _toForwardMessages.Count > 0)
            {
                bool isShow = ReplySenderText.Text != _senderHidden;
                return new ForRepMenu(type, _toForwardMessages, _system, isShow);
            }
            else if (_repliedMessage is not null)
            {
                return new ForRepMenu(type, _repliedMessage, _system);
            }
            return null;
        }

        public void SetForRepPageActions(ForRepMenu menuPage)
        {
            menuPage.HideSenderNameDel += () =>
            {
                if (ReplyMessageRow.Height.Value == 0 ||
                _toForwardMessages is null ||
                _toForwardMessages.Count == 0) return;

                _isHiddenSender = !_isHiddenSender;

                SetReplySentFromText();
            };

            menuPage.DoNotSendDel += () =>
            {
                //Clear temp 
                HideSelectionRow();
                ReplyMessageRow.Height = new GridLength(0);
                _toForwardMessages = null;
            };

            menuPage.ChangeRecipientDel += () =>
            {
                HideSelectionRow();
                ReplyMessageRow.Height = new GridLength(0);

                ForwardToPage page = new ForwardToPage(_system);
                page.CancelDel += () =>
                {
                    ReplyMessageRow.Height = new GridLength(0);
                    _toForwardMessages = null;
                    _repliedMessage = null;
                };

                page.ForwardSelected += async (senderId) =>
                {
                    HidePinnedChatAndShowChatMessages();

                    if (_toForwardMessages is not null &&
                    _toForwardMessages.Count > 0)
                    {
                        await SetForwardedMessage(_toForwardMessages, senderId);
                        //((MainWindow)Window.GetWindow(this)).ClearAllChatWindowsFromBosWindow();

                        //Check is this is chatWindow + sender id is opened in main MainWindow
                        if (senderId is not null) ((MainWindow)Window.GetWindow(this)).HideEnnesChat((int)senderId);
                    }
                    else if (_repliedMessage is not null)
                    {
                        await SentRepliedMessage(_repliedMessage, senderId, true);
                        //((MainWindow)Window.GetWindow(this)).ClearAllChatWindowsFromBosWindow();

                        ((MainWindow)Window.GetWindow(this)).HideEnnesChat(senderId);
                        //To check this thing
                        // await SetForwardedMessage(new List<Message>() { _repliedMessage}, senderId);
                    }
                    CommentTextBox.Focus();
                };

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
            };
        }

        public void SetReplySentFromText()
        {
            if (_toForwardMessages is null || _toForwardMessages.Count == 0) return;
            ReplySenderText.Text = _isHiddenSender ? _senderHidden :
                $"from {_system.GetUserById(_toForwardMessages.First().SenderUserId).Login}";
        }

        public void SetRepliedMessage(TelegramLib.MainClasses.Messages.Message mes)
        {
            _repliedMessage = mes;
        }

        public async Task SentRepliedMessage(Message mes, int? userIdToSend, bool isResend = false)
        {
            HideChatControlFeatures();

            //Get control
            UserControl? messageControl = GetMessageControlById(mes.Id);

            //Get temp active chat(From user chat control)
            TelegramLib.MainClasses.UserChat chat =

                userIdToSend is null || userIdToSend == _system.LoggedUser.Id ? _system.GetSavedChatMessages() :

                 _system.GetChatByChatterId((int)userIdToSend);

            if (chat is null) return;

            await SetUserChatByChat(chat);

            _chat = chat;

            //Set reply row in chat    
            SetReplyRowParams(messageControl, new List<Message>() { mes }, isResend);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focusable = true;
                this.Focus();
                Keyboard.Focus(this);

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);


        }

        public async ValueTask SetUserChatByChat(TelegramLib.MainClasses.UserChat chat)
        {
            int chatterId =
                chat.GetType() == typeof(SavedMessagesChat) ?
                _system.LoggedUser.Id : chat.Chatter.Id;

            if (chat.Id != _chat.Id ||
                chat.GetType() != _chat.GetType())
                await ((MainWindow)Window.GetWindow(this)).
                   SetOtherChatByUserId(chatterId);
        }

        private void ReplyBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void ReplyBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void SetUserImage(string imgName)
        {
            UserImage.ImageSource = ApiService.GetCachedBitmap(imgName);

            /*             new BitmapImage(new Uri(
                            FilesAction.GetUserImagePath(imgName), UriKind.Absolute));*/
        }

        public void SetAddMediaButVisibility()
        {
            /*            AddMediaButGrid.Visibility = 
                            SchedueleMessagesGrid.Visibility == Visibility.Visible ?
                            Visibility.Hidden : Visibility.Visible;*/
        }

        public void RestMediaMenu(TelegramLib.MainClasses.Messages.Message mes)
        {
            _mesMenu = new MesMenu(mes);
        }

        public async void SetChatterLittlePhotoVisibility(TelegramLib.MainClasses.User user)
        {
            //if (!user.BlockedUsers.Any(x => x.Id == _system.LoggedUser.Id)) return;

            if (_chat is null || (_chat.Chatter is not null && _chat.Chatter.Id == user.Id))
            {
                await SignalRHelperService.SetPhotoInEllipse(user,
                     UserImage, LittlePhotoEllipse);
            }
        }

        private async void ChatBox_Drop(object sender, DragEventArgs e)
        {
            if (OnlyPinnedHeaderGrid.Visibility == Visibility.Visible) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                List<string> paths = new List<string>();

                for (int i = 0; i < files.Length; i++)
                {
                    if ((FilesAction.IsFileIsImage(files[i]) ||
                        FilesAction.IsFileIsVideo(files[i])) &&
                        FilesAction.IsRealMedia(files[i]))
                    {
                        paths.Add(files[i]);
                    }
                }

                //Upload medias
                for (int i = 0; i < paths.Count; i++)
                {
                    paths[i] = await ApiService.UploadMediaAsync(paths[i]);
                    paths[i] = FilesAction.GetPathByPseudoPath(paths[i]);
                }
                if (paths.Count == 0) return;

                Window window = Window.GetWindow(this);
                if (window is MainWindow main)
                {
                    main.ClearSecFrame();
                    main.ClearThirdFrame();

                    bool isSched = SchedueleMessagesGrid.Visibility == Visibility.Visible;
                    SendMediaPage page = new SendMediaPage(paths, CommentTextBox.Text, _system, GetChat(), _toForwardMessages, isSched);

                    page.SendBut.PreviewMouseDown += (sender, e) =>
                    {
                        if (SchedueleMessagesGrid.Visibility != Visibility.Visible) return;

                        paths = page.GetPathsFromMedias();

                        bool isBand = page.GroupItems.IsChecked is null ? false : (bool)page.GroupItems.IsChecked;

                        List<MediaAction> medias = new List<MediaAction>();
                        for (int i = 0; i < paths.Count; i++)
                        {
                            medias.Add(new MediaAction(-1, _system.LoggedUser.Id, DateTime.Now.AddDays(1), Path.GetFileName(paths[i]), false, false, false, null));
                        }

                        SetScheduleMessage message =
                            new SetScheduleMessage(GetChat(),
                            medias.Cast<Message>().ToList(), _system,
                            _toForwardMessages,
                            isBandMessages: isBand);

                        ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(message);
                    };

                    main.SetSecondaryFrame(page);
                }
            }
        }

        public void UpdateGlobalMedias()
        {
            Window window = Window.GetWindow(this);

            if (window is MainWindow main)
            {
                main.UpdateGlobalMedias();
            }
        }

        private void ChatBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void BackOnlyChatGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            BackOnlyChatIcon.Foreground = new SolidColorBrush(Colors.White);
        }

        private void BackOnlyChatGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            BackOnlyChatIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void BackOnlyChatGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnlyChatSearchChatGrid.Visibility = Visibility.Hidden;

            //Set visual only chat stuff 
            SetOnlyChatVisState(false);
        }

        private void OnlyChatCalenar_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void OnlyChatCalenar_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void OnlyChatCalenar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Set calendar
            CalendarPage calendar = new CalendarPage();

            //calendar.

            Window window = Window.GetWindow(this);
            if (window is MainWindow main)
            {
                main.SetSecondaryFrame(calendar);
            }
        }

        private void OnlyChatArrowUp_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void OnlyChatArrowUp_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void OnlyChatArrowUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //move to up message
            ScrollToMessage(true);
        }

        private void OnlyChatArrowDown_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void OnlyChatArrowDown_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void OnlyChatArrowDown_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Move to down message
            ScrollToMessage(false);
        }

        int _mesIndex = -1;
        public void ScrollToMessage(bool isUp)
        {
            string findMes = OnlyChatSearchTextBox.Text;

            //Get message that contains mes
            List<TelegramLib.MainClasses.Messages.Message>? meses =
                _chat is null ? null : _chat.GetMessagesWithStr(findMes);
            if (meses is null || meses.Count == 0) return;

            _mesIndex =
                !isUp && _mesIndex <= 0 ? meses.Count - 1 :
                !isUp ? _mesIndex - 1 :
                isUp && _mesIndex >= meses.Count - 1 ? 0 :
                _mesIndex + 1;

            Message mes = meses[_mesIndex];
            if (mes is null) return;

            //Get next message index
            ScrollToMessageByMessageId(mes.Id);
        }

        private void OnlyChatSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _mesIndex = -1;
        }

    }

    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty AnimatedVerticalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedVerticalOffset",
                typeof(double),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(0.0, OnAnimatedVerticalOffsetChanged));

        public static double GetAnimatedVerticalOffset(DependencyObject obj)
            => (double)obj.GetValue(AnimatedVerticalOffsetProperty);

        public static void SetAnimatedVerticalOffset(DependencyObject obj, double value)
            => obj.SetValue(AnimatedVerticalOffsetProperty, value);

        private static void OnAnimatedVerticalOffsetChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer sv)
                sv.ScrollToVerticalOffset((double)e.NewValue);
        }
    }


}
