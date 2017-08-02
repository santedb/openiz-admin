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
 * Date: 2017-8-2
 */
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;

namespace OpenIZAdmin.Util
{
	/// <summary>
	/// Represents a utility for JWT tokens.
	/// </summary>
	public static class JwtUtil
	{
		/// <summary>
		/// Determines whether a JWT token is expired.
		/// </summary>
		/// <param name="token">The JWT token.</param>
		/// <returns>Returns true if the token is expired.</returns>
		/// <exception cref="System.ArgumentException">If the token is in an invalid format.</exception>
		internal static bool IsExpired(string token)
		{
			JwtSecurityToken securityToken;

			if (!TryParse(token, out securityToken))
			{
				throw new ArgumentException($"Unable to parse token: { token }");
			}

			// is the token expired?
			return securityToken.ValidTo <= DateTime.UtcNow;
		}

		/// <summary>
		/// Attempts to parse a JWT token.
		/// </summary>
		/// <param name="token">The JWT token to parse.</param>
		/// <param name="parsedToken">The parsed token.</param>
		/// <returns>Returns true if the token is parsed successfully.</returns>
		internal static bool TryParse(string token, out JwtSecurityToken parsedToken)
		{
			bool status;

			try
			{
				parsedToken = new JwtSecurityToken(token);
				status = true;
			}
			catch
			{
				// unable to parse token.
				status = false;
				parsedToken = null;
			}

			return status;
		}
	}
}