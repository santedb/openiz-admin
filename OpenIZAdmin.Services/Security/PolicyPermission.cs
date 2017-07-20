using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Security;
using OpenIZAdmin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZAdmin.Services.Security
{
    /// <summary>
	/// A policy permission
	/// </summary>
	[Serializable]
    public class PolicyPermission : IPermission, IUnrestrictedPermission
    {

        // True if unrestricted
        private bool m_isUnrestricted;
        private String m_policyId;
        private IPrincipal m_principal;

        // Security
        private Tracer m_traceSource = Tracer.GetTracer(typeof(PolicyPermission));

        /// <summary>
        /// Policy permission
        /// </summary>
        public PolicyPermission(PermissionState state, String policyId, IPrincipal principal)
        {
            this.m_isUnrestricted = state == PermissionState.Unrestricted;
            this.m_policyId = policyId;
            this.m_principal = principal;
        }


        /// <summary>
        /// Copy the permission
        /// </summary>
        public IPermission Copy()
        {
            return new PolicyPermission(this.m_isUnrestricted ? PermissionState.Unrestricted : PermissionState.None, this.m_policyId, this.m_principal);
        }

        /// <summary>
        /// Demand the permission
        /// </summary>
        public void Demand()
        {
            // PDP is different here, it is local off the claims identity
            if (this.m_principal?.Identity?.IsAuthenticated == false || this.m_principal == null)
                throw new UnauthorizedAccessException($"Policy {this.m_policyId} was denied");

            PolicyGrantType action = PolicyGrantType.Deny;
            var claimsPrincipal = this.m_principal as ClaimsPrincipal;
            if (claimsPrincipal == null) // No way to verify 
                action = PolicyGrantType.Deny;
            else if (claimsPrincipal is ClaimsPrincipal && claimsPrincipal.HasClaim(c => c.Type == Constants.OpenIzGrantedPolicyClaim && c.Value == this.m_policyId || this.m_policyId.StartsWith(String.Format("{0}.", c.Value))))
                action = PolicyGrantType.Grant;

            this.m_traceSource.TraceInfo("Policy Enforce: {0}({1}) = {2}", this.m_principal?.Identity?.Name, this.m_policyId, action);

            if (action != PolicyGrantType.Grant)
                throw new UnauthorizedAccessException($"Policy {this.m_policyId} was denied with reason : {action}");
        }

        /// <summary>
        /// Tries to demand <paramref name="policyId"/>
        /// </summary>
        public static bool TryDemand(IPrincipal principal, String policyId)
        {
            try
            {
                new PolicyPermission(PermissionState.Unrestricted, policyId, principal);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// From XML
        /// </summary>
        public void FromXml(SecurityElement elem)
        {
            string element = elem.Attribute("Unrestricted");
            if (element != null)
                this.m_isUnrestricted = Convert.ToBoolean(element);
            element = elem.Attribute("PolicyId");
            if (element != null)
                this.m_policyId = element;
            element = elem.Attribute("principal");
            if (element != null)
                throw new InvalidOperationException("Cannot find principal");
            else
                throw new InvalidOperationException("Must have policyid");
        }

        /// <summary>
        /// Intersect the permission
        /// </summary>
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
        /// If the two operations allow the exact set of operations
        /// </summary>
        public bool IsSubsetOf(IPermission target)
        {
            if (target == null)
                return !this.m_isUnrestricted;
            else
            {
                var permission = target as PolicyPermission;
                return permission.m_isUnrestricted == this.m_isUnrestricted &&
                this.m_policyId.StartsWith(permission.m_policyId);
            }
        }

        /// <summary>
        /// True if the permission is unrestricted
        /// </summary>
        public bool IsUnrestricted()
        {
            return this.m_isUnrestricted;
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
            element.AddAttribute("Unrestricted", this.m_isUnrestricted.ToString());
            element.AddAttribute("Policy", this.m_policyId);
            element.AddAttribute("Principal", this.m_principal.Identity.Name);
            return element;

        }

        public IPermission Union(IPermission target)
        {
            throw new NotImplementedException();
        }
    }
}
