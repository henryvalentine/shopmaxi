using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class ChildMenuController : Controller
    {
        #region Actions
        [HttpPost]
        public ActionResult AddChildMenu(List<ChildMenuObject> childMenuList, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var errorList = new List<GenericValidator>();
            var successList = new List<ChildMenuObject>();
            try
            {
                if (!childMenuList.Any())
                {
                    return Json( new GenericValidator {Code = -1, Error = message_Feedback.Menu_List_Empty}, JsonRequestBehavior.AllowGet);
                }

                childMenuList.ForEach(m =>
                {
                    var childMenu = m;
                    
                    var valStatus = ValidateChildMenu(childMenu);
                    if (valStatus.Code < 1)
                    {
                        errorList.Add(new GenericValidator {Code = -1, Error = valStatus.Error});
                    }
                    
                    var k = new ChildMenuServices().AddChildMenu(childMenu);
                    if (k < 1)
                    {
                       var error  = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                       errorList.Add(new GenericValidator {Code = -1, Error = error});
                    }
                    
                    successList.Add(childMenu);
                });

                if (errorList.Any())
                {
                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
                return Json( new GenericValidator {Code = 5, Error = message_Feedback.Insertion_Success}, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json( new GenericValidator {Code = -1, Error = message_Feedback.Process_Failed}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditChildMenu(ChildMenuObject childMenu, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                
                var valStatus = ValidateChildMenu(childMenu);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_childMenu"] == null)
                {
                    gVal.Code = -5;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                   
                var oldChildMenu = Session["_childMenu"] as ChildMenuObject;
                if (oldChildMenu == null || oldChildMenu.ChildMenuId < 1)
                {
                    gVal.Code = -5;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                oldChildMenu.Value = childMenu.Value.Trim();
                oldChildMenu.ParentMenuId = childMenu.ParentMenuId;
                oldChildMenu.IsParent = childMenu.IsParent;
                oldChildMenu.ChildMenuOrder = childMenu.ChildMenuOrder;
                oldChildMenu.Href = childMenu.Href.Trim();
                //oldChildMenu.Roles = childMenu.Roles.Trim();

                var k = new ChildMenuServices().UpdateChildMenu(oldChildMenu);
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
            catch
            {
                gVal.Code = 0;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteChildMenu(long id, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Invalid_Selection;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                    
                var k = new ChildMenuServices().DeleteChildMenu(id);
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
      
        public ActionResult GetMenuItemList(string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            return Json(GetMenuList(), JsonRequestBehavior.AllowGet);
        }
        #endregion
       

        #region Helpers
        private List<ChildMenuObject> GetMenuList()
        {
            return new ChildMenuServices().GetChildMenuList() ?? new List<ChildMenuObject>();
        }
        
        private List<GenericValidator> ValidateChildMenu(List<ChildMenuObject> childMenuList)
        {
            var validationStatusList = new List<GenericValidator>();
            childMenuList.ForEach(m =>
            {
                 var gVal = new GenericValidator();

                var childMenu = m;
                if (childMenu == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Fatal_Error;
                    validationStatusList.Add(gVal);
                }
                if (string.IsNullOrEmpty(childMenu.Value))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ChildMenu_Value_Error;
                   validationStatusList.Add(gVal);
                }

                if (string.IsNullOrEmpty(childMenu.Href))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ChildMenu_Href_Error;
                   validationStatusList.Add(gVal);
                }

                if (childMenu.ChildMenuOrder < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ChildMenu_Order_Error;
                   validationStatusList.Add(gVal);
                }
                if (childMenu.ParentMenuId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ParentMenu_Error;
                    validationStatusList.Add(gVal);
                }
                //if (string.IsNullOrEmpty(childMenu.Roles))
                //{
                //    gVal.Code = -1;
                //    gVal.Error = message_Feedback.ChildMenu_Roles_Error;
                //   validationStatusList.Add(gVal);
                //}
            });
           
            return validationStatusList;
        }
        private GenericValidator ValidateChildMenu(ChildMenuObject childMenu)
        {
            
                 var gVal = new GenericValidator();

                if (childMenu == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Fatal_Error;
                    return gVal;
                }
                if (string.IsNullOrEmpty(childMenu.Value))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ChildMenu_Value_Error;
                   return gVal;
                }

                if (string.IsNullOrEmpty(childMenu.Href))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ChildMenu_Href_Error;
                   return gVal;
                }

                if (childMenu.ChildMenuOrder < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ChildMenu_Order_Error;
                   return gVal;
                }
                //if (string.IsNullOrEmpty(childMenu.Roles))
                //{
                //    gVal.Code = -1;
                //    gVal.Error = message_Feedback.ChildMenu_Roles_Error;
                //   return gVal;
                //}
             gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
