namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Contacts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contacts()
        {
            Chat = new HashSet<Chat>();
            ChosenPrivacyContacts = new HashSet<ChosenPrivacyContacts>();
        }

        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? FriendId { get; set; }

        [StringLength(1)]
        public string Name { get; set; }

        [StringLength(1)]
        public string LastName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chat> Chat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenPrivacyContacts> ChosenPrivacyContacts { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
