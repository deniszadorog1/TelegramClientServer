namespace TelegramLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdvancedSettings
    {
        public int Id { get; set; }

        public int? SettingId { get; set; }

        public bool? IsShowChatName { get; set; }

        public bool? IsTotalUnredCount { get; set; }

        public bool? IsUseSysWIndowFrame { get; set; }

        public bool? IsShowTrayIcon { get; set; }

        public bool? IsShowTaskBarIcon { get; set; }

        public bool? IsCloseToTaskBar { get; set; }

        public bool? IsLaunchWhenStart { get; set; }

        public bool? IsUpdateAutomatically { get; set; }

        public bool? IsInstallBetaVersion { get; set; }

        public virtual Settings Settings { get; set; }
    }
}
