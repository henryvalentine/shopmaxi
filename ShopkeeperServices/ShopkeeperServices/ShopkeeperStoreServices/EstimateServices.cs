using System.Collections.Generic;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class EstimateServices
	{
        private readonly EstimateRepository _estimateRepository;  
        public EstimateServices()
		{
            _estimateRepository = new EstimateRepository();
		}

        public long AddEstimate(EstimateObject estimate, out string estimateNumber, out long customerId)
		{
            return _estimateRepository.AddEstimate(estimate, out estimateNumber, out customerId);
		}

        public long DeleteEstimateItem(long orderItemId)
        {
            return _estimateRepository.DeleteEstimateItem(orderItemId);
        }

        public long DeleteEstimate(long estimateId)
        {
            return _estimateRepository.DeleteEstimate(estimateId);
        }

        public long UpdateEstimate(EstimateObject estimate)
		{
			return _estimateRepository.UpdateEstimate(estimate);
		}

        public long ConvertEstimateToInvoice(string estimateNumber)
        {
            return _estimateRepository.ConvertEstimateToInvoice(estimateNumber);
        }
        
        public EstimateObject GetEstimate(long estimateId)
        {
            return _estimateRepository.GetEstimate(estimateId);
        }

        public SaleObject GetSalesDetails(string estimateNumber)
        {
            return _estimateRepository.GetSalesDetails(estimateNumber);
        }

        public EstimateObject GetEstimateByRef(string refNumber)  
        {
            return _estimateRepository.GetEstimateByRef(refNumber);
        }
    
        public EstimateObject GetEstimateDetails(long estimateId)
        {
            return _estimateRepository.GetEstimate(estimateId);
        }

        public List<EstimateObject> GetEstimateObjects(int? itemsPerPage, int? pageNumber, out int count)
        {
            return _estimateRepository.GetEstimates(itemsPerPage, pageNumber, out count);
        }
        public List<EstimateObject> GetEstimatesByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId)
        {
            return _estimateRepository.GetEstimatesByOutlet(itemsPerPage, pageNumber, out count, outletId);
        }

        public List<EstimateObject> GetEstimatesByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId)
        {
            return _estimateRepository.GetEstimatesByEmployee(itemsPerPage, pageNumber, out count, employeeId);
        }

        public List<EstimateObject> SearchEstimates(string searchCriteria)
        {
            return _estimateRepository.SearchEstimates(searchCriteria);
        }

        public List<EstimateObject> SearchOutletEstimate(string searchCriteria, int outletId)
        {
            return _estimateRepository.SearchOutletEstimate(searchCriteria, outletId);
        }

        public List<EstimateObject> SearchEmployeeEstimate(string searchCriteria, long employeeId)
        {
            return _estimateRepository.SearchEmployeeEstimate(searchCriteria, employeeId);
        }

	}
    
}

