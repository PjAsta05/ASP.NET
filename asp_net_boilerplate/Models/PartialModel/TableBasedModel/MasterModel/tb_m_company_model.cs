using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace asp_net_boilerplate.Models
{
    public class tb_m_company_model
    {
        [Key]
        public string entity_id { get; set; }
        public string company_code { get; set; }
        public string description { get; set; }
    }
}