//-------------------------------------------------------------------------------
//  count checkboxes with provided checked state
//-------------------------------------------------------------------------------
function CountAllCheckboxesWithState(CheckBoxId, CheckVal) {
    var TotalCount = 0;
    for (var i = 0; i < document.forms[0].elements.length; i++) //Loop through all form elements
    {
        var elm = document.forms[0].elements[i];
        if (elm.type === 'checkbox') //Check if the element is a checkbox
        {
            var str = elm.name;
            if (str.indexOf(CheckBoxId) !== -1) //See if checkbox has ID which we're looking for
            {
                if (elm.checked === CheckVal)
                {
                    TotalCount++;
                }
            }
        }
    }
    return TotalCount.toString();
}
