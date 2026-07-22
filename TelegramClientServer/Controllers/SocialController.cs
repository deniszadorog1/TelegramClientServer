using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelegramClientServer.Interfaces;
//using System.Data.Entity.Core.Common.CommandTrees;
using TelegramLib.MainClasses;
using TelegramLib.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SocialController : ControllerBase
    {
        private readonly IFController _clientProps;
        public SocialController(IFController clientProps)
        {
            _clientProps = clientProps;
        }

        //Add folder
        [HttpPut("AddFolder")]
        public void AddFolder([FromBody] FolderDTO folder)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (userId != folder.UserId) return;

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
            int userId = _clientProps.GetCurrentUserId();
            if (userId != folder.UserId) return NotFound();
            return DbService.UpdateFolder(folder.Folder, folder.UserId) ? Ok() : NotFound();
        }

        [HttpPost("UpdateUserLogin")]
        public void UpdateUserLogin([FromBody] UpdateUserLoginDTO dto)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (userId != dto.UserId) return;

            DbService.UpdateUserLogin(dto.UserId, dto.NewLogin);
        }
        public record UpdateUserLoginDTO(int UserId, string NewLogin);


        //Delete frolder()
        [HttpDelete("DeleteFolder")]
        public void DeleteFolder([FromBody] FolderDTO folder)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (userId != folder.UserId) return;

            DbService.RemoveFolder(folder.Folder.Id);
        }

        [HttpDelete("DeleteContactFromFolder")]
        public void DeleteContactFromFolder([FromBody] RemoveContactFromFolderDTO dto)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (userId != dto.UserId) return;

            DbService.DeleteUserFromFolder(dto.FolderId, dto.UserId);
        }
        public class RemoveContactFromFolderDTO
        {
            public int FolderId { get; set; }
            public int UserId { get; set; }
        }

        [HttpGet("GetUserByPhoneNumber")]
        public IActionResult GetUserByPhoneNumber(string phoneNumber)
        {
            TelegramLib.MainClasses.User res = DbService.GetUserByPhoneNumber(phoneNumber);

            if (res is null) return NotFound(null);

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

        [HttpGet("IsLoginExist")]
        public bool IsLoginExist(string login)
        {
            return DbService.IsLoginExist(login);
        }

        [HttpGet("GetLastFolderIdByUserId")]
        public int GetLastFolderIdByUserId(int userId)
        {
            int id = _clientProps.GetCurrentUserId();
            if (userId != id) return -1;

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
            int userId = _clientProps.GetCurrentUserId();
            if (userId != dto.UserId) return;

            DbService.UpdateUserNameSurname(dto.UserId, dto.Name, dto.Surname);
        }
        public record UpdateUserNameSurnameDTO(int UserId, string Name, string Surname);

        [HttpDelete("RemoveContact")]
        public void RemoveContact([FromBody] RemoveContactDTO contact)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (userId != contact.LoggedUser.Id) return;

            DbService.RemoveContact(contact.Contact, contact.LoggedUser);
        }
        public class RemoveContactDTO
        {
            public UserContactcs Contact { get; set; }
            public TelegramLib.MainClasses.User LoggedUser { get; set; }
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
            int userId = _clientProps.GetCurrentUserId();
            if (userId != contact.UserId) return;

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
            int userId = _clientProps.GetCurrentUserId();
            if (userId != contact.UserId) return;

            DbService.UnBlockContact(contact.UserId, contact.ContactId);
        }

        [HttpGet("GetStatMessageIdByItsReference")]
        public int? GetStatMessageIdByItsReference(int chatId, int refId)
        {
            return DbService.GetStatMessageIdByItsReference(chatId, refId);
        }

        [HttpGet("IsUserIsBlocked")]
        public bool IsUserIsBlocked(int userId, int contactId)
        {
            int id = _clientProps.GetCurrentUserId();
            if (userId != id) return false;

            return DbService.IsUserIsBlocked(userId, contactId);
        }

        [HttpGet("ContactBySenderAndReceiverIds")]
        public UserContactcs GetContactBySenderAndReceiverIds(int senderId, int receiverId)
        {
            return DbService.GetContactBySenderReceiverUserIds(senderId, receiverId);
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
            int userId = _clientProps.GetCurrentUserId();
            if (userId != setStatus.UserId) return;

            DbService.SetOnlineStatus(setStatus.UserId, setStatus.Status);
        }
        public class SetOnlineStatus()
        {
            public int UserId { get; set; }
            public bool Status { get; set; }
        }

        [HttpPost("AddUserImage")]
        public void AddUserImage([FromBody] ToAddUserImage toAddUserImage)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (userId != toAddUserImage.User.Id) return;

            DbService.AddUserImage(toAddUserImage.User, toAddUserImage.UserImageName);
        }
        public class ToAddUserImage
        {
            public TelegramLib.MainClasses.User User { get; set; }
            public string UserImageName { get; set; }
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

        [HttpDelete("DeleteUserImage")]
        public void DeleteUserImage([FromBody] DeleteUserImageDTO dto)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (userId != dto.UserId) return;

            DbService.RemoveUserImage(dto.UserImg, dto.UserId);
        }
        public class DeleteUserImageDTO
        {
            public TelegramLib.MainClasses.UserParams.UserImage UserImg { get; set; }
            public int UserId { get; set; }
        }

        //Diploma
        [HttpGet("stream/{chatId}")]
        public async Task GetStream(int chatId)
        {
            Response.ContentType = "application/x-ndjson";

            var messageStream = DbService.StreamMessagesById(chatId);

            await foreach (var message in messageStream)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                await Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(json + "\n"));
                await Response.Body.FlushAsync();
            }
        }

    }
}
