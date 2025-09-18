using asp_net_boilerplate.Models;
using asp_net_boilerplate.Provider;
using System.Linq;
using System.Web.Mvc;

namespace asp_net_boilerplate.Controllers
{
    public class DashboardController : BaseController
    {
       DashboardProvider dashboardProvider = new DashboardProvider();
        // GET: Dashboard
        public ActionResult Index()
        {
            var salesData = dashboardProvider.GetMonthlySales();

            ViewBag.Labels = string.Join(",", salesData.Select(d => $"'{d.Month.ToString("D2")}/{d.Year}'"));
            ViewBag.Values = string.Join(",", salesData.Select(d => d.Total));

            var topCustomers = dashboardProvider.GetTop10SalesByCustomer();
            ViewBag.TopCustomers = topCustomers;

            return View();
        }
    }
}