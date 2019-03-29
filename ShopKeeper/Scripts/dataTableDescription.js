
function tableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, retrieveRecordMethodName, deleteMethodRecordName, addBtnWidth) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "bPaginate": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            var deleteStr = '<a class="bankDelTx" title="Delete" id="trf' + aData[0] + '" style="cursor: pointer" ng-click="' + deleteMethodRecordName + '(' + aData[0] + ')"><img src="/Content/images/delete.png" /></a>';
            var template = '<td style="width: 5%">' + editStr + '&nbsp;' + deleteStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}


function parentMenutableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, retrieveRecordMethodName, addBtnWidth) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "bPaginate": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            var template = '<td style="width: 5%">' + editStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function productStockTableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, bulkTemplatePath, retrieveRecordMethodName, deleteMethodRecordName, addBtnWidth) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-3"l><"col-md-3"f><"#bulkUploadLink.col-md-3"><"#newItemLnk.col-md-3">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"i><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var detailStr = '<a title="Edit" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            var deleteStr = '<a class="bankDelTx" title="Delete" id="trf' + aData[0] + '" style="cursor: pointer" ng-click="' + deleteMethodRecordName + '(' + aData[0] + ')"><img src="/Content/images/delete.png" /></a>';
            var template = '<td style="width: 5%">' + detailStr + '&nbsp;' + editStr + '&nbsp;' + deleteStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var ttx = '<label style="float: right;"><a href = "' + bulkTemplatePath + '" style="float: right; text-align:right" title="Download Bulk upload Template"><img  style="width:60px; height:47px;margin-top: 10%" src="/images/downloadExcel.png"/></a></label>';
    var kkx = $compile(ttx)($scope);

    var tth = '<label style="float: right;"><br/> <a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a><br></label>';
    var kkh = $compile(tth)($scope);

    angular.element('#bulkUploadLink').append(kkx);
    angular.element('#newItemLnk').append(kkh);

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function productStockInsertManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, bulkTemplatePath, retrieveRecordMethodName, getIStockDetails, deleteMethodRecordName, addBtnWidth) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-3"l><"col-md-3"f><"#bulkUploadLink.col-md-3"><"#newItemLnk.col-md-3">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"i><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "bPaginate": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var detailStr = '<a title="Details" style="cursor: pointer" ng-click = "' + getIStockDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            //var deleteStr = '<a class="bankDelTx" title="Delete" id="trf' + aData[0] + '" style="cursor: pointer" ng-click="' + deleteMethodRecordName + '(' + aData[0] + ')"><img src="/Content/images/delete.png" /></a>';
            var template = '<td style="width: 5%">' + editStr + '&nbsp;&nbsp;' + detailStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var ttx = '<label style="float: right;"><a href = "' + bulkTemplatePath + '" style="float: right; text-align:right" title="Download Bulk upload Template"><img  style="width:60px; height:47px;margin-top: 10%" src="/images/downloadExcel.png"/></a></label>';
    var kkx = $compile(ttx)($scope);

    var tth = '<label style="float: right;"><br/> <button ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</button><br></label>';
    var kkh = $compile(tth)($scope);

    angular.element('#bulkUploadLink').append(kkx);
    angular.element('#newItemLnk').append(kkh);

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function inventoryManager($scope, $compile, tableDirective, tableOptions, bulkEditButtonValue, handleBulkEdit, getBulkEditTemplate, getSingleEdit, getItemDetails, deleteMethodRecord, addBtnWidth) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-3"l><"col-md-3"f><"#bulkUploadLink.col-md-3"><"#newItemLnk.col-md-3">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"i><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var detailStr = '<a title="Details" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + getSingleEdit + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            var deleteStr = '<a class="bankDelTx" title="Delete" id="trf' + aData[0] + '" style="cursor: pointer" ng-click="' + deleteMethodRecord + '(' + aData[0] + ')"><img src="/Content/images/delete.png" /></a>';
            var template = '<td style="width: 15%">' + detailStr + '&nbsp;&nbsp;' + editStr + '&nbsp;&nbsp;' + deleteStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var ttx = '<label style="float: right;"><a href= "' + getBulkEditTemplate + '" style="float: right; text-align:right; cursor: pointer" title="Bulk Edit"><img  style="width:60px; height:47px;margin-top: 10%" src="/images/downloadExcel.png"/></a></label>';
    var kkx = $compile(ttx)($scope);

    var tth = '<label style="float: right;"><br/> <a ng-click = "' + handleBulkEdit + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + bulkEditButtonValue + '</a><br></label>';
    var kkh = $compile(tth)($scope);

    angular.element('#bulkUploadLink').append(kkx);
    angular.element('#newItemLnk').append(kkh);

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function tableViewManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, retrieveRecordMethodName, getItemDetails, deleteMethodRecordName, addBtnWidth) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var detailStr = '<a title="Bulk Edit" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            var deleteStr = '<a class="bankDelTx" title="Delete" id="trf' + aData[0] + '" style="cursor: pointer" ng-click="' + deleteMethodRecordName + '(' + aData[0] + ')"><img src="/Content/images/delete.png" /></a>';
            var template = '<td style="width: 15%">' + detailStr + '&nbsp;&nbsp;' + editStr + '&nbsp;&nbsp;' + deleteStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function employeeTableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, retrieveRecordMethodName, getItemDetails, addBtnWidth) {
    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var detailStr = '<a title="Details" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            var template = '<td style="width: 15%">' + detailStr + '&nbsp;&nbsp;' + editStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}


function purchaseOderTableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, retrieveRecordMethodName, getItemDetails, addBtnWidth) {
    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var details = '<a title="Details" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var editStr = '';

            if (aData[7].toLocaleLowerCase() !== 'completely delivered' && aData[7] !== 'Completely Delivered') {
                editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            }

            var template = '<td style="width: 15%">' + details + '&nbsp;&nbsp;' + editStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function employeePurchaseOderTableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, retrieveRecordMethodName, getItemDetails, addBtnWidth) {
    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var details = '<a title="Details" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var editStr = '';

            if (aData[6].toLocaleLowerCase() !== 'completely delivered' && aData[6] !== 'Completely Delivered') {
                editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            }

            var template = '<td style="width: 15%">' + details + '&nbsp;&nbsp;' + editStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function estimatesTableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, retrieveRecordMethodName, getItemDetails, addBtnWidth) {
    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var details = '<a title="Details" style="cursor: pointer" ng-click = "' + getItemDetails + '(\'' + aData[1] + '\',\'' + aData[7] + '\')"><img src="/Content/images/details.png" /></a>';
            var editStr = '';

            if (aData[7] !== 'Approved' && aData[7] !== 'Processed') {
                editStr = '<a title="Edit" id="' + aData[0] + '" style="cursor: pointer" class="bankEdTx" ng-click = "' + retrieveRecordMethodName + '(' + aData[0] + ')"><img src="/Content/images/edit.png" /></a>';
            }

            var template = '<td style="width: 5%">' + details + '&nbsp;&nbsp;' + editStr + '</td>';

            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function invoiceTableManager($scope, $compile, tableDirective, tableOptions, newRecordButtomValue, prepareTemplateMethodName, getItemDetails, addBtnWidth) {
    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var details = '<a title="Details" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[1] + ')"><img src="/Content/images/details.png" /></a>';
            var template = '<td style="width: 5%">' + details + '</td>';

            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    var tth = '<div style="margin-top: 7.5%;"><a ng-click = "' + prepareTemplateMethodName + '()" class="btnAdd btn" style="width: ' + addBtnWidth + 'px; float: right; text-align:right">' + newRecordButtomValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function purchaseOrderReceptionTableManager($scope, $compile, tableDirective, tableOptions, getItemDetails) {
    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var detailStr = '<a title="Details" style="cursor: pointer" ng-click = "' + getItemDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';

            var template = '<td style="width: 15%">' + detailStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function salesLogManager($scope, $compile, tableDirective, tableOptions, newSaleButtonValue, newSaleLink, getSaleDetails, newSaleBtnWidth) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    angular.forEach(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',
        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);

            var editStr = '<a title="Details" style="cursor: pointer" ng-click = "' + getSaleDetails + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var template = '<td style="width: 5%">' + editStr + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });


    var tth = '<div style="margin-top: 7.5%;"><a href="' + newSaleLink + '" class="btnAdd btn" style="width: ' + newSaleBtnWidth + 'px; float: right; text-align:right">' + newSaleButtonValue + '</a></div>';
    var kkh = $compile(tth)($scope);
    angular.element('#newItemLnk').append(kkh);
    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    return jTable;
}

function employeeReportTableManager($scope, $compile, tableDirective, tableOptions, getReport, employee, strDt, endDt, filterCurrency) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });


    var cllCollection = [];
    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',

        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "bPaginate": true,
        "language":
            {
                "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                    '<option value="10">10</option>' +
                    '<option value="20">20</option>' +
                    '<option value="30">30</option>' +
                    '<option value="40">40</option>' +
                    '<option value="50">50</option>' +
                    '<option value="100">100</option>' +
                    '</select><br/>'
            },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        "fnInitComplete": function (oSettings, json) {
            $scope.cllCollection = json.aaData;
            var tt = 0;
            angular.forEach(json.aaData, function (s, i) {
                var sd = s[7].replace(',', '');
                tt += parseFloat(sd);
            });
            var ftrry = filterCurrency(tt);
            angular.element('#empTt').text(ftrry);
            angular.element('#prtEmpTbl').show();

            $scope.header = '<div class="row" style="color: #000; font-size:0.8em;"><div class="col-md-8"><h5>Sales by <b>' + employee.Name + '</b> for the period <b>' + strDt + '-' + endDt + '</b></h5>' +
              '</div><div class="col-md-4"><h5 style="font-size:0.8em;">Total Sales: <b style="color: #008000;">' + ftrry + '</b></h5>' +
              '</div></div><br/>';
        },
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);
            var details = '<a title="Get Report Details" id="' + aData[0] + '" style="cursor: pointer" ng-click = "' + getReport + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var template = '<td style="width: 5%">' + details + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }
    });

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    $scope.processing = false;
    jTable.removeAttr('width').attr('width', '100%');
    $scope.empPrint = true;
    return { jTable: jTable, collection: cllCollection };
}

function outletReportTableManager($scope, $compile, tableDirective, tableOptions, getReport) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',

        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "bPaginate": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            if ($scope.outletSalesAmount === undefined || $scope.outletSalesAmount === null) {
                $scope.outletSalesAmount = parseFloat(aData[7].replace(',', ''));
            }
            else {
                $scope.outletSalesAmount += parseFloat(aData[7].replace(',', ''));
            }

            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);
            var details = '<a title="Get Report Details" id="' + aData[0] + '" style="cursor: pointer" ng-click = "' + getReport + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var template = '<td style="width: 5%">' + details + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }

    });

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    $scope.processing = false;
    jTable.removeAttr('width').attr('width', 'auto');
    return jTable;
}

function customerInvoiceReportTableManager($scope, $compile, tableDirective, tableOptions) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',

        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "bPaginate": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);
            return nRow;
        }

    });

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    $scope.processing = false;
    jTable.removeAttr('width').attr('width', 'auto');
    return jTable;
}

function productReportTableManager($scope, $compile, tableDirective, tableOptions, getReport) {

    var columnOptions = [{
        "sName": tableOptions.itemId,
        "bSearchable": false,
        "bSortable": false
    }];

    $.each(tableOptions.columnHeaders, function (i, e) {
        columnOptions.push({ 'sName': e });
    });

    var jTable = tableDirective.dataTable({
        dom: '<"row"<"#topContainer.col-md-12"<"col-md-4"l><"col-md-4"f><"#newItemLnk.col-md-4">>>rt<"#bttmContainer.row"<"col-md-12"<"col-md-4"><"col-md-8"p>>>',

        "bServerSide": true,
        sAjaxSource: tableOptions.sourceUrl,
        "bProcessing": true,
        "bPaginate": true,
        "language": {
            "lengthMenu": 'Items per Page<select id="pgLenghtInfo">' +
                '<option value="10">10</option>' +
                '<option value="20">20</option>' +
                '<option value="30">30</option>' +
                '<option value="40">40</option>' +
                '<option value="50">50</option>' +
                '<option value="100">100</option>' +
                '</select><br/>'
        },
        "sPaginationType": "full_numbers",
        aoColumns: columnOptions,
        fnRowCallback: function (nRow, aData, iDisplayIndex) {
            var oSettings = jTable.fnSettings();
            $("td:first", nRow).html(oSettings._iDisplayStart + iDisplayIndex + 1);
            var details = '<a title="Get Report Details" id="' + aData[0] + '" style="cursor: pointer" ng-click = "' + getReport + '(' + aData[0] + ')"><img src="/Content/images/details.png" /></a>';
            var template = '<td style="width: 5%">' + details + '</td>';
            var ttd = $('td:last', nRow);
            ttd.after($compile(template)($scope));
            return nRow;
        }

    });

    $('.dataTables_filter input').addClass('form-control').attr('type', 'text').css({ 'width': '80%' });
    $('.dataTables_length select').addClass('form-control');
    $scope.processing = false;
    jTable.removeAttr('width').attr('width', 'auto');
    return jTable;
}

function setBackupControlDate($scope, minDate, maxDate) {
    $scope.today = function () {
        return new Date();
    };

    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };

    $scope.toggleMin = function () {
        $scope.minDate = minDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMax = function () {
        $scope.maxDate = maxDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMin();
    $scope.toggleMax();

    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };

    $scope.dateOptions =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.formats = ['dd/MM/yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];
}

function setTime($scope, minDate, maxDate) {
    $scope.today = function () {
        return new Date();
    };

    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };

    $scope.toggleMin = function () {
        $scope.minDate = minDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMax = function () {
        $scope.maxDate = maxDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMin();
    $scope.toggleMax();

    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };

    $scope.dateOptions =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.timeOptions =
     {
         readonlyInput: false,
         showMeridian: true
     };

    $scope.formats = ['HH:MM'];
    $scope.format = $scope.formats[0];
}

function setControlDate($scope, minDate, maxDate) {
    $scope.today = function () {
        return new Date();
    };

    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };

    $scope.toggleMin = function () {
        $scope.minDate = minDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMax = function () {
        $scope.maxDate = maxDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMin();
    $scope.toggleMax();

    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };

    $scope.dateOptions =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.formats = ['dd/MM/yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];
}

function setEndDate($scope, minDate, maxDate) {
    $scope.today = function () {
        return new Date();
    };

    $scope.today();

    $scope.clearEndDate = function () {
        $scope.expiryDate = null;
    };

    $scope.toggleEndMin = function () {
        $scope.minEndDate = minDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleEndMax = function () {
        $scope.maxEndDate = maxDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleEndMin();
    $scope.toggleEndMax();

    $scope.openEnDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.endDateOpened = true;
    };

    $scope.endDateOptions =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.endDateFormats = 'dd/MM/yyyy';
    $scope.endDateformat = $scope.endDateFormats;
}

function setMaxDate($scope, minDate, maxDate) {
    $scope.today = function () {
        return new Date();
    };

    $scope.today();

    $scope.clearEndDate = function () {
        $scope.expiryDate = null;
    };

    // Disable weekend selection
    //$scope.disabled = function (date, mode) {
    //    return (mode === 'day' && (date.getDay() === 0 || date.getDay() === 6));
    //};

    $scope.toggleEndMin = function () {
        $scope.minEndDate = minDate;
    };

    $scope.toggleEndMax = function () {
        $scope.maxEndDate = maxDate;
    };

    $scope.toggleEndMin();
    $scope.toggleEndMax();

    $scope.openEnDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.endDateOpened = true;
    };

    $scope.endDateOptions =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.endDateFormats = ['dd/MM/yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.endDateformat = $scope.endDateFormats[0];
}

function setMaxDateWithWeekends($scope, minDate, maxDate) {
    $scope.today = function () {
        return new Date();
    };

    $scope.today();

    $scope.clearEndDatep = function () {
        $scope.expiryDatep = null;
    };

    $scope.toggleEndMinp = function () {
        $scope.minEndDatep = minDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleEndMaxp = function () {
        $scope.maxEndDatep = maxDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleEndMinp();
    $scope.toggleEndMaxp();

    $scope.openEnDatep = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.endDateOpenedp = true;
    };

    $scope.endDateOptionsp =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.endDateFormatsp = 'dd/MM/yyyy';
    $scope.endDateformatp = $scope.endDateFormatsp;
}


function setExpiryDate($scope, minDate, maxDate) {
    $scope.maxToday = function () {
        return new Date();
    };

    $scope.maxToday();

    $scope.clearEndExpDate = function () {
        $scope.expDate = null;
    };

    $scope.toggleMinExpDate = function () {
        $scope.minExpDate = minDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMaxExpDate = function () {
        $scope.maxExpDate = maxDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleMinExpDate();
    $scope.toggleMaxExpDate();

    $scope.openExpDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.expDateOpened = true;
    };

    $scope.expDateOptions =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.expDateFormat = 'dd/MM/yyyy';
}


function setEndDateWithWeekends($scope, minDate, maxDate) {
    $scope.today = function () {
        return new Date();
    };

    $scope.today();

    $scope.clearEndDate = function () {
        $scope.expiryDate = null;
    };

    $scope.toggleEndMin = function () {
        $scope.minEndDate = minDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleEndMax = function () {
        $scope.maxEndDate = maxDate; //$scope.minDate ? null : new Date();
    };

    $scope.toggleEndMin();
    $scope.toggleEndMax();

    $scope.openEnDate = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.endDateOpened = true;
    };

    $scope.endDateOptions =
    {
        formatYear: 'yyyy',
        startingDay: 1
    };

    $scope.endDateFormats = 'dd/MM/yyyy';
    $scope.endDateformat = $scope.endDateFormats;
}