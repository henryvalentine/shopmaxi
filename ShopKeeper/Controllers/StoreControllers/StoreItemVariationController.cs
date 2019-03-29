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
	public class StoreItemVariationController : Controller
	{
		public StoreItemVariationController ()
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
        public ActionResult GetStoreItemVariationObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemVariationObject> filteredStoreItemVariationObjects;
                var countG = new StoreItemVariationServices().GetObjectCount();

                var pagedStoreItemVariationObjects = GetStoreItemVariations(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreItemVariationObjects = new StoreItemVariationServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreItemVariationObjects = pagedStoreItemVariationObjects;
                }

                if (!filteredStoreItemVariationObjects.Any())
                {
                    return Json(new List<StoreItemVariationObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemVariationObject, string> orderingFunction = (c =>  c.VariationProperty
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreItemVariationObjects = sortDirection == "asc" ? filteredStoreItemVariationObjects.OrderBy(orderingFunction) : filteredStoreItemVariationObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreItemVariationObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreItemVariationId), c.VariationProperty};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreItemVariationObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemVariationObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddStoreItemVariation()
        {
           return View(new StoreItemVariationObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreItemVariation(StoreItemVariationObject productVariation)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemVariation(productVariation);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new StoreItemVariationServices().AddStoreItemVariation(productVariation);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ?message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
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
        public ActionResult EditStoreItemVariation(StoreItemVariationObject productVariation)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemVariation(productVariation);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_productVariation"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreItemVariation = Session["_productVariation"] as StoreItemVariationObject;
                    if (oldStoreItemVariation == null || oldStoreItemVariation.StoreItemVariationId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreItemVariation.VariationProperty = productVariation.VariationProperty.Trim();
                    var k = new StoreItemVariationServices().UpdateStoreItemVariation(oldStoreItemVariation);
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
        public ActionResult DeleteStoreItemVariation(long id)
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
                    
                var k = new StoreItemVariationServices().DeleteStoreItemVariation(id);
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
        public ActionResult GetStoreItemVariation(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreItemVariationObject(), JsonRequestBehavior.AllowGet);
                }

                var productVariation = new StoreItemVariationServices().GetStoreItemVariation(id);
                if (id < 1)
                {
                    return Json(new StoreItemVariationObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_productVariation"] = productVariation;
                return Json(productVariation, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StoreItemVariationObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<StoreItemVariationObject> GetStoreItemVariations(int? itemsPerPage, int? pageNumber)
        {
            return new StoreItemVariationServices().GetStoreItemVariationObjects(itemsPerPage, pageNumber) ?? new List<StoreItemVariationObject>();
        }
        
        private GenericValidator ValidateStoreItemVariation(StoreItemVariationObject productVariation)
        {
            var gVal = new GenericValidator();
            if (productVariation == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(productVariation.VariationProperty))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Variation_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
