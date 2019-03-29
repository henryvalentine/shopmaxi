using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;
using WebGrease.Css.Extensions;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
    public class StoreItemStockController : Controller
    {
        #region Actions

        /// <summary>
        /// Handles calls Ajax from DataTable(to which the Facilities List is/to be bound)
        /// </summary>
        /// <param name="param">
        /// Ajax model that encapsulates all required parameters such as 
        /// filtering, pagination, soting, etc instructions from the DataTable
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStoreItemStockObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreItemStockObject> filteredStoreItemStockObjects;
                int countG;

                var pagedStoreItemStockObjects = GetStoreItemStocks(param.iDisplayLength, param.iDisplayStart, out countG);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreItemStockObjects = new StoreItemStockServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreItemStockObjects = pagedStoreItemStockObjects;
                }

                if (!filteredStoreItemStockObjects.Any())
                {
                    return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreItemStockObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreItemName : sortColumnIndex == 2 ? c.SKU : sortColumnIndex == 3 ? c.CategoryName : c.BrandName);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreItemStockObjects = sortDirection == "asc" ? filteredStoreItemStockObjects.OrderBy(orderingFunction) : filteredStoreItemStockObjects.OrderByDescending(orderingFunction);


                var result = from c in filteredStoreItemStockObjects select new[] { Convert.ToString(c.StoreItemStockId), c.StoreItemName, c.SKU, c.CategoryName, c.BrandName, c.QuantityInStockStr, c.QuantitySoldStr, c.ExpiryDate };

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = countG,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreItemStockObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddStoreItemStock(StoreItemStockObject storeItemStock, string subdomain)
        {
            var gVal = new GenericValidator();
            try
            {
                //if (ModelState.IsValid)
                //{

                var validationStatus = ValidateStoreItem(storeItemStock.StoreItemObject);
                if (validationStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = validationStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var valStatus = ValidateStoreItemStock(storeItemStock);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                storeItemStock.LastUpdated = DateTime.Now;

                //todo: Get Outlet Id, Store it in session and use it here
                //storeItemStock.StoreOutletId = 1;

                var k = new StoreItemStockServices().AddStoreItemStock(storeItemStock);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    gVal.Code = -1;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(storeItemStock.ImagePath))
                {
                    var newStockImage = new StockUploadObject
                    {
                        ImagePath = storeItemStock.ImagePath,
                        ImageViewId = (int)DefaultImageView.Default_View,
                        StoreItemStockId = k
                    };
                    var x = new StockUploadServices().AddStockUpload(newStockImage);
                    if (x < 1)
                    {
                        gVal.Error = x == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = -1;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }

                gVal.Code = k;
                gVal.Error = message_Feedback.Insertion_Success;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = 0;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddStoreItemStockVariants(List<StoreItemStockObject> productVariantList)
        {
            var gVal = new GenericValidator();
            try
            {
                if (!productVariantList.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Product_Variant_List_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var successList = new List<long>();
                var priceErorList = new List<GenericValidator>();
                var errorList = new List<StoreItemStockObject>();
                productVariantList.ForEach(storeItemStock =>
                {
                    //if (ModelState.IsValid)
                    //{
                    var valStatus = ValidateStoreItemStock(storeItemStock);
                    if (valStatus.Code < 1)
                    {
                        storeItemStock.FeedbackMessage = valStatus.Error;
                        errorList.Add(storeItemStock);
                    }
                    else
                    {
                        storeItemStock.LastUpdated = DateTime.Now;
                        //TODO: Use dynamic Value for StoreOutletId
                        storeItemStock.StoreOutletId = 1;
                        var k = new StoreItemStockServices().AddStoreItemStock(storeItemStock);
                        if (k < 1)
                        {
                            storeItemStock.FeedbackMessage = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                            errorList.Add(storeItemStock);
                        }
                        else
                        {
                            successList.Add(k);

                            if (storeItemStock.UnitOfMeasurementId > 0)
                            {
                                var itemPrice = new ItemPriceObject
                                {
                                    StoreItemStockId = k,
                                    Price = storeItemStock.Price,
                                    MinimumQuantity = 1,
                                    UoMId = storeItemStock.UnitOfMeasurementId
                                };

                                var x = new ItemPriceServices().AddItemPrice(itemPrice);
                                if (x < 1)
                                {
                                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                                    gVal.Code = -1;
                                    priceErorList.Add(gVal);
                                }
                            }

                            if (!string.IsNullOrEmpty(storeItemStock.ImagePath))
                            {
                                var newStockImage = new StockUploadObject
                                {
                                    ImagePath = storeItemStock.ImagePath,
                                    ImageViewId = (int)DefaultImageView.Default_View,
                                    StoreItemStockId = k
                                };
                                //Evaluate the result
                                new StockUploadServices().AddStockUpload(newStockImage);
                            }
                        }
                    }
                    //}
                    //else
                    //{
                    //    storeItemStock.FeedbackMessage = message_Feedback.Model_State_Error;
                    //    errorList.Add(storeItemStock);
                    //}
                });

                if (successList.Any())
                {
                    gVal.Error = successList.Count + " Items were added successfully.";
                }
                gVal.ErrorObjects = new List<ErrorObject>();
                if (errorList.Any())
                {

                    errorList.ForEach(m =>
                    {
                        var errorObject = new ErrorObject
                        {
                            SKU = m.SKU,
                            VariationProperty = m.VariationProperty,
                            VariationValue = m.VariationValue,
                            ErrorMessage = m.FeedbackMessage
                        };
                        if (!string.IsNullOrEmpty(m.ImagePath))
                        {
                            DeleteFile(m.ImagePath);
                        }
                        gVal.ErrorObjects.Add(errorObject);
                    });

                    gVal.Error += "The following Item(s) could not be added due to the error specified below.";
                    gVal.Code = -1;
                }
                else
                {
                    gVal.Code = 5;
                }

                return Json(gVal, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditStoreItemStock(StoreItemStockObject storeItemStock)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateStoreItemStock(storeItemStock);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_storeItemStock"] == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var oldStoreItemStock = Session["_storeItemStock"] as StoreItemStockObject;
                if (oldStoreItemStock == null || oldStoreItemStock.StoreItemStockId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                oldStoreItemStock.StoreItemObject.Name = storeItemStock.StoreItemObject.Name.Trim();
                oldStoreItemStock.StoreItemObject.Description = storeItemStock.StoreItemObject.Description;
                oldStoreItemStock.StoreItemObject.TechSpechs = storeItemStock.StoreItemObject.TechSpechs;
                oldStoreItemStock.StoreItemObject.StoreItemBrandId = storeItemStock.StoreItemObject.StoreItemBrandId;
                oldStoreItemStock.StoreItemObject.StoreItemTypeId = storeItemStock.StoreItemObject.StoreItemTypeId;
                oldStoreItemStock.StoreItemObject.StoreItemCategoryId = storeItemStock.StoreItemObject.StoreItemCategoryId;
                oldStoreItemStock.StoreItemObject.ChartOfAccountId = storeItemStock.StoreItemObject.ChartOfAccountId;

                oldStoreItemStock.SKU = storeItemStock.SKU;
                oldStoreItemStock.StoreItemVariationId = storeItemStock.StoreItemVariationId;
                oldStoreItemStock.StoreItemVariationValueId = storeItemStock.StoreItemVariationValueId;
                oldStoreItemStock.StoreItemId = storeItemStock.StoreItemId;
                oldStoreItemStock.QuantityInStock = storeItemStock.QuantityInStock;
                oldStoreItemStock.CostPrice = storeItemStock.CostPrice;
                oldStoreItemStock.ReorderLevel = storeItemStock.ReorderLevel;
                oldStoreItemStock.ReorderQuantity = storeItemStock.ReorderQuantity;
                oldStoreItemStock.LastUpdated = DateTime.Now;
                oldStoreItemStock.ShelfLocation = storeItemStock.ShelfLocation;
                oldStoreItemStock.ExpirationDate = storeItemStock.ExpirationDate;

                var k = new StoreItemStockServices().UpdateStoreItemStock(oldStoreItemStock);
                if (k < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                Session["_storeItemStock"] = oldStoreItemStock;
                gVal.Code = 5;
                gVal.Error = message_Feedback.Update_Success;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditInventory(StoreItemStockObject storeItemStock)
        {
            var gVal = new GenericValidator();
            try
            {
                //if (ModelState.IsValid)
                //{
                var priceErorList = new List<GenericValidator>();
                var valStatus = ValidateInventory(storeItemStock);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_storeItemStock"] == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var oldStoreItemStock = Session["_storeItemStock"] as StoreItemStockObject;
                if (oldStoreItemStock == null || oldStoreItemStock.StoreItemStockId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                //if (storeItemStock.StoreCurrencyId > 0)
                //{
                //    oldStoreItemStock.StoreCurrencyId = storeItemStock.StoreCurrencyId;
                //}

                oldStoreItemStock.StoreItemVariationId = storeItemStock.StoreItemVariationId;
                oldStoreItemStock.StoreItemVariationValueId = storeItemStock.StoreItemVariationValueId;
                if (storeItemStock.QuantityInStock > 0)
                {
                    oldStoreItemStock.QuantityInStock = storeItemStock.QuantityInStock;
                }

                //oldStoreItemStock.Price = storeItemStock.Price;
                oldStoreItemStock.LastUpdated = DateTime.Now;
                oldStoreItemStock.ExpirationDate = storeItemStock.ExpirationDate;

                //if (!string.IsNullOrEmpty(storeItemStock.ImagePath))
                //{
                //    if (!string.IsNullOrEmpty(oldStoreItemStock.ImagePath))
                //    {
                //        DeleteFile(oldStoreItemStock.ImagePath);
                //    }

                //    oldStoreItemStock.ImagePath = storeItemStock.ImagePath;
                //}

                var k = new StoreItemStockServices().UpdateStoreItemStock(oldStoreItemStock);
                if (k < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                //if (storeItemStock.UnitOfMeasurementId > 0)
                //{
                //    var itemPrice = new ItemPriceObject
                //    {
                //        ItemPriceId = oldStoreItemStock.ItemPriceId,
                //        StoreItemStockId = k,
                //        Price = storeItemStock.Price,
                //        MinimumQuantity = 1,
                //        UoMId = storeItemStock.UnitOfMeasurementId
                //    };

                //    var x = new ItemPriceServices().AddItemPrice(itemPrice);
                //    if (x < 1)
                //    {
                //        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                //        gVal.Code = -1;
                //        priceErorList.Add(gVal);
                //    }
                //}
                //if the StoreItemStock from the view has StockUploads
                if (storeItemStock.StockUploadObjects != null && storeItemStock.StockUploadObjects.Any())
                {
                    var stockUploadsToAddOrUpdate = new List<StockUploadObject>();

                    // then determin if the StoreItemStock in session has StockUploads too
                    if (oldStoreItemStock.StockUploadObjects.Any())
                    {
                        storeItemStock.StockUploadObjects.ForEach(m =>
                        {
                            //if yes, any StockUpload from the View whose FileId > 0 and StockUploadId matches anyone in session is for edit

                            if (m.FileId > 0)
                            {
                                var stockImges = oldStoreItemStock.StockUploadObjects.Where(z => z.StockUploadId == m.StockUploadId).ToList();
                                if (stockImges.Any())
                                {
                                    var stockImg = stockImges[0];
                                    stockImg.StoreItemStockId = oldStoreItemStock.StoreItemStockId;
                                    stockImg.FileId = m.FileId;
                                    stockUploadsToAddOrUpdate.Add(stockImg);
                                }
                                else
                                {
                                    //else it is a fresh StockUpload
                                    m.StoreItemStockId = oldStoreItemStock.StoreItemStockId;
                                    stockUploadsToAddOrUpdate.Add(m);
                                }
                            }

                        });
                    }
                    else
                    {
                        //else all are fresh StockUploads
                        storeItemStock.StockUploadObjects.ForEach(m =>
                        {
                            if (m.FileId > 0)
                            {
                                m.StoreItemStockId = oldStoreItemStock.StoreItemStockId;
                                stockUploadsToAddOrUpdate.Add(m);
                            }
                        });
                    }

                    //If anything happened positively within the foreach loop, then stockUploadsToAddOrUpdate should contain an Object to be worked on
                    if (stockUploadsToAddOrUpdate.Any())
                    {
                        var successCount = 0;
                        storeItemStock.StockUploadObjects = stockUploadsToAddOrUpdate;
                        storeItemStock.StoreItemStockId = oldStoreItemStock.StoreItemStockId;
                        //Process the contents of stockUploadsToAddOrUpdate
                        var errorList = ProcessStockImage(storeItemStock, out successCount);
                        //Check for errors
                        if (errorList.Any())
                        {
                            //if errors abound, buld an html for the error(s) information and return it back to the view
                            var sb = new StringBuilder();
                            sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                            sb.AppendLine(string.Format("<tr><th width=\"45%\">View</th><th width=\"55%\">Error</th></tr>"));
                            errorList.ForEach(m => sb.AppendLine(string.Format("<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", m.Email, m.Error)));
                            sb.AppendLine("</table>");
                            gVal.Error = sb.ToString();
                            gVal.Code = -2;
                            Session["_imgSessList"] = null;
                            return Json(gVal, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                Session["_storeItemStock"] = null;
                Session["_imgSessList"] = null;
                gVal.Code = 5;
                gVal.Error = message_Feedback.Update_Success;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteStoreItemStock(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Invalid_Selection;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                List<string> filePathList;
                var z = new StockUploadServices().DeleteStockUploadByStorItemId(id, out filePathList);
                if (z)
                {
                    gVal.Code = 5;
                    if (filePathList.Count > 0)
                    {
                        filePathList.ForEach(m => DeleteFile(m));
                    }
                }
                var k = new StoreItemStockServices().DeleteStoreItemStock(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Delete_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = message_Feedback.Delete_Failure;
                return Json(gVal, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStoreItemStock(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
                }

                var storeItemStock = new StoreItemStockServices().GetStoreItemStockDetails(id);
                if (id < 1)
                {
                    return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
                }

                Session["_storeItemStock"] = storeItemStock;

                return Json(storeItemStock, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreItemStockObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveFile(HttpPostedFileBase file)
        {
            var gVal = new GenericValidator();
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var filePath = SaveFile(file, "");
                    if (string.IsNullOrEmpty(filePath))
                    {
                        gVal.Code = -1;
                        gVal.Error = "File information could not be processed";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
                    gVal.Error = filePath;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = -1;
                gVal.Error = "Invalid file!";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = "File information could not be processed";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetListObjects()
        {
            try
            {
                var genericObject = new StockGenericObject
                {
                    StoreItemVariations = GetStoreItemVariations(),
                    StoreItemVariationValues = GetStoreItemVariationValues(),
                    StoreCurrencies = GetCurrencies(),
                    UnitsofMeasurement = GetUnitsofMeasurement()
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetListObjects2()
        {
            try
            {
                var genericObject = new StockGenericObject
                {
                    StoreItemVariations = GetStoreItemVariations(),
                    StoreItemVariationValues = GetStoreItemVariationValues(),
                    StoreCurrencies = GetCurrencies(),
                    ImageViews = GetImageViews(),
                    UnitsofMeasurement = GetUnitsofMeasurement()
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ProductUpload(HttpPostedFileBase file, string subdomain, int outletId)
        {
            var feedBackList = new List<GenericValidator>();
            var gVal = new GenericValidator();
            try
            {
                if (outletId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select an Outlet";
                    feedBackList.Add(gVal);
                    return Json(feedBackList, JsonRequestBehavior.AllowGet);
                }

                if (file.ContentLength > 0)
                {
                    var defaultCurrency = new StoreItemStockServices().GetStoreDefaultCurrency();
                    if (defaultCurrency == null || defaultCurrency.StoreCurrencyId < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = "Please set up you operational currency from the settings section.";
                        feedBackList.Add(gVal);
                        return Json(feedBackList, JsonRequestBehavior.AllowGet);
                    }

                    var folderPath = Server.MapPath("~/BulkUploads");

                    var fileName = file.FileName;
                    var path = folderPath + "/" + fileName;

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        var dInfo = new DirectoryInfo(folderPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(
                            new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                FileSystemRights.FullControl,
                                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                    }
                    else
                    {
                        var dInfo = new DirectoryInfo(folderPath);
                        var files = dInfo.GetFiles();
                        if (files.Any())
                        {
                            files.ForEach(fi => fi.Delete());
                        }
                    }

                    file.SaveAs(path);

                    var bulkUploadResults = new StoreItemStockUploadServices().ReadExcelData(path, "Inventory", defaultCurrency.StoreCurrencyId, outletId);

                    if (!bulkUploadResults.Any())
                    {
                        gVal.Code = -1;
                        gVal.Error = "An internal server error was encountered. Please try again later.";
                        feedBackList.Add(gVal);
                        return Json(feedBackList, JsonRequestBehavior.AllowGet);
                    }
                    return Json(bulkUploadResults, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = "The selected file is invalid";
                feedBackList.Add(gVal);
                return Json(feedBackList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                feedBackList.Add(gVal);
                return Json(feedBackList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInventory()
        {
            var gVal = new GenericValidator();
            try
            {
                var list = GetInventoryObjects(1);
                if (!list.Any())
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Inventory_List_Empty;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                var excel = new Application
                {
                    Visible = false
                };
                var wb = excel.Workbooks.Add();
                var sh = wb.Sheets.Add();
                sh.Name = "Inventory";

                sh.Cells[1, 1] = "SKU";

                sh.Cells[1, 2] = "Product/Variant_Value";

                sh.Cells[1, 3] = "Quantity_In_Stock";

                sh.Cells[1, 4] = "Expiry_Date(yyyy/MM/dd)";

                for (var i = 0; i < list.Count; i++)
                {
                    var rowIndex = i + 2;
                    sh.Cells[rowIndex.ToString(CultureInfo.InvariantCulture), "A"].Value2 = list[i].SKU;
                    sh.Cells[rowIndex.ToString(CultureInfo.InvariantCulture), "B"].Value2 = list[i].StoreItemName;
                    sh.Cells[rowIndex.ToString(CultureInfo.InvariantCulture), "C"].Value2 = list[i].QuantityInStock;
                    sh.Cells[rowIndex.ToString(CultureInfo.InvariantCulture), "D"].Value2 = list[i].ExpiryDate;
                }

                const string fileName = "Inventory.xlsx";

                var dir = Server.MapPath("~/InventoryBulkEdit");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    var dInfo = new DirectoryInfo(dir);
                    var dSecurity = dInfo.GetAccessControl();
                    dSecurity.AddAccessRule(
                        new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                            FileSystemRights.FullControl,
                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                            PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dInfo.SetAccessControl(dSecurity);
                }
                else
                {
                    var dInfo = new DirectoryInfo(dir);
                    var files = dInfo.GetFiles();
                    if (files.Any())
                    {
                        foreach (FileInfo fi in files)
                        {
                            fi.Delete();
                        }
                    }
                }

                // now we resize the columns
                //var excelCellrange = sh.Range[sh.Cells[1, 1], sh.Cells[sh.Cells.Count, list.Count]];
                //excelCellrange.EntireColumn.AutoFit();

                //Borders border = excelCellrange.Borders;
                //border.LineStyle = XlLineStyle.xlContinuous;
                //border.Weight = 2d;

                var filePath = dir + "/" + fileName;
                wb.SaveAs(filePath);
                wb.Close(true);
                excel.Quit();

                if (!DownloadContentFromFolder2(filePath))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Process_Failed;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = 5;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult BulkEdit(HttpPostedFileBase file)
        {
            var gVal = new GenericValidator();
            try
            {
                if (file.ContentLength > 0)
                {
                    var folderPath = Server.MapPath("~/BulkUploads");
                    var fileName = file.FileName;
                    var path = folderPath + "/" + fileName;

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        var dInfo = new DirectoryInfo(folderPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(
                            new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                FileSystemRights.FullControl,
                                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                    }
                    else
                    {
                        var dInfo = new DirectoryInfo(folderPath);
                        var files = dInfo.GetFiles();
                        if (files.Any())
                        {
                            foreach (var fi in files)
                            {
                                fi.Delete();
                            }
                        }
                    }

                    file.SaveAs(path);

                    var msg = string.Empty;
                    var errorList = new List<long>();

                    var successfulImports = new StoreItemStockUploadServices().EditExcelData(path, "Inventory", ref errorList, ref msg);

                    if (!successfulImports.Any())
                    {
                        gVal.Code = -1;
                        gVal.Error = msg;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (errorList.Any() && successfulImports.Any())
                    {
                        var feedbackMessage = successfulImports.Count + " records were successfully updated." +
                            "\n" + errorList.Count + " record(s) could not be uploaded due to specified/unspecified errors.";
                        if (msg.Length > 0)
                        {
                            feedbackMessage += "<br/>" + msg;
                        }

                        gVal.Code = -1;
                        gVal.Error = feedbackMessage;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (errorList.Any() && !successfulImports.Any())
                    {
                        var feedbackMessage = errorList.Count + " record(s) could not be uploaded due to specified/unspecified errors.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            feedbackMessage += "<br/>" + msg;
                        }

                        gVal.Code = -1;
                        gVal.Error = feedbackMessage;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (!errorList.Any() && successfulImports.Any())
                    {
                        var feedbackMessage = successfulImports.Count + " records were successfully uploaded.";

                        if (msg.Length > 0)
                        {
                            feedbackMessage += "<br/>" + msg;
                        }

                        gVal.Code = 5;
                        gVal.Error = feedbackMessage;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                }
                gVal.Code = -1;
                gVal.Error = "The selected file is invalid";
                return Json(gVal, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateFileSession(HttpPostedFileBase file)
        {
            var gVal = new GenericValidator();
            try
            {
                gVal.Code = 0;
                if (file != null && file.ContentLength > 0)
                {
                    List<SessionObj> imgSessList;
                    if (Session["_imgSessList"] == null)
                    {
                        imgSessList = new List<SessionObj>
                        {
                            new SessionObj
                            {
                                FileId = 1,
                                FileObj = file
                            }
                        };
                        gVal.Code = 1;
                    }
                    else
                    {
                        imgSessList = Session["_imgSessList"] as List<SessionObj>;
                        if (imgSessList == null || !imgSessList.Any())
                        {
                            imgSessList = new List<SessionObj>
                            {
                                new SessionObj
                                {
                                    FileId = 1,
                                    FileObj = file
                                }
                            };
                            gVal.Code = 1;
                        }
                        else
                        {
                            var count = imgSessList.Count + 1;
                            imgSessList.Add(
                                    new SessionObj
                                    {
                                        FileId = count,
                                        FileObj = file
                                    });

                            gVal.Code = count;
                        }
                    }

                    Session["_imgSessList"] = imgSessList;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = -1;
                gVal.Error = message_Feedback.Invalid_File;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDbCount(int brandId, string subdomain)
        {
            if (brandId < 1)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
            var dbcount = new StoreItemStockServices().GetObjectCountByBrand(brandId);
            return Json(dbcount, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SaveStockImage(int imageViewId, long stockItemId, HttpPostedFileBase file, string subdomain)
        {
            var gVal = new GenericValidator();
            try
            {
                if (imageViewId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select an Image view.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (stockItemId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your request could not be processed.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (file == null || file.ContentLength < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "The file is invalid";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (file.ContentLength > 4096000)
                {
                    gVal.Code = -1;
                    gVal.Error = "The Image size should not be larger than 4MB.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var filePath = SaveFile(file, "");
                if (string.IsNullOrEmpty(filePath))
                {
                    gVal.Code = -1;
                    gVal.FileName = file.FileName;
                    gVal.Error = "File information could not be processed";
                }

                var stockUpload = new StockUploadObject
                {
                    StoreItemStockId = stockItemId,
                    ImageViewId = imageViewId,
                    ImagePath = filePath,
                    LastUpdated = DateTime.Now
                };

                var stockUploadInfo = new StockUploadServices().AddStockUpload(stockUpload);
                if (stockUploadInfo < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "File information could not be processed";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = stockUploadInfo;
                gVal.FilePath = filePath;
                gVal.Error = "File information was successfully processed";
                return Json(gVal, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateStockImage(long id, int imageViewId, string oldPath, long stockItemId, HttpPostedFileBase file, string subdomain)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1 || string.IsNullOrEmpty(oldPath))
                {
                    gVal.Code = -1;
                    gVal.Error = "Your request could not be processed.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (imageViewId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Please select an Image view.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (stockItemId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Your request could not be processed.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var stockUpload = new StockUploadObject
                {
                    StockUploadId = id,
                    StoreItemStockId = stockItemId,
                    ImageViewId = imageViewId,
                    LastUpdated = DateTime.Now,
                    ImagePath = oldPath
                };

                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentLength > 4096000)
                    {
                        gVal.Code = -1;
                        gVal.Error = "The Image size should not be larger than 4MB.";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var filePath = SaveFile(file, oldPath);
                    if (string.IsNullOrEmpty(filePath))
                    {
                        gVal.Code = -1;
                        gVal.FileName = file.FileName;
                        gVal.Error = "File information could not be processed";
                    }

                    stockUpload.ImagePath = filePath;
                    gVal.FilePath = filePath;
                }

                var stockUploadInfo = new StockUploadServices().UpdateStockUpload(stockUpload);
                if (stockUploadInfo < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "File information could not be processed";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = id;
                gVal.Error = "File information was successfully processed";
                return Json(gVal, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveStockImages(List<StockUploadObject> stocks)
        {
            var gVal = new GenericValidator();
            try
            {
                if (stocks == null)
                {
                    gVal.Code = -1;
                    gVal.Error = "The selected file(s) could not be processed.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var successFullUploads = new List<GenericValidator>();
                var failedUploads = new List<GenericValidator>();

                stocks.ForEach(f =>
                {
                    var file = f.FileObj;

                    if (file != null && file.ContentLength > 0 && string.IsNullOrEmpty(f.ImagePath))
                    {
                        var filePath = SaveFile(file, "");
                        if (string.IsNullOrEmpty(filePath))
                        {
                            gVal.Code = -1;
                            gVal.FileName = file.FileName;
                            gVal.Error = "File information could not be processed";
                            failedUploads.Add(gVal);
                        }
                        gVal.Code = 5;
                        gVal.Error = filePath;

                        f.ImagePath = filePath;
                        var stockUploadInfo = new StockUploadServices().AddStockUpload(f);
                        if (stockUploadInfo < 1)
                        {
                            gVal.Code = -1;
                            gVal.FileName = file.FileName;
                            gVal.Error = "File information could not be processed";
                            failedUploads.Add(gVal);
                        }

                        gVal.Code = stockUploadInfo;
                        gVal.FileName = file.FileName;
                        gVal.Error = "File information was successfully processed";
                        successFullUploads.Add(gVal);
                    }

                    if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(f.ImagePath))
                    {
                        var filePath = SaveFile(file, f.ImagePath);
                        if (string.IsNullOrEmpty(filePath))
                        {
                            gVal.Code = -1;
                            gVal.FileName = file.FileName;
                            gVal.Error = "File information could not be processed";
                            failedUploads.Add(gVal);
                        }
                        gVal.Code = 5;
                        gVal.Error = filePath;

                        f.ImagePath = filePath;
                        var stockUploadInfo = new StockUploadServices().UpdateStockUpload(f);
                        if (stockUploadInfo < 1)
                        {
                            gVal.Code = -1;
                            gVal.FileName = file.FileName;
                            gVal.Error = "File information could not be processed";
                            failedUploads.Add(gVal);
                        }

                        gVal.Code = stockUploadInfo;
                        gVal.FileName = file.FileName;
                        gVal.Error = "File information was successfully processed";
                        successFullUploads.Add(gVal);
                    }

                    gVal.Code = -1;
                    gVal.Error = "The selected file is invalid";
                    failedUploads.Add(gVal);
                });

                if (failedUploads.Any())
                {
                    successFullUploads.AddRange(failedUploads);
                }

                return Json(successFullUploads, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteStockUpload(StockUploadObject stockUpload)
        {
            var gVal = new GenericValidator();
            try
            {
                if (stockUpload == null || stockUpload.StockUploadId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Invalid_Selection;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var k = new StockUploadServices().DeleteStockUpload(stockUpload.StockUploadId);
                if (!k)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Delete_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var status = DeleteFile(stockUpload.ImagePath);
                if (!status)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Delete_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = 5;
                gVal.Error = message_Feedback.Delete_Success;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Helpers
        public bool DownloadContentFromFolder(string path)
        {
            try
            {
                Response.Clear();
                var filename = Path.GetFileName(path);
                HttpContext.Response.Buffer = true;
                HttpContext.Response.Charset = "";
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = GetMimeType(filename);
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                Response.WriteFile(Server.MapPath(path));
                Response.End();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private GenericValidator ValidateStockUpload(StockUploadObject stockUpload)
        {
            var gVal = new GenericValidator();
            if (stockUpload == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (stockUpload.StoreItemStockId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Store_Item_Selection_Error;
                return gVal;
            }

            if (stockUpload.ImageViewId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Image_View_Error;
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }
        public bool DownloadContentFromFolder2(string path)
        {
            try
            {
                Response.Clear();
                var filename = Path.GetFileName(path);
                HttpContext.Response.Buffer = true;
                HttpContext.Response.Charset = "";
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = GetMimeType(filename);
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                Response.WriteFile(path);
                Response.End();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var ext = extension.ToLower();
                var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }

        private List<StoreItemVariationObject> GetStoreItemVariations()
        {
            return new StoreItemVariationServices().GetStoreItemVariations();
        }

        private List<StoreItemVariationValueObject> GetStoreItemVariationValues()
        {
            return new StoreItemVariationValueServices().GetStoreItemVariationValues();
        }
        private List<StoreItemObject> GetStoreItems()
        {
            return new StoreItemServices().GetStoreItems();
        }
        private List<StoreCurrencyObject> GetCurrencies()
        {
            return new StoreCurrencyServices().GetCurrencies();
        }

        private List<UnitsOfMeasurementObject> GetUnitsofMeasurement()
        {
            return new UnitOfMeasurementServices().GetUnitsofMeasurement();
        }

        private List<ImageViewObject> GetImageViews()
        {
            return new ImageViewServices().GetImageViews();
        }

        private List<StoreOutletObject> GetStoreOutlets()
        {
            return new StoreOutletServices().GetStoreOutlets();
        }

        private string SaveFile(HttpPostedFileBase file, string formerPath)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Images/StoreItemStock");

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                        var dInfo = new DirectoryInfo(mainPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                    }
                    var path = "";
                    if (SaveToFolder(file, ref path, mainPath, formerPath))
                    {
                        return PhysicalToVirtualPathMapper.MapPath(path).Replace("~", "");
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return string.Empty;
            }
        }

        private static string GenerateUniqueName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid();
        }

        private bool SaveToFolder(HttpPostedFileBase file, ref string path, string folderPath, string formerFilePath = null)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = GenerateUniqueName() + fileExtension;
                    var newPathv = Path.Combine(folderPath, fileName);
                    file.SaveAs(newPathv);
                    if (!string.IsNullOrWhiteSpace(formerFilePath))
                    {
                        if (!DeleteFile(formerFilePath))
                        {
                            return false;
                        }
                    }
                    path = newPathv;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        private bool DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return false;
                }

                if (!filePath.StartsWith("~"))
                {
                    filePath = "~" + filePath;
                }

                System.IO.File.Delete(Server.MapPath(filePath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private List<StoreItemStockObject> GetStoreItemStocks(int? itemsPerPage, int? pageNumber, out int countG)
        {
            return new StoreItemStockServices().GetStoreItemStockObjects(itemsPerPage, pageNumber, out countG) ?? new List<StoreItemStockObject>();
        }

        private List<StoreItemStockObject> GetInventoryObjects(int outletId)
        {
            return new StoreItemStockServices().GetInventoryObjects(outletId) ?? new List<StoreItemStockObject>();
        }

        private GenericValidator ValidateStoreItemStock(StoreItemStockObject storeItemStock)
        {
            var gVal = new GenericValidator();

            if (storeItemStock == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(storeItemStock.SKU))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.SKU_Error;
                return gVal;
            }

            //if (storeItemStock.QuantityInStock < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Quantity_In_Stock_Error;
            //    return gVal;
            //}

            if (storeItemStock.StoreItemVariationId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Variation_Selection_Error;
                return gVal;
            }

            if (storeItemStock.StoreItemVariationValueId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Variation_Value_Selection_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }

        private GenericValidator ValidateInventory(StoreItemStockObject storeItemStock)
        {
            var gVal = new GenericValidator();

            if (storeItemStock == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            //if (storeItemStock.QuantityInStock < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Quantity_In_Stock_Error;
            //    return gVal;
            //}

            //if (storeItemStock.UnitOfMeasurementId < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.UoM_Selection_Error;
            //    return gVal;
            //}

            //if (storeItemStock.Price < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.Price_In_Stock_Error;
            //    return gVal;
            //}

            if (storeItemStock.ExpirationDate < DateTime.Today)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Invalid_Expiry_Date;
                return gVal;
            }

            if (storeItemStock.StoreItemVariationId > 0)
            {
                if (storeItemStock.StoreItemVariationValueId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Product_Variation_Value_Selection_Error;
                    return gVal;
                }
            }

            gVal.Code = 5;
            return gVal;
        }

        private GenericValidator ValidateStoreItem(StoreItemObject product)
        {
            var gVal = new GenericValidator();
            if (product == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(product.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Name_Error;
                return gVal;
            }

            if (product.ChartOfAccountId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Chart_Of_Account_Selection_Error;
                return gVal;
            }
            if (product.StoreItemBrandId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Brand_Selection_Error;
                return gVal;
            }
            if (product.StoreItemCategoryId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Category_Selection_Error;
                return gVal;
            }
            if (product.StoreItemTypeId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Product_Type_Selection_Error;
                return gVal;
            }
            gVal.Code = 5;
            return gVal;
        }

        private GenericValidator AddOrUpdateStockImage(StockUploadObject stockUploadObject)
        {
            var gVal = new GenericValidator();
            try
            {
                if (stockUploadObject.FileObj == null || stockUploadObject.ImageViewId < 1 || stockUploadObject.FileObj.ContentLength < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.File_Error;
                    return gVal;
                }
                var logoPath = "";
                if (stockUploadObject.StockUploadId > 0)
                {
                    if (string.IsNullOrEmpty(stockUploadObject.ImagePath))
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.File_Error;
                        return gVal;
                    }

                    logoPath = SaveFile(stockUploadObject.FileObj, stockUploadObject.ImagePath);
                    if (string.IsNullOrEmpty(logoPath))
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.File_Upload_Error;
                        return gVal;
                    }

                    stockUploadObject.ImagePath = logoPath;
                    stockUploadObject.LastUpdated = DateTime.Now;

                    var z = new StockUploadServices().UpdateStockUpload(stockUploadObject);
                    if (z < 1)
                    {
                        gVal.Error = z == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = 0;
                        return gVal;
                    }
                    gVal.Code = 5;
                    return gVal;
                }
                else
                {
                    logoPath = SaveFile(stockUploadObject.FileObj, "");
                    if (string.IsNullOrEmpty(logoPath))
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.File_Upload_Error;
                        return gVal;
                    }

                    stockUploadObject.ImagePath = logoPath;
                    stockUploadObject.LastUpdated = DateTime.Now;

                    var z = new StockUploadServices().AddStockUpload(stockUploadObject);
                    if (z < 1)
                    {
                        gVal.Error = z == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                        gVal.Code = 0;
                        return gVal;
                    }
                    gVal.Code = 5;
                    return gVal;
                }

            }
            catch
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                return gVal;
            }
        }

        private List<GenericValidator> ProcessStockImage(StoreItemStockObject stockItemStockObject, out int successCount)
        {
            var gVal = new GenericValidator();
            var errorList = new List<GenericValidator>();
            successCount = 0;
            var succCount = 0;
            try
            {
                if (stockItemStockObject.StockUploadObjects != null && stockItemStockObject.StockUploadObjects.Any())
                {
                    if (Session["_imgSessList"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Update_Session_Time_Out_With_Success;
                        errorList.Add(gVal);
                    }

                    var imgSessList = Session["_imgSessList"] as List<SessionObj>;
                    if (imgSessList == null || !imgSessList.Any())
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Update_Session_Time_Out_With_Success;
                        errorList.Add(gVal);
                        return errorList;
                    }

                    stockItemStockObject.StockUploadObjects.ForEach(m =>
                    {
                        if (m == null || m.ImageViewId < 1 || m.FileId < 1)
                        {
                            gVal.Code = -1;
                            gVal.Error = message_Feedback.File_Error;
                            gVal.Email = "";
                            errorList.Add(gVal);
                        }

                        else
                        {
                            var stockUpload = imgSessList.Find(x => x.FileId == m.FileId);
                            if (stockUpload == null || stockUpload.FileObj.ContentLength < 1 || stockUpload.FileId < 1)
                            {
                                gVal.Code = -1;
                                gVal.Error = message_Feedback.File_Error;
                                gVal.Email = "";
                                errorList.Add(gVal);
                            }
                            else
                            {
                                m.FileObj = stockUpload.FileObj;
                                m.StoreItemStockId = stockItemStockObject.StoreItemStockId;

                                var yy = AddOrUpdateStockImage(m);
                                if (yy.Code < 1)
                                {
                                    gVal.Code = -1;
                                    gVal.Error = yy.Error;
                                    gVal.Email = m.ViewName;
                                    errorList.Add(gVal);
                                }
                                else
                                {
                                    succCount += 1;
                                }
                            }
                        }
                    });
                    successCount = succCount;
                    return errorList.Any() ? errorList : new List<GenericValidator>();
                }
                successCount = 0;
                return new List<GenericValidator>
                {
                    new GenericValidator
                    {
                       Code = -1,
                       Error = message_Feedback.File_Collection_Error,
                       Email = ""
                    }
                };
            }
            catch (Exception)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Process_Failed;
                errorList.Add(gVal);
                return errorList;
            }
        }

        #endregion

    }
}
