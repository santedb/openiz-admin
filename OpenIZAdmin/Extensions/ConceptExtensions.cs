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

using OpenIZ.Core.Model.DataTypes;
using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace OpenIZAdmin.Extensions
{
	/// <summary>
	/// Represents a collection of extensions for the <see cref="Concept"/> and <see cref="ConceptSet"/> classes.
	/// </summary>
	public static class ConceptExtensions
	{
		/// <summary>
		/// Converts a <see cref="IEnumerable{T}" /> of <see cref="Concept" /> to a <see cref="IEnumerable{T}" /> of <see cref="SelectListItem" />.
		/// </summary>
		/// <param name="source">The list of concepts to convert.</param>
		/// <param name="languageCode">The language code.</param>
		/// <param name="selectedExpression">An expression which evaluates to set selected items.</param>
		/// <param name="skipDefaultEntry">if set to <c>true</c> [skip default entry].</param>
		/// <returns>Returns a <see cref="IEnumerable{T}" /> of <see cref="SelectListItem" />.</returns>
		/// <exception cref="System.ArgumentNullException">If the source is null.</exception>
		public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<Concept> source, string languageCode = "en", Expression<Func<Concept, bool>> selectedExpression = null, bool skipDefaultEntry = false)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source), Locale.ValueCannotBeNull);
			}

			var selectList = new List<SelectListItem>();

			if (!skipDefaultEntry)
			{
				selectList.Add(new SelectListItem { Text = string.Empty, Value = string.Empty });
			}

			// force set the language code if null
			if (languageCode == null)
			{
				languageCode = "en";
			}

			foreach (var concept in source)
			{
				var item = new SelectListItem
				{
					Selected = selectedExpression != null && Convert.ToBoolean(selectedExpression.Compile().DynamicInvoke(concept)),
					Value = concept.Key.ToString()
				};

				// if the concept has names in the given language code
				if (concept.ConceptNames?.Any(c => c.Language == languageCode) == true)
				{
					item.Text = string.Join(" ", concept.ConceptNames.Where(cn => cn.Language == languageCode).Select(n => n.Name));
				}
				// if the concept has names in english
				else if (concept.ConceptNames?.Any(c => c.Language == "en") == true)
				{
					item.Text = string.Join(" ", concept.ConceptNames.Where(cn => cn.Language == "en").Select(n => n.Name));
				}
				// otherwise, default to the mnemonic of the concept
				else
				{
					item.Text = concept.Mnemonic;
				}

				selectList.Add(item);
			}

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
				throw new ArgumentNullException(nameof(source), Locale.ValueCannotBeNull);
			}

			var selectList = new List<SelectListItem>
			{
				new SelectListItem { Text = string.Empty, Value = string.Empty }
			};

			selectList.AddRange(source.Select(c => new SelectListItem { Text = c.Name, Value = c.Key.ToString() }));

			return selectList.OrderBy(c => c.Text);
		}
	}
}