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

            //form message
            (TelegramLib.MainClasses.Messages.Message mes,
             TelegramLib.MainClasses.Messages.Message toReply) = 
             _chatControl.GetTextMessageToSend(_chatControl.CommentTextBox.Text);

            if (mes is null) return;

            SetScheduleMessage message = 
                new SetScheduleMessage(_chatControl.GetChat(), mes, _system);
            
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(message);
        }
    }
}
