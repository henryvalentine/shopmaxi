using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ShopKeeper.GenericHelpers;
using ShopKeeper.Properties;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
    public class TransferNoteController : Controller
	{ 
        #region Actions
	   
        public ActionResult GetTransferNotes(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<TransferNoteObject> filteredTransferNoteObjects;
                int countG;

                var pagedParentMenuObjects = new TransferNoteServices().GetTransferNoteObjects(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredTransferNoteObjects = new TransferNoteServices().SearchTransferNotes(param.sSearch);
                    countG = filteredTransferNoteObjects.Count();
                }
                else
                {
                    filteredTransferNoteObjects = pagedParentMenuObjects;
                }

                if (!filteredTransferNoteObjects.Any())
                {
                    return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
                }
                
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<TransferNoteObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.TransferNoteNumber : sortColumnIndex == 2 ? c.DateGeneratedStr :
                  sortColumnIndex == 3 ? c.SourceOutletName : sortColumnIndex == 4 ? c.TargetOutletName : sortColumnIndex == 5 ? c.GeneratedBy : sortColumnIndex == 6 ? c.TotalAmountStr : c.StatusStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredTransferNoteObjects = sortDirection == "desc" ? filteredTransferNoteObjects.OrderBy(orderingFunction) : filteredTransferNoteObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredTransferNoteObjects; 

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.TransferNoteNumber, c.TotalAmountStr, c.DateGeneratedStr,
                                  c.SourceOutletName, c.TargetOutletName, c.GeneratedBy, c.StatusStr
                             };

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = countG,
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult GetOutletTransferNotes(JQueryDataTableParamModel param, int outletId)
        {
            try
            {
                IEnumerable<TransferNoteObject> filteredTransferNoteObjects;
                int countG;

                var pagedParentMenuObjects = new TransferNoteServices().GetTransferNotesByOutlet(param.iDisplayLength, param.iDisplayStart, out countG, outletId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredTransferNoteObjects = new TransferNoteServices().SearchOutletTransferNote(param.sSearch, outletId);
                    countG = filteredTransferNoteObjects.Count();
                }
                else
                {
                    filteredTransferNoteObjects = pagedParentMenuObjects;
                }

                if (!filteredTransferNoteObjects.Any())
                {
                    return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<TransferNoteObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.TransferNoteNumber : sortColumnIndex == 2 ? c.DateGeneratedStr :
                  sortColumnIndex == 3 ? c.SourceOutletName : sortColumnIndex == 4 ? c.TargetOutletName : sortColumnIndex == 5 ? c.GeneratedBy : sortColumnIndex == 6 ? c.TotalAmountStr : c.StatusStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredTransferNoteObjects = sortDirection == "desc" ? filteredTransferNoteObjects.OrderBy(orderingFunction) : filteredTransferNoteObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredTransferNoteObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.TransferNoteNumber, c.TotalAmountStr, c.DateGeneratedStr,
                                  c.SourceOutletName, c.TargetOutletName, c.GeneratedBy, c.StatusStr
                             };

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = countG,
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTransferNotesByEmployee(JQueryDataTableParamModel param, long employeeId)
        {
            try
            {
                IEnumerable<TransferNoteObject> filteredTransferNoteObjects;
                int countG;

                var pagedParentMenuObjects = new TransferNoteServices().GetTransferNotesByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, employeeId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredTransferNoteObjects = new TransferNoteServices().SearchEmployeeTransferNote(param.sSearch, employeeId);
                    countG = filteredTransferNoteObjects.Count();
                }
                else
                {
                    filteredTransferNoteObjects = pagedParentMenuObjects;
                }

                if (!filteredTransferNoteObjects.Any())
                {
                    return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<TransferNoteObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.TransferNoteNumber : sortColumnIndex == 2 ? c.DateGeneratedStr :
                  sortColumnIndex == 3 ? c.SourceOutletName : sortColumnIndex == 4 ? c.TargetOutletName : sortColumnIndex == 5 ? c.GeneratedBy : sortColumnIndex == 6 ? c.TotalAmountStr : c.StatusStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredTransferNoteObjects = sortDirection == "desc" ? filteredTransferNoteObjects.OrderBy(orderingFunction) : filteredTransferNoteObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredTransferNoteObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.TransferNoteNumber, c.TotalAmountStr, c.DateGeneratedStr,
                                  c.SourceOutletName, c.TargetOutletName, c.GeneratedBy, c.StatusStr
                             };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = countG,
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMyTransferNotes(JQueryDataTableParamModel param)
        {
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
                }

                
                IEnumerable<TransferNoteObject> filteredTransferNoteObjects;
                int countG;

                var pagedParentMenuObjects = new TransferNoteServices().GetTransferNotesByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, userInfo.UserProfile.Id);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredTransferNoteObjects = new TransferNoteServices().SearchEmployeeTransferNote(param.sSearch, userInfo.UserProfile.Id);
                    countG = filteredTransferNoteObjects.Count();
                }
                else
                {
                    filteredTransferNoteObjects = pagedParentMenuObjects;
                }

                if (!filteredTransferNoteObjects.Any())
                {
                    return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<TransferNoteObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.TransferNoteNumber : sortColumnIndex == 2 ? c.DateGeneratedStr :
                  sortColumnIndex == 3 ? c.SourceOutletName : sortColumnIndex == 4 ? c.TargetOutletName : sortColumnIndex == 5 ? c.GeneratedBy : sortColumnIndex == 6 ? c.TotalAmountStr : c.StatusStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredTransferNoteObjects = sortDirection == "desc" ? filteredTransferNoteObjects.OrderBy(orderingFunction) : filteredTransferNoteObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredTransferNoteObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.TransferNoteNumber, c.DateGeneratedStr,
                                  c.SourceOutletName, c.TargetOutletName, c.GeneratedBy, c.TotalAmountStr, c.DateTransferdStr, c.StatusStr
                             };

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = countG,
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<TransferNoteObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddTransferNote(TransferNoteObject transferNote)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateTransferNote(transferNote);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your session has timed out.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (!transferNote.TransferNoteItemObjects.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sold_Item_List_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }


                if (transferNote.TransferNoteItemObjects.Any(y => y.TotalQuantityRaised < 1 || y.Rate < 1 || y.TotalAmountRaised < 1))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please review the selected product(s) and try again";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                
                transferNote.Status = (int)TransfereNoteStatus.Pending;
                transferNote.GeneratedByUserId = userInfo.UserProfile.Id;
                transferNote.DateGenerated = DateTime.Now;
                var transferNoteNumber = "";

                var k = new TransferNoteServices().AddTransferNote(transferNote, out transferNoteNumber);
                if (k < 1)
                {
                    gVal.Error = message_Feedback.Insertion_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.Error = message_Feedback.Insertion_Success;
                gVal.Date = transferNote.DateGenerated.ToString("dd/MM/yyyy");
                gVal.ReferenceCode = transferNoteNumber;
                gVal.Time = transferNote.DateGenerated.ToString("hh:mm:ss tt");
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult EditTransferNote(TransferNoteObject transferNote)
        {
            var gVal = new GenericValidator();
            try
            {
                    var valStatus = ValidateTransferNote(transferNote);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var userInfo = GetSignedOnUser();
                    if (userInfo == null || userInfo.UserProfile.Id < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Your session has timed out.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                
                    var k = new TransferNoteServices().UpdateTransferNote(transferNote);
                    if (k < 1)
                    {
                        gVal.Error = message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = k;
                    gVal.ReferenceCode = transferNote.TransferNoteNumber;
                    gVal.Error = message_Feedback.Update_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTransferNote(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new TransferNoteObject(), JsonRequestBehavior.AllowGet);
                }
                var transferNote = new TransferNoteServices().GetTransferNote(id);
                return Json(transferNote, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new TransferNoteObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTransferNoteDetails(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new TransferNoteObject(), JsonRequestBehavior.AllowGet);
                }
                var transferNoteDetails = new TransferNoteServices().GetTransferNoteDetails(id);
                return Json(transferNoteDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new TransferNoteObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTransferNoteByRef(string refNumber)  
        {
            try
            {
                if (string.IsNullOrEmpty(refNumber))
                {
                    return Json(new TransferNoteObject(), JsonRequestBehavior.AllowGet);
                }
                var transferNoteDetails = new TransferNoteServices().GetTransferNoteByRef(refNumber);
                return Json(transferNoteDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new TransferNoteObject(), JsonRequestBehavior.AllowGet);
            }
        }

       public ActionResult GetCustomers()
        {
            try
            {

                var status = new CustomerServices().GetCustomers();
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ConvertTransferNoteToInvoice(string transferNoteNumber)
        {
            try
            {
                var selectables = new TransferNoteServices().ConvertTransferNoteToInvoice(transferNoteNumber);
                return Json(selectables, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteTransferNoteItem(long id)   
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid selection!";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var status = new TransferNoteServices().DeleteTransferNoteItem(id);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult DeleteTransferNote(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid selection!";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var status = new TransferNoteServices().DeleteTransferNote(id);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProducts(int page, int itemsPerPage)
        {
            try
            {
                var mainOutlet = new StoreItemStockServices().GetStoreDefaultOutlet();
                if (mainOutlet == null || mainOutlet.StoreOutletId < 1)
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }

                var storeItems = new ItemPriceServices().GetProducts(page, itemsPerPage);
                if (!storeItems.Any())
                {
                    return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
                }
                return Json(storeItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetListObjects()
        {
            try
            {
                var genericObject = new StockGenericObject
                {
                    CustomerTypes = new StoreCustomerTypeServices().GetStoreCustomerTypes() ?? new List<StoreCustomerTypeObject>(),
                    PaymentMethods = new StorePaymentMethodServices().GetStorePaymentMethods() ?? new List<StorePaymentMethodObject>()
                    //Customers = new SaleServices().GetCustomers()
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        
        #region Helpers

        private List<StoreCustomerTypeObject> GetCustomerTypes()
        {
            return new StoreCustomerTypeServices().GetStoreCustomerTypes() ?? new List<StoreCustomerTypeObject>();
        }
        private UserInfo GetSignedOnUser()
        {
            if (Session["_signonInfo"] == null)
            {
                return new UserInfo();
            }

            var userInfo = Session["_signonInfo"] as UserInfo;
            if (userInfo == null || userInfo.UserProfile == null || userInfo.UserProfile.Id < 1)
            {
                return new UserInfo();
            }

            return userInfo;
        }
        
        private GenericValidator ValidateTransferNote(TransferNoteObject transferNote)
        {
            var gVal = new GenericValidator();
            if (transferNote == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (transferNote.TotalAmount < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Derived_Total_Cost_Error;
                return gVal;
            }
           
            if (!transferNote.TransferNoteItemObjects.Any())
            {
                gVal.Code = -1;
                gVal.Error = "Please review the selected products and try again.";
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }

        private string PrepareAndSaveFile(HttpPostedFileBase file, string formerFilePath)
        {
            try
            {
                const string mainPath = "~/Stores/Uploads";
                var subPath = HostingEnvironment.MapPath(mainPath);
                if (string.IsNullOrEmpty(subPath))
                {
                    return null;
                }

                if (!Directory.Exists(subPath))
                {
                    Directory.CreateDirectory(subPath);
                    var dInfo = new DirectoryInfo(subPath);
                    var dSecurity = dInfo.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dInfo.SetAccessControl(dSecurity);
                }
                var path = "";
                if (!SaveToFolder(file, ref path, subPath, formerFilePath))
                {
                    return "";
                }
                return PhysicalToVirtualPathMapper.MapPath(path).Replace("~", "");
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
                    path = newPathv.Replace("~", string.Empty);
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

    }
}