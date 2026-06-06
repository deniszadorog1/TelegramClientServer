using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Services;

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        [HttpPut("AddShareContactMessage")]
        public void AddShareContactMessage(
            [FromBody] AddShareContactMessageDTO shareMessage)
        {
            DbService.AddShareMessage(shareMessage.SharedUserId,
                shareMessage.SharedName, shareMessage.ChatId,
                shareMessage.SenderId, shareMessage.Date);
        }
        public class AddShareContactMessageDTO
        {
            public int SharedUserId { get; set; }
            public string SharedName { get; set; }
            public int ChatId { get; set; }
            public int SenderId { get; set; }
            public DateTime Date { get; set; }
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


        [HttpPut("AddAndGetSchedMessage")]
        public TelegramLib.MainClasses.Messages.Message AddAndGetSchedMessage([FromBody] MessageDTO dto)
        {
            return DbService.AddAndGetSchedMessage(dto.Chat, dto.ActionMessage);
        }

        //Add ALL forwarded messages 
        [HttpPost("AddAndGetAllForwMessages")]
        public List<TelegramLib.MainClasses.Messages.Message> AddAndGetAllForwMessages([FromBody] AddAndGetAllForwMessagesDTO dto)
        {
            return DbService.AddAllForwardedMessages(dto.Chat, dto.Messages, dto.LoggedUserId);
        }

        public record AddAndGetAllForwMessagesDTO 
        {
            public UserChat Chat { get; set; }
            public List<TelegramLib.MainClasses.Messages.Message> Messages { get; set; }
            public int LoggedUserId { get; set; }
        }


        //Add Message
        [HttpPut("AddMessage")]
        public TelegramLib.MainClasses.Messages.Message AddMessage([FromBody] MessageDTO dto)
        {
            return DbService.AddMessage(dto.Chat, dto.ActionMessage);
        }
        public class MessageDTO
        {
            public UserChat Chat { get; set; }
            public Message ActionMessage { get; set; }
        }

        [HttpPut("AddMessages")]
        public List<TelegramLib.MainClasses.Messages.Message> AddMessages([FromBody] MessagesDTO dto)
        {
            return DbService.AddMessages(dto.Chat, dto.Messages);
           // return res ? Ok(true) : NotFound(false);
        }
        public record MessagesDTO
        {
            public UserChat Chat { get; set; }
            public List<Message> Messages { get; set; } 
        }

      

        [HttpPost("AddAndGetPairMediaMessages")]
        public ActionResult<List<MediaAction>> AddAndGetMediaMessages([FromBody] MediaMessagesRequestDTO request)
        {
            var result = DbService.AddAndGetPairsMediaMessages(request.Medias, request.Chat);
            return Ok(result);
        }
        public class MediaMessagesRequestDTO
        {
            public List<MediaAction> Medias { get; set; }
            public UserChat Chat { get; set; }

            public MediaMessagesRequestDTO() { }
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

        [HttpPost("ReadMessage")]
        public void ReadMessage([FromBody] ReadMessageDTO readMessage)
        {
            DbService.SetReadMessageAction(readMessage.Ids);
        }
        public class ReadMessageDTO
        {
            public List<int> Ids { get; set; }
        }

        [HttpPost("SetReadStatus")]
        public void SetReadStatus([FromBody] SetReadStatusDTO readStatDTO)
        {
            DbService.SetReadStatusByMessIdBySendTime(readStatDTO.Ids);
        }
        public class SetReadStatusDTO
        {
            public List<int> Ids { get; set; }
        }

        [HttpGet("GetReadStatus")]
        public bool GetReadStatus(int mesId)
        {
            return DbService.GetMessageReadStatusById(mesId);
        }

        [HttpGet("GetMessageIdByDateTime")]
        public int? GetMessageIdByDateTime(string sentTime)
        {
            DateTime.TryParse(sentTime, out DateTime checkDate);
            return DbService.GetCorrectIdBySentDate(checkDate);
        }

        [HttpPost("AddSavedMessage")]
        public List<TelegramLib.MainClasses.Messages.Message> AddSavedMessage([FromBody] AddSavedMessageDTO mesDTO)
        {
            return DbService.AddSavedMessage(mesDTO.SavedChatId, mesDTO.Messages);
        }
        public class AddSavedMessageDTO
        {
            public int SavedChatId { get; set; }
            public List<TelegramLib.MainClasses.Messages.Message> Messages { get; set; }
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


        [HttpGet("GetReplyIds")]
        public List<int?> GetReplyIds(List<TelegramLib.MainClasses.Messages.Message> messages)
        {
            return DbService.GetPairsForReplyMessage(messages);
        }
    }
}
