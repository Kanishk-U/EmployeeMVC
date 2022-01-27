using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.ViewModels
{
    public class ForgotPasswordViewModel
    {

        /// <summary>
        /// Email for forgot password
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
