/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-15
 */
using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Models.ProviderModels;
using OpenIZAdmin.Models.ProviderModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Util
{
    /// <summary>
    /// Provides a utility for managing providers.
    /// </summary>
    public static class ProviderUtil
	{
        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.ProviderModels.CreateProviderModel"/> to a <see cref="OpenIZ.Core.Model.Roles.Provider"/>.
        /// </summary>
        /// <param name="model">The CreateProviderModel object to convert.</param>
        /// <returns>Returns a Provider object.</returns>
        public static Provider ToProvider(CreateProviderModel model)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Converts a <see cref="OpenIZAdmin.Models.ProviderModels.EditProviderModel"/> to a <see cref="OpenIZ.Core.Model.Roles.Provider"/>.
        /// </summary>
        /// <param name="model">The EditProviderModel object to convert.</param>
        /// <returns>Returns a Provider object.</returns>
        public static Provider ToProvider(EditProviderModel model)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Converts a <see cref="OpenIZ.Core.Model.Roles.Provider"/> to a <see cref="OpenIZAdmin.Models.ProviderModels.ViewModels.ProviderViewModel"/>.
        /// </summary>
        /// <param name="provider">The Provider object to convert.</param>
        /// <returns>Returns a ProviderViewModel object.</returns>
		public static ProviderViewModel ToProviderViewModel(Provider provider)
		{
            ProviderViewModel viewModel = new ProviderViewModel();
            
            viewModel.Name = string.Join(" ", provider.Names.SelectMany(x => x.Component).Select(m => m.Value));
            viewModel.UserId = provider.Identifiers.FirstOrDefault().Value;
            viewModel.Key = provider.Identifiers.FirstOrDefault().Key;
            viewModel.VersionKey = provider.VersionKey;
            viewModel.ProviderSpecialty = (provider.ProviderSpecialty != null) ? string.Join(" ", provider.ProviderSpecialty.ConceptNames.Select(m => m.Name)) : string.Empty;
            viewModel.CreationTime = provider.CreationTime;

            return viewModel;            
		}

        /// <summary>
		/// Gets a provider entity.
		/// </summary>
		/// <param name="client">The IMSI service client.</param>
		/// <param name="key">The GUID key of the provider to retrieve.</param>
        /// <param name="versionKey">The version id of the provider entry to retrieve.</param>
		/// <returns>Returns a provider entity or null if no provider entity is found.</returns>		
        public static Provider GetProviderEntity(ImsiServiceClient client, string key, string versionKey)
        {
            Provider provider = null;

            try
            {
                var bundle = client.Query<Provider>(u => u.Identifiers.FirstOrDefault().Value == key);

                bundle.Reconstitute();

                provider = bundle.Item.OfType<Provider>().Cast<Provider>().FirstOrDefault();
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("Unable to retrieve provider entity: {0}", e.StackTrace);
#endif
                Trace.TraceError("Unable to retrieve provider entity: {0}", e.Message);
            }

            return provider;
        }

        /// <summary>
		/// Converts a <see cref="OpenIZ.Core.Model.Roles.Provider"/> to a <see cref="OpenIZAdmin.Models.ProviderModels.EditProviderModel"/>.
		/// </summary>
		/// <param name="providerEntity">The provider entity to convert to a provider user model.</param>
		/// <returns>Returns an EditProviderModel model.</returns>
		public static EditProviderModel ToEditProviderModel(Provider providerEntity)
        {
            EditProviderModel model = new EditProviderModel();

            //model.FamilyNames = provider.Names.Where(n => n.NameUse.Key).SelectMany(n => n.Component).Where(c => c.ComponentType.Key == NameComponentKeys.Family).Select(c => c.Value).ToList();

            //model.Email = userEntity.SecurityUser.Email;
            //model.FacilityId = userEntity.Relationships.Where(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).Select(r => r.Key).FirstOrDefault()?.ToString();
            //model.FamilyNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentType.Key == NameComponentKeys.Family).Select(c => c.Value).ToList();
            //model.GivenNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentType.Key == NameComponentKeys.Given).Select(c => c.Value).ToList();

            //model.Roles = userEntity.SecurityUser.Roles.Select(r => r.Name);
            //model.Username = userEntity.SecurityUser.UserName;
            //model.UserId = provider.SecurityUserKey.GetValueOrDefault(Guid.Empty);

            return model;
        }       
    }
}