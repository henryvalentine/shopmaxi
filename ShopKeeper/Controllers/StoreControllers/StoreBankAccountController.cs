using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class StoreBankAccountController : Controller
	{
		public StoreBankAccountController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region BankAccount
        public ActionResult BankAccounts()
		{
           // var bankAccounts = new BankAccountService().GetBankAccounts();

           // if (!bankAccounts.Any())
           // {
           //     return View(new List<BankAccount>());
           // }

           //// var tcx = "data:image/jpeg;base64," + Convert.ToBase64String(tx);
            return View();
		}

		
        #endregion
       

        #region General Utilities
        public int GetLoggedOnUserId()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                return Convert.ToInt32(membershipUser.ProviderUserKey);
            }

            return 0;
        }
        #endregion

    }
}
