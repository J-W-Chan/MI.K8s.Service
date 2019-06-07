using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.Account.Entity
{
    [Table("Customer")]
    public class UserEntity
    {
        [Key]
        [Column("CustomerID")]
        public int PKID { get; set; }

        public string CustomerPhone { get; set; }

        public string CustomerPwd { get; set; }

        [Column("last_login_time")]
        public DateTime? LastLoginTime { get; set; }

        [Column("login_times")]
        public int? ErrorLogin { get; set; }
    }
}
