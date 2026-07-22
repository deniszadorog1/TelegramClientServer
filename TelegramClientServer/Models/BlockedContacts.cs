namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BlockedContacts
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? BlockedContactId { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
