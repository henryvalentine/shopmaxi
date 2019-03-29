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
	public class StoreCityController : Controller
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
        public ActionResult GetCityObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreCityObject> filteredStoreCityObjects;
                var countG = new StoreCityServices().GetObjectCount();

                var pagedStoreCityObjects = GetCities(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreCityObjects = new StoreCityServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreCityObjects = pagedStoreCityObjects;
                }

                if (!filteredStoreCityObjects.Any())
                {
                    return Json(new List<StoreCityObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreCityObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.StateName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreCityObjects = sortDirection == "asc" ? filteredStoreCityObjects.OrderBy(orderingFunction) : filteredStoreCityObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreCityObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreCityId), c.Name, c.StateName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreCityObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreCityObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddCity(StoreCityObject city)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreCity(city);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new StoreCityServices().AddStoreCity(city);
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
        public ActionResult EditCity(StoreCityObject city)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreCity(city);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_city"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldStoreCity = Session["_city"] as StoreCityObject;
                    if (oldStoreCity == null || oldStoreCity.StoreCityId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreCity.Name = city.Name.Trim();
                    oldStoreCity.StoreStateId = city.StoreStateId;
                    var k = new StoreCityServices().UpdateStoreCity(oldStoreCity);
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
        public ActionResult DeleteCity(long id)
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

                var k = new StoreCityServices().DeleteStoreCity(id);
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
        public ActionResult GetCity(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreCityObject(), JsonRequestBehavior.AllowGet);
                }

                var city = new StoreCityServices().GetStoreCity(id);
                if (id < 1)
                {
                    return Json(new StoreCityObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_city"] = city;
                return Json(city, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreCityObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStates()
        {
            var countries = new StoreStateServices().GetStoreStates() ?? new List<StoreStateObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Helpers
        private List<StoreCityObject> GetCities(int? itemsPerPage, int? pageNumber)
        {
            return new StoreCityServices().GetStoreCityObjects(itemsPerPage, pageNumber) ?? new List<StoreCityObject>();
        }

        private GenericValidator ValidateStoreCity(StoreCityObject city)
        {
            var gVal = new GenericValidator();
            if (city == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(city.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.City_Name_Error;
                return gVal;
            }

            if (city.StoreStateId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.State_Selection_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
