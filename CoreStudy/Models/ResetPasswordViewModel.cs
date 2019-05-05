using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Check new password")]
        public string ConfirmNewPassword { get; set; }
    }
}
