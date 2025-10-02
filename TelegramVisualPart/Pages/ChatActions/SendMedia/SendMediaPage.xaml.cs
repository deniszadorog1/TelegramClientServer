using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
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
using TelegramVisualPart.Helper;
using TelegramVisualPart.UserControls.ChatsControls.ToSendMedias;

namespace TelegramVisualPart.Pages.ChatActions.SendMedia
{
    /// <summary>
    /// Логика взаимодействия для SendMediaPage.xaml
    /// </summary>
    public partial class SendMediaPage : Page
    {
        private string _firstMediaPath;
        public SendMediaPage(string firstMediaPath)
        {
            _firstMediaPath = firstMediaPath;
            InitializeComponent();

            AddMedia(_firstMediaPath);
        }

        public void AddMedia(string mediaPath)
        {
            if (FilesAction.IsFileIsImage(mediaPath))
            {
                AddImage(mediaPath);
            }
        }

        public void AddImage(string path)
        {
            MediaElBoxItem toAdd = new MediaElBoxItem(path);

            toAdd.ChangeMedia += () =>
            {
                SetMediaFile(toAdd);
            };

            toAdd.DeleteMedia += () =>
            {
                MediasBox.Items.Remove(MediasBox.Items
                    .OfType<ListBoxItem>()
                    .FirstOrDefault(x => x.Content == toAdd));

                if (MediasBox.Items.Count == 0)
                {
                    ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                }
            };

            ListBoxItem item = new ListBoxItem()
            {
                Content = toAdd
            };

            MediasBox.Items.Add(item);
        }

        private void SmileGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            SmileIcon.Foreground = new SolidColorBrush(Colors.White);
            SmileBlock.Visibility = Visibility.Visible;
        }

        private void SmileGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            SmileIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void SmileGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set show smile 
        }

        private void BottomBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void BottomBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        public void AddSmileInTextBox(string smile)
        {
            CaptureBox.Text += smile;
        }

        private void SmileBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            SmileBlock.Visibility = Visibility.Hidden;
        }

        private void CloseButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            CloseBut.Foreground = new SolidColorBrush(Colors.White);
        }

        private void CloseButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            CloseBut.Foreground = new SolidColorBrush(Colors.White);
        }

        private void CloseButGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void CancelBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void SendBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Send all messge
            List<Image> imgs = GetImagesFromMediaBox();
        
            ((MainWindow)Window.GetWindow(this)).SendBigImagesMessage(CaptureBox.Text, imgs);
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this); 
        }
        
        public List<Image> GetImagesFromMediaBox()
        {
            List<Image> res = new List<Image>();
            for(int i = 0; i < MediasBox.Items.Count; i++)
            {
                if (MediasBox.Items[i] is ListBoxItem item &&
                    item.Content is MediaElBoxItem el)
                {
                    res.Add(el.Img);
                }
            }
            return res;
        }

        private void AddBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Add 1 more media
            SetMediaFile(null);
        }

        private void SetMediaFile(MediaElBoxItem toChange)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose image",
                Filter = "Image files|*.png;*.jpg;*.jpeg;"
            };

            //*.mp4;*.mov;*.avi

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    if (toChange is not null)
                    {
                        toChange.SetImage(filePath);
                        return;
                    }
                    //to add Image 
                    AddImage(filePath);
                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                {
                  //
                }
            }
        }

    }
}
