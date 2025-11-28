using System;
using System.Collections.Generic;
using System.IO;
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
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Helper;
using static System.Collections.Specialized.BitVector32;

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu
{
    /// <summary>
    /// Логика взаимодействия для ReplyMessagePart.xaml
    /// </summary>
    public partial class ReplyMessagePart : UserControl
    {
        public ReplyMessagePart()
        {
            InitializeComponent();
        }

        public void SetReplyMessageParams(TelSystem system,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            if (mes.Id == -1)
            {
                ImageColumn.Width = new GridLength(0);
                TextGrid.Visibility = Visibility.Hidden;
                DeleteMessageGrid.Visibility = Visibility.Visible;
                return;
            }

            if (mes is MediaAction action && mes.Id != -1)
            {
                SetMediaPath(action.MediaName);
            }
            else ImageColumn.Width = new GridLength(0);

            WhoSent.Text = system.GetSenderUserById(mes.SenderUserId).Login;

            ReplyedMessage.Text =
                mes is MediaAction ? "Media" :
                mes is TelegramLib.MainClasses.Messages.TextMessage secText ? secText.Text :
                "Message";
        }

        public void SetMediaPath(string mediaName)
        {
            if (FilesAction.IsFileIsImage(mediaName))
            {
                string path = FilesAction.GetFullChatImagePath(mediaName);
                if (!File.Exists(path)) return;
                ReplyImage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            else if (FilesAction.IsFileIsGif(mediaName))
            {
                string fullGifName = FilesAction.GetFullGifPath(mediaName);

                BitmapSource firstGifImgSource = FilesAction.GetFirstImageFromGif(fullGifName);
                if (firstGifImgSource is null) return;

                ReplyImage.Source = firstGifImgSource;
            }
            else if (FilesAction.IsFileIsVideo(mediaName))
            {
                //string fullGifName = FilesAction.GetFullVideoPath(mediaName);

                

                //ReplyImage.Source = firstGifImgSource;
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
