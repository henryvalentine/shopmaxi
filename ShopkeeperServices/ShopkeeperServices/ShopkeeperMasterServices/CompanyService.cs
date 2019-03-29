using System;
using System.Collections.Generic;
using System.Linq;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using Shopkeeper.Repositories.Utilities;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class CompanyServices
	{
		private readonly CompanyRepository  _companyAccountRepository;
        public CompanyServices()
		{
            _companyAccountRepository = new CompanyRepository();
		}

        public long AddCompany(CompanyObject companyAccount)
		{
			try
			{
                return _companyAccountRepository.AddCompany(companyAccount);
			}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _companyAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateCompany(CompanyObject companyAccount)
		{
			try
			{
                return _companyAccountRepository.UpdateCompany(companyAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteCompany(long companyAccountId)
		{
			try
			{
                return _companyAccountRepository.DeleteCompany(companyAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public CompanyObject GetCompany(long companyAccountId)
		{
			try
			{
                return _companyAccountRepository.GetCompany(companyAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new CompanyObject();
			}
		}

        public List<CompanyObject> GetCompanys()
		{
			try
			{
                var objList = _companyAccountRepository.GetCompanys();
                if (objList == null || !objList.Any())
			    {
                    return new List<CompanyObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CompanyObject>();
			}
		}

        public List<CompanyObject> GetCompanyObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _companyAccountRepository.GetCompanyObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<CompanyObject>();
                }
                return objList;
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
                var objList = _companyAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<CompanyObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CompanyObject>();
            }
        }
	}

}
