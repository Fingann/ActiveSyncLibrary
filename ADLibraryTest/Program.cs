using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADLibraryTest
{
    using System.Collections;
    using System.DirectoryServices;
    using System.Security;

    using ADManagement;

    class Program
    {
        static void Main(string[] args)
        {
            ActiveDirectoryUtil ss = new ActiveDirectoryUtil("Serviceuser", ExtentionUtil.SecureStringExtenions.ConvertToSecureString("password"));
            ss.Setup();
            var excists = ss.AccauntManager.ValidateCredentials("userToTest", "UserToTestPass");
            Console.WriteLine(excists);

           
            var group2 = ss.AccauntManager.GetUserPhones("userToTest");
         
            Console.ReadKey();

        }

       
    }
}
