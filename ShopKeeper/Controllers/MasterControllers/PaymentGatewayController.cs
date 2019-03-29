using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Shopkeeper.DataObjects.DataObjects.Master;
using ShopKeeper.Properties;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.GenericHelpers;

namespace ShopKeeper.Controllers.MasterControllers
{
    [Authorize]
	public class PaymentGatewayController : Controller
	{
		public PaymentGatewayController ()
		{
			 ViewBag.LoadStatus = "0";
		}

        #region Actions
        public ActionResult PaymentGateways()
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
        public ActionResult GetPaymentGatewayObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<PaymentGatewayObject> filteredPaymentGatewayObjects;
                var countG = new PaymentGatewayServices().GetObjectCount();

                var pagedPaymentGatewayObjects = GetPaymentGateways(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredPaymentGatewayObjects = new PaymentGatewayServices().Search(param.sSearch);
                }
                else
                {
                    filteredPaymentGatewayObjects = pagedPaymentGatewayObjects;
                }

                if (!filteredPaymentGatewayObjects.Any())
                {
                    return Json(new List<PaymentGatewayObject>(), JsonRequestBehavior.AllowGet);
                }

                //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<PaymentGatewayObject, string> orderingFunction = (c => c.GatewayName
                                                                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                if (sortDirection == "asc")
                    filteredPaymentGatewayObjects = filteredPaymentGatewayObjects.OrderBy(orderingFunction);
                else
                    filteredPaymentGatewayObjects = filteredPaymentGatewayObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredPaymentGatewayObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.PaymentGatewayId), c.GatewayName };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredPaymentGatewayObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<PaymentGatewayObject>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult AddPaymentGateway()
        {
           return View(new PaymentGatewayObject());
        }
        
        [HttpPost]
        public ActionResult AddPaymentGateway(PaymentGatewayObject paymentGateway)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidatePaymentGateway(paymentGateway);
                    if (valStatus.Code < 1)
                    {
                        paymentGateway.PaymentGatewayId = 0;
                        paymentGateway.GatewayName = valStatus.Error;
                        return Json(paymentGateway, JsonRequestBehavior.AllowGet);
                    }
                    
                    var logoPath = SaveFile("");
                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        paymentGateway.LogoPath = logoPath;
                    }
                    var k = new PaymentGatewayServices().AddPaymentGateway(paymentGateway);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            paymentGateway.GatewayName = "Payment Gateway information already exists";
                        }
                        if (k == -4)
                        {
                            paymentGateway.GatewayName = "This Sort Code has been registered for another Payment Gateway.";
                        }
                        if (k != -3 && k != -4)
                        {
                            paymentGateway.GatewayName = "Payment Gateway information could not be added. Please try again";
                        }
                        paymentGateway.PaymentGatewayId = 0;
                        return Json(paymentGateway, JsonRequestBehavior.AllowGet);
                    }

                    paymentGateway.PaymentGatewayId = k;
                    return Json(paymentGateway, JsonRequestBehavior.AllowGet);
                }

                paymentGateway.PaymentGatewayId = -5;
                paymentGateway.GatewayName = message_Feedback.Model_State_Error;
                return Json(paymentGateway, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                paymentGateway.PaymentGatewayId = 0;
                paymentGateway.GatewayName = message_Feedback.Process_Failed;
                return Json(paymentGateway, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditPaymentGateway(PaymentGatewayObject paymentGateway)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidatePaymentGateway(paymentGateway);
                    if (valStatus.Code < 1)
                    {
                        paymentGateway.PaymentGatewayId = 0;
                        paymentGateway.GatewayName = valStatus.Error;
                        return Json(paymentGateway, JsonRequestBehavior.AllowGet);
                    }

                    if (Session["_paymentGateway"] == null)
                    {
                        paymentGateway.PaymentGatewayId = -5;
                        paymentGateway.GatewayName = message_Feedback.Session_Time_Out;
                        return Json(paymentGateway, JsonRequestBehavior.AllowGet);
                    }
                   
                    var oldPaymentGateway = Session["_paymentGateway"] as PaymentGatewayObject;
                    if (oldPaymentGateway == null || oldPaymentGateway.PaymentGatewayId < 1)
                    {
                       paymentGateway.PaymentGatewayId = -5;
                       paymentGateway.GatewayName = message_Feedback.Session_Time_Out;
                       return Json(paymentGateway, JsonRequestBehavior.AllowGet);
                    }

                    oldPaymentGateway.GatewayName = paymentGateway.GatewayName.Trim();
                    
                    var formerLogoPath = string.Empty;
                    if (!string.IsNullOrEmpty(oldPaymentGateway.LogoPath))
                    {
                        formerLogoPath = oldPaymentGateway.LogoPath;
                    }

                    var logoPath = SaveFile(formerLogoPath);

                    if (!string.IsNullOrEmpty(logoPath))
                    {
                        oldPaymentGateway.LogoPath = logoPath;
                    }

                    var k = new PaymentGatewayServices().UpdatePaymentGateway(oldPaymentGateway);
                    if (k < 1)
                    {
                        if (k == -3)
                        {
                            paymentGateway.GatewayName = "Payment Gateway information already exists";
                        }
                        if (k == -4)
                        {
                            paymentGateway.GatewayName = "This Sort Code has been registered for another Payment Gateway.";
                        }
                        if (k != -3 && k != -4)
                        {
                            paymentGateway.GatewayName = "Payment Gateway information could not be updated. Please try again";
                        }
                        paymentGateway.PaymentGatewayId = 0;
                        return Json(paymentGateway, JsonRequestBehavior.AllowGet);
                    }
                    return Json(oldPaymentGateway, JsonRequestBehavior.AllowGet);
                }

                paymentGateway.PaymentGatewayId = -5;
                paymentGateway.GatewayName = message_Feedback.Model_State_Error;
                return Json(paymentGateway, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                paymentGateway.PaymentGatewayId = 0;
                paymentGateway.GatewayName = message_Feedback.Process_Failed;
                return Json(paymentGateway, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeletePaymentGateway(long id)
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
                    
                var k = new PaymentGatewayServices().DeletePaymentGateway(id);
                if (k)
                {
                    gVal.Code = 5;
                    gVal.Error = "Payment Gateway information was successfully deleted.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }
                gVal.Code = -1;
                gVal.Error = "Payment Gateway information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                gVal.Code = 5;
                gVal.Error = "Payment Gateway information could not be deleted.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPaymentGateway(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new PaymentGatewayObject(), JsonRequestBehavior.AllowGet);
                }

                var paymentGateway = new PaymentGatewayServices().GetPaymentGateway(id);
                if (id < 1)
                {
                    return Json(new PaymentGatewayObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_paymentGateway"] = paymentGateway;
                if (!string.IsNullOrEmpty(paymentGateway.LogoPath))
                {
                    paymentGateway.LogoPath = paymentGateway.LogoPath.Replace("~", "");
                }
                return Json(paymentGateway, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
               return Json(new PaymentGatewayObject(), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public ActionResult CreateFileSession(HttpPostedFileBase file)
        {
            var gVal = new GenericValidator();
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    gVal.Code = 5;
                    Session["_pyImg"] = file;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                    
                }
                
                gVal.Code = -1;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }
        
        #endregion
       

        #region Helpers

        public string SaveFile(string formerPath)
        {
            try
            {
                if (Session["_pyImg"] == null)
                {
                    return string.Empty;
                }

                var file = Session["_pyImg"] as HttpPostedFileBase;

                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Images/PaymentGateways");

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
                        Session["_pyImg"] = null;
                        return PhysicalToVirtualPathMapper.MapPath(path);
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

        private static string GenerateUniqueGatewayName()
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
                    var fileGatewayName = GenerateUniqueGatewayName() + fileExtension;
                    var newPathv = Path.Combine(folderPath, fileGatewayName);
                    file.SaveAs(newPathv);
                    if (!string.IsNullOrWhiteSpace(formerFilePath))
                    {
                        if (!formerFilePath.StartsWith("~"))
                        {
                            formerFilePath = "~" + formerFilePath;
                        }
                        System.IO.File.Delete(Server.MapPath(formerFilePath));
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

        private List<PaymentGatewayObject> GetPaymentGateways(int? itemsPerPage, int? pageNumber)
        {
            return new PaymentGatewayServices().GetPaymentGatewayObjects(itemsPerPage, pageNumber) ?? new List<PaymentGatewayObject>();
        }

        private GenericValidator ValidatePaymentGateway(PaymentGatewayObject paymentGateway)
        {
            var gVal = new GenericValidator();
            if (paymentGateway == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(paymentGateway.GatewayName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Payment_Gateway_Error;
                return gVal;
            }
            
            gVal.Code = 5;
            gVal.Error = "";
            return gVal;
        }
        #endregion

    }
}
