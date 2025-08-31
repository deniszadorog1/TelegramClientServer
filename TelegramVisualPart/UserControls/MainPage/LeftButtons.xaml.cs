using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Settings.Folders;

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
                    Content = folder
                };

                item.PreviewMouseDown += FolderBut_PreviewMouseDown;

                FoldersBox.Items.Insert(FoldersBox.Items.IndexOf(EditFolderBoxItem), item);
            }
        }

        public void SetBasicParams()
        {
            AllChats.ButIcon.Kind = PackIconKind.Wechat;
            AllChats.ButText.Text = "All chats";

            Personal.ButIcon.Kind = PackIconKind.AccountCircle;
            Personal.ButText.Text = "Personal";

            Edit.ButIcon.Kind = PackIconKind.PlaylistEdit;
            Edit.ButText.Text = "Edit";
        }

        public event EventHandler? OnMenuClick;

        private void ShowMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMenuClick?.Invoke(this, EventArgs.Empty);
        }

        private void Edit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new FoldersPage(_system));
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

            ((MainWindow)Window.GetWindow(this)).SetAllChatsInMainPage();
        }

        private void FolderBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                item.Content is not LeftButtonsButton but) return;

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
