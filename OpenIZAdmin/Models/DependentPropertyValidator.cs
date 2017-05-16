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
 * User: Andrew
 * Date: 2017-4-20
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Models
{
    /// <summary>
	/// Custom validation for two properties.
	/// </summary>
    public class DependentPropertyValidator : ValidationAttribute//, IClientValidatable
    { 
        /// <summary>
		/// Initializes a new instance of the <see cref="DependentPropertyValidator"/> class.
		/// </summary>
        public DependentPropertyValidator(string prop1)
        {
            this._comparisonProperty = prop1;            
        }

        private readonly string _comparisonProperty;

        /// <inheritdoc />
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {            
            //AddReferenceTerm   
            var currentValue = value?.ToString();
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            //RelationshipType
            var comparisonValue = property.GetValue(validationContext.ObjectInstance)?.ToString();

            if (string.IsNullOrWhiteSpace(currentValue) && string.IsNullOrWhiteSpace(comparisonValue)) return ValidationResult.Success;

            if (!string.IsNullOrWhiteSpace(currentValue) && !string.IsNullOrWhiteSpace(comparisonValue)) return ValidationResult.Success;            

            return new ValidationResult(FormatErrorMessage((validationContext.DisplayName)));
        }

        ////new method
        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientValidationRule {ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())};
        //    rule.ValidationParameters.Add("comparisonproperty", _comparisonProperty);
        //    rule.ValidationType = "compare";
        //    yield return rule;
        //}



    }
}