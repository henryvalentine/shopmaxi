using System;
using System.Collections.Generic;
using System.Globalization;
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
	public class TransactionController : Controller
	{
        public TransactionController()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult Transactions()
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
        public ActionResult GetTransactionObjects(JQueryDataTableParamModel param)
        {
            try
            {
                var countG = new TransactionServices().GetObjectCount();

                var pagedTransactionObjects = GetTransactions(param.iDisplayLength, param.iDisplayStart);

                //if (!string.IsNullOrEmpty(param.sSearch))
                //{
                    //filteredTransactionObjects = new TransactionServices().Search(param.sSearch);
                //}
                //else
                //{
                    IEnumerable<TransactionObject> filteredTransactionObjects = pagedTransactionObjects;
                //}

                if (!filteredTransactionObjects.Any())
                {
                    return Json(new List<TransactionObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<TransactionObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.TransactionTypeName : 
                    sortColumnIndex == 2 ? c.PaymentMethodName : c.TransactionDate.ToString("dd/mm/yyyy hh:mm:ss tt")
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredTransactionObjects = sortDirection == "asc" ? filteredTransactionObjects.OrderBy(orderingFunction) : filteredTransactionObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredTransactionObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.TransactionId), c.TransactionTypeName, c.PaymentMethodName, c.Amount.ToString(CultureInfo.InvariantCulture), c.TransactionDate.ToString("dd/mm/yyyy hh:mm:ss tt")};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredTransactionObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<TransactionObject>(), JsonRequestBehavior.AllowGet);
            }
        }

	    public ActionResult AddTransaction()
        {
           return View(new TransactionObject());
        }
        
        [HttpPost]
        public ActionResult AddTransaction(TransactionObject transaction)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateTransaction(transaction);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    transaction.TransactionDate = DateTime.Now;
                    var k = new TransactionServices().AddTransaction(transaction);
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
        public ActionResult EditTransaction(TransactionObject transaction)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateTransaction(transaction);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_transaction"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldTransaction = Session["_transaction"] as TransactionObject;
                    if (oldTransaction == null || oldTransaction.TransactionId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldTransaction.TransactionTypeId = transaction.TransactionTypeId;
                    oldTransaction.PaymentMethodId = transaction.PaymentMethodId;
                    oldTransaction.Amount = transaction.Amount;
                    if (!string.IsNullOrEmpty(transaction.Remark))
                    {
                        oldTransaction.Remark = transaction.Remark.Trim();
                    }
                    var k = new TransactionServices().UpdateTransaction(oldTransaction);
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
        public ActionResult DeleteTransaction(long id)
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
                    
                var k = new TransactionServices().DeleteTransaction(id);
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
        public ActionResult GetTransaction(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new TransactionObject(), JsonRequestBehavior.AllowGet);
                }

                var transaction = new TransactionServices().GetTransaction(id);
                if (id < 1)
                {
                    return Json(new TransactionObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_transaction"] = transaction;
                return Json(transaction, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new TransactionObject(), JsonRequestBehavior.AllowGet);
                

            }
        }
        public ActionResult GetCountries()
        {
            var countries = new CountryServices().GetCountries() ?? new List<CountryObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<TransactionObject> GetTransactions(int? itemsPerPage, int? pageNumber)
        {
            return new TransactionServices().GetTransactionObjects(itemsPerPage, pageNumber) ?? new List<TransactionObject>();
        }
       
        private GenericValidator ValidateTransaction(TransactionObject transaction)
        {
            var gVal = new GenericValidator();
            if (transaction == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (transaction.Amount < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Transaction_Amount_Eror;
                return gVal;
            }
            if (transaction.TransactionTypeId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Transaction_Type_Selection_Error;
                return gVal;
            }
            if (transaction.PaymentMethodId < 1)
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
