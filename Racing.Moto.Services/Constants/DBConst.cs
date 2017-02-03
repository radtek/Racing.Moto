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

        // 角色名称
        public const string Role_Name_Admin = "管理员";
        public const string Role_Name_General_Agent = "总代理";
        public const string Role_Name_Agent = "代理";
        public const string Role_Name_User = "会员";
    }
}
