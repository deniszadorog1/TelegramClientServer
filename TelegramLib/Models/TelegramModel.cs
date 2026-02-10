using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace TelegramLib.Models
{
    public partial class TelegramModel : DbContext
    {
        public TelegramModel()
             : base("data source=(localdb)\\MSSQLLocalDB;initial catalog=TelegramClientServer;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }

        public virtual DbSet<AdvancedSettings> AdvancedSettings { get; set; }
        public virtual DbSet<AutoDeleteType> AutoDeleteType { get; set; }
        public virtual DbSet<AutoNight> AutoNight { get; set; }
        public virtual DbSet<AwayForType> AwayForType { get; set; }
        public virtual DbSet<BioSettings> BioSettings { get; set; }
        public virtual DbSet<BlockedContacts> BlockedContacts { get; set; }
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<ChatBG> ChatBG { get; set; }
        public virtual DbSet<ChatImage> ChatImage { get; set; }
        public virtual DbSet<ChatSettings> ChatSettings { get; set; }
        public virtual DbSet<ChosenPrivacyContacts> ChosenPrivacyContacts { get; set; }
        public virtual DbSet<ContactImageMask> ContactImageMask { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<ContactsInFolder> ContactsInFolder { get; set; }
        public virtual DbSet<DateOfBirthSettings> DateOfBirthSettings { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<FolderIcons> FolderIcons { get; set; }
        public virtual DbSet<ForwardMessagesSettings> ForwardMessagesSettings { get; set; }
        public virtual DbSet<GIF> GIF { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<LanguageType> LanguageType { get; set; }
        public virtual DbSet<LastSeenSettings> LastSeenSettings { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<MessagesSettings> MessagesSettings { get; set; }
        public virtual DbSet<MessageVideo> MessageVideo { get; set; }
        public virtual DbSet<MonitorNotifs> MonitorNotifs { get; set; }
        public virtual DbSet<MonitorSidesType> MonitorSidesType { get; set; }
        public virtual DbSet<NotificationChats> NotificationChats { get; set; }
        public virtual DbSet<NotificatioonsAndSound> NotificatioonsAndSound { get; set; }
        public virtual DbSet<PassCode> PassCode { get; set; }
        public virtual DbSet<PhoneNumberSettings> PhoneNumberSettings { get; set; }
        public virtual DbSet<PossibleChatBGs> PossibleChatBGs { get; set; }
        public virtual DbSet<PrivacySetting> PrivacySetting { get; set; }
        public virtual DbSet<PrivacySettingType> PrivacySettingType { get; set; }
        public virtual DbSet<ProfilePhotoSettings> ProfilePhotoSettings { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<ShareContactMessage> ShareContactMessage { get; set; }
        public virtual DbSet<Sounds> Sounds { get; set; }
        public virtual DbSet<StickerImage> StickerImage { get; set; }
        public virtual DbSet<Theme> Theme { get; set; }
        public virtual DbSet<ThemeColor> ThemeColor { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserColor> UserColor { get; set; }
        public virtual DbSet<UserImage> UserImage { get; set; }
        public virtual DbSet<UserSounds> UserSounds { get; set; }
        public virtual DbSet<UserTheme> UserTheme { get; set; }
        public virtual DbSet<WhoCanSeeType> WhoCanSeeType { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AutoDeleteType>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.AutoDeleteType)
                .HasForeignKey(e => e.AutoDeleteId);

            modelBuilder.Entity<AutoDeleteType>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.AutoDeleteType)
                .HasForeignKey(e => e.ChangedAutoDelId);

            modelBuilder.Entity<BioSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.BioSettings)
                .HasForeignKey(e => e.BioSetId);

            modelBuilder.Entity<ChatBG>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.ChatBG)
                .HasForeignKey(e => e.BgImageId);

            modelBuilder.Entity<ChatBG>()
                .HasMany(e => e.ChatSettings)
                .WithOptional(e => e.ChatBG)
                .HasForeignKey(e => e.BgName);

            modelBuilder.Entity<ChatImage>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.ChatImage)
                .HasForeignKey(e => e.ImageId);

            modelBuilder.Entity<DateOfBirthSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.DateOfBirthSettings)
                .HasForeignKey(e => e.DateOfBirthSetId);

            modelBuilder.Entity<FolderIcons>()
                .HasMany(e => e.Folder)
                .WithOptional(e => e.FolderIcons)
                .HasForeignKey(e => e.IconId);

            modelBuilder.Entity<ForwardMessagesSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.ForwardMessagesSettings)
                .HasForeignKey(e => e.ForwardMesSetId);

            modelBuilder.Entity<LanguageType>()
                .HasMany(e => e.Languages)
                .WithOptional(e => e.LanguageType)
                .HasForeignKey(e => e.TypeId);

            modelBuilder.Entity<LastSeenSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.LastSeenSettings)
                .HasForeignKey(e => e.LastSeenSetId);

            modelBuilder.Entity<MessagesSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.MessagesSettings)
                .HasForeignKey(e => e.MessagesSetId);

            modelBuilder.Entity<MessageVideo>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.MessageVideo)
                .HasForeignKey(e => e.VideoId);

            modelBuilder.Entity<MonitorSidesType>()
                .HasMany(e => e.MonitorNotifs)
                .WithOptional(e => e.MonitorSidesType)
                .HasForeignKey(e => e.Type);

            modelBuilder.Entity<PhoneNumberSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.PhoneNumberSettings)
                .HasForeignKey(e => e.PhoneNumberSetId);

            modelBuilder.Entity<PrivacySetting>()
                .HasMany(e => e.ChosenPrivacyContacts)
                .WithOptional(e => e.PrivacySetting)
                .HasForeignKey(e => e.SttingId);

            modelBuilder.Entity<PrivacySettingType>()
                .HasMany(e => e.ChosenPrivacyContacts)
                .WithOptional(e => e.PrivacySettingType)
                .HasForeignKey(e => e.SettingTypeId);

            modelBuilder.Entity<ProfilePhotoSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.ProfilePhotoSettings)
                .HasForeignKey(e => e.ProfPhotoSetId);

            modelBuilder.Entity<Settings>()
                .HasMany(e => e.AdvancedSettings)
                .WithOptional(e => e.Settings)
                .HasForeignKey(e => e.SettingId);

            modelBuilder.Entity<Settings>()
                .HasMany(e => e.ChatSettings)
                .WithOptional(e => e.Settings)
                .HasForeignKey(e => e.SettingId);

            modelBuilder.Entity<Settings>()
                .HasMany(e => e.NotificatioonsAndSound)
                .WithOptional(e => e.Settings)
                .HasForeignKey(e => e.SettingId);

            modelBuilder.Entity<Settings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.Settings)
                .HasForeignKey(e => e.SettingId);

            modelBuilder.Entity<ShareContactMessage>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.ShareContactMessage)
                .HasForeignKey(e => e.ShareContactId);

            modelBuilder.Entity<Sounds>()
                .HasMany(e => e.UserSounds)
                .WithOptional(e => e.Sounds)
                .HasForeignKey(e => e.ChosenSoundId);

            modelBuilder.Entity<StickerImage>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.StickerImage)
                .HasForeignKey(e => e.StickerId);

            modelBuilder.Entity<Theme>()
                .HasMany(e => e.UserTheme)
                .WithOptional(e => e.Theme)
                .HasForeignKey(e => e.TypeId);

            modelBuilder.Entity<ThemeColor>()
                .HasMany(e => e.UserTheme)
                .WithOptional(e => e.ThemeColor)
                .HasForeignKey(e => e.ColorId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.BlockedContacts)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.BlockedContactId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.BlockedContacts1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.ChatterId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Chat1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ChosenPrivacyContacts)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.ContactId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ContactImageMask)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.FriendId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ContactImageMask1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Contacts)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.FriendId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Contacts1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ContactsInFolder)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.ContactId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Folder)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.SenderId);

            modelBuilder.Entity<UserImage>()
                .HasMany(e => e.ProfilePhotoSettings)
                .WithOptional(e => e.UserImage)
                .HasForeignKey(e => e.PublicPhotoId);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.BioSettings)
                .WithOptional(e => e.WhoCanSeeType)
                .HasForeignKey(e => e.WhoSeeId);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.DateOfBirthSettings)
                .WithOptional(e => e.WhoCanSeeType)
                .HasForeignKey(e => e.WhoSeeId);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.ForwardMessagesSettings)
                .WithOptional(e => e.WhoCanSeeType)
                .HasForeignKey(e => e.WhoSeeId);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.LastSeenSettings)
                .WithOptional(e => e.WhoCanSeeType)
                .HasForeignKey(e => e.WhoSeeId);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.MessagesSettings)
                .WithOptional(e => e.WhoCanSeeType)
                .HasForeignKey(e => e.WhoSeeId);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.PhoneNumberSettings)
                .WithOptional(e => e.WhoCanSeeType)
                .HasForeignKey(e => e.WhoCanFindNumber);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.PhoneNumberSettings1)
                .WithOptional(e => e.WhoCanSeeType1)
                .HasForeignKey(e => e.WhoSeeId);

            modelBuilder.Entity<WhoCanSeeType>()
                .HasMany(e => e.ProfilePhotoSettings)
                .WithOptional(e => e.WhoCanSeeType)
                .HasForeignKey(e => e.WhoSeeId);
        }
    }
}
