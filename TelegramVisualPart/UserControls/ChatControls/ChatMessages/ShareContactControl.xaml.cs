using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages
{
    /// <summary>
    /// Логика взаимодействия для ShareContactControl.xaml
    /// </summary>
    public partial class ShareContactControl : UserControl
    {
        public event Func<Task> SharedClicked;
        private TelSystem _system;

        public ShareContactControl(TelSystem system)
        {
            InitializeComponent();

            _system = system;

            SetEvents();
        }

        private void ContactRow_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void ContactRow_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public async Task SetSenderImage(string imgName)
        {
            BgBrush.ImageSource = new BitmapImage(
            new Uri(await FilesAction.GetUserImagePath(imgName), UriKind.Absolute));
        }

        public async Task SetChatterImg(string imgName,
            TelegramLib.MainClasses.User user)
        {
            await SignalRHelperService.SetPhotoInEllipse(user,
                ImageIcon, UserEllipseImage);

            ImageIcon.ImageSource = new BitmapImage(
               new Uri(await FilesAction.GetUserImagePath(imgName), UriKind.Absolute));
        }

        public async Task SetSharedUserImage(string imgName)
        {
            return;
            ImageIcon.ImageSource = new BitmapImage(
                new Uri(await FilesAction.GetUserImagePath(imgName), UriKind.Absolute));
        }

        public void SetSharedUserName(string name)
        {
            NameBlock.Text = name;
        }

        public void SetSharedUserPhoneNumber(string number)
        {
            PhoneNumberBlock.Text = number;
        }

        public void SetSendTime()
        {
            DateTime time = DateTime.Now;

            SendTimeBlock.Text = $"{VisHelper.GetCorrectTimeParamVis(time.Hour.ToString())}:" +
                $"{VisHelper.GetCorrectTimeParamVis(time.Minute.ToString())}";
        }

        private void ContactRow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SharedClicked?.Invoke();
        }

        private const int _tickColWidth = 25;
        public void SetTickVis(string iconName)
        {
            TickColumn.Width = new GridLength(_tickColWidth);
            SetVisibility(iconName);
        }

        public void SetVisibility(string iconName)
        {
            TickIcon.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
        }

        public void SetPinColumnState(bool isPinned)
        {
            if (isPinned) PinnIcon.Visibility = Visibility.Visible;
            else PinnIcon.Visibility = Visibility.Hidden;
        }


        private const int _selectTickColWidth = 30;
        public void SetTickVisibility(bool isVis)
        {
            if (isVis)
            {
                this.Width += _selectTickColWidth;
                TickColumnDef.Width = new GridLength(_selectTickColWidth);
            }
            else if (TickColumnDef.Width.Value != 0)
            {
                this.Width -= _selectTickColWidth;
                TickColumnDef.Width = new GridLength(0);
            }
        }

        public void SetTickVisOnlyTickCol(bool isVis)
        {
            if (isVis)
            {
                TickColumnDef.Width = new GridLength(_selectTickColWidth);
            }
            else if (TickColumnDef.Width.Value != 0)
            {
                TickColumnDef.Width = new GridLength(0);
            }
        }

        public bool IsTickVisible()
        {
            return TickColumnDef.Width.Value != 0;
        }

        public void SetEvents()
        {
            SelectionTickObj.StatusChanged += () =>
            {
                //Pressed on tick
                //Update counter on user chat
                ((MainWindow)Window.GetWindow(this)).UpdateUserChatSelectedAmount();
            };
        }

        public void ChangeTickStatus()
        {
            if (!IsTickVisible()) return;
            SelectionTickObj.SetMirrorStatus();
        }

        public bool IsMessageIdTicked()
        {
            return SelectionTickObj.GetChosenStatus();
        }

        private void SenderEllipseImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Get user 
            DependencyObject check = this.Parent;
            if (check is not ListBoxItem item) return;

            int.TryParse(item.Tag.ToString(), out int mesId);

            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);
            if (mes is null) return;

            bool isSavedChat = _system.GetIsSavedMesChatStatus();

            //Settings logged user page
            if ((_system.LoggedUser.Id == mes.SenderUserId && !isSavedChat) ||
                (isSavedChat && mes.ForwardedFromId is null && mes.SenderUserId == 0) ||
                (isSavedChat && _system.LoggedUser.Id == mes.ForwardedFromId))
            {
                UserInfo logged = new UserInfo(_system.SavedMesesChat, _system);
                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(logged);
                return;
            }

            //Set other user page
            TelegramLib.MainClasses.UserChat chat = isSavedChat && mes.ForwardedFromId is not null ?
                _system.GetChatByChatterId((int)mes.ForwardedFromId) :
                _system.GetChatByMessage(mes);

            UserInfo info = new UserInfo(chat, _system);
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(info);

        }
    }
}
