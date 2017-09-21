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
 * User: khannan
 * Date: 2017-3-27
 */

using OpenIZ.Core.Model.Entities;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Localization;
using System;
using System.Linq;
using System.Reflection;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Provides operations for managing associations.
	/// </summary>
	/// <seealso cref="OpenIZAdmin.Controllers.BaseController" />
	[TokenAuthorize]
	public abstract class EntityBaseController : BaseController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityBaseController"/> class.
		/// </summary>
		public EntityBaseController() : base()
		{
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>Returns the entity instance.</returns>
		protected virtual Entity GetEntity(Guid id, Type modelType)
		{
			var getMethod = this.ImsiClient.GetType().GetRuntimeMethod("Get", new Type[] { typeof(Guid), typeof(Guid?) }).MakeGenericMethod(modelType);

			return getMethod.Invoke(this.ImsiClient, new object[] { id, null }) as Entity;
		}

		/// <summary>
		/// Gets the type of the model.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>Returns the type for a given model type.</returns>
		/// <exception cref="System.ArgumentNullException">If the type is null or empty.</exception>
		/// <exception cref="System.ArgumentException">If the model type is not supported.</exception>
		protected virtual Type GetModelType(string type)
		{
			if (string.IsNullOrEmpty(type) || string.IsNullOrWhiteSpace(type))
				throw new ArgumentNullException(nameof(type), Locale.ValueCannotBeNull);

			Type modelType;

			switch (type.ToLower())
			{
				case "manufacturedmaterial":
					modelType = typeof(ManufacturedMaterial);
					break;

				case "material":
					modelType = typeof(Material);
					break;

				case "place":
					modelType = typeof(Place);
					break;

				case "organization":
					modelType = typeof(Organization);
					break;

				default:
					throw new ArgumentException($"Unsupported type: { type }");
			}

			return modelType;
		}

		/// <summary>
		/// Updates the entity.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns>Returns the updated entity.</returns>
		protected T UpdateEntity<T>(Entity entity) where T : Entity
		{
			return this.UpdateEntity(entity, typeof(T)) as T;
		}

		/// <summary>
		/// Updates the entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>Returns the updated entity.</returns>
		protected virtual Entity UpdateEntity(Entity entity, Type modelType)
		{
			// set the creation time
			entity.CreationTime = DateTimeOffset.Now;

			// remove all the relationships where I am the target entity
			entity.Relationships.RemoveAll(r => r.TargetEntityKey == entity.Key);

			// null out the version key
			entity.VersionKey = null;

			var updateMethod = this.ImsiClient.GetType().GetRuntimeMethods().First(m => m.Name == "Update" && m.IsGenericMethod).MakeGenericMethod(modelType);

			var updatedEntity = updateMethod.Invoke(this.ImsiClient, new object[] { entity }) as Entity;

			MvcApplication.MemoryCache.Set(updatedEntity.Key.ToString(), updatedEntity, MvcApplication.CacheItemPolicy);

			return updatedEntity;
		}
	}
}