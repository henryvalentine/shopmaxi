using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class StoreStoreBankAccountRepository
    {
       private readonly IShopkeeperRepository<StoreBankAccount> _repository;
       private readonly UnitOfWork _uoWork;

        public StoreStoreBankAccountRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<StoreBankAccount>(_uoWork);
		}
       
        public long AddStoreBankAccount(StoreBankAccountObject bankAccount)
        {
            try
            {
                if (bankAccount == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.StoreId == bankAccount.StoreId && m.BankId == bankAccount.BankId && m.AccountName == bankAccount.AccountName && m.AccountNo == bankAccount.AccountNo) > 0)
                {
                    return -3;
                }
               
                var bankAccountEntity = ModelCrossMapper.Map<StoreBankAccountObject, StoreBankAccount>(bankAccount);
                if (bankAccountEntity == null || bankAccountEntity.BankId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(bankAccountEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreBankAccountId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreBankAccount(StoreBankAccountObject bankAccount)
        {
            try
            {
                if (bankAccount == null)
                {
                    return -2;
                }

                if (_repository.Count(m => m.StoreId == bankAccount.StoreId && m.BankId == bankAccount.BankId && m.AccountName == bankAccount.AccountName && m.AccountNo == bankAccount.AccountNo && (m.StoreBankAccountId != bankAccount.StoreBankAccountId)) > 0)
                {
                    return -3;
                }
               
                var bankAccountEntity = ModelCrossMapper.Map<StoreBankAccountObject, StoreBankAccount>(bankAccount);
                if (bankAccountEntity == null || bankAccountEntity.StoreBankAccountId < 1)
                {
                    return -2;
                }
                _repository.Update(bankAccountEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreBankAccount(long bankAccountId)
        {
            try
            {
                var returnStatus = _repository.Remove(bankAccountId);
                _uoWork.SaveChanges();
                return returnStatus.StoreBankAccountId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreBankAccountObject GetStoreBankAccount(long bankAccountId)
        {
            try
            {
                var myItem = _repository.Get(m => m.StoreBankAccountId == bankAccountId, "Bank,Store");
                if (myItem == null || myItem.StoreBankAccountId < 1)
                {
                    return new StoreBankAccountObject();
                }
                var bankAccountObject = ModelCrossMapper.Map<StoreBankAccount, StoreBankAccountObject>(myItem);
                if (bankAccountObject == null || bankAccountObject.StoreBankAccountId < 1)
                {
                    return new StoreBankAccountObject();
                }
                bankAccountObject.BankName = myItem.Bank.FullName;
                bankAccountObject.StoreName = myItem.Store.StoreName;
                return bankAccountObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreBankAccountObject();
            }
        }

        public List<StoreBankAccountObject> GetStoreBankAccountObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreBankAccount> bankAccountEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        bankAccountEntityList = _repository.GetWithPaging(m => m.StoreBankAccountId, tpageNumber, tsize, "Bank,Store").ToList();
                    }

                    else
                    {
                        bankAccountEntityList = _repository.GetAll("Bank,Store").ToList();
                    }

                    if (!bankAccountEntityList.Any())
                    {
                        return new List<StoreBankAccountObject>();
                    }
                    var bankAccountObjList = new List<StoreBankAccountObject>();
                    bankAccountEntityList.ForEach(m =>
                    {
                        var bankAccountObject = ModelCrossMapper.Map<StoreBankAccount, StoreBankAccountObject>(m);
                        if (bankAccountObject != null && bankAccountObject.StoreBankAccountId > 0)
                        {
                            bankAccountObject.BankName = m.Bank.FullName;
                            bankAccountObject.StoreName = m.Store.StoreName;
                            bankAccountObjList.Add(bankAccountObject);
                        }
                    });

                return bankAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
            }
        }

        public List<StoreBankAccountObject> Search(string searchCriteria)
        {
            try
            {
                long accountNo;
                var res = long.TryParse(searchCriteria, out accountNo);

                var bankAccountEntityList = res && accountNo > 0 ? _repository.GetAll(m => m.AccountName.ToLower().Contains(searchCriteria.ToLower()) || m.AccountNo.Equals(accountNo), "Bank,Store").ToList() : _repository.GetAll(m => m.AccountName.ToLower().Contains(searchCriteria.ToLower()), "Bank,Store").ToList();

                if (!bankAccountEntityList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                var bankAccountObjList = new List<StoreBankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<StoreBankAccount, StoreBankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.StoreBankAccountId > 0)
                    {
                        bankAccountObject.BankName = m.Bank.FullName;
                        bankAccountObject.StoreName = m.Store.StoreName;
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });
                return bankAccountObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
            }
        }

        public List<StoreBankAccountObject> GetStoreBankAccountsByBank(int? itemsPerPage, int? pageNumber, long bankId)
        {
            try
            {
                List<StoreBankAccount> bankAccountEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    bankAccountEntityList = _repository.GetWithPaging(m => m.BankId == bankId, m => m.StoreBankAccountId, tpageNumber, tsize, "Bank,Store").ToList();
                }

                else
                {
                    bankAccountEntityList = _repository.GetAll(m => m.BankId == bankId, "Bank,Store").ToList();
                }

                if (!bankAccountEntityList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                var bankAccountObjList = new List<StoreBankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<StoreBankAccount, StoreBankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.StoreBankAccountId > 0)
                    {
                        bankAccountObject.BankName = m.Bank.FullName;
                        bankAccountObject.StoreName = m.Store.StoreName;
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });

                return bankAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
            }
        }

        public List<StoreBankAccountObject> GetStoreBankAccountsByStore(int? itemsPerPage, int? pageNumber, long storeId)
        {
            try
            {
                List<StoreBankAccount> bankAccountEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    bankAccountEntityList = _repository.GetWithPaging(m => m.StoreId == storeId, m => m.StoreBankAccountId, tpageNumber, tsize, "Bank,Store").ToList();
                }

                else
                {
                    bankAccountEntityList = _repository.GetAll(m => m.StoreId == storeId, "Bank,Store").ToList();
                }

                if (!bankAccountEntityList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                var bankAccountObjList = new List<StoreBankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<StoreBankAccount, StoreBankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.StoreBankAccountId > 0)
                    {
                        bankAccountObject.BankName = m.Bank.FullName;
                        bankAccountObject.StoreName = m.Store.StoreName;
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });

                return bankAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreBankAccountObject>();
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

        public List<StoreBankAccountObject> GetStoreBankAccounts()
        {
            try
            {
                var bankAccountEntityList = _repository.GetAll().ToList();
                if (!bankAccountEntityList.Any())
                {
                    return new List<StoreBankAccountObject>();
                }
                var bankAccountObjList = new List<StoreBankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<StoreBankAccount, StoreBankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.StoreBankAccountId > 0)
                    {
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });
                return bankAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
