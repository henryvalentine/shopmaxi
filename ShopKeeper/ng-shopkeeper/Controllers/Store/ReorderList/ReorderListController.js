"use strict";

define(['application-configuration', 'productServices', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngReorderList', function ($compile)
    {
        return function ($scope, ngReorderList)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/Report/GetReorderList";
            tableOptions.itemId = 'StoreItemStockId';
            tableOptions.columnHeaders = ['StoreItemName', 'SKU', 'QuantityInStockStr', 'ReorderLevelStr', 'ReorderQuantityStr'];
            var ttc = reorderListTableManager($scope, $compile, ngReorderList, tableOptions);
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ttc = ttc;
            
        };
        
    });

    app.register.controller('reorderListController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'productServices', '$upload', 'fileReader','$http',
    function (ngDialog, $scope, $rootScope, $routeParams, productServices, $upload, fileReader,$http)
    {
       
        
    }]);
    
});


