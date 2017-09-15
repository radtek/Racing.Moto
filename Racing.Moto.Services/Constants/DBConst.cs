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
        public const string SP_PK_GeneratePKRate = "[dbo].[SP_PK_GeneratePKRate]";
        public const string User_Reset_Password = "123456";
        public const string Racing_Moto_S_Key = "Racing_Moto_S_Key";
        public const string RacingMoto_Run_Allowable = "RacingMoto_Run_Allowable";
    }

    /// <summary>
    /// 角色
    /// </summary>
    public class RoleConst
    {
        public const int Role_Id_All = 0;
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

        public static string GetRoleName(int roleId)
        {
            var roleName = "";
            switch (roleId)
            {
                case Role_Id_Admin: roleName = Role_Name_Admin; break;
                case Role_Id_General_Agent: roleName = Role_Name_General_Agent; break;
                case Role_Id_Agent: roleName = Role_Name_Agent; break;
                case Role_Id_Member: roleName = Role_Name_Member; break;
            }
            return roleName;
        }
    }

    /// <summary>
    /// 用户类型
    /// RoleId	RoleName
    /// 1	管理员
    /// 2	总代理
    /// 3	代理
    /// 4	会员
    /// </summary>
    public class UserType
    {
        public const int All = 0;
        public const int Admin = RoleConst.Role_Id_Admin;
        public const int GeneralAgent = RoleConst.Role_Id_General_Agent;
        public const int Agent = RoleConst.Role_Id_Agent;
        public const int Member = RoleConst.Role_Id_Member;
        //public const int Vistor = 5;
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
