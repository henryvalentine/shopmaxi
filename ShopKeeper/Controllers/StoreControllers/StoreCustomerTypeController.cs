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
	public class StoreCustomerTypeController : Controller
	{
		public StoreCustomerTypeController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult Countries()
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
        public ActionResult GetStoreCustomerTypeObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreCustomerTypeObject> filteredStoreCustomerTypeObjects;
                var countG = new StoreCustomerTypeServices().GetObjectCount();

                var pagedStoreCustomerTypeObjects = GetStoreCustomerTypes(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreCustomerTypeObjects = new StoreCustomerTypeServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreCustomerTypeObjects = pagedStoreCustomerTypeObjects;
                }

                if (!filteredStoreCustomerTypeObjects.Any())
                {
                    return Json(new List<StoreCustomerTypeObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreCustomerTypeObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.Code);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreCustomerTypeObjects = sortDirection == "asc" ? filteredStoreCustomerTypeObjects.OrderBy(orderingFunction) : filteredStoreCustomerTypeObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreCustomerTypeObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreCustomerTypeId), c.Name, c.Code};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreCustomerTypeObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreCustomerTypeObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddStoreCustomerType()
        {
           return View(new StoreCustomerTypeObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreCustomerType(StoreCustomerTypeObject customerType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreCustomerType(customerType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new StoreCustomerTypeServices().AddStoreCustomerType(customerType);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
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
        public ActionResult EditStoreCustomerType(StoreCustomerTypeObject customerType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreCustomerType(customerType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_customerType"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreCustomerType = Session["_customerType"] as StoreCustomerTypeObject;
                    if (oldStoreCustomerType == null || oldStoreCustomerType.StoreCustomerTypeId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldStoreCustomerType.Name = customerType.Name.Trim();
                    oldStoreCustomerType.Code = customerType.Code.Trim();
                    
                    if (!string.IsNullOrEmpty(customerType.Description))
                    {
                        oldStoreCustomerType.Description = customerType.Description.Trim();
                    }
                    var k = new StoreCustomerTypeServices().UpdateStoreCustomerType(oldStoreCustomerType);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Model_State_Error;
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
        public ActionResult DeleteStoreCustomerType(long id)
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
                    
                var k = new StoreCustomerTypeServices().DeleteStoreCustomerType(id);
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
        public ActionResult GetStoreCustomerType(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreCustomerTypeObject(), JsonRequestBehavior.AllowGet);
                }

                var customerType = new StoreCustomerTypeServices().GetStoreCustomerType(id);
                if (id < 1)
                {
                    return Json(new StoreCustomerTypeObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_customerType"] = customerType;
                return Json(customerType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StoreCustomerTypeObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<StoreCustomerTypeObject> GetStoreCustomerTypes(int? itemsPerPage, int? pageNumber)
        {
            return new StoreCustomerTypeServices().GetStoreCustomerTypeObjects(itemsPerPage, pageNumber) ?? new List<StoreCustomerTypeObject>();
        }
        
        private GenericValidator ValidateStoreCustomerType(StoreCustomerTypeObject customerType)
        {
            var gVal = new GenericValidator();
            if (customerType == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(customerType.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Customer_Type_Name;
                return gVal;
            }
            if (string.IsNullOrEmpty(customerType.Code))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Customer_Type_Code;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
