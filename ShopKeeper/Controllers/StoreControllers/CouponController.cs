using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.StoreControllers
{
    [Authorize]
	public class CouponController : Controller
	{
        public CouponController()
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
        public ActionResult GetCouponObjects(JQueryDataTableParamModel param, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                IEnumerable<CouponObject> filteredCouponObjects;
                var countG = new CouponServices().GetObjectCount();

                var pagedCouponObjects = GetCoupons(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCouponObjects = new CouponServices().Search(param.sSearch);
                }
                else
                {
                    filteredCouponObjects = pagedCouponObjects;
                }

                if (!filteredCouponObjects.Any())
                {
                    return Json(new List<CouponObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<CouponObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.Title : c.Code
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredCouponObjects = sortDirection == "asc" ? filteredCouponObjects.OrderBy(orderingFunction) : filteredCouponObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredCouponObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.CouponId), c.Title, c.Code, c.PercentageDeduction.ToString(CultureInfo.InvariantCulture), c.MinimumOrderAmount.ToString(CultureInfo.InvariantCulture), c.ValidityPeriod };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredCouponObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<CouponObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddCoupon(CouponObject coupon, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                
                var valStatus = ValidateCoupon(coupon);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                    
                var k = new CouponServices().AddCoupon(coupon);
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
                gVal.Code = 0;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditCoupon(CouponObject coupon, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                
                var valStatus = ValidateCoupon(coupon);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_coupon"] == null)
                {
                    gVal.Code = -5;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                   
                var oldCoupon = Session["_coupon"] as CouponObject;
                if (oldCoupon == null || oldCoupon.CouponId < 1)
                {
                    gVal.Code = -5;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                oldCoupon.Title = coupon.Title.Trim();
                oldCoupon.Code = coupon.Code.Trim();
                oldCoupon.PercentageDeduction = coupon.PercentageDeduction;
                oldCoupon.MinimumOrderAmount = coupon.MinimumOrderAmount;
                oldCoupon.ValidFrom = coupon.ValidFrom;
                oldCoupon.ValidTo = coupon.ValidTo;

                var k = new CouponServices().UpdateCoupon(oldCoupon);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                    gVal.Code = 0;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.Error = message_Feedback.Model_State_Error;
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
        public ActionResult DeleteCoupon(long id, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Invalid_Selection;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                    
                var k = new CouponServices().DeleteCoupon(id);
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
        public ActionResult GetCoupon(long id, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<ChildMenuObject>(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (id < 1)
                {
                    return Json(new CouponObject(), JsonRequestBehavior.AllowGet);
                }

                var coupon = new CouponServices().GetCoupon(id);
                if (id < 1)
                {
                    return Json(new CouponObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_coupon"] = coupon;
                return Json(coupon, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new CouponObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<CouponObject> GetCoupons(int? itemsPerPage, int? pageNumber)
        {
            return new CouponServices().GetCouponObjects(itemsPerPage, pageNumber) ?? new List<CouponObject>();
        }
        
        private GenericValidator ValidateCoupon(CouponObject coupon)
        {
            var gVal = new GenericValidator();
            if (coupon == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(coupon.Title))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Title_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(coupon.Code))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Code_Error;
                return gVal;
            }
            if (coupon.PercentageDeduction < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Percentage_Deduction_Error;
                return gVal;
            }

            if (coupon.MinimumOrderAmount < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Minimum_Order_Amount_Error;
                return gVal;
            }
            if (coupon.ValidFrom.Year == 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Valid_From_Error;
                return gVal;
            }

            if (coupon.ValidFrom < DateTime.Today)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Valid_From_Less_Error;
                return gVal;
            }

            if (coupon.ValidTo.Year == 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Valid_To_Error;
                return gVal;
            }

            if (coupon.ValidTo < DateTime.Today)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Valid_To_Less_Error;
                return gVal;
            }

            if (coupon.ValidTo < coupon.ValidFrom)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Coupon_Validity_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
