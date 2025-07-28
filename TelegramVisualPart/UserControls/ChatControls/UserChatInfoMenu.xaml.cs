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
using TelegramVisualPart.Pages;
using TelegramVisualPart.Pages.ChatActions;
using TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для UserChatInfoMenu.xaml
    /// </summary>
    public partial class UserChatInfoMenu : UserControl
    {
       // public event EventHandler ClearHystory;

        public UserChatInfoMenu()
        {
            InitializeComponent();

            SetBasicBlocks();
        }

        private TelegramLib.MainClasses.UserChat _chat;
        public void SetChatParam(TelegramLib.MainClasses.UserChat chat)
        {
            _chat = chat;
        }

        private TelSystem _system;
        public void SetSystemParam(TelSystem system)
        {
            _system = system;
        }

        public void SetBasicBlocks()
        {
            MuteNotifsBut.IconType.Kind = PackIconKind.VolumeMute;
            MuteNotifsBut.ButName.Text = "Mute notifications";

            ViewProfileBut.IconType.Kind = PackIconKind.AccountCircleOutline;
            ViewProfileBut.ButName.Text = "View profile";

            SetWallpaperBut.IconType.Kind = PackIconKind.PaintbrushOutline;
            SetWallpaperBut.ButName.Text = "Set Wallpaper";

            ExportHistoryBut.IconType.Kind = PackIconKind.Export;
            ExportHistoryBut.ButName.Text = "Export chat history";

            ClearChatBut.IconType.Kind = PackIconKind.Broom;
            ClearChatBut.ButName.Text = "Clear history";

            DeleteChatBut.IconType.Kind = PackIconKind.TrashCanOutline;
            DeleteChatBut.ButName.Text = "Delete chat";

            DeleteChatBut.IconType.Foreground = 
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
            DeleteChatBut.ButName.Foreground =
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
        }

        private void ViewProfileBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new UserInfo(_chat, _system));
        }

        private void DeleteChatBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new DeleteChat());
        }

        private void ClearChatBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new ClearChatHistory(_chat));
        }

        private void SetWallpaperBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SetChatWallpaper(_system.GetChosenChat().GetBackground()));
        }
    }
}
