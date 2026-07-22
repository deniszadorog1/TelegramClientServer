namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfilePhotoSettings
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProfilePhotoSettings()
        {
            PrivacySetting = new HashSet<PrivacySetting>();
        }

        public int Id { get; set; }

        public int? WhoSeeId { get; set; }

        public int? PublicPhotoId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrivacySetting> PrivacySetting { get; set; }

        public virtual UserImage UserImage { get; set; }

        public virtual WhoCanSeeType WhoCanSeeType { get; set; }
    }
}
