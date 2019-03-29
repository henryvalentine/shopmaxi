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
	public class StoreCurrencyController : Controller
	{
        public StoreCurrencyController()
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
        public ActionResult GetStoreCurrencyObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreCurrencyObject> filteredStoreCurrencyObjects;
                var countG = new StoreCurrencyServices().GetObjectCount();

                var pagedStoreCurrencyObjects = GetCurrencies(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreCurrencyObjects = new StoreCurrencyServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreCurrencyObjects = pagedStoreCurrencyObjects;
                }

                if (!filteredStoreCurrencyObjects.Any())
                {
                    return Json(new List<StoreCurrencyObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreCurrencyObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : sortColumnIndex == 2 ? c.Symbol : c.CountryName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreCurrencyObjects = sortDirection == "asc" ? filteredStoreCurrencyObjects.OrderBy(orderingFunction) : filteredStoreCurrencyObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreCurrencyObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreCurrencyId), c.Name, c.Symbol, c.CountryName};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreCurrencyObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreCurrencyObject>(), JsonRequestBehavior.AllowGet);
            }
        }

	    public ActionResult AddStoreCurrency()
        {
           return View(new StoreCurrencyObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreCurrency(StoreCurrencyObject storeCurrency)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreCurrency(storeCurrency);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new StoreCurrencyServices().AddStoreCurrency(storeCurrency);
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
        public ActionResult EditStoreCurrency(StoreCurrencyObject storeCurrency)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreCurrency(storeCurrency);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeCurrency"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreCurrency = Session["_storeCurrency"] as StoreCurrencyObject;
                    if (oldStoreCurrency == null || oldStoreCurrency.StoreCurrencyId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreCurrency.Name = storeCurrency.Name.Trim();
                    oldStoreCurrency.IsDefaultCurrency = storeCurrency.IsDefaultCurrency;
                    oldStoreCurrency.Symbol = storeCurrency.Symbol.Trim();
                    oldStoreCurrency.StoreCountryId = storeCurrency.StoreCountryId;
                    var k = new StoreCurrencyServices().UpdateStoreCurrency(oldStoreCurrency);
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
        public ActionResult DeleteStoreCurrency(long id)
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
                    
                var k = new StoreCurrencyServices().DeleteStoreCurrency(id);
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
        public ActionResult GetStoreCurrency(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreCurrencyObject(), JsonRequestBehavior.AllowGet);
                }

                var storeCurrency = new StoreCurrencyServices().GetStoreCurrency(id);
                if (id < 1)
                {
                    return Json(new StoreCurrencyObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeCurrency"] = storeCurrency;
                return Json(storeCurrency, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StoreCurrencyObject(), JsonRequestBehavior.AllowGet);
                

            }
        }
        public ActionResult GetCountries()
        {
            var countries = new CountryServices().GetCountries() ?? new List<StoreCountryObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<StoreCurrencyObject> GetCurrencies(int? itemsPerPage, int? pageNumber)
        {
            return new StoreCurrencyServices().GetStoreCurrencyObjects(itemsPerPage, pageNumber) ?? new List<StoreCurrencyObject>();
        }
       
        private GenericValidator ValidateStoreCurrency(StoreCurrencyObject storeCurrency)
        {
            var gVal = new GenericValidator();
            if (storeCurrency == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeCurrency.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Currency_Name_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeCurrency.Symbol))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Currency_Symbol_Error;
                return gVal;
            }
            if (storeCurrency.StoreCountryId < 1)
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
