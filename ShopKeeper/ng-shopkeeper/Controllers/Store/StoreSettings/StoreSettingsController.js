"use strict";

define(['application-configuration', 'storeSettingsServices', 'ngDialog'], function (app)
{
    app.register.controller('storeSettingsController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeSettingsServices',
    function (ngDialog, $scope, $rootScope, $routeParams, storeSettingsServices)
    {
        $scope.initializeController = function ()
        {
          storeSettingsServices.getCities($scope.getCitiesCompleted);
        };

        $scope.getCitiesCompleted = function (data)
        {
            $scope.cities = data;
        };
        
        $scope.processStoreSetting = function ()
        {
            var storeOutlet = new Object();

            storeOutlet.StoreOutletId = $scope.storeOutlet.StoreOutletId;
            storeOutlet.OutletName = $scope.storeOutlet.OutletName;
            storeOutlet.StoreAddressObject = { 'StreetNo': $scope.storeOutlet.StoreAddress, 'StoreCityId': $scope.storeOutlet.City.StoreCityId };
            storeOutlet.DefaultTax = $scope.storeOutlet.DefaultTax;
            if ($scope.storeOutlet.IsOperational)
            {
                storeOutlet.IsOperational = true;
            }
            else
            {
                storeOutlet.IsOperational = false;
            }
            
            if ($scope.storeOutlet.IsMainOutlet)
            {
                storeOutlet.IsMainOutlet = true;
            }
            else
            {
                storeOutlet.IsMainOutlet = false;
            }

            storeOutlet.FacebookHandle = $scope.storeOutlet.FacebookHandle;
            storeOutlet.TwitterHandle = $scope.storeOutlet.TwitterHandle;
            
            if (parseInt(storeOutlet.StoreCityId) < 1)
            {
                alert("ERROR: Please select a City. ");
                return;
            }
            
            if (storeOutlet.OutletName == undefined || storeOutlet.OutletName == null || storeOutlet.OutletName < 1)
            {
                alert("ERROR: Please provide Outlet Name. ");
                return;
            }
            if (storeOutlet.StoreAddressObject.StreetNo == undefined || storeOutlet.StoreAddressObject.StreetNo == null || storeOutlet.StoreAddressObject.StreetNo < 1) {
                alert("ERROR: Please provide Outlet Address. ");
                return;
            }

            if ($scope.storeOutlet.StoreOutletId < 1)
            {
                storeSettingsServices.addStoreOutlet(storeOutlet, $scope.processStoreOutletCompleted);
            }
            else
            {
                storeSettingsServices.editStoreOutlet(storeOutlet, $scope.processStoreOutletCompleted);
            }

        };
        $scope.processStoreSettingCompleted = function (data)
        {
            if (data.Code < 1)
            {
                   alert(data.Error);

            }
            else
            {
                alert(data.Error);
                ngDialog.close('/ng-shopkeeper/Views/Store/Outlets/ProcessOutlets.html', '');
                $scope.jtable.fnClearTable();
                $scope.initializeController();
           }
        };
        $scope.previewImage = function (e)
        {
            var el = (e.srcElement || e.target);
            if (el.files == null && el.files.length < 1)
            {
                return;
            }

            var obj = (e.srcElement || e.target).files[0];
            $scope.itemImgControl = el;

            fileReader.readAsDataUrl(obj, $scope)
            .then(function (result)
            {
                $scope.img.ImagePath2 = result;
            });

            $scope.img.ImageObj = obj;
        };

    }]);

});