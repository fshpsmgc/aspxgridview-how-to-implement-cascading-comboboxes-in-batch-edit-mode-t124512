var currentEditableVisibleIndex;
var preventEndEditOnLostFocus = false;
var lastCountryID;
var setValueFlag;
function CountriesCombo_SelectedIndexChanged(s, e) {
    var currentValue = grid.GetEditor('CountryID').GetValue();
    if (lastCountryID == currentValue) {
        if (CityID.GetSelectedIndex() < 0)
            CityID.SetSelectedIndex(0);
        return;
    }
    lastCountryID = currentValue;
    CityID.PerformCallback(currentValue);
}
function IntializeGlobalVariables(grid) {
    lastCountryID = grid.cplastCountryID;
    currentEditableVisibleIndex = -1;
    setValueFlag = -1;
}
function OnInit(s, e) {
    IntializeGlobalVariables(s);
}
function OnEndCallback(s, e) {
    IntializeGlobalVariables(s);
}
function CitiesCombo_EndCallback(s, e) {
    if (setValueFlag == -1)
        s.SetSelectedIndex(0);
    else if (setValueFlag > -1) {
        CityID.SetSelectedItem(CityID.FindItemByValue(setValueFlag));
        setValueFlag = -1;
    }
}
function OnBatchEditStartEditing(s, e) {
    currentEditableVisibleIndex = e.visibleIndex;
    var currentCountryID = grid.batchEditApi.GetCellValue(currentEditableVisibleIndex, "CountryID");
    var cityIDColumn = s.GetColumnByField("CityID");
    if (!e.rowValues.hasOwnProperty(cityIDColumn.index))
        return;
    var cellInfo = e.rowValues[cityIDColumn.index];
    if (lastCountryID == currentCountryID)
        if (CityID.FindItemByValue(cellInfo.value) != null)
            CityID.SetValue(cellInfo.value);
        else
            RefreshData(cellInfo, lastCountryID);
    else {
        if (currentCountryID == null) {
            CityID.SetSelectedIndex(-1);
            return;
        }
        lastCountryID = currentCountryID;
        RefreshData(cellInfo,lastCountryID);
    }
}
function RefreshData(cellInfo,countryID) {
    setValueFlag = cellInfo.value;
    setTimeout(function () {
        CityID.PerformCallback(countryID);
    }, 0);
}
function OnBatchEditEndEditing(s, e) {
    currentEditableVisibleIndex = -1;
    var cityIDColumn = s.GetColumnByField("CityID");
    if (!e.rowValues.hasOwnProperty(cityIDColumn.index))
        return;
    var cellInfo = e.rowValues[cityIDColumn.index];
    if (CityID.GetSelectedIndex() > -1 || cellInfo.text != CityID.GetText()) {
        cellInfo.value = CityID.GetValue();
        cellInfo.text = CityID.GetText();
        CityID.SetValue(null);
    }
}
function OnBatchEditRowValidating(s, e) {
    var cityIDColumn = s.GetColumnByField("CityID");
    var cellValidationInfo = e.validationInfo[cityIDColumn.index];
    if (!cellValidationInfo) return;
    var value = cellValidationInfo.value;
    if (!ASPxClientUtils.IsExists(value) || ASPxClientUtils.Trim(value) === "") {
        cellValidationInfo.isValid = false;
        cellValidationInfo.errorText = "City is required";
    }
}
function CitiesCombo_KeyDown(s, e) {
    var keyCode = ASPxClientUtils.GetKeyCode(e.htmlEvent);
    if (keyCode !== ASPxKey.Tab && keyCode !== ASPxKey.Enter) return;
    var moveActionName = e.htmlEvent.shiftKey ? "MoveFocusBackward" : "MoveFocusForward";
    if (grid.batchEditApi[moveActionName]()) {
        ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);
        preventEndEditOnLostFocus = true;
    }
}
function CitiesCombo_LostFocus(s, e) {
    if (!preventEndEditOnLostFocus)
        grid.batchEditApi.EndEdit();
    preventEndEditOnLostFocus = false;
}