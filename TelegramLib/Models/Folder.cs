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
        public int Id { get; set; }

        public int? OwnerId { get; set; }

        [StringLength(1)]
        public string Name { get; set; }

        public int? IconId { get; set; }

        public virtual FolderIcons FolderIcons { get; set; }

        public virtual User User { get; set; }
    }
}
