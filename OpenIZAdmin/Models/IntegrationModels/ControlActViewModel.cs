using Newtonsoft.Json;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Services;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models.EntityRelationshipModels;
using OpenIZAdmin.Services.Metadata.Concepts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenIZAdmin.Models.IntegrationModels
{
    /// <summary>
    /// Control act view model
    /// </summary>
    public class ControlActViewModel
    {
        /// <summary>
        /// Create a new view model
        /// </summary>
        public ControlActViewModel(Act act, IConceptService conceptService)
        {
            Id = act.Key.GetValueOrDefault();
            StartTime = act.StartTime;
            StopTime = act.StopTime;
            Status = (act.StatusConcept ?? conceptService.GetConcept(act.StatusConceptKey, true))?.Mnemonic;
            TypeName = (act.TypeConcept ?? conceptService.GetConcept(act.TypeConceptKey, true))?.ConceptNames.FirstOrDefault().Name;
            Objects = act.Participations.Select(p => new ActParticipationViewModel(p));

            if (act.Extensions.Any(o => o.ExtensionTypeKey == Constants.DetectedIssueExtensionTypeKey))
            {
                try
                {
                    var entityExtension = act.Extensions.First(e => e.ExtensionTypeKey == Constants.DetectedIssueExtensionTypeKey && e.ObsoleteVersionSequenceId == null);
                    var issues = JsonConvert.DeserializeObject<List<DetectedIssue>>(Encoding.UTF8.GetString(entityExtension.ExtensionValueXml));
                    this.Issues = issues;
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Unable to de-serialize the issue extensions: { e }");
                }
            }

            var exception = act.Tags?.FirstOrDefault(t => t.TagKey == "exception");
            if (exception != null)
                this.ErrorDetail = exception.Value;
        }

        /// <summary>
        /// Gets or sets the id of the object
        /// </summary>
        [Display(Name = "ImportEventId", ResourceType = typeof(Locale))]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the type name
        /// </summary>
        [Display(Name = "ImportEventType", ResourceType = typeof(Locale))]
        public String TypeName { get; set; }

        /// <summary>
        /// Start of time import
        /// </summary>
        [Display(Name = "ImportStart", ResourceType = typeof(Locale))]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// Stop time of the import
        /// </summary>
        [Display(Name = "ImportStop", ResourceType = typeof(Locale))]
        public DateTimeOffset? StopTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the object
        /// </summary>
        [Display(Name = "ImportStatus", ResourceType = typeof(Locale))]
        public string Status { get; set; }

        /// <summary>
		/// Gets or sets the areas served.
		/// </summary>
		/// <value>The areas served.</value>
        [Display(Name = "ImportObjects", ResourceType = typeof(Locale))]
        public IEnumerable<ActParticipationViewModel> Objects { get; set; }

        /// <summary>
        /// The user which created this run
        /// </summary>
        [Display(Name = "ImportUser", ResourceType = typeof(Locale))]
        public string CreatedBy { get; internal set; }

        /// <summary>
        /// Detected issues
        /// </summary>
        public List<DetectedIssue> Issues { get; internal set; }

        /// <summary>
        /// Error details
        /// </summary>
        public string ErrorDetail { get; private set;  }
    }
}