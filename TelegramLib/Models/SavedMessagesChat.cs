namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SavedMessagesChat")]
    public partial class SavedMessagesChat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SavedMessagesChat()
        {
            SavedMessages = new HashSet<SavedMessages>();
        }

        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? BgImageId { get; set; }

        public bool? IsRead { get; set; }

        public bool? IsPinned { get; set; }

        public virtual ChatBG ChatBG { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SavedMessages> SavedMessages { get; set; }

        public virtual User User { get; set; }
    }
}
