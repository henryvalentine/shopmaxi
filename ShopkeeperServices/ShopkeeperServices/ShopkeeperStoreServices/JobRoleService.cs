using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class JobRoleServices
	{
		private readonly JobRoleRepository  _jobRoleRepository;
        public JobRoleServices()
		{
            _jobRoleRepository = new JobRoleRepository();
		}

        public long AddJobRole(JobRoleObject jobRole)
		{
			try
			{
                return _jobRoleRepository.AddJobRole(jobRole);
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
                return _jobRoleRepository.UpdateJobRole(jobRole);
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
                return _jobRoleRepository.DeleteJobRole(jobRoleId);
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
                return _jobRoleRepository.GetJobRole(jobRoleId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new JobRoleObject();
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _jobRoleRepository.GetObjectCount();
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
                var objList = _jobRoleRepository.GetJobRoles();
                if (objList == null || !objList.Any())
			    {
                    return new List<JobRoleObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<JobRoleObject>();
			}
		}

        public List<JobRoleObject> GetJobRoleObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _jobRoleRepository.GetJobRoleObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<JobRoleObject>();
                }
                return objList;
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
                var objList = _jobRoleRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<JobRoleObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<JobRoleObject>();
            }
        }
	}

}
