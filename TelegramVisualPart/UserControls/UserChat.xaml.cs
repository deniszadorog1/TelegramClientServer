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
using static System.Net.Mime.MediaTypeNames;

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

            ChatBox.Items.Add(new TextMessage (
                GetConvertedStringMessage(CommentTextBox.Text), img));
            CommentTextBox.Text = string.Empty;

            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
        }

        private string GetConvertedStringMessage(string str)
        {
            const int checker = 20;

            for(int i = 0; i < str.Length; i++)
            {
                if(i % checker == 0)
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
                Title = "Choose image",
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                System.Windows.Controls.Image img =
                    new System.Windows.Controls.Image();
                img.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));

                if (img is null) return;
                AddImageMessage(img);
            }
        }

        private void AddImageMessage(System.Windows.Controls.Image img)
        {
            ChatBox.Items.Add(new ImageMessage(img));
        }
    }
}
