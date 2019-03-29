using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Web.Mvc;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Mandrill;
using Mandrill.Model;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;
using UserProfileObject = Shopkeeper.DataObjects.DataObjects.Store.UserProfileObject;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class SalesController : Controller
	{
        #region Actions
        private StoreOutletObject GetOutlet()
        {
            return new StoreOutletServices().GetStoreOutlet(1) ?? new StoreOutletObject();
        }

        private EmployeeObject GetEmployee()
        {
            return new EmployeeServices().GetEmployee(1) ?? new EmployeeObject();
        }
        private StoreOutletObject RetrieveOutlet()
        {
            if (Session["_outlet"] == null)
            {
                return new StoreOutletServices().GetStoreOutlet(1) ?? new StoreOutletObject();
            }
            var outletDetails = Session["_outlet"] as StoreOutletObject;
            if (outletDetails == null || outletDetails.StoreOutletId < 1)
            {
                return new StoreOutletServices().GetStoreOutlet(1) ?? new StoreOutletObject();
            }
            return outletDetails;
        }

        public ActionResult GetDueInvoices(int itemsPerPage, int pageNumber)
        {
            try
            {
                var countG = 0;

                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var pagedSaleObjects = new ReportServices().GetDueInvoices(itemsPerPage, pageNumber, out countG);

                if (!pagedSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result = from c in pagedSaleObjects
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.DateStr, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRevokedSales(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SaleObject> filteredSaleObjects;

                var countG = 0;

                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var pagedSaleObjects = new ReportServices().GetRevokedSales(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new ReportServices().SearchRevokedSales(param.sSearch);
                }
                else
                {
                    filteredSaleObjects = pagedSaleObjects;
                }

                if (!filteredSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }
                //GetDueInvoices(int? itemsPerPage, int? pageNumber, out int countG)  
                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber : sortColumnIndex == 2 ? c.CustomerName :
                  sortColumnIndex == 3 ? c.AmountDueStr : sortColumnIndex == 4 ? c.NetAmountStr : sortColumnIndex == 5 ? c.DateStr : c.DateRevokedStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "desc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredSaleObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.NetAmountStr,
                                 c.DateStr, c.DateRevokedStr
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
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetContactPersonInvoices(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SaleObject> filteredSaleObjects;

                int countG;

                var userInfo = GetSignedOnUser();

                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var pagedSaleObjects = new ReportServices().GetContactPersonInvoices(param.iDisplayLength, param.iDisplayStart, out countG, userInfo.UserProfile.EmployeeId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new ReportServices().SearchContactPersonInvoices(param.sSearch, userInfo.UserProfile.EmployeeId);
                }
                else
                {
                    filteredSaleObjects = pagedSaleObjects;
                }

                if (!filteredSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber : sortColumnIndex == 2 ? c.CustomerName :
                  sortColumnIndex == 3 ? c.AmountDueStr : sortColumnIndex == 4 ? c.NetAmountStr : sortColumnIndex == 5 ? c.PaymentStatus : c.DateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "desc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredSaleObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.CustomerName, c.DateStr, c.AmountDueStr, c.NetAmountStr,
                                  c.PaymentStatus
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
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMySalesReport(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SaleObject> filteredSaleObjects;

                int countG;

                var userInfo = GetSignedOnUser();

                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var pagedSaleObjects = new ReportServices().GetEmployeeSalesReport(param.iDisplayLength, param.iDisplayStart, out countG, userInfo.UserProfile.EmployeeId);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new ReportServices().SearchEmployeeSalesReport(param.sSearch, userInfo.UserProfile.EmployeeId);
                }
                else
                {
                    filteredSaleObjects = pagedSaleObjects;
                }

                if (!filteredSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber : sortColumnIndex == 2 ? c.CustomerName :
                  sortColumnIndex == 3 ? c.AmountDueStr : sortColumnIndex == 4 ? c.NetAmountStr : sortColumnIndex == 5 ? c.PaymentStatus : c.DateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "desc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredSaleObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.CustomerName, c.DateStr, c.AmountDueStr, c.NetAmountStr,
                                  c.PaymentStatus
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
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAdminSalesReport(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SaleObject> filteredSaleObjects;

                int countG;

                var pagedSaleObjects = new ReportServices().GetAdminSalesReport(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new ReportServices().SearchAdminSalesReport(param.sSearch);
                }
                else
                {
                    filteredSaleObjects = pagedSaleObjects;
                }

                if (!filteredSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber : sortColumnIndex == 2 ? c.CustomerName :
                  sortColumnIndex == 3 ? c.AmountDueStr : sortColumnIndex == 4 ? c.NetAmountStr : sortColumnIndex == 5 ? c.PaymentStatus : c.DateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "desc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredSaleObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.CustomerName, c.DateStr, c.AmountDueStr, c.NetAmountStr,
                                  c.PaymentStatus
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
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUncompletedTransaction(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<SaleObject> filteredSaleObjects;

                var countG = 0;

                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var pagedSaleObjects = new ReportServices().GetUncompletedTransactions(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new ReportServices().SearchUncompletedTransactions(param.sSearch);
                }
                else
                {
                    filteredSaleObjects = pagedSaleObjects;
                }

                if (!filteredSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber : sortColumnIndex == 2 ? c.CustomerName :
                  sortColumnIndex == 3 ? c.NetAmountStr : sortColumnIndex == 4 ? c.AmountPaidStr : c.BalanceStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "desc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredSaleObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.CustomerName, c.DateStr, c.NetAmountStr, c.AmountPaidStr, c.BalanceStr
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
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        private EmployeeObject RetrieveLoggedOnUserInfo()
        {
            if (Session["_user"] == null)
            {
                return new EmployeeServices().GetEmployee(1) ?? new EmployeeObject();
            }
            var employee = Session["_user"] as EmployeeObject;
            if (employee == null || employee.EmployeeId < 1)
            {
                return new EmployeeServices().GetEmployee(1) ?? new EmployeeObject();
            }
            return employee;
        }

        [HttpGet]
        public ActionResult GetSaleObjects(JQueryDataTableParamModel param)
        {
            var gVal = new GenericValidator();
            try
            {
                IEnumerable<SaleObject> filteredSaleObjects;

                var countG = new SaleServices().GetObjectCount();

                var pagedSaleObjects = GetSales(1, param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new SaleServices().Search(param.sSearch);
                }
                else
                {
                    filteredSaleObjects = pagedSaleObjects;
                }

                if (!filteredSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.DateStr : sortColumnIndex == 2 ? c.AmountDue.ToString(CultureInfo.InvariantCulture)
                    : sortColumnIndex == 3 ? c.NumberSoldItems.ToString(CultureInfo.InvariantCulture) : c.RegisterName);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "asc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);
                var displayedUserProfilenels = filteredSaleObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.SaleId), c.DateStr, c.AmountDue.ToString(CultureInfo.InvariantCulture), c.NumberSoldItems.ToString(CultureInfo.InvariantCulture), c.RegisterName };
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
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }


        //[HttpPost]
        //public ActionResult Synch()
        //{
        //    var gVal = new GenericValidator();
        //    var synchStatus = new DataSynchServices().SynchData();
        //    gVal.Code = synchStatus ? 5 : -1;
        //    gVal.Error = synchStatus ? "Data store was successfully updated." : "Process failed";
        //    return Json(gVal, JsonRequestBehavior.AllowGet);

        //}

        [HttpPost]
        public ActionResult AddSale(GenericSaleObject genericSale)
        {
            var gVal = new GenericValidator();

            try
            {
                var valStatus = ValidateSale(genericSale.Sale);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (!genericSale.StoreTransactions.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Transaction_List_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (genericSale.Sale.StoreItemSoldObjects.Any(y => y.QuantitySold < 1 || y.Rate < 1 || y.AmountSold < 1))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please review the selected product(s) and try again";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (!genericSale.Sale.StoreItemSoldObjects.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sold_Item_List_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your session has timed out.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var amountPaid = genericSale.StoreTransactions.Sum(m => m.TransactionAmount);

                var outStanding = genericSale.Sale.NetAmount - amountPaid;
                var balance = amountPaid - genericSale.Sale.NetAmount;

                if (outStanding > 0)
                {
                    genericSale.Sale.Status = (int)SaleStatus.Partly_Paid;
                }
                else
                {
                    if (balance >= 0 || outStanding <= 0)
                    {
                        genericSale.Sale.Status = (int)SaleStatus.Completed;
                    }
                }

                genericSale.Sale.Status = genericSale.Sale.NetAmount.Equals(amountPaid) ? (int)SaleStatus.Completed : (int)SaleStatus.Partly_Paid;

                genericSale.Sale.EmployeeId = userInfo.UserProfile.EmployeeId;
                genericSale.Sale.OutletId = userInfo.UserProfile.StoreOutletId;
                genericSale.Sale.RegisterId = 1;
                genericSale.Sale.Date = DateTime.Now;

                var invoiceRef = "";
                var k = new SaleServices().AddSale(genericSale.Sale, out invoiceRef);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : k == -7 ? "A customer with the same phone Number already exists" : message_Feedback.Insertion_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                List<long> transasactionSuccessList;
                List<long> paymentSuccessList;
                var transactionReports = AddTransactions(genericSale.StoreTransactions, k, out transasactionSuccessList, userInfo.UserProfile);
                if (transactionReports.Any())
                {
                    if (transasactionSuccessList.Any())
                    {
                        var i = DeleteTransactions(transasactionSuccessList);
                    }
                    new SaleServices().DeleteSale(k);
                    gVal.Error = message_Feedback.Process_Failed;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var paymentReports = AddPayments(genericSale.StoreTransactions, k, out paymentSuccessList);
                if (paymentReports.Any())
                {
                    if (paymentSuccessList.Any())
                    {
                        DeletePayments(paymentSuccessList);
                        DeleteTransactions(transasactionSuccessList);
                    }

                    new SaleServices().DeleteSale(k);
                    gVal.Error = message_Feedback.Process_Failed;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }


                if (genericSale.Sale.CustomerId != null && genericSale.Sale.CustomerId > 0)
                {
                    var invoice = new CustomerInvoiceObject
                    {
                        AmountDue = genericSale.Sale.AmountDue,
                        TotalVATAmount = genericSale.Sale.VATAmount,
                        TotalDiscountAmount = genericSale.Sale.DiscountAmount,
                        AmountPaid = amountPaid,
                        CustomerId = (long)genericSale.Sale.CustomerId
                    };

                    var invoiceStatus = new SalePaymentServices().ProcessCustomerInvoice(invoice);
                    if (invoiceStatus < 1)
                    {
                        if (paymentSuccessList.Any())
                        {
                            DeletePayments(paymentSuccessList);
                            DeleteTransactions(transasactionSuccessList);
                        }
                        new SalePaymentServices().BalanceCustomerInvoice(invoice);
                        new SaleServices().DeleteSale(k);
                        gVal.Error = message_Feedback.Process_Failed;
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }

                List<StoreItemSoldObject> itemsoldSuccessList;
                var itemsoldResult = AddSoldItems(genericSale.Sale.StoreItemSoldObjects.ToList(), k, out itemsoldSuccessList);
                if (itemsoldResult.Any())
                {
                    if (itemsoldSuccessList.Any())
                    {
                        DeletePayments(paymentSuccessList);
                        DeleteTransactions(transasactionSuccessList);
                        DeleteSoldItems(itemsoldSuccessList);
                    }

                    new SaleServices().DeleteSale(k);
                    gVal.Error = message_Feedback.Process_Failed;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.Date = genericSale.Sale.Date.ToString("dd/MM/yyyy");
                gVal.Outlet = new StoreOutletObject();
                var employee = RetrieveLoggedOnUserInfo();

                if (employee != null && employee.EmployeeId > 0)
                {
                    gVal.UserName = employee.Name;
                }
                else
                {
                    gVal.UserName = "";
                }

                gVal.ReferenceCode = invoiceRef;
                gVal.Time = genericSale.Sale.Date.ToString("hh:mm:ss tt");
                gVal.Error = message_Feedback.Insertion_Success;
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
        public ActionResult UpdateSoldItemDelivery(List<StoreItemSoldObject> soldItems)
        {
            var gVal = new GenericValidator();

            try
            {
                var k = new StoreItemStockServices().UpdateSoldItemDelivery(soldItems);
                if (!k)
                {
                    gVal.Error = message_Feedback.Update_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

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

        [HttpPost]
        public ActionResult UpdateSalePayment(SaleObject sale)
        {
            var gVal = new GenericValidator();

            try
            {
                var valStatus = ValidateSale(sale);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (!sale.Transactions.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Transaction_List_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your session has timed out.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var k = new SaleServices().UpdateSalePayment(sale, userInfo.UserProfile);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.Date = sale.Date.ToString("dd/MM/yyyy");
                gVal.UserName = userInfo.UserProfile.Name;
                gVal.ReferenceCode = sale.InvoiceNumber;
                gVal.Time = sale.Date.ToString("hh:mm:ss tt");
                gVal.Error = "Transaction payment was successfully updated.";
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
        public ActionResult RefundSale(RefundNoteObject refundNote)
        {
            var gVal = new GenericValidator();

            try
            {
                if (refundNote == null || !refundNote.ReturnedProductObjects.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = "An unknown error was encountered. Please refresh the page and try again.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (refundNote.ReturnedProductObjects.Any(r => r.QuantityReturned < 1 || r.AmountRefunded < 1))
                {
                    gVal.Code = -1;
                    gVal.Error = "An unknown error was encountered. Please refresh the page and try again.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your session has timed out.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                refundNote.OutletId = userInfo.UserProfile.StoreOutletId;
                refundNote.EmployeeId = userInfo.UserProfile.EmployeeId;
                refundNote.DateReturned = DateTime.Now;
                var refundNoteNumber = "";
                var k = new SaleServices().RefundSale(refundNote, out refundNoteNumber);
                if (k < 1)
                {
                    gVal.Error = "The sale refund process failed. Please try again ;later.";
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.RefundNoteNumber = refundNoteNumber;
                gVal.Code = k;
                gVal.Date = refundNote.DateReturned.ToString("dd/MM/yyyy");
                gVal.Error = message_Feedback.Insertion_Success;
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
        public ActionResult EditSale(GenericSaleObject genericSale)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateSale(genericSale.Sale);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (!genericSale.StoreTransactions.Any())
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Transaction_List_Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (!genericSale.Sale.StoreItemSoldObjects.Any())
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Sold_Item_List_Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_sale"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldSale = Session["_sale"] as SaleObject;
                    if (oldSale == null || oldSale.SaleId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var k = new SaleServices().UpdateSale(oldSale);
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

                gVal.Code = -5;
                gVal.Error = message_Feedback.Process_Failed;
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
        public ActionResult AddSalesCustomer(CustomerObject customer)
        {
            var gVal = new GenericValidator();
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your session has timed out.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var valStatus = ValidateCustomer(customer);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                customer.ContactPersonId = userInfo.UserProfile.EmployeeId;
                customer.StoreOutletId = userInfo.UserProfile.StoreOutletId;
                customer.DateProfiled = DateTime.Now;
                var k = new CustomerServices().AddCustomer(customer);
                if (k < 1)
                {
                    gVal.Code = -1;
                    if (k == -3)
                    {
                        gVal.Error = message_Feedback.Duplicate_Mobile_Number;
                    }
                    if (k == -4)
                    {
                        gVal.Error = message_Feedback.Duplicate_Email;
                    }
                    else
                    {
                        gVal.Error = "Customer information could not be processed. Please try again later.";
                    }

                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.Error = message_Feedback.Insertion_Success;
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
        public ActionResult DeleteSale(long id)
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

                var k = new SaleServices().DeleteSale(id);
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

        public ActionResult GetSale(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }

                var sale = new SaleServices().GetSale(id);
                if (id < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }

                return Json(sale, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRefundedSaleNotes(long saleId)
        {
            try
            {
                if (saleId < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }
                var refundNotes = new SaleServices().GetRefundedSaleNotes(saleId);

                return Json(refundNotes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetInvoice(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }
                var sale = new SaleServices().GetInvoice(id);
                if (sale.SaleId < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }

                return Json(sale, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInvoiceForRevoke(string invoiceNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }
                var sale = new SaleServices().GetInvoiceForRevoke(invoiceNumber);
                if (sale.SaleId < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }

                return Json(sale, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSalesInfo(string invoiceNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }
                var sale = new ReportServices().GetSalesReportDetails(invoiceNumber);
                if (sale.SaleId < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }

                return Json(sale, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRevokedDetails(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }
                var sale = new ReportServices().GetRefundedSale(id);
                if (sale.SaleId < 1)
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }

                return Json(sale, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProduct(string sku)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(sku))
                {
                    return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
                }

                //todo: use dynamic OutletId
                var outletId = 1;
                var itemPrices = GetItemPriceObjectBySku(sku, outletId);
                if (!itemPrices.Any())
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }
                return Json(itemPrices, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStates()
        {
            var countries = new StoreStateServices().GetStoreStates() ?? new List<StoreStateObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListObjects()
        {
            try
            {
                var genericObject = new StockGenericObject
                {
                    PaymentMethods = GetPaymentMethods(),
                    //Customers = new SaleServices().GetCustomers(0, 300)
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMoreCustomers(int pageNumber, int itemsPerPage)
        {
            try
            {
                var customers = new SaleServices().GetCustomers(pageNumber, itemsPerPage);
                return Json(customers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPaymenthod()
        {
            try
            {
                var genericObject = new StockGenericObject
                {
                    PaymentMethods = GetPaymentMethods()
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPriceLists(int categoryId)
        {
            try
            {
                if (categoryId < 1)
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }

                //todo: use dynamic OutletId
                var outletId = 1;
                var itemPrices = GetItemPrices(categoryId, outletId);
                if (!itemPrices.Any())
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }
                return Json(itemPrices, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetCustomers()
        {
            var customers = new SaleServices().GetCustomers();
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchItemPriceListByOutlet(string criteria)
        {
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
                }

                var storeItems = new ItemPriceServices().SearchItemPriceListByOutlet(userInfo.UserProfile.StoreOutletId, criteria);
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

        public ActionResult SearchAllItemPriceListByOutlet(string criteria)
        {
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
                }

                var storeItems = new ItemPriceServices().SearchAllItemPriceListByOutlet(userInfo.UserProfile.StoreOutletId, criteria);
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

        public ActionResult GetAllPriceLists(int page, int itemsPerPage)
        {
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
                }

                var storeItems = new ItemPriceServices().GetItemPriceListByOutlet(userInfo.UserProfile.StoreOutletId, page, itemsPerPage);
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

        public ActionResult GetAllItemPriceList(int page, int itemsPerPage)
        {
            try
            {
                var userInfo = GetSignedOnUser();
                if (userInfo == null || userInfo.UserProfile.Id < 1)
                {
                    return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
                }

                var storeItems = new ItemPriceServices().GetItemAllPriceListByOutlet(userInfo.UserProfile.StoreOutletId, page, itemsPerPage);
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

        public ActionResult GetUSBDevices()
        {
            try
            {
                return Json(USBEnumerator.GetUSBDevices(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Helpers
        private GenericValidator ValidateCustomer(CustomerObject customer)
        {
            var gVal = new GenericValidator();
            if (customer == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(customer.MobileNumber))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UserProfile_Mobile_Number;
                return gVal;
            }

            if (string.IsNullOrEmpty(customer.Gender))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UserProfile_Gender_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(customer.LastName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UserProfile_Last_Name_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(customer.OtherNames))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UserProfile_Other_Names_Error;
                return gVal;
            }


            gVal.Code = 5;
            return gVal;
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

        private IEnumerable<SaleObject> GetSales(int outletId, int? itemsPerPage, int? pageNumber)
        {
            return new SaleServices().GetSalesByOutlet(outletId, itemsPerPage, pageNumber) ?? new List<SaleObject>();
        }

        private IEnumerable<GenericValidator> AddPayments(List<StoreTransactionObject> transactions, long saleId, out  List<long> successList)
        {
            var gVal = new GenericValidator();
            var paymentErrorList = new List<GenericValidator>();
            var newsuccessList = new List<long>();
            try
            {
                if (!transactions.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sale_Payment_List_Error;
                    paymentErrorList.Add(gVal);
                    successList = new List<long>();
                    return paymentErrorList;
                }

                transactions.ForEach(m =>
                {

                    var payment = new SalePaymentObject
                    {
                        SaleId = saleId,
                        DatePaid = DateTime.Now,
                        AmountPaid = m.TransactionAmount
                    };

                    var report = ValidatePayment(payment);
                    if (report.Code < 1)
                    {
                        paymentErrorList.Add(report);
                    }
                    else
                    {
                        var y = new SalePaymentServices().AddSalePayment(payment);
                        if (y < 1)
                        {
                            gVal.Error = y == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                            gVal.Code = -1;
                            paymentErrorList.Add(gVal);
                        }
                        else
                        {
                            newsuccessList.Add(y);
                        }
                    }

                });
                successList = newsuccessList;
                return paymentErrorList;
            }
            catch (Exception)
            {
                successList = new List<long>();
                return paymentErrorList;
            }
        }

        private IEnumerable<GenericValidator> RefundPayments(List<StoreTransactionObject> transactions, long saleId, out  List<long> successList, bool saleRevoked)
        {
            var gVal = new GenericValidator();
            var refundedPaymentErrorList = new List<GenericValidator>();
            var newsuccessList = new List<long>();
            try
            {
                if (!transactions.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sale_Payment_List_Error;
                    refundedPaymentErrorList.Add(gVal);
                    successList = new List<long>();
                    return refundedPaymentErrorList;
                }

                transactions.ForEach(m =>
                {

                    var payment = new SalePaymentObject
                    {
                        SaleId = saleId,
                        AmountPaid = m.TransactionAmount
                    };

                    var y = new SalePaymentServices().RefundSalePayment(payment, saleRevoked);
                    if (y < 1)
                    {
                        gVal.Error = message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
                        refundedPaymentErrorList.Add(gVal);
                    }
                    else
                    {
                        newsuccessList.Add(y);
                    }


                });
                successList = newsuccessList;
                return refundedPaymentErrorList;
            }
            catch (Exception)
            {
                successList = new List<long>();
                return refundedPaymentErrorList;
            }
        }

        private IEnumerable<GenericValidator> AddTransactions(List<StoreTransactionObject> transactions, long saleId, out List<long> successList, UserProfileObject userProfile)
        {
            var gVal = new GenericValidator();
            var transactionErrorList = new List<GenericValidator>();
            var newsuccessList = new List<long>();
            try
            {
                if (!transactions.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sale_Payment_List_Error;
                    transactionErrorList.Add(gVal);
                    successList = new List<long>();
                    return transactionErrorList;
                }

                transactions.ForEach(m =>
                {
                    m.EffectedByEmployeeId = userProfile.EmployeeId;
                    m.TransactionDate = DateTime.Now;
                    m.StoreOutletId = userProfile.StoreOutletId;

                    var report = ValidateTransaction(m);
                    if (report.Code < 1)
                    {
                        transactionErrorList.Add(report);
                    }
                    else
                    {
                        var y = new StoreTransactionServices().AddStoreTransaction(m);
                        if (y < 1)
                        {
                            gVal.Error = y == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                            gVal.Code = -1;
                            transactionErrorList.Add(gVal);
                        }

                        else
                        {
                            var saleTransaction = new SaleTransactionObject
                            {
                                SaleId = saleId,
                                StoreTransactionId = y
                            };

                            var x = new SaleTransactionServices().AddSaleTransaction(saleTransaction);
                            if (x < 1)
                            {
                                new StoreTransactionServices().DeleteStoreTransaction(y);
                                gVal.Error = y == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                                gVal.Code = -1;
                                transactionErrorList.Add(gVal);
                            }

                            else
                            {
                                newsuccessList.Add(y);
                            }
                        }
                    }
                });

                successList = newsuccessList;
                return transactionErrorList;
            }
            catch (Exception)
            {
                successList = new List<long>();
                return transactionErrorList;
            }
        }

        private IEnumerable<GenericValidator> RevokeTransactions(List<StoreTransactionObject> transactions, long saleId, out List<long> successList, UserProfileObject userProfile, bool saleRevoked)
        {
            var gVal = new GenericValidator();
            var revokedTransactionErrorList = new List<GenericValidator>();
            var newsuccessList = new List<long>();
            try
            {
                if (!transactions.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sale_Payment_List_Error;
                    revokedTransactionErrorList.Add(gVal);
                    successList = new List<long>();
                    return revokedTransactionErrorList;
                }

                transactions.ForEach(m =>
                {
                    if (m.AmountRefunded > 0)
                    {
                        m.EffectedByEmployeeId = userProfile.EmployeeId;
                        m.SaleId = saleId;

                        var y = new StoreTransactionServices().RevokeStoreTransaction(m, saleRevoked);
                        if (y < 1)
                        {
                            gVal.Error = message_Feedback.Insertion_Failure;
                            gVal.Code = -1;
                            revokedTransactionErrorList.Add(gVal);
                        }

                        else
                        {
                            newsuccessList.Add(y);
                        }
                    }


                });

                successList = newsuccessList;
                return revokedTransactionErrorList;
            }
            catch (Exception)
            {
                successList = new List<long>();
                return revokedTransactionErrorList;
            }
        }

        private IEnumerable<GenericValidator> AddSoldItems(List<StoreItemSoldObject> soldItems, long saleId, out List<StoreItemSoldObject> successList)
        {
            var gVal = new GenericValidator();
            var soldItemErrorList = new List<GenericValidator>();
            var newsuccessList = new List<StoreItemSoldObject>();
            var mailingList = new List<StoreItemStockObject>();

            var serviceCategory = System.Configuration.ConfigurationManager.AppSettings["ServiceCategoryId"];

            var categoryId = 0;

            if (!string.IsNullOrEmpty(serviceCategory))
            {
                int.TryParse(serviceCategory, out categoryId);
            }

            try
            {
                if (!soldItems.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sold_Item_List_Error;
                    soldItemErrorList.Add(gVal);
                    successList = new List<StoreItemSoldObject>();
                    return soldItemErrorList;
                }

                soldItems.ForEach(m =>
                {
                    m.SaleId = saleId;
                    m.DateSold = DateTime.Now;
                    m.StoreItemCategoryId = categoryId;
                    var report = ValidateTransaction(m);
                    if (report.Code < 1)
                    {
                        soldItemErrorList.Add(report);
                    }
                    else
                    {
                        double remainingStockVolume;
                        var soldItem = new StoreItemStockServices().UpdateStoreItemStock(m, out remainingStockVolume);
                        if (soldItem.StoreItemSoldId < 1)
                        {
                            new ItemSoldServices().DeleteStoreItemSold(soldItem.StoreItemSoldId);
                            gVal.Error = message_Feedback.Insertion_Failure;
                            gVal.Code = -1;
                            soldItemErrorList.Add(gVal);
                        }

                        else
                        {
                            m.StoreItemSoldId = soldItem.StoreItemSoldId;
                            m.QuantityLeft = remainingStockVolume;
                            m.ReOrderQuantityStr = m.StoreItemStockObject.ReOrderQuantityStr != null ? m.StoreItemStockObject.ReOrderQuantityStr : "";
                            m.ReOrderLevelStr = m.StoreItemStockObject.ReOrderLevelStr != null ? m.StoreItemStockObject.ReOrderLevelStr : "";

                            newsuccessList.Add(m);
                            if (categoryId > 0)
                            {
                                if (m.StoreItemCategoryId != categoryId && m.StoreItemStockObject != null && m.StoreItemStockObject.ReorderLevel <= remainingStockVolume)
                                {
                                    m.StoreItemStockObject.QuantityInStock = remainingStockVolume;
                                    mailingList.Add(m.StoreItemStockObject);
                                }
                            }
                        }
                    }
                });

                successList = newsuccessList;
                if (mailingList.Any()) { SendMail(mailingList); }
                return soldItemErrorList;
            }
            catch (Exception)
            {
                successList = new List<StoreItemSoldObject>();
                return soldItemErrorList;
            }
        }

        private IEnumerable<GenericValidator> DepleteSoldItems(List<StoreItemSoldObject> soldItems, long saleId, out List<StoreItemSoldObject> successList)
        {
            var gVal = new GenericValidator();
            var returnedItemErrorList = new List<GenericValidator>();
            var newsuccessList = new List<StoreItemSoldObject>();
            try
            {
                if (!soldItems.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Sold_Item_List_Error;
                    returnedItemErrorList.Add(gVal);
                    successList = new List<StoreItemSoldObject>();
                    return returnedItemErrorList;
                }

                soldItems.ForEach(m =>
                {
                    if (m.ReturnedQuantity > 0)
                    {
                        m.SaleId = saleId;
                        var soldItemId = new StoreItemStockServices().ReturnItemSold(m);
                        if (soldItemId < 1)
                        {
                            gVal.Error = message_Feedback.Insertion_Failure;
                            gVal.Code = -1;
                            returnedItemErrorList.Add(gVal);
                        }

                        else
                        {
                            var success = new StoreItemSoldObject
                            {
                                StoreItemSoldId = soldItemId,
                                StoreItemStockId = m.StoreItemStockId
                            };

                            newsuccessList.Add(success);
                        }
                    }

                });

                successList = newsuccessList;
                return returnedItemErrorList;
            }
            catch (Exception)
            {
                successList = new List<StoreItemSoldObject>();
                return returnedItemErrorList;
            }
        }

        private void SendMail(List<StoreItemStockObject> itemsToReorder)
        {
            try
            {
                var storeInfo = GetSignedOnUser();
                if (storeInfo == null || storeInfo.StoreInfo.Id < 1)
                {
                    return;
                }
                var subject = "Stock level report " + DateTime.Today.ToString("dd/MM/yyyy");

                if (Request.Url != null)
                {
                    var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
                    var settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

                    var apiKey = System.Configuration.ConfigurationManager.AppSettings["mandrillApiKey"];
                    var appName = System.Configuration.ConfigurationManager.AppSettings["AplicationName"];

                    if (settings == null || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(appName))
                    {
                        return;
                    }

                    var msgBody = "<div class=\"row\"><div class=\"col-md-12\"><h4>Items Due for purchases</h4><br/>" +
                            "<table ng-emp-report-table id=\"paymentTypeRepTbl\"><thead style=\"background-color: #008000; height: 40px\">" +
                            "<tr style=\"color: #fff\"><th style=\"width: 3%; text-align: left\">S/N</th><th style=\"width: 25%; text-align: left\">" +
                             "Product</th><th style=\"width: 10%; text-align: left\">SKU</th><th style=\"width: 18%; text-align: left\">Quantity in stock</th><th style=\"width: 16%; text-align: left\">Reorder level</th><th style=\"width: 16%; text-align: left\">Reorder Quantity" +
                             "</th></tr></thead><tbody>";
                    var index = 1;
                    itemsToReorder.ForEach(s =>
                    {
                        msgBody += "<tr><td>" + index + "</td><td>" + s.StoreItemName + "</td><td>" + s.SKU + "</td><td>" + s.QuantityInStockStr + "</td><td>" + s.ReOrderLevelStr + "</td><td>" + s.ReOrderQuantityStr + "</td></tr>";
                        index++;
                    });

                    msgBody += "</tbody></table></div></div>";

                    #region Using Mandrill
                    var api = new MandrillApi(apiKey);
                    var receipint = new List<MandrillMailAddress> { new MandrillMailAddress(storeInfo.StoreInfo.StoreEmail) };
                    var message = new MandrillMessage()
                    {
                        AutoHtml = true,
                        To = receipint,
                        FromEmail = storeInfo.StoreInfo.StoreEmail,
                        FromName = appName,
                        Subject = subject,
                        Html = msgBody
                    };

                    api.Messages.SendAsync(message);

                    #endregion

                }

            }
            catch (Exception e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
            }
        }

        private List<StoreItemStockObject> GetStoreItemStocks(int categoryId, int outletId)
        {
            return new StoreItemStockServices().GetInventoriesByCategory(categoryId, outletId);
        }

        private IEnumerable<ItemPriceObject> GetItemPrices(int categoryId, int outletId)
        {
            return new ItemPriceServices().GetItemPriceListByStockItemCategory(categoryId, outletId) ?? new List<ItemPriceObject>();
        }

        private IEnumerable<ItemPriceObject> GetItemPriceObjectBySku(string sku, int outletId)
        {
            return new ItemPriceServices().GetItemPriceObjectBySku(sku, outletId) ?? new List<ItemPriceObject>();
        }

        private List<StorePaymentMethodObject> GetPaymentMethods()
        {
            return new StorePaymentMethodServices().GetStorePaymentMethods() ?? new List<StorePaymentMethodObject>();
        }
        //private List<PaymentTypeObject> GetPaymentTypes()
        //{
        //    return new PaymentTypeServices().GetPaymentTypes() ?? new List<PaymentTypeObject>();
        //}
        private bool DeleteTransactions(List<long> transactionIds)
        {
            try
            {
                var successCount = 0;
                transactionIds.ForEach(m =>
                {
                    var x = new SaleTransactionServices().DeleteSaleTransactionByTransaction(m);
                    if (x)
                    {
                        var s = new StoreTransactionServices().DeleteStoreTransaction(m);
                        if (s)
                        {
                            successCount += 1;
                        }
                    }
                });
                if (successCount == transactionIds.Count)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool DeletePayments(List<long> paymentIds)
        {
            try
            {
                var successCount = 0;
                paymentIds.ForEach(m =>
                {
                    var x = new SalePaymentServices().DeleteSalePayment(m);
                    if (x)
                    {
                        successCount += 1;
                    }
                });
                if (successCount == paymentIds.Count)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool DeleteSoldItems(List<StoreItemSoldObject> soldItemIds)
        {
            try
            {
                var successCount = 0;
                soldItemIds.ForEach(m =>
                {
                    var x = new ItemSoldServices().DeleteStoreItemSold(m.StoreItemSoldId);
                    if (x)
                    {
                        successCount += 1;
                    }
                });
                if (successCount == soldItemIds.Count)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<StoreItemCategoryObject> GetProductCategories()
        {
            return new StoreItemCategoryServices().GetStoreItemCategories();
        }
        private GenericValidator ValidateSale(SaleObject sale)
        {
            var gVal = new GenericValidator();
            if (sale == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (sale.AmountDue < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Amount_Due_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        private GenericValidator ValidatePayment(SalePaymentObject payment)
        {
            var gVal = new GenericValidator();
            if (payment == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (payment.AmountPaid < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Amount_Paid_Error;
                return gVal;
            }

            if (payment.SaleId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        private GenericValidator ValidateTransaction(StoreTransactionObject transaction)
        {
            var gVal = new GenericValidator();
            if (transaction == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (transaction.StorePaymentMethodId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Payment_Method_Selection_Error;
                return gVal;
            }

            if (transaction.TransactionAmount < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Transaction_Amount_Eror;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        private GenericValidator ValidateTransaction(StoreItemSoldObject soldItem)
        {
            var gVal = new GenericValidator();
            if (soldItem == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (soldItem.QuantitySold < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Quantity_Sold_Error;
                return gVal;
            }

            if (soldItem.AmountSold < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Amount_Sold_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
