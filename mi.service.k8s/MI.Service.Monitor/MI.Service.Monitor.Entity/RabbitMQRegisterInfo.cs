using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Service.Monitor.Entity
{

    [Table("RabbitMQRegisterInfo")]
    public class RabbitMQRegisterInfo
    {
        [Key]
        [Column("Id")]
        public int PKID { get; set; }
        public string RoutingKey { get; set; }
        public string QueueName { get; set; }
        public string ApiUrl { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
