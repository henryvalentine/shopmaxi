using System;
using System.Collections.Generic;
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
	public class ImageViewController : Controller
	{
        public ImageViewController()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult Countries()
		{
            return View();
		}

        /// <summary>
        /// Handles calls Ajax from DataTable(to which the Facilities List is/to be bound)
        /// </summary>
        /// <param name="param">
        /// Ajax model that encapsulates all required parameters such as 
        /// filtering, pagination, soting, etc instructions from the DataTable
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetImageViewObjects(JQueryDataTableParamModel param)
        {
           try
            {
                IEnumerable<ImageViewObject> filteredImageViewObjects;
                var countG = new ImageViewServices().GetObjectCount();

                var pagedImageViewObjects = GetImageViews(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredImageViewObjects = new ImageViewServices().Search(param.sSearch);
                }
                else
                {
                    filteredImageViewObjects = pagedImageViewObjects;
                }

                if (!filteredImageViewObjects.Any())
                {
                    return Json(new List<ImageViewObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<ImageViewObject, string> orderingFunction = (c =>  c.Name
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredImageViewObjects = sortDirection == "asc" ? filteredImageViewObjects.OrderBy(orderingFunction) : filteredImageViewObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredImageViewObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.ImageViewId), c.Name};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredImageViewObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<ImageViewObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddImageView()
        {
           return View(new ImageViewObject());
        }
        
        [HttpPost]
        public ActionResult AddImageView(ImageViewObject imageView)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateImageView(imageView);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new ImageViewServices().AddImageView(imageView);
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

                gVal.Code = -5;
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
        public ActionResult EditImageView(ImageViewObject imageView)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateImageView(imageView);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_imageView"] == null)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldImageView = Session["_imageView"] as ImageViewObject;
                    if (oldImageView == null || oldImageView.ImageViewId < 1)
                    {
                        gVal.Code = -5;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldImageView.Name = imageView.Name.Trim();
                    var k = new ImageViewServices().UpdateImageView(oldImageView);
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

                gVal.Code = -5;
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
        public ActionResult DeleteImageView(long id)
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

                if (id == 1)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Cannot_Delete;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                    
                var k = new ImageViewServices().DeleteImageView(id);
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
        public ActionResult GetImageView(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new ImageViewObject(), JsonRequestBehavior.AllowGet);
                }

                var imageView = new ImageViewServices().GetImageView(id);
                if (id < 1)
                {
                    return Json(new ImageViewObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_imageView"] = imageView;
                return Json(imageView, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new ImageViewObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<ImageViewObject> GetImageViews(int? itemsPerPage, int? pageNumber)
        {
            return new ImageViewServices().GetImageViewObjects(itemsPerPage, pageNumber) ?? new List<ImageViewObject>();
        }
        
        private GenericValidator ValidateImageView(ImageViewObject imageView)
        {
            var gVal = new GenericValidator();
            if (imageView == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(imageView.Name))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.ImageView_Name_Error;
                return gVal;
            }
           
            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
