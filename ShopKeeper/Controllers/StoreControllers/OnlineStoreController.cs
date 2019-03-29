using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.GenericHelpers;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    public class OnlineStreController : Controller
	{
        #region Actions
        
        public ActionResult GetLinks()
        {
            var countries = new CountryServices().GetCountries() ?? new List<StoreCountryObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
       
        #endregion

    }
}
