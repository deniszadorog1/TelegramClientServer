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
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;

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

            SetLanguageText.SetMessDeletion(this);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SpecialList.SetAutoDeletionValue(_chat.GetChatter().AutoDeletion);     
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
            _chat.GetChatter().AutoDeletion = new AutoDeleteDuration(type);
            _chat.AutoDel = type;

            //Set Auto Del in DB
            await ApiService.SetAutoDeletion(_chat.Id, type);

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

    }
}
