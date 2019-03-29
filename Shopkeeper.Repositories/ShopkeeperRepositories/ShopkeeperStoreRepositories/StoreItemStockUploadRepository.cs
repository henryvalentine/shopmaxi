using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreItemStockUploadRepository
    {
        private string _connectionString = "";
        public StoreItemStockUploadRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _connectionString = connectionString;
		}
        public List<GenericValidator> ReadExcelData(string filePath, string sheetName, long currencyId, int outletId)
        {
            var feedBackList = new List<GenericValidator>();
            var gVal = new GenericValidator();
            if (filePath.Length < 3 || new FileInfo(filePath).Exists == false || (Path.GetExtension(filePath) != ".xls" && Path.GetExtension(filePath) != ".xlsx"))
            {
                gVal.Code = -1;
                gVal.Error = "Invalid Excel File Format";
                feedBackList.Add(gVal);
                return feedBackList;
            }

            if (sheetName.Length < 1)
            {
                gVal.Code = -1;
                gVal.Error = "Invalid Excel Sheet Name";
                feedBackList.Add(gVal);
                return feedBackList;
            }

            var connectionstring = string.Empty;
            switch (Path.GetExtension(filePath))
            {
                case ".xls":
                    connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;'";
                    break;
                case ".xlsx":
                    connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;ImportMixedTypes=Text'";
                    break;
            }

            if (connectionstring == "")
            {
                gVal.Code = -1;
                gVal.Error = "Process Error! Please try again later";
                feedBackList.Add(gVal);
                return feedBackList;
            }

            var selectString = @"SELECT  [Code/SKU],[Product_Name/Model],[Brand(eg: Samsung)],[Category(eg: Monitor)],[Type(Eg: Opticals)],[Quantity_In_Stock],[Distinguishing_Property(eg: Color, Size, Weight)],[Distinguishing_Value(eg: Black, medium, 15kg)],[Unit_of_measurement_Code(eg: Kg)],[Expiry_Date(yyyy/MM/dd)],[Cost_Price(optional)], [Price_1], [Quantity_1], [Price_2], [Quantity_2], [Price_3], [Quantity_3], [Reorder_Quantity(optional)], [Reorder_Level(optional)], [Description(optional)], [Technical_Specifications(optional)] FROM [" + sheetName + "$]";
            var myCon = new OleDbConnection(connectionstring);
            try
            {
                if (myCon.State == ConnectionState.Closed)
                {
                    myCon.Open();
                }
                var cmd = new OleDbCommand(selectString, myCon);
                var adap = new OleDbDataAdapter(cmd);
                var ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables.Count < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid Product Template!";
                    feedBackList.Add(gVal);
                    return feedBackList;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid Product Template!";
                    feedBackList.Add(gVal);
                    return feedBackList;
                }

                for (var i = 0; i < dv.Count; i++)
                {
                    var productCode = dv[i].Row["Code/SKU"].ToString().Trim();
                    var productNameModel = dv[i].Row["Product_Name/Model"].ToString().Trim();
                    var mInfo = ProcessRecord(dv[i], currencyId, outletId);
                    if (mInfo.Code < 1)
                    {
                        mInfo.Name = productNameModel;
                        mInfo.SKU = productCode;
                        feedBackList.Add(mInfo);
                        continue;
                    }
                    feedBackList.Add(mInfo);
                }
                myCon.Close();
                return feedBackList;
            }
            catch (Exception ex)
            {
                myCon.Close();
                gVal.Code = -1;
                gVal.Error = ex.Message;
                feedBackList.Add(gVal);
                return feedBackList;
            }
        }

        private GenericValidator ProcessRecord(DataRowView dv, long currencyId, int outletId)
        {
            var gVal = new GenericValidator();

            if (dv == null)
            {
                gVal.Code = -1;
                gVal.Error = "An unknown error was encountered.";
                return gVal;
            }
            try
            {
                var mInfo = new StoreItemStock
                {
                    StoreOutletId = outletId
                };


                var productCode = dv.Row["Code/SKU"].ToString().Trim();
                var productNameModel = dv.Row["Product_Name/Model"].ToString().Trim();

                StoreItemStock existingStock;
                StoreItem existingItem;

                if (!string.IsNullOrEmpty(productNameModel))
                {
                    existingItem = VerifyExistingProduct(productNameModel, productCode, out existingStock);
                }

                else
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Product Name/Model.";
                    return gVal;
                }

                if (string.IsNullOrEmpty(productCode))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Product Code/SKU.";
                    return gVal;
                }

                mInfo.SKU = productCode;

                var product = new StoreItem
                {
                    Name = productNameModel
                };

                #region brand category type
                var brandStr = dv.Row["Brand(eg: Samsung)"].ToString().Trim();
                if (string.IsNullOrEmpty(brandStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Product Brand.";
                    return gVal;
                }
                var brand = AddBrand(new StoreItemBrand { Name = brandStr });
                product.StoreItemBrandId = brand.StoreItemBrandId;
                existingItem.StoreItemBrandId = brand.StoreItemBrandId;

                var categoryStr = dv.Row["Category(eg: Monitor)"].ToString().Trim();
                if (string.IsNullOrEmpty(categoryStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Product Category.";
                    return gVal;
                }

                var category = ProcessCategory(new StoreItemCategory { Name = categoryStr, LastUpdated = DateTime.Now });
                product.StoreItemCategoryId = category.StoreItemCategoryId;
                existingItem.StoreItemCategoryId = category.StoreItemCategoryId;

                var typeStr = dv.Row["Type(Eg: Opticals)"].ToString().Trim();
                if (string.IsNullOrEmpty(typeStr))
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Product Type.";
                    return gVal;
                }
                var type = AddType(new StoreItemType { Name = typeStr });
                product.StoreItemTypeId = type.StoreItemTypeId;
                existingItem.StoreItemTypeId = type.StoreItemTypeId;
                #endregion

                #region generic
                var decscription = dv.Row["Description(optional)"].ToString().Trim();
                if (!string.IsNullOrEmpty(decscription))
                {
                    product.Description = decscription;
                    existingItem.Description = decscription;
                }

                var techStr = dv.Row["Technical_Specifications(optional)"].ToString().Trim();
                if (!string.IsNullOrEmpty(techStr))
                {
                    product.TechSpechs = techStr;
                    existingItem.TechSpechs = techStr;
                }

                var quantityStr = dv.Row["Quantity_In_Stock"].ToString().Trim();
                if (!string.IsNullOrEmpty(quantityStr))
                {
                    double quantity;
                    var res = double.TryParse(quantityStr, out quantity);
                    if (res && quantity > 0)
                    {
                        mInfo.QuantityInStock = quantity;
                        existingStock.QuantityInStock = quantity;
                    }
                    else
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide a valid value for Quantity in Stock.";
                        return gVal;
                    }
                }
                else
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Quantity in Stock.";
                    return gVal;
                }

                var expiryDateStr = dv.Row["Expiry_Date(yyyy/MM/dd)"].ToString().Trim();

                if (!string.IsNullOrEmpty(expiryDateStr))
                {
                    DateTime expiryDate;
                    var res = DateTime.TryParse(expiryDateStr, out expiryDate);
                    if (res)
                    {
                        mInfo.ExpirationDate = expiryDate;
                    }
                }

                var costStr = dv.Row["Cost_Price(optional)"].ToString().Trim();

                if (!string.IsNullOrEmpty(costStr) && costStr != "0")
                {
                    double costPrice;
                    var res = double.TryParse(costStr, out costPrice);
                    if (res && costPrice > 0)
                    {
                        mInfo.CostPrice = costPrice;
                        existingStock.CostPrice = costPrice;
                    }
                    else
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide a valid value for Cost Price.";
                        return gVal;
                    }
                }

                #endregion

                #region price setup

                var priceList = new List<ItemPrice>();

                var priceStr1 = dv.Row["Price_1"].ToString().Trim();
                var priceStr2 = dv.Row["Price_2"].ToString().Trim();
                var priceStr3 = dv.Row["Price_3"].ToString().Trim();
                var qtyStr1 = dv.Row["Quantity_1"].ToString().Trim();
                var qtyStr2 = dv.Row["Quantity_2"].ToString().Trim();
                var qtyStr3 = dv.Row["Quantity_3"].ToString().Trim();

                var uomStr = dv.Row["Unit_of_measurement_Code(eg: Kg)"].ToString().Trim();

                long uomId = 0;

                if (!string.IsNullOrEmpty(uomStr))
                {
                    var codeRes = AddUomCode(new UnitsOfMeasurement { UoMCode = uomStr });
                    if (codeRes.UnitOfMeasurementId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Product's unit of measurement could not processed.";
                        return gVal;
                    }

                    uomId = codeRes.UnitOfMeasurementId;
                }

                if (!string.IsNullOrEmpty(priceStr1) || !string.IsNullOrEmpty(priceStr2) || !string.IsNullOrEmpty(priceStr3))
                {
                    if (uomId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide Product's unit of measurement.";
                        return gVal;
                    }

                }

                if (!string.IsNullOrEmpty(priceStr1))
                {
                    if (string.IsNullOrEmpty(qtyStr1))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide Quantity_1.";
                        return gVal;
                    }

                    double price1;
                    var pr1Res = double.TryParse(priceStr1, out price1);
                    if (pr1Res && price1 > 0)
                    {
                        double quantity1;
                        var qty1Res = double.TryParse(qtyStr1, out quantity1);
                        if (qty1Res && quantity1 > 0)
                        {
                            var itemPrice1 = new ItemPrice
                            {
                                StoreItemStockId = 0,
                                Price = price1,
                                MinimumQuantity = quantity1,
                                UoMId = uomId
                            };

                            priceList.Add(itemPrice1);
                        }
                    }
                }


                if (!string.IsNullOrEmpty(priceStr2))
                {
                    if (string.IsNullOrEmpty(qtyStr2))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide Quantity_2.";
                        return gVal;
                    }

                    double price2;
                    var pr2Res = double.TryParse(priceStr2, out price2);
                    if (pr2Res && price2 > 0)
                    {
                        double quantity2;
                        var qty2Res = double.TryParse(qtyStr2, out quantity2);
                        if (qty2Res && quantity2 > 0)
                        {
                            var itemPrice2 = new ItemPrice
                            {
                                StoreItemStockId = 0,
                                Price = price2,
                                MinimumQuantity = quantity2,
                                UoMId = uomId
                            };

                            priceList.Add(itemPrice2);
                        }
                    }
                }


                if (!string.IsNullOrEmpty(priceStr3))
                {
                    if (string.IsNullOrEmpty(qtyStr3))
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please provide Quantity_3.";
                        return gVal;
                    }

                    double price3;
                    var pr3Res = double.TryParse(priceStr3, out price3);
                    if (pr3Res && price3 > 0)
                    {
                        double quantity3;
                        var qty3Res = double.TryParse(qtyStr3, out quantity3);
                        if (qty3Res && quantity3 > 0)
                        {
                            var itemPrice3 = new ItemPrice
                            {
                                StoreItemStockId = 0,
                                Price = price3,
                                MinimumQuantity = quantity3,
                                UoMId = uomId
                            };

                            priceList.Add(itemPrice3);
                        }
                    }
                }

                #endregion

                #region others
                var reorderQuantitySoldStr = dv.Row["Reorder_Quantity(optional)"].ToString().Trim();
                if (!string.IsNullOrEmpty(reorderQuantitySoldStr))
                {
                    int reorderQuantity;
                    var res = int.TryParse(reorderQuantitySoldStr, out reorderQuantity);
                    if (res && reorderQuantity > 0)
                    {
                        mInfo.ReorderQuantity = reorderQuantity;
                        existingStock.ReorderQuantity = reorderQuantity;
                    }

                }

                var reorderLevelStr = dv.Row["Reorder_Level(optional)"].ToString().Trim();
                if (!string.IsNullOrEmpty(reorderLevelStr))
                {
                    int reorderLevel;
                    var res = int.TryParse(reorderLevelStr, out reorderLevel);
                    if (res && reorderLevel > 0)
                    {
                        mInfo.ReorderLevel = reorderLevel;
                        existingStock.ReorderLevel = reorderLevel;
                    }

                }

                var productVariationPropStr = dv.Row["Distinguishing_Property(eg: Color, Size, Weight)"].ToString().Trim();

                if (!string.IsNullOrEmpty(productVariationPropStr))
                {
                    var resP = ProcessVariation(productVariationPropStr);
                    if (resP < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Product Distinguishing Property Information could not be Processed.";
                        return gVal;
                    }

                    mInfo.StoreItemVariationId = resP;
                    existingStock.StoreItemVariationId = resP;
                }
                else
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Product Distinguishing Property.";
                    return gVal;
                }

                var productVariationValueStr = dv.Row["Distinguishing_Value(eg: Black, medium, 15kg)"].ToString().Trim();

                if (!string.IsNullOrEmpty(productVariationValueStr))
                {
                    var res = ProcessValue(productVariationValueStr);
                    if (res < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Product Distinguishing Value could not be processed.";
                        return gVal;
                    }

                    mInfo.StoreItemVariationValueId = res;
                    existingStock.StoreItemVariationValueId = res;
                }
                else
                {
                    gVal.Code = -1;
                    gVal.Error = "Please provide Product Distinguishing Value.";
                    return gVal;
                }
                #endregion

                #region process stock
                using (var db = new ShopKeeperStoreEntities("name=ShopKeeperStoreEntities"))
                {
                    StoreItem processedProduct;
                    StoreItemStock processedProductStock;
                    if (existingItem.StoreItemId > 0)
                    {
                        db.Entry(existingItem).State = EntityState.Modified;
                        db.SaveChanges();
                        processedProduct = existingItem;

                        mInfo.LastUpdated = DateTime.Now;
                        mInfo.StoreCurrencyId = currencyId;
                        mInfo.StoreItemId = processedProduct.StoreItemId;

                        if (existingStock.StoreItemStockId < 1)
                        {
                            processedProductStock = db.StoreItemStocks.Add(mInfo);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(existingStock).State = EntityState.Modified;
                            db.SaveChanges();
                            processedProductStock = existingStock;
                        }

                    }
                    else
                    {
                        processedProduct = db.StoreItems.Add(product);
                        db.SaveChanges();

                        mInfo.LastUpdated = DateTime.Now;
                        mInfo.StoreCurrencyId = currencyId;
                        mInfo.StoreItemId = processedProduct.StoreItemId;
                        processedProductStock = db.StoreItemStocks.Add(mInfo);
                        db.SaveChanges();
                    }

                    if (priceList.Any())
                    {
                        ProcessPrices(priceList, processedProductStock.StoreItemStockId);
                    }

                    gVal.Code = processedProductStock.StoreItemStockId;
                    return gVal;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = ex.Message;
                return gVal;
            }

        }

        public int GetOutlet()
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    var storeOutlets = db.StoreOutlets.ToList();
                    if (storeOutlets.Any())
                    {
                        var outletId = storeOutlets.Find(s => s.IsMainOutlet).StoreOutletId;
                        if (outletId < 1)
                        {
                            return storeOutlets[0].StoreOutletId;
                        }

                        return outletId;
                    }

                    return -2;
                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return -2;
            }
        }

        public StoreItem VerifyExistingProduct(string productNameModel, string productCode, out StoreItemStock existingStock)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    var existingProducts = db.StoreItems.Where(m => m.Name.ToLower().Trim() == productNameModel.ToLower().Trim()).ToList();
                    if (existingProducts.Any())
                    {
                        var existingItem = existingProducts[0];
                        var stockItems = db.StoreItemStocks.Where(s => s.StoreItemId == existingItem.StoreItemId && s.SKU.ToLower() == productCode.ToLower()).ToList();

                        if (!stockItems.Any())
                        {
                            existingStock = new StoreItemStock();
                            return existingItem;
                        }

                        existingStock = stockItems[0];
                        return existingItem;
                    }
                    existingStock = new StoreItemStock();
                    return new StoreItem();
                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                existingStock = new StoreItemStock();
                return new StoreItem();
            }
        }

        public int ProcessValue(string productVariationValueStr)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    var productVariationValues = db.StoreItemVariationValues.Where(m => m.Value.ToLower().Trim() == productVariationValueStr.ToLower().Trim()).ToList();
                    if (productVariationValues.Any())
                    {
                        var productVariationValue = productVariationValues[0];
                        return productVariationValue.StoreItemVariationValueId;
                    }
                    else
                    {
                        var productVariationValue = new StoreItemVariationValue { Value = productVariationValueStr.Trim() };
                        var processedVariationValue = db.StoreItemVariationValues.Add(productVariationValue);
                        db.SaveChanges();
                        return processedVariationValue.StoreItemVariationValueId;
                    }

                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return -2;
            }
        }

        public int ProcessVariation(string productVariationPropStr)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    var productVariations = db.StoreItemVariations.Where(m => m.VariationProperty.ToLower().Trim() == productVariationPropStr.ToLower().Trim()).ToList();
                    if (productVariations.Any())
                    {
                        var productVariation = productVariations[0];
                        return productVariation.StoreItemVariationId;
                    }
                    else
                    {
                        var productVariation = new StoreItemVariation { VariationProperty = productVariationPropStr.Trim() };
                        var processedVariation = db.StoreItemVariations.Add(productVariation);
                        db.SaveChanges();
                        return processedVariation.StoreItemVariationId;
                    }

                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return -2;
            }
        }

        public UnitsOfMeasurement AddUomCode(UnitsOfMeasurement uom)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    UnitsOfMeasurement uomInfo;
                    var uoms = db.UnitsOfMeasurements.Where(u => u.UoMCode.ToLower() == uom.UoMCode.ToLower()).ToList();
                    if (uoms.Any())
                    {
                        uomInfo = uoms[0];
                    }
                    else
                    {
                        var processeduom = db.UnitsOfMeasurements.Add(uom);
                        db.SaveChanges();
                        uomInfo = processeduom;
                    }

                    return uomInfo;
                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return new UnitsOfMeasurement();
            }
        }

        public StoreItemType AddType(StoreItemType storeType)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    StoreItemType storeTypeInfo;
                    var types = db.StoreItemTypes.Where(b => b.Name.Trim().ToLower().Replace(" ", "") == storeType.Name.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!types.Any())
                    {
                        var processedType = db.StoreItemTypes.Add(storeType);
                        db.SaveChanges();
                        storeTypeInfo = processedType;
                    }
                    else
                    {
                        storeTypeInfo = types[0];
                    }

                    return storeTypeInfo;
                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return new StoreItemType();
            }
        }

        public StoreItemCategory ProcessCategory(StoreItemCategory category)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    StoreItemCategory categoryInfo;
                    var categories = db.StoreItemCategories.Where(b => b.Name.Trim().ToLower().Replace(" ", "") == category.Name.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!categories.Any())
                    {
                        var processedCategory = db.StoreItemCategories.Add(category);
                        db.SaveChanges();
                        categoryInfo = processedCategory;
                    }
                    else
                    {
                        categoryInfo = categories[0];
                    }

                    return categoryInfo;
                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return new StoreItemCategory();
            }
        }

        public StoreItemBrand AddBrand(StoreItemBrand brand)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    StoreItemBrand brandInfo;
                    var brands = db.StoreItemBrands.Where(b => b.Name.Trim().ToLower().Replace(" ", "") == brand.Name.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!brands.Any())
                    {
                        brand.LastUpdated = DateTime.Now;
                        var processedBrand = db.StoreItemBrands.Add(brand);
                        db.SaveChanges();
                        brandInfo = processedBrand;
                    }
                    else
                    {
                        brandInfo = brands[0];
                    }
                    return brandInfo;
                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return new StoreItemBrand();
            }
        }

        public bool ProcessPrices(List<ItemPrice> priceList, long stockId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    if (priceList.Any())
                    {
                        var existingPriceList = db.ItemPrices.Where(i => i.StoreItemStockId == stockId).ToList();

                        priceList.ForEach(p =>
                        {
                            var matchFound = false;
                            if (existingPriceList.Any())
                            {
                                var pr = existingPriceList.Find(f => f.Price.Equals(p.Price));
                                if (pr != null && pr.ItemPriceId > 0)
                                {
                                    if (!pr.MinimumQuantity.Equals(p.MinimumQuantity))
                                    {
                                        pr.MinimumQuantity = p.MinimumQuantity;
                                        db.Entry(pr).State = EntityState.Modified;
                                        db.SaveChanges();
                                        matchFound = true;
                                    }
                                    else
                                    {
                                        matchFound = true;
                                    }
                                }
                                else
                                {
                                    var prq = existingPriceList.Find(f => f.MinimumQuantity.Equals(p.MinimumQuantity));
                                    if (prq != null && prq.ItemPriceId > 0)
                                    {
                                        if (!prq.Price.Equals(p.Price))
                                        {
                                            prq.Price = p.Price;
                                            db.Entry(prq).State = EntityState.Modified;
                                            db.SaveChanges();
                                            matchFound = true;
                                        }
                                        else
                                        {
                                            matchFound = true;
                                        }
                                    }
                                }
                            }

                            if (!matchFound || !existingPriceList.Any())
                            {
                                p.StoreItemStockId = stockId;
                                db.ItemPrices.Add(p);
                                db.SaveChanges();
                            }
                        });

                        return true;
                    }
                    return false;
                }
            }
            catch (DbEntityValidationException e)
            {
                var str = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    str += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State) + "\n";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        str += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage) + " \n";
                    }
                }

                ErrorLogger.LogError(e.StackTrace, e.Source, str);
                return true;
            }
        }

        public List<long> EditExcelData(string filePath, string sheetName, ref List<long> errorList, ref string msg)
        {
            if (filePath.Length < 3 || new FileInfo(filePath).Exists == false || (Path.GetExtension(filePath) != ".xls" && Path.GetExtension(filePath) != ".xlsx"))
            {
                msg = "Invalid Excel File Format";
                errorList = new List<long>();
                return new List<long>();
            }

            if (sheetName.Length < 1)
            {
                msg = "Invalid Excel Sheet Name";
                errorList = new List<long>();
                return new List<long>();
            }

            var connectionstring = string.Empty;
            switch (Path.GetExtension(filePath))
            {
                case ".xls":
                    connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;'";
                    break;
                case ".xlsx":
                    connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;ImportMixedTypes=Text'";
                    break;
            }

            if (connectionstring == "")
            {
                msg = "Process Error! Please try again later";
                errorList = new List<long>();
                return new List<long>();
            }

            var selectString = @"SELECT [SKU],[Quantity_In_Stock],[Expiry_Date(yyyy/MM/dd)] FROM [" + sheetName + "$]";
            var myCon = new OleDbConnection(connectionstring);
            try
            {
                if (myCon.State == ConnectionState.Closed)
                {
                    myCon.Open();
                }
                var cmd = new OleDbCommand(selectString, myCon);
                var adap = new OleDbDataAdapter(cmd);
                var ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables.Count < 1)
                {
                    msg = "Invalid Product Template!";
                    errorList = new List<long>();
                    return new List<long>();
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Product Template!";
                    errorList = new List<long>();
                    return new List<long>();
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Product Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                var successList = new List<long>();
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                    var productName = dv[i].Row["SKU"].ToString().Trim();
                    if (productName.Trim().Length < 3) { continue; }
                    var mInfo = ProcessData(dv[i], ref mymsg);
                    if (mInfo < 1)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", productName,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Unknown Error</td></tr>",
                                              productName));
                        errorList.Add(1);
                        continue;
                    }
                    successList.Add(mInfo);
                }
                sb.AppendLine("</table>");
                if (errorExist)
                {
                    var sbb = new StringBuilder();
                    sbb.AppendLine("Following error occurred while loading your data template:");
                    sbb.AppendLine(sb.ToString());
                    msg = sbb.ToString();
                }
                myCon.Close();
                return successList;
            }
            catch (Exception ex)
            {
                myCon.Close();
                msg = ex.Message;
                errorList = new List<long>();
                return new List<long>();
            }
        }

        private long ProcessData(DataRowView dv, ref string msg)
        {
            if (dv == null) { return 0; }
            try
            {
                using (var db = new ShopKeeperStoreEntities(_connectionString))
                {
                    var mInfo = new StoreItemStock();

                    var sku = dv.Row["SKU"].ToString().Trim();

                    if (!string.IsNullOrEmpty(sku))
                    {
                        var result = db.StoreItemStocks.Where(m => m.SKU == sku.Trim()).ToList();
                        if (result.Any())
                        {
                            mInfo = result[0];
                        }
                        else
                        {
                            msg = "Product could not be found.";
                            return 0;
                        }
                    }
                    else
                    {
                        msg = "Please provide SKU.";
                        return 0;
                    }

                    var quantityInStockStr = dv.Row["Quantity_In_Stock"].ToString().Trim();

                    if (!string.IsNullOrEmpty(quantityInStockStr))
                    {
                        int quantity;
                        var res = int.TryParse(quantityInStockStr, out quantity);
                        if (res && quantity > 0)
                        {
                            mInfo.QuantityInStock = quantity;
                        }
                        else
                        {
                            msg = "Please provide a valid value for Quantity in Stock.";
                            return 0;
                        }
                    }
                    else
                    {
                        msg = "Please provide Quantity in Stock.";
                        return 0;
                    }

                    var expiryDateStr = dv.Row["Expiry_Date(yyyy/MM/dd)"].ToString().Trim();

                    if (!string.IsNullOrEmpty(expiryDateStr))
                    {
                        DateTime expiryDate;
                        var res = DateTime.TryParse(expiryDateStr, out expiryDate);
                        if (res)
                        {
                            mInfo.ExpirationDate = expiryDate;
                        }
                    }

                    var processedProductStock = db.StoreItemStocks.Attach(mInfo);
                    db.Entry(mInfo).State = EntityState.Modified;
                    db.SaveChanges();
                    return processedProductStock.StoreItemStockId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }

        }
    }

}
