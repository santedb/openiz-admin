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
 * User: fyfej
 * Date: 2017-7-21
 */

using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Core;
using System;
using System.Security;
using System.Security.Claims;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace OpenIZAdmin.Services.Security
{
	/// <summary>
	/// Represents a policy permission.
	/// </summary>
	/// <seealso cref="System.Security.IPermission" />
	/// <seealso cref="System.Security.Permissions.IUnrestrictedPermission" />
	[Serializable]
	public class PolicyPermission : IPermission, IUnrestrictedPermission
	{
		/// <summary>
		/// The principal instance.
		/// </summary>
		private readonly IPrincipal principal;

		/// <summary>
		/// The trace source.
		/// </summary>
		private readonly Tracer traceSource = Tracer.GetTracer(typeof(PolicyPermission));

		/// <summary>
		/// The flag indicating whether the policy is unrestricted.
		/// </summary>
		private bool isUnrestricted;

		/// <summary>
		/// The policy identifier.
		/// </summary>
		private string policyId;

		/// <summary>
		/// Initializes a new instance of the <see cref="PolicyPermission"/> class.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="policyId">The policy identifier.</param>
		/// <param name="principal">The principal.</param>
		public PolicyPermission(PermissionState state, String policyId, IPrincipal principal)
		{
			this.isUnrestricted = state == PermissionState.Unrestricted;
			this.policyId = policyId;
			this.principal = principal;
		}

		/// <summary>
		/// Tries to demand <paramref name="policyId" />
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <param name="policyId">The policy identifier.</param>
		/// <returns><c>true</c> if the demand is successful, <c>false</c> otherwise.</returns>
		public static bool TryDemand(IPrincipal principal, String policyId)
		{
			try
			{
				new PolicyPermission(PermissionState.Unrestricted, policyId, principal).Demand();
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Creates and returns an identical copy of the current permission.
		/// </summary>
		/// <returns>A copy of the current permission.</returns>
		public IPermission Copy()
		{
			return new PolicyPermission(this.isUnrestricted ? PermissionState.Unrestricted : PermissionState.None, this.policyId, this.principal);
		}

		/// <summary>
		/// Throws a <see cref="T:System.Security.SecurityException" /> at run time if the security requirement is not met.
		/// </summary>
		/// <exception cref="System.UnauthorizedAccessException">
		/// </exception>
		public void Demand()
		{
			// PDP is different here, it is local off the claims identity
			if (this.principal?.Identity?.IsAuthenticated == false || this.principal == null)
				throw new UnauthorizedAccessException($"Policy {this.policyId} was denied");

			PolicyGrantType action = PolicyGrantType.Deny;
			var claimsPrincipal = this.principal as ClaimsPrincipal;
			if (claimsPrincipal == null) // No way to verify
				action = PolicyGrantType.Deny;
			else if (claimsPrincipal is ClaimsPrincipal && claimsPrincipal.HasClaim(c => c.Type == Constants.OpenIzGrantedPolicyClaim && c.Value == this.policyId || this.policyId.StartsWith(String.Format("{0}.", c.Value))))
				action = PolicyGrantType.Grant;

			this.traceSource.TraceInfo("Policy Enforce: {0}({1}) = {2}", this.principal?.Identity?.Name, this.policyId, action);

			if (action != PolicyGrantType.Grant)
				throw new UnauthorizedAccessException($"Policy {this.policyId} was denied with reason : {action}");
		}

		/// <summary>
		/// Parses a policy from a security element instance.
		/// </summary>
		/// <param name="elem">The elem.</param>
		/// <exception cref="System.InvalidOperationException">Cannot find principal
		/// or
		/// Must have policyid</exception>
		public void FromXml(SecurityElement elem)
		{
			var element = elem.Attribute("Unrestricted");

			if (element != null)
				this.isUnrestricted = Convert.ToBoolean(element);

			element = elem.Attribute("PolicyId");

			if (element != null)
				this.policyId = element;

			element = elem.Attribute("principal");

			if (element != null)
				throw new InvalidOperationException("Cannot find principal");

			throw new InvalidOperationException("Must have policyid");
		}

		/// <summary>
		/// Creates and returns a permission that is the intersection of the current permission and the specified permission.
		/// </summary>
		/// <param name="target">A permission to intersect with the current permission. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the intersection of the current permission and the specified permission. This new permission is null if the intersection is empty.</returns>
		public IPermission Intersect(IPermission target)
		{
			if (target == null)
				return null;
			if ((target as IUnrestrictedPermission)?.IsUnrestricted() == false)
				return target;
			else
				return this.Copy();
		}

		/// <summary>
		/// If the two operations allow the exact set of operations.
		/// </summary>
		/// <param name="target">A permission that is to be tested for the subset relationship. This permission must be of the same type as the current permission.</param>
		/// <returns>true if the current permission is a subset of the specified permission; otherwise, false.</returns>
		public bool IsSubsetOf(IPermission target)
		{
			if (target == null)
				return !this.isUnrestricted;
			else
			{
				var permission = target as PolicyPermission;
				return permission.isUnrestricted == this.isUnrestricted &&
				this.policyId.StartsWith(permission.policyId);
			}
		}

		/// <summary>
		/// Returns true if the permission is unrestricted.
		/// </summary>
		/// <returns>true if unrestricted use of the resource protected by the permission is allowed; otherwise, false.</returns>
		public bool IsUnrestricted()
		{
			return this.isUnrestricted;
		}

		/// <summary>
		/// Represent the element as XML
		/// </summary>
		public SecurityElement ToXml()
		{
			SecurityElement element = new SecurityElement("IPermission");
			Type type = this.GetType();
			StringBuilder AssemblyName = new StringBuilder(type.Assembly.ToString());
			AssemblyName.Replace('\"', '\'');
			element.AddAttribute("class", type.FullName + ", " + AssemblyName);
			element.AddAttribute("version", "1");
			element.AddAttribute("Unrestricted", this.isUnrestricted.ToString());
			element.AddAttribute("Policy", this.policyId);
			element.AddAttribute("Principal", this.principal.Identity.Name);
			return element;
		}

		/// <summary>
		/// Creates a permission that is the union of the current permission and the specified permission.
		/// </summary>
		/// <param name="target">A permission to combine with the current permission. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the union of the current permission and the specified permission.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IPermission Union(IPermission target)
		{
			throw new NotImplementedException();
		}
	}
}