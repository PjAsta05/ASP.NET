using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace asp_net_boilerplate.Models
{
    public class tb_m_asset_model
    {
        [Key]
        public int equipment { get; set; }
        public string tag_number { get; set; }
        public string description { get; set; }
        public string cat { get; set; }
        public string object_type { get; set; }

        [Key]
        public string entity_id { get; set; }
        public virtual tb_m_company_model tb_m_company { get; set; }
    }
}