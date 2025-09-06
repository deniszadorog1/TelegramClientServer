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


namespace TelegramVisualPart.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserChat.xaml
    /// </summary>
    public partial class UserChat : UserControl
    {
        public List<Message> _chatMessages = new List<Message>();
        public UserChat()
        {
            InitializeComponent();
            SetMarginForChatMenu();
            SetAutoDeleteTimer();

            SignalRService.TextMessageReceived += OnTextMessageReceived;
            SignalRService.MediaMessageReceived += OnMediaMessageRecived;

            SignalRService.UpdateOnlineStatusDel += UpdateOnlineStatus;
            SignalRService.UpdateUserImage += UpdateUserImage;
            SignalRService.ClearChatDel += ClearChatAction;

            SignalRService.SetContactLastSeenVisStateDel += SetLastVisState;
            SignalRService.UpdateContactPhotoDel += UpdateChatterIamge;
        }

        public void UpdateChatterIamge(TelegramLib.MainClasses.User user)
        {
            UpdateChatImages(user);
        }

        public void SetLastVisState(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_chat is null || _chat.GetChatter().ContactUserId != user.Id) return;

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
                TelegramLib.MainClasses.UserChat? chat = _system.Chats.FirstOrDefault(x => x.Chatter.ContactUserId == user.Id);
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

                if (chat is null || chat.Chatter.ContactUserId != user.Id) return;

                await SignalRHelperService.SetFastParamsForPhotoUpdate(user);

                for (int i = 0; i < ChatBox.Items.Count; i++)
                {
                    if (chat.Messages[i].SenderUserId != user.Id) continue;

                    if (ChatBox.Items[i] is ChatControls.TextMessage textMes)
                    {
                        SignalRHelperService.FastSetContactPhoto(user, _chat, textMes.BgBrush, textMes.UserEllipseImage);
                        //await SignalRHelperService.SetContactPhoto(user, _chat, textMes.BgBrush, textMes.UserEllipseImage);    

                        /*  textMes.BgBrush.ImageSource =
                              new BitmapImage(new Uri(FilesAction.GetUserImagePath(
                                  user.GetFirstImageNameInString()), UriKind.Absolute));*/
                    }
                    else if (ChatBox.Items[i] is MediaMessage mediaMes)
                    {
                        SignalRHelperService.FastSetContactPhoto(user, _chat, mediaMes.BgBrush, mediaMes.UserEllipseImage);

                        //await SignalRHelperService.SetContactPhoto(user, _chat, mediaMes.BgBrush, mediaMes.UserEllipseImage);

                        /*                        mediaMes.BgBrush.ImageSource =
                                                    new BitmapImage(new Uri(FilesAction.GetUserImagePath(
                                                        user.GetFirstImageNameInString()), UriKind.Absolute));
                                            */
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

        public void OnMediaMessageRecived(TelegramLib.MainClasses.User sender, TelegramLib.MainClasses.Messages.MediaAction message)
        {
            Dispatcher.Invoke(() =>
            {
                //Get chat wherer Logged is Sender 
                TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(sender.Id);
                if (chat is null) return;

                if (_chat is null || chat.Id != _chat.Id) AddMediaMessageInUnChosenChat(chat, message);
                else AddMediaMessageInChosenChat(message, sender);

                //Is temp chat is chosen
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
        }

        private async void AddMediaMessageInUnChosenChat(TelegramLib.MainClasses.UserChat chat, MediaAction message)
        {
            //Add in system 
            chat.Messages.Add(message);
            //Add in db
            await ApiService.AddMessage(message, chat);
        }

        private void OnTextMessageReceived(TelegramLib.MainClasses.User sender, TelegramLib.MainClasses.Messages.TextMessage message)
        {
            Dispatcher.Invoke(() =>
            {
                //Get chat wherer Logged is Sender 
                TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(sender.Id);
                if (chat is null) return;

                if (_chat is null || chat.Id != _chat.Id) AddTextMessageInUnChosenChat(chat, message);
                else AddTextMessageInChosenChat(message, sender);

                //Is temp chat is chosen
            });
        }

        private async void AddTextMessageInChosenChat(TelegramLib.MainClasses.Messages.TextMessage message,
            TelegramLib.MainClasses.User sender)
        {
/*            ChatBox.Items.Add(new ChatControls.TextMessage(
                GetConvertedStringMessage(message.Text),
                *//*sender.GetFirstImageNameInString()*//* await SignalRHelperService.GetUserPhotoToSet(sender),
                _system.Settings.GetChatSettings().FontName)); //Change on sender image 
*/

            ChatControls.TextMessage text = new ChatControls.TextMessage(
                GetConvertedStringMessage(message.Text),
                /*sender.GetFirstImageNameInString()*/ await SignalRHelperService.GetUserPhotoToSet(sender),
                _system.Settings.GetChatSettings().FontName);

            AddTextControl(text);


            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            await ApiService.AddMessage(message, _chat);

            message = (TelegramLib.MainClasses.Messages.TextMessage)await ApiService.GetLastChatMessage(_chat.Id);

            _chatMessages.Add(message);
            ((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();
        }

        public void AddTextControl(ChatControls.TextMessage text)
        {
            ListBoxItem item = new ListBoxItem()
            {
                Content = text
            };
            ChatBox.Items.Add(item);
        }

        private async void AddTextMessageInUnChosenChat(TelegramLib.MainClasses.UserChat chat,
            TelegramLib.MainClasses.Messages.TextMessage message)
        {
            //Add in system 
            chat.Messages.Add(message);
            //Add in db
            await ApiService.AddMessage(message, chat);
        }

        private TelegramLib.MainClasses.UserChat _chat;
        public async void SetUserChat(TelegramLib.MainClasses.UserChat chat)
        {
            if (chat is null) return;
            _chat = chat;

            await SetOnlineStatus();

            //_chat.Messages.Add(new TelegramLib.MainClasses.Messages.TextMessage(1, 1, DateTime.Now, "asd"));

            UserChatMenu.SetChatParam(_chat);

            ClearChat();

            SetChatParams(_chat.GetChatter());
            SetChatMessages();

            UserChatMenu.SetChatParam(_chat);

            RemoveRightContactInfo();
            SetUserBg();
        }

        public async Task SetOnlineStatus()
        {
            if (_chat is null) return;
            TelegramLib.MainClasses.User user = await ApiService.GetUserById(_chat.GetChatter().ContactUserId);
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
        public void SetChatParams(UserContactcs contact)
        {
            ChatFriendLogin.Text = contact.Name;
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
        public void SetSystemParam(TelSystem system)
        {
            //Set here chat messages(by ref)
            _system = system;
            UserChatMenu.SetSystemParam(system);
            SetTestChatMessages();

            EmojisBoard.SetSystem(_system);
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
                string imgName = _chatMessages[i].SenderId == _system.LoggedUser.Id ?
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
            }

            //Set chatter photo image
            if (_chatMessages.Count > 0) UpdateChatImages(await ApiService.GetUserById(_chat.Chatter.ContactUserId));
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
                        AddImageMessage(path, false, senderImgName);
                        return;
                    }
                case MediaType.Gif:
                    {
                        SendGif(path, senderImgName, isAdd: false);
                        return;
                    }
                case MediaType.Video:
                    {
                        AddMediaElement(path, senderImgName);
                        return;
                    }
                case MediaType.Sticker:
                    {
                        AddImageMessage(path, true, senderImgName);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public void SetTextMessageInChat(TelegramLib.MainClasses.Messages.TextMessage message, string senderImageName)
        {
            ChatControls.TextMessage newMes =
                new ChatControls.TextMessage(GetConvertedStringMessage(message.Text),
                senderImageName, _system.Settings.GetChatSettings().FontName);

            newMes.SetTime(message.SentTime);

            //ChatBox.Items.Add(newMes);
            AddTextControl(newMes);

            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
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
                AddTextMessage(_system.LoggedUser.GetFirstImageName().Name);
            }
        }



        private async void AddTextMessage(string senderImageName)
        {
            //Visaul add
/*            ChatBox.Items.Add(new ChatControls.TextMessage(
                GetConvertedStringMessage(CommentTextBox.Text), senderImageName, _system.Settings.GetChatSettings().FontName));
*/
            ChatControls.TextMessage text = new ChatControls.TextMessage(
                GetConvertedStringMessage(CommentTextBox.Text), senderImageName, _system.Settings.GetChatSettings().FontName);
            AddTextControl(text);

            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            //system add

            UserContactcs contact = await ApiService.GetContactByUserAndFriendIds(_system.LoggedUser.Id, _chat.Chatter.ContactUserId);

            TelegramLib.MainClasses.Messages.Message  toAdd = new TelegramLib.MainClasses.Messages.TextMessage(
                            _chatMessages.Count, contact.Id, _system.LoggedUser.Id,
                            DateTime.Now, CommentTextBox.Text);


            //Adding in DB
            await ApiService.AddMessage(toAdd, _chat);

            toAdd = await ApiService.GetLastChatMessage(_chat.Id);

            await SendMessageToReceiver((TelegramLib.MainClasses.Messages.TextMessage)toAdd);

            toAdd.SenderId = contact.Id;
            _chatMessages.Add(toAdd);

            CommentTextBox.Text = string.Empty;

            ((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();
        }

        private async Task SendMessageToReceiver(Message toAdd)
        {
            bool isReceiverOnline = await ApiService.IsUserOnline(_chat.GetChatter().ContactUserId);

            if (!isReceiverOnline)
            {
                TelegramLib.MainClasses.User receiver =
                await ApiService.GetUserById(_chat.GetChatter().ContactUserId);

                UserContactcs contact = await ApiService.GetContactByUserAndFriendIds(_system.LoggedUser.Id, receiver.Id);

                TelegramLib.MainClasses.UserChat chat =
                    await ApiService.GetChatByUserAndSenderId(receiver.Id, contact.Id);
                await ApiService.AddMessage(toAdd, chat);

                return;
            }
            await SendSignalRMessage(toAdd);
        }

        public void AddEmoji(string emoji)
        {
            CommentTextBox.Text += emoji;

            /*            ChatBox.Items.Add(new ChatControls.TextMessage(
                            GetConvertedStringMessage(emoji), _system.LoggedUser.GetFirstImageName().Name));

                        ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);*/

            EmojisBoard.Visibility = Visibility.Hidden;
        }

        private string GetConvertedStringMessage(string str)
        {
            const int checker = 15;

            for (int i = 0; i < str.Length; i++)
            {
                if (i % checker == 0)
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
                    AddImageMessage(filePath, false, _system.LoggedUser.GetFirstImageName().Name);
                    AddMediaPath(filePath);
                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                {
                    AddMediaElement(filePath, _system.LoggedUser.GetFirstImageName().Name);
                    AddMediaPath(filePath);
                }
            }
        }

        public void AddImageMessage(string filePath, bool isSticker, string senderImageName)
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

            AddImageMessage(img, isSticker, senderImageName);
        }

        public async void AddMediaPath(string filePath, bool isSticker = false, bool isAdd = true)
        {
            string fileName = Path.GetFileName(filePath);

            //Check it for presance

            /*            if (_chatMessages.Where(x => x is MediaAction media &&
                                media.MediaName == Path.GetFileName(fileName))
                            .ToList()
                            .Any()) return;*/

            Message newMediaMes = new MediaAction(-1, _chat.Chatter.Id,
                             _system.LoggedUser.Id, DateTime.Now, fileName, isSticker);

            if (isAdd)
            {
                await ApiService.AddMessage(newMediaMes, _chat);
                //Set id to image
                newMediaMes = await ApiService.GetLastChatMessage(_chat.Id);

                _chatMessages.Add(newMediaMes);

                await SendMessageToReceiver(newMediaMes);
                //await SendSignalRMessage(newMediaMes);
            }
        }
        public void AddMediaElement(string filePath, string senderImageName)
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

            AddVideoMessage(media, senderImageName);
        }

        public void SendGif(string gifPath, string senderImageName, bool isAdd = true)
        {
            var message = new MediaMessage(gifPath, senderImageName);
            message.PreviewMouseDown += ChatGif_PreviewMouseDown;
            ChatBox.Items.Add(message);
            AddMediaPath(gifPath, isAdd: isAdd);
        }

        private void ChatGif_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage) return;
            MediaMessage message = sender as MediaMessage;

            VisualActionPage page = new VisualActionPage(message.GetGifPath(), GetChatMediaPaths(MediaType.Gif));

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);

            VisualActionPageParams(message, MediaType.Gif, page);
        }

        private void AddVideoMessage(MediaElement el, string senderImageName)
        {
            var video = new MediaMessage(el, senderImageName);
            video.PreviewMouseDown += ChatVideo_PreviewMouseDown;
            ChatBox.Items.Add(video);
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

        public void AddImageMessage(Image img, bool isSticker, string senderImgName)
        {
            var message = new MediaMessage(img, isSticker, senderImgName);
            message.PreviewMouseDown += ChatImage_PreviewMouseDown;
            ChatBox.Items.Add(message);
        }

        public void AddStickerMessage(Image img, string senderImageName)
        {
            var message = new MediaMessage(img, true, senderImageName);
            message.PreviewMouseDown += ChatImage_PreviewMouseDown;
            ChatBox.Items.Add(message);

            TelegramLib.MainClasses.UserChat messages = _system.GetChosenChat();

            //_chat.AddSticker(img.Tag.ToString(), _system.LoggedUser.Id);

            AddMediaPath(img.Tag.ToString(), true);
        }

        private void ChatImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage message) return;
            _isGetStikers = message.IsSticker;
            if (_isGetStikers) return; //NO TO STICKERS

            VisualActionPage page = new VisualActionPage(message.GetImage(), GetChatImages());
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);

            MediaMessage item = ChatBox.Items.OfType<MediaMessage>()
                .Where(x => x == message).First();

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

        private void FindMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //find message menu

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

        public void AddContactInfo()
        {
            const int _userContactWidth = 450;
            double windowWidth = ((MainWindow)Window.GetWindow(this)).ActualWidth;

            ContactInfo info = new ContactInfo();
            info.SetContactInfo(_chat, _system, _system.ChosenChatContact);

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
                ContactInfoGrid.Children.Add(info);
            };
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
            double windowHeight = ((MainWindow)Window.GetWindow(this)).ActualHeight;
            info.Height = windowHeight <= info.Height ? info.Height : windowHeight - 230;
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

        public void ScrollToChosenItem(int index)
        {
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
            UserContactcs contcat = await ApiService.GetContactByUserAndFriendIds(_chat.Chatter.ContactUserId, _system.LoggedUser.Id);
            if (contcat is null) return;

            TelegramLib.MainClasses.UserChat chat = await ApiService.GetChatByUserAndSenderId(_chat.Chatter.ContactUserId, contcat.Id);
            if (chat is null) return;

            if (message is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                await SignalRService.SendTextMessage(_system.LoggedUser, text);
            }
            else if (message is TelegramLib.MainClasses.Messages.MediaAction media)
            {
                await SignalRService.SendMediaMessage(_system.LoggedUser, media);
            }
        }

        public void SetMessagesPosition(bool isGluedToLeft)
        {
            //Set that in chat can be ONLY MESSAGES
            for (int i = 0; i < ChatBox.Items.Count; i++)
            {
                if (ChatBox.Items[i] is not ListBoxItem item) continue; 

                if (_chatMessages[i].SenderUserId == _system.LoggedUser.Id &&
                    item.Content is UserControl ctrl)
                {
                    if (isGluedToLeft) 
                    {
                        item.HorizontalAlignment = HorizontalAlignment.Left;
                        item.Margin = new Thickness(0, 0, 0, 0);
                    }
                    else
                    {
                        item.HorizontalAlignment = HorizontalAlignment.Right;
                        item.Margin = new Thickness(0, 0, 0, 0);
                    }

                    if(item.Content is ChatControls.TextMessage text)
                    {
                        if(!isGluedToLeft) text.UserEllipseImage.Visibility = Visibility.Hidden;
                        else text.UserEllipseImage.Visibility = Visibility.Visible;
                    }
                    else if (item.Content is ChatControls.MediaMessage media)
                    {

                    }

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
    }
}
