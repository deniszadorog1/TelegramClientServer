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
        public int Id { get; set; }

        public int? R { get; set; }

        public int? G { get; set; }

        public int? B { get; set; }

        [StringLength(16)]
        public string HashColor { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
