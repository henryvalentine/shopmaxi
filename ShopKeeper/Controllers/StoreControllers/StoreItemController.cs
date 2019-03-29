using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class StoreItemController : Controller
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
        public ActionResult GetStoreItemObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemObject> filteredStoreItemObjects;
                var countG = new StoreItemServices().GetObjectCount();

                var pagedStoreItemObjects = GetStoreItems(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreItemObjects = new StoreItemServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreItemObjects = pagedStoreItemObjects;
                }

                if (!filteredStoreItemObjects.Any())
                {
                    return Json(new List<StoreItemObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemObject, string> orderingFunction = (c => c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                if (sortDirection == "asc")
                    filteredStoreItemObjects = filteredStoreItemObjects.OrderBy(orderingFunction);
                else
                    filteredStoreItemObjects = filteredStoreItemObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStoreItemObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreItemId), c.Name, c.StoreItemCategoryName, c.StoreItemTypeName, c.StoreItemBrandName, c.ParentItemName, c.ChartOfAccountTypeName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreItemObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddStoreItem(StoreItemObject product)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItem(product);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    //var logoPath = SaveFile("");
                    //if (!string.IsNullOrEmpty(logoPath))
                    //{
                    //    product.SampleImagePath = logoPath;
                    //}
                    var k = new StoreItemServices().AddStoreItem(product);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            product.Name = message_Feedback.Item_Duplicate;
                        }
                        if (k == -4)
                        {
                            product.Name = message_Feedback.Insertion_Failure;
                        }
                        if (k != -3 && k != -4)
                        {
                            product.Name = message_Feedback.Insertion_Failure;
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
        public ActionResult EditStoreItem(StoreItemObject product)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStoreItem(product);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_product"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldStoreItem = Session["_product"] as StoreItemObject;
                    if (oldStoreItem == null || oldStoreItem.StoreItemId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldStoreItem.Name = product.Name.Trim();
                    oldStoreItem.ChartOfAccountId = product.ChartOfAccountId;
                    oldStoreItem.StoreItemBrandId = product.StoreItemBrandId;
                    oldStoreItem.StoreItemTypeId = product.StoreItemTypeId;
                    oldStoreItem.StoreItemCategoryId = product.StoreItemCategoryId;

                    if (product.ParentItemId > 0)
                    {
                        oldStoreItem.ParentItemId = product.ParentItemId;
                    }

                    if (!string.IsNullOrEmpty(product.Description))
                    {
                        oldStoreItem.Description = product.Description.Trim();
                    }

                    //var formerSampleImagePath = string.Empty;
                    //if (!string.IsNullOrEmpty(oldStoreItem.SampleImagePath))
                    //{
                    //    formerSampleImagePath = oldStoreItem.SampleImagePath;
                    //}

                    //var logoPath = SaveFile(formerSampleImagePath);

                    //if (!string.IsNullOrEmpty(logoPath))
                    //{
                    //    oldStoreItem.SampleImagePath = logoPath;
                    //}

                    var k = new StoreItemServices().UpdateStoreItem(oldStoreItem);
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
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteStoreItem(long id)
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

                var k = new StoreItemServices().DeleteStoreItem(id);
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
        public ActionResult GetStoreItem(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreItemObject(), JsonRequestBehavior.AllowGet);
                }

                var product = new StoreItemServices().GetStoreItem(id);
                if (id < 1)
                {
                    return Json(new StoreItemObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_product"] = product;
                //if (!string.IsNullOrEmpty(product.SampleImagePath))
                //{
                //    product.SampleImagePath = product.SampleImagePath.Replace("~", "");
                //}
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreItemObject(), JsonRequestBehavior.AllowGet);
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
                    Session["_pImg"] = file;
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

        public ActionResult GetListObjects()
        {
            try
            {
                var genericObject = new GenericObject
                {
                    ProductBrands = GetProductBrands(),
                    ProductCategories = GetProductCategories(),
                    ProductTypes = GetProductTypes(),
                    ChartsOfAccount = GetChartSOfAccount(),
                    Products = GetProducts()
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new ChartOfAccountObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetStoreItemBrandObjects(string searchCriteria)
        {
            try
            {
                if (string.IsNullOrEmpty(searchCriteria))
                {
                    return Json(new List<StoreItemBrandObject>(), JsonRequestBehavior.AllowGet);
                }

                var filteredStoreItemObjects = new StoreItemBrandServices().Search(searchCriteria);

                if (!filteredStoreItemObjects.Any())
                {
                    return Json(new List<StoreItemBrandObject>(), JsonRequestBehavior.AllowGet);
                }

                var result = from c in filteredStoreItemObjects
                             select new[] { Convert.ToString(c.StoreItemBrandId), c.Name, c.Description, c.LastUpdated.ToString("dd/MM/yyyy") };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemBrandObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ProductUpload(HttpPostedFileBase file)
        {
            var gVal = new GenericValidator();
            try
            {
                if (file.ContentLength > 0)
                {
                    const string folderPath = "~/BulkUploads/Products";

                    var fileName = file.FileName;
                    var path = Server.MapPath(folderPath + "/" + fileName);

                    if (System.IO.File.Exists(Server.MapPath(path)))
                    {
                        System.IO.File.Delete(path);
                    }

                    file.SaveAs(Server.MapPath(folderPath + "/" + fileName));

                    var msg = string.Empty;
                    var errorList = new List<long>();

                    var successfulImports = new StoreItemUploadServices().ReadItemsExcelData(path, "products", ref errorList, ref msg);

                    if (!successfulImports.Any())
                    {
                        gVal.Code = -1;
                        gVal.Error = msg;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (errorList.Any() && successfulImports.Any())
                    {
                        var feedbackMessage = successfulImports.Count + " records were successfully uploaded." +
                            "\n" + errorList.Count + " record(s) could not be uploaded due to specified/unspecified errors.";
                        if (msg.Length > 0)
                        {
                            feedbackMessage += "<br/>" + msg;
                        }

                        gVal.Code = -1;
                        gVal.Error = feedbackMessage;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (errorList.Any() && !successfulImports.Any())
                    {
                        var feedbackMessage = errorList.Count + " record(s) could not be uploaded due to specified/unspecified errors.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            feedbackMessage += "<br/>" + msg;
                        }

                        gVal.Code = -1;
                        gVal.Error = feedbackMessage;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (!errorList.Any() && successfulImports.Any())
                    {
                        var feedbackMessage = successfulImports.Count + " records were successfully uploaded.";

                        if (msg.Length > 0)
                        {
                            feedbackMessage += "<br/>" + msg;
                        }

                        gVal.Code = 5;
                        gVal.Error = feedbackMessage;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }
                gVal.Code = -1;
                gVal.Error = "The selected file is invalid";
                return Json(gVal, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region Helpers
        public bool DownloadContentFromFolder(string path)
        {
            try
            {
                Response.Clear();
                var filename = Path.GetFileName(path);
                HttpContext.Response.Buffer = true;
                HttpContext.Response.Charset = "";
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = GetMimeType(filename);
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                Response.WriteFile(Server.MapPath(path));
                Response.End();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var ext = extension.ToLower();
                var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }

        public List<ChartOfAccountObject> GetChartSOfAccount()
        {
            return new ChartOfAccountServices().GetChartsOfAccount();
        }
        public List<StoreItemCategoryObject> GetProductCategories()
        {
            return new StoreItemCategoryServices().GetStoreItemCategories();
        }
        public List<StoreItemTypeObject> GetProductTypes()
        {
            return new StoreItemTypeServices().GetStoreItemTypes();
        }
        public List<StoreItemBrandObject> GetProductBrands()
        {
            return new StoreItemBrandServices().GetStoreItemBrands();
        }
        public List<StoreItemObject> GetProducts()
        {
            return new StoreItemServices().GetStoreItems();
        }
        public string SaveFile(string formerPath)
        {
            try
            {
                if (Session["_pImg"] == null)
                {
                    return string.Empty;
                }

                var file = Session["_pImg"] as HttpPostedFileBase;

                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Images/StoreItems");

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
                        Session["_pImg"] = null;
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

        private List<StoreItemObject> GetStoreItems(int? itemsPerPage, int? pageNumber)
        {
            return new StoreItemServices().GetStoreItemObjects(itemsPerPage, pageNumber) ?? new List<StoreItemObject>();
        }

        private GenericValidator ValidateStoreItem(StoreItemObject product)
        {
            var gVal = new GenericValidator();
            if (product == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(product.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Name_Error;
                return gVal;
            }

            if (product.ChartOfAccountId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Chart_Of_Account_Selection_Error;
                return gVal;
            }
            if (product.StoreItemBrandId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Brand_Selection_Error;
                return gVal;
            }
            if (product.StoreItemCategoryId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Category_Selection_Error;
                return gVal;
            }
            if (product.StoreItemTypeId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Type_Selection_Error;
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
