using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    [Table("ShopCar")]
    public class ShopCarEntity
    {
        [Key]
        [Column("CarID")]
        public int PKID { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }
    }
}
