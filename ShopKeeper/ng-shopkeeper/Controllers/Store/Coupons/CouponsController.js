"use strict";

define(['application-configuration', 'couponServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngCouponTable', function ($compile)
    {
        return function ($scope, ngCouponTable)
        {var authStatus = $scope.getAuthStatus();
            if (authStatus == false) {
                alert(authStatus);
                $scope.redir();
            } else {
                var tableOptions = {};
                tableOptions.sourceUrl = "/Coupon/GetCouponObjects";
                tableOptions.itemId = 'CouponId';
                tableOptions.columnHeaders = ['Title', 'Code', 'PercentageDeduction', 'MinimumOrderAmount', 'ValidityPeriod'];
                var ttc = tableManager($scope, $compile, ngCouponTable, tableOptions, 'New Coupon ', 'prepareCouponTemplate', 'getCoupon', 'deleteCoupon', 117);
                ttc.removeAttr('width').attr('width', 'auto');
                $scope.ttc = ttc;
            }
        };
    });
    
    app.register.controller('couponController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'couponServices', '$upload', 'fileReader','$http', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, couponServices, $upload, fileReader,$http, alertsService)
    {

        $scope.getAuthStatus = function () {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function () {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };
        $scope.initializeController = function ()
        {
            $scope.alerts = [];
            $scope.coupon = { 'CouponId': '', 'Title': '', 'Code': '', 'PercentageDeduction': '', 'ValidFrom': '', 'ValidTo': '', 'MinimumOrderAmount': '', 'Header': 'Update Coupon Information' };
        };
        

        var xcvb = new Date();
        var year = xcvb.getFullYear();
        var month = xcvb.getMonth() + 1;
        var day = xcvb.getDate();
        var minDate = year + '/' + month + '/' + day;

        setControlDate($scope, minDate, '');
        setEndDate($scope, minDate, '');

        $scope.prepareCouponTemplate = function ()
        {
            $scope.initializeController();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/Coupons/ProcessCoupons.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processCoupon = function ()
        {
            var coupon = new Object();
            coupon.Title = $scope.coupon.Title;
            coupon.Title = $scope.coupon.Code;
            coupon.CouponId = $scope.coupon.CouponId;
            coupon.PercentageDeduction = $scope.coupon.PercentageDeduction;
            coupon.ValidFrom = $scope.coupon.ValidFrom;
            coupon.ValidTo = $scope.coupon.ValidTo;
            coupon.MinimumOrderAmount = $scope.coupon.MinimumOrderAmount;
            
            if (coupon.Title == undefined || coupon.Title.length < 1 || coupon.Title == null)
            {
                alert("ERROR: Please provide Coupon  Title.");
                return;
            }
            
            if (coupon.Code == undefined || coupon.Code == null || coupon.Code.length < 1)
            {
                alert("ERROR: Please provide Coupon Code.");
                return;
            }
            
            if(coupon.PercentageDeduction == undefined || coupon.PercentageDeduction == null || coupon.PercentageDeduction.length < 1 || parseFloat(coupon.PercentageDeduction) < 1)
            {
                alert("ERROR: Please provide percentage Deduction.");
                return;
            }
            
            if (coupon.ValidFrom == null)
            {
                alert("ERROR: Please provide Validity Start Date .");
                return;
            }
            
            if (coupon.ValidTo == null)
            {
                alert("ERROR: Please provide Validity End Date .");
                return;
            }
            
            if (parseInt(coupon.CouponTypeId) < 1)
            {
                alert("ERROR: Please Coupon Type.");
                return;
            }
            
            if (coupon.MinimumOrderAmount == null || parseInt(coupon.MinimumOrderAmount) < 1)
            {
                alert("ERROR: Please Minimum Order Amount.");
                return;
            }


            if ($scope.coupon.CouponId < 1 )
            {
                couponServices.addCoupon(coupon, $scope.processCouponCompleted);
            }
            else
            {
                couponServices.editCoupon(coupon, $scope.processCouponCompleted);
            }
        };

        $scope.processCouponCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Code);

            }
            else
            {
                alert(data.Error);
                ngDialog.close('/ng-shopkeeper/Views/Store/Coupons/ProcessCoupons.html', '');
                $scope.ttc.fnClearTable();
                $scope.initializeController();
            }
        };
        
        $scope.getCoupon = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id == NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            couponServices.getCoupon(id, $scope.getCouponCompleted);
        };
       
        $scope.getCouponCompleted = function (data)
        {
            if (data.CouponId < 1)
            {
               alert("ERROR: Coupon  information could not be retrieved! ");
            }
            else
            {
                $scope.initializeController();
                
                $scope.coupon = data;

                $scope.coupon = {'CouponId': data.CouponId, 'Title': data.Title, 'PercentageDeduction': data.PercentageDeduction, 'ValidFrom': data.ValidFrom, 'ValidTo': data.ValidFrom, 'MinimumOrderAmount': data.MinimumOrderAmount, 'Header': 'Update Coupon  Information'};
                
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Coupons/ProcessCoupons.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            }
         };

        $scope.deleteCoupon = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Coupon information will be deleted permanently. Continue?"))
                {
                    return;
                }
                couponServices.deleteCoupon(id, $scope.deleteCouponCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteCouponCompleted = function (data)
        {
            if (data.Code < 1)
            {
                alert(data.Error);

            }
            else
            {
                $scope.ttc.fnClearTable();
                alert(data.Error);
            }
        };
        
    }]);
    
});

var genericListObject = {};
var itemsLoaded = false;



