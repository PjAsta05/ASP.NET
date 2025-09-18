using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_net_boilerplate.Models
{
    public class tb_t_sales_model
    {
        [Key]
        public int sales_id { get; set; }
        public int service_type_id { get; set; }
        public int customer_id { get; set; }
        public string transaction_no { get; set; }
        public DateTime transaction_date { get; set; }
        public decimal sub_total { get; set; }
        public decimal service_cost { get; set; }
        public decimal discount_percent { get; set; }
        public decimal discount_value { get; set; }
        public decimal tax_percent { get; set; }
        public decimal tax_value { get; set; }
        public decimal grand_total { get; set; }
        public string notes { get; set; }
    }
}