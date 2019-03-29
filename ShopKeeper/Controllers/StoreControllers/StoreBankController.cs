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
	public class StoreBankController : Controller
	{
        public StoreBankController()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult Banks()
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
        public ActionResult GetBankObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<BankObject> filteredBankObjects;
                var countG = new BankServices().GetObjectCount();

                var pagedBankObjects = GetBanks(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredBankObjects = new BankServices().Search(param.sSearch);
                }
                else
                {
                    filteredBankObjects = pagedBankObjects;
                }

                if (!filteredBankObjects.Any())
                {
                    return Json(new List<BankObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<BankObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.FullName :
                                                                    sortColumnIndex == 2 ? c.ShortName :
                                                                    c.SortCode
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                if (sortDirection == "asc")
                    filteredBankObjects = filteredBankObjects.OrderBy(orderingFunction);
                else
                    filteredBankObjects = filteredBankObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredBankObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.BankId), c.FullName, c.ShortName, c.SortCode };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredBankObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<BankObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult AddBank()
        {
           return View(new BankObject());
        }
        
        [HttpPost]
        public ActionResult AddBank(BankObject bank)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateBank(bank);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    bank.LastUpdated = DateTime.Now.ToString("yyyy/mm/dd hh:mm:ss tt");
                    var logoPath = SaveFile("");
                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        bank.LogoPath = logoPath;
                    }
                    var k = new BankServices().AddBank(bank);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            gVal.Error = message_Feedback.Item_Duplicate;
                        }
                        if (k == -4)
                        {
                            gVal.Error = message_Feedback.Duplicate_Sort_Code;
                        }
                        if (k != -3 && k != -4)
                        {
                            gVal.Error = message_Feedback.Update_Failure;
                        }
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
        public ActionResult EditBank(BankObject bank)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateBank(bank);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_bank"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldBank = Session["_bank"] as BankObject;

                    if (oldBank == null || oldBank.BankId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(bank, JsonRequestBehavior.AllowGet);
                    }

                    oldBank.FullName = bank.FullName.Trim();
                    oldBank.ShortName = bank.ShortName.Trim();
                    oldBank.SortCode = bank.SortCode.Trim();

                    var formerLogoPath = string.Empty;
                    if (!string.IsNullOrEmpty(oldBank.LogoPath))
                    {
                        formerLogoPath = oldBank.LogoPath;
                    }

                    var logoPath = SaveFile(formerLogoPath);

                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        oldBank.LogoPath = logoPath;
                    }

                    var k = new BankServices().UpdateBank(oldBank);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            gVal.Error = message_Feedback.Item_Duplicate;
                        }
                        if (k == -4)
                        {
                            gVal.Error = message_Feedback.Duplicate_Sort_Code;
                        }
                        if (k != -3 && k != -4)
                        {
                            gVal.Error = message_Feedback.Update_Failure;
                        }
                        gVal.Error = message_Feedback.Update_Success;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Error = message_Feedback.Insertion_Success;
                    gVal.Code = 5;
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
                return Json(bank, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteBank(long id)
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
                    
                var k = new BankServices().DeleteBank(id);
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
        public ActionResult GetBank(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new BankObject(), JsonRequestBehavior.AllowGet);
                }

                var bank = new BankServices().GetBank(id);
                if (id < 1)
                {
                    return Json(new BankObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_bank"] = bank;
                if (!string.IsNullOrEmpty(bank.LogoPath))
                {
                    bank.LogoPath = bank.LogoPath.Replace("~", "");
                }
                return Json(bank, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new BankObject(), JsonRequestBehavior.AllowGet);
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
                    Session["_bLogo"] = file;
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
                if (Session["_bLogo"] == null)
                {
                    return string.Empty;
                }

                var file = Session["_bLogo"] as HttpPostedFileBase;

                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Logo/Banks");

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
                        Session["_bLogo"] = null;
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

        private List<BankObject> GetBanks(int? itemsPerPage, int? pageNumber)
        {
            return new BankServices().GetBankObjects(itemsPerPage, pageNumber) ?? new List<BankObject>();
        }

        private GenericValidator ValidateBank(BankObject bank)
        {
            var gVal = new GenericValidator();
            if (bank == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(bank.FullName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Bank_Name_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(bank.SortCode))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Bank_Sort_Code_Error;
                return gVal;
            }
            gVal.Code = 5;
            gVal.Error = "";
            return gVal;
        }
        #endregion

    }
}
