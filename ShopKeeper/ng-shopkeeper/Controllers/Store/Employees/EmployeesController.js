"use strict";

define(['application-configuration', 'employeeServices', 'alertsService', 'ngDialog'], function (app)
{
    app.register.directive('ngEmployeeTable', function ($compile)
    {
        return function ($scope, ngEmployeeTable)
        {
                var tableOptions = {};
                tableOptions.sourceUrl = "/Employee/GetEmployeeObjects";
                tableOptions.itemId = 'EmployeeId';
                tableOptions.columnHeaders = ['Name', 'EmployeeNo', 'JobTitle', 'Outlet', 'Department', 'StatusStr'];
                var ttc = employeeTableManager($scope, $compile, ngEmployeeTable, tableOptions, 'New Employee', 'prepareEmployeeTemplate', 'getEmployee', 'getEmployeeDetails', 135);
                ttc.removeAttr('width').attr('width', '100%');
                $scope.jtable = ttc;
        };
    });


    app.register.controller('employeeController', ['ngDialog','$scope', '$rootScope', '$routeParams', 'employeeServices', 'alertsService',
    function (ngDialog, $scope, $rootScope, $routeParams, employeeServices, alertsService)
    {
       $scope.initializeController = function ()
       {
           $scope.initializeModel();
           $scope.genericListObject = {};
           employeeServices.getGenericList($scope.getGenericListCompleted);
       };

       $scope.initializeModel = function ()
       {
           var employee = new Object();
           employee.EmployeeId = 0;
           employee.Id = '';
           employee.EmployeeNo = '';
           employee.JobRoleId = '';
           employee.DateHiredStr = '';
           employee.DateLeftStr = '';
           employee.StreetNo = '';
           employee.StoreCityId = '';
           employee.StoreOutletId = '';
           employee.StoreAddressId = '';
           employee.StoreDepartmentId = '';
           $scope.buttonStatus = 1;
           $scope.buttonText = 'Add Employee';
           
            employee.department =
            {
                'StoreDepartmentId': '',
                'Name': '-- Select --'
            };

            employee.city =
            {
                'StoreCityId': '',
                'Name': '-- Select --'
            };
           
            employee.Status =
            {
                'StatusId': '1',
                'Name': 'Active'
            };
           
            employee.jobRole =
            {
                'JobRoleId': '',
                'JobTitle': '-- Select --'
            };
            employee.person =
            {
                'Id': '',
                'LastName': '-- Select --'
            };

           $scope.employee = employee;
       };
        
       var xcvb = new Date();
       var year = xcvb.getFullYear();
       var month = xcvb.getMonth() + 1;
       var day = xcvb.getDate();
       var minDate = year + '/' + month + '/' + day;

       setControlDate($scope, '', minDate);
       setEndDate($scope, minDate, '');
        
       $scope.getGenericListCompleted = function (data)
       {
           $scope.genericListObject.Statuses =
           [
               { 'StatusId': 1, 'Name': 'Active' },
               { 'StatusId': 2, 'Name': 'Suspended' },
               { 'StatusId': 3, 'Name': 'On-Leave' },
               { 'StatusId': 4, 'Name': 'InActive' }
           ];
           
           $scope.genericListObject.roles = data.Roles;
           $scope.genericListObject.departments = data.Departments;
           //$rootScope.getCountries();
       };

       $scope.prepareEmployeeTemplate = function ()
       {
           $scope.initializeModel();
           $scope.employee.Header = 'New Employee';
           ngDialog.open({
               template: '/ng-shopkeeper/Views/Store/Employees/ProcessEmployee.html',
               className: 'ngdialog-theme-flat',
               scope: $scope
           });
       };
       
       $scope.processEmployee = function ()
       {
            var employee = new Object();
            employee.EmployeeId = $scope.employee.EmployeeId;
            employee.EmployeeNo = $scope.employee.EmployeeNo;
            employee.RoleId = $scope.employee.Role.Id;
            employee.DateHired = $scope.employee.DateHiredStr;
            employee.DateLeft = $scope.employee.DateLeftStr;
            employee.StreetNo = $scope.employee.StreetNo;
            employee.StoreCityId = $scope.employee.City.StoreCityId;
            employee.StoreStateId = $scope.employee.State.StoreStateId;
            employee.StoreCountryId = $scope.employee.Country.StoreCountryId;
            employee.StoreOutletId = $scope.employee.StoreOutletId;
            employee.StoreAddressId = $scope.employee.StoreAddressId;
            employee.StoreDepartmentId = $scope.employee.department.StoreDepartmentId;
            employee.PhoneNumber = $scope.employee.PhoneNumber;
            employee.Password = $scope.employee.Password;
            employee.Email = $scope.employee.Email;
            employee.OtherNames = $scope.employee.OtherNames;
            employee.LastName = $scope.employee.LastName;
            
            if ($scope.edit || employee.EmployeeId > 0)
            {
                employee.Status = $scope.employee.StatusObject.StatusId;
            }
            else
            {
                employee.Status = 1;

                if (employee.Password == null || employee.Password.length < 1)
                {
                    alert("ERROR: Please Provide Password.");
                    return;
                }

                if (employee.Password.length < 8)
                {
                    alert("The password should be at least 8 characters wide");
                    return;
                }
                
            }
           
            if (!$scope.validatateEmployee(employee))
            {
                return;
            }

            if ($scope.employee.EmployeeId < 1)
            {
                employeeServices.addEmployee(employee, $scope.processEmployeeCompleted);
            }
            else
            {
                employeeServices.editEmployee(employee, $scope.processEmployeeCompleted);
            }
           
       };
       
       $scope.validatateEmployee = function (employee)
       {
           if (employee.Status == undefined || employee.Status < 1)
           {
               alert("ERROR: Please select a Status. ");
               return false;
           }

           if (employee.RoleId == undefined || employee.RoleId.length < 1)
            {
               alert("ERROR: Please select Role. ");
                return false;
           }
       
           
           if (employee.StoreCityId == undefined || employee.StoreCityId < 1)
           {
                alert("ERROR: Please select City. ");
                return false;
            }
           
            if (employee.StoreDepartmentId == undefined || employee.StoreDepartmentId < 1)
            {
                alert("ERROR: Please select a Department. ");
                return false;
            }

            if (employee.StreetNo == null || employee.StreetNo.length < 1)
            {
                alert("ERROR: Please Provide Address. ");
                return false;
            }

            if (employee.PhoneNumber == null || employee.PhoneNumber.length < 1) 
            {
                alert("ERROR: Please Provide Phone Number.");
                return false;
            }

            if (employee.OtherNames == null || employee.OtherNames.length < 1)
            {
                alert("ERROR: Please Provide Other Names.");
                return false;
            }

            if (employee.LastName == null || employee.LastName.length < 1)
            {
                alert("ERROR: Please Provide Last Name.");
                return false;
            }
           
            if (employee.Email == null || employee.Email.length < 1)
            {
                alert("ERROR: Please Provide Email.");
                return false;
            }

            return true;
        };

       $scope.processEmployeeCompleted = function (data)
       {
           if (data.Code < 1)
           {
                alert(data.Error);
                return;
           }
           else
           {
                alert(data.Error);
                $scope.jtable.fnClearTable();
                ngDialog.close('/ng-shopkeeper/Views/Store/Employees/ProcessEmployee.html', '');
                $scope.initializeModel();
            }
        };

       $scope.closeInfo = function ()
       {
           ngDialog.close('/ng-shopkeeper/Views/Store/Employees/EmployeeInfo.html', '');
           $scope.initializeModel();
       };
        
       $scope.getEmployee = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           employeeServices.getEmployee(id, $scope.getEmployeeCompleted);
       };

       $scope.getEmployeeCompleted = function (data)
       {
           if (data.Code < 1)
           {
             alert(data.Error);
           }
            else
           {
               var roles = $scope.genericListObject.roles.filter(function (r)
               {
                   return r.Id === data.RoleId;
               });

               if (roles.length > 0)
               {
                   data.Role = roles[0];
               }

               var departments = $scope.genericListObject.departments.filter(function (r)
               {
                   return r.StoreDepartmentId === data.StoreDepartmentId;
               });

               if (departments.length > 0)
               {
                  data.department = departments[0];
               }

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

               var sts = $scope.genericListObject.Statuses.filter(function (r)
               {
                   return r.StatusId === data.Status;
               });

               if (sts.length > 0)
               {
                   data.StatusObject = sts[0];
               }
               
               $scope.edit = true;
               data.Header = 'Update Employee Information';
               $scope.employee = data;
               ngDialog.open({
                   template: '/ng-shopkeeper/Views/Store/Employees/ProcessEmployee.html',
                   className: 'ngdialog-theme-flat',
                   scope: $scope
               });
            }
       };
        
       $scope.getEmployeeDetails = function (id)
       {
           if (parseInt(id) < 1 || id == undefined || id == NaN)
           {
               alert("ERROR: Invalid selection! ");
               return;
           }

           employeeServices.getEmployee(id, $scope.getEmployeeDetailsCompleted);
       };

       $scope.getEmployeeDetailsCompleted = function (data)
       {
           if (data.EmployeeId < 1)
           {
               alert(data.Error);
           }
           else
           {
               var employee = new Object();
               employee.EmployeeNo = data.EmployeeNo,
               employee.DateHiredStr = data.DateHiredStr,
               employee.DateLeftStr = data.DateLeftStr,
               employee.Department = data.Department,
               employee.Outlet = data.OutletName,
               employee.Name = data.Name,   
               employee.JobTitle = data.JobTitle,
               employee.Address = data.Address,
               employee.CityName = data.CityName,
               employee.Status = data.StatusStr;

               employee.Header = 'Employee Information';
               $scope.employee = data;
               ngDialog.open({
                   template: '/ng-shopkeeper/Views/Store/Employees/EmployeeInfo.html',
                   className: 'ngdialog-theme-flat',
                   scope: $scope
               });
           }
       };
       
       $scope.deleteEmployee = function (id)
       {
           if (parseInt(id) > 0)
           {
               if (!confirm("This Employee information will be deleted permanently. Continue?"))
               {
                   return;
               }
               employeeServices.deleteEmployee(id, $scope.deleteEmployeeCompleted);
           }
           else
           {
               alert('Invalid selection.');
           }
       };

       $scope.deleteEmployeeCompleted = function (data)
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

        $scope.getStatesCompleted = function (states) {
           $rootScope.processing = false;
           if (states == null || states.length < 1) {
               return;
           }

           $scope.states = states;

           var lStates = $scope.states.filter(function (r)
           {
               return r.StoreStateId === $scope.employee.StoreStateId;
           });

           if (lStates.length > 0)
           {
               $scope.employee.State = lStates[0];
               $rootScope.getCities($scope.employee.State, $scope.getCitiesCompleted);
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
               return r.StoreCityId === $scope.employee.StoreCityId;
           });

           if (lCities.length > 0)
           {
               $scope.employee.City = lCities[0];
           }
       };


    }]);
   
});

