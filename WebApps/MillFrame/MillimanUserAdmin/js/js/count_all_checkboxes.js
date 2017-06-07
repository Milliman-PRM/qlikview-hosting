//-------------------------------------------------------------------------------
//  count checkboxes with provided checked state
//-------------------------------------------------------------------------------
function CountAllCheckboxesWithState(CheckBoxId, CheckVal) {
    var TotalCount = 0;
    var idEndsWithSelector = "input:checkbox[id$='" + CheckBoxId + "']";
    $(idEndsWithSelector).each(function (index) {
        if (this.checked === CheckVal) TotalCount++;
    });

    return TotalCount.toString();
}
