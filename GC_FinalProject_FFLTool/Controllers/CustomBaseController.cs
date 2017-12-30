using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/*** This was a means to control the body HTML element of the website on a per-page basis ***/
/*** See http://ericsowell.com/blog/2011/6/21/auto-page-specific-css-with-asp-net-mvc     ***/

namespace GC_FinalProject_FFLTool.Controllers
{
    public class CustomBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string pageClass = filterContext.RouteData.Values["controller"].ToString() + "_" + filterContext.RouteData.Values["action"].ToString();

            ViewBag.PageClass = pageClass.ToLower();
        }
    }
}
