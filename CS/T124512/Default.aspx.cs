using System;
using DevExpress.Web;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
public partial class _Default : System.Web.UI.Page
{

    WorldCitiesEntities entity = new WorldCitiesEntities();
    protected void Page_Init(object sender, EventArgs e)
    {
        ((GridViewDataComboBoxColumn)grid.Columns["CountryId"]).PropertiesComboBox.DataSource = entity.Countries.ToList();
        grid.DataSource = entity.Customers.ToList();
        if (!IsPostBack)
        {
            grid.DataBind();
        }
    }
    protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
    {
        if (e.Column.FieldName == "CountryId")
            e.Editor.ClientInstanceName = "CountryEditor";
        if (e.Column.FieldName != "CityId")
            return;
        var editor = (ASPxComboBox)e.Editor;
        editor.ClientInstanceName = "CityEditor";
        editor.ClientSideEvents.EndCallback = "CitiesCombo_EndCallback";

    }

    protected void OnItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
    {
        int id;
        if (e.Value == null || !int.TryParse(e.Value.ToString(), out id))
            return;
        ASPxComboBox editor = source as ASPxComboBox;
        var query = entity.Cities.Where(city => city.CityId == id);
        editor.DataSource = query.ToList();
        editor.DataBind();
    }

    protected void OnItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
    {
        ASPxComboBox editor = source as ASPxComboBox;
        IQueryable<City> query;
        var take = e.EndIndex - e.BeginIndex + 1;
        var skip = e.BeginIndex;
        int countryValue = GetCurrentCountry();
        if (countryValue > -1)
            query = entity.Cities.Where(city => city.CityName.Contains(e.Filter) && city.Country.CountryId == countryValue).OrderBy(city => city.CityId);
        else
            //query = entity.Cities.Where(city => city.CityName.Contains(e.Filter)).OrderBy(city => city.CityId);
            query = Enumerable.Empty<City>().AsQueryable();
        query = query.Skip(skip).Take(take);
        editor.DataSource = query.ToList();
        editor.DataBind();
    }

    private int GetCurrentCountry()
    {
        object id = null;
        if (hf.TryGet("CurrentCountry", out id))
            return Convert.ToInt32(id);
        return -1;
    }
    protected void grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
    {
        throw new DemoException("Data modifications are not allowed in the online example. If you need to test the data editing functionality, please download the example on your machine and run it locally.");
        foreach (var args in e.InsertValues)
            InsertNewItem(args.NewValues);
        foreach (var args in e.UpdateValues)
            UpdateItem(args.Keys, args.NewValues);
        foreach (var args in e.DeleteValues)
            DeleteItem(args.Keys, args.Values);
        entity.SaveChanges();
        e.Handled = true;
    }
    public void InsertNewItem(OrderedDictionary newValues)
    {
        Customer customer = new Customer();
        LoadNewValues(customer, newValues);
        entity.Customers.Add(customer);

    }
    public void UpdateItem(OrderedDictionary keys, OrderedDictionary newValues)
    {
        int id = Convert.ToInt32(keys["CustomerId"]);
        Customer customer = entity.Customers.Where(x => x.CustomerId == id).FirstOrDefault<Customer>();
        LoadNewValues(customer, newValues);

    }
    public void DeleteItem(OrderedDictionary keys, OrderedDictionary values)
    {
        int id = Convert.ToInt32(keys["CustomerId"]);
        Customer customer = entity.Customers.Where(x => x.CustomerId == id).FirstOrDefault<Customer>();
        entity.Customers.Remove(customer);

    }
    protected void LoadNewValues(Customer item, OrderedDictionary values)
    {
        item.CustomerName = values["CustomerName"].ToString();
        item.CityId = Convert.ToInt32(values["CityId"]);
        item.CountryId = Convert.ToInt32(values["CountryId"]);
    }

}