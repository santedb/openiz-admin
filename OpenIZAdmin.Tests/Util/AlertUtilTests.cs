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
 * User: khannan
 * Date: 2016-11-14
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZAdmin.Tests.Util
{
	[TestClass]
	public class AlertUtilTests
	{

		[TestMethod]
		public void TestAlertMap()
		{
			var alertId = Guid.Parse("12186EC5-C5A7-4D6E-94B5-0F77C152790A");
			var alertCreatedBy = Guid.Parse("BCE8DC56-BCC5-4CBF-8649-C24F14606BF7");

			var actual = AlertUtil.ToAlertMessageInfo(new Models.AlertModels.ViewModels.AlertViewModel
			{
				Body = "body",
				CreatedBy = alertCreatedBy,
				Flags = Models.AlertModels.AlertMessageFlags.Acknowledged,
				From = "from",
				Id = alertId,
				Subject = "subject",
				Time = new DateTime(1970, 01, 01),
				To = "to"
			});

			Assert.AreEqual("body", actual.AlertMessage.Body);
		}
	}
}
