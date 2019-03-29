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
	public class SalutationController : Controller
	{
        public SalutationController()
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
        public ActionResult GetSalutationObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SalutationObject> filteredSalutationObjects;
                var countG = new SalutationServices().GetObjectCount();

                var pagedSalutationObjects = GetSalutations(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSalutationObjects = new SalutationServices().Search(param.sSearch);
                }
                else
                {
                    filteredSalutationObjects = pagedSalutationObjects;
                }

                if (!filteredSalutationObjects.Any())
                {
                    return Json(new List<SalutationObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SalutationObject, string> orderingFunction = (c =>  c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSalutationObjects = sortDirection == "asc" ? filteredSalutationObjects.OrderBy(orderingFunction) : filteredSalutationObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredSalutationObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.SalutationId), c.Name};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredSalutationObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SalutationObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RefreshSession()
        {
            try
            {
                return Json(5, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddSalutation()
        {
           return View(new SalutationObject());
        }
        
        [HttpPost]
        public ActionResult AddSalutation(SalutationObject salutation)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateSalutation(salutation);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new SalutationServices().AddSalutation(salutation);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
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
                gVal.Code = 0;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditSalutation(SalutationObject salutation)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateSalutation(salutation);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_salutation"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldSalutation = Session["_salutation"] as SalutationObject;
                    if (oldSalutation == null || oldSalutation.SalutationId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldSalutation.Name = salutation.Name.Trim();
                    var k = new SalutationServices().UpdateSalutation(oldSalutation);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = 0;
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
                gVal.Code = 0;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteSalutation(long id)
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
                    
                var k = new SalutationServices().DeleteSalutation(id);
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
        public ActionResult GetSalutation(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new SalutationObject(), JsonRequestBehavior.AllowGet);
                }

                var salutation = new SalutationServices().GetSalutation(id);
                if (id < 1)
                {
                    return Json(new SalutationObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_salutation"] = salutation;
                return Json(salutation, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new SalutationObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<SalutationObject> GetSalutations(int? itemsPerPage, int? pageNumber)
        {
            return new SalutationServices().GetSalutationObjects(itemsPerPage, pageNumber) ?? new List<SalutationObject>();
        }
        
        private GenericValidator ValidateSalutation(SalutationObject salutation)
        {
            var gVal = new GenericValidator();
            if (salutation == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(salutation.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Salutation_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
