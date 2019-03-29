using System;
using System.Collections.Generic;
using System.Configuration;
using ImportPermitPortal.DataObjects.Helpers;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class OnlineStoreRepository
    {
        private readonly IShopkeeperRepository<Sale> _repository;
        private readonly UnitOfWork _uoWork;
        private readonly ShopKeeperStoreEntities _db;

        public OnlineStoreRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<Sale>(_uoWork);
		}
       
        public long AddSale(SaleObject sale)
        {
            try
            {
                if (sale == null)
                {
                    return -2;
                }
                
                var saleEntity = ModelCrossMapper.Map<SaleObject, Sale>(sale);
                if (saleEntity == null || saleEntity.AmountDue < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(saleEntity);
                _uoWork.SaveChanges();
                return returnStatus.SaleId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSale(SaleObject sale)
        {
            try
            {
                if (sale == null)
                {
                    return -2;
                }
                
                var saleEntity = ModelCrossMapper.Map<SaleObject, Sale>(sale);
                if (saleEntity == null || saleEntity.SaleId < 1)
                {
                    return -2;
                }
                _repository.Update(saleEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSale(long saleId)
        {
            try
            {
                var returnStatus = _repository.Remove(saleId);
                _uoWork.SaveChanges();
                return returnStatus.SaleId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public SaleObject GetSale(long saleId)
        {
            try
            {
                using (var db = _db)
                {
                    var myItems =
                        (from sa in db.Sales.Where(m => m.SaleId == saleId)
                        join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                        join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                         join ps in db.UserProfiles on em.UserId equals ps.Id
                        select new SaleObject
                        {
                            SaleId = sa.SaleId,
                            RegisterId = sa.RegisterId,
                            EmployeeId = em.EmployeeId,
                            AmountDue = sa.AmountDue,
                            Status = sa.Status,
                            Date = sa.Date,
                            RegisterName = rs.Name,
                            SaleEmployeeName = ps.LastName + " " + ps.OtherNames
                        }).ToList();

                    if (!myItems.Any())
                    {
                        return new SaleObject();
                    }
                   
                    var target = myItems[0];

                    target.Transactions = new List<StoreTransactionObject>();
                    target.SoldItems = new List<StoreItemSoldObject>();

                        target.Transactions = (from sta in db.SaleTransactions.Where(m => m.SaleId == target.SaleId)
                        join st in db.StoreTransactions on sta.StoreTransactionId equals st.StoreTransactionId
                        join ef in db.Employees on st.EffectedByEmployeeId equals ef.EmployeeId
                        join ep in db.UserProfiles on ef.UserId equals ep.Id
                        join pm in db.StorePaymentMethods on st.StorePaymentMethodId equals pm.StorePaymentMethodId
                        join ty in db.StoreTransactionTypes on st.StoreTransactionTypeId equals ty.StoreTransactionTypeId
                        join so in db.StoreOutlets on st.StoreOutletId equals so.StoreOutletId
                        select new StoreTransactionObject
                        {
                            StoreTransactionId = st.StoreTransactionId,
                            StoreTransactionTypeId = ty.StoreTransactionTypeId,
                            StorePaymentMethodId = pm.StorePaymentMethodId,
                            EffectedByEmployeeId = ef.EmployeeId,
                            TransactionEmployeeName = ep.LastName + " " + ep.OtherNames,
                            TransactionAmount = st.TransactionAmount,
                            TransactionDate = st.TransactionDate,
                            Remark = st.Remark,
                            StoreOutletId = so.StoreOutletId,
                            OutletName = so.OutletName,
                            StoreTransactionTypeName = ty.Name,
                            PaymentMethodName = pm.Name
                        }).ToList();

                    //StoreName StoreAddress CustomerName CustomerAddress InvoiceNumber UnitPrice

                        var soldItems = (
                        from iss in db.StoreItemSolds.Where(m => m.SaleId == saleId)
                        join sts in db.StoreItemStocks on iss.StoreItemStockId equals sts.StoreItemStockId
                        //join su in db.StockUploads on sts.StoreItemStockId equals su.StoreItemStockId
                        join vv in db.StoreItemVariationValues on sts.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                        join it in db.ItemPrices on sts.StoreItemStockId equals it.StoreItemStockId
                        join um in db.UnitsOfMeasurements on it.UoMId equals um.UnitOfMeasurementId
                        join si in db.StoreItems on sts.StoreItemId equals si.StoreItemId
                        select  new StoreItemSoldObject
                        {
                            ItemSoldName = vv == null ? si.Name : si.Name + "/" + vv.Value,
                            Sku = sts.SKU,
                            QuantitySold = iss.QuantitySold,
                            //ImagePath = su.ImagePath,
                            UnitPrice = it.Price,
                            AmountSold = iss.AmountSold,
                            UoMCode = um.UoMCode,
                            DateSold = iss.DateSold
                        }).ToList();

                    soldItems.ForEach(m =>
                    {
                        //m.ImagePath = m.ImagePath == null || string.IsNullOrEmpty(m.ImagePath) ? "/Content/images/noImage.png" : m.ImagePath.Replace("~", "");
                        if (!target.SoldItems.Exists(s => s.StoreItemSoldId == m.StoreItemSoldId))
                        {
                            m.DateSoldStr = m.DateSold.ToString("dd/MM/yyyy");
                            target.SoldItems.Add(m);
                        }
                       
                    });
                    
                    target.Transactions.ForEach(m =>
                    {
                        target.AmountPaid += m.TransactionAmount;
                        m.TransactionDateStr = m.TransactionDate.ToString("dd/MM/yyyy");
                    });

                    var storeInfoList = db.StoreAddresses.Include("StoreCity").ToList();
                    if (storeInfoList.Any())
                    {
                        target.StoreAddress = storeInfoList[0].StreetNo + " " + storeInfoList[0].StoreCity.Name;
                    }
                    target.InvoiceNumber = "##########";
                    //StoreName StoreAddress CustomerName CustomerAddress InvoiceNumber 
                    target.DateStr = target.Date.ToString("dd/MM/yyyy");
                    return target;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SaleObject();
            }
        }

        public List<SaleObject> GetSalesByOutlet(long outletId, int? itemsPerPage, int? pageNumber)
        {
            try
            {
                using (var db = _db)
                {
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int) pageNumber;
                        var tsize = (int) itemsPerPage;
                        var myItems =
                            (from st in _db.StoreTransactions.Where(m => m.StoreOutletId == outletId).OrderBy(m => m.TransactionDate).Skip((tpageNumber) * tsize).Take(tsize)
                                join stt in db.SaleTransactions on st.StoreTransactionId equals stt.StoreTransactionId
                                join sa in db.Sales on stt.SaleId equals sa.SaleId
                                join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                select new SaleObject
                                {
                                    SaleId = sa.SaleId,
                                    RegisterId = sa.RegisterId,
                                    AmountDue = sa.AmountDue,
                                    Status = sa.Status,
                                    Date = sa.Date,
                                    RegisterName = rs.Name,
                                }).ToList();

                        if (!myItems.Any())
                        {
                            return new List<SaleObject>();
                        }
                        myItems.ForEach(m =>
                        {
                            m.NumberSoldItems = db.StoreItemSolds.Count(x => x.SaleId == m.SaleId);
                            m.DateStr = m.Date.ToString("dd/MM/yyyy");
                        });
                        return myItems;
                    }
                    return new List<SaleObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetSalesByEmployee(int outletId, int employeeId, int? itemsPerPage, int? pageNumber)
        {
            try
            {
                using (var db = _db)
                {
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        var myItems =
                            (from st in db.StoreTransactions.Where(m => m.StoreOutletId == outletId).OrderBy(m => m.TransactionDate).Skip((tpageNumber) * tsize).Take(tsize)
                             join stt in db.SaleTransactions on st.StoreTransactionId equals stt.StoreTransactionId
                             join sa in db.Sales on stt.SaleId equals sa.SaleId where sa.EmployeeId == employeeId
                             join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                             select new SaleObject
                             {
                                 SaleId = sa.SaleId,
                                 RegisterId = sa.RegisterId,
                                 AmountDue = sa.AmountDue,
                                 Status = sa.Status,
                                 Date = sa.Date,
                                 RegisterName = rs.Name,
                             }).ToList();

                        if (!myItems.Any())
                        {
                            return new List<SaleObject>();
                        }
                        myItems.ForEach(m =>
                        {
                            m.NumberSoldItems = db.StoreItemSolds.Count(x => x.SaleId == m.SaleId);
                            m.DateStr = m.Date.ToString("dd/MM/yyyy");
                        });
                        return myItems;
                    }
                    return new List<SaleObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetSaleObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                using (var db = _db)
                {
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;

                       var saleObjectList = (from sa in db.Sales.OrderBy(m => m.SaleId).Skip((tpageNumber)*tsize).Take(tsize)
                                             join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                             join ps in db.UserProfiles on em.UserId equals ps.Id
                                             join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                             select new SaleObject
                                             {
                                                 SaleId = sa.SaleId,
                                                 RegisterId = sa.RegisterId,
                                                 EmployeeId = em.EmployeeId,
                                                 AmountDue = sa.AmountDue,
                                                 Status = sa.Status,
                                                 Date = sa.Date,
                                                 RegisterName = rs.Name,
                                                 SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                             }).ToList();
                             
                        if (!saleObjectList.Any())
                        {
                            return new List<SaleObject>();
                        }
                        saleObjectList.ForEach(m =>
                        {
                            m.DateStr = m.Date.ToString("dd/MM/yyyy");
                        });
                        return saleObjectList;
                    }
                  
                   return new List<SaleObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
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

        public int GetObjectCount(Expression<Func<Sale, bool>> predicate)
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

        public List<SaleObject> Search(int outletId, string searchCriteria)
        {
            try
            {
                using (var db = _db)
                {
                    List<SaleObject> searchList;
                    DateTime date;
                    var result = DateTime.TryParse(searchCriteria, out date);
                    if (result)
                    {
                        searchList = (from sa in db.Sales.Where(m => m.Date == date)
                                      join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                      join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                      join st in db.StoreTransactions on stt.StoreTransactionId equals st.StoreTransactionId
                                      where st.StoreOutletId == outletId
                                      select new SaleObject
                                      {
                                          SaleId = sa.SaleId,
                                          RegisterId = sa.RegisterId,
                                          AmountDue = sa.AmountDue,
                                          Status = sa.Status,
                                          Date = sa.Date,
                                          RegisterName = rs.Name,
                                      }).ToList();
                    }
                    else
                    {
                        searchList = (from st in db.StoreTransactions.Where(m => m.StoreOutletId == outletId).OrderBy(m => m.TransactionDate)
                                      join stt in db.SaleTransactions on st.StoreTransactionId equals stt.StoreTransactionId
                                      join sa in db.Sales on stt.SaleId equals sa.SaleId
                                      join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                      select new SaleObject
                                      {
                                          SaleId = sa.SaleId,
                                          RegisterId = sa.RegisterId,
                                          AmountDue = sa.AmountDue,
                                          Status = sa.Status,
                                          Date = sa.Date,
                                          RegisterName = rs.Name,
                                      }).ToList();
                    }

                    if (!searchList.Any())
                    {
                        return new List<SaleObject>();
                    }
                    searchList.ForEach(m =>
                    {
                        m.NumberSoldItems = db.StoreItemSolds.Count(x => x.SaleId == m.SaleId);
                        m.DateStr = m.Date.ToString("dd/MM/yyyy");
                    });
                    return searchList;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> Search(string searchCriteria)
        {
            try
            {
                using (var db = _db)
                {
                    List<SaleObject> searchList; 
                    DateTime date;
                    var result = DateTime.TryParse(searchCriteria, out date);
                    if (result)
                    {
                        searchList = (from sa in db.Sales.Where(m => m.Date == date)
                                      join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                      join ps in db.UserProfiles on em.UserId equals ps.Id
                                      join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                      select new SaleObject
                                      {
                                          SaleId = sa.SaleId,
                                          RegisterId = sa.RegisterId,
                                          AmountDue = sa.AmountDue,
                                          Status = sa.Status,
                                          Date = sa.Date,
                                          RegisterName = rs.Name,
                                          SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                      }).ToList();
                    }
                    else
                    {
                        searchList = (from ps in db.UserProfiles.Where(m => m.LastName.Contains(searchCriteria) || m.OtherNames.Contains(searchCriteria))
                                      join em in db.Employees on ps.Id equals em.UserId
                                      join sa in db.Sales on em.EmployeeId equals sa.EmployeeId
                                      join rs in db.Registers on sa.RegisterId equals rs.RegisterId
                                      select new SaleObject
                                      {
                                          SaleId = sa.SaleId,
                                          RegisterId = sa.RegisterId,
                                          EmployeeId = em.EmployeeId,
                                          AmountDue = sa.AmountDue,
                                          Status = sa.Status,
                                          Date = sa.Date,
                                          RegisterName = rs.Name,
                                          SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                      }).ToList();
                    }
                     
                    if (!searchList.Any())
                    {
                        return new List<SaleObject>();
                    }
                    searchList.ForEach(m =>
                    {
                        m.NumberSoldItems = db.StoreItemSolds.Count(x => x.SaleId == m.SaleId);
                        m.DateStr = m.Date.ToString("dd/MM/yyyy");
                    });
                    return searchList;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetSales()
        {
            try
            {
                var saleEntityList = _repository.GetAll().ToList();
                if (!saleEntityList.Any())
                {
                    return new List<SaleObject>();
                }
                var saleObjList = new List<SaleObject>();
                saleEntityList.ForEach(m =>
                {
                    var saleObject = ModelCrossMapper.Map<Sale, SaleObject>(m);
                    if (saleObject != null && saleObject.SaleId > 0)
                    {
                        saleObjList.Add(saleObject);
                    }
                });
                return saleObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
