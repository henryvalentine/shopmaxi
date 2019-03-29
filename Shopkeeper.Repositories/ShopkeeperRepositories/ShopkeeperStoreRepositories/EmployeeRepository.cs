using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class EmployeeRepository
    {
       private readonly IShopkeeperRepository<Employee> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperStoreEntities _db = new ShopKeeperStoreEntities();

       public EmployeeRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<Employee>(_uoWork);
		}

       public long AddEmployee(EmployeeObject employee)
       {
           try
           {
               if (employee == null)
               {
                   return -2;
               }
               using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
               {
                   var duplicates = db.Employees.Count(m => m.UserProfile.LastName == employee.LastName && m.UserProfile.OtherNames == employee.OtherNames && employee.StoreOutletId == m.StoreOutletId);
                   if (duplicates > 0)
                   {
                       return -3;
                   }

                   var employeeEntity = ModelCrossMapper.Map<EmployeeObject, Employee>(employee);
                   if (employeeEntity == null || employeeEntity.StoreDepartmentId < 1)
                   {
                       return -2;
                   }

                   var outlets = db.StoreOutlets.ToList();
                   if (!outlets.Any())
                   {
                       return -2;
                   }

                   employeeEntity.StoreOutletId = outlets[0].StoreOutletId;
                   var returnStatus = _repository.Add(employeeEntity);
                   _uoWork.SaveChanges();
                   return returnStatus.EmployeeId;
               }
           }
           catch (DbEntityValidationException e)
           {
               var str = "";
               foreach (var eve in e.EntityValidationErrors)
               {
                   str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                       eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                   foreach (var ve in eve.ValidationErrors)
                   {
                       str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                           ve.PropertyName, ve.ErrorMessage) + " \n";
                   }
               }
               ErrorLogger.LogError(e.StackTrace, e.Source, str);
               return 0;
           }
       }

       public bool VerifyPhoneNumber(string phoneNumber)
       {
           try
           {
               using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
               {
                   var duplicates = db.UserProfiles.Count(m => m.MobileNumber == phoneNumber);
                   if (duplicates > 0)
                   {
                       return true;
                   }

                   return false;
               }
           }
           catch (Exception ex)
           {
               return false;
           }
       }

       public int UpdateEmployee(EmployeeObject employee)
       {
           try
           {
               if (employee == null)
               {
                   return -2;
               }

               var duplicates = _repository.Count(m => employee.PhoneNumber == m.UserProfile.MobileNumber && m.UserId != employee.UserId);
               if (duplicates > 0)
               {
                   return -3;
               }

               var employeeEntity = ModelCrossMapper.Map<EmployeeObject, Employee>(employee);
               if (employeeEntity == null || employeeEntity.EmployeeId < 1)
               {
                   return -2;
               }

               using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
               {
                   var userprofiles = db.UserProfiles.Where(p => p.Id == employee.UserId).Include("AspNetUsers").ToList();
                   if (!userprofiles.Any())
                   {
                       return -2;
                   }

                   var user = userprofiles[0];
                   var users = user.AspNetUsers;
                   if (!users.Any())
                   {
                       return -2;
                   }

                   const int status = (int)EmployeeStatus.Active;

                   var userInfo = users.ToList()[0];

                   userInfo.Email = employee.Email;
                   userInfo.UserName = employee.Email;

                   user.ContactEmail = employee.Email;
                   user.MobileNumber = employee.PhoneNumber;
                   user.OtherNames = employee.OtherNames;
                   user.LastName = employee.LastName;
                   user.IsActive = employee.Status == status;

                   db.Entry(user).State = EntityState.Modified;
                   db.SaveChanges();
                   db.Entry(employeeEntity).State = EntityState.Modified;
                   db.SaveChanges();
                   db.Entry(userInfo).State = EntityState.Modified;
                   db.SaveChanges();

                   return 5;
               }

           }
           catch (DbEntityValidationException e)
           {
               var str = "";
               foreach (var eve in e.EntityValidationErrors)
               {
                   str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                       eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                   str = eve.ValidationErrors.Aggregate(str, (current, ve) => current + (string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage) + " \n"));
               }
               ErrorLogger.LogError(e.StackTrace, e.Source, str);
               return 0;
           }
       }

       public int UpdateEmployeeProfile(EmployeeObject employee)
       {
           try
           {
               if (employee == null)
               {
                   return -2;
               }

               using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
               {
                   var duplicates = db.UserProfiles.Count(m => employee.PhoneNumber == m.MobileNumber && m.Id != employee.UserId);
                   if (duplicates > 0)
                   {
                       return -3;
                   }

                   var userprofiles = db.UserProfiles.Where(p => p.Id == employee.UserId).Include("AspNetUsers").ToList();
                   if (!userprofiles.Any())
                   {
                       return -2;
                   }

                   var user = userprofiles[0];
                   var users = user.AspNetUsers;
                   if (!users.Any())
                   {
                       return -2;
                   }

                   var userInfo = users.ToList()[0];

                   userInfo.Email = employee.Email;
                   userInfo.UserName = employee.Email;

                   user.ContactEmail = employee.Email;
                   user.MobileNumber = employee.PhoneNumber;
                   user.OtherNames = employee.OtherNames;
                   user.LastName = employee.LastName;
                   user.Birthday = employee.Birthday;

                   db.Entry(user).State = EntityState.Modified;
                   db.SaveChanges();
                   db.Entry(userInfo).State = EntityState.Modified;
                   db.SaveChanges();

                   return 5;
               }

           }
           catch (DbEntityValidationException e)
           {
               var str = "";
               foreach (var eve in e.EntityValidationErrors)
               {
                   str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                       eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                   str = eve.ValidationErrors.Aggregate(str, (current, ve) => current + (string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage) + " \n"));
               }
               ErrorLogger.LogError(e.StackTrace, e.Source, str);
               return 0;
           }
       }

       public int UpdateAdmin(EmployeeObject employee)
       {
           try
           {
               if (employee == null)
               {
                   return -2;
               }

               using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
               {
                   var duplicates = db.UserProfiles.Count(m => employee.PhoneNumber == m.MobileNumber && m.Id != employee.UserId);
                   if (duplicates > 0)
                   {
                       return -3;
                   }

                   var userprofiles = db.UserProfiles.Where(p => p.Id == employee.UserId).Include("AspNetUsers").ToList();
                   if (!userprofiles.Any())
                   {
                       return -2;
                   }

                   var user = userprofiles[0];
                   var users = user.AspNetUsers;
                   if (!users.Any())
                   {
                       return -2;
                   }

                   var userInfo = users.ToList()[0];

                   userInfo.Email = employee.Email;
                   userInfo.UserName = employee.Email;

                   user.ContactEmail = employee.Email;
                   user.MobileNumber = employee.PhoneNumber;
                   user.OtherNames = employee.OtherNames;
                   user.LastName = employee.LastName;
                   user.Birthday = employee.Birthday;

                   db.Entry(user).State = EntityState.Modified;
                   db.SaveChanges();
                   db.Entry(userInfo).State = EntityState.Modified;
                   db.SaveChanges();

                   return 5;
               }

           }
           catch (DbEntityValidationException e)
           {
               var str = "";
               foreach (var eve in e.EntityValidationErrors)
               {
                   str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                       eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                   str = eve.ValidationErrors.Aggregate(str, (current, ve) => current + (string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage) + " \n"));
               }
               ErrorLogger.LogError(e.StackTrace, e.Source, str);
               return 0;
           }
       }

       public bool UpdateProfileImage(string profileImage, long userId)
       {
           try
           {
               using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
               {

                   var profiles = db.UserProfiles.Where(s => s.Id == userId).ToList();
                   if (!profiles.Any())
                   {
                       return false;
                   }
                   var profile = profiles[0];
                   profile.PhotofilePath = profileImage;
                   db.Entry(profile).State = EntityState.Modified;
                   db.SaveChanges();
                   return true;
               }

           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }


       public bool DeleteEmployee(long employeeId)
       {
           try
           {
               var returnStatus = _repository.Remove(employeeId);
               _uoWork.SaveChanges();
               return returnStatus.EmployeeId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public EmployeeObject GetEmployee(long employeeId)
       {
           try
           {
               using (var db = _db)
               {
                   var employeeList = (from em in db.Employees.Where(m => m.EmployeeId == employeeId)
                                       join ps in db.UserProfiles on em.UserId equals ps.Id
                                       join asp in db.AspNetUsers on ps.Id equals asp.UserInfo_Id
                                       join sa in db.StoreAddresses on em.StoreAddressId equals sa.StoreAddressId
                                       join sc in db.StoreCities on sa.StoreCityId equals sc.StoreCityId
                                       join st in db.StoreStates on sc.StoreStateId equals st.StoreStateId
                                       join so in db.StoreOutlets on em.StoreOutletId equals so.StoreOutletId
                                       join sd in db.StoreDepartments on em.StoreDepartmentId equals sd.StoreDepartmentId
                                       select new EmployeeObject
                                       {
                                           EmployeeId = em.EmployeeId,
                                           UserId = em.UserId,
                                           EmployeeNo = em.EmployeeNo,
                                           Email = ps.ContactEmail,
                                           AspNetUserId = asp.Id,
                                           StoreCityId = sc.StoreCityId,
                                           StoreStateId = st.StoreStateId,
                                           StoreCountryId = st.StoreCountryId,
                                           DateHired = em.DateHired,
                                           DateLeft = em.DateLeft,
                                           StoreOutletId = em.StoreOutletId,
                                           StoreAddressId = sa.StoreAddressId,
                                           StoreDepartmentId = sd.StoreDepartmentId,
                                           Department = sd.Name,
                                           PhoneNumber = ps.MobileNumber,
                                           Outlet = so.OutletName,
                                           Name = ps.LastName + " " + ps.OtherNames,
                                           LastName = ps.LastName,
                                           OtherNames = ps.OtherNames,
                                           Address = sa.StreetNo,
                                           CityName = sc.Name,
                                           Status = em.Status
                                       }).ToList();
                   if (!employeeList.Any())
                   {
                       return new EmployeeObject();
                   }
                   var employee = employeeList[0];
                   employee.DateHiredStr = employee.DateHired.ToString("dd/MM/yyyy");
                   employee.StatusStr = Enum.GetName(typeof(EmployeeStatus), employee.Status);
                   if (employee.DateLeft != null)
                   {
                       employee.DateLeftStr = ((DateTime)employee.DateLeft).ToString("dd/MM/yyyy");
                   }
                   return employee;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new EmployeeObject();
           }
       }

       public EmployeeObject GetEmployeeByProfile(long userId)
       {
           try
           {
               using (var db = _db)
               {
                   var employeeList = (from ps in db.UserProfiles.Where(m => m.Id == userId)
                                       join em in db.Employees on ps.Id equals em.UserId
                                       join asp in db.AspNetUsers on ps.Id equals asp.UserInfo_Id
                                       join sa in db.StoreAddresses on em.StoreAddressId equals sa.StoreAddressId
                                       join sc in db.StoreCities on sa.StoreCityId equals sc.StoreCityId
                                       join st in db.StoreStates on sc.StoreStateId equals st.StoreStateId
                                       join cnt in db.StoreCountries on st.StoreCountryId equals cnt.StoreCountryId
                                       join so in db.StoreOutlets on em.StoreOutletId equals so.StoreOutletId
                                       join sd in db.StoreDepartments on em.StoreDepartmentId equals sd.StoreDepartmentId
                                       select new EmployeeObject
                                       {
                                           EmployeeId = em.EmployeeId,
                                           UserId = em.UserId,
                                           EmployeeNo = em.EmployeeNo,
                                           PhotofilePath = ps.PhotofilePath,
                                           AspNetUserId = asp.Id,
                                           StoreCityId = sc.StoreCityId,
                                           StoreCountryId = cnt.StoreCountryId,
                                           StoreStateId = st.StoreStateId,
                                           PhoneNumber = ps.MobileNumber,
                                           Email = ps.ContactEmail,
                                           DateHired = em.DateHired,
                                           DateLeft = em.DateLeft,
                                           LastName = ps.LastName,
                                           OtherNames = ps.OtherNames,
                                           Birthday = ps.Birthday,
                                           StoreOutletId = em.StoreOutletId,
                                           StoreAddressId = sa.StoreAddressId,
                                           StoreDepartmentId = sd.StoreDepartmentId,
                                           Department = sd.Name,
                                           Outlet = so.OutletName,
                                           Name = ps.LastName + " " + ps.OtherNames,
                                           Address = sa.StreetNo,
                                           CityName = sc.Name,
                                           Status = em.Status
                                       }).ToList();
                   if (!employeeList.Any())
                   {
                       return new EmployeeObject();
                   }
                   var employee = employeeList[0];
                   employee.DateHiredStr = employee.DateHired.ToString("dd/MM/yyyy");
                   employee.StatusStr = Enum.GetName(typeof(EmployeeStatus), employee.Status);
                   if (employee.DateLeft != null)
                   {
                       employee.DateLeftStr = ((DateTime)employee.DateLeft).ToString("dd/MM/yyyy");
                   }

                   if (employee.Birthday != null)
                   {
                       employee.BirthdayStr = ((DateTime)employee.Birthday).ToString("dd/MM/yyyy");
                   }
                   return employee;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new EmployeeObject();
           }
       }

       public EmployeeObject GetAdminUserProfile(long userId)
       {
           try
           {
               using (var db = _db)
               {
                   var profileList = (from ps in db.UserProfiles.Where(m => m.Id == userId)
                                      join asp in db.AspNetUsers on ps.Id equals asp.UserInfo_Id
                                      select new EmployeeObject
                                      {
                                          UserId = ps.Id,
                                          PhotofilePath = ps.PhotofilePath,
                                          AspNetUserId = asp.Id,
                                          Birthday = ps.Birthday,
                                          PhoneNumber = ps.MobileNumber,
                                          Email = ps.ContactEmail,
                                          LastName = ps.LastName,
                                          OtherNames = ps.OtherNames,
                                          Name = ps.LastName + " " + ps.OtherNames
                                      }).ToList();

                   if (!profileList.Any())
                   {
                       return new EmployeeObject();
                   }
                   var profile = profileList[0];
                   profile.DateHiredStr = profile.DateHired.ToString("dd/MM/yyyy");
                   profile.StatusStr = Enum.GetName(typeof(EmployeeStatus), profile.Status);
                   if (profile.DateLeft != null)
                   {
                       profile.DateLeftStr = ((DateTime)profile.DateLeft).ToString("dd/MM/yyyy");
                   }

                   if (profile.Birthday != null)
                   {
                       profile.BirthdayStr = ((DateTime)profile.Birthday).ToString("dd/MM/yyyy");
                   }
                   return profile;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new EmployeeObject();
           }
       }

       public UserProfileObject GetAdminProfile(string aspnetUserId)
       {
           try
           {
               using (var db = _db)
               {
                   var profileList = (from asp in db.AspNetUsers.Where(m => m.Id == aspnetUserId)
                                      join ps in db.UserProfiles on asp.UserInfo_Id equals ps.Id
                                      select new UserProfileObject
                                      {
                                          Id = ps.Id,
                                          IsActive = ps.IsActive,
                                          Name = ps.LastName + " " + ps.OtherNames,
                                          PhotofilePath = ps.PhotofilePath
                                      }).ToList();

                   if (!profileList.Any())
                   {
                       return new UserProfileObject();
                   }
                   var profile = profileList[0];
                   if (string.IsNullOrEmpty(profile.PhotofilePath))
                   {
                       profile.PhotofilePath = "/Content/images/noImage.png";
                   }

                   profile.DateHiredStr = profile.DateHired.ToString("dd/MM/yyyy");
                   profile.StatusStr = Enum.GetName(typeof(EmployeeStatus), profile.Status);

                   if (profile.DateLeft != null)
                   {
                       profile.DateLeftStr = ((DateTime)profile.DateLeft).ToString("dd/MM/yyyy");
                   }

                   if (profile.Birthday != null)
                   {
                       profile.BirthdayStr = ((DateTime)profile.Birthday).ToString("dd/MM/yyyy");
                   }

                   return profile;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new UserProfileObject();
           }
       }

       public UserProfileObject GetUserProfile(string aspnetUserId)
       {
           try
           {
               using (var db = _db)
               {
                   var profileList = (from asp in db.AspNetUsers.Where(m => m.Id == aspnetUserId)
                                      join ps in db.UserProfiles on asp.UserInfo_Id equals ps.Id
                                      join em in db.Employees on ps.Id equals em.UserId
                                      join sa in db.StoreAddresses on em.StoreAddressId equals sa.StoreAddressId
                                      join sc in db.StoreCities on sa.StoreCityId equals sc.StoreCityId
                                      join so in db.StoreOutlets on em.StoreOutletId equals so.StoreOutletId
                                      join sd in db.StoreDepartments on em.StoreDepartmentId equals sd.StoreDepartmentId
                                      select new UserProfileObject
                                      {
                                          EmployeeId = em.EmployeeId,
                                          Id = ps.Id,
                                          EmployeeNo = em.EmployeeNo,
                                          AspNetUserId = asp.Id,
                                          StoreCityId = sc.StoreCityId,
                                          DateHired = em.DateHired,
                                          IsActive = ps.IsActive,
                                          DateLeft = em.DateLeft,
                                          StoreOutletId = em.StoreOutletId,
                                          StoreAddressId = sa.StoreAddressId,
                                          StoreDepartmentId = sd.StoreDepartmentId,
                                          Department = sd.Name,
                                          Outlet = so.OutletName,
                                          Name = ps.LastName + " " + ps.OtherNames,
                                          Address = sa.StreetNo,
                                          CityName = sc.Name,
                                          Status = em.Status,
                                          PhotofilePath = ps.PhotofilePath
                                      }).ToList();

                   if (!profileList.Any())
                   {
                       return new UserProfileObject();
                   }

                   var profile = profileList[0];

                   if (string.IsNullOrEmpty(profile.PhotofilePath))
                   {
                       profile.PhotofilePath = "/Content/images/noImage.png";
                   }

                   profile.DateHiredStr = profile.DateHired.ToString("dd/MM/yyyy");
                   profile.StatusStr = Enum.GetName(typeof(EmployeeStatus), profile.Status);
                   if (profile.DateLeft != null)
                   {
                       profile.DateLeftStr = ((DateTime)profile.DateLeft).ToString("dd/MM/yyyy");
                   }
                   return profile;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new UserProfileObject();
           }
       }

       public UserProfileObject GetCustomerProfile(string aspnetUserId)
       {
           try
           {
               using (var db = _db)
               {
                   var profileList = (from asp in db.AspNetUsers.Where(m => m.Id == aspnetUserId)
                                      join ps in db.UserProfiles on asp.UserInfo_Id equals ps.Id
                                      join cu in db.Customers on ps.Id equals cu.UserId
                                      select new UserProfileObject
                                      {
                                          Id = ps.Id,
                                          AspNetUserId = asp.Id,
                                          IsActive = ps.IsActive,
                                          Name = ps.LastName + " " + ps.OtherNames,
                                          PhotofilePath = ps.PhotofilePath
                                      }).ToList();

                   if (!profileList.Any())
                   {
                       return new UserProfileObject();
                   }

                   var profile = profileList[0];

                   if (string.IsNullOrEmpty(profile.PhotofilePath))
                   {
                       profile.PhotofilePath = "/Content/images/noImage.png";
                   }
                   
                   return profile;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new UserProfileObject();
           }
       }

       public List<EmployeeObject> GetEmployeeObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               if ((itemsPerPage == null || !(itemsPerPage > 0)) || (pageNumber == null || !(pageNumber >= 0)))
                   return new List<EmployeeObject>();
               var tpageNumber = (int)pageNumber;
               var tsize = (int)itemsPerPage;
               using (var db = _db)
               {
                   var employeeList = (from em in db.Employees.OrderBy(m => m.UserProfile.LastName).Skip((tpageNumber) * tsize).Take(tsize)
                                       join ps in db.UserProfiles on em.UserId equals ps.Id
                                       join asp in db.AspNetUsers on ps.Id equals asp.UserInfo_Id
                                       join sa in db.StoreAddresses on em.StoreAddressId equals sa.StoreAddressId
                                       join sc in db.StoreCities on sa.StoreCityId equals sc.StoreCityId
                                       join so in db.StoreOutlets on em.StoreOutletId equals so.StoreOutletId
                                       join sd in db.StoreDepartments on em.StoreDepartmentId equals sd.StoreDepartmentId
                                       select new EmployeeObject
                                       {
                                           EmployeeId = em.EmployeeId,
                                           UserId = em.UserId,
                                           EmployeeNo = em.EmployeeNo,
                                           AspNetUserId = asp.Id,
                                           DateHired = em.DateHired,
                                           StoreCityId = sc.StoreCityId,
                                           DateLeft = em.DateLeft,
                                           StoreOutletId = em.StoreOutletId,
                                           StoreAddressId = sa.StoreAddressId,
                                           StoreDepartmentId = sd.StoreDepartmentId,
                                           Department = sd.Name,
                                           Outlet = so.OutletName,
                                           Name = ps.LastName + " " + ps.OtherNames,
                                           Address = sa.StreetNo,
                                           CityName = sc.Name,
                                           Status = em.Status
                                       }).ToList();

                   if (!employeeList.Any())
                   {
                       return new List<EmployeeObject>();
                   }
                   employeeList.ForEach(m =>
                   {
                       m.DateHiredStr = m.DateHired.ToString("dd/MM/yyyy");
                       m.StatusStr = Enum.GetName(typeof(EmployeeStatus), m.Status);
                       if (m.DateLeft != null)
                       {
                           m.DateLeftStr = ((DateTime)m.DateLeft).ToString("dd/MM/yyyy");
                       }
                   });

                   return employeeList;
               }
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

               using (var db = _db)
               {
                   var employeeList = (from ps in db.UserProfiles.Where(m => m.LastName.ToLower().Trim().Contains(searchCriteria) || m.OtherNames.ToLower().Trim().Contains(searchCriteria)).OrderBy(o => o.LastName)
                                       join em in db.Employees on ps.Id equals em.UserId
                                       join asp in db.AspNetUsers on ps.Id equals asp.UserInfo_Id
                                       join so in db.StoreOutlets on em.StoreOutletId equals so.StoreOutletId
                                       join sd in db.StoreDepartments on em.StoreDepartmentId equals sd.StoreDepartmentId
                                       select new EmployeeObject
                                       {
                                           EmployeeId = em.EmployeeId,
                                           UserId = em.UserId,
                                           EmployeeNo = em.EmployeeNo,
                                           AspNetUserId = asp.Id,
                                           DateHired = em.DateHired,
                                           DateLeft = em.DateLeft,
                                           StoreOutletId = em.StoreOutletId,
                                           StoreDepartmentId = sd.StoreDepartmentId,
                                           Department = sd.Name,
                                           Outlet = so.OutletName,
                                           Name = ps.LastName + " " + ps.OtherNames,
                                           Status = em.Status
                                       }).ToList();

                   if (!employeeList.Any())
                   {
                       return new List<EmployeeObject>();
                   }

                   employeeList.ForEach(m =>
                   {
                       m.DateHiredStr = m.DateHired.ToString("dd/MM/yyyy");
                       m.StatusStr = Enum.GetName(typeof(EmployeeStatus), m.Status);
                       if (m.DateLeft != null)
                       {
                           m.DateLeftStr = ((DateTime)m.DateLeft).ToString("dd/MM/yyyy");
                       }
                   });
                   return employeeList;
               }
           }

           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<EmployeeObject>();
           }
       }

       public int GetObjectCount()
       {
           try
           {
               return _repository.Count();
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public int GetObjectCount(Expression<Func<Employee, bool>> predicate)
       {
           try
           {
               return _repository.Count(predicate);
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }
       public List<EmployeeObject> GetEmployees()
       {
           try
           {
               var employeeEntityList = _repository.GetAll("UserProfile").ToList();
               if (!employeeEntityList.Any())
               {
                   return new List<EmployeeObject>();
               }
               var employeeObjList = new List<EmployeeObject>();
               employeeEntityList.ForEach(m =>
               {
                   var employeeObject = ModelCrossMapper.Map<Employee, EmployeeObject>(m);
                   if (employeeObject != null && employeeObject.EmployeeId > 0)
                   {
                       employeeObject.Name = m.UserProfile.LastName + " " + m.UserProfile.OtherNames;
                       employeeObjList.Add(employeeObject);
                   }
               });
               return employeeObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }

       public long GetLastId(int outletId)
       {
           try
           {

               return _repository.Count(m => m.StoreOutletId == outletId);
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }
    }
}
