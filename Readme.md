<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128534653/16.1.5%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T124512)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# ASPxGridView for ASP.NET Web Forms - Cascading Comboboxes in Batch Edit Mode
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/t124512/)**
<!-- run online end -->

This example demonstrates how to set up cascading combo boxes in [ASPxGridView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView) for use in the batch edit mode. In the example, the selection in the Country combo box column filters the item list of the City combo box column. 

![Cascading Combo Boxes in Batch Edit Mode](demo.gif)

Use the following technique to setup a cascade of [GridViewDataComboBoxColumn](https://docs.devexpress.com/AspNet/DevExpress.Web.GridViewDataComboBoxColumn) editors:  
**Respond to a selection change** of the first combo box column (in its client [SelectedIndexChanged](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.SelectedIndexChanged) event) and **initiate a callback request** for the second combo box column to **filter data source** items on the server (use a combination of the client-side [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method and server [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event).

## Setup the Grid and Combo Boxes
Create an [ASPxGridView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView), assign its data source, and set grid's [Editing Mode](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridViewEditingSettings.Mode) to `Batch`. Add two combo box columns ([GridViewDataComboBoxColumn](https://docs.devexpress.com/AspNet/DevExpress.Web.GridViewDataComboBoxColumn)) and set their data sources.

```xml
<dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" 
    DataSourceID="ObjectDataSourceCustomers"
    KeyFieldName="CustomerId" 
    ...
            
    <SettingsEditing Mode="Batch" />
                
        <dx:GridViewDataComboBoxColumn FieldName="CountryId" Caption="Country">
            <PropertiesComboBox 
                DataSourceID="CountriesDataSource"
                TextField="CountryName" 
                ValueField="CountryId">
                ...
            </PropertiesComboBox>
        </dx:GridViewDataComboBoxColumn>
                
        <dx:GridViewDataComboBoxColumn FieldName="CityId" Caption="City">
            <PropertiesComboBox ClientInstanceName="cmbCity"
                DataSourceID="CitiesDataSource"
                TextField="CityName" 
                ValueField="CityId" 
                ValueType="System.Int32">
            </PropertiesComboBox>
            ...
        </dx:GridViewDataComboBoxColumn>
    </Columns>
</dx:ASPxGridView>

<asp:SqlDataSource ID="CountriesDataSource" runat="server" ... />
<asp:SqlDataSource ID="CitiesDataSource" runat="server" ... />

<asp:ObjectDataSource ID="ObjectDataSourceCustomers" runat="server" ... />
```

## Assign Event Handlers
To create a cascade of combo box columns, handle the first combo box column's client-side [SelectedIndexChanged](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.SelectedIndexChanged) event and the second combo box column's server-side [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event. You can set the [SelectedIndexChanged](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.SelectedIndexChanged) handler in the [ClientSideEvents](https://docs.devexpress.com/AspNet/DevExpress.Web.ComboBoxProperties.ClientSideEvents) property of the first combo box column. 

```xml
<dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" ...
        ...
        <dx:GridViewDataComboBoxColumn FieldName="CountryId" Caption="Country">
            <PropertiesComboBox 
                ...
                <ClientSideEvents SelectedIndexChanged="onSelectedCountryChanged" />
```

The [GridViewDataComboBoxColumn](https://docs.devexpress.com/AspNet/DevExpress.Web.GridViewDataComboBoxColumn) does not expose the server-side events in the markup, so you must set the [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event handler on the server side. Use the Grid View's server-side [CellEditorInitialize](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView.CellEditorInitialize) event handler to set the handler of the second combo box column's [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event.

```xml
<dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" ...
    OnCellEditorInitialize="grid_CellEditorInitialize"...
    ...
```

```c#
protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e) {
    ASPxGridView gridView = sender as ASPxGridView;

    if (e.Column.FieldName == "CityId") {
        ASPxComboBox cmbCity = (e.Editor as ASPxComboBox);
        cmbCity.Callback += cmbCity_OnCallback;
    }
}
```

## Respond to a Selection Change and Initiate a Callback
Handle the first combo box column's client-side [SelectedIndexChanged](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.SelectedIndexChanged) event. In the event handler, call the client-side [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method of the second combo box column. This sends a callback to the server for the second editor to filter its item list. In the [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method's parameter, pass the first combo box's selected value to use it as a filter criterion on the server.

```js
function onSelectedCountryChanged(s, e) {
    lastEditedCountry = s.GetValue();
    grid.batchEditApi.SetCellValue(currentRowIndex, "CityId", null);
    cmbCity.PerformCallback(s.GetValue());
}
```

If the Country value in the currently edited row is different from the Country value in the previously edited row, you must fill the City combo box column with appropriate items.

```js
function onBatchEditStartEditing(s, e) {
    currentRowIndex = e.visibleIndex;
    currentColumnIndex = e.focusedColumn.index;
    var currentCountry = s.batchEditApi.GetCellValue(currentRowIndex, "CountryId");
    if (currentCountry != lastEditedCountry && e.focusedColumn.fieldName == "CityId") {
        lastEditedCountry = currentCountry;
        e.cancel = true;
        cmbCity.PerformCallback(lastEditedCountry);
    }
}
```

## Filter Data Source Items
Handle the second combo box column's server-side [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event that fires in response to a call to the client-side [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method. In the handler, use the event argument's [Parameter](https://docs.devexpress.com/AspNet/DevExpress.Web.CallbackEventArgsBase.Parameter) property to obtain the first combo box column's selected value passed from the client side. Use this value to filter the second combo box column's data source.

```c#
void cmbCity_OnCallback(object source, CallbackEventArgsBase e) {
    FillCityCombo(source as ASPxComboBox, e.Parameter);
}

protected void FillCityCombo(ASPxComboBox cmb, string country) {
    // Reset the City combo box column.
    cmb.DataSourceID = null;
    cmb.Items.Clear();

    if (!string.IsNullOrEmpty(country)) {
        // Get the list of cities from the data source, filter it with the passed parameter, and fill the combo box with filtered items.
        cmb.DataSource = WorldCitiesDataProvider.GetCities()
                                                .Where(c => c.CountryId == Convert.ToInt32(country))
                                                .OrderBy(c => c.CityName)
                                                .GroupBy(c => c.CityName)
                                                .Select(g => g.FirstOrDefault())
                                                .ToList();

                cmb.DataBindItems();
            }
        }
```

## Files to Look At
- [Default.aspx](./CS/CascadingComboBoxesBatch/Default.aspx) (VB: [Default.aspx](./VB//CascadingComboBoxesBatch//Default.aspx))
- [Default.aspx.cs](./CS//CascadingComboBoxesBatch//Default.aspx.cs) (VB: [Default.aspx.vb](./VB//CascadingComboBoxesBatch//Default.aspx.vb))

## More Examples
- [GridView for ASP.NET MVC- A simple implementation of cascading comboboxes in Batch Edit mode](https://github.com/DevExpress-Examples/gridview-a-simple-implementation-of-cascading-comboboxes-in-batch-edit-mode-t155879)
- [ASPxGridView - How to implement cascading combo boxes in Batch Edit mode by using WebMethods](https://github.com/DevExpress-Examples/aspxgridview-how-to-implement-cascading-combo-boxes-in-batch-edit-mode-by-using-webmethods-t356740)

