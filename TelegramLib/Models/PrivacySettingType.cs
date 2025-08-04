namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PrivacySettingType")]
    public partial class PrivacySettingType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrivacySettingType()
        {
            ChosenPrivacyContacts = new HashSet<ChosenPrivacyContacts>();
        }

        public int Id { get; set; }

        [StringLength(1)]
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenPrivacyContacts> ChosenPrivacyContacts { get; set; }
    }
}
