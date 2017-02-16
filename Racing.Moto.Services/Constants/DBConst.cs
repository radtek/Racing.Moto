using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services.Constants
{
    public class DBConst
    {
        public const string SP_PK_GeneratePK = "[dbo].[SP_PK_GeneratePK]";
    }

    /// <summary>
    /// 角色
    /// </summary>
    public class RoleConst
    {
        // 角色ID
        public const int Role_Id_Admin = 1;
        public const int Role_Id_General_Agent = 2;
        public const int Role_Id_Agent = 3;
        public const int Role_Id_Member = 4;

        // 角色名称
        public const string Role_Name_Admin = "管理员";
        public const string Role_Name_General_Agent = "总代理";
        public const string Role_Name_Agent = "代理";
        public const string Role_Name_Member = "会员";
    }

    /// <summary>
    /// 对用户的操作
    /// </summary>
    public enum UserOperation
    {
        Add = 1,
        Edit = 2,
        Delete = 3
    }

}
