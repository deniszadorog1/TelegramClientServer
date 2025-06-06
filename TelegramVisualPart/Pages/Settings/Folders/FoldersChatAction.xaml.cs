using MaterialDesignThemes.Wpf;
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
using TelegramVisualPart.Enums;

namespace TelegramVisualPart.Pages.Settings.Folders
{
    /// <summary>
    /// Логика взаимодействия для FoldersChatAction.xaml
    /// </summary>
    public partial class FoldersChatAction : Page
    {
        private Frame _frame;
        private FolderChatActionType _type;
        public FoldersChatAction(Frame frame, FolderChatActionType type)
        {
            _frame = frame;
            _type = type;

            InitializeComponent();

            SetBasicBlocks();
            SetBlocksByType();
        }

        public void SetBlocksByType()
        {
            if(_type == FolderChatActionType.AddChatInFolder)
            {
                ChatTypesStack.Children.Remove(MutedChats);
                ChatTypesStack.Children.Remove(ReadChats);
                ChatTypesStack.Children.Remove(ArchivedChats);
                return;
            }

            ChatTypesStack.Children.Remove(ContactsChats);
            ChatTypesStack.Children.Remove(NoneContactsChats);
            ChatTypesStack.Children.Remove(GroupsChats); 
            ChatTypesStack.Children.Remove(ChannelsChats);
            ChatTypesStack.Children.Remove(BotsChats);

            ChatTypesRow.Height = new GridLength(MutedChats.Height * 3 + 10);
             
        }

        public void SetBasicBlocks()
        {
            ContactsChats.IconType.Kind = PackIconKind.Account;
            ContactsChats.TypeName.Text = "Contacts";
            ContactsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderContactColor"];

            NoneContactsChats.IconType.Kind = PackIconKind.QuestionMarkCircle;
            NoneContactsChats.TypeName.Text = "Non-Contacts";
            NoneContactsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderNonContactColor"];

            GroupsChats.IconType.Kind = PackIconKind.UserGroup;
            GroupsChats.TypeName.Text = "Groups";
            GroupsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderGroupColor"];

            ChannelsChats.IconType.Kind = PackIconKind.AirHorn;
            ChannelsChats.TypeName.Text = "Channels";
            ChannelsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderChannelsColor"];

            BotsChats.IconType.Kind = PackIconKind.Android;
            BotsChats.TypeName.Text = "Bots";
            BotsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderBotsColor"];


            MutedChats.IconType.Kind = PackIconKind.VolumeMute;
            MutedChats.TypeName.Text = "Muted";
            MutedChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderBotsColor"];

            ReadChats.IconType.Kind = PackIconKind.MessageText;
            ReadChats.TypeName.Text = "Read";
            ReadChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderNonContactColor"];

            ArchivedChats.IconType.Kind = PackIconKind.Archive;
            ArchivedChats.TypeName.Text = "Archived";
            ArchivedChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderContactColor"];
        }

        private void ClearSearchBoxGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ClearSearchBoxBut.Foreground = Brushes.White;
        }

        private void ClearSearchBoxGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearSearchBoxBut.Foreground = Brushes.Gray;
        }

        private void ClearSearchBoxGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ChatSearchBox.Text = string.Empty;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(_frame)).ClearThirdFrame();
        }

        private void ChatTypes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
