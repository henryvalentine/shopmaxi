using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.GenericHelpers;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class StoreStateController : Controller
	{
        public StoreStateController()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult States()
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
        public ActionResult GetStateObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreStateObject> filteredStateObjects;
                var countG = new StoreStateServices().GetObjectCount();

                var pagedStateObjects = GetStates(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStateObjects = new StoreStateServices().Search(param.sSearch);
                }
                else
                {
                    filteredStateObjects = pagedStateObjects;
                }

                if (!filteredStateObjects.Any())
                {
                    return Json(new List<StoreStateObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreStateObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.CountryName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStateObjects = sortDirection == "asc" ? filteredStateObjects.OrderBy(orderingFunction) : filteredStateObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStateObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreStateId), c.Name, c.CountryName};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStateObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreStateObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddState()
        {
           return View(new StoreStateObject());
        }
        
        [HttpPost]
        public ActionResult AddState(StoreStateObject state)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateState(state);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    
                    var k = new StoreStateServices().AddStoreState(state);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
                        return Json(state, JsonRequestBehavior.AllowGet);
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
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditState(StoreStateObject state)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateState(state);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_state"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldState = Session["_state"] as StoreStateObject;
                    if (oldState == null || oldState.StoreStateId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldState.Name = state.Name.Trim();
                    oldState.StoreCountryId = state.StoreCountryId;
                    var k = new StoreStateServices().UpdateStoreState(oldState);
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
        public ActionResult DeleteState(long id)
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
                    
                var k = new StoreStateServices().DeleteStoreState(id);
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
        public ActionResult GetState(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreStateObject(), JsonRequestBehavior.AllowGet);
                }

                var state = new StoreStateServices().GetStoreState(id);
                if (id < 1)
                {
                    return Json(new StoreStateObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_state"] = state;
                return Json(state, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StoreStateObject(), JsonRequestBehavior.AllowGet);
                

            }
        }
        public ActionResult GetCountries()
        {
            var countries = new CountryServices().GetCountries() ?? new List<StoreCountryObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<StoreStateObject> GetStates(int? itemsPerPage, int? pageNumber)
        {
            return new StoreStateServices().GetStoreStateObjects(itemsPerPage, pageNumber) ?? new List<StoreStateObject>();
        }
       
        private GenericValidator ValidateState(StoreStateObject state)
        {
            var gVal = new GenericValidator();
            if (state == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(state.Name))
            {
                gVal.Code = -1;
                gVal.Error = "Please provide State name.";
                return gVal;
            }

            if (state.StoreCountryId < 1)
            {
                gVal.Code = -1;
                gVal.Error = "Please select a Country.";
                return gVal;
            }
           
            gVal.Code = 5;
            gVal.Error = "";
            return gVal;
        }
        #endregion

    }
}
