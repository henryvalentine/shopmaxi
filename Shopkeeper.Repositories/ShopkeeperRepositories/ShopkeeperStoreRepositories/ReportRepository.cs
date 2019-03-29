using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class ReportRepository
    {
        private readonly IShopkeeperRepository<ParentMenu> _repository;
        private readonly UnitOfWork _uoWork;
        private readonly ShopKeeperStoreEntities _db;

        public ReportRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<ParentMenu>(_uoWork);
		}

        #region SALES REPORT
        public List<StoreItemSoldObject> GetStoreItemTypeReport(int? itemsPerPage, int? pageNumber, out int countG, int typeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var soldItems = (from sts in db.StoreItems.Where(s => s.StoreItemTypeId == typeId).OrderBy(m => m.Name).Skip((tpageNumber) * tsize).Take(tsize)
                                         join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                         join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate).Include("Sale") on sti.StoreItemStockId equals sto.StoreItemStockId
                                         join em in db.Employees on sto.Sale.EmployeeId equals em.EmployeeId
                                         join usr in db.UserProfiles on em.UserId equals usr.Id
                                         
                                         join ui in db.UnitsOfMeasurements on sto.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sts.Name : sts.Name + "/" + vv.Value,
                                             QuantitySold = sto.QuantitySold,
                                             AmountSold = sto.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             DateSold = sto.Sale.Date,
                                             SaleId = sto.Sale.SaleId,
                                             Employee = usr.LastName + " " + usr.OtherNames,
                                             Rate = sto.Rate

                                         }).ToList();

                        if (!soldItems.Any())
                        {
                            countG = 0;
                            return new List<StoreItemSoldObject>();
                        }

                        soldItems.ForEach(s =>
                        {
                            s.DateSoldStr = s.DateSold.ToString("dd/MM/yyyy");
                            s.AmountSoldStr = s.AmountSold.ToString("n0");
                            s.QuantitySoldStr = s.QuantitySold.ToString("n0");
                        });

                        countG = (from sts in db.StoreItems.Where(s => s.StoreItemTypeId == typeId)
                                  join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                  join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate) on sti.StoreItemStockId equals sto.StoreItemStockId
                                  select sts).Count();
                        return soldItems;

                    }

                }
                countG = 0;
                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreItemSoldObject>();
            }
        }
        public List<StoreItemSoldObject> GetStoreItemCategoryReport(int? itemsPerPage, int? pageNumber, out int countG, int categoryId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var soldItems = (from sts in db.StoreItems.Where(s => s.StoreItemCategoryId == categoryId).OrderBy(m => m.Name).Skip((tpageNumber) * tsize).Take(tsize)
                                         join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                         join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate).Include("Sale") on sti.StoreItemStockId equals sto.StoreItemStockId
                                         join em in db.Employees on sto.Sale.EmployeeId equals em.EmployeeId
                                         join usr in db.UserProfiles on em.UserId equals usr.Id
                                         
                                         join ui in db.UnitsOfMeasurements on sto.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sts.Name : sts.Name + "/" + vv.Value,
                                             QuantitySold = sto.QuantitySold,
                                             AmountSold = sto.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             DateSold = sto.Sale.Date,
                                             SaleId = sto.Sale.SaleId,
                                             Employee = usr.LastName + " " + usr.OtherNames,
                                             Rate = sto.Rate

                                         }).ToList();

                        if (!soldItems.Any())
                        {
                            countG = 0;
                            return new List<StoreItemSoldObject>();
                        }

                        soldItems.ForEach(s =>
                        {
                            s.DateSoldStr = s.DateSold.ToString("dd/MM/yyyy");
                            s.AmountSoldStr = s.AmountSold.ToString("n0");
                            s.QuantitySoldStr = s.QuantitySold.ToString("n0");
                        });

                        countG = (from sts in db.StoreItems.Where(s => s.StoreItemCategoryId == categoryId)
                                         join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                         join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate) on sti.StoreItemStockId equals sto.StoreItemStockId
                                         select sto.Sale).Count();
                        return soldItems;

                    }

                }
                countG = 0;
                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreItemSoldObject>();
            }
        }

        public List<StoreItemSoldObject> GetAllSalesReportByCategory(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {

                        var sales = db.Sales.Where(s => (s.Date >= startDate && s.Date <= endDate)).Include("SaleTransactions").Include("StoreItemSolds").OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize).ToList();

                        if (!sales.Any())
                        {
                            return new List<StoreItemSoldObject>();
                        }

                        var overAllCategory = new List<StoreItemSoldObject>();

                        sales.ForEach(s =>
                        {
                            var transactionsList = s.SaleTransactions.ToList();

                            var amountPaid = 0.0;

                            var categorizedSoldItems = new List<StoreItemSoldObject>();

                            transactionsList.ForEach(y =>
                            {
                                var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == y.StoreTransactionId).ToList();
                                if (!transactions.Any())
                                {
                                    return;
                                }

                                transactions.ForEach(n =>
                                {
                                    amountPaid += n.TransactionAmount;
                                });

                            });

                            var soldItems = s.StoreItemSolds.ToList();

                            soldItems.ForEach(i =>
                            {
                                var storeItems = (from st in db.StoreItemStocks.Where(w => w.StoreItemStockId == i.StoreItemStockId).Include("StoreItem")
                                                  join sts in db.StoreItems.Include("StoreItemCategory") on st.StoreItemId equals sts.StoreItemId
                                                  select new StoreItemStockObject
                                                  {
                                                      CategoryName = sts.StoreItemCategory.Name,
                                                      CostPrice = st.CostPrice,
                                                      StoreItemCategoryId = sts.StoreItemCategoryId,
                                                      StoreItemStockId = st.StoreItemStockId

                                                  }).ToList();

                                if (!storeItems.Any())
                                {
                                    return;
                                }

                                var refCategory = storeItems[0];

                                var soldItem = new StoreItemSoldObject();
                                var existing = false;

                                var existingCategory = categorizedSoldItems.Find(c => c.StoreItemCategoryId == refCategory.StoreItemCategoryId);

                                if (existingCategory == null || existingCategory.StoreItemCategoryId < 1)
                                {
                                    soldItem.StoreItemStockId = i.StoreItemStockId;
                                    soldItem.AmountSold = i.AmountSold;
                                    soldItem.Rate = i.Rate;
                                    soldItem.CategoryName = refCategory.CategoryName;
                                    soldItem.DateSold = s.Date;
                                    soldItem.CostPrice = refCategory.CostPrice;
                                    soldItem.QuantitySold = i.QuantitySold;
                                    soldItem.StoreItemCategoryId = refCategory.StoreItemCategoryId;
                                    soldItem.StoreItemSoldId = i.StoreItemSoldId;

                                    if (storeItems[0].CostPrice != null && storeItems[0].CostPrice > 0)
                                    {
                                        soldItem.CostOfSale = ((double)storeItems[0].CostPrice) * i.QuantitySold;
                                    }
                                }
                                else
                                {
                                    soldItem = existingCategory;
                                    soldItem.AmountSold += i.AmountSold;
                                    soldItem.QuantitySold += i.QuantitySold;

                                    if (storeItems[0].CostPrice != null && storeItems[0].CostPrice > 0)
                                    {
                                        soldItem.CostOfSale += ((double)storeItems[0].CostPrice) * i.QuantitySold;
                                    }
                                    existing = true;
                                }

                                if (existing == false)
                                {
                                    categorizedSoldItems.Add(soldItem);
                                }
                            });

                            if (!categorizedSoldItems.Any())
                            {
                                return;
                            }

                            categorizedSoldItems.ForEach(d =>
                            {

                                if (!s.AmountDue.Equals(s.NetAmount))
                                {
                                    if (s.Discount > 0)
                                    {
                                        d.NetAmount = d.AmountSold - ((d.AmountSold * s.Discount) / 100);
                                    }

                                    if (s.VAT > 0)
                                    {
                                        d.NetAmount += (d.AmountSold * s.VAT) / 100;
                                    }
                                }
                                else
                                {
                                    d.NetAmount = d.AmountSold;
                                }

                                //profit
                                d.Profit = d.NetAmount - d.CostOfSale;

                                if (!s.NetAmount.Equals(amountPaid) && amountPaid > 0)
                                {
                                    if (amountPaid < d.NetAmount)
                                    {
                                        d.AmountPaid = amountPaid;
                                    }
                                    else
                                    {
                                        if (amountPaid > d.NetAmount)
                                        {
                                             d.AmountPaid = amountPaid - d.NetAmount;
                                             amountPaid -= d.NetAmount;
                                        }

                                        if (amountPaid.Equals(d.NetAmount))
                                        {
                                            d.AmountPaid = amountPaid;
                                            amountPaid = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    if (s.NetAmount.Equals(amountPaid))
                                    {
                                        d.AmountPaid = d.AmountSold;
                                    }
                                    else
                                    {
                                        if (!s.NetAmount.Equals(amountPaid) && amountPaid < 1)
                                        {
                                            d.AmountPaid = 0;
                                        }
                                    }

                                }

                                var existingCategory2 = overAllCategory.Find(c => c.StoreItemCategoryId == d.StoreItemCategoryId);

                                if (existingCategory2 == null || existingCategory2.StoreItemCategoryId < 1)
                                {
                                    overAllCategory.Add(d);
                                }
                                else
                                {
                                    existingCategory2.NetAmount += d.NetAmount;
                                    existingCategory2.AmountSold += d.AmountSold;
                                    existingCategory2.QuantitySold += d.QuantitySold;
                                    existingCategory2.CostOfSale += d.CostOfSale;
                                    existingCategory2.AmountPaid += d.AmountPaid;
                                    existingCategory2.Profit += d.Profit;
                                }

                            });
                        });

                        if (!overAllCategory.Any())
                        {
                            return new List<StoreItemSoldObject>();
                        }

                        var allCategorizedSales = new List<StoreItemSoldObject>();

                        overAllCategory.ForEach(g =>
                        {
                            var existing = allCategorizedSales.Find(z => z.StoreItemCategoryId == g.StoreItemCategoryId);
                            if (existing == null || existing.StoreItemCategoryId < 1)
                            {
                                allCategorizedSales.Add(g);
                            }
                            else
                            {
                                existing.NetAmount += g.NetAmount;
                                existing.AmountSold += g.AmountSold;
                                existing.QuantitySold += g.QuantitySold;
                                existing.CostOfSale += g.CostOfSale;
                                existing.AmountPaid += g.AmountPaid;
                                existing.Profit += g.Profit;
                            }
                        });

                        return overAllCategory;

                    }

                }

                ErrorLogger.LogError("Get category report", "Repot repo", "items per page evalutation failed");
                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }

        public List<StoreItemSoldObject> GetSalesReportByCategory(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate, int categoryId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => (s.Date >= startDate && s.Date <= endDate)).Include("SaleTransactions").Include("StoreItemSolds").OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                      join so in db.StoreItemSolds on sa.SaleId equals so.SaleId
                                      join st in db.StoreItemStocks.Where(p => p.StoreItem.StoreItemCategoryId == categoryId) on so.StoreItemStockId equals st.StoreItemStockId
                                      select sa).ToList();

                        if (!sales.Any())
                        {
                            return new List<StoreItemSoldObject>();
                        }

                        var overAllCategory = new List<StoreItemSoldObject>();

                        sales.ForEach(s =>
                        {
                            var transactionsList = s.SaleTransactions.ToList();

                            var amountPaid = 0.0;

                            var categorizedSoldItems = new List<StoreItemSoldObject>();

                            transactionsList.ForEach(y =>
                            {
                                var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == y.StoreTransactionId).ToList();
                                if (!transactions.Any())
                                {
                                    return;
                                }

                                transactions.ForEach(n =>
                                {
                                    amountPaid += n.TransactionAmount;
                                });

                            });

                            var soldItems = s.StoreItemSolds.ToList();

                            soldItems.ForEach(i =>
                            {
                                var storeItems = (from st in db.StoreItemStocks.Where(w => w.StoreItemStockId == i.StoreItemStockId).Include("StoreItem")
                                                  join sts in db.StoreItems.Where(x => x.StoreItemCategoryId == categoryId).Include("StoreItemCategory") on st.StoreItemId equals sts.StoreItemId
                                                  select new StoreItemStockObject
                                                  {
                                                      CategoryName = sts.StoreItemCategory.Name,
                                                      CostPrice = st.CostPrice,
                                                      StoreItemCategoryId = sts.StoreItemCategoryId,
                                                      StoreItemStockId = st.StoreItemStockId

                                                  }).ToList();

                                if (!storeItems.Any())
                                {
                                    return;
                                }

                                var refCategory = storeItems[0];

                                var soldItem = new StoreItemSoldObject();
                                var existing = false;

                                var existingCategory = categorizedSoldItems.Find(c => c.StoreItemCategoryId == refCategory.StoreItemCategoryId && c.StoreItemCategoryId == categoryId);

                                if (existingCategory == null || existingCategory.StoreItemCategoryId < 1)
                                {
                                    soldItem.StoreItemStockId = i.StoreItemStockId;
                                    soldItem.AmountSold = i.AmountSold;
                                    soldItem.Rate = i.Rate;
                                    soldItem.CategoryName = refCategory.CategoryName;
                                    soldItem.DateSold = s.Date;
                                    soldItem.CostPrice = refCategory.CostPrice;
                                    soldItem.QuantitySold = i.QuantitySold;
                                    soldItem.StoreItemCategoryId = refCategory.StoreItemCategoryId;
                                    soldItem.StoreItemSoldId = i.StoreItemSoldId;

                                    if (storeItems[0].CostPrice != null && storeItems[0].CostPrice > 0)
                                    {
                                        soldItem.CostOfSale = ((double)storeItems[0].CostPrice) * i.QuantitySold;
                                        soldItem.Profit = soldItem.AmountSold - soldItem.CostOfSale;
                                    }
                                    categorizedSoldItems.Add(soldItem);
                                }
                                else
                                {
                                    if (existingCategory.StoreItemCategoryId == categoryId)
                                    {
                                        soldItem = existingCategory;
                                        soldItem.AmountSold += i.AmountSold;
                                        soldItem.QuantitySold += i.QuantitySold;

                                        if (storeItems[0].CostPrice != null && storeItems[0].CostPrice > 0)
                                        {
                                            soldItem.CostOfSale += ((double) storeItems[0].CostPrice)*i.QuantitySold;
                                            soldItem.Profit += (soldItem.AmountSold - soldItem.CostOfSale);
                                        }
                                    }
                                }

                            });

                            if (!categorizedSoldItems.Any())
                            {
                                return;
                            }

                            categorizedSoldItems.ForEach(d =>
                            {

                                if (!s.AmountDue.Equals(s.NetAmount))
                                {
                                    if (s.Discount > 0)
                                    {
                                        d.NetAmount = d.AmountSold - ((d.AmountSold * s.Discount) / 100);
                                    }

                                    if (s.VAT > 0)
                                    {
                                        d.NetAmount += (d.AmountSold * s.VAT) / 100;
                                    }
                                }
                                else
                                {
                                    d.NetAmount = d.AmountSold;
                                }

                                if (!s.NetAmount.Equals(amountPaid) && amountPaid > 0)
                                {
                                    if (amountPaid < d.NetAmount)
                                    {
                                        d.AmountPaid = amountPaid;
                                    }
                                    else
                                    {
                                        d.AmountPaid = amountPaid - d.NetAmount;
                                        amountPaid -= d.NetAmount;
                                    }
                                }
                                else
                                {

                                    if (s.NetAmount.Equals(amountPaid))
                                    {
                                        d.AmountPaid = d.AmountSold;
                                    }
                                    else
                                    {
                                        if (!s.NetAmount.Equals(amountPaid) && amountPaid < 1)
                                        {
                                            d.AmountPaid = 0;
                                        }
                                    }

                                }

                                var existingCategory2 = overAllCategory.Find(c => c.StoreItemCategoryId == d.StoreItemCategoryId);

                                if (existingCategory2 == null || existingCategory2.StoreItemCategoryId < 1)
                                {
                                    overAllCategory.Add(d);
                                }
                                else
                                {
                                    existingCategory2.AmountSold += d.AmountSold;
                                    existingCategory2.QuantitySold += d.QuantitySold;
                                    existingCategory2.CostOfSale += d.CostOfSale;
                                    existingCategory2.Profit += d.Profit;
                                }


                            });
                        });

                        if (!overAllCategory.Any())
                        {
                            return new List<StoreItemSoldObject>();
                        }

                        var allCategorizedSales = new List<StoreItemSoldObject>();

                        overAllCategory.ForEach(g =>
                        {
                            var existing = allCategorizedSales.Find(z => z.StoreItemCategoryId == g.StoreItemCategoryId);
                            if (existing == null || existing.StoreItemCategoryId < 1)
                            {
                                allCategorizedSales.Add(g);
                            }
                            else
                            {
                                existing.NetAmount += g.NetAmount;
                                existing.AmountSold += g.AmountSold;
                                existing.QuantitySold += g.QuantitySold;
                                existing.CostOfSale += g.CostOfSale;
                                existing.AmountPaid += g.AmountPaid;
                                existing.Profit += g.Profit;
                            }
                        });

                        return overAllCategory;
                    }

                }
                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }

        public List<StoreItemSoldObject> GetStoreItemReport(int? itemsPerPage, int? pageNumber, out int countG, long itemId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        //var soldItems = ( from sto in db.StoreItemSolds.Where(a => a.StoreItemStockId == itemId && (a.DateSold <= endDate && a.DateSold >= startDate))
                        //                  join sti in db.StoreItemStocks.OrderBy(m => m.StoreItem.Name).Skip((tpageNumber) * tsize).Take(tsize) on sto.StoreItemStockId equals sti.StoreItemStockId
                        //                  join sts in db.StoreItems on sti.StoreItemId equals sts.StoreItemId
                        //                  join em in db.Employees on sto.Sale.EmployeeId equals em.EmployeeId
                        //                join usr in db.UserProfiles on em.UserId equals usr.Id
                        //                
                        //                join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                        //                join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                        //                select new StoreItemSoldObject
                        //                {
                        //                    StoreItemName = vv == null ? sts.Name : sts.Name + "/" + vv.Value,
                        //                    QuantitySold = sto.QuantitySold,
                        //                    AmountSold = sto.AmountSold,
                        //                    StoreItemStockId = sti.StoreItemStockId,
                        //                    UoMCode = ui.UoMCode,
                        //                    DateSold = sto.Sale.Date,
                        //                    SaleId = sto.Sale.SaleId,
                        //                    Employee = usr.LastName + " " + usr.OtherNames

                        //                }).ToList();

                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.StoreItemStockId == itemId && d.DateSold <= endDate && d.DateSold >= startDate)
                                         join sti in db.StoreItemStocks.OrderBy(m => m.StoreItem.Name).Skip((tpageNumber) * tsize).Take(tsize) on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                         
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemSoldId = sts.StoreItemSoldId,
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             QuantityLeft = sti.QuantityInStock,
                                             DateSold = sts.DateSold,
                                             Rate = sts.Rate,
                                             QuantityAlreadySold = sti.TotalQuantityAlreadySold

                                         }).ToList();

                        if (!soldItems.Any())
                        {
                            countG = 0;
                            return new List<StoreItemSoldObject>();
                        }
                        soldItems.ForEach(n =>
                        {
                            n.DateSoldStr = n.DateSold.ToString("dd/MM/yyyy hh:mm tt");
                            n.QuantitySoldStr = n.QuantitySold.ToString("n0");
                            n.AmountSoldStr = n.AmountSold.ToString("n0");
                            n.QuantityLeftStr = n.QuantityLeft.ToString("n0");
                            n.RateStr = n.Rate.ToString("n0");
                        });
                        
                        countG = (from sts in db.StoreItems.Where(s => s.StoreItemId == itemId)
                                    join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                    join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate) on sti.StoreItemStockId equals sto.StoreItemStockId select sts).Count();
                        return soldItems;

                    }

                }
                countG = 0;
                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreItemSoldObject>();
            }
        }
        public List<StoreItemSoldObject> GetAllProductReport(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        //(d => ((d.Sale.Date == startDate || d.Sale.Date == endDate) ||  (d.Sale.Date > startDate && d.Sale.Date < endDate)))


                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.DateSold >= startDate && d.DateSold <= endDate)
                                         join sti in db.StoreItemStocks.OrderBy(m => m.StoreItem.Name).Skip((tpageNumber) * tsize).Take(tsize) on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemSoldId = sts.StoreItemSoldId,
                                             StoreItemStockId = sti.StoreItemStockId,
                                             InvoiceNumber = sts.Sale.InvoiceNumber,
                                             Description = sto.Description,
                                             Sku = sti.SKU,
                                             CostPrice = sti.CostPrice,
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             QuantityLeft = sti.QuantityInStock,
                                             DateSold = sts.DateSold,
                                             Rate = sts.Rate,
                                             QuantityAlreadySold = sti.TotalQuantityAlreadySold
                                              
                                         }).ToList(); 

                        if (!soldItems.Any())
                        {
                            return new List<StoreItemSoldObject>();
                        }

                        
                        soldItems.ForEach(n =>
                        {
                            n.DateSoldStr = n.DateSold.ToString("dd/MM/yyyy");
                            n.QuantitySoldStr = n.QuantitySold.ToString("n0");
                            n.AmountSoldStr = n.AmountSold.ToString("n0");
                            n.RateStr = n.Rate.ToString("n0");
                            n.QuantityLeftStr = n.QuantityLeft.ToString("n0");
                            n.StockValueStr = n.CostPrice != null? (n.QuantityLeft * ((double)n.CostPrice)).ToString("n0") : "";
                            if (n.CostPrice != null)
                            {
                                n.StockValue = n.QuantityLeft*((double) n.CostPrice);
                                n.StockValueStr = n.StockValue.ToString("n0");
                                n.CostOfSale = n.QuantitySold*((double) n.CostPrice);
                                n.CostOfSaleStr = n.CostOfSale.ToString("n0");
                            }
                            else
                            {
                                n.StockValue = 0;
                                n.StockValueStr = "0";
                                n.CostOfSale = 0;
                                n.CostOfSaleStr = "0";
                            }
                            
                            n.RateStr = n.Rate.ToString("n0");
                        });
                        
                        return soldItems;

                    }

                }
         
                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }
        public List<StoreItemSoldObject> GetSingleProductReport(int? itemsPerPage, int? pageNumber, long itemId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.StoreItemStockId == itemId && d.DateSold <= endDate && d.DateSold >= startDate)
                                         join sti in db.StoreItemStocks.OrderBy(m => m.StoreItem.Name).Skip((tpageNumber) * tsize).Take(tsize) on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                         
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemSoldId = sts.StoreItemSoldId,
                                             StoreItemStockId = sti.StoreItemStockId,
                                             InvoiceNumber = sts.Sale.InvoiceNumber,
                                             Description = sto.Description,
                                             Sku = sti.SKU,
                                             CostPrice = sti.CostPrice,
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             QuantityLeft = sti.QuantityInStock,
                                             DateSold = sts.DateSold,
                                             Rate = sts.Rate,
                                             QuantityAlreadySold = sti.TotalQuantityAlreadySold

                                         }).ToList();

                        if (!soldItems.Any())
                        {
                            return new List<StoreItemSoldObject>();
                        }
                        soldItems.ForEach(n =>
                        {
                            n.DateSoldStr = n.DateSold.ToString("dd/MM/yyyy");
                            n.QuantitySoldStr = n.QuantitySold.ToString("n0");
                            n.AmountSoldStr = n.AmountSold.ToString("n0");
                            n.QuantityLeftStr = n.QuantityLeft.ToString("n0");
                            n.StockValueStr = n.CostPrice != null ? (n.QuantityLeft * ((double)n.CostPrice)).ToString("n0") : "";
                            n.RateStr = n.Rate.ToString("n0");
                        });

                        return soldItems;

                    }

                }
         
                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }

        public List<StoreItemStockObject> GetPriceList(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    var serviceCategory = System.Configuration.ConfigurationManager.AppSettings["ServiceCategoryId"];

                    var categoryId = 0;

                    if (!string.IsNullOrEmpty(serviceCategory))
                    {
                        int.TryParse(serviceCategory, out categoryId);
                    }

                    using (var db = _db)
                    {
                        var soldItems = (from sti in db.StoreItemStocks.OrderBy(m => m.StoreItemStockId).Skip((tpageNumber) * tsize).Take(tsize)
                                         join sto in db.StoreItems.Where(c => c.StoreItemCategoryId != categoryId).Include("StoreItemCategory") on sti.StoreItemId equals sto.StoreItemId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemStockObject
                                         {
                                             StoreItemStockId = sti.StoreItemStockId,
                                             Description = sto.Description,
                                             SKU = sti.SKU,
                                             StoreItemCategoryId = sto.StoreItemCategoryId,
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantityInStock = sti.QuantityInStock,
                                             TotalQuantityAlreadySold = sti.TotalQuantityAlreadySold,
                                             CategoryName = sto.StoreItemCategory.Name

                                         }).ToList();

                        if (!soldItems.Any())
                        {
                            return new List<StoreItemStockObject>();
                        }

                        soldItems.ForEach(n =>
                        {
                             n.ItemPriceObjects = new List<ItemPriceObject>();
                            var prices = (from it in db.ItemPrices.Where(p => p.StoreItemStockId == n.StoreItemStockId).Include("UnitsOfMeasurement")
                                 select new ItemPriceObject
                                 {
                                     ItemPriceId = it.ItemPriceId,
                                     StoreItemStockId = it.StoreItemStockId,
                                     Price = it.Price,
                                     Remark = it.Remark,
                                     UoMId = it.UoMId,
                                     MinimumQuantity = it.MinimumQuantity,
                                     UoMCode = it.UnitsOfMeasurement.UoMCode

                                 }).ToList();

                            if (prices.Any())
                            {
                                prices.ForEach(p =>
                                {
                                    p.PriceStr = p.Price.ToString("n0");
                                    p.MinimumQuantityStr = p.MinimumQuantity.ToString("n0");
                                    n.ItemPriceObjects.Add(p);
                                }); 
                            }
                            n.QuantityInStockStr = n.QuantityInStock.ToString("n0");
                            n.TotalQuantityAlreadySoldStr = n.TotalQuantityAlreadySold.ToString("n0");
                        });

                        return soldItems.OrderBy(i => i.StoreItemCategoryId).ToList();
                    }  

                }

                return new List<StoreItemStockObject>();
            }
            catch (Exception ex)
            {
                return new List<StoreItemStockObject>();
            }
        }

        public List<StoreItemStockObject> GetRecommendedPurchases(int itemsPerPage, int pageNumber, out int countG)
        {
            try
            {
                using (var db = _db)
                {
                    var soldItems = (from sti in db.StoreItemStocks.Where(s => s.ReorderLevel > 0 && s.QuantityInStock <= s.ReorderLevel).OrderBy(m => m.StoreItemStockId).Skip((pageNumber) * itemsPerPage).Take(itemsPerPage)
                                     join sto in db.StoreItems.Include("StoreItemCategory") on sti.StoreItemId equals sto.StoreItemId
                                     join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     select new StoreItemStockObject
                                     {
                                         StoreItemStockId = sti.StoreItemStockId,
                                         Description = sto.Description,
                                         ReorderLevel = sti.ReorderLevel,
                                         ReorderQuantity = sti.ReorderQuantity,
                                         SKU = sti.SKU,
                                         StoreItemCategoryId = sto.StoreItemCategoryId,
                                         StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                         QuantityInStock = sti.QuantityInStock,
                                         TotalQuantityAlreadySold = sti.TotalQuantityAlreadySold,
                                         CategoryName = sto.StoreItemCategory.Name

                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        countG = 0;
                        return new List<StoreItemStockObject>();
                    }

                    soldItems.ForEach(n =>
                    {
                        n.QuantityInStockStr = n.QuantityInStock.ToString("n0");
                        n.ReOrderLevelStr = n.ReorderLevel.ToString("n0");
                        n.ReOrderQuantityStr = n.ReorderQuantity.ToString("n0");
                        n.TotalQuantityAlreadySoldStr = n.TotalQuantityAlreadySold.ToString("n0");
                    });
                    countG = (db.StoreItemStocks.Where(s => s.ReorderLevel > 0 && s.QuantityInStock <= s.ReorderLevel)).Count();
                    return soldItems.OrderBy(i => i.StoreItemCategoryId).ToList();
                }
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreItemStockObject>();
            }
        }

        public DailySaleReport GetReportSnapshots(DateTime refDate) 
        {
            try
            {
                var rep = new DailySaleReport
                {
                    TotalItemsToReorder = 0,
                    TotalDueInvoices = 0,
                    CustomerPerformances = new List<CustomerInvoiceObject>(),
                    HighSalingStockList = new List<StoreItemSoldObject>(),
                    SaleComparism = new List<SaleComparism>()
                };

                using (var db = _db)
                {
                    var refDate1 = DateTime.Today;
                    

                    var today = DateTime.Today;
                    var day1 = today.AddDays(-1);
                    var day2 = today.AddDays(-2);
                    var day3 = today.AddDays(-3);
                    var day4 = today.AddDays(-4);
                    var day5 = today.AddDays(-5);
                    var day6 = today.AddDays(-6);

                    var refunded = (int) SaleStatus.Refund_Note_Issued;
                    rep.TotalItemsToReorder = db.StoreItemStocks.Count(s => s.ReorderLevel > 0 && s.QuantityInStock <= s.ReorderLevel);
                    rep.TotalDueInvoices = (from ds in db.Sales.Where(a => a.Date <= day2 && a.Status != refunded)
                                               join sp in db.SalePayments on ds.SaleId equals sp.SaleId
                                               where ds.NetAmount > sp.AmountPaid
                                               select ds).Count();

                    var customerprformances = (from cu in db.CustomerInvoices.OrderByDescending(r => r.TotalAmountPaid).Take(10).Include("Customer") 
                                               join ps in db.UserProfiles on cu.Customer.UserId equals ps.Id
                                                   select new CustomerInvoiceObject
                                                   {
                                                        CustomerName = ps.LastName + " " + ps.OtherNames + "(" + ps.MobileNumber + ")",
                                                        Id = cu.Id,
                                                        CustomerId = cu.CustomerId,
                                                        TotalAmountDue = cu.TotalAmountDue,
                                                        InvoiceBalance = cu.InvoiceBalance,
                                                        TotalVATAmount = cu.TotalVATAmount,
                                                        TotalDiscountAmount = cu.TotalDiscountAmount,
                                                        TotalAmountPaid = cu.TotalAmountPaid

                                                   }).ToList(); 

                    rep.CustomerPerformances = customerprformances;


                    var dailySales = db.Sales.Where(a => a.Date >= refDate1 && a.Date  <= refDate).Include("SalePayments").ToList();

                    var dailyRefunds = db.RefundNotes.Where(a => a.DateReturned >= refDate1 && a.DateReturned <= refDate).ToList();

                    rep.DailySalesCount = db.Sales.Count(a => a.Date >= refDate1 && a.Date <= refDate);

                    var day1Sales = db.Sales.Where(a => a.Date >= day1 && a.Date < today).Include("SalePayments").ToList();
                    var day2Sales = db.Sales.Where(a => a.Date >= day2 && a.Date < day1).Include("SalePayments").ToList();
                    var day3Sales = db.Sales.Where(a => a.Date >= day3 && a.Date < day2).Include("SalePayments").ToList();
                    var day4Sales = db.Sales.Where(a => a.Date >= day4 && a.Date < day3).Include("SalePayments").ToList();
                    var day5Sales = db.Sales.Where(a => a.Date >= day5 && a.Date < day4).Include("SalePayments").ToList();
                    var day6Sales = db.Sales.Where(a => a.Date >= day6 && a.Date < day5).Include("SalePayments").ToList();

                    var highSalingStock = (from sd in db.StoreItemSolds.Where(u => u.DateSold >= refDate1 && u.DateSold <= refDate).OrderByDescending(r => r.QuantitySold).Take(5).Include("StoreItemStock")
                                           join stt in db.StoreItems.Include("StoreItemBrand") on sd.StoreItemStock.StoreItemId equals stt.StoreItemId select 
                                           new StoreItemSoldObject
                                           {
                                               BrandName = stt.StoreItemBrand.Name,
                                               StoreItemStockId = sd.StoreItemStockId,
                                               QuantitySold = sd.QuantitySold,
                                               StoreItemName = stt.Name,
                                               DateSold = sd.DateSold

                                           }).ToList();

                    if (highSalingStock.Any())
                    {
                        highSalingStock.ForEach(o =>
                        {
                            var exis = rep.HighSalingStockList.Find(y => y.StoreItemStockId == o.StoreItemStockId);
                            if (exis != null && exis.StoreItemStockId > 0)
                            {
                                exis.QuantitySold += o.QuantitySold;
                            }
                            else
                            {
                                rep.HighSalingStockList.Add(o);
                            }
                        });
                    }

                    if (dailySales.Any())
                    {
                        double t0 = 0;
                        dailySales.ForEach(t =>
                        {
                            rep.TotalDailySales += t.SalePayments.Sum(c => c.AmountPaid);
                            t0 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t0,
                            DateStr = dailySales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (dailyRefunds.Any())
                    {
                        dailyRefunds.ForEach(t =>
                        {
                            rep.TotalDailyRefunds += t.NetAmount;
                        });
                    }

                    if (day1Sales.Any())
                    {
                        double t1 = 0;
                        day1Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day1Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day2Sales.Any())
                    {
                        double t1 = 0;
                        day2Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day2Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day3Sales.Any())
                    {
                        double t1 = 0;
                        day3Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day3Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day4Sales.Any())
                    {
                        double t1 = 0;
                        day4Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day4Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day5Sales.Any())
                    {
                        double t1 = 0;
                        day5Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day5Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day6Sales.Any())
                    {
                        double t1 = 0;
                        day6Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day6Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    return rep;

                }
            }
            catch (Exception ex)
            {
                return new DailySaleReport();
            }
        }

        public DailySaleReport GetMarketerReportSnapshots()
        {
            try
            {
                var rep = new DailySaleReport
                {
                    CustomerPerformances = new List<CustomerInvoiceObject>(),
                    SaleComparism = new List<SaleComparism>()
                };

                using (var db = _db)
                {
                    var today = DateTime.Today;
                    var day1 = DateTime.Today.AddDays(-1);
                    var day2 = DateTime.Today.AddDays(-2);
                    var day3 = DateTime.Today.AddDays(-3);
                    var day4 = DateTime.Today.AddDays(-4);
                    var day5 = DateTime.Today.AddDays(-5);
                    var day6 = DateTime.Today.AddDays(-6);
                    
                    var customerprformances = (from cu in db.CustomerInvoices.OrderByDescending(r => r.TotalAmountPaid).Take(10).Include("Customer")
                                               join ps in db.UserProfiles on cu.Customer.UserId equals ps.Id
                                               select new CustomerInvoiceObject
                                               {
                                                   CustomerName = ps.LastName + " " + ps.OtherNames + "(" + ps.MobileNumber + ")",
                                                   Id = cu.Id,
                                                   CustomerId = cu.CustomerId,
                                                   TotalAmountDue = cu.TotalAmountDue,
                                                   InvoiceBalance = cu.InvoiceBalance,
                                                   TotalVATAmount = cu.TotalVATAmount,
                                                   TotalDiscountAmount = cu.TotalDiscountAmount,
                                                   TotalAmountPaid = cu.TotalAmountPaid

                                               }).ToList();

                    rep.CustomerPerformances = customerprformances;

                    rep.DailySalesCount = db.Sales.Count(a => a.Date >= today);
                    var day1Sales = db.Sales.Where(a => a.Date >= day1 && a.Date < today).Include("SalePayments").ToList();
                    var day2Sales = db.Sales.Where(a => a.Date >= day2 && a.Date < day1).Include("SalePayments").ToList();
                    var day3Sales = db.Sales.Where(a => a.Date >= day3 && a.Date < day2).Include("SalePayments").ToList();
                    var day4Sales = db.Sales.Where(a => a.Date >= day4 && a.Date < day3).Include("SalePayments").ToList();
                    var day5Sales = db.Sales.Where(a => a.Date >= day5 && a.Date < day4).Include("SalePayments").ToList();
                    var day6Sales = db.Sales.Where(a => a.Date >= day6 && a.Date < day5).Include("SalePayments").ToList();

                    if (day1Sales.Any())
                    {
                        double t1 = 0;
                        day1Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day1Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day2Sales.Any())
                    {
                        double t1 = 0;
                        day2Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day2Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day3Sales.Any())
                    {
                        double t1 = 0;
                        day3Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day3Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day4Sales.Any())
                    {
                        double t1 = 0;
                        day4Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day4Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day5Sales.Any())
                    {
                        double t1 = 0;
                        day5Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day5Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    if (day6Sales.Any())
                    {
                        double t1 = 0;
                        day6Sales.ForEach(t =>
                        {
                            t1 += t.SalePayments.Sum(c => c.AmountPaid);
                        });

                        rep.SaleComparism.Add(new SaleComparism
                        {
                            SaleMagnitude = t1,
                            DateStr = day6Sales[0].Date.ToString("dd/MM/yyyy")
                        });
                    }

                    return rep;

                }
            }
            catch (Exception ex)
            {
                return new DailySaleReport();
            }
        }

        public List<StoreItemCategoryObject> GetStockReport(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var services = 3;
                        var catObjects = new List<StoreItemCategoryObject>();
                        var categories = db.StoreItemCategories.Where(o => o.StoreItemCategoryId != services).ToList();
                        if (!categories.Any())
                        {
                            return new List<StoreItemCategoryObject>();
                        }
                        categories.ForEach(c =>
                        {
                            var catObject = ModelCrossMapper.Map<StoreItemCategory, StoreItemCategoryObject>(c);
                            if (catObject == null || catObject.StoreItemCategoryId < 1)
                            {
                                return;
                            }

                            catObject.StockItems = new List<StoreItemStockObject>();

                            catObject.StockItems = 
                                (from sti in db.StoreItemStocks.OrderBy(m => m.StoreItem.Name).Skip((tpageNumber) * tsize).Take(tsize)
                                join sto in db.StoreItems.Where(s => s.StoreItemCategoryId == catObject.StoreItemCategoryId) on sti.StoreItemId equals sto.StoreItemId
                                join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                
                                 select new StoreItemStockObject
                                {
                                    StoreItemStockId = sti.StoreItemStockId,
                                    Description = sto.Description,
                                    SKU = sti.SKU,
                                    CostPrice = sti.CostPrice,
                                    StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                    QuantityInStock = sti.QuantityInStock,
                                    TotalQuantityAlreadySold = sti.TotalQuantityAlreadySold,
                                    CategoryName = sto.StoreItemCategory.Name

                                }).ToList();

                            if (!catObject.StockItems.Any())
                            {
                                return;
                            }

                            catObject.StockItems.ForEach(n =>
                            {
                                if (n.CostPrice != null && n.CostPrice > 0)
                                {
                                    n.StockValue = n.QuantityInStock*(double) n.CostPrice;
                                    n.StockValueStr = n.StockValue.ToString("n2");
                                    n.CostPriceStr = ((double) n.CostPrice).ToString("n2");
                                }
                                else
                                {
                                    n.CostPrice = 0;
                                    n.StockValue = 0;
                                    n.StockValueStr = "0.00";
                                }
                                n.QuantityInStockStr = n.QuantityInStock.ToString("n2");
                               
                            });
                            catObjects.Add(catObject);
                        });

                        return catObjects;

                    }

                }

                return new List<StoreItemCategoryObject>();
            }
            catch (Exception ex)
            {
                return new List<StoreItemCategoryObject>();
            }
        }

        public List<StoreItemSoldObject> GetAllProductActivity(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.DateSold <= endDate && d.DateSold >= startDate).Include("Sale")
                                         join sti in db.StoreItemStocks.OrderBy(m => m.StoreItem.Name).Skip((tpageNumber) * tsize).Take(tsize) on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                         
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemSoldId = sts.StoreItemSoldId,
                                             StoreItemStockId = sti.StoreItemStockId,
                                             InvoiceNumber = sts.Sale.InvoiceNumber,
                                             Description = sto.Description,
                                             Sku = sti.SKU,
                                             Rate = sts.Rate,
                                             StoreItemName = sto.Name,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             QuantityLeft = sti.QuantityInStock,
                                             DateSold = sts.DateSold,
                                             QuantityAlreadySold = sti.TotalQuantityAlreadySold

                                         }).ToList();
                        
                        if (soldItems.Any())
                        {
                            soldItems.ForEach(n =>
                            {
                                n.QuantityBought = 0;
                                n.AmountBought = 0;
                                n.DateSoldStr = n.DateSold.ToString("dd/MM/yyyy");
                                n.DateBoughtStr = "";
                                n.QuantityLeftStr = n.QuantityLeft.ToString("n0");
                                n.QuantitySoldStr = n.QuantitySold.ToString("n0");
                                n.AmountSoldStr = n.AmountSold.ToString("n0");

                            });
                        }
                        
                        var purchaseOrders = db.PurchaseOrders.Where(s => s.StatusCode > 1 && s.PurchaseOrderPayments.Any() && (s.ActualDeliveryDate != null && s.ActualDeliveryDate <= endDate && s.ActualDeliveryDate >= startDate)).OrderByDescending(m => m.PurchaseOrderId).Skip((tpageNumber) * tsize).Take(tsize).Include("PurchaseOrderItems").ToList();

                        if (purchaseOrders.Any())
                        {
                            purchaseOrders.ForEach(n =>
                            {
                                var purchaseOrderItems = n.PurchaseOrderItems.ToList(); 
                                purchaseOrderItems.ForEach(p =>
                                {
                                    var storeItems = (from t in db.StoreItemStocks.Where(k => k.StoreItemStockId == p.StoreItemStockId).Include("StoreItem")
                                            join dd in db.PurchaseOrderItemDeliveries on p.PurchaseOrderItemId equals dd.PurchaseOrderItemId

                                            select new StoreItemSoldObject
                                            {
                                                PurchaseOrderId = n.PurchaseOrderId,
                                                StoreItemStockId = p.StoreItemStockId,
                                                InvoiceNumber = n.PurchaseOrderNumber,
                                                Description = t.StoreItem.Description,
                                                Sku = t.SKU,
                                                StoreItemName = t.StoreItem.Name,
                                                QuantityLeft = t.QuantityInStock,
                                                QuantityOrdered = p.QuantityOrdered,
                                                QuantityDelivered = dd.QuantityDelivered,
                                                DateDelivered = dd.DateDelivered,
                                                QuantityInStock = t.QuantityInStock,

                                            }).ToList();
 
                                    if (storeItems.Any())
                                    {
                                        storeItems.ForEach(g =>
                                        {
                                            g.DateBoughtStr = g.DateDelivered.ToString("dd/MM/yyyy");
                                            g.QuantityBoughtStr = p.QuantityDelivered.ToString("n0");
                                            g.AmountBoughtStr = g.AmountSold.ToString("n0");
                                            g.QuantityLeftStr = g.QuantityLeft.ToString("n0");
                                            soldItems.Add(g);
                                        });
                                    }
                                    
                                });
                            });
                        }
                        
                        return soldItems;

                    }

                }

                return new List<StoreItemSoldObject>();
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }
        
        public List<SaleObject> GetEmployeeSalesReport(int? itemsPerPage, int? pageNumber, out int countG, long employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.EmployeeId == employeeId && (s.Date <= endDate && s.Date >= startDate)).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                                join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                                select new SaleObject
                                                {
                                                    SaleId = sa.SaleId,
                                                    RegisterId = sa.RegisterId,
                                                    AmountDue = sa.AmountDue,
                                                    VATAmount = sa.VATAmount,
                                                    NetAmount = sa.NetAmount,
                                                    InvoiceNumber = sa.InvoiceNumber,
                                                    Discount = sa.Discount,
                                                    DiscountAmount = sa.DiscountAmount,
                                                    Status = sa.Status,
                                                    CustomerId = sa.CustomerId,
                                                    Date = sa.Date,
                                                    StoreTransactionId = stt.StoreTransactionId

                                                }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames + "(" + ps.MobileNumber + ")" : "N/A",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });

                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });
                       
                        countG = db.Sales.Count(s => s.EmployeeId == employeeId && (s.Date <= endDate && s.Date >= startDate));
                        return sales;
                       
                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetEmployeeSalesReport2(int? itemsPerPage, int? pageNumber, long employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.EmployeeId == employeeId && (s.Date <= endDate && s.Date >= startDate)).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames + "(" + ps.MobileNumber + ")" : "N/A",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });

                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        return sales;

                    }

                }
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetEmployeeSalesReport3(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => (s.Date <= endDate && s.Date >= startDate)).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames + "(" + ps.MobileNumber + ")" : "N/A",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });

                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        return sales.OrderByDescending(m => m.SaleId).ToList();

                    }

                }
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetDailySales(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    var today = DateTime.Today;
                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => (s.Date >= today)).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });

                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        return sales;

                    }

                }
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetCustomerSalesReport(int? itemsPerPage, int? pageNumber, out int countG, long customerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.CustomerId == customerId && (s.Date <= endDate && s.Date >= startDate)).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        countG = db.Sales.Count(s => s.CustomerId == customerId && (s.Date <= endDate && s.Date >= startDate));
                        return sales;
                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }

        public CustomerStatement GetCustomerStatements(int itemsPerPage, int pageNumber, long customerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var customerInvoice = db.CustomerInvoices.Where(c => c.CustomerId == customerId).ToList();
                    if (!customerInvoice.Any())
                    {
                        return new CustomerStatement();
                    }

                    var sales = (from sa in db.Sales.Where(s => s.Date <= endDate && s.Date >= startDate && s.CustomerId == customerId).OrderByDescending(m => m.SaleId).Skip((pageNumber) * itemsPerPage).Take(itemsPerPage)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 join rg in db.Registers.Include("StoreOutlet") on sa.RegisterId equals rg.RegisterId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     OutletName = rg.StoreOutlet.OutletName,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     CustomerId = sa.CustomerId,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new CustomerStatement();
                    }

                    double totalNet = 0;
                    double totalPaid = 0;

                    for(int i = 0; i < sales.Count(); i++)
                    {
                        var s = sales[i];
                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            continue;
                        }

                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });

                        s.AmountPaid = amountPaid;
                        s.Balance = s.NetAmount - s.AmountPaid;

                        if (i > 0)
                        {
                            //balance brought forward
                            s.Balance += sales[i - 1].Balance;
                        }

                        totalNet += s.NetAmount;
                        totalPaid += amountPaid;

                        s.BalanceStr = s.Balance.ToString("n0");
                        s.AmountPaidStr = amountPaid.ToString("n0"); 
                        s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }
                    }

                    return new CustomerStatement
                    {
                        BalanceBroughtForward = customerInvoice[0].InvoiceBalance,
                        TotalNet = totalNet,
                        TotalPaid = totalPaid,
                        StatemenList = sales
                    };
                }

            }
            catch (Exception ex)
            {
                return new CustomerStatement();
            }
        }

        public CustomerStatement GetAllCustomerStatements(int itemsPerPage, int pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.Date <= endDate && s.Date >= startDate).OrderByDescending(m => m.SaleId).Skip((pageNumber) * itemsPerPage).Take(itemsPerPage)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     join rg in db.Registers.Include("StoreOutlet") on sa.RegisterId equals rg.RegisterId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         OutletName = rg.StoreOutlet.OutletName,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new CustomerStatement();
                        }

                        double balance = 0;

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });

                            s.AmountPaid = amountPaid;
                            s.Balance = s.NetAmount - s.AmountPaid;

                            balance += s.Balance;

                            s.BalanceStr = s.Balance.ToString("n0");
                            s.AmountPaidStr = amountPaid.ToString("n0"); s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        return new CustomerStatement
                        {
                            BalanceBroughtForward = balance, 
                            StatemenList = sales
                        };
                    }

            }
            catch (Exception ex)
            {
                return new CustomerStatement();
            }
        }
        
        public List<SaleObject> GetAllCustomersSalesReport(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.Date >= startDate && s.Date <= endDate).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     join rg in db.Registers.Include("StoreOutlet") on sa.RegisterId equals rg.RegisterId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         OutletName = rg.StoreOutlet.OutletName,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        return sales;
                    }

                }
               
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetSingleCustomerSalesReport(int? itemsPerPage, int? pageNumber, long customerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.CustomerId == customerId && (s.Date <= endDate && s.Date >= startDate)).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     join rg in db.Registers.Include("StoreOutlet") on sa.RegisterId equals rg.RegisterId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         OutletName = rg.StoreOutlet.OutletName,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        return sales;
                    }

                }

                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }

        public List<CustomerInvoiceObject> GetCustomerInvoiceReports(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var customerInvoice = (from sa in db.CustomerInvoices.OrderBy(m => m.CustomerId).Skip((tpageNumber) * tsize).Take(tsize)
                                               join cs in db.Customers.Where(s => s.DateProfiled <= endDate && s.DateProfiled >= startDate).Include("UserProfile") on sa.CustomerId equals cs.CustomerId
                                               
                                     select new CustomerInvoiceObject
                                     {
                                         Id = sa.Id,
                                         CustomerId = sa.CustomerId,
                                         TotalAmountPaid = sa.TotalAmountPaid,
                                         TotalVATAmount = sa.TotalVATAmount,
                                         TotalDiscountAmount = sa.TotalDiscountAmount,
                                         TotalAmountDue = sa.TotalAmountDue,
                                         InvoiceBalance = sa.InvoiceBalance,
                                         DateProfiled = cs.DateProfiled,
                                         CustomerName = cs != null? cs.UserProfile.LastName + " " + cs.UserProfile.OtherNames : "N/A"
                                     }).ToList();

                        if (!customerInvoice.Any())
                        {
                            countG = 0;
                            return new List<CustomerInvoiceObject>();
                        }

                        customerInvoice.ForEach(s =>
                        {
                            s.TotalAmountPaidStr = s.TotalAmountPaid.ToString("n0");
                            s.TotalVATAmountStr = s.TotalVATAmount.ToString("n0");
                            s.TotalDiscountAmountStr = s.TotalDiscountAmount.ToString("n0"); 
                            s.TotalAmountDueStr = s.TotalAmountDue.ToString("n0");
                            s.InvoiceBalanceStr = s.InvoiceBalance.ToString("n0");
                            s.DateProfiledStr = s.DateProfiled != null? ((DateTime)s.DateProfiled).ToString("dd/MM/yyyy") : "Not Available";
                        });

                        countG = (from sa in db.CustomerInvoices.OrderBy(m => m.CustomerId).Skip((tpageNumber) * tsize).Take(tsize)
                                               join cs in db.Customers.Where(s => s.DateProfiled <= endDate && s.DateProfiled >= startDate) on sa.CustomerId equals cs.CustomerId select sa).Count();
                        return customerInvoice;

                    } 

                }
                countG = 0;
                return new List<CustomerInvoiceObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<CustomerInvoiceObject>();
            }
        }
        
        public List<CustomerInvoiceObject> GetSingleCustomerInvoiceStatements(int? itemsPerPage, int? pageNumber, long customerId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var customerInvoice = (from sa in db.CustomerInvoices.Where(c => c.CustomerId == customerId).OrderBy(m => m.CustomerId).Skip((tpageNumber) * tsize).Take(tsize)
                                               join cs in db.Customers.Include("UserProfile") on sa.CustomerId equals cs.CustomerId
                                               join st in db.StoreOutlets on cs.StoreOutletId equals st.StoreOutletId
                                               select new CustomerInvoiceObject
                                               {
                                                   Id = sa.Id,
                                                   CustomerId = sa.CustomerId,
                                                   TotalAmountPaid = sa.TotalAmountPaid,
                                                   OutletName = st.OutletName,
                                                   TotalVATAmount = sa.TotalVATAmount,
                                                   TotalDiscountAmount = sa.TotalDiscountAmount,
                                                   TotalAmountDue = sa.TotalAmountDue,
                                                   InvoiceBalance = sa.InvoiceBalance,
                                                   DateProfiled = cs.DateProfiled,
                                                   CustomerName = cs != null ? cs.UserProfile.LastName + " " + cs.UserProfile.OtherNames : "N/A"
                                               }).ToList();

                        if (!customerInvoice.Any())
                        {
                            return new List<CustomerInvoiceObject>();
                        }

                        customerInvoice.ForEach(s =>
                        {
                            s.TotalAmountPaidStr = s.TotalAmountPaid.ToString("n0");
                            s.TotalVATAmountStr = s.TotalVATAmount.ToString("n0");
                            s.TotalDiscountAmountStr = s.TotalDiscountAmount.ToString("n0");
                            s.TotalAmountDueStr = s.TotalAmountDue.ToString("n0");
                            s.InvoiceBalanceStr = s.InvoiceBalance.ToString("n0");
                            s.DateProfiledStr = s.DateProfiled != null ? ((DateTime)s.DateProfiled).ToString("dd/MM/yyyy") : "Not Available";
                        });

                        return customerInvoice;

                    }

                }
               
                return new List<CustomerInvoiceObject>();
            }
            catch (Exception ex)
            {
                return new List<CustomerInvoiceObject>();
            }
        }

        public List<CustomerInvoiceObject> GetAllCustomerInvoiceStatements(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var customerInvoice = (from sa in db.CustomerInvoices.OrderBy(m => m.CustomerId).Skip((tpageNumber) * tsize).Take(tsize)
                                               join cs in db.Customers.Include("UserProfile") on sa.CustomerId equals cs.CustomerId
                                               join st in db.StoreOutlets on cs.StoreOutletId equals st.StoreOutletId
                                               select new CustomerInvoiceObject
                                               {
                                                   Id = sa.Id,
                                                   CustomerId = sa.CustomerId,
                                                   TotalAmountPaid = sa.TotalAmountPaid,
                                                   OutletName = st.OutletName,
                                                   TotalVATAmount = sa.TotalVATAmount,
                                                   TotalDiscountAmount = sa.TotalDiscountAmount,
                                                   TotalAmountDue = sa.TotalAmountDue,
                                                   InvoiceBalance = sa.InvoiceBalance,
                                                   DateProfiled = cs.DateProfiled,
                                                   CustomerName = cs != null ? cs.UserProfile.LastName + " " + cs.UserProfile.OtherNames : "N/A"
                                               }).ToList();

                        if (!customerInvoice.Any())
                        {
                            return new List<CustomerInvoiceObject>();
                        }

                        customerInvoice.ForEach(s =>
                        {
                            s.TotalAmountPaidStr = s.TotalAmountPaid.ToString("n0");
                            s.TotalVATAmountStr = s.TotalVATAmount.ToString("n0");
                            s.TotalDiscountAmountStr = s.TotalDiscountAmount.ToString("n0");
                            s.TotalAmountDueStr = s.TotalAmountDue.ToString("n0");
                            s.InvoiceBalanceStr = s.InvoiceBalance.ToString("n0");
                            s.DateProfiledStr = s.DateProfiled != null ? ((DateTime)s.DateProfiled).ToString("dd/MM/yyyy") : "Not Available";
                        });

                        return customerInvoice;

                    }

                }

                return new List<CustomerInvoiceObject>();
            }
            catch (Exception ex)
            {
                return new List<CustomerInvoiceObject>();
            }
        }

        public List<SupplierInvoiceObject> GetAllSupplierStatements(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    //todo: to be modified with purchase order payment
                    
                    //var pending = (int)PurchaseOrderDeliveryStatus.Pending;
                    //s => s.StatusCode > pending && s.PurchaseOrderPayments.Any()

                    using (var db = _db)   
                    {
                        var lpos = db.PurchaseOrders.OrderByDescending(m => m.PurchaseOrderId).Skip((tpageNumber) * tsize).Take(tsize).Include("StoreOutlet").Include("Supplier").Include("PurchaseOrderPayments").ToList();

                        if (!lpos.Any())
                        {
                            return new List<SupplierInvoiceObject>();
                        }

                        var filteredLpos = new List<SupplierInvoiceObject>();
                        
                        lpos.ForEach(s =>
                        {
                            var lpoPayments = s.PurchaseOrderPayments.ToList();
                            var payment = lpoPayments.Sum(o => o.AmountPaid);
                            var tt = payment ?? 0;

                            var tc = s.DerivedTotalCost ?? 0;
                            var net = (tc + s.VATAmount) - s.DiscountAmount;
                            var balance = net - tt;
                            var statement = new SupplierInvoiceObject();
                            if (!filteredLpos.Exists(l => l.SupplierId == s.SupplierId))
                            {
                                var newSupplier = new SupplierInvoiceObject
                                {
                                    SupplierId = s.SupplierId,
                                    TotalAmountPaid = tt,
                                    TotalAmountDue = net,
                                    InvoiceBalance = balance,
                                    TotalVATAmount = s.VATAmount,
                                    TotalDiscountAmount = s.DiscountAmount,
                                    DateJoined = s.Supplier.DateJoined,
                                    SupplierName = s.Supplier.CompanyName,
                                    DateJoinedStr = s.Supplier.DateJoined.ToString("dd/MM/yyyy")
                                };
                                statement = newSupplier;
                            }
                            else
                            {
                                var existing = filteredLpos.Find(l => l.SupplierId == s.SupplierId);
                                if (existing != null && existing.SupplierId > 0)
                                {
                                    existing.TotalAmountPaid += tt;
                                    existing.TotalAmountDue += net;
                                    existing.TotalVATAmount += s.VATAmount;
                                    existing.TotalDiscountAmount += s.DiscountAmount;
                                    existing.InvoiceBalance += balance;
                                    statement = existing;
                                }
                            }

                            statement.TotalAmountPaidStr = statement.TotalAmountPaid.ToString("n0");
                            statement.AmountDueStr = statement.TotalAmountDue.ToString("n0");
                            statement.VATAmountStr = statement.TotalVATAmount.ToString("n0");
                            statement.BalanceStr = statement.InvoiceBalance.ToString("n0");
                            statement.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            statement.DateJoinedStr = statement.DateJoined.ToString("dd/MM/yyyy");
                            filteredLpos.Add(statement);
                        });

                        return filteredLpos;
                    }

                }

                return new List<SupplierInvoiceObject>();
            }
            catch (Exception ex)
            {
                return new List<SupplierInvoiceObject>();
            }
        }

        public List<SupplierInvoiceObject> GetSingleSupplierStatements(int? itemsPerPage, int? pageNumber, int supplierId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    var pending = (int)PurchaseOrderDeliveryStatus.Pending;
                    using (var db = _db)
                    {
                        var lpos = db.PurchaseOrders.Where(s => s.SupplierId == supplierId).OrderByDescending(m => m.PurchaseOrderId).Skip((tpageNumber) * tsize).Take(tsize).Include("StoreOutlet").Include("Supplier").Include("PurchaseOrderPayments").ToList();

                        if (!lpos.Any())
                        {
                            return new List<SupplierInvoiceObject>();
                        }

                        var filteredLpos = new List<SupplierInvoiceObject>();

                        lpos.ForEach(s =>
                        {
                            var lpoPayments = s.PurchaseOrderPayments.ToList();
                            var payment = lpoPayments.Sum(o => o.AmountPaid);
                            var tt = payment ?? 0;

                            var tc = s.DerivedTotalCost ?? 0;
                            var net = (tc + s.VATAmount) - s.DiscountAmount;
                            var balance = net - tt;
                            var statement = new SupplierInvoiceObject();
                            if (!filteredLpos.Exists(l => l.SupplierId == s.SupplierId))
                            {
                                var newSupplier = new SupplierInvoiceObject
                                {
                                    SupplierId = s.SupplierId,
                                    TotalAmountPaid = tt,
                                    TotalAmountDue = net,
                                    InvoiceBalance = balance,
                                    TotalVATAmount = s.VATAmount,
                                    TotalDiscountAmount = s.DiscountAmount,
                                    DateJoined = s.Supplier.DateJoined,
                                    SupplierName = s.Supplier.CompanyName
                                };
                                statement = newSupplier;
                            }
                            else
                            {
                                var existing = filteredLpos.Find(l => l.SupplierId == s.SupplierId);
                                if (existing != null && existing.SupplierId > 0)
                                {
                                    existing.TotalAmountPaid += tt;
                                    existing.TotalAmountDue += net;
                                    existing.TotalVATAmount += s.VATAmount;
                                    existing.TotalDiscountAmount += s.DiscountAmount;
                                    existing.InvoiceBalance += balance;
                                    statement = existing;
                                }
                            }

                            statement.TotalAmountPaidStr = statement.TotalAmountPaid.ToString("n0");
                            statement.AmountDueStr = statement.TotalAmountDue.ToString("n0");
                            statement.VATAmountStr = statement.TotalVATAmount.ToString("n0");
                            statement.BalanceStr = statement.InvoiceBalance.ToString("n0");
                            statement.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            statement.DateJoinedStr = statement.DateJoined.ToString("dd/MM/yyyy");
                            filteredLpos.Add(statement);
                        });

                        return filteredLpos;
                    }

                }

                return new List<SupplierInvoiceObject>();
            }
            catch (Exception ex)
            {
                return new List<SupplierInvoiceObject>();
            }
        }

        public List<SaleObject> GetEmployeeSalesReport(int? itemsPerPage, int? pageNumber, out int countG, long employeeId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.EmployeeId == employeeId).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }
                        
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            } 

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }
                          
                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");
                            s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            
                        });

                        countG = db.Sales.Count(s => s.EmployeeId == employeeId);
                        return sales.OrderByDescending(m => m.SaleId).ToList();
                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetContactPersonInvoices(int? itemsPerPage, int? pageNumber, out int countG, long employeeId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.Customer.ContactPersonId == employeeId).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");
                            s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        });

                        countG = db.Sales.Count(s => s.EmployeeId == employeeId);
                        return sales.OrderBy(m => m.InvoiceNumber).ToList();

                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetAdminSalesReport(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");
                            s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        });

                        countG = db.Sales.Count();
                        return sales.OrderByDescending(m => m.SaleId).ToList();

                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }
        
        public List<SaleObject> GetUncompletedTransactions(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    const int uncompleted = (int) SaleStatus.Partly_Paid;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => s.Status == uncompleted).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });

                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = s.AmountPaid.ToString("n0");
                            s.Balance = s.NetAmount - s.AmountPaid;
                            s.BalanceStr = s.Balance.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        });

                        countG = db.Sales.Count(s => s.Status == uncompleted);
                        return sales;

                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetRevokedSales(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    var refunded = (int) SaleStatus.Refund_Note_Issued;
                    using (var db = _db)
                    {
                        var sales =  (from sa in db.Sales.Where(s => s.Status == refunded).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         CustomerId = sa.CustomerId,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }
                        
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var refunds = db.RefundNotes.Where(y => y.SaleId == s.SaleId).OrderByDescending(g => g.DateReturned).ToList();
                            if (refunds.Any())
                            {
                                s.DateRevokedStr = refunds[0].DateReturned.ToString("dd/MM/yyyy");
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }
                          
                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");
                            s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            
                        });

                        countG = db.Sales.Count(s => s.Status == refunded);
                        return sales;
                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetDueInvoices(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    var day2 = DateTime.Today.AddDays(-2);
                    const int refunded = (int)SaleStatus.Refund_Note_Issued;
                    
                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(a => a.Date <= day2 && a.Status != refunded).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                                join sp in db.SalePayments on sa.SaleId equals sp.SaleId
                                                where sa.NetAmount > sp.AmountPaid
                                               join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                               select new SaleObject
                                               {
                                                   SaleId = sa.SaleId,
                                                   RegisterId = sa.RegisterId,
                                                   AmountDue = sa.AmountDue,
                                                   VATAmount = sa.VATAmount,
                                                   NetAmount = sa.NetAmount,
                                                   InvoiceNumber = sa.InvoiceNumber,
                                                   Discount = sa.Discount,
                                                   DiscountAmount = sa.DiscountAmount,
                                                   Status = sa.Status,
                                                   CustomerId = sa.CustomerId,
                                                   Date = sa.Date,
                                                   StoreTransactionId = stt.StoreTransactionId

                                               }).ToList();


                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames + "("+ ps.MobileNumber +")" : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0"); s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        });

                        countG = (from sa in db.Sales.Where(a => a.Date <= day2 && a.Status != refunded).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                                join sp in db.SalePayments on sa.SaleId equals sp.SaleId
                                  where !sa.NetAmount.Equals(sp.AmountPaid)
                                  select sa).Count();
                        return sales;

                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }
        
        public List<StoreTransactionObject> GetSalesReportByPaymentType(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var transactions = (from tt in db.StoreTransactions.Where(s => s.TransactionDate <= endDate && s.TransactionDate >= startDate).OrderBy(m => m.TransactionDate).Skip((tpageNumber) * tsize).Take(tsize)
                                         .Include("StorePaymentMethod").Include("StoreOutlet").Include("StoreTransactionType")
                                     select new StoreTransactionObject
                                     {
                                         StoreTransactionId = tt.StoreTransactionId,
                                         StoreTransactionTypeId = tt.StoreTransactionTypeId,
                                         EffectedByEmployeeId = tt.EffectedByEmployeeId,
                                         StorePaymentMethodId = tt.StorePaymentMethodId,
                                         StoreOutletId = tt.StoreOutletId,
                                         TransactionAmount =  tt.TransactionAmount,
                                         TransactionDate = tt.TransactionDate,
                                         PaymentMethodName = tt.StorePaymentMethod.Name,
                                         StoreTransactionTypeName = tt.StoreTransactionType.Name,
                                         OutletName = tt.StoreOutlet.OutletName

                                     }).ToList();

                        if (!transactions.Any())
                        {
                            countG = 0;
                            return new List<StoreTransactionObject>();
                        }

                        transactions.ForEach(s =>
                        {
                            s.TransactionDateStr = s.TransactionDate.ToString("dd/MM/yyyy");
                            s.TransactionAmountStr = s.TransactionAmount.ToString("n0");
                        });

                        countG = db.StoreTransactions.Count(s => s.TransactionDate <= endDate && s.TransactionDate >= startDate);
                        return transactions;
                    }

                }
                countG = 0;
                return new List<StoreTransactionObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreTransactionObject>();
            }
        }

        public List<StorePaymentMethodObject> GetAllPaymentTypeReports(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int) pageNumber;
                    var tsize = (int) itemsPerPage;

                    using (var db = _db)
                    {
                        var pMethods = db.StorePaymentMethods.ToList();
                        if (!pMethods.Any())
                        {
                            return new List<StorePaymentMethodObject>();
                        }

                        var transactionList = new List<StorePaymentMethodObject>();

                        pMethods.ForEach(p =>
                        {
                            var paymentTypeObj = ModelCrossMapper.Map<StorePaymentMethod, StorePaymentMethodObject>(p);
                            if (paymentTypeObj == null || paymentTypeObj.StorePaymentMethodId < 1)
                            {
                                return;
                            }

                            var credit = (int) TransactionTypeEnum.Credit;
                            var debit = (int)TransactionTypeEnum.Debit;

                            var transactions = db.StoreTransactions.Where(s => s.TransactionDate <= endDate && s.TransactionDate >= startDate && s.StorePaymentMethodId == p.StorePaymentMethodId)
                                    .OrderBy(m => m.TransactionDate)
                                    .Skip((tpageNumber)*tsize)
                                    .Take(tsize)
                                    .Include("StorePaymentMethod")
                                    .Include("StoreOutlet")
                                    .Include("StoreTransactionType")
                                    .ToList();

                            if (!transactions.Any())
                            {
                                return;
                            }
                            
                            paymentTypeObj.TotalTransactions = transactions.Count();
                            paymentTypeObj.TotalTransactionValue = transactions.Where(e => e.StoreTransactionTypeId == credit).Sum(t => t.TransactionAmount);
                            paymentTypeObj.TotalRefundedAmount = transactions.Where(e => e.StoreTransactionTypeId == debit).Sum(t => t.TransactionAmount);
                            paymentTypeObj.TotalAvailableAmount = paymentTypeObj.TotalTransactionValue - paymentTypeObj.TotalRefundedAmount;
                            transactionList.Add(paymentTypeObj);
                        });

                        return transactionList;
                    }

                }
                return new List<StorePaymentMethodObject>();
            }
            catch (Exception ex)
            {
                return new List<StorePaymentMethodObject>();
            }
        }

        public List<StorePaymentMethodObject> GetSinglePaymentTypeReports(int? itemsPerPage, int? pageNumber, long paymentMethodTypeId, DateTime startDate, DateTime endDate) 
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var pMethods = db.StorePaymentMethods.Where(p => p.StorePaymentMethodId == paymentMethodTypeId).ToList();
                        if (!pMethods.Any())
                        {
                            return new List<StorePaymentMethodObject>();
                        }
                        var transactionList = new List<StorePaymentMethodObject>();

                        var y = pMethods[0];
                        var q = pMethods[0];
                        
                        var paymentTypeObj = ModelCrossMapper.Map<StorePaymentMethod, StorePaymentMethodObject>(y);
                        if (paymentTypeObj == null || paymentTypeObj.StorePaymentMethodId < 1)
                        {
                            return new List<StorePaymentMethodObject>();
                        }

                            var credit = (int)TransactionTypeEnum.Credit;
                            var debit = (int)TransactionTypeEnum.Debit;

                            var transactions = db.StoreTransactions.Where(s => s.TransactionDate <= endDate && s.TransactionDate >= startDate && s.StorePaymentMethodId == q.StorePaymentMethodId)
                                    .OrderBy(m => m.TransactionDate)
                                    .Skip((tpageNumber) * tsize)
                                    .Take(tsize)
                                    .Include("StorePaymentMethod")
                                    .Include("StoreOutlet")
                                    .Include("StoreTransactionType")
                                    .ToList();

                            if (!transactions.Any())
                            {
                                return new List<StorePaymentMethodObject>();
                            }

                            paymentTypeObj.TotalTransactions = transactions.Count();
                            paymentTypeObj.TotalTransactionValue += transactions.Where(e => e.StoreTransactionTypeId == credit).Sum(t => t.TransactionAmount);
                            paymentTypeObj.TotalRefundedAmount += transactions.Where(e => e.StoreTransactionTypeId == debit).Sum(t => t.TransactionAmount);
                            paymentTypeObj.TotalAvailableAmount = paymentTypeObj.TotalTransactionValue - paymentTypeObj.TotalRefundedAmount;
                            transactionList.Add(paymentTypeObj);

                           return transactionList;
                    }

                }

                return new List<StorePaymentMethodObject>();
            }
            catch (Exception ex)
            {
                return new List<StorePaymentMethodObject>();
            }
        }

        public List<SaleObject> GetSalesReportByOutlet(int? itemsPerPage, int? pageNumber, out int countG, int outletId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from ot in db.StoreOutlets.Where(s => s.StoreOutletId == outletId).OrderBy(m => m.OutletName).Skip((tpageNumber) * tsize).Take(tsize)
                                     join rs in db.Registers on ot.StoreOutletId equals rs.CurrentOutletId
                                     join sa in db.Sales.Where(s => s.Date <= endDate && s.Date >= startDate) on rs.RegisterId equals sa.RegisterId
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     join tt in db.StoreTransactions on stt.StoreTransactionId equals tt.StoreTransactionId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         CustomerId = sa.CustomerId,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        });

                        countG = (from ot in db.StoreOutlets.Where(s => s.StoreOutletId == outletId)
                                join rs in db.Registers on ot.StoreOutletId equals rs.CurrentOutletId
                                join sa in db.Sales.Where(s => s.Date <= endDate && s.Date >= startDate) on rs.RegisterId equals sa.RegisterId
                                select sa).Count();

                        return sales;

                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetAllOutletsSalesReports(int? itemsPerPage, int? pageNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from ot in db.StoreOutlets.OrderBy(m => m.OutletName).Skip((tpageNumber) * tsize).Take(tsize)
                                     join rs in db.Registers on ot.StoreOutletId equals rs.CurrentOutletId
                                     join sa in db.Sales.Where(s => s.Date <= endDate && s.Date >= startDate) on rs.RegisterId equals sa.RegisterId
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     join tt in db.StoreTransactions on stt.StoreTransactionId equals tt.StoreTransactionId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         CustomerId = sa.CustomerId,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new List<SaleObject>();
                        }

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        });

                        return sales;

                    }

                }
             
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetSingleOutletSalesReports(int? itemsPerPage, int? pageNumber, int outletId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from ot in db.StoreOutlets.Where(s => s.StoreOutletId == outletId).OrderBy(m => m.OutletName).Skip((tpageNumber) * tsize).Take(tsize)
                                     join rs in db.Registers on ot.StoreOutletId equals rs.CurrentOutletId
                                     join sa in db.Sales.Where(s => s.Date <= endDate && s.Date >= startDate) on rs.RegisterId equals sa.RegisterId
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     join tt in db.StoreTransactions on stt.StoreTransactionId equals tt.StoreTransactionId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         CustomerId = sa.CustomerId,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            return new List<SaleObject>();
                        }

                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                        });
                        return sales;

                    }

                }
             
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }
        public List<SaleObject> GetDailySalesReport(int? itemsPerPage, int? pageNumber, out int countG, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var sales = (from sa in db.Sales.Where(s => (s.Date <= endDate && s.Date >= startDate)).OrderByDescending(m => m.SaleId).Skip((tpageNumber) * tsize).Take(tsize)
                                     join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                     select new SaleObject
                                     {
                                         SaleId = sa.SaleId,
                                         RegisterId = sa.RegisterId,
                                         AmountDue = sa.AmountDue,
                                         VATAmount = sa.VATAmount,
                                         NetAmount = sa.NetAmount,
                                         CustomerId = sa.CustomerId,
                                         InvoiceNumber = sa.InvoiceNumber,
                                         Discount = sa.Discount,
                                         DiscountAmount = sa.DiscountAmount,
                                         Status = sa.Status,
                                         Date = sa.Date,
                                         StoreTransactionId = stt.StoreTransactionId

                                     }).ToList();

                        if (!sales.Any())
                        {
                            countG = 0;
                            return new List<SaleObject>();
                        }
                        sales.ForEach(s =>
                        {
                            if (s.CustomerId != null && s.CustomerId > 0)
                            {
                                var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                                 join ps in db.UserProfiles on cu.UserId equals ps.Id
                                                 select new CustomerObject
                                                 {
                                                     UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                     CustomerId = cu.CustomerId,
                                                     UserId = ps.Id
                                                 }).ToList();

                                if (customers.Any())
                                {
                                    var customer = customers[0];
                                    s.CustomerName = customer.UserProfileName;
                                }
                                else
                                {
                                    s.CustomerName = "N/A";
                                }
                            }

                            var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                            if (!transactions.Any())
                            {
                                return;
                            }

                            var amountPaid = 0.0;
                            transactions.ForEach(n =>
                            {
                                amountPaid += n.TransactionAmount;
                            });
                            s.AmountPaid = amountPaid;
                            s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                            s.DateStr = s.Date.ToString("dd/MM/yyyy");
                            s.VATAmountStr = s.VATAmount.ToString("n0");
                            s.NetAmountStr = s.NetAmount.ToString("n0");
                            s.AmountDueStr = s.AmountDue.ToString("n0");
                            s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                            s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        });

                        countG = db.Sales.Count(s => (s.Date <= endDate && s.Date >= startDate));
                        return sales;

                    }

                }
                countG = 0;
                return new List<SaleObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<SaleObject>();
            }
        }
        public SaleObject GetSalesReportDetails(long saleId)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(l => l.SaleId == saleId)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                 join ps in db.UserProfiles on em.UserId equals ps.Id
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     CustomerId = sa.CustomerId,
                                     NetAmount = sa.NetAmount,
                                     VAT = sa.VAT,
                                     SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                     Discount = sa.Discount,
                                     VATAmount = sa.VATAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId
                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new SaleObject();
                    }
                    
                    var s = sales[0];

                    var customerNames = (from cu in db.Customers.Where(c => c.CustomerId == s.CustomerId)
                                         join sp in db.UserProfiles on cu.UserId equals sp.Id
                                         select sp.LastName + " " + sp.OtherNames).ToList();
                    if (customerNames.Any())
                    {
                        s.CustomerName = customerNames[0];
                    }
                    else
                    {
                        s.CustomerName = "N/A";
                    }

                    s.StoreItemSoldObjects = new List<StoreItemSoldObject>();
                    s.EmployeeObject = new EmployeeObject();
                    s.SalePaymentObjects = new List<SalePaymentObject>();
                    s.Transactions = new List<StoreTransactionObject>();

                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             Rate = sts.Rate
                                         }).ToList();

                        if (!soldItems.Any())
                        {
                            return new SaleObject();
                        }

                     s.StoreItemSoldObjects = soldItems;
                    
                    var transactions = (from t in db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).Include("StorePaymentMethod")
                                            join em in db.Employees on t.EffectedByEmployeeId equals em.EmployeeId
                                            join ps in db.UserProfiles on em.UserId equals ps.Id
                                            join ot in db.StoreOutlets on t.StoreOutletId equals ot.StoreOutletId
                                            select new StoreTransactionObject
                                            {
                                                StoreTransactionId = t.StoreTransactionId,
                                                StoreTransactionTypeId = t.StoreTransactionTypeId,
                                                StorePaymentMethodId = t.StorePaymentMethodId,
                                                EffectedByEmployeeId = t.EffectedByEmployeeId,
                                                TransactionAmount = t.TransactionAmount,
                                                PaymentMethodName = t.StorePaymentMethod.Name,
                                                TransactionDate = t.TransactionDate,
                                                Remark = t.Remark,
                                                StoreOutletId = t.StoreOutletId,
                                                TransactionEmployeeName = ps.LastName + " " + ps.OtherNames,
                                                OutletName = ot.OutletName

                                            }).ToList();

                        if (!transactions.Any())
                        {
                            return new SaleObject();
                        }

                    s.OutletName = transactions[0].OutletName;
                    s.Transactions = transactions;

                    var amountPaid = 0.0;
                    transactions.ForEach(n =>
                    {
                        amountPaid += n.TransactionAmount;
                    });

                    s.AmountPaid = amountPaid;
                    s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    s.DateStr = s.Date.ToString("dd/MM/yyyy hh:mm tt");
                    s.Balance = s.NetAmount - s.AmountPaid;
                    return s;

                }
            }
            catch (Exception ex)
            {
                return new SaleObject();
            }
        }

        public SaleObject GetSalesReportDetails(string invoiceNumber)
        {
            try
            {
                using (var db = _db)
                {
                    var completed = (int) SaleStatus.Completed;
                    var partlyPaid = (int)SaleStatus.Partly_Paid;
                    var sales = (from sa in db.Sales.Where(l => l.InvoiceNumber == invoiceNumber.Trim() && l.Status == completed || l.Status == partlyPaid)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                 join ps in db.UserProfiles on em.UserId equals ps.Id
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     CustomerId = sa.CustomerId,
                                     NetAmount = sa.NetAmount,
                                     VAT = sa.VAT,
                                     SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     VATAmount = sa.VATAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId
                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new SaleObject();
                    }

                    var s = sales[0];

                    var customerNames = (from cu in db.Customers.Where(c => c.CustomerId == s.CustomerId)
                                         join sp in db.UserProfiles on cu.UserId equals sp.Id
                                         select sp.LastName + " " + sp.OtherNames).ToList();
                    if (customerNames.Any())
                    {
                        s.CustomerName = customerNames[0];
                    }
                    else
                    {
                        s.CustomerName = "N/A";
                    }

                    s.StoreItemSoldObjects = new List<StoreItemSoldObject>();
                    s.EmployeeObject = new EmployeeObject();
                    s.SalePaymentObjects = new List<SalePaymentObject>();
                    s.Transactions = new List<StoreTransactionObject>();

                    var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                     join sti in db.StoreItemStocks.Include("StoreItemVariationValue") on sts.StoreItemStockId equals sti.StoreItemStockId
                                     join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                     join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemName = sti.StoreItemVariationValue == null ? sto.Name : sto.Name + "/" + sti.StoreItemVariationValue.Value,
                                         QuantitySold = sts.QuantitySold,
                                         Sku = sti.SKU,
                                         AmountSold = sts.AmountSold,
                                         StoreItemVariationValueId = sti.StoreItemVariationValueId,
                                         StoreItemStockId = sts.StoreItemStockId,
                                         UoMCode = ui.UoMCode,
                                         Rate = sts.Rate
                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new SaleObject();
                    }

                    soldItems.ForEach(sts =>
                    {

                        var images = db.StockUploads.Where(i => i.StoreItemStockId == sts.StoreItemStockId).Include("ImageView").ToList();
                        if (images.Any())
                        {
                            var frontView = images.Find(o => o.ImageView.Name.ToLower() == "front view");
                            if (frontView == null || frontView.StockUploadId < 1)
                            {
                                frontView = images[0];
                            }

                            sts.ImagePath = frontView.ImagePath;
                        }

                        sts.ImagePath = string.IsNullOrEmpty(sts.ImagePath) ? "/Content/images/noImage.png" : PhysicalToVirtualPathMapper.MapPath(sts.ImagePath);

                        sts.ReturnedProductObjects = new List<ReturnedProductObject>();
                        var returnedItems = db.ReturnedProducts.Where(r => r.StoreItemStockId == sts.StoreItemStockId).ToList();
                        if (returnedItems.Any())
                        {
                            var rt = returnedItems[0];
                            var retObj = new ReturnedProductObject
                            {
                                ReturnedProductId = rt.ReturnedProductId,
                                IssueTypeId = rt.IssueTypeId,
                                StoreItemStockId = rt.StoreItemStockId,
                                RefundNoteId = rt.RefundNoteId,
                                DateReturned = rt.DateReturned,
                                AmountRefunded = rt.AmountRefunded,
                                QuantityBought = rt.QuantityBought,
                                QuantityReturned = rt.QuantityReturned
                            };
                            sts.QuantityBought = rt.QuantityBought;
                            sts.ReturnedQuantity += rt.QuantityReturned;
                            sts.ReturnedProductObjects.Add(retObj);
                        }

                        s.StoreItemSoldObjects.Add(sts);
                    });


                    s.StoreItemSoldObjects = soldItems;

                    var transactions = (from t in db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).Include("StorePaymentMethod")
                                        join em in db.Employees on t.EffectedByEmployeeId equals em.EmployeeId
                                        join ps in db.UserProfiles on em.UserId equals ps.Id
                                        join ot in db.StoreOutlets on t.StoreOutletId equals ot.StoreOutletId
                                        select new StoreTransactionObject
                                        {
                                            StoreTransactionId = t.StoreTransactionId,
                                            StoreTransactionTypeId = t.StoreTransactionTypeId,
                                            StorePaymentMethodId = t.StorePaymentMethodId,
                                            EffectedByEmployeeId = t.EffectedByEmployeeId,
                                            TransactionAmount = t.TransactionAmount,
                                            PaymentMethodName = t.StorePaymentMethod.Name,
                                            TransactionDate = t.TransactionDate,
                                            Remark = t.Remark,
                                            StoreOutletId = t.StoreOutletId,
                                            TransactionEmployeeName = ps.LastName + " " + ps.OtherNames,
                                            OutletName = ot.OutletName

                                        }).ToList();

                    if (!transactions.Any())
                    {
                        return new SaleObject();
                    }

                    s.OutletName = transactions[0].OutletName;
                    s.Transactions = transactions;

                    var amountPaid = 0.0;
                    transactions.ForEach(n =>
                    {
                        amountPaid += n.TransactionAmount;
                    });

                    s.AmountPaid = amountPaid;
                    s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    s.DateStr = s.Date.ToString("dd/MM/yyyy hh:mm tt");
                    s.Balance = s.NetAmount - s.AmountPaid;
                    return s;

                }
            }
            catch (Exception ex)
            {
                return new SaleObject();
            }
        }

        public SaleObject GetRefundedSale(long saleId)
        {
            try
            {
                using (var db = _db)
                {
                    var refunded = (int)SaleStatus.Refund_Note_Issued;
                    var sales = (from sa in db.Sales.Where(l => l.SaleId == saleId && l.Status == refunded)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 join em in db.Employees on sa.EmployeeId equals em.EmployeeId
                                 join ps in db.UserProfiles on em.UserId equals ps.Id
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     CustomerId = sa.CustomerId,
                                     NetAmount = sa.NetAmount,
                                     VAT = sa.VAT,
                                     SaleEmployeeName = ps.LastName + " " + ps.OtherNames,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     VATAmount = sa.VATAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId
                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new SaleObject();
                    }

                    var s = sales[0];

                    var customerNames = (from cu in db.Customers.Where(c => c.CustomerId == s.CustomerId)
                                         join sp in db.UserProfiles on cu.UserId equals sp.Id
                                         select sp.LastName + " " + sp.OtherNames).ToList();
                    if (customerNames.Any())
                    {
                        s.CustomerName = customerNames[0];
                    }
                    else
                    {
                        s.CustomerName = "N/A";
                    }

                    s.StoreItemSoldObjects = new List<StoreItemSoldObject>();
                    s.EmployeeObject = new EmployeeObject();
                    s.SalePaymentObjects = new List<SalePaymentObject>();
                    s.Transactions = new List<StoreTransactionObject>();


                    var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                     join sti in db.StoreItemStocks.Include("StoreItemVariationValue") on sts.StoreItemStockId equals sti.StoreItemStockId
                                     join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                     join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemName = sti.StoreItemVariationValue == null ? sto.Name : sto.Name + "/" + sti.StoreItemVariationValue.Value,
                                         QuantitySold = sts.QuantitySold,
                                         Sku = sti.SKU,
                                         AmountSold = sts.AmountSold,
                                         StoreItemVariationValueId = sti.StoreItemVariationValueId,
                                         StoreItemStockId = sts.StoreItemStockId,
                                         UoMCode = ui.UoMCode,
                                         Rate = sts.Rate
                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new SaleObject();
                    }

                    soldItems.ForEach(sts =>
                    {

                        var images = db.StockUploads.Where(i => i.StoreItemStockId == sts.StoreItemStockId).Include("ImageView").ToList();
                        if (images.Any())
                        {
                            var frontView = images.Find(o => o.ImageView.Name.ToLower() == "front view");
                            if (frontView == null || frontView.StockUploadId < 1)
                            {
                                frontView = images[0];
                            }

                            sts.ImagePath = frontView.ImagePath;
                        }

                        sts.ImagePath = string.IsNullOrEmpty(sts.ImagePath) ? "/Content/images/noImage.png" : PhysicalToVirtualPathMapper.MapPath(sts.ImagePath);
                        s.StoreItemSoldObjects.Add(sts);
                    });


                    var transactions = (from t in db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).Include("StorePaymentMethod")
                                        join em in db.Employees on t.EffectedByEmployeeId equals em.EmployeeId
                                        join ps in db.UserProfiles on em.UserId equals ps.Id
                                        join ot in db.StoreOutlets on t.StoreOutletId equals ot.StoreOutletId
                                        select new StoreTransactionObject
                                        {
                                            StoreTransactionId = t.StoreTransactionId,
                                            StoreTransactionTypeId = t.StoreTransactionTypeId,
                                            StorePaymentMethodId = t.StorePaymentMethodId,
                                            EffectedByEmployeeId = t.EffectedByEmployeeId,
                                            TransactionAmount = t.TransactionAmount,
                                            PaymentMethodName = t.StorePaymentMethod.Name,
                                            TransactionDate = t.TransactionDate,
                                            Remark = t.Remark,
                                            StoreOutletId = t.StoreOutletId,
                                            TransactionEmployeeName = ps.LastName + " " + ps.OtherNames,
                                            OutletName = ot.OutletName

                                        }).ToList();

                    if (!transactions.Any())
                    {
                        return new SaleObject();
                    }

                    s.OutletName = transactions[0].OutletName;
                    s.Transactions = transactions;

                    var amountPaid = 0.0;
                    transactions.ForEach(n =>
                    {
                        amountPaid += n.TransactionAmount;
                    });
                  
                    s.AmountPaid = amountPaid;
                    s.AmountPaidStr = amountPaid.ToString("n0"); s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    s.DateStr = s.Date.ToString("dd/MM/yyyy hh:mm tt");
                    s.Balance = s.NetAmount - s.AmountPaid;
                    return s;

                }
            }
            catch (Exception ex)
            {
                return new SaleObject();
            }
        }

        public List<StoreItemSoldObject> GetProductSalesReportDetails(ProductReport productReport)
        {
            try
            {
                using (var db = _db)
                {
                    var soldItems = (from sts in db.StoreItemSolds.Where(d => d.StoreItemStockId == productReport.ProductId && d.DateSold <= productReport.EndDate && d.DateSold >= productReport.StartDate)
                                     join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                     join sto in db.StoreItems on sti.StoreItemId equals sto.StoreItemId
                                     join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                     join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                         QuantitySold = sts.QuantitySold,
                                         AmountSold = sts.AmountSold,
                                         UoMCode = ui.UoMCode,
                                         QuantityLeft = sti.QuantityInStock,
                                         DateSold = sts.DateSold,
                                         Rate = sts.Rate,
                                         QuantityAlreadySold = sti.TotalQuantityAlreadySold

                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new List<StoreItemSoldObject>();
                    }

                    soldItems.ForEach(n =>
                    {
                        n.DateSoldStr = n.DateSold.ToString("dd/MM/yyyy hh:mm tt");
                    });

                    return soldItems;

                }
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }

        #region Searches

        public List<SaleObject> SearchUncompletedTransactions(string searchCriteria)
        {
            try
            {
                const int uncompleted = (int)SaleStatus.Partly_Paid;

                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(a => a.Status == uncompleted
                                                 && (a.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || a.Employee.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                                 || a.Employee.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower()))).OrderByDescending(m => m.SaleId)
                                 join sp in db.SalePayments on sa.SaleId equals sp.SaleId
                                 where !sa.NetAmount.Equals(sp.AmountPaid)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     CustomerId = sa.CustomerId,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems.Where(g => g.Name.ToLower().Contains(searchCriteria.ToLower())) on sti.StoreItemId equals sto.StoreItemId
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             Rate = sts.Rate
                                         }).ToList();
                        if (!soldItems.Any())
                        {
                            return;
                        }

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        soldItems.ForEach(n =>
                        {
                            if (string.IsNullOrEmpty(s.StoreItemName))
                            {
                                s.StoreItemName = n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                            else
                            {
                                s.StoreItemName += ", " + n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                        });
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });

                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = "Partly Paid";
                        s.Balance = s.NetAmount - s.AmountPaid;
                        s.BalanceStr = s.Balance.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }


        public List<SaleObject> SearchDueInvoices(string searchCriteria)
        {
            try
            {

                 var day2 = DateTime.Today.AddDays(-2);
                 const int refunded = (int)SaleStatus.Refund_Note_Issued;
                
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(a => a.Date <= day2 && a.Status != refunded
                                                 && (a.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || a.Employee.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                                 || a.Employee.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower()))).OrderByDescending(m => m.SaleId)
                                 join sp in db.SalePayments on sa.SaleId equals sp.SaleId
                                 where !sa.NetAmount.Equals(sp.AmountPaid)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     CustomerId = sa.CustomerId,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems.Where(g => g.Name.ToLower().Contains(searchCriteria.ToLower())) on sti.StoreItemId equals sto.StoreItemId
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode,
                                             Rate = sts.Rate

                                         }).ToList();
                        if (!soldItems.Any())
                        {
                            return;
                        }

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        soldItems.ForEach(n =>
                        {
                            if (string.IsNullOrEmpty(s.StoreItemName))
                            {
                                s.StoreItemName = n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                            else
                            {
                                s.StoreItemName += ", " + n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                        });
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0"); s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> SearchRevokedSales(string searchCriteria)
        {
            try
            {
                const int refunded = (int)SaleStatus.Refund_Note_Issued;

                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(a => a.Status == refunded
                                                 && (a.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || a.Employee.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                                 || a.Employee.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower()))).OrderByDescending(m => m.SaleId)
                                 join sp in db.SalePayments on sa.SaleId equals sp.SaleId
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     CustomerId = sa.CustomerId,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems.Where(g => g.Name.ToLower().Contains(searchCriteria.ToLower())) on sti.StoreItemId equals sto.StoreItemId
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode
                                         }).ToList();

                        if (!soldItems.Any())
                        {
                            return;
                        }

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        soldItems.ForEach(n =>
                        {
                            if (string.IsNullOrEmpty(s.StoreItemName))
                            {
                                s.StoreItemName = n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                            else
                            {
                                s.StoreItemName += ", " + n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                        });
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0"); s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> SearchEmployeeSalesReport(string searchCriteria, long employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(s => s.EmployeeId == employeeId && (s.Date <= endDate && s.Date >= startDate)
                                     && (s.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || s.Employee.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                     || s.Employee.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                                     )).OrderByDescending(m => m.SaleId)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     CustomerId = sa.CustomerId,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems.Where(g => g.Name.ToLower().Contains(searchCriteria.ToLower())) on sti.StoreItemId equals sto.StoreItemId
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode

                                         }).ToList();
                        if (!soldItems.Any())
                        {
                            return;
                        }

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        soldItems.ForEach(n =>
                        {
                            if (string.IsNullOrEmpty(s.StoreItemName))
                            {
                                s.StoreItemName = n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                            else
                            {
                                s.StoreItemName += ", " + n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                        });
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<StoreItemStockObject> SearchRecommendedPurchases(string searchCriteria)
        {
            try
            {
                using (var db = _db)
                {
                    var soldItems = (from sti in db.StoreItemStocks.Where(s => s.ReorderLevel > 0 && s.QuantityInStock <= s.ReorderLevel && (s.SKU.ToLower().Contains(searchCriteria.ToLower()) || s.StoreItem.Name.ToLower().Contains(searchCriteria.ToLower())))
                                     join sto in db.StoreItems.Include("StoreItemCategory") on sti.StoreItemId equals sto.StoreItemId
                                     join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     select new StoreItemStockObject
                                     {
                                         StoreItemStockId = sti.StoreItemStockId,
                                         Description = sto.Description,
                                         ReorderLevel = sti.ReorderLevel,
                                         ReorderQuantity = sti.ReorderQuantity,
                                         SKU = sti.SKU,
                                         StoreItemCategoryId = sto.StoreItemCategoryId,
                                         StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                         QuantityInStock = sti.QuantityInStock,
                                         TotalQuantityAlreadySold = sti.TotalQuantityAlreadySold,
                                         CategoryName = sto.StoreItemCategory.Name

                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new List<StoreItemStockObject>();
                    }

                    soldItems.ForEach(n =>
                    {
                        n.QuantityInStockStr = n.QuantityInStock.ToString("n0");
                        n.ReOrderLevelStr = n.ReorderLevel.ToString("n0");
                        n.ReOrderQuantityStr = n.ReorderQuantity.ToString("n0");
                        n.TotalQuantityAlreadySoldStr = n.TotalQuantityAlreadySold.ToString("n0");
                    });
                    return soldItems.OrderBy(i => i.StoreItemCategoryId).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<StoreItemStockObject>();
            }
        }

        public List<SaleObject> SearchCustomerSalesReport(string searchCriteria, long customerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(s => s.CustomerId == customerId && (s.Date <= endDate && s.Date >= startDate)
                                     && (s.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || s.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                     || s.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                                     )).OrderByDescending(m => m.SaleId)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     CustomerId = sa.CustomerId,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems.Where(g => g.Name.ToLower().Contains(searchCriteria.ToLower())) on sti.StoreItemId equals sto.StoreItemId
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode

                                         }).ToList();
                        if (!soldItems.Any())
                        {
                            return;
                        }

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        soldItems.ForEach(n =>
                        {
                            if (string.IsNullOrEmpty(s.StoreItemName))
                            {
                                s.StoreItemName = n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                            else
                            {
                                s.StoreItemName += ", " + n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                        });
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }
        public List<CustomerInvoiceObject> SearchCustomerInvoiceReport(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var customerInvoices = (from sa in db.CustomerInvoices.OrderBy(m => m.CustomerId)
                                            join cs in db.Customers.Where(s => s.DateProfiled <= endDate && s.DateProfiled >= startDate && (s.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()) || s.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower()))).Include("UserProfile") on sa.CustomerId equals cs.CustomerId
                                             select new CustomerInvoiceObject
                                             {
                                                 Id = sa.Id,
                                                 CustomerId = sa.CustomerId,
                                                 TotalAmountPaid = sa.TotalAmountPaid,
                                                 TotalAmountDue = sa.TotalAmountDue,
                                                 TotalVATAmount = sa.TotalVATAmount,
                                                 TotalDiscountAmount = sa.TotalDiscountAmount,
                                                 InvoiceBalance = sa.InvoiceBalance,
                                                 DateProfiled = sa.Customer.DateProfiled,
                                                 CustomerName = cs.UserProfile.LastName + " " + cs.UserProfile.OtherNames
                                             }).ToList();

                        if (!customerInvoices.Any())
                        {
                            return new List<CustomerInvoiceObject>();
                        }

                        customerInvoices.ForEach(s =>
                        {
                            s.TotalAmountPaidStr = s.TotalAmountPaid.ToString("n0");
                            s.TotalAmountDueStr = s.TotalAmountDue.ToString("n0");
                            s.InvoiceBalanceStr = s.InvoiceBalance.ToString("n0");
                            s.TotalVATAmountStr = s.TotalVATAmount.ToString("n0");
                            s.TotalDiscountAmountStr = s.TotalDiscountAmount.ToString("n0");
                            s.DateProfiledStr = s.DateProfiled != null? ((DateTime)s.DateProfiled).ToString("dd/MM/yyyy") : "Not Available";
                        }); 

                    return customerInvoices;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CustomerInvoiceObject>();
            }
        }

        public List<SaleObject> SearchAdminSalesReport(string searchCriteria)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(s => s.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || s.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                     || s.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                                     ).OrderByDescending(m => m.SaleId)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     CustomerId = sa.CustomerId,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems.Where(g => g.Name.ToLower().Contains(searchCriteria.ToLower())) on sti.StoreItemId equals sto.StoreItemId
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode

                                         }).ToList();
                        if (!soldItems.Any())
                        {
                            return;
                        }

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        soldItems.ForEach(n =>
                        {
                            if (string.IsNullOrEmpty(s.StoreItemName))
                            {
                                s.StoreItemName = n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                            else
                            {
                                s.StoreItemName += ", " + n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                        });
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0"); s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> SearchEmployeeSalesReport(string searchCriteria, long employeeId)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(s => s.EmployeeId == employeeId
                                     && (s.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || s.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                     || s.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                                     )).OrderByDescending(m => m.SaleId)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     CustomerId = sa.CustomerId,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }

                    sales.ForEach(s =>
                    {
                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }

                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0");
                        s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                    });


                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> SearchContactPersonInvoices(string searchCriteria, long employeeId)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(s => s.Customer.ContactPersonId == employeeId
                                     && (s.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || s.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                     || s.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                                     )).OrderByDescending(m => m.SaleId)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     CustomerId = sa.CustomerId,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        var soldItems = (from sts in db.StoreItemSolds.Where(d => d.SaleId == s.SaleId)
                                         join sti in db.StoreItemStocks on sts.StoreItemStockId equals sti.StoreItemStockId
                                         join sto in db.StoreItems.Where(g => g.Name.ToLower().Contains(searchCriteria.ToLower())) on sti.StoreItemId equals sto.StoreItemId
                                         
                                         join ui in db.UnitsOfMeasurements on sts.UoMId equals ui.UnitOfMeasurementId
                                         join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                         select new StoreItemSoldObject
                                         {
                                             StoreItemName = vv == null ? sto.Name : sto.Name + "/" + vv.Value,
                                             QuantitySold = sts.QuantitySold,
                                             AmountSold = sts.AmountSold,
                                             UoMCode = ui.UoMCode

                                         }).ToList();
                        if (!soldItems.Any())
                        {
                            return;
                        }

                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        soldItems.ForEach(n =>
                        {
                            if (string.IsNullOrEmpty(s.StoreItemName))
                            {
                                s.StoreItemName = n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                            else
                            {
                                s.StoreItemName += ", " + n.StoreItemName + " : " + n.QuantitySold.ToString("n0") + n.UoMCode;
                            }
                        });
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0"); s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }
        
        public List<StoreTransactionObject> SearchSalesReportByPaymentType(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {

                    var transactions = (from tt in db.StoreTransactions.Where(s => (s.StorePaymentMethod.Name.ToLower().Contains(searchCriteria.ToLower()) || s.StoreTransactionType.Name.ToLower().Contains(searchCriteria.ToLower())) && (s.TransactionDate <= endDate && s.TransactionDate >= startDate)).OrderBy(m => m.TransactionDate)
                                         .Include("StorePaymentMethod").Include("StoreOutlet").Include("StoreTransactionType")
                                        select new StoreTransactionObject
                                        {
                                            StoreTransactionId = tt.StoreTransactionId,
                                            StoreTransactionTypeId = tt.StoreTransactionTypeId,
                                            EffectedByEmployeeId = tt.EffectedByEmployeeId,
                                            StorePaymentMethodId = tt.StorePaymentMethodId,
                                            StoreOutletId = tt.StoreOutletId,
                                            TransactionAmount = tt.TransactionAmount,
                                            TransactionDate = tt.TransactionDate,
                                            PaymentMethodName = tt.StorePaymentMethod.Name

                                        }).ToList();

                    if (!transactions.Any())
                    {
                       return new List<StoreTransactionObject>();
                    }

                    transactions.ForEach(s =>
                    {
                        s.TransactionDateStr = s.TransactionDate.ToString("dd/MM/yyyy");
                        s.TransactionAmountStr = s.TransactionAmount.ToString("n0");
                    });

                    return transactions;

                }
            }
            catch (Exception ex)
            {
                return new List<StoreTransactionObject>();
            }
        }
        public List<SaleObject> SearchDailySalesReport(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from sa in db.Sales.Where(s => (s.Date <= endDate && s.Date >= startDate)
                                     && (s.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || s.Employee.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                     || s.Employee.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                                     )).OrderByDescending(m => m.SaleId)
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     NetAmount = sa.NetAmount,
                                     CustomerId = sa.CustomerId,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }
                    sales.ForEach(s =>
                    {
                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }
                        
                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                    });
                    return sales;

                }
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }
        public List<StoreItemSoldObject> SearStoreItemReport(string searchCriteria, long itemId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var soldItems = (from sts in db.StoreItems.Where(s => s.StoreItemId == itemId && s.Name.ToLower().Contains(searchCriteria.ToLower())).OrderBy(m => m.Name)
                                     join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                     join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate).Include("Sale") on sti.StoreItemStockId equals sto.StoreItemStockId
                                     join em in db.Employees on sto.Sale.EmployeeId equals em.EmployeeId
                                     join usr in db.UserProfiles on em.UserId equals usr.Id
                                     
                                     join ui in db.UnitsOfMeasurements on sto.UoMId equals ui.UnitOfMeasurementId
                                     join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemName = vv == null ? sts.Name : sts.Name + "/" + vv.Value,
                                         QuantitySold = sto.QuantitySold,
                                         AmountSold = sto.AmountSold,
                                         UoMCode = ui.UoMCode,
                                         DateSold = sto.Sale.Date,
                                         SaleId = sto.Sale.SaleId,
                                         Employee = usr.LastName + " " + usr.OtherNames,
                                         Rate = sto.Rate
                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new List<StoreItemSoldObject>();
                    }

                    soldItems.ForEach(s =>
                    {
                        s.DateSoldStr = s.DateSold.ToString("dd/MM/yyyy");
                        s.AmountSoldStr = s.AmountSold.ToString("n0");
                        s.QuantitySoldStr = s.QuantitySold.ToString("n0");
                    });

                    return soldItems;

                }
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }
        public List<SaleObject> SearchSalesReportByOutlet(string searchCriteria, int outletId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var sales = (from ot in db.StoreOutlets.Where(s => s.StoreOutletId == outletId).Where(z => z.OutletName.ToLower().Contains(searchCriteria.ToLower()))
                                 join rs in db.Registers on ot.StoreOutletId equals rs.CurrentOutletId
                                 join sa in db.Sales.Where(s => s.Date <= endDate && s.Date >= startDate && (s.InvoiceNumber.ToLower().Contains(searchCriteria.ToLower()) || s.Employee.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                                     || s.Employee.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                                     )) on rs.RegisterId equals sa.RegisterId
                                 join stt in db.SaleTransactions on sa.SaleId equals stt.SaleId
                                 join tt in db.StoreTransactions on stt.StoreTransactionId equals tt.StoreTransactionId
                                 select new SaleObject
                                 {
                                     SaleId = sa.SaleId,
                                     RegisterId = sa.RegisterId,
                                     AmountDue = sa.AmountDue,
                                     VATAmount = sa.VATAmount,
                                     CustomerId = sa.CustomerId,
                                     NetAmount = sa.NetAmount,
                                     InvoiceNumber = sa.InvoiceNumber,
                                     Discount = sa.Discount,
                                     DiscountAmount = sa.DiscountAmount,
                                     Status = sa.Status,
                                     Date = sa.Date,
                                     StoreTransactionId = stt.StoreTransactionId

                                 }).ToList();

                    if (!sales.Any())
                    {
                        return new List<SaleObject>();
                    }

                    sales.ForEach(s =>
                    {
                        if (s.CustomerId != null && s.CustomerId > 0)
                        {
                            var customers = (from cu in db.Customers.Where(d => d.CustomerId == s.CustomerId)
                                             join ps in db.UserProfiles on cu.UserId equals ps.Id
                                             select new CustomerObject
                                             {
                                                 UserProfileName = cu != null ? ps.LastName + " " + ps.OtherNames : "",
                                                 CustomerId = cu.CustomerId,
                                                 UserId = ps.Id
                                             }).ToList();

                            if (customers.Any())
                            {
                                var customer = customers[0];
                                s.CustomerName = customer.UserProfileName;
                            }
                            else
                            {
                                s.CustomerName = "N/A";
                            }
                        }

                        var transactions = db.StoreTransactions.Where(t => t.StoreTransactionId == s.StoreTransactionId).ToList();
                        if (!transactions.Any())
                        {
                            return;
                        }

                        var amountPaid = 0.0;
                        transactions.ForEach(n =>
                        {
                            amountPaid += n.TransactionAmount;
                        });
                        s.AmountPaid = amountPaid;
                        s.AmountPaidStr = amountPaid.ToString("n0");s.StatusStr = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');
                        s.AmountDueStr = s.AmountDue.ToString("n0");
                        s.VATAmountStr = s.VATAmount.ToString("n0");
                        s.NetAmountStr = s.NetAmount.ToString("n0");
                        s.DiscountAmountStr = s.DiscountAmount.ToString("n0");
                        s.DateStr = s.Date.ToString("dd/MM/yyyy");
                        s.PaymentStatus = Enum.GetName(typeof(SaleStatus), s.Status).Replace('_', ' ');

                    });

                    return sales;

                }
            }
            catch (Exception ex)
            {
                return new List<SaleObject>();
            }
        }
        public List<StoreItemSoldObject> SearchStoreItemCategoryReport(string searchCriteria, int categoryId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var soldItems = (from sts in db.StoreItems.Where(s => s.StoreItemCategoryId == categoryId).Where(n => n.Name.ToLower().Contains(searchCriteria.ToLower())).OrderBy(m => m.Name)
                                     join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                     join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate).Include("Sale") on sti.StoreItemStockId equals sto.StoreItemStockId
                                     join em in db.Employees on sto.Sale.EmployeeId equals em.EmployeeId
                                     join usr in db.UserProfiles.Where(n => n.LastName.ToLower().Contains(searchCriteria.ToLower()) || n.OtherNames.ToLower().Contains(searchCriteria.ToLower())) on em.UserId equals usr.Id
                                     
                                     join ui in db.UnitsOfMeasurements on sto.UoMId equals ui.UnitOfMeasurementId
                                     join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemName = vv == null ? sts.Name : sts.Name + "/" + vv.Value,
                                         QuantitySold = sto.QuantitySold,
                                         AmountSold = sto.AmountSold,
                                         UoMCode = ui.UoMCode,
                                         DateSold = sto.Sale.Date,
                                         SaleId = sto.Sale.SaleId,
                                         Employee = usr.LastName + " " + usr.OtherNames,
                                         Rate = sto.Rate

                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new List<StoreItemSoldObject>();
                    }

                    soldItems.ForEach(s =>
                    {
                        s.DateSoldStr = s.DateSold.ToString("dd/MM/yyyy");
                        s.AmountSoldStr = s.AmountSold.ToString("n0");
                        s.QuantitySoldStr = s.QuantitySold.ToString("n0");
                    });

                    return soldItems;

                }
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }
        public List<StoreItemSoldObject> SearchStoreItemTypeReport(string searchCriteria, int typeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var soldItems = (from sts in db.StoreItems.Where(s => s.StoreItemTypeId == typeId).Where(n => n.Name.ToLower().Contains(searchCriteria.ToLower())).OrderBy(m => m.Name)
                                     join sti in db.StoreItemStocks on sts.StoreItemId equals sti.StoreItemId
                                     join sto in db.StoreItemSolds.Where(a => a.Sale.Date >= startDate && a.Sale.Date <= endDate).Include("Sale") on sti.StoreItemStockId equals sto.StoreItemStockId
                                     join em in db.Employees on sto.Sale.EmployeeId equals em.EmployeeId
                                     join usr in db.UserProfiles.Where(n => n.LastName.ToLower().Contains(searchCriteria.ToLower()) || n.OtherNames.ToLower().Contains(searchCriteria.ToLower())) on em.UserId equals usr.Id
                                     
                                     join ui in db.UnitsOfMeasurements on sto.UoMId equals ui.UnitOfMeasurementId
                                     join vv in db.StoreItemVariationValues on sti.StoreItemVariationValueId equals vv.StoreItemVariationValueId
                                     select new StoreItemSoldObject
                                     {
                                         StoreItemName = vv == null ? sts.Name : sts.Name + "/" + vv.Value,
                                         QuantitySold = sto.QuantitySold,
                                         AmountSold = sto.AmountSold,
                                         UoMCode = ui.UoMCode,
                                         DateSold = sto.Sale.Date,
                                         SaleId = sto.Sale.SaleId,
                                         Employee = usr.LastName + " " + usr.OtherNames,
                                         Rate = sto.Rate

                                     }).ToList();

                    if (!soldItems.Any())
                    {
                        return new List<StoreItemSoldObject>();
                    }

                    soldItems.ForEach(s =>
                    {
                        s.DateSoldStr = s.DateSold.ToString("dd/MM/yyyy");
                        s.AmountSoldStr = s.AmountSold.ToString("n0");
                        s.QuantitySoldStr = s.QuantitySold.ToString("n0");
                    });

                    return soldItems;

                }
            }
            catch (Exception ex)
            {
                return new List<StoreItemSoldObject>();
            }
        }
        #endregion

        #endregion

        #region PURCHASE ORDER REPORTS
        public List<PurchaseOrderObject> GetPurchaseOrdersByStatus(int? itemsPerPage, int? pageNumber, out int count, int status, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var purchaseOrders = db.PurchaseOrders.Where(s => s.StatusCode == status && ((s.DateTimePlaced <= endDate && s.DateTimePlaced >= startDate) || (s.ActualDeliveryDate <= endDate && s.ActualDeliveryDate >= startDate))).OrderByDescending(m => m.PurchaseOrderId).Skip(tpageNumber).Take(tsize)
                                .Include("ChartOfAccount").Include("PurchaseOrderItems").Include("Employee").Include("Supplier").Include("StoreOutlet").Include("PurchaseOrderPayments").Include("Invoices")
                                .ToList();

                        if (purchaseOrders.Any())
                        {
                            var newList = new List<PurchaseOrderObject>();
                            purchaseOrders.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                                if (orderObject == null || orderObject.PurchaseOrderId < 1)
                                {
                                    return;
                                }

                                var chart = order.ChartOfAccount;
                                var employee = order.Employee;
                                var outlet = order.StoreOutlet;
                                orderObject.SupplierName = order.Supplier.CompanyName;
                                orderObject.AccountCode = chart.AccountCode;

                                orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                                orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                                var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                                if (delStatus != null)
                                {
                                    orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                                }

                                var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                                if (groups.Any())
                                {
                                    orderObject.AccountGroupName = groups[0].Name;
                                }

                                if (employee != null && employee.EmployeeId > 0)
                                {
                                    var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                    if (profiles.Any())
                                    {
                                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                        orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                    }

                                }

                                if (outlet != null && outlet.StoreOutletId > 0)
                                {
                                    orderObject.OutletName = outlet.OutletName;
                                }

                                newList.Add(orderObject);
                            });

                            count = db.PurchaseOrders.Count();
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<PurchaseOrderObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> GetPurchaseOrderReports(int? itemsPerPage, int? pageNumber, out int count, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var purchaseOrders = db.PurchaseOrders.Where(s => (s.DateTimePlaced <= endDate && s.DateTimePlaced >= startDate) || (s.ActualDeliveryDate <= endDate && s.ActualDeliveryDate >= startDate)).OrderByDescending(m => m.PurchaseOrderId).Skip(tpageNumber).Take(tsize)
                                .Include("ChartOfAccount").Include("PurchaseOrderItems").Include("Employee").Include("Supplier").Include("StoreOutlet").Include("PurchaseOrderPayments").Include("Invoices")
                                .ToList();

                        if (purchaseOrders.Any())
                        {
                            var newList = new List<PurchaseOrderObject>();
                            purchaseOrders.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                                if (orderObject == null || orderObject.PurchaseOrderId < 1)
                                {
                                    return;
                                }

                                var chart = order.ChartOfAccount;
                                var employee = order.Employee;
                                var outlet = order.StoreOutlet;
                                orderObject.SupplierName = order.Supplier.CompanyName;
                                orderObject.AccountCode = chart.AccountCode;

                                orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                                orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                                var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                                if (delStatus != null)
                                {
                                    orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                                }

                                var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                                if (groups.Any())
                                {
                                    orderObject.AccountGroupName = groups[0].Name;
                                }

                                if (employee != null && employee.EmployeeId > 0)
                                {
                                    var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                    if (profiles.Any())
                                    {
                                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                        orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                    }

                                }

                                if (outlet != null && outlet.StoreOutletId > 0)
                                {
                                    orderObject.OutletName = outlet.OutletName;
                                }

                                newList.Add(orderObject);
                            });

                            count = db.PurchaseOrders.Count();
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<PurchaseOrderObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> GetPurchaseOrdersByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var purchaseOrders = db.PurchaseOrders.Where(s => s.StoreOutletId == outletId && ((s.DateTimePlaced <= endDate && s.DateTimePlaced >= startDate) || (s.ActualDeliveryDate <= endDate && s.ActualDeliveryDate >= startDate))).OrderByDescending(m => m.PurchaseOrderId).Skip(tpageNumber).Take(tsize)
                                .Include("ChartOfAccount")
                                .Include("Employee")
                                .Include("StoreOutlet")
                                .Include("Supplier")
                                .ToList();

                        if (purchaseOrders.Any())
                        {
                            var newList = new List<PurchaseOrderObject>();
                            purchaseOrders.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                                if (orderObject == null || orderObject.PurchaseOrderId < 1)
                                {
                                    return;
                                }

                                var chart = order.ChartOfAccount;
                                var employee = order.Employee;
                                var outlet = order.StoreOutlet;
                                orderObject.SupplierName = order.Supplier.CompanyName;
                                orderObject.AccountCode = chart.AccountCode;

                                orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                                orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                                var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                                if (groups.Any())
                                {
                                    orderObject.AccountGroupName = groups[0].Name;
                                }

                                if (employee != null && employee.EmployeeId > 0)
                                {
                                    var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                    if (profiles.Any())
                                    {
                                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                        orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                    }

                                }

                                if (outlet != null && outlet.StoreOutletId > 0)
                                {
                                    orderObject.OutletName = outlet.OutletName;
                                }
                                var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                                if (delStatus != null)
                                {
                                    orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                                }
                                newList.Add(orderObject);
                            });

                            count = db.PurchaseOrders.Count(o => o.StoreOutletId == outletId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<PurchaseOrderObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> GetPurchaseOrderReportsByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var purchaseOrders = db.PurchaseOrders.Where(s => s.GeneratedById == employeeId && ((s.DateTimePlaced <= endDate && s.DateTimePlaced >= startDate) || (s.ActualDeliveryDate <= endDate && s.ActualDeliveryDate >= startDate))).OrderByDescending(m => m.PurchaseOrderId).Skip(tpageNumber).Take(tsize)
                                .Include("ChartOfAccount")
                                .Include("Employee")
                                .Include("StoreOutlet")
                                .Include("Supplier")
                                .ToList();

                        if (purchaseOrders.Any())
                        {
                            var newList = new List<PurchaseOrderObject>();
                            purchaseOrders.ForEach(order =>
                            {
                                var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                                if (orderObject == null || orderObject.PurchaseOrderId < 1)
                                {
                                    return;
                                }

                                var chart = order.ChartOfAccount;
                                var employee = order.Employee;
                                var outlet = order.StoreOutlet;
                                orderObject.SupplierName = order.Supplier.CompanyName;
                                orderObject.AccountCode = chart.AccountCode;

                                orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                                orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                                orderObject.ActualDeliveryDateStr = order.ActualDeliveryDate != null ? ((DateTime)order.ActualDeliveryDate).ToString("dd/MM/yyyy") : "";
                                orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                                var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                                if (groups.Any())
                                {
                                    orderObject.AccountGroupName = groups[0].Name;
                                }

                                if (employee != null && employee.EmployeeId > 0)
                                {
                                    var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                    if (profiles.Any())
                                    {
                                        orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                        orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                    }

                                }

                                if (outlet != null && outlet.StoreOutletId > 0)
                                {
                                    orderObject.OutletName = outlet.OutletName;
                                }

                                var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                                if (delStatus != null)
                                {
                                    orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                                }

                                newList.Add(orderObject);
                            });

                            count = db.PurchaseOrders.Count(o => o.GeneratedById == employeeId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<PurchaseOrderObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<PurchaseOrderObject>();
            }
        }

        #region SEARCHES
        public List<PurchaseOrderObject> SearchPurchaseOrderReportsByStatus(string searchCriteria, int status, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var purchaseOrders = db.PurchaseOrders.Where(o => o.StatusCode == status && ((o.DateTimePlaced <= endDate && o.DateTimePlaced >= startDate) || (o.ActualDeliveryDate <= endDate && o.ActualDeliveryDate >= startDate))
                         && ((o.ChartOfAccount.AccountCode.ToLower().Contains(searchCriteria.ToLower())
                        || o.ChartOfAccount.AccountGroup.Name.ToLower().Contains(searchCriteria.ToLower()))
                        || o.Employee.EmployeeNo.ToLower().Contains(searchCriteria.ToLower())
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Supplier.CompanyName.ToLower().Contains(searchCriteria.ToLower())))
                            .Include("ChartOfAccount")
                            .Include("Employee")
                            .Include("StoreOutlet")
                            .Include("Supplier")
                            .ToList();

                    if (purchaseOrders.Any())
                    {
                        var newList = new List<PurchaseOrderObject>();
                        purchaseOrders.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                            if (orderObject == null || orderObject.PurchaseOrderId < 1)
                            {
                                return;
                            }

                            var chart = order.ChartOfAccount;
                            var employee = order.Employee;
                            var outlet = order.StoreOutlet;
                            orderObject.SupplierName = order.Supplier.CompanyName;
                            orderObject.AccountCode = chart.AccountCode;

                            orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                            orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                orderObject.AccountGroupName = groups[0].Name;
                            }

                            if (employee != null && employee.EmployeeId > 0)
                            {
                                var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                if (profiles.Any())
                                {
                                    orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                    orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                }

                            }

                            if (outlet != null && outlet.StoreOutletId > 0)
                            {
                                orderObject.OutletName = outlet.OutletName;
                            }

                            var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                            if (delStatus != null)
                            {
                                orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                            }

                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<PurchaseOrderObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> SearchPurchaseOrderReports(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var purchaseOrders = db.PurchaseOrders.Where(o => ((o.DateTimePlaced <= endDate && o.DateTimePlaced >= startDate) || (o.ActualDeliveryDate <= endDate && o.ActualDeliveryDate >= startDate))
                         && ((o.ChartOfAccount.AccountCode.ToLower().Contains(searchCriteria.ToLower())
                        || o.ChartOfAccount.AccountGroup.Name.ToLower().Contains(searchCriteria.ToLower()))
                        || o.Employee.EmployeeNo.ToLower().Contains(searchCriteria.ToLower())
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Supplier.CompanyName.ToLower().Contains(searchCriteria.ToLower())))
                            .Include("ChartOfAccount")
                            .Include("Employee")
                            .Include("StoreOutlet")
                            .Include("Supplier")
                            .ToList();

                    if (purchaseOrders.Any())
                    {
                        var newList = new List<PurchaseOrderObject>();
                        purchaseOrders.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                            if (orderObject == null || orderObject.PurchaseOrderId < 1)
                            {
                                return;
                            }

                            var chart = order.ChartOfAccount;
                            var employee = order.Employee;
                            var outlet = order.StoreOutlet;
                            orderObject.SupplierName = order.Supplier.CompanyName;
                            orderObject.AccountCode = chart.AccountCode;

                            orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                            orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                orderObject.AccountGroupName = groups[0].Name;
                            }

                            if (employee != null && employee.EmployeeId > 0)
                            {
                                var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                if (profiles.Any())
                                {
                                    orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                    orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                }

                            }

                            if (outlet != null && outlet.StoreOutletId > 0)
                            {
                                orderObject.OutletName = outlet.OutletName;
                            }

                            var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                            if (delStatus != null)
                            {
                                orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                            }

                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<PurchaseOrderObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> SearchOutletPurchaseOrderReports(string searchCriteria, int outletId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var purchaseOrders = db.PurchaseOrders.Where(o => o.StoreOutletId == outletId &&
                         ((o.DateTimePlaced <= endDate && o.DateTimePlaced >= startDate) || (o.ActualDeliveryDate <= endDate && o.ActualDeliveryDate >= startDate))
                         && ((o.ChartOfAccount.AccountCode.ToLower().Contains(searchCriteria.ToLower())
                        || o.ChartOfAccount.AccountGroup.Name.ToLower().Contains(searchCriteria.ToLower()))
                        || o.Employee.EmployeeNo.ToLower().Contains(searchCriteria.ToLower())
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Supplier.CompanyName.ToLower().Contains(searchCriteria.ToLower())))
                            .Include("ChartOfAccount")
                            .Include("Employee")
                            .Include("StoreOutlet")
                            .Include("Supplier")
                            .ToList();

                    if (purchaseOrders.Any())
                    {
                        var newList = new List<PurchaseOrderObject>();
                        purchaseOrders.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                            if (orderObject == null || orderObject.PurchaseOrderId < 1)
                            {
                                return;
                            }

                            var chart = order.ChartOfAccount;
                            var employee = order.Employee;
                            var outlet = order.StoreOutlet;
                            orderObject.SupplierName = order.Supplier.CompanyName;
                            orderObject.AccountCode = chart.AccountCode;

                            orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                            orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                orderObject.AccountGroupName = groups[0].Name;
                            }

                            if (employee != null && employee.EmployeeId > 0)
                            {
                                var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                if (profiles.Any())
                                {
                                    orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                    orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                }

                            }

                            if (outlet != null && outlet.StoreOutletId > 0)
                            {
                                orderObject.OutletName = outlet.OutletName;
                            }

                            var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                            if (delStatus != null)
                            {
                                orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                            }

                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<PurchaseOrderObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PurchaseOrderObject>();
            }
        }
        public List<PurchaseOrderObject> SearchEmployeePurchaseOrderReports(string searchCriteria, long employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var purchaseOrders = db.PurchaseOrders.Where(o => o.GeneratedById == employeeId
                         && ((o.DateTimePlaced <= endDate && o.DateTimePlaced >= startDate) || (o.ActualDeliveryDate <= endDate && o.ActualDeliveryDate >= startDate))
                         &&((o.ChartOfAccount.AccountCode.ToLower().Contains(searchCriteria.ToLower())
                        || o.ChartOfAccount.AccountGroup.Name.ToLower().Contains(searchCriteria.ToLower()))
                        || o.Employee.EmployeeNo.ToLower().Contains(searchCriteria.ToLower())
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Supplier.CompanyName.ToLower().Contains(searchCriteria.ToLower())))
                            .Include("ChartOfAccount")
                            .Include("Employee")
                            .Include("StoreOutlet")
                            .Include("Supplier")
                            .ToList();

                    if (purchaseOrders.Any())
                    {
                        var newList = new List<PurchaseOrderObject>();
                        purchaseOrders.ForEach(order =>
                        {
                            var orderObject = ModelCrossMapper.Map<PurchaseOrder, PurchaseOrderObject>(order);
                            if (orderObject == null || orderObject.PurchaseOrderId < 1)
                            {
                                return;
                            }

                            var chart = order.ChartOfAccount;
                            var employee = order.Employee;
                            var outlet = order.StoreOutlet;
                            orderObject.SupplierName = order.Supplier.CompanyName;
                            orderObject.AccountCode = chart.AccountCode;

                            orderObject.DateTimePlacedStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.DerivedTotalCostStr = order.DerivedTotalCost != null ? ((double)(order.DerivedTotalCost)).ToString("n0") : "";
                            orderObject.ActualDeliveryDateStr = order.DateTimePlaced.ToString("dd/MM/yyyy");
                            orderObject.ExpectedDeliveryDateStr = order.ExpectedDeliveryDate.ToString("dd/MM/yyyy");

                            var groups = db.AccountGroups.Where(g => g.AccountGroupId == chart.AccountGroupId).ToList();
                            if (groups.Any())
                            {
                                orderObject.AccountGroupName = groups[0].Name;
                            }

                            if (employee != null && employee.EmployeeId > 0)
                            {
                                var profiles = db.UserProfiles.Where(p => p.Id == employee.UserId).ToList();
                                if (profiles.Any())
                                {
                                    orderObject.GeneratedByEmployeeNo = employee.EmployeeNo;
                                    orderObject.GeneratedByEmployeeName = profiles[0].LastName + profiles[0].OtherNames;
                                }

                            }

                            if (outlet != null && outlet.StoreOutletId > 0)
                            {
                                orderObject.OutletName = outlet.OutletName;
                            }

                            var delStatus = Enum.GetName(typeof(PurchaseOrderDeliveryStatus), orderObject.StatusCode);
                            if (delStatus != null)
                            {
                                orderObject.DeliveryStatus = delStatus.Replace("_", " ");
                            }

                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<PurchaseOrderObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PurchaseOrderObject>();
            }
        }
        #endregion

        #endregion

        #region ESTIMATES REPORTS
        public List<EstimateObject> GetEstimatesByConversionStatus(int? itemsPerPage, int? pageNumber, bool status, out int count, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var estimates = db.Estimates.Where(s => s.ConvertedToInvoice == status && s.DateCreated <= endDate && s.DateCreated >= startDate).OrderBy(m => m.DateCreated).Skip(tpageNumber).Take(tsize)
                                .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                                .ToList();

                        if (estimates.Any())
                        {
                            var newList = new List<EstimateObject>();
                            estimates.ForEach(order =>
                            {
                                var customer = order.Customer;
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }



                                var customerProfile = db.UserProfiles.Find(customer.UserId);
                                if (customerProfile == null || customerProfile.Id < 1)
                                {
                                    return;
                                }

                                orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                                if (employee != null && employee.Id > 0)
                                {
                                    orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                                }

                                orderObject.OutletName = order.StoreOutlet.OutletName;
                                orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                                orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                                orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                                orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                                orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                                orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                                orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                                newList.Add(orderObject);
                            });

                            count = db.Estimates.Count();
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<EstimateObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> GetEstimateReports(int? itemsPerPage, int? pageNumber, out int count, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var estimates = db.Estimates.Where(s => s.DateCreated <= endDate && s.DateCreated >= startDate).OrderBy(m => m.DateCreated).Skip(tpageNumber).Take(tsize)
                                .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                                .ToList();

                        if (estimates.Any())
                        {
                            var newList = new List<EstimateObject>();
                            estimates.ForEach(order =>
                            {
                                var customer = order.Customer;
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }



                                var customerProfile = db.UserProfiles.Find(customer.UserId);
                                if (customerProfile == null || customerProfile.Id < 1)
                                {
                                    return;
                                }

                                orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                                if (employee != null && employee.Id > 0)
                                {
                                    orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                                }

                                orderObject.OutletName = order.StoreOutlet.OutletName;
                                orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                                orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                                orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                                orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                                orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                                orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                                orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                                newList.Add(orderObject);
                            });

                            count = db.Estimates.Count();
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<EstimateObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> GetEstimatesByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var estimates = db.Estimates.Where(o => o.OutletId == outletId && (o.DateCreated <= endDate && o.DateCreated >= startDate)).OrderBy(m => m.DateCreated).Skip(tpageNumber).Take(tsize)
                              .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                                .ToList();

                        if (estimates.Any())
                        {
                            var newList = new List<EstimateObject>();
                            estimates.ForEach(order =>
                            {
                                var customer = order.Customer;
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }



                                var customerProfile = db.UserProfiles.Find(customer.UserId);
                                if (customerProfile == null || customerProfile.Id < 1)
                                {
                                    return;
                                }

                                orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                                if (employee != null && employee.Id > 0)
                                {
                                    orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                                }

                                orderObject.OutletName = order.StoreOutlet.OutletName;
                                orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                                orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                                orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                                orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                                orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                                orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                                orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                                newList.Add(orderObject);

                            });

                            count = db.Estimates.Count(o => o.OutletId == outletId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<EstimateObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> GetEstimatesByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = _db)
                    {
                        var estimates = db.Estimates.Where(o => o.CreatedById == employeeId && (o.DateCreated <= endDate && o.DateCreated >= startDate)).OrderBy(m => m.DateCreated).Skip(tpageNumber).Take(tsize)
                               .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                                .ToList();

                        if (estimates.Any())
                        {
                            var newList = new List<EstimateObject>();
                            estimates.ForEach(order =>
                            {
                                var customer = order.Customer;
                                var employee = order.UserProfile;

                                var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                                if (orderObject == null || orderObject.Id < 1)
                                {
                                    return;
                                }



                                var customerProfile = db.UserProfiles.Find(customer.UserId);
                                if (customerProfile == null || customerProfile.Id < 1)
                                {
                                    return;
                                }

                                orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                                if (employee != null && employee.Id > 0)
                                {
                                    orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                                }

                                orderObject.OutletName = order.StoreOutlet.OutletName;
                                orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                                orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                                orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                                orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                                orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                                orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                                orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                                newList.Add(orderObject);
                            });

                            count = db.Estimates.Count(o => o.CreatedById == employeeId);
                            return newList;
                        }
                    }

                }
                count = 0;
                return new List<EstimateObject>();
            }
            catch (Exception ex)
            {
                count = 0;
                return new List<EstimateObject>();
            }
        }
        
        #region SEARCHES
        public List<EstimateObject> SearchEstimatesByConversionStatus(string searchCriteria, bool status, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => o.ConvertedToInvoice == status && (o.DateCreated <= endDate && o.DateCreated >= startDate)
                        && ((o.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                        || o.EstimateNumber.ToLower().Contains(searchCriteria.ToLower())))
                           .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                            .ToList();

                    if (estimates.Any())
                    {
                        var newList = new List<EstimateObject>();
                        estimates.ForEach(order =>
                        {
                            var customer = order.Customer;
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            var customerProfile = db.UserProfiles.Find(customer.UserId);
                            if (customerProfile == null || customerProfile.Id < 1)
                            {
                                return;
                            }

                            orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                            if (employee != null && employee.Id > 0)
                            {
                                orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                            }

                            orderObject.OutletName = order.StoreOutlet.OutletName;
                            orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                            orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                            orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                            orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                            orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                            orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                            orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<EstimateObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> SearchEstimates(string searchCriteria, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => (o.DateCreated <= endDate && o.DateCreated >= startDate)
                        && ((o.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                        || o.EstimateNumber.ToLower().Contains(searchCriteria.ToLower())))
                           .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                            .ToList();

                    if (estimates.Any())
                    {
                        var newList = new List<EstimateObject>();
                        estimates.ForEach(order =>
                        {
                            var customer = order.Customer;
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            var customerProfile = db.UserProfiles.Find(customer.UserId);
                            if (customerProfile == null || customerProfile.Id < 1)
                            {
                                return;
                            }

                            orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                            if (employee != null && employee.Id > 0)
                            {
                                orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                            }

                            orderObject.OutletName = order.StoreOutlet.OutletName;
                            orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                            orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                            orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                            orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");
                            orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                            orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                            orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<EstimateObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> SearchOutletEstimate(string searchCriteria, int outletId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => o.OutletId == outletId
                         && (o.DateCreated <= endDate && o.DateCreated >= startDate)
                         && ((o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                        || o.EstimateNumber.ToLower().Contains(searchCriteria.ToLower())))
                           .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                            .ToList();

                    if (estimates.Any())
                    {
                        var newList = new List<EstimateObject>();
                        estimates.ForEach(order =>
                        {
                            var customer = order.Customer;
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                            var customerProfile = db.UserProfiles.Find(customer.UserId);
                            if (customerProfile == null || customerProfile.Id < 1)
                            {
                                return;
                            }

                            orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                            if (employee != null && employee.Id > 0)
                            {
                                orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                            }

                            orderObject.OutletName = order.StoreOutlet.OutletName;
                            orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                            orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                            orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                            orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");

                            orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                            orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<EstimateObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EstimateObject>();
            }
        }
        public List<EstimateObject> SearchEmployeeEstimate(string searchCriteria, long employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var db = _db)
                {
                    var estimates = db.Estimates.Where(o => o.CreatedById == employeeId
                        && (o.DateCreated <= endDate && o.DateCreated >= startDate)
                        && ((o.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower()))
                        || o.StoreOutlet.OutletName.ToLower().Contains(searchCriteria.ToLower())
                        || o.Customer.UserProfile.LastName.ToLower().Contains(searchCriteria.ToLower())
                         || o.Customer.UserProfile.OtherNames.ToLower().Contains(searchCriteria.ToLower())
                        || o.EstimateNumber.ToLower().Contains(searchCriteria.ToLower())))
                           .Include("Customer").Include("UserProfile").Include("StoreOutlet")
                            .ToList();

                    if (estimates.Any())
                    {
                        var newList = new List<EstimateObject>();
                        estimates.ForEach(order =>
                        {
                            var customer = order.Customer;
                            var employee = order.UserProfile;

                            var orderObject = ModelCrossMapper.Map<Estimate, EstimateObject>(order);
                            if (orderObject == null || orderObject.Id < 1)
                            {
                                return;
                            }

                            orderObject.InvoiceStatus = orderObject.ConvertedToInvoice ? "Processed" : "Pending";

                            var customerProfile = db.UserProfiles.Find(customer.UserId);
                            if (customerProfile == null || customerProfile.Id < 1)
                            {
                                return;
                            }

                            orderObject.CustomerName = customerProfile.LastName + " " + customerProfile.OtherNames;

                            if (employee != null && employee.Id > 0)
                            {
                                orderObject.GeneratedByEmployee = employee.LastName + " " + employee.OtherNames;
                            }

                            orderObject.OutletName = order.StoreOutlet.OutletName;
                            orderObject.AmountDueStr = orderObject.AmountDue.ToString("n0");
                            orderObject.NetAmountStr = orderObject.NetAmount.ToString("n0");
                            orderObject.DiscountAmountStr = orderObject.DiscountAmount.ToString("n0");
                            orderObject.VATAmountStr = orderObject.VATAmount.ToString("n0");

                            orderObject.DateCreatedStr = orderObject.DateCreated.ToString("dd/MM/yyyy");
                            orderObject.LastUpdatedStr = orderObject.LastUpdated.ToString("dd/MM/yyyy");
                            newList.Add(orderObject);
                        });

                        return newList;
                    }
                    return new List<EstimateObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<EstimateObject>();
            }
        }
        #endregion

        #endregion

        public OnlineStoreObject GetDefaults()
        {
            try
            {
                using (var db = _db)
                {
                    var categories = (from ct in db.StoreItemCategories
                                      select new StoreItemCategoryObject
                                      {
                                          StoreItemCategoryId = ct.StoreItemCategoryId,
                                          Name = ct.Name
                                      }).ToList();

                    var itemTypes = (from tp in db.StoreItemTypes
                                     select new StoreItemTypeObject
                                     {
                                         StoreItemTypeId = tp.StoreItemTypeId,
                                         Name = tp.Name
                                     }).ToList();

                    var itemBrands = (from bd in db.StoreItemBrands
                                      select new StoreItemBrandObject
                                      {
                                          StoreItemBrandId = bd.StoreItemBrandId,
                                          Name = bd.Name
                                      }).ToList();

                    var paymentTypes = (from bd in db.StorePaymentMethods
                                        select new StorePaymentMethodObject
                                      {
                                          StorePaymentMethodId = bd.StorePaymentMethodId,
                                          Name = bd.Name
                                      }).ToList();
                    

                     var outlets = (from bd in db.StoreOutlets
                                        select new StoreOutletObject
                                      {
                                          StoreOutletId = bd.StoreOutletId,
                                          OutletName = bd.OutletName
                                      }).ToList();

                    var employees = (from em in db.Employees
                                     join ps in db.UserProfiles on em.UserId equals ps.Id
                                     select new UserProfileObject
                                      {
                                          Name = ps.LastName + " " + ps.OtherNames,
                                          Id = ps.Id,
                                          EmployeeId = em.EmployeeId,
                                      }).ToList();

                    //var items = (from st in db.StoreItems.OrderBy(m => m.StoreItemId).Take(50)
                    //             join stk in db.StoreItemStocks on st.StoreItemId equals stk.StoreItemId

                    //             select new StoreItemObject
                    //             {
                    //                 StoreItemId = st.StoreItemId,
                    //                 StoreItemBrandName = st.StoreItemBrand.Name,
                    //                 StoreItemTypeName = st.StoreItemType.Name,
                    //                 StoreItemCategoryName = st.StoreItemCategory.Name,
                    //                 StoreItemStockId = stk.StoreItemStockId,
                    //                 StoreItemBrandId = st.StoreItemBrandId,
                    //                 StoreItemTypeId = st.StoreItemTypeId,
                    //                 StoreItemCategoryId = st.StoreItemCategoryId,
                    //                 ChartOfAccountId = st.ChartOfAccountId,
                    //                 Name = st.Name,
                    //                 Description = st.Description,
                    //                 ParentItemId = st.ParentItemId

                    //             }).ToList();
                   
                    var obj = new OnlineStoreObject
                    {
                        ItemCategories = categories,
                        ItemTypes = itemTypes,
                        ItemBrands = itemBrands,
                        //Items = items,
                        Employees = employees,
                        PaymentMethods = paymentTypes,
                        Outlets = outlets
                    };
                    return obj;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new OnlineStoreObject();
            }
        }

        public List<SupplierObject> GetSuppliers()
        {
            try
            {
                using (var db = _db)
                {
                    var suppliers = (from sp in db.Suppliers select
                                         new SupplierObject
                                         {
                                             Name = sp.CompanyName,
                                             SupplierId = sp.SupplierId,
                                             TIN = sp.TIN
                                         }).ToList();

                    return suppliers;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SupplierObject>();
            }
        }
    }
}

