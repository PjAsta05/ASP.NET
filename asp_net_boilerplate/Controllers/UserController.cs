using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;
using System;
using System.Web.Mvc;

namespace asp_net_boilerplate.Controllers
{
    public class UserController : BaseController
    {
        UserProvider userProvider = new UserProvider();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadUser()
        {
            int draw = Convert.ToInt32(Request.Form["draw"]);
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string searchValue = Request.Form["search[value]"];

            var data = userProvider.GetUserList(start, length, searchValue);
            var recordsTotal = userProvider.GetUserCount();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(Guid user_id)
        {
            var user = userProvider.GetUser(user_id);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Store(tb_m_user_model user)
        {
            var status = userProvider.storeUser(user);
            return Content(status);
        }

        public ActionResult ChangePass(Guid user_id, string old_password, string new_password)
        {
            var status = userProvider.changePassword(user_id, old_password, new_password);
            return Content(status);
        }

        public ActionResult Delete(Guid user_id)
        {
            var status = userProvider.deleteUser(user_id);
            return Content(status);
        }
    }
}