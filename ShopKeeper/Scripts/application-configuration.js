
"use strict";

define(['angularAMD', 'angular-route', 'ui-bootstrap', 'angular-sanitize', 'ui.select', 'blockUI', 'ngDialog', 'ngLocale', 'ngAnimate', 'ngIdle', 'ngStorage', 'ngCookies', 'ui.utils.masks', 'angucomplete'], function (angularAMD)
{
    var app = angular.module("mainModule", ['ngRoute', 'blockUI', 'ngSanitize', 'ui.select', 'ui.bootstrap', 'ngDialog', 'ngLocale', 'ngAnimate', 'ngIdle', 'ngStorage', 'ngCookies', 'ui.utils.masks', 'angucomplete']);
    
    app.config([
        'ngDialogProvider', function (ngDialogProvider)
        {
            ngDialogProvider.setDefaults({
                className: 'ngdialog-theme-default',
                showClose: true,
                closeByDocument: false,
                closeByEscape: false
            });
        }
    ])
    .config(function (IdleProvider, KeepaliveProvider) //Configure user session manager
    {
        var ticketIdle = 15 * 60;
        IdleProvider.idle(ticketIdle);
        IdleProvider.timeout(15);
        KeepaliveProvider.interval(300);
        KeepaliveProvider.http('/Purchaseorder/RefreshSession');
    });
    
    app.filter('propsFilter', function () {
        return function (items, props) {
            var out = [];

            if (angular.isArray(items)) {
                items.forEach(function (item) {
                    var itemMatches = false;

                    var keys = Object.keys(props);
                    for (var i = 0; i < keys.length; i++) {
                        var prop = keys[i];
                        var text = props[prop].toLowerCase();
                        if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                            itemMatches = true;
                            break;
                        }
                    }

                    if (itemMatches) {
                        out.push(item);
                    }
                });
            } else {
                // Let the output be the input untouched
                out = items;
            }

            return out;
        };
    });
    
    //app.config(function ($httpProvider) {
    //    $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
    //    $httpProvider.defaults.withCredentials = true;
    //});
    
    app.config(function (blockUIConfigProvider)
    {

        // Change the default overlay message
        blockUIConfigProvider.message("Processing...");
        // Change the default delay to 100ms before the blocking is visible
        blockUIConfigProvider.delay(1);
        // Disable automatically blocking of the user interface
        blockUIConfigProvider.autoBlock(false);

    });

    app.config(['$routeProvider', function ($routeProvider)
    {
        $routeProvider
        .when("welcome", angularAMD.route({

            templateUrl: function (rp)
            { return 'ng-shopkeeper/Views/Store/Welcome/Welcome.html'; },

            resolve: {
            load: ['$q', '$rootScope', '$location', function ($q, $rootScope, $location)
            {
                var controller = '/ng-shopkeeper/Controllers/Store/Welcome/WelcomeController';
                var deferred = $q.defer();
                require([controller], function ()
                {
                    $rootScope.$apply(function ()
                    {
                        deferred.resolve();
                    });
                });
                return deferred.promise;
            }]
        }
        }))
        //.when("dashboard_admin", angularAMD.route({

        //    templateUrl: function (rp) { return 'ng-shopkeeper/Views/Store/Dashboard/Dashboard.html'; },
        //    controllerUrl: "ng-shopkeeper/Controllers/Store/Dashboard/DashboardController"
        //}))
        .when("signIn", angularAMD.route({

            templateUrl: function (rp) { return 'ng-shopkeeper/Views/Store/signIn.html'; }
            //controllerUrl: "ng-shopkeeper/Views/Store/dashboardController"

        }))
            .when("/:section/:tree", angularAMD.route({
                templateUrl: function (rp) {
                   
                    return 'ng-shopkeeper/Views/' + rp.section + '/' + rp.tree + '.html';
                },

                resolve: {
                    load: ['$q', '$rootScope', '$location', function ($q, $rootScope, $location)
                    {
                        var path = $location.path();
                        var parsePath = path.split("/");
                        var controllerPath = parsePath[2];
                        var section = parsePath[0];
                        
                        var controller = '/ng-shopkeeper/Controllers/' + section + '/' + controllerPath + "Controller.js";

                        var deferred = $q.defer();
                        require([controller], function ()
                        {
                            $rootScope.$apply(function ()
                            {
                                deferred.resolve();
                            });
                        });
                        return deferred.promise;
                    }]
                }
            }))
            .when("/:section/:tree/:id", angularAMD.route({
                
                templateUrl: function (rp)
                {
                     return 'ng-shopkeeper/Views/' + rp.section + '/' + rp.tree + '/' + rp.tree + '.html';

                },

                resolve: {
                    load: ['$q', '$rootScope', '$location', function ($q, $rootScope, $location)
                    {
                        var path = $location.path();
                        var parsePath = path.split("/");
                        var controllerPath = parsePath[2];
                        var section = parsePath[1];
                        
                        var controller = '/ng-shopkeeper/Controllers/' + section + '/' + controllerPath + "/" + controllerPath + "Controller.js";

                        var deferred = $q.defer();
                        require([controller], function ()
                        {
                            $rootScope.$apply(function ()
                            {
                                deferred.resolve();
                            });
                        });
                        return deferred.promise;
                    }]
                }
            }))
      
            .when("/:section/:tree/:id/:nm/:bn/:cn/:tn/:ds/:bid/:sid/:scid", angularAMD.route({
                templateUrl: function (rp) {
                    return 'ng-shopkeeper/Views/' + rp.section + '/' + rp.tree + '/' + rp.tree + '.html';

                },

                resolve:
                    {
                    load: ['$q', '$rootScope', '$location', function ($q, $rootScope, $location)
                    {
                        var path = $location.path();
                        var parsePath = path.split("/");
                        var controllerPath = parsePath[2];
                        var section = parsePath[1];

                        var controller = '/ng-shopkeeper/Controllers/' + section + '/' + controllerPath + "/" + controllerPath + "Controller.js";

                        var deferred = $q.defer();
                        require([controller], function () {
                            $rootScope.$apply(function () {
                                deferred.resolve();
                            });
                        });
                        return deferred.promise;
                    }]
                }
            }))
            .otherwise({ redirectTo: 'welcome' });

    }]);
    

    //defaultController.$inject = ['$scope', '$rootScope', '$http', '$location', 'blockUI'];
    app.controller("defaultController", ['ngDialog', '$scope', '$rootScope', '$http', '$location', '$timeout', 'blockUI', '$cookies', '$localStorage', 'Idle', 'Keepalive', '$modal', '$window',
        function (ngDialog, $scope, $rootScope, $http, $location, $timeout, blockUI, $cookies, $localStorage, Idle, Keepalive, $modal, $window)
        {
            /* Get the hosting environment url */
            var homeroot = $location.protocol() + '://' + $location.host();

            var port = $location.port();

            if (port > 0) {
                homeroot += ':' + port;
            }

            $rootScope.homeroot = homeroot;
            /* Get the hosting environment url Ends */


            function setUIBusy() {
                $rootScope.busy = true;
            };

            function stopUIBusy() {
                $rootScope.busy = false;
            };

            $scope.$on('$routeChangeStart', function (scope, next, current)
            {
                setUIBusy();
            });

            $scope.$on('$routeChangeSuccess', function (scope, next, current)
            {
                stopUIBusy();
                setTimeout(function ()
                {
                    angular.element('.nv-header').fadeIn();
                    angular.element('.nv-body').fadeIn();
                    angular.element('.nv-footer').fadeIn();

                }, 500);
            });

            /*        DOM Manipulation   */
            $rootScope.closeAccess = function ()
            {
                angular.element(".ui-mask").hide();
                angular.element('#authsection').hide();
            }
            $rootScope.showAccess = function () {
                $rootScope.closeCart();
                angular.element(".ui-mask").fadeIn();
                angular.element("#authsection").fadeIn();

            }
            
            $rootScope.showCart = function() 
            {
                $rootScope.closeAccess();
                angular.element(".ui-mask").fadeIn();
                angular.element("#cart").fadeIn();
            }

            $rootScope.closeCart = function ()
            {
                angular.element(".ui-mask").hide();
                angular.element('#cart').hide();
            }
            
            /*   DOM Manipulation Ends         */


            $rootScope.getItemDetails = function (item)
            {
                if (item.StoreItemStockId < 1)
                {
                    return;
                }
                $rootScope.closeCart();
                $location.path('/Store/ItemInfo/' + item.StoreItemStockId + '/' + item.Name + '/' + item.StoreItemBrandName + '/' + item.StoreItemCategoryName + '/' + item.StoreItemTypeName + '/' + item.Description + '/' + item.StoreItemBrandId + '/' + item.StoreItemTypeId + '/' + item.StoreItemCategoryId); 
            };

            $scope.priceLookUp = function ()
               {
                   $scope.priceList = [];
                   ngDialog.open({
                       template: '/ng-shopkeeper/Views/Store/Sales/PriceLookUp.html',
                       className: 'ngdialog-theme-flat',
                       scope: $scope
                   });

                  // angular.element('.twitter-typeahead:first').find('.twitter-typeahead:first-child').remove();
               };
        
            $scope.search = function ()
            {
                if ($rootScope.search == null || $rootScope.search.length < 1)
                {
                    return;
                }
                $scope.AjaxGet("/Account/GetLinks", $scope.getDefaultsComplete);
            };

            $scope.searchCompleted = function ()
            {
                if ($rootScope.search == null || $rootScope.search.length < 1)
                {
                    return;
                }
                
            };
            
            // For session Management
            function closeModals() {
                if ($rootScope.warning) {
                    $rootScope.warning.close();
                    $rootScope.warning = null;
                }

                if ($rootScope.timedout) {
                    $rootScope.timedout.close();
                    $rootScope.timedout = null;
                }
            }

            $rootScope.$on('IdleStart', function () {
                closeModals();

                $rootScope.warning = $modal.open({
                    templateUrl: 'warning-dialog.html',
                    windowClass: 'modal-danger'
                });
            });

            $rootScope.$on('IdleEnd', function () {
                closeModals();
            });

            $rootScope.$on('IdleTimeout', function ()
            {
                closeModals();
                $rootScope.signOut();
            });
         
            $scope.initializeDefaultController = function ()
            {
                $rootScope.model = { 'UserName': '', 'Password': '' };
                $scope.ngfeedback = false;
                $scope.getSignedOnUser();
            };

            $rootScope.setAuth = function ()
            {
                $rootScope.ShoppingCart = null;
                $rootScope.cartInfo = 'Your cart is empty. Select items from catalogues to start shopping.';
                $rootScope.cartItemsCount = 0;
                $rootScope.model = {'UserName': '', 'Password': '' };
                $rootScope.signupObj = {'Email': '', 'Password': '', 'LastName': '', 'OtherNames': ''};
                $rootScope.getViewTrail();

                //Get the store items and their links from the Items Categories.
                $rootScope.busy = true;
                $scope.AjaxGet("/Account/GetLinks", $rootScope.getDefaultsComplete);
            };

            $rootScope.getDefaultsComplete = function (response)
            {
                $rootScope.busy = false;
                if (response == null)
                {
                    $location.path('/Home/Index');
                }
                else
                {
                    $rootScope.categories = response.ItemCategories;
                    $rootScope.itemTypes = response.ItemTypes;
                    $rootScope.itemBrands = response.ItemBrands;
                    $rootScope.items = response.Items;
                    $rootScope.store = response.StoreInfo;
                }

                var originalGeoInfo = $rootScope.getCookie('geo_info');
                
                if (originalGeoInfo === undefined || originalGeoInfo == null || originalGeoInfo.length < 1)
                {
                    $scope.AjaxGet("https://freegeoip.net/json", $rootScope.getIpCompleted);
                }
                else
                {
                    $rootScope.geoInfo = originalGeoInfo;
                    console.log('Retrieved Cookie Info : ' + originalGeoInfo);
                    var keys = Object.keys(originalGeoInfo);

                    angular.forEach(keys, function (t, k)
                    {
                        console.log(t + ' : ' + originalGeoInfo[t]);
                    });
                    
                    $scope.AjaxGet("/ShoppingCart/GetClientCartByClientIp?ip=" + $rootScope.geoInfo.ip, $rootScope.getClientCartByIpCompleted);
                }
            };

            $rootScope.getIpCompleted = function (response)
            {
                if (response == null || response.ip.length < 1)
                {
                    $scope.AjaxGet("/ShoppingCart/GetClientCartByIp", $rootScope.getClientCartByIpCompleted);
                    return;
                }
                else
                {
                    $rootScope.geoInfo = response;
                    $rootScope.setCookie('geo_info', response, 90);
                    $scope.AjaxGet("/ShoppingCart/GetClientCartByClientIp?ip=" + response.ip, $rootScope.getClientCartByIpCompleted);
                }

            };

            $rootScope.getClientCartByIpCompleted = function (cart)
            {
                $rootScope.getCountries();
                if (cart == null || cart.ShoppingCartId < 1 || cart.ShopingCartItemObjects === null)
                {
                    $rootScope.ShoppingCart = null;
                }
                else
                {
                    $rootScope.ShoppingCart = cart;
                    var cartItemsCount = 0;
                    var cartAmount = 0;
                    angular.forEach(cart.ShopingCartItemObjects, function (t, k)
                    {
                        cartItemsCount += t.QuantityOrdered;
                        cartAmount += t.UnitPrice*t.QuantityOrdered;
                    });

                    $rootScope.cartTotalAmount = cartAmount;
                    $rootScope.cartItemsCount = cartItemsCount;
                    $rootScope.cartInfo = 'Your cart!';
                }
               
            };
            
            $rootScope.getCountries = function ()
            {
                $scope.AjaxGet("/ShoppingCart/GetCountries", $rootScope.getCountriesCompleted);
            };

            $rootScope.getCountriesCompleted = function (response)
            {
                if (response == null || response.length < 1)
                {
                    return;
                }

                $rootScope.countries = response;
                if ($rootScope.geoInfo !== undefined && $rootScope.geoInfo !== null && $rootScope.geoInfo.country_name.length > 0)
                {
                    var clientCountries = response.filter(function (country)
                    {
                        return country.Name.toLowerCase().trim() === $rootScope.geoInfo.country_name.toLowerCase().trim();
                    });

                    if (clientCountries.length > 0)
                    {
                        $rootScope.clientCountry = clientCountries[0];
                    }
                }
            };
            
            $rootScope.getStates = function (country)
            {
                if (country.StoreCountryId < 1)
                {
                    return;
                }

                $rootScope.processing = true;
                $scope.AjaxGet("/ShoppingCart/GetCountryStates?countryId=" + country.StoreCountryId, $rootScope.getStatesCompleted);
            };

            $rootScope.getStatesCompleted = function (states)
            {
                $rootScope.processing = false;
                if (states == null || states.length < 1)
                {
                    return;
                }

                $rootScope.states = states;
            };

            $scope.getCities = function (state)
            {
                if (state.StoreStateId < 1)
                {
                    return;
                }
                $rootScope.processing = true;
                $scope.AjaxGet("/ShoppingCart/GetStateCities?stateId=" + state.StoreStateId, $rootScope.getCitiesCompleted);
            };

            $rootScope.getCitiesCompleted = function (cities)
            {
                $rootScope.processing = false;
                if (cities == null || cities.length < 1)
                {
                    return;
                }

                $rootScope.cities = cities;
            };
          
            $rootScope.getCookie = function (name)
            {
                var re = new RegExp(name + "=([^;]+)");
                var value = re.exec(document.cookie);
                return (value != null) ? unescape(value[1]) : null;
            }

            $rootScope.setCookie = function(variable, value, expDays)
            {
                var d = new Date();
                d = new Date(d.getTime() + expDays);
                document.cookie = variable + '=' + value + '; expires=' + d.toGMTString() + '; Path=/;';
            }

            $rootScope.deleteCookie = function (name)
            {
                document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT; Path=/;';
            };
            
            $scope.initializeApplicationComplete = function (response)
            {
                $rootScope.MenuItems = response.MenuItems;
                $rootScope.displayContent = true;
                $rootScope.IsloggedIn = true;
            };

            //todo: To Load Menu items and their Children dynamically after authentication
            $scope.initializeApplication = function ()
            {
                blockUI.start();
                $scope.AjaxGet("/api/main/InitializeApplication");
                blockUI.stop();
            };
            
            //Get Store settings Callback
            var timeOut = 0;           
            
            $scope.login = function ()
            {
                ngDialog.open({
                    template: '/ng-shopkeeper/Views/Store/Welcome/signin.html',
                    className: 'ngdialog-theme-flat',
                    scope: $scope
                });
            };

            //Authenticate User
            $rootScope.signIn = function (auth)
            {
                if (auth == null)
                {
                    alert('Action failed');
                    return;
                }
                if (auth.UserName == null || auth.UserName.length < 1)
                {
                    alert('Please provide your Email');
                    return;
                }

                if (auth.Password == null || auth.Password.length < 1)
                {
                    alert('Please provide your Password');
                    return;
                }

                var model = { 'UserName': auth.UserName, 'Password': auth.Password };
                setUIBusy();
                $scope.AjaxPostAuth({ model: model }, "/Account/NgSignIn", $rootScope.signinComplete);
            };

            $rootScope.signinComplete = function (response)
            {
                stopUIBusy();
                if (response.Code < 1)
                {
                    alert(response.UserName);
                    return;
                }

                $rootScope.userName = response.UserName;
                $rootScope.isAuthenticated = true;
                $rootScope.user = response.UserProfile;
                $rootScope.store = response.StoreInfo;
                $rootScope.res = response;

                //$rootScope.getCountries();
                $rootScope.closeAccess();

                var isCustomer = response.Roles.filter(function (r)
                {
                    return r === "Customer";
                });
                
                if (isCustomer.length > 0)
                {
                    return;
                }
                else
                {
                    $rootScope.isCashier = false;
                    $rootScope.isAdmin = false;
                    $rootScope.isPurchaser = false;
                    $rootScope.isMarketer = false;

                    angular.forEach(response.Roles,function (r)
                    {
                        if (r === 'Admin' || r === 'Super_Admin')
                        {
                            $rootScope.isAdmin = true;
                        }

                        if (r === 'Cashier')
                        {
                            $rootScope.isCashier = true;
                        }
                       
                        if (r === 'Purchasing')
                        {
                            $rootScope.isPurchaser = true;
                        }
                        if (r === 'Marketer')
                        {
                            $rootScope.isMarketer = true;
                        }
                    });

                    //sort parent menu items
                    response.UserLinks.sort(function (a, b)
                    {
                        return (a['MenuOrder'] > b['MenuOrder']) ? 1 : ((a['MenuOrder'] < b['MenuOrder']) ? -1 : 0);
                    });

                    //sort child menu items
                    angular.forEach(response.UserLinks, function (r)
                    {
                        if (r.ChildMenuObjects.length > 0)
                        {
                            r.ChildMenuObjects.sort(function (a, b)
                            {
                                return (a['ChildMenuOrder'] > b['ChildMenuOrder']) ? 1 : ((a['ChildMenuOrder'] < b['ChildMenuOrder']) ? -1 : 0);
                            });
                        }

                    });

                    $rootScope.links = response.UserLinks;
                    angular.element('.autoFade').fadeIn('fast');
                    angular.element('.nv-side').fadeIn('fast');
                    $window.location.href = $rootScope.homeroot + '/ngy.html#/Store/Dashboard/Dashboard';
                    //Idle.watch();
                    
                }
            };

            $rootScope.logOut = function ()
            {
                $scope.AjaxPost("/Account/NgSignOut", $rootScope.logOutCompleted);
            };

            $rootScope.logOutCompleted = function ()
            {
                $rootScope.isAuthenticated = false;
                $scope.userName = '';
                $rootScope.closeAccess();
                $location.path('/Store/Start/Start');
            };
            
            $rootScope.signOut = function ()
            {
                $scope.AjaxPost("/Account/NgSignOut", $rootScope.signOutCompleted);
            };

            $rootScope.signOutCompleted = function ()
            {
                $rootScope.store = null;
                $rootScope.user = null;
                $rootScope.isAuthenticated = false;
                $location.path('/Store/Start/Start');
            };

            $rootScope.customerLogOut = function ()
            {
                $scope.AjaxPost("/Account/NgSignOut", $rootScope.customerLogOutCompleted);
            };

            $rootScope.customerLogOutCompleted = function ()
            {
                $rootScope.store = null;
                $rootScope.user = null;
                $rootScope.isAuthenticated = false;
                $location.path("/Welcome/Welcome");

            };
            
            //Admin authentication

            $scope.verifyLogin = function ()
            {
                $scope.AjaxGet("/Account/VerifyLogin", $scope.verifyLoginComplete);
            };
            
            $scope.verifyLoginComplete = function (response)
            {
                var redir = '/Account/AdminLogOff';
                if (r.Code < 1)
                {
                    $('#signin').removeClass('pleasewait');
                    alert('Your session has timed out or access denied!');
                    window.href = redir;
                }
             
                if (response.IsAuthenicated === false)
                {
                    $rootScope.isAuthenticated = false;
                    $scope.error = 'Authentication Failed: Invalid Credentials';
                    $rootScope.user = {};
                    window.href = redir;
                }
                else
                {
                    var roles = response.Roles.filter(function (r, i)
                    {
                        return r === "Super_Admin";
                    });

                    if (roles.length < 1)
                    {
                        alert('Access denied!');
                        window.href = redir;
                    }

                    $scope.ngfeedback = false;
                    $rootScope.isAuthenticated = true;
                    $rootScope.user = response;
                }
            };
            
            $rootScope.getCurrentUser = function ()
            {
                return setTimeout(function ()
                {
                    $http({ method: 'GET', url: "/Account/GetSignedOnUser" }).success(function (response) {
                        alert(response.IsAuthenicated);
                        return response;
                    });
                }, 1);
            };
            
            $scope.getSignedOnUser = function ()
            {
                $scope.AjaxGet("/Account/GetSignedOnUser", $scope.getSignedOnUserCompleted);
            };
            
            $scope.authenicateUserComplete = function (response)
            {

                if (r.Code < 1)
                {
                    $('#signin').removeClass('pleasewait');
                    alert(r.UserName);
                    return;
                } else {
                    var redir = '/ngy.html#Store/Dashboard/Dashboard';
                    window.location = redir.replace("/Account", "");
                }


                if (response.IsAuthenicated === false)
                {
                    $rootScope.isAuthenticated = false;
                    $scope.error = 'Authentication Failed: Invalid Credentials';
                    $scope.ngfeedback = true;
                    $rootScope.user = {};
                } else {
                    $scope.ngfeedback = false;
                    $rootScope.isAuthenticated = true;
                    $rootScope.user = response;
                    if ($rootScope.redirectUrl == null || $rootScope.redirectUrl.length < 1)
                    {
                        $location.path = '/ngy.html#dashboard';
                    } else
                    {
                        $location.path = $rootScope.redirectUrl;
                        $rootScope.countdown(response.TicketTimeOut);
                    }
                }
            };
            
            $scope.getSignedOnUserCompleted = function (response)
            {
                if (response.IsAuthenicated === false)
                {
                    $rootScope.isAuthenticated = false;
                    $rootScope.user = {};

                    if (path.indexOf("ngy.html") < 0)
                    {
                        $location.path('/Start/Start'); 
                    }

                    if (path.indexOf("ngs.html") < 0)
                    {
                        $location.path('/Welcome/Welcome');
                    }
                    else
                    {
                        $window.location.href = $rootScope.homeroot + '/ngy.html#/Store/Welcome/Welcome';
                    }
                }
                else
                {
                    $rootScope.user = response.UserProfile;
                    $rootScope.store = response.StoreInfo;
                    $rootScope.getCountries();
                    $rootScope.res = response;

                    var customerRoles = response.Roles.filter(function (r)
                    {
                        return r === 'Customer';
                    });

                    if (customerRoles.length > 0)
                    {
                        $rootScope.isCustomer = true;
                        return;
                    }

                    var roles = response.Roles.filter(function (r)
                    {
                        return r === 'Admin' || r === 'Super_Admin';
                    });
                    if (roles.length > 0)
                    {
                        $rootScope.isAdmin = true;
                    }
                    else
                    {
                        $rootScope.isAdmin = false;
                    }

                    var roles2 = response.Roles.filter(function (r)
                    {
                        return r === 'Cashier';
                    });

                    if (roles2.length) {
                        $rootScope.isCashier = true;

                    }
                    else {
                        $rootScope.isCashier = false;
                    }

                    var roles3 = response.Roles.filter(function (r) {
                        return r === 'Purchasing';
                    });

                    if (roles3.length) {
                        $rootScope.isPurchaser = true;

                    }
                    else {
                        $rootScope.isPurchaser = false;
                    }

                    var roles4 = response.Roles.filter(function (r) {
                        return r === 'Marketer';
                    });

                    if (roles4.length) {
                        $rootScope.isMarketer = true;

                    }
                    else {
                        $rootScope.isMarketer = false;
                    }

                    //sort parent menu items
                    response.UserLinks.sort(function (a, b) {
                        return (a['MenuOrder'] > b['MenuOrder']) ? 1 : ((a['MenuOrder'] < b['MenuOrder']) ? -1 : 0);
                    });

                    //sort child menu items
                    angular.forEach(response.UserLinks, function (r) {
                        if (r.ChildMenuObjects.length > 0) {
                            r.ChildMenuObjects.sort(function (a, b) {
                                return (a['ChildMenuOrder'] > b['ChildMenuOrder']) ? 1 : ((a['ChildMenuOrder'] < b['ChildMenuOrder']) ? -1 : 0);
                            });
                        }

                    });

                    $rootScope.links = response.UserLinks;
                    angular.element('.autoFade').fadeIn('fast');
                    angular.element('.nv-side').fadeIn('fast');
                    Idle.watch();
                }
            };

            $rootScope.signUp = function (auth)
            {
                if (auth == null)
                {
                    alert('Action failed');
                    return;
                }
                if (auth.LastName == null || auth.LastName.length < 1)
                {
                    alert('Please provide your Last Name');
                    return;
                }

                if (auth.OtherNames == null || auth.OtherNames.length < 1)
                {
                    alert('Please provide your Other Names');
                    return;
                }

                if (auth.Email == null || auth.Email.length < 1)
                {
                    alert('Please provide your Email');
                    return;
                }

                if (auth.Password == null || auth.Password.length < 1)
                {
                    alert('Please provide your Password');
                    return;
                }

                var model = { 'Email': auth.Email, 'Password': auth.Password, 'LastName' : auth.LastName, 'OtherNames' : auth.OtherNames };
                
                $scope.AjaxPostAuth({ model: model }, "/Account/SignUp", $rootScope.signupComplete);
            };

            $rootScope.signupComplete = function (response)
            {
                alert(response.Feedback);

                if (response.Code < 1)
                {
                    return;
                }

                $rootScope.userName = response.UserName;
                $rootScope.isAuthenticated = true;
               angular.element('#signUpSection').hide();
            };

            $rootScope.countdown = function ()
            {
                $rootScope.stopped = $timeout(function ()
                {
                    if (timeOut <= 1)
                    {
                        $rootScope.stop($rootScope.stopped);
                    } else {
                        timeOut--;
                        $scope.countdown();
                    }
                   
                }, 1000);
                
            };
            
            $rootScope.stop = function (stopped)
            {
                $timeout.cancel(stopped);
                $rootScope.isAuthenticated = false;
            };

            $scope.authenicateUserError = function (response) {
                alert("ERROR= " + response.IsAuthenicated);
            };

            $scope.AjaxGet = function (route, successFunction)
            {
                setTimeout(function () {
                    $http({ method: 'GET', url: route }).success(function (response)
                    {
                        successFunction(response);
                    });
                }, 1);

            };

            $scope.AjaxPostAuth = function (data, route, callbackFunction)
            {
                setTimeout(function ()
                {
                    setUIBusy();
                    $http.post(route, data).success(function (response)
                    {
                        stopUIBusy();
                        callbackFunction(response, status);
                    });
                }, 1000);
            };
            
            $scope.AjaxPost = function (route, callbackFunction)
            {
                setTimeout(function () {
                    $http.post(route).success(function (response)
                    {
                        callbackFunction(response);
                    });
                }, 10);

            };
            
            $scope.AjaxGetWithData = function (data, route, successFunction)
            {
                setTimeout(function ()
                {
                    $http({ method: 'GET', url: route, params: data }).success(function (response)
                    {
                        successFunction(response);
                    });
                }, 1);
            };
            
            $scope.AjaxPostWithData = function (data, route, successFunction) {
                setTimeout(function () {
                    $http({ method: 'POST', url: route, params: data }).success(function (response) {
                        successFunction(response);
                    });
                }, 1);
            };
            
            $scope.AjaxPostAction = function (data, route, callbackFunction)
            {
                setUIBusy();
                setTimeout(function ()
                {
                    $http.post(route, data).success(function (response)
                    {
                        stopUIBusy();
                        callbackFunction(response, status);
                    });
                }, 1000);
            };

            /*              manage shopping cart globally                   */

            $rootScope.checkOut = function ()
            {
                if ($rootScope.ShoppingCart == null || $rootScope.ShoppingCart.ShoppingCartId < 1 || $rootScope.ShoppingCart.ShopingCartItemObjects === null || $rootScope.ShoppingCart.ShopingCartItemObjects.length < 1) {
                    alert('Your cart is empty!');
                    return;
                }
                
                var cartItemsCount = 0;
                var cartAmount = 0;
                angular.forEach($rootScope.ShoppingCart.ShopingCartItemObjects, function (t, k)
                {
                    cartItemsCount += t.QuantityOrdered;
                    cartAmount += t.UnitPrice * t.QuantityOrdered;
                });

                $rootScope.cartTotalAmount = cartAmount;
                $rootScope.cartItemsCount = cartItemsCount;
                $location.path('/Store/CheckOut/CheckOut');
                $rootScope.closeCart();
            };

            $rootScope.addToWishList = function ()
            {
                if ($rootScope.ShoppingCart.ShopingCartItemObjects == null || $rootScope.ShoppingCart.ShoppingCartId < 1 || $rootScope.ShoppingCart.ShopingCartItemObjects === null) {
                    return;
                }

                $rootScope.ShoppingCart.ShopingCartItemObjects = 0;

                var cartItemsCount = 0;
                var cartAmount = 0;
                angular.forEach($rootScope.ShoppingCart.ShopingCartItemObjects, function (t, k) {
                    cartItemsCount += t.QuantityOrdered;
                    cartAmount += t.UnitPrice * t.QuantityOrdered;
                });

                $rootScope.cartTotalAmount = cartAmount;
                $rootScope.cartItemsCount = cartItemsCount;

            };

            $rootScope.checkQuantity = function (cartItem)
            {
                var test = parseInt(cartItem.QuantityOrdered);

                if (isNaN(test) || test < 1)
                {
                    $scope.removeCartItem(cartItem);
                    return;
                }
                else {
                    $scope.updateCartItem(cartItem);
                }
            };

            $rootScope.updateCartItem = function (cartItem)
            {
                if (cartItem.ShopingCartItemId < 1)
                {
                    return;
                }

                $scope.AjaxPostAction({ cartItemObject: cartItem }, "/ShoppingCart/UpdateShoppingCartItem", $rootScope.updateItemCompleted);
            };

            $rootScope.updateItemCompleted = function (response)
            {
                if (response < 1)
                {
                    $scope.setError('Cart Item could not be updated. Please try again later.');
                    return;
                }

                var cartItemsCount = 0;
                var cartAmount = 0;

                var cartItemOject = { ShoppinCartItemId: cartItem.ShopingCartItemId, QuantityOrdered: cartItem.QuantityOrdered };
                $scope.cartItemOject = cartItemOject;

                angular.forEach($rootScope.ShoppingCart.ShopingCartItemObjects, function (x, y) {
                    if (x.ShopingCartItemId === $scope.cartItemOject.ShoppinCartItemId) {
                        x.QuantityOrdered = $scope.cartItemOject.QuantityOrdered;
                    }
                    cartItemsCount += x.QuantityOrdered;
                    cartAmount += x.UnitPrice * x.QuantityOrdered;

                });

                $rootScope.cartTotalAmount = cartAmount;
                $rootScope.cartItemsCount = cartItemsCount;
            };

            $rootScope.removeCartItem = function (cartItem)
            {
                if (cartItem.ShopingCartItemId < 1)
                {
                    return;
                }
                var msg = 'This item will be removed permanently from your cart. continue?';
                if (!confirm(msg))
                {
                    return;
                }

                $rootScope.busy = true;
                $scope.AjaxPostWithData(cartItem.ShopingCartItemId, "/ShoppingCart/UpdateShoppingCartItem", $rootScope.removeCartItemCompleted);
            };

            $rootScope.removeCartItemCompleted = function (response)
            {
                $rootScope.busy = false;
                if (response < 1)
                {
                    $scope.setError('Item could not be removed from your cart.');
                    return;
                }

                if (response === 10)
                {
                    $rootScope.ShoppingCart = null;
                    $scope.setSuccessFeedback('You have successfully removed your cart.');
                    return;
                }

                var cartItemsCount = 0;
                var cartAmount = 0;

                angular.forEach($rootScope.ShoppingCart.ShopingCartItemObjects, function (x, y)
                {
                    if (x.ShopingCartItemId === $scope.cartItemId)
                    {
                        $rootScope.ShoppingCart.ShopingCartItemObjects.splice(y, 1);
                    }
                    cartItemsCount += x.QuantityOrdered;
                    cartAmount += x.UnitPrice * x.QuantityOrdered;
                });

                $rootScope.cartTotalAmount = cartAmount;
                $rootScope.cartItemsCount = cartItemsCount;
            };
            
            $rootScope.setViewTrail = function (item)
            {
                if ($localStorage.previouslyViewedItems == null || $localStorage.previouslyViewedItems === undefined)
                {
                    $localStorage.previouslyViewedItems = [];
                }
                var similarItems = $localStorage.previouslyViewedItems.filter(function (t) {
                    return t.StoreItemStockId === item.StoreItemStockId;
                });

                if (similarItems.length > 0)
                {
                    return;
                }
                $localStorage.previouslyViewedItems.push(item);
                return;
            };

            $rootScope.getViewTrail = function ()
            {
                var viewdItems = $localStorage.previouslyViewedItems;
                $rootScope.viewedItems = viewdItems;
            };
           
        }]);
     // Bootstrap Angular when DOM is ready
    angularAMD.bootstrap(app);
    return app;
});


//var path = $location.absUrl();
//if (path.indexOf("ngx.html") > -1)
//{
//KeepaliveProvider.http('/BillingCycle/RefreshSession');
//}