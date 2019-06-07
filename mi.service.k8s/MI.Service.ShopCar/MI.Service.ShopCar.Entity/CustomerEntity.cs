using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    [Table("Customer")]
    public class CustomerEntity
    {
        [Key]
        public int CustomerID { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerPwd { get; set; }
        public DateTime? last_login_time { get; set; }
        public int login_times { get; set; }
    }
}
