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
        private string _firstMediaPath;
        public SendMediaType _sendType = SendMediaType.Single;

        public SendMediaPage(string firstMediaPath)
        {
            _firstMediaPath = firstMediaPath;
            InitializeComponent();

            AddMedia(_firstMediaPath);

            SetMediaRowInList();
            SetMediaGroupList();
            _paths.Add(_firstMediaPath);
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
            //Send all messge
            List<Image> imgs = GetImagesFromMediaBox();

            ((MainWindow)Window.GetWindow(this)).SendBigImagesMessage(CaptureBox.Text, imgs);
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

                    _paths.Add(filePath);
                    GroupImages();
                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                {
                    //
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
            GroupImgGrid.Visibility = Visibility.Visible;
        }

        private void GroupItems_Unchecked(object sender, RoutedEventArgs e)
        {
            _sendType = SendMediaType.Single;

            MediasBox.Visibility = Visibility.Visible;
            GroupImgGrid.Visibility = Visibility.Hidden;
        }

        private void CompressImage_Checked(object sender, RoutedEventArgs e)
        {

        }

        public List<string> _paths = new List<string>();
        public List<RowDefinition> _medRows = new List<RowDefinition>();
        public void GroupImages()
        {
            //going through all media
            //set [i] img 
            //if i >= imgs.Count
            //set size = 0 to other blocks

            for (int i = 0; i < _mediaGroupItems.Count; i++)
            {
                if (i >= _paths.Count)
                {
                    int divider = (int)Math.Ceiling(i / 2.0);
                    //Hide row which is no need
                    for (int j = divider; j < _medRows.Count; j++)
                    {
                        _medRows[j].Height =
                           new GridLength(0, GridUnitType.Star);
                    }

                    //Show rows which is chosen
                    for (int j = 0; j < divider; j++)
                    {
                        _medRows[j].Height =
                            new GridLength(1, GridUnitType.Star);
                    }
                    break;
                }
                else
                {
                    _mediaGroupItems[i].SetImage(_paths[i]);

                    int divider = (int)Math.Ceiling(i / 2.0);
                    _medRows[divider].Height =
                        new GridLength(1, GridUnitType.Star);
                }
            }

            if (_paths.Count <= 1)
            {
                TwoMedCol.Width =
                    new GridLength(0, GridUnitType.Star);
            }
            else
            {
                TwoMedCol.Width =
                    new GridLength(1, GridUnitType.Star);
            }
        }

        private void SetMediaRowInList()
        {
            _medRows.Clear();

            _medRows.Add(OneMedRow);
            _medRows.Add(TwoMedRow);
            _medRows.Add(ThreeMedRow);
            _medRows.Add(FourMedRow);
            _medRows.Add(FiveMedRow);

        }

        private List<MediaElBoxItem> _mediaGroupItems = new List<MediaElBoxItem>();
        private void SetMediaGroupList()
        {
            _mediaGroupItems.Clear();

            _mediaGroupItems.Add(OneMedia);
            _mediaGroupItems.Add(TwoMedia);

            _mediaGroupItems.Add(ThreeMedia);
            _mediaGroupItems.Add(FourMedia);

            _mediaGroupItems.Add(FiveMedia);
            _mediaGroupItems.Add(SixMedia);

            _mediaGroupItems.Add(SevenMedia);
            _mediaGroupItems.Add(EightMedia);

            _mediaGroupItems.Add(NineMedia);
            _mediaGroupItems.Add(TenMedia);

            SetEventsForGroupedItems();
        }

        private void SetEventsForGroupedItems()
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
        }
    }
}
