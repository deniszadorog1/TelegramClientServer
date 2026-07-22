namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NotificatioonsAndSound")]
    public partial class NotificatioonsAndSound
    {
        public int Id { get; set; }

        public int? SettingId { get; set; }

        public bool? DesktopNotification { get; set; }

        public bool? FlashTaskBar { get; set; }

        public bool? AllowSound { get; set; }

        public bool? PrivateChat { get; set; }

        public bool? PinnedMessage { get; set; }

        public virtual Settings Settings { get; set; }
    }
}
