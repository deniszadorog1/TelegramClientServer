namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PossibleChatBGs
    {
        public int Id { get; set; }

        public int? ChatId { get; set; }

        public int? ChatBgId { get; set; }

        public bool? IsGeneral { get; set; }

        public virtual Chat Chat { get; set; }

        public virtual ChatBG ChatBG { get; set; }
    }
}
