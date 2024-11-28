var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable1("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable1("completed");
        }
        else {
            if (url.includes("purchased")) {
                loadDataTable1("purchased");
            }
            else {
                if (url.includes("rejected")) {
                    loadDataTable("rejected");
                }
                else {
                    loadDataTable("pending");
                }
            }
        }
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status,
            type: 'GET',  // Ensure you're using the correct HTTP method
            dataType: 'json', // Ensure you're expecting JSON response
            error: function (xhr, error, thrown) {
                // Handle AJAX error
                console.error("Error fetching data:", thrown);
                console.log("XHR Response:", xhr.responseText);

                // Show a friendly error message if there's an issue
                $('#tblData').html('<tr><td colspan="6" class="text-center">Error fetching data. Please try again later.</td></tr>');
            },
            dataSrc: function (json) {
                // Check if data is empty or null and handle accordingly
                if (!json || !json.data || json.data.length === 0) {
                    // Show a message for no data available
                    $('#tblData').html('<tr><td colspan="6" class="text-center">No data available for this status.</td></tr>');
                    return [];  // Return empty array to prevent DataTables from displaying incorrect data
                }
                return json.data; // Return the valid data
            }
        },
        "columns": [
            { data: 'title', "width": "17%" },
            { data: 'location', "width": "17%" },
            { data: 'startingPrice', "width": "13%" },
            { data: 'listingDate', "width": "12%" },
            { data: 'aunctionEndDate', "width": "12%" },
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
                    </div>`;
                },
                "width": "10%"
            }
        ],
        "initComplete": function (settings, json) {
            // Check if no data was returned and handle accordingly
            if (json.data && json.data.length === 0) {
                $('#tblData').html('<tr><td colspan="6" class="text-center">No data available for this status.</td></tr>');
            }
        }
    });
}

function loadDataTable1(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status,
            type: 'GET',
            dataType: 'json',
            error: function (xhr, error, thrown) {
                // Handle AJAX error
                console.error("Error fetching data:", thrown);
                console.log("XHR Response:", xhr.responseText);

                $('#tblData').html('<tr><td colspan="6" class="text-center">Error fetching data. Please try again later.</td></tr>');
            },
            dataSrc: function (json) {
                // Check if data is empty or null and handle accordingly
                if (!json || !json.data || json.data.length === 0) {
                    // Show a message for no data available
                    $('#tblData').html('<tr><td colspan="6" class="text-center">No data available for this status.</td></tr>');
                    return [];  // Return empty array to prevent DataTables from displaying incorrect data
                }
                return json.data; // Return the valid data
            }
        },
        "columns": [
            { data: 'title', "width": "17%" },
            { data: 'location', "width": "17%" },
            { data: 'startingPrice', "width": "13%" },
            { data: 'listingDate', "width": "12%" },
            { data: 'aunctionEndDate', "width": "12%" },
            { data: 'categoryName', "width": "8%" },
            {
                data: 'productId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/Admin/Order/Purchased?productId=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>
                        </a>
                       
                    </div>`;
                },
                "width": "10%"
            }
        ],
        "initComplete": function (settings, json) {
            // Check if no data was returned and handle accordingly
            if (json.data && json.data.length === 0) {
                $('#tblData').html('<tr><td colspan="6" class="text-center">No data available for this status.</td></tr>');
            }
        }
    });
}
