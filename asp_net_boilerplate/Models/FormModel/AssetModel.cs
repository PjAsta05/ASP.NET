using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace asp_net_boilerplate.Models
{
    public class AssetModel
    {
        public List<tag_number_model> tag_number_list { get; set; }
        public List<tb_m_asset_model> tb_m_asset_list { get; set; }
    }
}