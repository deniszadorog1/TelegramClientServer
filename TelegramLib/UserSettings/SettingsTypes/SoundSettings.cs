using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Chat;
using TelegramLib.Enums.Settings.MuteNotifs;

namespace TelegramLib.UserSettings.SettingsTypes
{
    public class SoundSettings
    {
        public List<string> MesSounds { get; set; }
        public string ChosenSound { get; set; }
        public int Volume { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? MuteTime { get; set; }
        public List<MuteDuration> DurationsTypes { get; set; }
        public bool IsArchived { get; set; }

        private readonly DateTime _mutedForever = new DateTime(3000, 01, 01);
        public SoundSettings()
        {
            MesSounds = new List<string>();
            ChosenSound = null;
            Volume = 100;
            IsEnabled = true;
            MuteTime = null;
            DurationsTypes = new List<MuteDuration>();
        }

        public SoundSettings(List<string> mesSounds, string chosenSound,
            int volume, bool isEnabled, DateTime? mute, 
            List<MuteDuration> durTypes)
        {
            MesSounds = mesSounds;
            ChosenSound = chosenSound;
            Volume = volume;
            IsEnabled = isEnabled;
            MuteTime = mute;
            DurationsTypes = durTypes;
        }

        public void AddSound(string soundName)
        {
            MesSounds.Add(soundName);
        }

        public void SetChosenSound(string soundName)
        {
            if (!MesSounds.Contains(soundName)) return;
            ChosenSound = soundName;
        }

        public string GetChosenSound()
        {
            return ChosenSound;
        }

        public bool IsEqualToChosenSoundName(string soundName)
        {
            return ChosenSound == soundName;
        }

        public void SetVolume(int newVolume)
        {
            Volume = newVolume;
        }

        public int GetVolume()
        {
            return Volume;
        }

        public void ToMirrorEnStatus()
        {
            IsEnabled = !IsEnabled;
        }

        public void ToMirrorMuteStatus()
        {
            if (MuteTime is null)
            {
                MuteTime = _mutedForever;
                return;
            };
            MuteTime = null;
        }

        public bool IsForeverMuted()
        {
            if (MuteTime is null) return false;
            return MuteTime.Value.Year == _mutedForever.Year;
        }

        public void ToMuteForever()
        {
            MuteTime = _mutedForever;
        }

        public void AddDuration(int durTypeIndex)
        {
            DurationsTypes.Add((MuteDuration)durTypeIndex);
        }

        //WTF IS THAT
        public void SetMuteTime()
        {
            MuteDuration mutedTime = DurationsTypes.Last();

            MuteTime = mutedTime == MuteDuration.FifteenMin ? DateTime.Now.AddMinutes(15) :
                       mutedTime == MuteDuration.ThirtyMin ? DateTime.Now.AddMinutes(30) :
                       mutedTime == MuteDuration.OneHour ? DateTime.Now.AddHours(1) :
                       mutedTime == MuteDuration.TwoHours ? DateTime.Now.AddHours(2) :
                       mutedTime == MuteDuration.ThreeHours ? DateTime.Now.AddHours(3) :
                       mutedTime == MuteDuration.FourHours ? DateTime.Now.AddHours(4) :
                       mutedTime == MuteDuration.EightHours ? DateTime.Now.AddHours(8) :
                       mutedTime == MuteDuration.TwelveHours ? DateTime.Now.AddHours(12) :
                       mutedTime == MuteDuration.OneDay ? DateTime.Now.AddDays(1) :
                       mutedTime == MuteDuration.TwoDays ? DateTime.Now.AddDays(2) :
                       mutedTime == MuteDuration.ThreeDays ? DateTime.Now.AddDays(3) :
                       mutedTime == MuteDuration.OneWeek ? DateTime.Now.AddDays(7) :
                       mutedTime == MuteDuration.TwoWeeks ? DateTime.Now.AddDays(14) :
                       mutedTime == MuteDuration.OneMonth ? DateTime.Now.AddMonths(1) :
                       mutedTime == MuteDuration.TwoMonths ? DateTime.Now.AddMonths(2) :
                       mutedTime == MuteDuration.ThreeMonths ? DateTime.Now.AddMonths(3) :
                       DateTime.Now; 
        }

        public void SetCustomDate(int days, int hours, int minutes)
        {
            MuteTime = DateTime.Now.AddDays(days).AddHours(hours).AddMinutes(minutes);
        }

        public void ToMirrorArchive()
        {
            IsArchived = !IsArchived;
        }
    }
}
