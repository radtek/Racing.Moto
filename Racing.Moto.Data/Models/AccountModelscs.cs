using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "请输入{0}")]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(100, ErrorMessage = "{0}长度至少{2}位.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "请输入{0}")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [Display(Name = "验证码")]
        public string Captcha { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        //[Required(ErrorMessage = "请输入{0}")]
        //[RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$", ErrorMessage = "请输入正确的{0}")]
        //[DataType(DataType.EmailAddress)]
        //[Display(Name = "邮箱")]
        //public string Email { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(100, ErrorMessage = "{0}长度至少{2}位.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "两次输入的密码不一致.")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage = "请输入{0}")]
        //[Display(Name = "验证码")]
        //public string Captcha { get; set; }
    }
}
