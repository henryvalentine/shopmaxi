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
	public class StoreAddressController : Controller
	{

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
        public ActionResult GetStoreAddressObjects(JQueryDataTableParamModel param)
        {
            var gVal = new GenericValidator();
            try
            {
                IEnumerable<StoreAddressObject> filteredStoreAddressObjects;
                var countG = new StoreAddressServices().GetObjectCount();

                var pagedStoreAddressObjects = GetCities(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreAddressObjects = new StoreAddressServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreAddressObjects = pagedStoreAddressObjects;
                }

                if (!filteredStoreAddressObjects.Any())
                {
                    return Json(new List<StoreAddressObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreAddressObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StreetNo : c.CityName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreAddressObjects = sortDirection == "asc" ? filteredStoreAddressObjects.OrderBy(orderingFunction) : filteredStoreAddressObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreAddressObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreAddressId), c.StreetNo, c.CityName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreAddressObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreAddressObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddStoreAddress(StoreAddressObject storeAddress)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreAddress(storeAddress);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new StoreAddressServices().AddStoreAddress(storeAddress);
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
        public ActionResult EditStoreAddress(StoreAddressObject storeAddress)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreAddress(storeAddress);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeAddress"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldStoreAddress = Session["_storeAddress"] as StoreAddressObject;
                    if (oldStoreAddress == null || oldStoreAddress.StoreAddressId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreAddress.StreetNo = storeAddress.StreetNo.Trim();
                    oldStoreAddress.StoreCityId = storeAddress.StoreCityId;
                    var k = new StoreAddressServices().UpdateStoreAddress(oldStoreAddress);
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
        public ActionResult DeleteStoreAddress(long id)
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

                var k = new StoreAddressServices().DeleteStoreAddress(id);
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
        public ActionResult GetStoreAddress(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    return Json(new StoreAddressObject(), JsonRequestBehavior.AllowGet);
                }

                var storeAddress = new StoreAddressServices().GetStoreAddress(id);
                if (id < 1)
                {
                    return Json(new StoreAddressObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeAddress"] = storeAddress;
                return Json(storeAddress, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreAddressObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStates()
        {
            var countries = new StoreStateServices().GetStoreStates() ?? new List<StoreStateObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Helpers
        private List<StoreAddressObject> GetCities(int? itemsPerPage, int? pageNumber)
        {
            return new StoreAddressServices().GetStoreAddressObjects(itemsPerPage, pageNumber) ?? new List<StoreAddressObject>();
        }

        private GenericValidator ValidateStoreAddress(StoreAddressObject storeAddress)
        {
            var gVal = new GenericValidator();
            if (storeAddress == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeAddress.StreetNo))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Store_Address_StreetNo_Error;
                return gVal;
            }

            if (storeAddress.StoreCityId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.City_Selection_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
