using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AccountModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenIZAdmin.Util
{
    /// <summary>
	/// Provides a utility for managing users.
	/// </summary>
    public static class AccountUtil
    {

        /// <summary>
		/// Converts a user entity to a edit user model.
		/// </summary>
		/// <param name="imsiClient">The Imsi Service Client client.</param>
		/// <param name="amiClient">The Ami service client.</param>
		/// <param name="userEntity">The user entity to convert to a edit user model.</param>
		/// <returns>Returns a edit user model.</returns>
		public static UpdateProfileModel ToUpdateProfileModel(ImsiServiceClient imsiClient, AmiServiceClient amiClient, UserEntity userEntity)
        {
            var securityUserInfo = amiClient.GetUser(userEntity.SecurityUser.Key.Value.ToString());

            var model = new UpdateProfileModel
            {
                FamilyNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList(),
                GivenNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList(),
                //PhoneType = userEntity.SecurityUser.p
                PhoneNumber = userEntity.SecurityUser.PhoneNumber
            };            

            model.FamilyNamesList.AddRange(model.FamilyNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
            model.GivenNamesList.AddRange(model.GivenNames.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));

            //----would like to make this more compact - not happy with this code block - START ------//
            var facilityId = userEntity.Relationships.Where(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation).Select(r => r.Key).FirstOrDefault()?.ToString();

            if (facilityId != null && facilityId.Any())
            {
                var healthFacility = userEntity.Relationships.FirstOrDefault(r => r.RelationshipType.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation);

                if (healthFacility?.TargetEntityKey != null)
                {
                    var place = imsiClient.Get<Place>(healthFacility.TargetEntityKey.Value, null) as Place;
                    string facilityName = string.Join(" ", place.Names.SelectMany(n => n.Component)?.Select(c => c.Value));

                    var facility = new List<FacilitiesModel>();
                    facility.Add(new FacilitiesModel(facilityName, facilityId));

                    model.FacilityList.AddRange(facility.Select(f => new SelectListItem { Text = f.Name, Value = f.Id }));

                    model.Facilities.Add(facilityId.ToString());
                }
            }

            model.Language = userEntity.LanguageCommunication.Select(l => l.Key).FirstOrDefault().GetValueOrDefault(Guid.Empty);
            model.LanguageList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = Locale.English,
            		// TODO: add the GUID for english here
            		Value = Guid.NewGuid().ToString()
                },
                new SelectListItem
                {
                    Text = Locale.Kiswahili,
            		// TODO: add the GUID for Kiswahili here
            		Value = Guid.NewGuid().ToString()
                }
            };

            return model;
        }
    }   
}