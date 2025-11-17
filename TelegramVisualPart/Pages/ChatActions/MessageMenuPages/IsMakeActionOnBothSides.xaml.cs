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

namespace TelegramVisualPart.Pages.ChatActions.MessageMenuPages
{
    /// <summary>
    /// Логика взаимодействия для IsMakeActionOnBothSides.xaml
    /// </summary>
    public partial class IsMakeActionOnBothSides : Page
    {
        private TelegramLib.MainClasses.User _user;
        private BothUsersMessageAction _bothType;

        public event Func<ValueTask> MakeAction;
            
        //To set action for one message
        public IsMakeActionOnBothSides(
            TelegramLib.MainClasses.User user,
            BothUsersMessageAction bothType)
        {
            _user = user;
            _bothType = bothType;

            InitializeComponent();

            SetBasicParams();
        }

        //To Delete selected messages
        public IsMakeActionOnBothSides(TelegramLib.MainClasses.User user)
        {
            _user = user;
            _bothType = BothUsersMessageAction.Delete;

            InitializeComponent();
            SetBasicParams();
        }

        public void SetBasicParams()
        {
            ActionName.Text = _bothType.ToString();
            ActionCheckText.Text = _bothType.ToString();
            DeleteBut.Content = _bothType.ToString();
           

            LoginPartCheckText.Text = _user.Login;
        }

        private void DeleteBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["CloseButBg"];
        }

        private void DeleteBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void DeleteBut_Click(object sender, RoutedEventArgs e)
        {
            MakeAction?.Invoke();
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void CheckBoxStack_MouseEnter(object sender, MouseEventArgs e)
        {
            //Set action
            Cursor = Cursors.Hand;
        }

        private void CheckBoxStack_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void CheckBoxStack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsInBoth.IsChecked = !IsInBoth.IsChecked;
        }
    }
}
