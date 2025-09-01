using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls.FoldersPrivacy;

namespace TelegramVisualPart.Pages.Settings.Folders
{
    /// <summary>
    /// Логика взаимодействия для FoldersPage.xaml
    /// </summary>
    public partial class FoldersPage : Page
    {
        private TelSystem _system;
     
        public FoldersPage(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetBasicParams();
            SetCreatedFolderItems();

            SetTestObject();
        }

        private void SetTestObject()
        {
            TestThing.FolderName.Text = "Folder Name";
            TestThing.AmountOfChats.Text = "Amount of Users";
            TestThing.BucketIcon.Visibility = Visibility.Hidden;
        }

        public void SetCreatedFolderItems()
        {
            foreach (Folder folder in _system.Folders)
            {
                CreateFolderControl(folder);
            }
        }

        public void CreateFolderControl(Folder folder)
        {
            FolderLittleInfo info = new FolderLittleInfo()
            {
                Padding = new Thickness(0, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = this.Width
            };

            info.SetFolderName(folder.Name);
            info.SetAmountOfItems(folder.Contacts.Count);
            info.IconType.Kind = Helper.FilesAction.GetIconTypeByString(folder.IconName);

            info.BucketClicked += FolderBucket_Clicked;

            ListBoxItem item = new ListBoxItem()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(0),
                Content = info,
            };

            item.PreviewMouseDown += EnterToFolderSettings_PreviewMouseDown;

            Folders.Items.Insert(Folders.Items.IndexOf(AddFolderBoxItem), item);
        }

        private async void FolderBucket_Clicked(object sender, EventArgs e)
        {
            if (sender is not FolderLittleInfo folderInfo) return;

            Folder folder = _system.GetFolderByName(folderInfo.FolderName.Text);

            await ApiService.RemoveFolder(folder, _system.LoggedUser.Id);

            ListBoxItem item = ItemsControl.ContainerFromElement(Folders, folderInfo) as ListBoxItem;
            Folders.Items.Remove(item);

            _system.RemoveFolder(folder);

            ((MainWindow)Window.GetWindow(this)).UpdateFolders();
        }

        private void EnterToFolderSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                item.Content is not FolderLittleInfo info) return;

            if (IsBucketIsClicked(e, info)) return;

            Folder folder = _system.GetFolderByName(info.FolderName.Text);
            if (folder is null) return;

            FolderAction folderSettingsPage = new FolderAction(_system, folder);

            if (info.GetIsRemove()) return;
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(folderSettingsPage);
        }

        private bool IsBucketIsClicked(MouseButtonEventArgs e, FolderLittleInfo info)
        {
            var clickedElement = e.OriginalSource as DependencyObject;
            var bucket = FilesAction.FindParentByName(clickedElement, info.BucketGrid.Name);
            return bucket is not null;
        }

        public void SetBasicParams()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            if (_system.Settings.IsTabsOnTheLeft) LeftTabs.IsChecked = true;
            else ShitTabs.IsChecked = true;
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SettingsPage(_system));
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateNewFolderBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderAction action = new FolderAction(_system);

            action.FolderCreated += Folder_Created;

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(action);
        }

        public void Folder_Created(object sender, EventArgs e)
        {
            if (sender is not FolderAction action) return;

            //Add folder control
            CreateFolderControl(_system.GetFolderByName(action.GetFolderName()));
        }

        private void ShitTabs_Checked(object sender, RoutedEventArgs e)
        {
            if (!_system.Settings.IsTabsOnTheLeft) return;
                _system.Settings.IsTabsOnTheLeft = false;
            ((MainWindow)Window.GetWindow(this)).UpdateTabsStandings();
        }

        private void LeftTabs_Checked(object sender, RoutedEventArgs e)
        {
            if (_system.Settings.IsTabsOnTheLeft) return;
            _system.Settings.IsTabsOnTheLeft = true;
            ((MainWindow)Window.GetWindow(this)).UpdateTabsStandings();
        }
    }
}
