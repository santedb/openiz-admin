/*
 * Copyright 2016-2018 Mohawk College of Applied Arts and Technology
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may
 * obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * User: khannan
 * Date: 2018-5-7
 */

using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Security;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZAdmin.Services.Dataset
{
	/// <summary>
	/// Represents a data install action.
	/// </summary>
	[XmlType(nameof(DataInstallAction), Namespace = "http://openiz.org/data")]
	public abstract class DataInstallAction
	{
		/// <summary>
		/// Gets the name of the action.
		/// </summary>
		/// <value>The name of the action.</value>
		public abstract string ActionName { get; }

		/// <summary>
		/// Gets or sets the association which is the specified data for stuff that cannot be serialized.
		/// </summary>
		/// <value>The association.</value>
		[XmlElement("associate")]
		public List<DataAssociation> Association { get; set; }

		/// <summary>
		/// Gets the elements to be acted upon.
		/// </summary>
		/// <value>The element.</value>
		[XmlElement("ConceptReferenceTerm", typeof(ConceptReferenceTerm), Namespace = "http://openiz.org/model")]
		[XmlElement("ConceptName", typeof(ConceptName), Namespace = "http://openiz.org/model")]
		[XmlElement("EntityRelationship", typeof(EntityRelationship), Namespace = "http://openiz.org/model")]
		[XmlElement("Concept", typeof(Concept), Namespace = "http://openiz.org/model")]
		[XmlElement("ConceptSet", typeof(ConceptSet), Namespace = "http://openiz.org/model")]
		[XmlElement("ConceptRelationship", typeof(ConceptRelationship), Namespace = "http://openiz.org/model")]
		[XmlElement("AssigningAuthority", typeof(AssigningAuthority), Namespace = "http://openiz.org/model")]
		[XmlElement("ConceptClass", typeof(ConceptClass), Namespace = "http://openiz.org/model")]
		[XmlElement("SecurityPolicy", typeof(SecurityPolicy), Namespace = "http://openiz.org/model")]
		[XmlElement("SecurityRole", typeof(SecurityRole), Namespace = "http://openiz.org/model")]
		[XmlElement("SecurityUser", typeof(SecurityUser), Namespace = "http://openiz.org/model")]
		[XmlElement("ExtensionType", typeof(ExtensionType), Namespace = "http://openiz.org/model")]
		[XmlElement("CodeSystem", typeof(CodeSystem), Namespace = "http://openiz.org/model")]
		[XmlElement("ReferenceTerm", typeof(ReferenceTerm), Namespace = "http://openiz.org/model")]
		[XmlElement("IdentifierType", typeof(IdentifierType), Namespace = "http://openiz.org/model")]
		[XmlElement("UserEntity", typeof(UserEntity), Namespace = "http://openiz.org/model")]
		[XmlElement("Entity", typeof(Entity), Namespace = "http://openiz.org/model")]
		[XmlElement("Organization", typeof(Organization), Namespace = "http://openiz.org/model")]
		[XmlElement("Person", typeof(Person), Namespace = "http://openiz.org/model")]
		[XmlElement("Provider", typeof(Provider), Namespace = "http://openiz.org/model")]
		[XmlElement("Material", typeof(Material), Namespace = "http://openiz.org/model")]
		[XmlElement("ManufacturedMaterial", typeof(ManufacturedMaterial), Namespace = "http://openiz.org/model")]
		[XmlElement("Patient", typeof(Patient), Namespace = "http://openiz.org/model")]
		[XmlElement("Place", typeof(Place), Namespace = "http://openiz.org/model")]
		[XmlElement("Bundle", typeof(Bundle), Namespace = "http://openiz.org/model")]
		[XmlElement("Act", typeof(Act), Namespace = "http://openiz.org/model")]
		[XmlElement("SubstanceAdministration", typeof(SubstanceAdministration), Namespace = "http://openiz.org/model")]
		[XmlElement("QuantityObservation", typeof(QuantityObservation), Namespace = "http://openiz.org/model")]
		[XmlElement("CodedObservation", typeof(CodedObservation), Namespace = "http://openiz.org/model")]
		[XmlElement("EntityIdentifier", typeof(EntityIdentifier), Namespace = "http://openiz.org/model")]
		[XmlElement("TextObservation", typeof(TextObservation), Namespace = "http://openiz.org/model")]
		[XmlElement("PatientEncounter", typeof(PatientEncounter), Namespace = "http://openiz.org/model")]
		public IdentifiedData Element { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether errors should be ignored.
		/// </summary>
		/// <value><c>true</c> if errors should be ignored; otherwise, <c>false</c>.</value>
		[XmlAttribute("skipIfError")]
		public bool IgnoreErrors { get; set; }
	}
}