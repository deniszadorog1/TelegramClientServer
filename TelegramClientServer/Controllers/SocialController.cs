using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialController : ControllerBase
    {
        //Add contact
        [HttpPut("AddContact")]
        public void AddContact([FromBody] ContactDTO contact)
        {
            DbService.AddContact(contact.Contact, contact.UserId);
        }
        public class ContactDTO()
        {
            public UserContactcs Contact { get; set; }
            public int UserId { get; set; }
        }

        //update contact
        [HttpPost("UpdateContact")]
        public void UpdateContact([FromBody] ContactDTO contact)
        {
            DbService.UpdateContact(contact.Contact, contact.UserId);
        }

        //blcok contact (ITS UPDATE!!!)
        //unblock contact (ITS UPDATE!!!)

        //Add folder
        [HttpPut("AddFolder")]
        public void AddFolder([FromBody] FolderDTO folder)
        {
            DbService.AddFolder(folder.Folder, folder.UserId);
        }
        public class FolderDTO()
        {
            public TelegramLib.MainClasses.FolderObjs.Folder Folder { get; set; }
            public int UserId { get; set; }
        }

        //Update folder
        [HttpPost("UpdateFolder")]
        public void UpdateFolder([FromBody] FolderDTO folder)
        {
            DbService.UpdateFolder(folder.Folder, folder.UserId);
        }

        //Delete frolder()
        [HttpDelete("DeleteFolder")]
        public void DeleteFolder([FromBody] FolderDTO folder)
        {
            DbService.RemoveFolder(folder.Folder.Id);
        }

        //Add Message
        [HttpPut("AddMessage")]
        public void AddMessage(MessageDTO message)
        {
            DbService.AddMessage(message.Chat, message.ActionMessage);
        }
        public class MessageDTO()
        {
            public UserChat Chat { get; set; }
            public TelegramLib.MainClasses.Messages.Message ActionMessage { get; set; }
        }

        //Update chat
        [HttpPost("UpdateChat")]
        public void UpdateChat([FromBody] ChatDTO chat)
        {
            DbService.UpdateChat(chat.Chat);
        }
        public class ChatDTO()
        {
            public UserChat Chat { get; set; }
        }

        //Clear chat
        [HttpDelete("ClearChat")]
        public void ClearChat([FromBody] ChatDTO chat)
        {
            DbService.ClearChat(chat.Chat.Id);
        }

        //Add chat bg
        [HttpPost("AddChatBg")]
        public void AddChatBg([FromBody] ChatDTO chat)
        {
            DbService.SetChosenBgInPosssibleBGs(chat.Chat.Id, DbService.GetChatBgIdByName(chat.Chat.ChatBg.FileName));
        }

        //user BG 

        //Add Chat img
        [HttpPut("AddChatImage")]
        public void AddChatImage([FromBody] PathDTO path)
        {
            DbService.AddChatImage(path.Path);
        }
        public class PathDTO() 
        {
            public string Path { get; set; }
        }

        //Add chat video
        [HttpPut("AddVideo")]
        public void AddVideo([FromBody] PathDTO path)
        {
            DbService.AddChatVideo(path.Path);
        }

        //Add blocked user
        [HttpPut("AddBlockedContact")]
        public void AddBlockedContact([FromBody] BlockedContactDTO contact)
        {
            DbService.AddBlockedContact(contact.UserId, contact.ContactId);
        }
        public class BlockedContactDTO() 
        {
            public int UserId { get; set; }
            public int ContactId { get; set; }
        }

        //Remove from blocked user
        [HttpDelete("DeleteBlockedContact")]
        public void DeleteBlockedContact([FromBody] BlockedContactDTO contact)
        {
            DbService.UnBlockContact(contact.UserId, contact.ContactId);
        }

    }
}
