using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Constants
{
    public class DBConst
    {
        public const string SP_PK_GeneratePK = "[dbo].[SP_PK_GeneratePK]";
        //public const string SP_PK_GeneratePKRate = "[dbo].[SP_PK_GeneratePKRate]";
        public const string User_Reset_Password = "123456";
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
