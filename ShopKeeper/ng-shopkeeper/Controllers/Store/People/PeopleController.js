"use strict";

define(['application-configuration', 'personServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader', 'datePicker'], function (app)
{

    app.register.directive('ngUserProfilesTable', function ($compile)
    {
        return function ($scope, ngUserProfilesTable)
        { var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/UserProfile/GetUserProfileObjects";
                tableOptions.itemId = 'Id';
                tableOptions.columnHeaders = ['Salutation', 'LastName', 'OtherNames', 'Gender', 'BirthdayStr'];
                var ttc = tableManager($scope, $compile, ngUserProfilesTable, tableOptions, 'New UserProfile', 'prepareUserProfileTemplate', 'getUserProfile', 'deleteUserProfile', 118);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    }); 

    app.register.directive("ngVariantImg", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.person.Img = (e.srcElement || e.target).files[0];
                    $scope.ImgControl = el;
                    $scope.previewImg();
                });
            }
        };
    });

    app.register.controller('personController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'personServices', '$upload', 'fileReader','$timeout', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, personServices, $upload, fileReader,$timeout,alertsService)
    {
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $rootScope.applicationModule = "UserProfile";
       
        $scope.alerts = [];
        $scope.initializeController = function ()
        {
            $scope.person = new Object();
            $scope.person.Id = 0;
            $scope.person.Salutation = {'SalutationId' : '', 'Name' : '-- Select --'};
            $scope.person.LastName = '';
            $scope.person.OtherNames = '';
            $scope.person.BirthdayStr = '';
            $scope.person.PhotofilePath = '';
            $scope.person.genders = [{ 'Name': 'Male', 'GenderId': '1' }, {'Name': 'Female', 'GenderId': '2' }];
            $scope.person.Header = 'New UserProfile';
            $scope.person.Gender = { 'GenderId': '1', 'Name': '-- Select Gender --' };
            $scope.getSalutations();
        };
      
        $scope.getSalutations = function ()
        {
            personServices.getSalutations($scope.getSalutationsCompleted);
        };
        
        $scope.getSalutationsCompleted = function (data)
        {
            $scope.person.Salutations = data;
        };

        var xcvb = new Date();
        var year = xcvb.getFullYear() - 15;
        var month = 12;
        var day = 31;
        var maxDate = year + '/' + month + '/' + day;
        setControlDate($scope, '', maxDate);
        
        $scope.prepareUserProfileTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/UserProfiles/ProcessUserProfile.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
       
        $scope.processUserProfile = function ()
        {
            var person = new Object();
            person.LastName = $scope.person.LastName;
            person.OtherNames = $scope.person.OtherNames;
            person.Birthday = $scope.person.BirthdayStr;
            person.Gender = $scope.person.Gender.Name;
            person.SalutationId = $scope.person.Salutation.SalutationId;
            
            if (!$scope.ValidateUserProfile(person))
            {
                return;
            }
           
            if ($scope.person.Id < 1)
            {
                personServices.addUserProfile(person, $scope.processUserProfileCompleted);
            }
            else
            {
                personServices.editUserProfile(person, $scope.processUserProfileCompleted);
            }
         };
        
        $scope.processUserProfileCompleted = function (data)
        {
            if (data.Code < 1)
            {
               alert(data.Error);
            }
            else
            {
                alert(data.Error);
                ngDialog.close('/ng-shopkeeper/Views/Store/UserProfiles/ProcessUserProfile.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
            }
        };

        $scope.getUserProfile = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

           personServices.getUserProfile(id, $scope.getUserProfileCompleted);
        };
        
        $scope.getUserProfileCompleted = function (data)
        {
            if (data.Id < 1)
            {
                alert("ERROR: UserProfile information could not be retrieved! ");

            }
            else
            {
                $scope.initializeController();
                $scope.person.Id = data.Id;
                $scope.person.LastName = data.LastName;
                $scope.person.OtherNames = data.OtherNames;
                $scope.person.BirthdayStr = data.BirthdayStr;
                $scope.person.PhotofilePath = data.PhotofilePath;
                
                if (data.Gender == 'Female')
                {
                    $scope.person.Gender =
                    {
                        'GenderId': ' 2',
                        'Name': 'Female'
                    };
                }
                
                $scope.person.Salutation =
                {
                    'SalutationId': data.SalutationId,
                    'Name': data.Salutation
                };
                
                if (data.Gender == 'Male')
                {
                    $scope.person.Gender = { 'GenderId': ' 1', 'Name': 'Male' };
                }
                
                $scope.person.Header = 'Update UserProfile Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/UserProfiles/ProcessUserProfile.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
        $scope.deleteUserProfile = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This UserProfile information will be deleted permanently. Continue?")) {
                    return;
                }
                personServices.deleteUserProfile(id, $scope.deleteUserProfileCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteUserProfileCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.jtable.fnClearTable();
                alert(data.Error);
            }
        };
        
        $scope.previewImg = function ()
        {
            fileReader.readAsDataUrl($scope.person.Img, $scope)
            .then(function (result)
            {
                $scope.person.PhotofilePath = result;

            });
            
            $upload.upload({
                url: "/Prson/CreateFileSession",
                method: "POST",
                data: { file: $scope.person.Img },
            })
             .success(function (data)
             {
                 if (data.code < 1)
                 {
                     alert('Image could not be processed. Please try re-uploading it later.');
                 }
             });
        };
        
        $scope.ValidateUserProfile = function (person)
        {
            if (person.LastName == null || person.LastName.trim().length < 1)
            {
                alert("ERROR: Please Provide Last Name.");
                return false;
            }
            if (person.OtherNames == null || person.OtherNames.trim().length < 1)
            {
                alert("ERROR: Please Provide Other Names.");
                return false;
            }
            
            if (person.SalutationId == null || person.SalutationId < 1)
            {
                alert("ERROR: Please Provide Salution.");
                return false;
            }
            
            if (person.Gender == null || person.Gender.trim().length < 1)
            {
                alert("ERROR: Please select Gender.");
                return false;
            }
            return true;
        };
    }]);

});