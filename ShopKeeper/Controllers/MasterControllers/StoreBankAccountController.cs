using System;
using System.Web.Mvc;
using System.Web.Security;

namespace ShopKeeper.Controllers.MasterControllers
{
    [Authorize]
	public class StoreBankAccountController : Controller
	{
		public StoreBankAccountController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
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
       

        #region Helpers
       
        #endregion

    }
}
