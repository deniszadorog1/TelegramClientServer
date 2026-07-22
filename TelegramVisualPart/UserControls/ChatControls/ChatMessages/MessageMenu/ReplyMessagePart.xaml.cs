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
using TelegramVisualPart.Services;
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
            const string mesStr = "Message";
            const string mediaStr = "Media";


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
                mes is MediaAction ? mediaStr :
                mes is TelegramLib.MainClasses.Messages.TextMessage secText ? secText.RepliedQuote == string.Empty ? secText.Text : secText.RepliedQuote :
                mesStr;
        }

        public async void SetMediaPath(string mediaName)
        {
            if (FilesAction.IsFileIsImage(mediaName))
            {
                string path = FilesAction.GetPathByName(mediaName);
                if (path is null) return;

                BitmapImage cached = ApiService.GetCachedBitmap(path);

                ReplyImage.Source = cached is not null ? cached : await SignalRHelperService.LoadBitmap(path);
            }
            else if (FilesAction.IsFileIsGif(mediaName))
            {
                string fullGifName = FilesAction.GetPathByName(mediaName);

                BitmapSource firstGifImgSource = FilesAction.GetFirstImageFromGif(fullGifName);
                if (firstGifImgSource is null) return;

                ReplyImage.Source = firstGifImgSource;
            }
            else if (FilesAction.IsFileIsVideo(mediaName))
            {
                string path = FilesAction.GetPathByName(mediaName);

                Image img = await VisHelper.GetFirstFrameAsync(path);
                if (img is null) return;
                ReplyImage.Source = img.Source;
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
