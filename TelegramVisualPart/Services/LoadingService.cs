using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.MainClasses;

namespace TelegramVisualPart.Services
{
    public static class LoadingService
    {
        public static async Task LoadFullDataInBackground(TelSystem system)
        {
            while (true)
            {
                var batch = await ApiService.GetPartlyContacts(system.LoggedUser.Id, system.Contacts.Last().Id);
                if (batch.Count == 0) break;
                system.Contacts.AddRange(batch);

                var secBatch = await ApiService.GetPartlyChats(system.LoggedUser.Id, system.Chats.Last().Id);
                if (secBatch.Count == 0) break;
                system.Chats.AddRange(secBatch);
            }
        }
    }
}
