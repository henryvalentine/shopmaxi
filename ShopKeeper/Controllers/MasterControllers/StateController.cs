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
	public class StateController : Controller
	{
        public StateController()
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
                IEnumerable<StateObject> filteredStateObjects;
                var countG = new StateServices().GetObjectCount();

                var pagedStateObjects = GetStates(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStateObjects = new StateServices().Search(param.sSearch);
                }
                else
                {
                    filteredStateObjects = pagedStateObjects;
                }

                if (!filteredStateObjects.Any())
                {
                    return Json(new List<StateObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StateObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.CountryName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStateObjects = sortDirection == "asc" ? filteredStateObjects.OrderBy(orderingFunction) : filteredStateObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStateObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StateId), c.Name, c.CountryName};
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
                return Json(new List<StateObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddState()
        {
           return View(new StateObject());
        }
        
        [HttpPost]
        public ActionResult AddState(StateObject state)
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
                    
                    var k = new StateServices().AddState(state);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = k;
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
        public ActionResult EditState(StateObject state)
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
                   
                    var oldState = Session["_state"] as StateObject;
                    if (oldState == null || oldState.StateId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldState.Name = state.Name.Trim();
                    oldState.CountryId = state.CountryId;
                    var k = new StateServices().UpdateState(oldState);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = 0;
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
                    
                var k = new StateServices().DeleteState(id);
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
                    return Json(new StateObject(), JsonRequestBehavior.AllowGet);
                }

                var state = new StateServices().GetState(id);
                if (id < 1)
                {
                    return Json(new StateObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_state"] = state;
                return Json(state, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new StateObject(), JsonRequestBehavior.AllowGet);
                

            }
        }
        public ActionResult GetCountries()
        {
            var countries = new CountryServices().GetCountries() ?? new List<CountryObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<StateObject> GetStates(int? itemsPerPage, int? pageNumber)
        {
            return new StateServices().GetStateObjects(itemsPerPage, pageNumber) ?? new List<StateObject>();
        }
       
       private GenericValidator ValidateState(StateObject state)
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
                gVal.Error = message_Feedback.State_Name_Error;
                return gVal;
            }

            if (state.CountryId < 1)
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
