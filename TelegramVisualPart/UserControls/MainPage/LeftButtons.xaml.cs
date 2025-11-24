using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Http.Metadata;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.UserControls.ChatControls.ChatButsControls;
using TelegramVisualPart.UserControls.FolderControls;

namespace TelegramVisualPart.UserControls.MainPage
{
    /// <summary>
    /// Логика взаимодействия для LeftButtons.xaml
    /// </summary>
    public partial class LeftButtons : UserControl
    {
        private readonly SolidColorBrush _activeBgColor =
    (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];

        private TelSystem _system;
        public LeftButtons()
        {
            InitializeComponent();

            HamburgMenu.IconType.Kind = PackIconKind.HamburgerMenu;

            SetBasicParams();
        }

        public void SetSystemParam(TelSystem system)
        {
            _system = system;

            UpdateFolders();
        }

        public void SetFolders()
        {
            for (int i = 0; i < _system.Folders.Count; i++)
            {
                LeftButtonsButton folder = new LeftButtonsButton();
                folder.SetIconKind(FilesAction.GetIconTypeByString(_system.Folders[i].IconName));
                folder.SetButtonText(_system.Folders[i].Name);

                ListBoxItem item = new ListBoxItem()
                {
                    Content = folder,
                    Tag = _system.Folders[i].Id
                };

                item.PreviewMouseLeftButtonDown += FolderBut_PreviewMouseDown;
                item.PreviewMouseRightButtonDown += SetFolderMenu_PreviewMouseRightButtonDown;

                FoldersBox.Items.Insert(FoldersBox.Items.IndexOf(EditFolderBoxItem), item);
            }
        }

        public void SetFolderMenu_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item) return;
            int.TryParse(item.Tag.ToString(), out int folderId);
            System.Windows.Point point = e.GetPosition(this);

            FolderMenu menu = new FolderMenu(folderId, _system, (MainWindow)Window.GetWindow(this));

            Size windowSize =  ((MainWindow)Window.GetWindow(this)).GetWindowSize();

            menu.Loaded += (sender, e) =>
            {
                //is x to big
                if (point.X + menu.ActualWidth > windowSize.Width)
                {
                    Canvas.SetLeft(menu, point.X - menu.Width);
                }
                else Canvas.SetLeft(menu, point.X);

                //is y too big
                if (point.Y + menu.ActualHeight > windowSize.Height)
                {
                    Canvas.SetTop(menu, windowSize.Height - menu.ActualHeight);
                }
                else Canvas.SetTop(menu, point.Y);
            };

            ((MainWindow)Window.GetWindow(this)).AddFolderMenu(menu);

            //Add menu on main
        }

        public void SetBasicParams()
        {
            AllChats.ButIcon.Kind = PackIconKind.Wechat;
            //AllChats.ButText.Text = "All chats";

            Personal.ButIcon.Kind = PackIconKind.AccountCircle;
            //Personal.ButText.Text = "Personal";

            Edit.ButIcon.Kind = PackIconKind.PlaylistEdit;
            //Edit.ButText.Text = "Edit";
        }

        

        public event EventHandler? OnMenuClick;

        private void ShowMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMenuClick?.Invoke(this, EventArgs.Empty);
        }

        private void Edit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new FoldersPage(_system, false));
        }

        public void ClearButtonsEffects()
        {
            foreach (var item in FoldersBox.Items)
            {
                if (item is not ListBoxItem boxItem ||
                    boxItem.Content is not LeftButtonsButton but) continue;
                but.SetBasicColors();
                boxItem.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void AllChatsItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                item.Content is not LeftButtonsButton but) return;

            ClearButtonsEffects();

            but.SetActiveColor();
            item.Background = _activeBgColor;

            _system.Settings.ChosenFolderId = -1;
            ((MainWindow)Window.GetWindow(this)).SetAllChatsInMainPage();
        }

        private void FolderBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                item.Content is not LeftButtonsButton but) return;

            int.TryParse(item.Tag.ToString(), out int check);
            _system.Settings.ChosenFolderId = check;

            ClearButtonsEffects();

            but.SetActiveColor();
            item.Background = _activeBgColor;

            ((MainWindow)Window.GetWindow(this)).SetChosenFolderByName(but.ButText.Text);
        }

        public void UpdateFolders()
        {
            RemoveFolders();
            SetFolders();
        }

        public void RemoveFolders()
        {
            List<ListBoxItem> items = new List<ListBoxItem>();
            foreach (ListBoxItem item in FoldersBox.Items)
            {
                //remove test folders later
                if (item.Name == AllChatsItem.Name ||
                    item.Name == PersonalItem.Name ||
                    item.Name == EditFolderBoxItem.Name) continue;

                items.Add(item);
            }

            foreach (ListBoxItem item in items)
            {
                FoldersBox.Items.Remove(item);
            }
        }
    }
}
