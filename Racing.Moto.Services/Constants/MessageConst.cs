using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services.Constants
{
    public class MessageConst
    {
        public const string System_Error = "系统错误,请稍后重试!";
        public const string PK_IS_NOT_OPEN = "当前期开盘时间已过,请稍后下期开盘!";
        public const string USER_EXIST_USERNAME = "当前用户名已存在";
        public const string USER_EMAIL_USERNAME_NOT_MATTCHING = "用户名与邮箱不匹配";
        public const string USER_INVALID_CODE = "验证码错误";


        public const string USER_BALANCE_IS_NOT_ENOUGH = "余额不足!";

        public const string USER_BALANCE_IS_NOT_ENOUGH_CODE = "01";
    }
}
