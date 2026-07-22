using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using AutoDeleteType = TelegramLib.Enums.Chat.AutoDeleteType;

namespace TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion
{
    /// <summary>
    /// Логика взаимодействия для NewMessagesDeletion.xaml
    /// </summary>
    public partial class NewMessagesDeletion : Page
    {
        private UserChat _chat;
        private TelSystem _system;

        public NewMessagesDeletion(UserChat chat, TelSystem system)
        {
            _chat = chat;
            _system = system;
            InitializeComponent();

            SetRemoveButVisibility();
            SetChosenAutoDelType();

            SetLanguageText.SetMessDeletion(this);
            SetBaseTextBlocks();
        }

        public void SetBaseTextBlocks()
        {
            if (_chat.Chatter is null) return;
            SetDestructBut.Text = _chat.GetChatter().Login;
        }

        public void SetRemoveButVisibility()
        {
            if (_chat.AutoDel == AutoDeleteType.Nothing)
            {
                RemoveBut.Visibility = Visibility.Hidden;
                return;
            }
        }

        public void SetChosenAutoDelType()
        {
            if (_chat.AutoDel == AutoDeleteType.Nothing) return;
            SpecialList.ValueByIndex((int)_chat.AutoDel);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //SpecialList.SetAutoDeletionValue(_chat.GetChatter().AutoDeletion);     
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void SetDestructBut_MouseEnter(object sender, MouseEventArgs e)
        {
            SetDestructBut.TextDecorations = TextDecorations.Underline;
            Cursor = Cursors.Hand;
        }

        private void SetDestructBut_MouseLeave(object sender, MouseEventArgs e)
        {
            SetDestructBut.TextDecorations = null;
            Cursor = null;
        }

        private void SetDestructBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Go to anouther page
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SelfDestructTimer(_system));
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
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private async void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            AutoDeleteType type = SpecialList.GetChosenAutoDelItem();

            UserContactcs contact = _system.GetContactByUserId(_chat.GetChatter().Id);
            if (contact is not null) contact.AutoDeletion = new AutoDeleteDuration(type);

            _chat.AutoDel = type;

            AddStaticMessage();

            //Set Auto Del in DB
            await ApiService.SetAutoDeletion(_chat.Id, type);

            ((MainWindow)Window.GetWindow(this)).UpdateAutoDelVis(_chat);
            ((MainWindow)Window.GetWindow(this)).UpdateChatAutDelIconVisibility();
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private async void RemoveBut_Click(object sender, RoutedEventArgs e)
        {
            _chat.AutoDel = AutoDeleteType.Nothing;
            await ApiService.SetAutoDeletion(_chat.Id, _chat.AutoDel);

            AddStaticMessage();

            ((MainWindow)Window.GetWindow(this)).UpdateAutoDelVis(_chat);
            ((MainWindow)Window.GetWindow(this)).UpdateChatAutDelIconVisibility();
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        public async void AddStaticMessage()
        {
            await ((MainWindow)Window.GetWindow(this))
                .AddStatMessage(new StaticMessage(_chat.AutoDel, _system.LoggedUser.Id),
                true, _chat);
        }
    }
}
