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
});

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.originalEvent.dataTransfer.setData("text", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    var data = ev.originalEvent.dataTransfer.getData("text");
    var clonedEle = $('#' + data).clone()[0];
    ev.target.appendChild(clonedEle);

}
