using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Models.Core
{
    /// <summary>
    /// Represents an edit entity address view model.
    /// </summary>
    public class EditEntityAddressViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditEntityAddressViewModel"/> class.
        /// </summary>
        public EditEntityAddressViewModel()
        {
            this.CountryList = new List<SelectListItem>();
            this.CountyList = new List<SelectListItem>();
            this.CityList = new List<SelectListItem>();
            this.StateList = new List<SelectListItem>();
            this.PrecinctList = new List<SelectListItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditEntityAddressViewModel"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public EditEntityAddressViewModel(EntityAddress address) : this()
        {
            this.Address = new EntityAddressViewModel(address);

            if (!string.IsNullOrEmpty(this.Address.Country))
            {
                this.CountryList.Add(new SelectListItem() { Text = this.Address.Country, Value = this.Address.Country, Selected = true });
            }

            if (!string.IsNullOrEmpty(this.Address.County))
            {
                this.CountyList.Add(new SelectListItem() { Text = this.Address.County, Value = this.Address.County, Selected = true });
            }

            if (!string.IsNullOrEmpty(this.Address.City))
            {
                this.CityList.Add(new SelectListItem() { Text = this.Address.City, Value = this.Address.City, Selected = true });
            }

            if (!string.IsNullOrEmpty(this.Address.State))
            {
                this.StateList.Add(new SelectListItem() { Text = this.Address.State, Value = this.Address.State, Selected = true });
            }

            if (!string.IsNullOrEmpty(this.Address.Precinct))
            {
                this.PrecinctList.Add(new SelectListItem() { Text = this.Address.Precinct, Value = this.Address.Precinct, Selected = true });
            }
        }

        /// <summary>
        /// Gets or sets the address
        /// </summary>
        /// <value>The address.</value>
        public EntityAddressViewModel Address { get; set; }

        /// <summary>
        /// Gets or sets the list of countries.
        /// </summary>
        public List<SelectListItem> CountryList { get; set; }

        /// <summary>
        /// Gets or sets the list of counties.
        /// </summary>
        public List<SelectListItem> CountyList { get; set; }

        /// <summary>
        /// Gets or sets the list of cities.
        /// </summary>
        public List<SelectListItem> CityList { get; set; }

        /// <summary>
        /// Gets or sets the list of states.
        /// </summary>
        public List<SelectListItem> StateList { get; set; }

        /// <summary>
        /// Gets or sets the list of countries.
        /// </summary>
        public List<SelectListItem> PrecinctList { get; set; }
    }
}