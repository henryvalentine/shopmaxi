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
	public class RegisterController : Controller
	{
        public RegisterController()
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
        public ActionResult GetRegisterObjects(JQueryDataTableParamModel param)
        {
            var gVal = new GenericValidator();
            try
            {
                IEnumerable<RegisterObject> filteredRegisterObjects;
                var countG = new RegisterServices().GetObjectCount();

                var pagedRegisterObjects = GetRegisters(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredRegisterObjects = new RegisterServices().Search(param.sSearch);
                }
                else
                {
                    filteredRegisterObjects = pagedRegisterObjects;
                }

                if (!filteredRegisterObjects.Any())
                {
                    return Json(new List<RegisterObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<RegisterObject, string> orderingFunction = (c => sortColumnIndex == 1 ?  c.Name: c.OutletName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredRegisterObjects = sortDirection == "asc" ? filteredRegisterObjects.OrderBy(orderingFunction) : filteredRegisterObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredRegisterObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.RegisterId), c.Name, c.OutletName};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredRegisterObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<RegisterObject>(), JsonRequestBehavior.AllowGet);
            }
        }


	    public ActionResult AddRegister()
        {
           return View(new RegisterObject());
        }
        
        [HttpPost]
        public ActionResult AddRegister(RegisterObject register)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateRegister(register);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new RegisterServices().AddRegister(register);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
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
        public ActionResult EditRegister(RegisterObject register)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateRegister(register);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_register"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldRegister = Session["_register"] as RegisterObject;
                    if (oldRegister == null || oldRegister.RegisterId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldRegister.Name = register.Name.Trim();
                    //todo: OutletId should be dynamic
                    oldRegister.CurrentOutletId = register.CurrentOutletId;

                    var k = new RegisterServices().UpdateRegister(oldRegister);
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
        public ActionResult DeleteRegister(long id)
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
                    
                var k = new RegisterServices().DeleteRegister(id);
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
        public ActionResult GetRegister(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new RegisterObject(), JsonRequestBehavior.AllowGet);
                }

                var register = new RegisterServices().GetRegister(id);
                if (id < 1)
                {
                    return Json(new RegisterObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_register"] = register;
                return Json(register, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new RegisterObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        public ActionResult GetStoreOutlets(string subdomain)
        {
           var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            return Json(new StoreOutletServices().GetStoreOutlets(), JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<RegisterObject> GetRegisters(int? itemsPerPage, int? pageNumber)
        {
            return new RegisterServices().GetRegisterObjects(itemsPerPage, pageNumber) ?? new List<RegisterObject>();
        }
        
        private GenericValidator ValidateRegister(RegisterObject register)
        {
            var gVal = new GenericValidator();
            if (register == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(register.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Register_Name_Error;
                return gVal;
            }

            if (register.CurrentOutletId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Outlet_Selection_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
