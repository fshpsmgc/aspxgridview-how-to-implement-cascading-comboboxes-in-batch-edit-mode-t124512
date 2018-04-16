<%@ Page Language="vb" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ASPxGridView - How to implement cascading comboboxes in Batch Edit mode</title>
    <script src="BatchEditScript.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        ASPxGridView - How to implement cascading comboboxes in Batch Edit mode
        <dx:ASPxHiddenField runat="server" ID="hf" ClientInstanceName="hf"></dx:ASPxHiddenField>
       <dx:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" OnBatchUpdate="grid_BatchUpdate" KeyFieldName="CustomerId" OnCellEditorInitialize="grid_CellEditorInitialize">
           <Columns>
               <dx:GridViewCommandColumn ShowNewButtonInHeader="true" ShowDeleteButton="true"></dx:GridViewCommandColumn>
               <dx:GridViewDataColumn FieldName="CustomerId" Visible="false">
                   <EditFormSettings Visible="False" />
               </dx:GridViewDataColumn>
               <dx:GridViewDataTextColumn Width="140" FieldName="CustomerName">
                   <PropertiesTextEdit>
                       <ValidationSettings RequiredField-IsRequired="true" Display="None"></ValidationSettings>
                   </PropertiesTextEdit>
               </dx:GridViewDataTextColumn>
               <dx:GridViewDataComboBoxColumn Width="120" Caption="Country" FieldName="CountryId">
                   <PropertiesComboBox EnableCallbackMode="true" CallbackPageSize="30" ValueField="CountryId" TextField="CountryName" ValueType="System.Int32">
                       <ValidationSettings RequiredField-IsRequired="true" Display="None"></ValidationSettings>
                       <ClientSideEvents SelectedIndexChanged="CountriesCombo_SelectedIndexChanged" />
                   </PropertiesComboBox>
               </dx:GridViewDataComboBoxColumn>
               <dx:GridViewDataComboBoxColumn Caption="City" FieldName="CityId" Width="120px">
                   <PropertiesComboBox EnableCallbackMode="true" CallbackPageSize="20" IncrementalFilteringMode="Contains" OnItemRequestedByValue="OnItemRequestedByValue" OnItemsRequestedByFilterCondition="OnItemsRequestedByFilterCondition" ValueType="System.Int32" TextField="CityName" ValueField="CityId">
                   </PropertiesComboBox>
               </dx:GridViewDataComboBoxColumn>
           </Columns>
           <ClientSideEvents BatchEditStartEditing="OnBatchEditStartEditing"  />
           <SettingsEditing Mode="Batch">
               <BatchEditSettings ShowConfirmOnLosingChanges="true" EditMode="Cell" />
           </SettingsEditing>
       </dx:ASPxGridView>
    </form>
</body>
</html>