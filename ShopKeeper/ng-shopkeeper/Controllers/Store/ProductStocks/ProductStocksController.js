"use strict";

define(['application-configuration', 'storeItemStockServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader', 'datePicker'], function (app)
{
    app.register.directive('ngStocks', function ($compile)
    {
        return function ($scope, ngStocks)
        {
            var tableOptions = {};
            tableOptions.sourceUrl = "/StoreItemStock/GetStoreItemStockObjects";
            tableOptions.itemId = 'StoreItemStockId';
            tableOptions.columnHeaders = ["StoreItemName", "SKU", "CategoryName", "BrandName", "QuantityInStockStr", "QuantitySoldStr", "ExpiryDate"];
            var ttc = productStockInsertManager($scope, $compile, ngStocks, tableOptions, 'New Inventory', 'prepareStoreItemStockTemplate', '/StoreItemStock/DownloadContentFromFolder?path=~/BulkTemplates/NewInventory.xlsx', 'getStoreItemStock', 'getItemDetails', 'deleteStoreItemStock', 126);
            ttc.removeAttr('width').attr('width', '100%');
            $scope.ngTable = ttc;
        };
    });
    
    app.register.directive('ngSku', function ()
    {
        return function ($scope, ngSku, attrs)
        {
            ngSku.bind("keydown keypress", function (event)
            {
                if (event.which === 13)
                {
                    $scope.getProductsBySku();
                }
            });

            $scope.skuControl = ngSku;
            $scope.skuControl.focus();
        };
    });
   
    app.register.directive("ngVariantImg", function ()
    {
        return {
            link: function ($scope, el)
            {
                el.bind("change", function (e)
                {
                    $scope.targetElement = (e.srcElement || e.target).files[0];
                    $scope.variantImgControl = el;
                    $scope.previewVariantImage();
                });
            }
        };
    });

    app.register.controller('productStockController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'storeItemStockServices', '$upload', 'fileReader','$timeout', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, storeItemStockServices, $upload, fileReader,$timeout, alertsService)
    {
        $rootScope.applicationModule = "StoreItemStock";

        //date picker settings
        var xcvb = new Date();
        var year = xcvb.getFullYear();
        var month = xcvb.getMonth() + 1;
        var day = xcvb.getDate();
        var miniDate = year + '/' + month + '/' + day;
        setControlDate($scope, miniDate, '');

        $scope.getOutlets = function ()
        {
            storeItemStockServices.getOutlets($scope.getOutletsCompleted);
        };

        $scope.getOutletsCompleted = function (outlets)
        {
            $scope.outlets = outlets;
        };

        //Error Control Toggling 
        $scope.setError = function (errorMessage) {
            $scope.error = errorMessage;
            $scope.negativefeedback = true;
            $scope.success = '';
            $scope.positivefeedback = false;
        };

        $scope.clearError = function ()
        {
            $scope.error = '';
            $scope.negativefeedback = false;
            $scope.success = '';
            $scope.positivefeedback = false;
        };

        $scope.setSuccessFeedback = function (successMessage)
        {
            $scope.error = '';
            $scope.negativefeedback = false;
            $scope.success = successMessage;
            $scope.positivefeedback = true;
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
                $scope.img.ImagePath2 = result;
            });

            $scope.img.ImageObj = obj;
        };

        $scope.getAuthStatus = function ()
        {
            return $rootScope.isAuthenticated;
        };

        $scope.redir = function ()
        {
            $rootScope.redirectUrl = $location.path();
            $location.path = "/ngy.html#signIn";
        };

        $scope.initializeComponents = function ()
        {
            $scope.details = false;
            $scope.showInfo = false;
            $scope.hideNewEdit = false;

            $scope.images = [];
            $scope.prices = [];
            $scope.showImages = false;
            $scope.stockId = 0;
            $scope.storeItemStock = new Object();
            $scope.storeItemStock.StoreItemStockId = '';
            $scope.storeItemStock.Header = 'Create New Inventory';
            $scope.storeItemStock.StoreItemVariation = new Object();
            $scope.storeItemStock.StoreItemVariation.StoreItemVariationId = '';

            $scope.storeItemStock.StoreItemVariationValue = new Object();
            $scope.storeItemStock.StoreItemVariationValue.StoreItemVariationValueId = '';
            $scope.storeItemStock.QuantityInStock = '';
            $scope.storeItemStock.SKU = '';
            $scope.storeItemStock.Price = '';
            $scope.storeItemStock.PublishOnline = false;      
            $scope.storeItemStock.Discontinued = false;
            $scope.UnitOfMeasurementId = '';
            $scope.UoMCode = '';
            $scope.storeItemStock.MinimumQuantity = '';
            $scope.storeItemStock.ReorderLevel = '';
            $scope.storeItemStock.ReorderQuantity = '';
            $scope.storeItemStock.ShelfLocation = '';
            $scope.storeItemStock.VariantImage = '';
            $scope.storeItemStock.ExpirationDate = '';
            $scope.storeItemStock.VariantImage = '';
            $scope.storeItemStock.SKU = '';

            $scope.storeItemStock.StoreItemObject = {'StoreItemId': '', 'Name': '', 'Description' : '', 'StoreItemBrandId' : '', 'StoreItemTypeId' : '', 'StoreItemCategoryId' : '', 'ChartOfAccountId':'', 'TechSpechs' : ''};

            $scope.storeItemStock.StoreItemVariationObject = { 'StoreItemVariationId': '', 'Name': '-- Select option --'};
            $scope.storeItemStock.StoreItemVariationValueObject = { 'StoreItemVariationValueId': '', 'Name': '-- Select option --' };
            
            $scope.storeItemStock.StoreItemObject.ProductBrand = { 'StoreItemBrandId': '', 'Name': '-- Select Product Brand --' };

            $scope.storeItemStock.StoreItemObject.ParentProduct = { 'ParentItemId': '', 'Name': '-- Select Parent Product --' };

            $scope.storeItemStock.StoreItemObject.ProductCategory = { 'StoreItemCategoryId': '', 'Name': '-- Select Product Category --' };

            $scope.storeItemStock.StoreItemObject.ChartOfAccount = { 'ChartOfAccountId': '', 'AccountType': '-- Select Chart of Account --' };

            $scope.storeItemStock.StoreItemObject.ProductType = { 'StoreItemTypeId': '', 'Name': '-- Select Product Type --' };
        };

        $scope.initializeImg = function ()
        {
            $scope.img = { 'ImageViewObject': { 'ImageViewId': '', 'Name': '-- Select View --', 'Description': '' }, 'ImageViewId': '', 'imgObject': '', 'ImagePath': '', 'ImagePath2': '/Content/images/noImage.png', 'StoreItemStockId': '', 'StockUploadId': '' };
        };

        $scope.initializeController = function ()
        {
            $scope.outlet = { StoreOutletId: '', OutletName: '-- select outlet --' };
            $scope.initializeComponents();
            storeItemStockServices.getProductListObjects($scope.getProductListObjectsCompleted);
        };
        
        //Navigation
        $scope.prepareStoreItemStockTemplate = function ()
        {
            $scope.initializeComponents();
            $scope.newEdit = true;
        };

        $scope.goBack = function ()
        {
            $scope.clearError();
            $scope.newEdit = false;
            $scope.bulkUpload = false;
            $scope.stockId = $scope.storeItemStock.StoreItemStockId;
            $scope.initializePrice();
            $scope.initializeImg();
            $scope.initializeComponents();
        }

        $scope.hideUpload = function ()
        {
            $scope.newEdit = true;
            $scope.bulkUpload = false;
            $scope.stockId = $scope.storeItemStock.StoreItemStockId;
        }
        
        $scope.scanSKU = function ()
        {
            $scope.skuControl.focus();
        };

        $scope.generateSKU = function ()
        {
            if ($scope.dbx == null)
            {
                $scope.setError('Please Select Product Brand');
                return;
            }
            else
            {
                if ($scope.storeItemStock.SKU !== undefined && $scope.storeItemStock.SKU.length > 0)
                {
                    return;
                }

                $scope.count++;
                var sid = "";
                if ($scope.count >= 999999)
                {
                    sid = $scope.count.toString();
                }
                else
                {
                    var preceedingZeros = '';
                    var numStr = $scope.count.toString();
                    var countStr = '99999';

                    for (var z = numStr.length; z < countStr.length; z++)
                    {
                        preceedingZeros += '0';
                    }
                    sid = $scope.dbx + preceedingZeros + numStr;
                }

                $scope.storeItemStock.SKU = sid;
            }
        };

        $scope.getProductListObjectsCompleted = function (data)
        {
            var genericListObject = {};
            
            genericListObject.productTypes = data.ProductTypes;
            genericListObject.productCategories = data.ProductCategories;
            genericListObject.productBrands = data.ProductBrands;
            genericListObject.chartsOfAccount = data.ChartsOfAccount;
            $scope.genericListObject = genericListObject;
            storeItemStockServices.getGenericList2($scope.getGenericListCompleted);
            $scope.getOutlets();
        };

        $scope.getGenericListCompleted = function (data)
        {
            $scope.genericListObject.storeOutlets = data.StoreOutlets;
            $scope.genericListObject.storeItemVariations = data.StoreItemVariations;
            $scope.genericListObject.storeItemVariationValues = data.StoreItemVariationValues;
            $scope.genericListObject.storeItems = data.StoreItems;
            $scope.genericListObject.unitsofMeasurement = data.UnitsofMeasurement;
            $scope.genericListObject.currencies = data.StoreCurrencies;
            $scope.genericListObject.imageViews = data.ImageViews;

            if (data.StoreCurrencies !== undefined && data.StoreCurrencies.length > 0)
            {
                angular.forEach(data.StoreCurrencies, function (x, i)
                {
                    if (x.StoreCurrencyId === 1)
                    {
                        $scope.defaultCurrency = x;
                    }
                });
            }
        };
        
        $scope.getDbx = function (brand)
        {
            if (brand == null)
            {
                $scope.setError("Please select a valid Product Brand");
                return;
            }
            
            if (parseInt(brand.StoreItemBrandId) < 1)
            {
                $scope.setError("Please select a valid Product Brand");
                return;
            }
           
            storeItemStockServices.getDbx(brand.StoreItemBrandId, $scope.getDbxCompleted);
        };
       
        $scope.getDbxCompleted = function (count)
        {
            $scope.dbx = $scope.storeItemStock.StoreItemObject.ProductBrand.StoreItemBrandId;
            $scope.count = count;
        };
        
        $scope.processStoreItemStock = function ()
        {
            var product = new Object();
            product.StoreItemId = $scope.storeItemStock.StoreItemObject.StoreItemId;
            product.Description = $scope.storeItemStock.StoreItemObject.Description;
            product.TechSpechs = $scope.storeItemStock.StoreItemObject.TechSpechs;
            product.Name = $scope.storeItemStock.StoreItemObject.Name;
            product.StoreItemBrandId = $scope.storeItemStock.StoreItemObject.ProductBrand.StoreItemBrandId;
            product.StoreItemCategoryId = $scope.storeItemStock.StoreItemObject.ProductCategory.StoreItemCategoryId;
            if ($scope.storeItemStock.StoreItemObject.ChartOfAccount !== undefined && $scope.storeItemStock.StoreItemObject.ChartOfAccount !== null)
            {
                product.ChartOfAccountId = $scope.storeItemStock.StoreItemObject.ChartOfAccount.ChartOfAccountId;
            }
            
            product.StoreItemTypeId = $scope.storeItemStock.StoreItemObject.ProductType.StoreItemTypeId;
           
            if (product.Name == undefined || product.Name.length < 1 || product.Name == null)
            {
                $scope.setError("ERROR: Please provide Product  Name/Model.");
                return;
            }

            if (parseInt(product.StoreItemBrandId) < 1)
            {
                $scope.setError("ERROR: Please Product Brand.");
                return;
            }

            if (parseInt(product.StoreItemCategoryId) < 1)
            {
                $scope.setError("ERROR: Please Product Category.");
                return;
            }
            
            if (parseInt(product.StoreItemTypeId) < 1)
            {
                $scope.setError("ERROR: Please Product Type.");
                return;
            }
            
            var storeItemStock = new Object();
            storeItemStock.SKU = $scope.storeItemStock.SKU;

            if ($scope.storeItemStock.StoreItemVariationObject.StoreItemVariationId > 0)
            {
                storeItemStock.StoreItemVariationId = $scope.storeItemStock.StoreItemVariationObject.StoreItemVariationId;
                if (parseInt($scope.storeItemStock.StoreItemVariationValueObject.StoreItemVariationValueId))
                {
                    storeItemStock.StoreItemVariationValueId = $scope.storeItemStock.StoreItemVariationValueObject.StoreItemVariationValueId;
                }
                else
                {
                    $scope.setError('Please select a Distinguishing Value!');
                    return;
                }
            }

            storeItemStock.StoreItemId = product.StoreItemId;
            storeItemStock.StoreItemStockId = $scope.storeItemStock.StoreItemStockId;
            storeItemStock.QuantityInStock = $scope.storeItemStock.QuantityInStock;
            storeItemStock.CostPrice = $scope.storeItemStock.CostPrice;
            storeItemStock.ReorderLevel = $scope.storeItemStock.ReorderLevel;
            storeItemStock.ReorderQuantity = $scope.storeItemStock.ReorderQuantity;
            storeItemStock.ShelfLocation = $scope.storeItemStock.ShelfLocation;
            storeItemStock.ExpirationDate = $scope.storeItemStock.ExpirationDate;
            storeItemStock.StoreCurrencyId =  $scope.defaultCurrency.StoreCurrencyId;
            storeItemStock.StoreItemObject = product;

            if (!$scope.ValidateProps2(storeItemStock))
            {
                return;
            }

            $scope.stockName = $scope.storeItemStock.StoreItemObject.Name;
               
            if ($scope.storeItemStock.StoreItemStockId == null || $scope.storeItemStock.StoreItemStockId < 1)
            {
                storeItemStockServices.addStoreItemStock(storeItemStock, $scope.processStoreItemStockCompleted);
            }
            else
            {
                storeItemStockServices.editStoreItemStock(storeItemStock, $scope.processStoreItemStockCompleted);
            }
            
        };

        $scope.ValidateProps = function (storeItemStock)
        {
            if ($scope.productVariant.productVariation.StoreItemVariationId < 1)
            {
                $scope.setError("ERROR: Please select Inventory Variation Property.");
                return false;
            }
            
            if ($scope.productVariant.productVariationValue.StoreItemVariationValueId < 1)
            {
                $scope.setError("ERROR: Please select Inventory Variation Value.");
                return false;
            }

            if (parseInt(storeItemStock.StoreItemId) < 1)
            {
                $scope.setError("ERROR: Please select a Product.");
                return false;
            }

            if (storeItemStock.SKU == undefined || storeItemStock.SKU == null || storeItemStock.SKU.length < 1)
            {
                $scope.setError("ERROR: Please provide the SKU for this Inventory. ");
                return false;
            }

            //if (parseInt(storeItemStock.QuantityInStock) < 1)
            //{
            //    $scope.setError("ERROR: Please providde Quantity in Stock.");
            //    return false;
            //}

            if (parseInt(storeItemStock.Price) < 1)
            {
                $scope.setError("ERROR: Please providde Price.");
                return false;
            }

            if (parseInt( $scope.defaultCurrency.StoreCurrencyId) < 1)
            {

                $scope.setError("ERROR: Please select a Currency.");
                return false;
            }
            return true;
        };
        
        $scope.ValidateProps2 = function (storeItemStock)
        {
            if (parseInt(storeItemStock.StoreItemId) < 1)
            {
                $scope.setError("ERROR: Please select a Product.");
                return false;
            }

            if (storeItemStock.SKU == undefined || storeItemStock.SKU == null || storeItemStock.SKU.length < 1)
            {
                $scope.setError("ERROR: Please provide the SKU for this Inventory. ");
                return false;
            }

            //if (parseInt(storeItemStock.QuantityInStock) < 1) {
            //    $scope.setError("ERROR: Please providde Quantity in Stock.");
            //    return false;
            //}

            if (parseInt(storeItemStock.Price) < 1) {
                $scope.setError("ERROR: Please providde Price.");
                return false;
            }

            if (storeItemStock.StoreCurrencyId < 1)
            {

                $scope.setError("ERROR: Please select a Currency.");
                return false;
            }
            return true;
        };
        
        $scope.processStoreItemStockCompleted = function (data)
        {
            alert(data.Error);

            if (data.Code < 1)
            {
                return;
            }
            else {
                $scope.prices = [];
                $scope.ngTable.fnClearTable();
                $scope.stockId = data.Code;
                $scope.showImages = true;
                $scope.initializePrice();
                $scope.initializeImg();
            }
        };

        $scope.getStoreItemStock = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                $scope.setError("ERROR: Invalid selection! ");
                return;
            }

            $scope.stockId = id;
            storeItemStockServices.getStoreItemStock(id, $scope.getStoreItemStockCompleted);
        };
        
        $scope.getStoreItemStockCompleted = function (data)
        {
            if (data.StoreItemStockId < 1)
            {
                $scope.setError("ERROR: Inventory information could not be retrieved! Please try again ");
                return;
            }
            
        $scope.initializeComponents();
        $scope.storeItemStock = data;
        $scope.stockId = data.StoreItemStockId;
        $scope.storeItemStock.Header = 'Update Inventory information';
        $scope.images = [];
        $scope.initializeImg();

        //Get enrolled Item Prices
        storeItemStockServices.getPrices(data.StoreItemStockId, $scope.getPricesCompleted);

        //Product Variations & Variation Values
        if (data.StoreItemVariationId != null && data.StoreItemVariationId != undefined && data.StoreItemVariationId > 0)
        {
            var variatons = $scope.genericListObject.storeItemVariations.filter(function (variation)
            {
                return variation.StoreItemVariationId === data.StoreItemVariationId;
            });
            if (variatons.length > 0)
            {
                var variatonsValues = $scope.genericListObject.storeItemVariationValues.filter(function (variationValue) {
                    return variationValue.StoreItemVariationValueId === data.StoreItemVariationValueId;
                });

                if (variatonsValues.length > 0)
                {
                    $scope.storeItemStock.StoreItemVariationObject = variatons[0];
                    $scope.storeItemStock.StoreItemVariationValueObject = variatonsValues[0];
                }
            }
        }

        //Product Image Views
        if (data.StockUploadObjects != null && data.StockUploadObjects.length > 0) 
        {
            angular.forEach(data.StockUploadObjects, function(img, i)
            {
                var views = $scope.genericListObject.imageViews.filter(function(view) 
                {
                    return view.ImageViewId === img.ImageViewId;
                });
                if (views.length > 0)
                {
                    var item =
                    {
                        'StockUploadId' : img.StockUploadId,
                        'ImageViewObject': views[0], 
                        'ImageViewId': img.ImageViewId, 
                        'imgObject': '',
                        'ImagePath': img.ImagePath,
                        'ImagePath2': img.ImagePath, 
                        'StoreItemStockId': img.StoreItemStockId
                    }

                    $scope.images.push(item);
                }
            });
        }
                
        //Currencies
        var currencies = $scope.genericListObject.currencies.filter(function (currency)
        {
            return currency.StoreCurrencyId === data.StoreCurrencyId;
        });

        if (currencies.length > 0)
        {
            $scope.defaultCurrency = currencies[0];
        }

        //product Types
        var productTypes = $scope.genericListObject.productTypes.filter(function (type)
        {
            return type.StoreItemTypeId === $scope.storeItemStock.StoreItemObject.StoreItemTypeId;
        });

        if (productTypes.length > 0) {
            $scope.storeItemStock.StoreItemObject.ProductType = productTypes[0];
        }

        //product Brands
        var brands = $scope.genericListObject.productBrands.filter(function (brand)
        {
            return brand.StoreItemBrandId === $scope.storeItemStock.StoreItemObject.StoreItemBrandId;
        });

        if (productTypes.length > 0) {
            $scope.storeItemStock.StoreItemObject.ProductBrand = brands[0];
        }

        //product categories
        var categories = $scope.genericListObject.productCategories.filter(function (category) {
            return category.StoreItemCategoryId === $scope.storeItemStock.StoreItemObject.StoreItemCategoryId;
        });

        if (categories.length > 0) {
            $scope.storeItemStock.StoreItemObject.ProductCategory = categories[0];
        }


        //charts of Account
        var chartsOfAccount = $scope.genericListObject.chartsOfAccount.filter(function (chartOAc)
        {
            return chartOAc.ChartOfAccountId === $scope.storeItemStock.StoreItemObject.ChartOfAccountId;
        });

        if (chartsOfAccount.length > 0)
        {
            $scope.storeItemStock.StoreItemObject.ChartOfAccount = chartsOfAccount[0];
        }
                
            //stock initializations
            //$scope.storeItemStock.UnitOfMeasurementId = data.UnitOfMeasurementId;
            //$scope.storeItemStock.UoMCode = data.UoMCode;
            //$scope.storeItemStock.MinimumQuantity = data.MinimumQuantity;
            $scope.storeItemStock.SKU = data.SKU;
            $scope.storeItemStock.QuantityInStock = data.QuantityInStock;
            $scope.storeItemStock.CostPrice = data.CostPrice;
            $scope.storeItemStock.ReorderLevel = data.ReorderLevel;
            $scope.storeItemStock.ReorderQuantity = data.ReorderQuantity;
            $scope.storeItemStock.ShelfLocation = data.ShelfLocation;
            $scope.storeItemStock.ExpirationDate = data.ExpiryDate;
            $scope.storeItemStock.PublishOnline = data.PublishOnline;
            $scope.storeItemStock.Discontinued = data.Discontinued;
            $scope.storeItemStock.TechSpechs = data.TechSpechs;
            $scope.storeItemStock.Description = data.Description;
            $scope.storeItemStock.StoreItemStockId = data.StoreItemStockId;
            
            //show inventory enrollment view
            $scope.showImages = true;
            $scope.newEdit = true;
            $scope.initializePrice();
            $scope.initializeImg();
        };
        
        $scope.getPricesCompleted = function (data)
        {
            $scope.prices = data;
        };

        $scope.getItemDetails = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                $scope.setError("ERROR: Invalid selection! ");
                return;
            }

            storeItemStockServices.getStoreItemStock(id, $scope.getItemDetailsCompleted);
        };
        
        $scope.getItemDetailsCompleted = function (data)
        {
            if (data.StoreItemStockId < 1)
            {
                $scope.setError("ERROR: Inventory information could not be retrieved! Please try again ");
                return;
            }

            $scope.initializeComponents();
            $scope.storeItemStock = data;
            $scope.stockId = data.StoreItemStockId;
            $scope.storeItemStock.Header = 'Update Inventory information';
            $scope.images = [];
            $scope.prices = [];
            $scope.initializeImg();

            //Get enrolled Item Prices
            storeItemStockServices.getPrices(data.StoreItemStockId, $scope.getPricesCompleted);

            //Product Variations & Variation Values
            if (data.StoreItemVariationId != null && data.StoreItemVariationId != undefined && data.StoreItemVariationId > 0) {
                var variatons = $scope.genericListObject.storeItemVariations.filter(function (variation) {
                    return variation.StoreItemVariationId === data.StoreItemVariationId;
                });
                if (variatons.length > 0) {
                    var variatonsValues = $scope.genericListObject.storeItemVariationValues.filter(function (variationValue) {
                        return variationValue.StoreItemVariationValueId === data.StoreItemVariationValueId;
                    });

                    if (variatonsValues.length > 0) {
                        $scope.storeItemStock.StoreItemVariationObject = variatons[0];
                        $scope.storeItemStock.StoreItemVariationValueObject = variatonsValues[0];
                    }
                }
            }

            //Product Image Views
            if (data.StockUploadObjects != null && data.StockUploadObjects.length > 0) {
                angular.forEach(data.StockUploadObjects, function (img, i) {
                    var views = $scope.genericListObject.imageViews.filter(function (view) {
                        return view.ImageViewId === img.ImageViewId;
                    });
                    if (views.length > 0) {
                        var item =
                        {
                            'StockUploadId': img.StockUploadId,
                            'ImageViewObject': views[0],
                            'ImageViewId': img.ImageViewId,
                            'imgObject': '',
                            'ImagePath': img.ImagePath,
                            'ImagePath2': img.ImagePath,
                            'StoreItemStockId': img.StoreItemStockId
                        }

                        $scope.images.push(item);
                    }
                });
            }

            //Currencies
            var currencies = $scope.genericListObject.currencies.filter(function (currency) {
                return currency.StoreCurrencyId === data.StoreCurrencyId;
            });

            if (currencies.length > 0) {
                $scope.defaultCurrency = currencies[0];
            }

            //product Types
            var productTypes = $scope.genericListObject.productTypes.filter(function (type) {
                return type.StoreItemTypeId === $scope.storeItemStock.StoreItemObject.StoreItemTypeId;
            });

            if (productTypes.length > 0) {
                $scope.storeItemStock.StoreItemObject.ProductType = productTypes[0];
            }

            //product Brands
            var brands = $scope.genericListObject.productBrands.filter(function (brand) {
                return brand.StoreItemBrandId === $scope.storeItemStock.StoreItemObject.StoreItemBrandId;
            });

            if (productTypes.length > 0) {
                $scope.storeItemStock.StoreItemObject.ProductBrand = brands[0];
            }

            //product categories
            var categories = $scope.genericListObject.productCategories.filter(function (category) {
                return category.StoreItemCategoryId === $scope.storeItemStock.StoreItemObject.StoreItemCategoryId;
            });

            if (categories.length > 0) {
                $scope.storeItemStock.StoreItemObject.ProductCategory = categories[0];
            }


            //charts of Account
            var chartsOfAccount = $scope.genericListObject.chartsOfAccount.filter(function (chartOAc) {
                return chartOAc.ChartOfAccountId === $scope.storeItemStock.StoreItemObject.ChartOfAccountId;
            });

            if (chartsOfAccount.length > 0) {
                $scope.storeItemStock.StoreItemObject.ChartOfAccount = chartsOfAccount[0];
            }

            $scope.storeItemStock.SKU = data.SKU;
            $scope.storeItemStock.QuantityInStock = data.QuantityInStock;
            $scope.storeItemStock.CostPrice = data.CostPrice;
            $scope.storeItemStock.ReorderLevel = data.ReorderLevel;
            $scope.storeItemStock.ReorderQuantity = data.ReorderQuantity;
            $scope.storeItemStock.ShelfLocation = data.ShelfLocation;
            $scope.storeItemStock.ExpirationDate = data.ExpiryDate;
            $scope.storeItemStock.TechSpechs = data.TechSpechs;
            $scope.storeItemStock.Description = data.Description;
            $scope.storeItemStock.StoreItemStockId = data.StoreItemStockId;
            $scope.stockId = data.StoreItemStockId;

            //show inventory details view
            $scope.details = true;
        };
        
        $scope.deleteStoreItemStock = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Inventory information will be deleted permanently. Continue?"))
                {
                    return;
                }
                storeItemStockServices.deleteStoreItemStock(id, $scope.deleteStoreItemStockCompleted);
            }
            else
            {
                $scope.setError('Invalid selection.');
            }
        };

        $scope.deleteStoreItemStockCompleted = function (data)
        {
            if (data.Code < 1)
            {
                $scope.setError(data.Error);

            }
            else
            {
                $scope.setSuccessFeedback(data.Error);
                $scope.ngTable.fnClearTable();
            }
        };
        
        function setUIBusy() {
            angular.element('.sowBusy').fadeIn('fast');
        };

        function stopUIBusy() {
            angular.element('.sowBusy').fadeOut('fast');
        };

       //Bulk Upload
        $scope.ProcessDocument = function (e)
        {
            var bulkDocument = (e.srcElement || e.target).files[0];

            if (bulkDocument == null)
            {
                //$scope.setError('Please select bulk Upload file.');
                return;
            }

            if (bulkDocument.size < 1)
            {
                $scope.setError('Please select a valid file.');
                return;
            }
            
            if ($scope.outlet === undefined || $scope.outlet === null || $scope.outlet.StoreOutletId < 1)
            {
                alert('Please select an Outlet');
                return;
            }

            setUIBusy();
            $upload.upload({
                url: "/StoreItemStock/ProductUpload?outletId=" + $scope.outlet.StoreOutletId,
                method: "POST",
                data: { file: bulkDocument }
            })
           .progress(function (evt)
           {
               $scope.progress = parseInt(100.0 * evt.loaded / evt.total);
           })
           .success(function (data)
           {
               stopUIBusy();
               if (data === null < 1)
               {
                   $scope.setError('File could not be processed. Please try again later.');
               }
               else {
                   var errors = [];
                   var successes = [];
                   angular.forEach(data, function(v, i) 
                   {
                       if (v.Code < 1)
                       {
                           errors.push(v);
                       } 
                       else
                       {
                           successes.push(v); 
                       }
                   });

                   if (errors.length > 0 && successes.length < 1) 
                   {
                       $scope.uploadError = "The following items could not be processed due to the stated reasons.";
                       ngDialog.open({
                           template: '/ng-shopkeeper/Views/Store/ProductStocks/ProductUploadFeedback.html',
                           className: 'ngdialog-theme-flat',
                           scope: $scope
                       });
                       return;
                   }

                   if (errors.length > 0 && successes.length > 0)
                   {
                       $scope.uploadError = successes.length + " Items were successfully processed, While the items displayed below could not be processed due to the stated reasons.";
                       $scope.bulkError = true;
                       $scope.errors = errors;
                       ngDialog.open({
                           template: '/ng-shopkeeper/Views/Store/ProductStocks/ProductUploadFeedback.html',
                           className: 'ngdialog-theme-flat',
                           scope: $scope
                       });
                   }
                   else
                   {
                       $scope.setSuccessFeedback(successes.length + " Items were successfully processed.");

                       $scope.ngTable.fnClearTable();
                       $scope.bulkUpload = false;
                       $scope.details = false;
                       $scope.showImages = true;
                       $scope.newEdit = true;
                   }
               }
           });
        };
        
       
        //Inventory Images
        $scope.validateImage = function ()
        {
            if (($scope.img.ImageObj === undefined || $scope.img.ImageObj == null || $scope.img.ImageObj.size < 1) && ($scope.img.ImagePath == null || $scope.img.ImagePath.length < 1)) 
            {
                alert('Please attach at Image.');
                return false;
            }
            
            if ($scope.img.ImageObj !== undefined && $scope.img.ImageObj !== null && $scope.img.ImageObj.size > 4096000)
            {
                alert('The Image size should not be larger than 4MB.');
                return false;
            }

            if ($scope.img.ImageViewObject.ImageViewId < 1) 
            {
                alert('Please select an Image View.');
                return false;
            }

            var duplicates = $scope.images.filter(function (item, i)
            {
                return (item.ImageViewId === $scope.img.ImageViewObject.ImageViewId);
            });
            
            if (duplicates > 0) 
            {
                alert('Please select a different Image View.');
                return false;
            }
            return true;
        };

        $scope.getImage = function (viewId)
        {
            if (viewId < 1)
            {
                alert('Invalid selection');
                return;
            }

            var views = $scope.images.filter(function (item, i)
            {
                return (item.ImageViewId === viewId);
            });
            
            if (views.length < 1) 
            {
                alert('Invalid selection');
                return;
            }

            $scope.img = views[0];

        };

        $scope.processStockImage = function ()
        {

            if (!$scope.validateImage())
            {
                return;
            }

            var obj =
                   {
                       'ImageViewObject': $scope.img.ImageViewObject,
                       'ImageViewId': $scope.img.ImageViewObject.ImageViewId,
                       'ImagePath': $scope.img.ImagePath,
                       'ImagePath2': $scope.img.ImagePath2,
                       'StoreItemStockId': $scope.stockId,
                       'StockUploadId': $scope.img.StockUploadId,
                       'imgObject': $scope.img.ImageObj
                   };

            var url = '';
            if ($scope.img.StockUploadId == null || $scope.img.StockUploadId == undefined || $scope.img.StockUploadId < 1)
            {
                url = "/StoreItemStock/SaveStockImage?imageViewId=" + obj.ImageViewId + "&stockItemId=" + obj.StoreItemStockId;
            }
            else
            {
                url = "/StoreItemStock/UpdateStockImage?id=" + obj.StockUploadId + "&imageViewId=" + obj.ImageViewId + "&oldPath=" + obj.ImagePath + "&stockItemId=" + obj.StoreItemStockId;
            }
            $rootScope.busy = true;
            $upload.upload({
                url: url,
                method: "POST",
                data: {file: obj.imgObject }
            })
           .success(function (data)
           {
               $rootScope.busy = false;
               
               if (data.Code == null || data.Code < 1)
               {
                   alert('Inventory Image could not be processed. Please try again later.');
                   return false;
               }
               else
               {
                   $scope.setSuccessFeedback(data.Error);
                   if (obj.StockUploadId == null || obj.StockUploadId == undefined || obj.StockUploadId < 1)
                   {
                       obj.StockUploadId = data.Code;
                       obj.ImagePath = data.FilePath;
                       obj.ImagePath2 = data.FilePath;
                       $scope.images.push(obj);
                   }
                   else
                   {
                       var images = $scope.images.filter(function (p)
                       {
                           return p.StockUploadId === obj.StockUploadId;
                       });

                       if (images != null && images.length > 0)
                       {
                           images[0].ImageViewId = obj.ImageViewId;
                           if ($scope.img.ImageObj !== undefined && $scope.img.ImageObj !== null && $scope.img.ImageObj.size > 0)
                           {
                               images[0].ImagePath = data.FilePath;
                               images[0].ImagePath2 = data.FilePath;
                           }
                       }
                   }

                   $scope.initializeImg();
                   angular.element('#imgView').val('');
                   return true;
               }
           });
        };

        $scope.removeImage = function (stockUpload)
        {
            if (stockUpload == null || stockUpload.StockUploadId < 1)
            {
                $scope.setError('Invalid selection');
                return;
            }

            if (!confirm("This Image information will be deleted permanently. Continue?"))
            {
                return;
            }

            $scope.imageToDel = stockUpload;
            storeItemStockServices.deleteStockUpload(stockUpload, $scope.removeImageCompleted);

        };
        
        $scope.removeImageCompleted = function (response)
        {
            alert(response.Error);
            if (response.Code < 1)
            {
                return;
            }

            angular.forEach($scope.images, function (img, i)
            {
                if (img.StockUploadId === $scope.imageToDel.StockUploadId)
                {
                    $scope.images.splice(i, 1);
                }
            });
        };


        //Item Price Management
        $scope.initializePrice = function ()
        {
            $scope.itemPrice = {'Price' : '', 'ItemPriceId' : '', 'MinimumQuantity' : '', 'UoMId' : '', 'unitofMeasurement' : {'UnitOfMeasurementId': '','UoMCode': ' -- Select Unit Of Measurement --'}, 'Header' : 'Add Price' };
           
        };
       
        $scope.preparePriceTemplate = function ()
        {
            $scope.initializePrice();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/ProductStocks/StockPrice.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };

        $scope.processItemPrice = function () {
           
            var itemPrice =
            {
                'Price': $scope.itemPrice.Price, 
                'ItemPriceId': $scope.itemPrice.ItemPriceId, 
                'MinimumQuantity': $scope.itemPrice.MinimumQuantity, 
                'UoMId': $scope.itemPrice.unitofMeasurement.UnitOfMeasurementId, 
                'StoreItemStockId': $scope.stockId,
                'UoMCode' : $scope.itemPrice.unitofMeasurement.UoMCode
            };

            $scope.tempItemPrice = itemPrice;

            if (!$scope.validatateItem(itemPrice))
            {
                return;
            }

            if ($scope.itemPrice.ItemPriceId < 1)
            {
                storeItemStockServices.addItemPrice(itemPrice, $scope.processItemPriceCompleted);
            }
            else
            {
                storeItemStockServices.editItemPrice(itemPrice, $scope.processItemPriceCompleted);
            }
            
        };

        $scope.validatateItem = function (itemPrice)
        {
            if (itemPrice.StoreItemStockId == undefined || itemPrice.StoreItemStockId < 1) {
                $scope.setError("ERROR: A fatal error was encountered. Please refresh the page and try again.");
                return false;
            }

            if (itemPrice.UoMId == undefined || itemPrice.UoMId < 1) {
                $scope.setError("ERROR: Please select Unit of Measurement. ");
                return false;
            }

            if (parseInt(itemPrice.Price) < 1)
            {
                $scope.setError("ERROR: Please Provide Price. ");
                return false;
            }

            if (parseInt(itemPrice.MinimumQuantity) < 1)
            {
                $scope.setError("ERROR: Please Provide Quantity. ");
                return false;
            }
            return true;
        };

        $scope.processItemPriceCompleted = function (data)
        {
            if (data.Code < 1)
            {
                $scope.setError(data.Error);
                return;
            }
            else {
                $scope.setSuccessFeedback(data.Error);
                $scope.tempItemPrice.unitofMeasurement = $scope.itemPrice.unitofMeasurement;
                if ($scope.tempItemPrice.ItemPriceId == null || $scope.tempItemPrice.ItemPriceId < 1)
                {
                    $scope.tempItemPrice.ItemPriceId = data.Code;
                    $scope.prices.push($scope.tempItemPrice);
                }
                else
                {
                    var items = $scope.prices.filter(function (p)
                    {
                        return p.ItemPriceId === data.ItemPriceId;
                    });

                    if (items != null && items.length > 0) 
                    {
                        items[0].Price = $scope.tempItemPrice.Price.trim();
                        items[0].MinimumQuantity = $scope.tempItemPrice.MinimumQuantity;
                        items[0].UoMId = $scope.tempItemPrice.UoMId;
                        items[0].StoreItemStockId = $scope.tempItemPrice.StoreItemStockId;
                        items[0].UoMCode = $scope.tempItemPrice.UoMCode;
                        items[0].unitofMeasurement = $scope.itemPrice.unitofMeasurement;
                    }
                    
                }

                $scope.initializePrice();
            }
        };
        
        $scope.getPrice = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                $scope.setError("ERROR: Invalid selection! ");
                return;
            }

            var items = $scope.prices.filter(function (p)
            {
                return p.ItemPriceId === id;
            });

            if (items != null && items.length > 0)
            {
                var uoms = $scope.genericListObject.unitsofMeasurement.filter(function (uom)
                {
                    return uom.UnitOfMeasurementId === items[0].UoMId;
                });

                if (uoms.length > 0)
                {
                    items[0].unitofMeasurement = uoms[0];
                }
                $scope.itemPrice = items[0];

            }
            else
            {
                $scope.setError("ERROR: Price information could not be retrieved. Please try again later.");
                return;
            }
           
        };
        
        $scope.deletePrice = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This  Price information will be deleted permanently. Continue?"))
                {
                    return;
                }
                storeItemStockServices.deleteItemPrice(id, $scope.deleteItemPriceCompleted);
            }
            else
            {
                $scope.setError('Invalid selection.');
            }
        };

        $scope.deleteItemPriceCompleted = function (data)
        {
            if (data.Code < 1) {
                $scope.setError(data.Error);
                return;
            }
            else
            {
                $scope.setSuccessFeedback(data.Error);
                angular.forEach($scope.prices, function (p, i)
                {
                    if (p.ItemPriceId === data.Code)
                    {
                        $scope.prices.splice(i, 1);
                    }
                });
            }
        };

    }]);
});
