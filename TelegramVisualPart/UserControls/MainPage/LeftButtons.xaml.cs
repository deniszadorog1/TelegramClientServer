using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
using TelegramLib.MainClasses.FolderObjs;
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

            SetFolders();
        }

        public void SetFolders()
        {
            for(int i = 0; i < _system.Folders.Count; i++)
            {
                LeftButtonsButton folder = new LeftButtonsButton();
                folder.SetIconKind(GetIconTypeByString(_system.Folders[i].IconName));
                folder.SetButtonText(_system.Folders[i].Name);

                ListBoxItem item = new ListBoxItem()
                {
                    Content = folder
                };

                item.PreviewMouseDown += FolderBut_PreviewMouseDown;

                FoldersBox.Items.Insert(FoldersBox.Items.IndexOf(EditFolderBoxItem), item);
            }
        }

        public PackIconKind GetIconTypeByString(string iconName)
        {
            if (Enum.TryParse<PackIconKind>(iconName, out var kind))
            {
                return kind;
            }
            return PackIconKind.Folder;
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
            foreach(var item in FoldersBox.Items)
            {
                if (item is not ListBoxItem boxItem || 
                    boxItem.Content is not LeftButtonsButton but) continue;
                but.SetBasicColors();
                boxItem.Background = new SolidColorBrush(Colors.Transparent);
            }
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
    }
}
