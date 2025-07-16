using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.MainClasses.Messages;
using TelegramLib.UserSettings;
using TelegramLib.UserSettings.SettingsTypes;

namespace TelegramLib.MainClasses
{
    public class TelSystem
    {
        public User LoggedUser { get; set; }
        public MainSettings Settings { get; set; }

        public List<UserChat> Chats { get; set; }

        public List<UserContactcs> Contacts { get; set; }

        public TelSystem(User user, MainSettings settings,
            List<UserChat> chats, List<UserContactcs> contacts)
        {
            LoggedUser = user;
            Settings = settings;
            Chats = chats;
            Contacts = contacts;
        }
        
        //Test system
        public TelSystem()
        {
            LoggedUser = new User();
            Settings = new MainSettings();
            Chats = new List<UserChat>();
            Contacts = new List<UserContactcs>();

            SetTestSystemParams();
        }

        public UserContactcs ChosenChatContact;
        public void SetTempChatter(string login)
        {
            ChosenChatContact = Contacts.Where(x => x.Name == login).FirstOrDefault();
        }

        public void SetTestSystemParams()
        {
            SetTestUserContacts();
        }

        public void SetTestUserContacts()
        {
            Contacts.Add(new UserContactcs(1, "FirstName", "FirstUserName",  DateTime.Now, "FirstBIO", "FirstPhoneNumber", null));
            Contacts.Add(new UserContactcs(1, "SecondName", "SecondUserName",  DateTime.Now, "SecondBIO", "SecondPhoneNumber", null));
            Contacts.Add(new UserContactcs(1, "ThirdName", "ThirdUserName",  DateTime.Now, "ThirdBIO", "ThirdPhoneNumber", null));
        }

        public List<Message> GetTestMessages()
        {
            return new List<Message>();
            List<Message> res = new List<Message>();

            res.Add(new TextMessage(1, -1, DateTime.Now, "First"));
            res.Add(new TextMessage(2, -1, DateTime.Now, "Second"));
            res.Add(new MediaAction(3, -1, DateTime.Now, "TestGif.gif"));
            res.Add(new MediaAction(4, -1, DateTime.Now, "Mine.jpg"));
            res.Add(new TextMessage(5, -1, DateTime.Now, "Three"));
            res.Add(new MediaAction(6, -1, DateTime.Now, "Cow.jpg"));
            res.Add(new MediaAction(7, -1, DateTime.Now, "TestVideo.mp4"));
            res.Add(new TextMessage(8, -1, DateTime.Now, "Four"));
            res.Add(new MediaAction(9, -1, DateTime.Now, "Hand.jpg"));
            res.Add(new MediaAction(10, -1, DateTime.Now, "TestGif.gif"));
            return res;
        }
       
    }
}
