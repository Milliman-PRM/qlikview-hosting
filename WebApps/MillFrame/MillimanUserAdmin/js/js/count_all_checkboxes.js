//-------------------------------------------------------------------------------
//  count checkboxes with provided checked state
//-------------------------------------------------------------------------------
function CountAllCheckboxesWithState(CheckBoxId, CheckVal, WhereToWrite) {
    var TotalCount = 0;
    var idEndsWithSelector = "input:checkbox[id$='" + CheckBoxId + "']";
    $(idEndsWithSelector).each(function (index) {
        if (this.checked === CheckVal) TotalCount++;
    });

    if (WhereToWrite === undefined) return TotalCount.toString();

    var idEndsWithSelector = "[id$='" + WhereToWrite + "']";
    $(idEndsWithSelector).each(function (index) {
        this.innerText = TotalCount.toString() + ' selected';
    });
}
