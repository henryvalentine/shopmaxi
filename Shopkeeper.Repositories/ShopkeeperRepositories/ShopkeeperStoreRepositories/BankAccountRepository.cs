using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class BankAccountRepository
    {
        private readonly IShopkeeperRepository<BankAccount> _repository;
        private readonly UnitOfWork _uoWork;

        public BankAccountRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<BankAccount>(_uoWork);
        }

        public long AddBankAccount(BankAccountObject bankAccount)
        {
            try
            {
                if (bankAccount == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.CustomerId == bankAccount.CustomerId && m.BankId == bankAccount.BankId && m.AccountName == bankAccount.AccountName && m.AccountNo == bankAccount.AccountNo) > 0)
                {
                    return -3;
                }

                var bankAccountEntity = ModelCrossMapper.Map<BankAccountObject, BankAccount>(bankAccount);
                if (bankAccountEntity == null || bankAccountEntity.BankId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(bankAccountEntity);
                _uoWork.SaveChanges();
                return returnStatus.BankAccountId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateBankAccount(BankAccountObject bankAccount)
        {
            try
            {
                if (bankAccount == null)
                {
                    return -2;
                }

                if (_repository.Count(m => m.CustomerId == bankAccount.CustomerId && m.BankId == bankAccount.BankId && m.AccountName == bankAccount.AccountName && m.AccountNo == bankAccount.AccountNo && (m.BankAccountId != bankAccount.BankAccountId)) > 0)
                {
                    return -3;
                }

                var bankAccountEntity = ModelCrossMapper.Map<BankAccountObject, BankAccount>(bankAccount);
                if (bankAccountEntity == null || bankAccountEntity.BankAccountId < 1)
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

        public bool DeleteBankAccount(long bankAccountId)
        {
            try
            {
                var returnStatus = _repository.Remove(bankAccountId);
                _uoWork.SaveChanges();
                return returnStatus.BankAccountId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public BankAccountObject GetBankAccount(long bankAccountId)
        {
            try
            {
                var myItem = _repository.Get(m => m.BankAccountId == bankAccountId, "StoreBank");
                if (myItem == null || myItem.BankAccountId < 1)
                {
                    return new BankAccountObject();
                }
                var bankAccountObject = ModelCrossMapper.Map<BankAccount, BankAccountObject>(myItem);
                if (bankAccountObject == null || bankAccountObject.BankAccountId < 1)
                {
                    return new BankAccountObject();
                }
                bankAccountObject.BankName = myItem.StoreBank.FullName;
                
                return bankAccountObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BankAccountObject();
            }
        }

        public BankAccountObject GetBankAccountByCustomerId(long customerId)
        {
            try
            {
                var myItem = _repository.Get(m => m.CustomerId == customerId, "StoreBank");
                if (myItem == null || myItem.BankAccountId < 1)
                {
                    return new BankAccountObject();
                }
                var bankAccountObject = ModelCrossMapper.Map<BankAccount, BankAccountObject>(myItem);
                if (bankAccountObject == null || bankAccountObject.BankAccountId < 1)
                {
                    return new BankAccountObject();
                }
                bankAccountObject.BankName = myItem.StoreBank.FullName;
                
                return bankAccountObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BankAccountObject();
            }
        }

        public List<BankAccountObject> GetBankAccountObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<BankAccount> bankAccountEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    bankAccountEntityList = _repository.GetWithPaging(m => m.BankAccountId, tpageNumber, tsize, "StoreBank").ToList();
                }

                else
                {
                    bankAccountEntityList = _repository.GetAll("StoreBank").ToList();
                }

                if (!bankAccountEntityList.Any())
                {
                    return new List<BankAccountObject>();
                }
                var bankAccountObjList = new List<BankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<BankAccount, BankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.BankAccountId > 0)
                    {
                        bankAccountObject.BankName = m.StoreBank.FullName;
                        
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });

                return bankAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankAccountObject>();
            }
        }

        public List<BankAccountObject> Search(string searchCriteria)
        {
            try
            {
                long accountNo;
                var res = long.TryParse(searchCriteria, out accountNo);

                var bankAccountEntityList = res && accountNo > 0 ? _repository.GetAll(m => m.AccountName.ToLower().Contains(searchCriteria.ToLower()) || m.AccountNo.Equals(accountNo), "StoreBank").ToList() : _repository.GetAll(m => m.AccountName.ToLower().Contains(searchCriteria.ToLower()), "StoreBank").ToList();

                if (!bankAccountEntityList.Any())
                {
                    return new List<BankAccountObject>();
                }
                var bankAccountObjList = new List<BankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<BankAccount, BankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.BankAccountId > 0)
                    {
                        bankAccountObject.BankName = m.StoreBank.FullName;
                        
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });
                return bankAccountObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankAccountObject>();
            }
        }

        public List<BankAccountObject> GetBankAccountsByBank(int? itemsPerPage, int? pageNumber, long bankId)
        {
            try
            {
                List<BankAccount> bankAccountEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    bankAccountEntityList = _repository.GetWithPaging(m => m.BankId == bankId, m => m.BankAccountId, tpageNumber, tsize, "StoreBank").ToList();
                }

                else
                {
                    bankAccountEntityList = _repository.GetAll(m => m.BankId == bankId, "StoreBank").ToList();
                }

                if (!bankAccountEntityList.Any())
                {
                    return new List<BankAccountObject>();
                }
                var bankAccountObjList = new List<BankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<BankAccount, BankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.BankAccountId > 0)
                    {
                        bankAccountObject.BankName = m.StoreBank.FullName;
                        
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });

                return bankAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankAccountObject>();
            }
        }

        public List<BankAccountObject> GetBankAccountsByStore(int? itemsPerPage, int? pageNumber, long storeId)
        {
            try
            {
                List<BankAccount> bankAccountEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    bankAccountEntityList = _repository.GetWithPaging(m => m.CustomerId == storeId, m => m.BankAccountId, tpageNumber, tsize, "StoreBank").ToList();
                }

                else
                {
                    bankAccountEntityList = _repository.GetAll(m => m.CustomerId == storeId, "StoreBank").ToList();
                }

                if (!bankAccountEntityList.Any())
                {
                    return new List<BankAccountObject>();
                }
                var bankAccountObjList = new List<BankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<BankAccount, BankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.BankAccountId > 0)
                    {
                        bankAccountObject.BankName = m.StoreBank.FullName;
                        
                        bankAccountObjList.Add(bankAccountObject);
                    }
                });

                return bankAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankAccountObject>();
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

        public List<BankAccountObject> GetBankAccounts()
        {
            try
            {
                var bankAccountEntityList = _repository.GetAll().ToList();
                if (!bankAccountEntityList.Any())
                {
                    return new List<BankAccountObject>();
                }
                var bankAccountObjList = new List<BankAccountObject>();
                bankAccountEntityList.ForEach(m =>
                {
                    var bankAccountObject = ModelCrossMapper.Map<BankAccount, BankAccountObject>(m);
                    if (bankAccountObject != null && bankAccountObject.BankAccountId > 0)
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
