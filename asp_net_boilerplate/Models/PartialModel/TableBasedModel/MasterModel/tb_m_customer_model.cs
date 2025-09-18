using System;
using System.ComponentModel.DataAnnotations;

namespace asp_net_boilerplate.Models
{
    public class tb_m_customer_model
    {
        [Key]
        public Guid customer_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string handphone { get; set; }
        public string email { get; set; }
        public string action { get; set; }
    }
}