using System;
using System.ComponentModel.DataAnnotations;

namespace asp_net_boilerplate.Models
{
    public class tb_m_service_type_model
    {
        [Key]
        public Guid service_type_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string action { get; set; }
    }
}