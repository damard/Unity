LVCGame.toggleSelectAllCheckBox = function(elem)
{
    var jInputs = $("input[type='checkbox']");
    var state = $(elem).attr('checked');
    jInputs.attr('checked', state);
    return false;
};

LVCGame.getUrlParams = function()
{
    var qsParm = {};
    var query = window.location.search.substring(1);
    var parms = query.split('&');
    for (var i=0; i<parms.length; i++) 
    {
        var pos = parms[i].indexOf('=');
        if (pos > 0) 
        {
            var key = parms[i].substring(0,pos);
            var val = parms[i].substring(pos+1);
            qsParm[key] = val;
        }
    }
    return qsParm;
};

LVCGame.reloadWithParam = function(newParams)
{
    var urlParams = LVCGame.getUrlParams();
    for(var key in newParams)
    {
        // Update the parameter map
        if(newParams[key] === null)
        {
            delete urlParams[key];
        }
        else
        {
            urlParams[key] = newParams[key];
        }
    }
    document.location.search = $.param(urlParams);
};

$(document).ready(function(){
    $("#search").keypress(function(evt){
        // If the return key was pressed.
        if(evt.which == 13)
        {
            var newParams = {};
            var search = $("#search").val();
            if($.trim(search).length === 0)
            {
                newParams.filter = null;
            }
            else
            {
                newParams.filter = search;
            }
            LVCGame.reloadWithParam(newParams);
        }
    });
    var filterVal = LVCGame.getUrlParams().filter;
    if((filterVal !== null) && (filterVal !== undefined))
    {
        filterVal = filterVal.replace(/\+/g, ' ');
        $("#search").val(decodeURIComponent(filterVal));
    }
    
    // Take out the "Search..." when people want to start typing
     $("#search").focus(function()
	{    
		if( $("#search").val() == "Search...")
		{
			$("#search").val("");
		}
	});

	// bring "Search..." back in when no string entered
	$("#search").blur(function()
	{    
		if( $("#search").val() == "")
		{
			$("#search").val("Search...");
		}
	});
});

LVCGame.refreshFinished = function()
{
	//$.get(document.location, {});
	document.location = document.location;
};


LVCGame.refreshGameTypeList = function()
{
	url = "refresh/";
	
	// Hide refresh button
	$("#refreshAllGameTypesButton").fadeOut("fast", function()
	{ 
		$("#loading-indicator").fadeIn("fast");
	});
	
	$.post(url, {}, LVCGame.refreshFinished);

};

