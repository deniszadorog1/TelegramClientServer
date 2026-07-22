using MaterialDesignThemes.Wpf;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.Windows;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoggedUserProfile.xaml
    /// </summary>
    public partial class LoggedUserProfile : Page
    {
        private User _user;
        private TelSystem _system;

        public LoggedUserProfile(User user, TelSystem system)
        {
            _user = user;
            _system = system;

            InitializeComponent();
            SetBasicParams();

            SetLanguageText.SetLoggedUserProfile(this);
        }

        public async void SetBasicParams()
        {
            CloseBut.IconType.Kind = PackIconKind.Close;
            SettingsBut.IconType.Kind = PackIconKind.LeadPencil;

            UserLoginBlock.Text = _user.Name;

            SetOnlineStatus();
       
            PhoneNumberBlock.Text = _user.PhoneNumber;
            UserNameBlock.Text = _user.Login;
            BioBlock.Text = _user.BIO;
            if(_user.BirthDay is not null)BirthdayBlock.Text = $"{_user.BirthDay.Value.Day}.{_user.BirthDay.Value.Month}.{_user.BirthDay.Value.Year}";


            string path = await FilesAction.GetUserImagePath(_user.GetFirstImageName().Name);

            UserImage.ImageSource = UserImage.ImageSource = 
                ApiService.GetCachedBitmap(path) is BitmapImage b and not null ? b : 
                await SignalRHelperService.LoadBitmap(path);
            
            BlocksSize();
        }

        public void BlocksSize()
        {
            SetRowHeight(BioColumn, BioBlock, BioRowIcon);
            SetRowHeight(BirthDayColumn, BirthdayBlock, BirthRowIcon);
        }

        public void SetRowHeight(RowDefinition row, TextBlock block, RowDefinition iconRow)
        {
            const int devHeight = 40;
            if (block.Text == string.Empty)
            {
                row.Height = new GridLength(0);
                iconRow.Height = new GridLength(0);
                BlockColumn.Height = new GridLength(BlockColumn.Height.Value - devHeight);
            }
            else
            {
                if (block == BioBlock)
                {
                    SetBioBlockHeight();
                    return;
                }
                row.Height = new GridLength(1, GridUnitType.Star);
                iconRow.Height = new GridLength(1, GridUnitType.Star);
                BlockColumn.Height = new GridLength(BlockColumn.Height.Value + devHeight);
            }
        }

        public void SetBioBlockHeight()
        {
            const int maxLength = 30;
            const double minLengthVal = 1.25; 
            const double maxLengthVal = 1.75; 

            if(BioBlock.Text.Length < maxLength)
            {
                BioColumn.Height = new GridLength(minLengthVal, GridUnitType.Star);
                return;
            }
            BioColumn.Height = new GridLength(maxLengthVal, GridUnitType.Star);
        }

        private void SetOnlineStatus()
        {
            if (_system.LoggedUser.IsOnline)
            {
                LastSeenOnline.Foreground =
                    (SolidColorBrush)Application.Current.FindResource("TempActiveTextColor");
                LastSeenOnline.Text = VisConstParamsJsonService.GetStringByName("OnlineStat");
                return;
            }
            LastSeenOnline.Text = $"{_system.LoggedUser.LastSeenOnline.Day}.{_system.LoggedUser.LastSeenOnline.Month}.{_system.LoggedUser.LastSeenOnline.Year}";
        }

        private void Buts_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.White;
            Cursor = Cursors.Hand;
        }

        private void Buts_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.Gray;
            Cursor = null;
        }

        private void SettingsBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new MyProfile.MyProfileSettings(_user, _system, this));
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void Ellipse_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MediaWindow mediaWindow = new MediaWindow(
                _system.LoggedUser, (MainWindow)Window.GetWindow(this), 
                Enums.MediaShow.MediaShowType.UserImages, _system);

            //Is exist
            if (((MainWindow)Window.GetWindow(this))
                .IsMediaWindowIsExistByUserId(_system.LoggedUser.Id)) return;
            mediaWindow.Show();
        }

        private async void ToRemoveUserImage_MouseDown(object sender, EventArgs e)
        {
            UserImage.ImageSource = new BitmapImage(new Uri(
                await FilesAction.GetUserImagePath(_user.GetFirstImageName().Name), UriKind.Absolute));
        }

        private void UserNameBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            UserNameBlock.TextDecorations = TextDecorations.Underline;
            Cursor = Cursors.Hand;
        }

        private void UserNameBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            UserNameBlock.TextDecorations = null;
            Cursor = null;
        }

        private void UserNameBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(UserNameBlock.Text);
            Window window = Window.GetWindow(this);

            if(window is MainWindow main)
            {
                main.SetTemporaryText("Username is copied!");
            }
        }
    }
}
