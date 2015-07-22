
/**
 * Invoked when the LVCType (Enumeration) has changed. This function will validate the enumeration entered.
 */
LVCGame.lvcTypeChanged = function(evt)
{
    var jElem = $(evt.target);
    var ret = LVCGame.validateLvcTypeChanged(evt);
    
    if(ret.isError)
    {
        jElem.addClass("inputError").attr("title", ret.message);
        $(".submitButton").attr("disabled","disabled");
    }
    else
    {
        jElem.removeClass("inputError").removeAttr("title");
        $(".submitButton").removeAttr("disabled");
    }
};

/**
 * Validates the offset input field.
 */
LVCGame.offsetChanged = function(evt)
{
    // There isn't any restriction except that it must be a valid float.
    var jElem = $(evt.target);
    var val = parseFloat(jElem.val());
    if(isNaN(val))
    {
        var trimStr = $.trim(jElem.val());
        if(trimStr !== '-')
        {
            jElem.addClass("inputError").attr("title", "\""+jElem.val()+"\" is not a valid decimal number.");
            $(".submitButton").attr("disabled","disabled");
        }
        else
        {
            jElem.val(trimStr);
            jElem.removeClass("inputError").removeAttr("title");
            $(".submitButton").removeAttr("disabled");
        }
    }
    else
    {
        jElem.val(val);
        jElem.removeClass("inputError").removeAttr("title");
        $(".submitButton").removeAttr("disabled");
    }
};

/**
 * Determines if there are any invalid mappings in the outgoing mappings table.
 * 
 * @return True if the mappings are valid, false otherwise.
 */
LVCGame.isOutgoingMappingsValid = function()
{
    return $(".inputError").length === 0;
};

/**
  * POSTS all selected outgoing mappings to the deletion url.
  */
LVCGame.deleteOutgoingMappings = function()
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
            
            $.post("/gamemappings/outgoing/delete/", post, function(){
                document.location = document.location;
            });
        }
    }
};

$(document).ready(function() {
    // Add validation to the lvcType input field (The enumeration)
    $(".lvcTypeInput").change(LVCGame.lvcTypeChanged);
    // Add validation to the offset input fields
    $(".offsetValue").change(LVCGame.offsetChanged);
});