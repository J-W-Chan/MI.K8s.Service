using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.Picture.Entity
{
    [Table("StartProdect")]
    public class StartProdect
    {
        [Key]
        [Column("StartID")]
        public int PKID { get; set; }
        public string StartName { get; set; }
        public string Describe { get; set; }
        public int Price { get; set; }
        public string StartImg { get; set; }
        public string LinkPage { get; set; }
    }
}
