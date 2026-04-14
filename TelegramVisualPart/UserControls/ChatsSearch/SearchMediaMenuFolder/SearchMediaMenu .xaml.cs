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
using TelegramLib.Models;
using TelegramVisualPart.Pages.ChatActions.MessageMenuPages;

namespace TelegramVisualPart.UserControls.ChatsSearch.SearchMediaMenuFolder
{
    /// <summary>
    /// Логика взаимодействия для SearchMediaMenu.xaml
    /// </summary>
    public partial class SearchMediaMenu : UserControl
    {
        private TelegramLib.MainClasses.Messages.Message _mes;
        private TelSystem _system;
        private MainWindow _main;
        public SearchMediaMenu(
            MainWindow main,
            TelSystem system, 
            TelegramLib.MainClasses.Messages.Message mes)
        {
            _main = main;
            _mes = mes;
            _system = system;

            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            GoToMessage.SetParams(PackIconKind.EyeOutline, "Go to message");
            ForwardMessage.SetParams(PackIconKind.ArrowRight, "Forward message");
        }

        private void GoToMessage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _main.ShowChosenMessageByMessageId(_mes.Id);
        }

        private void ForwardMessage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ForwardToPage forward = new ForwardToPage(_system, _mes);
            _main.SetSecondaryFrame(forward);
        }

    }
}
