using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Master;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.MasterControllers
{
    [Authorize]
	public class BillingCycleController : Controller
	{
        public BillingCycleController()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult Countries()
		{
            return View();
		}
        public ActionResult RefreshSession()
        {
            try
            {
                return Json(5, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
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
        public ActionResult GetBillingCycleObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<BillingCycleObject> filteredBillingCycleObjects;
                var countG = new BillingCycleServices().GetObjectCount();

                var pagedBillingCycleObjects = GetBillingCycles(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredBillingCycleObjects = new BillingCycleServices().Search(param.sSearch);
                }
                else
                {
                    filteredBillingCycleObjects = pagedBillingCycleObjects;
                }

                if (!filteredBillingCycleObjects.Any())
                {
                    return Json(new List<BillingCycleObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<BillingCycleObject, string> orderingFunction = (c => sortColumnIndex == 1 ?  c.Name :sortColumnIndex == 2 ? c.Code : c.Duration.ToString(CultureInfo.InvariantCulture));

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredBillingCycleObjects = sortDirection == "asc" ? filteredBillingCycleObjects.OrderBy(orderingFunction) : filteredBillingCycleObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredBillingCycleObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.BillingCycleId), c.Name, c.Code, c.Duration.ToString(CultureInfo.InvariantCulture) };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredBillingCycleObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<BillingCycleObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
	    public ActionResult AddBillingCycle()
        {
           return View(new BillingCycleObject());
        }
        
        [HttpPost]
        public ActionResult AddBillingCycle(BillingCycleObject billingCycle)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateBillingCycle(billingCycle);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new BillingCycleServices().AddBillingCycle(billingCycle);
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

                gVal.Code = -5;
                gVal.Error = message_Feedback.Model_State_Error;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = 0;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditBillingCycle(BillingCycleObject billingCycle)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateBillingCycle(billingCycle);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_billingCycle"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldBillingCycle = Session["_billingCycle"] as BillingCycleObject;
                    if (oldBillingCycle == null || oldBillingCycle.BillingCycleId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldBillingCycle.Name = billingCycle.Name.Trim();
                    oldBillingCycle.Code = billingCycle.Code.Trim();
                    oldBillingCycle.Duration = billingCycle.Duration;
                    if (string.IsNullOrEmpty(billingCycle.Remark))
                    {
                        oldBillingCycle.Remark = billingCycle.Remark;
                    }
                    var k = new BillingCycleServices().UpdateBillingCycle(oldBillingCycle);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Update_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = -5;
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
        public ActionResult DeleteBillingCycle(long id)
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
                    
                var k = new BillingCycleServices().DeleteBillingCycle(id);
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
                gVal.Error = message_Feedback.Delete_Failure;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetBillingCycle(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new BillingCycleObject(), JsonRequestBehavior.AllowGet);
                }

                var billingCycle = new BillingCycleServices().GetBillingCycle(id);
                if (id < 1)
                {
                    return Json(new BillingCycleObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_billingCycle"] = billingCycle;
                return Json(billingCycle, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new BillingCycleObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       
        #region Helpers
        private List<BillingCycleObject> GetBillingCycles(int? itemsPerPage, int? pageNumber)
        {
            return new BillingCycleServices().GetBillingCycleObjects(itemsPerPage, pageNumber) ?? new List<BillingCycleObject>();
        }

        private GenericValidator ValidateBillingCycle(BillingCycleObject billingCycle)
        {
            var gVal = new GenericValidator();
            if (billingCycle == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(billingCycle.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Billing_Cycle_Name_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(billingCycle.Code))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Billing_Cycle_Code_Error;
                return gVal;
            }
            if (billingCycle.Duration < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Billing_Cycle_Duration_Error;
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
