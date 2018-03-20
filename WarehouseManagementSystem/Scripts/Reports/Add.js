$(document).ready(function (e) {
    $(document).on('dragover', '.cell.table-cell', function (e) {
        allowDrop(e);
    });
    $(document).on('drop', '.cell.table-cell', function (e) {
        drop(e);
    });
    $(document).on('dragstart', '.column-container .column-name', function (e) {
        drag(e);
    });
    $(document).on('dblclick ', '#ReportInteractionContainer .cell.table-cell p', function (e) {
        $(e.target).remove();
    });
    $(document).on('click ', '#JoinSelector .container-icon-close svg', function (e) {
        resetJoinSelector();
    });
    $(document).on('click', '#ReportInteractionTab', function (e) {
        document.getElementById('ReportCode').setAttribute('style', 'display:none');
        document.getElementById('ReportCodeTab').classList.remove('active');
        document.getElementById('ReportInteractionContainer').setAttribute('style', 'display:inline-block');
        document.getElementById('ReportInteractionTab').classList.add('active');
    });
    $(document).on('click', '#ReportCodeTab', function (e) {
        document.getElementById('ReportInteractionContainer').setAttribute('style', 'display:none');
        document.getElementById('ReportInteractionTab').classList.remove('active');
        document.getElementById('ReportCode').setAttribute('style', 'display:inline-block');
        document.getElementById('ReportCodeTab').classList.add('active');
    });
    $(document).on('click', '#BtnTestQuery', function (e) {
        insertTrailingSpaces();
        testQuery();
    });
    $(document).on('click', '#BtnAddReport', function (e) {
        insertTrailingSpaces();
        saveReport();
    });

    window.SqlQueryBuilder = new SqlQueryBuilder();
});

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.originalEvent.dataTransfer.setData("columnDrag", ev.target.id);

}

function drop(ev) {
    ev.preventDefault();
    var eleInitiatorId = ev.originalEvent.dataTransfer.getData("columnDrag");
    var clonedEle = $('#' + eleInitiatorId).clone()[0];
    ev.target.appendChild(clonedEle);

    var ele = $(document.getElementById(eleInitiatorId)).closest('.column-container')[0];
    var column = new Column();
    column = column.createFromElement(ele);

    var selectedTable = new Table();
    selectedTable._tableId = column._tableId;
    selectedTable._tableName = column._tableName;


    if (window.SqlQueryBuilder._tables.length == 0) {
        window.SqlQueryBuilder.setTables(selectedTable);
    }
    else if (!window.SqlQueryBuilder.doesTableExist(selectedTable)) {

        window.SqlQueryBuilder.setTables(selectedTable);

        //need to figure out which potential foreign keys will be available for user to select from
        var potentialJoins = window.SqlQueryBuilder.getPotentialJoins(selectedTable._tableId);

        displayJoinSelector();
        populateJoinSelector(potentialJoins);
    }


    //window.SqlQueryBuilder.setColumns(column);
}


class SqlQueryBuilder {
    constructor() {
        this._columns = [];
        this._tables = [];
        this._joins = [];
    }

    get getJoins() {
        return this._joins;
    }
    setJoins(join) {
        if (join instanceof Join) {
            this._joins.push(join);
        }
    }
    get getColumns() {
        return this._columns;
    }
    setColumns(column) {
        if (column instanceof Column) {
            if (!doesJoinExist(column)) {

                //setJoins()
            }
            this._columns.push(column);
        }
    }
    get getTables() {
        return this._tables();
    }
    setTables(table) {
        this._tables.push(table);
    }
    doesJoinExist(sourceTableId, sourceColumnId, targetTableId, targetColumnId) {
        for (let i = 0; i < this._joins.length; i++) {
            if (this._joins[i]._sourceTable._tableId == sourceTableId) {
                if (this._joins[i]._sourceColumn._columnId == sourceColumnId) {
                    if (this._joins[i]._targetTable._tableId == targetTableId) {
                        if (this_joins[i].targetColumn._columnId == targetColumnId) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    removeColumn(tableId, columnId) {
        for (let i = 0; i < this._columns.length; i++) {
            if (this._columns[i]._tableId == tableId) {
                if (this._columns[i]._columnId == columnId) {
                    this._columns.splice(i, 1);
                }
            }
        }
    }
    getForeignKeyColumn(column) {
        var tables = serverTableModels;

        for (let i = 0; i < serverTableModels.length; i++) {
            var serverTable = serverTableModels[i];
            if (serverTable.Id == foreignKey._ParentTable._tableId) {
                for (let j = 0; j < serverTable.Columns.length; j++) {
                    var serverColumn = serverTable.Columns[j];
                    if (serverColumn.ParentColumnId == foreignKey._ParentTable._parentColumnId) {
                        var ForeignKey = new ForeignKey();
                        var parentTable = new Table();
                        parentTable._tableId = serverTable.TableId;
                        parentTable._tableName = serverTable.Name;

                        var childTable = new Table();

                    }
                }
            }
        }
    }
    getPotentialJoins(childTableId) {
        var joins = [];

        var serverChildTable = this.getServerTableModelById(childTableId);

        for (let i = 0; i < serverChildTable.Columns.length; i++) {
            var serverChildColumn = serverChildTable.Columns[i];

            //only select foreign keys from server tables
            if (serverChildColumn.IsForeignkey) {

                //get parentTable 
                var serverParentTable = this.getServerTableModelById(serverChildColumn.ForeignKey.ParentTable.Id);
                //only return the potential joinable table if it is already in the query.  E.G. we cannot join using users table if it isn't in the query
                if (this.doesTableExist(serverParentTable.Id)) {
                    var join = new Join();
                    var sourceTable = new Table();
                    sourceTable._tableId = serverParentTable.Id;
                    sourceTable._tableName = serverParentTable.Name;
                    var sourceColumn = new Column();
                    sourceColumn._columnId = serverChildColumn.ForeignKey.ParentColumnId;
                    sourceColumn._columnName = this.getColumnById(this.getServerTableModelById(sourceTable._tableId), serverChildColumn.ForeignKey.ParentColumnId).Name;

                    join._sourceTable = sourceTable;
                    join._sourceColumn = sourceColumn;
                    join._targetTable = serverChildTable;
                    join._targetColumn = serverChildColumn;

                    joins.push(join);
                }
            }
        }
        return joins;
    }
    getServerTableModelById(tableId) {
        for (let i = 0; i < serverTableModels.length; i++) {
            if (serverTableModels[i].Id == tableId) {
                return serverTableModels[i];
            }
        }
        return;
    }
    doesTableExist(tableId) {
        for (let i = 0; i < this._tables.length; i++) {
            if (tableId == this._tables[i]._tableId) {
                return true;
            }
        }
        return false;
    }
    getColumnById(table, Id) {
        for (let i = 0; i < table.Columns.length; i++) {
            var serverColumn = table.Columns[i];
            if (serverColumn.Id == Id) {
                return serverColumn;
            }
        }
        return;
    }

}
class Table {
    constructor(tableId, tableName, columns) {
        this._tableId = tableId;
        this._tableName = tableName;
        this._columns = [];
    }
}
class Column extends Table {
    constructor(tableId, tableName, columnId, columnName, isForeignKey, foreignKey) {
        super(tableId, tableName);
        this._columnId = columnId;
        this._columnName = columnName;
        this._IsForeignKey = isForeignKey;
        this._ForeignKey = foreignKey;
    }
    createFromElement(ele) {
        var column = new Column();

        column._columnId = ele.dataset.columnId;
        column._columnName = ele.dataset.columnName;
        column._tableName = ele.dataset.tableName;
        column._tableId = ele.dataset.tableId;
        column._IsForeignKey = false

        if (ele.classList.contains("foreign-key")) {
            column._IsForeignKey = true;
            column._ForeignKey = new ForeignKey();
            var parentTable = new Table();
            parentTable._tableId = ele.dataset.parentTableId;
            parentTable._tableName = ele.dataset.parentTableName;
            var childTable = new Table();
            childTable._tableId = ele.dataset.childTableId;
            childTable._tableName = ele.dataset.childTableName;
            column._ForeignKey._ParentTable = parentTable;
            column._ForeignKey._ChildTable = childTable;
            column._ForeignKey.childColumnId = ele.dataset.childTableColumnId;
            column._ForeignKey.parentColumnId = ele.dataset.parentTableColumnId;


        }
        return column;
    }
}
class Join {
    constructor(sourceTable, sourceColumn, targetTable, targetColumn) {
        this._sourceTable = sourceTable;
        this._sourceColumn = sourceColumn;
        this._targetTable = targetTable;
        this._targetColumn = targetColumn;
    }
}
class ForeignKey {
    constructor(parentTable, childTable, childColumnId, parentColumnId) {
        this._ParentTable = parentTable;
        this._ChildTable = childTable;
        this._childColumnId = childColumnId;
        this._parentColumnId = parentColumnId;
    }

}

function blurElements(elementsToBlur) {
    for (let i = 0; i < elementsToBlur.length; i++) {
        $(elementsToBlur[i]).css('filter', 'blur(3px)');
    }
}
function unblurElements(elementsToBlur) {
    for (let i = 0; i < elementsToBlur.length; i++) {
        $(elementsToBlur[i]).css('filter', 'blur(0px)');
    }
}
function resetJoinSelector() {
    document.getElementById('JoinSelector').setAttribute('style', 'display: none');
    document.getElementById('JoinSelector').dataset.foreignKeys = "";
    document.getElementById('JoinTable').innerHTML = "";
    document.getElementById('JoinColumn').innerHTML = "";
    document.getElementById('JoinSelector').getElementsByClassName('container-confirm')[0].dataset.foreignKey = "";
    var elesTounblur = [];
    elesTounblur.push(document.getElementsByTagName('nav')[0]);
    elesTounblur.push(document.getElementsByTagName('main')[0]);
    elesTounblur.push(document.getElementsByTagName('footer')[0]);
    unblurElements(elesTounblur);
}
function displayJoinSelector() {
    document.getElementById('JoinSelector').setAttribute('style', 'display:inline-block');
    var elesToBlur = [];
    elesToBlur.push(document.getElementsByTagName('nav')[0]);
    elesToBlur.push(document.getElementsByTagName('main')[0]);
    elesToBlur.push(document.getElementsByTagName('footer')[0]);
    blurElements(elesToBlur);
}
function populateJoinSelector(joins) {
    document.getElementById('JoinSelector').dataset.foreignKeys = JSON.stringify(joins);
    document.getElementById('JoinSelector').getElementsByClassName('container-confirm')[0].dataset.foreignKey = JSON.stringify(joins[0]);
    document.getElementById('JoinTable').innerHTML = joins[0]._sourceTable._tableName;
    document.getElementById('JoinColumn').innerHTML = joins[0]._sourceColumn._columnName;
}

function testQuery() {
    let sqlQuery = quill.getText();
    ajaxTestQuery(sqlQuery);
}

function saveReport() {
    var reportName = document.getElementById('ReportCreationReportName').value;
    var reportQuery = quill.getText();
    ajaxSaveReport(reportName, reportQuery);
}
function ajaxSaveReport(reportName, reportQuery) {
    let data = JSON.stringify({ 'reportName': reportName, 'reportQuery': reportQuery});
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/Reports/SaveReport",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                alert("Finished Success");
            }
            else {
                alert(response.responseText);
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}

function insertTrailingSpaces() {
    var totalLineLength = 0;
    quill.getContents().eachLine(function (line, attributes, i) {
        var lineLength = quill.getLine(i)[0].cachedText.split('\n')[i].length
        totalLineLength += lineLength;
        if (quill.getText(totalLineLength-1, 1) != " ") {
            quill.insertText(totalLineLength, " ", "Format", "Value", "api");
            totalLineLength += 2;
        }
    })
}