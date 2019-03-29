using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class ItemPriceController : Controller
	{
        public ItemPriceController()
		{
			 ViewBag.LoadStatus = "0";
		}

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
        public ActionResult GetItemPriceObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<ItemPriceObject> filteredItemPriceObjects;
                var countG = new ItemPriceServices().GetObjectCount();

                var pagedItemPriceObjects = GetItemPrices(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredItemPriceObjects = new ItemPriceServices().Search(param.sSearch);
                }
                else
                {
                    filteredItemPriceObjects = pagedItemPriceObjects;
                }

                if (!filteredItemPriceObjects.Any())
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<ItemPriceObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreItemStockName : sortColumnIndex == 2 ? c.MinimumQuantity.ToString(CultureInfo.InvariantCulture)
                    : c.UoMCode
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredItemPriceObjects = sortDirection == "asc" ? filteredItemPriceObjects.OrderBy(orderingFunction) : filteredItemPriceObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredItemPriceObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.ItemPriceId), c.StoreItemStockName, c.Price.ToString(CultureInfo.InvariantCulture), c.MinimumQuantity.ToString(CultureInfo.InvariantCulture), c.UoMCode };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredItemPriceObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddItemPrice(ItemPriceObject itemPrice)
        {
            var gVal = new GenericValidator();
            try
            {
                var valStatus = ValidateItemPrice(itemPrice);
                if (valStatus.Code < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var k = new ItemPriceServices().AddItemPrice(itemPrice);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                    gVal.Code = 0;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = k;
                gVal.Error = message_Feedback.Insertion_Success;
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
        public ActionResult AddPriceList(List<ItemPriceObject> priceList)
        {
            var gVal = new GenericValidator();
            try
            {
                if (priceList.Any())
                {
                    var successCount = 0;
                    var errorList = new List<GenericValidator>();
                    priceList.ForEach(itemPrice =>
                    {
                        var valStatus = ValidateItemPrice(itemPrice);
                        if (valStatus.Code < 1)
                        {
                            gVal.Code = -1;
                            gVal.Magnitude = itemPrice.Price;
                            gVal.Error = valStatus.Error;
                            errorList.Add(gVal);
                        }

                        var k = new ItemPriceServices().AddItemPrice(itemPrice);
                        if (k < 1)
                        {
                            gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Insertion_Failure;
                            gVal.Code = -1;
                            gVal.Magnitude = itemPrice.Price;
                            errorList.Add(gVal);
                        }
                        successCount++;
                    });

                    if (successCount > 0)
                    {
                        gVal.Error = successCount + " Items were added successfully.";
                    }

                    if (errorList.Any())
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("<table width=\"70%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                        sb.AppendLine(
                            string.Format(
                                "<tr><th width=\"25%\">Price</th><th width=\"25%\">Quantity</th><th width=\"25%\">Error</th></tr>"));

                        errorList.ForEach(m => sb.AppendLine(
                            string.Format(
                                "<tr border=\"1\"><td width=\"25%\">{0}</td><td width=\"25%\">{1}</td><td width=\"25%\">{2}</td></tr>",
                                m.Magnitude, m.Code, m.Error)));

                        sb.AppendLine("</table>");
                        gVal.Error += "The following Item(s) could not be added due to the error specified below." + "\n" + sb;

                        gVal.Code = -1;
                    }
                    else
                    {
                        gVal.Code = 5;
                    }

                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = message_Feedback.Model_State_Error;
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
        public ActionResult EditItemPrice(ItemPriceObject itemPrice)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateItemPrice(itemPrice);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = 0;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_itemPrice"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var oldItemPrice = Session["_itemPrice"] as ItemPriceObject;
                    if (oldItemPrice == null || oldItemPrice.ItemPriceId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    oldItemPrice.Price = itemPrice.Price;
                    oldItemPrice.StoreItemStockId = itemPrice.StoreItemStockId;
                    oldItemPrice.UoMId = itemPrice.UoMId;
                    oldItemPrice.MinimumQuantity = itemPrice.MinimumQuantity;
                    oldItemPrice.Remark = itemPrice.Remark;

                    var k = new ItemPriceServices().UpdateItemPrice(oldItemPrice);
                    if (k < 1)
                    {
                        gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                        gVal.Code = 0;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    gVal.Code = 5;
                    gVal.Error = message_Feedback.Update_Success;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = -5;
                gVal.Error = message_Feedback.Process_Failed;
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
        public ActionResult EditPrice(ItemPriceObject itemPrice)
        {
            var gVal = new GenericValidator();
            try
            {
                if (itemPrice.ItemPriceId < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = "Item could not be processed. Please try again.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var valStatus = ValidateItemPrice(itemPrice);
                if (valStatus.Code < 1)
                {
                    gVal.Code = 0;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var k = new ItemPriceServices().UpdateItemPrice(itemPrice);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                    gVal.Code = 0;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = itemPrice.ItemPriceId;
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
        public ActionResult DeleteItemPrice(long id)
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

                var k = new ItemPriceServices().DeleteItemPrice(id);
                if (k)
                {
                    gVal.Code = id;
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
        public ActionResult GetItemPrice(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new ItemPriceObject(), JsonRequestBehavior.AllowGet);
                }

                var itemPrice = new ItemPriceServices().GetItemPrice(id);
                if (id < 1)
                {
                    return Json(new ItemPriceObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_itemPrice"] = itemPrice;
                return Json(itemPrice, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new ItemPriceObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetItemPriceList(long stockItemId)
        {
            try
            {
                if (stockItemId < 1)
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }

                var itemPrices = new ItemPriceServices().GetItemPrices(stockItemId);
                if (!itemPrices.Any())
                {
                    return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
                }
                return Json(itemPrices, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<ItemPriceObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStates()
        {
            var countries = new StoreStateServices().GetStoreStates() ?? new List<StoreStateObject>();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListObjects()
        {
            try
            {
                var genericObject = new StockGenericObject
                {
                    Inventories = GetStoreItemStocks(),
                    UnitsofMeasurement = GetUnitsofMeasurement()
                    //StoreCurrencies = GetCurrencies()
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetItemPrices(string criteria)
        {
            try
            {
                if (string.IsNullOrEmpty(criteria))
                {
                    return Json(new ItemPriceObject(), JsonRequestBehavior.AllowGet);
                }

                var itemPrices = new ItemPriceServices().GetItemPrices(criteria);

                return Json(itemPrices, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new ItemPriceObject(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region Helpers
        private List<ItemPriceObject> GetItemPrices(int? itemsPerPage, int? pageNumber)
        {
            return new ItemPriceServices().GetItemPriceObjects(itemsPerPage, pageNumber) ?? new List<ItemPriceObject>();
        }

        private List<StoreCurrencyObject> GetCurrencies()
        {
            return new StoreCurrencyServices().GetCurrencies();
        }

        private List<StoreItemStockObject> GetStoreItemStocks()
        {
            return new StoreItemStockServices().GetStoreItemStockObjects();
        }
        private List<UnitsOfMeasurementObject> GetUnitsofMeasurement()
        {
            return new UnitOfMeasurementServices().GetUnitsofMeasurement();
        }

        private GenericValidator ValidateItemPrice(ItemPriceObject itemPrice)
        {
            var gVal = new GenericValidator();
            if (itemPrice == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (itemPrice.Price < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Price_In_Stock_Error;
                return gVal;
            }

            if (itemPrice.MinimumQuantity < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Min_Quantity_Error;
                return gVal;
            }

            if (itemPrice.StoreItemStockId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Store_Item_Stock_Selection_Error;
                return gVal;
            }

            if (itemPrice.UoMId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UoM_Selection_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
