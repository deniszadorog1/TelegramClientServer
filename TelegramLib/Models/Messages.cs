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

        public int? UserId { get; set; }

        public int? FriendId { get; set; }

        [StringLength(1024)]
        public string Message { get; set; }

        public int? ImageId { get; set; }

        public int? StickerId { get; set; }

        public int? GifId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SentDate { get; set; }

        public virtual ChatImage ChatImage { get; set; }

        public virtual GIF GIF { get; set; }

        public virtual User User { get; set; }

        public virtual StickerImage StickerImage { get; set; }

        public virtual User User1 { get; set; }
    }
}
