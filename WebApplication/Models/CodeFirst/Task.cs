namespace WebApplication.Models.CodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Task")]
    public partial class Task
    {
        public Task()
        {
            //Reminders = new HashSet<Reminder>();
            Children = new HashSet<Task>();
            //Contacts = new HashSet<Contact>();
            Labels = new HashSet<Label>();
        }

        public long ID { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        //[Required]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastMod { get; set; }
        [Required]
        [StringLength(2000)]
        public string Name { get; set; }
        public TaskPriority Priority { get; set; }
        public int? Color { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public int? Length { get; set; }
        public int? ActualLength { get; set; }
        public bool IsCompleted { get; set; }
        public short PercentOfCompletion { get; set; }
        public DateTimeOffset? CompletedTime { get; set; }
        public bool Deleted { get; set; }
        public int SortOrder { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int? TimeUnitCount { get; set; }
        public RecurrenceInterval RecurrenceInterval { get; set; }
        public int? RepetitionsMaxCount { get; set; }
        public DateTimeOffset? RepetitionsEndDateTime { get; set; }
        public int? AutomaticSnoozeTime { get; set; }
        public int? MaxAutomaticSnoozeCount { get; set; }
        public int? PlayingTime { get; set; }
        public bool? Vibrate { get; set; }
        public string VibratePattern { get; set; }
        public bool? Led { get; set; }
        public string LedPattern { get; set; }
        public int? LedColor { get; set; }
        public long? ParentID { get; set; }
        [ForeignKey("ParentID")]
        public virtual ICollection<Task> Children { get; set; }
        public virtual Task Parent { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }
        public virtual ICollection<TaskOccurrence> TaskRecurrences { get; set; }

        //public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<Label> Labels { get; set; }
    }
}
