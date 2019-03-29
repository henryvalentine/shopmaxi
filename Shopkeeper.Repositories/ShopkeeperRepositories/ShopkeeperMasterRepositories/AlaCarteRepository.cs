using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class AlaCarteRepository
    {
        private readonly IShopkeeperRepository<AlaCarte> _repository;
       private readonly UnitOfWork _uoWork;

       public AlaCarteRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<AlaCarte>(_uoWork);
		}
       
        public long AddAlaCarte(AlaCarteObject alaCarte)
        {
            try
            {
                if (alaCarte == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => String.Equals(m.ItemName.Trim(), alaCarte.ItemName.Trim(), StringComparison.CurrentCultureIgnoreCase));
                if (duplicates > 0)
                {
                    return -3;
                }
                var alaCarteEntity = ModelCrossMapper.Map<AlaCarteObject, AlaCarte>(alaCarte);
                if (alaCarteEntity == null || string.IsNullOrEmpty(alaCarteEntity.ItemName))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(alaCarteEntity);
                _uoWork.SaveChanges();
                return returnStatus.AlaCarteId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateAlaCarte(AlaCarteObject alaCarte)
        {
            try
            {
                if (alaCarte == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => String.Equals(m.ItemName.Trim(), alaCarte.ItemName.Trim(), StringComparison.CurrentCultureIgnoreCase) && (m.AlaCarteId != alaCarte.AlaCarteId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var alaCarteEntity = ModelCrossMapper.Map<AlaCarteObject, AlaCarte>(alaCarte);
                if (alaCarteEntity == null || alaCarteEntity.AlaCarteId < 1)
                {
                    return -2;
                }
                _repository.Update(alaCarteEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteAlaCarte(long alaCarteId)
        {
            try
            {
                var returnStatus = _repository.Remove(alaCarteId);
                _uoWork.SaveChanges();
                return returnStatus.AlaCarteId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public AlaCarteObject GetAlaCarte(long alaCarteId)
        {
            try
            {
                var myItem = _repository.GetById(alaCarteId);
                if (myItem == null || myItem.AlaCarteId < 1)
                {
                    return new AlaCarteObject();
                }
                var alaCarteObject = ModelCrossMapper.Map<AlaCarte, AlaCarteObject>(myItem);
                if (alaCarteObject == null || alaCarteObject.AlaCarteId < 1)
                {
                    return new AlaCarteObject();
                }
                return alaCarteObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new AlaCarteObject();
            }
        }

        public List<AlaCarteObject> GetAlaCarteObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<AlaCarte> alaCarteEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    alaCarteEntityList = _repository.GetWithPaging(m => m.AlaCarteId, tpageNumber, tsize).ToList();
                }

                else
                {
                    alaCarteEntityList = _repository.GetAll().ToList();
                }

                if (!alaCarteEntityList.Any())
                {
                    return new List<AlaCarteObject>();
                }
                var alaCarteObjList = new List<AlaCarteObject>();
                alaCarteEntityList.ForEach(m =>
                {
                    var alaCarteObject = ModelCrossMapper.Map<AlaCarte, AlaCarteObject>(m);
                    if (alaCarteObject != null && alaCarteObject.AlaCarteId > 0)
                    {
                        alaCarteObjList.Add(alaCarteObject);
                    }
                });

                return alaCarteObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AlaCarteObject>();
            }
        }

        public List<AlaCarteObject> Search(string searchCriteria)
        {
            try
            {
               var alaCarteEntityList = _repository.GetAll(m => m.ItemName.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!alaCarteEntityList.Any())
                {
                    return new List<AlaCarteObject>();
                }
                var alaCarteObjList = new List<AlaCarteObject>();
                alaCarteEntityList.ForEach(m =>
                {
                    var alaCarteObject = ModelCrossMapper.Map<AlaCarte, AlaCarteObject>(m);
                    if (alaCarteObject != null && alaCarteObject.AlaCarteId > 0)
                    {
                        alaCarteObjList.Add(alaCarteObject);
                    }
                });
                return alaCarteObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AlaCarteObject>();
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

        public int GetObjectCount(Expression<Func<AlaCarte, bool>> predicate)
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

        public List<AlaCarteObject> GetAlaCartes()
        {
            try
            {
                var alaCarteEntityList = _repository.GetAll().ToList();
                if (!alaCarteEntityList.Any())
                {
                    return new List<AlaCarteObject>();
                }
                var alaCarteObjList = new List<AlaCarteObject>();
                alaCarteEntityList.ForEach(m =>
                {
                    var alaCarteObject = ModelCrossMapper.Map<AlaCarte, AlaCarteObject>(m);
                    if (alaCarteObject != null && alaCarteObject.AlaCarteId > 0)
                    {
                        alaCarteObjList.Add(alaCarteObject);
                    }
                });
                return alaCarteObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
