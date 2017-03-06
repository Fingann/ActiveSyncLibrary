

namespace ADManagement
{
    using System;
    using System.Security;

    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ActiveDirectoryUtil
    {

        public AccountManagementAD AccauntManager = new AccountManagementAD();

        private string ServiceUser = string.Empty;

        private SecureString servicePassword = new SecureString();
        /// <summary>
        /// Initializes a new instance of the ActiveDirectoryUtil class.
        /// </summary>
        public ActiveDirectoryUtil(): this(String.Empty, new SecureString())
        {
        }

        public ActiveDirectoryUtil(string user, SecureString password)
        {
            this.ServiceUser = user;
            this.servicePassword = password;
        }

        public ActiveDirectoryUtil Setup()
        {
            AccauntManager = new AccountManagementAD(this.ServiceUser, this.servicePassword);
            return this;
        }

    }
}