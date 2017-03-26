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
 * Date: 2017-3-13
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Web;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Messaging.IMSI.Client;
using Quartz;

namespace OpenIZAdmin.Scheduler
{
	/// <summary>
	/// Represents a concept set scheduler job.
	/// </summary>
	public class ConceptSetJob : BaseJob, IJob
	{
		/// <summary>
		/// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
		/// fires that is associated with the <see cref="T:Quartz.IJob" />.
		/// </summary>
		/// <param name="context">The execution context.</param>
		/// <remarks>The implementation may wish to set a  result object on the
		/// JobExecutionContext before this method exits.  The result itself
		/// is meaningless to Quartz, but may be informative to
		/// <see cref="T:Quartz.IJobListener" />s or
		/// <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
		/// execution.</remarks>
		public void Execute(IJobExecutionContext context)
		{
			ThreadPool.QueueUserWorkItem(state =>
			{
				try
				{
					var client = this.GetServiceClient<ImsiServiceClient>(Constants.Imsi);

					var conceptSets = new List<ConceptSet>();

					var offset = 0;
					var totalCount = 1;

					while (offset < totalCount)
					{
						var bundle = client.Query<ConceptSet>(c => c.ObsoletionTime == null, offset, 100, true);

						bundle.Reconstitute();

						conceptSets.AddRange(bundle.Item.OfType<ConceptSet>().Where(c => c.ObsoletionTime == null));

						offset += 100;
						totalCount = bundle.TotalResults;
					}

					foreach (var conceptSet in conceptSets)
					{
						this.MemoryCache.Set(new CacheItem(conceptSet.Key?.ToString(), conceptSet), new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 5, 0), Priority = CacheItemPriority.Default });
					}
				}
				catch (Exception e)
				{
					Trace.TraceError($"Unable to retrieve concept sets: { e }");
				}
			});
		}
	}
}