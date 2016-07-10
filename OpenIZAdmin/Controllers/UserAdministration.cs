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
 * Date: 2016-7-8
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Controllers
{
	public class UserAdministration : Controller
	{
		public UserAdministration()
		{

		}

		[HttpGet]
		public ActionResult CreateRole()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateRole(object model)
		{
			if (ModelState.IsValid)
			{

			}

			TempData["error"] = "Unable to create role";

			return View(model);
		}

		[HttpGet]
		public ActionResult CreateUser()
		{
			return View();
		}

		[HttpPost]
		public ActionResult CreateUser(object model)
		{
			if (ModelState.IsValid)
			{

			}

			TempData["error"] = "Unable to create user";

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteRole(object model)
		{
			if (ModelState.IsValid)
			{

			}

			TempData["error"] = "Unable to delete role";

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteUser(object model)
		{
			if (ModelState.IsValid)
			{

			}

			TempData["error"] = "Unable to delete user";

			return View(model);
		}

		[HttpGet]
		public ActionResult GetRole(string id)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{

			}

			TempData["error"] = "Role not found";

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult GetRoles()
		{
			return View();
		}

		[HttpGet]
		public ActionResult GetUser(string id)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
			{

			}

			TempData["error"] = "User not found";

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult GetUsers()
		{
			return View();
		}

		[HttpGet]
		public ActionResult UpdateUser()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateUser(object model)
		{
			if (ModelState.IsValid)
			{

			}

			TempData["error"] = "Unable to update user";

			return View(model);
		}
	}
}