using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
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
	public class StockUploadController : Controller
	{
        #region Actions

        [HttpPost]
        public ActionResult DeleteStockUpload(long id, string subdomain)
        {
            var gVal = new GenericValidator();
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Delete_Failure;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }

            
            try
            {
                if (id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Invalid_Selection;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var k = new StockUploadServices().DeleteStockUpload(id);
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
                gVal.Code = id;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStockUpload(long id, string subdomain)
        {
            var gVal = new GenericValidator();
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Delete_Failure;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (id < 1)
                {
                    return Json(new StockUploadObject(), JsonRequestBehavior.AllowGet);
                }

                var stockUpload = new StockUploadServices().GetStockUpload(id);
                if (id < 1)
                {
                    return Json(new StockUploadObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_stockUpload"] = stockUpload;
                return Json(stockUpload, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockUploadObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetImageViews(string subdomain)
        {
            var images = new ImageViewServices().GetImageViews() ?? new List<ImageViewObject>();
            return Json(images, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateFileSession(HttpPostedFileBase file)
        {
            var gVal = new GenericValidator();
            try
            {
                gVal.Code = 0;
                if (file != null && file.ContentLength > 0)
                {
                    List<SessionObj> imgSessList;
                    if (Session["_imgSessList"] == null)
                    {
                        imgSessList = new List<SessionObj>();
                        imgSessList.Add(new SessionObj
                        {
                            FileId = 1,
                            FileObj = file
                        });
                        gVal.Code = 1;
                    }
                    else
                    {
                        imgSessList = Session["_imgSessList"] as List<SessionObj>;
                        if (imgSessList == null || !imgSessList.Any())
                        {
                            imgSessList = new List<SessionObj>
                            {
                                new SessionObj
                                {
                                    FileId = 1,
                                    FileObj = file
                                }
                            };
                            gVal.Code = 1;
                        }
                        else
                        {
                            var count = imgSessList.Count + 1;
                            imgSessList = new List<SessionObj>
                            {
                                new SessionObj
                                {
                                    FileId = count,
                                    FileObj = file
                                }
                            };
                            gVal.Code = count;
                        }
                    }

                    Session["_imgSessList"] = imgSessList;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = -1;
                gVal.Error = message_Feedback.Invalid_File;
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

        public string SaveFile(HttpPostedFileBase file, string formerPath, string subdomain)
        {
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return string.Empty;
            }
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Images/StoreItemStock/" + subdomain);

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
                        if (!DeleteFile(formerFilePath))
                        {
                            return false;
                        }
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

        private bool DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return false;
                }

                if (!filePath.StartsWith("~"))
                {
                    filePath = "~" + filePath;
                }

                System.IO.File.Delete(Server.MapPath(filePath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Helpers

        #endregion

    }
}
