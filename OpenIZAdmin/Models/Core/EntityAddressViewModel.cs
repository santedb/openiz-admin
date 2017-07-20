using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;

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
            this.Country = address.Component?.Find(o => o.ComponentTypeKey == AddressComponentKeys.Country)?.Value;
            this.City = address.Component?.Find(o => o.ComponentTypeKey == AddressComponentKeys.City)?.Value;
            this.County= address.Component?.Find(o => o.ComponentTypeKey == AddressComponentKeys.County)?.Value;
            this.State = address.Component?.Find(o => o.ComponentTypeKey == AddressComponentKeys.State)?.Value;
            this.StreetAddress = address.Component?.Find(o => o.ComponentTypeKey == AddressComponentKeys.StreetAddressLine)?.Value;
            this.Precinct = address.Component?.Find(o => o.ComponentTypeKey == AddressComponentKeys.Precinct)?.Value;
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
    }
}