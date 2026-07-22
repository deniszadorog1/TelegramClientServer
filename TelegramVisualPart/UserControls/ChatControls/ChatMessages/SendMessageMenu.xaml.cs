using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Pages.ChatActions;

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages
{
    /// <summary>
    /// Логика взаимодействия для SendMessageMenu.xaml
    /// </summary>
    public partial class SendMessageMenu : UserControl
    {
        public SendMessageMenu()
        {
            InitializeComponent();

            SetVisParams();
        }

        private UserChat _chatControl;
        private TelegramLib.MainClasses.TelSystem _system;
        public void SetUserChatControl(
            UserChat chatControl,
            TelegramLib.MainClasses.TelSystem system)
        {
            _chatControl = chatControl;
            _system = system;
        }

        public void SetVisParams()
        {
            ScedSend.SetParams(MaterialDesignThemes.Wpf.PackIconKind.CalendarToday, "schedule message");
        }

        private void ScedSend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_chatControl is null || _chatControl.GetChat() is null) return;

            if (string.IsNullOrWhiteSpace(_chatControl.CommentTextBox.Text))
            {
                ((MainWindow)Window.GetWindow(this)).SetTemporaryText("Misha, Stop doing weird tests!!!");
                return;
            }

            //form message
            (TelegramLib.MainClasses.Messages.Message mes,
             TelegramLib.MainClasses.Messages.Message toReply) =
             _chatControl.GetTextMessageToSend(_chatControl.CommentTextBox.Text);

            List<Message> forwardMes = _chatControl.GetToForwardMessages();

            if (mes is null ||

               (mes is TelegramLib.MainClasses.Messages.TextMessage textMes &&
                textMes.Text == string.Empty)) return;

            if (mes is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                text.Text = text.Text.Trim(' ');
                text.Text = text.Text.Replace("\n", "");
                text.Text = text.Text.Trim('\r');
                text.Text = text.Text.Replace("\r\n", "");
            }

            SetScheduleMessage message =
                new SetScheduleMessage(_chatControl.GetChat(), new List<Message>() { mes }, _system, _chatControl.GetToForwardMessages());

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(message);
        }
    }
}
