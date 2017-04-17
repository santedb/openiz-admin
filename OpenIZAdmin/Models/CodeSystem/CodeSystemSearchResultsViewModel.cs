using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZAdmin.Models.Core;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZAdmin.Models.CodeSystem
{
    /// <summary>
	/// Represents a view model for a code system search result.
	/// </summary>
    public class CodeSystemSearchResultsViewModel : CodeSystemModel
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="CodeSystemSearchResultsViewModel"/> class.
		/// </summary>
		public CodeSystemSearchResultsViewModel()
        {
            //this.Names = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeSystemSearchResultsViewModel"/> class
        /// with a specific <see cref="CodeSystem"/> instance.
        /// </summary>
        /// <param name="codeSystem">The <see cref="OpenIZ.Core.Model.DataTypes.CodeSystem"/> instance.</param>
        public CodeSystemSearchResultsViewModel(OpenIZ.Core.Model.DataTypes.CodeSystem codeSystem) : this()
        {
            Name = codeSystem.Name;
            Oid = codeSystem.Oid;
            Description = codeSystem.Description;
            //Domain = codeSystem.Authority; ??????????????????????????
            Url = codeSystem.Url;
            Version = codeSystem.VersionText;            
        }
    }
}