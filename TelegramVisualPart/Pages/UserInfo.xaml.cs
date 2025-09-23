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

            ContactInfo.SetContactInfo(_chat, _system, _system.GetContactByUserId(chat.Chatter.Id)); /*_system.ChosenChatContact*/
            SetMaxValue();

            ContactInfo.UpdateAction += UpdatePage;
        }

        public void UpdatePage()
        {
            this.MaxHeight = ContactInfo.MaxHeight;
        }

        public void UpdateContact(UserContactcs contact)
        {
            ContactInfo.UpdateParams(contact);
        }

        public int GetHiddenLineIfContactNull()
        {
            return ContactInfo.GetHiddenParamsHeight();
        }

        public void SetMaxValue()
        {
            this.MaxHeight = ContactInfo.MaxHeight;
        }

        public void SetCustomMaxValue(double heightValue)
        {
            MaxHeight = heightValue;
        }

        public void UpdateBlockAction()
        {
            ContactInfo.BlockButVisibility();
        }

        public void ContactRemoveAction()
        {
            ContactInfo.ContactRemovedAction();
        }
    }
}
