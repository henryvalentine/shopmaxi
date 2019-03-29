using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Newtonsoft.Json;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
    public class PurchaseorderController : Controller
	{
        #region Actions

        public ActionResult GetPurchaseOrders(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<PurchaseOrderObject> filteredPurchaseOrderObjects;
                int countG;

                var pagedParentMenuObjects = new PurchaseOrderServices().GetPurchaseOrderObjects(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPurchaseOrderObjects = new PurchaseOrderServices().SearchPurchaseOrders(param.sSearch);
                    countG = filteredPurchaseOrderObjects.Count();
                }
                else
                {
                    filteredPurchaseOrderObjects = pagedParentMenuObjects;
                }

                if (!filteredPurchaseOrderObjects.Any())
                {
                    return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<PurchaseOrderObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.SupplierName : sortColumnIndex == 2 ? c.GeneratedByEmployeeName :
                  sortColumnIndex == 3 ? c.DerivedTotalCostStr : sortColumnIndex == 4 ? c.DateTimePlacedStr : sortColumnIndex == 5 ? c.ActualDeliveryDateStr : c.DeliveryStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredPurchaseOrderObjects = sortDirection == "desc" ? filteredPurchaseOrderObjects.OrderBy(orderingFunction) : filteredPurchaseOrderObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredPurchaseOrderObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.PurchaseOrderId), c.PurchaseOrderNumber,
                                 c.SupplierName, c.GeneratedByEmployeeName, c.DerivedTotalCostStr,
                                 c.DateTimePlacedStr, c.ActualDeliveryDateStr, c.DeliveryStatus
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
                return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOutletPurchaseOrders(JQueryDataTableParamModel param, int outletId)
        {
            try
            {
                IEnumerable<PurchaseOrderObject> filteredPurchaseOrderObjects;
                int countG;

                var pagedParentMenuObjects = new PurchaseOrderServices().GetPurchaseOrdersByOutlet(param.iDisplayLength, param.iDisplayStart, out countG, outletId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPurchaseOrderObjects = new PurchaseOrderServices().SearchOutletPurchaseOrder(param.sSearch, outletId);
                    countG = filteredPurchaseOrderObjects.Count();
                }
                else
                {
                    filteredPurchaseOrderObjects = pagedParentMenuObjects;
                }

                if (!filteredPurchaseOrderObjects.Any())
                {
                    return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<PurchaseOrderObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.SupplierName : sortColumnIndex == 2 ? c.GeneratedByEmployeeName :
                  sortColumnIndex == 3 ? c.DerivedTotalCostStr : sortColumnIndex == 4 ? c.DateTimePlacedStr : sortColumnIndex == 5 ? c.ActualDeliveryDateStr : c.DeliveryStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredPurchaseOrderObjects = sortDirection == "desc" ? filteredPurchaseOrderObjects.OrderBy(orderingFunction) : filteredPurchaseOrderObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredPurchaseOrderObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.PurchaseOrderId), c.PurchaseOrderNumber,
                                 c.SupplierName, c.GeneratedByEmployeeNo, c.DerivedTotalCostStr,
                                 c.DateTimePlacedStr, c.ActualDeliveryDateStr, c.DeliveryStatus
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
                return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPurchaseOrdersByEmployee(JQueryDataTableParamModel param, long employeeId)
        {
            try
            {
                IEnumerable<PurchaseOrderObject> filteredPurchaseOrderObjects;
                int countG;

                var pagedParentMenuObjects = new PurchaseOrderServices().GetPurchaseOrdersByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, employeeId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPurchaseOrderObjects = new PurchaseOrderServices().SearchEmployeePurchaseOrder(param.sSearch, employeeId);
                    countG = filteredPurchaseOrderObjects.Count();
                }
                else
                {
                    filteredPurchaseOrderObjects = pagedParentMenuObjects;
                }

                if (!filteredPurchaseOrderObjects.Any())
                {
                    return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<PurchaseOrderObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.SupplierName : sortColumnIndex == 2 ? c.GeneratedByEmployeeName :
                  sortColumnIndex == 3 ? c.DerivedTotalCostStr : sortColumnIndex == 4 ? c.DateTimePlacedStr : sortColumnIndex == 5 ? c.ActualDeliveryDateStr : c.DeliveryStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredPurchaseOrderObjects = sortDirection == "desc" ? filteredPurchaseOrderObjects.OrderBy(orderingFunction) : filteredPurchaseOrderObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredPurchaseOrderObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.PurchaseOrderId), c.PurchaseOrderNumber,
                                 c.SupplierName, c.GeneratedByEmployeeNo, c.DerivedTotalCostStr, c.DateTimePlacedStr, c.ActualDeliveryDateStr, c.DeliveryStatus
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
                return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMyPurchaseOrders(JQueryDataTableParamModel param)
        {
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
                }


                IEnumerable<PurchaseOrderObject> filteredPurchaseOrderObjects;
                int countG;

                var pagedParentMenuObjects = new PurchaseOrderServices().GetPurchaseOrdersByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, userInfo.UserProfile.EmployeeId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPurchaseOrderObjects = new PurchaseOrderServices().SearchEmployeePurchaseOrder(param.sSearch, userInfo.UserProfile.Id);
                    countG = filteredPurchaseOrderObjects.Count();
                }
                else
                {
                    filteredPurchaseOrderObjects = pagedParentMenuObjects;
                }

                if (!filteredPurchaseOrderObjects.Any())
                {
                    return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<PurchaseOrderObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.SupplierName : sortColumnIndex == 2 ? c.DerivedTotalCostStr : sortColumnIndex == 3 ? c.DateTimePlacedStr : sortColumnIndex == 4 ? c.ActualDeliveryDateStr : c.DeliveryStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredPurchaseOrderObjects = sortDirection == "desc" ? filteredPurchaseOrderObjects.OrderBy(orderingFunction) : filteredPurchaseOrderObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredPurchaseOrderObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.PurchaseOrderId), c.PurchaseOrderNumber,
                                 c.SupplierName, c.GeneratedByEmployeeName, c.DerivedTotalCostStr,
                                 c.DateTimePlacedStr, c.ActualDeliveryDateStr, c.DeliveryStatus
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
                return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddPurchaseOrder(PurchaseOrderObject purchaseOrder)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidatePurchaseOrder(purchaseOrder);
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

                purchaseOrder.StoreOutletId = userInfo.UserProfile.StoreOutletId;
                purchaseOrder.StatusCode = (int)PurchaseOrderDeliveryStatus.Pending;
                purchaseOrder.GeneratedById = userInfo.UserProfile.EmployeeId;
                purchaseOrder.DateTimePlaced = DateTime.Now;

                string purchaseOrderNumber;
                var k = new PurchaseOrderServices().AddPurchaseOrder(purchaseOrder, out purchaseOrderNumber);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.PurchaseOrderNumber = purchaseOrderNumber;
                gVal.Error = message_Feedback.Insertion_Success;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditPurchaseOrder(PurchaseOrderObject purchaseOrder)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidatePurchaseOrder(purchaseOrder);
                if (valStatus.Code < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var k = new PurchaseOrderServices().UpdatePurchaseOrder(purchaseOrder);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
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

        public ActionResult GetPurchaseOrder(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new PurchaseOrderObject(), JsonRequestBehavior.AllowGet);
                }
                var purchaseOrder = new PurchaseOrderServices().GetPurchaseOrder(id);
                return Json(purchaseOrder, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new PurchaseOrderObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPurchaseOrderDetails(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    return Json(new PurchaseOrderObject(), JsonRequestBehavior.AllowGet);
                }
                var purchaseOrderDetails = new PurchaseOrderServices().GetPurchaseOrderDetails(id);
                return Json(purchaseOrderDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new PurchaseOrderObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSelectables(string subdomain)
        {
            var gVal = new GenericValidator();
            try
            {
                var selectables = new PurchaseOrderServices().GetSelectables();
                return Json(selectables, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteOrderItem(long orderItemId)
        {
            var gVal = new GenericValidator();
            try
            {
                if (orderItemId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid selection!";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var status = new PurchaseOrderServices().DeleteOrderItem(orderItemId);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletePurchaseOrderItemDelivery(long id)
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

                var status = new PurchaseOrderServices().DeletePurchaseOrderItemDelivery(id);
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
        [HttpPost]
        public ActionResult AddPurchaseOrderDelivery(PurchaseOrderObject deliveryObject)
        {
            try
            {
                var mainOutlet = new StoreItemStockServices().GetStoreDefaultOutlet();
                if (mainOutlet == null || mainOutlet.StoreOutletId < 1)
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }

                //var status = new PurchaseOrderServices().AddPurchaseOrderDelivery(deliveryObject);
                //return Json(status, JsonRequestBehavior.AllowGet);
                return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditPurchaseOrderDelivery(PurchaseOrderObject deliveryObject)
        {
            try
            {
                var mainOutlet = new StoreItemStockServices().GetStoreDefaultOutlet();
                if (mainOutlet == null || mainOutlet.StoreOutletId < 1)
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }

                //var status = new PurchaseOrderServices().EditPurchaseOrderDelivery(deliveryObject);
                //return Json(status, JsonRequestBehavior.AllowGet);
                return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ProcessPurchaseorderDelivery(POrderInfo delivery)
        {
            var gVal = new GenericValidator();
            try
            {
                var retStatus = ValidatePurchaseOrderItemDelivery(delivery.DeliveredItems);
                if (retStatus.Code < 1)
                {
                    return Json(retStatus, JsonRequestBehavior.AllowGet);
                }

                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your session has timed out.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                delivery.DeliveredItems.ForEach(d =>
                {
                    d.ReceivedById = userInfo.UserProfile.EmployeeId;
                });

                var status = new PurchaseOrderServices().ProcessPurchaseOrderDeliveries(delivery);
                if (status < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Process failed. Please try again later.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = status;
                gVal.Error = "Process was successfully completed.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                gVal.Code = -1;
                gVal.Error = "An unknown error was encountered. Please try again later.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RefreshSession()
        {
            return Json(5, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProcessPurchaseorderInvoice(HttpPostedFileBase file, long invoiceId, long purchaseOrderId, string referenceCode, int statusCode, String dueDate, string dateSent, string attachment)
        {
            var gVal = new GenericValidator();
            try
            {
                if (purchaseOrderId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invoice information could not be processed.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var invoiceItem = new InvoiceJson
                {
                    InvoiceId = invoiceId,
                    PurchaseOrderId = purchaseOrderId,
                    ReferenceCode = referenceCode,
                    StatusCode = statusCode
                };

                if (!string.IsNullOrEmpty(dueDate))
                {
                    DateTime d1;
                    var res1 = DateTime.TryParse(dueDate, out d1);
                    if (res1 && d1.Year > 1)
                    {
                        invoiceItem.DueDate = d1;
                    }
                    else
                    {
                        gVal.Code = -1;
                        gVal.Error = "Request could not be processed. Please try again later.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }

                if (!string.IsNullOrEmpty(dateSent))
                {
                    DateTime d2;
                    var res2 = DateTime.TryParse(dateSent, out d2);
                    if (res2 && d2.Year > 1)
                    {
                        invoiceItem.DateSent = d2;

                    }
                    else
                    {
                        gVal.Code = -1;
                        gVal.Error = "Invalid invoice reception date.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }

                var path = "";
                if (file != null && file.ContentLength > 0)
                {
                    path = PrepareAndSaveFile(file, attachment);
                    if (string.IsNullOrEmpty(path) || path == "/")
                    {
                        gVal.Code = -1;
                        gVal.Error = "Invoice information Could not be saved.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(attachment))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please select an invoice file to upload.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    path = attachment;
                }

                invoiceItem.Attachment = path;

                invoiceItem.StatusCode = (int)PurchaseOrderInvoiceStatus.Accepted;
                invoiceItem.Attachment = path;

                var tx = new PurchaseOrderServices().ProcessPurchaseOrderInvoice(invoiceItem);
                if (tx < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invoice information Could not be processed.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Error = "Invoice was successfully procesed";
                gVal.FilePath = path.Replace("~", "");
                gVal.Code = tx;

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

        private GenericValidator ValidatePurchaseOrder(PurchaseOrderObject purchaseOrder)
        {
            var gVal = new GenericValidator();
            if (purchaseOrder == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (purchaseOrder.DerivedTotalCost < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Derived_Total_Cost_Error;
                return gVal;
            }

            if (purchaseOrder.SupplierId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Supplier_Selection_Error;
                return gVal;
            }

            if (purchaseOrder.AccountId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Chart_Of_Account_Selection_Error;
                return gVal;
            }

            if (!purchaseOrder.PurchaseOrderItemObjects.Any())
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Purchase_Order_Items_Error;
                return gVal;
            }

            if (purchaseOrder.ExpectedDeliveryDate.Year == 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Expected_Delivery_Date_Error;
                return gVal;
            }



            gVal.Code = 5;
            return gVal;
        }

        private GenericValidator ValidatePurchaseOrderItemDelivery(List<PurchaseOrderItemDeliveryObject> items)
        {
            var gVal = new GenericValidator();
            if (!items.Any())
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            foreach (var d in items)
            {
                if (d.QuantityDelivered < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Quantity_Delivered_Error;
                    return gVal;
                }

                if (d.DateDelivered.Year == 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Date_Delivered_Error;
                    return gVal;
                }

                if (d.ExpiryDate != null && d.ExpiryDate.Value.Year == 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Invalid_Expiry_Date;
                    return gVal;
                }
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