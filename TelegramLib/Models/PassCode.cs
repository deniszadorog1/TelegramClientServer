namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PassCode")]
    public partial class PassCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PassCode()
        {
            PrivacySetting = new HashSet<PrivacySetting>();
        }

        public int Id { get; set; }

        public int? Minutes { get; set; }

        public bool? IsWinUnlock { get; set; }

        [Column("Passcode")]
        [StringLength(12)]
        public string Passcode1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrivacySetting> PrivacySetting { get; set; }
    }
}
