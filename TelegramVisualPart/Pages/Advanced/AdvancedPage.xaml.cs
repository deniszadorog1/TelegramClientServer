using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls.AdvancedControls.AdvancedButtons;

namespace TelegramVisualPart.Pages.Advanced
{
    /// <summary>
    /// Логика взаимодействия для AdvancedPage.xaml
    /// </summary>
    public partial class AdvancedPage : Page
    {
        private TelSystem _system;
        public AdvancedPage(TelSystem system)
        {
            _system = system;

            InitializeComponent();

            SetBaseBlocks();

            SetBasicParams();

            SetLanguageText.SetAdvancedPage(this);
        }

        public void SetBasicParams()
        {
            AdvancedSettings settings = _system.Settings.GetAdvSettings();

            ShowChatNameBox.IsChecked = settings.IsShowChatName;
            UnreadCountBox.IsChecked = settings.IsShowTotalUnReads;
            WindowFrame.IsChecked = settings.IsUserWindowSysFrame;

            TrayIconBox.IsChecked = settings.IsShowTrayIcon;
            TaskBarBox.IsChecked = settings.IsShowTaskbarIcon;
            CloseToTaskBarBox.IsChecked = settings.IsCloseToTaskbar;
            AtStartLaunchTelegramBox.IsChecked = settings.LaunchTelegram;

            IsAskDownloadPath.Toggle.IsChecked = settings.IsAskDownloadPath;
            
            VersionBut.Toggle.IsChecked = settings.IsUpdateAutomatically;
            InstalBetaBut.Toggle.IsChecked = settings.IsInstallBetaVersion;
        }

        public void SetBaseBlocks()
        {
            GetBackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            DownloadPathBut.IconType.Kind = PackIconKind.FileOutline;

            Downloads.IconType.Kind = PackIconKind.DownloadOutline;

            PrivateChatsBut.IconType.Kind = PackIconKind.AccountCircleOutline;
        }

        private void GetBackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInDb();
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new Settings.SettingsPage(_system));
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInDb();
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;
            SetValueToParam(box, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;
            SetValueToParam(box, false);
        }

        public void SetValueToParam(CheckBox box, bool value)
        {
            if (box == ShowChatNameBox) _system.Settings.GetAdvSettings().IsShowChatName = value;
            else if (box == UnreadCountBox) _system.Settings.GetAdvSettings().IsShowTotalUnReads = value;
            else if (box == WindowFrame) _system.Settings.GetAdvSettings().IsUserWindowSysFrame = value;
            else if (box == TrayIconBox) _system.Settings.GetAdvSettings().IsShowTrayIcon = value;
            else if (box == TaskBarBox) _system.Settings.GetAdvSettings().IsShowTaskbarIcon = value;
            else if (box == CloseToTaskBarBox) _system.Settings.GetAdvSettings().IsCloseToTaskbar = value; 
            else if (box == AtStartLaunchTelegramBox) _system.Settings.GetAdvSettings().LaunchTelegram = value; 
        }

        private void IsAskDownloadPath_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _system.Settings.GetAdvSettings().IsAskDownloadPath = (bool)IsAskDownloadPath.Toggle.IsChecked;
        }

        private void InstalBetaBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _system.Settings.GetAdvSettings().IsInstallBetaVersion = (bool)InstalBetaBut.Toggle.IsChecked;
        }

        private void VersionBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _system.Settings.GetAdvSettings().IsUpdateAutomatically = (bool)VersionBut.Toggle.IsChecked;
        }

        private void PrivateChatsBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInDb();
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.Other.InfoMessage(("Its already been saved.")));
        }

        private void UpdateInDb()
        {
            ApiService.UpdateAdvanced(_system.Settings.AdvSettings);
        }

        private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void CheckBox_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void TaskBarBox_Checked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetVisibilityInTaskBar(true);
        }

        private void TaskBarBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetVisibilityInTaskBar(false);
        }

        private void ShowChatNameBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
