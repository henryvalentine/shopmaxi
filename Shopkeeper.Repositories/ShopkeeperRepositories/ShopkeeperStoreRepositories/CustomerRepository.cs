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
    public class CustomerRepository
    {
       private readonly IShopkeeperRepository<Customer> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperStoreEntities _dbStoreEntities;
       public CustomerRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _dbStoreEntities = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_dbStoreEntities);
           _repository = new ShopkeeperRepository<Customer>(_uoWork);
		}

       public long AddCustomer(UserProfileObject user)
       {
           try
           {
               if (user == null || user.CustomerObjects == null || !user.CustomerObjects.Any() || user.DeliveryAddressObject == null || user.DeliveryAddressObject.CityId < 1)
               {
                   return -2;
               }

               using (var db = _dbStoreEntities)
               {
                   var duplicateMobiles = db.UserProfiles.Count(m => m.MobileNumber == user.MobileNumber);
                   if (duplicateMobiles > 0)
                   {
                       return -3;
                   }

                   if (!string.IsNullOrEmpty(user.ContactEmail))
                   {
                       var duplicateMails = db.UserProfiles.Count(m => m.ContactEmail == user.ContactEmail);
                       if (duplicateMails > 0)
                       {
                           return -4;
                       }
                   }

                   var userEntity = ModelCrossMapper.Map<UserProfileObject, UserProfile>(user);
                   if (userEntity == null || string.IsNullOrEmpty(userEntity.LastName) || string.IsNullOrEmpty(userEntity.OtherNames))
                   {
                       return -2;
                   }
                   userEntity.IsActive = true;
                   var processedUser = db.UserProfiles.Add(userEntity);
                   db.SaveChanges();

                   var customerInfo = user.CustomerObjects.ToList()[0];
                   var customer = new Customer
                   {
                       DateProfiled = DateTime.Now,
                       StoreCustomerTypeId = customerInfo.StoreCustomerTypeId,
                       StoreOutletId = customerInfo.StoreOutletId,
                       ContactPersonId = user.ContactPersonId,
                       UserId = processedUser.Id
                   };

                   var processedCustomer = db.Customers.Add(customer);
                   db.SaveChanges();

                   var deliveryAddress = new DeliveryAddress
                   {
                       CustomerId = processedCustomer.CustomerId,
                       AddressLine1 = user.DeliveryAddressObject.AddressLine1,
                       AddressLine2 = user.DeliveryAddressObject.AddressLine2,
                       CityId = user.DeliveryAddressObject.CityId,
                       MobileNumber = user.MobileNumber,
                       TelephoneNumber = user.OfficeLine,
                       ContactEmail = user.ContactEmail
                   };

                   db.DeliveryAddresses.Add(deliveryAddress);
                   db.SaveChanges();

                   return processedUser.Id;
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

       public long AddCustomer(CustomerObject user)
       {
           try
           {
               if (user == null)
               {
                   return -2;
               }

               var customerEntity = ModelCrossMapper.Map<CustomerObject, Customer>(user);
               if (customerEntity == null || customerEntity.StoreCustomerTypeId < 1)
               {
                   return -2;
               }

               using (var db = _dbStoreEntities)
               {
                   var duplicateMobiles = db.UserProfiles.Count(m => m.MobileNumber == user.MobileNumber);
                   if (duplicateMobiles > 0)
                   {
                       return -3;
                   }


                   if (!string.IsNullOrEmpty(user.ContactEmail))
                   {
                       var duplicateMails = db.UserProfiles.Count(m => m.ContactEmail == user.ContactEmail);
                       if (duplicateMails > 0)
                       {
                           return -4;
                       }
                   }

                   var profileEntity = new UserProfile
                   {
                       Id = 0,
                       LastName = user.LastName,
                       OtherNames = user.OtherNames,
                       Gender = user.Gender,
                       Birthday = user.Birthday,
                       PhotofilePath = null,
                       IsActive = true,
                       ContactEmail = user.ContactEmail,
                       MobileNumber = user.MobileNumber,
                       OfficeLine = user.OfficeLine
                   };

                   var profile = db.UserProfiles.Add(profileEntity);
                   db.SaveChanges();

                   customerEntity.UserId = profile.Id;
                   var processedCustomer = db.Customers.Add(customerEntity);
                   db.SaveChanges();
                   return processedCustomer.CustomerId;
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

       public int UpdateCustomer(UserProfileObject user)
       {
           try
           {
               if (user == null || user.CustomerObjects == null || user.DeliveryAddressObject == null || user.DeliveryAddressObject.CityId < 1)
               {
                   return -2;
               }
               using (var db = _dbStoreEntities)
               {
                   var duplicateMobiles = db.UserProfiles.Count(m => m.MobileNumber == user.MobileNumber && m.Id != user.Id);
                   if (duplicateMobiles > 0)
                   {
                       return -3;
                   }

                   if (!string.IsNullOrEmpty(user.ContactEmail))
                   {
                       var duplicateMails = db.UserProfiles.Count(m => m.ContactEmail == user.ContactEmail && m.Id != user.Id);
                       if (duplicateMails > 0)
                       {
                           return -4;
                       }
                   }

                   var userEntities = db.UserProfiles.Where(p => p.Id == user.Id).ToList();
                   if (!userEntities.Any())
                   {
                       return 0;
                   }
                   var userEntity = userEntities[0];
                   var customerInfo = user.CustomerObjects.ToList()[0];
                   var customers = db.Customers.Where(p => p.CustomerId == customerInfo.CustomerId).ToList();
                   if (!customers.Any())
                   {
                       return 0;
                   }

                   var deliveryAddresses = db.DeliveryAddresses.Where(d => d.Id == user.DeliveryAddressObject.Id).ToList();
                   if (!deliveryAddresses.Any())
                   {
                       return 0;
                   }

                   var customerEntity = customers[0];
                   userEntity.LastName = user.LastName;
                   userEntity.OtherNames = user.OtherNames;
                   userEntity.Gender = user.Gender;
                   userEntity.Birthday = user.Birthday;
                   userEntity.ContactEmail = user.ContactEmail;
                   userEntity.MobileNumber = user.MobileNumber;
                   userEntity.OfficeLine = user.OfficeLine;


                   db.Entry(userEntity).State = EntityState.Modified;
                   db.SaveChanges();

                   customerEntity.StoreCustomerTypeId = customerInfo.StoreCustomerTypeId;
                   customerEntity.ContactPersonId = user.ContactPersonId;
                   if (customerInfo.StoreOutletId > 0)
                   {
                       customerEntity.StoreOutletId = customerInfo.StoreOutletId;
                   }

                   db.Entry(customerEntity).State = EntityState.Modified;
                   db.SaveChanges();

                   var deliveryAddress = deliveryAddresses[0];



                   deliveryAddress.MobileNumber = user.MobileNumber;
                   deliveryAddress.TelephoneNumber = user.OfficeLine;
                   deliveryAddress.ContactEmail = user.ContactEmail;
                   deliveryAddress.AddressLine1 = user.DeliveryAddressObject.AddressLine1;
                   deliveryAddress.AddressLine2 = user.DeliveryAddressObject.AddressLine2;
                   deliveryAddress.CityId = user.DeliveryAddressObject.CityId;

                   db.Entry(deliveryAddress).State = EntityState.Modified;
                   db.SaveChanges();

                   return 5;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteCustomer(long customerId)
       {
           try
           {
               using (var db = _dbStoreEntities)
               {
                   var customers = db.Customers.Where(c => c.CustomerId == customerId).ToList();
                   if (!customers.Any())
                   {
                       return false;
                   }
                   var customer = customers[0];
                   var userProfiles = db.UserProfiles.Where(p => p.Id == customer.UserId).ToList();
                   if (!userProfiles.Any())
                   {
                       return false;
                   }
                   var profile = userProfiles[0];
                   db.Customers.Remove(customer);
                   db.SaveChanges();

                   db.UserProfiles.Remove(profile);
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

       public UserProfileObject GetCustomer(long customerId)
       {
           try
           {
               using (var db = _dbStoreEntities)
               {
                   var customers = db.Customers.Where(m => m.CustomerId == customerId).Include("UserProfile").Include("StoreCustomerType").Include("StoreOutlet").Include("DeliveryAddresses").Include("CustomerInvoices").ToList();
                   if (!customers.Any())
                   {
                       return new UserProfileObject();
                   }

                   var user = customers[0].UserProfile;
                   var customer = customers[0];
                   if (user == null || user.Id < 1)
                   {
                       return new UserProfileObject();
                   }

                   var userObject = ModelCrossMapper.Map<UserProfile, UserProfileObject>(user);
                   if (userObject == null || userObject.Id < 1)
                   {
                       return new UserProfileObject();
                   }

                   var customerObject = ModelCrossMapper.Map<Customer, CustomerObject>(customer);
                   if (customerObject == null || customerObject.CustomerId < 1)
                   {
                       return new UserProfileObject();
                   }

                   if (customer.CustomerInvoices.Any())
                   {
                       var invoice = customer.CustomerInvoices.ToList()[0];
                       userObject.CustomerInvoiceObject = new CustomerInvoiceObject
                       {
                           TotalAmountPaid = invoice.TotalAmountPaid,
                           TotalAmountDue = invoice.TotalAmountDue,
                           InvoiceBalance = invoice.InvoiceBalance
                       };

                   }

                   customerObject.CustomerTypeName = customer.StoreCustomerType.Name;
                   //customerObject.StoreOutletName = customer.StoreOutlet.OutletName;
                   userObject.BirthdayStr = userObject.Birthday != null ? ((DateTime)userObject.Birthday).ToString("dd/MM/yyyy") : "";
                   customerObject.FirstPurchaseDateStr = customer.FirstPurchaseDate != null ? ((DateTime)customer.FirstPurchaseDate).ToString("dd/MM/yyyy") : "";
                   userObject.CustomerObjects = new List<CustomerObject> { customerObject };
                   userObject.DeliveryAddressObject = new DeliveryAddressObject();
                   userObject.ContactPersonId = customer.ContactPersonId;
                   if (customer.DeliveryAddresses != null && customer.DeliveryAddresses.Any())
                   {
                       var address = customer.DeliveryAddresses.ToList()[0];

                       var cities = (from c in db.StoreCities.Where(s => s.StoreCityId == address.CityId)
                                     join s in db.StoreStates on c.StoreStateId equals s.StoreStateId
                                     join co in db.StoreCountries on s.StoreCountryId equals co.StoreCountryId
                                     select new StoreCityObject
                                     {
                                         StoreCityId = c.StoreCityId,
                                         Name = c.Name,
                                         StoreStateId = s.StoreStateId,
                                         StateName = s.Name,
                                         CountryId = co.StoreCountryId,
                                         CountryName = co.Name

                                     }).ToList();

                       if (cities.Any())
                       {
                           var city = cities[0];

                           userObject.DeliveryAddressObject = new DeliveryAddressObject
                           {
                               Id = address.Id,
                               StateId = city.StoreStateId,
                               CountryId = city.CountryId,
                               CityName = city.Name,
                               StateName = city.StateName,
                               CountryName = city.CountryName,
                               AddressLine1 = address.AddressLine1,
                               AddressLine2 = address.AddressLine2,
                               CityId = address.CityId,
                               MobileNumber = userObject.MobileNumber,
                               TelephoneNumber = userObject.OfficeLine
                           };
                       }
                   }
                   return userObject;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new UserProfileObject();
           }
       }

       public List<CustomerObject> SearchCustomer(string searchCriteria)
       {
           try
           {
               using (var db = _dbStoreEntities)
               {
                   var searchList = (from p in db.UserProfiles
                                     where p.LastName.ToLower().Contains(searchCriteria.ToLower()) || p.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                      || p.MobileNumber.Contains(searchCriteria)
                                     join c in db.Customers on p.Id equals c.UserId
                                     join ct in db.StoreCustomerTypes on c.StoreCustomerTypeId equals ct.StoreCustomerTypeId

                                     select new CustomerObject
                                     {
                                         UserProfileName = p.LastName + " " + p.OtherNames + "(" + p.MobileNumber + ")",
                                         CustomerTypeName = ct.Name,
                                         CreditLimit = ct.CreditLimit,
                                         CreditWorthy = ct.CreditWorthy,
                                         CustomerId = c.CustomerId,
                                         ReferredByCustomerId = c.ReferredByCustomerId,
                                         StoreCustomerTypeId = ct.StoreCustomerTypeId,
                                         Email = p.ContactEmail,
                                         MobileNumber = p.MobileNumber,
                                         OfficeLine = p.OfficeLine,
                                         Birthday = p.Birthday,
                                         UserId = p.Id,

                                     }).ToList();

                   if (!searchList.Any())
                   {
                       return new List<CustomerObject>();
                   }

                   return searchList;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<CustomerObject>();
           }
       }

       public List<CustomerObject> GetCustomerObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               List<Customer> customerEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   customerEntityList = _repository.GetWithPaging(m => m.UserProfile.LastName, tpageNumber, tsize, "UserProfile, StoreCustomerType, StoreOutlet").ToList();
               }

               else
               {
                   customerEntityList = _repository.GetAll("UserProfile, StoreCustomerType, StoreOutlet").ToList();
               }

               if (!customerEntityList.Any())
               {
                   return new List<CustomerObject>();
               }
               var customerObjList = new List<CustomerObject>();
               customerEntityList.ForEach(m =>
               {
                   var customerObject = ModelCrossMapper.Map<Customer, CustomerObject>(m);
                   if (customerObject != null && customerObject.CustomerId > 0)
                   {
                       customerObject.UserProfileName = m.UserProfile.LastName + " " + m.UserProfile.OtherNames;
                       customerObject.CustomerTypeName = m.StoreCustomerType.Name;
                       customerObject.Email = m.UserProfile.ContactEmail;
                       customerObject.MobileNumber = m.UserProfile.MobileNumber;
                       customerObject.OfficeLine = m.UserProfile.OfficeLine;
                       customerObject.Gender = m.UserProfile.Gender;
                       customerObject.StoreOutletName = m.StoreOutlet.OutletName;
                       customerObject.BirthDayStr = m.UserProfile.Birthday != null ? ((DateTime)m.UserProfile.Birthday).ToString("dd/MM/yyyy") : "";
                       customerObjList.Add(customerObject);
                   }
               });

               if (!customerObjList.Any())
               {
                   return new List<CustomerObject>();
               }

               return customerObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<CustomerObject>();
           }
       }

       public List<CustomerObject> Search(string searchCriteria)
       {
           try
           {
               using (var db = _dbStoreEntities)
               {
                   var searchList = (from p in db.UserProfiles.Where(i => i.LastName.ToLower().Contains(searchCriteria.ToLower()) || i.OtherNames.ToLower().Contains(searchCriteria.ToLower()) || i.MobileNumber.Contains(searchCriteria))
                                     join c in db.Customers on p.Id equals c.UserId
                                     join ct in db.StoreCustomerTypes on c.StoreCustomerTypeId equals ct.StoreCustomerTypeId
                                     join so in db.StoreOutlets on c.StoreOutletId equals so.StoreOutletId
                                     select new CustomerObject
                                     {
                                         UserProfileName = p.LastName + " " + p.OtherNames,
                                         CustomerTypeName = ct.Name,
                                         StoreOutletName = so.OutletName,
                                         CustomerId = c.CustomerId,
                                         ReferredByCustomerId = c.ReferredByCustomerId,
                                         StoreCustomerTypeId = ct.StoreCustomerTypeId,
                                         StoreOutletId = so.StoreOutletId,
                                         Email = p.ContactEmail,
                                         MobileNumber = p.MobileNumber,
                                         OfficeLine = p.OfficeLine,
                                         Birthday = p.Birthday,
                                         UserId = p.Id,

                                     }).ToList();

                   if (!searchList.Any())
                   {
                       return new List<CustomerObject>();
                   }

                   searchList.ForEach(m =>
                   {
                       m.BirthDayStr = m.Birthday != null ? ((DateTime)m.Birthday).ToString("dd/MM/yyyy") : "";
                   });

                   return searchList;
               }
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<CustomerObject>();
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

       public int GetObjectCount(Expression<Func<Customer, bool>> predicate)
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
       public List<CustomerObject> GetCustomers()
       {
           try
           {
               var customerEntityList = _repository.GetAll().ToList();
               if (!customerEntityList.Any())
               {
                   return new List<CustomerObject>();
               }
               var customerObjList = new List<CustomerObject>();
               customerEntityList.ForEach(m =>
               {
                   var customerObject = ModelCrossMapper.Map<Customer, CustomerObject>(m);
                   if (customerObject != null && customerObject.CustomerId > 0)
                   {
                       customerObjList.Add(customerObject);
                   }
               });
               return customerObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
       
    }
}
