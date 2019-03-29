using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ShopKeeper.GenericHelpers;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class ParentMenuController : Controller
	{
        /// <summary>
        /// Handles calls Ajax from DataTable(to which the Facilities List is/to be bound)
        /// </summary>
        /// <param name="param">
        /// Ajax model that encapsulates all required parameters such as 
        /// filtering, pagination, soting, etc instructions from the DataTable
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetParentMenuObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<ParentMenuObject> filteredParentMenuObjects;
                var countG = 0;

                var pagedParentMenuObjects = GetMenuList(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ParentMenuServices().Search(param.sSearch);
                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;
                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<ParentMenuObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<ParentMenuObject, string> orderingFunction = (c => c.Value);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "asc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredParentMenuObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.ParentMenuId), c.Value, c.MenuOrder.ToString(CultureInfo.InvariantCulture), c.RoleName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredParentMenuObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<ParentMenuObject>(), JsonRequestBehavior.AllowGet);
            }
        }


        #region Actions

        public ActionResult GetParentMenu(int menuId)
        {
            return Json(GetMenu(menuId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCascades(int menuId)
        {
            return Json(GetParentCascades(menuId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddChildMenu(ChildMenuObject childMenu)
        {
            try
            {
                var valStatus = ValidateChildMenu(childMenu);
                if (valStatus.Code < 1)
                {
                    return Json(valStatus, JsonRequestBehavior.AllowGet);
                }

                var k = new ParentMenuServices().AddChildMenuWithRoles(childMenu);
                if (k < 1)
                {
                    var error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    return Json(new GenericValidator { Code = -1, Error = error }, JsonRequestBehavior.AllowGet);
                }

                return Json(new GenericValidator { Code = 5, Error = message_Feedback.Insertion_Success }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new GenericValidator { Code = -1, Error = message_Feedback.Process_Failed }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddSingleParentMenu(ParentMenuObject parentMenu)
        {
            try
            {
                var valStatus = ValidateParentMenu(parentMenu);
                if (valStatus.Code < 1)
                {
                    return Json(valStatus, JsonRequestBehavior.AllowGet);
                }

                var k = new ParentMenuServices().AddParentMenuWithRoles(parentMenu);
                if (k < 1)
                {
                    var error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    return Json(new GenericValidator { Code = -1, Error = error }, JsonRequestBehavior.AllowGet);
                }

                return Json(new GenericValidator { Code = 5, Error = message_Feedback.Insertion_Success }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new GenericValidator { Code = -1, Error = message_Feedback.Process_Failed }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddParentMenu(List<ParentMenuObject> parentMenuList)
        {
            var errorList = new List<GenericValidator>();
            var successList = new List<ParentMenuObject>();
            try
            {
                if (!parentMenuList.Any())
                {
                    return Json(new GenericValidator
                    {
                        Code = -1,
                        Error = message_Feedback.Menu_List_Empty
                    }, JsonRequestBehavior.AllowGet);
                }

                parentMenuList.ForEach(m =>
                {
                    var parentMenu = m;

                    var valStatus = ValidateParentMenu(parentMenu);
                    if (valStatus.Code < 1)
                    {
                        errorList.Add(new GenericValidator { Code = -1, Error = valStatus.Error });
                    }

                    var k = new ParentMenuServices().AddParentMenu(parentMenu);
                    if (k < 1)
                    {
                        var error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        errorList.Add(new GenericValidator { Code = -1, Error = error });
                    }

                    successList.Add(parentMenu);
                });

                if (errorList.Any())
                {
                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
                return Json(new GenericValidator { Code = 5, Error = message_Feedback.Insertion_Success }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new GenericValidator { Code = -1, Error = message_Feedback.Process_Failed }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditParentMenu(ParentMenuObject parentMenu)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateParentMenu(parentMenu);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new ParentMenuServices().UpdateParentMenuWithRoles(parentMenu);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = k;
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
        public ActionResult EditChildMenu(ChildMenuObject childMenu)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateChildMenu(childMenu);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new ParentMenuServices().UpdateChildMenuWithRoles(childMenu);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = k;
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

        public ActionResult GetMenuItemList(List<string> roleIds)
        {
            return Json(GetMenuList(roleIds), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Helpers

        private ParentMenuObject GetMenu(int menuId)
        {
            return new ParentMenuServices().GetParentMenu(menuId) ?? new ParentMenuObject();
        }
        private List<ChildMenuObject> GetParentCascades(int menuId)
        {
            return new ParentMenuServices().GetCascades(menuId);
        }
        private List<ParentMenuObject> GetMenuList(int? itemsPerPage, int? pageNumber, out int countG)
        {
            return new ParentMenuServices().GetParentMenuList(itemsPerPage, pageNumber, out countG) ?? new List<ParentMenuObject>();
        }
        private List<ParentMenuObject> GetMenuList(List<string> roleIds)
        {
            return new ParentMenuServices().GetParentMenuList(roleIds) ?? new List<ParentMenuObject>();
        }
        private List<GenericValidator> ValidateParentMenu(List<ParentMenuObject> parentMenuList)
        {
            var validationStatusList = new List<GenericValidator>();
            parentMenuList.ForEach(m =>
            {
                var gVal = new GenericValidator();

                var parentMenu = m;
                if (parentMenu == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Fatal_Error;
                    validationStatusList.Add(gVal);
                }
                if (string.IsNullOrEmpty(parentMenu.Value))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ParentMenu_Value_Error;
                    validationStatusList.Add(gVal);
                }

                if (string.IsNullOrEmpty(parentMenu.Href))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ParentMenu_Href_Error;
                    validationStatusList.Add(gVal);
                }

                if (parentMenu.MenuOrder < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ParentMenu_Order_Error;
                    validationStatusList.Add(gVal);
                }
                if (!parentMenu.ParentMenuRoleObjects.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.ParentMenu_Roles_Error;
                    validationStatusList.Add(gVal);
                }
            });

            return validationStatusList;
        }
        private GenericValidator ValidateParentMenu(ParentMenuObject parentMenu)
        {

            var gVal = new GenericValidator();

            if (parentMenu == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(parentMenu.Value))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.ParentMenu_Value_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(parentMenu.Href))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.ParentMenu_Href_Error;
                return gVal;
            }

            if (parentMenu.MenuOrder < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.ParentMenu_Order_Error;
                return gVal;
            }
            if (!parentMenu.ParentMenuRoleObjects.Any())
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.ParentMenu_Roles_Error;
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
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

            if (childMenu.ParentMenuId < 1)
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

            if (!childMenu.ChildMenuRoleObjects.Any())
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.ChildMenu_Roles_Error;
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }
        #endregion


    }
}
