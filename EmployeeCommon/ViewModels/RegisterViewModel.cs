using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EmployeeCommon.ViewModels
{
    public class RegisterViewModel
    {
        /// <summary>
        /// Username of user
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        [Required]
        public string Password { get; set; }


        /// <summary>
        /// Confirm password user
        /// </summary>
        [Required(ErrorMessage = "The Confirm Password field is required")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }


        /// <summary>
        /// Email of user
        /// </summary>
        [Required]
        [EmailAddress]
        [Remote("IsAlreadyExists", "Default", ErrorMessage = "Email already exists!")]
        public string Email { get; set; }

        /// <summary>
        /// Mobile details of user
        /// </summary>
        [Required]
        public string Mobile { get; set; }

        


    }
}
