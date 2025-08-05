namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContactsInFolder")]
    public partial class ContactsInFolder
    {
        public int Id { get; set; }

        public int? FolderId { get; set; }

        public int? ContactId { get; set; }

        public bool? IsExclude { get; set; }

        public virtual Contacts Contacts { get; set; }

        public virtual Folder Folder { get; set; }
    }
}
