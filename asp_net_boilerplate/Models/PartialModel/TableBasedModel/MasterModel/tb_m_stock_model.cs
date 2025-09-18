using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace asp_net_boilerplate.Models
{
    public class tb_m_stock_model
    {
        [Key]
        public Guid stock_id { get; set; }
        public Guid category_id { get; set; }
        public string category_name { get; set; }
        public Guid sub_category_id { get; set; }
        public string sub_category_name { get; set; }
        public string name { get; set; }
        public string barcode { get; set; }
        public string notes { get; set; }
        public decimal price { get; set; }
        public string action { get; set; }
    }
}