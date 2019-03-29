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
	public class DeliveryMethodController : Controller
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
        public ActionResult GetDeliveryMethodObjects(JQueryDataTableParamModel param, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<DeliveryMethodObject>(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                IEnumerable<DeliveryMethodObject> filteredDeliveryMethodObjects;
                var countG = new DeliveryMethodServices().GetObjectCount();

                var pagedDeliveryMethodObjects = GetDeliveryMethods(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredDeliveryMethodObjects = new DeliveryMethodServices().Search(param.sSearch);
                }
                else
                {
                    filteredDeliveryMethodObjects = pagedDeliveryMethodObjects;
                }

                if (!filteredDeliveryMethodObjects.Any())
                {
                    return Json(new List<DeliveryMethodObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<DeliveryMethodObject, string> orderingFunction = (c =>  c.MethodTitle
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredDeliveryMethodObjects = sortDirection == "asc" ? filteredDeliveryMethodObjects.OrderBy(orderingFunction) : filteredDeliveryMethodObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredDeliveryMethodObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.DeliveryMethodId), c.MethodTitle};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredDeliveryMethodObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<DeliveryMethodObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult AddDeliveryMethod(DeliveryMethodObject deliveryMethod, string subdomain)
        {
            var gVal = new GenericValidator();
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateDeliveryMethod(deliveryMethod);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new DeliveryMethodServices().AddDeliveryMethod(deliveryMethod);
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
        public ActionResult EditDeliveryMethod(DeliveryMethodObject deliveryMethod, string subdomain)
        {
            var gVal = new GenericValidator();
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateDeliveryMethod(deliveryMethod);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_deliveryMethod"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldDeliveryMethod = Session["_deliveryMethod"] as DeliveryMethodObject;
                    if (oldDeliveryMethod == null || oldDeliveryMethod.DeliveryMethodId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldDeliveryMethod.MethodTitle = deliveryMethod.MethodTitle.Trim();
                    if (!string.IsNullOrEmpty(deliveryMethod.Description))
                    {
                        oldDeliveryMethod.Description = deliveryMethod.Description.Trim();
                    }
                    var k = new DeliveryMethodServices().UpdateDeliveryMethod(oldDeliveryMethod);
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
        public ActionResult DeleteDeliveryMethod(long id)
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
                    
                var k = new DeliveryMethodServices().DeleteDeliveryMethod(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Delete_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = message_Feedback.Model_State_Error;
                return Json(gVal, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDeliveryMethod(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new DeliveryMethodObject(), JsonRequestBehavior.AllowGet);
                }

                var deliveryMethod = new DeliveryMethodServices().GetDeliveryMethod(id);
                if (id < 1)
                {
                    return Json(new DeliveryMethodObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_deliveryMethod"] = deliveryMethod;
                return Json(deliveryMethod, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new DeliveryMethodObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<DeliveryMethodObject> GetDeliveryMethods(int? itemsPerPage, int? pageNumber)
        {
            return new DeliveryMethodServices().GetDeliveryMethodObjects(itemsPerPage, pageNumber) ?? new List<DeliveryMethodObject>();
        }
        
        private GenericValidator ValidateDeliveryMethod(DeliveryMethodObject deliveryMethod)
        {
            var gVal = new GenericValidator();
            if (deliveryMethod == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(deliveryMethod.MethodTitle))
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
