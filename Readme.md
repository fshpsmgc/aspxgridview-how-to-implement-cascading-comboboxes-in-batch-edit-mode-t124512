<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128534653/16.1.5%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T124512)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# ASPxGridView for ASP.NET Web Forms - Cascading Comboboxes in Batch Edit Mode
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/t124512/)**
<!-- run online end -->

This example demonstrates how to set up cascading combo boxes in [ASPxGridView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView) for use in the batch edit mode. In this project, the selection in the Country column's combo box editor filters the item list of the combo box in the City column. 

![Cascading Combo Boxes in Batch Edit Mode](demo.gif)

The cascading combo boxes for the grid in the batch edit mode use the same technique as other cascading combo box implementations. This technique involves the following steps:
- [Response to a selection change](#respond-to-a-selection-change) of the primary combo box editor (in its client [SelectedIndexChanged](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.SelectedIndexChanged) event).
- A [callback request](#assign-a-callback-handler) for the secondary column's combo box (with the client-side [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method).
- [Filtering the data source items](#filter-the-second-combo-box-values) on the server (in the server-side [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event handler). 

There are implementation details that are specific to the batch edit mode: 
- The server-side Callback [event handler has to be assigned](#assign-a-callback-handler) in the server-side [CellEditorInitialize](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView.CellEditorInitialize) event handler.  
- Items in the editors that depend on other editor's value should be [updated manually](#manually-update-the-second-combo-box) with the [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method every time the user starts editing the columns' values.

## Setup the Grid and its Column Editors
Create an [ASPxGridView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView), assign its data source, and set the grid's [editing mode](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridViewEditingSettings.Mode) to `Batch`. Add two columns of type [GridViewDataComboBoxColumn](https://docs.devexpress.com/AspNet/DevExpress.Web.GridViewDataComboBoxColumn) and set their data sources.

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

## On Client: Initiate the Update of the Secondary Combo Box Items

On the client, initiate callbacks to the server to update the items in the secondary combo box. 

### Manually Update the Secondary Combo Box

In batch edit mode, the grid uses a single instance of a column editor for all cells in this column. This instance does not perform a round trip to the server on each selection change. Because of that, you should manually update the items in the editors that depend on other editor's value (the secondary column's combo box editor). 

To update the items, handle the grid's client-side [BatchEditStartEditing](https://docs.devexpress.com/AspNet/DevExpress.Web.GridViewClientSideEvents.BatchEditStartEditing) event and call the secondary combo box's [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method with the selected country.



```js
function onBatchEditStartEditing(s, e) {
    currentRowIndex = e.visibleIndex;
    var currentCountry = s.batchEditApi.GetCellValue(currentRowIndex, "CountryId");

    if (currentCountry != lastEditedCountry && e.focusedColumn.fieldName == "CityId") {
        lastEditedCountry = currentCountry;
        e.cancel = true;
        cmbCity.PerformCallback(lastEditedCountry);
    }
}
```

### Respond to a Selection Change

Handle the [SelectedIndexChanged](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.SelectedIndexChanged) event of the primary column's combo box editor. In the event handler, call the client-side [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method of the secondary column's combo box editor. This sends a callback to the server for the secondary editor to filter its item list. In the [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method's parameter, pass the primary combo box's selected value to use it as a filter criterion on the server.

```xml
<dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" ...
        ...
        <dx:GridViewDataComboBoxColumn FieldName="CountryId" Caption="Country">
            <PropertiesComboBox 
                ...
                <ClientSideEvents SelectedIndexChanged="onSelectedCountryChanged" />
```

```js
function onSelectedCountryChanged(s, e) {
    lastEditedCountry = s.GetValue();
    grid.batchEditApi.SetCellValue(currentRowIndex, "CityId", null);
    cmbCity.PerformCallback(s.GetValue());
}
```

## On Server: Setup and Handle a Callback

On the server, create a handler that filters the items in the secondary combo box and assign as the secondary editor's [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event handler

### Filter the Secondary Combo Box Values

Handle the secondary combo box column's server-side [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event that fires in response to a call to the client-side [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method. In the handler, use the event argument's [Parameter](https://docs.devexpress.com/AspNet/DevExpress.Web.CallbackEventArgsBase.Parameter) property to obtain the primary combo box column's selected value passed from the client side. Use this value to filter the secondary combo box column's data source.

Handle the server-side [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event for the secondary combo box column (this event fires in response to a client-side [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) method call). In the event handler, use the [Parameter](https://docs.devexpress.com/AspNet/DevExpress.Web.CallbackEventArgsBase.Parameter) event property to obtain the primary column's value from the client side. Filter the secondary combobox's data source based on this value.

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

### Assign a Callback Handler

The column's combo box editor instance is created at runtime. Because of that, you should use the grid's server-side [CellEditorInitialize](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView.CellEditorInitialize) event handler to access the secondary column's editor instance and set its [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback) event handler.

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

## Documentation
- [ASPxGridView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView)
- [SelectedIndexChanged](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.SelectedIndexChanged)
- [PerformCallback](https://docs.devexpress.com/AspNet/js-ASPxClientComboBox.PerformCallback(parameter)) 
- [Callback](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCallback.Callback)
- [BatchEditStartEditing](https://docs.devexpress.com/AspNet/DevExpress.Web.GridViewClientSideEvents.BatchEditStartEditing)

## Files to Look At
- [Default.aspx](./CS/CascadingComboBoxesBatch/Default.aspx) (VB: [Default.aspx](./VB//CascadingComboBoxesBatch//Default.aspx))
- [Default.aspx.cs](./CS//CascadingComboBoxesBatch//Default.aspx.cs) (VB: [Default.aspx.vb](./VB//CascadingComboBoxesBatch//Default.aspx.vb))

## More Examples
- [Cascading Editors (Batch Editing) Demo](https://demos.devexpress.com/ASPxGridViewDemos/GridEditing/CascadingComboBoxesBatch.aspx)
- [GridView for ASP.NET MVC- A simple implementation of cascading comboboxes in Batch Edit mode](https://github.com/DevExpress-Examples/gridview-a-simple-implementation-of-cascading-comboboxes-in-batch-edit-mode-t155879)
- [ASPxGridView - How to implement cascading combo boxes in Batch Edit mode by using WebMethods](https://github.com/DevExpress-Examples/aspxgridview-how-to-implement-cascading-combo-boxes-in-batch-edit-mode-by-using-webmethods-t356740)

