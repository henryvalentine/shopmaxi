using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class CompanyRepository
    {
       private readonly IShopkeeperRepository<Company> _repository;
       private readonly UnitOfWork _uoWork;

       public CompanyRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<Company>(_uoWork);
		}
       
        public long AddCompany(CompanyObject company)
        {
            try
            {
                if (company == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == company.Name.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var companyEntity = ModelCrossMapper.Map<CompanyObject, Company>(company);
                if (companyEntity == null || string.IsNullOrEmpty(companyEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(companyEntity);
                _uoWork.SaveChanges();
                return returnStatus.CompanyId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateCompany(CompanyObject company)
        {
            try
            {
                if (company == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == company.Name.Trim().ToLower() && (m.CompanyId != company.CompanyId)) > 0)
                {
                    return -3;
                }
                
                var companyEntity = ModelCrossMapper.Map<CompanyObject, Company>(company);
                if (companyEntity == null || companyEntity.CompanyId < 1)
                {
                    return -2;
                }
                _repository.Update(companyEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteCompany(long companyId)
        {
            try
            {
                var returnStatus = _repository.Remove(companyId);
                _uoWork.SaveChanges();
                return returnStatus.CompanyId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public CompanyObject GetCompany(long companyId)
        {
            try
            {
                var myItem = _repository.GetById(companyId);
                if (myItem == null || myItem.CompanyId < 1)
                {
                    return new CompanyObject();
                }
                var companyObject = ModelCrossMapper.Map<Company, CompanyObject>(myItem);
                if (companyObject == null || companyObject.CompanyId < 1)
                {
                    return new CompanyObject();
                }
                return companyObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new CompanyObject();
            }
        }

        public List<CompanyObject> GetCompanyObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<Company> companyEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        companyEntityList = _repository.GetWithPaging(m => m.CompanyId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        companyEntityList = _repository.GetAll().ToList();
                    }

                    if (!companyEntityList.Any())
                    {
                        return new List<CompanyObject>();
                    }
                    var companyObjList = new List<CompanyObject>();
                    companyEntityList.ForEach(m =>
                    {
                        var companyObject = ModelCrossMapper.Map<Company, CompanyObject>(m);
                        if (companyObject != null && companyObject.CompanyId > 0)
                        {
                            companyObjList.Add(companyObject);
                        }
                    });

                return companyObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CompanyObject>();
            }
        }

        public List<CompanyObject> Search(string searchCriteria)
        {
            try
            {
                var companyEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!companyEntityList.Any())
                {
                    return new List<CompanyObject>();
                }
                var companyObjList = new List<CompanyObject>();
                companyEntityList.ForEach(m =>
                {
                    var companyObject = ModelCrossMapper.Map<Company, CompanyObject>(m);
                    if (companyObject != null && companyObject.CompanyId > 0)
                    {
                        companyObjList.Add(companyObject);
                    }
                });
                return companyObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CompanyObject>();
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
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<CompanyObject> GetCompanys()
        {
            try
            {
                var companyEntityList = _repository.GetAll().ToList();
                if (!companyEntityList.Any())
                {
                    return new List<CompanyObject>();
                }
                var companyObjList = new List<CompanyObject>();
                companyEntityList.ForEach(m =>
                {
                    var companyObject = ModelCrossMapper.Map<Company, CompanyObject>(m);
                    if (companyObject != null && companyObject.CompanyId > 0)
                    {
                        companyObjList.Add(companyObject);
                    }
                });
                return companyObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
