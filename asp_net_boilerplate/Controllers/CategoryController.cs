using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;
using System;
using System.Web.Mvc;

namespace asp_net_boilerplate.Controllers
{
    public class CategoryController : BaseController
    {
        CategoryProvider categoryProvider = new CategoryProvider();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadCategory()
        {
            int draw = Convert.ToInt32(Request.Form["draw"]);
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string searchValue = Request.Form["search[value]"];

            var data = categoryProvider.getCategory(start, length, searchValue);
            var recordsTotal = categoryProvider.getCategoryCount();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(Guid category_id)
        {
            var category = categoryProvider.getCategory(category_id);
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Store(tb_m_category_model category)
        {
            var status = categoryProvider.storeCategory(category);
            return Content(status);
        }

        public ActionResult Delete(Guid category_id)
        {
            var status = categoryProvider.deleteCategory(category_id);
            return Content(status);
        }
    }
}