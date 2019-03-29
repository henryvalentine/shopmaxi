using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.Controllers
{
    [Authorize]
	public class ChartOfAccountController : Controller
	{
        public ChartOfAccountController()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult Cities()
		{
            return View();
		}

        /// <summary>
        /// Handles calls Ajax from DataTable(to which the Facilities List is/to be bound)
        /// </summary>
        /// <param name="param">
        /// Ajax model that encapsulates all required parameters such as 
        /// filtering, pagination, soting, etc instructions from the DataTable
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetChartOfAccountObjects(JQueryDataTableParamModel param)
        {

            try
            {
                IEnumerable<ChartOfAccountObject> filteredChartOfAccountObjects;
                var countG = new ChartOfAccountServices().GetObjectCount();

                var pagedChartOfAccountObjects = GetCities(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredChartOfAccountObjects = new ChartOfAccountServices().Search(param.sSearch);
                }
                else
                {
                    filteredChartOfAccountObjects = pagedChartOfAccountObjects;
                }

                if (!filteredChartOfAccountObjects.Any())
                {
                    return Json(new List<ChartOfAccountObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<ChartOfAccountObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.AccountType : c.AccountCode
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredChartOfAccountObjects = sortDirection == "asc" ? filteredChartOfAccountObjects.OrderBy(orderingFunction) : filteredChartOfAccountObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredChartOfAccountObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.ChartOfAccountId), c.AccountType, c.AccountCode, c.AccountGroupName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredChartOfAccountObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<ChartOfAccountObject>(), JsonRequestBehavior.AllowGet);
            }
        }

	    public ActionResult AddChartOfAccount()
        {
           return View(new ChartOfAccountObject());
        }
        
        [HttpPost]
        public ActionResult AddChartOfAccount(ChartOfAccountObject chartOfAccount)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateChartOfAccount(chartOfAccount);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new ChartOfAccountServices().AddChartOfAccount(chartOfAccount);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = k;
                    gVal.Error = message_Feedback.Insertion_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = -1;
                gVal.Error = message_Feedback.Model_State_Error;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditChartOfAccount(ChartOfAccountObject chartOfAccount)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateChartOfAccount(chartOfAccount);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_chartOfAccount"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldChartOfAccount = Session["_chartOfAccount"] as ChartOfAccountObject;
                    if (oldChartOfAccount == null || oldChartOfAccount.ChartOfAccountId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldChartOfAccount.AccountType = chartOfAccount.AccountType.Trim();
                    oldChartOfAccount.AccountCode = chartOfAccount.AccountCode.Trim();
                    oldChartOfAccount.AccountGroupId = chartOfAccount.AccountGroupId;
                    var k = new ChartOfAccountServices().UpdateChartOfAccount(oldChartOfAccount);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate: message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Update_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = -5;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteChartOfAccount(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Invalid_Selection;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                    
                var k = new ChartOfAccountServices().DeleteChartOfAccount(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Delete_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = message_Feedback.Delete_Failure;
                return Json(gVal, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetChartOfAccount(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    return Json(new ChartOfAccountObject(), JsonRequestBehavior.AllowGet);
                }

                var chartOfAccount = new ChartOfAccountServices().GetChartOfAccount(id);
                if (id < 1)
                {
                    return Json(new ChartOfAccountObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_chartOfAccount"] = chartOfAccount;
                return Json(chartOfAccount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new ChartOfAccountObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetAccountGroups(string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            var countries = new AccountGroupServices().GetAccountGroups() ?? new List<AccountGroupObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<ChartOfAccountObject> GetCities(int? itemsPerPage, int? pageNumber)
        {
            return new ChartOfAccountServices().GetChartOfAccountObjects(itemsPerPage, pageNumber) ?? new List<ChartOfAccountObject>();
        }
       
        private GenericValidator ValidateChartOfAccount(ChartOfAccountObject chartOfAccount)
        {
            var gVal = new GenericValidator();
            if (chartOfAccount == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(chartOfAccount.AccountType))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Chart_Of_Account_Type_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(chartOfAccount.AccountCode))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Chart_Of_Account_Code_Error;
                return gVal;
            }
            if (chartOfAccount.AccountGroupId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Account_Group_Selection_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
