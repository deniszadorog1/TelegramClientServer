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
        public void SetSliderWithFolders(List<Folder> folders, TelSystem system)
        {
            _folders = folders;
            _system = system;

            SetFolders();
        }
        private void SetFolders()
        {
            TabsPanel.Children.Clear();

            AddFolderBlock("Personal", isAllChats: true);
            for (int i = 0; i < _folders.Count; i++)
            {
                AddFolderBlock(_folders[i].Name);
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

        public void AddFolderBlock(string name, bool isAllChats = false)
        {
            TextBlock block = new TextBlock()
            {
                Foreground = Brushes.Gray,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Text = name,
                Margin = new Thickness(5, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            block.PreviewMouseDown += SetAnimation_PreviewMouseDown;

            if (isAllChats) block.PreviewMouseDown += SetAllChats_PreviewMouseDown;
            else block.PreviewMouseDown += SetFolderChats_PreviewMouseDown;

            TabsPanel.Children.Add(block);
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
