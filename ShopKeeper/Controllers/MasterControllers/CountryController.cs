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
	public class CountryController : Controller
	{
        public CountryController()
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
        public ActionResult GetCountryObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<CountryObject> filteredCountryObjects;
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
                    return Json(new List<CountryObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<CountryObject, string> orderingFunction = (c => c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredCountryObjects = sortDirection == "asc" ? filteredCountryObjects.OrderBy(orderingFunction) : filteredCountryObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredCountryObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.CountryId), c.Name };
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
                return Json(new List<CountryObject>(), JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AddCountry()
        {
            return View(new CountryObject());
        }

        [HttpPost]
        public ActionResult AddCountry(CountryObject country)
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
        public ActionResult EditCountry(CountryObject country)
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

                    var oldCountry = Session["_country"] as CountryObject;
                    if (oldCountry == null || oldCountry.CountryId < 1)
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
                    return Json(new CountryObject(), JsonRequestBehavior.AllowGet);
                }

                var country = new CountryServices().GetCountry(id);
                if (id < 1)
                {
                    return Json(new CountryObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_country"] = country;
                return Json(country, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new CountryObject(), JsonRequestBehavior.AllowGet);


            }
        }

        #endregion


        #region Helpers
        private List<CountryObject> GetCountrys(int? itemsPerPage, int? pageNumber)
        {
            return new CountryServices().GetCountryObjects(itemsPerPage, pageNumber) ?? new List<CountryObject>();
        }

        private GenericValidator ValidateCountry(CountryObject country)
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
