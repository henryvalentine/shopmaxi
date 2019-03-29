using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Shopkeeper.DataObjects.DataObjects.Master;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices;

namespace ShopKeeper.Controllers
{
    [WebApiConfig.AllowCrossSiteJsonAttribute]
    
    public class RemoteController : ApiController
    {
        [HttpGet]
        public List<SubscriptionPackageObject> GetPackages()
        {
            return new SubscriptionPackageServices().GetSubscriptionPackages();
        }

    }
}