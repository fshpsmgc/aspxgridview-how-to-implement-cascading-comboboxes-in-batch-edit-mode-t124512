using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI;
using System.Linq;
using DevExpress.Web;

namespace CascadingComboBoxesBatch {
    public partial class Default : Page {
        protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e) {
            ASPxGridView gridView = sender as ASPxGridView;
            // Assign a callback handler to the City column editor.
            if (e.Column.FieldName == "CityId") {
                ASPxComboBox cmbCity = (e.Editor as ASPxComboBox);
                cmbCity.Callback += cmbCity_OnCallback;
            }
        }
        protected void FillCityCombo(ASPxComboBox cmb, string country) {
            // Reset the City column combo box editor.
            cmb.DataSourceID = null;
            cmb.Items.Clear();

            if (!string.IsNullOrEmpty(country)) {
                // Get the list of cities from the data source, filter it with the passes parameter, and fill the combo box with filtered items.
                cmb.DataSource = WorldCitiesDataProvider.GetCities().Where(c => c.CountryId == Convert.ToInt32(country))
                                                                       .OrderBy(c => c.CityName)
                                                                       .GroupBy(c => c.CityName)
                                                                       .Select(g => g.FirstOrDefault())
                                                                       .ToList();

                cmb.DataBindItems();
            }
        }
        void cmbCity_OnCallback(object source, CallbackEventArgsBase e) {
            FillCityCombo(source as ASPxComboBox, e.Parameter);
        }
    }
}