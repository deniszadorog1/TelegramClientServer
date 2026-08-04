using System;
using System.Collections.Generic;
using System.Globalization;
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
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses;
using TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для MonthDay.xaml
    /// </summary>
    public partial class MonthDay : UserControl
    {
        private TelegramLib.MainClasses.Messages.Message _pinned;
        private string _pinnerLogin;
        private TelegramLib.MainClasses.Messages.StaticMessage _statMes;
        private TelegramLib.MainClasses.UserChat _chat;
        private TelegramLib.MainClasses.TelSystem _system;

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
            TelegramLib.MainClasses.Messages.Message message,
            TelegramLib.MainClasses.Messages.StaticMessage statMes,
            TelegramLib.MainClasses.UserChat chat,
            TelegramLib.MainClasses.TelSystem system)
        {
            _pinned = message;
            _pinnerLogin = pinner;
            _statMes = statMes;
            _chat = chat;
            _system = system;

            InitializeComponent();

            SetPinnedMessageParams();
        }

        private const string _deletedMessage = "Deleted Message";

        private const string _pinnedStr = "pinned";
        private const string _mediaStr = "media";
        private const string _SharedStr = "Shared contact";
        public void SetPinnedMessageParams()
        {
            //Set pinnedLogin 
            LeftMessage.Text = _pinnerLogin;

            if (SetAutoDeleteString()) return;
            if (SetDateStatMes()) return;

            CenterMessage.Text = _pinnedStr;
            RightMessage.Text =
                _pinned is null ? _deletedMessage :
                _pinned is TelegramLib.MainClasses.Messages.TextMessage text ? text.Text :
                _pinned is TelegramLib.MainClasses.Messages.MediaAction media ? _mediaStr :
                _SharedStr;
        }

        public bool SetDateStatMes()
        {
            if (_statMes.Date is null) return false;

            PinnedMessageCol.Width = new GridLength(0);

            LeftMessage.Text = _statMes.Date.Value.Day.ToString();
            CenterMessage.Text = _statMes.Date?.ToString("MMMM", CultureInfo.InvariantCulture);

            return true;
        }

        public bool SetAutoDeleteString()
        {
            const string _autoDel = "canceled auto-del";
            const string _autoDelDur = "set autoDel dur:";


            if (_statMes.DelType is null) return false;

            CenterColumn.Width = new GridLength(0);
            if (_statMes.DelType == AutoDeleteType.Nothing)
            {
                RightMessage.Text = _autoDel;
            }
            else
            {
                RightMessage.Text =
                    $"{_autoDelDur} {_system.GetAutDelDurationInString((AutoDeleteType)_statMes.DelType)}";
            }

            RightMessage.MouseLeftButtonDown += SetAutoDelPage;

            return true;
        }

        public void SetAutoDelPage(object sender, MouseEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this))
                .SetSecondaryFrame(new NewMessagesDeletion(_chat, _system));
        }

        private void RightMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (RightMessage.Text != _deletedMessage) RightMessage.TextDecorations = TextDecorations.Underline;
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
