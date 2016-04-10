using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using BR.EzTravel.Web.Helpers;
using BR.EzTravel.Web.Models;

namespace BR.EzTravel.Web.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        private string _ApplicationName;

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "CustomRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Custom Role Provider");
            }

            base.Initialize(name, config);

            _ApplicationName = GetConfigValue(config["applicationName"],
                          System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
        }

        public override string ApplicationName
        {
            get { return _ApplicationName; }
            set { _ApplicationName = value; }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            roleName = roleName.ToLower();
            usernameToMatch = usernameToMatch.ToLower();
            var userNames = new List<string>();

            if (!string.IsNullOrEmpty(roleName))
            {
                try
                {
                    using (var context = new ExHolidayEntities())
                    {
                        userNames = context.tblmembers
                            .Where(a =>
                                a.Active
                                && a.Roles.ToLower().Contains(roleName)
                                && a.PICEmail.ToLower() == usernameToMatch)
                            .Select(a => a.PICEmail).ToList();
                    }
                }
                catch (Exception ex)
                {
                    ex.Log();
                    throw;
                }
            }

            return userNames.ToArray();
        }

        public override string[] GetAllRoles()
        {
            var roles = new List<string>();

            try
            {
                using (var context = new ExHolidayEntities())
                {
                    roles = context.refroles.Select(a => a.Role).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return roles.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            var roles = new List<string>();

            if (string.IsNullOrEmpty(username))
                return roles.ToArray();

            try
            {
                username = username.ToLower();
                using (var context = new ExHolidayEntities())
                {
                    var access = context.tblmembers.Where(a => a.Active && a.PICEmail.ToLower() == username).SingleOrDefault();
                    if (access == null)
                        return roles.ToArray();

                    roles = access.Roles.Split(',').ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                throw;
            }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return FindUsersInRole(roleName, string.Empty);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var resp = FindUsersInRole(roleName, username);
            return (resp != null && resp.Count() > 0);
        }

        public override bool RoleExists(string roleName)
        {
            bool result = false;
            try
            {
                using (var context = new ExHolidayEntities())
                {
                    result = context.refroles.Any(a => a.Role == roleName);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                return result;
            }

            return result;
        }

        #region Not Implemented
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
