using System;
using System.Collections.Generic;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
    public class ReportServices
    {
        private readonly ReportRepository _reportRepository;
        public ReportServices()
        {
            _reportRepository = new ReportRepository();
        }

        #region SALES REPORT
        public List<StoreItemSoldObject> GetAllSalesReportByCategory(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetAllSalesReportByCategory(itemsPerPage, pageNumber, startDate, endDate);
        }
        public List<StoreItemSoldObject> GetSalesReportByCategory(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate, int categoryId)
        {
            return _reportRepository.GetSalesReportByCategory(itemsPerPage, pageNumber, startDate, endDate, categoryId);
        }

        public List<SaleObject> GetEmployeeSalesReport(int? itemsPerPage, int? pageNumber, out int countG, long employeeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetEmployeeSalesReport(itemsPerPage, pageNumber, out countG, employeeId, startDate, endDate);
        }
        public List<SaleObject> GetEmployeeSalesReport2(int? itemsPerPage, int? pageNumber, long employeeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetEmployeeSalesReport2(itemsPerPage, pageNumber, employeeId, startDate, endDate);
        }
        public List<SaleObject> GetDailySales(int? itemsPerPage, int? pageNumber)
        {
            return _reportRepository.GetDailySales(itemsPerPage, pageNumber);
        }

        public List<SaleObject> GetDueInvoices(int? itemsPerPage, int? pageNumber, out int countG)
        {
            return _reportRepository.GetDueInvoices(itemsPerPage, pageNumber, out countG);
        }
        public List<SaleObject> GetRevokedSales(int? itemsPerPage, int? pageNumber, out int countG)
        {
            return _reportRepository.GetRevokedSales(itemsPerPage, pageNumber, out countG);
        }
        public List<SaleObject> GetEmployeeSalesReport3(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetEmployeeSalesReport3(itemsPerPage, pageNumber, startDate, endDate);
        }
        public List<SaleObject> GetEmployeeSalesReport(int? itemsPerPage, int? pageNumber, out int countG, long employeeId)
        {
            return _reportRepository.GetEmployeeSalesReport(itemsPerPage, pageNumber, out countG, employeeId);
        }

        public List<SaleObject> GetContactPersonInvoices(int? itemsPerPage, int? pageNumber, out int countG, long employeeId)
        {
            return _reportRepository.GetContactPersonInvoices(itemsPerPage, pageNumber, out countG, employeeId);
        }

        public List<SaleObject> GetAdminSalesReport(int? itemsPerPage, int? pageNumber, out int countG)
        {
            return _reportRepository.GetAdminSalesReport(itemsPerPage, pageNumber, out countG);
        }

        public List<StoreItemSoldObject> GetStoreItemTypeReport(int? itemsPerPage, int? pageNumber, out int countG, int typeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetStoreItemTypeReport(itemsPerPage, pageNumber, out countG, typeId, startDate, endDate);
        }
        public List<StoreItemSoldObject> GetStoreItemCategoryReport(int? itemsPerPage, int? pageNumber, out int countG, int categoryId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetStoreItemCategoryReport(itemsPerPage, pageNumber, out countG, categoryId, startDate, endDate);
        }
        public List<StoreItemSoldObject> GetStoreItemReport(int? itemsPerPage, int? pageNumber, out int countG, long itemId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetStoreItemReport(itemsPerPage, pageNumber, out countG, itemId, startDate, endDate);
        }

        public List<StoreItemSoldObject> GetSingleProductReport(int? itemsPerPage, int? pageNumber, long itemId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetSingleProductReport(itemsPerPage, pageNumber, itemId, startDate, endDate);
        }

        public List<StoreItemSoldObject> GetAllProductReport(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetAllProductReport(itemsPerPage, pageNumber, startDate, endDate);
        }

        public List<StoreItemStockObject> GetRecommendedPurchases(int itemsPerPage, int pageNumber, out int countG)
        {
            return _reportRepository.GetRecommendedPurchases(itemsPerPage, pageNumber, out countG);
        }

        public DailySaleReport GetReportSnapshots(DateTime refDate)
        {
            return _reportRepository.GetReportSnapshots(refDate);
        }

        public DailySaleReport GetMarketerReportSnapshots()
        {
            return _reportRepository.GetMarketerReportSnapshots();
        }

        public List<SupplierObject> GetSuppliers()
        {
            return _reportRepository.GetSuppliers();
        }

        public List<StoreItemStockObject> GetPriceList(int? itemsPerPage, int? pageNumber)
        {
            return _reportRepository.GetPriceList(itemsPerPage, pageNumber);
        }
        public List<StoreItemCategoryObject> GetStockReport(int? itemsPerPage, int? pageNumber)
        {
            return _reportRepository.GetStockReport(itemsPerPage, pageNumber);
        }
        public List<StoreTransactionObject> GetSalesReportByPaymentType(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetSalesReportByPaymentType(itemsPerPage, pageNumber, out countG, startDate, endDate);
        }

        public List<StorePaymentMethodObject> GetAllPaymentTypeReports(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetAllPaymentTypeReports(itemsPerPage, pageNumber, startDate, endDate);
        }
        public List<StorePaymentMethodObject> GetSinglePaymentTypeReports(int? itemsPerPage, int? pageNumber, long transactionTypeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetSinglePaymentTypeReports(itemsPerPage, pageNumber, transactionTypeId, startDate, endDate);
        }

        public List<SaleObject> GetAllOutletsSalesReports(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetAllOutletsSalesReports(itemsPerPage, pageNumber, startDate, endDate);
        }
        public List<SaleObject> GetSingleOutletSalesReports(int? itemsPerPage, int? pageNumber, int outletId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetSingleOutletSalesReports(itemsPerPage, pageNumber, outletId, startDate, endDate);
        }

        public List<SaleObject> GetAllCustomersSalesReport(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetAllCustomersSalesReport(itemsPerPage, pageNumber, startDate, endDate);
        }

        public CustomerStatement GetCustomerStatements(int itemsPerPage, int pageNumber, long customerId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetCustomerStatements(itemsPerPage, pageNumber, customerId, startDate, endDate);
        }

        public CustomerStatement GetAllCustomerStatements(int itemsPerPage, int pageNumber, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetAllCustomerStatements(itemsPerPage, pageNumber, startDate, endDate);
        }

        public List<SaleObject> GetSingleCustomerSalesReport(int? itemsPerPage, int? pageNumber, long customerId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetSingleCustomerSalesReport(itemsPerPage, pageNumber, customerId, startDate, endDate);
        }

        public List<CustomerInvoiceObject> GetSingleCustomerInvoiceStatements(int? itemsPerPage, int? pageNumber, long customerId)
        {
            return _reportRepository.GetSingleCustomerInvoiceStatements(itemsPerPage, pageNumber, customerId);
        }
        public List<CustomerInvoiceObject> GetAllCustomerInvoiceStatements(int? itemsPerPage, int? pageNumber)
        {
            return _reportRepository.GetAllCustomerInvoiceStatements(itemsPerPage, pageNumber);
        }

        public List<SupplierInvoiceObject> GetAllSupplierStatements(int? itemsPerPage, int? pageNumber)
        {
            return _reportRepository.GetAllSupplierStatements(itemsPerPage, pageNumber);
        }
        public List<SupplierInvoiceObject> GetSingleSupplierStatements(int? itemsPerPage, int? pageNumber, int supplierId)
        {
            return _reportRepository.GetSingleSupplierStatements(itemsPerPage, pageNumber, supplierId);
        }

        public List<SaleObject> GetSalesReportByOutlet(int? itemsPerPage, int? pageNumber, out int countG, int outletId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetSalesReportByOutlet(itemsPerPage, pageNumber, out countG, outletId, startDate, endDate);
        }
        public List<SaleObject> GetDailySalesReport(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetDailySalesReport(itemsPerPage, pageNumber, out countG, startDate, endDate);
        }
        public List<CustomerInvoiceObject> GetCustomerInvoiceReports(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetCustomerInvoiceReports(itemsPerPage, pageNumber, out countG, startDate, endDate);
        }
        public List<SaleObject> GetCustomerSalesReport(int? itemsPerPage, int? pageNumber, out int countG, long customerId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetCustomerSalesReport(itemsPerPage, pageNumber, out countG, customerId, startDate, endDate);
        }
        public List<SaleObject> SearchEmployeeSalesReport(string searchCriteria, long employeeId)
        {
            return _reportRepository.SearchEmployeeSalesReport(searchCriteria, employeeId);
        }

        public List<SaleObject> SearchContactPersonInvoices(string searchCriteria, long employeeId)
        {
            return _reportRepository.SearchContactPersonInvoices(searchCriteria, employeeId);
        }

        public List<SaleObject> SearchAdminSalesReport(string searchCriteria)
        {
            return _reportRepository.SearchAdminSalesReport(searchCriteria);
        }

        public List<SaleObject> SearchEmployeeSalesReport(string searchCriteria, long employeeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchEmployeeSalesReport(searchCriteria, employeeId, startDate, endDate);
        }
        public List<SaleObject> SearchDueInvoices(string searchCriteria)
        {
            return _reportRepository.SearchDueInvoices(searchCriteria);
        }
        public List<SaleObject> SearchRevokedSales(string searchCriteria)
        {
            return _reportRepository.SearchRevokedSales(searchCriteria);
        }
        public List<StoreItemStockObject> SearchRecommendedPurchases(string searchCriteria)
        {
            return _reportRepository.SearchRecommendedPurchases(searchCriteria);
        }
        public List<StoreTransactionObject> SearchSalesReportByPaymentType(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchSalesReportByPaymentType(searchCriteria, startDate, endDate);
        }
        public List<SaleObject> SearchDailySalesReport(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchDailySalesReport(searchCriteria, startDate, endDate);
        }
        public List<StoreItemSoldObject> SearStoreItemReport(string searchCriteria, long itemId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearStoreItemReport(searchCriteria, itemId, startDate, endDate);
        }
        public List<StoreItemSoldObject> SearchStoreItemCategoryReport(string searchCriteria, int categoryId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearStoreItemReport(searchCriteria, categoryId, startDate, endDate);
        }
        public List<SaleObject> SearchSalesReportByOutlet(string searchCriteria, int outletId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchSalesReportByOutlet(searchCriteria, outletId, startDate, endDate);
        }
        public List<StoreItemSoldObject> SearchStoreItemTypeReport(string searchCriteria, int typeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchStoreItemTypeReport(searchCriteria, typeId, startDate, endDate);
        }
        public List<SaleObject> SearchCustomerSalesReport(string searchCriteria, long customerId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchCustomerSalesReport(searchCriteria, customerId, startDate, endDate);
        }
        public List<CustomerInvoiceObject> SearchCustomerInvoiceReport(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchCustomerInvoiceReport(searchCriteria, startDate, endDate);
        }
        public SaleObject GetSalesReportDetails(long saleId)
        {
            return _reportRepository.GetSalesReportDetails(saleId);
        }
        public SaleObject GetSalesReportDetails(string invoiceNumber)
        {
            return _reportRepository.GetSalesReportDetails(invoiceNumber);
        }

        public SaleObject GetRefundedSale(long saleId)
        {
            return _reportRepository.GetRefundedSale(saleId);
        }

        public List<StoreItemSoldObject> GetProductSalesReportDetails(ProductReport productReport)
        {
            return _reportRepository.GetProductSalesReportDetails(productReport);
        }

        public List<SaleObject> GetUncompletedTransactions(int? itemsPerPage, int? pageNumber, out int countG)
        {
            return _reportRepository.GetUncompletedTransactions(itemsPerPage, pageNumber, out countG);
        }

        public List<SaleObject> SearchUncompletedTransactions(string searchCriteria)
        {
            return _reportRepository.SearchUncompletedTransactions(searchCriteria);
        }

        public OnlineStoreObject GetDefaults()
        {
            return _reportRepository.GetDefaults();
        }
        #endregion

        #region PURCHASE ORDER REPORTS
        public List<PurchaseOrderObject> GetPurchaseOrderReports(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetPurchaseOrderReports(itemsPerPage, pageNumber, out countG, startDate, endDate);
        }
        public List<PurchaseOrderObject> GetPurchaseOrderReportsByOutlet(int? itemsPerPage, int? pageNumber, out int countG, int outletId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetPurchaseOrdersByOutlet(itemsPerPage, pageNumber, out countG, outletId, startDate, endDate);
        }
        public List<PurchaseOrderObject> GetPurchaseOrderReportsByEmployee(int? itemsPerPage, int? pageNumber, out int countG, long employeeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetPurchaseOrderReportsByEmployee(itemsPerPage, pageNumber, out countG, employeeId, startDate, endDate);
        }
        public List<PurchaseOrderObject> GetPurchaseOrdersByStatus(int? itemsPerPage, int? pageNumber, out int countG, int status, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetPurchaseOrdersByStatus(itemsPerPage, pageNumber, out countG, status, startDate, endDate);
        }
        public List<PurchaseOrderObject> SearchPurchaseOrderReports(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchPurchaseOrderReports(searchCriteria, startDate, endDate);
        }
        public List<PurchaseOrderObject> SearchOutletPurchaseOrderReports(string searchCriteria, int outletId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchOutletPurchaseOrderReports(searchCriteria, outletId, startDate, endDate);
        }
        public List<PurchaseOrderObject> SearchEmployeePurchaseOrderReports(string searchCriteria, long employeeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchEmployeePurchaseOrderReports(searchCriteria, employeeId, startDate, endDate);
        }
        public List<PurchaseOrderObject> SearchPurchaseOrderReportsByStatus(string searchCriteria, int status, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchPurchaseOrderReportsByStatus(searchCriteria, status, startDate, endDate);
        }
        #endregion

        #region ESTIMATE REPORTS
        public List<EstimateObject> GetEstimateReports(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetEstimateReports(itemsPerPage, pageNumber, out countG, startDate, endDate);
        }
        public List<EstimateObject> GetEstimateReportsByOutlet(int? itemsPerPage, int? pageNumber, out int countG, int outletId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetEstimatesByOutlet(itemsPerPage, pageNumber, out countG, outletId, startDate, endDate);
        }
        public List<EstimateObject> GetEstimateReportsByEmployee(int? itemsPerPage, int? pageNumber, out int countG, long employeeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetEstimatesByEmployee(itemsPerPage, pageNumber, out countG, employeeId, startDate, endDate);
        }
        public List<EstimateObject> GetEstimatesByConversionStatus(int? itemsPerPage, int? pageNumber, bool status, out int countG, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.GetEstimatesByConversionStatus(itemsPerPage, pageNumber, status, out countG, startDate, endDate);
        }
        public List<EstimateObject> SearchEstimateReports(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchEstimates(searchCriteria, startDate, endDate);
        }
        public List<EstimateObject> SearchOutletEstimateReports(string searchCriteria, int outletId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchOutletEstimate(searchCriteria, outletId, startDate, endDate);
        }
        public List<EstimateObject> SearchEmployeeEstimateReports(string searchCriteria, long employeeId, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchEmployeeEstimate(searchCriteria, employeeId, startDate, endDate);
        }
        public List<EstimateObject> SearchEstimatesByConversionStatus(string searchCriteria, bool status, DateTime startDate, DateTime endDate)
        {
            return _reportRepository.SearchEstimatesByConversionStatus(searchCriteria, status, startDate, endDate);
        }
        #endregion
    }
    
}





