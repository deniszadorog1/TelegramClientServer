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
            if(mes is MediaAction action)
            {
                string path = FilesAction.GetFullChatImagePath(action.MediaName);
                if (File.Exists(path)) 
                    ReplyImage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                
            }
            else ImageColumn.Width = new GridLength(0);

            WhoSent.Text = system.GetSenderUserById(mes.SenderUserId).Login;

            ReplyedMessage.Text =
                mes is MediaAction ? "Media" :
                mes is TelegramLib.MainClasses.Messages.TextMessage text ? text.Text :
                "Message";
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
