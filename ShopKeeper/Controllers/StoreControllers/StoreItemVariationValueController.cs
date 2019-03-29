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
	public class StoreItemVariationValueController : Controller
	{
		public StoreItemVariationValueController ()
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
        public ActionResult GetStoreItemVariationValueObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemVariationValueObject> filteredStoreItemVariationValueObjects;
                var countG = new StoreItemVariationValueServices().GetObjectCount();

                var pagedStoreItemVariationValueObjects = GetStoreItemVariationValues(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreItemVariationValueObjects = new StoreItemVariationValueServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreItemVariationValueObjects = pagedStoreItemVariationValueObjects;
                }

                if (!filteredStoreItemVariationValueObjects.Any())
                {
                    return Json(new List<StoreItemVariationValueObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemVariationValueObject, string> orderingFunction = (c =>  c.Value
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreItemVariationValueObjects = sortDirection == "asc" ? filteredStoreItemVariationValueObjects.OrderBy(orderingFunction) : filteredStoreItemVariationValueObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreItemVariationValueObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreItemVariationValueId), c.Value};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreItemVariationValueObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemVariationValueObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
	    public ActionResult AddStoreItemVariationValue()
        {
           return View(new StoreItemVariationValueObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreItemVariationValue(StoreItemVariationValueObject productVariationValue)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemVariationValue(productVariationValue);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new StoreItemVariationValueServices().AddStoreItemVariationValue(productVariationValue);
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
        public ActionResult EditStoreItemVariationValue(StoreItemVariationValueObject productVariationValue)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemVariationValue(productVariationValue);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_productVariationValue"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreItemVariationValue = Session["_productVariationValue"] as StoreItemVariationValueObject;
                    if (oldStoreItemVariationValue == null || oldStoreItemVariationValue.StoreItemVariationValueId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreItemVariationValue.Value = productVariationValue.Value.Trim();
                    var k = new StoreItemVariationValueServices().UpdateStoreItemVariationValue(oldStoreItemVariationValue);
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
        public ActionResult DeleteStoreItemVariationValue(long id)
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
                    
                var k = new StoreItemVariationValueServices().DeleteStoreItemVariationValue(id);
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
        public ActionResult GetStoreItemVariationValue(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreItemVariationValueObject(), JsonRequestBehavior.AllowGet);
                }

                var productVariationValue = new StoreItemVariationValueServices().GetStoreItemVariationValue(id);
                if (id < 1)
                {
                    return Json(new StoreItemVariationValueObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_productVariationValue"] = productVariationValue;
                return Json(productVariationValue, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StoreItemVariationValueObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<StoreItemVariationValueObject> GetStoreItemVariationValues(int? itemsPerPage, int? pageNumber)
        {
            return new StoreItemVariationValueServices().GetStoreItemVariationValueObjects(itemsPerPage, pageNumber) ?? new List<StoreItemVariationValueObject>();
        }
        
        private GenericValidator ValidateStoreItemVariationValue(StoreItemVariationValueObject productVariationValue)
        {
            var gVal = new GenericValidator();
            if (productVariationValue == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(productVariationValue.Value))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Variation_Value_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
