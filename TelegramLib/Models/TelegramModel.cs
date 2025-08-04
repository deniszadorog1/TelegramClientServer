using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace TelegramLib.Models
{
    public partial class TelegramModel : DbContext
    {
        public TelegramModel()
            : base("name=TelegramModel")
        {
        }

        public virtual DbSet<AdvancedSettings> AdvancedSettings { get; set; }
        public virtual DbSet<AutoDeleteType> AutoDeleteType { get; set; }
        public virtual DbSet<AutoNight> AutoNight { get; set; }
        public virtual DbSet<AwayForType> AwayForType { get; set; }
        public virtual DbSet<BioSettings> BioSettings { get; set; }
        public virtual DbSet<BlockedUsers> BlockedUsers { get; set; }
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<ChatBG> ChatBG { get; set; }
        public virtual DbSet<ChatImage> ChatImage { get; set; }
        public virtual DbSet<ChatSettings> ChatSettings { get; set; }
        public virtual DbSet<ChosenPrivacyContacts> ChosenPrivacyContacts { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<DateOfBirthSettings> DateOfBirthSettings { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<FolderIcons> FolderIcons { get; set; }
        public virtual DbSet<ForwardMessagesSettings> ForwardMessagesSettings { get; set; }
        public virtual DbSet<GIF> GIF { get; set; }
        public virtual DbSet<LastSeenSettings> LastSeenSettings { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<MessagesSettings> MessagesSettings { get; set; }
        public virtual DbSet<NotificatioonsAndSound> NotificatioonsAndSound { get; set; }
        public virtual DbSet<PhoneNumberSettings> PhoneNumberSettings { get; set; }
        public virtual DbSet<PrivacySetting> PrivacySetting { get; set; }
        public virtual DbSet<PrivacySettingType> PrivacySettingType { get; set; }
        public virtual DbSet<ProfilePhotoSettings> ProfilePhotoSettings { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<StickerImage> StickerImage { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Theme> Theme { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserImage> UserImage { get; set; }
        public virtual DbSet<WhoCanSeeType> WhoCanSeeType { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AutoDeleteType>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.AutoDeleteType)
                .HasForeignKey(e => e.AutoDeleteId);

            modelBuilder.Entity<BioSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.BioSettings)
                .HasForeignKey(e => e.BioSetId);

            modelBuilder.Entity<ChatBG>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.ChatBG)
                .HasForeignKey(e => e.BgImageId);

            modelBuilder.Entity<ChatImage>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.ChatImage)
                .HasForeignKey(e => e.ImageId);

            modelBuilder.Entity<Contacts>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.Contacts)
                .HasForeignKey(e => e.ChatterId);

            modelBuilder.Entity<Contacts>()
                .HasMany(e => e.ChosenPrivacyContacts)
                .WithOptional(e => e.Contacts)
                .HasForeignKey(e => e.ContactId);

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

            modelBuilder.Entity<LastSeenSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.LastSeenSettings)
                .HasForeignKey(e => e.LastSeenSetId);

            modelBuilder.Entity<MessagesSettings>()
                .HasMany(e => e.PrivacySetting)
                .WithOptional(e => e.MessagesSettings)
                .HasForeignKey(e => e.MessagesSetId);

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

            modelBuilder.Entity<StickerImage>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.StickerImage)
                .HasForeignKey(e => e.StickerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.BlockedUsers)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.BlockedUserId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.BlockedUsers1)
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
                .HasMany(e => e.Folder)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.FriendId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.UserId);

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
