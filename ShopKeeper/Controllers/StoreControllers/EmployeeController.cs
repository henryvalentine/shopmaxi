using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;
using ShopKeeper.Models;
using WebGrease.Css.Extensions;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class EmployeeController : Controller
	{
        private static string _identityConnection = ConfigurationManager.ConnectionStrings["MasterIdentityConnection"].ConnectionString;
        public EmployeeController()
		{
			 ViewBag.LoadStatus = "0";
		}

        public EmployeeController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        private void SetAccountConnection(string connectionString)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext(connectionString)));
        } 
        public UserManager<ApplicationUser> UserManager { get; private set; }

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
        public ActionResult GetEmployeeObjects(JQueryDataTableParamModel param)
        {
            var gVal = new GenericValidator();
            try
            {
                IEnumerable<EmployeeObject> filteredEmployeeObjects;
                var countG = new EmployeeServices().GetObjectCount();

                var pagedEmployeeObjects = GetEmployees(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredEmployeeObjects = new EmployeeServices().Search(param.sSearch);
                }
                else
                {
                    filteredEmployeeObjects = pagedEmployeeObjects;
                }

                if (!filteredEmployeeObjects.Any())
                {
                    return Json(new List<EmployeeObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<EmployeeObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Name : sortColumnIndex == 2 ? c.EmployeeNo : sortColumnIndex == 3 ? c.JobTitle
                : sortColumnIndex == 4 ? c.Outlet : sortColumnIndex == 5 ? c.Department : c.StatusStr);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredEmployeeObjects = sortDirection == "asc" ? filteredEmployeeObjects.OrderBy(orderingFunction) : filteredEmployeeObjects.OrderByDescending(orderingFunction);
                filteredEmployeeObjects.ForEach(u =>
                {
                    var roles = UserManager.GetRoles(u.AspNetUserId);
                    if (roles.Any())
                    {
                        u.UserRole = roles[0];
                    }
                });


                var displayedUserProfilenels = filteredEmployeeObjects;
                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.EmployeeId), c.Name, c.EmployeeNo, c.UserRole, c.Outlet, c.Department, c.StatusStr };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredEmployeeObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<EmployeeObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddEmployee(EmployeeObject employee, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }

            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateEmployee(employee);
                if (valStatus.Code < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(employee.Password))
                {
                    gVal.Code = -1;
                    gVal.Error = "ERROR: Please Provide Password.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var checkDuplicatePhoneNumber = new EmployeeServices().VerifyPhoneNumber(employee.PhoneNumber);
                if (checkDuplicatePhoneNumber)
                {
                    gVal.Code = -1;
                    gVal.Error = "A user with the same phone number already exists.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var address = new StoreAddressObject
                {
                    StreetNo = employee.StreetNo,
                    StoreCityId = employee.StoreCityId
                };

                var ad = new StoreAddressServices().AddStoreAddress(address);
                if (ad < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Employee_Address_Add_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                UserManager.UserLockoutEnabledByDefault = false;
                var status = (int)EmployeeStatus.Active;
                var user = new ApplicationUser
                {
                    UserName = employee.Email,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    UserInfo = new ApplicationDbContext.UserProfile
                    {
                        LastName = employee.LastName,
                        OtherNames = employee.OtherNames,
                        IsActive = employee.Status == status,
                        MobileNumber = employee.PhoneNumber,
                        ContactEmail = employee.Email
                    }
                };

                var result = await UserManager.CreateAsync(user, employee.Password);

                if (!result.Succeeded)
                {
                    gVal.Code = -1;
                    gVal.Error = result.Errors.ToList()[0];
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(storeSetting.EntityConnectionString)));
                var role = roleManager.FindById(employee.RoleId);
                if (role == null)
                {
                    gVal.Code = -1;
                    gVal.Error = "User information could not be updated.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                UserManager.AddToRole(user.Id, role.Name);

                //todo: change to dynamic outletid
                employee.StoreAddressId = ad;
                //employee.StoreOutletId = 1;

                var employeeNo = GenerateEmpoyeeNo();

                if (string.IsNullOrEmpty(employeeNo))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Process_Failed;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                employee.EmployeeNo = employeeNo;
                employee.UserId = user.UserInfo.Id;
                var k = new EmployeeServices().AddEmployee(employee);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    gVal.Code = 0;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = k;
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
        public async Task<ActionResult> EditEmployee(EmployeeObject employee, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateEmployee(employee);
                if (valStatus.Code < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_employee"] == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var oldEmployee = Session["_employee"] as EmployeeObject;
                if (oldEmployee == null || oldEmployee.EmployeeId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var user = UserManager.FindByEmail(employee.Email);
                if (user == null)
                {
                    gVal.Code = -1;
                    gVal.Error = "User information could not be updated.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }


                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(storeSetting.EntityConnectionString)));
                var role = roleManager.FindById(employee.RoleId);
                if (role == null)
                {
                    gVal.Code = -1;
                    gVal.Error = "User information could not be updated.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var userRoles = UserManager.GetRoles(oldEmployee.AspNetUserId).ToList();
                if (!userRoles.Any())
                {
                    return Json(new EmployeeObject(), JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(employee.Password))
                {
                    var passwordHash = new PasswordHasher().HashPassword(employee.Password);
                    user.PasswordHash = passwordHash;
                    var result = await UserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        gVal.Code = -1;
                        gVal.Error = "User information could not be updated.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }
                
                var address = new StoreAddressObject
                {
                    StreetNo = employee.StreetNo,
                    StoreAddressId = employee.StoreAddressId,
                    StoreCityId = employee.StoreCityId
                };

                var ad = new StoreAddressServices().UpdateStoreAddress(address);
                if (ad < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Employee_Address_Update_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                oldEmployee.Email = employee.Email;
                oldEmployee.PhoneNumber = employee.PhoneNumber;
                oldEmployee.OtherNames = employee.OtherNames;
                oldEmployee.LastName = employee.LastName;
                oldEmployee.Status = employee.Status;
                oldEmployee.EmployeeNo = employee.EmployeeNo;
                oldEmployee.RoleId = employee.RoleId;
                oldEmployee.DateHired = employee.DateHired;
                oldEmployee.DateLeft = employee.DateLeft;
                oldEmployee.StoreOutletId = employee.StoreOutletId;
                oldEmployee.StoreAddressId = employee.StoreAddressId;
                oldEmployee.StoreDepartmentId = employee.StoreDepartmentId;


                var k = new EmployeeServices().UpdateEmployee(oldEmployee);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? "A different user with similar phone number already exists!" : message_Feedback.Update_Failure;
                    gVal.Code = 0;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var userRole = userRoles[0];

                if (role.Name.ToLower() != userRole.ToLower())
                {
                    UserManager.RemoveFromRole(oldEmployee.AspNetUserId, userRole);
                    UserManager.AddToRole(oldEmployee.AspNetUserId, role.Name);
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

        [HttpPost]
        public async Task<ActionResult> EditProfile(EmployeeObject employee)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateProfile(employee);
                if (valStatus.Code < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_employee"] == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var oldEmployee = Session["_employee"] as EmployeeObject;
                if (oldEmployee == null || oldEmployee.EmployeeId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var user = UserManager.FindByEmail(employee.Email);
                if (user == null)
                {
                    gVal.Code = -1;
                    gVal.Error = "User information could not be updated.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(employee.Password))
                {

                    if (string.IsNullOrEmpty(employee.OriginalPassword))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide your original password.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrEmpty(employee.ConfirmPassword))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide your password password confirmation.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (employee.Password != employee.ConfirmPassword)
                    {
                        gVal.Code = -1;
                        gVal.Error = "The passwords do not match";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var passwordResult = UserManager.CheckPassword(user, employee.OriginalPassword);
                    if (!passwordResult)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Your original password could not be verified.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var passwordHash = new PasswordHasher().HashPassword(employee.Password);
                    user.PasswordHash = passwordHash;
                    var result = await UserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        gVal.Code = -1;
                        gVal.Error = "User information could not be updated.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.PasswordUpdated = true;
                }

                var address = new StoreAddressObject
                {
                    StreetNo = employee.StreetNo,
                    StoreAddressId = employee.StoreAddressId,
                    StoreCityId = employee.StoreCityId
                };

                var ad = new StoreAddressServices().UpdateStoreAddress(address);
                if (ad < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Employee_Address_Update_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                oldEmployee.Email = employee.Email;
                oldEmployee.PhoneNumber = employee.PhoneNumber;
                oldEmployee.OtherNames = employee.OtherNames;
                oldEmployee.LastName = employee.LastName;
                oldEmployee.EmployeeNo = employee.EmployeeNo;
                oldEmployee.Birthday = employee.Birthday;
                oldEmployee.StoreAddressId = employee.StoreAddressId;
                oldEmployee.Birthday = employee.Birthday;

                var k = new EmployeeServices().UpdateEmployeeProfile(oldEmployee);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? "A different user with similar phone number already exists!" : message_Feedback.Update_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = 5;
                gVal.Error = "Your profile was successfully updated.";
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
        public async Task<ActionResult> EditAdminProfile(EmployeeObject employee)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateAdminProfile(employee);
                if (valStatus.Code < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_employee"] == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var oldEmployee = Session["_employee"] as EmployeeObject;
                if (oldEmployee == null || oldEmployee.UserId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var user = UserManager.FindByEmail(employee.Email);
                if (user == null)
                {
                    gVal.Code = -1;
                    gVal.Error = "User information could not be updated.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(employee.Password))
                {

                    if (string.IsNullOrEmpty(employee.OriginalPassword))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide your original password.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrEmpty(employee.ConfirmPassword))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide your password password confirmation.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (employee.Password != employee.ConfirmPassword)
                    {
                        gVal.Code = -1;
                        gVal.Error = "The passwords do not match";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var passwordResult = UserManager.CheckPassword(user, employee.OriginalPassword);
                    if (!passwordResult)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Your original password could not be verified.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var passwordHash = new PasswordHasher().HashPassword(employee.Password);
                    user.PasswordHash = passwordHash;
                    var result = await UserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        gVal.Code = -1;
                        gVal.Error = "User information could not be updated.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.PasswordUpdated = true;
                }


                oldEmployee.Email = employee.Email;
                oldEmployee.PhoneNumber = employee.PhoneNumber;
                oldEmployee.OtherNames = employee.OtherNames;
                oldEmployee.LastName = employee.LastName;
                oldEmployee.EmployeeNo = employee.EmployeeNo;
                oldEmployee.Birthday = employee.Birthday;

                var k = new EmployeeServices().UpdateAdmin(oldEmployee);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? "A different user with similar phone number already exists!" : message_Feedback.Update_Failure;
                    gVal.Code = 0;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = 5;
                gVal.Error = "Your profile was successfully updated.";
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
        public ActionResult DeleteEmployee(long id)
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

                var k = new EmployeeServices().DeleteEmployee(id);
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
        public ActionResult GetEmployee(long id, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (id < 1)
                {
                    return Json(new EmployeeObject(), JsonRequestBehavior.AllowGet);
                }

                var employee = new EmployeeServices().GetEmployee(id);
                if (id < 1)
                {
                    return Json(new EmployeeObject(), JsonRequestBehavior.AllowGet);
                }

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(storeSetting.EntityConnectionString)));
                var roles = roleManager.Roles.ToList();
                if (!roles.Any())
                {
                    return Json(new List<string>(), JsonRequestBehavior.AllowGet);
                }

                var userRoles = UserManager.GetRoles(employee.AspNetUserId).ToList();
                if (!userRoles.Any())
                {
                    return Json(new EmployeeObject(), JsonRequestBehavior.AllowGet);
                }

                var userRole = userRoles[0];

                roles.ForEach(m =>
                {
                    if (m.Name.ToLower() == userRole.ToLower())
                    {
                        employee.RoleId = m.Id;
                    }
                });

                Session["_employee"] = employee;
                return Json(employee, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new EmployeeObject(), JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult GetMyProfile()
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

                var employee = userInfo.Roles.Any(r => r == "Admin") ? new EmployeeServices().GetAdminUserProfile(userInfo.UserProfile.Id) : new EmployeeServices().GetEmployeeByProfile(userInfo.UserProfile.Id);

                if (employee.UserId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your profile could not be retrieved.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                Session["_employee"] = employee;
                return Json(employee, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new EmployeeObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetListObjects(string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new CustomerObject(), JsonRequestBehavior.AllowGet);
            }

            try
            {
                var genericObject = new EmployeeGenericObject
                {
                    //UserProfiles = GetUserProfiles(),
                    Departments = GetDepartments(),
                    //Cities = GetCities()
                    Roles = GetRoleList(storeSetting.EntityConnectionString)
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

        private List<IdentityRole> GetRoleList(string connectionString)
        {
            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(connectionString)));
                var roles = roleManager.Roles.ToList();
                if (!roles.Any())
                {
                    return new List<IdentityRole>();
                }
                var filteredRoles = new List<IdentityRole>();
                roles.ForEach(m =>
                {
                    if (m.Name.ToLower() != "super_admin")
                    {
                        filteredRoles.Add(m);
                    }
                });

                return filteredRoles;
            }
            catch (Exception)
            {
                return new List<IdentityRole>();
            }
        }
        private List<EmployeeObject> GetEmployees(int? itemsPerPage, int? pageNumber)
        {
            return new EmployeeServices().GetEmployeeObjects(itemsPerPage, pageNumber) ?? new List<EmployeeObject>();
        }

        private List<UserProfileObject> GetUserProfiles()
        {
            return new UserProfileServices().GetUserProfiles() ?? new List<UserProfileObject>();
        }

        private List<JobRoleObject> GetJobRoles()
        {
            return new JobRoleServices().GetJobRoles() ?? new List<JobRoleObject>();
        }

        private List<StoreCityObject> GetCities()
        {
            return new StoreCityServices().GetCities() ?? new List<StoreCityObject>();
        }
        private List<StoreDepartmentObject> GetDepartments()
        {
            return new StoreDepartmentServices().GetStoreDepartments() ?? new List<StoreDepartmentObject>();
        }

        private GenericValidator ValidateProfile(EmployeeObject employee)
        {
            var gVal = new GenericValidator();
            if (employee == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (employee.Birthday != null)
            {
                if (employee.Birthday.Value.Year == 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide a valid birthday";
                    return gVal;
                }
            }

            if (employee.StoreCityId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.City_Selection_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.StreetNo))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Employee_Address_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.PhoneNumber))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.OtherNames))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.LastName))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }


            if (string.IsNullOrEmpty(employee.Email))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Email.";
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }

        private GenericValidator ValidateAdminProfile(EmployeeObject employee)
        {
            var gVal = new GenericValidator();
            if (employee == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (employee.Birthday != null)
            {
                if (employee.Birthday.Value.Year == 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide a valid birthday";
                    return gVal;
                }
            }

            if (string.IsNullOrEmpty(employee.PhoneNumber))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.OtherNames))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.LastName))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }


            if (string.IsNullOrEmpty(employee.Email))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Email.";
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }

        private GenericValidator ValidateEmployee(EmployeeObject employee)
        {
            var gVal = new GenericValidator();
            if (employee == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (employee.DateHired > DateTime.Today)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Date_Hired_Error;
                return gVal;
            }

            if (employee.DateLeft != null)
            {
                if (employee.DateHired > employee.DateLeft)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Date_Hired_Error;
                    return gVal;
                }
            }

            if (string.IsNullOrEmpty(employee.RoleId))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.JobRole_Selection_Error;
                return gVal;
            }

            if (employee.StoreCityId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.City_Selection_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.StreetNo))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Employee_Address_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.PhoneNumber))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.OtherNames))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }

            if (string.IsNullOrEmpty(employee.LastName))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Phone Number.";
                return gVal;
            }


            if (string.IsNullOrEmpty(employee.Email))
            {
                gVal.Code = -1;
                gVal.Error = "ERROR: Please Provide Email.";
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }

        private string GenerateEmpoyeeNo()
        {
            try
            {
                var zeroLimit = ConfigurationManager.AppSettings["ZerosLimit"];
                if (string.IsNullOrEmpty(zeroLimit))
                {
                    return null;
                }
                var count = new EmployeeServices().GetLastId(1);
                count++;
                var employeeId = "";
                if (!string.IsNullOrEmpty(zeroLimit))
                {
                    var limit = int.Parse(zeroLimit);
                    if (limit > 0)
                    {
                        if (count < 10)
                        {
                            return "000" + count;
                        }

                        var target = count.ToString(CultureInfo.InvariantCulture);
                        if (target.Count() >= limit)
                        {
                            return target;
                        }
                        var i = target.Count();
                        for (var x = i; x < zeroLimit.Count(); x++)
                        {
                            employeeId += "0";
                        }
                        employeeId += target;
                        return employeeId;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

    }
}
