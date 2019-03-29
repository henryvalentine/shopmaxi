using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.DataObjects.DataObjects.Store;
using UserProfileObject = Shopkeeper.DataObjects.DataObjects.Store.UserProfileObject;

namespace ImportPermitPortal.DataObjects.Helpers
{
    public class GenericValidator
    {
        public string Error { get; set; }
        public string RedirectUri { get; set; }
        public string StoreAddress { get; set; }
        public string StoreName { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string CurrencyCode { get; set; }
        public string StoreAlias { get; set; }
        public string ReferenceCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FilePath { get; set; }
        public long Gx { get; set; }
        public string PackageName { get; set; }
        public long Code { get; set; }
        public int Duration { get; set; }
        public bool IsTrial { get; set; }
        public bool IsBankOption { get; set; }
        public double Magnitude { get; set; }
        public string Date { get; set; }
        public string PaymentOption { get; set; }
        public string FileName { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Time { get; set; }
        public List<string> UserRoles { get; set; }
        public List<ErrorObject> ErrorObjects { get; set; }
        public StoreOutletObject Outlet { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string RefundNoteNumber { get; set; }
        public bool PasswordUpdated { get; set; }
        public long CustomerId { get; set; }
    }

    public class ApplicationMenu
    {
        [Key]
        public Guid MenuId { get; set; }
        public string Description { get; set; }
        public string Route { get; set; }
        public string Module { get; set; }
        public int MenuOrder { get; set; }
        public Boolean RequiresAuthenication { get; set; }

    }

    public class ProductReport
    {
        public long ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class StateReportDefault
    {
        public List<CustomerObject> Customers { get; set; }
        public List<SupplierObject> Suppliers { get; set; }
        public List<StoreItemCategoryObject> Categories { get; set; }
    }


    public class POrderInfo
    {
        public double VATAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double FOB { get; set; }
        public List<PurchaseOrderItemDeliveryObject> DeliveredItems { get; set; }
    }

    public class ErrorObject
    {
        public string SKU { get; set; }
        public string VariationProperty { get; set; }
        public string VariationValue { get; set; }
        public string ErrorMessage { get; set; }
        public string ImagePath { get; set; }
    }
    
    public class UserInfo2
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Feedback { get; set; }
        public string AppPath { get; set; }
        public int Code { get; set; }
        public List<string> Roles { get; set; }
        public bool IsAuthenticated { get; set; }
        public long TicketTimeOut { get; set; }
        public Shopkeeper.DataObjects.DataObjects.Master.UserProfileObject UserProfile { get; set; }
    }

    public class CustomerStatement
    {
        public double BalanceBroughtForward { get; set; }
        public double TotalNet { get; set; }
        public double TotalPaid { get; set; }
        public List<SaleObject> StatemenList { get; set; }
    }


    public class SyncStats
    {
        public bool IsSuccessful { get; set; }
        public int SyncVolume { get; set; }
        public DateTime SyncStartTime { get; set; }
        public DateTime SyncEndTime { get; set; }
        public int TotalChangesUpload { get; set; }
        public int TotalChangesDownload { get; set; }
        public string TableSynced { get; set; }

    }


    public class ReportObject
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ItemsPerPage { get; set; }
        public int PageNumber { get; set; }
        public int EmployeeId { get; set; }
        public int OutletId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public int PaymentMethodTypeId { get; set; }
        public int TypeId { get; set; }
    }

    public class OnlineStoreObject
    {
        public StoreInfo StoreInfo { get; set; }
        public List<StoreItemCategoryObject> ItemCategories { get; set; }
        public List<StoreItemTypeObject> ItemTypes { get; set; }
        public List<StoreItemBrandObject> ItemBrands { get; set; }
        public List<StoreItemObject> Items { get; set; }
        public List<UserProfileObject> Employees { get; set; }
        public List<StoreOutletObject> Outlets { get; set; }
        public List<StorePaymentMethodObject> PaymentMethods { get; set; }

    }
    
    public class UserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Feedback { get; set; }
        public string AppPath { get; set; }
        public double Code { get; set; }
        public string Error { get; set; }
        public List<string> Roles { get; set; }
        public List<ParentMenuObject> UserLinks { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsActivationNeeded { get; set; }
        public long TicketTimeOut { get; set; }
        public UserProfileObject UserProfile { get; set; }
        public XKeyUnPObject XKeyUnP { get; set; }
        public Shopkeeper.DataObjects.DataObjects.Master.StoreSettingObject StoreInfo { get; set; }
    }
    
    public class StoreInfo
    {
        public string StoreName { get; set; }
        public string CompanyName { get; set; }
        public string StoreEmail { get; set; }
        public bool Ltpd { get; set; }
        public string StoreAddress { get; set; }
        public string StoreLogo { get; set; }
        public string LoggedOnUser { get; set; }
        public long TicketTimeOut { get; set; }
        public string StoreAlias { get; set; }
        public bool IsAuthenticated { get; set; }
        public string DefaultCurrencySymbol { get; set; }
        public string DefaultCurrency { get; set; }

    }

}

