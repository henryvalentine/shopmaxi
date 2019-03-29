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

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class AccountGroupController : Controller
	{
        public AccountGroupController()
		{
		}

        #region Actions
        public ActionResult AccountGroups()
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
	    /// <param name="subdomain">The Current Store in session</param>
	    /// <returns></returns>
	    [HttpGet]
        public ActionResult GetAccountGroupObjects(JQueryDataTableParamModel param)
        {
        
            try
            {
               IEnumerable<AccountGroupObject> filteredAccountGroupObjects;
                var countG = new AccountGroupServices().GetObjectCount();

                var pagedAccountGroupObjects = GetAccountGroups(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredAccountGroupObjects = new AccountGroupServices().Search(param.sSearch);
                }
                else
                {
                    filteredAccountGroupObjects = pagedAccountGroupObjects;
                }

                if (!filteredAccountGroupObjects.Any())
                {
                    return Json(new List<AccountGroupObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<AccountGroupObject, string> orderingFunction = (c =>  c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredAccountGroupObjects = sortDirection == "asc" ? filteredAccountGroupObjects.OrderBy(orderingFunction) : filteredAccountGroupObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredAccountGroupObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.AccountGroupId), c.Name};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredAccountGroupObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<AccountGroupObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
	    public ActionResult AddAccountGroup()
        {
           return View(new AccountGroupObject());
        }
        
        [HttpPost]
        public ActionResult AddAccountGroup(AccountGroupObject accountGroup)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateAccountGroup(accountGroup);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new AccountGroupServices().AddAccountGroup(accountGroup);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
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
        public ActionResult EditAccountGroup(AccountGroupObject accountGroup)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateAccountGroup(accountGroup);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_accountGroup"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldAccountGroup = Session["_accountGroup"] as AccountGroupObject;
                    if (oldAccountGroup == null || oldAccountGroup.AccountGroupId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldAccountGroup.Name = accountGroup.Name.Trim();

                    var k = new AccountGroupServices().UpdateAccountGroup(oldAccountGroup);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Update_Success;
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
        public ActionResult DeleteAccountGroup(long id)
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
                    
                var k = new AccountGroupServices().DeleteAccountGroup(id);
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
        public ActionResult GetAccountGroup(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new AccountGroupObject(), JsonRequestBehavior.AllowGet);
                }

                var accountGroup = new AccountGroupServices().GetAccountGroup(id);
                if (id < 1)
                {
                    return Json(new AccountGroupObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_accountGroup"] = accountGroup;
                return Json(accountGroup, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new AccountGroupObject(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
       

        #region Helpers
        private List<AccountGroupObject> GetAccountGroups(int? itemsPerPage, int? pageNumber)
        {
            return new AccountGroupServices().GetAccountGroupObjects(itemsPerPage, pageNumber) ?? new List<AccountGroupObject>();
        }
        
        private GenericValidator ValidateAccountGroup(AccountGroupObject accountGroup)
        {
            var gVal = new GenericValidator();
            if (accountGroup == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(accountGroup.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Account_Group_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
