using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
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
	public class StoreItemCategoryController : Controller
	{
		public StoreItemCategoryController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult StoreItemCategorys()
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
        public ActionResult GetStoreItemCategoryObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemCategoryObject> filteredStoreItemCategoryObjects;
                var countG = new StoreItemCategoryServices().GetObjectCount();

                var pagedStoreItemCategoryObjects = GetStoreItemCategorys(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreItemCategoryObjects = new StoreItemCategoryServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreItemCategoryObjects = pagedStoreItemCategoryObjects;
                }

                if (!filteredStoreItemCategoryObjects.Any())
                {
                    return Json(new List<StoreItemCategoryObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemCategoryObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : c.ParentName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                if (sortDirection == "asc")
                    filteredStoreItemCategoryObjects = filteredStoreItemCategoryObjects.OrderBy(orderingFunction);
                else
                    filteredStoreItemCategoryObjects = filteredStoreItemCategoryObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreItemCategoryObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreItemCategoryId), c.Name, c.ParentName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreItemCategoryObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemCategoryObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult AddStoreItemCategory()
        {
           return View(new StoreItemCategoryObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreItemCategory(StoreItemCategoryObject storeItemCategory)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemCategory(storeItemCategory);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var logoPath = SaveFile("");
                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        storeItemCategory.ImagePath = logoPath;
                    }

                    storeItemCategory.LastUpdated = DateTime.Now;
                    
                    var k = new StoreItemCategoryServices().AddStoreItemCategory(storeItemCategory);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            gVal.Error = message_Feedback.Item_Duplicate;
                        }
                        
                        if (k != -3 && k != -4)
                        {
                            gVal.Error = message_Feedback.Insertion_Failure;
                        }
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = 5;
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
        public ActionResult EditStoreItemCategory(StoreItemCategoryObject storeItemCategory)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemCategory(storeItemCategory);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storeItemCategory"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreItemCategory = Session["_storeItemCategory"] as StoreItemCategoryObject;
                    if (oldStoreItemCategory == null || oldStoreItemCategory.StoreItemCategoryId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldStoreItemCategory.Name = storeItemCategory.Name.Trim();
                    
                    if (!string.IsNullOrEmpty(storeItemCategory.Description))
                    {
                        oldStoreItemCategory.Description = storeItemCategory.Description.Trim();
                    }

                    if (storeItemCategory.ParentCategoryId > 0)
                    {
                        oldStoreItemCategory.ParentCategoryId = storeItemCategory.ParentCategoryId;
                    }

                    var formerImagePath = string.Empty;
                    if (!string.IsNullOrEmpty(oldStoreItemCategory.ImagePath))
                    {
                        formerImagePath = oldStoreItemCategory.ImagePath;
                    }

                    var logoPath = SaveFile(formerImagePath);

                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        oldStoreItemCategory.ImagePath = logoPath;
                    }
                    oldStoreItemCategory.LastUpdated = DateTime.Now;
                    var k = new StoreItemCategoryServices().UpdateStoreItemCategory(oldStoreItemCategory);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            gVal.Error = message_Feedback.Item_Duplicate;
                        }
                        
                        if (k != -3 && k != -4)
                        {
                            gVal.Error = message_Feedback.Update_Failure;
                        }
                        gVal.Code = -1;
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
        public ActionResult DeleteStoreItemCategory(long id)
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
                    
                var k = new StoreItemCategoryServices().DeleteStoreItemCategory(id);
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
        public ActionResult GetStoreItemCategory(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreItemCategoryObject(), JsonRequestBehavior.AllowGet);
                }

                var storeItemCategory = new StoreItemCategoryServices().GetStoreItemCategory(id);
                if (id < 1)
                {
                    return Json(new StoreItemCategoryObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storeItemCategory"] = storeItemCategory;
                if (!string.IsNullOrEmpty(storeItemCategory.ImagePath))
                {
                    storeItemCategory.ImagePath = storeItemCategory.ImagePath.Replace("~", "");
                }
                return Json(storeItemCategory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new StoreItemCategoryObject(), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult CreateFileSession(HttpPostedFileBase file)
        {
            var gVal = new GenericValidator();
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    gVal.Code = 5;
                    Session["_pTypeImg"] = file;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                
                gVal.Code = -1;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStoreItemCategories()
        {
            var itemCategories = new StoreItemCategoryServices().GetStoreItemCategories() ?? new List<StoreItemCategoryObject>();
            return Json(itemCategories, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #region Helpers
        public string SaveFile(string formerPath)
        {
            try
            {
                if (Session["_pTypeImg"] == null)
                {
                    return string.Empty;
                }

                var file = Session["_pTypeImg"] as HttpPostedFileBase;

                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Images/StoreItemCategorys");

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                        var dInfo = new DirectoryInfo(mainPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                    }
                    var path = "";
                    if (SaveToFolder(file, ref path, mainPath, formerPath))
                    {
                        Session["_pTypeImg"] = null;
                        return PhysicalToVirtualPathMapper.MapPath(path);
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return string.Empty;
            }
        }

        private static string GenerateUniqueName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid();
        }

        private bool SaveToFolder(HttpPostedFileBase file, ref string path, string folderPath, string formerFilePath = null)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = GenerateUniqueName() + fileExtension;
                    var newPathv = Path.Combine(folderPath, fileName);
                    file.SaveAs(newPathv);
                    if (!string.IsNullOrWhiteSpace(formerFilePath))
                    {
                        if (!formerFilePath.StartsWith("~"))
                        {
                            formerFilePath = "~" + formerFilePath;
                        }
                        System.IO.File.Delete(Server.MapPath(formerFilePath));
                    }
                    path = newPathv;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        private List<StoreItemCategoryObject> GetStoreItemCategorys(int? itemsPerPage, int? pageNumber)
        {
            return new StoreItemCategoryServices().GetStoreItemCategoryObjects(itemsPerPage, pageNumber) ?? new List<StoreItemCategoryObject>();
        }

       private GenericValidator ValidateStoreItemCategory(StoreItemCategoryObject storeItemCategory)
        {
            var gVal = new GenericValidator();
            if (storeItemCategory == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storeItemCategory.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Category_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
