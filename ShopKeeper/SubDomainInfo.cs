using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShopKeeper
{
    public class SubdomainRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            try
            {
                if (httpContext.Request.Url == null) return null;
                var host = httpContext.Request.Url.Host;
                var hostCmponents = host.Split('.');
                var segments = httpContext.Request.Url.AbsolutePath.Split('/');
                
                var subdomain = "";
                var controller = "";
                var action = "";

                //#if DEBUG

                //#endif

                //www.monroe.localhost.com

                subdomain = hostCmponents.Length > 4 && !string.IsNullOrEmpty(hostCmponents[0]) ? hostCmponents[1] : hostCmponents.Length == 4 && !string.IsNullOrEmpty(hostCmponents[0]) && hostCmponents[0] != "www" ? hostCmponents[0] :
                    hostCmponents.Length == 4 && !string.IsNullOrEmpty(hostCmponents[0]) && hostCmponents[0] == "www" ? hostCmponents[1] : 
                    hostCmponents.Length == 3 && !string.IsNullOrEmpty(hostCmponents[0]) && hostCmponents[0] != "www" ? hostCmponents[0] :
                    hostCmponents.Length == 3 && !string.IsNullOrEmpty(hostCmponents[0]) && hostCmponents[0] == "www" ? hostCmponents[1] :
                    hostCmponents.Length == 2 && !string.IsNullOrEmpty(hostCmponents[0]) && hostCmponents[0] != "www" ? hostCmponents[0] : "";
                    
                if (!string.IsNullOrEmpty(subdomain))
                {
                    controller = (segments.Length > 2) ? segments[1] : (segments.Length == 2) && !string.IsNullOrEmpty(segments[0]) ? segments[0] : "Account";
                    action = (segments.Length > 2) ? segments[2] : (segments.Length == 2) && !string.IsNullOrEmpty(segments[0]) ? segments[1] : "Dashboard";
                }
                else
                {
                    //if (segments.Length > 2 && segments[2].Trim().Equals("Store"))
                    //{
                    //    controller = "Store";
                    //}

                    //else
                    //{
                        controller = (segments.Length > 2) ? segments[2] : "Account";
                    //}

                        action = (segments.Length >= 3) ? segments[3] : "XSwict";
                }
                
                var routeData = new RouteData(this, new MvcRouteHandler());
                routeData.Values.Add("controller", controller); //Goes to the relevant Controller  class
                routeData.Values.Add("action", action); //Goes to the relevant action method on the specified Controller
                routeData.Values.Add("subdomain", subdomain); //pass subdomain as argument to action method
                return routeData;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public RouteBase Route { get; set; }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            try
            {
                //var subdomain = values.ContainsKey("subdomain");
                //var rr = Route.GetVirtualPath(requestContext, values); 
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public class SubdomainTenancy : ActionFilterAttribute
        //{
        //    public override void OnActionExecuting(ActionExecutingContext filterContext)
        //    {
        //        string subdomain = filterContext.RequestContext.HttpContext.Request.Params["subdomain"]; // A subdomain specified as a query parameter takes precedence over the hostname.
        //        if (subdomain == null)
        //        {
        //            string host = filterContext.RequestContext.HttpContext.Request.Headers["Host"];
        //            int index = host.IndexOf('.');
        //            if (index >= 0)
        //                subdomain = host.Substring(0, index);
        //            filterContext.Controller.ViewBag.Subdomain = subdomain;
        //        }
        //        base.OnActionExecuting(filterContext);
        //    }
        //}
        
    }
}
