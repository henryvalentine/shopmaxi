using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
    public class CustomerServices
    {
        private readonly CustomerRepository _customerRepository;
        public CustomerServices()
        {
            _customerRepository = new CustomerRepository();
        }

        public long AddCustomer(UserProfileObject customer)
        {
            return _customerRepository.AddCustomer(customer);
        }

        public long AddCustomer(CustomerObject customer)
        {
            return _customerRepository.AddCustomer(customer);
        }

        public int GetObjectCount()
        {
            return _customerRepository.GetObjectCount();
        }

        public int UpdateCustomer(UserProfileObject customer)
        {
            return _customerRepository.UpdateCustomer(customer);
        }

        public bool DeleteCustomer(long customerId)
        {
            return _customerRepository.DeleteCustomer(customerId);
        }

        public UserProfileObject GetCustomer(long customerId)
        {
            return _customerRepository.GetCustomer(customerId);
        }

        public List<CustomerObject> GetCustomers()
        {
            return _customerRepository.GetCustomers();
        }

        public List<CustomerObject> GetCustomerObjects(int? itemsPerPage, int? pageNumber)
        {
            return _customerRepository.GetCustomerObjects(itemsPerPage, pageNumber);
        }

        public List<CustomerObject> Search(string searchCriteria)
        {
            return _customerRepository.Search(searchCriteria);
        }

        public List<CustomerObject> SearchCustomer(string searchCriteria)
        {
            return _customerRepository.SearchCustomer(searchCriteria);
        }
    }

}
