using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    [Table("Product")]
    public class ProductEntity
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string ProductImg { get; set; }
        public string Describe { get; set; }
        public bool Discontinued { get; set; }
        public int? DisplayID { get; set; }
        public bool? ProductIsDel { get; set; }
        public int? Price { get; set; }
    }
}
