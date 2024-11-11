var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }

});

function loadDataTable(status) {
    debugger
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: 'productId', "width": "20%" },
            { data: 'title', "width": "23%" },
            { data: 'location', "width": "15%" },
            { data: 'startingPrice', "width": "7%" },
            { data: 'categoryName', "width": "8%" },
            {
                data: 'productId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2">
            <i class="bi bi-pencil-square"></i>
        </a>
        <a href="/admin/order/delete?orderId=${data}" class="btn btn-danger mx-2">
            <i class="bi bi-trash"></i>
        </a>
    </div>`
                },
                "width": "10%"
            }
        ]
    });
}