using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using TelegramVisualPart.Enums;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.Services;
using static System.Collections.Specialized.BitVector32;

namespace TelegramVisualPart.UserControls.FolderControls
{
    /// <summary>
    /// Логика взаимодействия для FolderMenu.xaml
    /// </summary>
    public partial class FolderMenu : UserControl
    {
        private int _folderId = -1;
        private TelSystem _system;
        private MainWindow _mainWindow;

        public FolderMenu(int folderId, TelSystem system, MainWindow mainWindow)
        {
            _folderId = folderId;
            _system = system;
            _mainWindow = mainWindow;

            InitializeComponent();
            SetBasicParams();
        }
        //Set Edit all folders
        public FolderMenu(TelSystem system, MainWindow window)
        {
            _mainWindow = window;
            _system = system;

            InitializeComponent();

            SetBasicParams();
            SetEmptyFolderParams();
        }

        private const int _baseButHeight = 40;
        private void SetEmptyFolderParams()
        {
            EditFolderBut.Height = 0;
            RemoveBut.Height = 0;

            EditAllFolders.Height = _baseButHeight;
        }

        public void SetBasicParams()
        {
            EditFolderBut.SetBasicParams("Edit folder", PackIconKind.PencilOutline); ;
            EditAllFolders.SetBasicParams("Edit All folders", PackIconKind.PencilOutline); ;
            
            RemoveBut.SetBasicParams("Remove", PackIconKind.GarbageCanOutline); ;
            RemoveBut.SetColor(new SolidColorBrush(Colors.Red));
        }

        private async void But_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender == EditFolderBut)
            {
                FolderAction action = new FolderAction
                    (_system, _system.GetFolderById(_folderId));

                _mainWindow.SetSecondaryFrame(action);

                action.UpdateFolder += () =>
                {
                    _mainWindow.UpdateFolders();
                };
            }
            else if(sender == RemoveBut)
            {
                TelegramLib.MainClasses.FolderObjs.Folder folder = 
                    _system.GetFolderById(_folderId);

                await ApiService.RemoveFolder(folder, _system.LoggedUser.Id);
                _system.RemoveFolder(folder);

                _mainWindow.UpdateFolders();
            }
            else if(sender == EditAllFolders)
            {
                FoldersPage page = new FoldersPage(_system, false);

                _mainWindow.SetSecondaryFrame(page);
            }
        }

        public int GetFolderId()
        {
            return _folderId;
        }
    }
}
