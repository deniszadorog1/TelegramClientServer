namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContactImageMask")]
    public partial class ContactImageMask
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? FriendId { get; set; }

        [StringLength(255)]
        public string ImageName { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
