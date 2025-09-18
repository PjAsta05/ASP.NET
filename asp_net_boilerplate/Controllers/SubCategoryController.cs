using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace asp_net_boilerplate.Controllers
{
    public class SubCategoryController : BaseController
    {
        SubCategoryProvider subCategoryProvider = new SubCategoryProvider();
        CategoryProvider categoryProvider = new CategoryProvider();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadSubCategory()
        {
            int draw = Convert.ToInt32(Request.Form["draw"]);
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string searchValue = Request.Form["search[value]"];

            var data = subCategoryProvider.getSubCategory(start, length, searchValue);
            var recordsTotal = subCategoryProvider.getSubCategoryCount();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCategory()
        {
            List<category_model> categories = categoryProvider.getAllCategory() ?? new List<category_model>();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(Guid sub_category_id)
        {
            var subCategory = subCategoryProvider.getSubCategory(sub_category_id);
            return Json(subCategory, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Store(tb_m_sub_category_model subCategory)
        {
            var status = subCategoryProvider.storeSubCategory(subCategory);
            return Content(status);
        }

        public ActionResult Delete(Guid sub_category_id)
        {
            var status = subCategoryProvider.deleteSubCategory(sub_category_id);
            return Content(status);
        }
    }
}