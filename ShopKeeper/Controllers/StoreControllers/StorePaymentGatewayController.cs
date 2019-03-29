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
	public class StorePaymentGatewayController : Controller
	{
		public StorePaymentGatewayController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult StorePaymentGateways()
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
        public ActionResult GetStorePaymentGatewayObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StorePaymentGatewayObject> filteredStorePaymentGatewayObjects;
                var countG = new StorePaymentGatewayServices().GetObjectCount();

                var pagedStorePaymentGatewayObjects = GetStorePaymentGateways(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStorePaymentGatewayObjects = new StorePaymentGatewayServices().Search(param.sSearch);
                }
                else
                {
                    filteredStorePaymentGatewayObjects = pagedStorePaymentGatewayObjects;
                }

                if (!filteredStorePaymentGatewayObjects.Any())
                {
                    return Json(new List<StorePaymentGatewayObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StorePaymentGatewayObject, string> orderingFunction = (c => c.GatewayName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                if (sortDirection == "asc")
                    filteredStorePaymentGatewayObjects = filteredStorePaymentGatewayObjects.OrderBy(orderingFunction);
                else
                    filteredStorePaymentGatewayObjects = filteredStorePaymentGatewayObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredStorePaymentGatewayObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StorePaymentGatewayId), c.GatewayName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStorePaymentGatewayObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StorePaymentGatewayObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult AddStorePaymentGateway()
        {
           return View(new StorePaymentGatewayObject());
        }
        
        [HttpPost]
        public ActionResult AddStorePaymentGateway(StorePaymentGatewayObject storePaymentGateway)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStorePaymentGateway(storePaymentGateway);
                    if (valStatus.Code < 1)
                    {
                        storePaymentGateway.StorePaymentGatewayId = 0;
                        storePaymentGateway.GatewayName = valStatus.Error;
                        return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
                    }
                    
                    var logoPath = SaveFile("");
                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        storePaymentGateway.LogoPath = logoPath;
                    }
                    var k = new StorePaymentGatewayServices().AddStorePaymentGateway(storePaymentGateway);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            storePaymentGateway.GatewayName = "Payment Gateway information already exists";
                        }
                        if (k == -4)
                        {
                            storePaymentGateway.GatewayName = "This Sort Code has been registered for another Payment Gateway.";
                        }
                        if (k != -3 && k != -4)
                        {
                            storePaymentGateway.GatewayName = "Payment Gateway information could not be added. Please try again";
                        }
                        storePaymentGateway.StorePaymentGatewayId = 0;
                        return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
                    }

                    storePaymentGateway.StorePaymentGatewayId = k;
                    return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
                }

                storePaymentGateway.StorePaymentGatewayId = -5;
                storePaymentGateway.GatewayName = message_Feedback.Model_State_Error;
                return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                storePaymentGateway.StorePaymentGatewayId = 0;
                storePaymentGateway.GatewayName = message_Feedback.Process_Failed;
                return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditStorePaymentGateway(StorePaymentGatewayObject storePaymentGateway)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateStorePaymentGateway(storePaymentGateway);
                    if (valStatus.Code < 1)
                    {
                        storePaymentGateway.StorePaymentGatewayId = 0;
                        storePaymentGateway.GatewayName = valStatus.Error;
                        return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_storePaymentGateway"] == null)
                    {
                        storePaymentGateway.StorePaymentGatewayId = -5;
                        storePaymentGateway.GatewayName = message_Feedback.Session_Time_Out;
                        return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldStorePaymentGateway = Session["_storePaymentGateway"] as StorePaymentGatewayObject;
                    if (oldStorePaymentGateway == null || oldStorePaymentGateway.StorePaymentGatewayId < 1)
                    {
                       storePaymentGateway.StorePaymentGatewayId = -5;
                       storePaymentGateway.GatewayName = message_Feedback.Session_Time_Out;
                       return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
                    }

                    oldStorePaymentGateway.GatewayName = storePaymentGateway.GatewayName.Trim();
                    
                    var formerLogoPath = string.Empty;
                    if (!string.IsNullOrEmpty(oldStorePaymentGateway.LogoPath))
                    {
                        formerLogoPath = oldStorePaymentGateway.LogoPath;
                    }

                    var logoPath = SaveFile(formerLogoPath);

                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        oldStorePaymentGateway.LogoPath = logoPath;
                    }

                    var k = new StorePaymentGatewayServices().UpdateStorePaymentGateway(oldStorePaymentGateway);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            storePaymentGateway.GatewayName = "Payment Gateway information already exists";
                        }
                        if (k == -4)
                        {
                            storePaymentGateway.GatewayName = "This Sort Code has been registered for another Payment Gateway.";
                        }
                        if (k != -3 && k != -4)
                        {
                            storePaymentGateway.GatewayName = "Payment Gateway information could not be updated. Please try again";
                        }
                        storePaymentGateway.StorePaymentGatewayId = 0;
                        return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
                    }
                    return Json(oldStorePaymentGateway, JsonRequestBehavior.AllowGet);
                }

                storePaymentGateway.StorePaymentGatewayId = -5;
                storePaymentGateway.GatewayName = message_Feedback.Model_State_Error;
                return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                storePaymentGateway.StorePaymentGatewayId = 0;
                storePaymentGateway.GatewayName = message_Feedback.Process_Failed;
                return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteStorePaymentGateway(long id)
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
                    
                var k = new StorePaymentGatewayServices().DeleteStorePaymentGateway(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = "Payment Gateway information was successfully deleted.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = "Payment Gateway information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = "Payment Gateway information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStorePaymentGateway(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StorePaymentGatewayObject(), JsonRequestBehavior.AllowGet);
                }

                var storePaymentGateway = new StorePaymentGatewayServices().GetStorePaymentGateway(id);
                if (id < 1)
                {
                    return Json(new StorePaymentGatewayObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_storePaymentGateway"] = storePaymentGateway;
                if (!string.IsNullOrEmpty(storePaymentGateway.LogoPath))
                {
                    storePaymentGateway.LogoPath = storePaymentGateway.LogoPath.Replace("~", "");
                }
                return Json(storePaymentGateway, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new StorePaymentGatewayObject(), JsonRequestBehavior.AllowGet);
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
                    Session["_pyImg"] = file;
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
                if (Session["_pyImg"] == null)
                {
                    return string.Empty;
                }

                var file = Session["_pyImg"] as HttpPostedFileBase;

                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Images/StorePaymentGateways");

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
                        Session["_pyImg"] = null;
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

        private static string GenerateUniqueGatewayName()
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
                    var fileGatewayName = GenerateUniqueGatewayName() + fileExtension;
                    var newPathv = Path.Combine(folderPath, fileGatewayName);
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

        private List<StorePaymentGatewayObject> GetStorePaymentGateways(int? itemsPerPage, int? pageNumber)
        {
            return new StorePaymentGatewayServices().GetStorePaymentGatewayObjects(itemsPerPage, pageNumber) ?? new List<StorePaymentGatewayObject>();
        }

        private GenericValidator ValidateStorePaymentGateway(StorePaymentGatewayObject storePaymentGateway)
        {
            var gVal = new GenericValidator();
            if (storePaymentGateway == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(storePaymentGateway.GatewayName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Payment_Gateway_Error;
                return gVal;
            }
            
            gVal.Code = 5;
            gVal.Error = "";
            return gVal;
        }
        #endregion

    }
}
