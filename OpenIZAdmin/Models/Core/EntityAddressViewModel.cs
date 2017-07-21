using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using System.Text;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
    /// Entity address view model
    /// </summary>
    public class EntityAddressViewModel
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public EntityAddressViewModel()
        {

        }

        /// <summary>
        /// Creates entity address view model
        /// </summary>
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
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the county
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or set sthe city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the street address
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the precinct
        /// </summary>
        public string Precinct { get; set; }

        /// <summary>
        /// Postal code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Represent as a string
        /// </summary>
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

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
    }
}