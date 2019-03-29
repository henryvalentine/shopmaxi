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
	public class CurrencyController : Controller
	{
        public CurrencyController()
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
        public ActionResult GetCurrencyObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<CurrencyObject> filteredCurrencyObjects;
                var countG = new CurrencyServices().GetObjectCount();

                var pagedCurrencyObjects = GetCities(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCurrencyObjects = new CurrencyServices().Search(param.sSearch);
                }
                else
                {
                    filteredCurrencyObjects = pagedCurrencyObjects;
                }

                if (!filteredCurrencyObjects.Any())
                {
                    return Json(new List<CurrencyObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<CurrencyObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : sortColumnIndex == 2 ? c.Symbol : c.CountryName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredCurrencyObjects = sortDirection == "asc" ? filteredCurrencyObjects.OrderBy(orderingFunction) : filteredCurrencyObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredCurrencyObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.CurrencyId), c.Name, c.Symbol, c.CountryName};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredCurrencyObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CurrencyObject>(), JsonRequestBehavior.AllowGet);
            }
        }

	    public ActionResult AddCurrency()
        {
           return View(new CurrencyObject());
        }
        
        [HttpPost]
        public ActionResult AddCurrency(CurrencyObject currency)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateCurrency(currency);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new CurrencyServices().AddCurrency(currency);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
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
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditCurrency(CurrencyObject currency)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateCurrency(currency);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_currency"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldCurrency = Session["_currency"] as CurrencyObject;
                    if (oldCurrency == null || oldCurrency.CurrencyId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldCurrency.Name = currency.Name.Trim();
                    oldCurrency.Symbol = currency.Symbol.Trim();
                    oldCurrency.CountryId = currency.CountryId;
                    var k = new CurrencyServices().UpdateCurrency(oldCurrency);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Update_Success;
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
        public ActionResult DeleteCurrency(long id)
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
                    
                var k = new CurrencyServices().DeleteCurrency(id);
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
        public ActionResult GetCurrency(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new CurrencyObject(), JsonRequestBehavior.AllowGet);
                }

                var currency = new CurrencyServices().GetCurrency(id);
                if (id < 1)
                {
                    return Json(new CurrencyObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_currency"] = currency;
                return Json(currency, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new CurrencyObject(), JsonRequestBehavior.AllowGet);
                

            }
        }
        public ActionResult GetCountries()
        {
            var countries = new CountryServices().GetCountries() ?? new List<CountryObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<CurrencyObject> GetCities(int? itemsPerPage, int? pageNumber)
        {
            return new CurrencyServices().GetCurrencyObjects(itemsPerPage, pageNumber) ?? new List<CurrencyObject>();
        }
       
        private GenericValidator ValidateCurrency(CurrencyObject currency)
        {
            var gVal = new GenericValidator();
            if (currency == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(currency.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Currency_Name_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(currency.Symbol))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Currency_Symbol_Error;
                return gVal;
            }
            if (currency.CountryId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Country_Selection_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
