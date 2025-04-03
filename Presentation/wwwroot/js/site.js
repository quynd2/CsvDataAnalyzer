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
        "searching": true,
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
            chartInstance = new Chart(ctx, {
                type: 'line',
                data: chartData,
                options: {
                    scales: {
                        x: {
                            ticks: {
                                color: '#e0e0e0'
                            }
                        },
                        y: {
                            ticks: {
                                color: '#e0e0e0'
                            }
                        }
                    },
                    plugins: {
                        legend: {
                            labels: {
                                color: '#e0e0e0'
                            }
                        }
                    }
                }
            });
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
    var loader = document.getElementById('loader');
    loader.style.display = 'block';

    if (!fileInput) {
        $("#uploadError").text('Please select a file').show();
        loader.style.display = 'none';
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
            loader.style.display = 'none';
            if (response.success) {
                alert('File uploaded successfully.');
                location.reload();
            } else {
                $('#uploadError').text(response.message).show();
            }
        },
        error: function (xhr, status, error) {
            loader.style.display = 'none';
            alert('Error uploading file: ' + error);
        }
    });
}
