"use strict";

define(['application-configuration', 'storeOutletServices', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.controller('storeOutletController', ['$scope', '$rootScope', '$routeParams', 'storeOutletServices', '$upload', 'fileReader',
    function ($scope, $rootScope, $routeParams, storeOutletServices, $upload, fileReader)
    {
       
        $scope.initializeController = function ()
        {
            $scope.storeOutlet = new Object();
            $scope.storeOutlet.StoreOutletId = 0;
            $scope.storeOutlet.City = new Object();
            $scope.storeOutlet.StoreSettingObject = new Object();
            $scope.storeOutlet.Country = {Name : '-- select --', StoreCountryId : ''};
            $scope.storeOutlet.State = { "Name": '-- select --', StoreStateId: '' };
            $scope.storeOutlet.StoreSettingObject.StoreCurrencyObject = { Name: '-- select --', StoreCurrencyId: '' };;
            $scope.storeOutlet.City.StoreCityId = 0;
            $scope.storeOutlet.City.Name = '';
            $scope.storeOutlet.DeductStockAtSalesPoint = false;
            $scope.storeOutlet.OutletName = '';
            $scope.storeOutlet.StoreAddress = '';
            $scope.storeOutlet.StoreSettingObject.Url = '';
            $scope.storeOutlet.DefaultTax = '';
            $scope.storeOutlet.FacebookHandle = '';
            $scope.storeOutlet.TwitterHandle = '';
            $scope.storeOutlet.IsOperational = true;
            $scope.storeOutlet.IsMainOutlet = false;
            $scope.storeOutlet.StoreSettingObject.DbSyncTimeShedule = new Date();
            storeOutletServices.getCurrencies($scope.getCurrenciesComplted);

            //currencies
        };

        //Time picker settings
        setTime($scope, '', '');
        
        $scope.getCurrenciesComplted = function (currencies)
        {
            if (currencies.length > 0)
            {
                angular.forEach(currencies, function (c, i)
                {
                    c.Name = c.Name + '(' + c.Symbol + ')';
                    if (c.StoreCurrencyId === $scope.storeOutlet.StoreSettingObject.DefaultCurrencyId)
                    {
                        $scope.storeOutlet.StoreSettingObject.StoreCurrencyObject = c;
                    }
                });
               
                $scope.currencies = currencies;
            }
            
        };

        $scope.prepareStoreOutletTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Outlets/ProcessOutlets.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processStoreOutlet = function ()
        {
            $scope.storeOutlet.StoreAddressObject = { 'StreetNo': $scope.storeOutlet.StoreAddress, 'StoreCityId': $scope.storeOutlet.City.StoreCityId };
            $scope.storeOutlet.IsOperational = true;
            $scope.storeOutlet.IsMainOutlet = true;
            $scope.storeOutlet.StoreSettingObject.DefaultCurrencyId = $scope.storeOutlet.StoreSettingObject.StoreCurrencyObject.StoreCurrencyId;

            if (parseInt($scope.storeOutlet.StoreSettingObject.DefaultCurrencyId) < 1)
            {
                alert("ERROR: Please select your opertaional currency. ");
                return;
            }
            
            if ($scope.storeOutlet.OutletName == undefined || $scope.storeOutlet.OutletName == null || $scope.storeOutlet.OutletName < 1)
            {
                alert("ERROR: Please provide Outlet Name. ");
                return;
            }
            if ($scope.storeOutlet.StoreAddressObject.StreetNo == undefined || $scope.storeOutlet.StoreAddressObject.StreetNo == null || $scope.storeOutlet.StoreAddressObject.StreetNo < 1) {
                alert("ERROR: Please provide Outlet Address. ");
                return;
            }

            if ($scope.storeOutlet.StoreOutletId < 1)
            {
                storeOutletServices.addStoreOutlet($scope.storeOutlet, $scope.processStoreOutletCompleted);
            }
            else
            {
                storeOutletServices.editStoreOutlet($scope.storeOutlet, $scope.processStoreOutletCompleted);
            }

        };
        
        $scope.getCitiesCompleted = function (data)
        {
            $scope.cities = data;
        };

        $scope.processStoreOutletCompleted = function (data)
        {
            alert(data.Error);
        };

        $scope.getStoreOutlet = function ()
        {
            storeOutletServices.getStoreOutlet($scope.getStoreOutletCompleted);
        };
        
        $scope.getStoreOutletCompleted = function (data)
        {
            $scope.initializeController();

            if (data.StoreOutletId > 0)
            {
                $scope.storeOutlet = data;
                if ($scope.storeOutlet.StoreSettingObject.StoreLogoPath === undefined || $scope.storeOutlet.StoreSettingObject.StoreLogoPath === null || $scope.storeOutlet.StoreSettingObject.StoreLogoPath.length < 1) {
                    $scope.storeOutlet.StoreSettingObject.StoreLogoPath = "/Content/images/noImage.png";
                }
                var countries = $rootScope.countries.filter(function (r)
                {
                    return r.StoreCountryId === data.StoreCountryId;
                });

                if (countries.length > 0)
                {
                    $scope.storeOutlet.Country = countries[0];
                    $rootScope.getStates($scope.storeOutlet.Country, $scope.getStatesCompleted);
                }

                $scope.storeOutlet.StoreAddress = data.StoreSettingObject.StoreAddress;
            }
        };
        
        $scope.getStatesCompleted = function (states)
        {
            $rootScope.processing = false;
            if (states == null || states.length < 1)
            {
                return;
            }

            $scope.states = states;

            var lStates = $scope.states.filter(function (r)
            {
                return r.StoreStateId === $scope.storeOutlet.StoreStateId;
            });

            if (states.length > 0)
            {
                $scope.storeOutlet.State = lStates[0];

                $rootScope.getCities($scope.storeOutlet.State, $scope.getCitiesCompleted);
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
                return r.StoreCityId === $scope.storeOutlet.StoreCityId;
            });

            if (lCities.length > 0)
            {
                $scope.storeOutlet.City = lCities[0];
            }
        };

        //preview stock image
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
                $scope.storeOutlet.StoreSettingObject.StoreLogoPath = result;
            });

            $scope.processImage(obj);
        };

        $scope.processImage = function (img) 
        {
            if ((img === undefined || img == null || img.size < 1) && ($scope.img.ImagePath == null || $scope.img.ImagePath.length < 1)) {
                alert('Please attach at Image.');
                return;
            }

            if (img !== undefined && img !== null && img.size > 4096000)
            {
                alert('The Image size should not be larger than 4MB.');
                return;
            }

            var url = "/StoreOutlet/UploadStoreLogo?oldPath=" + $scope.storeOutlet.StoreSettingObject.OriginalLogoPath;

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
                        $scope.storeOutlet.StoreSettingObject.OriginalLogoPath = data.FilePath;
                    }
                });
        };

    }]);

});