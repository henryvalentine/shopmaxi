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
    public class SupplierRepository
    {
        private readonly IShopkeeperRepository<Supplier> _repository;
       private readonly UnitOfWork _uoWork;

       public SupplierRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<Supplier>(_uoWork);
		}
       
        public long AddSupplier(SupplierObject supplier)
        {
            try
            {
                if (supplier == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => (m.CompanyName.Trim().ToLower() == supplier.CompanyName.Trim().ToLower() || m.TIN != null && supplier.TIN != null && m.TIN.Trim() == supplier.TIN.Trim()));
                if (duplicates > 0)
                {
                    return -3;
                }
                var supplierEntity = ModelCrossMapper.Map<SupplierObject, Supplier>(supplier);
                if (supplierEntity == null || string.IsNullOrEmpty(supplierEntity.CompanyName))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(supplierEntity);
                _uoWork.SaveChanges();
                return returnStatus.SupplierId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSupplier(SupplierObject supplier)
        {
            try
            {
                if (supplier == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => (m.CompanyName.Trim().ToLower() == supplier.CompanyName.Trim().ToLower() || m.TIN != null && supplier.TIN  != null && m.TIN.Trim() == supplier.TIN.Trim()) && (m.SupplierId != supplier.SupplierId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var supplierEntity = ModelCrossMapper.Map<SupplierObject, Supplier>(supplier);
                if (supplierEntity == null || supplierEntity.SupplierId < 1)
                {
                    return -2;
                }
                _repository.Update(supplierEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSupplier(long supplierId)
        {
            try
            {
                var returnStatus = _repository.Remove(supplierId);
                _uoWork.SaveChanges();
                return returnStatus.SupplierId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public SupplierObject GetSupplier(long supplierId)
        {
            try
            {
                var myItem = _repository.Get(m => m.SupplierId == supplierId, "SupplierAddresses, PurchaseOrders");
                if (myItem == null || myItem.SupplierId < 1)
                {
                    return new SupplierObject();
                }
                var supplierObject = ModelCrossMapper.Map<Supplier, SupplierObject>(myItem);
                if (supplierObject == null || supplierObject.SupplierId < 1)
                {
                    return new SupplierObject();
                }

                supplierObject.SupplierAddressObjects = new List<SupplierAddressObject>();
                supplierObject.PurchaseOrderObjects = new List<PurchaseOrderObject>();

                if (myItem.SupplierAddresses.Any())
                {
                    var addressList = myItem.SupplierAddresses.ToList();
                    addressList.ForEach(m =>
                    {
                        var address = ModelCrossMapper.Map<SupplierAddress, SupplierAddressObject>(m);
                        if (address != null && address.AddressId > 0)
                        {
                            supplierObject.SupplierAddressObjects.Add(address);
                        }
                    });
                }

                if (myItem.PurchaseOrders.Any())
                {
                    var purchaseList = myItem.PurchaseOrders.ToList();
                    purchaseList.ForEach(m =>
                    {
                        var purchaseOrder = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(m);
                        if (purchaseOrder != null && purchaseOrder.PurchaseOrderId > 0)
                        {
                            supplierObject.PurchaseOrderObjects.Add(purchaseOrder);
                        }
                    });
                }

                return supplierObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SupplierObject();
            }
        }

        public List<SupplierObject> GetSupplierObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<Supplier> supplierEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    supplierEntityList = _repository.GetWithPaging(m => m.SupplierId, tpageNumber, tsize, "SupplierAddresses, PurchaseOrders").ToList();
                }

                else
                {
                    supplierEntityList = _repository.GetAll("SupplierAddresses, PurchaseOrders").ToList();
                }

                if (!supplierEntityList.Any())
                {
                    return new List<SupplierObject>();
                }
                var supplierObjList = new List<SupplierObject>();
                supplierEntityList.ForEach(m =>
                {
                    var supplierObject = ModelCrossMapper.Map<Supplier, SupplierObject>(m);
                    if (supplierObject != null && supplierObject.SupplierId > 0)
                    {
                        supplierObject.SupplierAddressObjects = new List<SupplierAddressObject>();
                        supplierObject.PurchaseOrderObjects = new List<PurchaseOrderObject>();

                        if (m.SupplierAddresses.Any())
                        {
                            var addressList = m.SupplierAddresses.ToList();
                            addressList.ForEach(x =>
                            {
                                var address = ModelCrossMapper.Map<SupplierAddress, SupplierAddressObject>(x);
                                if (address != null && address.AddressId > 0)
                                {
                                    supplierObject.SupplierAddressObjects.Add(address);
                                }
                            });
                        }

                        if (m.PurchaseOrders.Any())
                        {
                            var purchaseList = m.PurchaseOrders.ToList();
                            purchaseList.ForEach(x =>
                            {
                                var purchaseOrder = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(x);
                                if (purchaseOrder != null && purchaseOrder.PurchaseOrderId > 0)
                                {
                                    supplierObject.PurchaseOrderObjects.Add(purchaseOrder);
                                }
                            });
                        }
                        supplierObject.DateRegistered = supplierObject.DateJoined.ToString("dd/MM/yyyy");
                        supplierObjList.Add(supplierObject);
                    }

                });

                return supplierObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SupplierObject>();
            }
        }

        public List<SupplierObject> Search(string searchCriteria)
        {
            try
            {
                var supplierEntityList = _repository.GetAll(m => m.CompanyName.ToLower().Contains(searchCriteria.ToLower()), "SupplierAddresses, PurchaseOrders").ToList();

                if (!supplierEntityList.Any())
                {
                    return new List<SupplierObject>();
                }
                var supplierObjList = new List<SupplierObject>();
                supplierEntityList.ForEach(m =>
                {
                    var supplierObject = ModelCrossMapper.Map<Supplier, SupplierObject>(m);
                    if (supplierObject != null && supplierObject.SupplierId > 0)
                    {
                        supplierObject.SupplierAddressObjects = new List<SupplierAddressObject>();
                        supplierObject.PurchaseOrderObjects = new List<PurchaseOrderObject>();

                        if (m.SupplierAddresses.Any())
                        {
                            var addressList = m.SupplierAddresses.ToList();
                            addressList.ForEach(x =>
                            {
                                var address = ModelCrossMapper.Map<SupplierAddress, SupplierAddressObject>(x);
                                if (address != null && address.AddressId > 0)
                                {
                                    supplierObject.SupplierAddressObjects.Add(address);
                                }
                            });
                        }

                        if (m.PurchaseOrders.Any())
                        {
                            var purchaseList = m.PurchaseOrders.ToList();
                            purchaseList.ForEach(x =>
                            {
                                var purchaseOrder = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(x);
                                if (purchaseOrder != null && purchaseOrder.PurchaseOrderId > 0)
                                {
                                    supplierObject.PurchaseOrderObjects.Add(purchaseOrder);
                                }
                            });
                        }
                        supplierObject.DateRegistered = supplierObject.DateJoined.ToString("dd/MM/yyyy");
                        supplierObjList.Add(supplierObject);
                    }

                });
                return supplierObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SupplierObject>();
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

        public int GetObjectCount(Expression<Func<Supplier, bool>> predicate)
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

        public List<SupplierObject> GetSuppliers()
        {
            try
            {
                var supplierEntityList = _repository.GetAll().ToList();
                if (!supplierEntityList.Any())
                {
                    return new List<SupplierObject>();
                }
                var supplierObjList = new List<SupplierObject>();
                supplierEntityList.ForEach(m =>
                {
                    var supplierObject = ModelCrossMapper.Map<Supplier, SupplierObject>(m);
                    if (supplierObject != null && supplierObject.SupplierId > 0)
                    {
                        supplierObjList.Add(supplierObject);
                    }
                });
                return supplierObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
