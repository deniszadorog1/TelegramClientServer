namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserSounds
    {
        public int Id { get; set; }

        public int? ChosenSoundId { get; set; }

        public int? Volume { get; set; }

        public bool? IsDefaultSound { get; set; }

        public int? UserId { get; set; }

        public virtual Sounds Sounds { get; set; }

        public virtual User User { get; set; }
    }
}
