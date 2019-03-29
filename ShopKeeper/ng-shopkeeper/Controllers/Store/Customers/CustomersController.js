"use strict";

define(['application-configuration', 'customerServices', 'alertsService', 'ngDialog', 'angularFileUpload', 'fileReader'], function (app)
{
    app.register.directive('ngCustomer', function ($compile)
    {
        return function ($scope, ngCustomer)
        { 
            var tableOptions = {};
            tableOptions.sourceUrl = "/Customer/GetCustomerObjects";
            tableOptions.itemId = 'CustomerId';
            tableOptions.columnHeaders = ['UserProfileName', 'CustomerTypeName', 'StoreOutletName', 'Email', 'MobileNumber', 'OfficeLine', 'BirthDayStr'];
            var ttc = customerTableManager($scope, $compile, ngCustomer, tableOptions, 'New Customer ', 'prepareCustomerTemplate', '/StoreItemStock/DownloadContentFromFolder?path=~/BulkTemplates/Customers/customer_bulk.xlsx', 'getCustomer', 'getCustomerDetails', 129);

            ttc.removeAttr('width').attr('width', '100%');
            $scope.ttc = ttc;
        };
    });
   
    app.register.controller('customerController', ['ngDialog', '$scope', '$rootScope', '$routeParams', 'customerServices', '$upload', 'fileReader','$http',
    function (ngDialog, $scope, $rootScope, $routeParams, customerServices, $upload, fileReader,$http)
    {
        $scope.isAdmin = $rootScope.isAdmin;

        var xcvb = new Date();
        var year = xcvb.getFullYear();
        var month = xcvb.getMonth() + 1;
        var day = xcvb.getDate();
        var maxDate = year + '/' + month + '/' + day;

        setControlDate($scope, '', maxDate);
       
        $scope.initializeController = function ()
        {
            $scope.initializeCustomer();
            customerServices.getSelectables($scope.getSelectablesCompleted);
        };

        $scope.prepareCustomerTemplate = function ()
        {
            $scope.initializeCustomer();
            $scope.user.Header = 'Profile a Customer';
            $scope.customerInfo = true;
        };

        $scope.initializeCustomer = function ()
        {
            $scope.user =
            {
                Id: '',
                SalutationId: '',
                LastName: '',
                OtherNames: '',
                Gender: '', 
                Birthday: '',
                BirthdayStr : '',
                PhotofilePath: '',
                IsActive: '',
                ContactEmail: '',
                MobileNumber: '',
                OfficeLine: '',
                CustomerObjects:
                                  [
                                      {
                                          CustomerId: '',
                                          ReferredByCustomerId: '',
                                          StoreCustomerTypeId: '',
                                          StoreOutletId: '',
                                          UserId: '',
                                          FirstPurchaseDate: ''
                                      } 
                                  ]
                ,
                ContactPerson: {EmployeeId : '', Name : ''}
                ,
                DeliveryAddressObject:
                                         {
                                            Id : '',
                                            StateId : '',
                                            CountryId : '',
                                            CityName : '',
                                            StateName : '',
                                            CountryName : '',
                                            AddressLine1 : '',
                                            AddressLine2 : '',
                                            CityId : '',
                                            MobileNumber : '',
                                            TelephoneNumber :''
                                        }
            };

            $scope.user.customerType =
            {
                StoreCustomerTypeId: '',
                Name: '-- select customer type --',
                Code: '',
                Description : ''
            };
            
            $scope.user.storeOutlet =
            {
                StoreOutletId: '',
                OutletName: '-- select outlet --'
            };

            $scope.genders = [{ genderId: 1, name: 'Male' }, { genderId: 2, name: 'Female' }, { genderId: 3, name: 'Corporate' }];
            $scope.user.StoreCountryObject = { StoreCountryId: '', Name: '-- select country --' };
            $scope.user.StoreStateObject = { StoreStateId: '', Name: '-- select State --' };
            $scope.user.StoreCityObject = { StoreCityId: '', Name: '-- select city --' };
        };
        
        $scope.processCustomer = function ()
        {
            if ($scope.user.MobileNumber == null || $scope.user.MobileNumber === undefined || $scope.user.MobileNumber.length < 1)
            {
                alert("ERROR: Please provide customer's Mobile phone number.");
                return;
            }

            if ($scope.user.OtherNames == null || $scope.user.OtherNames === undefined || $scope.user.OtherNames.length < 1)
            {
                alert("ERROR: Please provide Customer's Other names.");
                return;
            }
            if ($scope.user.LastName == null || $scope.user.LastName === undefined || $scope.user.LastName.length < 1)
            {
                alert("ERROR: Please Customer's Last Name.");
                return;
            }
           
            if ($scope.user.customerType == null || $scope.user.customerType === undefined || $scope.user.customerType.StoreCustomerTypeId < 1)
            {
                alert("ERROR: Please select Customer Type.");
                return;
            }
            
            if ($scope.user.storeOutlet == null || $scope.user.storeOutlet === undefined || $scope.user.storeOutlet.StoreOutletId < 1)
            {
                alert("ERROR: Please select Store Outlet.");
                return;
            }

            if ($scope.user.GenderObject == null || $scope.user.GenderObject === undefined || $scope.user.GenderObject.genderId < 1)
            {
                alert("ERROR: Please select a gender.");
                return;
            }

            if ($scope.user.StoreCountryObject == null || $scope.user.StoreCountryObject === undefined || $scope.user.StoreCountryObject.StoreCountryId < 1)
            {
                alert("ERROR: Please select a Country.");
                return;
            }

            if ($scope.user.StoreStateObject == null || $scope.user.StoreStateObject === undefined || $scope.user.StoreStateObject.StoreStateId < 1)
            {
                alert("ERROR: Please select a state.");
                return;
            }

            if ($scope.user.StoreCityObject == null || $scope.user.StoreCityObject === undefined || $scope.user.StoreCityObject.StoreCityId < 1)
            {
                alert("ERROR: Please select a city.");
                return;
            }

            if ($scope.user.AddressLine1 == null || $scope.user.AddressLine1 === undefined || $scope.user.AddressLine1.length < 1)
            {
                alert("ERROR: Please provide Customer's street number.");
                return;
            }

            $scope.user.DeliveryAddressObject.Id = $scope.user.DeliveryAddressObject.Id;
            $scope.user.DeliveryAddressObject.StateId = $scope.user.StoreStateObject.StoreStateId;
            $scope.user.DeliveryAddressObject.CountryId = $scope.user.StoreCountryObject.StoreCountryId;
            $scope.user.DeliveryAddressObject.AddressLine1 = $scope.user.AddressLine1;
            $scope.user.DeliveryAddressObject.AddressLine2 = $scope.user.DeliveryAddressObject.AddressLine2;
            $scope.user.DeliveryAddressObject.CityId = $scope.user.StoreCityObject.StoreCityId;

            if ($scope.user.ContactPerson !== undefined && $scope.user.ContactPerson !== null && $scope.user.ContactPerson.EmployeeId > 0)
            {
                $scope.user.ContactPersonId = $scope.user.ContactPerson.EmployeeId;
            }
                
            $scope.user.Gender = $scope.user.GenderObject.name;
            $scope.user.CustomerObjects[0].StoreCustomerTypeId = $scope.user.customerType.StoreCustomerTypeId;
            $scope.user.CustomerObjects[0].StoreOutletId = $scope.user.storeOutlet.StoreOutletId;
            $scope.user.Birthday = $scope.user.BirthdayStr;
            
            if ($scope.user.Id == null || $scope.user.Id === '' || parseInt($scope.user.Id) === 0 || parseInt($scope.user.Id) < 1 || $scope.user.Id == undefined)
            {
                customerServices.addCustomer($scope.user, $scope.processCustomerCompleted);
            }
            else
            {
                customerServices.editCustomer($scope.user, $scope.processCustomerCompleted);
            }
        };

        $scope.processCustomerCompleted = function (data)
        {
            alert(data.Error);
            if (data.Code < 1)
            {
                return;
            }
            else
            {
                $scope.ttc.fnClearTable();
                $scope.customerInfo = false;
                $scope.initializeCustomer();
            }
        };
        
        $scope.getSelectablesCompleted = function (data)
        {
            $scope.outlets = data.StoreOutlets;
            $scope.customerTypes = data.CustomerTypes;
            $scope.employees = data.Employees;
 
        };

        $scope.getStates = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }
            customerServices.getStates(id, $scope.getStatesCompleted);
        };

        $scope.getStatesCompleted = function (data)
        {
            if (data == null || data.length < 1)
            {
                return;
            }

           $scope.user.StoreCountryObject.StoreStateObjects = data;
        };
     
        $scope.getCustomer = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }
            customerServices.getCustomer(id, $scope.getCustomerCompleted);
        };
       
        $scope.getCustomerCompleted = function (data)
        {
            
            if (data.CustomerObjects.length < 1)
            {
               alert("ERROR: Customer information could not be retrieved! ");
            }
            else
            {
                $scope.initializeCustomer();

                $scope.user = data;

               var types = $scope.customerTypes.filter(function (p) 
                {
                    return p.StoreCustomerTypeId === data.CustomerObjects[0].StoreCustomerTypeId;
                   
                });

                if (types.length > 0)
                {
                    $scope.user.customerType = types[0];
                }

                var genders = $scope.genders.filter(function (g)
                {
                    return g.name === data.Gender;
                });

                if (genders != null && genders.length > 0)
                {
                    $scope.user.GenderObject = genders[0];
                }

                var outlets = $scope.outlets.filter(function (g)
                {
                    return g.StoreOutletId === data.StoreOutletId;
                });

                if (outlets != null && outlets.length > 0)
                {
                    $scope.user.storeOutlet = outlets[0];
                }

                if (data.ContactPersonId !== null && data.ContactPersonId > 0)
                {
                    var contacts = $scope.employees.filter(function (p)
                    {
                        return p.EmployeeId === data.ContactPersonId;

                    });

                    if (types.length > 0)
                    {
                        $scope.user.ContactPerson = contacts[0];
                    }
                }

                $scope.user.AddressLine1 = data.DeliveryAddressObject.AddressLine1;
                $scope.user.Header = 'Update Customer Information';
                $scope.customerInfo = true;
                $scope.processing = true;
                $scope.getStates2(data.DeliveryAddressObject.CountryId);
            }
        };
        
        $scope.getCustomerDetails = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }
            customerServices.getCustomer(id, $scope.getCustomerDetailsCompleted);
        };

        $scope.getCustomerDetailsCompleted = function (data)
        {
            if (data.Id < 1)
            {
                alert("ERROR: Customer information could not be retrieved! ");
            }
            else
            {
                $scope.initializeCustomer();
                $scope.customerDetails = true;
                $scope.details = data;
            }
        };

        $scope.getStates2 = function (id)
        {
            if (parseInt(id) < 1 || id == undefined || id === NaN)
            {
                alert("ERROR: Invalid selection! ");
                return;
            }
            customerServices.getStates(id, $scope.getStatesCompleted2);
        };

        $scope.getStatesCompleted2 = function (data)
        {
            $scope.processing = false;

            if (data == null || data.length < 1)
            {
                alert('Customer address information could not be retrieved. Please try again later.');
                return;
            }

            var countries = $rootScope.countries.filter(function (o)
            {
                return o.StoreCountryId === $scope.user.DeliveryAddressObject.CountryId;
            });

            if (countries != null && countries.length > 0)
            {
                countries[0].StoreStatesObjects = data;
                $scope.user.StoreCountryObject = countries[0];

                angular.forEach(data, function (s)
                {
                    if (s.StoreStateId === $scope.user.DeliveryAddressObject.StateId)
                    {
                        $scope.user.StoreStateObject = s;

                        angular.forEach(s.StoreCityObjects, function (c)
                        {
                            if (c.StoreCityId === $scope.user.DeliveryAddressObject.CityId)
                            {
                                $scope.user.StoreCityObject = c;
                            }
                        });
                    }
                   
                });
              
            }

        };

        $scope.deleteCustomer = function (id)
        {
            if (parseInt(id) > 0)
            {
                if (!confirm("This Customer  information will be deleted permanently. Continue?"))
                {
                    return;
                }
                customerServices.deleteCustomer(id, $scope.deleteCustomerCompleted);
            }
            else
            {
                alert('Invalid selection.');
            }
        };

        $scope.deleteCustomerCompleted = function (data)
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
        
        $scope.refreshCustomerBrands = function (searchCriteria)
        {
            var params = { searchCriteria: searchCriteria };
            return $http.get('/Customer/GetCustomerObjects', { params: params }).then(function (response)
            {
                $scope.genericListObject.customerBrands = response;
            });
            
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
               
                return;
            }

            if (bulkDocument.size < 1)
            {
                $scope.setError('Please select a valid file.');
                return;
            }

            //if ($scope.outlet === undefined || $scope.outlet === null || $scope.outlet.StoreOutletId < 1) {
            //    alert('Please select an Outlet');
            //    return;
            //}

            setUIBusy();
            $upload.upload({
                url: "/Customer/UploadCustomers",
                method: "POST",
                data: { file: bulkDocument }
            })
           .progress(function (evt) {
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
                   angular.forEach(data, function (v, i)
                   {
                       if (v.Code < 1) {
                           errors.push(v);
                       }
                       else
                       {
                           successes.push(v);
                       }
                   });

                   if (errors.length > 0 && successes.length < 1) {
                       $scope.uploadError = "The following Customer(s) could not be processed due to the stated reasons.";
                       ngDialog.open({
                           template: '/ng-shopkeeper/Views/Store/Customers/CustomerUploadFeedback.html',
                           className: 'ngdialog-theme-flat',
                           scope: $scope
                       });
                       return;
                   }

                   if (errors.length > 0 && successes.length > 0) {
                       $scope.uploadError = successes.length + " Customer(s) were successfully processed, While those displayed below could not be processed due to the stated reasons.";
                       $scope.bulkError = true;
                       $scope.errors = errors;
                       ngDialog.open({
                           template: '/ng-shopkeeper/Views/Store/Customers/CustomerUploadFeedback.html',
                           className: 'ngdialog-theme-flat',
                           scope: $scope
                       });
                   }
                   else {
                       $scope.setSuccessFeedback(successes.length + " Customer(s) were successfully processed.");

                       $scope.ngTable.fnClearTable();
                       $scope.bulkUpload = false;
                       $scope.details = false;
                       $scope.newEdit = true;
                   }
               }
           });
        };

        $scope.goBack = function () {
            $scope.customerInfo = false;
            $scope.customerDetails = false;
            $scope.bulkUpload = false;
            $scope.details = false;
            $scope.initializeCustomer();
        };


    }]);
    
});
