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
	public class DocumentTypeController : Controller
	{
		public DocumentTypeController ()
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
        public ActionResult GetDocumentTypeObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<DocumentTypeObject> filteredDocumentTypeObjects;
                var countG = new DocumentTypeServices().GetObjectCount();

                var pagedDocumentTypeObjects = GetDocumentTypes(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredDocumentTypeObjects = new DocumentTypeServices().Search(param.sSearch);
                }
                else
                {
                    filteredDocumentTypeObjects = pagedDocumentTypeObjects;
                }

                if (!filteredDocumentTypeObjects.Any())
                {
                    return Json(new List<DocumentTypeObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<DocumentTypeObject, string> orderingFunction = (c =>  c.TypeName);

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredDocumentTypeObjects = sortDirection == "asc" ? filteredDocumentTypeObjects.OrderBy(orderingFunction) : filteredDocumentTypeObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredDocumentTypeObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.DocumentTypeId), c.TypeName};
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredDocumentTypeObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<DocumentTypeObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult AddDocumentType(DocumentTypeObject documentType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateDocumentType(documentType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                    var k = new DocumentTypeServices().AddDocumentType(documentType);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? "Document Type information already exists" : "Document Type information could not be updated. Please try again";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
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
        public ActionResult EditDocumentType(DocumentTypeObject documentType)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateDocumentType(documentType);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_documentType"] == null)
                    {
                        gVal.Code = -1;
                        gVal.Error = message_Feedback.Session_Time_Out;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldDocumentType = Session["_documentType"] as DocumentTypeObject;
                    if (oldDocumentType == null || oldDocumentType.DocumentTypeId < 1)
                    {
                       gVal.Code = -1;
                       gVal.Error = message_Feedback.Session_Time_Out;
                       return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    oldDocumentType.TypeName = documentType.TypeName.Trim();
                    
                    if (!string.IsNullOrEmpty(documentType.Description))
                    {
                        oldDocumentType.Description = documentType.Description.Trim();
                    }
                    var k = new DocumentTypeServices().UpdateDocumentType(oldDocumentType);
                    if (k < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = k == -3 ? "Document Type information already exists" : "Document Type information could not be updated. Please try again";
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    gVal.Code = 5;
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
        public ActionResult DeleteDocumentType(long id)
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
                    
                var k = new DocumentTypeServices().DeleteDocumentType(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = "Document Type information was successfully deleted.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = "Document Type information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = "Document Type information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDocumentType(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    return Json(new DocumentTypeObject(), JsonRequestBehavior.AllowGet);
                }

                var documentType = new DocumentTypeServices().GetDocumentType(id);
                if (id < 1)
                {
                    return Json(new DocumentTypeObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_documentType"] = documentType;
                return Json(documentType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

               return Json(new DocumentTypeObject(), JsonRequestBehavior.AllowGet);
                

            }
        }

        #endregion
       

        #region Helpers
        private List<DocumentTypeObject> GetDocumentTypes(int? itemsPerPage, int? pageNumber)
        {
            return new DocumentTypeServices().GetDocumentTypeObjects(itemsPerPage, pageNumber) ?? new List<DocumentTypeObject>();
        }

        private GenericValidator ValidateDocumentType(DocumentTypeObject documentType)
        {
            var gVal = new GenericValidator();
            if (documentType == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(documentType.TypeName))
            {
                gVal.Code = -1;
                gVal.Error = "Please provide Document Type Name.";
                return gVal;
            }
           
            gVal.Code = 5;
            gVal.Error = "";
            return gVal;
        }
        #endregion

    }
}
