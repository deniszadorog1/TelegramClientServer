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

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для MonthDay.xaml
    /// </summary>
    public partial class MonthDay : UserControl
    {
        private TelegramLib.MainClasses.Messages.Message _pinned;
        private string _pinnerLogin;

        private DateTime _date;

        public event Action ScrollToPinned;

        //Set Date
        public MonthDay(DateTime date)
        {
            _date = date;
            InitializeComponent();
        }

        //Set Pinned Message
        public MonthDay(string pinner,
            TelegramLib.MainClasses.Messages.Message message)
        {
            _pinned = message;
            _pinnerLogin = pinner;

            InitializeComponent();

            SetPinnedMessageParams();
        }

        private const string _deletedMessage = "Deleted Message";

        public void SetPinnedMessageParams()
        {
            //Set pinnedLogin 
            LeftMessage.Text = _pinnerLogin;
            CenterMessage.Text = "pinned";
            RightMessage.Text =
                _pinned is null ? _deletedMessage : 
                _pinned is TelegramLib.MainClasses.Messages.TextMessage text ? text.Text :
                _pinned is TelegramLib.MainClasses.Messages.MediaAction media ? "media" :
                "Shared contact";               
        }

        private void RightMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if(RightMessage.Text != _deletedMessage) RightMessage.TextDecorations = TextDecorations.Underline;
        }

        private void RightMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            RightMessage.TextDecorations = null;
        }

        private void RightMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Scroll to message
            ScrollToPinned?.Invoke();
        }
    }
}
