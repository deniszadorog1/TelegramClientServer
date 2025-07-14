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
using TelegramVisualPart.Enums;
using TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для AutoDeleteForUsers.xaml
    /// </summary>
    public partial class ToChooseChats : Page
    {
        public ToChooseChats()
        {
            InitializeComponent();         
        }

        private List<UserContactcs> _contacts;
        private ChooseType _type;

        //Set here chosen contacts
        public ToChooseChats(ChooseType type)
        {
            //_contacts = toChoseFrom;
            _type = type;

            InitializeComponent();

            SetParams();
        }

        public void SetParams() 
        {
            PageName.Text = _type == ChooseType.AlwaysShare ? "Always share with" :
                "Never share with";
        }        

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void ApplyBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        public void AddAppliedChat(ChatToApply chatControl)
        {
            ChosenChat toAdd = new ChosenChat()
            {
                //Set here name as chatUser login (to compare with it)
                Name = chatControl.Name,
                VerticalAlignment = VerticalAlignment.Center
            };

            toAdd._removeChatEvent += ChosenChat_RemoveClicked;

            //Set params of chosen chat
            ChatsPanel.Children.Add(toAdd);
        }

        public void RemoveAppliedChat(ChatToApply toRemove)
        {
            ChatsPanel.Children.Remove(ChatsPanel.Children.OfType<ChosenChat>().
                Where(x => x.Name == toRemove.Name).First());
        }

        private void TestParam_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ChatToApply) return;

            ChatToApply temp = sender as ChatToApply;

            if (temp.GetIdClicked())
            {
                AddAppliedChat(temp);
                return;
            }
            RemoveAppliedChat(temp);
        }

        private void ChosenChat_RemoveClicked(object sender, EventArgs e)
        {
            if (sender is not ChosenChat) return;
            ChosenChat test = sender as ChosenChat;

            //Clear chat to apply
            ClearChatToApply(test);

            //Remove chosen chat
            ChatsPanel.Children.Remove(test);
        }

        private void ClearChatToApply(ChosenChat test)
        {
            ChatToApply toClear = ChatsPanelToChoose.Children.OfType<ChatToApply>().
                Where(x => x.Name == test.Name).FirstOrDefault();
            if (toClear is null) return;

            toClear.DiscardChat();
        }

    }
}
