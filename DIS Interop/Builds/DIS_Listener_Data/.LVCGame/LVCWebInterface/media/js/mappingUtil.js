/**
 * The various components of the DIS Enumeration. 
 */
LVCGame.ENUMERATION_COMPONENTS = ["kind", "domain", "country", "category", "subcategory", "specific", "extra"];

/**
 * This variable acts as a synchronised lock that restarts everytime a key entered. It is used to
 * maintain the time counter for gamemapping listbox autopopulate 
 */
var userEntryTimer;

/**
 * Keep record of the last GameType field focused. Used to populate values selected from the gamemapping listbox
 */
var currentGameTypeField;

/**
 * This function will validate and format the inputted LVC Type (Enumeration)
 *
 * @return an object with two parameters. ret["isError"] = [true|false]; ret["message"] = the_error_message;
 */
LVCGame.validateLvcTypeChanged = function(evt)
{
    var jElem = $(evt.target);
    var value = $.trim(jElem.val());
    
    var isError = false;
    var message = null;
    
    //var splitEnum = value.split(" ");
    var temp = value.split(" ");
    var splitEnum = [];
    for(var p=0; p<temp.length; p++)
    {
        if($.trim(temp[p]).length > 0)
        {
            splitEnum.push(temp[p]);
        }
    }
    
    formattedEnum = [];
    if(splitEnum.length == LVCGame.ENUMERATION_COMPONENTS.length)
    {
        var currentEnum;
        for(var i=0; i<LVCGame.ENUMERATION_COMPONENTS.length && !isError; i++)
        {
            currentEnum = parseInt(splitEnum[i], 10);
            if(isNaN(currentEnum))
            {
                isError = true;
                message = "Component \""+LVCGame.ENUMERATION_COMPONENTS[i]+"\" is not a number.";
            }
            else if(currentEnum < 0)
            {
                // FAIL!
                isError = true;
                message = "Component \""+LVCGame.ENUMERATION_COMPONENTS[i]+"\" must be greater or equal to zero.";
            }
            else
            {
                if(i==2)
                {
                    // Country - 16 bits
                    if(currentEnum > 65534)
                    {
                        isError = true;
                        message = "Country component cannot be greater than 65534.";
                    }
                    else
                    {
                        formattedEnum.push(("   " + currentEnum).slice(-3));
                    }
                }
                else
                {
                    // Everything else - 8 bits
                    if(currentEnum > 255)
                    {
                        isError = true;
                        message = "Component \""+LVCGame.ENUMERATION_COMPONENTS[i]+"\" cannot be greater than 127.";
                    }
                    else
                    {
                        formattedEnum.push(("   " + currentEnum).slice(-3));
                    }
                }
            }
        }
    }
    else if(value.length === 0 || value == '-')
    {
        // Defined 'delete' values
        isError = false;
        formattedEnum.push('-');
    }
    else
    {
        isError = true;
        message = "Not enough components. Required Components are:\n[Kind Domain Country Category SubCategory Specific Extra]";
    }
    
    var ret = {};
    ret.isError = isError;
    ret.message = message;
    
    if(!isError)
    {
        jElem.val(formattedEnum.join(' '));
    }
    
    return ret;
};

 /** 
  * When a user makes focus on a textfield, either a new field or one previously selected, populate the 
  * gameMappings list box with the value already entered.
  */
 LVCGame.populateGameMappingFilterList = function(evt)
 {
	// keep a record of the last gametype selected. This is important for selected items from the drop down list
	currentGameTypeField = "#" + evt.target.id;
	
	// populate the mappinglist drop down list when a textfield is clicked
	LVCGame.sendPopulationRequest(evt.target.value);
 };
 
 /**
  * Initialises the filter/population request to the LVCGame server. This method is accessible
  * from the autopopulate timer or when a user focuses on a textfield
  */
 LVCGame.sendPopulationRequest = function(x)
 {
	$("#gameMappingList").unbind("change");
	
	// contact the server, request HTML string of filtered gametype records
	$.get("/gamemappings/filter/populate/", {term:x},
		function(data){
		
			// Use DOM replacement to hot swap the entire select object
			$("#gameMappingList").replaceWith(data);
			
			// the option:selected change event needs to be rebinded			
			$("#gameMappingList").bind("change", LVCGame.listItemSelected);
	});
		
 };
 
 /**
  * Validate the value typed in the gamemapping textfield. 
  * IMPORTANT NOTE: There is a red flash of validate when a user navigates from a partially types testfield,
  * to the listbox. At this point in time, this has been okay'ed
  *
  * @return a bool indicated valid, or not valid
  */
 LVCGame.validateGameTypeChanged = function(evt)
 {
	var isSuccess = false;
	
	if( evt.target.value.length > 0)
	{
		// Using an async ajax GET call - we must wait for a reply
		// before rendering error/success formattings
		$.ajax({
			type: "GET",
			async: false,
			url: "/gamemappings/filter/validate/",
			data: ({term: evt.target.value}),
			success: function(data){
				if( data == "True")	{
					// if the search process found a matching gametype, make 
					// sure the textfield is rendered correctly.
					isSuccess = true;
				}
			}
		});
	}
	
	// return status for css modifications
    return isSuccess;
 };
 
 /**
  * This initTimeout is triggered no user activity has been registered for 1000ms, at which point it
  * automatically makes a sendPopulationRequest.
  */
 LVCGame.initTimeout = function(x)
 {
	// clear time out. After each successive key strokes a user makes within the
	// timeout threshold, restart it.
	clearTimeout(userEntryTimer);
	
	// if timeout is reached, repopulate the gametype mapping list
	userEntryTimer = setTimeout(
		function() { LVCGame.sendPopulationRequest(x)}, 
		1000);
 };
 
 /**
  * This method copies a selected game type from the gameMapping listbox to the
  * last gametype textfield focused.
  */
 LVCGame.listItemSelected = function()
 {	
	// copy the selected item to the textfield
	$(currentGameTypeField).attr('value', $("select option:selected").text()).trigger('change');
 };

