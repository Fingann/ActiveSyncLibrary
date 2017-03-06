using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADManagement
{
    using System.DirectoryServices.AccountManagement;

    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("Person")]
    public class UserPrincipalEx : UserPrincipal
    {
        // Inplement the constructor using the base class constructor. 
        public UserPrincipalEx(PrincipalContext context)
            : base(context)
        {
        }

        // Implement the constructor with initialization parameters.    
        public UserPrincipalEx(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled)
        {
        }

        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityType, identityValue);
        }
       

       // Create the "extensionAttribute2" property.    
       [DirectoryProperty("title")]
        public string Title
        {
            get
            {
                if (ExtensionGet("title").Length != 1) return string.Empty;

                return (string)ExtensionGet("title")[0];
            }
            set
            {
                ExtensionSet("title", value);
            }
        }

        // Create the "extensionAttribute2" property.    
        [DirectoryProperty("vasco-Locked")]
        public bool VascoLocked
        {
            get
            {
                if (ExtensionGet("vasco-Locked").Length != 1) return false;

                return (int)ExtensionGet("vasco-Locked")[0] == 1 ;
            }
            set
            {
                ExtensionSet("vasco-Locked", value);
            }
        }
    }
}