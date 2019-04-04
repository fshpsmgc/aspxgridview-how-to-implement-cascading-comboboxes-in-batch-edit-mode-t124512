var curentEditingIndex;
var lastCountry = null;
var isCustomCascadingCallback = false;
function CountriesCombo_SelectedIndexChanged(s, e) {
    lastCountry = s.GetValue();
    isCustomCascadingCallback = true;
    RefreshData(lastCountry);
}
function CitiesCombo_EndCallback(s, e) {
    if (isCustomCascadingCallback) {
        if (s.GetItemCount() > 0)
            grid.batchEditApi.SetCellValue(curentEditingIndex, "CityId", s.GetItem(0).value);
        isCustomCascadingCallback = false;
    }
}
function OnBatchEditStartEditing(s, e) {
    curentEditingIndex = e.visibleIndex;
    var currentCountry = grid.batchEditApi.GetCellValue(curentEditingIndex, "CountryId");
    hf.Set("CurrentCountry", currentCountry);
    if (currentCountry != lastCountry && e.focusedColumn.fieldName == "CityId" && currentCountry != null) {
        lastCountry = currentCountry;
        RefreshData(currentCountry);
    }
}
function RefreshData(countryValue) {
    hf.Set("CurrentCountry", countryValue);
    CityEditor.PerformCallback();
}