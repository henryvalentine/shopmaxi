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
	public class StoreCountryController : Controller
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
        public ActionResult GetCountryObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreCountryObject> filteredCountryObjects;
                var countG = new CountryServices().GetObjectCount();

                var pagedCountryObjects = GetCountrys(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCountryObjects = new CountryServices().Search(param.sSearch);
                }
                else
                {
                    filteredCountryObjects = pagedCountryObjects;
                }

                if (!filteredCountryObjects.Any())
                {
                    return Json(new List<StoreCountryObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreCountryObject, string> orderingFunction = (c => c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredCountryObjects = sortDirection == "asc" ? filteredCountryObjects.OrderBy(orderingFunction) : filteredCountryObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredCountryObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreCountryId), c.Name };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredCountryObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreCountryObject>(), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AddCountry(StoreCountryObject country)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateCountry(country);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new CountryServices().AddCountry(country);
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
        public ActionResult EditCountry(StoreCountryObject country)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateCountry(country);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_country"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldCountry = Session["_country"] as StoreCountryObject;
                    if (oldCountry == null || oldCountry.StoreCountryId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldCountry.Name = country.Name.Trim();
                    var k = new CountryServices().UpdateCountry(oldCountry);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = k;
                    gVal.Error = message_Feedback.Model_State_Error;
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
        public ActionResult DeleteCountry(long id)
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

                var k = new CountryServices().DeleteCountry(id);
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
        public ActionResult GetCountry(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreCountryObject(), JsonRequestBehavior.AllowGet);
                }

                var country = new CountryServices().GetCountry(id);
                if (id < 1)
                {
                    return Json(new StoreCountryObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_country"] = country;
                return Json(country, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new StoreCountryObject(), JsonRequestBehavior.AllowGet);


            }
        }

        #endregion


        #region Helpers
        private List<StoreCountryObject> GetCountrys(int? itemsPerPage, int? pageNumber)
        {
            return new CountryServices().GetCountryObjects(itemsPerPage, pageNumber) ?? new List<StoreCountryObject>();
        }

        private GenericValidator ValidateCountry(StoreCountryObject country)
        {
            var gVal = new GenericValidator();
            if (country == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(country.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Country_Name_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        #endregion
    }
}
