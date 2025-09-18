using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_net_boilerplate.Models
{
    public class tb_t_sales_item_model
    {
        [Key]
        public int sales_item_id { get; set; }
        public decimal qty { get; set; }
        public decimal price { get; set; }
        public decimal discount_percent { get; set; }
        public decimal discount_value { get; set; }
        public decimal sub_total { get; set; }
    }
}