/*
 * Copyright 2016-2017 Mohawk College of Applied Arts and Technology
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
 * User: Nityan
 * Date: 2016-8-1
 */

using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.MaterialModels;
using OpenIZAdmin.Models.MaterialModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Provides a utility for managing materials.
	/// </summary>
	public static class MaterialUtil
	{
        /// <summary>        
        /// </summary>
        /// <param name="imsiClient">The Imsi service client.</param>        
        /// <returns>Returns a CreateMaterialModel object for the new material.</returns>
        public static CreateMaterialModel ToCreateMaterialModel(ImsiServiceClient imsiClient)
        {
            var model = new CreateMaterialModel();

            var formConcepts = imsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.Form && m.ObsoletionTime == null);

            model.FormConcepts.AddRange(formConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Value = c.Key.Value.ToString() }));

            var quantityConcepts = imsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null);

            model.QuantityConcepts.AddRange(quantityConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Value = c.Key.Value.ToString() }));

            return model;
        }

        /// <summary>
        /// Converts a <see cref="CreateMaterialModel"/> instance to a <see cref="Material"/> instance.
        /// </summary>
        /// <param name="model">The CreateMaterialModel to convert.</param>        
        /// <returns>Returns a material object with the new details.</returns>
        public static Material ToCreateMaterial(CreateMaterialModel model)
        {
            var material = new Material
				{
					Key = Guid.NewGuid(),
					Names = new List<EntityName>
					{
						new EntityName(NameUseKeys.OfficialRecord, model.Name)
					},
					FormConcept = new Concept
					{
						Key = Guid.Parse(model.FormConcept),
					},
					QuantityConcept = new Concept
					{
						Key = Guid.Parse(model.QuantityConcept),
					}
				};

            return material;
        }
       
        /// <summary>
        /// Converts a <see cref="Material"/> instance to a <see cref="EditMaterialModel"/> instance.
        /// </summary>
        /// <param name="imsiClient">The Imsi service client.</param>        
        /// <param name="material">The material to convert.</param>
        /// <returns>Returns a EditMaterialModel view model.</returns>
        public static EditMaterialModel ToEditMaterialModel(ImsiServiceClient imsiClient, Material material)
        {
            var model = new EditMaterialModel
            {
                Name = material.Names.FirstOrDefault().Component.FirstOrDefault().Value
            };

            var formConcepts = imsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.Form && m.ObsoletionTime == null);
            model.FormConcepts.AddRange(formConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Selected = material.FormConceptKey == c.Key, Value = c.Key.Value.ToString() }));

            var quantityConcepts = imsiClient.Query<Concept>(m => m.Class.Key == ConceptClassKeys.UnitOfMeasure && m.ObsoletionTime == null);
            model.QuantityConcepts.AddRange(quantityConcepts.Item.OfType<Concept>().Select(c => new SelectListItem { Text = c.Mnemonic, Selected = material.QuantityConceptKey == c.Key, Value = c.Key.Value.ToString() }));

            return model;
        }

        /// <summary>
        /// Converts a <see cref="Material"/> instance to a <see cref="MaterialSearchResultViewModel"/> instance.
        /// </summary>
        /// <param name="material">The material to convert.</param>
        /// <returns>Returns a material search result view model.</returns>
        public static MaterialSearchResultViewModel ToMaterialSearchResultViewModel(Material material)
		{
			var viewModel = new MaterialSearchResultViewModel
			{
				CreationTime = material.CreationTime.DateTime,
				Key = material.Key.GetValueOrDefault(Guid.Empty),
				Name = string.Join(", ", material.Names.SelectMany(m => m.Component).Select(c => c.Value)),
				VersionKey = material.VersionKey
			};

			return viewModel;
		}

        /// <summary>
        /// Converts a <see cref="EditMaterialModel"/> instance to a <see cref="Material"/> instance.
        /// </summary>
        /// <param name="model">The EditMaterialModel to convert.</param>
        /// <param name="material">The material to convert.</param>
        /// <returns>Returns a material object with the updated details.</returns>
        public static Material ToUpdateMaterial(EditMaterialModel model, Material material)
        {
            material.Names = new List<EntityName>
            {
                new EntityName(NameUseKeys.OfficialRecord, model.Name)
            };

            material.FormConcept = new Concept
            {
                Key = Guid.Parse(model.FormConcept),
            };

            material.QuantityConcept = new Concept
            {
                Key = Guid.Parse(model.QuantityConcept),
            };

            return material;
        }


        /// <summary>
        /// Converts a <see cref="Material"/> instance to a <see cref="MaterialViewModel"/> instance.
        /// </summary>        
        /// <param name="material">The material to convert.</param>
        /// <param name="key">The Guid identifier of the material.</param>
        /// <param name="versionKey">The Guid version key of the material.</param>
        /// <returns>Returns a material object with the updated details.</returns>
        public static MaterialViewModel ToMaterialViewModel(Material material, Guid key, Guid versionKey)
        {
            var model = new MaterialViewModel
            {
                Key = key,
                Name = string.Join(" ", material.Names.SelectMany(n => n.Component).Select(c => c.Value)),
                FormConcept = material.FormConcept.Mnemonic,
                QuantityConcept = material.QuantityConcept.Mnemonic,
                VersionKey = versionKey
            };

            return model;
        }

    }
}