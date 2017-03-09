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
 * Date: 2017-2-17
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Represents a collection of extensions for the <see cref="Concept"/> and <see cref="ConceptSet"/> classes.
	/// </summary>
	public static class ConceptExtensions
	{
		/// <summary>
		/// Converts a <see cref="ConceptSet"/> to a <see cref="IEnumerable{T}"/> of <see cref="SelectListItem"/>.
		/// </summary>
		/// <param name="source">The concept set to convert.</param>
		/// <returns>Returns a <see cref="IEnumerable{T}"/> of <see cref="SelectListItem"/>.</returns>
		public static IEnumerable<SelectListItem> ToSelectList(this ConceptSet source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source), "Value cannot be null");
			}

			return source.Concepts.ToSelectList();
		}

		/// <summary>
		/// Converts a <see cref="IEnumerable{T}" /> of <see cref="Concept" /> to a <see cref="IEnumerable{T}" /> of <see cref="SelectListItem" />.
		/// </summary>
		/// <param name="source">The list of concepts to convert.</param>
		/// <param name="selectedExpression">An expression which evaluates to set selected items.</param>
		/// <returns>Returns a <see cref="IEnumerable{T}" /> of <see cref="SelectListItem" />.</returns>
		/// <exception cref="System.ArgumentNullException">If the source is null.</exception>
		public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<Concept> source, Expression<Func<Concept, bool>> selectedExpression = null)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source), "Value cannot be null");
			}

			var selectList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			selectList.AddRange(
				selectedExpression != null ?
				source.Select(c => new SelectListItem
				{
					Selected = Convert.ToBoolean(selectedExpression.Compile().DynamicInvoke(c)),
					Text = c.ConceptNames.Any() ? string.Join(" ", c.ConceptNames.Select(n => n.Name)) : c.Mnemonic,
					Value = c.Key.ToString()
				}) :
				source.Select(c => new SelectListItem
				{
					Text = c.ConceptNames.Any() ? string.Join(" ", c.ConceptNames.Select(n => n.Name)) : c.Mnemonic,
					Value = c.Key.ToString()
				}));

			return selectList.OrderBy(c => c.Text);
		}

		/// <summary>
		/// Converts a <see cref="IEnumerable{T}"/> of <see cref="ConceptClass"/> to a <see cref="IEnumerable{T}"/> of <see cref="SelectListItem"/>.
		/// </summary>
		/// <param name="source">The list of concept classes to convert.</param>
		/// <returns>Returns a <see cref="IEnumerable{T}"/> of <see cref="SelectListItem"/>.</returns>
		public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<ConceptClass> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source), "Value cannot be null");
			}

			var selectList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			selectList.AddRange(source.Select(c => new SelectListItem { Text = c.Name, Value = c.Key.ToString() }));

			return selectList;
		}
	}
}