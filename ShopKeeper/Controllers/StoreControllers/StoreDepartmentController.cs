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
	public class StoreDepartmentController : Controller
	{
		public StoreDepartmentController ()
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
        public ActionResult GetStoreDepartmentObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreDepartmentObject> filteredStoreDepartmentObjects;
                var countG = new StoreDepartmentServices().GetObjectCount();

                var pagedStoreDepartmentObjects = GetStoreDepartments(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreDepartmentObjects = new StoreDepartmentServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreDepartmentObjects = pagedStoreDepartmentObjects;
                }

                if (!filteredStoreDepartmentObjects.Any())
                {
                    return Json(new List<StoreDepartmentObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreDepartmentObject, string> orderingFunction = (c =>  c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreDepartmentObjects = sortDirection == "asc" ? filteredStoreDepartmentObjects.OrderBy(orderingFunction) : filteredStoreDepartmentObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreDepartmentObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreDepartmentId), c.Name};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreDepartmentObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreDepartmentObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddStoreDepartment()
        {
           return View(new StoreDepartmentObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreDepartment(StoreDepartmentObject storeDepartment)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreDepartment(storeDepartment);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new StoreDepartmentServices().AddStoreDepartment(storeDepartment);
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
        public ActionResult EditStoreDepartment(StoreDepartmentObject storeDepartment)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreDepartment(storeDepartment);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeDepartment"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreDepartment = Session["_storeDepartment"] as StoreDepartmentObject;
                    if (oldStoreDepartment == null || oldStoreDepartment.StoreDepartmentId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldStoreDepartment.Name = storeDepartment.Name.Trim();
                    if (!string.IsNullOrEmpty(storeDepartment.Description))
                    {
                        oldStoreDepartment.Description = storeDepartment.Description.Trim();
                    }
                    var k = new StoreDepartmentServices().UpdateStoreDepartment(oldStoreDepartment);
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
        public ActionResult DeleteStoreDepartment(long id)
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
                    
                var k = new StoreDepartmentServices().DeleteStoreDepartment(id);
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
        public ActionResult GetStoreDepartment(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreDepartmentObject(), JsonRequestBehavior.AllowGet);
                }

                var storeDepartment = new StoreDepartmentServices().GetStoreDepartment(id);
                if (id < 1)
                {
                    return Json(new StoreDepartmentObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeDepartment"] = storeDepartment;
                return Json(storeDepartment, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StoreDepartmentObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<StoreDepartmentObject> GetStoreDepartments(int? itemsPerPage, int? pageNumber)
        {
            return new StoreDepartmentServices().GetStoreDepartmentObjects(itemsPerPage, pageNumber) ?? new List<StoreDepartmentObject>();
        }
        
        private GenericValidator ValidateStoreDepartment(StoreDepartmentObject storeDepartment)
        {
            var gVal = new GenericValidator();
            if (storeDepartment == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeDepartment.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Delivery_Method_Title_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
