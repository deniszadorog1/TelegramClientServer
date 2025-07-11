using MaterialDesignThemes.Wpf;
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
using TelegramVisualPart.Pages.Settings.Folders;

namespace TelegramVisualPart.UserControls.MainPage
{
    /// <summary>
    /// Логика взаимодействия для LeftButtons.xaml
    /// </summary>
    public partial class LeftButtons : UserControl
    {
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
    }
}
