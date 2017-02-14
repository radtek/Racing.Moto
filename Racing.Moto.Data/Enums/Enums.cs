using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Enums
{
    public enum LoginStatus
    {
        // Summary:
        //     The user was successfully created.
        Success = 0,
        //
        // Summary:
        //     The user name was not found in the database.
        InvalidUserName = 1,
        //
        // Summary:
        //     The password is not formatted correctly.
        InvalidPassword = 2,
        //
        // Summary:
        //     The user was locked.
        IsLocked = 3,
        //
        // Summary:
        //     The user is disabled.
        Disabled = 4
    }

    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserType
    {
        All = 0,
        Admin = 1,
        GeneralAgent = 2,
        Agent = 3,
        Member = 4,
        Vistor = 5
    }

    /// <summary>
    /// 批量修改类型
    /// </summary>
    public enum BatchRateType
    {
        Rank = 1,
        BigSmall = 2,
        OddEven = 3,
        All = 4
    }
}
