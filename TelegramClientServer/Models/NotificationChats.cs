namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NotificationChats
    {
        public int Id { get; set; }

        public bool? IsOn { get; set; }

        public int? ChatId { get; set; }

        public virtual Chat Chat { get; set; }
    }
}
