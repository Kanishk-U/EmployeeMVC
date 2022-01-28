using System.Web;
using System.Web.Mvc;
using EmployeeCommon.ViewModels;
using EmployeeMVC.Identity;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Mail;
using System.Net;
using System.Net.Http;

namespace EmployeeMVC.Controllers
{
    public class AccountController : Controller
    {

        #region User acount Register Login Logout
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        #region private
        private string cred = "**************";
        #endregion


        #region === [ Register user ] ===========================================================
        /// <summary>
        /// Register employee post method
        /// </summary>
        /// <param name="rvm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegisterViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                var appDbContext = new ApplicationDBContext();
                var userStore = new ApplicationUserStore(appDbContext);
                var userManager = new ApplicationUserManager(userStore);
                var passwordHash = Crypto.HashPassword(rvm.Password);
                var user = new ApplicationUser() { Email = rvm.Email, UserName = rvm.Username, PhoneNumber = rvm.Mobile, PasswordHash = passwordHash };
                Microsoft.AspNet.Identity.IdentityResult result =  userManager.Create(user);

                if (result.Succeeded)
                {
                    //role
                    userManager.AddToRole(user.Id, "Customer");

                    //login
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new AuthenticationProperties(), userIdentity);
                }

                return RedirectToAction("Index", "Default");
            }
            else
            {
                ModelState.AddModelError("My Error", "Invalid Data");
                return View();
            }

            
        }
        #endregion
        //########################################################################


        #region === [ Login logout user ] ===========================================================
        /// <summary>
        /// Login get method
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login post method
        /// </summary>
        /// <param name="lvm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginViewModel lvm)
        {
            var appDbContext = new ApplicationDBContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            var user = userManager.Find(lvm.Username, lvm.Password);
            if (user != null)
            {
                //login
                var authenticationManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties(), userIdentity);
                
                return RedirectToAction("Index", "Default");
                
            }
            else
            {
                ModelState.AddModelError("myerror", "Invalid User or Password");

                return View();
            }
            
        }

        
        public PartialViewResult LoginValidation(string Username = "", string Password = "")
        {
            
            LoginViewModel lvm = new LoginViewModel();
            lvm.Username = Username;
            lvm.Password = Password;
            var appDbContext = new ApplicationDBContext();
            var userStore = new ApplicationUserStore(appDbContext);
            var userManager = new ApplicationUserManager(userStore);
            var user = userManager.Find(lvm.Username, lvm.Password);
            if (user != null)
            {
                //login
                var authenticationManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties(), userIdentity);
                
                return PartialView("_login",lvm);

            }
            else if (string.IsNullOrEmpty(lvm.Username) && string.IsNullOrEmpty(lvm.Username))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                ModelState.AddModelError("NameRequired", "User Name Field is required");
                ModelState.AddModelError("PassRequired", "Password Field is required");
                return PartialView("_login", lvm);
            }
            else if(string.IsNullOrEmpty(lvm.Username))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                ModelState.AddModelError("NameRequired","User Name Field is required");
                
                return PartialView("_login", lvm);
            }
            else if (string.IsNullOrEmpty(lvm.Password))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                ModelState.AddModelError("PassRequired", "Password Field is required");
                
                return PartialView("_login", lvm);
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                ModelState.AddModelError("myerror", "Invalid User or Password");
                              
                return PartialView("_login", lvm);
            }
        }


        /// <summary>
        /// User logout
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
        #endregion
        //########################################################################


        #region === [ Forgot/Reset passwrd ] ===========================================================
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appDbContext = new ApplicationDBContext();
                var userStore = new ApplicationUserStore(appDbContext);
                var userManager = new ApplicationUserManager(userStore);
                var provider = new DpapiDataProtectionProvider("EmployeeApp");
                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("Confirm Email"));
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);
                // If the user is found AND Email is confirmed
                if (user != null)
                {
                    // Generate the reset password token
                    var token = await userManager.GeneratePasswordResetTokenAsync(user.Id);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Url.Scheme);

                    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AccountController));  //Declaring Log4Net
                    // Log the password reset link
                    logger.Warn(passwordResetLink);
                    SendVerificationLinkEmail(model.Email, passwordResetLink);
                    // Send the user to Forgot Password Confirmation view
                    return View("ForgotPasswordConfirmation");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var appDbContext = new ApplicationDBContext();
                var userStore = new ApplicationUserStore(appDbContext);
                var userManager = new ApplicationUserManager(userStore);
                var user = await userManager.FindByEmailAsync(model.Email);
                var provider = new DpapiDataProtectionProvider("EmployeeApp");
                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("Confirm Email"));
                if (user != null)
                {
                    // reset the user password
                    var result = await userManager.ResetPasswordAsync(user.Id, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }

        /// <summary>
        /// Method for creation of link for verification of email by sender
        /// </summary>
        /// <param name="emailID"></param>
        /// <param name="activationCode"></param>
        /// <param name="emailFor"></param>
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string link)
        {

            
           
            var fromEmail = new MailAddress("kanishk.gupta@unthinkable.co", "Kanishk");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = cred;

            string subject = "";
            string body = "";
            
            subject = "Reset Password";
            body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";





            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail.Address);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

        }
        #endregion
        //########################################################################


        #endregion
        //########################################################################
    }
}