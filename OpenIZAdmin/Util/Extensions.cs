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
		/// Converts a boolean to a string representation to display account status 
		/// </summary>
		/// <returns>Returns a string based on the boolean value</returns>
        public static string ToLockoutStatus(this bool value)
        {
            return value ? Locale.Locked : Locale.Unlocked;
        }

        /// <summary>
		/// Converts a boolean to a string representation to display user profile activation/deactivation 
		/// </summary>
		/// <returns>Returns a string based on the boolean value</returns>
        public static string ToObsoleteStatus(this bool value)
        {
            return value ? Locale.Deactivated : Locale.Active;
        }

        /// <summary>
        /// Converts a boolean to a string representation to display Yes or No 
        /// </summary>
        /// <returns>Returns a string based on the boolean value</returns>
        public static string ToYesOrNo(this bool value)
        {
            return value ? Locale.Yes : Locale.No;
        }
    }
}