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
using System.Windows.Threading;
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
        private TelegramLib.MainClasses.Messages.Message _replied;

        public ForRepMenu(RepForType actType,
            TelegramLib.MainClasses.Messages.Message repliedMessage,
            TelSystem system)
        {
            _actType = actType;
            _replied = repliedMessage;
            _system = system;

            InitializeComponent();

            SetBasicParams();
            SetVisibleMessages();

            RemoveRestButtons();
        }

        public ForRepMenu(RepForType actType,
            List<TelegramLib.MainClasses.Messages.Message> messages,
            TelSystem system, bool isForwardVis)
        {
            _actType = actType;
            _messages = messages;
            _system = system;

            InitializeComponent();

            SetBasicParams();
            SetVisibleMessages();

            RemoveRestButtons();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                SetVisForForwardedFromParam(isForwardVis);
            }), DispatcherPriority.Loaded);
        }

        public void RemoveRestButtons()
        {
            if (_actType == RepForType.ForwardAction)
            {
                ActionButtonsStack.Children.Remove(ShowMessage);
                return;
            }

            ActionButtonsStack.Children.Remove(HideSenderNameBut);
        }

        public void SetVisibleMessages()
        {
            if (_actType == RepForType.ForwardAction)
            {
                SetForwardedMessages();
            }
            else if (_actType == RepForType.ReplyAction)
            {
                AddMessageInStack(_replied);
            }
        }

        public void SetForwardedMessages()
        {
            for (int i = 0; i < _messages.Count; i++)
            {
                AddMessageInStack(_messages[i]);
            }
        }

        public async void AddMessageInStack(Message message)
        {
            if (message is TelegramLib.MainClasses.Messages.TextMessage text)
            {
                UserControls.ChatControls.TextMessage control = new
                    UserControls.ChatControls.TextMessage(_system, text);

                AddControlIsStack(control);
            }
            else if (message is MediaAction media)
            {
                MediaMessage mediaMes = new MediaMessage(_system, media);
                AddControlIsStack(mediaMes);
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
            HideSenderNameBut.SetParams(MaterialDesignThemes.Wpf.PackIconKind.HideOutline, "Hide Sender");
            ShowMessage.SetParams(MaterialDesignThemes.Wpf.PackIconKind.EyeOutline, "Show in Chat");

            DoNotSend.SetParams(MaterialDesignThemes.Wpf.PackIconKind.GarbageCanOutline, "Cancel action");

            var brush = (SolidColorBrush)Application.Current.Resources["ToBlockContact"];
            DoNotSend.PaintBlocks(brush);
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

            SetVisForForwardedFromParam(isShow);
        }

        public void SetVisForForwardedFromParam(bool isShow)
        {
            for (int i = 0; i < MessagesStack.Children.Count; i++)
            {
                if (MessagesStack.Children[i] is UserControls.ChatControls.TextMessage text) text.SetForwardedRowHeight(isShow);
                if (MessagesStack.Children[i] is MediaMessage media) media.SetForwardedRowHeight(isShow);
            }

            ChangeForwardText(isShow);
        }

        public void ChangeForwardText(bool isShow)
        {
            HideSenderNameBut.ButText.Text = isShow ? "Hide sender" : "Show sender";
        }

        public bool IsForwardedRowIsHidden()
        {
            if (MessagesStack.Children.Count == 0) return false;

            if (MessagesStack.Children[0] is UserControls.ChatControls.TextMessage text) return text.IsForwardedRowIsHidden();
            if (MessagesStack.Children[0] is MediaMessage media) return media.IsForwardedRowIsHidden();

            return false;
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

            //if(IsForwardedRowIsHidden()) 

            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
            ((MainWindow)Window.GetWindow(this)).FocusUserChat();
        }

        private void CancelBut_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
            ((MainWindow)Window.GetWindow(this)).FocusUserChat();
        }

        private void ShowMessage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_replied is null) return;

            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
            ((MainWindow)Window.GetWindow(this)).ShowChosenMessageByMessageId(_replied.Id);
        }
    }
}
