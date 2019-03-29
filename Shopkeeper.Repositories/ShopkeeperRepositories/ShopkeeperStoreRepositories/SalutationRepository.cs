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
    public class SalutationRepository
    {
        private readonly IShopkeeperRepository<Salutation> _repository;
       private readonly UnitOfWork _uoWork;

       public SalutationRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<Salutation>(_uoWork);
		}
       
        public long AddSalutation(SalutationObject salutation)
        {
            try
            {
                if (salutation == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == salutation.Name.Trim().ToLower() && (m.SalutationId != salutation.SalutationId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var salutationEntity = ModelCrossMapper.Map<SalutationObject, Salutation>(salutation);
                if (salutationEntity == null || string.IsNullOrEmpty(salutationEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(salutationEntity);
                _uoWork.SaveChanges();
                return returnStatus.SalutationId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSalutation(SalutationObject salutation)
        {
            try
            {
                if (salutation == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == salutation.Name.Trim().ToLower() && (m.SalutationId != salutation.SalutationId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var salutationEntity = ModelCrossMapper.Map<SalutationObject, Salutation>(salutation);
                if (salutationEntity == null || salutationEntity.SalutationId < 1)
                {
                    return -2;
                }
                _repository.Update(salutationEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSalutation(long salutationId)
        {
            try
            {
                var returnStatus = _repository.Remove(salutationId);
                _uoWork.SaveChanges();
                return returnStatus.SalutationId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public SalutationObject GetSalutation(long salutationId)
        {
            try
            {
                var myItem = _repository.GetById(salutationId);
                if (myItem == null || myItem.SalutationId < 1)
                {
                    return new SalutationObject();
                }
                var salutationObject = ModelCrossMapper.Map<Salutation, SalutationObject>(myItem);
                if (salutationObject == null || salutationObject.SalutationId < 1)
                {
                    return new SalutationObject();
                }
                return salutationObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SalutationObject();
            }
        }

        public List<SalutationObject> GetSalutationObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<Salutation> salutationEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    salutationEntityList = _repository.GetWithPaging(m => m.SalutationId, tpageNumber, tsize).ToList();
                }

                else
                {
                    salutationEntityList = _repository.GetAll().ToList();
                }

                if (!salutationEntityList.Any())
                {
                    return new List<SalutationObject>();
                }
                var salutationObjList = new List<SalutationObject>();
                salutationEntityList.ForEach(m =>
                {
                    var salutationObject = ModelCrossMapper.Map<Salutation, SalutationObject>(m);
                    if (salutationObject != null && salutationObject.SalutationId > 0)
                    {
                        salutationObjList.Add(salutationObject);
                    }
                });

                return salutationObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalutationObject>();
            }
        }

        public List<SalutationObject> Search(string searchCriteria)
        {
            try
            {
               var salutationEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!salutationEntityList.Any())
                {
                    return new List<SalutationObject>();
                }
                var salutationObjList = new List<SalutationObject>();
                salutationEntityList.ForEach(m =>
                {
                    var salutationObject = ModelCrossMapper.Map<Salutation, SalutationObject>(m);
                    if (salutationObject != null && salutationObject.SalutationId > 0)
                    {
                        salutationObjList.Add(salutationObject);
                    }
                });
                return salutationObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalutationObject>();
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

        public int GetObjectCount(Expression<Func<Salutation, bool>> predicate)
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

        public List<SalutationObject> GetSalutations()
        {
            try
            {
                var salutationEntityList = _repository.GetAll().ToList();
                if (!salutationEntityList.Any())
                {
                    return new List<SalutationObject>();
                }
                var salutationObjList = new List<SalutationObject>();
                salutationEntityList.ForEach(m =>
                {
                    var salutationObject = ModelCrossMapper.Map<Salutation, SalutationObject>(m);
                    if (salutationObject != null && salutationObject.SalutationId > 0)
                    {
                        salutationObjList.Add(salutationObject);
                    }
                });
                return salutationObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
