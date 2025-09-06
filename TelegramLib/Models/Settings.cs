namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Settings
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Settings()
        {
            AdvancedSettings = new HashSet<AdvancedSettings>();
            ChatSettings = new HashSet<ChatSettings>();
            NotificatioonsAndSound = new HashSet<NotificatioonsAndSound>();
            PrivacySetting = new HashSet<PrivacySetting>();
        }

        public int Id { get; set; }

        public int? UserId { get; set; }

        public bool? IsFolderTabsIsLeft { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvancedSettings> AdvancedSettings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatSettings> ChatSettings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificatioonsAndSound> NotificatioonsAndSound { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrivacySetting> PrivacySetting { get; set; }

        public virtual User User { get; set; }
    }
}
