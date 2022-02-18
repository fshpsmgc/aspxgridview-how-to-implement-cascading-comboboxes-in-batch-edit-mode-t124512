using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace CascadingComboBoxesBatch {
	public partial class WorldCitiesContext : DbContext {
		static WorldCitiesContext() {
			Database.SetInitializer<WorldCitiesContext>(null);
		}
		public WorldCitiesContext()
			: base("ConnectionString") {
		}
		public DbSet<City> Cities { get; set; }
		public DbSet<Country> Countries { get; set; }
		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Configurations.Add(new CityMap());
			modelBuilder.Configurations.Add(new CountryMap());
		}
	}

	public partial class City {
		public City() {
			this.Countries = new List<Country>();
		}
		public int CityId { get; set; }
		public string CityName { get; set; }
		public Nullable<int> CountryId { get; set; }
		public virtual Country Country { get; set; }
		public virtual ICollection<Country> Countries { get; set; }
	}
	public partial class Country {
		public Country() {
			this.Cities = new List<City>();
		}
		public int CountryId { get; set; }
		public string CountryName { get; set; }
		public Nullable<int> CapitalId { get; set; }
		public virtual ICollection<City> Cities { get; set; }
		public virtual City City { get; set; }
	}
	public class CityMap : EntityTypeConfiguration<City> {
		public CityMap() {
			this.HasKey(t => t.CityId);
			this.Property(t => t.CityName)
				.HasMaxLength(255);
			this.ToTable("Cities");
			this.Property(t => t.CityId).HasColumnName("CityId");
			this.Property(t => t.CityName).HasColumnName("CityName");
			this.Property(t => t.CountryId).HasColumnName("CountryId");
			this.HasOptional(t => t.Country)
				.WithMany(t => t.Cities)
				.HasForeignKey(d => d.CountryId);
		}
	}
	public class CountryMap : EntityTypeConfiguration<Country> {
		public CountryMap() {
			this.HasKey(t => t.CountryId);
			this.Property(t => t.CountryName)
				.HasMaxLength(255);
			this.ToTable("Countries");
			this.Property(t => t.CountryId).HasColumnName("CountryId");
			this.Property(t => t.CountryName).HasColumnName("CountryName");
			this.Property(t => t.CapitalId).HasColumnName("CapitalId");
			this.HasOptional(t => t.City)
				.WithMany(t => t.Countries)
				.HasForeignKey(d => d.CapitalId);
		}
	}
}
