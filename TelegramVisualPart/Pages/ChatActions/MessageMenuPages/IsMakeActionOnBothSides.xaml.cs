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
        
        private BothUsersMessageAction _actionType;
        private bool _isBothDelete = true;

        public event Func<ValueTask> MakeAction;
            
        //To set action for one message
        public IsMakeActionOnBothSides(
            TelegramLib.MainClasses.User user,
            BothUsersMessageAction bothType)
        {
            _user = user;
            _actionType = bothType;

            InitializeComponent();

            SetBasicParams();
        }

        //To Delete selected messages
        public IsMakeActionOnBothSides(TelegramLib.MainClasses.User user,
            bool isBothDelete)
        {
            _user = user;
            _actionType = BothUsersMessageAction.Delete;
            _isBothDelete = isBothDelete;

            InitializeComponent();
            SetBasicParams();
        }

        public void SetBasicParams()
        {
            const int bothDeleteBlockHeight = 25;

            BothUsersMessageAction act = 
                _actionType == BothUsersMessageAction.SchedDelete ? 
                BothUsersMessageAction.Delete : _actionType;

            ActionName.Text = act.ToString();
            ActionCheckText.Text = act.ToString();
            DeleteBut.Content = act.ToString();
           
            LoginPartCheckText.Text = _user is null ? string.Empty : _user.Login;

            IsInBoth.Visibility = _isBothDelete ? Visibility.Visible : Visibility.Hidden;
            if(!_isBothDelete)
            {
                CheckBoxRow.Height = new GridLength(0);
                Height -= bothDeleteBlockHeight;
            }

            if(_actionType == BothUsersMessageAction.UnPin || 
                _actionType == BothUsersMessageAction.SchedDelete)
            {
                const int checkBoxRowHeight = 50;

                CheckBoxStack.Visibility = Visibility.Hidden;
                Height -= checkBoxRowHeight;
                CheckBoxRow.Height = new GridLength(0);

                IsInBoth.IsChecked = true;
            }
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
