using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_net_boilerplate.Models
{
    public class tb_m_user_model
    {
        [Key]
        public Guid user_id { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string action { get; set; }
        public string password { get; set; }
        public bool is_allow_login { get; set; }
        public bool is_deleted { get; set; }
    }
}