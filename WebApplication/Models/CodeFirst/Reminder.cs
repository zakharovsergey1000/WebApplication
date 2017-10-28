namespace WebApplication.Models.CodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reminder")]
    public partial class Reminder
    {
        public long ID { get; set; }
        public long TaskID { get; set; }
        public long ReminderDateTime { get; set; }
        public ReminderTimeMode ReminderMode { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; }
        public bool IsAlarm { get; set; }
        public int? AutomaticSnoozeTime { get; set; }
        public int? MaxAutomaticSnoozeCount { get; set; }
        public int? PlayingTime { get; set; }
        public bool? Vibrate { get; set; }
        public string VibratePattern { get; set; }
        public bool? Led { get; set; }
        public string LedPattern { get; set; }
        public int? LedColor { get; set; }
        public virtual Task Task { get; set; }

    }
}
