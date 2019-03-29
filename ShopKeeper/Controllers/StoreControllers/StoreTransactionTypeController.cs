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
	public class StoreTransactionTypeController : Controller
	{
		public StoreTransactionTypeController ()
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
        public ActionResult GetStoreTransactionTypeObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreTransactionTypeObject> filteredStoreTransactionTypeObjects;
                var countG = new StoreTransactionTypeServices().GetObjectCount();

                var pagedStoreTransactionTypeObjects = GetStoreTransactionTypes(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreTransactionTypeObjects = new StoreTransactionTypeServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreTransactionTypeObjects = pagedStoreTransactionTypeObjects;
                }

                if (!filteredStoreTransactionTypeObjects.Any())
                {
                    return Json(new List<StoreTransactionTypeObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreTransactionTypeObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.Action
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreTransactionTypeObjects = sortDirection == "asc" ? filteredStoreTransactionTypeObjects.OrderBy(orderingFunction) : filteredStoreTransactionTypeObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreTransactionTypeObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreTransactionTypeId), c.Name, c.Action };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreTransactionTypeObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreTransactionTypeObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
	    public ActionResult AddStoreTransactionType()
        {
           return View(new StoreTransactionTypeObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreTransactionType(StoreTransactionTypeObject storeTransactionType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreTransactionType(storeTransactionType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new StoreTransactionTypeServices().AddStoreTransactionType(storeTransactionType);
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
        public ActionResult EditStoreTransactionType(StoreTransactionTypeObject storeTransactionType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreTransactionType(storeTransactionType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeTransactionType"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreTransactionType = Session["_storeTransactionType"] as StoreTransactionTypeObject;
                    if (oldStoreTransactionType == null || oldStoreTransactionType.StoreTransactionTypeId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreTransactionType.Name = storeTransactionType.Name.Trim();
                    oldStoreTransactionType.Action = storeTransactionType.Action.Trim();

                    if (!string.IsNullOrEmpty(storeTransactionType.Description))
                    {
                        oldStoreTransactionType.Description = storeTransactionType.Description;
                    }

                    var k = new StoreTransactionTypeServices().UpdateStoreTransactionType(oldStoreTransactionType);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
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
        public ActionResult DeleteStoreTransactionType(long id)
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
                    
                var k = new StoreTransactionTypeServices().DeleteStoreTransactionType(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Delete_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = message_Feedback.Model_State_Error;
                return Json(gVal, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStoreTransactionType(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreTransactionTypeObject(), JsonRequestBehavior.AllowGet);
                }

                var storeTransactionType = new StoreTransactionTypeServices().GetStoreTransactionType(id);
                if (id < 1)
                {
                    return Json(new StoreTransactionTypeObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeTransactionType"] = storeTransactionType;
                return Json(storeTransactionType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StoreTransactionTypeObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<StoreTransactionTypeObject> GetStoreTransactionTypes(int? itemsPerPage, int? pageNumber)
        {
            return new StoreTransactionTypeServices().GetStoreTransactionTypeObjects(itemsPerPage, pageNumber) ?? new List<StoreTransactionTypeObject>();
        }
        
        private GenericValidator ValidateStoreTransactionType(StoreTransactionTypeObject storeTransactionType)
        {
            var gVal = new GenericValidator();
            if (storeTransactionType == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeTransactionType.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Transaction_Type_Name_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeTransactionType.Action))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Transaction_Type_Action_Error;
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
