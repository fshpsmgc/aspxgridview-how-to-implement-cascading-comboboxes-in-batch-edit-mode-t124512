<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ASPxGridView - How to implement cascading comboboxes in Batch Edit mode</title>
    <script src="BatchEditScript.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        ASPxGridView - How to implement cascading comboboxes in Batch Edit mode
            <dx:ASPxGridView runat="server" OnDataBinding="grid_DataBinding" OnBatchUpdate="grid_BatchUpdate" KeyFieldName="CustomerID" ClientInstanceName="grid" ID="grid">
                <Columns>
                    <dx:GridViewCommandColumn ShowEditButton="true" ShowNewButtonInHeader="true" ShowDeleteButton="true"></dx:GridViewCommandColumn>
                    <dx:GridViewDataColumn FieldName="CustomerID">
                        <EditFormSettings Visible="False" />
                    </dx:GridViewDataColumn>
                    <dx:GridViewDataTextColumn FieldName="CustomerName">
                        <PropertiesTextEdit>
                            <ValidationSettings RequiredField-IsRequired="true"  Display="None"></ValidationSettings>
                        </PropertiesTextEdit>
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataComboBoxColumn Caption="Country" FieldName="CountryID">
                        <PropertiesComboBox ValueField="CountryID" ClientInstanceName="CountryID" TextField="CountryName" ValueType="System.Int32">
                            <ValidationSettings RequiredField-IsRequired="true"  Display="None"></ValidationSettings>
                            <ClientSideEvents SelectedIndexChanged="CountriesCombo_SelectedIndexChanged" />
                        </PropertiesComboBox>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn FieldName="CityID" Width="100px" Caption="City">
                        <PropertiesComboBox ValueType="System.Int32"  TextField="CityName" ValueField="CityID">
                        </PropertiesComboBox>
                        <EditItemTemplate>
                            <dx:ASPxComboBox runat="server" Width="100%" ValueType="System.Int32" OnInit="CityCmb_Init" IncrementalFilteringMode="StartsWith"  TextField="CityName" OnCallback="CityCmb_Callback" EnableCallbackMode="true" ValueField="CityID" ID="CityCmb" ClientInstanceName="CityID">
                                <ClientSideEvents EndCallback="CitiesCombo_EndCallback" KeyDown="CitiesCombo_KeyDown" LostFocus="CitiesCombo_LostFocus" />
                                <ValidationSettings RequiredField-IsRequired="true"  Display="None"></ValidationSettings>
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
                </Columns>
                <ClientSideEvents Init="OnInit" EndCallback="OnEndCallback" BatchEditRowValidating="OnBatchEditRowValidating" BatchEditStartEditing="OnBatchEditStartEditing" BatchEditEndEditing="OnBatchEditEndEditing" />
                <SettingsEditing Mode="Batch">
                    <BatchEditSettings ShowConfirmOnLosingChanges="true" EditMode="Row" />
                </SettingsEditing>
            </dx:ASPxGridView>
    </form>
</body>
</html>
