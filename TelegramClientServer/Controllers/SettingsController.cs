using Microsoft.AspNetCore.Mvc;
using System.Data;
using TelegramLib;
using TelegramLib.Helpers;
using TelegramLib.Models;
using TelegramLib.Services;
using TelegramLib.UserSettings.SettingsTypes;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        //ADVANCED
        [HttpPost("UpdateAdvanced")] 
        public void UpdateAdvanced([FromBody] AdvancedDTO adv)
        {
            DbService.UpdateAdvanced(adv.Advanced);
        }
        public class AdvancedDTO()
        {
            public TelegramLib.UserSettings.SettingsTypes.AdvancedSettings Advanced{get;set;}
        }

        //NOTIFICATIONS
        [HttpPost("UpdateNotifsAndSounds")]
        public void UpdateNotification([FromBody] NotifsAndSoundDTO notifsSettings)
        {
            DbService.UpdateNotificationSoundsSettings(notifsSettings.NotifsAndSoundSettings);
        }
        public class NotifsAndSoundDTO()
        {
            public NotificationSettings NotifsAndSoundSettings { get; set; }
        }

        //CHAT SETTINGS
        [HttpPost("UpdateChatSettings")]
        public void UpdateChatSettings([FromBody] ChatSettingsDTO chatSets)
        {
            DbService.UpdateChatSettings(chatSets.ChatSet);
        }
        public class ChatSettingsDTO()
        {
            public TelegramLib.UserSettings.SettingsTypes.ChatSettings ChatSet { get; set; }
        }

        //Update PRIVACY
        [HttpPost("UpdatePrivacySettings")]
        public void UpdatePrivacySettings([FromBody] PrivacySettingsDTO privSets)
        {
            DbService.UpdatePrivacySettings(privSets.ChatSet);
        }
        public class PrivacySettingsDTO()
        {
            public PrivAndSecSettings ChatSet { get; set; }
        }

        //USER COLOR
        [HttpPost("UpdateUserColor")]
        public void UpdatePrivacySettings([FromBody] ChosenColorDTO color)
        {
            DbService.UpdateColor(color.ChosenColor);
        }
        public class ChosenColorDTO()
        {
            public ColorHelper ChosenColor { get; set; }
        }



    }
}
