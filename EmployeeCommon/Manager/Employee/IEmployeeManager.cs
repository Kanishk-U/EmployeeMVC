using EmployeeCommon.Models;
using EmployeeCommon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.Manager.Employee
{
    public interface IEmployeeManager
    {
        #region === [ Declarations ] ===========================================================
        /// <summary>
        /// For Employee Keyword Search
        /// </summary>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        List<EmployeeModel> EmployeeKeywordSearch(string Keyword);


        /// <summary>
        /// For Employee Sorting
        /// </summary>
        /// <param name="SortColumn"></param>
        /// <param name="IconClass"></param>
        /// <param name="employees"></param>
        /// <returns></returns>
        List<EmployeeModel> EmployeeSorting(string SortColumn, string IconClass, List<EmployeeModel> employees);


        /// <summary>
        /// Employee listing custom sorting
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="employees"></param>
        /// <returns></returns>
        List<EmployeeModel> CustomPaging(int PageNo, List<EmployeeModel> employees);


        /// <summary>
        /// For Employee reporting drowdown 
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        List<EmployeeModel> EmployeeReportingDDL(int? EmployeeId);


        /// <summary>
        /// Returns Employee with specified ID
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        EmployeeModel GetEmployeeDetails(int EmployeeId);


        /// <summary>
        /// For Dropdown selected value
        /// </summary>
        /// <param name="Employee"></param>
        /// <returns></returns>
        EmployeeModel EmployeeDDLselected(EmployeeModel Employee);


        /// <summary>
        /// List of Employees reporting specified ID
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        List<EmployeeModel> EmployeeReporting(int EmployeeId);


        /// <summary>
        /// Checks if email already exists or not
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        bool EmployeeEmailVal(string Email);

        /// <summary>
        /// Add Employee
        /// </summary>
        /// <param name="viewModel"></param>
        void AddEmployee(EmployeeFileViewModel viewModel);

        /// <summary>
        /// Edit Employee
        /// </summary>
        /// <param name="viewModel"></param>
        void EditEmployee(EmployeeFileViewModel viewModel);


        /// <summary>
        /// Delete Employee
        /// </summary>
        /// <param name="Employee"></param>
        void RemoveEmployee(EmployeeModel Employee);

        /// <summary>
        /// Gets Ip of user
        /// </summary>
        /// <returns></returns>
        string GetIp();
        #endregion
        //########################################################################
    }
}
