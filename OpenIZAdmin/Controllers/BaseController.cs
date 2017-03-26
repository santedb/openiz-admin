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
 * Date: 2016-11-19
 */

using System.Security.Principal;
using System.Web;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Attributes;
using OpenIZAdmin.Services.Http;
using OpenIZAdmin.Services.Http.Security;
using System.Web.Mvc;
using OpenIZAdmin.DAL;

namespace OpenIZAdmin.Controllers
{
	/// <summary>
	/// Represents a base controller.
	/// </summary>
	[TokenAuthorize]
	public abstract class BaseController : Controller
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseController"/> class.
		/// </summary>
		protected BaseController()
		{
			TempData.Clear();
		}

		/// <summary>
		/// Gets the <see cref="AmiServiceClient"/> instance.
		/// </summary>
		protected AmiServiceClient AmiClient { get; private set; }

		/// <summary>
		/// Gets the <see cref="ImsiServiceClient"/> instance.
		/// </summary>
		protected ImsiServiceClient ImsiClient { get; private set; }

		/// <summary>
		/// Gets or sets the concept client.
		/// </summary>
		/// <value>The concept client.</value>
		protected ConceptClient ConceptClient { get; set; }

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected override void Dispose(bool disposing)
		{
			this.AmiClient?.Dispose();
			this.ConceptClient?.Dispose();
			this.ImsiClient?.Dispose();
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the device service client.
		/// </summary>
		/// <returns>Returns an AMI service client instance or null.</returns>
		protected AmiServiceClient GetDeviceServiceClient()
		{
			var deviceIdentity = ApplicationSignInManager.LoginAsDevice();

			if (deviceIdentity == null)
			{
				return null;
			}

			this.Request.Cookies.Add(new HttpCookie("access_token", deviceIdentity.AccessToken));

			var restClientService = new RestClientService(Constants.Ami)
			{
				Credentials = new AmiCredentials(new GenericPrincipal(deviceIdentity, null), this.Request)
			};

			return new AmiServiceClient(restClientService);
		}

		/// <summary>
		/// Called when the action is executing.
		/// </summary>
		/// <param name="filterContext">The filter context of the action executing.</param>
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var amiRestClient = new RestClientService(Constants.Ami)
			{
				Accept = Constants.ApplicationXml,
				Credentials = new AmiCredentials(this.User, HttpContext.Request)
			};

			this.AmiClient = new AmiServiceClient(amiRestClient);

			var imsiRestClient = new RestClientService(Constants.Imsi)
			{
				Accept = Constants.ApplicationXml,
				Credentials = new ImsCredentials(this.User, HttpContext.Request)
			};

			this.ImsiClient = new ImsiServiceClient(imsiRestClient);
			this.ConceptClient = new ConceptClient(imsiRestClient);

			base.OnActionExecuting(filterContext);
		}
	}
}