using System.Web.Mvc;
using System.Web.Routing;

namespace ShopKeeper.GenericHelpers
{
    public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
    {
        //public override void OnException(ExceptionContext filterContext)
        //{
        //    filterContext.ExceptionHandled = true;
        //    filterContext.Result = new RedirectToRouteResult(
        //        new RouteValueDictionary(new { action = "Error", controller = "Intro" }));
        //}
    }
}