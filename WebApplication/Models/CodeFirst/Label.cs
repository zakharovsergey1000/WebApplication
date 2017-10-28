namespace WebApplication.Models.CodeFirst
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Label")]
    public partial class Label
    {
        public Label()
        {
            Children = new HashSet<Label>();
            //Contacts = new HashSet<Contact>();
            //Files = new HashSet<File>();
            //Records = new HashSet<Record>();
            Tasks = new HashSet<Task>();
        }

        public long ID { get; set; }

        public string ApplicationUserId { get; set; }
        //[Required]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public DateTimeOffset Created { get; set; }
            
        [Required]
        [StringLength(256)]
        public string Text { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public DateTimeOffset LastMod { get; set; }

        public long? ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual ICollection<Label> Children { get; set; }

        public virtual Label Parent { get; set; }
        
        public bool Deleted { get; set; }
   
        //public virtual ICollection<Contact> Contacts { get; set; }

        //public virtual ICollection<File> Files { get; set; }

        //public virtual ICollection<Record> Records { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
        public long? CompanyID { get; set; }

        public bool IsCompany { get; set; }

        public bool IsSection { get; set; }

    }
}
