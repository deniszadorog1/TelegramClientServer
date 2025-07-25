using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
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
using TelegramVisualPart.UserControls.ChatControls;
using TelegramVisualPart.Pages.VisualPages;

using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using Application = System.Windows.Application;
using TelegramLib.MainClasses;

using TelegramLib.MainClasses.Messages;
using TelegramLib.Enums.Messages;
using System.IO;
using Path = System.IO.Path;
using TelegramVisualPart.Helper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TelegramLib.MainClasses.ChatFitures;
using System.Windows.Media.Effects;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics.Eventing.Reader;
using Accessibility;
using System.Windows.Threading;
using System.Security.Cryptography.X509Certificates;

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
        }

        private TelegramLib.MainClasses.UserChat _chat;
        public void SetUserChat(TelegramLib.MainClasses.UserChat chat)
        {
            if (chat is null) return;
            _chat = chat;
            UserChatMenu.SetChatParam(_chat);

            ClearChat();

            SetChatParams(_chat.GetChatter());
            SetChatMessages();

            UserChatMenu.SetChatParam(_chat);

            RemoveRightContactInfo();
            SetUserBg();
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
                    Radius = 20
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

            ChatFriendLastSeen.Text = contact.LastSeen is null ? _lastSeenDefault :
                $"{contact.LastSeen.Value.Month}.{contact.LastSeen.Value.Day}.{contact.LastSeen.Value.Year}";
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
        }

        public void SetTestChatMessages()
        {
            //Get Chatter here (Contact type)
            _chatMessages = _system.GetTestMessages();
            SetMessagesInChat();
        }

        public void SetMessagesInChat()
        {
            for (int i = 0; i < _chatMessages.Count; i++)
            {
                if (_chatMessages[i] is TelegramLib.MainClasses.Messages.TextMessage text)
                {
                    SetTextMessageInChat(text);
                    //text
                }
                else if (_chatMessages[i] is MediaAction media)
                {
                    //Video or photo
                    SetMediaMessageInChat(media);
                }
            }
        }

        public void SetMediaMessageInChat(MediaAction message)
        {
            //Got type (To know what folder to search in)
            MediaType type = message.IsSticker ? MediaType.Sticker : 
                FilesAction.GetMediaTypeFromFilename(message.MediaName);

            string path = FilesAction.GetFilePathByMediaType(type, message.MediaName);

            switch (type)
            {
                case MediaType.Image:
                    {
                        AddImageMessage(path, false);
                        return;
                    }
                case MediaType.Gif:
                    {
                        SendGif(path);
                        return;
                    }
                case MediaType.Video:
                    {
                        AddMediaElement(path);
                        return;
                    }
                case MediaType.Sticker:
                    {
                        AddImageMessage(path, true);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public void SetTextMessageInChat(TelegramLib.MainClasses.Messages.TextMessage message)
        {
            ChatControls.TextMessage newMes = new ChatControls.TextMessage(GetConvertedStringMessage(message.Text));
            newMes.SetTime(message.SentTime);

            ChatBox.Items.Add(newMes);
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
                AddTextMessage();
            }
        }

        private void AddTextMessage()
        {
            ChatBox.Items.Add(new ChatControls.TextMessage(
                GetConvertedStringMessage(CommentTextBox.Text)));

            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            _chatMessages.Add(new
                TelegramLib.MainClasses.Messages.TextMessage(
                _chatMessages.Count, _system.LoggedUser.Id,
                DateTime.Now, CommentTextBox.Text));

            CommentTextBox.Text = string.Empty;

            ((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();
        }

        public void AddEmoji(string emoji)
        {
            ChatBox.Items.Add(new ChatControls.TextMessage(
                GetConvertedStringMessage(emoji)));
            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            EmojisBoard.Visibility = Visibility.Hidden;
        }

        private string GetConvertedStringMessage(string str)
        {
            const int checker = 20;

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
                    AddImageMessage(filePath, false);
                    AddMediaPath(filePath);
                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                {
                    AddMediaElement(filePath);
                    AddMediaPath(filePath);
                }
            }
        }

        public void AddImageMessage(string filePath, bool isSticker)
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

            AddImageMessage(img, isSticker);
        }

        public void AddMediaPath(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (_chatMessages.Where(x => x is MediaAction media &&
                    media.MediaName == Path.GetFileName(fileName))
                .ToList()
                .Any()) return;
                    
            _chatMessages.Add(new MediaAction(_chatMessages.Count + 1,
                _system.LoggedUser.Id, DateTime.Now, fileName, false));
        }

        public void AddMediaElement(string filePath)
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

            AddVideoMessage(media);
        }

        public void SendGif(string gifPath)
        {
            var message = new MediaMessage(gifPath);
            message.PreviewMouseDown += ChatGif_PreviewMouseDown;
            ChatBox.Items.Add(message);
            AddMediaPath(gifPath);
        }

        private void ChatGif_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage) return;
            MediaMessage message = sender as MediaMessage;

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new VisualActionPage(message.GetGifPath(), GetChatMediaPaths()));
        }

        private void AddVideoMessage(MediaElement el)
        {
            var video = new MediaMessage(el);
            video.PreviewMouseDown += ChatVideo_PreviewMouseDown;
            ChatBox.Items.Add(video);
        }

        private void ChatVideo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage) return;
            MediaMessage message = sender as MediaMessage;

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new VisualActionPage(message.GetVideo(), GetChatMediaPaths()));
        }

        public List<string> GetChatMediaPaths()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < _chatMessages.Count; i++)
            {
                if (_chatMessages[i] is MediaAction media &&
                    (FilesAction.GetMediaTypeFromFilename(media.MediaName) == MediaType.Gif ||
                    FilesAction.GetMediaTypeFromFilename(media.MediaName) == MediaType.Video))
                {
                    MediaType type = FilesAction.GetMediaTypeFromFilename(media.MediaName);

                    string path = FilesAction.GetFilePathByMediaType(type, media.MediaName);
                    res.Add(path);
                }
            }
            return res;
        }

        public void AddImageMessage(Image img, bool isSticker)
        {
            var message = new MediaMessage(img, isSticker);
            message.PreviewMouseDown += ChatImage_PreviewMouseDown;
            ChatBox.Items.Add(message);
        }

        public void AddStickerMessage(Image img)
        {
            var message = new MediaMessage(img, true);
            message.PreviewMouseDown += ChatImage_PreviewMouseDown;
            ChatBox.Items.Add(message);

            TelegramLib.MainClasses.UserChat messages =  _system.GetChosenChat();

            _chat.AddSticker(img.Tag.ToString(), _system.LoggedUser.Id);
        }

        private void ChatImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage) return;
            MediaMessage message = sender as MediaMessage;

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new VisualActionPage(message.GetImage(), GetChatImages()));
        }

        public List<Image> GetChatImages()
        {
            List<Image> res = new List<Image>();

            for (int i = 0; i < _chatMessages.Count; i++)
            {
                if (_chatMessages[i] is MediaAction media &&
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
            const int _userContactWidth = 400;
            double windowWidth = ((MainWindow)Window.GetWindow(this)).ActualWidth;

            if (windowWidth + _userContactWidth <=
                SystemParameters.PrimaryScreenWidth)
            {
                ((MainWindow)Window.GetWindow(this)).Width =
                    windowWidth + _userContactWidth;
            }

            ContactInfo info = new ContactInfo();
            info.SetContactInfo(_chat);
            info.CloseButGrid.MouseDown += CloseContactInfo_MouseDown;

            UserInfoColumn.Width = new GridLength(_userContactWidth);
            ContactInfoGrid.Children.Add(info);
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

            TextBlock block = EmojisBoard.TabsPanel.Children.OfType<TextBlock>().Where
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
            Pages.UserInfo info = new Pages.UserInfo(_chat);
            SetUserInfoPageHeight(info);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(info);
        }

        public void SetUserInfoPageHeight(Pages.UserInfo info)
        {
            double windowHeight = ((MainWindow)Window.GetWindow(this)).ActualHeight;
            info.Height = windowHeight <= info.Height ? info.Height : windowHeight - 250;
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
            if(_chat is not null && !_chat.GetBackground().IsGeneral)
            {
                SetChatBackground();
                return;
            }
            //set general
            if(_chat is not null && _chat.GetBackground().IsGeneral)
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
                DateTime? time =  _chat.GetFirstMessageDateTime();
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
    }
}
