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

namespace TelegramVisualPart.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserChat.xaml
    /// </summary>
    public partial class UserChat : UserControl
    {
        public UserChat()
        {
            InitializeComponent();

            SetMarginForChatMenu();
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
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(CommentTextBox.Text)) return;
                AddTextMessage();
            }
        }

        private void AddTextMessage()
        {
            /*            string filePath = "/Visuals/Images/UserImages/Minato.jpg";
                        System.Windows.Controls.Image img = new System.Windows.Controls.Image()
                        {
                            Source = new BitmapImage(new Uri(filePath, UriKind.Absolute))
                        };*/

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            ChatBox.Items.Add(new TextMessage(
                GetConvertedStringMessage(CommentTextBox.Text), img));

            //Back Message + date


            CommentTextBox.Text = string.Empty;

            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
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
            AddFile.Foreground = Brushes.White;
        }

        private void AddFile_MouseLeave(object sender, MouseEventArgs e)
        {
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
                    // Изображение
                    var img = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri(filePath, UriKind.Absolute)),
                        //Width = 200 
                    };

                    AddImageMessage(img);
                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
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

                    AddVideoMessage(media);
                }
            }
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
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new VisualActionPage(message.GetVideo()));
        }

        private void AddImageMessage(Image img)
        {
            var message = new MediaMessage(img);
            message.PreviewMouseDown += ChatImage_PreviewMouseDown;
            ChatBox.Items.Add(message);
        }

        private void ChatImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not MediaMessage) return;
            MediaMessage message = sender as MediaMessage;
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new VisualActionPage(message.GetImage()));
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
            const int _userContactWidth = 350;
            double windowWidth = ((MainWindow)Window.GetWindow(this)).ActualWidth;

            if (windowWidth + _userContactWidth <=
                SystemParameters.PrimaryScreenWidth)
            {
                ((MainWindow)Window.GetWindow(this)).Width =
                    windowWidth + _userContactWidth;
            }

            ContactInfo info = new ContactInfo();
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
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new Pages.UserInfo());
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void EmojisGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            EmojisBoard.Visibility = Visibility.Visible;
        }

        private void EmojisBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            EmojisBoard.Visibility = Visibility.Hidden;
        }
    }
}
