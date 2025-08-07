namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Messages
    {
        public int Id { get; set; }

        public int? ChatId { get; set; }

        public int? SenderId { get; set; }

        [StringLength(1024)]
        public string Message { get; set; }

        public int? ImageId { get; set; }

        public int? StickerId { get; set; }

        public int? GifId { get; set; }

        public int? VideoId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SentDate { get; set; }

        public virtual Chat Chat { get; set; }

        public virtual ChatImage ChatImage { get; set; }

        public virtual GIF GIF { get; set; }

        public virtual User User { get; set; }

        public virtual StickerImage StickerImage { get; set; }

        public virtual MessageVideo MessageVideo { get; set; }
    }
}
