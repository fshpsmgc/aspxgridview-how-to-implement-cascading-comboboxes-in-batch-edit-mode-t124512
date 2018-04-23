Imports System
Imports DevExpress.Web.ASPxClasses
Imports DevExpress.Web.ASPxEditors
Imports DevExpress.Web.ASPxGridView

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        CType(grid.Columns("CountryID"), GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = DataProvider.GetCountries()
        CType(grid.Columns("CityID"), GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = DataProvider.GetCities()

        If Not IsPostBack Then
            grid.DataBind()
        End If
    End Sub

    Protected Sub CityCmb_Callback(ByVal sender As Object, ByVal e As CallbackEventArgsBase)
        Dim countryID As Integer = Convert.ToInt32(e.Parameter)
        Dim c As ASPxComboBox = TryCast(sender, ASPxComboBox)
        c.DataSource = DataProvider.GetCities(countryID)
        c.DataBind()
    End Sub

    Protected Sub grid_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        grid.DataSource = DataProvider.GetCustomers()
    End Sub

    Protected Sub CityCmb_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim cityCombo As ASPxComboBox = TryCast(sender, ASPxComboBox)
        Dim container As GridViewEditItemTemplateContainer = TryCast(cityCombo.NamingContainer, GridViewEditItemTemplateContainer)
        Dim countryID As Integer = Convert.ToInt32(container.Grid.GetRowValues(container.Grid.VisibleStartIndex, "CountryID"))
        grid.JSProperties("cplastCountryID") = countryID
        cityCombo.DataSource = DataProvider.GetCities(countryID)
    End Sub

    Protected Sub grid_BatchUpdate(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs)
        For Each args In e.InsertValues
            DataProvider.InsertCustomer(CStr(args.NewValues("CustomerName")), CInt((args.NewValues("CountryID"))), CInt((args.NewValues("CityID"))))
        Next args
        For Each args In e.UpdateValues
            DataProvider.UpdateCustomer(CInt((args.Keys("CustomerID"))), CStr(args.NewValues("CustomerName")), CInt((args.NewValues("CountryID"))), CInt((args.NewValues("CityID"))))
        Next args
        For Each args In e.DeleteValues
            DataProvider.DeleteCustomer(CInt((args.Keys("CustomerID"))))
        Next args
        e.Handled = True
    End Sub
End Class