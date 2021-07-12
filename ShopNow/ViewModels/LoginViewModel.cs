using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{

    public class LoginCreateViewModel
    {
        [Required(ErrorMessage = "Phone Number address is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string PhoneNumber { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }


}

