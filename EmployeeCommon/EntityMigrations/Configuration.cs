namespace EmployeeCommon.EntityMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EmployeeCommon.Models.EmployeesDBEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"EntityMigrations";
            ContextKey = "EmployeeCommon.Models.EmployeesDBEntities";
        }

        protected override void Seed(EmployeeCommon.Models.EmployeesDBEntities context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
