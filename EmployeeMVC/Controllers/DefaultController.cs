using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EmployeeCommon.Models;
using EmployeeCommon.ViewModels;
using EmployeeCommon.Manager.Employee;
using FileUpload = EmployeeCommon.Models.FileUpload;
using EmployeeMVC.Filters;
using System.Configuration;

namespace EmployeeMVC.Controllers
{
    [MyExceptionFilter]
    public class DefaultController : Controller
    {
        #region === [ Default Parameters ] ===========================================================
        private IEmployeeManager employeeManager;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DefaultController));  //Declaring Log4Net
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DefaultController()
        {

        }

        /// <summary>
        /// Constructor with dependency
        /// </summary>
        /// <param name="_employeeManager"></param>
        public DefaultController(IEmployeeManager employeeManager)
        {
            this.employeeManager = employeeManager;
        }
        #endregion
        //########################################################################


        #region === [Employee listing ] ===========================================================

        // GET: Default

        /// <summary>
        /// Index listing page of employees
        /// </summary>
        /// <param name="txtsearch"></param>
        /// <param name="SortColumn"></param>
        /// <param name="IconClass"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        [MyAuthenticationFilter]
        public ActionResult Index(string txtsearch = "", string SortColumn = "FirstName", string IconClass = "fa-sort-asc", int PageNo = 1, string Msg = "")
        {

            
            List<EmployeeModel> employees = employeeManager.EmployeeKeywordSearch(txtsearch);

            ViewBag.msg = Msg;


            //Sorting
            employees = employeeManager.EmployeeSorting(SortColumn, IconClass, employees); 
            ViewBag.SortColumn = SortColumn;
            ViewBag.IconClass = IconClass;

            /* Paging */
            employees = CustomPaging(PageNo, employees);

            return View(employees);
        }


        /// <summary>
        /// On Search button click gets employee searched
        /// </summary>
        /// <param name="txtsearch"></param>
        /// <returns></returns>
        public PartialViewResult Search(string txtsearch = "")
        {
            List<EmployeeModel> model = employeeManager.EmployeeKeywordSearch(txtsearch);
            model = CustomPaging(1, model);
            return PartialView("IndexPartial", model);
        }


        /// <summary>
        /// Sorts List and manage paging on clicks
        /// </summary>
        /// <param name="SortColumn"></param>
        /// <param name="IconClass"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public PartialViewResult SortList(string SortColumn = "FirstName", string IconClass = "fa-sort-asc", int PageNo = 1)
        {
            List<EmployeeModel> model = employeeManager.EmployeeKeywordSearch("");

            model = employeeManager.EmployeeSorting(SortColumn, IconClass, model);
            ViewBag.SortColumn = SortColumn;
            ViewBag.IconClass = IconClass;
            model = CustomPaging(PageNo, model);
            return PartialView("IndexPartial", model);
        }





        #endregion
        //########################################################################



        #region === [Custom paging] ===========================================================

        /// <summary>
        /// Custom paging for employees
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="employees"></param>
        /// <returns></returns>
        private List<EmployeeModel> CustomPaging(int PageNo, List<EmployeeModel> employees)
        {
            int NoOfRecordsPerPage = Convert.ToInt32(ConfigurationManager.AppSettings["NoOfPages"]);
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(employees.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.Records = employees.Count();
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            employees = employees.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();
            return employees;
        }
        #endregion
        //########################################################################



        #region === [Add employee] ===========================================================

        /// <summary>
        /// Add employee page
        /// </summary>
        /// <returns></returns>

        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            //For reporting dropdown
            ViewBag.Reporting = employeeManager.EmployeeReportingDDL(null); 
            EmployeeFileViewModel viewModels = new EmployeeFileViewModel();

            return View(viewModels);
        }



        /// <summary>
        /// Add employee Post form
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(EmployeeFileViewModel viewModel)
        {
            var req = Request;
           
            if (ModelState.IsValid)
            {
                string msg = "Employee Added Succesfully";
                employeeManager.AddEmployee(viewModel);               
                return RedirectToAction("Index", "Default", new { Msg = msg });
            }
            else
            {
                return Create();
            }

        }
        #endregion
        //########################################################################



        #region === [Edit employee] ===========================================================

        /// <summary>
        /// Edit Employe page
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int Id)
        {
            try {
                EmployeeFileViewModel viewModel = new EmployeeFileViewModel();

                //Employee Details
                EmployeeModel Employee = employeeManager.GetEmployeeDetails(Id);
                if (Employee is null)
                {

                    return RedirectToAction("Index", "Default", new { Msg = String.Format("Unable to Access Employee with ID {0}", Id) });
                }
                viewModel.Emp = Employee;

                //Reporting dropwdown Bag
                ViewBag.Reporting = employeeManager.EmployeeReportingDDL(Id);

                //Reporting selected employee dropwdown
                ViewBag.ReportingTo = employeeManager.EmployeeDDLselected(Employee);
                //Reporting list of partial view
                ViewBag.ReportingEmployee = employeeManager.EmployeeReporting(Id);
                return View(viewModel);
            }
            catch(Exception ex)
            {
                //logger.Error(ex.ToString());
                return View();
            }
            
        }

        

        /// <summary>
        /// Edit employee post from
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(EmployeeFileViewModel viewModel)
        {
            var req = Request;
            EmployeeModel emp = viewModel.Emp;
            FileUpload fileUpload = viewModel.File;
            
            if (ModelState.IsValid)
            {               
                employeeManager.EditEmployee(viewModel);
                string msg = "Employee Updated Successfully";
                return RedirectToAction("Index", "Default", new { Msg = msg });
            }
            else
            {
                return Edit(emp.Id);
            }

        }
        #endregion
        //########################################################################



        #region === [Delete employee] ===========================================================
        
        /// <summary>
        /// Delete employee with specific id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AdminAuthorization]
        public ActionResult Delete(int Id)
        {
            EmployeeModel Employee = employeeManager.GetEmployeeDetails(Id); 
            employeeManager.RemoveEmployee(Employee);
            return RedirectToAction("Index", "Default", new { Msg = "Employee Deleted Successfully" });
        }
        #endregion
        //########################################################################



        #region === [Remote Email validation] ===========================================================
        /// <summary>
        /// Email validation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public JsonResult IsAlreadyExists(EmployeeModel Emp)
        {
            

            bool status;
            if (employeeManager.EmployeeEmailVal(Emp.Email))
            {
                //Already registered  
                status = false;
            }
            else
            {
                //Available to use  
                status = true;
            }

            return Json(status, JsonRequestBehavior.AllowGet);

        }
        #endregion
        //########################################################################



        #region === [Download File] ===========================================================
        /// <summary>
        /// Downloading file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult DownLoadFile(int id)
        {

            var FileById = employeeManager.GetEmployeeDetails(id); 

            return File(FileById.FileData, FileById.FileMimeType, FileById.FileName);

        }
        #endregion
        //########################################################################


        #region === [API Test] ===========================================================

        public ActionResult TestEmployee()
        {
            return View();
        }
        #endregion
        //########################################################################


        
    }
}