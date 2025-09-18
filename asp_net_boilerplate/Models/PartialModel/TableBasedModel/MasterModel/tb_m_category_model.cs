using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace asp_net_boilerplate.Models
{
    public class tb_m_category_model
    {
        [Key]
        public Guid category_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string action { get; set; }
    }
}