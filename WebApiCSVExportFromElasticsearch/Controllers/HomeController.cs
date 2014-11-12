using System.Web.Mvc;

namespace WebApiCSVExportFromElasticsearch.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";

			return View();
		}
	}
}
