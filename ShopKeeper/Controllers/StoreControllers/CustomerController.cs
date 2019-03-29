using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;
using WebGrease.Css.Extensions;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class CustomerController : Controller
	{
        public CustomerController()  
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        
        /// <summary>
        /// Handles calls Ajax from DataTable(to which the Facilities List is/to be bound)
        /// </summary>
        /// <param name="param">
        /// Ajax model that encapsulates all required parameters such as 
        /// filtering, pagination, soting, etc instructions from the DataTable
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCustomerObjects(JQueryDataTableParamModel param, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
           try
            {
                IEnumerable<CustomerObject> filteredCustomerObjects;
                var countG = new CustomerServices().GetObjectCount();

                var pagedCustomerObjects = GetCustomers(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCustomerObjects = new CustomerServices().Search(param.sSearch);
                }
                else
                {
                    filteredCustomerObjects = pagedCustomerObjects;
                }

                if (!filteredCustomerObjects.Any())
                {
                    return Json(new List<CustomerObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<CustomerObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.UserProfileName : sortColumnIndex == 2 ? c.CustomerTypeName : c.StoreOutletName);
                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredCustomerObjects = sortDirection == "asc" ? filteredCustomerObjects.OrderBy(orderingFunction) : filteredCustomerObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredCustomerObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.CustomerId), c.UserProfileName, c.CustomerTypeName, c.StoreOutletName, c.Email, c.MobileNumber, c.OfficeLine, c.BirthDayStr };
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
                return Json(new List<CustomerObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AddCustomer(UserProfileObject customer, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
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

                customer.DateProfiled = DateTime.Now;
                if (userInfo.UserProfile.StoreOutletId < 1)
                {
                    if (customer.CustomerObjects[0].StoreOutletId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please customer's provide an outlet";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }

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
        public ActionResult EditCustomer(UserProfileObject customer, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateCustomer(customer);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var k = new CustomerServices().UpdateCustomer(customer);
                if (k < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = k == -3 ? message_Feedback.Duplicate_Mobile_Number : message_Feedback.Duplicate_Email;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.Error = "Customer information was successfully updated.";
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
        public ActionResult DeleteCustomer(long id, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                var k = new CustomerServices().DeleteCustomer(id);
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
        public ActionResult GetCustomer(long id, string subdomain)
        {
            try
            {
                var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
                if (storeSetting == null || storeSetting.StoreId < 1)
                {
                    return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
                }
                if (id < 1)
                {
                    return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
                }

                var customer = new CustomerServices().GetCustomer(id);
                return Json(customer, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetListObjects(string subdomain)
        {
            try
            {
                var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
                if (storeSetting == null || storeSetting.StoreId < 1)
                {
                    return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
                }
                
                var genericObject = new CustomerGenericObject
                {
                    CustomerTypes = GetCustomerTypes(),
                    StoreOutlets = GetStoreOutlets(),
                    Countries =  new SaleServices().GetCountries()
                }; 

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new ChartOfAccountObject(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult GetCustomerTypes(string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new StoreCustomerTypeObject(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                var customerTypes = GetCustomerTypes();

                return Json(customerTypes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreCustomerTypeObject(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStates(long countryId, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new StoreCustomerTypeObject(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (countryId < 1)
                {
                    return Json(new List<StoreCountryObject>(), JsonRequestBehavior.AllowGet);
                }

                var states = new SaleServices().GetStates(countryId);

                return Json(states, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new ChartOfAccountObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadCustomers(HttpPostedFileBase file, string subdomain)
        {
            var feedBackList = new List<GenericValidator>();

            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(feedBackList, JsonRequestBehavior.AllowGet);
            }
            
            var gVal = new GenericValidator();
            try
            {
                if (file.ContentLength > 0)
                {
                    var folderPath = Server.MapPath("~/BulkUploads/Customers/" + subdomain);

                    var fileName = file.FileName;
                    var path = folderPath + "/" + fileName;

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        var dInfo = new DirectoryInfo(folderPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(
                            new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                FileSystemRights.FullControl,
                                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                    }
                    else
                    {
                        var dInfo = new DirectoryInfo(folderPath);
                        var files = dInfo.GetFiles();
                        if (files.Any())
                        {
                            files.ForEach(fi => fi.Delete());
                        }
                    }

                    file.SaveAs(path);

                    var bulkUploadResults = new CustomerBulkUploadServices().ReadExcelData(path, "customers");

                    if (!bulkUploadResults.Any())
                    {
                        gVal.Code = -1;
                        gVal.Error = "An internal server error was encountered. Please try again later.";
                        feedBackList.Add(gVal);
                        return Json(feedBackList, JsonRequestBehavior.AllowGet);
                    }
                    return Json(bulkUploadResults, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = "The selected file is invalid";
                feedBackList.Add(gVal);
                return Json(feedBackList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                feedBackList.Add(gVal);
                return Json(feedBackList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SearchCustomer(string criteria, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (string.IsNullOrEmpty(criteria))
                {
                    return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
                }

                var customer = new CustomerServices().SearchCustomer(criteria);
                return Json(customer, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
       

        #region Helpers
        private List<UserProfileObject> GetUserProfiles()
        {
            return new UserProfileServices().GetUserProfiles() ?? new List<UserProfileObject>();
        }
        private List<StoreCustomerTypeObject> GetCustomerTypes()
        {
            return new StoreCustomerTypeServices().GetStoreCustomerTypes() ?? new List<StoreCustomerTypeObject>();
        }
        private List<StoreOutletObject> GetStoreOutlets()
        {
            return new StoreOutletServices().GetStoreOutlets() ?? new List<StoreOutletObject>();
        }
        private List<CustomerObject> GetCustomerList()
        {
            return new CustomerServices().GetCustomers() ?? new List<CustomerObject>();
        }
        private List<CustomerObject> GetCustomers(int? itemsPerPage, int? pageNumber)
        {
            return new CustomerServices().GetCustomerObjects(itemsPerPage, pageNumber) ?? new List<CustomerObject>();
        }
        private GenericValidator ValidateCustomer(UserProfileObject customer)
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

            if (customer.DeliveryAddressObject == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Customer_Address_Error;
                return gVal;
            }
            
            if (customer.DeliveryAddressObject.CityId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.City_Selection_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(customer.DeliveryAddressObject.AddressLine1))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Street_Number_Error;
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
        
        #endregion

    }
}
