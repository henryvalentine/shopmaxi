using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
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
	public class UserProfileController : Controller
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
        public ActionResult GetUserProfileObjects(JQueryDataTableParamModel param)
        {
            try
            {
                IEnumerable<UserProfileObject> filteredUserProfileObjects;
                var countG = new UserProfileServices().GetObjectCount();

                var pagedUserProfileObjects = GetUserProfiles(param.iDisplayLength, param.iDisplayStart);

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredUserProfileObjects = new UserProfileServices().Search(param.sSearch);
                }
                else
                {
                    filteredUserProfileObjects = pagedUserProfileObjects;
                }

                if (!filteredUserProfileObjects.Any())
                {
                    return Json(new List<UserProfileObject>(), JsonRequestBehavior.AllowGet);
                }

                var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                Func<UserProfileObject, string> orderingFunction = (c => sortColumnIndex == 1 ? c.LastName : sortColumnIndex == 2 ? c.OtherNames :
                    sortColumnIndex == 3 ? c.Gender : c.Salutation
                    );

                var sortDirection = Request["sSortDir_0"]; // asc or desc
                filteredUserProfileObjects = sortDirection == "asc" ? filteredUserProfileObjects.OrderBy(orderingFunction) : filteredUserProfileObjects.OrderByDescending(orderingFunction);

                var displayedUserProfilenels = filteredUserProfileObjects;

                var result = from c in displayedUserProfilenels
                             select new[] { Convert.ToString(c.Id), c.Salutation, c.LastName, c.OtherNames, c.Gender, c.BirthdayStr };
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = countG,
                    iTotalDisplayRecords = filteredUserProfileObjects.Count(),
                    aaData = result
                },
                   JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<UserProfileObject>(), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AddUserProfile(UserProfileObject person)
        {
            var gVal = new GenericValidator();
            try
            {
                if (ModelState.IsValid)
                {
                    var valStatus = ValidateUserProfile(person);
                    if (valStatus.Code < 1)
                    {
                        gVal.Code = -1;
                        gVal.Error = valStatus.Error;
                        return Json(gVal, JsonRequestBehavior.AllowGet);
                    }

                    var imgPath = SaveFile("");
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        person.PhotofilePath = imgPath;
                    }

                    var k = new UserProfileServices().AddUserProfile(person);
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
        public ActionResult EditUserProfile(UserProfileObject person)
        {
            var gVal = new GenericValidator();
            try
            {
                //if (ModelState.IsValid)
                //{
                var valStatus = ValidateUserProfile(person);
                if (valStatus.Code < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = valStatus.Error;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_person"] == null)
                {
                    gVal.Code = -5;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                var oldUserProfile = Session["_person"] as UserProfileObject;
                if (oldUserProfile == null || oldUserProfile.Id < 1)
                {
                    gVal.Code = -5;
                    gVal.Error = message_Feedback.Session_Time_Out;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                oldUserProfile.LastName = person.LastName.Trim();
                oldUserProfile.OtherNames = person.OtherNames.Trim();
                oldUserProfile.Gender = person.Gender;
                oldUserProfile.Birthday = person.Birthday;

                var formerImagePath = string.Empty;
                if (!string.IsNullOrEmpty(oldUserProfile.PhotofilePath))
                {
                    formerImagePath = oldUserProfile.PhotofilePath;
                }

                var newImagePath = SaveFile(formerImagePath);

                if (!string.IsNullOrEmpty(newImagePath))
                {
                    oldUserProfile.PhotofilePath = newImagePath;
                }

                if (!string.IsNullOrEmpty(oldUserProfile.PhotofilePath))
                {
                    if (!string.IsNullOrEmpty(oldUserProfile.PhotofilePath))
                    {
                        DeleteFile(oldUserProfile.PhotofilePath);
                    }

                    oldUserProfile.PhotofilePath = person.PhotofilePath;
                }


                var k = new UserProfileServices().UpdateUserProfile(oldUserProfile);
                if (k < 1)
                {
                    gVal.Error = k == -3 ? message_Feedback.Item_Duplicate : message_Feedback.Update_Failure;
                    gVal.Code = 0;
                    return Json(gVal, JsonRequestBehavior.AllowGet);
                }

                gVal.Code = k;
                gVal.Error = message_Feedback.Model_State_Error;
                return Json(gVal, JsonRequestBehavior.AllowGet);
                //}

                //gVal.Code = -5;
                //gVal.Error = message_Feedback.Model_State_Error;
                //return Json(gVal, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                gVal.Code = 0;
                gVal.Error = message_Feedback.Process_Failed;
                return Json(gVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteUserProfile(long id)
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

                var k = new UserProfileServices().DeleteUserProfile(id);
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
        public ActionResult GetUserProfile(long id)
        {
            var gVal = new GenericValidator();
            try
            {
                if (id < 1)
                {
                    return Json(new UserProfileObject(), JsonRequestBehavior.AllowGet);
                }

                var person = new UserProfileServices().GetUserProfile(id);
                if (id < 1)
                {
                    return Json(new UserProfileObject(), JsonRequestBehavior.AllowGet);
                }
                Session["_person"] = person;
                return Json(person, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new UserProfileObject(), JsonRequestBehavior.AllowGet);


            }
        }

        public ActionResult GetSalutations(string subdomain)
        {
            try
            {
                return Json(GetSalutationObjects(), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new List<SalutationObject>(), JsonRequestBehavior.AllowGet);
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
                    Session["_personImg"] = file;
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

        private string SaveFile(string formerPath)
        {
            try
            {
                if (Session["_personImg"] == null)
                {
                    return string.Empty;
                }

                var file = Session["_personImg"] as HttpPostedFileBase;

                if (file != null && file.ContentLength > 0)
                {
                    var mainPath = Server.MapPath("~/Uploads/UserProfiles");

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
                        Session["_personImg"] = null;
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
        private List<UserProfileObject> GetUserProfiles(int? itemsPerPage, int? pageNumber)
        {
            return new UserProfileServices().GetUserProfileObjects(itemsPerPage, pageNumber) ?? new List<UserProfileObject>();
        }

        private List<SalutationObject> GetSalutationObjects()
        {
            return new SalutationServices().GetSalutations() ?? new List<SalutationObject>();
        }

        private GenericValidator ValidateUserProfile(UserProfileObject person)
        {
            var gVal = new GenericValidator();
            if (person == null)
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.Fatal_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(person.LastName))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UserProfile_Last_Name_Error;
                return gVal;
            }
            if (string.IsNullOrEmpty(person.OtherNames))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UserProfile_Other_Names_Error;
                return gVal;
            }

            if (string.IsNullOrEmpty(person.Gender))
            {
                gVal.Code = -1;
                gVal.Error = message_Feedback.UserProfile_Gender_Error;
                return gVal;
            }

            // if (person.SalutationId < 1)
            //{
            //    gVal.Code = -1;
            //    gVal.Error = message_Feedback.UserProfile_Salutation_Error;
            //    return gVal;
            //}

            gVal.Code = 5;
            return gVal;
        }
        #endregion

    }
}
