using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SBSC.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
        }

        protected void Application_BeginRequest()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exception = Server.GetLastError();
        //    Response.Clear();

        //    var httpException = exception as HttpException;

        //    if (httpException != null)
        //    {
        //        string action;

        //        switch (httpException.GetHttpCode())
        //        {
        //            case 404:
        //                // page not found
        //                action = "NotFound";
        //                break;
        //            //case 403:
        //            //    // forbidden
        //            //    action = "Forbidden";
        //            //    break;
        //            //case 500:
        //            //    // server error
        //            //    action = "HttpError500";
        //            //    break;
        //            default:
        //                action = "Unknown";
        //                break;
        //        }

        //        // clear error on server
        //        Server.ClearError();

        //        Response.Redirect(String.Format("~/Error/{0}", action));
        //    }
        //    else
        //    {
        //        // this is my modification, which handles any type of an exception.
        //        Response.Redirect(String.Format("~/Error/Unknown"));
        //    }
        //}
    }
}
