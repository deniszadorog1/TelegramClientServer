namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Chat")]
    public partial class Chat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Chat()
        {
            Messages = new HashSet<Messages>();
            PossibleChatBGs = new HashSet<PossibleChatBGs>();
        }

        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? ChatterId { get; set; }

        public int? BgImageId { get; set; }

        public int? AutoDeleteId { get; set; }

        public bool? IsMute { get; set; }

        public virtual AutoDeleteType AutoDeleteType { get; set; }

        public virtual ChatBG ChatBG { get; set; }

        public virtual Contacts Contacts { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Messages> Messages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PossibleChatBGs> PossibleChatBGs { get; set; }
    }
}
