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
using System.Windows.Shapes;
using TelegramLib.Enums.Settings.Notifs;
using TelegramVisualPart.UserControls.ChatsControls;
using TelegramVisualPart.UserControls.SettingsControls.NotificationPrivacy;

namespace TelegramVisualPart.CustWindows
{
    /// <summary>
    /// Логика взаимодействия для ToastWindow.xaml
    /// </summary>
    public partial class ToastWindow : Window
    {
        public ToastWindow()
        {
            InitializeComponent();
        }

        //Items notifications
        private int _amountTestMessages;
        private NotifMessageSide _side;
        public ToastWindow(int amountTestMessages, NotifMessageSide side)
        {
            _amountTestMessages = amountTestMessages;
            _side = side;

            InitializeComponent();

            //Set test Messages
            SetTestMessages();

            //To set window side
            SetWindowSide();
        }

        public void SetWindowSide()
        {
            //base.OnContentRendered(e);

            var screen = SystemParameters.WorkArea;

            switch (_side)
            {
                case NotifMessageSide.TopLeft:
                    {
                        VerticalAlignment = VerticalAlignment.Top;
                        HorizontalAlignment = HorizontalAlignment.Left;
                        MessagesStack.VerticalAlignment = VerticalAlignment.Top;

                        this.Left = 10;
                        this.Top = 10;
                        break;
                    };
                case NotifMessageSide.TopRight:
                    {
                        VerticalAlignment = VerticalAlignment.Top;
                        HorizontalAlignment = HorizontalAlignment.Right;
                        MessagesStack.VerticalAlignment = VerticalAlignment.Top;

                        this.Left = screen.Width - Width - 10;
                        this.Top = 10;
                        break;
                    }
                case NotifMessageSide.BottomRight:
                    {
                        VerticalAlignment = VerticalAlignment.Bottom;
                        HorizontalAlignment = HorizontalAlignment.Right;
                        MessagesStack.VerticalAlignment = VerticalAlignment.Bottom;

                        this.Left = screen.Width - Width - 10; 
                        this.Top = screen.Height - Height - 10;

                        break;
                    }
                case NotifMessageSide.BottomLeft:
                    {
                        VerticalAlignment = VerticalAlignment.Bottom;
                        HorizontalAlignment = HorizontalAlignment.Left;
                        MessagesStack.VerticalAlignment = VerticalAlignment.Bottom;

                        this.Left = 10;
                        this.Top = screen.Height - Height - 10;
                        break;
                    }
            }
            //await Task.Delay(3000);

            //Close();
        }


        private void SetTestMessages()
        {
            MessagesStack.Children.Clear();

            for (int i = 0; i < _amountTestMessages; i++)
            {
                WindowMessage message = new WindowMessage();

                MessagesStack.Children.Add(message);
            }
        }





    }
}
