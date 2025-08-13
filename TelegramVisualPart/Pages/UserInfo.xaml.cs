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

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserInfo.xaml
    /// </summary>
    public partial class UserInfo : Page
    {
        private UserChat _chat;
        private TelSystem _system;
        public UserInfo(UserChat chat, TelSystem system)
        {
            _system = system;
            _chat = chat;
            InitializeComponent();

            ContactInfo.SetContactInfo(_chat, _system, _system.ChosenChatContact);
        }
    }
}
