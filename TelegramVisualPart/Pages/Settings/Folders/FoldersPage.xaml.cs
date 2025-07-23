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
        }

        public void SetCreatedFolderItems()
        {
            foreach(Folder folder in _system.Folders)
            {
                CreateFolderControl(folder);
            }
        }

        public void CreateFolderControl(Folder folder)
        {
            FolderLittleInfo info = new FolderLittleInfo()
            {
                Padding = new Thickness(20, 5, 22, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = this.Width
            };

            info.SetFolderName(folder.Name);
            info.SetAmountOfItems(folder.Contacts.Count);

            ListBoxItem item = new ListBoxItem()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(0),
                Content = info,
            };

            Folders.Items.Insert(Folders.Items.IndexOf(AddFolderBoxItem), item);
        }

        public void SetBasicParams()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;
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
    }
}
