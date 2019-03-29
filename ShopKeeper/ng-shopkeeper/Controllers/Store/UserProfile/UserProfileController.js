"use strict";

define(['application-configuration', 'employeeServices', 'angularFileUpload', 'fileReader', 'ngDialog'], function (app)
{
    app.register.controller('profileController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'employeeServices', '$upload', 'fileReader',
    function (ngDialog, $scope, $rootScope, $routeParams, employeeServices, $upload, fileReader)
    {
       $scope.initializeController = function ()
       {
           $scope.initializeModel();
           employeeServices.getMyProfile($scope.getMyProfileCompleted);
       };

       $scope.initializeModel = function ()
       {
           var profile = new Object();
           $scope.img = { path: '/Content/images/noImage.png' }
           profile.StreetNo = '';
           profile.StoreCityId = '';
           profile.StoreAddressId = '';
           
            profile.city =
            {
                'StoreCityId': '',
                'Name': '-- Select --'
            };
           
           $scope.profile = profile;
       };
        
       var xcvb = new Date();
       var year = xcvb.getFullYear();
       var month = xcvb.getMonth() + 1;
       var day = xcvb.getDate();
       var maxDate = year + '/' + month + '/' + day;

       setControlDate($scope, '', maxDate);
        
       $scope.processProfile = function ()
       {
            $scope.profile.StoreCityId = $scope.profile.City.StoreCityId;
            $scope.profile.StoreStateId = $scope.profile.State.StoreStateId;
            $scope.profile.StoreCountryId = $scope.profile.Country.StoreCountryId;
            $scope.profile.Birthday = $scope.profile.BirthdayStr;

            if ($scope.profile.Password !== undefined && $scope.profile.Password !== null && $scope.profile.Password.length > 0)
            {

                if ($scope.profile.OriginalPassword === undefined || $scope.profile.OriginalPassword === null || $scope.profile.OriginalPassword.length < 1)
                {
                   alert("Please provide your original password.");
                   return;
                }

                if ($scope.profile.ConfirmPassword === undefined || $scope.profile.ConfirmPassword === null || $scope.profile.ConfirmPassword.length < 1) {
                    alert("Please provide password confirmation.");
                    return;
                }

                if ($scope.profile.Password !== $scope.profile.ConfirmPassword)
                {
                   alert("The passwords do not match");
                   return;
                }

                if ($scope.profile.Password.length < 8 || $scope.profile.ConfirmPassword.length < 8)
                {
                    alert("The passwords should be at least 8 characters wide");
                    return;
                }
           }

           if (!$scope.validatateProfile($scope.profile))
            {
                return;
            }
            employeeServices.editMyProfile($scope.profile, $scope.processProfileCompleted);
       };
       
       $scope.processAdminProfile = function ()
       {
           $scope.profile.Birthday = $scope.profile.BirthdayStr;

           if (!$scope.validatateAdminProfile($scope.profile))
           {
               return;
           }
            
            employeeServices.editAdminProfle($scope.profile, $scope.processProfileCompleted);

       };

       $scope.validatateProfile = function (profile)
       {
           if (profile.StoreCityId == undefined || profile.StoreCityId < 1)
           {
                alert("ERROR: Please select City. ");
                return false;
            }

            if (profile.StreetNo == null || profile.StreetNo.length < 1)
            {
                alert("ERROR: Please Provide Address. ");
                return false;
            }

            if (profile.PhoneNumber == null || profile.PhoneNumber.length < 1) 
            {
                alert("ERROR: Please Provide Phone Number.");
                return false;
            }

            if (profile.OtherNames == null || profile.OtherNames.length < 1)
            {
                alert("ERROR: Please Provide Other Names.");
                return false;
            }

            if (profile.LastName == null || profile.LastName.length < 1)
            {
                alert("ERROR: Please Provide Last Name.");
                return false;
            }
           
            if (profile.Email == null || profile.Email.length < 1)
            {
                alert("ERROR: Please Provide Email.");
                return false;
            }

            return true;
        };

       $scope.validatateAdminProfile = function (profile)
       {
           if (profile.PhoneNumber == null || profile.PhoneNumber.length < 1) {
               alert("ERROR: Please Provide Phone Number.");
               return false;
           }

           if (profile.OtherNames == null || profile.OtherNames.length < 1) {
               alert("ERROR: Please Provide Other Names.");
               return false;
           }

           if (profile.LastName == null || profile.LastName.length < 1) {
               alert("ERROR: Please Provide Last Name.");
               return false;
           }

           if (profile.Email == null || profile.Email.length < 1) {
               alert("ERROR: Please Provide Email.");
               return false;
           }

           return true;
       };

       $scope.processProfileCompleted = function (data)
       {
           alert(data.Error);
           if (data.PasswordUpdated === true) {
               $rootScope.logOut();
           }
        };

       $scope.getProfile = function ()
       {
           employeeServices.getMyProfile($scope.getMyProfileCompleted);
       };

       $scope.getMyProfileCompleted = function (data)
       {
           if (data.Code < 1)
           {
             alert(data.Error);
           }
            else
           {
               if ($rootScope.isAdmin === false)
               {
                   data.StreetNo = data.Address;

                   var countries = $rootScope.countries.filter(function (r)
                   {
                       return r.StoreCountryId === data.StoreCountryId;
                   });

                   if (countries.length > 0)
                   {
                       data.Country = countries[0];
                       $rootScope.getStates(data.Country, $scope.getStatesCompleted);
                   }

               }

               $scope.img.path = data.PhotofilePath;
               $scope.profile = data;
            }
       };
        
        $scope.getStatesCompleted = function (states) {
           $rootScope.processing = false;
           if (states == null || states.length < 1) {
               return;
           }

           $scope.states = states;

           var lStates = $scope.states.filter(function (r)
           {
               return r.StoreStateId === $scope.profile.StoreStateId;
           });

           if (lStates.length > 0)
           {
               $scope.profile.State = lStates[0];
               $rootScope.getCities($scope.profile.State, $scope.getCitiesCompleted);
           }
       };

       $scope.getCitiesCompleted = function (cities)
       {
           $rootScope.processing = false;
           if (cities == null || cities.length < 1)
           {
               return;
           }

           $scope.cities = cities;

           var lCities = $scope.cities.filter(function (r)
           {
               return r.StoreCityId === $scope.profile.StoreCityId;
           });

           if (lCities.length > 0)
           {
               $scope.profile.City = lCities[0];
           }
       };

        //preview stock image
       $scope.previewImage = function (e) {
           var el = (e.srcElement || e.target);
           if (el.files == null && el.files.length < 1) {
               return;
           }

           var obj = (e.srcElement || e.target).files[0];
           $scope.itemImgControl = el;

           fileReader.readAsDataUrl(obj, $scope)
           .then(function (result)
           {
               $scope.img.path = result;
           });

           $scope.processImage(obj);
       };

       $scope.processImage = function (img) {
           if (img === undefined || img == null || img.size < 1) {
               alert('Please attach an Image.');
               return;
           }

           if (img.size > 4096000)
           {
               alert('The Image size should not be larger than 4MB.');
               return;
           }

           var url = "/StoreOutlet/UpdateProfileImage";
           
           $rootScope.busy = true;
           $scope.uploading = true;

           $upload.upload({
               url: url,
               method: "POST",
               data: { file: img }
           })
               .success(function (data)
               {
                   $rootScope.busy = false;
                   $scope.uploading = false;

                   alert(data.Error);
                   if (data.Code > 0)
                   {
                       $scope.profile.PhotofilePath = data.FilePath;
                       $rootScope.user.PhotofilePath = data.FilePath;
                   }
               });
       };

    }]);
   
});

