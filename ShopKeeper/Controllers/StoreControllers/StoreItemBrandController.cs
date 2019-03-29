using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class StoreItemBrandController : Controller
	{
		public StoreItemBrandController ()
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
        public ActionResult GetStoreItemBrandObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemBrandObject> filteredStoreItemBrandObjects;
                var countG = new StoreItemBrandServices().GetObjectCount();

                var pagedStoreItemBrandObjects = GetStoreItemBrands(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreItemBrandObjects = new StoreItemBrandServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreItemBrandObjects = pagedStoreItemBrandObjects;
                }

                if (!filteredStoreItemBrandObjects.Any())
                {
                    return Json(new List<StoreItemBrandObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemBrandObject, string> orderingFunction = (c =>  c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreItemBrandObjects = sortDirection == "asc" ? filteredStoreItemBrandObjects.OrderBy(orderingFunction) : filteredStoreItemBrandObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreItemBrandObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreItemBrandId), c.Name};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreItemBrandObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemBrandObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddStoreItemBrand()
        {
           return View(new StoreItemBrandObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreItemBrand(StoreItemBrandObject storeItemBrand)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemBrand(storeItemBrand);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    storeItemBrand.LastUpdated = DateTime.Now;
                    var k = new StoreItemBrandServices().AddStoreItemBrand(storeItemBrand);
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
        public ActionResult EditStoreItemBrand(StoreItemBrandObject storeItemBrand)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemBrand(storeItemBrand);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeItemBrand"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreItemBrand = Session["_storeItemBrand"] as StoreItemBrandObject;
                    if (oldStoreItemBrand == null || oldStoreItemBrand.StoreItemBrandId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreItemBrand.Name = storeItemBrand.Name.Trim();
                    oldStoreItemBrand.LastUpdated = DateTime.Now;
                    if (!string.IsNullOrEmpty(storeItemBrand.Description))
                    {
                        oldStoreItemBrand.Description = storeItemBrand.Description.Trim();
                    }
                    var k = new StoreItemBrandServices().UpdateStoreItemBrand(oldStoreItemBrand);
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
        public ActionResult DeleteStoreItemBrand(long id)
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
                    
                var k = new StoreItemBrandServices().DeleteStoreItemBrand(id);
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
        public ActionResult GetStoreItemBrand(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreItemBrandObject(), JsonRequestBehavior.AllowGet);
                }

                var storeItemBrand = new StoreItemBrandServices().GetStoreItemBrand(id);
                if (id < 1)
                {
                    return Json(new StoreItemBrandObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeItemBrand"] = storeItemBrand;
                return Json(storeItemBrand, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new StoreItemBrandObject(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
       

        #region Helpers
        private List<StoreItemBrandObject> GetStoreItemBrands(int? itemsPerPage, int? pageNumber)
        {
            return new StoreItemBrandServices().GetStoreItemBrandObjects(itemsPerPage, pageNumber) ?? new List<StoreItemBrandObject>();
        }

        private GenericValidator ValidateStoreItemBrand(StoreItemBrandObject storeItemBrand)
        {
            var gVal = new GenericValidator();
            if (storeItemBrand == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeItemBrand.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Brand_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
