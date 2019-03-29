
"use strict";
require.config({
    baseUrl: "",

    // alias libraries paths
    paths:
        {
            'application-configuration': 'scripts/application-configuration',
            'ngIdle': 'Scripts/angular-idle.min',
            'angular': 'scripts/angular.min',
            'angucomplete': 'Scripts/angucomplete',
            'ngCookies': 'Scripts/angular-cookies.min',
            'ngAnimate': 'Scripts/angular-animate.min',
            'angular-szn-autocomplete': 'Scripts/angular-szn-autocomplete',
            'datePicker': 'Scripts/datePicker',
            'ui.bootstrap.datetimepicker': 'Scripts/datetime-picker',
            'ui.select': 'Scripts/select.min',
            'angular-route': 'scripts/angular-route',
            'angularAMD': 'scripts/angularAMD',
            'ui-bootstrap': 'scripts/ui-bootstrap-tpls-0.11.2.min',
            'blockUI': 'scripts/angular-block-ui',
            'angularFileUpload': 'scripts/angular-file-upload.min',
            'ngload': 'scripts/ngload',
            'bootstrap': 'scripts/bootstrap.min',
            'jDataTable': 'scripts/jquery.dataTables',
            'jDataTableBuilder': 'Scripts/dataTableDescription',
            'ngDialog': 'scripts/ngDialog',
            'fileReader': 'scripts/fileReader',
            'ng-currency': 'scripts/ng-currency',
            'ngStorage': 'scripts/ngStorage.min',
            'ngLocale': 'Scripts/i18n/angular-locale_en-ng',
            'ui.utils.masks': 'scripts/masks',


            /*           STORE SERVICES     */
            'storeBankServices': 'ng-shopkeeper/services/Store/storeBankServices',
            'storeCountryServices': 'ng-shopkeeper/services/Store/storeCountryServices',
            'storeStateServices': 'ng-shopkeeper/services/Store/storeStateServices',
            'storeCityServices': 'ng-shopkeeper/services/Store/storeCityServices',
            'storeCurrencyServices': 'ng-shopkeeper/services/Store/storeCurrencyServices',
            'storeSubscriptionPackageServices': 'ng-shopkeeper/Store/services/subscriptionPackageServices',
            'productTypeServices': 'ng-shopkeeper/services/Store/productTypeServices',
            'deliveryMethodServices': 'ng-shopkeeper/services/Store/deliveryMethodServices',
            'ajaxService': 'ng-shopkeeper/services/ajaxServices',
            'alertsService': 'ng-shopkeeper/services/alertsServices',
            'storePaymentGatewayServices': 'ng-shopkeeper/services/Store/storePaymentGatewayServices',
            'storeDepartmentServices': 'ng-shopkeeper/services/Store/storeDepartmentServices',
            'productBrandServices': 'ng-shopkeeper/services/Store/productBrandServices',
            'customerTypeServices': 'ng-shopkeeper/services/Store/customerTypeServices',
            'documentTypeServices': 'ng-shopkeeper/services/Store/documentTypeServices',
            'productServices': 'ng-shopkeeper/services/Store/productServices',
            'productCategoryServices': 'ng-shopkeeper/services/Store/productCategoryServices',
            'productVariationServices': 'ng-shopkeeper/services/Store/productVariationServices',
            'productVariationValueServices': 'ng-shopkeeper/services/Store/productVariationValueServices',
            'supplierServices': 'ng-shopkeeper/services/Store/supplierServices',
            'unitsOfMeasurementServices': 'ng-shopkeeper/services/Store/unitsOfMeasurementServices',
            'storeTransactionTypeServices': 'ng-shopkeeper/services/Store/storeTransactionTypeServices',
            'storeTransactionServices': 'ng-shopkeeper/services/Store/storeTransactionServices',
            'chartOfAccountServices': 'ng-shopkeeper/services/Store/chartOfAccountServices',
            'accountGroupServices': 'ng-shopkeeper/services/Store/accountGroupServices',
            'storeOutletServices': 'ng-shopkeeper/services/Store/storeOutletServices',
            'storeSettingsServices': 'ng-shopkeeper/services/Store/storeSettingsServices',
            'storeItemStockServices': 'ng-shopkeeper/services/Store/storeItemStockServices',
            'couponServices': 'ng-shopkeeper/services/Store/couponServices',
            'imageViewServices': 'ng-shopkeeper/services/Store/imageViewServices',
            'itemPriceServices': 'ng-shopkeeper/services/Store/itemPriceServices',
            'personServices': 'ng-shopkeeper/services/Store/personServices',
            'employeeServices': 'ng-shopkeeper/services/Store/employeeServices',
            'saleServices': 'ng-shopkeeper/services/Store/saleServices',
            'registerServices': 'ng-shopkeeper/services/Store/registerServices',
            'welcomeServices': 'ng-shopkeeper/services/Store/welcomeServices',
            'salesReportServices': 'ng-shopkeeper/services/Store/salesReportServices',
            'customerServices': 'ng-shopkeeper/services/Store/customerServices',
            'purchaseOrderServices': 'ng-shopkeeper/services/Store/purchaseOrderServices',
            'estimateServices': 'ng-shopkeeper/services/Store/estimateServices',
            'parentMenuServices': 'ng-shopkeeper/services/Store/parentMenuServices',
            'dashboardServices': 'ng-shopkeeper/services/Store/dashboardServices',
            'issueTypeServices': 'ng-shopkeeper/services/Store/issueTypeServices',
            'transferNoteServices': 'ng-shopkeeper/services/Store/transferNoteServices',


            /*                          ACCOUNT                                        */

            'accountService': '/ng-shopkeeper/Services/accountService',

            'angular-sanitize': 'scripts/angular-sanitize.min'
        },


    // Add angular modules that does not support AMD out of the box, put it in a shim
    shim: {
        'fileupload-shim': ['angular'],
        'fileupload': ['angular'],
        'angularAMD': ['angular'],
        'angular-route': ['angular'],
        'blockUI': ['angular'],
        'angular-sanitize': ['angular'],
        'ui-bootstrap': ['angular'],
        'ngDialog': ['angular'],
        'angularFileUpload': ['angular'],
        'ngAnimate': ['angular'],
        'fileReader': ['angular'],
        'datePicker': ['angular'],
        'ui.select': ['angular'],
        'ui.bootstrap.datetimepicker': ['angular'],
        'ng-currency': ['angular'],
        'ngLocale': ['angular'],
        'ngSanitize': ['angular'],
        'ngStorage': ['angular'],
        'ngIdle': ['angular'],
        'ngCookies': ['angular'],
        'ui.utils.masks': ['angular'],
        'angucomplete': ['angular']
    },
    // kick start application
    deps: ['application-configuration']
});
