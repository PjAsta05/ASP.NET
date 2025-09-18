using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;
using System;
using System.Web.Mvc;

namespace asp_net_boilerplate.Controllers
{
    public class ServiceController : BaseController
    {
        ServiceProvider serviceProvider = new ServiceProvider();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadService()
        {
            int draw = Convert.ToInt32(Request.Form["draw"]);
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string searchValue = Request.Form["search[value]"];

            var data = serviceProvider.getService(start, length, searchValue);
            var recordsTotal = serviceProvider.getServiceCount();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(Guid service_type_id)
        {
            var service = serviceProvider.getService(service_type_id);
            return Json(service, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Store(tb_m_service_type_model service)
        {
            var status = serviceProvider.storeService(service);
            return Content(status);
        }

        public ActionResult Delete(Guid service_type_id)
        {
            var status = serviceProvider.deleteService(service_type_id);
            return Content(status);
        }
    }
}