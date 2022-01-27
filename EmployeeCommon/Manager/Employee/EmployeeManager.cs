using EmployeeCommon.Models;
using EmployeeCommon.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FileUpload = EmployeeCommon.Models.FileUpload;

namespace EmployeeCommon.Manager.Employee
{
    public class EmployeeManager : IEmployeeManager
    {
        #region === [ Default Parameters ] ===========================================================
        /// <summary>
        /// Dbcontext instance
        /// </summary>
        EmployeesDBEntities db = new EmployeesDBEntities();


        /// <summary>
        /// Constructor Employee manager
        /// </summary>
        public EmployeeManager()
        {
        }
        #endregion
        //########################################################################


        #region === [ Add Employee ] ===========================================================
        /// <summary>
        /// Add employee on Post request
        /// </summary>
        /// <param name="viewModel"></param>
        public void AddEmployee(EmployeeFileViewModel viewModel)
        {
            EmployeeModel emp = viewModel.Emp;
            FileUpload fileUpload = viewModel.File;
            if (fileUpload.files != null)
            {
                emp.FileName = fileUpload.files.FileName;
                emp.FileMimeType = fileUpload.files.ContentType;
                emp.FileSize = fileUpload.files.ContentLength;
                using (Stream fs = fileUpload.files.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        emp.FileData = br.ReadBytes((Int32)fs.Length);
                    }
                }
            }
            emp.IpCreated = GetIp();
            emp.UserCreated = Environment.UserName;
            emp.MachineCreated = Environment.MachineName;
            emp.DateCreated = DateTime.Now.ToString();
            db.dat_Employee.Add(emp);
            db.SaveChanges();
        }
        #endregion
        //########################################################################


        #region === [ Custom Paging ] ===========================================================
        /// <summary>
        /// Custom Paging on Index Page
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="employees"></param>
        /// <returns></returns>
        public List<EmployeeModel> CustomPaging(int PageNo, List<EmployeeModel> employees)
        {
            int NoOfRecordsPerPage = 5;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(employees.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            //ViewBag.PageNo = PageNo;
            //ViewBag.NoOfPages = NoOfPages;
            employees = employees.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();
            return employees;
        }
        #endregion
        //########################################################################


        #region === [ Edit Employee ] ===========================================================
        /// <summary>
        /// Updation of Employee on Post request
        /// </summary>
        /// <param name="viewModel"></param>
        public void EditEmployee(EmployeeFileViewModel viewModel)
        {
            EmployeeModel emp = viewModel.Emp;
            FileUpload fileUpload = viewModel.File;
            EmployeeModel Employee = db.dat_Employee.Where(temp => temp.Id == emp.Id).FirstOrDefault();
            Employee.FirstName = emp.FirstName;
            Employee.MiddleName = emp.MiddleName;
            Employee.LastName = emp.LastName;
            Employee.Father = emp.Father;
            Employee.Email = emp.Email;
            Employee.DOB = emp.DOB;
            Employee.Program = emp.Program;
            Employee.Region = emp.Region;
            Employee.Address = emp.Address;
            Employee.Address2 = emp.Address2;
            Employee.City = emp.City;
            Employee.State = emp.State;
            Employee.ZIP = emp.ZIP;
            Employee.Contact = emp.Contact;
            Employee.Gender = emp.Gender;
            Employee.Reporting = emp.Reporting;
            Employee.IpModified = GetIp();
            Employee.UserModified = Environment.UserName;
            Employee.MachineModified = Environment.MachineName;
            Employee.DateModified = DateTime.Now.ToString();
            if (fileUpload.files != null)
            {
                Employee.FileName = fileUpload.files.FileName;
                Employee.FileMimeType = fileUpload.files.ContentType;
                Employee.FileSize = fileUpload.files.ContentLength;
                using (Stream fs = fileUpload.files.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        Employee.FileData = br.ReadBytes((Int32)fs.Length);
                    }
                }
            }


            db.SaveChanges();
        }
        #endregion
        //########################################################################


        #region === [ DDL reporting selected value ] ===========================================================
        /// <summary>
        /// Selected value at dropdown 
        /// </summary>
        /// <param name="Employee"></param>
        /// <returns></returns>
        public EmployeeModel EmployeeDDLselected(EmployeeModel Employee)
        {
            return db.dat_Employee.Where(temp => temp.Id == Employee.Reporting).FirstOrDefault();
        }
        #endregion
        //########################################################################


        #region === [ Email Validation ] ===========================================================
        /// <summary>
        /// Email check if alread exists
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public bool EmployeeEmailVal(string Email)
        {
            return db.dat_Employee.Any(a => a.Email.ToLower() == Email.ToLower());
        }
        #endregion
        //########################################################################


        #region === [ KeyWord Search ] ===========================================================

        /// <summary>
        /// Employee Search with Keyword
        /// </summary>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        public List<EmployeeModel> EmployeeKeywordSearch(string Keyword)
        {
            return db.dat_Employee.Where(temp => temp.FirstName.Contains(Keyword)).ToList();
        }
        #endregion
        //########################################################################


        #region === [ Reporting Employee List ] ===========================================================
        /// <summary>
        /// Returns List of reporting Employee
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public List<EmployeeModel> EmployeeReporting(int EmployeeId)
        {
            return db.dat_Employee.Where(temp => temp.Reporting == EmployeeId).ToList();
        }
        #endregion
        //########################################################################


        #region === [ Reporting DDL ] ===========================================================
        /// <summary>
        /// Dropdown List binding
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public List<EmployeeModel> EmployeeReportingDDL(int? EmployeeId)
        {
            if (EmployeeId == null)
            {
                return db.dat_Employee.Where(temp => temp.FirstName != null).ToList();
            }
            else
            {
                return db.dat_Employee.Where(temp => temp.Id != EmployeeId).ToList();
            }
        }
        #endregion
        //########################################################################


        #region === [ Employee Sorting ] ===========================================================
        /// <summary>
        /// Return sorted columns 
        /// </summary>
        /// <param name="SortColumn"></param>
        /// <param name="IconClass"></param>
        /// <param name="employees"></param>
        /// <returns></returns>
        public List<EmployeeModel> EmployeeSorting(string SortColumn, string IconClass, List<EmployeeModel> employees)
        {
            switch (SortColumn)
            {
                case "FirstName":
                    if (IconClass == "fa-sort-asc")
                    { employees = employees.OrderBy(temp => temp.FirstName).ToList(); }
                    else
                    { employees = employees.OrderByDescending(temp => temp.FirstName).ToList(); }
                    break;
                case "Email":
                    if (IconClass == "fa-sort-asc")
                    { employees = employees.OrderBy(temp => temp.Email).ToList(); }
                    else
                    { employees = employees.OrderByDescending(temp => temp.Email).ToList(); }
                    break;
                case "DOB":
                    if (IconClass == "fa-sort-asc")
                    { employees = employees.OrderBy(temp => temp.DOB).ToList(); }
                    else
                    { employees = employees.OrderByDescending(temp => temp.DOB).ToList(); }
                    break;
                case "Contact":
                    if (IconClass == "fa-sort-asc")
                    { employees = employees.OrderBy(temp => temp.Contact).ToList(); }
                    else
                    { employees = employees.OrderByDescending(temp => temp.Contact).ToList(); }
                    break;
                case "Gender":
                    if (IconClass == "fa-sort-asc")
                    { employees = employees.OrderBy(temp => temp.Gender).ToList(); }
                    else
                    { employees = employees.OrderByDescending(temp => temp.Gender).ToList(); }
                    break;
                case "Program":
                    if (IconClass == "fa-sort-asc")
                    { employees = employees.OrderBy(temp => temp.Program).ToList(); }
                    else
                    { employees = employees.OrderByDescending(temp => temp.Program).ToList(); }
                    break;

            }

            return employees;
        }
        #endregion
        //########################################################################


        #region === [ Gets Employee Details ] ===========================================================
        /// <summary>
        /// Returns object for specific ID
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public EmployeeModel GetEmployeeDetails(int EmployeeId)
        {
            
            return db.dat_Employee.Where(temp => temp.Id == EmployeeId).FirstOrDefault();
        }
        #endregion
        //########################################################################


        #region === [Delete Employee ] ===========================================================
        /// <summary>
        /// Delete Employee
        /// </summary>
        /// <param name="Employee"></param>
        public void RemoveEmployee(EmployeeModel Employee)
        {
            db.dat_Employee.Remove(Employee);
            db.SaveChanges();
        }
        #endregion
        //########################################################################

        #region === [Get Ip] ===========================================================
        /// <summary>
        /// Gets Ip of User
        /// </summary>
        /// <returns></returns>
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }
        #endregion
        //########################################################################
    }
}
