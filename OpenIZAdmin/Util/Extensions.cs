using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenIZAdmin.Localization;

namespace OpenIZAdmin.Util
{
    /// <summary>
	/// Represents an extension class of view display purposes.
	/// </summary>
    public static class Extensions
    {
        /// <summary>
		/// Converts a boolean to a string representation 
		/// </summary>
		/// <returns>Returns a string based on the boolean value</returns>
        public static string ToLockoutStatus(this bool value)
        {
            return value ? Locale.Locked : Locale.Unlocked;
        }
    }
}