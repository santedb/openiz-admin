using OpenIZ.Core.Model.Entities;
using System;
using System.Linq;

namespace OpenIZAdmin.Models.PlaceModels
{
	/// <summary>
	/// Represents a related place model.
	/// </summary>
	public class RelatedPlaceModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RelatedPlaceModel"/> class.
		/// </summary>
		public RelatedPlaceModel()
		{
		}

		public RelatedPlaceModel(Place place)
		{
			this.CreationTime = place.CreationTime.DateTime;
			this.Id = place.Key.Value;
			this.Name = string.Join(" ", place.Names.SelectMany(n => n.Component).Select(c => c.Value));

			if (place.TypeConcept != null)
			{
				this.Type = string.Join(" ", place.TypeConcept.ConceptNames.Select(c => c.Name));
			}
		}

		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the key of the related place.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the related place.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type of the related place.
		/// </summary>
		public string Type { get; set; }
	}
}