using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreItemUploadRepository
    {
        private readonly IShopkeeperRepository<StoreItem> _repository;
        private readonly UnitOfWork _uoWork;
        private readonly ShopKeeperStoreEntities _dbStoreEntities = new ShopKeeperStoreEntities();
        public StoreItemUploadRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _dbStoreEntities = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(_dbStoreEntities);
           _repository = new ShopkeeperRepository<StoreItem>(_uoWork);
		}
        public List<long> ReadExcelData(string filePath, string sheetName, ref List<long> errorList, ref string msg)
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

            var selectString = @"SELECT [Name],[Parent_Product],[Product_Type],[Product_Brand],[Product_Category] FROM [" + sheetName + "$]";
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
                    var productName = dv[i].Row["Name"].ToString().Trim();
                    if (productName.Trim().Length < 3) { continue; }
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
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
        private long ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return 0; }
            try
            {
                using (var db = _dbStoreEntities)
                {
                    var mInfo = new StoreItem
                    {
                        Name = dv.Row["Name"].ToString().Trim()
                    };
                    var duplicates = db.StoreItems.Count(m => m.Name.ToLower().Trim() == mInfo.Name.ToLower().Trim());
                    if (duplicates > 0)
                    {
                        return 0;
                    }
                    var parentProductStr = dv.Row["Parent_Product_Name"].ToString().Trim();
                    if (!string.IsNullOrEmpty(parentProductStr))
                    {
                        var parentItems = db.StoreItems.Where(m => m.Name.ToLower().Trim() == parentProductStr.ToLower().Trim()).ToList();
                        if (parentItems.Any())
                        {
                            var parentItemId = parentItems[0].StoreItemId;
                            if (parentItemId > 0)
                            {
                                mInfo.ParentItemId = parentItemId;
                            }
                        }
                    }

                    var productTypeStr = dv.Row["Product_Type"].ToString().Trim();

                    if (!string.IsNullOrEmpty(productTypeStr))
                    {
                        var productTypes = db.StoreItemTypes.Where(m => m.Name.ToLower().Trim() == productTypeStr.ToLower().Trim()).ToList();
                        if (productTypes.Any())
                        {
                            var productType = productTypes[0];
                            if (productType != null && productType.StoreItemTypeId > 0)
                            {
                                mInfo.StoreItemTypeId = productType.StoreItemTypeId;
                            }
                        }
                        else
                        {
                            var productType = new StoreItemType { Name = productTypeStr.Trim() };
                            var processedProductType = db.StoreItemTypes.Add(productType);
                            db.SaveChanges();
                            if (processedProductType.StoreItemTypeId > 0)
                            {
                                mInfo.StoreItemTypeId = processedProductType.StoreItemTypeId;
                            }
                            else
                            {
                                msg = "Product Type Information could not be added.";
                                return 0;
                            }
                        }
                    }
                    else
                    {
                        msg = "Product Type is empty.";
                        return 0;
                    }

                    var productBrandStr = dv.Row["Product_Brand"].ToString().Trim();

                    if (!string.IsNullOrEmpty(productBrandStr))
                    {
                        var productTypes = db.StoreItemTypes.Where(m => m.Name.ToLower().Trim() == productBrandStr.ToLower().Trim()).ToList();
                        if (productTypes.Any())
                        {
                            var productType = productTypes[0];
                            if (productType != null && productType.StoreItemTypeId > 0)
                            {
                                mInfo.StoreItemTypeId = productType.StoreItemTypeId;
                            }
                        }
                        else
                        {
                            var productBrand = new StoreItemBrand { Name = productBrandStr.Trim() };
                            var processedProductBrand = db.StoreItemBrands.Add(productBrand);
                            db.SaveChanges();
                            if (processedProductBrand.StoreItemBrandId > 0)
                            {
                                mInfo.StoreItemBrandId = processedProductBrand.StoreItemBrandId;
                            }
                            else
                            {
                                msg = "Product Brand Information could not be added.";
                                return 0;
                            }
                        }
                    }
                    else
                    {
                        msg = "Product Brand is empty.";
                        return 0;
                    }

                    var productCategoryStr = dv.Row["Product_Category"].ToString().Trim();

                    if (!string.IsNullOrEmpty(productCategoryStr))
                    {
                        var productCategories = db.StoreItemCategories.Where(m => m.Name.ToLower().Trim() == productCategoryStr.ToLower().Trim()).ToList();
                        if (productCategories.Any())
                        {
                            var productCategory = productCategories[0];
                            if (productCategory != null && productCategory.StoreItemCategoryId > 0)
                            {
                                mInfo.StoreItemTypeId = productCategory.StoreItemCategoryId;
                            }
                        }
                        else
                        {
                            var productCategory = new StoreItemCategory { Name = productCategoryStr.Trim() };
                            var processedProductCategory = db.StoreItemCategories.Add(productCategory);
                            db.SaveChanges();
                            if (processedProductCategory.StoreItemCategoryId > 0)
                            {
                                mInfo.StoreItemCategoryId = processedProductCategory.StoreItemCategoryId;
                            }
                            else
                            {
                                msg = "Product Category is empty.";
                                return 0;
                            }
                        }
                    }
                    else
                    {
                        msg = "Product Category is empty.";
                        return 0;
                    }

                    var processedProduct = db.StoreItems.Add(mInfo);
                    db.SaveChanges();
                    return processedProduct.StoreItemId;
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
