using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;

namespace asp_net_boilerplate.Controllers
{
    public class AssetController : Controller
    {
        public string entity_id_global = "wk0008";

        AssetProvider assetProvider = new AssetProvider();
        FunctionalProvider functionalProvider = new FunctionalProvider();

        // GET: Asset
        public ActionResult Index()
        {
            AssetModel assetModel = new AssetModel();

            assetModel.tag_number_list = functionalProvider.getTagNumberList(entity_id_global);
            assetModel.cat_list = functionalProvider.getCatList(entity_id_global);
            assetModel.object_type_list = functionalProvider.getObjectTypeList(entity_id_global);

            return View(assetModel);
        }

        public JsonResult Get(int equipment)
        {
            var jsonResult = Json(assetProvider.getAsset(equipment, entity_id_global), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult List()
        {
            int page = Int32.Parse(Request.Form.GetValues("start").FirstOrDefault());
            int display_row = Int32.Parse(Request.Form.GetValues("length").FirstOrDefault());
            string tag_number = Request.Form.GetValues("tag_number").FirstOrDefault();

            var jsonResult = Json(new { data = assetProvider.getAssetList(entity_id_global, page, display_row, tag_number), recordsFiltered = assetProvider.getAssetList(entity_id_global, 0, int.MaxValue, tag_number).Count() }, JsonRequestBehavior.AllowGet);
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

            string status = assetProvider.storeAsset(tb_m_asset);

            return Content(status);
        }

        [HttpPost]
        public ActionResult Delete(int equipment)
        {
            string status = assetProvider.deleteAsset(equipment, entity_id_global);

            return Content(status);
        }
    }
}