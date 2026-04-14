using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelegramLib.MainClasses;
using TelegramLib.Services;

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        //Add contact
        [HttpPut("AddContact")]
        public void AddContact([FromBody] ContactDTO contact)
        {
            DbService.AddContact(contact.Contact, contact.UserId);
        }

        //update contact
        [HttpPost("UpdateContact")]
        public void UpdateContact([FromBody] ContactDTO contact)
        {
            DbService.UpdateContact(contact.Contact, contact.UserId);
        }
        public class ContactDTO
        {
            public UserContactcs Contact { get; set; }
            public int UserId { get; set; }
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

        [HttpGet("GetContactMask")]
        public TelegramLib.MainClasses.UserParams.UserImage GetContactMask(int loggedUserId,
            int contactUserId)
        {
            return DbService.GetContactMaskByContactUserId(loggedUserId, contactUserId);
        }

        [HttpGet("GetLastUserContact")]
        public UserContactcs GetContact(int userId)
        {
            return DbService.GetLastAddedContactByUser(userId);
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
    }
}
