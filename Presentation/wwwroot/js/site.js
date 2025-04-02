var chartInstance;

document.getElementById('filterButton').addEventListener('click', function () {
    var fromDate = document.getElementById('fromDate').value;
    var toDate = document.getElementById('toDate').value;
    window.location.href = '?fromDate=' + fromDate + '&toDate=' + toDate;
});

function initializeDataTable() {
    $('#dataTable').DataTable({
        "bInfo": false,
        "paging": false,
        "searching": false,
        "ordering": true
    });
}

function changePage(page) {
    var fromDate = document.getElementById('fromDate').value;
    var toDate = document.getElementById('toDate').value;
    window.location.href = '?page=' + page + '&fromDate=' + fromDate + '&toDate=' + toDate;
}

function showChart() {
    var fromDate = document.getElementById('fromDate').value;
    var toDate = document.getElementById('toDate').value;

    $.ajax({
        url: 'Home/GetChartData',
        type: 'GET',
        data: { fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $('#chartModal').modal('show');
            var ctx = document.getElementById('dataChart').getContext('2d');
            var chartData = {
                labels: data.labels,
                datasets: [{
                    label: 'Values',
                    data: data.values,
                    borderColor: 'blue',
                    fill: false
                }, {
                    label: 'Min',
                    data: Array(data.values.length).fill(data.minimum),
                    borderColor: 'green',
                    borderDash: [5, 5],
                    fill: false
                }, {
                    label: 'Max',
                    data: Array(data.values.length).fill(data.maximum),
                    borderColor: 'red',
                    borderDash: [5, 5],
                    fill: false
                }, {
                    label: 'Average',
                    data: Array(data.values.length).fill(data.average),
                    borderColor: 'orange',
                    borderDash: [5, 5],
                    fill: false
                }]
            };

            if (chartInstance) {
                chartInstance.destroy();
            }
            chartInstance = new Chart(ctx, { type: 'line', data: chartData });
        }
    });
}

$('#chartModal').on('hidden.bs.modal', function () {
    if (chartInstance) {
        chartInstance.destroy();
        chartInstance = null;
    }
});

function uploadFile() {
    var formData = new FormData();
    var fileInput = $("#csvFile")[0].files[0];

    if (!fileInput) {
        $("#uploadError").text('Please select a file').show();
        return;
    }
    formData.append("csvFile", fileInput);

    $.ajax({
        url: '/Home/Upload',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                $('#uploadError').text(response.message).show();
            }
        },
        error: function (xhr, status, error) {
            alert('Error uploading file: ' + error);
        }
    });
}
