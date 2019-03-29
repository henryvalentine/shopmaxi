using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
    public class EmployeeServices
    {
        private readonly EmployeeRepository _employeeRepository;
        public EmployeeServices()
        {
            _employeeRepository = new EmployeeRepository();
        }

        public long AddEmployee(EmployeeObject employee)
        {
            try
            {
                return _employeeRepository.AddEmployee(employee);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int GetObjectCount()
        {
            try
            {
                return _employeeRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateEmployee(EmployeeObject employee)
        {
            try
            {
                return _employeeRepository.UpdateEmployee(employee);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }
        

        public bool DeleteEmployee(long employeeId)
        {
            try
            {
                return _employeeRepository.DeleteEmployee(employeeId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public bool VerifyPhoneNumber(string phoneNumber)
        {
            return _employeeRepository.VerifyPhoneNumber(phoneNumber);
        }

        public int UpdateAdmin(EmployeeObject employee)
        {
            return _employeeRepository.UpdateAdmin(employee);
        }
        public int UpdateEmployeeProfile(EmployeeObject employee)
        {
            return _employeeRepository.UpdateEmployeeProfile(employee);
        }

        public EmployeeObject GetEmployee(long employeeId)
        {
            return _employeeRepository.GetEmployee(employeeId);
        }

        public bool UpdateProfileImage(string profileImage, long userId)
        {
            return _employeeRepository.UpdateProfileImage(profileImage, userId);
        }

        public EmployeeObject GetEmployeeByProfile(long userId)
        {
            return _employeeRepository.GetEmployeeByProfile(userId);
        }

        public EmployeeObject GetAdminUserProfile(long userId)
        {
            return _employeeRepository.GetAdminUserProfile(userId);
        }

        public UserProfileObject GetCustomerProfile(string aspnetUserId)
        {
            return _employeeRepository.GetCustomerProfile(aspnetUserId);
        }

        public UserProfileObject GetUserProfile(string aspnetUserId)
        {
            return _employeeRepository.GetUserProfile(aspnetUserId);
        }

        public UserProfileObject GetAdminProfile(string aspnetUserId)
        {
            return _employeeRepository.GetAdminProfile(aspnetUserId);
        }

        public List<EmployeeObject> GetEmployees()
        {
            try
            {
                var objList = _employeeRepository.GetEmployees();
                if (objList == null || !objList.Any())
                {
                    return new List<EmployeeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EmployeeObject>();
            }
        }

        public List<EmployeeObject> GetEmployeeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _employeeRepository.GetEmployeeObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<EmployeeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EmployeeObject>();
            }
        }

        public List<EmployeeObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _employeeRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<EmployeeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EmployeeObject>();
            }
        }

        public long GetLastId(int outletId)
        {
            return _employeeRepository.GetLastId(outletId);
        }
    }

}
