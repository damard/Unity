/*
 * Provides the javascript code to render metrics data.
 * 
 * Depends on JQuery Sparklines
 */

LVCGame.metricsSettings = {};
LVCGame.metricsSettings.refreshDelay = 1000;

LVCGame.metricsSettings.tickDurationHistory = {};
LVCGame.metricsSettings.tickDurationHistory.precision = 1;
LVCGame.metricsSettings.tickDurationHistory.options = {};
LVCGame.metricsSettings.tickDurationHistory.options.type = 'line';
LVCGame.metricsSettings.tickDurationHistory.options.width = "100%";
LVCGame.metricsSettings.tickDurationHistory.options.height = "64px";
LVCGame.metricsSettings.tickDurationHistory.options.lineColor = '#A0522D';
LVCGame.metricsSettings.tickDurationHistory.options.fillColor = false;
LVCGame.metricsSettings.tickDurationHistory.options.chartRangeMin = 0;
LVCGame.metricsSettings.tickDurationHistory.options.normalRangeMin = 0;
LVCGame.metricsSettings.tickDurationHistory.options.normalRangeColor = "#F5F1C2";
LVCGame.metricsSettings.tickDurationHistory.options.maxSpotColor = "#FF0000";
LVCGame.metricsSettings.tickDurationHistory.options.minSpotColor = "";
LVCGame.metricsSettings.tickDurationHistory.options.spotColor = '';
LVCGame.metricsSettings.tickDurationHistory.options.spotRadius = 3;

LVCGame.metricsSettings.pluginTickDurationHistory = {};
LVCGame.metricsSettings.pluginTickDurationHistory.precision = 1;
LVCGame.metricsSettings.pluginTickDurationHistory.colorList = [
    '#000080',    //navy
    '#008000',    //green
    '#808000',    //olive
    '#008080',    //teal
    '#0000FF',    //blue
    '#00FF00',    //lime
    '#800080',    //purple
    '#FF00FF',    //fuchsia
    '#800000',    //maroon
    '#00FFFF'    //aqua
];
LVCGame.metricsSettings.pluginTickDurationHistory.options = {};
LVCGame.metricsSettings.pluginTickDurationHistory.options.type = 'line';
LVCGame.metricsSettings.pluginTickDurationHistory.options.width = "100%";
LVCGame.metricsSettings.pluginTickDurationHistory.options.height = "64px";
LVCGame.metricsSettings.pluginTickDurationHistory.options.lineColor = '#A0522D';
LVCGame.metricsSettings.pluginTickDurationHistory.options.fillColor = false;
LVCGame.metricsSettings.pluginTickDurationHistory.options.chartRangeMin = 0;
LVCGame.metricsSettings.pluginTickDurationHistory.options.minSpotColor = "";
LVCGame.metricsSettings.pluginTickDurationHistory.options.maxSpotColor = "#FF0000";
LVCGame.metricsSettings.pluginTickDurationHistory.options.spotColor = '';
LVCGame.metricsSettings.pluginTickDurationHistory.options.spotRadius = 3;

LVCGame.metricsSettings.tickTimeAllocation = {};
LVCGame.metricsSettings.tickTimeAllocation.precision = 1;
LVCGame.metricsSettings.tickTimeAllocation.drColor = "#00D800";
LVCGame.metricsSettings.tickTimeAllocation.pluginColor = "#0000FF";
LVCGame.metricsSettings.tickTimeAllocation.evtProcessingColor = "#FF0000";

LVCGame.metricsSettings.tickTimeAllocation.options = {};

LVCGame.metricsSettings.tickTimeAllocation.options.type = 'pie';
LVCGame.metricsSettings.tickTimeAllocation.options.width = "120px";
LVCGame.metricsSettings.tickTimeAllocation.options.height = "120px";
LVCGame.metricsSettings.tickTimeAllocation.options.offset = 0;
LVCGame.metricsSettings.tickTimeAllocation.options.sliceColors = [  
    LVCGame.metricsSettings.tickTimeAllocation.drColor,
    LVCGame.metricsSettings.tickTimeAllocation.pluginColor,
    LVCGame.metricsSettings.tickTimeAllocation.evtProcessingColor
];

LVCGame.metricsSettings.entityCreatedHistory = {};
LVCGame.metricsSettings.entityCreatedHistory.collapse = 100; // Collapse all the data into 100 columns
LVCGame.metricsSettings.entityCreatedHistory.precision = 1;
LVCGame.metricsSettings.entityCreatedHistory.periodElemId = '#entityCreatedEventsPeriod';
LVCGame.metricsSettings.entityCreatedHistory.graphSummary = '#entityCreatedSectionHistoryDataCell';
LVCGame.metricsSettings.entityCreatedHistory.graphElemId = '#entityCreatedHistoryGraph';
LVCGame.metricsSettings.entityCreatedHistory.options = {};
LVCGame.metricsSettings.entityCreatedHistory.options.type = 'bar';
LVCGame.metricsSettings.entityCreatedHistory.options.barWidth = 5;
LVCGame.metricsSettings.entityCreatedHistory.options.barSpacing = 2;
LVCGame.metricsSettings.entityCreatedHistory.options.width = "100%";
LVCGame.metricsSettings.entityCreatedHistory.options.height = "64px";
LVCGame.metricsSettings.entityCreatedHistory.options.barColor = '#000080';
LVCGame.metricsSettings.entityCreatedHistory.options.fillColor = false;

LVCGame.metricsSettings.entityUpdatedHistory = {};
LVCGame.metricsSettings.entityUpdatedHistory.collapse = 100; // Collapse all the data into 100 columns
LVCGame.metricsSettings.entityUpdatedHistory.precision = 1;
LVCGame.metricsSettings.entityUpdatedHistory.periodElemId = '#entityUpdatedEventsPeriod';
LVCGame.metricsSettings.entityUpdatedHistory.graphSummary = '#entityUpdatedSectionHistoryDataCell';
LVCGame.metricsSettings.entityUpdatedHistory.graphElemId = '#entityUpdatedHistoryGraph';
LVCGame.metricsSettings.entityUpdatedHistory.options = {};
LVCGame.metricsSettings.entityUpdatedHistory.options.type = 'bar';
LVCGame.metricsSettings.entityUpdatedHistory.options.barWidth = 5;
LVCGame.metricsSettings.entityUpdatedHistory.options.barSpacing = 2;
LVCGame.metricsSettings.entityUpdatedHistory.options.width = "100%";
LVCGame.metricsSettings.entityUpdatedHistory.options.height = "64px";
LVCGame.metricsSettings.entityUpdatedHistory.options.barColor = '#000080';
LVCGame.metricsSettings.entityUpdatedHistory.options.fillColor = false;


LVCGame.metricsSettings.entityDeletedHistory = {};
LVCGame.metricsSettings.entityDeletedHistory.collapse = 100; // Collapse all the data into 100 columns
LVCGame.metricsSettings.entityDeletedHistory.precision = 1;
LVCGame.metricsSettings.entityDeletedHistory.periodElemId = '#entityDeletedEventsPeriod';
LVCGame.metricsSettings.entityDeletedHistory.graphSummary = '#entityDeletedSectionHistoryDataCell';
LVCGame.metricsSettings.entityDeletedHistory.graphElemId = '#entityDeletedHistoryGraph';
LVCGame.metricsSettings.entityDeletedHistory.options = {};
LVCGame.metricsSettings.entityDeletedHistory.options.type = 'bar';
LVCGame.metricsSettings.entityDeletedHistory.options.barWidth = 5;
LVCGame.metricsSettings.entityDeletedHistory.options.barSpacing = 2;
LVCGame.metricsSettings.entityDeletedHistory.options.width = "100%";
LVCGame.metricsSettings.entityDeletedHistory.options.height = "64px";
LVCGame.metricsSettings.entityDeletedHistory.options.barColor = '#000080';
LVCGame.metricsSettings.entityDeletedHistory.options.fillColor = false;

LVCGame.metricsSettings.weaponFireHistory = {};
LVCGame.metricsSettings.weaponFireHistory.collapse = 100; // Collapse all the data into 100 columns
LVCGame.metricsSettings.weaponFireHistory.precision = 1;
LVCGame.metricsSettings.weaponFireHistory.periodElemId = '#weaponFireEventsPeriod';
LVCGame.metricsSettings.weaponFireHistory.graphSummary = '#weaponFireSectionHistoryDataCell';
LVCGame.metricsSettings.weaponFireHistory.graphElemId = '#weaponFireHistoryGraph';
LVCGame.metricsSettings.weaponFireHistory.options = {};
LVCGame.metricsSettings.weaponFireHistory.options.type = 'bar';
LVCGame.metricsSettings.weaponFireHistory.options.barWidth = 5;
LVCGame.metricsSettings.weaponFireHistory.options.barSpacing = 2;
LVCGame.metricsSettings.weaponFireHistory.options.width = "100%";
LVCGame.metricsSettings.weaponFireHistory.options.height = "64px";
LVCGame.metricsSettings.weaponFireHistory.options.barColor = '#000080';
LVCGame.metricsSettings.weaponFireHistory.options.fillColor = false;

LVCGame.metricsSettings.detonationHistory = {};
LVCGame.metricsSettings.detonationHistory.collapse = 100; // Collapse all the data into 100 columns
LVCGame.metricsSettings.detonationHistory.precision = 1;
LVCGame.metricsSettings.detonationHistory.periodElemId = '#detonationEventsPeriod';
LVCGame.metricsSettings.detonationHistory.graphSummary = '#detonationSectionHistoryDataCell';
LVCGame.metricsSettings.detonationHistory.graphElemId = '#detonationHistoryGraph';
LVCGame.metricsSettings.detonationHistory.options = {};
LVCGame.metricsSettings.detonationHistory.options.type = 'bar';
LVCGame.metricsSettings.detonationHistory.options.barWidth = 5;
LVCGame.metricsSettings.detonationHistory.options.barSpacing = 2;
LVCGame.metricsSettings.detonationHistory.options.width = "100%";
LVCGame.metricsSettings.detonationHistory.options.height = "64px";
LVCGame.metricsSettings.detonationHistory.options.barColor = '#000080';
LVCGame.metricsSettings.detonationHistory.options.fillColor = false;



LVCGame.updateTickTimeAllocation = function(averageTickDuration, averageDRDuration, averagePluginDuration)
{
    var evtProcessing = averageTickDuration - (averageDRDuration + averagePluginDuration);
    var options = LVCGame.metricsSettings.tickTimeAllocation.options;

    $('#tickTimeAllocationGraph').sparkline( [averageDRDuration, averagePluginDuration, evtProcessing], options);
    $("#tickPluginTimeAllocation").text(averageTickDuration.toFixed(LVCGame.metricsSettings.tickTimeAllocation.precision) + " ms");
    $("#tickDRTimeAllocation").text(averageDRDuration.toFixed(LVCGame.metricsSettings.tickTimeAllocation.precision) + " ms");
    $("#tickEvtProcessingTimeAllocation").text(evtProcessing.toFixed(LVCGame.metricsSettings.tickTimeAllocation.precision) + " ms");
    $("#tickTimeAllocation").text(  "Average Tick Time: " + 
                                    averageTickDuration.toFixed(LVCGame.metricsSettings.tickTimeAllocation.precision) + " ms");
};

LVCGame.updateTickDurationHistory = function(tickDurationHistory, minTick, maxTick, averageTick, period)
{
    var options = LVCGame.metricsSettings.tickDurationHistory.options;
    options.chartRangeMax = 1.2 * averageTick;
    options.normalRangeMax = averageTick;
    
    $('#tickDurationHistory').sparkline( tickDurationHistory, options);
    var precision = LVCGame.metricsSettings.tickDurationHistory.precision;
    var content = [ "Period: ", (period / 1000).toFixed(precision),
                    "s; Min: ", minTick.toFixed(precision), 
                    "ms; Max:", maxTick.toFixed(precision), 
                    "ms; Average:", averageTick.toFixed(precision), "ms"];
    $('#tickDurationHistoryDataCell').text(content.join(' '));
};

LVCGame.updatePluginTickDurationHistory = function(pluginDurationHistory, period)
{
    var history = {};           // A map of arrays keyed against the plugin
    var allPluginStats = {};    // A map of maps keyed against the plugin
    /* {"DIS": {"min" : xx, "max": xx, "sum": xx},
            *   "HLA-13":{"min":xx, "max":xx, "sum":xx}}
            */
    var instanceHistory; // An object of the plugin tick times at a single tick instance
    var pluginHistory;
    var pluginStats;
    
    // Pull out the very last insertion to get list of current plugins
    var lastInserted = pluginDurationHistory[pluginDurationHistory.length-1];
    // For each tick cycle,
    for(var p=0; p<pluginDurationHistory.length; p++)
    {
        instanceHistory = pluginDurationHistory[p];
    
        // For each plugin,
        for(var key in lastInserted)
        {
			if (lastInserted.hasOwnProperty(key)) 
			{
				pluginHistory = history[key];
				pluginStats = allPluginStats[key];
				if(instanceHistory[key]  === undefined)
				{
					instanceHistory[key] = 0;
				} 	  
				   
				if(!pluginHistory)
				{
					pluginHistory = [];
					history[key] = pluginHistory;
					
					pluginStats = {};
					pluginStats.min = 65535;
					pluginStats.max = 0;
					pluginStats.sum = 0;
					allPluginStats[key] = pluginStats;
				}
				
				// Store the plugin tick duration.
				pluginHistory.push(instanceHistory[key]);
				pluginStats.min = Math.min(pluginStats.min, instanceHistory[key]);
				pluginStats.max = Math.max(pluginStats.max, instanceHistory[key]);
				pluginStats.sum = pluginStats.sum + instanceHistory[key];
			}
        }
        
    }
    
    var legendBuilder = [];
    
    var options = LVCGame.metricsSettings.pluginTickDurationHistory.options;
    options.composite = false;  // The first graph wipes all the old graphs. The subsequent graphs are composited on top.
    var index = 0;
    var precision = LVCGame.metricsSettings.pluginTickDurationHistory.precision;
    for(var pluginName in history)
    {
	    if (history.hasOwnProperty(pluginName)) 
		{
			options.lineColor = LVCGame.metricsSettings.pluginTickDurationHistory.colorList[index];
			$('#pluginTickDurationHistory').sparkline( history[pluginName], options);
			options.composite = true;
			
			pluginStats = allPluginStats[pluginName];
			legendBuilder.push("<tr><td style=background-color:");
			legendBuilder.push(options.lineColor);
			legendBuilder.push("></td><td class=\"text-right\">");
			legendBuilder.push(pluginName);
			legendBuilder.push("</td><td class=\"text-right\">");
			legendBuilder.push(pluginStats.min.toFixed(precision));
			legendBuilder.push(" ms</td><td class=\"text-right\">");
			legendBuilder.push(pluginStats.max.toFixed(precision));
			legendBuilder.push(" ms</td><td class=\"text-right\">");
			legendBuilder.push((pluginStats.sum / history[pluginName].length).toFixed(precision));
			legendBuilder.push(" ms</td></tr>");

			// Cycle through the colours.
			index = (index + 1) % LVCGame.metricsSettings.pluginTickDurationHistory.colorList.length;
		}
    }
    $('#pluginTickTimePeriod').text("Period: " + (period/1000).toFixed(precision) + " s");
    $('#pluginTickTimeHistorySection tbody').empty().append(legendBuilder.join(''));
};

LVCGame.updateBarChart = function(history, period, optionsKey)
{
    var barChartConfig = LVCGame.metricsSettings[optionsKey];
    var periodSecs = period/1000.0;
    var precision = LVCGame.metricsSettings[optionsKey].precision;
    $(barChartConfig.periodElemId).text(periodSecs.toFixed(precision));
    
    var collapse = barChartConfig.collapse;
    if(history.length < collapse)
    {
        // Too little data
        $(barChartConfig.graphSummary).text("Insufficient Data");
        return;
    }
    
    var dataPointsPerColumn = Math.floor(history.length / collapse);
    var collapsedData = [];
    var eventCount = 0;
    var sum = 0;
    var max = 0;
    for(var k = 0; k < collapse; k++)
    {
        sum = 0;
        for(var w = 0; w < dataPointsPerColumn; w++)
        {
            sum = sum + history[(k * dataPointsPerColumn) + w];
        }
        collapsedData[k] = sum;
        max = Math.max(sum, max);
        eventCount = eventCount + sum;
    }


    var options = barChartConfig.options;
    options.chartRangeMin = 0;
    options.chartRangeMax = max;

    $(barChartConfig.graphElemId).sparkline( collapsedData, options);
    var rate = (eventCount / periodSecs).toFixed(precision);
    $(barChartConfig.graphSummary).text("Max: " + max + " Events/Tick; Average " + rate + " Events/Second");
};

LVCGame.metricsUpdateCallback = function(data)
{
    var tickDurationHistory = [];
    var pluginDurationHistory = [];
    
    var entityCreatedHistory = [];
    var entityUpdatedHistory = [];
    var entityDeletedHistory = [];
    var weaponFireHistory = [];
    var detonationHistory = [];
    
    var maxTickTime = 0;
    var minTickTime = 65535;
    var tickTime = 0;
    var drTime = 0;
    var pluginTime = 0;
    
    var totalTickTime = 0;              // Sum of tickEnd - tickStart (ms)
    var totalDRTime = 0;                // Sum of drEnd - drStart (ms)
    var totalPluginTime = 0;            // Sum of pluginEnd - pluginStart (ms)
    
	// update the entity count
	$("#uptime").text(data.LVCGame.uptime);
	$("#entityCount").text(data.LVCGame.entityCount);
	$("#otherCount").text(data.LVCGame.otherCount);
	$("#landCount").text(data.LVCGame.landCount);
	$("#airCount").text(data.LVCGame.airCount);
	$("#surfaceCount").text(data.LVCGame.surfaceCount);
	$("#subsurfaceCount").text(data.LVCGame.subsurfaceCount);
	$("#spaceCount").text(data.LVCGame.spaceCount);

	// update graphs
    for(var i=0; i<data.Metrics.length; i++)
    {
        tickTime = (data.Metrics[i].tickEnd - data.Metrics[i].tickStart) / 1000;
        drTime = (data.Metrics[i].drEnd - data.Metrics[i].drStart) / 1000;
        pluginTime = (data.Metrics[i].pluginEnd - data.Metrics[i].pluginStart) / 1000;
        
        totalTickTime = totalTickTime + tickTime;
        totalDRTime = totalDRTime + drTime;
        totalPluginTime = totalPluginTime + pluginTime;
        
        tickDurationHistory.push(tickTime);
        maxTickTime = Math.max(maxTickTime, tickTime);
        minTickTime = Math.min(minTickTime, tickTime);
        
        pluginDurationHistory.push(data.Metrics[i].pluginTickDuration);
        entityCreatedHistory.push(data.Metrics[i].entityCreatedCount);
        entityUpdatedHistory.push(data.Metrics[i].entityUpdateCount);
        entityDeletedHistory.push(data.Metrics[i].entityDeletedCount);
        weaponFireHistory.push(data.Metrics[i].weaponFireCount);
        detonationHistory.push(data.Metrics[i].detonationCount);
    }
    var period = (data.Metrics[data.Metrics.length-1].created - data.Metrics[0].created ) / 1000;
    
    var averageTickDuration = totalTickTime / data.Metrics.length;
    var averageDRDuration = totalDRTime / data.Metrics.length;
    var averagePluginDuration = totalPluginTime / data.Metrics.length;
    LVCGame.updateTickTimeAllocation(averageTickDuration, averageDRDuration, averagePluginDuration);
    LVCGame.updateTickDurationHistory(tickDurationHistory, minTickTime, maxTickTime, averageTickDuration, period);
    //LVCGame.updatePluginTickDurationHistory(pluginDurationHistory, period);
    
    LVCGame.updateBarChart(entityCreatedHistory, period, 'entityCreatedHistory');
    LVCGame.updateBarChart(entityUpdatedHistory, period, 'entityUpdatedHistory');
    LVCGame.updateBarChart(entityDeletedHistory, period, 'entityDeletedHistory');
    
    LVCGame.updateBarChart(weaponFireHistory, period, 'weaponFireHistory');
    LVCGame.updateBarChart(detonationHistory, period, 'detonationHistory');
    
    LVCGame.metricsUpdateInteval = setTimeout(LVCGame.updateMetrics,  LVCGame.metricsSettings.refreshDelay);
};

 
LVCGame.updateMetrics = function()
{
    $.getJSON("/metrics/", LVCGame.metricsUpdateCallback);
};
 
$(document).ready(function()
{
    $.ajaxSetup({ cache: false });
    $("#tickPluginTimeAllocationColor").css("background-color", LVCGame.metricsSettings.tickTimeAllocation.pluginColor);
    $("#tickDRTimeAllocationColor").css("background-color", LVCGame.metricsSettings.tickTimeAllocation.drColor);
    $("#tickEvtProcessingTimeAllocationColor").css("background-color", LVCGame.metricsSettings.tickTimeAllocation.evtProcessingColor);
    LVCGame.updateMetrics();
});
