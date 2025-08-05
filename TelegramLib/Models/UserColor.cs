namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserColor")]
    public partial class UserColor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserColor()
        {
            ChatSettings = new HashSet<ChatSettings>();
        }

        public int Id { get; set; }

        public int? R { get; set; }

        public int? G { get; set; }

        public int? B { get; set; }

        [StringLength(16)]
        public string HashColor { get; set; }

        public int? UserId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatSettings> ChatSettings { get; set; }

        public virtual User User { get; set; }
    }
}
