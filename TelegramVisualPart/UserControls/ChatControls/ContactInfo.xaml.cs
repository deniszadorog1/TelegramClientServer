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

        private void MenuButGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
        
        }

        private void BlockLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.ActionsFolder.BlockContact());
        }

        private void DeleteLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.ActionsFolder.DeleteContact());
        }

        private void EditContactLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.ActionsFolder.EditUserContact());
        }

        private void ShareLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.ActionsFolder.ShareContact());

        }
    }
}
