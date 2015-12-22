using System.Net;
using System.Web.Mvc;

namespace SBSC.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Unknown()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return View("Unknown");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View("NotFound");
        }
    }
}