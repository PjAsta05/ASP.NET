using asp_net_boilerplate.Provider;
using System.Web.Mvc;

namespace asp_net_boilerplate.Controllers
{
    public class LoginController : Controller
    {
        UserProvider userProvider = new UserProvider();

        // GET: Login
        public ActionResult Index(string returnUrl = null)
        {
            if (Session["UserID"] != null)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(string username, string password, string returnUrl)
        {
            var user = userProvider.Authenticate(
                username: username,
                password: password
            );

            if (user != null)
            {
                Session["UserID"] = user.user_id;
                Session["Username"] = user.username;
                Session["Name"] = user.name;

                if (!string.IsNullOrEmpty(returnUrl) && !returnUrl.ToLower().Contains("/login"))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                ViewBag.ReturnUrl = returnUrl;
                return View("Index");
            }
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}