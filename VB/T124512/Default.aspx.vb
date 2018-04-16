Imports System
Imports DevExpress.Web
Imports System.Linq
Imports System.Collections.Generic
Imports System.Collections.Specialized
Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private entity As New WorldCitiesEntities()
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        CType(grid.Columns("CountryId"), GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = entity.Countries.ToList()
        grid.DataSource = entity.Customers.ToList()
        If Not IsPostBack Then
            grid.DataBind()
        End If
    End Sub
    Protected Sub grid_CellEditorInitialize(ByVal sender As Object, ByVal e As ASPxGridViewEditorEventArgs)
        If e.Column.FieldName = "CountryId" Then
            e.Editor.ClientInstanceName = "CountryEditor"
        End If
        If e.Column.FieldName <> "CityId" Then
            Return
        End If
        Dim editor = CType(e.Editor, ASPxComboBox)
        editor.ClientInstanceName = "CityEditor"
        editor.ClientSideEvents.EndCallback = "CitiesCombo_EndCallback"

    End Sub

    Protected Sub OnItemRequestedByValue(ByVal source As Object, ByVal e As ListEditItemRequestedByValueEventArgs)

        Dim id_Renamed As Integer = Nothing
        If e.Value Is Nothing OrElse (Not Integer.TryParse(e.Value.ToString(), id_Renamed)) Then
            Return
        End If
        Dim editor As ASPxComboBox = TryCast(source, ASPxComboBox)
        Dim query = entity.Cities.Where(Function(city) city.CityId = id_Renamed)
        editor.DataSource = query.ToList()
        editor.DataBind()
    End Sub

    Protected Sub OnItemsRequestedByFilterCondition(ByVal source As Object, ByVal e As ListEditItemsRequestedByFilterConditionEventArgs)
        Dim editor As ASPxComboBox = TryCast(source, ASPxComboBox)
        Dim query As IQueryable(Of City)
        Dim take = e.EndIndex - e.BeginIndex + 1
        Dim skip = e.BeginIndex
        Dim countryValue As Integer = GetCurrentCountry()
        If countryValue > -1 Then
            query = entity.Cities.Where(Function(city) city.CityName.Contains(e.Filter) AndAlso city.Country.CountryId = countryValue).OrderBy(Function(city) city.CityId)
        Else
            query = entity.Cities.Where(Function(city) city.CityName.Contains(e.Filter)).OrderBy(Function(city) city.CityId)
        End If
        query = query.Skip(skip).Take(take)
        editor.DataSource = query.ToList()
        editor.DataBind()
    End Sub

    Private Function GetCurrentCountry() As Integer

        Dim id_Renamed As Object = Nothing
        If hf.TryGet("CurrentCountry", id_Renamed) Then
            Return Convert.ToInt32(id_Renamed)
        End If
        Return -1
    End Function
    Protected Sub grid_BatchUpdate(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs)
        Throw New DemoException("Data modifications are not allowed in the online example. If you need to test the data editing functionality, please download the example on your machine and run it locally.")
        For Each args In e.InsertValues
            InsertNewItem(args.NewValues)
        Next args
        For Each args In e.UpdateValues
            UpdateItem(args.Keys, args.NewValues)
        Next args
        For Each args In e.DeleteValues
            DeleteItem(args.Keys, args.Values)
        Next args
        entity.SaveChanges()
        e.Handled = True
    End Sub
    Public Sub InsertNewItem(ByVal newValues As OrderedDictionary)
        Dim customer As New Customer()
        LoadNewValues(customer, newValues)
        entity.Customers.Add(customer)

    End Sub
    Public Sub UpdateItem(ByVal keys As OrderedDictionary, ByVal newValues As OrderedDictionary)

        Dim id_Renamed As Integer = Convert.ToInt32(keys("CustomerId"))
        Dim customer As Customer = entity.Customers.Where(Function(x) x.CustomerId = id_Renamed).FirstOrDefault()
        LoadNewValues(customer, newValues)

    End Sub
    Public Sub DeleteItem(ByVal keys As OrderedDictionary, ByVal values As OrderedDictionary)

        Dim id_Renamed As Integer = Convert.ToInt32(keys("CustomerId"))
        Dim customer As Customer = entity.Customers.Where(Function(x) x.CustomerId = id_Renamed).FirstOrDefault()
        entity.Customers.Remove(customer)

    End Sub
    Protected Sub LoadNewValues(ByVal item As Customer, ByVal values As OrderedDictionary)
        item.CustomerName = values("CustomerName").ToString()
        item.CityId = Convert.ToInt32(values("CityId"))
        item.CountryId = Convert.ToInt32(values("CountryId"))
    End Sub

End Class