﻿using System;
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
    public class BankRepository
    {
       private readonly IShopkeeperRepository<Bank> _repository;
       private readonly UnitOfWork _uoWork;

        public BankRepository()
        {
            var conn = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperStoreContext = new ShopKeeperMasterEntities(conn);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<Bank>(_uoWork);
		}
       
        public long AddBank(BankObject bank)
        {
            try
            {
                if (bank == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.FullName.Trim().ToLower() == bank.FullName.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                if (_repository.Count(m => m.SortCode.Trim().ToLower() == bank.SortCode.Trim().ToLower()) > 0)
                {
                    return -4;
                }
                var bankEntity = ModelCrossMapper.Map<BankObject, Bank>(bank);
                if (bankEntity == null || string.IsNullOrEmpty(bankEntity.FullName))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(bankEntity);
                _uoWork.SaveChanges();
                return returnStatus.BankId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateBank(BankObject bank)
        {
            try
            {
                if (bank == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.FullName.Trim().ToLower() == bank.FullName.Trim().ToLower() && (m.BankId != bank.BankId)) > 0)
                {
                    return -3;
                }
                if (_repository.Count(m => m.SortCode.Trim().ToLower() == bank.SortCode.Trim().ToLower() && (m.BankId != bank.BankId)) > 0)
                {
                    return -4;
                }
                var bankEntity = ModelCrossMapper.Map<BankObject, Bank>(bank);
                if (bankEntity == null || bankEntity.BankId < 1)
                {
                    return -2;
                }
                _repository.Update(bankEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteBank(long bankId)
        {
            try
            {
                var returnStatus = _repository.Remove(bankId);
                _uoWork.SaveChanges();
                return returnStatus.BankId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public BankObject GetBank(long bankId)
        {
            try
            {
                var myItem = _repository.GetById(bankId);
                if (myItem == null || myItem.BankId < 1)
                {
                    return new BankObject();
                }
                var bankObject = ModelCrossMapper.Map<Bank, BankObject>(myItem);
                if (bankObject == null || bankObject.BankId < 1)
                {
                    return new BankObject();
                }
                return bankObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BankObject();
            }
        }

        public List<BankObject> GetBankObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<Bank> bankEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        bankEntityList = _repository.GetWithPaging(m => m.BankId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        bankEntityList = _repository.GetAll().ToList();
                    }

                    if (!bankEntityList.Any())
                    {
                        return new List<BankObject>();
                    }
                    var bankObjList = new List<BankObject>();
                    bankEntityList.ForEach(m =>
                    {
                        var bankObject = ModelCrossMapper.Map<Bank, BankObject>(m);
                        if (bankObject != null && bankObject.BankId > 0)
                        {
                            bankObjList.Add(bankObject);
                        }
                    });

                return bankObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankObject>();
            }
        }

        public List<BankObject> Search(string searchCriteria)
        {
            try
            {
                var bankEntityList = _repository.GetAll(m => m.FullName.ToLower().Contains(searchCriteria.ToLower()) || m.ShortName.ToLower().Contains(searchCriteria.ToLower()) || m.SortCode.Contains(searchCriteria)).ToList();

                if (!bankEntityList.Any())
                {
                    return new List<BankObject>();
                }
                var bankObjList = new List<BankObject>();
                bankEntityList.ForEach(m =>
                {
                    var bankObject = ModelCrossMapper.Map<Bank, BankObject>(m);
                    if (bankObject != null && bankObject.BankId > 0)
                    {
                        bankObjList.Add(bankObject);
                    }
                });
                return bankObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BankObject>();
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

        public List<BankObject> GetBanks()
        {
            try
            {
                var bankEntityList = _repository.GetAll().ToList();
                if (!bankEntityList.Any())
                {
                    return new List<BankObject>();
                }
                var bankObjList = new List<BankObject>();
                bankEntityList.ForEach(m =>
                {
                    var bankObject = ModelCrossMapper.Map<Bank, BankObject>(m);
                    if (bankObject != null && bankObject.BankId > 0)
                    {
                        bankObjList.Add(bankObject);
                    }
                });
                return bankObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}