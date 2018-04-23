using System;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView;

public partial class _Default : System.Web.UI.Page {
    protected void Page_Init(object sender, EventArgs e) {
        ((GridViewDataComboBoxColumn)grid.Columns["CountryID"]).PropertiesComboBox.DataSource = DataProvider.GetCountries();
        ((GridViewDataComboBoxColumn)grid.Columns["CityID"]).PropertiesComboBox.DataSource = DataProvider.GetCities();

        if (!IsPostBack)
            grid.DataBind();
    }

    protected void CityCmb_Callback(object sender, CallbackEventArgsBase e) {
        int countryID = Convert.ToInt32(e.Parameter);
        ASPxComboBox c = sender as ASPxComboBox;
        c.DataSource = DataProvider.GetCities(countryID);
        c.DataBind();
    }

    protected void grid_DataBinding(object sender, EventArgs e) {
        grid.DataSource = DataProvider.GetCustomers();
    }

    protected void CityCmb_Init(object sender, EventArgs e) {
        ASPxComboBox cityCombo = sender as ASPxComboBox;
        GridViewEditItemTemplateContainer container = cityCombo.NamingContainer as GridViewEditItemTemplateContainer;
        int countryID = Convert.ToInt32(container.Grid.GetRowValues(container.Grid.VisibleStartIndex, "CountryID"));
        grid.JSProperties["cplastCountryID"] = countryID;
        cityCombo.DataSource = DataProvider.GetCities(countryID);
    }

    protected void grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e) {
        foreach (var args in e.InsertValues)
            DataProvider.InsertCustomer((string)args.NewValues["CustomerName"], (int)args.NewValues["CountryID"], (int)args.NewValues["CityID"]);
        foreach (var args in e.UpdateValues)
            DataProvider.UpdateCustomer((int)args.Keys["CustomerID"], (string)args.NewValues["CustomerName"], (int)args.NewValues["CountryID"], (int)args.NewValues["CityID"]);
        foreach (var args in e.DeleteValues)
            DataProvider.DeleteCustomer((int)args.Keys["CustomerID"]);
        e.Handled = true;
    }
}