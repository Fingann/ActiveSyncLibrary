using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADManagement
{
    using System.DirectoryServices.AccountManagement;

    [DirectoryObjectClass("group")]
    [DirectoryRdnPrefix("CN")]

    public class GroupPrincipalsEx : GroupPrincipal
    {
        public GroupPrincipalsEx(PrincipalContext context)
            : base(context)
        {
        }

        public GroupPrincipalsEx(PrincipalContext context, string samAccountName)
            : base(context, samAccountName)
        {
        }

        [DirectoryProperty("wWWHomePage")]
        public string wWWHomePage
        {
            get
            {
                if (ExtensionGet("wWWHomePage").Length != 1) return null;

                return (string)ExtensionGet("wWWHomePage")[0];

            }
            set
            {
                this.ExtensionSet("wWWHomePage", value);
            }
        }
    }
}