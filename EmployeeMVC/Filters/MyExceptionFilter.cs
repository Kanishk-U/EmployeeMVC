using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using EmployeeMVC.Controllers;

namespace EmployeeMVC.Filters
{
    public class MyExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DefaultController));  //Declaring Log4Net
            string Error = "Message:" + filterContext.Exception.Message + ",Type:" + filterContext.Exception.GetType().ToString() + "Source: " + filterContext.Exception.Source;
            logger.Error(Error);
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("~/Error.html");
        }
    }
}