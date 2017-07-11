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
 * Date: 2017-7-10
 */

using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZAdmin.Core.Auditing.Model;
using OpenIZAdmin.Core.Auditing.Services;
using OpenIZAdmin.Core.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace OpenIZAdmin.Core.Auditing.Core
{
	/// <summary>
	/// Represents an audit helper.
	/// </summary>
	public abstract class CoreAuditServiceBase : ICoreAuditService
	{
		/// <summary>
		/// The security audit code constant.
		/// </summary>
		private const string SecurityAuditCode = "SecurityAuditCode";

		/// <summary>
		/// Gets the audit service.
		/// </summary>
		/// <value>The audit service.</value>
		protected static IAuditService AuditService => OpenIZAdminEngineContext.Current.Resolve<IAuditService>();

		/// <summary>
		/// Gets the context.
		/// </summary>
		/// <value>The context.</value>
		protected HttpContext Context { get; set; }

		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		public abstract void AuditGenericError(OutcomeIndicator outcomeIndicator, AuditCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception);

		/// <summary>
		/// Audits the generic error.
		/// </summary>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="exception">The exception.</param>
		public abstract void AuditGenericError(OutcomeIndicator outcomeIndicator, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, Exception exception);

		/// <summary>
		/// Creates the audit code.
		/// </summary>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <returns>Returns the created audit code.</returns>
		protected static AuditCode CreateAuditCode(EventTypeCode eventTypeCode)
		{
			var typeCode = typeof(EventTypeCode).GetRuntimeField(eventTypeCode.ToString()).GetCustomAttribute<XmlEnumAttribute>();
			return new AuditCode(typeCode.Name, SecurityAuditCode);
		}

		/// <summary>
		/// Adds the object information.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message.</param>
		/// <param name="idType">Type of the identifier.</param>
		/// <param name="lifecycle">The lifecycle.</param>
		/// <param name="role">The role.</param>
		/// <param name="type">The type.</param>
		/// <param name="objectKeyPropertyName">Name of the object key property.</param>
		/// <param name="objectKeyClassifier">The object key classifier.</param>
		/// <param name="includeDetail">if set to <c>true</c> [include detail].</param>
		/// <param name="objects">The objects.</param>
		protected void AddObjectInfo<T>(AuditData message, AuditableObjectIdType idType, AuditableObjectLifecycle lifecycle, AuditableObjectRole role, AuditableObjectType type, string objectKeyPropertyName, string objectKeyClassifier = null, bool includeDetail = false, T objects = default(T))
		{
			this.AddObjectInfoEx<T>(message, idType, lifecycle, role, type, objectKeyPropertyName, objectKeyClassifier, includeDetail, new List<T>() { objects });
		}

		/// <summary>
		/// Audit internal resources.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message">The message.</param>
		/// <param name="idType">Type of the identifier.</param>
		/// <param name="lifecycle">The lifecycle.</param>
		/// <param name="role">The role.</param>
		/// <param name="type">The type.</param>
		/// <param name="objectKeyPropertyName">Name of the object key property.</param>
		/// <param name="objectKeyClassifier">The object key classifier.</param>
		/// <param name="includeDetail">if set to <c>true</c> [include detail].</param>
		/// <param name="objects">The objects.</param>
		/// <exception cref="System.ArgumentNullException">objectKeyPropertyName
		/// or
		/// message</exception>
		/// <exception cref="System.ArgumentException">objectKeyPropertyName</exception>
		protected void AddObjectInfoEx<T>(AuditData message, AuditableObjectIdType idType, AuditableObjectLifecycle lifecycle, AuditableObjectRole role, AuditableObjectType type, string objectKeyPropertyName, string objectKeyClassifier = null, bool includeDetail = false, IEnumerable<T> objects = null)
		{
			if (objects == null)
				return;

			// Validate parameters
			if (objectKeyPropertyName == null)
				throw new ArgumentNullException(nameof(objectKeyPropertyName));

			if (message == null)
				throw new ArgumentNullException(nameof(message));

			// Get key property
			var objectKeyProperty = typeof(T).GetProperty(objectKeyPropertyName);
			if (objectKeyProperty == null)
				throw new ArgumentException("objectKeyPropertyName");

			var idScope = typeof(T).Name;

			// Audit objects
			foreach (var obj in objects)
			{
				if (obj == null)
				{
					continue;
				}

				var auditableObject = new AuditableObject
				{
					IDTypeCode = idType,
					LifecycleType = lifecycle,
					Role = role,
					Type = type,
					ObjectId = idType == AuditableObjectIdType.Uri ? objectKeyProperty.GetValue(obj).ToString() : $"{objectKeyProperty.GetValue(obj)}^^^{objectKeyClassifier ?? idScope}"
				};

				if (includeDetail)
				{
					string typeName = null;

					if (obj.GetType().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any() && obj.GetType().FullName.Contains("AnonymousType"))
					{
						typeName = objectKeyClassifier ?? idScope;
					}

					var detail = CreateObjectDataExtension(obj, name: typeName);

					auditableObject.ObjectData.Add(detail);
				}

				message.AuditableObjects.Add(auditableObject);
			}
		}

		/// <summary>
		/// Creates the base audit.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <returns>Returns the created base audit data.</returns>
		protected virtual AuditData CreateBaseAudit(ActionType actionType, AuditCode eventTypeCode, EventIdentifierType eventIdentifierType)
		{
			return new AuditData
			{
				ActionCode = actionType,
				CorrelationToken = Guid.NewGuid(),
				EventTypeCode = eventTypeCode,
				EventIdentifier = eventIdentifierType,
				Timestamp = DateTime.Now
			};
		}

		/// <summary>
		/// Creates the base audit.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created base audit data.</returns>
		protected virtual AuditData CreateBaseAudit(ActionType actionType, AuditCode eventTypeCode, EventIdentifierType eventIdentifierType, OutcomeIndicator outcomeIndicator)
		{
			var audit = CreateBaseAudit(actionType, eventTypeCode, eventIdentifierType);

			audit.Outcome = outcomeIndicator;

			return audit;
		}

		/// <summary>
		/// Creates the base audit.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="eventTypeCode">The event type code.</param>
		/// <param name="eventIdentifierType">Type of the event identifier.</param>
		/// <param name="outcomeIndicator">The outcome indicator.</param>
		/// <returns>Returns the created base audit data.</returns>
		protected virtual AuditData CreateBaseAudit(ActionType actionType, EventTypeCode eventTypeCode, EventIdentifierType eventIdentifierType, OutcomeIndicator outcomeIndicator)
		{
			return this.CreateBaseAudit(actionType, CreateAuditCode(eventTypeCode), eventIdentifierType, outcomeIndicator);
		}

		/// <summary>
		/// Creates the base auditable object.
		/// </summary>
		/// <param name="idType">Type of the identifier.</param>
		/// <param name="lifecycleType">Type of the lifecycle.</param>
		/// <param name="role">The role.</param>
		/// <param name="type">The type.</param>
		/// <returns>AuditableObject.</returns>
		protected AuditableObject CreateBaseAuditableObject(AuditableObjectIdType idType, AuditableObjectLifecycle lifecycleType, AuditableObjectRole role, AuditableObjectType type)
		{
			return this.CreateBaseAuditableObject(idType, lifecycleType, null, role, type);
		}

		/// <summary>
		/// Creates the base auditable object.
		/// </summary>
		/// <param name="idType">Type of the identifier.</param>
		/// <param name="lifecycleType">Type of the lifecycle.</param>
		/// <param name="objectId">The object identifier.</param>
		/// <param name="role">The role.</param>
		/// <param name="type">The type.</param>
		/// <returns>Returns the created auditable object instance.</returns>
		protected AuditableObject CreateBaseAuditableObject(AuditableObjectIdType idType, AuditableObjectLifecycle lifecycleType, string objectId, AuditableObjectRole role, AuditableObjectType type)
		{
			return new AuditableObject
			{
				IDTypeCode = idType,
				LifecycleType = lifecycleType,
				ObjectId = objectId,
				Role = role,
				Type = type
			};
		}

		/// <summary>
		/// Creates the object data extension.
		/// </summary>
		/// <param name="detail">The detail.</param>
		/// <param name="enforceType">Type of the enforce.</param>
		/// <param name="name">The name.</param>
		/// <returns>Returns the created <see cref="ObjectDataExtension" /> instance.</returns>
		protected ObjectDataExtension CreateObjectDataExtension(object detail, Type enforceType = null, string name = null)
		{
			var retVal = new ObjectDataExtension();

			enforceType = enforceType ?? detail.GetType();

			if (enforceType.Namespace == "System.Data.Entity.DynamicProxies")
			{
				enforceType = enforceType.BaseType;
			}

			using (var ms = new MemoryStream())
			{
				var writer = XmlWriter.Create(ms);

				writer.WriteStartElement(name ?? enforceType.Name);

				this.WriteObject(writer, detail);

				writer.WriteEndDocument();

				writer.Close();

				retVal.Value = ms.GetBuffer().Take((int)ms.Length).ToArray();
			}

			retVal.Key = name ?? enforceType.AssemblyQualifiedName;

			return retVal;
		}

		/// <summary>
		/// Writes the object.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="detail">The detail.</param>
		private void WriteObject(XmlWriter writer, object detail)
		{
			foreach (var prop in detail.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (prop.GetIndexParameters().Length != 0) continue;
				object value = prop.GetValue(detail);
				if (value == null) continue;
				if (prop.PropertyType.IsPrimitive ||
					prop.PropertyType == typeof(String) ||
					prop.PropertyType == typeof(Guid) ||
					prop.PropertyType.FullName.StartsWith("System.Nullable") ||
					prop.PropertyType == typeof(System.Collections.Specialized.NameValueCollection))
				{
					writer.WriteStartElement(prop.Name);

					writer.WriteValue(value.ToString());

					writer.WriteEndElement();
				}
			}
		}
	}
}