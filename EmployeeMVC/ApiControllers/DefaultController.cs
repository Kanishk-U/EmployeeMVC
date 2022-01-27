using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EmployeeCommon.Models;
using EmployeeCommon.Manager;


namespace EmployeeMVC.ApiControllers
{

    public class DefaultController : ApiController
    {

        #region API method calls
        /// <summary>
        /// Gets employee list
        /// </summary>
        /// <returns></returns>
        public List<EmployeeModel> Get()
        {
            EmployeesDBEntities db = new EmployeesDBEntities();
            List<EmployeeModel> employees = db.dat_Employee.ToList();
            return employees;
        }

        public EmployeeModel GetEmployeeById(long EmpId)
        {
            EmployeesDBEntities db = new EmployeesDBEntities();
            EmployeeModel employee = db.dat_Employee.Where(temp => temp.Id == EmpId).FirstOrDefault();
            return employee;
        }

        public void PostEmployee(EmployeeModel newEmp)
        {
            EmployeesDBEntities db = new EmployeesDBEntities();
            db.dat_Employee.Add(newEmp);
            db.SaveChanges();
        }

        public void PutEmployee(EmployeeModel emp)
        {
            EmployeesDBEntities db = new EmployeesDBEntities();
            EmployeeModel employee = db.dat_Employee.Where(temp => temp.Id == emp.Id).FirstOrDefault();
            employee.FirstName = emp.FirstName;
            db.SaveChanges();
        }

        public void DeleteEmployee(long EmpId)
        {
            EmployeesDBEntities db = new EmployeesDBEntities();
            EmployeeModel employee = db.dat_Employee.Where(temp => temp.Id == EmpId).FirstOrDefault();
            db.dat_Employee.Remove(employee);
            db.SaveChanges();
        }
        #endregion
    }
}
