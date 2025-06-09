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

namespace TelegramVisualPart.Pages.Settings.NotifsAndSounds
{
    /// <summary>
    /// Логика взаимодействия для NotAndSoundSettings.xaml
    /// </summary>
    public partial class NotAndSoundSettings : Page
    {
        public NotAndSoundSettings()
        {
            InitializeComponent();

            SetButsVisibility();
        }

        public void SetButsVisibility()
        {
            DeskTopNotifs.Icon.Kind = PackIconKind.BellOutline;
            DeskTopNotifs.TextBlock.Text = "Desktop notifications";

            FlashBarIcon.Icon.Kind = PackIconKind.Barcode;
            FlashBarIcon.TextBlock.Text = "Flash the taskbar icon";

            AllowSound.Icon.Kind = PackIconKind.Speakerphone;
            AllowSound.TextBlock.Text = "Allow sound";

            PrivateChat.Icon.Kind = PackIconKind.AccountCircleOutline;
            PrivateChat.TextBlock.Text = "Private chats";

            PinnedMessages.Icon.Kind = PackIconKind.PinOutline;
            PinnedMessages.TextBlock.Text = "Pinned messages";
        }

        private void Buts_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;

            if (sender is PackIcon icon)
            {
                icon.Foreground = Brushes.White;
            }
        }

        private void Buts_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if(sender is PackIcon icon)
            {
                icon.Foreground = Brushes.Gray;
            }
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SettingsPage());
        }
    }
}
