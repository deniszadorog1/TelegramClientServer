using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TelegramLib.Enums.Settings.Notifs;
using TelegramLib.MainClasses;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.CustWindows;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls;
using TelegramVisualPart.UserControls.SettingsControls.NotificationPrivacy;

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

            SetBaseMonitorMessages();

            ActivateChosenParams();

            SetLanguageText.SetNotsAndSounds(this);
        }

        public void ActivateChosenParams()
        {
            SetChosenStack();
            SetChosenNumberPfMessages();
        }

        public void SetChosenNumberPfMessages()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                TextBlock? chosenBlock = TabsPanel.Children
                    .OfType<TextBlock>()
                    .FirstOrDefault(x => x.Text == _system.Settings.NotSettings.AmountOfMonMessages.ToString());

                if (chosenBlock is null) return;
                SetMesesNotifsChanged(chosenBlock);

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        public void SetChosenStack()
        {
            SetMessagesInMonitor();
        }

        public void SetBaseMonitorMessages()
        {
            //Base grid vertical positions
            SetBaseGridsAlignment();

            //Set MouseDown event
            SetEventsMonitorBordersGrids();
        }

        private ToastWindow _toast;
        public void SetEventsMonitorBordersGrids()
        {
            for (int i = 0; i < MonitorBordersGrid.Children.Count; i++)
            {
                if (MonitorBordersGrid.Children[i] is MessagesStack stack)
                {
                    stack.PreviewMouseDown += (sender, e) =>
                    {
                        if (sender is not MessagesStack stack) return;
                        SetSideParam(stack);
                        SetMessagesInMonitor();
                    };

                    stack.MouseEnter += (sender, e) =>
                    {
                        if (sender is not MessagesStack stack) return;
                        NotifMessageSide side = GetSideByStack(stack);

                        _toast = new ToastWindow(
                            _system.Settings.NotSettings.AmountOfMonMessages,
                            side);
                        _toast.Show();
                    };

                    stack.MouseLeave += (sender, e) =>
                    {
                        _toast.Close();
                        _toast = null;
                    };
                }
            }
        }

        private NotifMessageSide GetSideByStack(MessagesStack stack)
        {
            return stack == TopLeftStack ? NotifMessageSide.TopLeft :
                stack == TopRightStack ? NotifMessageSide.TopRight :
                stack == BottomLeftStack ? NotifMessageSide.BottomLeft :
                NotifMessageSide.BottomRight;
        }

        public void SetSideParam(MessagesStack stack)
        {
            _system.Settings.NotSettings.SideType =
                stack == TopLeftStack ? NotifMessageSide.TopLeft :
                stack == TopRightStack ? NotifMessageSide.TopRight :
                stack == BottomLeftStack ? NotifMessageSide.BottomLeft :
                /*stack == BottomRightStack ?*/ NotifMessageSide.BottomRight;
        }

        public void SetBaseGridsAlignment()
        {
            TopLeftStack.BaseGrid.VerticalAlignment = VerticalAlignment.Top;
            TopRightStack.BaseGrid.VerticalAlignment = VerticalAlignment.Top;

            BottomLeftStack.BaseGrid.VerticalAlignment = VerticalAlignment.Bottom;
            BottomRightStack.BaseGrid.VerticalAlignment = VerticalAlignment.Bottom;
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

        private async void ToggleEvent_MouseDown(object sender, EventArgs e)
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

            await ApiService.UpdateNotificationSettings(_system.Settings.GetNotSettings());
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
            //DeskTopNotifs.TextBlock.Text = "Desktop notifications";

            FlashBarIcon.Icon.Kind = PackIconKind.Barcode;
            //FlashBarIcon.TextBlock.Text = "Flash the taskbar icon";

            AllowSound.Icon.Kind = PackIconKind.Speakerphone;
            //AllowSound.TextBlock.Text = "Allow sound";

            PrivateChat.Icon.Kind = PackIconKind.AccountCircleOutline;
            //PrivateChat.TextBlock.Text = "Private chats";

            PinnedMessages.Icon.Kind = PackIconKind.PinOutline;
            //PinnedMessages.TextBlock.Text = "Pinned messages";
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SettingsPage(_system));
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public TextBlock GetTextBlockByBorder(Border border)
        {
            return border == OneBorder ? OneMes :
                border == TwoBorder ? TwoMes :
                border == ThreeBorder ? ThreeMes :
                border == FourBorder ? FourMes :
                FiveMes;
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border) return;
            TextBlock block = GetTextBlockByBorder(border);
            SetMesesNotifsChanged(block);
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock block) return;
            SetMesesNotifsChanged(block);

            UpdateMonitorInDB();
        }

        public async Task UpdateMonitorInDB()
        {
            await ApiService.UpdateMonitor(_system.LoggedUser.Id,
                 _system.Settings.NotSettings.SideType,
                 _system.Settings.NotSettings.AmountOfMonMessages);
        }

        public void SetMesesNotifsChanged(TextBlock block)
        {
            SetAmountOfMessages(block);

            ClearForegroundForTabs();

            block.Foreground =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            Point targetPos = block.TranslatePoint(new Point(0, 0), RectGrid);
            double targetWidth = block.ActualWidth;

            var transform = ActiveRect.RenderTransform as TranslateTransform;
            double currentX = transform?.X ?? 0;
            double currentWidth = ActiveRect.ActualWidth;

            Duration animDuration = TimeSpan.FromMilliseconds(150);

            var moveAnim = new DoubleAnimation
            {
                From = currentX,
                To = targetPos.X,
                Duration = animDuration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var widthAnim = new DoubleAnimation
            {
                From = currentWidth,
                To = targetWidth,
                Duration = animDuration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            transform?.BeginAnimation(TranslateTransform.XProperty, moveAnim);
            ActiveRect.BeginAnimation(FrameworkElement.WidthProperty, widthAnim);
        }

        private void SetAmountOfMessages(TextBlock block)
        {
            int.TryParse(block.Text, out int amount);
            _system.Settings.NotSettings.AmountOfMonMessages = amount;

            //Set in monitor
            SetMessagesInMonitor();

        }

        private void SetMessagesInMonitor()
        {
            const int height = 20;
            const int padding = 5;
            //Get chosen stack
            MessagesStack stack = GetChosenMessageStack();

            //Set amount of messages
            ClearAllStacks();
            stack.MesStack.Visibility = Visibility.Visible;
            stack.BaseGrid.Visibility = Visibility.Hidden;

            for (int i = 0; i < _system.Settings.NotSettings.AmountOfMonMessages; i++)
            {
                MonitorMessage message = new MonitorMessage()
                {
                    Height = height,
                    Margin = new Thickness(0, 0, 0, padding),
                };

                if (stack == BottomLeftStack ||
                    stack == BottomRightStack)
                {
                    message.VerticalAlignment = VerticalAlignment.Bottom;
                }

                stack.MesStack.Children.Add(message);
            }

        }

        public void ClearAllStacks()
        {
            for (int i = 0; i < MonitorBordersGrid.Children.Count; i++)
            {
                if (MonitorBordersGrid.Children[i] is MessagesStack stack)
                {
                    stack.MesStack.Children.Clear();
                    stack.MesStack.Visibility = Visibility.Hidden;
                    stack.BaseGrid.Visibility = Visibility.Visible;
                }
            }
        }

        public MessagesStack GetChosenMessageStack()
        {
            NotifMessageSide side =
                _system.Settings.NotSettings.SideType;

            return side == NotifMessageSide.TopLeft ? TopLeftStack :
                side == NotifMessageSide.TopRight ? TopRightStack :
                side == NotifMessageSide.BottomLeft ? BottomLeftStack :
                BottomRightStack;
        }


        public void ClearForegroundForTabs()
        {
            for (int i = 0; i < TabsPanel.Children.Count; i++)
            {
                if (TabsPanel.Children[i] is TextBlock textBlock)
                {
                    textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        public void UpdateSide_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _system.Settings.NotSettings.SideType =
                sender == TopLeftStack ? NotifMessageSide.TopLeft :
                sender == TopRightStack ? NotifMessageSide.TopRight :
                sender == BottomLeftStack ? NotifMessageSide.BottomLeft :
               /* sender == TopLeftStack ?*/ NotifMessageSide.TopLeft;

            UpdateMonitorInDB();
        }
    }
}
