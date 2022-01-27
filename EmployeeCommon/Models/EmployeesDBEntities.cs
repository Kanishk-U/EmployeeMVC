using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.Models
{
    public partial class EmployeesDBEntities : DbContext
    {
        public EmployeesDBEntities()
            : base("name=EmployeesDBEntities")
        {
        }

        

        public virtual DbSet<EmployeeModel> dat_Employee { get; set; }
    }
}
