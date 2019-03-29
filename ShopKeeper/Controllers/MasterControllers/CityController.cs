using System;
using System.Collections.Generic;
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
    public class CityController : Controller
    {
        public CityController()
        {
            ViewBag.LoadStatus = "0";
        }

        #region Actions
        public ActionResult Cities()
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
        public ActionResult GetCityObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<CityObject> filteredCityObjects;
                var countG = new CityServices().GetObjectCount();

                var pagedCityObjects = GetCities(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCityObjects = new CityServices().Search(param.sSearch);
                }
                else
                {
                    filteredCityObjects = pagedCityObjects;
                }

                if (!filteredCityObjects.Any())
                {
                    return Json(new List<CityObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<CityObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.StateName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredCityObjects = sortDirection == "asc" ? filteredCityObjects.OrderBy(orderingFunction) : filteredCityObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredCityObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.CityId), c.Name, c.StateName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredCityObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CityObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddCity()
        {
            return View(new CityObject());
        }

        [HttpPost]
        public ActionResult AddCity(CityObject city)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateCity(city);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new CityServices().AddCity(city);
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
        public ActionResult EditCity(CityObject city)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateCity(city);
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

                    var oldCity = Session["_city"] as CityObject;
                    if (oldCity == null || oldCity.CityId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldCity.Name = city.Name.Trim();
                    oldCity.StateId = city.StateId;
                    var k = new CityServices().UpdateCity(oldCity);
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

                var k = new CityServices().DeleteCity(id);
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
                    return Json(new CityObject(), JsonRequestBehavior.AllowGet);
                }

                var city = new CityServices().GetCity(id);
                if (id < 1)
                {
                    return Json(new CityObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_city"] = city;
                return Json(city, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new CityObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStates()
        {
            var countries = new StateServices().GetStates() ?? new List<StateObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Helpers
        private List<CityObject> GetCities(int? itemsPerPage, int? pageNumber)
        {
            return new CityServices().GetCityObjects(itemsPerPage, pageNumber) ?? new List<CityObject>();
        }

        private GenericValidator ValidateCity(CityObject city)
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

            if (city.StateId < 1)
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
