using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    [Table("CarList")]
    public class CarListEntity
    {
        [Key]
        [Column("ID")]
        public int PKID { get; set; }
        public int? CarID { get; set; }
        public int ProductID { get; set; }
        public int VersionID { get; set; }
        public int Count { get; set; }
        public double UnitPrice { get; set; }
        public int? ColorID { get; set; }
        public bool? IsCheck { get; set; }
    }
}
