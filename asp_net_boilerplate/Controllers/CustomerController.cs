using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;
using System;
using System.Web.Mvc;

namespace asp_net_boilerplate.Controllers
{
    public class  CustomerController : BaseController
    {
        CustomerProvider customerProvider = new CustomerProvider();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadCustomer()
        {
            int draw = Convert.ToInt32(Request.Form["draw"]);
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string searchValue = Request.Form["search[value]"];

            var data = customerProvider.getCustomerList(start, length, searchValue);
            var recordsTotal = customerProvider.getCustomerCount();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(Guid customer_id)
        {
            var customer = customerProvider.getCustomer(customer_id);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Store(tb_m_customer_model customer)
        {
            var status = customerProvider.storeCustomer(customer);
            return Content(status);
        }

        public ActionResult Delete(Guid customer_id)
        {
            var status = customerProvider.deleteCustomer(customer_id);
            return Content(status);
        }
    }
}