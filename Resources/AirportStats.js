$(function() { 
	$("#dialog").dialog({ 
		autoOpen: false, 
		modal: false, 
		position: { my: "left top", at: "left+150 top+40"}, 
		resizable: false 
	});
	$("#infodialog").dialog({ 
		autoOpen: false, 
		modal: false, 
		position: { my: "left top", at: "left+150 top+40"}, resizable: false 
	});
	$(".info-dialog").on("click", function(e) { 
		e.preventDefault(); 
		$("#infodialog").html(""); 
		$("#infodialog").dialog("option", "title", "Loading...").dialog("open"); 
		$("#infodialog").dialog("option", "width", 500 ); 
		$("#infodialog").dialog("option", "height", "auto" );
		$("#infodialog").load(this.href, function() { 
			$(this).dialog("option", "title", $(this).find("h1").text()); 
			$(this).find("h1").remove(); 
		}); 
	}); 
	$(".ajax-dialog").on("click", function(e) { 
		e.preventDefault(); 
		$("#dialog").html(""); 
		$("#dialog").dialog("option", "title", "Loading...").dialog("open"); 
		$("#dialog").dialog("option", "maxWidth", 350 ); 
		$("#dialog").dialog("option", "width", "auto" );
		$("#dialog").dialog("option", "height", "auto" );
		$("#dialog").load(this.href, function() { 
			$(this).dialog("option", "title", $(this).find("h1").text()); 
			$(this).find("h1").remove(); 
		}); 
	}); 
	$(".loadChart").on("click", function (e) { 
		e.preventDefault(); 
		$("#dialog").html(""); 
		$("#dialog").dialog("option", "title", "Loading...").dialog("open");
		$("#dialog").dialog("option", "width", 1000 ); 
		$("#dialog").dialog("option", "height", "auto" );										
		var jqxhr = $.getJSON(this.href, function(){})
		.done(function(jsonData){
			const rawChartOptions = jsonData.chartOptions[0];
			$("#dialog").dialog("option", "title", rawChartOptions.title);
			const rawSeriesConfig = jsonData.seriesConfig[0];
			const rawChartData = jsonData.chartData[0]; 
			const labels = Object.keys(rawChartData); 
			const seriesData = Object.values(rawChartData);
			var canvas = document.createElement('canvas'); 
			var multiAxis = false;
			canvas.id = "myCanvas"; 
			Chart.defaults.global.legend.position = "right"; 
			Chart.defaults.global.defaultColor = "#CCC"; 
			Chart.defaults.global.defaultFontColor = "#CCC"; 
			Chart.defaults.global.defaultFontSize = 10; 
			Chart.defaults.global.datasets.backgroundColor = "#223747";
			Chart.defaults.global.elements.point.backgroundColor = "#223747";
			Chart.defaults.scale.gridLines.color = "#CCC";
			var arr = []; 
			if(rawChartOptions.type == "stackedBar"){ 
				for(var i=0; i<rawSeriesConfig.seriesLabels.length; i++) {
					arr.push({ 
						label: rawSeriesConfig.seriesLabels[i], 
						data: seriesData.map(o => o[rawSeriesConfig.seriesKeys[i]]), 
						backgroundColor: rawSeriesConfig.seriesColors[i], 
						borderColor: '#CCC',
						borderWidth: 1,
						order: rawSeriesConfig.seriesOrders[i], 
						stack: rawSeriesConfig.seriesStacks[i], 
						fill: false 
					});
				} 
				rawChartOptions.type = "bar" 
			}else if(rawChartOptions.type == "multiAxisLine"){
				for(var i=0; i<rawSeriesConfig.seriesLabels.length; i++) {
					arr.push({ 
						label: rawSeriesConfig.seriesLabels[i], 
						data: seriesData.map(o => o[rawSeriesConfig.seriesKeys[i]]), 
						borderColor: rawSeriesConfig.seriesColors[i], 
						pointHoverBackgroundColor: rawSeriesConfig.seriesColors[i], 
						order: rawSeriesConfig.seriesOrders[i], 
						yAxisID: rawSeriesConfig.seriesYAxis[i],
						pointHitRadius: 10,
						fill: false 
					});
				} 
				multiAxis = true;
				rawChartOptions.type = "line"
			} else {
				for(var i=0; i<rawSeriesConfig.seriesLabels.length; i++) {
					arr.push({ 
						label: rawSeriesConfig.seriesLabels[i], 
						data: seriesData.map(o => o[rawSeriesConfig.seriesKeys[i]]), 
						borderColor: rawSeriesConfig.seriesColors[i], 
						pointHoverBackgroundColor: rawSeriesConfig.seriesColors[i], 
						order: rawSeriesConfig.seriesOrders[i], 
						pointHitRadius: 10,
						fill: false 
					});
				} 
			} 
			var myChart = new Chart(canvas, { 
				type: rawChartOptions.type, 
				data: { 
					labels: labels, 
					datasets: arr, 
				}, 
				options: rawChartOptions.options 
			}); 
			div = document.getElementById('dialog');
			div.appendChild(canvas); 
			if(rawChartOptions.money != false) { 
				myChart.options.scales.yAxes[0].ticks.callback = function(value, index, values) {return rawChartOptions.money + value.toLocaleString();}; 
				myChart.options.tooltips.callbacks.label = function (tooltipItem, data) { var datasetLabel = data.datasets[tooltipItem.datasetIndex].label || ''; return datasetLabel + ": " + rawChartOptions.money + tooltipItem.yLabel.toLocaleString();}; 
				myChart.options.tooltips.callbacks.title = function (tooltipItem, data) {return rawChartOptions.day + " " + tooltipItem[0].xLabel;};
				myChart.update(); 
			} else { 
				myChart.options.scales.yAxes[0].ticks.callback = function(value, index, values) {return value.toLocaleString();}; 
				myChart.options.tooltips.callbacks.label = function (tooltipItem, data) { var datasetLabel = data.datasets[tooltipItem.datasetIndex].label || ''; return datasetLabel + ": " + tooltipItem.yLabel.toLocaleString();};
				myChart.options.tooltips.callbacks.title = function (tooltipItem, data) {return rawChartOptions.day + " " + tooltipItem[0].xLabel;};
				myChart.update(); 
			}
			if(multiAxis == true){
				myChart.options.scales.yAxes[1].beforeUpdate = function(scale) {
					var nLeftTickCount = scale.chart.scales['yAxisLeft'].ticks.length - 1;
					var rightMaxValue = Math.max.apply(Math, scale.chart.config.data.datasets[2].data);
					var rightIncValue = Math.ceil(rightMaxValue / nLeftTickCount);
					scale.chart.options.scales.yAxes[1].ticks.stepSize = rightIncValue;
					if(rightMaxValue != 0){
						scale.chart.options.scales.yAxes[1].ticks.max = rightIncValue * Math.abs(nLeftTickCount);
					} else {
					scale.chart.options.scales.yAxes[1].ticks.max = 1;
					}
					return;
				}
				myChart.options.scales.yAxes[2].beforeUpdate = function(scale) {
					var nLeftTickCount = scale.chart.scales['yAxisLeft'].ticks.length - 1;
					var rightMaxValue = Math.max.apply(Math, scale.chart.config.data.datasets[4].data);
					var rightIncValue = Math.ceil(rightMaxValue * 5 / nLeftTickCount) / 5;
					scale.chart.options.scales.yAxes[2].ticks.stepSize = rightIncValue;
					if(rightMaxValue != 0){
						scale.chart.options.scales.yAxes[2].ticks.max = rightIncValue * Math.abs(nLeftTickCount);
					} else {
					scale.chart.options.scales.yAxes[2].ticks.max = 1;
					}
					return;
				}
			}
		})
		.fail(function(jqxhr, textStatus, errorThrown) {
			console.error(jqxhr, textStatus, errorThrown);
			console.error(jqxhr.responseJSON);
		}); 
	});
});									