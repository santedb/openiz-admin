﻿using OpenIZAdmin.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Models.IntegrationModels
{
    /// <summary>
    /// Integration control model
    /// </summary>
    public class IntegrationControlModel
    {

        /// <summary>
        /// Gets or sets the applet content.
        /// </summary>
        [Display(Name = "File", ResourceType = typeof(Locale))]
        [Required(ErrorMessageResourceName = "FileRequired", ErrorMessageResourceType = typeof(Locale))]
        public HttpPostedFileBase TargetPopulationFile { get; set; }


    }
}