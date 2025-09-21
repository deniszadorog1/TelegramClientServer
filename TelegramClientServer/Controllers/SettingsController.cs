using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using TelegramLib;
using TelegramLib.Helpers;
using TelegramLib.Models;
using TelegramLib.Services;
using TelegramLib.UserSettings;
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
            //chatSets.ChatSet = DbService.GetChatSettingsBySettingsId(1);
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
            DbService.UpdatePrivacySettings(privSets.Settings);
        }
        public class PrivacySettingsDTO()
        {
            public PrivAndSecSettings Settings { get; set; }
        }

        //USER COLOR
        [HttpPost("UpdateUserColor")]
        public void UpdatePrivacyColor([FromBody] ChosenColorDTO color)
        {
            DbService.UpdateColor(color.ChosenColor);
        }
        public class ChosenColorDTO()
        {
            public ColorHelper ChosenColor { get; set; }
        }


        [HttpGet("GetLastSeenVisState")]
        public void GetLastSeenVisState(int userId)
        {
            DbService.GetLastSeenStateByUserId(userId);
        }

        [HttpGet("GetSettingsByUserId")]
        public MainSettings GetSettingsByUserId(int userId)
        {
            return DbService.GetSettingsByUserId(userId);
        }

        [HttpPost("AddWallpaper")]
        public void AddWallpaper([FromBody] AddWallpaperDTO newPaper)
        {
            DbService.AddWallpaper(newPaper.ImgName);
        }
        public class AddWallpaperDTO()
        {
            public string ImgName { get; set; }
        }

        //Add blocked user
        [HttpPost("UpdateNotifMonitor")]
        public void UpdateNotifMonitor([FromBody] NotifMonitorDTO notMonitor)
        {
            DbService.UpdateWindowNotifcation(notMonitor.Side, 
                notMonitor.MessagesAmount, notMonitor.UserId);
        }
        public class NotifMonitorDTO()
        {
            public TelegramLib.Enums.Settings.Notifs.NotifMessageSide Side{ get; set; }
            public int MessagesAmount { get; set; }
            public int UserId { get; set; }
        }

        [HttpPost("UpdateSound")]
        public void UpdateSound([FromBody] UpdateSoundDTO soundUpdate) 
        {
            DbService.UpdateSounds(soundUpdate.UserId, soundUpdate.Sound,
                soundUpdate.Volume, soundUpdate.IsDefault);
        }
        public class UpdateSoundDTO()
        {
            public string Sound { get; set; }
            public int Volume { get; set; }
            public int UserId { get; set; }
            public bool IsDefault { get; set; }
        }

        [HttpPut("AddSound")]
        public void AddSound([FromBody] AddSoundDTO sound)
        {
            DbService.AddSound(sound.Name);
        }
        public class AddSoundDTO()
        {
            public string Name { get; set; }
        }

        [HttpGet("GetAllSounds")]
        public List<string> GetAllSounds()
        {
            return DbService.GetAllSounds();
        }


        
        [HttpPost("UpdateFoldersPosition")]
        public void UpdateFoldersPosition([FromBody] UpdateFolderPositionDTO folderPos)
        {
            DbService.UpdateFolderPosition(folderPos.UserId, folderPos.State);
        }

        public class UpdateFolderPositionDTO()
        {
            public int UserId { get; set; }
            public bool State { get; set; }
        }

    }
}
