namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MonitorNotifs
    {
        public int Id { get; set; }

        public int? Type { get; set; }

        public int? MessagesAmount { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }

        public virtual MonitorSidesType MonitorSidesType { get; set; }
    }
}
