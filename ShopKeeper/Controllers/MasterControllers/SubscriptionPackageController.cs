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
	public class SubscriptionPackageController : Controller
	{
		public SubscriptionPackageController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult SubscriptionPackages()
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
        public ActionResult GetSubscriptionPackageObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SubscriptionPackageObject> filteredSubscriptionPackageObjects;
                var countG = new SubscriptionPackageServices().GetObjectCount();

                var pagedSubscriptionPackageObjects = GetSubscriptionPackages(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSubscriptionPackageObjects = new SubscriptionPackageServices().Search(param.sSearch);
                }
                else
                {
                    filteredSubscriptionPackageObjects = pagedSubscriptionPackageObjects;
                }

                if (!filteredSubscriptionPackageObjects.Any())
                {
                    return Json(new List<SubscriptionPackageObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SubscriptionPackageObject, string> 
                    orderingFunction = (c =>  c.PackageTitle);

                //sortColumnIndex == 2 ? c.FileStorageSpace.ToString(CultureInfo.InvariantCulture) :
                //sortColumnIndex == 3 ? c.NumberOfStoreItems.ToString(CultureInfo.InvariantCulture) :
                //sortColumnIndex == 4 ? c.NumberOfOutlets.ToString(CultureInfo.InvariantCulture) :
                //sortColumnIndex == 5 ? c.Registers.ToString(CultureInfo.InvariantCulture) :
                //sortColumnIndex == 6 ? c.NumberOfUsers.ToString(CultureInfo.InvariantCulture) :
                //c.MaximumTransactions.ToString(CultureInfo.InvariantCulture)

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSubscriptionPackageObjects = sortDirection == "asc" ? filteredSubscriptionPackageObjects.OrderBy(orderingFunction) : filteredSubscriptionPackageObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredSubscriptionPackageObjects;
                
                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.SubscriptionPackageId), c.PackageTitle, c.DedicatedAccountManager.ToString(CultureInfo.InvariantCulture), c.TelephoneSupport.ToString(CultureInfo.InvariantCulture), 
                                 c.EmailSupport.ToString(CultureInfo.InvariantCulture), c.LiveChat.ToString(CultureInfo.InvariantCulture)};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredSubscriptionPackageObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SubscriptionPackageObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddSubscriptionPackage()
        {
           return View(new SubscriptionPackageObject());
        }
        
        [HttpPost]
        public ActionResult AddSubscriptionPackage(SubscriptionPackageObject subscriptionPackage)
        {
            var gVal = new GenericValidator();
            try
            {
                //if (ModelState.IsValid)
                //{
                    var valStatus = ValidateSubscriptionPackage(subscriptionPackage);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new SubscriptionPackageServices().AddSubscriptionPackage(subscriptionPackage);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    return Json(subscriptionPackage, JsonRequestBehavior.AllowGet);
                //}

                //gVal.Code = -1;
                //gVal.Error = message_Feedback.Model_State_Error;
                //return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditSubscriptionPackage(SubscriptionPackageObject subscriptionPackage)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateSubscriptionPackage(subscriptionPackage);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_subscriptionPackage"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldSubscriptionPackage = Session["_subscriptionPackage"] as SubscriptionPackageObject;
                    if (oldSubscriptionPackage == null || oldSubscriptionPackage.SubscriptionPackageId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldSubscriptionPackage.PackageTitle = subscriptionPackage.PackageTitle.Trim();
                    oldSubscriptionPackage.FileStorageSpace = subscriptionPackage.FileStorageSpace;
                    oldSubscriptionPackage.NumberOfStoreProducts = subscriptionPackage.NumberOfStoreProducts;
                    oldSubscriptionPackage.NumberOfOutlets = subscriptionPackage.NumberOfOutlets;
                    oldSubscriptionPackage.Registers = subscriptionPackage.Registers;
                    oldSubscriptionPackage.NumberOfUsers = subscriptionPackage.NumberOfUsers;
                    oldSubscriptionPackage.UseReportBuilder = subscriptionPackage.UseReportBuilder;
                    oldSubscriptionPackage.GenerateReports = subscriptionPackage.GenerateReports;
                    oldSubscriptionPackage.MaximumTransactions = subscriptionPackage.MaximumTransactions;
                    oldSubscriptionPackage.MaximumCustomer = subscriptionPackage.MaximumCustomer;
                    oldSubscriptionPackage.TransactionFee = subscriptionPackage.TransactionFee;
                    oldSubscriptionPackage.LiveChat = subscriptionPackage.LiveChat;
                    oldSubscriptionPackage.EmailSupport = subscriptionPackage.EmailSupport;
                    oldSubscriptionPackage.TelephoneSupport = subscriptionPackage.TelephoneSupport;
                    oldSubscriptionPackage.DedicatedAccountManager = subscriptionPackage.DedicatedAccountManager;

                    if (!string.IsNullOrEmpty(subscriptionPackage.Note))
                    {
                        oldSubscriptionPackage.Note = subscriptionPackage.Note.Trim();
                    }
                    
                    var k = new SubscriptionPackageServices().UpdateSubscriptionPackage(oldSubscriptionPackage);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    return Json(oldSubscriptionPackage, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteSubscriptionPackage(long id)
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
                    
                var k = new SubscriptionPackageServices().DeleteSubscriptionPackage(id);
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
        public ActionResult GetSubscriptionPackage(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new SubscriptionPackageObject(), JsonRequestBehavior.AllowGet);
                }

                var subscriptionPackage = new SubscriptionPackageServices().GetSubscriptionPackage(id);
                if (id < 1)
                {
                    return Json(new SubscriptionPackageObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_subscriptionPackage"] = subscriptionPackage;
                return Json(subscriptionPackage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new SubscriptionPackageObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<SubscriptionPackageObject> GetSubscriptionPackages(int? itemsPerPage, int? pageNumber)
        {
            return new SubscriptionPackageServices().GetSubscriptionPackageObjects(itemsPerPage, pageNumber) ?? new List<SubscriptionPackageObject>();
        }

        private GenericValidator ValidateSubscriptionPackage(SubscriptionPackageObject subscriptionPackage)
        {
            var gVal = new GenericValidator();
            if (subscriptionPackage == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(subscriptionPackage.PackageTitle.Trim()))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Sub_Package_Name_Error;
                return gVal;
            }

            //if (subscriptionPackage.FileStorageSpace < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Sub_Package_File_Storage_Error;
            //    return gVal;
            //}

            //if (subscriptionPackage.NumberOfStoreItems < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Sub_Package_Number_of_Product_Error;
            //    return gVal;
            //}

            //if (subscriptionPackage.NumberOfOutlets < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Sub_Package_Number_of_Outlet_Error;
            //    return gVal;
            //}

            //if (subscriptionPackage.Registers < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Sub_Package_Number_of_Registers_Error;
            //    return gVal;
            //}

            //if (subscriptionPackage.NumberOfUsers < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Sub_Package_Number_of_Users_Error;
            //    return gVal;
            //}

            //if (subscriptionPackage.MaximumTransactions < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Sub_Package_Max_Transaction_Error;
            //    return gVal;
            //}
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
