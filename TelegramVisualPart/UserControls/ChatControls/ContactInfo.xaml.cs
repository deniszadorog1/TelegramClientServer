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
using TelegramLib.MainClasses;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ContactInfo.xaml
    /// </summary>
    public partial class ContactInfo : UserControl
    {
        public ContactInfo()
        {
            InitializeComponent();
            SetIconsSize();
        }

        private TelegramLib.MainClasses.UserChat _chat;
        public void SetContactInfo(TelegramLib.MainClasses.UserChat chat)
        {
            _chat = chat;
            SetUserParams();
        }

        private void SetUserParams()
        {
            Username.Text = _chat.GetChatter().Name;
            LastSeenOnline.Text = _chat.GetChatter().GetLastSeen();

            MobileNumber.SetUpperText(_chat.GetChatter().GetPhoneNumber());
            MobileNumber.SetBottomText("Mobile");

            Login.SetUpperText(_chat.GetChatter().GetUserName());
            Login.SetBottomText("Username");

            Birthdate.SetUpperText(_chat.GetChatter().GetBirthDate());
            Birthdate.SetBottomText("Date of Birth");

            TNOtificationToggle.IsChecked = _chat.GetChatter().GetNotifsState();
        }

        private void SetIconsSize()
        {           
            SetIconSize(InfoIcon);
            SetIconSize(BellIcon);

            SetIconSize(ImageIcon);
            SetIconSize(VideoIcon);
            SetIconSize(FileIcon);
            SetIconSize(LinkIcon);
            SetIconSize(GifIcon);

            SetIconSize(SendIcon);
            SetIconSize(PenIcon);
            SetIconSize(CanIcon);
            SetIconSize(HandIcon);

            ContactMenu.Margin = new Thickness(
                0,
                UpperRow.Height.Value + 10,
                20,
                0
                );
        }

        private const int _iconWidth = 30;
        private const int _iconHeight = 30;
        private void SetIconSize(PackIcon icon)
        {
            icon.Width = _iconWidth;
            icon.Height = _iconHeight;
        } 

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is Grid grid)
            {
                grid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
            }
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if(sender is Grid grid)
            {
                grid.Background = Brushes.Transparent;
            }
            Cursor = null;
        }

        private void SendMessageBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = Brushes.White;
        }

        private void CloseButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = Brushes.Gray;
        }

        private void CloseButGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void MenuButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            MoreInfoBut.Foreground = Brushes.White;
        }

        private void MenuButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            MoreInfoBut.Foreground = Brushes.Gray;
        }

        private void BlockLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.ActionsFolder.BlockContact());
        }

        private void DeleteLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                new Pages.UserInfoContact.ActionsFolder.DeleteContact());
        }

        private void EditContactLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.ActionsFolder.EditUserContact());
        }

        private void ShareLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.ActionsFolder.ShareContact());

        }

        private void Line_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_chat is null) return;
            if (sender is Grid grid)
            {
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.
                    SentObjectsUserInfo.SentItemsUserContact(
                    ((MainWindow)Window.GetWindow(this)).GetSystem(),
                    GetItemType(grid.Name), _chat));
            }
        }

        private Enums.SentItemsTypes GetItemType(string name)
        {
            return name == PhotosLine.Name.ToString() ? Enums.SentItemsTypes.Photos :
                name == VideosLine.Name.ToString() ? Enums.SentItemsTypes.Video :
                name == FilesLine.Name.ToString() ? Enums.SentItemsTypes.File :
                name == LinksLine.Name.ToString() ? Enums.SentItemsTypes.SharedLinks :
                name == GIFsLine.Name.ToString() ? Enums.SentItemsTypes.GIFs :
                Enums.SentItemsTypes.Photos;
        }

        private bool _isMenuOpen = false;

        private void MenuButGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMenuOpen = !_isMenuOpen;

            if (_isMenuOpen) ContactMenu.Visibility = Visibility.Visible;
            else ContactMenu.Visibility = Visibility.Hidden;
        }
    }
}
