using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    [Table("ColorVersion")]
    public class ColorVersionEntity
    {
        [Key]
        [Column("ID")]
        public int PKID { get; set; }

        public int VersionID { get; set; }

        public string Color { get;set; }

        public string ColorImg { get; set; }
    }
}
