/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: Nityan
 * Date: 2016-7-13
 */
using OpenIZAdmin.Models.RealmModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	[Authorize]
	public class RealmController : Controller
	{
		public RealmController()
		{

		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult JoinRealm()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult JoinRealm(JoinRealmModel model)
		{
			return View();
		}

		[HttpGet]
		public ActionResult LeaveRealm()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LeaveRealm(LeaveRealmModel model)
		{
			return View();
		}

		[HttpGet]
		public ActionResult SwitchRealm()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SwitchRealm(SwitchRealmModel model)
		{
			return View();
		}

		[HttpGet]
		public ActionResult UpdateRealmSettings()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateRealmSettings(UpdateRealmModel model)
		{
			return View();
		}
	}
}