using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class StoreTransactionController : Controller
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
        public ActionResult GetStoreTransactionObjects(JQueryDataTableParamModel param)
        {
            try
            {
                var countG = new StoreTransactionServices().GetObjectCount();

                var pagedStoreTransactionObjects = GetStoreTransactions(param.iDisplayLength, param.iDisplayStart);

                //if (!string.IsNullOrEmpty(param.sSearch))
                //{
                //filteredStoreTransactionObjects = new StoreTransactionServices().Search(param.sSearch);
                //}
                //else
                //{
                IEnumerable<StoreTransactionObject> filteredStoreTransactionObjects = pagedStoreTransactionObjects;
                //}

                if (!filteredStoreTransactionObjects.Any())
                {
                    return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreTransactionObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreTransactionTypeName :
                    sortColumnIndex == 2 ? c.PaymentMethodName : c.TransactionDate.ToString("dd/mm/yyyy hh:mm:ss tt")
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreTransactionObjects = sortDirection == "asc" ? filteredStoreTransactionObjects.OrderBy(orderingFunction) : filteredStoreTransactionObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreTransactionObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreTransactionId), c.StoreTransactionTypeName, c.PaymentMethodName, c.TransactionAmount.ToString(CultureInfo.InvariantCulture), c.TransactionDate.ToString("dd/mm/yyyy hh:mm:ss tt") };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreTransactionObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AddStoreTransaction(StoreTransactionObject storeTransaction)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreTransaction(storeTransaction);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    storeTransaction.TransactionDate = DateTime.Now;
                    var k = new StoreTransactionServices().AddStoreTransaction(storeTransaction);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
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
        public ActionResult AddSaleTransaction(StoreTransactionObject storeTransaction)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreTransaction(storeTransaction);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    storeTransaction.TransactionDate = DateTime.Now;
                    var k = new StoreTransactionServices().AddStoreTransaction(storeTransaction);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
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
        public ActionResult EditStoreTransaction(StoreTransactionObject storeTransaction)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreTransaction(storeTransaction);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeTransaction"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldStoreTransaction = Session["_storeTransaction"] as StoreTransactionObject;
                    if (oldStoreTransaction == null || oldStoreTransaction.StoreTransactionId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreTransaction.StoreTransactionTypeId = storeTransaction.StoreTransactionTypeId;
                    oldStoreTransaction.StorePaymentMethodId = storeTransaction.StorePaymentMethodId;
                    oldStoreTransaction.TransactionAmount = storeTransaction.TransactionAmount;
                    if (!string.IsNullOrEmpty(storeTransaction.Remark))
                    {
                        oldStoreTransaction.Remark = storeTransaction.Remark.Trim();
                    }
                    var k = new StoreTransactionServices().UpdateStoreTransaction(oldStoreTransaction);
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
        public ActionResult DeleteStoreTransaction(long id)
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

                var k = new StoreTransactionServices().DeleteStoreTransaction(id);
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
        public ActionResult GetStoreTransaction(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreTransactionObject(), JsonRequestBehavior.AllowGet);
                }

                var storeTransaction = new StoreTransactionServices().GetStoreTransaction(id);
                if (id < 1)
                {
                    return Json(new StoreTransactionObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeTransaction"] = storeTransaction;
                return Json(storeTransaction, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new StoreTransactionObject(), JsonRequestBehavior.AllowGet);


            }
        }
        public ActionResult GetCountries()
        {
            var countries = new CountryServices().GetCountries() ?? new List<StoreCountryObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Helpers
        private List<StoreTransactionObject> GetStoreTransactions(int? itemsPerPage, int? pageNumber)
        {
            return new StoreTransactionServices().GetStoreTransactionObjects(itemsPerPage, pageNumber) ?? new List<StoreTransactionObject>();
        }

        private GenericValidator ValidateStoreTransaction(StoreTransactionObject storeTransaction)
        {
            var gVal = new GenericValidator();
            if (storeTransaction == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (storeTransaction.TransactionAmount < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Transaction_Amount_Eror;
                return gVal;
            }
            if (storeTransaction.StoreTransactionTypeId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Transaction_Type_Selection_Error;
                return gVal;
            }
            if (storeTransaction.StorePaymentMethodId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Payment_Method_Selection_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
