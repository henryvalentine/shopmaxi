using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Master;
using ShopKeeper.GenericHelpers;
using ShopkeeperServices;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices;

namespace ShopKeeper.Controllers
{
    //[SubdomainRoute.SubdomainTenancy]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        private StoreSettingObject GetStoreInfo(string subdomain)
        {
            try
            {
                var domainOj = new StoreSettingObject();
              
                if (Session["_mySettings"] == null)
                {
                    domainOj = new StoreSettingServices().GetStoreSettingByAlias(subdomain);
                    
                    if (domainOj.StoreId < 1)
                    {
                        return null;
                    }

                    var connection = DbContextSwitcherServices.SwitchSqlDatabase(domainOj);

                    if (string.IsNullOrEmpty(connection))
                    {
                        return null;
                    }
                    domainOj.SqlConnectionString = connection;
                    Session["_mySettings"] = domainOj;
                    return domainOj;
                }
                else
                {
                    var sessDom = Session["_mySettings"] as StoreSettingObject;
                    if (sessDom == null || sessDom.StoreId < 1)
                    {
                        return null;
                    }
                    return domainOj;
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}