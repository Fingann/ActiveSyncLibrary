using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ADManagement
{
    using System.Collections;
    using System.Security;

    using ADManagement.Model;

    public class AccountManagementAD : IDisposable
    {
        public void Dispose()
        {
            //TODO: create idisposeable
        }

        #region Variables

        private readonly string sDomain = "bkk.no";
        private readonly string sDefaultOU = "CN=Users,DC=bkk,DC=no";
        //private string sDefaultRootOU = "DC=BKK,DC=NO";
        private readonly string sServiceUser = @"ServiceUser";
        private readonly SecureString secureServicePassword = new SecureString();



        #endregion

        #region Constructor

        public AccountManagementAD()
        {
        }

      
        public AccountManagementAD(string User, SecureString Password)
        {
            sServiceUser = User;
            secureServicePassword = Password;
            var test = ExtentionUtil.SecureStringExtenions.ConvertToUnsecureString(Password);

        }
        public AccountManagementAD(string User, SecureString Password, string sDomain, string sDefaultOU)
        {
            sServiceUser = User;
            secureServicePassword = Password;
            this.sDomain = sDomain;
            this.sDefaultOU = sDefaultOU;
        }

       

        #endregion Constructor

        #region Validate Methods

        /// <summary>
        ///     Validates the username and password of a given user
        /// </summary>
        /// <param name="sUserName">The username to validate</param>
        /// <param name="sPassword">The password of the username to validate</param>
        /// <returns>Returns True of user is valid</returns>
        public bool ValidateCredentials(string sUserName, string sPassword)
        {
            var oPrincipalContext = GetPrincipalContext();
            return oPrincipalContext.ValidateCredentials(sUserName, sPassword);
        }

        /// <summary>
        ///     Validates the username and password of a given user
        /// </summary>
        /// <returns>
        ///     Returns True of user is valid
        /// </returns>
        public bool ValidateCredentials()
        {
            var oPrincipalContext = GetPrincipalContext();
            return oPrincipalContext.ValidateCredentials(sServiceUser, ExtentionUtil.SecureStringExtenions.ConvertToUnsecureString(this.secureServicePassword));
        }

        /// <summary>
        ///     Checks if the User Account is Expired
        /// </summary>
        /// <param name="sUserName">The username to check</param>
        /// <returns>Returns true if Expired</returns>
        public bool IsUserExpired(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            if (oUserPrincipal == null) return true;
            if (oUserPrincipal.AccountExpirationDate != null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Checks if user exists on AD
        /// </summary>
        /// <param name="sUserName">The username to check</param>
        /// <returns>Returns true if username Exists</returns>
        public bool IsUserExisiting(string sUserName)
        {
            if (GetUser(sUserName) == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Force disabling of a user account
        /// </summary>
        /// <param name="sUserName">The username to disable</param>
        public bool IsUserAccountEnabled(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            if (oUserPrincipal == null) return false;
            if (oUserPrincipal.Enabled == null || oUserPrincipal.Enabled == false)
            {
                return false;
            }

            return true;
        }


        public string GetProperty(Principal principal, string property)
        {
            var directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            if (directoryEntry.Properties.Contains(property))
                return directoryEntry.Properties[property].Value.ToString();
            return string.Empty;
        }


        /// <summary>
        ///     Checks if user account is locked
        /// </summary>
        /// <param name="sUserName">The username to check</param>
        /// <returns>Returns true of Account is locked</returns>
        public bool IsAccountLocked(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            if (oUserPrincipal == null) return true;
            return oUserPrincipal.IsAccountLockedOut();
        }

        #endregion

        #region Search Methods

        /// <summary>
        ///     Gets a certain user on Active Directory
        /// </summary>
        /// <param name="sUserName">The username to get</param>
        /// <returns>Returns the UserPrincipal Object</returns>
        public UserPrincipalEx GetUser(string sUserName)
        {
            var oPrincipalContext = GetPrincipalContext();

            var oUserPrincipal =
                UserPrincipalEx.FindByIdentity(oPrincipalContext, IdentityType.SamAccountName, sUserName);


            return oUserPrincipal;
        }

        public List<Phone> GetUserPhones(string sSserName)
        {
            var group2 = this.GetUserUnderlyingObject(sSserName);
            var children = group2.Children;
            var activesyncDevices = (children.Find("CN=ExchangeActiveSyncDevices"));
            return MapPhoneValues(activesyncDevices);    
        }

        public List<Phone> MapPhoneValues(DirectoryEntry de)
        {
            var phoneList = new List<Phone>();
            if (de.Children != null)
            {
                foreach (DirectoryEntry child in de.Children)
                {
                    var phone = new Phone();
                    PropertyCollection pc = child.Properties;
                    IDictionaryEnumerator ide = pc.GetEnumerator();
                    ide.Reset();
                    Console.WriteLine(Environment.NewLine);
                

                    while (ide.MoveNext())
                    {
                        PropertyValueCollection pvc = ide.Entry.Value as PropertyValueCollection;

                        phone[ide.Entry.Key.ToString()] = pvc.Value;
                        Console.WriteLine(string.Format("{0}: {1}", ide.Entry.Key.ToString(), pvc.Value));

                    }
                    phoneList.Add(phone);
                }
            }
            return phoneList;
        }

        public DirectoryEntry GetUserUnderlyingObject(string sUserName)
        {
            var oPrincipalContext = GetPrincipalContext();

            var oUserPrincipal =
                UserPrincipalEx.FindByIdentity(oPrincipalContext, IdentityType.SamAccountName, sUserName);


            return oUserPrincipal.GetUnderlyingObject() as DirectoryEntry;
        }

        /// <summary>
        ///     Gets a certain group on Active Directory
        /// </summary>
        /// <param name="sGroupName">The group to get</param>
        /// <returns>Returns the GroupPrincipal Object</returns>
        public GroupPrincipal GetGroup(string sGroupName)
        {
            var oPrincipalContext = GetPrincipalContext();

            var oGroupPrincipal =
                GroupPrincipal.FindByIdentity(oPrincipalContext, sGroupName);
            return oGroupPrincipal;
        }

        /// <summary>
        ///     Gets the user account lock time.
        /// </summary>
        /// <param name="sUserName">Name of the s user.</param>
        /// <returns></returns>
        public DateTime GetUserAccountLockTime(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            if (oUserPrincipal == null) return new DateTime();
            oUserPrincipal.UnlockAccount();
            if (oUserPrincipal.AccountLockoutTime != null) return oUserPrincipal.AccountLockoutTime.Value;

            return new DateTime();
        }



        #endregion

        #region User Account Methods

      
        /// <summary>
        ///     Sets the user password
        /// </summary>
        /// <param name="sUserName">The username to set</param>
        /// <param name="sNewPassword">The new password to use</param>
        /// <param name="sMessage">Any output messages</param>
        public void SetUserPassword(string sUserName, string sNewPassword, out string sMessage)
        {
            try
            {
                var oUserPrincipal = GetUser(sUserName);
                oUserPrincipal.SetPassword(sNewPassword);

                sMessage = "";
            }
            catch (Exception ex)
            {
                sMessage = ex.Message;
            }
        }

        /// <summary>
        ///     Enables a disabled user account
        /// </summary>
        /// <param name="sUserName">The username to enable</param>
        public void EnableUserAccount(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.Enabled = true;
            oUserPrincipal.Save();
        }

        /// <summary>
        ///     Force disabling of a user account
        /// </summary>
        /// <param name="sUserName">The username to disable</param>
        public void DisableUserAccount(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);

            oUserPrincipal.Enabled = false;
            oUserPrincipal.Save();
        }

        /// <summary>
        ///     Force expire password of a user
        /// </summary>
        /// <param name="sUserName">The username to expire the password</param>
        public void ExpireUserPassword(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.ExpirePasswordNow();
            oUserPrincipal.Save();
        }

        /// <summary>
        ///     Unlocks a locked user account
        /// </summary>
        /// <param name="sUserName">The username to unlock</param>
        public void UnlockUserAccount(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.UnlockAccount();


            oUserPrincipal.Save();
        }


        /// <summary>
        ///     Creates a new user on Active Directory
        /// </summary>
        /// <param name="sOU">The OU location you want to save your user</param>
        /// <param name="sUserName">The username of the new user</param>
        /// <param name="sPassword">The password of the new user</param>
        /// <param name="sGivenName">The given name of the new user</param>
        /// <param name="sSurname">The surname of the new user</param>
        /// <returns>returns the UserPrincipal object</returns>
        public UserPrincipal CreateNewUser(string sOU,
            string sUserName, string sPassword, string sGivenName, string sSurname)
        {
            if (!IsUserExisiting(sUserName))
            {
                var oPrincipalContext = GetPrincipalContext(sOU);

                var oUserPrincipal = new UserPrincipal
                    (oPrincipalContext, sUserName, sPassword, true /*Enabled or not*/);

                //User Log on Name
                oUserPrincipal.UserPrincipalName = sUserName;
                oUserPrincipal.GivenName = sGivenName;
                oUserPrincipal.Surname = sSurname;
                oUserPrincipal.Save();

                return oUserPrincipal;
            }
            return GetUser(sUserName);
        }

        /// <summary>
        ///     Deletes a user in Active Directory
        /// </summary>
        /// <param name="sUserName">The username you want to delete</param>
        /// <returns>Returns true if successfully deleted</returns>
        public bool DeleteUser(string sUserName)
        {
            try
            {
                var oUserPrincipal = GetUser(sUserName);

                oUserPrincipal.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Group Methods

        /// <summary>
        ///     Creates a new group in Active Directory
        /// </summary>
        /// <param name="sOU">The OU location you want to save your new Group</param>
        /// <param name="sGroupName">The name of the new group</param>
        /// <param name="sDescription">The description of the new group</param>
        /// <param name="oGroupScope">The scope of the new group</param>
        /// <param name="bSecurityGroup">
        ///     True is you want this group
        ///     to be a security group, false if you want this as a distribution group
        /// </param>
        /// <returns>Returns the GroupPrincipal object</returns>
        public GroupPrincipal CreateNewGroup(string sOU, string sGroupName,
            string sDescription, GroupScope oGroupScope, bool bSecurityGroup)
        {
            var oPrincipalContext = GetPrincipalContext(sOU);

            var oGroupPrincipal = new GroupPrincipal(oPrincipalContext, sGroupName);
            oGroupPrincipal.Description = sDescription;
            oGroupPrincipal.GroupScope = oGroupScope;
            oGroupPrincipal.IsSecurityGroup = bSecurityGroup;
            oGroupPrincipal.Save();

            return oGroupPrincipal;
        }

        /// <summary>
        ///     Adds the user for a given group
        /// </summary>
        /// <param name="sUserName">The user you want to add to a group</param>
        /// <param name="sGroupName">The group you want the user to be added in</param>
        /// <returns>Returns true if successful</returns>
        public bool AddUserToGroup(string sUserName, string sGroupName)
        {
            try
            {
                var oUserPrincipal = GetUser(sUserName);
                var oGroupPrincipal = GetGroup(sGroupName);
                if (oUserPrincipal == null || oGroupPrincipal == null)
                {
                    if (!IsUserGroupMember(sUserName, sGroupName))
                    {
                        oGroupPrincipal.Members.Add(oUserPrincipal);
                        oGroupPrincipal.Save();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Removes user from a given group
        /// </summary>
        /// <param name="sUserName">The user you want to remove from a group</param>
        /// <param name="sGroupName">The group you want the user to be removed from</param>
        /// <returns>Returns true if successful</returns>
        public bool RemoveUserFromGroup(string sUserName, string sGroupName)
        {
            try
            {
                var oUserPrincipal = GetUser(sUserName);
                var oGroupPrincipal = GetGroup(sGroupName);
                if (oUserPrincipal == null || oGroupPrincipal == null)
                {
                    if (IsUserGroupMember(sUserName, sGroupName))
                    {
                        oGroupPrincipal.Members.Remove(oUserPrincipal);
                        oGroupPrincipal.Save();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Checks if user is a member of a given group
        /// </summary>
        /// <param name="sUserName">The user you want to validate</param>
        /// <param name="sGroupName">
        ///     The group you want to check the
        ///     membership of the user
        /// </param>
        /// <returns>Returns true if user is a group member</returns>
        public bool IsUserGroupMember(string sUserName, string sGroupName)
        {
            var oUserPrincipal = GetUser(sUserName);
            var oGroupPrincipal = GetGroup(sGroupName);

            if (oUserPrincipal == null || oGroupPrincipal == null)
            {
                return oGroupPrincipal.Members.Contains(oUserPrincipal);
            }
            return false;
        }


        /// <summary>
        ///     Gets a list of the users group memberships
        /// </summary>
        /// <param name="sUserName">The user you want to get the group memberships</param>
        /// <returns>Returns an arraylist of group memberships</returns>
        public List<string> GetUserGroups(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);
            PrincipalSearchResult<Principal> oPrincipalSearchResult;
            try
            {
                oPrincipalSearchResult = oUserPrincipal.GetGroups();
            }
            catch (Exception ex)
            {
                
                return null;
            }


            return oPrincipalSearchResult.Select(oResult => oResult.Name.ToString()).ToList();
        }

        /// <summary>
        ///     Gets a list of the users authorization groups
        /// </summary>
        /// <param name="sUserName">The user you want to get authorization groups</param>
        /// <returns>Returns an arraylist of group authorization memberships</returns>
        public List<string> GetUserAuthorizationGroups(string sUserName)
        {
            var oUserPrincipal = GetUser(sUserName);

            var oPrincipalSearchResult =
                oUserPrincipal.GetAuthorizationGroups();

            return oPrincipalSearchResult.Select(oResult => oResult.Name).ToList();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Gets the base principal context
        /// </summary>
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext()
        {
            var oPrincipalContext = new PrincipalContext
                (ContextType.Domain, sDomain, sDefaultOU, ContextOptions.SimpleBind,
                    sServiceUser, ExtentionUtil.SecureStringExtenions.ConvertToUnsecureString(this.secureServicePassword));
            return oPrincipalContext;
        }

        /// <summary>
        ///     Gets the principal context on specified OU
        /// </summary>
        /// <param name="sOU">The OU you want your Principal Context to run on</param>
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext(string sOU)
        {
            var oPrincipalContext =
                new PrincipalContext(ContextType.Domain, sDomain, sOU,
                    ContextOptions.SimpleBind, sServiceUser, ExtentionUtil.SecureStringExtenions.ConvertToUnsecureString(this.secureServicePassword));
            return oPrincipalContext;
        }

        #endregion


        #region Extention 
        public static T GetValue<T>(System.DirectoryServices.PropertyCollection properties, string name, T defaultValue = default(T))
        {
            var value = properties[name];

            if (value == null)
                return defaultValue;

            return (T)value.Value;

            // if you have some cast problems, you could use this:
            return (T)Convert.ChangeType(value, typeof(T));
        }
        #endregion Extention


    }
}