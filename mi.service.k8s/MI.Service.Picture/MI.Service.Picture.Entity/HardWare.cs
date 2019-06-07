using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.Picture.Entity
{
    [Table("HardWare")]
    public class HardWare
    {
        [Key]
        [Column("HardWareID")]
        public int PKID { get; set; }
        public string ProductName { get; set; }
        public string Describe { get; set; }
        public int? Price { get; set; }
        public string HardWareImg { get; set; }
        public bool? Home { get; set; }
        public string LinkPage { get; set; }
        public int? VersionID { get; set; }
        public int ProductID { get; set; }
    }
}
