"use strict";

define(['application-configuration', 'storeAddressServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('storeAddressTable', function ($compile)
    {
        return function ($scope, storeAddressTable)
        {
            var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/StoreAddress/GetStoreAddressObjects";
                tableOptions.itemId = 'StoreAddressId';
                tableOptions.columnHeaders = ['StreetNo', 'CityName'];
                var ttc = tableManager($scope, $compile, storeAddressTable, tableOptions, 'New Store Address', 'prepareStoreAddressTemplate', 'getStoreAddress', 'deleteStoreAddress', 93);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.jtable = ttc;
            }
        };
    });
    
    app.register.controller('StoreAddressController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeAddressServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeAddressServices, alertsService)
    {
        $rootScope.applicationModule = "StoreAddress";
        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
       $scope.initializeController = function ()
       {
           $scope.cities = [];
           storeAddressServices.getCities($scope.getCitiesCompleted);
           $scope.storeAddress = new Object();
           $scope.storeAddress.StoreAddressId = 0;
           $scope.storeAddress.StreetNo = '';
           $scope.storeAddress.City = new Object();
           $scope.storeAddress.City.StoreCityId = '';
           $scope.storeAddress.City.Name = '';

       };

       $scope.getCitiesCompleted = function (data)
       {
           $scope.cities = data;
       };

       $scope.prepareStoreAddressTemplate = function ()
       {
           $scope.initializeController();
           $scope.storeAddress.Header = 'Create New Store Address';
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/StoreAddresses/ProcessStoreAddresses.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };

       $scope.processStoreAddress = function ()
       {
           var storeAddress = new Object();
           storeAddress.Name = $scope.storeAddress.StreetNo;
           storeAddress.StateId = $scope.storeAddress.City.StoreCityId;
           if (storeAddress.StreetNo == undefined || storeAddress.StreetNo.length < 1)
           {
               alert("ERROR: Please provide Address. ");
               return;
           }

           if (storeAddress.StoreCityId == undefined || storeAddress.StoreCityId < 1)
           {
               alert("ERROR: Please select a City. ");
               return;
           }
           
           if ($scope.storeAddress.StoreAddressId < 1)
           {
               storeAddressServices.addStoreAddress(storeAddress, $scope.processStoreAddressCompleted);
           }
           
           else
           {
               storeAddressServices.editStoreAddress(storeAddress, $scope.processStoreAddressCompleted);
           }

       };

       $scope.processStoreAddressCompleted = function (data)
       {
           if (data.StoreAddressId < 1)
           {
                alert(data.Name);

           }
           else
           {
            alert(data.Error);
            ngDialog.close('/ng-shopkeeper/Views/Store/StoreAddresses/ProcessStoreAddresses.html', '');
            $scope.jtable.fnClearTable();
            $scope.initializeController();
           }
        };

       $scope.getStoreAddress = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           storeAddressServices.getStoreAddress(id, $scope.getStoreAddressCompleted);
       };

       $scope.getStoreAddressCompleted = function (data)
       {
           if (data.StoreAddressId < 1)
           {
             alert("ERROR: StoreAddress information could not be retrieved! ");
           }
            else
            {
                $scope.initializeController();
                $scope.storeAddress = data;
                $scope.storeAddress.State = {};
                $scope.storeAddress.State.StateId = data.StateId; 
                $scope.storeAddress.State.Name = '';
                $scope.storeAddress.Header = 'Update StoreAddress Information';
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/StoreAddresses/ProcessStoreAddresses.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
        };
        
       $scope.deleteStoreAddress = function (id)
       {
           if (parseInt(id) > 0)
           {
               if (!confirm("This StoreAddress information will be deleted permanently. Continue?"))
               {
                   return;
               }
               storeAddressServices.deleteStoreAddress(id, $scope.deleteStoreAddressCompleted);
           } else {
               alert('Invalid selection.');
           }
       };

       $scope.deleteStoreAddressCompleted = function (data)
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
    }]);
   
});

