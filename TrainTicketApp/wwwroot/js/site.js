// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Load the Visualization API and the corechart package.
google.charts.load('current', { 'packages': ['corechart'] });

// Set a callback to run when the Google Visualization API is loaded.
google.charts.setOnLoadCallback(drawPieChart);
google.charts.setOnLoadCallback(drawBarChart);

function drawPieChart() {

    // Create the data table.
    var data = new google.visualization.DataTable();

    data.addColumn('string', 'Jegytípus');
    data.addColumn('number', 'Eladások');

    for (var key in ticketStats)
    {
        let stat = ticketStats[key];
        data.addRow([key, parseInt(stat)]);
    }

    // Set chart options
    var options = {
        'title': 'Vásárolt jegy tipusok statisztikája',
        'width': 1080,
        'height': 560
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.PieChart(document.getElementById('pie_div'));
    chart.draw(data, options);
}

function drawBarChart() {

    // Create the data table.
    var data = new google.visualization.DataTable();

    data.addColumn('string', 'Dátum');
    data.addColumn('number', 'Eladások');

    for (var key in dayStats) {
        let stat = dayStats[key];
        data.addRow([key, parseInt(stat)]);
    }

    // Set chart options
    var options = {
        'title': 'Napi vásárlások statisztikája',
        'width': 1080,
        'height': 560
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.BarChart(document.getElementById('bar_div'));
    chart.draw(data, options);
}

function addFields() {
    let container = document.getElementById("container");
    let i = container.childElementCount / 4;

    console.log(i);

    let label = document.createElement("label");
    label.innerText = "Időpont " + (i + 1);
    container.appendChild(label);

    let time = document.createElement("input");
    time.type = "time";
    time.name = "trainTime" + i;
    time.className += "form-control mb-2 mr-sm-2";
   
    let selectList = document.createElement("select");
    selectList.id = "ticketType" + i;
    selectList.name = "ticketType" + i;
    selectList.className += "form-control mb-2 mr-sm-2"

    container.appendChild(time);
    container.appendChild(selectList);
    container.appendChild(document.createElement("br"));

    console.log(ticketTypes);

    for (var j = 0; j < ticketTypes.length; j++) {
        var option = document.createElement("option");

        console.log(ticketTypes);
        option.value = ticketTypes[j].Id;
        option.text = ticketTypes[j].Name;
        selectList.appendChild(option);
    }


}