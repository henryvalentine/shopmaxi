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
    public class EstimateController : Controller
	{ 
        #region Actions
	   
        public ActionResult GetEstimates(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<EstimateObject> filteredEstimateObjects;
                int countG;

                var pagedParentMenuObjects = new EstimateServices().GetEstimateObjects(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new EstimateServices().SearchEstimates(param.sSearch);
                    countG = filteredEstimateObjects.Count();
                }
                else
                {
                    filteredEstimateObjects = pagedParentMenuObjects;
                }

                if (!filteredEstimateObjects.Any())
                {
                    return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
                }
                
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<EstimateObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.CustomerName : sortColumnIndex == 2 ? c.DateCreatedStr :
                  sortColumnIndex == 3 ? c.LastUpdatedStr : sortColumnIndex == 4 ? c.AmountDueStr : sortColumnIndex == 5 ? c.NetAmountStr : sortColumnIndex == 6 ? c.GeneratedByEmployee : c.InvoiceStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredEstimateObjects = sortDirection == "desc" ? filteredEstimateObjects.OrderBy(orderingFunction) : filteredEstimateObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredEstimateObjects; 

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.EstimateNumber,
                                 c.CustomerName, c.DateCreatedStr,
                                 c.AmountDueStr, c.NetAmountStr, c.GeneratedByEmployee, c.InvoiceStatus
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
                return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult GetOutletEstimates(JQueryDataTableParamModel param, int outletId)
        {
            try
            {
                IEnumerable<EstimateObject> filteredEstimateObjects;
                int countG;

                var pagedParentMenuObjects = new EstimateServices().GetEstimatesByOutlet(param.iDisplayLength, param.iDisplayStart, out countG, outletId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new EstimateServices().SearchOutletEstimate(param.sSearch, outletId);
                    countG = filteredEstimateObjects.Count();
                }
                else
                {
                    filteredEstimateObjects = pagedParentMenuObjects;
                }

                if (!filteredEstimateObjects.Any())
                {
                    return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<EstimateObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.CustomerName : sortColumnIndex == 2 ? c.DateCreatedStr :
                  sortColumnIndex == 3 ? c.LastUpdatedStr : sortColumnIndex == 4 ? c.AmountDueStr : sortColumnIndex == 5 ? c.NetAmountStr : sortColumnIndex == 6 ? c.GeneratedByEmployee : c.InvoiceStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredEstimateObjects = sortDirection == "desc" ? filteredEstimateObjects.OrderBy(orderingFunction) : filteredEstimateObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredEstimateObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.EstimateNumber,
                                 c.CustomerName, c.DateCreatedStr,
                                 c.AmountDueStr, c.NetAmountStr, c.GeneratedByEmployee, c.InvoiceStatus
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
                return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEstimatesByEmployee(JQueryDataTableParamModel param, long employeeId)
        {
            try
            {
                IEnumerable<EstimateObject> filteredEstimateObjects;
                int countG;

                var pagedParentMenuObjects = new EstimateServices().GetEstimatesByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, employeeId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new EstimateServices().SearchEmployeeEstimate(param.sSearch, employeeId);
                    countG = filteredEstimateObjects.Count();
                }
                else
                {
                    filteredEstimateObjects = pagedParentMenuObjects;
                }

                if (!filteredEstimateObjects.Any())
                {
                    return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<EstimateObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.CustomerName : sortColumnIndex == 2 ? c.DateCreatedStr :
                  sortColumnIndex == 3 ? c.LastUpdatedStr : sortColumnIndex == 4 ? c.AmountDueStr : sortColumnIndex == 5 ? c.NetAmountStr : sortColumnIndex == 6 ? c.GeneratedByEmployee : c.InvoiceStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredEstimateObjects = sortDirection == "desc" ? filteredEstimateObjects.OrderBy(orderingFunction) : filteredEstimateObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredEstimateObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.EstimateNumber,
                                 c.CustomerName, c.DateCreatedStr,
                                 c.AmountDueStr, c.NetAmountStr, c.GeneratedByEmployee, c.InvoiceStatus
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
                return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMyEstimates(JQueryDataTableParamModel param)
        {
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
                }

                
                IEnumerable<EstimateObject> filteredEstimateObjects;
                int countG;

                var pagedParentMenuObjects = new EstimateServices().GetEstimatesByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, userInfo.UserProfile.EmployeeId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new EstimateServices().SearchEmployeeEstimate(param.sSearch, userInfo.UserProfile.Id);
                    countG = filteredEstimateObjects.Count();
                }
                else
                {
                    filteredEstimateObjects = pagedParentMenuObjects;
                }

                if (!filteredEstimateObjects.Any())
                {
                    return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<EstimateObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.CustomerName : sortColumnIndex == 2 ? c.DateCreatedStr :
                  sortColumnIndex == 3 ? c.LastUpdatedStr : sortColumnIndex == 4 ? c.AmountDueStr : sortColumnIndex == 5 ? c.NetAmountStr : sortColumnIndex == 6 ? c.GeneratedByEmployee : c.InvoiceStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredEstimateObjects = sortDirection == "desc" ? filteredEstimateObjects.OrderBy(orderingFunction) : filteredEstimateObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredEstimateObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.EstimateNumber,
                                 c.CustomerName, c.DateCreatedStr,
                                 c.AmountDueStr, c.NetAmountStr, c.GeneratedByEmployee, c.InvoiceStatus
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
                return Json(new List<EstimateObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AddEstimate(EstimateObject estimate)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateEstimate(estimate);
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

                estimate.ConvertedToInvoice = false;
                estimate.StoreOutletId = userInfo.UserProfile.StoreOutletId;
                estimate.CreatedById = userInfo.UserProfile.Id;
                estimate.OutletId = userInfo.UserProfile.StoreOutletId;
                estimate.ContactPersonId = userInfo.UserProfile.EmployeeId;
                estimate.DateCreated = DateTime.Now;
                estimate.LastUpdated = DateTime.Now;
                var estimateNumber = "";
                long customerId;
                var k = new EstimateServices().AddEstimate(estimate, out estimateNumber, out customerId);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.CustomerId = customerId;
                gVal.Error = message_Feedback.Insertion_Success;
                gVal.Date = estimate.DateCreated.ToString("dd/MM/yyyy");
                gVal.ReferenceCode = estimateNumber;
                gVal.Time = estimate.DateCreated.ToString("hh:mm:ss tt");
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
        public ActionResult EditEstimate(EstimateObject estimate)
        {
            var gVal = new GenericValidator();
            try
            {
                    var valStatus = ValidateEstimate(estimate);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new EstimateServices().UpdateEstimate(estimate);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate: message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = 5;
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

        public ActionResult GetEstimate(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
                }
                var estimate = new EstimateServices().GetEstimate(id);
                return Json(estimate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEstimateDetails(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
                }
                var estimateDetails = new EstimateServices().GetEstimateDetails(id);
                return Json(estimateDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEstimateByRef(string refNumber)  
        {
            try
            {
                if (string.IsNullOrEmpty(refNumber))
                {
                    return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
                }
                var estimateDetails = new EstimateServices().GetEstimateByRef(refNumber);
                return Json(estimateDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEstimateInvoice(string estimateNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(estimateNumber))
                {
                    return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
                }
                var estimateDetails = new EstimateServices().GetSalesDetails(estimateNumber);
                return Json(estimateDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new EstimateObject(), JsonRequestBehavior.AllowGet);
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
        public ActionResult ConvertEstimateToInvoice(string estimateNumber)
        {
            try
            {
                var selectables = new EstimateServices().ConvertEstimateToInvoice(estimateNumber);
                return Json(selectables, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteEstimateItem(long id)   
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

                var status = new EstimateServices().DeleteEstimateItem(id);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult DeleteEstimate(long id)
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

                var status = new EstimateServices().DeleteEstimate(id);
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
                    PaymentMethods = new StorePaymentMethodServices().GetStorePaymentMethods() ?? new List<StorePaymentMethodObject>(),
                    Customers = new SaleServices().GetCustomers()
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
        
        private GenericValidator ValidateEstimate(EstimateObject estimate)
        {
            var gVal = new GenericValidator();
            if (estimate == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (estimate.AmountDue < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Derived_Total_Cost_Error;
                return gVal;
            }
           
            if (!estimate.EstimateItemObjects.Any())
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Estimate_Items_Error;
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