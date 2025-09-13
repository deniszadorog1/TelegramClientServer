using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.MainClasses.FolderObjs
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
        public List<UserContactcs> Contacts { get; set; }
        public List<UserContactcs> ExcludedContacts { get; set; }

        public Folder(int id, string name, string iconName,
            List<UserContactcs> contacts,
            List<UserContactcs> excludedContacts)
        {
            Id = id;
            Name = name;
            IconName = iconName;
            Contacts = contacts;
            ExcludedContacts = excludedContacts;
        }

        public Folder()
        {
            Id = -1;
            Name = string.Empty;
            IconName = string.Empty;
            Contacts = new List<UserContactcs>();
            ExcludedContacts = new List<UserContactcs>();
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            Name = name;
        }

        public void SetIconName(string name)
        {
            IconName = name;
        }

        public void AddContact(UserContactcs contact)
        {
            if (!(Contacts.FirstOrDefault(x => x.Id == contact.Id) is null)) return; 
            Contacts.Add(contact);
        }

        public void AddExcludedContacts(UserContactcs contact)
        {
            ExcludedContacts.Add(contact);
        }

/*        public void RemoveContactByName(string name)
        {
            Contacts.Remove(Contacts.Where(x => x.IsNamesAreEqual(name)).FirstOrDefault());
        }*/

        public void RemoveContactById(int contactId)
        {
            UserContactcs toRemove =  Contacts.FirstOrDefault(x => x.Id == contactId);
            if (toRemove is null) return;

            Contacts.Remove(toRemove);
        }

        public void SetContacts(List<UserContactcs> contacts)
        {
            Contacts = contacts;
        }

        public void SetExcludeContacts(List<UserContactcs> contacts)
        {
            ExcludedContacts = contacts;
        }
    }
}
