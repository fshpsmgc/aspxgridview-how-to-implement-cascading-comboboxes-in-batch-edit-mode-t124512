using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CascadingComboBoxesBatch {
	public static class WorldCitiesDataProvider {
		const string WorldCitiesDataContextKey = "DXWorldCitiesDataContext";
		public static WorldCitiesContext DB {
			get {
				if (HttpContext.Current.Items[WorldCitiesDataContextKey] == null)
					HttpContext.Current.Items[WorldCitiesDataContextKey] = new WorldCitiesContext();
				return (WorldCitiesContext)HttpContext.Current.Items[WorldCitiesDataContextKey];
			}
		}
		public static IEnumerable<Country> GetCountries() {
			return DB.Countries
				.OrderBy(c => c.CountryName)
				.ToList();
		}

		public static IEnumerable<City> GetCities() {
			return DB.Cities
				.OrderBy(c => c.CityName)
				.ToList();
		}
		const string SessionKey = "DXDemoWorldCustomers";
		public static IList<EditableWorldCustomer> GetEditableCustomers() {
			IList<EditableWorldCustomer> customersList = HttpContext.Current.Session[SessionKey] as IList<EditableWorldCustomer>;
			if (customersList == null) {
				customersList = new List<EditableWorldCustomer>();
				customersList.Add(new EditableWorldCustomer() { CustomerId = 1, CustomerName = "Maria Anders", CountryId = 63, CityId = 775 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 2, CustomerName = "Thomas Hardy", CountryId = 1, CityId = 125 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 3, CustomerName = "Yang Wang", CountryId = 17, CityId = 2025 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 4, CustomerName = "Elizabeth Brown", CountryId = 35, CityId = 332 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 5, CustomerName = "Sven Ottlieb", CountryId = 217, CityId = 1459 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 6, CustomerName = "Janine Labrune", CountryId = 220, CityId = 1651 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 7, CustomerName = "Martine Rance", CountryId = 218, CityId = 844 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 8, CustomerName = "Maria Larsson", CountryId = 219, CityId = 2444 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 9, CustomerName = "Peter Franken", CountryId = 234, CityId = 1900 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 10, CustomerName = "Carlos Hernandez", CountryId = 5, CityId = 2445 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 11, CustomerName = "Philip Cramer", CountryId = 154, CityId = 356 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 12, CustomerName = "Yoshi Tannamuri", CountryId = 10, CityId = 2447 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 13, CustomerName = "Peter Heinken", CountryId = 159, CityId = 475 });
				customersList.Add(new EditableWorldCustomer() { CustomerId = 14, CustomerName = "Mike Taison", CountryId = 216, CityId = 2219 });
				HttpContext.Current.Session[SessionKey] = customersList;
			}
			return customersList;
		}
		public static EditableWorldCustomer GetEditableCustomerByID(int customerID) {
			return GetEditableCustomers().Where(c => c.CustomerId.Equals(customerID)).SingleOrDefault();
		}
		public static int GetNewEditableCustomerID() {
			IList<EditableWorldCustomer> editableCustomers = GetEditableCustomers();
			if (editableCustomers.Count() == 0) return 1;
			return editableCustomers.OrderBy(c => c.CustomerId).Last().CustomerId + 1;
		}
		public static void InsertCustomer(EditableWorldCustomer customer) {
			var newCustomer = new EditableWorldCustomer() {
				CustomerId = GetNewEditableCustomerID(),
				CustomerName = customer.CustomerName,
				CountryId = customer.CountryId,
				CityId = customer.CityId
			};
			GetEditableCustomers().Add(newCustomer);
		}
		public static void UpdateCustomer(EditableWorldCustomer customer) {
			EditableWorldCustomer editableCustomer = GetEditableCustomerByID(customer.CustomerId);
			if (editableCustomer == null)
				return;
			editableCustomer.CustomerName = customer.CustomerName;
			editableCustomer.CountryId = customer.CountryId;
			editableCustomer.CityId = customer.CityId;
		}
	}
	public class EditableWorldCustomer {
		public int CustomerId { get; set; }
		[Required(ErrorMessage = "Customer Name is required")]
		[StringLength(50, ErrorMessage = "Must be under 50 characters")]
		public string CustomerName { get; set; }
		public int? CountryId { get; set; }
		public int? CityId { get; set; }
	}
}