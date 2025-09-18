using System.Web.Mvc;


namespace asp_net_boilerplate.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserID"] == null &&
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Login")
            {
                string returnUrl = filterContext.HttpContext.Request.RawUrl;

                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            controller = "Login",
                            action = "Index",
                            returnUrl = returnUrl
                        }
                    )
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}