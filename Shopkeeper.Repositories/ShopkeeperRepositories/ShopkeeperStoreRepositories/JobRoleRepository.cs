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
    public class JobRoleRepository
    {
       private readonly IShopkeeperRepository<JobRole> _repository;
       private readonly UnitOfWork _uoWork;

       public JobRoleRepository()
       {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<JobRole>(_uoWork);
	   }
       
        public long AddJobRole(JobRoleObject jobRole)
        {
            try
            {
                if (jobRole == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.JobTitle.Trim().ToLower() == jobRole.JobTitle.Trim().ToLower() && (m.JobRoleId != jobRole.JobRoleId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var jobRoleEntity = ModelCrossMapper.Map<JobRoleObject, JobRole>(jobRole);
                if (jobRoleEntity == null || string.IsNullOrEmpty(jobRoleEntity.JobTitle))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(jobRoleEntity);
                _uoWork.SaveChanges();
                return returnStatus.JobRoleId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateJobRole(JobRoleObject jobRole)
        {
            try
            {
                if (jobRole == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.JobTitle.Trim().ToLower() == jobRole.JobTitle.Trim().ToLower() && (m.JobRoleId != jobRole.JobRoleId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var jobRoleEntity = ModelCrossMapper.Map<JobRoleObject, JobRole>(jobRole);
                if (jobRoleEntity == null || jobRoleEntity.JobRoleId < 1)
                {
                    return -2;
                }
                _repository.Update(jobRoleEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteJobRole(long jobRoleId)
        {
            try
            {
                var returnStatus = _repository.Remove(jobRoleId);
                _uoWork.SaveChanges();
                return returnStatus.JobRoleId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public JobRoleObject GetJobRole(long jobRoleId)
        {
            try
            {
                var myItem = _repository.GetById(jobRoleId);
                if (myItem == null || myItem.JobRoleId < 1)
                {
                    return new JobRoleObject();
                }
                var jobRoleObject = ModelCrossMapper.Map<JobRole, JobRoleObject>(myItem);
                if (jobRoleObject == null || jobRoleObject.JobRoleId < 1)
                {
                    return new JobRoleObject();
                }
                return jobRoleObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new JobRoleObject();
            }
        }

        public List<JobRoleObject> GetJobRoleObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<JobRole> jobRoleEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    jobRoleEntityList = _repository.GetWithPaging(m => m.JobRoleId, tpageNumber, tsize).ToList();
                }

                else
                {
                    jobRoleEntityList = _repository.GetAll().ToList();
                }

                if (!jobRoleEntityList.Any())
                {
                    return new List<JobRoleObject>();
                }
                var jobRoleObjList = new List<JobRoleObject>();
                jobRoleEntityList.ForEach(m =>
                {
                    var jobRoleObject = ModelCrossMapper.Map<JobRole, JobRoleObject>(m);
                    if (jobRoleObject != null && jobRoleObject.JobRoleId > 0)
                    {
                        jobRoleObjList.Add(jobRoleObject);
                    }
                });

                return jobRoleObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<JobRoleObject>();
            }
        }

        public List<JobRoleObject> Search(string searchCriteria)
        {
            try
            {
               var jobRoleEntityList = _repository.GetAll(m => m.JobTitle.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!jobRoleEntityList.Any())
                {
                    return new List<JobRoleObject>();
                }
                var jobRoleObjList = new List<JobRoleObject>();
                jobRoleEntityList.ForEach(m =>
                {
                    var jobRoleObject = ModelCrossMapper.Map<JobRole, JobRoleObject>(m);
                    if (jobRoleObject != null && jobRoleObject.JobRoleId > 0)
                    {
                        jobRoleObjList.Add(jobRoleObject);
                    }
                });
                return jobRoleObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<JobRoleObject>();
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

        public int GetObjectCount(Expression<Func<JobRole, bool>> predicate)
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

        public List<JobRoleObject> GetJobRoles()
        {
            try
            {
                var jobRoleEntityList = _repository.GetAll().ToList();
                if (!jobRoleEntityList.Any())
                {
                    return new List<JobRoleObject>();
                }
                var jobRoleObjList = new List<JobRoleObject>();
                jobRoleEntityList.ForEach(m =>
                {
                    var jobRoleObject = ModelCrossMapper.Map<JobRole, JobRoleObject>(m);
                    if (jobRoleObject != null && jobRoleObject.JobRoleId > 0)
                    {
                        jobRoleObjList.Add(jobRoleObject);
                    }
                });
                return jobRoleObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
