//-------------------------------------------------------------------------------
// highlight selected row javascript function
//-------------------------------------------------------------------------------
function ChangeRowColor(row) {
    var color = row.className;
    if (color !== 'gvHighlight') oldColor = color;
    if (color === 'gvHighlight') row.className = oldColor;
    else row.className = 'gvHighlight';

    /*if (navigator.appName == "Microsoft Internet Explorer") {
    var color = row.style.backgroundColor; // set original color to var color
    var highlightColor = '#ccc';
    if (color != highlightColor) oldColor = color; // set var color = oldcolor
    if (color == highlightColor) row.style.backgroundColor = oldColor; // set back to original color
    else row.style.backgroundColor = highlightColor; // apply new color
    // alert("You're USING the Internet Explorer browser.")
    }
    if (navigator.appName == "Netscape") {
    var color = row.style.backgroundColor; // set original color to var color
    var highlightColor = 'rgb(204, 204, 204)'; //
    if (color != highlightColor) oldColor = color; // set var color = oldcolor
    if (color == highlightColor) row.style.backgroundColor = oldColor; // set back to original color
    else row.style.backgroundColor = highlightColor; // apply new color
    //alert("You're USING a Mozilla based browser.")
    }
    else {
    // do nothing
    // alert("You're NOT using the IE or Mozilla.")
    }*/
}

//-------------------------------------------------------------------------------
//  toggle checkboxes in gridview javascript function
//-------------------------------------------------------------------------------
function SelectAllCheckboxes(CheckBoxId, CheckVal) {
    var idEndsWithSelector = "input:checkbox[id$='" + CheckBoxId + "']";
    $(idEndsWithSelector).each(function (index) {
        this.checked = CheckVal;  // does not trigger the click event on the checkbox
    })
    return CountAllCheckboxesWithState(CheckBoxId, CheckVal);  // returns string
}
