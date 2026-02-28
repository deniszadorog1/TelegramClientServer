using System;
using System.Collections.Generic;
using System.IO;
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
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.UserControls.ChatsControls.ToSendMedias;

namespace TelegramVisualPart.Pages.ChatActions.SendMedia
{
    /// <summary>
    /// Логика взаимодействия для SendMediaPage.xaml
    /// </summary>
    public partial class SendMediaPage : Page
    {
        private List<string> _firstMediaPath;
        private string _text;
        public SendMediaType _sendType = SendMediaType.Single;

        public SendMediaPage(List<string> firstMediaPath, string text)
        {
            _firstMediaPath = firstMediaPath;
            _text = text;
            InitializeComponent();

            _paths.AddRange(firstMediaPath);

            SetCaptureText();
            SetBasePaths();
        }

        public void SetBasePaths()
        {
            foreach(var path in _firstMediaPath)
            {
                AddMedia(path);
            }
        }

        private void SetCaptureText()
        {
            CaptureBox.Text = _text;
        }

        public void AddMedia(string mediaPath)
        {
            AddMediaInVis(mediaPath);
        }

        public void AddMediaInVis(string path)
        {
            MediaElBoxItem toAdd = new MediaElBoxItem(path);
            toAdd.SetChosenSize();

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
            //Send all message
            List<Image> imgs = GetImagesFromMediaBox();
            List<string> paths = GetPathsFromMedias();

            ((MainWindow)Window.GetWindow(this)).SendBigImagesMessage(CaptureBox.Text, imgs, paths, _sendType);
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        public List<Image> GetImagesFromMediaBox()
        {
            List<Image> res = new List<Image>();
            for (int i = 0; i < MediasBox.Items.Count; i++)
            {
                if (MediasBox.Items[i] is ListBoxItem item &&
                    item.Content is MediaElBoxItem el)
                {
                    res.Add(el.Img);
                }
            }
            return res;
        }

        public List<string> GetPathsFromMedias()
        {
            List<string> res = new List<string>();

            for (int i = 0; i < MediasBox.Items.Count; i++)
            {
                if (MediasBox.Items[i] is ListBoxItem item &&
                    item.Content is MediaElBoxItem el)
                {
                    res.Add(el.GetMediaPath());
                }
            }
            return res;
        }

        private void AddBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Add more media
            SetMediaFile(null);
        }

        private async void SetMediaFile(MediaElBoxItem toChange)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose image",
                Filter = "Image and Video files|*.png;*.jpg;*.jpeg;*.mp4;*.mov;*.avi",
                Multiselect = true
            };

            //*.mp4;*.mov;*.avi

            if (openFileDialog.ShowDialog() == true)
            {
                string[] names = openFileDialog.FileNames;

                string filePath = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                if(names.Length > 1)
                {
                    foreach (var path in names) AddMediaInVis(path);

                    _paths.AddRange(names.ToList());
                    GroupImages();
                    return;
                }


                if (extension == ".png" || extension == ".jpg" ||
                    extension == ".jpeg")
                {
                    if (toChange is not null)
                    {
                        toChange.SetImage(filePath);
                        return;
                    }
                    //to add Image 
                    AddMediaInVis(filePath);

                    _paths.Add(filePath);
                    GroupImages();
                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                {
                    Image img = await VisHelper.GetFirstFrameAsync(filePath);

                    if (toChange is not null) return;
                    AddMediaInVis(filePath);

                    _paths.Add(filePath);
                    GroupImages();
                }
            }
        }

        private void RememberChoice_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void GroupItems_Checked(object sender, RoutedEventArgs e)
        {
            _sendType = SendMediaType.Group;
            GroupImages();

            MediasBox.Visibility = Visibility.Hidden;
            GroupScroll.Visibility = Visibility.Visible;
        }

        private void GroupItems_Unchecked(object sender, RoutedEventArgs e)
        {
            _sendType = SendMediaType.Single;

            MediasBox.Visibility = Visibility.Visible;
            GroupScroll.Visibility = Visibility.Hidden;
        }

        private void CompressImage_Checked(object sender, RoutedEventArgs e)
        {

        }

        public List<string> _paths = new List<string>();
        public void GroupImages()
        {
            //going through all media
            //set [i] img 
            //if i >= imgs.Count
            //set size = 0 to other blocks

            GroupPanel.Children.Clear();
            for (int i = 0; i < _paths.Count; i++)
            {
                MediaElBoxItem toAdd = new MediaElBoxItem(_paths[i]);
                toAdd.SetChosenSize();

                toAdd.ChangeMedia += () =>
                {
                    SetMediaFile(toAdd);
                };

                toAdd.DeleteMedia += () =>
                {
                    GroupPanel.Children.Remove(GroupPanel.Children
                        .OfType<MediaElBoxItem>()
                        .FirstOrDefault(x => x.Content == toAdd));

                    if (MediasBox.Items.Count == 0)
                    {
                        ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                    }
                };

                GroupPanel.Children.Add(toAdd);
            }
        }

       /* private void SetEventsForGroupedItems()
        {
            //Set events
            //Set index tag
            for (int i = 0; i < _mediaGroupItems.Count; i++)
            {
                MediaElBoxItem item = _mediaGroupItems[i];
                item.Tag = i.ToString();

                item.ChangeMedia += () =>
                {
                    SetMediaFile(item);
                };

                item.DeleteMedia += () =>
                {
                    int.TryParse(item.Tag.ToString(), out int index);
                    _paths.RemoveAt(index);

                    item.ClearParams();

                    GroupImages();

                    if (_paths.Count == 0)
                    {
                        ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                    }
                };
            }
        }*/
    }
}
