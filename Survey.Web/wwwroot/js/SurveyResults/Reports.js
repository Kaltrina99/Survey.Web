let reports = document.querySelectorAll(".questionchart");
let selectchart = document.querySelectorAll(".selectchart");

function Initial() {
    for (var i = 0; i < reports.length; i++) {
        let chartdiv = reports[i].querySelectorAll(".chart")[0];
        let chartData = getQuestionData(reports[i]);
        let chartid = chartdiv.getAttribute("id");
        ColumChart(chartid, chartData);
    }
}

function getQuestionData(element)
{
    let chartdatainput = element.querySelectorAll("input");
    let chartdata = [];
    for (let j = 0; j < chartdatainput.length; j++) {
        let questionname = chartdatainput[j].getAttribute("name");
        let questionscore = chartdatainput[j].value;
        let data = { "Question": questionname, "Score": questionscore }
        chartdata.push(data);
    }
    return chartdata;
}
function PieChart(id, data)
{
    am4core.ready(function () {

        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end

        // Create chart instance
        var chart = am4core.create(id, am4charts.PieChart);

        // Add data
        chart.data = data;

        // Add and configure Series
        var pieSeries = chart.series.push(new am4charts.PieSeries());
        pieSeries.dataFields.value = "Score";
        pieSeries.dataFields.category = "Question";
        pieSeries.slices.template.stroke = am4core.color("#fff");
        pieSeries.slices.template.strokeOpacity = 1;

        // This creates initial animation
        pieSeries.hiddenState.properties.opacity = 1;
        pieSeries.hiddenState.properties.endAngle = -90;
        pieSeries.hiddenState.properties.startAngle = -90;

        chart.hiddenState.properties.radius = am4core.percent(0);


    }); // end am4core.ready()
}
function DonutChart(id,data) {

    am4core.ready(function () {

        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end

        // Create chart instance
        var chart = am4core.create(id, am4charts.PieChart);

        // Add data
        chart.data = data;

        // Set inner radius
        chart.innerRadius = am4core.percent(50);

        // Add and configure Series
        var pieSeries = chart.series.push(new am4charts.PieSeries());
        pieSeries.dataFields.value = "Score";
        pieSeries.dataFields.category = "Question";
        pieSeries.slices.template.stroke = am4core.color("#fff");
        pieSeries.slices.template.strokeWidth = 2;
        pieSeries.slices.template.strokeOpacity = 1;

        // This creates initial animation
        pieSeries.hiddenState.properties.opacity = 1;
        pieSeries.hiddenState.properties.endAngle = -90;
        pieSeries.hiddenState.properties.startAngle = -90;

    });
}
function HorizontalColumn(id, data)
{


    am4core.useTheme(am4themes_animated);

    // Create chart instance
    var chart = am4core.create(id, am4charts.XYChart);

    // Add data
    chart.data = data;

    var categoryAxis = chart.yAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "Question";
    categoryAxis.renderer.grid.template.location = 0;

    var valueAxis = chart.xAxes.push(new am4charts.ValueAxis());

    var series = chart.series.push(new am4charts.ColumnSeries());
    series.dataFields.valueX = "Score";
    series.dataFields.categoryY = "Question";
}
function ColumChart(id,data)
{
    am4core.ready(function () {

        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end

        // Create chart instance
        var chart = am4core.create(id, am4charts.XYChart);

        // Add data
        chart.data = data;


        var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
        categoryAxis.dataFields.category = "Question";
        categoryAxis.renderer.grid.template.location = 0;
        categoryAxis.renderer.minGridDistance = 30;

        categoryAxis.renderer.labels.template.adapter.add("dy", function (dy, target) {
            if (target.dataItem && target.dataItem.index & 2 == 2) {
                return dy + 25;
            }
            return dy;
        });

        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

        // Create series
        var series = chart.series.push(new am4charts.ColumnSeries());
        series.dataFields.valueY = "Score";
        series.dataFields.categoryX = "Question";
        series.name = "Score";
        series.columns.template.tooltipText = "{categoryX}: [bold]{valueY}[/]";
        series.columns.template.fillOpacity = .8;

        var columnTemplate = series.columns.template;
        columnTemplate.strokeWidth = 2;
        columnTemplate.strokeOpacity = 1;

    }); // end am4core.ready()
}
Initial();

selectchart.forEach(x => {
    x.addEventListener("change", (event) => {
        let selected = event.target.value;
        let container = event.target.parentNode.parentNode.parentNode;
        
        let chart = container.querySelector(".chart");
        let id =chart.getAttribute("id");
        let data = getQuestionData(container);
        if (selected == 1)
        {
            ColumChart(id,data)
        }
        else if (selected == 2)
        {
            HorizontalColumn(id, data)
        }
        else if (selected == 3)
        {
            PieChart(id, data)
        }
        else if (selected == 4)
        {
            DonutChart(id,data)
        }
       

    })
})