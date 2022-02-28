<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CascadingComboBoxesBatch.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <script>
        var currentRowIndex = -1;
        var currentColumnIndex = -1;
        var lastEditedCountry = -1;

        function onBatchEditStartEditing(s, e) {
            currentRowIndex = e.visibleIndex;
            currentColumnIndex = e.focusedColumn.index;
            var currentCountry = s.batchEditApi.GetCellValue(currentRowIndex, "CountryId");

            // Check if the secondary combo box has correct items and initiate the callback to update them if necessary.
            if (currentCountry != lastEditedCountry && e.focusedColumn.fieldName == "CityId") {
                lastEditedCountry = currentCountry;
                e.cancel = true;
                cmbCity.PerformCallback(lastEditedCountry);
            }
        }

        // Handle the selection change and send a callback for the City column editor.
        function onSelectedCountryChanged(s, e) {
            lastEditedCountry = s.GetValue();
            grid.batchEditApi.SetCellValue(currentRowIndex, "CityId", null);
            cmbCity.PerformCallback(s.GetValue());
        }

        function onCityEndCallback(s, e) {
            lp.Hide();
            grid.batchEditApi.StartEdit(currentRowIndex, currentColumnIndex);
        }

        function onGridEndCallback(s, e) {
            lastEditedCountry = -1;
        }

        function onFocusedCellChanging(s, e) {
            e.cancel = cmbCity.InCallback();
        }

        function onBeginCallback(s, e) {
            window.setTimeout(function () {
                if (cmbCity.InCallback()) lp.ShowInElement(grid.batchEditApi.GetCellTextContainer(currentRowIndex, "CityId"));
            }, 300);
        }
    </script>

    <form id="form1" runat="server">
        <dx:ASPxLoadingPanel runat="server" ID="loadingPanel" ClientInstanceName="lp" Modal="true" />

        <dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" Width="100%" AutoGenerateColumns="false"
            DataSourceID="ObjectDataSourceCustomers"
            KeyFieldName="CustomerId" 
            OnCellEditorInitialize="grid_CellEditorInitialize">

            <ClientSideEvents 
                BatchEditStartEditing="onBatchEditStartEditing" 
                EndCallback="onGridEndCallback" 
                FocusedCellChanging="onFocusedCellChanging" />
            
            <SettingsEditing Mode="Batch" />
            
            <Columns>
                <dx:GridViewCommandColumn 
                    ShowNewButtonInHeader="true" 
                    ShowDeleteButton="true" 
                    ShowCancelButton="true" 
                    Width="150"/>

                <dx:GridViewDataColumn FieldName="CustomerName" />

                <dx:GridViewDataComboBoxColumn FieldName="CountryId" Caption="Country">
                    <PropertiesComboBox TextField="CountryName" ValueField="CountryId" ValueType="System.Int32" EnableSynchronization="false" DataSecurityMode="Strict"
                        IncrementalFilteringMode="StartsWith" DataSourceID="CountriesDataSource">
                        <ClientSideEvents SelectedIndexChanged="onSelectedCountryChanged" />
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                
                <dx:GridViewDataComboBoxColumn FieldName="CityId" Caption="City">
                    <PropertiesComboBox EnableSynchronization="false" IncrementalFilteringMode="StartsWith" ClientInstanceName="cmbCity"
                        DataSourceID="CitiesDataSource" ValueField="CityId" TextField="CityName" ValueType="System.Int32" DataSecurityMode="Strict">
                        <ClientSideEvents EndCallback="onCityEndCallback" BeginCallback="onBeginCallback" />
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
            </Columns>
        </dx:ASPxGridView>
        
        <asp:SqlDataSource ID="CountriesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT [CountryId], [CountryName] FROM [Countries]"/>
        <asp:SqlDataSource ID="CitiesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT [CityId], [CityName] FROM [Cities]"/>

        <asp:ObjectDataSource ID="ObjectDataSourceCustomers" runat="server" 
            DataObjectTypeName="CascadingComboBoxesBatch.EditableWorldCustomer" 
            InsertMethod="InsertCustomer" 
            SelectMethod="GetEditableCustomers" 
            TypeName="CascadingComboBoxesBatch.WorldCitiesDataProvider" 
            UpdateMethod="UpdateCustomer" />
    </form>
</body>
</html>
