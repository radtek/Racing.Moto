using Racing.Moto.Game.Data.Entities;
using Racing.Moto.Game.Data.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Membership
{
    public abstract class MembershipProvider : ProviderBase
    {
        public abstract void Config(NameValueCollection settings);

        /// <summary>override this method to implement any function that is not included in this provider</summary>
        public abstract object Extension(int type, object obj);

        public abstract User GetUser(string username, bool includeRoles);

        public abstract User GetUser(int userId, bool includeRoles);

        public abstract List<User> GetAllUsers(bool includeRoles);

        public abstract bool Exsits(string username);

        /// <returns>0: successful, otherwise: failed</returns>
        public abstract LoginStatus SignIn(string username, bool isPersistent = false, int? timeout = null);

        /// <returns>0: successful, otherwise: failed</returns>
        public abstract LoginStatus SignIn(string username, string password, bool isPersistent = false, int? timeout = null);

        public abstract void SignOut();

        /// <returns>
        ///  0: valid
        ///  1: user doesn't exist
        ///  2: invalid password
        ///  3: locked
        ///  4: disabled
        /// </returns>
        public abstract LoginStatus Authenticate(string username, string password);

        /// <returns>0: successful, 1: the user doesn't exsit, 2: wrong password</returns>
        public abstract int ChangePassword(string username, string password, string newPassword);

        /// <returns>0: successful, 1: the user doesn't exsit</returns>
        public abstract int ChangePassword(string username, string newPassword);

        /// <returns>0: successful, otherwise: failed</returns>
        public abstract int CreateUser(User user);

        /// <returns>0: successful, otherwise: failed</returns>
        public abstract int UpdateUser(User user, bool updateRoles);

        /// <returns>0: successful, otherwise: failed</returns>
        public abstract int LockUser(int userId, bool locked);

        /// <returns>0: successful, otherwise: failed</returns>
        public abstract int EnableUser(int userId, bool enabled);

        /// <returns>0: successful, otherwise: failed</returns>
        public abstract int DeleteUser(int userId);

        //public abstract bool IsInRole(string username, params int[] roleIds);
    }
}
