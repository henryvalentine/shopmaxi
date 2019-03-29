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
	public class UnitOfMeasurementController : Controller
	{
        public UnitOfMeasurementController()
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
        public ActionResult GetUnitOfMeasurementObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<UnitsOfMeasurementObject> filteredUnitsOfMeasurementObjects;
                var countG = new UnitOfMeasurementServices().GetObjectCount();

                var pagedUnitsOfMeasurementObjects = GetUnitofMeasurements(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredUnitsOfMeasurementObjects = new UnitOfMeasurementServices().Search(param.sSearch);
                }
                else
                {
                    filteredUnitsOfMeasurementObjects = pagedUnitsOfMeasurementObjects;
                }

                if (!filteredUnitsOfMeasurementObjects.Any())
                {
                    return Json(new List<UnitsOfMeasurementObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<UnitsOfMeasurementObject, string> orderingFunction = (c =>  c.UoMCode
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredUnitsOfMeasurementObjects = sortDirection == "asc" ? filteredUnitsOfMeasurementObjects.OrderBy(orderingFunction) : filteredUnitsOfMeasurementObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredUnitsOfMeasurementObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.UnitOfMeasurementId), c.UoMCode};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredUnitsOfMeasurementObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<UnitsOfMeasurementObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
	    public ActionResult AddUnitOfMeasurement()
        {
           return View(new UnitsOfMeasurementObject());
        }
        
        [HttpPost]
        public ActionResult AddUnitOfMeasurement(UnitsOfMeasurementObject unitofMeasurement)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateUnitofMeasurement(unitofMeasurement);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new UnitOfMeasurementServices().AddUnitOfMeasurement(unitofMeasurement);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
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
        public ActionResult EditUnitofMeasurement(UnitsOfMeasurementObject unitofMeasurement)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateUnitofMeasurement(unitofMeasurement);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_unitofMeasurement"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldUnitofMeasurement = Session["_unitofMeasurement"] as UnitsOfMeasurementObject;
                    if (oldUnitofMeasurement == null || oldUnitofMeasurement.UnitOfMeasurementId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldUnitofMeasurement.UoMCode = unitofMeasurement.UoMCode.Trim();

                    if (!string.IsNullOrEmpty(unitofMeasurement.UoMDescription))
                    {
                        oldUnitofMeasurement.UoMDescription = unitofMeasurement.UoMDescription;
                    }

                    var k = new UnitOfMeasurementServices().UpdateUnitOfMeasurement(oldUnitofMeasurement);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Update_Success;
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
        public ActionResult DeleteUnitOfMeasurement(long id)
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
                    
                var k = new UnitOfMeasurementServices().DeleteUnitOfMeasurement(id);
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
        public ActionResult GetUnitofMeasurement(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new UnitsOfMeasurementObject(), JsonRequestBehavior.AllowGet);
                }

                var unitofMeasurement = new UnitOfMeasurementServices().GetUnitsOfMeasurement(id);
                if (id < 1)
                {
                    return Json(new UnitsOfMeasurementObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_unitofMeasurement"] = unitofMeasurement;
                return Json(unitofMeasurement, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new UnitsOfMeasurementObject(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
       

        #region Helpers
        private List<UnitsOfMeasurementObject> GetUnitofMeasurements(int? itemsPerPage, int? pageNumber)
        {
            return new UnitOfMeasurementServices().GetUnitsOfMeasurementObjects(itemsPerPage, pageNumber) ?? new List<UnitsOfMeasurementObject>();
        }
        
        private GenericValidator ValidateUnitofMeasurement(UnitsOfMeasurementObject unitofMeasurement)
        {
            var gVal = new GenericValidator();
            if (unitofMeasurement == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(unitofMeasurement.UoMCode))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UoM_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
