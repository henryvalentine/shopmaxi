using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
    public class ReportController : Controller
    {

        #region SALES REPORTS
        public ActionResult GetEmployeeSalesReport(JQueryDataTableParamModel param, long employeeId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<SaleObject> filteredParentMenuObjects;
                var countG = 0;

                var time = new TimeSpan(23, 59, 59);

                endDate = endDate.Add(time);

                var pagedParentMenuObjects = new ReportServices().GetEmployeeSalesReport(param.iDisplayLength, param.iDisplayStart, out countG, employeeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearchEmployeeSalesReport(param.sSearch, employeeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber : sortColumnIndex == 2 ? c.CustomerName :
                  sortColumnIndex == 3 ? c.AmountDueStr : sortColumnIndex == 4 ? c.VATAmountStr : sortColumnIndex == 5 ? c.DiscountAmountStr : sortColumnIndex == 6 ? c.NetAmountStr : sortColumnIndex == 7 ? c.AmountPaidStr : c.DateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "desc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredParentMenuObjects;

                var result =
                    from c in displayedPersonnels
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                        c.DateStr
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

        public ActionResult GetReorderList(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemStockObject> filteredReorderList;
                var countG = 0;

                var reorderList = new ReportServices().GetRecommendedPurchases(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredReorderList = new ReportServices().SearchRecommendedPurchases(param.sSearch);
                    countG = filteredReorderList.Count();

                }
                else
                {
                    filteredReorderList = reorderList;

                }

                if (!filteredReorderList.Any())
                {
                    return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemStockObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreItemName : c.SKU);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredReorderList = sortDirection == "desc" ? filteredReorderList.OrderBy(orderingFunction) : filteredReorderList.OrderByDescending(orderingFunction);

                var result =
                    from c in filteredReorderList
                    select new[] { Convert.ToString(c.StoreItemStockId),
                        c.StoreItemName, c.SKU, c.QuantityInStockStr, c.ReOrderLevelStr, c.ReOrderQuantityStr
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
                return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSingleEmployeeSalesReport(long employeeId, string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var time = new TimeSpan(23, 59, 59);

                endDate = endDate.Add(time);

                var reportsObjects = new ReportServices().GetEmployeeSalesReport2(itemsPerPage, pageNumber, employeeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!reportsObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result =
                    from c in reportsObjects
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                        c.DateStr
                    };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllEmployeeSalesReport(string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var reportsObjects = new ReportServices().GetEmployeeSalesReport3(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!reportsObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result =
                    from c in reportsObjects
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                        c.DateStr
                    };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDailySales(int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                var reportsObjects = new ReportServices().GetDailySales(itemsPerPage, pageNumber);

                if (!reportsObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result =
                    from c in reportsObjects
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                        c.DateStr
                    };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllOutletsSalesReports(string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var reportsObjects = new ReportServices().GetAllOutletsSalesReports(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!reportsObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result =
                    from c in reportsObjects
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                        c.DateStr
                    };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSingleOutletSalesReports(int outletId, string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var reportsObjects = new ReportServices().GetSingleOutletSalesReports(itemsPerPage, pageNumber, outletId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!reportsObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result =
                    from c in reportsObjects
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                        c.DateStr
                    };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllCustomersSalesReport(string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                // //{ StartDate: $scope.startDate, endDate.Add(new TimeSpan(23, 59, 59)): $scope.endDate, CustomerId: payload.customer.CustomerId }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var time = new TimeSpan(23, 59, 59);

                endDate = endDate.Add(time);

                var reportsObjects = new ReportServices().GetAllCustomersSalesReport(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!reportsObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result =
                    from c in reportsObjects
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.DateStr, c.CustomerName, c.OutletName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr
                    };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSingleCustomerSalesReport(long customerId, string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var reportsObjects = new ReportServices().GetSingleCustomerSalesReport(itemsPerPage, pageNumber, customerId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!reportsObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                var result =
                    from c in reportsObjects
                    select new[] { Convert.ToString(c.SaleId),
                        c.InvoiceNumber, c.DateStr, c.CustomerName, c.OutletName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr
                    };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCustomerInvoiceReports(JQueryDataTableParamModel param, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<CustomerInvoiceObject> filteredParentMenuObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetCustomerInvoiceReports(param.iDisplayLength, param.iDisplayStart, out countG, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearchCustomerInvoiceReport(param.sSearch, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<CustomerInvoiceObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.CustomerName : c.DateProfiledStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "desc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredParentMenuObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.Id), c.CustomerName,
                                 c.TotalAmountDueStr, c.TotalVATAmountStr, c.TotalDiscountAmountStr, c.TotalAmountPaidStr, c.InvoiceBalanceStr, c.DateProfiledStr
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
                return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllCustomerStatementSummaries(int itemsPerPage, int pageNumber)
        {
            try
            {
                var customerStatements = new ReportServices().GetAllCustomerInvoiceStatements(itemsPerPage, pageNumber);


                var result = from c in customerStatements
                             select new[] { Convert.ToString(c.Id), c.CustomerName, c.DateProfiledStr, c.OutletName,
                                 c.TotalAmountDueStr, c.TotalVATAmountStr, c.TotalDiscountAmountStr, c.TotalAmountPaidStr, c.InvoiceBalanceStr
                             };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSingleCustomerInvoiceStatementSummaries(long customerId, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                var customerStatements = new ReportServices().GetSingleCustomerInvoiceStatements(itemsPerPage, pageNumber, customerId);


                var result = from c in customerStatements
                             select new[] { Convert.ToString(c.Id), c.CustomerName, c.DateProfiledStr, c.OutletName,
                                 c.TotalAmountDueStr, c.TotalVATAmountStr, c.TotalDiscountAmountStr, c.TotalAmountPaidStr, c.InvoiceBalanceStr
                             };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllCustomerStatements(int itemsPerPage, int pageNumber, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var customerStatements = new ReportServices().GetAllCustomerStatements(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                return Json(customerStatements, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCustomerStatements(long customerId, int itemsPerPage, int pageNumber, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var customerStatements = new ReportServices().GetCustomerStatements(itemsPerPage, pageNumber, customerId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                return Json(customerStatements, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllSupplierStatements(int itemsPerPage, int pageNumber)
        {
            try
            {

                var suppliersStatements = new ReportServices().GetAllSupplierStatements(itemsPerPage, pageNumber);

                return Json(suppliersStatements, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSingleSupplierStatements(int supplierId, int itemsPerPage, int pageNumber)
        {
            try
            {

                var suppliersStatements = new ReportServices().GetSingleSupplierStatements(itemsPerPage, pageNumber, supplierId);

                return Json(suppliersStatements, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CustomerInvoiceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCustomerSalesReport(JQueryDataTableParamModel param, long customerId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<SaleObject> filteredParentMenuObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetCustomerSalesReport(param.iDisplayLength, param.iDisplayStart, out countG, customerId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearchCustomerSalesReport(param.sSearch, customerId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }


                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber :
                  sortColumnIndex == 2 ? c.AmountDueStr : sortColumnIndex == 3 ? c.VATAmountStr : sortColumnIndex == 4 ? c.DiscountAmountStr : sortColumnIndex == 5 ? c.NetAmountStr : sortColumnIndex == 6 ? c.AmountPaidStr : c.DateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "desc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredParentMenuObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                                 c.DateStr
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
        public ActionResult GetProductSalesReport(JQueryDataTableParamModel param, long productId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<StoreItemSoldObject> filteredParentMenuObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetStoreItemReport(param.iDisplayLength, param.iDisplayStart, out countG, productId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearStoreItemReport(param.sSearch, productId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }


                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemSoldObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreItemName : sortColumnIndex == 2 ? c.QuantitySoldStr :
                  sortColumnIndex == 3 ? c.AmountSoldStr : c.DateSoldStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "desc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredParentMenuObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.StoreItemStockId),
                                 c.QuantitySoldStr, c.RateStr, c.AmountSoldStr, c.DateSoldStr,
                                 c.QuantityLeftStr
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
        public ActionResult GetAllProductSalesReport(string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var productReports = new ReportServices().GetAllProductReport(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!productReports.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }

                return Json(productReports, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSingleProductReport(string startDateStr, string endDateStr, long itemId, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var productReports = new ReportServices().GetSingleProductReport(itemsPerPage, pageNumber, itemId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!productReports.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }
                ;
                return Json(productReports, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllSalesReportByCategory(string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var categorizedReports = new ReportServices().GetAllSalesReportByCategory(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                return Json(categorizedReports, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSalesReportByCategory(int categoryId, string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (categoryId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a category.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var categorizedReports = new ReportServices().GetSalesReportByCategory(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)), categoryId);

                return Json(categorizedReports, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllPaymentTypeReports(string startDateStr, string endDateStr, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var paymentTypeReports = new ReportServices().GetAllPaymentTypeReports(itemsPerPage, pageNumber, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!paymentTypeReports.Any())
                {
                    return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
                }

                //var result = from c in paymentTypeReports
                //             select new[] { Convert.ToString(c.StoreTransactionId), c.PaymentMethodName, c.TransactionDateStr, c.TransactionAmountStr, c.InvoiceNumber, c.CustomerName
                //             };
                return Json(paymentTypeReports, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSinglePaymentTypeReports(string startDateStr, string endDateStr, int paymentMethodTypeId, int itemsPerPage, int pageNumber)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var paymentTypeReports = new ReportServices().GetSinglePaymentTypeReports(itemsPerPage, pageNumber, paymentMethodTypeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!paymentTypeReports.Any())
                {
                    return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
                }

                return Json(paymentTypeReports, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPaymentTypeSalesReport(JQueryDataTableParamModel param, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<StoreTransactionObject> filteredParentMenuObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetSalesReportByPaymentType(param.iDisplayLength, param.iDisplayStart, out countG, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearchSalesReportByPaymentType(param.sSearch, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
                }


                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreTransactionObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.PaymentMethodName : sortColumnIndex == 2 ? c.TransactionAmountStr :
                  sortColumnIndex == 3 ? c.StoreTransactionTypeName : c.TransactionDateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "desc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredParentMenuObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.StoreTransactionId),
                                 c.PaymentMethodName, c.TransactionAmountStr, c.StoreTransactionTypeName,
                                 c.TransactionDateStr
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
                return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetOutletSalesReport(JQueryDataTableParamModel param, int outletId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<SaleObject> filteredSaleObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetSalesReportByOutlet(param.iDisplayLength, param.iDisplayStart, out countG, outletId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new ReportServices().SearchSalesReportByOutlet(param.sSearch, outletId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredSaleObjects.Count();

                }
                else
                {
                    filteredSaleObjects = pagedParentMenuObjects;

                }

                if (!filteredSaleObjects.Any())
                {
                    return Json(new List<SaleObject>(), JsonRequestBehavior.AllowGet);
                }


                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<SaleObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.InvoiceNumber : sortColumnIndex == 2 ? c.CustomerName :
                  sortColumnIndex == 3 ? c.AmountDueStr : sortColumnIndex == 4 ? c.VATAmountStr : sortColumnIndex == 5 ? c.DiscountAmountStr : sortColumnIndex == 6 ? c.NetAmountStr : sortColumnIndex == 7 ? c.AmountPaidStr : c.DateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "desc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredSaleObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                                 c.DateStr
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
        public ActionResult GetItemCategorySalesReport(JQueryDataTableParamModel param, int categoryId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<StoreItemSoldObject> filteredParentMenuObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetStoreItemCategoryReport(param.iDisplayLength, param.iDisplayStart, out countG, categoryId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearchStoreItemCategoryReport(param.sSearch, categoryId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<StoreItemSoldObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemSoldObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreItemName : sortColumnIndex == 2 ? c.AmountSoldStr :
                  sortColumnIndex == 3 ? c.AmountSoldStr : c.DateSoldStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "desc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredParentMenuObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.StoreItemName, c.AmountSoldStr, c.AmountSoldStr,
                                 c.DateSoldStr
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
                return Json(new List<StoreItemSoldObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetItemTypeSalesReport(JQueryDataTableParamModel param, int typeId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<StoreItemSoldObject> filteredParentMenuObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetStoreItemTypeReport(param.iDisplayLength, param.iDisplayStart, out countG, typeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearchStoreItemTypeReport(param.sSearch, typeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = pagedParentMenuObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<StoreItemSoldObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemSoldObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreItemName : sortColumnIndex == 2 ? c.AmountSoldStr :
                  sortColumnIndex == 3 ? c.AmountSoldStr : c.DateSoldStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredParentMenuObjects = sortDirection == "desc" ? filteredParentMenuObjects.OrderBy(orderingFunction) : filteredParentMenuObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredParentMenuObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.StoreItemName, c.AmountSoldStr, c.AmountSoldStr,
                                 c.DateSoldStr
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
                return Json(new List<StoreItemSoldObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetMySalesReport(JQueryDataTableParamModel param, DateTime startDate, DateTime endDate)
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

                var pagedSaleObjects = new ReportServices().GetEmployeeSalesReport(param.iDisplayLength, param.iDisplayStart, out countG, userInfo.UserProfile.Id, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredSaleObjects = new ReportServices().SearchEmployeeSalesReport(param.sSearch, userInfo.UserProfile.Id, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
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
                  sortColumnIndex == 3 ? c.AmountDueStr : sortColumnIndex == 4 ? c.VATAmountStr : sortColumnIndex == 5 ? c.DiscountAmountStr : sortColumnIndex == 6 ? c.NetAmountStr : sortColumnIndex == 7 ? c.AmountPaidStr : c.DateStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredSaleObjects = sortDirection == "desc" ? filteredSaleObjects.OrderBy(orderingFunction) : filteredSaleObjects.OrderByDescending(orderingFunction);

                var displayedPersonnels = filteredSaleObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.SaleId),
                                 c.InvoiceNumber, c.CustomerName, c.AmountDueStr, c.VATAmountStr, c.DiscountAmountStr, c.NetAmountStr, c.AmountPaidStr,
                                 c.DateStr
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

        public ActionResult GetReportSnapshots()
        {
            try
            {
                var reportSnapShots = new DailySaleReport();

                if (User.IsInRole("Marketer"))
                {
                    reportSnapShots = new ReportServices().GetMarketerReportSnapshots();
                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        var refDate = DateTime.Today.Add(new TimeSpan(23, 59, 59));
                        reportSnapShots = new ReportServices().GetReportSnapshots(refDate);
                    }
                }

                return Json(reportSnapShots, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new DailySaleReport(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDefaults()
        {
            try
            {
                var defaults = new ReportServices().GetDefaults();
                return Json(defaults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSupplliers()
        {
            try
            {
                var defaults = new ReportServices().GetSuppliers();
                return Json(defaults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCustomers()
        {
            try
            {
                var customers = new SaleServices().GetCustomers();
                return Json(customers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPriceList(int itemsPerPage, int pageNumber)
        {
            try
            {
                var priceList = new ReportServices().GetPriceList(itemsPerPage, pageNumber);

                if (!priceList.Any())
                {
                    return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
                }

                return Json(priceList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreTransactionObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStockReport(int itemsPerPage, int pageNumber)
        {
            try
            {
                var categoryList = new ReportServices().GetStockReport(itemsPerPage, pageNumber);
                if (!categoryList.Any())
                {
                    return Json(new List<StoreItemCategoryObject>(), JsonRequestBehavior.AllowGet);
                }

                return Json(categoryList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemCategoryObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCustomersAndSuppliers()
        {
            try
            {
                var defaults = new SaleServices().GetCustomersAndSuppliers();
                return Json(defaults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StateReportDefault(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSalesReportDetails(long saleId)
        {
            var gVal = new GenericValidator();
            try
            {
                if (saleId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid selection!";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var saleInfo = new ReportServices().GetSalesReportDetails(saleId);
                return Json(saleInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProductSalesReportDetails(ProductReport productReport)
        {
            var gVal = new GenericValidator();
            try
            {
                if (productReport.ProductId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid selection!";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var saleInfo = new ReportServices().GetProductSalesReportDetails(productReport);
                return Json(saleInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SaleObject(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region PURCHASE ORDER REPORTS
        public ActionResult GetPurchaseOrderReportsByEmployee(JQueryDataTableParamModel param, long employeeId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<PurchaseOrderObject> filteredPurchaseOrderObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetPurchaseOrderReportsByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, employeeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPurchaseOrderObjects = new ReportServices().SearchEmployeePurchaseOrderReports(param.sSearch, employeeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
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
                             select new[] { Convert.ToString(c.PurchaseOrderId),
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
        public ActionResult GetPurchaseOrderReports(JQueryDataTableParamModel param, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<PurchaseOrderObject> filteredParentMenuObjects;
                var countG = 0;

                var filteredPurchaseOrderObjects = new ReportServices().GetPurchaseOrderReports(param.iDisplayLength, param.iDisplayStart, out countG, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredParentMenuObjects = new ReportServices().SearchPurchaseOrderReports(param.sSearch, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredParentMenuObjects.Count();

                }
                else
                {
                    filteredParentMenuObjects = filteredPurchaseOrderObjects;

                }

                if (!filteredParentMenuObjects.Any())
                {
                    return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<PurchaseOrderObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.SupplierName : sortColumnIndex == 2 ? c.GeneratedByEmployeeName :
                  sortColumnIndex == 3 ? c.DerivedTotalCostStr : sortColumnIndex == 4 ? c.DateTimePlacedStr : sortColumnIndex == 5 ? c.ActualDeliveryDateStr : c.DeliveryStatus);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredPurchaseOrderObjects = sortDirection == "desc" ? filteredPurchaseOrderObjects.OrderBy(orderingFunction).ToList() : filteredPurchaseOrderObjects.OrderByDescending(orderingFunction).ToList();

                var displayedPersonnels = filteredPurchaseOrderObjects;

                var result = from c in displayedPersonnels
                             select new[] { Convert.ToString(c.PurchaseOrderId),
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
        public ActionResult GetPurchaseOrdersReportByOutlet(JQueryDataTableParamModel param, int outletId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<PurchaseOrderObject> filteredPurchaseOrderObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetPurchaseOrderReportsByOutlet(param.iDisplayLength, param.iDisplayStart, out countG, outletId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPurchaseOrderObjects = new ReportServices().SearchOutletPurchaseOrderReports(param.sSearch, outletId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
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
                             select new[] { Convert.ToString(c.PurchaseOrderId),
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
        public ActionResult GetPurchaseOrderReportsByStatus(JQueryDataTableParamModel param, int categoryId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<PurchaseOrderObject> filteredPurchaseOrderObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetPurchaseOrdersByStatus(param.iDisplayLength, param.iDisplayStart, out countG, categoryId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPurchaseOrderObjects = new ReportServices().SearchPurchaseOrderReportsByStatus(param.sSearch, categoryId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
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
                             select new[] { Convert.ToString(c.PurchaseOrderId),
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


        #endregion

        #region ESTIMATES REPORTS
        public ActionResult GetEstimateReportsByEmployee(JQueryDataTableParamModel param, long employeeId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<EstimateObject> filteredEstimateObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetEstimateReportsByEmployee(param.iDisplayLength, param.iDisplayStart, out countG, employeeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new ReportServices().SearchEmployeeEstimateReports(param.sSearch, employeeId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredEstimateObjects.Count();
                }
                else
                {
                    filteredEstimateObjects = pagedParentMenuObjects;
                }

                if (!filteredEstimateObjects.Any())
                {
                    return Json(new List<PurchaseOrderObject>(), JsonRequestBehavior.AllowGet);
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
        public ActionResult GetEstimateReports(JQueryDataTableParamModel param, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<EstimateObject> filteredEstimateObjects;
                var countG = 0;

                var filteredPurchaseOrderObjects = new ReportServices().GetEstimateReports(param.iDisplayLength, param.iDisplayStart, out countG, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new ReportServices().SearchEstimateReports(param.sSearch, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
                    countG = filteredEstimateObjects.Count();

                }
                else
                {
                    filteredEstimateObjects = filteredPurchaseOrderObjects;

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
        public ActionResult GetEstimateReportsByOutlet(JQueryDataTableParamModel param, int outletId, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<EstimateObject> filteredEstimateObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetEstimateReportsByOutlet(param.iDisplayLength, param.iDisplayStart, out countG, outletId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new ReportServices().SearchOutletEstimateReports(param.sSearch, outletId, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
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
        public ActionResult GetEstimatesByConversionStatus(JQueryDataTableParamModel param, bool status, string startDateStr, string endDateStr)
        {
            var gVal = new GenericValidator();
            try
            {
                if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                DateTime startDate;
                var res1 = DateTime.TryParse(startDateStr, out startDate);

                DateTime endDate;
                var res2 = DateTime.TryParse(endDateStr, out endDate);

                if (!res1 || !res2 || startDate.Year <= 1 || endDate.Year <= 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select a valid date range.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                IEnumerable<EstimateObject> filteredEstimateObjects;
                var countG = 0;

                var pagedParentMenuObjects = new ReportServices().GetEstimatesByConversionStatus(param.iDisplayLength, param.iDisplayStart, status, out countG, startDate, endDate.Add(new TimeSpan(23, 59, 59)));

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEstimateObjects = new ReportServices().SearchEstimatesByConversionStatus(param.sSearch, status, startDate, endDate.Add(new TimeSpan(23, 59, 59)));
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
        #endregion
    }
}
