namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserTheme")]
    public partial class UserTheme
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? TypeId { get; set; }

        public int? ColorId { get; set; }

        public virtual Theme Theme { get; set; }

        public virtual ThemeColor ThemeColor { get; set; }

        public virtual User User { get; set; }
    }
}
