using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ImportPermitPortal.DataObjects.Helpers;
using Mandrill;
using Mandrill.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopkeeperServices;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices;
using ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices;
using ShopKeeper.GenericHelpers;
using ShopKeeper.Models;
using ShopKeeper.Properties;
using StoreSettingObject = Shopkeeper.DataObjects.DataObjects.Master.StoreSettingObject;
using UserProfileObject = Shopkeeper.DataObjects.DataObjects.Store.UserProfileObject;

namespace ShopKeeper.Controllers
{
    
    public class AccountController : Controller
    {
        private static string _identityConnection = ConfigurationManager.ConnectionStrings["MasterIdentityConnection"].ConnectionString;
        
        public AccountController() 
        {
           
        }

        private string _domain = ".shopkeeper.ng";
        private void SetAccountConnection(string connectionString)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext(connectionString)));
        } 

        private void GetIdentityConnection()
        {
            if (Session["identityconnectionString"] == null)
            {
                _identityConnection = string.Empty;
            }
            _identityConnection = (string)Session["identityconnectionString"];
            if (string.IsNullOrEmpty(_identityConnection))
            {
                var currentStorePath = Request.GetOwinContext().Request.Uri.LocalPath;
                if (string.IsNullOrEmpty(currentStorePath))
                {
                    var masterIdentityConnection = ConfigurationManager.ConnectionStrings["MasterIdentityConnection"].ConnectionString;
                    if (string.IsNullOrEmpty(masterIdentityConnection))
                    {
                        return;
                    }
                    _identityConnection = masterIdentityConnection;
                }
                else
                {
                    var path = currentStorePath.Split('/')[0];
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }
                    var storeInfo = new StoreServices().GetStoreSetting(path.Trim().ToLower());
                    if (storeInfo == null || storeInfo.StoreId < 1)
                    {
                        return;
                    }

                    var connection = DbContextSwitcherServices.SwitchSqlDatabase(storeInfo);
                    if (string.IsNullOrEmpty(connection))
                    {
                        return;
                    }

                    _identityConnection = connection;

                }
            }

            SetAccountConnection(_identityConnection);

        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        [AllowAnonymous]
        public ActionResult XSwict(string subdomain)
        {

            if (string.IsNullOrEmpty(subdomain) || HttpContext.Request.Url == null)
            {
                //redirect to shopkeeper.ng
                return Redirect("shopkeeper.ng");
            }

            if (subdomain == "shopkeeper")
            {
                //redirect to shopkeeper.ng
                return Redirect("shopkeeper.ng");
            }

            var store = new SessionHelpers().GetStoreInfo(subdomain);

            if (store == null || store.StoreId < 1)
            {
                //redirect to shopkeeper.ng
                return Redirect("shopkeeper.ng");
            }
            
            var req = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;
            ViewBag.StoreImage = store.StoreLogoPath;
            ViewBag.StoreName = store.StoreName;
            var main = req + "/ngs.html#Store/Welcome/Welcome";
            return Redirect(main);
        }
        
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult GetSignedOnUser()
        {
            if (Session["_signonInfo"] == null)
            {
                return Json(new UserInfo { IsAuthenticated = false }, JsonRequestBehavior.AllowGet);
            }
            var userInfo = Session["_signonInfo"] as UserInfo;
            if (userInfo == null || string.IsNullOrEmpty(userInfo.UserId))
            {
               return Json(new UserInfo {IsAuthenticated = false}, JsonRequestBehavior.AllowGet);
            } 
            return Json(userInfo, JsonRequestBehavior.AllowGet);
               
        }

        [Authorize]
        public ActionResult VerifyLogin()
        {
            if (Session["_adminSignonInfo"] == null)
            {
                return Json(new UserInfo { IsAuthenticated = false }, JsonRequestBehavior.AllowGet);
            }
            var userInfo = Session["_adminSignonInfo"] as UserInfo;
            if (userInfo == null || string.IsNullOrEmpty(userInfo.UserId))
            {
               return Json(new UserInfo { IsAuthenticated = false }, JsonRequestBehavior.AllowGet);
            }

            if (userInfo.Roles.All(r => r != "Super_Admin"))
            {
                return Json(new UserInfo { IsAuthenticated = false }, JsonRequestBehavior.AllowGet);
            }

            userInfo.Code = 5;
            userInfo.IsAuthenticated = true;
            return Json(userInfo, JsonRequestBehavior.AllowGet);

        }

        [AllowAnonymous]
        public ActionResult SignIn(string subdomain)
        {
            ViewBag.ReturnUrl = ViewBag.ReturnUrl;
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            ViewBag.StoreImage = store.StoreLogoPath;
            ViewBag.StoreName = store.StoreName;
            ViewBag.Error = "";
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        public ActionResult Welcome(string subdomain, string returnUrl)
        {
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1 || subdomain == "shopkeeper")
            {
                //redirect to shopkeeper.ng
                return Redirect("shopkeeper.ng");
            }

            if (HttpContext.Request.Url == null)
            {
                //redirect to shopkeeper.ng
                return Redirect("shopkeeper.ng");
            }

            var req = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;
            ViewBag.StoreImage = store.StoreLogoPath;
            ViewBag.StoreName = store.StoreName;
            var main = req + "/ngs.html#Store/Welcome/Welcome";
            return Redirect(main);
        }

        [HttpPost]
        public ActionResult SignoffSession()
        {
            AuthenticationManager.SignOut();
           return Json(5, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLinks(string subdomain)
        {
            var store = new SessionHelpers().GetStoreInfo(subdomain);
            if (store == null || store.StoreId < 1)
            {
                return Json(new OnlineStoreObject(), JsonRequestBehavior.AllowGet);
            }

            var storeObj = new DefaultServices().GetDefaults();
            
            var storeInfo = new StoreInfo
            {
                StoreName = store.StoreName,
                StoreLogo = store.StoreLogoPath,
                CompanyName = store.CompanyName,
                StoreEmail = store.CustomerEmail,
                TicketTimeOut = Session.Timeout,
                StoreAlias = store.StoreAlias
            };

            storeObj.StoreInfo = storeInfo;
            return Json(storeObj, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string subdomain)
        {
            var gVal = new GenericValidator();
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                gVal.Code = -1;
                gVal.Error = "Internal server error was encountered. Please try again later.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            
            if (ModelState.IsValid)
            {
                SetAccountConnection(_identityConnection);
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    var userInfo = new UserInfo
                    {
                        UserId = user.Id,
                        Code = 5,
                        UserName = User.Identity.Name,
                        IsAuthenticated = true,
                        TicketTimeOut = Session.Timeout
                    };

                    userInfo.Roles = GetUserRoles(userInfo.UserId, subdomain);
                    var userProfile = new EmployeeServices().GetUserProfile(user.Id);
                    if (userProfile == null || userProfile.Id < 1)
                    {
                        LogOff();
                        return Json(new UserInfo
                        {
                            UserName = message_Feedback.Invalid_Credentials,
                            IsAuthenticated = false
                        }, JsonRequestBehavior.AllowGet);
                    }
                    userInfo.UserProfile = new UserProfileObject();
                    userInfo.UserProfile = userProfile;
                    Session["_signonInfo"] = userInfo;
                    return Json(userInfo, JsonRequestBehavior.AllowGet);
                }

                return Json(new UserInfo
                {
                    UserName = message_Feedback.User_Not_Verified,
                    Code = -1,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new UserInfo
            {
                UserName = message_Feedback.Invalid_Credentials,
                Code = -1,
                IsAuthenticated = false
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(LoginViewModel model, string subdomain, string returnUrl)
        {
            try
            {
                var storeInfo = new SessionHelpers().GetStoreInfo(subdomain); 
                if (storeInfo == null || storeInfo.StoreId < 1)
                {
                    ViewBag.Error = message_Feedback.Login_Failed;
                    return View(model);
                }
               
                if (ModelState.IsValid)
                {
                    SetAccountConnection(storeInfo.SqlConnectionString);
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if (user != null)
                    {
                        await SignInAsync(user, model.RememberMe);
                        var userInfo = new UserInfo
                        {
                            UserId = user.Id,
                            UserName = User.Identity.Name,
                            IsAuthenticated = true,
                            TicketTimeOut = Session.Timeout
                        };

                        userInfo.Roles = GetUserRoles(userInfo.UserId, subdomain);
                        var userProfile = new EmployeeServices().GetUserProfile(user.Id);
                        if (userProfile == null || userProfile.Id < 1)
                        {
                            LogOff();
                            return Json(new UserInfo
                            {
                                UserName = message_Feedback.Invalid_Credentials,
                                IsAuthenticated = false
                            }, JsonRequestBehavior.AllowGet);
                        }
                        userInfo.UserProfile = new UserProfileObject();
                        userInfo.UserProfile = userProfile;
                        Session["_signonInfo"] = userInfo;
                        //return File("/ngy.html", "text/html");
                        return RedirectToStoreUrl(returnUrl);
                    }
                    
                    return Json( new UserInfo
                    {
                        UserName = message_Feedback.Invalid_Credentials,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                ViewBag.Error = message_Feedback.Invalid_Credentials;
                return View(model);
            }
            catch (Exception ex)
            {
               ViewBag.Error = message_Feedback.Invalid_Credentials;
               return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AdminSignIn(LoginViewModel model, string subdomain, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetAccountConnection(_identityConnection);
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if (user != null)
                    {
                        await SignInAsync(user, model.RememberMe);
                        var userInfo = new UserInfo2
                        {
                            UserId = user.Id,
                            UserName = User.Identity.Name,
                            IsAuthenticated = true,
                            TicketTimeOut = Session.Timeout
                        };

                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(_identityConnection)));
                        
                        var roles = roleManager.Roles.ToList();
                        if (!roles.Any())
                        {
                            LogOff();
                            return Json(new UserInfo
                            {
                                UserName = message_Feedback.Invalid_Credentials,
                                IsAuthenticated = false
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var userRoles = UserManager.GetRoles(userInfo.UserId).ToList();
                        if (!userRoles.Any())
                        {
                            LogOff();
                            return Json(new UserInfo
                            {
                                UserName = message_Feedback.Invalid_Credentials,
                                IsAuthenticated = false
                            }, JsonRequestBehavior.AllowGet);
                        }

                        userInfo.Roles = new List<string>();

                        var userProfile = new StoreServices().GetAdminUserProfile(user.Id);
                        if (userProfile == null || userProfile.Id < 1)
                        {
                            LogOff();
                            return Json(new UserInfo
                            {
                                UserName = message_Feedback.Invalid_Credentials,
                                IsAuthenticated = false
                            }, JsonRequestBehavior.AllowGet);
                        }

                        roles.ForEach(r =>
                        {
                            var refRole = userRoles.Find(x => x == r.Name);
                            if (!string.IsNullOrEmpty(refRole))
                            {
                                userInfo.Roles.Add(refRole);
                            }
                        });

                        if (!userInfo.Roles.Any())
                        {
                            LogOff();
                            return Json(new UserInfo2
                            {
                                UserName = message_Feedback.Invalid_Credentials,
                                IsAuthenticated = false
                            }, JsonRequestBehavior.AllowGet);
                        }

                        userInfo.UserProfile = new Shopkeeper.DataObjects.DataObjects.Master.UserProfileObject();
                        userInfo.UserProfile = userProfile;
                        userInfo.Code = 5;
                        userInfo.IsAuthenticated = true;
                        Session["_adminSignonInfo"] = userInfo;
                        return Json(userInfo, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Invalid_Credentials,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new UserInfo
                {
                    UserName = message_Feedback.Invalid_Credentials,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new UserInfo
                {
                    UserName = message_Feedback.Invalid_Credentials,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> NgSignIn(LoginViewModel model, string subdomain)
        {
            try
            {
                if (HttpContext.Request.Url == null)
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Login_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }
                
                if (string.IsNullOrEmpty(model.UserName))
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Empty_Email,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Password_Empty,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                //Get store settings
                var storeInfo = new SessionHelpers().GetStoreInfo(subdomain);
                if (storeInfo == null || storeInfo.StoreId < 1)
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Login_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                //Set sql connection string for the current store
                SetAccountConnection(storeInfo.SqlConnectionString);
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user == null)
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Invalid_Credentials,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                //sign in the user
                await SignInAsync(user, model.RememberMe);
                var userInfo = new UserInfo
                {
                    UserId = user.Id,
                    UserName = User.Identity.Name,
                    IsAuthenticated = true,
                    TicketTimeOut = Session.Timeout,
                    UserProfile = new UserProfileObject(),
                    UserLinks = new List<ParentMenuObject>()
                };

                //Get Store Address
                var storeAddress = new StoreAddressServices().GetStoreAddress();
                if (!string.IsNullOrEmpty(storeAddress.StreetNo))
                {
                    storeInfo.StoreAddress = storeAddress.StreetNo;
                }

                //get user Profile
                userInfo.Roles = GetUserRoles(userInfo.UserId, subdomain);

                var roles = GetSystemRoles(storeInfo.SqlConnectionString);
                if (!userInfo.Roles.Any() || !roles.Any())
                {
                    LogOff();
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Login_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                var roleIds = new List<string>();
                userInfo.Roles.ForEach(r =>
                {
                    var x = roles.Find(d => d.Name == r);
                    if (x != null && !string.IsNullOrEmpty(x.Id))
                    {
                        roleIds.Add(x.Id);
                    }
                });

                if (!roleIds.Any())
                {
                    LogOff();
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Login_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                var userLinks = new ParentMenuServices().GetParentMenuList(roleIds);
                if (!userLinks.Any())
                {
                    LogOff();
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Login_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                userInfo.UserLinks = userLinks;
                
                userInfo.UserProfile = new UserProfileObject();

                if (userInfo.Roles.Any(r => r != "Customer"))
                {
                    if (userInfo.Roles.Any(r => r == "Admin" || r == "Super_Admin"))
                    {
                        userInfo.UserProfile = new EmployeeServices().GetAdminProfile(user.Id);
                    }
                    else
                    {
                        if (userInfo.Roles.Any(r => r != "Admin" && r != "Super_Admin"))
                        {
                            userInfo.UserProfile = new EmployeeServices().GetUserProfile(user.Id);
                        }
                    }
                }
                else
                {
                    userInfo.UserProfile = new EmployeeServices().GetCustomerProfile(user.Id);
                }
                
                if (userInfo.UserProfile == null || userInfo.UserProfile.Id < 1)
                {
                    LogOff();
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Login_Failed,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                if (!userInfo.UserProfile.IsActive)
                {
                    LogOff();
                    return Json(new UserInfo
                    {
                        UserName = "Your Account is in review. Please contact the Admin.",
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                userInfo.Code = 5;
                Session["_signonInfo"] = userInfo;
                Session["_storeInfo"] = storeInfo;

                return Json(userInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new UserInfo
                {
                    UserName = message_Feedback.Invalid_Credentials,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult UpdateStoreSettings(StoreSettingObject setting, string subdomain)
        {
            var gVal = new GenericValidator();
            try
            {
                var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
                if (storeSetting == null || storeSetting.StoreId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Internal server error was encountered. Please try again later.";
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(setting.StoreName))
                {
                    gVal.Code = -1;
                    gVal.Error = "Plese provide store name";
                    return Json(setting, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(setting.StoreEmail))
                {
                    gVal.Code = -1;
                    gVal.Error = "Plese provide store Email";
                    return Json(setting, JsonRequestBehavior.AllowGet);
                }

                if (setting.CountryId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Plese select a Country";
                    return Json(setting, JsonRequestBehavior.AllowGet);
                }

                if (setting.StateId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Plese select a State/Province/Region";
                    return Json(setting, JsonRequestBehavior.AllowGet);
                }

                if (setting.CityId < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Plese select a City";
                    return Json(setting, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(setting.StreetNo))
                {
                    gVal.Code = -1;
                    gVal.Error = "Plese provide store Address";
                    return Json(setting, JsonRequestBehavior.AllowGet);
                }

                var status = new StoreSettingServices().UpdateStoreSetting(setting);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                gVal.Code = -1;
                gVal.Error = "An unknown error was encountered. Request could not be completed.";
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCountries(string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<CountryObject>(), JsonRequestBehavior.AllowGet);
            }
            var processStatus = new DefaultServices().GetCountries();

            return Json(processStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCountryStates(long countryId, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<CountryObject>(), JsonRequestBehavior.AllowGet);
            }
            var processStatus = new DefaultServices().GetCountryStates(countryId);

            return Json(processStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStateCities(long stateId, string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return Json(new List<CountryObject>(), JsonRequestBehavior.AllowGet);
            }
            var processStatus = new DefaultServices().GetStateCities(stateId);

            return Json(processStatus, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUpViewModel model, string subdomain)
        {
            try
            {
                //Get store settings
                var storeInfo = new SessionHelpers().GetStoreInfo(subdomain);
                if (storeInfo == null || storeInfo.StoreId < 1)
                {
                    return Json(new UserInfo
                    {
                        Feedback = message_Feedback.SignUp_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                if (ModelState.IsValid)
                {
                    //Set sql connection string for the current store
                    SetAccountConnection(storeInfo.SqlConnectionString);

                    var ttxd = UserManager.FindByEmail(model.Email);

                    if (ttxd != null)
                    {
                        return Json(new UserInfo
                        {
                            Feedback = "The Email address " + model.Email + " is already taken",
                            Code = -1,
                            IsAuthenticated = false
                        }, JsonRequestBehavior.AllowGet);
                    }
                    
                    UserManager.UserLockoutEnabledByDefault = true;
                    UserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    UserManager.MaxFailedAccessAttemptsBeforeLockout = 5;
                    
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        UserInfo = new ApplicationDbContext.UserProfile
                        {
                            OtherNames = model.OtherNames,
                            LastName = model.LastName,
                            IsActive = true,
                        }
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, "Customer");
                        
                        var usrModel = new UserViewModel
                        {
                            IsUser = true,
                            SecurityStamp = user.SecurityStamp,
                            Email = user.Email,
                            StoreWebAddress = storeInfo.StoreAlias + _domain,
                            StoreAlias = storeInfo.StoreAlias,
                            StoreName = storeInfo.StoreName
                        };
                        var userInfo = new UserInfo
                        {
                            UserName = model.OtherNames.Split(' ')[0],
                            Code = 7,
                            IsAuthenticated = false
                        };
                         
                        var status = await SendSignUpMail(usrModel);
                        if (!status)
                        {
                            userInfo.Feedback = message_Feedback.Signup_Email_Confirmation_Failed;
                            return Json(userInfo, JsonRequestBehavior.AllowGet);
                        }

                        await SignInAsync(user, true);

                        userInfo.IsAuthenticated = true;
                        userInfo.Roles = new List<string> { "Customer" };
                        Session["_signonInfo"] = userInfo;
                        Session["_storeInfo"] = storeInfo;

                        userInfo.Feedback = message_Feedback.Signup_Successful;
                        return Json(userInfo, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new UserInfo
                    {
                        Feedback = message_Feedback.SignUp_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);

                }

                return Json(new UserInfo
                {
                    Feedback = message_Feedback.Invalid_Credentials,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new UserInfo
                {
                    Feedback = message_Feedback.Invalid_Credentials,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> VerifyDelete(VerifyUserViewModel model, string subdomain)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Empty_Email,
                        Code = 5,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Password_Empty,
                        Code = 5,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                //Get store settings
                var storeInfo = new SessionHelpers().GetStoreInfo(subdomain);
                if (storeInfo == null || storeInfo.StoreId < 1)
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Login_Failed,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                //Set sql connection string for the current store
                SetAccountConnection(storeInfo.SqlConnectionString);
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user == null)
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.Invalid_Credentials,
                        Code = -1,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }
                
                //get user roles
                var roles = GetUserRoles(user.Id, subdomain);
                if (roles.Any(r => r == "Super_Admin" || r == "Admin"))
                {
                    return Json(new UserInfo
                    {
                        UserName = message_Feedback.User_Verified,
                        Code = 5,
                        IsAuthenticated = false
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new UserInfo
                {
                    UserName = message_Feedback.Invalid_Credentials,
                    Code = -1,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new UserInfo
                {
                    UserName = message_Feedback.Invalid_Credentials,
                    IsAuthenticated = false
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            GetIdentityConnection();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
       public async Task<ActionResult> RegisterSubscriptionAccount(RegisterModel registerModel)
        {
            var gVal = new GenericValidator();
            try
            {
                GetIdentityConnection();
                var validateResult = ValidateUserInfo(registerModel);
                if (validateResult.Code < 1)
                {
                    gVal.Code = validateResult.Code;
                    gVal.Error = validateResult.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var ttxd = UserManager.FindByEmail(registerModel.Email);
                if (ttxd != null)
                {
                    gVal.Code = -1;
                    gVal.Error = "The Email address " + registerModel.Email + " is already taken";
                    return Json(gVal, JsonRequestBehavior.AllowGet); 
                }

                var ttxy = UserManager.Users.Count(m => m.PhoneNumber.Trim().Replace(" ", "") == registerModel.PhoneNumber.Trim().Replace(" ", ""));

                if (ttxy > 0)
                {
                    gVal.Code = -1;
                    gVal.Error = "The Phone number " + registerModel.PhoneNumber + " is already taken";
                    return Json(gVal, JsonRequestBehavior.AllowGet); ;
                }
                var roles = new List<string>
                {
                    "Super_Admin",
                    "Admin",
                    "Cashier",
                    "StockKeeper",
                    "Customer"
                };

                if (!CreateRoles(roles, _identityConnection))
                {
                    gVal.Code = -2;
                    gVal.Error = message_Feedback.Admin_Account_Creation_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet); 
                }

                UserManager.UserLockoutEnabledByDefault = false;
                var user = new ApplicationUser
                    {
                        UserName = registerModel.UserName,
                        Email = registerModel.Email,
                        PhoneNumber = registerModel.PhoneNumber,

                    };

                    var result = await UserManager.CreateAsync(user, registerModel.Password);

                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, "Admin");
                        gVal.Code = 5;
                        gVal.Error = message_Feedback.Insertion_Success;
                        gVal.PaymentOption = registerModel.PaymentOption;
                        registerModel.Tbsr = user.Id;


                        if (!SendMail(registerModel))
                        {
                            var msg = message_Feedback.Email_Failure;
                            if (registerModel.IsTrial)
                            {
                                gVal.Error = message_Feedback.Subscription_Success_1;
                            }
                            if (registerModel.IsBankOption)
                            {
                                gVal.Error = msg + "<br/>" + message_Feedback.Subscription_Success_2;
                            }
                            if (!registerModel.IsBankOption && !registerModel.IsTrial)
                            {
                                gVal.Error = msg + "<br/>" + message_Feedback.Subscription_Success_3;
                            }
                        }

                        else
                        {
                            var msg = message_Feedback.Email_Success;
                            if (registerModel.IsTrial)
                            {
                                gVal.Error = message_Feedback.Subscription_Success_1;
                            }
                            if (registerModel.IsBankOption)
                            {
                                gVal.Error = msg + "<br/>" + message_Feedback.Subscription_Success_2;
                            }
                            if (!registerModel.IsBankOption && !registerModel.IsTrial)
                            {
                                gVal.Error = msg + "<br/>" + message_Feedback.Subscription_Success_3;
                            }

                        }
                        return Json(gVal, JsonRequestBehavior.AllowGet); ;
                    }

                    gVal.Code = -2;
                    gVal.Error = message_Feedback.Admin_Account_Creation_Failure;
                    return Json(gVal, JsonRequestBehavior.AllowGet); 
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -2;
                gVal.Error = message_Feedback.Admin_Account_Creation_Failure;
                return Json(gVal, JsonRequestBehavior.AllowGet); 
            }
        }

        [Authorize]
        public ActionResult Dashboard(string subdomain)
        {
            var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
            if (storeSetting == null || storeSetting.StoreId < 1)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(storeSetting);
        }

        //[Authorize]
        public ActionResult GetRoles(string subdomain)
        {
            return Json(GetRoleLists(subdomain), JsonRequestBehavior.AllowGet);
        }

        public int PopupShowDelay
        {
            get { return 60000 * (Session.Timeout - 1); }
        }

         [AllowAnonymous]
        public ActionResult ComputeHash()
         {

            const string stringToHash = "fb96e7280fd946e6a0c3446e90c602c5" + "6153" + "102" + "200000" +
                                        "http://localhost:12582/Hanshaker/ConfirmCustomerDetails" +
                                        "E3D228595329D00F85F2DD170D221C3F48A2F397882440D52CD3E2F89A00580347A4F7F6B2F6FB325971B0C12B32939612F28638A1CCE0395212FEA51148DE9";
          var hash =  GetHash(stringToHash);
          return Json(hash, JsonRequestBehavior.AllowGet);
        }
        
        private string GetHash(string dataToHash)
        {
            try
            {
                using (SHA512 shaM = new SHA512Managed())
                {
                    var hashStr = Convert.ToBase64String(shaM.ComputeHash(Encoding.UTF8.GetBytes(dataToHash)));
                    return hashStr;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        
        public ActionResult GetStoreSetting()
        {
            var store = new SessionHelpers().GetStoreInfo();
            if (store == null || store.StoreId < 1)
            {
                return Json(new UserInfo(), JsonRequestBehavior.AllowGet); 
            }
            if (Session["_signonInfo"] == null)
            {
                return Json(new UserInfo(), JsonRequestBehavior.AllowGet); 
            }

            var userInfo = Session["_signonInfo"] as UserInfo;
            if (userInfo == null || string.IsNullOrEmpty(userInfo.UserId))
            {
                return Json(new UserInfo(), JsonRequestBehavior.AllowGet);
            }
            

            //Get Store Address
            var storeAddress = new StoreAddressServices().GetStoreAddress();
            if (string.IsNullOrEmpty(storeAddress.StreetNo))
            {
                return Json(new StoreInfo(), JsonRequestBehavior.AllowGet);
            }
            var storeInfo = new StoreInfo
            {
                StoreName = store.StoreName,
                StoreLogo = store.StoreLogoPath,
                CompanyName = store.CompanyName,
                StoreEmail = store.CustomerEmail,
                LoggedOnUser = userInfo.UserName,
                IsAuthenticated = userInfo.IsAuthenticated,
                StoreAddress = storeAddress.StreetNo,
                TicketTimeOut = Session.Timeout,
                StoreAlias = store.StoreAlias,
                DefaultCurrency = store.DefaultCurrency,
                DefaultCurrencySymbol = store.DefaultCurrencySymbol
            };
          
            return Json(storeInfo, JsonRequestBehavior.AllowGet);
        }

        private GenericValidator ValidateUserInfo(RegisterModel model)
        {
            var gVal = new GenericValidator();
            try
            {
              
                if (string.IsNullOrEmpty(model.Email))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Empty_Email;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.UserName))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Empty_User_Name;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Password_Empty;
                    return gVal;
                }

                if (model.Password.Length < 8)
                {
                    gVal.Code = -1;
                    gVal.Error = message_Feedback.Minimum_Password_Lenght;
                    return gVal;
                }

                gVal.Code = 5;
                return gVal;
            }
            catch (Exception ex)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Validation_Failed;
                return gVal;
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }
        
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        private bool CreateRoles(List<string> roles, string connectionString)
        {
            try
            {
                if (!roles.Any())
                {
                    return false;
                }
                var successList = new List<string>();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(connectionString)));
                roles.ForEach(m =>
                {
                    if (!roleManager.RoleExists(m.Trim()))
                    {
                        var role = new IdentityRole {Name = m};
                        roleManager.Create(role);
                        successList.Add(m);
                    }
                });

                return successList.Count == roles.Count;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private List<IdentityRole> GetRoleLists(string subdomain)
        {
            try
            {
                var storeSetting = new SessionHelpers().GetStoreInfo(subdomain);
                if (storeSetting == null || storeSetting.StoreId < 1)
                {
                    return new List<IdentityRole>();
                }

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(storeSetting.SqlConnectionString)));

                var userRoles = roleManager.Roles.ToList();
                return userRoles;
            }
            catch (Exception)
            {
                return new List<IdentityRole>();
            }
        }

        private List<string> GetUserRoles(string userId, string subdomain)
        {
            try
            {
               var roles =  GetRoleLists(subdomain);
                if (!roles.Any())
                {
                    return new List<string>();
                }

                //roles.ForEach(m =>
                //{
                    
                //});

                var userRoles = UserManager.GetRoles(userId);
                return userRoles.ToList();
            }
            catch (Exception)
            {
                return new List<string>(); 
            }
        }
        
        private async Task<bool> SendSignUpMail(UserViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.SecurityStamp))
                {
                    return false;
                }

                if (Request.Url != null)
                {
                    var msgBody = "Thanks for signing up on " + model.StoreWebAddress + "<br/>Please click on the link below to activate your account<br/><br/>" + "<a style=\"color:green; cursor:pointer\" title=\"Activate Account\" href=" + Url.Action("ConfirmSignupEmail", "Account", new { email = model.Email, code = model.SecurityStamp, dir = model.StoreAlias }, Request.Url.Scheme) + ">Activate your account!</a>";
                    var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
                    var settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

                    var apiKey = ConfigurationManager.AppSettings["mandrillApiKey"];
                    var appName = model.StoreName;

                    if (settings == null || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(appName))
                    {
                        return false;
                    }
                    //return RedirectToAction("Index", "Home");
                    #region Using Mandrill
                    var api = new MandrillApi(apiKey);
                    var receipint = new List<MandrillMailAddress> { new MandrillMailAddress(model.Email) };
                    var message = new MandrillMessage()
                    {
                        AutoHtml = true,
                        To = receipint,
                        FromEmail = settings.Smtp.From,
                        FromName = appName,
                        Subject = "Account Activation Required",
                        Html = msgBody
                    };

                    var result = await api.Messages.SendAsync(message);

                    if (result[0].Status != MandrillSendMessageResponseStatus.Sent)
                    {
                        return false;
                    }
                   
                    return true;
                    #endregion

                }

                return false;
            }
            catch (Exception e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return false;
            }
        }

        private bool SendMail(RegisterModel registerModel)
        {
            try
            {
                if (registerModel == null || registerModel.Duration < 1)
                {
                    return false;
                }

                var msgBody = "";

                if (registerModel.IsTrial)
                {
                    //var logo = Server.MapPath("~/Images/msgHeader.png");
                    //Add the HTML and Text bodies
                    if (Request.Url != null)
                        msgBody = string.Format("Hi " + registerModel.CompanyName + "<br/>You have successfully subscribed for a Trial Package on shopKeeper.com<br/>Click on the link below to activate your subscription. <br/><a href=\"{0}\"title=\"Activate Subscription\">{1}</a>",
                            Url.Action("ConfirmEmail", "Account",
                                new { registerModel.ReferenceCode, registerModel.Gx }, Request.Url.Scheme), "Please Activate Your Subscription!");
                }
                else
                {
                    msgBody = "<br/>Hi," + registerModel.CompanyName + "<br/><br/>You have successfully subscribed for a Package on <b>shopKeeper.com<b/> <br/>The details of your subscription are presented below: <br/><br/>" +
                        "<div class=\"row\"><div class=\"row\"><div class=\"col-md-5\"><label>Reference:</label></div><div class=\"col-md-7\"><h4 class=\"control-label\" style=\"font-weight: bold\">" +
                        registerModel.ReferenceCode
                        +
                        "</div></div><div class=\"row\"><div class=\"col-md-5\"><label>Package:</label></div><div class=\"col-md-7\"><h4 class=\"control-label\" style=\"font-weight: bold\">" +
                        registerModel.PackageName + "</h4>"
                        +
                        "</div></div><div class=\"row\"><div class=\"col-md-5\"><label>Duration(Days):</label></div><div class=\"col-md-7\"><h4 class=\"control-label\" style=\"font-weight: bold\">" +
                        registerModel.Duration + "</h4>"
                        +
                        "</div></div><div class=\"row\"><div class=\"col-md-5\"><label>Amount Due(&#8358;):</label></div><div class=\"col-md-7\"><h4 class=\"control-label\" style=\"font-weight: bold\">" +
                        registerModel.AmountDue + "</h4>"
                        +
                        "</div></div><div class=\"row\"><div class=\"col-md-5\"><label>Company:</label></div><div class=\"col-md-7\"><h4 class=\"control-label\">" +
                        registerModel.CompanyName + "</h4>"
                        +
                        "</div></div><div class=\"row\"><div class=\"col-md-5\"><label>Store: </label></div><div class=\"col-md-7\">"
                        + "<h4 class=\"control-label\">" + registerModel.StoreName + "</h4>"
                        + "</div></div>" +
                        "<div class=\"row\">"
                        + "<div class=\"col-md-5\"><label>Date Subscribed: </label></div><div class=\"col-md-7\">"
                            + "<h4 class=\"control-label\">" + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt") + "</h4>"
                            + "</div></div></div><br/><br/>";

                    if (Request.Url != null)
                        msgBody += string.Format("Please Click on the link below to activate your subscription. <br/><a href=\"{0}\"title=\"Activate Subscription\">{1}</a>",
                            Url.Action("ConfirmEmail", "Account",
                                new { code = registerModel.ReferenceCode, uaz = registerModel.Tbsr }, Request.Url.Scheme), "Please Activate Your Subscription!");
                }

             #region Using Mandrill

            var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
            var settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

            if (settings == null)
            {
               return false;
            }
                
            var mail = new MailMessage(new MailAddress(settings.Smtp.From), new MailAddress(registerModel.Email))
            {
                Subject = "Your Subscription Information",
                Body = msgBody,
                IsBodyHtml = true
            };
                
            var smtp = new SmtpClient(settings.Smtp.Network.Host)
            {
                Credentials = new NetworkCredential(settings.Smtp.Network.UserName, settings.Smtp.Network.Password),
                EnableSsl = true,
                Port = settings.Smtp.Network.Port
            };

            //myMessage.Html = MessageHeader.GetMsgMarkUp(msgId, msgBody, logo);

            smtp.Send(mail);
            return true;
            #endregion

            }
            catch (Exception e)
            {
                ErrorLogger.LogError(e.StackTrace, e.Source, e.Message);
                return false;
            }
       }

		[AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string code, string uaz)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(uaz))
            {
                return View("Error");
            }

            var tokenVerificationResult = new StoreServices().VerifyToken(code, uaz);
            if (tokenVerificationResult < 0)
            {
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return View("Login");
            }

            var result = await UserManager.FindByIdAsync(uaz);
		    if (result == null)
		    {
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return View("Login");
		    }
		    result.EmailConfirmed = true;
            var emailActivationResult = await UserManager.UpdateAsync(result);
            if (!emailActivationResult.Succeeded)
            {
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return View("Login");
            }

            ViewBag.ConfirmStat = 5;
            ViewBag.ConfirmErr = message_Feedback.Activation_Success; ;
            return View("Login");
            
        }

        private List<IdentityRole> GetSystemRoles(string connectionString)
        {
            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext(connectionString)));
                var roles = roleManager.Roles.ToList();
                if (!roles.Any())
                {
                    return new List<IdentityRole>();
                }
                var filteredRoles = new List<IdentityRole>();
                roles.ForEach(m =>
                {
                    if (m.Name.ToLower() != "super_admin")
                    {
                        filteredRoles.Add(m);
                    }
                });

                return filteredRoles;
            }
            catch (Exception)
            {
                return new List<IdentityRole>();
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmSignupEmail(string email, string code, string dir)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(dir))
            {
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                 return Redirect("shopkeeper.ng");
            }

            var store = new SessionHelpers().GetStoreInfo(dir);
            if (store == null || store.StoreId < 1)
            {
                //redirect to shopkeeper.ng
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return Redirect("shopkeeper.ng");
            }

            if (HttpContext.Request.Url == null)
            {
                //redirect to shopkeeper.ng
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return Redirect("shopkeeper.ng");
            }
            

            var req = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;
            ViewBag.StoreImage = store.StoreLogoPath;
            ViewBag.StoreName = store.StoreName;
            var main = req + "/ngs.html#Store/Activate/Activate";

            SetAccountConnection(store.EntityConnectionString);

            var result = await UserManager.FindByEmailAsync(email);
            if (result == null)
            {
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return Redirect("shopkeeper.ng");
            }

            if (result.SecurityStamp != code)
            {
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return Redirect("shopkeeper.ng");
            }

            result.EmailConfirmed = true;
            var emailActivationResult = await UserManager.UpdateAsync(result);
            if (!emailActivationResult.Succeeded)
            {
                ViewBag.ConfirmStat = -1;
                ViewBag.ConfirmErr = message_Feedback.Activation_Failure;
                return Redirect("shopkeeper.ng");
            }

            return Redirect(main);

        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            GetIdentityConnection();
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            GetIdentityConnection();
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AdminLogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("XSwict");
        }

        [HttpPost]

        public ActionResult SignOut()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("SignIn", "Account");
        }

        [HttpPost]
        public ActionResult NgSignOut()
        {
            try
            {
                AuthenticationManager.SignOut();
                return Json( 1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private ActionResult RedirectToStoreUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Dashboard");
        }


        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}