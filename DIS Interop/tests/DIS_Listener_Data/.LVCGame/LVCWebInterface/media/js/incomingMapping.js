
LVCGame.GameTypeSelection = {};


LVCGame.addIncomingMappings = function(defaultEnum)
{
    if( LVCGame.addIncomingMappingWindow === null || 
        LVCGame.addIncomingMappingWindow === undefined || 
        LVCGame.addIncomingMappingWindow.closed === true)
    {
        var url = null;
        if(defaultEnum === null || defaultEnum === undefined)
        {
            url = "/gamemappings/incoming/add/";
        }
        else
        {
            var params = {};
            params.lvcType = defaultEnum;
            url = "/gamemappings/incoming/add/?"+$.param(params);
        }
        LVCGame.addIncomingMappingWindow = window.open( url, "", "toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=no,copyhistory=no,width=600,height=300");
    }
    LVCGame.addIncomingMappingWindow.focus();
};


/**
  * POSTS all selected outgoing mappings to the deletion url.
  */
LVCGame.deleteIncomingMappings = function()
{
    var jInputs = $("input[type='checkbox']:checked.rowselector");
    if(jInputs.length > 0)
    {
        var answer = false;
        
        if(jInputs.length == 1)
        {
            answer = confirm("Are you sure you want to delete this mapping?");
        }
        else
        {
            answer = confirm("Are you sure you want to delete these " + jInputs.length + " mappings?");
        }
        
        if(answer === true)
        {
            var jCurrent = null;
            var post = {};
            for( var i=0; i<jInputs.length; i++)
            {
                jCurrent = $(jInputs[i]);
                post[jCurrent.attr("name")] = jCurrent.val();
            }
            
            $.post("/gamemappings/incoming/delete/", post, function(){
                document.location = document.location;
            });
        }
    }
};

LVCGame.doGameTypeSelection = function(noSpaceLVCType, lvcType)
{
	// Cache current item for which selection is for
	LVCGame.GameTypeSelection.noSpaceLVCType = noSpaceLVCType;
	
	
	// Popup dialog for game type selection
	$("#GameTypeSelection").dialog('option', 'title', 'Select Game Type for "' +lvcType +'"').dialog('open');

};

LVCGame.setSelectedGameType = function(selectedName, selectedType)
{
	// Piss off dialog
	$("#GameTypeSelection").dialog('close');
	
	// Now we find our corresponding node and set its value!
	$("#name_"+LVCGame.GameTypeSelection.noSpaceLVCType).text(selectedName);
	$("#details_"+LVCGame.GameTypeSelection.noSpaceLVCType).text(selectedType);
	$("#input_"+LVCGame.GameTypeSelection.noSpaceLVCType).attr('value', selectedType);

};