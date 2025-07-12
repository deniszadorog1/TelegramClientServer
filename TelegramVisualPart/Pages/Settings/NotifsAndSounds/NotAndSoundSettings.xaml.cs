using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.MainClasses;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls;
using TelegramVisualPart.UserControls.DifferButs;

namespace TelegramVisualPart.Pages.Settings.NotifsAndSounds
{
    /// <summary>
    /// Логика взаимодействия для NotAndSoundSettings.xaml
    /// </summary>
    public partial class NotAndSoundSettings : Page
    {
        private TelSystem _system;
        public NotAndSoundSettings(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetButsVisibility();

            SetBasicParams();

            SetToggleEvents();
        }

        public void SetToggleEvents()
        {
            DeskTopNotifs.Toggle.Checked += ToggleEvent_MouseDown;
            DeskTopNotifs.Toggle.Unchecked += ToggleEvent_MouseDown;

            FlashBarIcon.Toggle.Checked += ToggleEvent_MouseDown;
            FlashBarIcon.Toggle.Unchecked += ToggleEvent_MouseDown;

            AllowSound.Toggle.Checked += ToggleEvent_MouseDown;
            AllowSound.Toggle.Unchecked += ToggleEvent_MouseDown;

            PrivateChat.Toggle.Checked += ToggleEvent_MouseDown;
            PrivateChat.Toggle.Unchecked += ToggleEvent_MouseDown;

            PinnedMessages.Toggle.Checked += ToggleEvent_MouseDown;
            PinnedMessages.Toggle.Unchecked += ToggleEvent_MouseDown;
        }

        private void ToggleEvent_MouseDown(object sender, EventArgs e)
        {
            if (sender is not ToggleButton toggle ||
                toggle is null) return;

            ToggleIconBut but = HelperService.FindParent<ToggleIconBut>(toggle);

            if (but.Name == DeskTopNotifs.Name)
            {
                _system.Settings.GetNotSettings().IsDesktopNotifications =
                    (bool)DeskTopNotifs.Toggle.IsChecked;
            }
            else if (but.Name == FlashBarIcon.Name)
            {
                _system.Settings.GetNotSettings().IsFlashTaskBar =
                    (bool)FlashBarIcon.Toggle.IsChecked;
            }
            else if (but.Name == AllowSound.Name)
            {
                _system.Settings.GetNotSettings().IsAllowSounds =
                    (bool)AllowSound.Toggle.IsChecked;
            }
            else if (but.Name == PrivateChat.Name)
            {
                _system.Settings.GetNotSettings().IsPrivateChats =
                    (bool)PrivateChat.Toggle.IsChecked;
            }
            else if (but.Name == PinnedMessages.Name)
            {
                _system.Settings.GetNotSettings().IsPinnedMessages =
                    (bool)PinnedMessages.Toggle.IsChecked;
            }
        }
        public void SetBasicParams()
        {
            NotificationSettings notSettings = _system.Settings.GetNotSettings();
            SetToggleIconButton(DeskTopNotifs, notSettings.IsDesktopNotifications);
            SetToggleIconButton(FlashBarIcon, notSettings.IsFlashTaskBar);
            SetToggleIconButton(AllowSound, notSettings.IsAllowSounds);

            SetToggleIconButton(PrivateChat, notSettings.IsPrivateChats);
            SetToggleIconButton(PinnedMessages, notSettings.IsPinnedMessages);
        }

        private void SetToggleIconButton(ToggleIconBut but, bool isOn)
        {
            but.Toggle.IsChecked = isOn;
        }

        public void SetButsVisibility()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

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

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SettingsPage(_system));
        }
    }
}
