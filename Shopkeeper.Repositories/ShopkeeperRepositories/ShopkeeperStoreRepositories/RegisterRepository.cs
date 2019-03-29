using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class RegisterRepository
    {
       private readonly IShopkeeperRepository<Register> _repository;
       private readonly UnitOfWork _uoWork;

       public RegisterRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString); 
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<Register>(_uoWork);
		}
       
        public long AddRegister(RegisterObject register)
        {
            try
            {
                if (register == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(register.Name.Trim().ToLower()) && register.CurrentOutletId.Equals(m.CurrentOutletId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var registerEntity = ModelCrossMapper.Map<RegisterObject, Register>(register);
                if (registerEntity == null || string.IsNullOrEmpty(registerEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(registerEntity);
                _uoWork.SaveChanges();
                return returnStatus.RegisterId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateRegister(RegisterObject register)
        {
            try
            {
                if (register == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(register.Name.Trim().ToLower()) && register.CurrentOutletId.Equals(m.CurrentOutletId) && (m.RegisterId != register.RegisterId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var registerEntity = ModelCrossMapper.Map<RegisterObject, Register>(register);
                if (registerEntity == null || registerEntity.RegisterId < 1)
                {
                    return -2;
                }
                _repository.Update(registerEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteRegister(long registerId)
        {
            try
            {
                var returnStatus = _repository.Remove(registerId);
                _uoWork.SaveChanges();
                return returnStatus.RegisterId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public RegisterObject GetRegister(long registerId)
        {
            try
            {
                var myItem = _repository.Get(m => m.RegisterId == registerId, "StoreOutlet");
                if (myItem == null || myItem.RegisterId < 1)
                {
                    return new RegisterObject();
                }
                var registerObject = ModelCrossMapper.Map<Register, RegisterObject>(myItem);
                if (registerObject == null || registerObject.RegisterId < 1)
                {
                    return new RegisterObject();
                }
                return registerObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new RegisterObject();
            }
        }

        public List<RegisterObject> GetRegisterObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<Register> registerEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    registerEntityList = _repository.GetWithPaging(m => m.RegisterId, tpageNumber, tsize, "StoreOutlet").ToList();
                }

                else
                {
                    registerEntityList = _repository.GetAll("StoreOutlet").ToList();
                }

                if (!registerEntityList.Any())
                {
                    return new List<RegisterObject>();
                }
                var registerObjList = new List<RegisterObject>();
                registerEntityList.ForEach(m =>
                {
                    var registerObject = ModelCrossMapper.Map<Register, RegisterObject>(m);
                    if (registerObject != null && registerObject.RegisterId > 0)
                    {
                        registerObject.OutletName = m.StoreOutlet.OutletName;
                        registerObjList.Add(registerObject);
                    }
                });

                return registerObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<RegisterObject>();
            }
        }

        public List<RegisterObject> Search(string searchCriteria)
        {
            try
            {
               var registerEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()), "StoreOutlet").ToList();

                if (!registerEntityList.Any())
                {
                    return new List<RegisterObject>();
                }
                var registerObjList = new List<RegisterObject>();
                registerEntityList.ForEach(m =>
                {
                    var registerObject = ModelCrossMapper.Map<Register, RegisterObject>(m);
                    if (registerObject != null && registerObject.RegisterId > 0)
                    {
                        registerObject.OutletName = m.StoreOutlet.OutletName;
                        registerObjList.Add(registerObject);
                    }
                });
                return registerObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<RegisterObject>();
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

        public int GetObjectCount(Expression<Func<Register, bool>> predicate)
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
        public List<RegisterObject> GetRegisters()
        {
            try
            {
                var registerEntityList = _repository.GetAll().ToList();
                if (!registerEntityList.Any())
                {
                    return new List<RegisterObject>();
                }
                var registerObjList = new List<RegisterObject>();
                registerEntityList.ForEach(m =>
                {
                    var registerObject = ModelCrossMapper.Map<Register, RegisterObject>(m);
                    if (registerObject != null && registerObject.RegisterId > 0)
                    {
                        registerObjList.Add(registerObject);
                    }
                });
                return registerObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
