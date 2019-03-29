"use strict";

define(['application-configuration', 'parentMenuServices', 'alertsService', 'ngDialog'], function (app)
{

    app.register.directive('ngParentMenu', function ($compile)
    {
        return function ($scope, ngParentMenu)
        {
                var tableOptions = {};
                tableOptions.sourceUrl = "/ParentMenu/GetParentMenuObjects";
                tableOptions.itemId = 'ParentMenuId';
                tableOptions.columnHeaders = ['Value', 'MenuOrder', 'RoleName'];
                var ttc = parentMenutableManager($scope, $compile, ngParentMenu, tableOptions, 'New Menu', 'prepareMenuTemplate', 'getParentMenu', 105);
                ttc.removeAttr('width').attr('width', '100%');
                $scope.jtable = ttc;
        };

    });
    
    app.register.controller('parentMenuController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'parentMenuServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, parentMenuServices, alertsService)
    {
        $scope.getRoles = function ()
        {
            parentMenuServices.getRoles($scope.getRolesCompleted);
        };

        $scope.goBack = function ()
        {
            $scope.menuView = false;
        };

        $scope.getRolesCompleted = function (roles)
        {
            $scope.roles = [];
            if (roles && roles.length > 0)
            {
                angular.forEach(roles, function (r, i)
                {
                    r.Status = false;
                });
                $scope.roles = roles;
            }
        };

        $scope.initializeController = function ()
        {
           $scope.initializeMenu();
           $scope.getRoles();
        };
       
        $scope.initializeMenu = function ()
        {
            $scope.parentMenu = {'ParentMenuId' : 0, 'Value': '', 'Href': '', 'MenuOrder': '', 'ParentMenuRoleObjects': [], 'ChildMenuObjects' : [], 'GlyphIconClass' : '' };
            $scope.Header = 'New Parent Menu';
            $scope.process = false;
            $scope.initializeRoles();
        };
        
        $scope.initializeChildMenu = function ()
        {
            $scope.childMenu =
            {
                'ChildMenuId' : 0,
                'ParentMenuId': 0,
                'IsParent': false,
                'ParentChildId': null,
                'Value': '',
                'RoleName' : '',
                'Href': '',
                'ChildMenuOrder': '',
                'ChildMenuRoleObjects': []
            };
            $scope.childHeader = 'New Child Menu';
            $scope.process = false;
            $scope.initializeRoles();
        };

        $scope.initializeRoles = function ()
        {
            angular.forEach($scope.roles, function (r, i)
            {
                r.Status = false;
            });
        };

        $scope.prepareMenuTemplate = function ()
        {
            $scope.initializeMenu();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/MenuSettings/ProcessParentMenu.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };

        $scope.prepareChildMenuTemplate = function ()
        {
            $scope.initializeChildMenu();
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/MenuSettings/ProcessChildMenu.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };
        
        $scope.processMenu = function ()
        {
            var parentMenu = $scope.parentMenu;
            
            if (!parentMenu.Value || parentMenu.Value.length < 1)
            {
                alert("Please provide Menu display Text. ");
                return;
            }
            
            if (parentMenu.Href == undefined || parentMenu.Href.length < 1)
            {
                alert("ERROR: Please provide Menu Link. ");
                return;
            }

            if (parentMenu.MenuOrder == undefined || parentMenu.MenuOrder < 1)
            {
                alert("ERROR: Please provide Menu Order. ");
                return;
            }
            
            if (parentMenu.ParentMenuRoleObjects == undefined || parentMenu.ParentMenuRoleObjects.length < 1)
            {
                alert("ERROR: Please provide at least one role to access the Menu. ");
                return;
            }

            if (parentMenu.ParentMenuId < 1)
            {
                $scope.parentMenu.ChildMenuObjects = [];
                parentMenuServices.addParentMenu(parentMenu, $scope.processParentMenuCompleted);
            }

            if (parentMenu.ParentMenuId > 0)
            {
                parentMenuServices.updateParentMenu(parentMenu, $scope.processParentMenuCompleted);
            }

            var roleNames = '';
            angular.forEach($scope.parentMenu.ParentMenuRoleObjects, function (r, i)
            {
                roleNames += r.RoleName + ', ';
            });

            $scope.parentMenu.RoleName = roleNames;
            $scope.process = true;
        };

        $scope.processParentMenuCompleted = function (data)
        {
            $scope.process = false;
            alert(data.Error);
            if (data.Code == null || data.Code < 1) {
                alert(data.Error);
            }
            else {
                $scope.jtable.fnClearTable();
                ngDialog.close('/ng-shopkeeper/Views/Store/MenuSettings/ProcessParentMenu.html', '');
                $scope.initializeMenu();
            }
        };

        $scope.processChildMenu = function ()
        {
            $scope.childMenu.ParentMenuId = $scope.parentMenu.ParentMenuId;

            var childMenu = $scope.childMenu;

            if (childMenu.Value === undefined || childMenu.Value.length < 1)
            {
                alert("Please provide Menu display Text. ");
                return;
            }

            if (childMenu.Href == undefined || childMenu.Href.length < 1)
            {
                alert("ERROR: Please provide Menu Link. ");
                return;
            }

            if (childMenu.ChildMenuOrder == undefined || childMenu.ChildMenuOrder < 1)
            {
                alert("ERROR: Please provide Menu Order. ");
                return;
            }

            if (childMenu.ChildMenuRoleObjects == undefined || childMenu.ChildMenuRoleObjects.length < 1)
            {
                alert("ERROR: Please provide at least one role to access the Menu. ");
                return;
            }
            
            if (childMenu.ChildMenuId < 1)
            {
                parentMenuServices.addChildMenu(childMenu, $scope.processChildMenuCompleted);
            }

            if (childMenu.ChildMenuId > 0)
            {
                parentMenuServices.updateChildMenu(childMenu, $scope.processChildMenuCompleted);
            }

            var roleNames = '';
            angular.forEach($scope.childMenu.ChildMenuRoleObjects, function (r, i)
            {
                roleNames += r.RoleName + ', ';
            });
            $scope.childMenu.RoleName = roleNames;
             
            $scope.process = true;
        };

        $scope.processChildMenuCompleted = function (data)
        {
            $scope.process = false;
            alert(data.Error);
            if (data.Code == null || data.Code < 1)
            {
                return;
            }
           
            if ($scope.childMenu.ChildMenuId < 1)
            {
                $scope.childMenu.ChildMenuId = data.Code;
                if ($scope.parentMenu.ChildMenuObjects === null || $scope.parentMenu.ChildMenuObjects === undefined)
                {
                    $scope.parentMenu.ChildMenuObjects = [];
                }
                $scope.parentMenu.ChildMenuObjects.push($scope.childMenu);
                $scope.resetChildRoleName($scope.childMenu);
            }
            else
            {
                angular.forEach($scope.parentMenu.ChildMenuObjects, function (x, i)
                {
                    if (x.ChildMenuId === $scope.childMenu.ChildMenuId)
                    {
                        x.Value = $scope.childMenu.Value;
                        x.RoleName = $scope.childMenu.RoleName;
                        x.Href = $scope.childMenu.Href;
                        x.ChildMenuOrder = $scope.childMenu.ChildMenuOrder;
                        x.ChildMenuRoleObjects = $scope.childMenu.ChildMenuRoleObjects;
                        $scope.resetChildRoleName(x);
                    }
                });
            }
            
            ngDialog.close('/ng-shopkeeper/Views/Store/MenuSettings/ProcessChildMenu.html', '');
            $scope.initializeChildMenu();
            
        };

        $scope.resetParentRoleName = function (menu)
        {
            var roleNames = '';
            angular.forEach(menu.ParentMenuRoleObjects, function (r, i)
            {
                roleNames += r.RoleName + ', ';
            });
            menu.RoleName = roleNames;
        };

        $scope.resetChildRoleName = function (menu)
        {
            var roleNames = '';
            angular.forEach(menu.ChildMenuRoleObjects, function (r, i)
            {
                roleNames += r.RoleName + ', ';
            });
            menu.RoleName = roleNames;
        };
        
        $scope.addRole = function (status, role)
        {
             if (!status) {
                 if ($scope.parentMenu.ParentMenuRoleObjects != null && $scope.parentMenu.ParentMenuRoleObjects.length > 0)
                 {
                     angular.forEach($scope.parentMenu.ParentMenuRoleObjects, function (x, n)
                     {
                         if (x.RoleId === role.Id)
                         {
                             $scope.parentMenu.ParentMenuRoleObjects.splice(n, 1);
                         }
                     });
                 }
             }
             
            else
                 {
                     $scope.parentMenu.ParentMenuRoleObjects.push({
                         ParentMenuId: 0,
                         ParentMenuRoleId: 0,
                         RoleId: role.Id,
                         RoleName: role.Name
                    });
            }
            var roleNames = '';
            angular.forEach($scope.parentMenu.ParentMenuRoleObjects, function (x, n) {
                roleNames += x.RoleName + ', ';
            });

            $scope.parentMenu.RoleName = roleNames;
        };

        $scope.addChildRole = function (status, role)
        {
            if (!status)
            {
                if ($scope.childMenu.ChildMenuRoleObjects != null && $scope.childMenu.ChildMenuRoleObjects.length > 0)
                {
                    angular.forEach($scope.childMenu.ChildMenuRoleObjects, function (x, n)
                    {
                        if (x.RoleId === role.Id)
                        {
                            $scope.childMenu.ChildMenuRoleObjects.splice(n, 1);
                        }
                    });
                }
            }
            else
            {
                $scope.childMenu.ChildMenuRoleObjects.push({
                    ChildMenuId: 0,
                    ChildMenuRoleId: 0,
                    RoleId: role.Id,
                    RoleName: role.Name
                });
            }
            var roleNames = '';
            angular.forEach($scope.childMenu.ChildMenuRoleObjects, function (x, n)
            {
                roleNames += x.RoleName + ', ';
            });

            $scope.parentMenu.RoleName = roleNames;
        };

        $scope.getParentMenu = function (id)
        {
            if (id == undefined || id === NaN || parseFloat(id) < 1)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }

            parentMenuServices.getParentMenu(id, $scope.getParentMenuCompleted);
        };
        
        $scope.getParentMenuCompleted = function (data)
        {
            if (data.ParentMenuId < 1)
            {
                alert("ERROR: Menu information could not be retrieved! ");

            }
            else
            {
                $scope.initializeMenu();
                $scope.parentMenu = data;
                $scope.menuView = true;
                $scope.getParentMenuChildren(data.ParentMenuId);
            }
        };

        $scope.getParentMenuChildren = function (id)
        {
            if (id == undefined || id === NaN || parseFloat(id) < 1)
            {
               return;
            }

            parentMenuServices.getParentMenuChildren(id, $scope.getParentMenuChildrenCompleted);
        };

        $scope.getParentMenuChildrenCompleted = function (data)
        {
            if (data.length < 1)
            {
                return;
            }
            $scope.parentMenu.ChildMenuObjects = data;
        };

        $scope.setParentMenuForEdit = function ()
        {
            if ($scope.parentMenu === undefined || $scope.parentMenu === null || $scope.parentMenu.ParentMenuId < 1)
            {
                alert("An unknown error was encountered. Please try again later.");
                return;
            }
           
            angular.forEach($scope.parentMenu.ParentMenuRoleObjects, function (r, i)
            {
                angular.forEach($scope.roles, function (x, i)
                {
                    if (x.Id === r.RoleId)
                    {
                        x.Status = true;
                    }
                });

            });

            $scope.Header = 'Update Menu Information';
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/MenuSettings/ProcessParentMenu.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };

        $scope.setChildMenuForEdit = function (childMenu)
        {
            if (childMenu === undefined || childMenu === null || childMenu.ChildMenuId < 1)
            {
                alert("An unknown error was encountered. Please try again later.");
                return;
            }
            $scope.initializeChildMenu();
            $scope.initializeRoles();
            angular.forEach(childMenu.ChildMenuRoleObjects, function (r, i)
            {
                angular.forEach($scope.roles, function (x, i)
                {
                    if (x.Id === r.RoleId)
                    {
                        x.Status = true;
                    }
                });

            });
            
            $scope.childMenu = childMenu;
            $scope.childHeader = 'Update Child Menu Information';
            ngDialog.open({
                template: '/ng-shopkeeper/Views/Store/MenuSettings/ProcessChildMenu.html',
                className: 'ngdialog-theme-flat',
                scope: $scope
            });
        };

    }]);

});