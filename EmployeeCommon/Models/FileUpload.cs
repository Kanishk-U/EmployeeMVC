using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeCommon.Models
{
    public class FileUpload
    {
        /// <summary>
        /// File Uploaded
        /// </summary>
        public HttpPostedFileBase files { get; set; }
    }
}