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
	public class TransactionTypeController : Controller
	{
		public TransactionTypeController ()
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
        public ActionResult GetTransactionTypeObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<TransactionTypeObject> filteredTransactionTypeObjects;
                var countG = new TransactionTypeServices().GetObjectCount();

                var pagedTransactionTypeObjects = GetTransactionTypes(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredTransactionTypeObjects = new TransactionTypeServices().Search(param.sSearch);
                }
                else
                {
                    filteredTransactionTypeObjects = pagedTransactionTypeObjects;
                }

                if (!filteredTransactionTypeObjects.Any())
                {
                    return Json(new List<TransactionTypeObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<TransactionTypeObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.Action
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredTransactionTypeObjects = sortDirection == "asc" ? filteredTransactionTypeObjects.OrderBy(orderingFunction) : filteredTransactionTypeObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredTransactionTypeObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.TransactionTypeId), c.Name, c.Action };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredTransactionTypeObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<TransactionTypeObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
	    public ActionResult AddTransactionType()
        {
           return View(new TransactionTypeObject());
        }
        
        [HttpPost]
        public ActionResult AddTransactionType(TransactionTypeObject transactionType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateTransactionType(transactionType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new TransactionTypeServices().AddTransactionType(transactionType);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? "Transaction Type information already exists" : "Transaction Type information could not be added. Please try again";
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
        public ActionResult EditTransactionType(TransactionTypeObject transactionType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateTransactionType(transactionType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_transactionType"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldTransactionType = Session["_transactionType"] as TransactionTypeObject;
                    if (oldTransactionType == null || oldTransactionType.TransactionTypeId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldTransactionType.Name = transactionType.Name.Trim();
                    oldTransactionType.Action = transactionType.Action.Trim();

                    if (!string.IsNullOrEmpty(transactionType.Description))
                    {
                        oldTransactionType.Description = transactionType.Description;
                    }

                    var k = new TransactionTypeServices().UpdateTransactionType(oldTransactionType);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? "Transaction Type information already exists" : "Transaction Type information could not be updated. Please try again";
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
        public ActionResult DeleteTransactionType(long id)
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
                    
                var k = new TransactionTypeServices().DeleteTransactionType(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = "Transaction Type information was successfully deleted.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = "Transaction Type information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = "Transaction Type information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetTransactionType(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new TransactionTypeObject(), JsonRequestBehavior.AllowGet);
                }

                var transactionType = new TransactionTypeServices().GetTransactionType(id);
                if (id < 1)
                {
                    return Json(new TransactionTypeObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_transactionType"] = transactionType;
                return Json(transactionType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new TransactionTypeObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<TransactionTypeObject> GetTransactionTypes(int? itemsPerPage, int? pageNumber)
        {
            return new TransactionTypeServices().GetTransactionTypeObjects(itemsPerPage, pageNumber) ?? new List<TransactionTypeObject>();
        }
        
        private GenericValidator ValidateTransactionType(TransactionTypeObject transactionType)
        {
            var gVal = new GenericValidator();
            if (transactionType == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(transactionType.Name))
            {
                gVal.Code = -1;
                gVal.Error = "Please provide Transaction Type name.";
                return gVal;
            }
            if (string.IsNullOrEmpty(transactionType.Action))
            {
                gVal.Code = -1;
                gVal.Error = "Please provide the Action for this Transaction Type.";
                return gVal;
            }
            gVal.Code = 5;
            gVal.Error = "";
            return gVal;
        }
        #endregion

    }
}
