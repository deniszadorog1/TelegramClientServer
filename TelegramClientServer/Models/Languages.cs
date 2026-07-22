namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Languages
    {
        public int Id { get; set; }

        public int? TypeId { get; set; }

        public int? UserId { get; set; }

        public virtual LanguageType LanguageType { get; set; }

        public virtual User User { get; set; }
    }
}
