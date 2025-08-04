namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PrivacySetting")]
    public partial class PrivacySetting
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrivacySetting()
        {
            ChosenPrivacyContacts = new HashSet<ChosenPrivacyContacts>();
        }

        public int Id { get; set; }

        public int? SettingId { get; set; }

        public int? PhoneNumberSetId { get; set; }

        public int? LastSeenSetId { get; set; }

        public int? ProfPhotoSetId { get; set; }

        public int? ForwardMesSetId { get; set; }

        public int? MessagesSetId { get; set; }

        public int? DateOfBirthSetId { get; set; }

        public int? BioSetId { get; set; }

        public int? AwayForTypeId { get; set; }

        public virtual AwayForType AwayForType { get; set; }

        public virtual BioSettings BioSettings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenPrivacyContacts> ChosenPrivacyContacts { get; set; }

        public virtual DateOfBirthSettings DateOfBirthSettings { get; set; }

        public virtual ForwardMessagesSettings ForwardMessagesSettings { get; set; }

        public virtual LastSeenSettings LastSeenSettings { get; set; }

        public virtual MessagesSettings MessagesSettings { get; set; }

        public virtual PhoneNumberSettings PhoneNumberSettings { get; set; }

        public virtual ProfilePhotoSettings ProfilePhotoSettings { get; set; }

        public virtual Settings Settings { get; set; }
    }
}
