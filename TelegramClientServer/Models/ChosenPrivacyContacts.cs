namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChosenPrivacyContacts
    {
        public int Id { get; set; }

        public int? ContactId { get; set; }

        public int? SettingTypeId { get; set; }

        public bool? IsShare { get; set; }

        public int? SttingId { get; set; }

        public virtual User User { get; set; }

        public virtual PrivacySettingType PrivacySettingType { get; set; }

        public virtual PrivacySetting PrivacySetting { get; set; }
    }
}
