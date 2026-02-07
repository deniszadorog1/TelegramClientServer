using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Enums.Menus;
using TelegramVisualPart.UserControls.ChatControls;
using TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu;

namespace TelegramVisualPart.Pages.ChatActions.MessageMenuPages
{
    /// <summary>
    /// Логика взаимодействия для ForRepMenu.xaml
    /// </summary>
    public partial class ForRepMenu : Page
    {
        private List<TelegramLib.MainClasses.Messages.Message> _messages;

        public event Action DoNotSendDel;
        public event Action ChangeRecipientDel;
        public event Action HideSenderNameDel;

        private RepForType _actType;
        private TelSystem _system;

        public ForRepMenu(RepForType actType,
            List<TelegramLib.MainClasses.Messages.Message> messages,
            TelSystem system)
        {
            _actType = actType;   
            _messages = messages;
            _system = system;

            InitializeComponent();

            SetBasicParams();
            SetVisibleMessages();
        }

        public void SetVisibleMessages()
        {
            if (_actType == RepForType.ForwardAction)
            {
                SetForwardedMessages();
            }
        }

        public void SetForwardedMessages()
        {
            for (int i = 0; i < _messages.Count; i++)
            {
                if (_messages[i] is TelegramLib.MainClasses.Messages.TextMessage text)
                {
                    UserControls.ChatControls.TextMessage control = new
                        UserControls.ChatControls.TextMessage(_system, text);

                    AddControlIsStack(control);
                }
                else if (_messages[i] is MediaAction media)
                {
                    //MediaMessage message = new MediaMessage()
                }
            }
        }

        public void AddControlIsStack(UserControl control)
        {
            control.HorizontalAlignment = HorizontalAlignment.Right;
            control.Margin = new Thickness(0, 5, 5, 5);

            MessagesStack.Children.Add(control);
        }

        public void SetBasicParams()
        {
            ChangeRecipient.SetParams(MaterialDesignThemes.Wpf.PackIconKind.FormatRotateNinety, "Change sended");
            DoNotSend.SetParams(MaterialDesignThemes.Wpf.PackIconKind.GarbageCanOutline, "Cancel action");
            HideSenderNameBut.SetParams(MaterialDesignThemes.Wpf.PackIconKind.HideOutline, "Hide Sender");
        }

        private void HideSenderNameBut_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetForwardedVisibility();
            HideSenderNameDel?.Invoke();
        }

        public void SetForwardedVisibility()
        {
            //Get is show param
            //Set forward row height

            if (MessagesStack.Children.Count == 0) return;
            bool isShow = GetForwardVisParam(MessagesStack.Children[0] as UserControl);

            for(int i = 0; i < MessagesStack.Children.Count; i++)
            {
                if (MessagesStack.Children[i] is UserControls.ChatControls.TextMessage text) text.SetForwardedRowHeight(isShow);
                if (MessagesStack.Children[i] is MediaMessage media) media.SetForwardedRowHeight(isShow);
            }
        }

        public bool GetForwardVisParam(UserControl control)
        {
            if (control is MediaMessage media) return media.ForwardedRow.Height.Value == 0;
            else if (control is UserControls.ChatControls.TextMessage text) return text.ForwardedRow.Height.Value == 0;

            return false;
        }

        private void ChangeRecipient_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeRecipientDel?.Invoke();
        }

        private void DoNotSend_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoNotSendDel?.Invoke();

            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void SaveBut_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Set action

            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void CancelBut_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }
    }
}
