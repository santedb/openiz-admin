using Microsoft.AspNet.Identity.Owin;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Messaging.IMSI.Client;
using OpenIZAdmin.Localization;
using OpenIZAdmin.Models;
using OpenIZAdmin.Models.AccountModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                Surname = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Family).Select(c => c.Value).ToList(),
                GivenNames = userEntity.Names.Where(n => n.NameUse.Key == NameUseKeys.OfficialRecord).SelectMany(n => n.Component).Where(c => c.ComponentTypeKey == NameComponentKeys.Given).Select(c => c.Value).ToList(),
                PhoneNumber = userEntity.SecurityUser.PhoneNumber,
                Email = userEntity.SecurityUser.Email
            };            

            model.SurnamesList.AddRange(model.Surname.Select(f => new SelectListItem { Text = f, Value = f, Selected = true }));
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

            //model.Language = userEntity.LanguageCommunication.Select(l => l.Key).FirstOrDefault().GetValueOrDefault(Guid.Empty);
            model.Language = userEntity.LanguageCommunication.Select(l => l.LanguageCode).FirstOrDefault();
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
            		Value = Locale.EN
                },
                new SelectListItem
                {
                    Text = Locale.Kiswahili,            		
            		Value = Locale.SW
                }
            };
                        
            var bundle = imsiClient.Query<ConceptSet>(c => c.Mnemonic == "TelecomAddressType");
            var telecomList = bundle.Item.OfType<ConceptSet>().ToList().FirstOrDefault();            

            model.PhoneTypeList.Add(new SelectListItem { Text = "", Value = "" });
            foreach (Concept con in telecomList.Concepts)
            {
                    
                model.PhoneTypeList.Add(new SelectListItem { Text = con.Type.ToString(), Value = con.Key.ToString() });
            }                                            

            return model;
        }

        /// <summary>
        /// Converts a <see cref="UpdateProfileModel"/> instance to a <see cref="SecurityUserInfo"/> instance.
        /// </summary>
        /// <param name="model">The create user object to convert.</param>
        /// <param name="user">The user entity instance.</param>        
        /// <param name="securityUserInfo">The user security info instance.</param>        
        /// <returns>Returns a security user info model.</returns>
        public static SecurityUserInfo ToSecurityUserInfo(UpdateProfileModel model, UserEntity user, SecurityUserInfo securityUserInfo, AmiServiceClient client)
        {
            var userInfo = new SecurityUserInfo
            {
                Email = model.Email,
                Roles = new List<SecurityRoleInfo>(),
                User = user.SecurityUser,
                UserId = user.Key,
                Password = securityUserInfo.Password,
                UserName = securityUserInfo.UserName
            };

            userInfo.User.Email = model.Email;
            userInfo.User.PhoneNumber = model.PhoneNumber;            

            //get any roles assigned to the user and add for the update
            if (securityUserInfo.Roles.Any())
            {               
                foreach (var role in securityUserInfo.Roles)
                {
                    userInfo.Roles.Add(role);
                }               
            }
            
            return userInfo;                      
        }

    }    
}