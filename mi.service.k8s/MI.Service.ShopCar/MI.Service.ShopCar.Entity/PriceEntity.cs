using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    [Table("Price")]
    public class PriceEntity
    {
        [Key]
        [Column("ID")]
        public int PKID { get; set; }

        public int? VersionID { get; set; }

        public int? ProductID { get; set; }

        public int? ColorID { get; set; }

        public double Price { get; set; }
    }
}
