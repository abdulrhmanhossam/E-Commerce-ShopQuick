const ctx = document.getElementById('myChart');

$.ajax({
    type: "POST",
    url: "/Order/getSalesData",
    data: "",
    dataType: "json",
    success: salesChart,
    error: "onError"
});

function salesChart(dataSales) {
    var barColor = ["red", "blue", "green", "orange", "brown", "purple", "black"];
    const ctx = document.getElementById('myChart');

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: dataSales[0],
            datasets: [{
                backgroundColor: barColor,
                data: dataSales[1],
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
    $.ajax({
        type: "POST",
        url: "/Order/getPopularProductList",
        data: "",
        dataType: "json",
        success: ProductsChart,
        error: "onError"
    });
}

function ProductsChart(data) {
    var barColor = ["red", "blue", "green", "orange", "brown", "purple", "black"];
    const ctx = document.getElementById('myChart1');

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data[0],
            datasets: [{
                labels: 'Popular Items',
                backgroundColor: barColor,
                data: data[1],
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
    $.ajax({
        type: "POST",
        url: "/Order/getTopBuyerList",
        data: "",
        dataType: "json",
        success: BuyersChart,
        error: "onError"
    });
}

//function BuyersChart(data) {
//    var barColor = ["red", "blue", "green", "orange", "brown", "purple", "black"];
//    const ctx = document.getElementById('myChart2');

//    new Chart(ctx, {
//        type: 'polarArea',
//        data: {
//            labels: data[0],
//            datasets: [{
//                labels: 'Popular Items',
//                backgroundColor: barColor,
//                data: data[1],
//                borderWidth: 1
//            }]
//        },
//        options: {
//            scales: {
//                y: {
//                    beginAtZero: true
//                }
//            }
//        }
//    });
//}