using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using asp_net_boilerplate.Models;
using asp_net_boilerplate.DataCenter;

namespace asp_net_boilerplate.Controllers
{
    public class AssetController : Controller
    {
        public string entity_id_global = "wk0008";

        AssetDataCenter assetDataCenter = new AssetDataCenter();

        // GET: Asset
        public ActionResult Index()
        {
            AssetModel assetModel = new AssetModel();

            assetModel.tb_m_asset_list = assetDataCenter.getAssetList(entity_id_global);

            return View(assetModel);
        }
        public JsonResult GetAsset(int equipment)
        {
            var jsonResult = Json(assetDataCenter.getAsset(equipment, entity_id_global), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetAssetList()
        {
            var jsonResult = Json(assetDataCenter.getAssetList(entity_id_global), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult Store()
        {
            tb_m_asset_model tb_m_asset = new tb_m_asset_model();

            tb_m_asset.equipment = Int32.Parse(Request["equipment"].ToString());
            tb_m_asset.tag_number = Request["tag_number"].ToString();
            tb_m_asset.description = Request["description"].ToString();
            tb_m_asset.cat = Request["cat"].ToString();
            tb_m_asset.object_type = Request["object_type"].ToString();
            tb_m_asset.entity_id = entity_id_global;

            string status = assetDataCenter.storeAsset(tb_m_asset);

            return Content(status);
        }

        [HttpDelete]
        public ActionResult Store(int equipment)
        {
            string status = assetDataCenter.deleteAsset(equipment, entity_id_global);

            return Content(status);
        }
    }
}