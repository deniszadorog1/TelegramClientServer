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

        [Column(TypeName = "datetime2")]
        public DateTime? SentDate { get; set; }

        public int? ShareContactId { get; set; }

        public bool IsRead { get; set; }

        public bool? IsPinned { get; set; }

        public int? ReplyId { get; set; }

        public int? ForwardedFrom { get; set; }

        public int? MessageRefference { get; set; }

        public int? ChangedAutoDelId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StatDate { get; set; }

        public bool IsEdited { get; set; }

        public bool IsInSchedule { get; set; }

        public bool IsSavedMessage { get; set; }

        [StringLength(1024)]
        public string MessageQuote { get; set; }

        public virtual AutoDeleteType AutoDeleteType { get; set; }

        public virtual Chat Chat { get; set; }

        public virtual ChatImage ChatImage { get; set; }

        public virtual GIF GIF { get; set; }

        public virtual User User { get; set; }

        public virtual ShareContactMessage ShareContactMessage { get; set; }

        public virtual StickerImage StickerImage { get; set; }

        public virtual MessageVideo MessageVideo { get; set; }
    }
}
