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
    public class StoreOutletController : Controller  
    {
        public StoreOutletController()
        {
            ViewBag.LoadStatus = "0";
        }

        #region Actions
       
        /// <summary>
        /// Handles calls Ajax from DataTable(to which the Facilities List is/to be bound)
        /// </summary>
        /// <param name="param">
        /// Ajax model that encapsulates all required parameters such as 
        /// filtering, pagination, soting, etc instructions from the DataTable
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStoreOutletObjects(JQueryDataTableParamModel param)
        {
            try
            {
                var countG = new StoreOutletServices().GetObjectCount();

                var pagedStoreOutletObjects = GetStoreOutlets(param.iDisplayLength, param.iDisplayStart);

                //if (!string.IsNullOrEmpty(param.sSearch))
                //{
                //filteredStoreOutletObjects = new StoreOutletServices().Search(param.sSearch);
                //}
                //else
                //{
                IEnumerable<StoreOutletObject> filteredStoreOutletObjects = pagedStoreOutletObjects;
                //}

                if (!filteredStoreOutletObjects.Any())
                {
                    return Json(new List<StoreOutletObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreOutletObject, string> orderingFunction = (c => c.OutletName );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreOutletObjects = sortDirection == "asc" ? filteredStoreOutletObjects.OrderBy(orderingFunction) : filteredStoreOutletObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreOutletObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreOutletId), c.OutletName, c.IsMainOutlet.ToString(), c.DefaultTax.ToString(), c.IsOperational.ToString(), c.DateCreated.ToString("dd/MM/yyyy") };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = countG,
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreOutletObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddStoreOutlet(StoreOutletObject storeOutlet)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreOutlet(storeOutlet);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var storeAddress = new StoreAddressObject
                    {
                        StoreCityId = storeOutlet.StoreAddressObject.StoreCityId,
                        StreetNo = storeOutlet.StoreAddressObject.StreetNo
                    };
                    var t = new StoreAddressServices().AddStoreAddress(storeAddress);
                    if (t < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Outlet_Address_Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    storeOutlet.StoreAddressId = t;
                    storeOutlet.DateCreated = DateTime.Now;
                    var k = new StoreOutletServices().AddStoreOutlet(storeOutlet);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = 5;
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
        public ActionResult EditStoreOutlet(StoreOutletObject storeOutlet)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreOutlet(storeOutlet);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeOutlet"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldStoreOutlet = Session["_storeOutlet"] as StoreOutletObject;
                    if (oldStoreOutlet == null || oldStoreOutlet.StoreOutletId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldStoreOutlet.OutletName = storeOutlet.OutletName;
                    oldStoreOutlet.DefaultTax = storeOutlet.DefaultTax;
                    oldStoreOutlet.FacebookHandle = storeOutlet.FacebookHandle;

                    var storeAddress = new StoreAddressObject
                    {
                        StoreCityId = storeOutlet.StoreAddressObject.StoreCityId,
                        StreetNo = storeOutlet.StoreAddressObject.StreetNo,
                        StoreAddressId = oldStoreOutlet.StoreAddressId
                    };

                    var t = new StoreAddressServices().UpdateStoreAddress(storeAddress);
                    if (t < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Outlet_Address_Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldStoreOutlet.IsOperational = storeOutlet.IsOperational;
                    if (!string.IsNullOrEmpty(oldStoreOutlet.FacebookHandle))
                    {
                        oldStoreOutlet.FacebookHandle = storeOutlet.FacebookHandle;
                    }

                    if (!string.IsNullOrEmpty(oldStoreOutlet.TwitterHandle))
                    {
                        oldStoreOutlet.TwitterHandle = storeOutlet.TwitterHandle;
                    }
                    
                    var k = new StoreOutletServices().UpdateStoreOutlet(oldStoreOutlet);
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
        public ActionResult DeleteStoreOutlet(long id)
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

                var k = new StoreOutletServices().DeleteStoreOutlet(id);
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
        public ActionResult GetStoreOutlet(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreOutletObject(), JsonRequestBehavior.AllowGet);
                }

                var storeOutlet = new StoreOutletServices().GetStoreOutlet(id);
                if (id < 1)
                {
                    return Json(new StoreOutletObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeOutlet"] = storeOutlet;
                return Json(storeOutlet, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new StoreOutletObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetCities()
        {
            var cities = new StoreCityServices().GetCities() ?? new List<StoreCityObject>();
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOutlets()
        {
            var outlets = new StoreOutletServices().GetStoreOutlets();
            return Json(outlets, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Helpers
        private List<StoreOutletObject> GetStoreOutlets(int? itemsPerPage, int? pageNumber)
        {
            return new StoreOutletServices().GetStoreOutletObjects(itemsPerPage, pageNumber) ?? new List<StoreOutletObject>();
        }

        private GenericValidator ValidateStoreOutlet(StoreOutletObject storeOutlet)
        {
            var gVal = new GenericValidator();
            if (storeOutlet == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(storeOutlet.OutletName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Outlet_Name_Eror;
                return gVal;
            }

            if (storeOutlet.StoreAddressObject.StoreCityId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.City_Selection_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(storeOutlet.StoreAddressObject.StreetNo))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Outlet_Address_Not_Provided;
                return gVal;
            }
            
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
