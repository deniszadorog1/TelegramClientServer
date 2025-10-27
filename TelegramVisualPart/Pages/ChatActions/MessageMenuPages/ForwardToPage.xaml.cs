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
using TelegramLib.MainClasses;
using TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages;

namespace TelegramVisualPart.Pages.ChatActions.MessageMenuPages
{
    /// <summary>
    /// Логика взаимодействия для ForwardToPage.xaml
    /// </summary>
    public partial class ForwardToPage : Page
    {
        private TelSystem _system;
        private TelegramLib.MainClasses.Messages.Message _mes;
        public ForwardToPage(TelSystem system,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            _system = system;
            _mes = mes;

            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            ChatsPanel.Children.Clear();

            for (int i = 0; i < _system.Chats.Count; i++)
            {
                ListBoxItem item = new ListBoxItem()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = _system.Chats[i].Chatter.Id,
                    Padding = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Top,
                };
                item.PreviewMouseDown += Contacts_PreviewMouseDown;

                ChatToApply contact = new ChatToApply(_system.Chats[i].Chatter);
                contact.HorizontalAlignment = HorizontalAlignment.Stretch;

                contact.AddedUserImage(_system.Chats[i].Chatter);
                contact.TypeName.Text = _system.Chats[i].Chatter.Login;
                contact.AutoDeletionType.Text = _system.Chats[i].Chatter.GetLastSeenInChat();

                contact.Tag = _system.Chats[i].Chatter.GetFirstImageName().Name;
                item.Content = contact;

                ChatPanel.Items.Add(item);
            }
        }

        public void Contacts_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item) return;
            int.TryParse(item.Tag.ToString(), out int userSendId);

            ((MainWindow)Window.GetWindow(this)).SetForwardMessage(_mes, userSendId);
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }
    }
}
