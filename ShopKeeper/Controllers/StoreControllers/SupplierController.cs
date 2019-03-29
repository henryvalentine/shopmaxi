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
	public class SupplierController : Controller
	{
		public SupplierController ()
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
        public ActionResult GetSupplierObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SupplierObject> filteredSupplierObjects;
                var countG = new SupplierServices().GetObjectCount();

                var pagedSupplierObjects = GetSuppliers(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSupplierObjects = new SupplierServices().Search(param.sSearch);
                }
                else
                {
                    filteredSupplierObjects = pagedSupplierObjects;
                }

                if (!filteredSupplierObjects.Any())
                {
                    return Json(new List<SupplierObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SupplierObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.CompanyName :  c.TIN);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSupplierObjects = sortDirection == "asc" ? filteredSupplierObjects.OrderBy(orderingFunction) : filteredSupplierObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredSupplierObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.SupplierId), c.CompanyName, c.TIN, c.DateRegistered };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredSupplierObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SupplierObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddSupplier()
        {
           return View(new SupplierObject());
        }
        
        [HttpPost]
        public ActionResult AddSupplier(SupplierObject supplier)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateSupplier(supplier);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    supplier.DateJoined = DateTime.Now;
                    var k = new SupplierServices().AddSupplier(supplier);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
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
        public ActionResult EditSupplier(SupplierObject supplier)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateSupplier(supplier);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_supplier"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldSupplier = Session["_supplier"] as SupplierObject;
                    
                    if (oldSupplier == null || oldSupplier.SupplierId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldSupplier.CompanyName = supplier.CompanyName.Trim();
                    if (!string.IsNullOrEmpty(oldSupplier.Note))
                    {
                        oldSupplier.Note = supplier.Note.Trim();
                    }
                    var k = new SupplierServices().UpdateSupplier(oldSupplier);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Insertion_Success;
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
        public ActionResult DeleteSupplier(long id)
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
                    
                var k = new SupplierServices().DeleteSupplier(id);
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
        public ActionResult GetSupplier(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new SupplierObject(), JsonRequestBehavior.AllowGet);
                }

                var supplier = new SupplierServices().GetSupplier(id);
                if (id < 1)
                {
                    return Json(new SupplierObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_supplier"] = supplier;
                return Json(supplier, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new SupplierObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<SupplierObject> GetSuppliers(int? itemsPerPage, int? pageNumber)
        {
            return new SupplierServices().GetSupplierObjects(itemsPerPage, pageNumber) ?? new List<SupplierObject>();
        }
        
        private GenericValidator ValidateSupplier(SupplierObject supplier)
        {
            var gVal = new GenericValidator();
            if (supplier == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(supplier.CompanyName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Supplier_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
