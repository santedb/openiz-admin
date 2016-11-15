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
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZAdmin.Models.AlertModels.ViewModels;
using OpenIZAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZAdmin.Tests.Util
{
	/// <summary>
	/// Contains tests for the <see cref="AlertUtil"/> class.
	/// </summary>
	[TestClass]
	public class AlertUtilTests
	{
		/// <summary>
		/// The internal reference to the <see cref="AlertMessageInfo"/> instance.
		/// </summary>
		private AlertMessageInfo alertMessageInfo;

		/// <summary>
		/// The internal reference to the <see cref="AlertViewModel"/> instance.
		/// </summary>
		private AlertViewModel alertViewModel;

		/// <summary>
		/// Runs cleanup after each test execution.
		/// </summary>
		[TestCleanup]
		public void Cleanup()
		{
			this.alertMessageInfo = null;
			this.alertViewModel = null;
		}

		/// <summary>
		/// Runs initialization before each test execution.
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
			this.alertMessageInfo = new AlertMessageInfo(new OpenIZ.Core.Alert.Alerting.AlertMessage
			{
				Body = "this is some body text of an alert message",
				CreatedBy = new OpenIZ.Core.Model.Security.SecurityUser
				{
					Key = Guid.Parse("3B4A85D4-9D4E-4D4F-A027-32B69648A6A9")
				},
				CreationTime = new DateTimeOffset(1970, 01, 01, 00, 00, 00, TimeSpan.FromHours(5)),
				Flags = OpenIZ.Core.Alert.Alerting.AlertMessageFlags.Alert,
				From = "_UNIT_TEST_SYSTEM",
				Key = Guid.Parse("B20878A9-1DF6-4687-81AA-B8AF7B695423"),
				RcptTo = new List<OpenIZ.Core.Model.Security.SecurityUser>
				{
					new OpenIZ.Core.Model.Security.SecurityUser
					{
						Key = Guid.Parse("E3C8D2CA-2646-4075-B054-EF36631CF24D"),
						UserName = "administrator"
					},
					new OpenIZ.Core.Model.Security.SecurityUser
					{
						Key = Guid.Parse("A5E510E6-6484-48BF-898C-1902C6C3919A"),
						UserName = "lucy"
					}
				},
				Subject = "UNIT TEST ALERT",
				TimeStamp = new DateTimeOffset(new DateTime(1970, 01, 01)),
				To = "administrator"
			});

			this.alertViewModel = new AlertViewModel
			{
				Body = "body",
				CreatedBy = Guid.Parse("BCE8DC56-BCC5-4CBF-8649-C24F14606BF7"),
				Flags = Models.AlertModels.AlertMessageFlags.Acknowledged,
				From = "from",
				Id = Guid.Parse("12186EC5-C5A7-4D6E-94B5-0F77C152790A"),
				Subject = "subject",
				Time = new DateTime(1970, 01, 01),
				To = "to"
			};
		}

		/// <summary>
		/// Tests the mapping of an alert view model to an alert message info.
		/// </summary>
		[TestMethod]
		public void TestToAlertMessageInfo()
		{
			var actual = AlertUtil.ToAlertMessageInfo(this.alertViewModel);

			Assert.AreEqual("body", actual.AlertMessage.Body);
		}

		/// <summary>
		/// Tests the mapping of an alert message info to an alert view model.
		/// </summary>
		[TestMethod]
		public void TestToAlertViewModel()
		{
			var actual = AlertUtil.ToAlertViewModel(this.alertMessageInfo);

			Assert.AreEqual("this is some body text of an alert message", actual.Body);
			Assert.AreEqual(Guid.Parse("3B4A85D4-9D4E-4D4F-A027-32B69648A6A9"), actual.CreatedBy);
			Assert.AreEqual(Models.AlertModels.AlertMessageFlags.Alert, actual.Flags);
			Assert.AreEqual("_UNIT_TEST_SYSTEM", actual.From);
			Assert.AreEqual(Guid.Parse("B20878A9-1DF6-4687-81AA-B8AF7B695423"), actual.Id);
			Assert.AreEqual("UNIT TEST ALERT", actual.Subject);
			Assert.AreEqual(new DateTime(1970, 01, 01), actual.Time);
			Assert.AreEqual("administrator", actual.To);
		}

		/// <summary>
		/// Tests the mapping of an alert message info to an alert view model.
		/// </summary>
		[TestMethod]
		public void TestToAlertViewModelEmptyBody()
		{
			this.alertMessageInfo.AlertMessage.Body = string.Empty;

			var actual = AlertUtil.ToAlertViewModel(this.alertMessageInfo);

			Assert.AreEqual(string.Empty, actual.Body);
			Assert.AreEqual(Guid.Parse("3B4A85D4-9D4E-4D4F-A027-32B69648A6A9"), actual.CreatedBy);
			Assert.AreEqual(Models.AlertModels.AlertMessageFlags.Alert, actual.Flags);
			Assert.AreEqual("_UNIT_TEST_SYSTEM", actual.From);
			Assert.AreEqual(Guid.Parse("B20878A9-1DF6-4687-81AA-B8AF7B695423"), actual.Id);
			Assert.AreEqual("UNIT TEST ALERT", actual.Subject);
			Assert.AreEqual(new DateTime(1970, 01, 01), actual.Time);
			Assert.AreEqual("administrator", actual.To);
		}
	}
}
