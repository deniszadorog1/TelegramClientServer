namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Chat")]
    public partial class Chat
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? ChatterId { get; set; }

        public int? BgImageId { get; set; }

        public int? AutoDeleteId { get; set; }

        public bool? IsMute { get; set; }

        public virtual AutoDeleteType AutoDeleteType { get; set; }

        public virtual ChatBG ChatBG { get; set; }

        public virtual Contacts Contacts { get; set; }

        public virtual User User { get; set; }
    }
}
