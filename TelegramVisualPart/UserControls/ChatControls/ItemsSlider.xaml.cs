using Microsoft.AspNetCore.WebUtilities;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.Models;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.FolderControls;
using TelegramVisualPart.Windows;
using Folder = TelegramLib.MainClasses.FolderObjs.Folder;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ItemsSlider.xaml
    /// </summary>
    public partial class ItemsSlider : UserControl
    {
        public ItemsSlider()
        {
            InitializeComponent();
        }

        private List<Folder> _folders;
        private TelSystem _system;
        private MainWindow _window;

        public void SetSliderWithFolders(List<Folder> folders, TelSystem system,
            MainWindow window)
        {
            _folders = folders;
            _system = system;
            _window = window;

            SetFolders();
        }
        public void SetFolders()
        {
            TabsPanel.Children.Clear();

            AddFolderBlock("Personal", -1, isAllChats: true);
            for (int i = 0; i < _folders.Count; i++)
            {
                AddFolderBlock(_folders[i].Name, _folders[i].Id);
            }

            SetBasicChosenFolder();
        }

        private void SetBasicChosenFolder()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //Set basic folder
                Folder? folder = _folders.FirstOrDefault(x => x.Id == _system.Settings.ChosenFolderId);
                TextBlock block = folder is null ? TabsPanel.Children.OfType<TextBlock>().First() :
                    TabsPanel.Children.OfType<TextBlock>().First(x => x.Text == folder.Name);

                if (block is null) return;
                SetAnimation(block);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void AddFolderBlock(string name, int folderId, bool isAllChats = false)
        {
            TextBlock block = new TextBlock()
            {
                Foreground = Brushes.Gray,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Text = name,
                Margin = new Thickness(10, 0, 10, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.Transparent,
                Tag = folderId
            };

            block.PreviewMouseLeftButtonDown += SetAnimation_PreviewMouseDown;
            block.PreviewMouseRightButtonDown += SetFolderMenuBlock;

            block.MouseEnter += TextBlock_MouseEnter;
            block.MouseLeave += TextBlock_MouseLeave;

            if (isAllChats) block.PreviewMouseLeftButtonDown += SetAllChats_PreviewMouseDown;
            else block.PreviewMouseLeftButtonDown += SetFolderChats_PreviewMouseDown;

            TabsPanel.Children.Add(block);
        }

        public void SetFolderMenuBlock(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock block) return;

            int.TryParse(block.Tag.ToString(), out int folderId);
            FolderMenu menu = null;

            if (folderId != -1) menu = new FolderMenu(folderId, _system, _window);
            else menu = new FolderMenu(_system, _window);

            Size windowSize = ((MainWindow)Window.GetWindow(this)).GetWindowSize();
            Point point = e.GetPosition(this);

            menu.Loaded += (sender, e) =>
            {
                //is x to big
                if (point.X + menu.ActualWidth > windowSize.Width)
                {
                    Canvas.SetLeft(menu, point.X - menu.Width);
                }
                else Canvas.SetLeft(menu, point.X);

                //is y too big

                double mult = menu.GetFolderId() != -1 ? 1 : 2.5;

                if (point.Y + menu.ActualHeight * mult  > windowSize.Height)
                {
                    Canvas.SetTop(menu, windowSize.Height - menu.ActualHeight * mult);
                }
                else Canvas.SetTop(menu, point.Y + menu.ActualHeight * mult / 1.75);
            };
            _window.AddFolderMenu(menu);
        }

        public void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }
        public void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void SetAllChats_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetAllChatsInMainPage();
        }

        private void SetFolderChats_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock block) return;
            ((MainWindow)Window.GetWindow(this)).SetChosenFolderByName(block.Text);
        }
        private void SetAnimation_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock block) return;
            SetAnimation(block);
        }

        private void SetAnimation(TextBlock block)
        {
            ClearForegroundForTabs();

            block.Foreground =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            Point targetPos = block.TranslatePoint(new Point(0, 0), RectGrid);
            double targetWidth = block.ActualWidth;

            var transform = ActiveRect.RenderTransform as TranslateTransform;
            double currentX = transform?.X ?? 0;
            double currentWidth = ActiveRect.ActualWidth;

            Duration animDuration = TimeSpan.FromMilliseconds(300);

            var moveAnim = new DoubleAnimation
            {
                From = currentX,
                To = targetPos.X /*- (block.Margin.Left + block.Margin.Right)*/,
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

        public void ClearForegroundForTabs()
        {
            for (int i = 0; i < TabsPanel.Children.Count; i++)
            {
                if (TabsPanel.Children[i] is TextBlock textBlock)
                {
                    textBlock.Foreground = Brushes.Gray;
                }
            }
        }
    }
}
