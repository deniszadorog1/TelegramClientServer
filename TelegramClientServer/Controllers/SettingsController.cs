using Microsoft.AspNetCore.Mvc;
using System.Data;
using TelegramLib;
using TelegramLib.Services;
using TelegramLib.UserSettings.SettingsTypes;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        [HttpPost("UpdateAdvanced")] 
        public void UpdateAdvanced([FromBody] AdvancedDTO adv)
        {
            DbService.UpdateAdvanced(adv.Advanced);
        }
        public class AdvancedDTO()
        {
            public AdvancedSettings Advanced{get;set;}
        }

        [HttpPost("UpdateNotifsAndSounds")]
        public void UpdateNotification([FromBody] NotifsAndSoundDTO notifsSettings)
        {
            DbService.UpdateNotificationSoundsSettings(notifsSettings.NotifsAndSoundSettings);
        }
        public class NotifsAndSoundDTO()
        {
            public NotificationSettings NotifsAndSoundSettings { get; set; }
        }

        [HttpPost("UpdateChatSettings")]
        public void UpdateChatSettings([FromBody] ChatSettingsDTO chatSets)
        {
            DbService.UpdateChat
        }
        public class ChatSettingsDTO()
        {
            public ChatSettings ChatSet { get; set; }
        }

        // GET: api/<SettingsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SettingsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SettingsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SettingsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SettingsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
