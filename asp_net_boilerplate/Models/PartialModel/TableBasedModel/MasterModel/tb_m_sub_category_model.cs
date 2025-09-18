using System;
using System.ComponentModel.DataAnnotations;

namespace asp_net_boilerplate.Models
{
    public class tb_m_sub_category_model
    {
        [Key]
        public Guid sub_category_id { get; set; }
        public Guid category_id { get; set; }
        public string sub_category_name { get; set; }
        public string category_name { get; set; }
        public string description { get; set; }
        public string action { get; set; }
    }
}
