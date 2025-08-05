namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChatSettings
    {
        public int Id { get; set; }

        public int? SettingId { get; set; }

        public int? ThemeId { get; set; }

        public int? UserColorId { get; set; }

        public int? AutoNightId { get; set; }

        [StringLength(255)]
        public string Font { get; set; }

        [StringLength(255)]
        public string BgName { get; set; }

        public bool? IsSentWithEnter { get; set; }

        public virtual AutoNight AutoNight { get; set; }

        public virtual Settings Settings { get; set; }

        public virtual Theme Theme { get; set; }

        public virtual UserColor UserColor { get; set; }
    }
}
