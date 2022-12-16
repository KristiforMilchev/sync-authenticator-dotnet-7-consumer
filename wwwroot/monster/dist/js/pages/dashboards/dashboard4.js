// -------------------------------------------------------------------------------------------------------------------------------------------
// Dashboard 4 : Chart Init Js
// -------------------------------------------------------------------------------------------------------------------------------------------
$(function () {
  "use strict";
   

   var option_Total_Visits = {
        series: [{
            name: '',
            data: [0, 5, 6, 10, 9, 12, 4, 9]
        }
        ],
        chart: {
            type: 'bar',
            height: 50,
            width: 70,
            offsetX: -20,
            toolbar: {
                show: false,
            },
            sparkline: {
                enabled: true
            },
        },
       colors: ["#7460ee"],
        grid: {
            show: false,
        },
        plotOptions: {
            bar: {
                horizontal: false,
                startingShape: 'flat',
                endingShape: 'flat',
                columnWidth: '70%',
                barHeight: '100%',
            },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            width: 3,
            show: true,
            colors: ['transparent']
        },
        xaxis: {
            axisBorder: {
                show: false,
            },
            axisTicks: {
                show: false,
            },
            labels: {
                show: false,
            },
        },
        yaxis: {
            labels: {
                show: false,
            },
        },
        axisBorder: {
            show: false,
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            theme: "dark",
            style: {
                fontSize: '12px',
                fontFamily: 'Poppins,sans-serif',
            },
            x: {
                show: false,
            },
            y: {
                formatter: undefined,
            }
        }
    };

    var chart_column_basic = new ApexCharts(document.querySelector("#total-visits"), option_Total_Visits);
    chart_column_basic.render();
     
  });
