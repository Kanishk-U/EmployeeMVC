using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeMVC.Identity
{
    /// <summary>
    /// For Identity database interaction
    /// </summary>
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext():base("DefaultConnection")
        {

        }
    }
}