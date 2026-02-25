using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public class ContactDTO
        {
            public UserContactcs Contact { get; set; }
            public int UserId { get; set; }
        }

        [HttpPut("AddShareContactMessage")]
        public void AddShareContactMessage(
            [FromBody] AddShareContactMessageDTO shareMessage)
        {
            DbService.AddShareMessage(shareMessage.SharedUserId,
                shareMessage.SharedName, shareMessage.ChatId,
                shareMessage.SenderId, shareMessage.Message);
        }
        public class AddShareContactMessageDTO
        {
            public int SharedUserId { get; set; }
            public string SharedName { get; set; }
            public int ChatId { get; set; }
            public int SenderId { get; set; }
            public string Message { get; set; }
        }

        //Update date for schedMessage
        [HttpPost("UpdateSchedMessageDate")]
        public void UpdateSchedMessageDate([FromBody] UpdateSchedMessageDateDTO dto)
        {
            DbService.UpdateDateInSchedMessageById(dto.MessageId, dto.NewDate);
        }

        public class UpdateSchedMessageDateDTO
        {
            public int MessageId { get; set; }
            public DateTime NewDate { get; set; }
        }

        //update contact
        [HttpPost("UpdateContact")]
        public void UpdateContact([FromBody] ContactDTO contact)
        {
            DbService.UpdateContact(contact.Contact, contact.UserId);
        }

        [HttpPost("SetContactMask")]
        public void SetContactMask([FromBody] SetContactMaskDTO dto)
        {
            DbService.SetMaskImage(dto.Contact, dto.LoggedUserId);
        }
        public class SetContactMaskDTO
        {
            public UserContactcs Contact { get; set; }
            public int LoggedUserId { get; set; }
        }

        [HttpPost("EditSchedMessage")]
        public void EditSchedMessage([FromBody] EditSchedMessageDTO editDTO)
        {
            DbService.EditSchedMessage(
                editDTO.MesId, editDTO.TextMes, editDTO.MediaMes);
        }

        public class EditSchedMessageDTO
        {
            public int MesId { get; set; }
            public TextMessage? TextMes { get; set; }
            public MediaAction? MediaMes { get; set; }
        }

        [HttpPost("EditSavedChatMessage")]
        public void EditSavedChatMessage([FromBody] EditMessageDTO editDTO)
        {
            DbService.EditSavedMessage(editDTO.TextMes, editDTO.MediaMes);
        }


        [HttpPost("EditMessage")]
        public void EditMessage([FromBody] EditMessageDTO editDTO)
        {
            DbService.EditMessage(editDTO.ChatId, editDTO.TextMes, editDTO.MediaMes);
        }

        public class EditMessageDTO
        {
            public int ChatId { get; set; }
            public TelegramLib.MainClasses.Messages.TextMessage? TextMes { get; set; }
            public TelegramLib.MainClasses.Messages.MediaAction? MediaMes { get; set; }
        }

        [HttpGet("GetContactMask")]
        public TelegramLib.MainClasses.UserParams.UserImage GetContactMask(int loggedUserId,
            int contactUserId)
        {
            return DbService.GetContactMaskByContactUserId(loggedUserId, contactUserId);
        }

        [HttpGet("GetLastSavedMessage")]
        public TelegramLib.MainClasses.Messages.Message GetLastSavedMessage(int chatId)
        {
            return DbService.GetLastSavedMessage(chatId);
        }

        [HttpGet("GetLastMessageBandId")]
        public int GetLastMessageBandId()
        {
            return DbService.GetLastMessageBandId();
        }


        [HttpGet("GetMessagesByChatId")]
        public List<TelegramLib.MainClasses.Messages.Message> GetMessagesByChatId(int chatId)
        {
            return DbService.GetMessagesByChatId(chatId);
        }

        [HttpGet("GetLastUserContact")]
        public UserContactcs GetContact(int userId)
        {
            return DbService.GetLastAddedContactByUser(userId);
        }

        [HttpGet("GetLastSharedMessageIdByChatId")]
        public int GetLastSharedMessageIdByChatId(int chatId)
        {
            return DbService.GetLastSharedMessageId(chatId);
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

        [HttpGet("IsChatterIdIsContact")]
        public bool IsChatterIdIsContact(int userId, int friendUserId)
        {
            return DbService.IsChatterIdIsContact(userId, friendUserId);
        }

        //blcok contact (ITS UPDATE!!!)
        //unblock contact (ITS UPDATE!!!)

        //Add folder
        [HttpPut("AddFolder")]
        public void AddFolder([FromBody] FolderDTO folder)
        {
            DbService.AddFolder(folder.Folder, folder.UserId);
        }
        public class FolderDTO
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


        [HttpPost("UpdateUserLogin")]
        public void UpdateUserLogin([FromBody] UpdateUserLoginDTO dto)
        {
            DbService.UpdateUserLogin(dto.UserId, dto.NewLogin);
        }
        public record UpdateUserLoginDTO(int UserId, string NewLogin);

        [HttpDelete("ClearSaveChatById")]
        public void ClearSaveChatById([FromBody] ClearSaveMessagesChatDTO dto)
        {
            DbService.ClearSaveChatById(dto.Id);
        }
        public class ClearSaveMessagesChatDTO
        {
            public int Id { get; set; }
        }

        //Delete frolder()
        [HttpDelete("DeleteFolder")]
        public void DeleteFolder([FromBody] FolderDTO folder)
        {
            DbService.RemoveFolder(folder.Folder.Id);
        }

        [HttpDelete("DeleteMessageById")]
        public void DeleteMessageById([FromBody] DeleteMessageDTO dto)
        {
            DbService.RemoveMessageById(dto.Id);
        }
        public class DeleteMessageDTO
        {
            public int Id { get; set; }
        }

        [HttpDelete("DeleteManyMessages")]
        public void DeleteManyMessages([FromBody] DeleteManyMessagesDTO dto)
        {
            DbService.RemoveManyMessages(dto.IdsToDelete, dto.IsBoth);
        }
        public class DeleteManyMessagesDTO
        {
            public List<int> IdsToDelete { get; set; }
            public bool IsBoth { get; set; }
        }


        [HttpDelete("DeleteChatById")]
        public void DeleteChatById([FromBody] DeleteChatByChatterIdDTO chatterIdDTO)
        {
            DbService.DeleteChatById(chatterIdDTO.ChatId);
        }
        public class DeleteChatByChatterIdDTO()
        {
            public int ChatId { get; set; }
        }

        [HttpDelete("DeleteContactFromFolder")]
        public void DeleteContactFromFolder([FromBody] RemoveContactFromFolderDTO dto)
        {
            DbService.DeleteUserFromFolder(dto.FolderId, dto.UserId);
        }
        public class RemoveContactFromFolderDTO
        {
            public int FolderId { get; set; }
            public int UserId { get; set; }
        }

        [HttpPut("AddAndGetSchedMessage")]
        public TelegramLib.MainClasses.Messages.Message AddAndGetSchedMessage([FromBody] MessageDTO dto)
        {
            return DbService.AddAndGetSchedMessage(dto.Chat, dto.ActionMessage);
        }

        //Add Message
        [HttpPut("AddMessage")]
        public IActionResult AddMessage([FromBody] MessageDTO dto)
        {
            bool res = DbService.AddMessage(dto.Chat, dto.ActionMessage);
            return res ? Ok(true) : NotFound(false);
        }
        public class MessageDTO
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

        [HttpGet("GetPairToMessage")]
        public IActionResult GetPairToMessage(int mesId)
        {
            var message = DbService.GetPairOfMessageBySentTime(mesId);
            if (message == null) return NoContent();

            var json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            return Content(json, "application/json");
            //return DbService.GetPairOfMessageBySentTime(mesId);
        }


        [HttpGet("GetMessageById")]
        public TelegramLib.MainClasses.Messages.Message GetMessageById(int id)
        {
            return DbService.GetMessageById(id);
        }

        [HttpGet("IsLoginExist")]
        public bool IsLoginExist(string login)
        {
            return DbService.IsLoginExist(login);
        }

        [HttpGet("GetLastFolderIdByUserId")]
        public int GetLastFolderIdByUserId(int userId)
        {
            return DbService.GetLastFolderIdByOwnerId(userId);
        }

        [HttpGet("GetUsersPhoneNumber")]
        public string GetUsersPhoneNumber()
        {
            return DbService.GetPhoneNumberFromUser();
        }

        [HttpPost("UpdateUserNameSurname")]
        public void UpdateUserNameSurname([FromBody] UpdateUserNameSurnameDTO dto)
        {
            DbService.UpdateUserNameSurname(dto.UserId, dto.Name, dto.Surname);
        }
        public record UpdateUserNameSurnameDTO(int UserId, string Name, string Surname);

        //Update chat
        [HttpPost("UpdateChat")]
        public void UpdateChat([FromBody] ChatDTO chat)
        {
            DbService.UpdateChat(chat.Chat);
        }

        public class ChatDTO
        {
            public UserChat Chat { get; set; }
        }

        [HttpPost("SetPinStatus")]
        public void SetPinStatus([FromBody] SetPinStatusDTO pinDto)
        {
            DbService.SetPinStatus(pinDto.MesId, pinDto.PinStatus, pinDto.IsSaveMessageChat);
        }
        public class SetPinStatusDTO
        {
            public int MesId { get; set; }
            public bool PinStatus { get; set; }
            public bool IsSaveMessageChat { get; set; }
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
            DbService.RemoveContact(contact.Contact, contact.LoggedUser);
        }
        public class RemoveContactDTO
        {
            public UserContactcs Contact { get; set; }
            public TelegramLib.MainClasses.User LoggedUser { get; set; }
        }

        [HttpPut("AddChat")]
        public void AddChat([FromBody] AddChatDTO addChat)
        {
            DbService.AddChat(addChat.UserId, addChat.ChatterContactId);
        }
        public class AddChatDTO
        {
            public int UserId { get; set; }
            public int ChatterContactId { get; set; }
        }

        [HttpPut("AddStatMessage")]
        public void AddStatMessage([FromBody] AddStatMessageDTO sto)
        {
            DbService.AddStatMessage(sto.ChatId, sto.StatMessage);
        }
        public class AddStatMessageDTO
        {
            public StaticMessage StatMessage { get; set; }
            public int ChatId { get; set; }
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
        public class AutoDelDTO
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
        public class PathDTO
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
        public class BlockedContactDTO
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

        [HttpGet("GetLastStatMesIdByChatId")]
        public int? GetLastStatMesIdByChatId(int chatId)
        {
            return DbService.GetLastStatMesIdByChatId(chatId);
        }

        [HttpGet("GetStatMessageIdByItsReference")]
        public int? GetStatMessageIdByItsReference(int chatId, int refId)
        {
            return DbService.GetStatMessageIdByItsReference(chatId, refId);
        }


        [HttpGet("IsUserIsBlocked")]
        public bool IsUserIsBlocked(int userId, int contactId)
        {
            return DbService.IsUserIsBlocked(userId, contactId);
        }

        [HttpGet("IsChatExist")]
        public bool IsChatExist(int userId, int contactId)
        {
            return DbService.IsChatIsExist(userId, contactId);
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
        public class ChatWallpaperDTO
        {
            public ChatBackground ToSetPaper { get; set; }
            public int ChatId { get; set; }
        }

        [HttpGet("GetChatBgIdByName")]
        public int GetChatBgidByName(string name)
        {
            return DbService.GetChatBgIdByName(name);
        }

        [HttpGet("IsUserOnline")]
        public bool IsUserOnline(int userId)
        {
            return DbService.IsUserOnline(userId);
        }

        [HttpGet("GetUserById")]
        public TelegramLib.MainClasses.User GetUserById(int userId)
        {
            return DbService.GetUserById(userId);
        }

        [HttpPost("SetUserOnlineStatus")]
        public void SetuserOnlineStatus([FromBody] SetOnlineStatus setStatus)
        {
            DbService.SetOnlineStatus(setStatus.UserId, setStatus.Status);
        }
        public class SetOnlineStatus()
        {
            public int UserId { get; set; }
            public bool Status { get; set; }
        }

        [HttpPost("IsContactContactsInContacts")]
        public bool IsContactContactsInContacts([FromBody] ContactCheckRequest isContains)
        {
            return DbService.IsContactContactsInContacts(isContains.Contact, isContains.ToCheck);
        }
        public class ContactCheckRequest
        {
            public UserContactcs? Contact { get; set; }
            public UserContactcs? ToCheck { get; set; }
        }



        [HttpPost("AddUserImage")]
        public void AddUserImage([FromBody] ToAddUserImage toAddUserImage)
        {
            DbService.AddUserImage(toAddUserImage.User, toAddUserImage.UserImageName);
        }
        public class ToAddUserImage
        {
            public TelegramLib.MainClasses.User User { get; set; }
            public string UserImageName { get; set; }
        }

        [HttpPost("ReadMessage")]
        public void ReadMessage([FromBody] ReadMessageDTO readMessage)
        {
            DbService.SetReadMessageAction(readMessage.Id);
        }
        public class ReadMessageDTO
        {
            public int Id { get; set; }
        }

        [HttpPost("SetReadStatus")]
        public void SetReadStatus([FromBody] SetReadStatusDTO readStatDTO)
        {
            DbService.SetReadStatusByMessIdBySendTime(readStatDTO.MesId);
        }
        public class SetReadStatusDTO
        {
            public int MesId { get; set; }
        }

        [HttpPost("ChangeNotificationState")]
        public void ChangeNotificationState([FromBody] ChangeNotificationStateDTO dto)
        {
            DbService.ChangeNotificationState(dto.ChatId, dto.State);
        }
        public class ChangeNotificationStateDTO
        {
            public int ChatId { get; set; }
            public bool State { get; set; }
        }


        [HttpGet("GetReadStatus")]
        public bool GetReadStatus(int mesId)
        {
            return DbService.GetMessageReadStatusById(mesId);
        }

        [HttpGet("IsDateMesIsExistInChat")]
        public bool? IsDateMesIsExistInChat(int loggedId, int chatterId, string date)
        {
            DateTime.TryParse(date, out DateTime checkDate);

            return DbService.IsInChatterChatIsExistDateMessage(loggedId, chatterId, checkDate);
        }

        [HttpGet("GetMessageIdByDateTime")]
        public int? GetMessageIdByDateTime(string sentTime)
        {
            DateTime.TryParse(sentTime, out DateTime checkDate);
            return DbService.GetCorrectIdBySentDate(checkDate);
        }

        //Set saved messages chat

        [HttpGet("GetSavedMessagesChat")]
        public TelegramLib.MainClasses.SavedMessagesChat GetSavedMessagesChat(int userId)
        {
            return DbService.GetSavedMessageChat(userId);
        }

        [HttpGet("IsDateStatContainsInSavedMessageChat")]
        public bool IsDateStatContainsInSavedMessageChat(int chatId, DateTime date)
        {
            return DbService.IsDateStatContainsInSavedMessageChat(chatId, date);
        }

        [HttpGet("GetLastStatDateIdInSavedChat")]
        public int? GetLastStatDateIdInSavedChat(int chatId)
        {
            return DbService.GetLastStatDateIdInSavedChat(chatId);
        }

        [HttpGet("GetIdOfLastSavedMessage")]
        public int? GetIdOfLastSavedMessage(int chatId)
        {
            return DbService.GetIdOfLastSavedMessage(chatId);
        }


        [HttpPut("AddSavedMessage")]
        public void AddSavedMessage([FromBody] AddSavedMessageDTO mesDTO)
        {
            DbService.AddSavedMessage(mesDTO.SavedChatId, mesDTO.Mes);
        }
        public class AddSavedMessageDTO
        {
            public int SavedChatId { get; set; }
            public TelegramLib.MainClasses.Messages.Message Mes { get; set; }
        }

        [HttpPut("AddSavedChat")]
        public void AddSavedChat([FromBody] AddSavedChatDTO dto)
        {
            DbService.AddSavedMessagesChat(dto.UserId);
        }
        public class AddSavedChatDTO
        {
            public int UserId { get; set; }
        }

        [HttpDelete("DeleteUserImage")]
        public void DeleteUserImage([FromBody] DeleteUserImageDTO dto)
        {
            DbService.RemoveUserImage(dto.UserImg, dto.UserId);
        }

        public class DeleteUserImageDTO
        {
            public TelegramLib.MainClasses.UserParams.UserImage UserImg { get; set; }
            public int UserId { get; set; }
        }

        [HttpDelete("DeleteSavedMessage")]
        public void DeleteSavedMessage([FromBody] DeleteSavedMessageDTO dto)
        {
            DbService.RemoveSavedMessage(dto.SavedChatId, dto.ToRemoveIds);
        }

        public class DeleteSavedMessageDTO
        {
            public int SavedChatId { get; set; }
            public List<int> ToRemoveIds { get; set; }
        }

    }
}
