namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Contacts
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? FriendId { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string LastName { get; set; }

        public bool? IsNotifsIsOn { get; set; }

        public bool? IsBlocked { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
