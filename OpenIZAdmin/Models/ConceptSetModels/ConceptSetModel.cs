using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OpenIZAdmin.Models.ConceptModels;

namespace OpenIZAdmin.Models.ConceptSetModels
{
	/// <summary>
	/// Represents a base concept set model.
	/// </summary>
	public abstract class ConceptSetModel
	{
		/// <summary>
		/// Gets or sets a list of concept of the concept set.
		/// </summary>
		public List<ConceptViewModel> Concepts { get; set; }

		/// <summary>
		/// Gets or sets the creation time of the concept set.
		/// </summary>
		[Display(Name = "CreationTime", ResourceType = typeof(Localization.Locale))]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the key of the concept.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the mnemonic of the concept.
		/// </summary>
		[Display(Name = "Mnemonic", ResourceType = typeof(Localization.Locale))]
		public virtual string Mnemonic { get; set; }

		/// <summary>
		/// Gets or sets the name of the concept set.
		/// </summary>
		[Display(Name = "Name", ResourceType = typeof(Localization.Locale))]
		public virtual string Name { get; set; }

		/// <summary>
		/// Get or sets the OID of the concept set.
		/// </summary>
		[Display(Name = "Oid", ResourceType = typeof(Localization.Locale))]
		public virtual string Oid { get; set; }

		/// <summary>
		/// Get or sets the URL of the concept set.
		/// </summary>
		[Display(Name = "Url", ResourceType = typeof(Localization.Locale))]
		public virtual string Url { get; set; }
	}
}