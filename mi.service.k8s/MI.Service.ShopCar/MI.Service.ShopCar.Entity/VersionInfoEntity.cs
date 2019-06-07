using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.ShopCar.Entity
{
    [Table("VersionInfo")]
    public class VersionInfoEntity
    {
        [Key]
        public int VersionID { get; set; }
        public int? ProductID { get; set; }
        public string VersionInfo { get; set; }
        public string Describe { get; set; }
    }
}
