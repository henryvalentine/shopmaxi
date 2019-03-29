using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ShopKeeper.Properties;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;


namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class StoreItemTypeController : Controller
	{
		public StoreItemTypeController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult StoreItemTypes()
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
        public ActionResult GetStoreItemTypeObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemTypeObject> filteredStoreItemTypeObjects;
                var countG = new StoreItemTypeServices().GetObjectCount();

                var pagedStoreItemTypeObjects = GetStoreItemTypes(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreItemTypeObjects = new StoreItemTypeServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreItemTypeObjects = pagedStoreItemTypeObjects;
                }

                if (!filteredStoreItemTypeObjects.Any())
                {
                    return Json(new List<StoreItemTypeObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemTypeObject, string> orderingFunction = (c => c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                if (sortDirection == "asc")
                    filteredStoreItemTypeObjects = filteredStoreItemTypeObjects.OrderBy(orderingFunction);
                else
                    filteredStoreItemTypeObjects = filteredStoreItemTypeObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreItemTypeObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreItemTypeId), c.Name };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreItemTypeObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemTypeObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult AddStoreItemType()
        {
           return View(new StoreItemTypeObject());
        }
        
        [HttpPost]
        public ActionResult AddStoreItemType(StoreItemTypeObject productType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemType(productType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var logoPath = SaveFile("");
                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        productType.SampleImagePath = logoPath;
                    }
                    var k = new StoreItemTypeServices().AddStoreItemType(productType);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            productType.Name = message_Feedback.Item_Duplicate;
                        }
                        
                        if (k != -3 && k != -4)
                        {
                            productType.Name = message_Feedback.Insertion_Failure;
                        }
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
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditStoreItemType(StoreItemTypeObject productType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItemType(productType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_productType"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStoreItemType = Session["_productType"] as StoreItemTypeObject;
                    if (oldStoreItemType == null || oldStoreItemType.StoreItemTypeId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldStoreItemType.Name = productType.Name.Trim();
                    
                    if (!string.IsNullOrEmpty(productType.Description))
                    {
                        oldStoreItemType.Description = productType.Description.Trim();
                    }

                    var formerSampleImagePath = string.Empty;
                    if (!string.IsNullOrEmpty(oldStoreItemType.SampleImagePath))
                    {
                        formerSampleImagePath = oldStoreItemType.SampleImagePath;
                    }

                    var logoPath = SaveFile(formerSampleImagePath);

                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        oldStoreItemType.SampleImagePath = logoPath;
                    }

                    var k = new StoreItemTypeServices().UpdateStoreItemType(oldStoreItemType);
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
                    gVal.Error = message_Feedback.Update_Success;
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
        public ActionResult DeleteStoreItemType(long id)
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
                    
                var k = new StoreItemTypeServices().DeleteStoreItemType(id);
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
        public ActionResult GetStoreItemType(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreItemTypeObject(), JsonRequestBehavior.AllowGet);
                }

                var productType = new StoreItemTypeServices().GetStoreItemType(id);
                if (id < 1)
                {
                    return Json(new StoreItemTypeObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_productType"] = productType;
                if (!string.IsNullOrEmpty(productType.SampleImagePath))
                {
                    productType.SampleImagePath = productType.SampleImagePath.Replace("~", "");
                }
                return Json(productType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new StoreItemTypeObject(), JsonRequestBehavior.AllowGet);
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
                    var mainPath = Server.MapPath("~/Images/StoreItemTypes");

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

        private List<StoreItemTypeObject> GetStoreItemTypes(int? itemsPerPage, int? pageNumber)
        {
            return new StoreItemTypeServices().GetStoreItemTypeObjects(itemsPerPage, pageNumber) ?? new List<StoreItemTypeObject>();
        }
        
        private GenericValidator ValidateStoreItemType(StoreItemTypeObject productType)
        {
            var gVal = new GenericValidator();
            if (productType == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(productType.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Type_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
