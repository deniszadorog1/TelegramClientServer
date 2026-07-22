using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelegramClientServer.Interfaces;
using TelegramLib.MainClasses;
using TelegramLib.Services;

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly IFController _clientProps;
        public ContactController(IFController clientProps)
        {
            _clientProps = clientProps;
        }

        //Add contact
        [HttpPut("AddContact")]
        public void AddContact([FromBody] ContactDTO contact)
        {
/*            int userId = _clientProps.GetCurrentUserId();
            if (contact.UserId != userId) return;*/

            DbService.AddContact(contact.Contact, contact.UserId);
        }

        //update contact
        [HttpPost("UpdateContact")]
        public void UpdateContact([FromBody] ContactDTO contact)
        {
            int userId = _clientProps.GetCurrentUserId();
            if (contact.UserId != userId) return;

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
            int userId = _clientProps.GetCurrentUserId();
            if (dto.LoggedUserId != userId) return;

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
            int userId = _clientProps.GetCurrentUserId();
            if (loggedUserId != userId) return null;

            return DbService.GetContactMaskByContactUserId(loggedUserId, contactUserId);
        }

        [HttpGet("GetLastUserContact")]
        public UserContactcs GetContact(int userId)
        {
            int id = _clientProps.GetCurrentUserId();
            if (id != userId) return null;

            return DbService.GetLastAddedContactByUser(userId);
        }

        [HttpGet("IsContactExist")]
        public bool IsContactExist(int userId, int friendId)
        {
            int id = _clientProps.GetCurrentUserId();
            if (id != userId) return false;

            return DbService.IsContactIsExist(userId, friendId);
        }

        [HttpGet("IsChatterIdIsContact")]
        public bool IsChatterIdIsContact(int userId, int friendUserId)
        {
            int id = _clientProps.GetCurrentUserId();
            if (id != userId) return false;

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
