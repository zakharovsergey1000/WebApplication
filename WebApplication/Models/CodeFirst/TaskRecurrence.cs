using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models.CodeFirst
{
    [Table("TaskOccurrence")]
    public partial class TaskOccurrence
    {
        public long ID { get; set; }
        public long TaskID { get; set; }
        public int OrdinalNumber { get; set; }
        public virtual Task Task { get; set; }
    }
}


