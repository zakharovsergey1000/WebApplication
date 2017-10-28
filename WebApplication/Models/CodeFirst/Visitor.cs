using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models.CodeFirst
{
    [Table("Visitor")]
    public partial class Visitor
    {
        public long ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public long IP { get; set; }
        public string IpString { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }

    }
}