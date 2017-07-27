using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Localization;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
    /// Represents an entity address view model.
    /// </summary>
    public class EntityAddressViewModel
    {

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityAddressViewModel"/> class.
		/// </summary>
		public EntityAddressViewModel()
        {

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityAddressViewModel"/> class.
		/// </summary>
		/// <param name="address">The address.</param>
		public EntityAddressViewModel(EntityAddress address)
        {
            this.Country = address?.Component?.Find(o => o?.ComponentTypeKey == AddressComponentKeys.Country)?.Value;
            this.City = address?.Component?.Find(o => o?.ComponentTypeKey == AddressComponentKeys.City)?.Value;
            this.County= address?.Component?.Find(o => o?.ComponentTypeKey == AddressComponentKeys.County)?.Value;
            this.State = address?.Component?.Find(o => o?.ComponentTypeKey == AddressComponentKeys.State)?.Value;
            this.StreetAddress = address?.Component?.Find(o => o?.ComponentTypeKey == AddressComponentKeys.StreetAddressLine)?.Value;
            this.Precinct = address?.Component?.Find(o => o?.ComponentTypeKey == AddressComponentKeys.Precinct)?.Value;
            this.PostalCode = address?.Component?.Find(o => o?.ComponentTypeKey == AddressComponentKeys.PostalCode)?.Value;
        }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        /// <value>The country.</value>
        [Display(Name = "AddressCountry", ResourceType = typeof(Locale))]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the county
        /// </summary>
        /// <value>The county.</value>
        [Display(Name = "AddressCounty", ResourceType = typeof(Locale))]
        public string County { get; set; }

        /// <summary>
        /// Gets or set the city
        /// </summary>
        /// <value>The city.</value>
        [Display(Name = "AddressCity", ResourceType = typeof(Locale))]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        /// <value>The state.</value>
        [Display(Name = "AddressState", ResourceType = typeof(Locale))]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the street address
        /// </summary>
        /// <value>The street address.</value>
        [Display(Name = "AddressStreetAddress", ResourceType = typeof(Locale))]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the precinct
        /// </summary>
        /// <value>The precinct.</value>
        [Display(Name = "AddressPrecinct", ResourceType = typeof(Locale))]
        public string Precinct { get; set; }

        /// <summary>
        /// Gets or sets the postal code
        /// </summary>
        /// <value>The postal code.</value>
        [Display(Name = "AddressPostalCode", ResourceType = typeof(Locale))]
        public string PostalCode { get; set; }

        /// <summary>
		/// Converts a <see cref="EntityAddressViewModel"/> instance to a <see cref="EntityAddress"/> instance.
		/// </summary>
		/// <returns>Returns a <see cref="EntityAddress"/> instance.</returns>
        public EntityAddress ToEntityAddress()
        {
            return new EntityAddress(AddressUseKeys.Public, StreetAddress, Precinct, City, County, State, Country, PostalCode);
        }

        /// <summary>
        /// Represent the address as a string instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.StreetAddress))
                sb.AppendFormat("{0}, ", this.StreetAddress);
            if (!string.IsNullOrEmpty(this.Precinct))
                sb.AppendFormat("{0}, ", this.Precinct);
            if (!string.IsNullOrEmpty(this.City))
                sb.AppendFormat("{0}, ", this.City);
            if (!string.IsNullOrEmpty(this.County))
                sb.AppendFormat("{0}, ", this.County);
            if (!string.IsNullOrEmpty(this.State))
                sb.AppendFormat("{0}, ", this.State);
            if (!string.IsNullOrEmpty(this.Country))
                sb.AppendFormat("{0}, ", this.Country);
            if (!string.IsNullOrEmpty(this.PostalCode))
                sb.AppendFormat("{0}, ", this.PostalCode);

            if(sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
    }
}