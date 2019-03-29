using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Shopkeeper.Datacontracts.CustomizedDataObjects;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopKeeper.GenericHelpers;
using ShopKeeper.Properties;
using ShopkeeperServices;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ImportPermitPortal.DataObjects.Helpers;
using Mandrill;
using Mandrill.Model;
using Microsoft.SqlServer.Management.Common;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Models;
using StoreSettingObject = Shopkeeper.DataObjects.DataObjects.Master.StoreSettingObject;

namespace ShopKeeper.Controllers.MasterControllers
{
   
    public class StoreController : Controller
    {
        private static string _dbConnection = string.Empty;

        #region Actions
       
        /// <summary>
        /// Handles calls Ajax from DataTable(to which the Facilities List is/to be bound)
        /// </summary>
        /// <param name="param">
        /// Ajax model that encapsulates all required parameters such as 
        /// filtering, pagination, soting, etc instructions from the DataTable
        /// </param>
        /// <returns></returns>

         [Authorize]
        [HttpGet]
        public ActionResult GetStoreObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<StoreObject> filteredStoreObjects;
                var countG = new StoreServices().GetObjectCount();

                var pagedStoreObjects = GetStores(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredStoreObjects = new StoreServices().Search(param.sSearch);
                }
                else
                {
                    filteredStoreObjects = pagedStoreObjects;
                }

                if (!filteredStoreObjects.Any())
                {
                    return Json(new List<StoreObject>(), JsonRequestBehavior.AllowGet);
                }


                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<StoreObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.StoreName : sortColumnIndex == 2 ? c.CompanyName : sortColumnIndex == 3 ? c.TotalOutlets.ToString(CultureInfo.InvariantCulture) : sortColumnIndex == 1 ? c.BillingCycleCode : c.SubscriptionStatus.ToString());

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredStoreObjects = sortDirection == "asc" ? filteredStoreObjects.OrderBy(orderingFunction) : filteredStoreObjects.OrderByDescending(orderingFunction);
                
                var displayedUserProfilenels = filteredStoreObjects;
                //Product, SKU, Cost, Quantity In Stock,  Expiry Date
                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.StoreId), c.StoreName, c.CompanyName, c.TotalOutlets.ToString(CultureInfo.InvariantCulture), c.BillingCycleCode, c.SubscriptionStatus.ToString() };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredStoreObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<StoreObject>(), JsonRequestBehavior.AllowGet);
            }
        }

         public ActionResult RefreshSession()
         {
             try
             {
                 return Json(5, JsonRequestBehavior.AllowGet);

             }
             catch (Exception)
             {
                 return Json(0, JsonRequestBehavior.AllowGet);
             }
         }

        [HttpPost]
        public async Task<ActionResult> Subscribe(StoreObject store)
        {
            var gVal = new GenericValidator();
            try
            {
                if (store == null)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Model_State_Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (store.IsTrial)
                {
                    var secondndResult = ValidateTrial(store);
                    if (secondndResult.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = secondndResult.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                }

                else
                {
                    var valStatus = ValidateSubscription(store);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }
                    
                }

                store.SecreteKey = Guid.NewGuid().ToString().Replace("-", "");
                store.DateCreated = DateTime.Today;
                store.LastUpdated = DateTime.Today;
                store.StoreAlias = store.StoreAlias.Trim().ToLower();
                var storeId = new StoreServices().AddStore(store);

                if (storeId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = storeId == -2 ? message_Feedback.Subscription_Failure : storeId == -3 ? message_Feedback.Store_Name_Error_2 : message_Feedback.company_Name_Duplicate;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                store.StoreId = storeId;
                var dbName = "SHPKPR" + store.StoreId;
                var dbCreationStatus = CreateDB(dbName, store);
                if (!dbCreationStatus)
                {
                    DeleteStore(store.StoreId);
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Subscription_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var subHistId = AddSubscriptionHistory(store);
                if (subHistId < 1)
                {
                    DeleteStore(store.StoreId);
                    DropDatabase(_dbConnection);
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Subscription_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                    
                }
                
                gVal.Code = 5;
                gVal.StoreAddress = store.StoreAlias + _domainExtension;
                gVal.StoreAlias = store.StoreAlias;
                gVal.CompanyName = store.CompanyName;
                gVal.StoreName = store.StoreName;
                gVal.IsTrial = store.IsTrial;
                gVal.CurrencyCode = store.DefaultCurrency;
                //gVal.ReferenceCode = store.SecreteKey;
                gVal.PackageName = store.PackageName;
                gVal.Magnitude = store.Amount;
                gVal.Duration = store.Duration;
                gVal.PaymentOption = store.PaymentOption;
                gVal.Gx = store.StoreId;

                if (store.IsBankOption)
                {
                    gVal.Error = message_Feedback.Subscription_Success_2;
                }
                if (!store.IsBankOption && !store.IsTrial)
                {
                    gVal.Error = message_Feedback.Subscription_Success_3;
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

        [Authorize]
        [HttpPost]
        public ActionResult DeleteStore(long id)
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
                var k = new StoreServices().DeleteStore(id);
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

         [Authorize]
        public ActionResult GetStore(long id)
        {
            try
            {
                if (id < 1)
                {
                    return Json(new StoreObject(), JsonRequestBehavior.AllowGet);
                }

                var store = new StoreServices().GetStore(id);
                if (id < 1)
                {
                    return Json(new StoreObject(), JsonRequestBehavior.AllowGet);
                }

                Session["_Store"] = store;
                
                return Json(store, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StoreObject(), JsonRequestBehavior.AllowGet);
            }
        }

        private string SaveLogo(string path)
        {
            try
            {

                if (Session["_storeLogo"] == null)
                {
                    return string.Empty;
                }

                var file = Session["_storeLogo"] as HttpPostedFileBase;

                if (file != null && file.ContentLength > 0)
                {
                    var filePath = SaveFile(file, path);
                    return string.IsNullOrEmpty(filePath) ? string.Empty : filePath;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
               return string.Empty;
            }
        }

        public ActionResult GetListObjects()
        {
            try
            {
                var genericObject = new StoreGenericObject
                {
                    Currencies = GetCurrencies(),
                    //BillingCycles = GetBillingCycles(),
                    PaymentMethods = GetPaymentMethods()
                };

                return Json(genericObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new StockGenericObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Currencies()
        {
            try
            {
                return Json(GetCurrencies(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<CurrencyObject>(), JsonRequestBehavior.AllowGet);
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

                    Session["_storeLogo"] = file;
                    gVal.Code = 5;
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

        public ActionResult GetPackage(long id)
        {
            try
            {
                var package = new SubscriptionPackageServices().GetSubscriptionPackage(id);

                if (package == null || package.SubscriptionPackageId < 1)
                {
                   return Json(new SubscriptionPackageObject(), JsonRequestBehavior.AllowGet);
                }
                return Json(package, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new SubscriptionPackageObject(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPackages()
        {
            try
            {
                return Json(GetSubscriptionPackages(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<SubscriptionPackageObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Helpers

        private async Task<bool> SendMail(UserViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.SecurityStamp))
                {
                    return false;
                }

                var type = 0;
                const string label = "Activate your Account!";
                if (model.IsUser)
                {
                    type = (int)MessageEventEnum.New_User;
                }

                else
                {
                    type = (int)MessageEventEnum.New_Account;
                }

                if (type < 1)
                {
                    type = (int)MessageEventEnum.New_Account;
                }

                var msgBody = "";
                var msg = new MessageTemplateServices().GetMessageTemplate(type);
                if (msg.Id < 1)
                {
                    return false;
                }

                var emMs = new MessageObject
                {
                    UserId = model.Id,
                    MessageTemplateId = msg.Id,
                    Status = (int)MessageStatus.Pending,
                    DateSent = DateTime.Now,
                    MessageBody = msg.MessageContent
                };

                var sta = new MessageServices().AddMessage(emMs);
                if (sta < 1)
                {
                    return false;
                }

                if (Request.Url != null)
                {
                    msg.MessageContent = msg.MessageContent.Replace("\n", "<br/>");
                    msg.Subject = msg.Subject.Replace("\n", "<br/>");
                    msgBody += msg.MessageContent.Replace("{email verification link}", "<a style=\"color:green; cursor:pointer\" title=\"Activate Account\" href=" + Url.Action("ConfirmEmail", "Account", new { email = model.Email, code = model.SecurityStamp, ixf = sta }, Request.Url.Scheme) + ">" + label + "</a>").Replace("\n", "<br/>");

                    msgBody += "<br/><br/>" + msg.Footer.Replace("\n", "<br/>");
                }

                if (Request.Url != null)
                {


                    var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
                    var settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

                    var apiKey = ConfigurationManager.AppSettings["mandrillApiKey"];
                    var appName = ConfigurationManager.AppSettings["AplicationName"];

                    if (settings == null || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(appName))
                    {
                        return false;
                    }

                    #region Using Mandrill
                    var api = new MandrillApi(apiKey);
                    var receipint = new List<MandrillMailAddress> { new MandrillMailAddress(model.Email) };
                    var message = new MandrillMessage()
                    {
                        AutoHtml = true,
                        To = receipint,
                        FromEmail = settings.Smtp.From,
                        FromName = appName,
                        Subject = msg.Subject,
                        Html = msgBody
                    };

                    var result = await api.Messages.SendAsync(message);

                    if (result[0].Status != MandrillSendMessageResponseStatus.Sent)
                    {
                        emMs.Status = (int)MessageStatus.Failed;
                    }
                    else
                    {
                        emMs.Status = (int)MessageStatus.Sent;
                    }
                    #endregion

                    emMs.Id = sta;
                    emMs.MessageBody = msgBody;
                    var tts = new MessageServices().UpdateMessage(emMs);
                    if (tts < 1)
                    {
                        return false;
                    }

                    return true;

                }

                return false;
            }
            catch (Exception e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return false;
            }
        }

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
        
        private string _domainExtension = string.Empty;
        public bool DropDatabase(string connectionName)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString))
                {
                    var serverConnection = new ServerConnection(sqlConnection);
                    var server = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
                    server.KillDatabase(sqlConnection.Database);

                    return true;
                }

            }
            catch (Exception)
            {
               return false;
            }
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

        private List<SubscriptionPackageObject> GetSubscriptionPackages()
        {
            return new SubscriptionPackageServices().GetSubscriptionPackages();
        }

        private long AddTransaction(StoreObject store)
        {
            try
            {
                var transaction = new TransactionObject
                {
                    TransactionTypeId = (int) TransactionTypeEnum.Credit,
                    PaymentMethodId = store.PaymentMethodId,
                    Amount = store.Amount,
                    TransactionDate = DateTime.Today
                };

                return new TransactionServices().AddTransaction(transaction);
               
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private long AddStoreSettings(StoreSettingObject storeSetting)
        {
            try
            {
                return new StoreSettingServices().AddStoreSetting(storeSetting);
            }

            catch (Exception)
            {
                return 0;
            }
        }

        private bool CreateDB(string dbName, StoreObject store)
        {
            try
            {
                var storeSettings = DbContextSwitcherServices.GetConnectionStringParameters();
                if (storeSettings == null || string.IsNullOrEmpty(storeSettings.DataSource))
                {
                    return false;
                }

                //storeSettings.InitialCatalog = dbName;
                
                var storageSpace = store.StorageSize/2;
                var dbObject = new DBObject
                {
                    DBSize = storageSpace,
                    DBName = dbName,
                    ConnectionString = storeSettings.SqlConnectionString,
                    ScriptFilePath = Server.MapPath(storeSettings.DBScriptPath)
                };

                var dbCreationResult = DBCreator.CreateDB(dbObject);
                if (!dbCreationResult)
                {
                    return false;
                }

                storeSettings.StoreId = store.StoreId;
                storeSettings.InitialCatalog = dbName;
                
                var k = AddStoreSettings(storeSettings);
                if (k < 1)
                {
                    return false;
                }

                var subscriptionSetting = new SubscriptionSettingObject
                {
                    StoreId = store.StoreId,
                    SecreteKey = store.SecreteKey,
                    DatabaseSpace = storageSpace,
                    FileStorageSpace = storageSpace,
                    Url = store.StoreAlias.ToLower().Trim() + storeSettings.DomainExtension,
                    DateSubscribed = DateTime.Today,
                    ExpiryDate = DateTime.Today.AddDays(store.Duration),
                    SubscriptionStatus = store.SubscriptionStatus,
                };

                var connectionString = DbContextSwitcherServices.SwitchEntityDatabase(storeSettings);
                if (string.IsNullOrEmpty(storeSettings.DataSource))
                {
                    return false;
                }

                _dbConnection = connectionString;
                _domainExtension = storeSettings.DomainExtension;
                Session["entityConnectionString"] = connectionString;
                Session["identityconnectionString"] = dbObject.ConnectionString.Replace("master", dbName);
                var result = new SubscriptionSettingServices(connectionString).AddSubscriptionSetting(subscriptionSetting);
                if (result < 1)
                {
                    return false;  
                }

                var storPath = CreateStoreDirectory("~/Stores/"+ dbName + "/Assets");
                return storPath;
            }
            catch (Exception e)
            {
               ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return false;
            }
        }
        
        private bool DeleteSubscription(long storeId)
        {
            try
            {
                return new StoreSubscriptionServices().DeleteStoreSubscription(storeId);

            }
            catch (Exception)
            {
                return false;
            }
        }

        private long AddSubscriptionHistory(StoreObject store)
        {
            try
            {
                var expiryDate = DateTime.Today.AddDays(store.Duration);
                var subHistory = new StoreSubscriptionHistoryObject
                {
                   StoreId = store.StoreId,
                   SubscriptionPackageId = store.SubscriptionPackageId,
                   DateSubscribed = DateTime.Today,
                   Duration = store.Duration,
                   SubscriptionExpiryDate = expiryDate,
                };

                if (store.PaymentMethodId > 0)
                {
                    subHistory.PaymentId = store.PaymentMethodId;
                }

               return new StoreSubscriptionServices().AddStoreSubscription(subHistory);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private bool DeleteSubscriptionHistory(long subHistId)
        {
            try
            {
                return new StoreSubscriptionServices().DeleteStoreSubscription(subHistId);
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        private List<PaymentMethodObject> GetPaymentMethods()
        {
            return new PaymentMethodServices().GetPaymentMethods();
        }

        private List<CurrencyObject> GetCurrencies()
        {
            return new CurrencyServices().GetCurrencies();
        }

        private List<BillingCycleObject> GetBillingCycles()
        {
            return new BillingCycleServices().GetBillingCycles();
        }
       
        private string SaveFile(HttpPostedFileBase file,  string folderPath)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath(folderPath + "/Assets");

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                        var dInfo = new DirectoryInfo(mainPath);
                        var dSecurity = dInfo.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dInfo.SetAccessControl(dSecurity);
                    }
                    var path = "";
                    if (SaveToFolder(file, ref path, mainPath, ""))
                    {
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

        private bool CreateStoreDirectory(string storeVirtualPath)
        {
            try
            {
                var storePhysicalPath = Server.MapPath(storeVirtualPath);
                if (!Directory.Exists(storePhysicalPath))
                {
                    Directory.CreateDirectory(storePhysicalPath);
                    var dInfo = new DirectoryInfo(storePhysicalPath);
                    var dSecurity = dInfo.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dInfo.SetAccessControl(dSecurity);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
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
        private List<StoreObject> GetStores(int? itemsPerPage, int? pageNumber)
        {
            return new StoreServices().GetStoreObjects(itemsPerPage, pageNumber) ?? new List<StoreObject>();
        }

        private GenericValidator ValidateTrial(StoreObject store)
        {
            var gVal = new GenericValidator();

            if (store == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }
            
            if (string.IsNullOrEmpty(store.StoreName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Store_Name_Error_1;
                return gVal;
            }
            
            if (string.IsNullOrEmpty(store.CustomerEmail))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Customer_Email_Empty;
                return gVal;
            }

            if (string.IsNullOrEmpty(store.DefaultCurrency))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Billing_Cycle_Selection_Error;
                return gVal;
            }

           gVal.Code = 5;
           return gVal;
        }

        private GenericValidator ValidateSubscription(StoreObject store)
        {
            var gVal = new GenericValidator();

            if (store == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (store.PaymentMethodId < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Payment_Method_Selection_Error;
                return gVal;
            }

            if (store.Duration < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Subscription_Duration_Error;
                return gVal;
            }

            if (store.Amount < 1)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Subscription_Amount_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(store.StoreName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Store_Name_Error_1;
                return gVal;
            }

            if (string.IsNullOrEmpty(store.BillingCycleCode))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Billing_Cycle_Selection_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(store.CustomerEmail))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Customer_Email_Empty;
                return gVal;
            }

            if (string.IsNullOrEmpty(store.DefaultCurrency))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Billing_Cycle_Selection_Error;
                return gVal;
            }

            gVal.Code = 5;
            return gVal;
        }
        
        #endregion

    }
}
