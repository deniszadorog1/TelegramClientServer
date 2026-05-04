using Microsoft.AspNetCore.Mvc.RazorPages;
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
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatsControls.ToSendMedias;

namespace TelegramVisualPart.Pages.ChatActions.SendMedia
{
    /// <summary>
    /// Логика взаимодействия для SendMediaPage.xaml
    /// </summary>
    public partial class SendMediaPage : System.Windows.Controls.Page
    {
        private List<string> _firstMediaPath;
        private string _text;
        public SendMediaType _sendType = SendMediaType.Single;

        public bool _isScheduleSend;
        private TelSystem _system;
        private UserChat _chat;

        private List<TelegramLib.MainClasses.Messages.Message> _forwardMessages;

        public SendMediaPage(List<string> firstMediaPath,
            string text, TelSystem system, 
            UserChat chat,
            List<TelegramLib.MainClasses.Messages.Message> forwardMessages,
            bool isSchedule = false)
        {
            _firstMediaPath = firstMediaPath;
            _text = text;
            _isScheduleSend = isSchedule;
            _forwardMessages = forwardMessages;
            
            _system = system;
            _chat = chat;

            InitializeComponent();

            _paths.AddRange(firstMediaPath);

            SetCaptureText();
            SetBasePaths();
        }

        public void SetBasePaths()
        {
            MediasBox.Items.Clear();
            for (int i = 0; i < _paths.Count; i++)
            {
                int pathIndex = i;

                MediaElBoxItem toAdd = new MediaElBoxItem(_paths[pathIndex]);
                toAdd.SetChosenSize();

                toAdd.ChangeMedia += () =>
                {
                    SetMediaFile(toAdd, pathIndex);
                };

                toAdd.DeleteMedia += () =>
                {
                    MediasBox.Items.Remove(MediasBox.Items
                        .OfType<ListBoxItem>()
                        .FirstOrDefault(x => x.Content == toAdd));
                    _paths.RemoveAt(pathIndex);
                    _paths.RemoveAt(pathIndex);

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
        }

        private void SetCaptureText()
        {
            CaptureBox.Text = _text;
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
            if (_isScheduleSend) return;

            //Send all message
            List<Image> imgs = GetImagesFromMediaBox();
            List<string> paths = GetPathsFromMedias();

            ((MainWindow)Window.GetWindow(this)).SendBigImagesMessage(CaptureBox.Text, imgs, paths, _sendType);
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);

            //((MainWindow)Window.GetWindow(this)).UpdateUserChatTalkControl();
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
            SetMediaFile(null, -1);
        }

        private async void SetMediaFile(MediaElBoxItem toChange, int pathIdToChange)
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

                //Upload medias
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = await ApiService.UploadMediaAsync(names[i]);
                    names[i] = FilesAction.GetPathByPseudoPath(names[i]);
                }

                if (names.Length > 1)
                {
                    _paths.AddRange(names.ToList());

                    SetBasePaths();
                    GroupImages();
                    return;
                }

                if (extension == ".png" || extension == ".jpg" ||
                    extension == ".jpeg")
                {
                    if (toChange is not null)
                    {
                        toChange.SetImage(filePath);
                        if (pathIdToChange != -1) _paths[pathIdToChange] = filePath;
                    }
                    else _paths.Add(filePath);
                    GroupImages();
                    SetBasePaths();

                }
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                {
                    //Image img = await VisHelper.GetFirstFrameAsync(filePath);

                    if (toChange is not null) return;
                    _paths.Add(filePath);
                    GroupImages();
                    SetBasePaths();
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

            SetBasePaths();

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
                int index = i;

                toAdd.SetChosenSize();

                toAdd.ChangeMedia += () =>
                {
                    SetMediaFile(toAdd, index);
                };

                toAdd.DeleteMedia += () =>
                {
                    MediaElBoxItem? toRemove = GroupPanel.Children
                        .OfType<MediaElBoxItem>()
                        .FirstOrDefault(x => x == toAdd);

                    if (toRemove is null) return;
                    _paths.RemoveAt(index);


                    GroupPanel.Children.Remove(toRemove);

                    if (MediasBox.Items.Count == 0)
                    {
                        ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                    }
                };

                GroupPanel.Children.Add(toAdd);
            }
        }

        private void SendBut_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SchedMesBorder.Visibility = Visibility.Visible;
        }

        private void SchedMesBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void SchedMesBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            SchedMesBorder.Visibility = Visibility.Hidden;
        }

        private void SchedMesBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool isBand = GroupItems.IsChecked is null ? false : (bool)GroupItems.IsChecked;

            Window window = Window.GetWindow(this);
            if (window is null || window is not MainWindow main || _chat is null) return;

            main.ClearSecFrame();
            main.ClearThirdFrame();

            //Set sched action
            List<MediaAction> medias = new List<MediaAction>();
            for (int i = 0; i < _paths.Count; i++)
            {
                medias.Add(new MediaAction(-1, _system.LoggedUser.Id, DateTime.Now.AddDays(1), System.IO.Path.GetFileName(_paths[i]), false, false, false, null));
            }

            SetScheduleMessage message =
                new SetScheduleMessage(_chat, medias.Cast<Message>().ToList(), _system, _forwardMessages, isBandMessages:isBand);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(message);
        }
    }
}
