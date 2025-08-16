using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
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

        [HttpGet("GetLastUserContact")]
        public UserContactcs GetContact(int userId)
        {
            return DbService.GetLastAddedContactByUser(userId);
        }

        [HttpGet("GetLastChatMessage")]
        public TelegramLib.MainClasses.Messages.Message GetLastChatMessage(int chatId)
        {
            return DbService.GetLastChatMessage(chatId); 
        }

        [HttpGet("IsContactExist")]
        public bool IsContactExist(int userId, int friendId)
        {
            return DbService.IsContactIsExist(userId, friendId);
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
        public IActionResult UpdateFolder([FromBody] FolderDTO folder)
        {
            return DbService.UpdateFolder(folder.Folder, folder.UserId) ? Ok() : NotFound();
        }

        //Delete frolder()
        [HttpDelete("DeleteFolder")]
        public void DeleteFolder([FromBody] FolderDTO folder)
        {
            DbService.RemoveFolder(folder.Folder.Id);
        }

        //Add Message
        [HttpPut("AddMessage")]
        public IActionResult AddMessage(MessageDTO message)
        {
            bool res = DbService.AddMessage(message.Chat, message.ActionMessage);
            return res ? Ok(true) : NotFound(false);
        }
        public class MessageDTO()
        {
            public UserChat Chat { get; set; }
            public Message ActionMessage { get; set; }
        }

        [HttpGet("GetUserByPhoneNumber")]
        public IActionResult GetUserByPhoneNumber(string phoneNumber)
        {
            TelegramLib.MainClasses.User res = DbService.GetUserByPhoneNumber(phoneNumber);

            if (res is null) return NotFound("Some SHIT");

            return Ok(res);
        }

        [HttpGet("GetUsersPhoneNumber")]
        public string GetUsersPhoneNumber()
        {
            return DbService.GetPhoneNumberFromUser();
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

        [HttpDelete("RemoveContact")]
        public void RemoveContact([FromBody] RemoveContactDTO contact)
        {
            DbService.RemoveContact(contact.Contact);
        }
        public class RemoveContactDTO()
        {
            public UserContactcs Contact { get; set; }
        }


        [HttpPut("AddChat")]
        public void AddChat([FromBody] AddChatDTO addChat)
        {
            DbService.AddChat(addChat.UserId, addChat.ChatterContactId);
        }
        public class AddChatDTO()
        {
            public int UserId { get; set; }
            public int ChatterContactId { get; set; }
        }

        //Add chat bg
        [HttpPost("AddChatBg")]
        public void AddChatBg([FromBody] ChatDTO chat)
        {
            DbService.SetChosenBgInPossibleBGs(chat.Chat.Id, DbService.GetChatBgIdByName(chat.Chat.ChatBg.FileName));
        }

        [HttpPost("SetAutoDeletion")]
        public void SetAutoDeletion([FromBody] AutoDelDTO autDel)
        {
            DbService.SetAutoDel(autDel.ChatId, autDel.DelType);
        }
        public class AutoDelDTO()
        {
            public int ChatId { get; set; }
            public TelegramLib.Enums.Chat.AutoDeleteType DelType { get; set; }
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

        [HttpGet("GetChatByUserAndContactId")]
        public UserChat GetChatByUserAndContactId(int userId, int contactId)
        {
           return DbService.GetChatByUserAndContactIds(userId, contactId);
        }

        [HttpGet("ContactBySenderAndReceiverIds")]
        public UserContactcs GetContactBySenderAndReceiverIds(int senderId, int receiverId)
        {
            return DbService.GetContactBySenderReceiverUserIds(senderId, receiverId);
        }

        [HttpPost("SetChatWallpaper")]
        public void SetChatWallpaper([FromBody] ChatWallpaperDTO toSetPaper)
        {
            DbService.SetChatWallpaper(toSetPaper.ToSetPaper, toSetPaper.ChatId);
        }
        public class ChatWallpaperDTO()
        {
            public ChatBackground ToSetPaper { get; set; }
            public int ChatId { get; set; }
        }

        [HttpGet("GetChatBgIdByName")]
        public int GetChatBgidByName(string name)
        {
            return DbService.GetChatBgIdByName(name);
        }
       
    }
}
