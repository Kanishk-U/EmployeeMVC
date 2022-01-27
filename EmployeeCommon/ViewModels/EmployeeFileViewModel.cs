using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EmployeeCommon.Models;
using FileUpload = EmployeeCommon.Models.FileUpload;

namespace EmployeeCommon.ViewModels
{
    public class EmployeeFileViewModel
    {

        /// <summary>
        /// ViewModel Constructor
        /// </summary>
        public EmployeeFileViewModel()
        {
            
                this.Emp = new EmployeeModel();
                this.File = new FileUpload();
            
        }

        #region === [ Object Properties ] ===========================================================
        /// <summary>
        /// Model Employee
        /// </summary>
        public EmployeeModel Emp { get; set; }


        /// <summary>
        /// Model File upload
        /// </summary>
        public FileUpload File { get; set; }
        #endregion
    }
}