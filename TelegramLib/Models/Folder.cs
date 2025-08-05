namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Folder")]
    public partial class Folder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Folder()
        {
            ContactsInFolder = new HashSet<ContactsInFolder>();
        }

        public int Id { get; set; }

        public int? OwnerId { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? IconId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactsInFolder> ContactsInFolder { get; set; }

        public virtual FolderIcons FolderIcons { get; set; }

        public virtual User User { get; set; }
    }
}
