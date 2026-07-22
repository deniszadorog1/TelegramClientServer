using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramClientServer.Interfaces;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.Services;

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ChatController : ControllerBase
    {
        private readonly IFController _clientProps;

        public ChatController(IFController clientProps)
        {
            _clientProps = clientProps;
        }


        [HttpDelete("ClearSaveChatById")]
        public void ClearSaveChatById([FromBody] ClearSaveMessagesChatDTO dto)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (!DbService.IsChatOwnedByUser(dto.Id, userId)) return;

            DbService.ClearSaveChatById(dto.Id);
        }

        public class ClearSaveMessagesChatDTO
        {
            public int Id { get; set; }
        }

        [HttpDelete("DeleteChatById")]
        public void DeleteChatById([FromBody] DeleteChatByChatterIdDTO chatterIdDTO)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (!DbService.IsChatOwnedByUser(chatterIdDTO.ChatId, userId)) return;

            DbService.DeleteChatById(chatterIdDTO.ChatId);
        }
        public class DeleteChatByChatterIdDTO()
        {
            public int ChatId { get; set; }
        }

        //Update chat
        [HttpPost("UpdateChat")]
        public void UpdateChat([FromBody] ChatDTO chat)
        {
            DbService.UpdateChat(chat.Chat);
        }

        //Clear chat
        [HttpDelete("ClearChat")]
        public void ClearChat([FromBody] ChatDTO chat)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (!DbService.IsChatOwnedByUser(chat.Chat.Id, userId)) return;

            DbService.ClearChat(chat.Chat.Id);
        }

        //Add chat bg
        [HttpPost("AddChatBg")]
        public void AddChatBg([FromBody] ChatDTO chat)
        {
            DbService.SetChosenBgInPossibleBGs(chat.Chat.Id, DbService.GetChatBgIdByName(chat.Chat.ChatBg.FileName));
        }

        public class ChatDTO
        {
            public UserChat Chat { get; set; }
        }


        [HttpGet("IsChatExist")]
        public bool IsChatExist(int userId, int contactId)
        {
            return DbService.IsChatIsExist(userId, contactId);
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

        [HttpGet("GetPartlyChats")]
        public List<TelegramLib.MainClasses.UserChat> GetPartlyChats(int userId, int tempChatId)
        {
            return DbService.GetPartOfTheUserChat(userId, tempChatId);
        }

        [HttpGet("GetPartlyMessages")]
        public List<TelegramLib.MainClasses.Messages.Message> GetPartlyMessages(int chatId, int tempMessageId)
        {
            return DbService.GetPartOfTheMessages(chatId, tempMessageId);
        }

        [HttpGet("GetPartlyContacts")]
        public List<TelegramLib.MainClasses.UserContactcs> GetPartlyContacts(int userId, int tempContactId)
        {
            return DbService.GetPartOfTheContacts(userId, tempContactId);
        }

        [HttpGet("GetChatBgIdByName")]
        public int GetChatBgIdByName(string name)
        {
            return DbService.GetChatBgIdByName(name);
        }

        [HttpGet("IsDateMesIsExistInChat")]
        public bool? IsDateMesIsExistInChat(int loggedId, int chatterId, string date)
        {
            DateTime.TryParse(date, out DateTime checkDate);

            return DbService.IsInChatterChatIsExistDateMessage(loggedId, chatterId, checkDate);
        }

        //Set saved messages chat
        [HttpGet("GetSavedMessagesChat")]
        public TelegramLib.MainClasses.SavedMessagesChat GetSavedMessagesChat(int userId)
        {
            if (_clientProps.GetCurrentUserId() != userId) return null;

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

        [HttpPut("AddSavedChat")]
        [AllowAnonymous]
        public void AddSavedChat([FromBody] AddSavedChatDTO dto)
        {
            DbService.AddSavedMessagesChat(dto.UserId);
        }
        public class AddSavedChatDTO
        {
            public int UserId { get; set; }
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
            int userId = _clientProps.GetCurrentUserId();
            if (!DbService.IsChatOwnedByUser(chatId, userId)) return null;

            return DbService.GetMessagesByChatId(chatId);
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

    }
}
